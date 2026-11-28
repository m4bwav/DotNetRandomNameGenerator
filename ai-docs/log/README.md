# Log

One file per event, never edited: `YYYY-MM-DD-HHMMSS-op-xxxx.md` holding `## [YYYY-MM-DD] op | title` and, if needed, a few lines of detail (op: add, update, change, supersede, verify, verify-failed, prune, handoff, index, or any short word). Write an event with `everlast.py log <repo> --op OP --title T [--body B]`, or by creating such a file by hand. Read the history in order with `everlast.py log-show <repo>`. One file per event means two branches never edit the same file, so the log never causes a merge conflict. `ARCHIVE.md`, when present, is the history from before the split (the old single log.md), frozen.

