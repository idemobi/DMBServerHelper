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
| `<strong>`             | `Html.PB_Span().AsBold()`                                                      |
| `<em>`                 | `Html.PB_Span().AsItalic()`                                                    |
| `<code>`               | `Html.PB_Span().AsMonospace()`                                                 |
| `<ul>` / `<li>`        | `BlockBuilder` imbriqués avec les helpers Bootstrap équivalents                |

### 2. Règle RZ1006 — blocs `@:` toujours multi-lignes

**Toute** balise `@using` contenant du texte `@:` doit être expansée sur plusieurs lignes.
Un bloc sur une seule ligne provoque l'erreur RZ1006 (Razor ferme le `using` trop tôt).

```razor
@* ❌ INTERDIT — provoque RZ1006 *@
@using (Html.PB_Span().AsBold().Begin()) { @:Texte }

@* ✅ CORRECT *@
@using (Html.PB_Span().AsBold().Begin())
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

### 5. Helpers CSS — utiliser les méthodes sémantiques plutôt que les classes brutes

| Classe Bootstrap brute                        | Helper à utiliser                         |
|-----------------------------------------------|-------------------------------------------|
| `AddClass("text-center")`                     | `AsTextCenter()`                          |
| `AddClass("lead")`                            | `AsLead()`                                |
| `AddClass("fw-bold")`                         | `AsBold()`                                |
| `AddClass("fst-italic")`                      | `AsItalic()`                              |
| `AddClass("font-monospace")`                  | `AsMonospace()`                           |
| `AddClass("font-monospace small")`            | `AsMonospaceSmall()`                      |
| `AddClass("text-muted small mb-0")`           | `AsSmallMuted().AsNoMargin()`             |
| `AddClass("fw-semibold")`                     | `AsSemiBold()`                            |
| `AddClass("mb-0")`                            | `AsNoMargin()`                            |
| `AddClass("text-muted")`                      | `AsMuted()`                               |
| `AddClass("h4")`                              | `AsH4()`                                  |
| `AddClass("alert-heading")`                   | `AsAlertHeading()`                        |
| `AddClass("text-primary")`                    | `AsTextPrimary()`                         |
| `AddClass("text-success")`                    | `AsTextSuccess()`                         |

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

| Placeholder                  | Exemple                                                              |
|------------------------------|----------------------------------------------------------------------|
| `{{NAMESPACE}}`              | `DMBEffectBuilder`                                                   |
| `{{CONTROLLER}}`             | `EffectBuilder`                                                      |
| `{{PACKAGE_NAME}}`           | `EffectBuilder`                                                      |
| `{{TAGLINE}}`                | `Giving life to your components with simple Fluent API extensions.`  |
| `{{DESCRIPTION}}`            | Une ou deux phrases sur le rôle du package.                          |
| `{{HERO_BTN_1_TITLE}}`       | `Get Started`                                                        |
| `{{HERO_BTN_1_ACTION}}`      | `GettingStarted`                                                     |
| `{{HERO_BTN_1_ICON}}`        | `bi-play-fill`                                                       |
| `{{HERO_BTN_2_TITLE}}`       | `Browse Effects`                                                     |
| `{{HERO_BTN_2_CONTROLLER}}`  | `ImageEffect`                                                        |
| `{{HERO_BTN_2_ACTION}}`      | `Index`                                                              |
| `{{HERO_BTN_2_ICON}}`        | `bi-grid`                                                            |
| `{{PILLAR_N_ICON}}`          | `bi-code-slash`                                                      |
| `{{PILLAR_N_COLOR}}`         | `text-primary` / `text-warning` / `text-success`                     |
| `{{PILLAR_N_TITLE}}`         | `Fluent C# API`                                                      |
| `{{PILLAR_N_DESC}}`          | Courte phrase se terminant par le nom du builder concerné.           |
| `{{FAMILIES_HEADING}}`       | `Three Effect Families`                                              |
| `{{FAMILIES_INTRO}}`         | Courte phrase introduisant les familles.                             |
| `{{FAMILY_N_PREVIEW}}`       | Snippet Razor rendant un composant live.                             |
| `{{FAMILY_N_ICON}}`          | `bi-image`                                                           |
| `{{FAMILY_N_COLOR}}`         | `text-primary` / `text-success` / `text-warning`                     |
| `{{FAMILY_N_TITLE}}`         | `Image Effects`                                                      |
| `{{FAMILY_N_DESC}}`          | Courte phrase décrivant la famille.                                  |
| `{{FAMILY_N_CODE}}`          | `.GrayscaleHoverEffect()`                                            |
| `{{PHILOSOPHY_TITLE}}`       | `Design for intent, not implementation`                              |
| `{{PHILOSOPHY_TEXT}}`        | Une ou deux phrases sur l'intention de design.                       |
| `{{NEXT_STEP_TITLE}}`        | `Next Step`                                                          |
| `{{NEXT_STEP_DESC}}`         | Une phrase décrivant la page suivante.                               |
| `{{NEXT_STEP_LABEL}}`        | `Getting Started`                                                    |
| `{{NEXT_STEP_ACTION}}`       | `GettingStarted`                                                     |
| `{{NEXT_STEP_ICON}}`         | `bi-play-fill`                                                       |
| `{{STEP_N_TITLE}}`           | `Install the NuGet Package`                                          |
| `{{STEP_N_DESC}}`            | Paragraphe avant le bloc de code.                                    |
| `{{STEP_N_CODE}}`            | Contenu du code block.                                               |
| `{{STEP_N_LANG}}`            | `CodeLanguage.Bash` ou `CodeLanguage.CSharp`                         |
| `{{STEP_N_CODE_TITLE}}`      | Titre affiché au-dessus du code block.                               |
| `{{QUICKREF_TITLE}}`         | `Common Effects`                                                     |
| `{{QUICKREF_ITEM_N}}`        | `.GrayscaleHoverEffect()`                                            |
| `{{CATALOG_CONTROLLER}}`     | `ImageEffect`                                                        |
| `{{CATALOG_ACTION}}`         | `Index`                                                              |
| `{{CATALOG_LABEL}}`          | `See Full Catalog`                                                   |
| `{{CATALOG_ICON}}`           | `bi-collection`                                                      |
| `{{ARCH_N_TITLE}}`           | `Component Separation`                                               |
| `{{ARCH_N_BODY}}`            | Paragraphe expliquant le concept.                                    |
| `{{ARCH_N_CODE}}`            | Contenu du code block illustrant le concept.                         |
| `{{ARCH_N_LANG}}`            | `CodeLanguage.CSharp` ou `CodeLanguage.Css`                          |
| `{{ARCH_N_CODE_TITLE}}`      | Titre du code block.                                                 |
| `{{TECH_KEY_N}}`             | `Backend`                                                            |
| `{{TECH_VAL_N}}`             | `.NET 10 / ASP.NET Core`                                             |
| `{{PIPE_N_TITLE}}`           | `Fluent API Call`                                                    |
| `{{PIPE_N_BODY}}`            | Paragraphe expliquant cette étape du pipeline.                       |
| `{{PIPE_N_CODE}}`            | Contenu du code block de l'étape.                                    |
| `{{PIPE_N_LANG}}`            | `CodeLanguage.CSharp`                                                |
| `{{CHAIN_TITLE}}`            | `Chaining Multiple Effects`                                          |
| `{{CHAIN_DESC}}`             | Courte phrase sur le chaînage.                                       |
| `{{CHAIN_CODE}}`             | Exemple de chaînage.                                                 |
| `{{CHAIN_LANG}}`             | `CodeLanguage.CSharp`                                                |
| `{{CHAIN_CODE_TITLE}}`       | Titre du code block.                                                 |
| `{{PERF_TITLE}}`             | `Optimized for Speed`                                                |
| `{{PERF_DESC}}`              | Courte phrase sur les performances.                                  |

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

