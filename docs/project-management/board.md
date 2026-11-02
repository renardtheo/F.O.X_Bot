# FOX Roadmap — GitHub Projects

Board states: **Backlog → Ready → In Progress → In Review → Blocked → Done**.

Custom fields: Priority (P0,P1,P2), Area (Core, Runtime, Web, AI, Hardware, Docs), Iteration (2 weeks).

Create with `bash scripts/bootstrap-github.sh OWNER REPO`. The script creates a GitHub Project, project fields, repository labels and imports the backlog. Configure the **Status** built-in field and board view manually if not already present; then enable auto-add in Projects workflows.

Backlog is initially treated as source-of-truth for the import. Running the script again will skip issue titles already present but will not overwrite existing issue descriptions.
