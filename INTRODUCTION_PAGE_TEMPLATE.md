# Page Template — Introduction / Getting Started / Architecture / Rendering Pipeline

## Purpose

Ce fichier est le template de référence pour les quatre pages standard présentes dans chaque
catégorie du site PageBuilder : **Introduction**, **Getting Started**, **Architecture**, **Rendering Pipeline**.

Implémentation de référence : `labs_idemobi_com/Views/EffectBuilder/`

---

## Règles absolues (à respecter sans exception)

### 1. Aucune balise HTML brute
Toutes les balises HTML sont interdites dans les vues. Utiliser exclusivement les composants PageBuilder :

| HTML interdit          | Remplacement PageBuilder                                                       |
|------------------------|--------------------------------------------------------------------------------|
| `<div>`                | `Html.BlockBuilder()`                                                          |
| `<p>`                  | `Html.PB_P()`                                                                  |
| `<h1>` … `<h6>`        | `Html.TitleBuilder().SetTitle("...", TitleLevel.One)` … `.Six`                 |
| `<span>`, `<i>`        | `Html.PB_Span()`                                                               |
| `<strong>`             | `Html.PB_Span().AddClass("fw-bold")`                                           |
| `<em>`                 | `Html.PB_Span().AddClass("fst-italic")`                                        |
| `<code>`               | `Html.PB_Span().AddClass("font-monospace bg-body-secondary rounded px-1")`     |
| `<ul>` / `<li>`        | `BlockBuilder` imbriqués avec les classes Bootstrap équivalentes               |

### 2. Règle RZ1006 — blocs `@:` toujours multi-lignes

**Toute** balise `@using` contenant du texte `@:` doit être expansée sur plusieurs lignes.
Un bloc sur une seule ligne provoque l'erreur RZ1006 (Razor ferme le `using` trop tôt).

```razor
@* ❌ INTERDIT — provoque RZ1006 *@
@using (Html.PB_Span().AddClass("fw-bold").Begin()) { @:Texte }

@* ✅ CORRECT *@
@using (Html.PB_Span().AddClass("fw-bold").Begin())
{
    @:Texte
}
```

La seule exception : les blocs **vides** `{ }` (sans `@:`) sont acceptés sur une ligne.
```razor
@using (Html.PB_Span().AddClass("bi bi-check").Begin()) { }
```

### 3. `FlexBlockBuilder` n'accepte pas `.AddClass()`
`FlexBlockBuilder` n'implémente pas `ICanUseCustomClasses`. Pour ajouter des classes sur un flex,
utiliser `BlockBuilder().AddClass("d-flex ...")` à la place.

```razor
@* ❌ INTERDIT *@
@using (Html.FlexBlockBuilder().AddClass("gap-2").Begin())

@* ✅ CORRECT *@
@using (Html.BlockBuilder().AddClass("d-flex align-items-center gap-2").Begin())
```

### 4. Couleurs toujours theme-aware
Ne jamais utiliser `bg-dark`, `bg-light`, `bg-white`, `text-dark`, `text-white`.
Utiliser `bg-body`, `bg-body-secondary`, `bg-body-tertiary`, `text-muted`, `text-body`.

### 5. Sidebar sticky
La sidebar utilise `BlockBuilder().AddClass("sticky-top").SetStyle("top", "130px")`.
Ne jamais écrire `style="top: 130px"` inline dans du HTML brut.

---

## Usings requis (communs aux 4 pages)

```razor
@using DMBBootstrapBuilder
@using DMBComponentBuilder
@using {{NAMESPACE}}
@using DMBPageBuilder
@{
    Layout = "_Layout";
}
```

`{{NAMESPACE}}` = namespace du package concerné (ex. `DMBEffectBuilder`).
À omettre si aucun type du package n'est utilisé directement dans la vue.

---

## Emplacements des fichiers