@* ── Hero ── *@
@using (Html.SectionBuilder().SetHeight(125, UnitSize.px).BootstrapBackgroundEffect(VariantStyle.Normal).Begin())
{
    @using (Html.FlexBlockBuilder().SetAlignItemsCenter().SetJustifyContentCenter().Begin())
    {
        @using (Html.BlockBuilder().AsTextCenter().Begin())
        {
            @Html.TitleBuilder().SetTitle("Introduction — {{PACKAGE_NAME}}", TitleLevel.One).Render()
            @using (Html.PB_P().AsLead().Begin())
            {
                @:{{TAGLINE}}
            }
            @using (Html.FlexBlockBuilder().SetJustifyContentCenter().WithGap(Old_Gap.Gap2).Begin())
            {
                @Html.Button(ActionItemFactory.AspRoute("{{CONTROLLER}}", "{{HERO_BTN_1_ACTION}}").SetTitle("{{HERO_BTN_1_TITLE}}").SetIcon(IconStruct.Bootstrap("{{HERO_BTN_1_ICON}}")).SetVariant(VariantStyle.Primary)).Render()
                @Html.Button(ActionItemFactory.AspRoute("{{HERO_BTN_2_CONTROLLER}}", "{{HERO_BTN_2_ACTION}}").SetTitle("{{HERO_BTN_2_TITLE}}").SetIcon(IconStruct.Bootstrap("{{HERO_BTN_2_ICON}}")).SetVariant(VariantStyle.Secondary).SetOutlined()).Render()
            }
        }
    }
}

