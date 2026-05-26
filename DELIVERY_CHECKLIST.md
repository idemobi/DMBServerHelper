# DMBServerHelper Delivery Checklist

## Purpose

Use this checklist before finishing changes in `DMBServerHelper`.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Project folder: `DMBServerHelper`
- Project role: core server infrastructure package for the PageBuilder ecosystem.
- Publication host: `labs_idemobi_com`

## Code checklist

- Public API compatibility was preserved, or the breaking change was explicitly requested.
- New or changed public members have useful English XML documentation.
- Configuration loading behavior is deterministic and documented.
- Cookie/session registry behavior is explicit.
- Cookie and session serialization, parsing, default values, and deletion behavior were reviewed.
- Localization lookup order and fallback behavior were reviewed.
- Validation attributes and adapters remain aligned.
- No unrelated files were reformatted or refactored.

## Documentation checklist

- README was updated when project behavior or usage changed.
- `DOCUMENTATION_RULES.md` was followed for XML docs and reference documentation.
- `EXAMPLES_AND_TUTORIALS_RULES.md` was used only for example, demo, information, instruction, concept, or tutorial pages.
- `DRAWIO_DIAGRAM_RULES.md` was followed when diagrams were added or updated.
- Documentation names `DMBServerHelper` concepts, not copied source-project concepts.
- Documentation is written in English unless the task explicitly requested another language for website content.

## Localization checklist

- Localization keys follow `LOCALIZATION_NOMENCLATURE.md`.
- Existing key families were reused where possible.
- Resource changes were kept consistent with generated designer files.

## Verification checklist

- Do not run `dotnet build`, `dotnet test`, `dotnet restore`, or `dotnet format` unless explicitly requested.
- If no build or tests were run, say so in the final response.
- If only text checks were run, name those checks precisely.
- Mention any remaining risks or manual validation needs.

## Final response checklist

- Summarize changed files.
- Mention that build/test were not run unless explicitly requested and actually executed.
- List follow-up items only when they are useful and specific.
