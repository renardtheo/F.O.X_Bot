# Architecture v0.1 (proposed)

React web UI → ASP.NET Core API (project domain, authorization, orchestration) → Python robot runtime (behavior/drivers) → simulator or hardware adapters.

AI Gateway in core communicates with self-hosted Ollama. Project adapters query GitHub/GitLab with read-only scopes initially. Code/doc retrieval is isolated and must cite its sources.

## Contract rules
- Core sends *intent* (e.g. `express.happy`), not servo coordinates.
- Each body declares capabilities; missing capabilities degrade gracefully.
- Hardware commands need bounds, timeout and safe shutdown behavior.
- Use HTTP for commands initially and event-stream/WebSocket for realtime later.
- Capture important decisions in ADRs before adding infrastructure.

## ADR index
- ADR-001: monorepo (accepted).
- ADR-002: fully customizable physical bodies (accepted).