@* ── 3 piliers visuels ── *@
@using (Html.BlockBuilder().AsSectionContentTop().Begin())
{
    @using (Html.RowBuilder().AsPillarGrid().Begin())
    {
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AsSidebarCard().Begin())
            {
                @using (Html.BlockBuilder().AsPaddedBlock().Begin())
                {
                    @using (Html.PB_Span().AddClass("bi {{PILLAR_1_ICON}} fs-2 text-primary d-block mb-2").Begin()) { }
                    @using (Html.PB_Span().AsSemiBold().Begin())
                    {
                        @:{{PILLAR_1_TITLE}}
                    }
                    @using (Html.PB_P().AsSmallMuted().AsNoMargin().Begin())
                    {
                        @:{{PILLAR_1_DESC}}
                    }
                }
            }
        }
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AsSidebarCard().Begin())
            {
                @using (Html.BlockBuilder().AsPaddedBlock().Begin())
                {
                    @using (Html.PB_Span().AddClass("bi {{PILLAR_2_ICON}} fs-2 text-warning d-block mb-2").Begin()) { }
                    @using (Html.PB_Span().AsSemiBold().Begin())
                    {
                        @:{{PILLAR_2_TITLE}}
                    }
                    @using (Html.PB_P().AsSmallMuted().AsNoMargin().Begin())
                    {
                        @:{{PILLAR_2_DESC}}
                    }
                }
            }
        }
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AsSidebarCard().Begin())
            {
                @using (Html.BlockBuilder().AsPaddedBlock().Begin())
                {
                    @using (Html.PB_Span().AddClass("bi {{PILLAR_3_ICON}} fs-2 text-success d-block mb-2").Begin()) { }
                    @using (Html.PB_Span().AsSemiBold().Begin())
                    {
                        @:{{PILLAR_3_TITLE}}
                    }
                    @using (Html.PB_P().AsSmallMuted().AsNoMargin().Begin())
                    {
                        @:{{PILLAR_3_DESC}}
                    }
                }
            }
        }
    }
}

