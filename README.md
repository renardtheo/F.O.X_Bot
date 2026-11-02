# Project F.O.X. 🦊

**Fully customizable open-source-ready desk companion and developer project assistant.**

FOX combines an expressive virtual/physical companion with project management, code exploration and optional self-hosted Ollama-based chat. The body is user-designed: no fixed physical geometry is required.

## What's included

- **ASP.NET Core (.NET 10)**: health, companion expressions, demo projects/tasks, minimal Ollama chat gateway.
- **Python/FastAPI runtime**: simulated driver and capability registry prototype.
- **React/TypeScript web**: expressive companion, local Kanban and chat panel.
- **Docker Compose**, GitHub Actions, issue templates and backlog bootstrap scripts.
- **Contracts, decisions, onboarding and hardware contribution guidance**.

## Quick start

```sh
cp .env.example .env
docker compose up --build
```

- Web UI: **http://localhost:8088**
- API: http://localhost:5080/health
- Robot runtime: http://localhost:8001/docs

If Ollama is not running, the companion and task demo still work; chat will report an error. Configure `OLLAMA_BASE_URL` and `OLLAMA_MODEL` in `.env`. This project does **not** bundle Ollama or download a model.

## Roadmap & current limits

This is a **development-ready vertical slice**, not a completed robot. **No real GitHub project synchronization, repository indexing/RAG, persistent storage, physical drivers or core-to-Python forwarding** yet. Task/companion API state currently resets on restart. See [architecture contracts](docs/architecture/runtime-contracts.md), [onboarding](docs/onboarding/quickstart.md) and [backlog](backlog/issues.json).

## Repository setup

Use `gh auth login`, create an empty private GitHub repository, and run `bash scripts/bootstrap-github.sh OWNER REPO` to import issues and create a board. See [GitHub board](docs/project-management/board.md).

## Licensing

Licensing is **not yet selected**. Do not publicly redistribute under an assumed open-source license. See `LICENSE-DECISION.md` and decide software, hardware CAD and media licenses prior to publication.
