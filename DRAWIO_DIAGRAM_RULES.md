# DMBServerHelper Draw.io Diagram Rules

## Objective

Draw.io diagrams may be created for information pages, instruction pages, concept pages, architecture pages, configuration pages, and tutorials when a visual model makes the explanation clearer.

Do not create a diagram as decoration. A diagram must explain a real concept, flow, dependency, lifecycle, state model, registry, localization lookup, configuration flow, cookie flow, or session flow.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Diagram publication root: `labs_idemobi_com/wwwroot/drawio`
- Default area folder for this project: `ServerHelper`
- Shared template path: `labs_idemobi_com/wwwroot/drawio/_templates/concept-flow-template.drawio.svg`
- Preferred page rendering approach: use existing PageBuilder or BootstrapBuilder image helpers when available.

## Required format

All Draw.io diagrams must be saved as enriched SVG files:

```text
*.drawio.svg
```

The SVG must remain editable in Draw.io. It must contain the embedded Draw.io model, either through the Draw.io `content` attribute or Draw.io metadata containing an `<mxfile>` payload.

Do not commit flattened SVG-only exports, PNG-only exports, screenshots, or manually rewritten SVG diagrams for Draw.io-managed documentation diagrams.

## Publication path

Diagrams used by `labs_idemobi_com` pages must be stored under:

```text
labs_idemobi_com/wwwroot/drawio/{Area}/{diagram-name}.drawio.svg
```

Use the area folder to identify the documented module or concept:

- `ServerHelper`
- `ServerHelper/Configuration`
- `ServerHelper/Localization`
- `ServerHelper/Cookies`
- `ServerHelper/Sessions`
- `ServerHelper/Validation`

Use a stable, descriptive diagram file name:

- `configuration-loading-flow.drawio.svg`
- `localizer-lookup-flow.drawio.svg`
- `cookie-definition-lifecycle.drawio.svg`
- `session-definition-lifecycle.drawio.svg`
- `validation-adapter-flow.drawio.svg`

When referenced from Razor, use the web path:

```html
<img src="/drawio/ServerHelper/configuration-loading-flow.drawio.svg" alt="DMBServerHelper configuration loading flow diagram" />
```

Prefer rendering diagrams through the existing PageBuilder or BootstrapBuilder image helpers when an appropriate helper exists.

## Draw.io grid and layout rules

Draw.io source geometry must stay grid-aligned:

- enable the Draw.io grid,
- use a `gridSize` of `10`,
- place x, y, width, and height values on multiples of `10`,
- keep connector waypoints on grid intersections,
- use orthogonal connectors for process and architecture flows,
- keep related elements in rows or columns with consistent spacing,
- avoid freehand, skewed, or visually approximate positioning.

Use a left-to-right flow for configuration and lookup pipelines. Use top-to-bottom flow only when it matches the concept better.

## Light and dark mode compatibility

Diagrams must work on both light and dark page themes.

Required practices:

- keep the root SVG background transparent,
- include `color-scheme: light dark` on the root SVG when exported,
- use Draw.io `adaptiveColors="auto"` in the embedded graph model when available,
- avoid pure black text on transparent backgrounds,
- avoid pure white fills without a visible border,
- use high-contrast stroke and text colors,
- use restrained semantic colors that remain distinguishable in both themes,
- verify labels remain readable when the hosting page switches between light and dark themes.

When editing the exported SVG manually is unavoidable, only adjust theme-compatibility attributes and do not break the embedded Draw.io model.

## Accessibility and page usage

Every diagram rendered in a page must have meaningful alternative text.

The surrounding page must explain the diagram in normal text. Do not rely on the diagram as the only source of critical information.

If the diagram is complex, add a short textual summary immediately before or after it.

## Naming and maintenance

- Keep diagram names stable once linked from a page.
- Update the diagram in the same change set as the concept page when the documented flow changes.
- Keep labels short enough to remain readable at desktop and tablet widths.
- Avoid implementation details that change often unless the page is specifically about that implementation.
- Prefer one focused diagram over a large all-in-one diagram.

## Template

Use this editable SVG template as the starting point for new diagrams:

```text
labs_idemobi_com/wwwroot/drawio/_templates/concept-flow-template.drawio.svg
```

The template includes:

- embedded Draw.io model data,
- grid size `10`,
- grid-aligned geometry,
- transparent SVG background,
- light/dark color-scheme support,
- a simple concept flow that can be renamed and extended.