@* ── What is + aperçu live ── *@
@using (Html.BlockBuilder().AsSectionContent().Begin())
{
    @using (Html.RowBuilder().SetGapY(Gap.G5).SetAlignItems(AlignItems.Center).Begin())
    {
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col7, ResponsiveBreakpoint.Lg).Begin())
        {
            @Html.TitleBuilder().SetTitle("What is {{PACKAGE_NAME}}?", TitleLevel.Two).AsSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{DESCRIPTION}}
            }
            @using (Html.BlockBuilder().AsBenefitRow().Begin())
            {
                @using (Html.PB_Span().AddClass("bi bi-check-circle-fill text-success me-3 mt-1").Begin()) { }
                @using (Html.BlockBuilder().Begin())
                {
                    @:{{BENEFIT_1_DESC}}
                }
            }
            @using (Html.BlockBuilder().AsBenefitRow().Begin())
            {
                @using (Html.PB_Span().AddClass("bi bi-check-circle-fill text-success me-3 mt-1").Begin()) { }
                @using (Html.BlockBuilder().Begin())
                {
                    @:{{BENEFIT_2_DESC}}
                }
            }
            @using (Html.BlockBuilder().AsBenefitRow().Begin())
            {
                @using (Html.PB_Span().AddClass("bi bi-check-circle-fill text-success me-3 mt-1").Begin()) { }
                @using (Html.BlockBuilder().Begin())
                {
                    @:{{BENEFIT_3_DESC}}
                }
            }
        }
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col5, ResponsiveBreakpoint.Lg).Begin())
        {
            @using (Html.BlockBuilder().AsSidebarCard().Begin())
            {
                @using (Html.BlockBuilder().AsPreviewWrapper().Begin())
                {
                    {{PREVIEW_COMPONENT}}
                }
                @using (Html.BlockBuilder().AsPaddedBlock().Begin())
                {
                    @(Html.CodeBlock(@"{{PREVIEW_CODE}}", CodeLanguage.CSharp))
                    @using (Html.PB_P().AsSmallMuted().AsNoMargin().Begin())
                    {
                        @:{{PREVIEW_HINT}}
                    }
                }
            }
        }
    }
}

@* ── Trois familles ── *@
@using (Html.BlockBuilder().AsSectionContent().Begin())
{
    @using (Html.BlockBuilder().AsTextCenter().Begin())
    {
        @Html.TitleBuilder().SetTitle("{{FAMILIES_HEADING}}", TitleLevel.Two).AsSectionHeading().Render()
        @using (Html.PB_P().AsMuted().Begin())
        {
            @:{{FAMILIES_INTRO}}
        }
    }
    @using (Html.RowBuilder().AsPillarGrid().Begin())
    {
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AsPillarCard().Begin())
            {
                @using (Html.BlockBuilder().AsPreviewWrapper().Begin())
                {
                    {{FAMILY_1_PREVIEW}}
                }
                @using (Html.BlockBuilder().AsTopMargin().Begin())
                {
                    @using (Html.PB_Span().AddClass("bi {{FAMILY_1_ICON}} {{FAMILY_1_COLOR}} me-1").Begin()) { }
                    @Html.TitleBuilder().SetTitle("{{FAMILY_1_TITLE}}", TitleLevel.Five).AsCompact().Render()
                }
                @using (Html.PB_P().AsSmallMuted().Begin())
                {
                    @:{{FAMILY_1_DESC}}
                }
                @using (Html.PB_Span().AsMonospaceSmall().Begin())
                {
                    @:{{FAMILY_1_CODE}}
                }
            }
        }
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AsPillarCard().Begin())
            {
                @using (Html.BlockBuilder().AsPreviewWrapper().Begin())
                {
                    {{FAMILY_2_PREVIEW}}
                }
                @using (Html.BlockBuilder().AsTopMargin().Begin())
                {
                    @using (Html.PB_Span().AddClass("bi {{FAMILY_2_ICON}} {{FAMILY_2_COLOR}} me-1").Begin()) { }
                    @Html.TitleBuilder().SetTitle("{{FAMILY_2_TITLE}}", TitleLevel.Five).AsCompact().Render()
                }
                @using (Html.PB_P().AsSmallMuted().Begin())
                {
                    @:{{FAMILY_2_DESC}}
                }
                @using (Html.PB_Span().AsMonospaceSmall().Begin())
                {
                    @:{{FAMILY_2_CODE}}
                }
            }
        }
        @using (Html.ColBuilder().SetCol(ColSize.Col12).SetCol(ColSize.Col4, ResponsiveBreakpoint.Md).Begin())
        {
            @using (Html.BlockBuilder().AsPillarCard().Begin())
            {
                @using (Html.BlockBuilder().AsPreviewWrapper().Begin())
                {
                    {{FAMILY_3_PREVIEW}}
                }
                @using (Html.BlockBuilder().AsTopMargin().Begin())
                {
                    @using (Html.PB_Span().AddClass("bi {{FAMILY_3_ICON}} {{FAMILY_3_COLOR}} me-1").Begin()) { }
                    @Html.TitleBuilder().SetTitle("{{FAMILY_3_TITLE}}", TitleLevel.Five).AsCompact().Render()
                }
                @using (Html.PB_P().AsSmallMuted().Begin())
                {
                    @:{{FAMILY_3_DESC}}
                }
                @using (Html.PB_Span().AsMonospaceSmall().Begin())
                {
                    @:{{FAMILY_3_CODE}}
                }
            }
        }
    }
}