```
Introduction       : labs_idemobi_com/Views/{{CONTROLLER}}/Introduction.cshtml
Getting Started    : labs_idemobi_com/Views/{{CONTROLLER}}/GettingStarted.cshtml
Architecture       : labs_idemobi_com/Views/{{CONTROLLER}}/Architecture.cshtml
Rendering Pipeline : labs_idemobi_com/Views/{{CONTROLLER}}/RenderingPipeline.cshtml

Controller         : labs_idemobi_com/Controllers/{{CONTROLLER}}Controller.cs
                     → une action IActionResult publique par page
```

---

## Placeholders

| Placeholder              | Exemple                                                              |
|--------------------------|----------------------------------------------------------------------|
| `{{NAMESPACE}}`          | `DMBEffectBuilder`                                                   |
| `{{CONTROLLER}}`         | `EffectBuilder`                                                      |
| `{{PACKAGE_NAME}}`       | `EffectBuilder`                                                      |
| `{{TAGLINE}}`            | `Giving life to your components with simple Fluent API extensions.`  |
| `{{DESCRIPTION}}`        | Une ou deux phrases sur le rôle du package.                          |
| `{{PILLARS_HEADING}}`    | `Three Pillars of Effects`                                           |
| `{{PILLARS_INTRO}}`      | Courte phrase introduisant les trois domaines.                       |
| `{{ICON_N}}`             | Classe Bootstrap Icons, ex. `bi-image`                               |
| `{{PILLAR_N_TITLE}}`     | `Image Effects`                                                      |
| `{{PILLAR_N_DESC}}`      | Courte phrase se terminant par le nom du builder concerné.           |
| `{{PILLAR_N_CODE}}`      | `.PulseEffect()`                                                     |
| `{{PHILOSOPHY_TITLE}}`   | `Core Philosophy`                                                    |
| `{{PHILOSOPHY_TEXT}}`    | Une ou deux phrases sur l'intention de design.                       |
| `{{BENEFIT_N_LABEL}}`    | `No CSS Overhead`                                                    |
| `{{BENEFIT_N_DESC}}`     | Une phrase.                                                          |
| `{{PREVIEW_HINT}}`       | Courte phrase décrivant l'aperçu interactif.                         |
| `{{PREVIEW_COMPONENT}}`  | Snippet Razor rendant un composant live (ex. `ImageRender(...)`).    |
| `{{PREVIEW_CODE}}`       | One-liner affiché sous l'aperçu dans la balise monospace.            |
| `{{STEP_N_TITLE}}`       | `Install the NuGet Package`                                          |
| `{{STEP_N_DESC}}`        | Paragraphe avant le bloc de code.                                    |
| `{{STEP_N_CODE}}`        | Contenu du code block.                                               |
| `{{STEP_N_LANG}}`        | `CodeLanguage.Bash` ou `CodeLanguage.CSharp`                         |
| `{{STEP_N_CODE_TITLE}}`  | Titre affiché au-dessus du code block.                               |
| `{{QUICKREF_ITEM_N}}`    | `.GrayscaleHoverEffect()`                                            |
| `{{ARCH_N_TITLE}}`       | `Component Separation`                                               |
| `{{ARCH_N_CODE}}`        | Contenu du code block illustrant le concept.                         |
| `{{ARCH_N_LANG}}`        | `CodeLanguage.CSharp` ou `CodeLanguage.Css`                          |
| `{{TECH_KEY_N}}`         | `Backend`                                                            |
| `{{TECH_VAL_N}}`         | `.NET 10 / ASP.NET Core`                                             |
| `{{PIPE_N_TITLE}}`       | `Fluent API Call`                                                    |
| `{{PIPE_N_BODY}}`        | Paragraphe expliquant cette étape du pipeline.                       |
| `{{PIPE_N_CODE}}`        | Contenu du code block de l'étape.                                    |
| `{{PIPE_N_LANG}}`        | `CodeLanguage.CSharp`                                                |
| `{{PERF_TITLE}}`         | `Optimized for Speed`                                                |
| `{{PERF_DESC}}`          | Courte phrase sur les performances.                                  |
| `{{NEXT_STEP_LABEL}}`    | `Getting Started`                                                    |
| `{{NEXT_STEP_CONTROLLER}}`| `EffectBuilder`                                                     |
| `{{NEXT_STEP_ACTION}}`   | `GettingStarted`                                                     |
| `{{NEXT_STEP_ICON}}`     | `bi-play-fill`                                                       |
| `{{NEXT_STEP_DESC}}`     | Une phrase décrivant la page suivante.                               |

