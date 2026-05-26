# DMBServerHelper Local Development Runbook

## Purpose

Provide a lightweight workflow for local work in `DMBServerHelper`.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Project folder: `DMBServerHelper`
- Project role: core server infrastructure package for the PageBuilder ecosystem.
- Publication host: `labs_idemobi_com`

## Orientation

Start by reading:

- [README.md](README.md)
- [PROJECT_MAP.md](PROJECT_MAP.md)
- [DOCUMENTATION_RULES.md](DOCUMENTATION_RULES.md)
- [DELIVERY_CHECKLIST.md](DELIVERY_CHECKLIST.md)

For example or tutorial page work, also read:

- [EXAMPLES_AND_TUTORIALS_RULES.md](EXAMPLES_AND_TUTORIALS_RULES.md)
- [DRAWIO_DIAGRAM_RULES.md](DRAWIO_DIAGRAM_RULES.md)

## Work loop

1. Identify the affected feature family:
   - configuration,
   - domain composition,
   - localization,
   - cookies,
   - sessions,
   - validation,
   - API documentation registration,
   - documentation pages.
2. Read the relevant code and local rules before editing.
3. Keep edits local to the smallest useful area.
4. Update XML documentation for touched public APIs.
5. Update README or guidance files when behavior changes.
6. Run only checks that the user explicitly permits.

## Build and test policy

Do not run these commands unless explicitly requested:

```text
dotnet build
dotnet test
dotnet restore
dotnet format
```

When the user does request build or test verification, report exactly what was run and what failed or passed.

## Documentation page workflow

When editing pages in `labs_idemobi_com`:

- follow `EXAMPLES_AND_TUTORIALS_RULES.md`,
- use existing PageBuilder ecosystem components,
- use `CodeBlockBuilder` or `Html.CodeBlock(...)` for code examples,
- use `ActionItem` with `ButtonRender` for action-style links,
- use `DRAWIO_DIAGRAM_RULES.md` for editable diagrams,
- do not claim DocumentationBuilder was run unless it was actually run.

## Safe inspection commands

Useful read-only commands:

```text
rg "GenericConfiguration" DMBServerHelper
rg "CookieDefinition" DMBServerHelper
rg "SessionDefinition" DMBServerHelper
find DMBServerHelper -maxdepth 2 -type f | sort
git diff -- DMBServerHelper
```

Prefer `rg` for searches.

## Final report

At the end of a task, include:

- files changed,
- verification performed,
- build/test status,
- remaining risks or follow-up items.
