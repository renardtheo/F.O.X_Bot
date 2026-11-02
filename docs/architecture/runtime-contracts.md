# Runtime boundaries

- `core-api` owns task/project business logic and interactions with Ollama.
- `robot-runtime` is independently operational, exposing `/health`, `/capabilities`, `/expressions`.
- `web` uses the core API, which currently simulates expressions independently of the Python runtime. **Core-to-runtime forwarding is not yet wired**; tracked as a follow-up.
- Data are currently stored in process memory and reset on restart. SQLite and external GitHub Kanban synchronization are roadmap items.
- No automatic shell execution or source code write capabilities are implemented.

## Security

Limit access to trusted networks. Ollama requests must be restricted to an approved endpoint and model; protect tokens and logs. User-controlled repository content is never trusted as executable instructions.