Couleurs des icônes piliers (position fixe) : `text-primary` / `text-success` / `text-warning`.

---

## Squelette 1 — Introduction

```razor
@using DMBBootstrapBuilder
@using DMBComponentBuilder
@using {{NAMESPACE}}
@using DMBPageBuilder
@{
    Layout = "_Layout";
}

@using (Html.SectionBuilder().SetHeight(125, UnitSize.px).Begin())
{
    @using (Html.FlexBlockBuilder().SetAlignItemsCenter().SetJustifyContentCenter().Begin())
    {
        @using (Html.BlockBuilder().AddClass("text-center").Begin())
        {
            @Html.TitleBuilder().SetTitle("Introduction — {{PACKAGE_NAME}}", TitleLevel.One).Render()
            @using (Html.PB_P().AddClass("lead").Begin())
            {
                @:{{TAGLINE}}
            }
        }
    }
}

@using (Html.ContainerBuilder().SetPadding(SpacingSide.Y, SpacingSize.Five).Begin())
{
    @using (Html.RowBuilder().Begin())
    {
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col8, ResponsiveBreakpoint.Md).Begin())
        {
            @Html.TitleBuilder().SetTitle("What is {{PACKAGE_NAME}}?", TitleLevel.Two).AddClass("mb-4").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{DESCRIPTION}}
            }

            @Html.TitleBuilder().SetTitle("{{PILLARS_HEADING}}", TitleLevel.Three).AddClass("mt-5").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{PILLARS_INTRO}}
            }

            @using (Html.RowBuilder().SetGap(Gap.G4).AddClass("mt-2").Begin())
            {
                @using (Html.ColBuilder().SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
                {
                    @using (Html.BlockBuilder().AddClass("p-3 border rounded-4 h-100 shadow-sm").Begin())
                    {
                        @using (Html.PB_Span().AddClass("bi {{ICON_1}} fs-1 text-primary mb-3 d-block").Begin()) { }
                        @Html.TitleBuilder().SetTitle("{{PILLAR_1_TITLE}}", TitleLevel.Five).Render()
                        @using (Html.PB_P().AddClass("small text-muted").Begin())
                        {
                            @:{{PILLAR_1_DESC}}
                        }
                        @using (Html.PB_Span().AddClass("font-monospace small").Begin())
                        {
                            @:{{PILLAR_1_CODE}}
                        }
                    }
                }
                @using (Html.ColBuilder().SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
                {
                    @using (Html.BlockBuilder().AddClass("p-3 border rounded-4 h-100 shadow-sm").Begin())
                    {
                        @using (Html.PB_Span().AddClass("bi {{ICON_2}} fs-1 text-success mb-3 d-block").Begin()) { }
                        @Html.TitleBuilder().SetTitle("{{PILLAR_2_TITLE}}", TitleLevel.Five).Render()
                        @using (Html.PB_P().AddClass("small text-muted").Begin())
                        {
                            @:{{PILLAR_2_DESC}}
                        }
                        @using (Html.PB_Span().AddClass("font-monospace small").Begin())
                        {
                            @:{{PILLAR_2_CODE}}
                        }
                    }
                }
                @using (Html.ColBuilder().SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
                {
                    @using (Html.BlockBuilder().AddClass("p-3 border rounded-4 h-100 shadow-sm").Begin())
                    {
                        @using (Html.PB_Span().AddClass("bi {{ICON_3}} fs-1 text-warning mb-3 d-block").Begin()) { }
                        @Html.TitleBuilder().SetTitle("{{PILLAR_3_TITLE}}", TitleLevel.Five).Render()
                        @using (Html.PB_P().AddClass("small text-muted").Begin())
                        {
                            @:{{PILLAR_3_DESC}}
                        }
                        @using (Html.PB_Span().AddClass("font-monospace small").Begin())
                        {
                            @:{{PILLAR_3_CODE}}
                        }
                    }
                }
            }

            @using (Html.BlockBuilder().AddClass("alert alert-info border-0 shadow-sm rounded-4 p-4 my-4").Begin())
            {
                @using (Html.BlockBuilder().AddClass("d-flex").Begin())
                {
                    @using (Html.BlockBuilder().AddClass("me-3").Begin())
                    {
                        @using (Html.PB_Span().AddClass("bi bi-lightbulb-fill fs-2 text-info").Begin()) { }
                    }
                    @using (Html.BlockBuilder().Begin())
                    {
                        @Html.TitleBuilder().SetTitle("{{PHILOSOPHY_TITLE}}", TitleLevel.Four).AddClass("alert-heading").Render()
                        @using (Html.PB_P().AddClass("mb-0").Begin())
                        {
                            @:{{PHILOSOPHY_TEXT}}
                        }
                    }
                }
            }

            @Html.TitleBuilder().SetTitle("Key Benefits", TitleLevel.Three).AddClass("mt-5").Render()

            @using (Html.BlockBuilder().AddClass("mb-3 d-flex align-items-start").Begin())
            {
                @using (Html.PB_Span().AddClass("bi bi-check-circle-fill text-success me-3 mt-1").Begin()) { }
                @using (Html.BlockBuilder().Begin())
                {
                    @using (Html.PB_Span().AddClass("fw-bold").Begin())
                    {
                        @:{{BENEFIT_1_LABEL}}:
                    }
                    @: {{BENEFIT_1_DESC}}
                }
            }
            @using (Html.BlockBuilder().AddClass("mb-3 d-flex align-items-start").Begin())
            {
                @using (Html.PB_Span().AddClass("bi bi-check-circle-fill text-success me-3 mt-1").Begin()) { }
                @using (Html.BlockBuilder().Begin())
                {
                    @using (Html.PB_Span().AddClass("fw-bold").Begin())
                    {
                        @:{{BENEFIT_2_LABEL}}:
                    }
                    @: {{BENEFIT_2_DESC}}
                }
            }
            @using (Html.BlockBuilder().AddClass("mb-3 d-flex align-items-start").Begin())
            {
                @using (Html.PB_Span().AddClass("bi bi-check-circle-fill text-success me-3 mt-1").Begin()) { }
                @using (Html.BlockBuilder().Begin())
                {
                    @using (Html.PB_Span().AddClass("fw-bold").Begin())
                    {
                        @:{{BENEFIT_3_LABEL}}:
                    }
                    @: {{BENEFIT_3_DESC}}
                }
            }
        }

        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AddClass("sticky-top").SetStyle("top", "130px").Begin())
            {
                @using (Html.BlockBuilder().AddClass("border-0 shadow-sm rounded-4 overflow-hidden").Begin())
                {
                    @using (Html.BlockBuilder().AddClass("p-4").Begin())
                    {
                        @Html.TitleBuilder().SetTitle("Quick Preview", TitleLevel.Four).Render()
                        @using (Html.PB_P().AddClass("text-muted small").Begin())
                        {
                            @:{{PREVIEW_HINT}}
                        }
                        @using (Html.BlockBuilder().AddClass("rounded-3 overflow-hidden").Begin())
                        {
                            {{PREVIEW_COMPONENT}}
                        }
                        @using (Html.BlockBuilder().AddClass("mt-3").Begin())
                        {
                            @using (Html.PB_Span().AddClass("bg-body-secondary p-2 rounded d-block small font-monospace overflow-x-auto").Begin())
                            {
                                @:{{PREVIEW_CODE}}
                            }
                        }
                    }
                }
                @using (Html.BlockBuilder().AddClass("mt-4 p-4 bg-body-secondary rounded-4 shadow-sm").Begin())
                {
                    @Html.TitleBuilder().SetTitle("Next Step", TitleLevel.Five).Render()
                    @using (Html.PB_P().AddClass("small text-muted").Begin())
                    {
                        @:{{NEXT_STEP_DESC}}
                    }
                    @Html.Button(ActionItemFactory.AspRoute("{{CONTROLLER}}", "{{NEXT_STEP_ACTION}}").SetTitle("{{NEXT_STEP_LABEL}}").SetIcon(IconStruct.Bootstrap("{{NEXT_STEP_ICON}}")).SetVariant(VariantStyle.Primary)).Render()
                }
            }
        }
    }
}
```

