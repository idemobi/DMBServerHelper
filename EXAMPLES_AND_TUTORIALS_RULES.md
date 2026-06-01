# DMBServerHelper Examples and Tutorials Rules

## Objective

Define how information pages, instruction pages, concept pages, example pages, demo pages, and tutorials are created for `DMBServerHelper`.

These rules apply only when the task explicitly creates or updates:

- information pages,
- instruction pages,
- concept pages,
- example pages,
- demo pages,
- tutorial pages,
- walkthrough pages,
- example partials rendered through `RenderExamplePartialAsync`.

Do not use this file as the rule source for XML API documentation or reference documentation. Use `DOCUMENTATION_RULES.md` for that work.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Default documentation area: `ServerHelper`
- Publication target: `../labs_idemobi_com`
- Shared UI stack: `DMBBootstrapBuilder`, `DMBComponentBuilder`, `DMBFormBuilder`, and `DMBPageBuilder`
- Default DocumentationViewer package id for this project: `DMBServerHelper`
- Default DocumentationViewer namespace for this project: `DMBServerHelper`
- Expected page emphasis: configuration, domain composition, localization, typed cookies, typed sessions, and API documentation registration.

## Publication target

Examples and tutorials must be written in `../labs_idemobi_com`.

Use the existing MVC conventions in that project:

- controller actions in `labs_idemobi_com/Controllers`,
- full pages in `labs_idemobi_com/Views/{FeatureOrComponent}/`,
- reusable example partials in `labs_idemobi_com/Views/Shared/Examples/`,
- generated raw-code mirrors in `labs_idemobi_com/Views/Shared/Examples_Raw/`.

AI may create or update source example partials under `Views/Shared/Examples/`. The developer or prebuild step is responsible for regenerating `Views/Shared/Examples_Raw/` when required.

## Shared UI stack

Example, information, concept, and tutorial pages must use the existing PageBuilder ecosystem components instead of ad-hoc layout markup when a suitable component exists.

Prefer:

- `DMBBootstrapBuilder` for layout, titles, cards, rows, columns, alerts, badges, buttons, tables, tabs, and Bootstrap-oriented UI.
- `DMBComponentBuilder` for reusable non-form visual components available in the project.
- `DMBFormBuilder` for forms, fields, validation examples, and form-state demonstrations.
- `DMBPageBuilder` for page metadata, raw HTML builders, render lifecycle concepts, and low-level HTML composition.

Do not introduce a new frontend framework or independent demo system for examples.

## Page categories

There are two distinct page formats:

- general information pages,
- component-like example pages.

Do not merge the two formats unless the user explicitly asks for a hybrid page.

`DMBServerHelper` is primarily an infrastructure package. Most pages should be general information, instruction, or concept pages. Use the component-like format only when a page is dedicated to one public class or one tightly scoped API family and benefits from multiple rendered examples.

## General information page format

Use this format for package introductions, architecture pages, configuration guides, localization guides, cookie guides, session guides, validation guides, and tutorials that are not focused on one component class.

Required structure:

1. Title.
2. Short context paragraph explaining the topic and audience.
3. Explanation sections with deterministic headings.
4. Practical integration or usage section when relevant.
5. Notes, constraints, or next steps.
6. Links to related documentation pages or API reference when useful.

General information pages:

- may include short code snippets rendered with `CodeBlockBuilder`,
- may include diagrams or structured lists when they clarify architecture, instructions, configuration behavior, or concepts,
- should not include component galleries unless the page is explicitly an overview gallery,
- should avoid long inline API listings that belong in DocumentationViewer.

## Draw.io diagrams on information and tutorial pages

Information pages, instruction pages, concept pages, architecture pages, configuration pages, and tutorials may include Draw.io diagrams when a visual model clarifies the explanation.

Draw.io diagrams must follow `DRAWIO_DIAGRAM_RULES.md`.

Do not use Draw.io diagrams as decoration. Add a diagram only when it explains a real configuration flow, localization lookup, cookie lifecycle, session lifecycle, validation adapter flow, or API documentation registration flow.

## Code examples on information pages

Code examples on general information pages must use `CodeBlockBuilder` or the existing `Html.CodeBlock(...)` helper when available.

Do not write raw `<pre><code>` blocks by hand when `CodeBlockBuilder` can render the example.

Code examples must:

- specify the language,
- have a useful title when the page contains more than one snippet,
- enable copy behavior when that is consistent with existing pages,
- stay focused on the concept explained by the page.

## Links and actions on information pages

Links that behave like page actions must be generated through `ActionItem` implementations and rendered with `ButtonRender` when possible.

Prefer:

- `AspRouteActionItem` or `ActionItemFactory.AspRoute(...)` for MVC route links,
- `UrlActionItem` for external or absolute URLs,
- `ButtonRender` to render action buttons consistently with BootstrapBuilder.

Use plain inline anchors only for natural text links inside paragraphs where a button/action would be visually inappropriate.

## Component-like page format

Use this format for a page dedicated to one public class or one tightly scoped API family that needs multiple examples.

Every component-like page must follow the same high-level order:

1. Title.
2. Explanation of the API or API family.
3. DocumentationViewer button linking to the relevant API documentation.
4. Gallery of examples rendered with `@await Html.RenderExamplePartialAsync(...)` when examples are useful.
5. Showcase list or usage matrix.

### Title

Use `TitleBuilder` for the main title.

The title should name the class or API family clearly. Prefer the public class name when the page is class-specific, for example `GenericConfiguration<T>`, `CookieDefinition`, `SessionDefinition`, `CombinedStringLocalizer`, or `DomainComposite`.

### API explanation

The explanation must be short and practical.

It should cover:

- what the API configures, stores, resolves, validates, or composes,
- the primary use case,
- the most important configuration points,
- any important security, lifecycle, session, cookie, localization, or validation concern.

Do not duplicate the full API reference on the page.

### DocumentationViewer button

Every component-like page must include a visible button linking to the generated API documentation.

Use the existing DocumentationViewer route through the `Documentation` controller:

```csharp
@Url.Action("Show", "Documentation", new
{
    groupName = "NuGet",
    packageId = "DMBServerHelper",
    version = "0.9",
    namespaceName = "DMBServerHelper",
    objectName = "CookieDefinition"
})
```

Adjust `packageId`, `namespaceName`, and `objectName` to the API being documented.

When rendering the DocumentationViewer button, prefer an `ActionItem` implementation rendered through `ButtonRender` instead of a handwritten `<a class="btn ...">` element.

## Minimum example coverage

When a class or API family gets a dedicated example gallery, include at least:

- normal usage,
- missing or empty input behavior when applicable,
- invalid or error behavior when applicable,
- one realistic application integration example.

Add more examples when the API has important configuration, security, localization, cookie, session, validation, or documentation registration behavior.

## Delivery checklist for examples

Before finishing an example or tutorial task, verify:

- the page is under `labs_idemobi_com`,
- the page uses existing BootstrapBuilder, ComponentBuilder, FormBuilder, or PageBuilder components where appropriate,
- code examples use `CodeBlockBuilder` or `Html.CodeBlock(...)`,
- action links use `ActionItem` with `ButtonRender` when appropriate,
- Draw.io diagrams follow `DRAWIO_DIAGRAM_RULES.md`,
- DocumentationViewer links target `DMBServerHelper`,
- no build/test command was run unless the user explicitly requested it.
