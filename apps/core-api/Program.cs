using System.Net.Http.Json;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddHttpClient("ollama", client => {
    client.Timeout = TimeSpan.FromSeconds(90);
    client.BaseAddress = new Uri(builder.Configuration["Ollama:BaseUrl"] ?? "http://localhost:11434");
});
var app = builder.Build();
app.UseCors(policy => policy.WithOrigins(builder.Configuration["Web:Origin"] ?? "http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
var store = new FoxStore();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "fox-core" }));
app.MapGet("/api/companion", () => Results.Ok(store.Companion));
app.MapPost("/api/companion/expression", (ExpressionRequest request) => {
    if (!FoxStore.AllowedExpressions.Contains(request.Expression))
        return Results.BadRequest(new { error = "Unknown expression" });
    store.Companion = new CompanionStatus(request.Expression, DateTimeOffset.UtcNow);
    return Results.Ok(store.Companion);
});
app.MapGet("/api/projects", () => Results.Ok(store.Projects));
app.MapGet("/api/tasks", () => Results.Ok(store.Tasks));
app.MapPost("/api/tasks", (CreateTask request) => {
    if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 150)
        return Results.BadRequest(new { error = "Title must be 1-150 characters" });
    var task = new FoxTask(Guid.NewGuid(), request.Title.Trim(), "todo");
    store.Tasks.Add(task);
    return Results.Created($"/api/tasks/{task.Id}", task);
});
app.MapPatch("/api/tasks/{id:guid}", (Guid id, UpdateTask request) => {
    if (!new[] { "todo", "in-progress", "review", "done" }.Contains(request.Status))
        return Results.BadRequest(new { error = "Invalid task status" });
    var index = store.Tasks.FindIndex(x => x.Id == id);
    if (index < 0) return Results.NotFound();
    var existing = store.Tasks[index];
    store.Tasks[index] = existing with { Status = request.Status };
    return Results.Ok(store.Tasks[index]);
});
app.MapPost("/api/ai/chat", async (ChatRequest request, IHttpClientFactory clients, IConfiguration config, CancellationToken cancellationToken) => {
    if (string.IsNullOrWhiteSpace(request.Message) || request.Message.Length > 4000)
        return Results.BadRequest(new { error = "Message must be 1-4000 characters" });
    var model = config["Ollama:Model"] ?? "gemma3:4b";
    try {
        var http = clients.CreateClient("ollama");
        using var response = await http.PostAsJsonAsync("/api/chat", new {
            model, stream = false, messages = new[] {
                new { role = "system", content = "You are FOX, a friendly desk companion and software project assistant. Be concise. Do not claim to know repository content you have not been given." },
                new { role = "user", content = request.Message }
            }
        }, cancellationToken);
        if (!response.IsSuccessStatusCode) return Results.Problem("Ollama unavailable or model missing", statusCode: 502);
        using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var answer = json.RootElement.GetProperty("message").GetProperty("content").GetString();
        return Results.Ok(new { answer, model });
    } catch (HttpRequestException) { return Results.Problem("Cannot connect to Ollama", statusCode: 503); }
      catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested) { return Results.Problem("Ollama request timed out", statusCode: 504); }
});
app.Run();

record ExpressionRequest(string Expression);
record ChatRequest(string Message);
record CreateTask(string Title);
record UpdateTask(string Status);
record FoxTask(Guid Id, string Title, string Status);
record FoxProject(string Id, string Name, string Description);
record CompanionStatus(string Expression, DateTimeOffset UpdatedAt);
sealed class FoxStore {
    public static readonly HashSet<string> AllowedExpressions = new(StringComparer.OrdinalIgnoreCase) { "idle", "happy", "thinking", "curious", "alert", "sleeping" };
    public CompanionStatus Companion { get; set; } = new("idle", DateTimeOffset.UtcNow);
    public List<FoxProject> Projects { get; } = [new("fox", "Project F.O.X.", "Modular open-source desk companion")];
    public List<FoxTask> Tasks { get; } = [new(Guid.NewGuid(), "Initialiser la plateforme", "in-progress"), new(Guid.NewGuid(), "Créer un driver simulé", "todo")];
}