---

## Squelette 2 — Getting Started

```razor
@using DMBBootstrapBuilder
@using DMBComponentBuilder
@using {{NAMESPACE}}
@using DMBPageBuilder
@{
    Layout = "_Layout";
}

@using (Html.SectionBuilder().SetHeight(125, UnitSize.px).Begin())
{
    @using (Html.FlexBlockBuilder().SetAlignItemsCenter().SetJustifyContentCenter().Begin())
    {
        @using (Html.BlockBuilder().AddClass("text-center").Begin())
        {
            @Html.TitleBuilder().SetTitle("Getting Started — {{PACKAGE_NAME}}", TitleLevel.One).Render()
            @using (Html.PB_P().AddClass("lead").Begin())
            {
                @:{{TAGLINE}}
            }
        }
    }
}

@using (Html.ContainerBuilder().SetPadding(SpacingSide.Y, SpacingSize.Five).Begin())
{
    @using (Html.RowBuilder().Begin())
    {
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col8, ResponsiveBreakpoint.Md).Begin())
        {
            @Html.TitleBuilder().SetTitle("1. {{STEP_1_TITLE}}", TitleLevel.Two).AddClass("mb-4").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{STEP_1_DESC}}
            }
            @(Html.CodeBlock(@"{{STEP_1_CODE}}", {{STEP_1_LANG}})
                .SetTitle("{{STEP_1_CODE_TITLE}}")
                .SetCopyButton())

            @Html.TitleBuilder().SetTitle("2. {{STEP_2_TITLE}}", TitleLevel.Two).AddClass("mb-4 mt-5").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{STEP_2_DESC}}
            }
            @(Html.CodeBlock(@"{{STEP_2_CODE}}", {{STEP_2_LANG}})
                .SetTitle("{{STEP_2_CODE_TITLE}}")
                .SetCopyButton())

            @Html.TitleBuilder().SetTitle("3. {{STEP_3_TITLE}}", TitleLevel.Two).AddClass("mb-4 mt-5").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{STEP_3_DESC}}
            }
            @(Html.CodeBlock(@"{{STEP_3_CODE}}", {{STEP_3_LANG}})
                .SetTitle("{{STEP_3_CODE_TITLE}}")
                .SetCopyButton())

            @Html.TitleBuilder().SetTitle("4. {{STEP_4_TITLE}}", TitleLevel.Two).AddClass("mb-4 mt-5").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{STEP_4_DESC}}
            }
            @(Html.CodeBlock(@"{{STEP_4_CODE}}", {{STEP_4_LANG}})
                .SetTitle("{{STEP_4_CODE_TITLE}}")
                .SetCopyButton())
        }

        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AddClass("sticky-top").SetStyle("top", "130px").Begin())
            {
                @using (Html.BlockBuilder().AddClass("border-0 shadow-sm rounded-4 overflow-hidden").Begin())
                {
                    @using (Html.BlockBuilder().AddClass("p-4").Begin())
                    {
                        @Html.TitleBuilder().SetTitle("{{QUICKREF_TITLE}}", TitleLevel.Five).Render()
                        @using (Html.BlockBuilder().AddClass("border-top py-2").Begin())
                        {
                            @using (Html.PB_Span().AddClass("font-monospace small").Begin())
                            {
                                @:{{QUICKREF_ITEM_1}}
                            }
                        }
                        @using (Html.BlockBuilder().AddClass("border-top py-2").Begin())
                        {
                            @using (Html.PB_Span().AddClass("font-monospace small").Begin())
                            {
                                @:{{QUICKREF_ITEM_2}}
                            }
                        }
                        @using (Html.BlockBuilder().AddClass("border-top py-2").Begin())
                        {
                            @using (Html.PB_Span().AddClass("font-monospace small").Begin())
                            {
                                @:{{QUICKREF_ITEM_3}}
                            }
                        }
                        @using (Html.BlockBuilder().AddClass("border-top py-2").Begin())
                        {
                            @using (Html.PB_Span().AddClass("font-monospace small").Begin())
                            {
                                @:{{QUICKREF_ITEM_4}}
                            }
                        }
                        @using (Html.BlockBuilder().AddClass("border-top py-2").Begin())
                        {
                            @using (Html.PB_Span().AddClass("font-monospace small").Begin())
                            {
                                @:{{QUICKREF_ITEM_5}}
                            }
                        }
                        @using (Html.BlockBuilder().AddClass("mt-3").Begin())
                        {
                            @Html.Button(ActionItemFactory.AspRoute("{{CATALOG_CONTROLLER}}", "{{CATALOG_ACTION}}").SetTitle("{{CATALOG_LABEL}}").SetIcon(IconStruct.Bootstrap("{{CATALOG_ICON}}")).SetVariant(VariantStyle.Primary)).Render()
                        }
                    }
                }
                @using (Html.BlockBuilder().AddClass("mt-4 p-4 bg-body-secondary rounded-4 shadow-sm").Begin())
                {
                    @Html.TitleBuilder().SetTitle("Next Step", TitleLevel.Five).Render()
                    @using (Html.PB_P().AddClass("small text-muted").Begin())
                    {
                        @:{{NEXT_STEP_DESC}}
                    }
                    @Html.Button(ActionItemFactory.AspRoute("{{CONTROLLER}}", "{{NEXT_STEP_ACTION}}").SetTitle("{{NEXT_STEP_LABEL}}").SetIcon(IconStruct.Bootstrap("{{NEXT_STEP_ICON}}")).SetVariant(VariantStyle.Primary)).Render()
                }
            }
        }
    }
}
```

