#!/usr/bin/env bash
set -euo pipefail
# Usage: bash scripts/bootstrap-github.sh OWNER REPO
OWNER="${1:?Expected GitHub owner}"
REPO="${2:?Expected repo name}"
TARGET="$OWNER/$REPO"
command -v gh >/dev/null || { echo 'Install GitHub CLI (gh)'; exit 1; }
command -v python3 >/dev/null || { echo 'Install Python 3'; exit 1; }
gh auth status
gh repo view "$TARGET" >/dev/null
for label in 'type:story' 'type:bug' 'type:chore' 'area:core' 'area:runtime' 'area:web' 'area:ai' 'area:hardware' 'area:docs' 'priority:P0' 'priority:P1' 'priority:P2'; do
  gh label create "$label" --repo "$TARGET" --color 'BFD4F2' --force >/dev/null
done
python3 scripts/import-backlog.py "$TARGET"
PROJECT_NUMBER="$(gh project list --owner "$OWNER" --format json --jq '.projects[] | select(.title == "FOX Roadmap") | .number' | head -n1)"
if [[ -z "$PROJECT_NUMBER" ]]; then
  PROJECT_NUMBER="$(gh project create --owner "$OWNER" --title 'FOX Roadmap' --format json --jq '.number')"
fi
for FIELD in 'Priority:P0,P1,P2' 'Area:Core,Runtime,Web,AI,Hardware,Docs'; do
 NAME="${FIELD%%:*}"
 OPTIONS="${FIELD#*:}"
 EXISTING="$(gh project field-list "$PROJECT_NUMBER" --owner "$OWNER" --format json --jq ".fields[] | select(.name == \"$NAME\") | .name" 2>/dev/null || true)"
 if [[ -z "$EXISTING" ]]; then
   gh project field-create "$PROJECT_NUMBER" --owner "$OWNER" --name "$NAME" --data-type SINGLE_SELECT --single-select-options "$OPTIONS" >/dev/null
 fi
done
# Add issues to project (safe to re-run; API ignores already-added items).
while IFS= read -r url; do
  gh project item-add "$PROJECT_NUMBER" --owner "$OWNER" --url "$url" >/dev/null || echo "Could not add $url"
done < <(gh issue list --repo "$TARGET" --state all --limit 500 --json url --jq '.[].url')
echo "Project ready: https://github.com/users/$OWNER/projects/$PROJECT_NUMBER (org owners use /orgs/$OWNER/projects/$PROJECT_NUMBER)"
echo 'Configure Status options and Board layout through GitHub Projects UI.'
