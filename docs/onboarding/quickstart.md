# Quickstart

Requires Docker Desktop / Docker Engine with Compose.

```sh
cp .env.example .env
# update Ollama address/model if you have a reachable Ollama server
docker compose up --build
```

Open http://localhost:8088 for the companion, http://localhost:5080/health for the C# API and http://localhost:8001/docs for the Python runtime.

For local frontend work: run `dotnet run --project apps/core-api --urls http://localhost:5080` in one terminal, and `cd apps/web && npm install && npm run dev` in another.

Run Python tests with `cd apps/robot-runtime && pip install -e '.[dev]' && pytest`.

**Known limitation:** task data is temporary, no live GitHub link, no project RAG or physical servo support yet.