---

## Squelette 3 — Architecture

```razor
@using DMBBootstrapBuilder
@using DMBComponentBuilder
@using {{NAMESPACE}}
@using DMBPageBuilder
@{
    Layout = "_Layout";
}

@using (Html.SectionBuilder().SetHeight(125, UnitSize.px).Begin())
{
    @using (Html.FlexBlockBuilder().SetAlignItemsCenter().SetJustifyContentCenter().Begin())
    {
        @using (Html.BlockBuilder().AddClass("text-center").Begin())
        {
            @Html.TitleBuilder().SetTitle("Architecture — {{PACKAGE_NAME}}", TitleLevel.One).Render()
            @using (Html.PB_P().AddClass("lead").Begin())
            {
                @:{{TAGLINE}}
            }
        }
    }
}

@using (Html.ContainerBuilder().SetPadding(SpacingSide.Y, SpacingSize.Five).Begin())
{
    @using (Html.RowBuilder().Begin())
    {
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col8, ResponsiveBreakpoint.Md).Begin())
        {
            @Html.TitleBuilder().SetTitle("1. {{ARCH_1_TITLE}}", TitleLevel.Two).AddClass("mb-4").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{ARCH_1_BODY}}
            }
            @(Html.CodeBlock(@"{{ARCH_1_CODE}}", {{ARCH_1_LANG}})
                .SetTitle("{{ARCH_1_CODE_TITLE}}")
                .SetCopyButton())

            @Html.TitleBuilder().SetTitle("2. {{ARCH_2_TITLE}}", TitleLevel.Two).AddClass("mb-4 mt-5").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{ARCH_2_BODY}}
            }
            @(Html.CodeBlock(@"{{ARCH_2_CODE}}", {{ARCH_2_LANG}})
                .SetTitle("{{ARCH_2_CODE_TITLE}}")
                .SetCopyButton())

            @Html.TitleBuilder().SetTitle("3. {{ARCH_3_TITLE}}", TitleLevel.Two).AddClass("mb-4 mt-5").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{ARCH_3_BODY}}
            }
            @(Html.CodeBlock(@"{{ARCH_3_CODE}}", {{ARCH_3_LANG}})
                .SetTitle("{{ARCH_3_CODE_TITLE}}")
                .SetCopyButton())
        }

        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AddClass("sticky-top").SetStyle("top", "130px").Begin())
            {
                @using (Html.BlockBuilder().AddClass("border-0 shadow-sm rounded-4 overflow-hidden").Begin())
                {
                    @using (Html.BlockBuilder().AddClass("p-4").Begin())
                    {
                        @using (Html.BlockBuilder().AddClass("d-flex align-items-center gap-2 mb-3").Begin())
                        {
                            @using (Html.PB_Span().AddClass("bi bi-gear-fill fs-4 text-primary").Begin()) { }
                            @Html.TitleBuilder().SetTitle("Tech Stack", TitleLevel.Five).AddClass("mb-0").Render()
                        }
                        @using (Html.BlockBuilder().AddClass("border-top pt-3").Begin())
                        {
                            @using (Html.BlockBuilder().AddClass("mb-2 small").Begin())
                            {
                                @using (Html.PB_Span().AddClass("fw-bold").Begin())
                                {
                                    @:{{TECH_KEY_1}}:
                                }
                                @: {{TECH_VAL_1}}
                            }
                            @using (Html.BlockBuilder().AddClass("mb-2 small").Begin())
                            {
                                @using (Html.PB_Span().AddClass("fw-bold").Begin())
                                {
                                    @:{{TECH_KEY_2}}:
                                }
                                @: {{TECH_VAL_2}}
                            }
                            @using (Html.BlockBuilder().AddClass("mb-2 small").Begin())
                            {
                                @using (Html.PB_Span().AddClass("fw-bold").Begin())
                                {
                                    @:{{TECH_KEY_3}}:
                                }
                                @: {{TECH_VAL_3}}
                            }
                            @using (Html.BlockBuilder().AddClass("small").Begin())
                            {
                                @using (Html.PB_Span().AddClass("fw-bold").Begin())
                                {
                                    @:{{TECH_KEY_4}}:
                                }
                                @: {{TECH_VAL_4}}
                            }
                        }
                    }
                }
                @using (Html.BlockBuilder().AddClass("mt-4 p-4 bg-body-secondary rounded-4 shadow-sm").Begin())
                {
                    @Html.TitleBuilder().SetTitle("Next Step", TitleLevel.Five).Render()
                    @using (Html.PB_P().AddClass("small text-muted").Begin())
                    {
                        @:{{NEXT_STEP_DESC}}
                    }
                    @Html.Button(ActionItemFactory.AspRoute("{{CONTROLLER}}", "{{NEXT_STEP_ACTION}}").SetTitle("{{NEXT_STEP_LABEL}}").SetIcon(IconStruct.Bootstrap("{{NEXT_STEP_ICON}}")).SetVariant(VariantStyle.Primary)).Render()
                }
            }
        }
    }
}
```