@* ── Philosophy + Next Step ── *@
@using (Html.BlockBuilder().AsSectionContent().Begin())
{
    @using (Html.FlexBlockBuilder().WithGap(Old_Gap.Gap3).SetAlignItemsStretch().Begin())
    {
        @using (Html.BlockBuilder().AsInfoCallout().SetStyle("margin", "0").SetStyle("flex", "2").Begin())
        {
            @using (Html.FlexBlockBuilder().Begin())
            {
                @using (Html.BlockBuilder().AsCalloutIcon().Begin())
                {
                    @using (Html.PB_Span().AddClass("bi bi-lightbulb-fill fs-2 text-info").Begin()) { }
                }
                @using (Html.BlockBuilder().Begin())
                {
                    @Html.TitleBuilder().SetTitle("{{PHILOSOPHY_TITLE}}", TitleLevel.Four).AsAlertHeading().Render()
                    @using (Html.PB_P().AsNoMargin().Begin())
                    {
                        @:{{PHILOSOPHY_TEXT}}
                    }
                }
            }
        }
        @using (Html.BlockBuilder().AsSidebarCard().SetStyle("flex", "1").Begin())
        {
            @using (Html.BlockBuilder().AsPaddedBlock().SetStyle("height", "100%").Begin())
            {
                @using (Html.FlexBlockBuilder().SetFlexColumn().SetAlignItemsStart().SetJustifyContentCenter().SetStyle("height", "100%").Begin())
                {
                    @Html.TitleBuilder().SetTitle("{{NEXT_STEP_TITLE}}", TitleLevel.Five).Render()
                    @using (Html.PB_P().AsSmallMuted().Begin())
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

@* ── Hero ── *@
@using (Html.SectionBuilder().SetHeight(125, UnitSize.px).BootstrapBackgroundEffect(VariantStyle.Normal).Begin())
{
    @using (Html.FlexBlockBuilder().SetAlignItemsCenter().SetJustifyContentCenter().Begin())
    {
        @using (Html.BlockBuilder().AsTextCenter().Begin())
        {
            @Html.TitleBuilder().SetTitle("Getting Started — {{PACKAGE_NAME}}", TitleLevel.One).Render()
            @using (Html.PB_P().AsLead().Begin())
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
        @using (Html.ColBuilder().SetCol(ColSize.Col12).Begin())
        {
            @* ── Step 1 ── *@
            @Html.TitleBuilder().SetTitle("1. {{STEP_1_TITLE}}", TitleLevel.Two).AsSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{STEP_1_DESC}}
            }
            @(Html.CodeBlock(@"{{STEP_1_CODE}}", {{STEP_1_LANG}})
                .SetTitle("{{STEP_1_CODE_TITLE}}")
                .SetCopyButton())

            @* ── Step 2 ── *@
            @Html.TitleBuilder().SetTitle("2. {{STEP_2_TITLE}}", TitleLevel.Two).AsSubSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{STEP_2_DESC}}
            }
            @(Html.CodeBlock(@"{{STEP_2_CODE}}", {{STEP_2_LANG}})
                .SetTitle("{{STEP_2_CODE_TITLE}}")
                .SetCopyButton())

            @* ── Step 3 ── *@
            @Html.TitleBuilder().SetTitle("3. {{STEP_3_TITLE}}", TitleLevel.Two).AsSubSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{STEP_3_DESC}}
            }
            @using (Html.RowBuilder().AsEffectGrid().Begin())
            {
                @using (Html.ColBuilder().SetCol(ColSize.Col6, ResponsiveBreakpoint.Md).Begin())
                {
                    @using (Html.BlockBuilder().AsEffectTeaserCard().Begin())
                    {
                        @Html.TitleBuilder().SetTitle("{{TEASER_1_TITLE}}", TitleLevel.Six).AsTextPrimary().Render()
                        @(Html.CodeBlock(@"{{TEASER_1_CODE}}", CodeLanguage.CSharp))
                    }
                }
                @using (Html.ColBuilder().SetCol(ColSize.Col6, ResponsiveBreakpoint.Md).Begin())
                {
                    @using (Html.BlockBuilder().AsEffectTeaserCard().Begin())
                    {
                        @Html.TitleBuilder().SetTitle("{{TEASER_2_TITLE}}", TitleLevel.Six).AsTextSuccess().Render()
                        @(Html.CodeBlock(@"{{TEASER_2_CODE}}", CodeLanguage.CSharp))
                    }
                }
            }

            @* ── Step 4 ── *@
            @Html.TitleBuilder().SetTitle("4. {{STEP_4_TITLE}}", TitleLevel.Two).AsSubSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{STEP_4_DESC}}
            }
            @(Html.CodeBlock(@"{{STEP_4_CODE}}", {{STEP_4_LANG}})
                .SetTitle("{{STEP_4_CODE_TITLE}}")
                .SetCopyButton())

            @* ── Quick reference ── *@
            @using (Html.BlockBuilder().AsInfoCallout().Begin())
            {
                @using (Html.FlexBlockBuilder().Begin())
                {
                    @using (Html.BlockBuilder().AsCalloutIcon().Begin())
                    {
                        @using (Html.PB_Span().AddClass("bi bi-collection-fill fs-2 text-info").Begin()) { }
                    }
                    @using (Html.BlockBuilder().Begin())
                    {
                        @Html.TitleBuilder().SetTitle("{{QUICKREF_TITLE}}", TitleLevel.Five).AsAlertHeading().Render()
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsMonospaceSmall().Begin())
                            {
                                @:{{QUICKREF_ITEM_1}}
                            }
                        }
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsMonospaceSmall().Begin())
                            {
                                @:{{QUICKREF_ITEM_2}}
                            }
                        }
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsMonospaceSmall().Begin())
                            {
                                @:{{QUICKREF_ITEM_3}}
                            }
                        }
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsMonospaceSmall().Begin())
                            {
                                @:{{QUICKREF_ITEM_4}}
                            }
                        }
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsMonospaceSmall().Begin())
                            {
                                @:{{QUICKREF_ITEM_5}}
                            }
                        }
                        @using (Html.BlockBuilder().AsTopMargin().Begin())
                        {
                            @Html.Button(ActionItemFactory.AspRoute("{{CATALOG_CONTROLLER}}", "{{CATALOG_ACTION}}").SetTitle("{{CATALOG_LABEL}}").SetIcon(IconStruct.Bootstrap("{{CATALOG_ICON}}")).SetVariant(VariantStyle.Secondary).SetOutlined()).Render()
                        }
                    }
                }
            }

            @* ── Next Step ── *@
            @using (Html.BlockBuilder().AsSidebarNote().Begin())
            {
                @using (Html.BlockBuilder().AsTextCenter().Begin())
                {
                    @Html.TitleBuilder().SetTitle("{{NEXT_STEP_TITLE}}", TitleLevel.Four).Render()
                    @using (Html.PB_P().AsSmallMuted().Begin())
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

@* ── Hero ── *@
@using (Html.SectionBuilder().SetHeight(125, UnitSize.px).BootstrapBackgroundEffect(VariantStyle.Normal).Begin())
{
    @using (Html.FlexBlockBuilder().SetAlignItemsCenter().SetJustifyContentCenter().Begin())
    {
        @using (Html.BlockBuilder().AsTextCenter().Begin())
        {
            @Html.TitleBuilder().SetTitle("Architecture — {{PACKAGE_NAME}}", TitleLevel.One).Render()
            @using (Html.PB_P().AsLead().Begin())
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
        @using (Html.ColBuilder().SetCol(ColSize.Col12).Begin())
        {
            @* ── Section 1 ── *@
            @Html.TitleBuilder().SetTitle("1. {{ARCH_1_TITLE}}", TitleLevel.Two).AsSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{ARCH_1_BODY}}
            }
            @(Html.CodeBlock(@"{{ARCH_1_CODE}}", {{ARCH_1_LANG}})
                .SetTitle("{{ARCH_1_CODE_TITLE}}")
                .SetCopyButton())

            @* ── Section 2 ── *@
            @Html.TitleBuilder().SetTitle("2. {{ARCH_2_TITLE}}", TitleLevel.Two).AsSubSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{ARCH_2_BODY}}
            }
            @(Html.CodeBlock(@"{{ARCH_2_CODE}}", {{ARCH_2_LANG}})
                .SetTitle("{{ARCH_2_CODE_TITLE}}")
                .SetCopyButton())

            @* ── Section 3 ── *@
            @Html.TitleBuilder().SetTitle("3. {{ARCH_3_TITLE}}", TitleLevel.Two).AsSubSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{ARCH_3_BODY}}
            }
            @(Html.CodeBlock(@"{{ARCH_3_CODE}}", {{ARCH_3_LANG}})
                .SetTitle("{{ARCH_3_CODE_TITLE}}")
                .SetCopyButton())

            @* ── Tech Stack ── *@
            @using (Html.BlockBuilder().AsInfoCallout().Begin())
            {
                @using (Html.FlexBlockBuilder().Begin())
                {
                    @using (Html.BlockBuilder().AsCalloutIcon().Begin())
                    {
                        @using (Html.PB_Span().AddClass("bi bi-gear-fill fs-2 text-info").Begin()) { }
                    }
                    @using (Html.BlockBuilder().Begin())
                    {
                        @Html.TitleBuilder().SetTitle("Tech Stack", TitleLevel.Five).AsAlertHeading().Render()
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsBold().Begin())
                            {
                                @:{{TECH_KEY_1}}:
                            }
                            @: {{TECH_VAL_1}}
                        }
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsBold().Begin())
                            {
                                @:{{TECH_KEY_2}}:
                            }
                            @: {{TECH_VAL_2}}
                        }
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsBold().Begin())
                            {
                                @:{{TECH_KEY_3}}:
                            }
                            @: {{TECH_VAL_3}}
                        }
                        @using (Html.BlockBuilder().AsSidebarRow().Begin())
                        {
                            @using (Html.PB_Span().AsBold().Begin())
                            {
                                @:{{TECH_KEY_4}}:
                            }
                            @: {{TECH_VAL_4}}
                        }
                    }
                }
            }

            @* ── Next Step ── *@
            @using (Html.BlockBuilder().AsSidebarNote().Begin())
            {
                @using (Html.BlockBuilder().AsTextCenter().Begin())
                {
                    @Html.TitleBuilder().SetTitle("{{NEXT_STEP_TITLE}}", TitleLevel.Four).Render()
                    @using (Html.PB_P().AsSmallMuted().Begin())
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

Les étapes numérotées utilisent `AsTimelineStep()` + `AsTimelineStepBadge()`.
La dernière étape passe `"bg-success"` à `AsTimelineStepBadge()` et affiche une icône `bi-check` au lieu d'un chiffre.

```razor
@using DMBBootstrapBuilder
@using DMBComponentBuilder
@using {{NAMESPACE}}
@using DMBPageBuilder
@{
    Layout = "_Layout";
}

@* ── Hero ── *@
@using (Html.SectionBuilder().SetHeight(125, UnitSize.px).BootstrapBackgroundEffect(VariantStyle.Normal).Begin())
{
    @using (Html.FlexBlockBuilder().SetAlignItemsCenter().SetJustifyContentCenter().Begin())
    {
        @using (Html.BlockBuilder().AsTextCenter().Begin())
        {
            @Html.TitleBuilder().SetTitle("Rendering Pipeline — {{PACKAGE_NAME}}", TitleLevel.One).Render()
            @using (Html.PB_P().AsLead().Begin())
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
        @using (Html.ColBuilder().SetCol(ColSize.Col12).Begin())
        {
            @* ── Étape numérotée (répéter pour chaque étape intermédiaire) ── *@
            @using (Html.BlockBuilder().AsTimelineStep().Begin())
            {
                @using (Html.BlockBuilder().AsTimelineStepBadge().Begin())
                {
                    @:1
                }
                @Html.TitleBuilder().SetTitle("{{PIPE_1_TITLE}}", TitleLevel.Three).AsH4().Render()
                @using (Html.PB_P().Begin())
                {
                    @:{{PIPE_1_BODY}}
                }
                @(Html.CodeBlock(@"{{PIPE_1_CODE}}", {{PIPE_1_LANG}}))
            }

            @* ── Dernière étape (badge vert + icône check) ── *@
            @using (Html.BlockBuilder().AsTimelineStep().Begin())
            {
                @using (Html.BlockBuilder().AsTimelineStepBadge("bg-success").Begin())
                {
                    @using (Html.PB_Span().AddClass("bi bi-check").Begin()) { }
                }
                @Html.TitleBuilder().SetTitle("{{PIPE_LAST_TITLE}}", TitleLevel.Three).AsH4().Render()
                @using (Html.PB_P().Begin())
                {
                    @:{{PIPE_LAST_BODY}}
                }
                @(Html.CodeBlock(@"{{PIPE_LAST_CODE}}", {{PIPE_LAST_LANG}}))
            }

            @* ── Chaining ── *@
            @Html.TitleBuilder().SetTitle("{{CHAIN_TITLE}}", TitleLevel.Two).AsSubSectionHeading().Render()
            @using (Html.PB_P().Begin())
            {
                @:{{CHAIN_DESC}}
            }
            @(Html.CodeBlock(@"{{CHAIN_CODE}}", {{CHAIN_LANG}})
                .SetTitle("{{CHAIN_CODE_TITLE}}")
                .SetCopyButton())

            @* ── Performance highlight ── *@
            @using (Html.BlockBuilder().AsInfoCallout().Begin())
            {
                @using (Html.FlexBlockBuilder().Begin())
                {
                    @using (Html.BlockBuilder().AsCalloutIcon().Begin())
                    {
                        @using (Html.PB_Span().AddClass("bi bi-lightning-charge-fill fs-2 text-warning").Begin()) { }
                    }
                    @using (Html.BlockBuilder().Begin())
                    {
                        @Html.TitleBuilder().SetTitle("{{PERF_TITLE}}", TitleLevel.Five).AsAlertHeading().Render()
                        @using (Html.PB_P().AsNoMargin().Begin())
                        {
                            @:{{PERF_DESC}}
                        }
                    }
                }
            }

            @* ── Next Step ── *@
            @using (Html.BlockBuilder().AsSidebarNote().Begin())
            {
                @using (Html.BlockBuilder().AsTextCenter().Begin())
                {
                    @Html.TitleBuilder().SetTitle("{{NEXT_STEP_TITLE}}", TitleLevel.Four).Render()
                    @using (Html.PB_P().AsSmallMuted().Begin())
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
