#!/usr/bin/env python3
"""Create starter GitHub issues idempotently by exact title using GitHub CLI."""
import json
import subprocess
import sys
from pathlib import Path

def gh(*args):
    return subprocess.check_output(['gh', *args], text=True)

def main():
    target = sys.argv[1]
    existing = json.loads(gh('issue', 'list', '--repo', target, '--state', 'all', '--limit', '500', '--json', 'title'))
    names = {item['title'] for item in existing}
    for item in json.loads(Path('backlog/issues.json').read_text(encoding='utf-8')):
        if item['title'] in names:
            print('Skipping', item['title'])
            continue
        args = ['issue', 'create', '--repo', target, '--title', item['title'], '--body', item['body']]
        for label in item['labels']:
            args += ['--label', label]
        print(gh(*args).strip())
if __name__ == '__main__':
    main()