---

## Squelette 4 — Rendering Pipeline

Chaque étape du pipeline utilise un `BlockBuilder` avec `position-relative` et un cercle
numéroté en `position-absolute`. La dernière étape utilise `bg-success` et l'icône `bi-check`
à la place du numéro. Dupliquer le bloc d'étape autant que nécessaire.

```razor
@using DMBBootstrapBuilder
@using DMBComponentBuilder
@using {{NAMESPACE}}
@using DMBPageBuilder
@{
    Layout = "_Layout";
}

@using (Html.SectionBuilder().SetHeight(125, UnitSize.px).Begin())
{
    @using (Html.FlexBlockBuilder().SetAlignItemsCenter().SetJustifyContentCenter().Begin())
    {
        @using (Html.BlockBuilder().AddClass("text-center").Begin())
        {
            @Html.TitleBuilder().SetTitle("Rendering Pipeline — {{PACKAGE_NAME}}", TitleLevel.One).Render()
            @using (Html.PB_P().AddClass("lead").Begin())
            {
                @:{{TAGLINE}}
            }
        }
    }
}

@using (Html.ContainerBuilder().SetPadding(SpacingSide.Y, SpacingSize.Five).Begin())
{
    @using (Html.RowBuilder().Begin())
    {
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col8, ResponsiveBreakpoint.Md).Begin())
        {
            @* ── Étape numérotée (répéter pour chaque étape) ── *@
            @using (Html.BlockBuilder().AddClass("position-relative ps-5 border-start border-2 border-primary mb-5").Begin())
            {
                @using (Html.BlockBuilder().AddClass("position-absolute top-0 start-0 translate-middle-x bg-primary text-white rounded-circle d-flex align-items-center justify-content-center").SetStyle("width", "40px").SetStyle("height", "40px").SetStyle("left", "-1px").Begin())
                {
                    @:1
                }
                @Html.TitleBuilder().SetTitle("{{PIPE_1_TITLE}}", TitleLevel.Three).AddClass("h4").Render()
                @using (Html.PB_P().Begin())
                {
                    @:{{PIPE_1_BODY}}
                }
                @(Html.CodeBlock(@"{{PIPE_1_CODE}}", {{PIPE_1_LANG}}))
            }

            @* ── Dernière étape (cercle vert + icône check) ── *@
            @using (Html.BlockBuilder().AddClass("position-relative ps-5 border-start border-2 border-primary mb-5").Begin())
            {
                @using (Html.BlockBuilder().AddClass("position-absolute top-0 start-0 translate-middle-x bg-success text-white rounded-circle d-flex align-items-center justify-content-center").SetStyle("width", "40px").SetStyle("height", "40px").SetStyle("left", "-1px").Begin())
                {
                    @using (Html.PB_Span().AddClass("bi bi-check").Begin()) { }
                }
                @Html.TitleBuilder().SetTitle("{{PIPE_LAST_TITLE}}", TitleLevel.Three).AddClass("h4").Render()
                @using (Html.PB_P().Begin())
                {
                    @:{{PIPE_LAST_BODY}}
                }
                @(Html.CodeBlock(@"{{PIPE_LAST_CODE}}", {{PIPE_LAST_LANG}}))
            }

            @Html.TitleBuilder().SetTitle("{{CHAIN_TITLE}}", TitleLevel.Two).AddClass("mt-5 mb-4").Render()
            @using (Html.PB_P().Begin())
            {
                @:{{CHAIN_DESC}}
            }
            @(Html.CodeBlock(@"{{CHAIN_CODE}}", {{CHAIN_LANG}})
                .SetTitle("{{CHAIN_CODE_TITLE}}")
                .SetCopyButton())
        }

        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AddClass("sticky-top").SetStyle("top", "130px").Begin())
            {
                @using (Html.BlockBuilder().AddClass("border-0 shadow-sm rounded-4 overflow-hidden").Begin())
                {
                    @using (Html.BlockBuilder().AddClass("p-4 text-center").Begin())
                    {
                        @using (Html.PB_Span().AddClass("bi bi-lightning-charge-fill text-warning fs-1").Begin()) { }
                        @Html.TitleBuilder().SetTitle("{{PERF_TITLE}}", TitleLevel.Five).AddClass("mt-3").Render()
                        @using (Html.PB_P().AddClass("small text-muted").Begin())
                        {
                            @:{{PERF_DESC}}
                        }
                    }
                }
                @using (Html.BlockBuilder().AddClass("mt-4 p-4 bg-body-secondary rounded-4 shadow-sm").Begin())
                {
                    @Html.TitleBuilder().SetTitle("Next Step", TitleLevel.Five).Render()
                    @using (Html.PB_P().AddClass("small text-muted").Begin())
                    {
                        @:{{NEXT_STEP_DESC}}
                    }
                    @Html.Button(ActionItemFactory.AspRoute("{{NEXT_STEP_CONTROLLER}}", "{{NEXT_STEP_ACTION}}").SetTitle("{{NEXT_STEP_LABEL}}").SetIcon(IconStruct.Bootstrap("{{NEXT_STEP_ICON}}")).SetVariant(VariantStyle.Primary)).Render()
                }
            }
        }
    }
}
```
