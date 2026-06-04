#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBComponentBuilder;
using DMBPageBuilder;

#endregion

namespace DMBServerHelperLabs
{
    /// <summary>
    ///     Provides layout and text extension methods used by the DMBServerHelper labs views.
    /// </summary>
    public static class DMBServerHelperLabsViewExtensions
    {
        #region Static methods

        /// <summary>
        ///     Adds centered text alignment to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsTextCenter(this BlockBuilder builder)
            => builder.AddClass("text-center");

        /// <summary>
        ///     Adds standard section content spacing to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSectionContent(this BlockBuilder builder)
            => builder.AddClass("container-lg px-4 pb-5");

        /// <summary>
        ///     Adds standard section content spacing with top padding to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSectionContentTop(this BlockBuilder builder)
            => builder.AddClass("container-lg px-4 pt-4 pb-5");

        /// <summary>
        ///     Adds the labs sidebar card visual style to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSidebarCard(this BlockBuilder builder)
            => builder.AddClass("border-0 shadow-sm rounded-4 overflow-hidden");

        /// <summary>
        ///     Adds the labs sidebar note visual style to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSidebarNote(this BlockBuilder builder)
            => builder.AddClass("mt-4 p-4 bg-body-secondary rounded-4 shadow-sm");

        /// <summary>
        ///     Adds the labs sidebar row visual style to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsSidebarRow(this BlockBuilder builder)
            => builder.AddClass("border-top py-2");

        /// <summary>
        ///     Adds the labs pillar card visual style to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsPillarCard(this BlockBuilder builder)
            => builder.AddClass("p-3 border rounded-4 h-100 shadow-sm");

        /// <summary>
        ///     Adds the labs benefit row layout to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsBenefitRow(this BlockBuilder builder)
            => builder.AddClass("mb-3 d-flex align-items-start");

        /// <summary>
        ///     Adds the labs info callout visual style to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsInfoCallout(this BlockBuilder builder)
            => builder.AddClass("alert alert-info border-0 shadow-sm rounded-4 p-4 my-4");

        /// <summary>
        ///     Adds the labs callout icon spacing to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsCalloutIcon(this BlockBuilder builder)
            => builder.AddClass("me-3");

        /// <summary>
        ///     Adds top margin to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsTopMargin(this BlockBuilder builder)
            => builder.AddClass("mt-3");

        /// <summary>
        ///     Adds the labs preview wrapper visual style to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsPreviewWrapper(this BlockBuilder builder)
            => builder.AddClass("rounded-3 overflow-hidden");

        /// <summary>
        ///     Adds standard padding to a block.
        /// </summary>
        /// <param name="builder">The block builder to configure.</param>
        /// <returns>The configured <see cref="BlockBuilder"/>.</returns>
        public static BlockBuilder AsPaddedBlock(this BlockBuilder builder)
            => builder.AddClass("p-4");

        /// <summary>
        ///     Adds labs pillar grid spacing to a row.
        /// </summary>
        /// <param name="builder">The row builder to configure.</param>
        /// <returns>The configured <see cref="RowBuilder"/>.</returns>
        public static RowBuilder AsPillarGrid(this RowBuilder builder)
            => builder.AddClass("g-4 mt-2");

        /// <summary>
        ///     Adds lead paragraph styling.
        /// </summary>
        /// <param name="builder">The paragraph builder to configure.</param>
        /// <returns>The configured <see cref="PB_ParagraphBuilder"/>.</returns>
        public static PB_ParagraphBuilder AsLead(this PB_ParagraphBuilder builder)
            => builder.AddClass("lead");

        /// <summary>
        ///     Adds muted paragraph styling.
        /// </summary>
        /// <param name="builder">The paragraph builder to configure.</param>
        /// <returns>The configured <see cref="PB_ParagraphBuilder"/>.</returns>
        public static PB_ParagraphBuilder AsMuted(this PB_ParagraphBuilder builder)
            => builder.AddClass("text-muted");

        /// <summary>
        ///     Adds small muted paragraph styling.
        /// </summary>
        /// <param name="builder">The paragraph builder to configure.</param>
        /// <returns>The configured <see cref="PB_ParagraphBuilder"/>.</returns>
        public static PB_ParagraphBuilder AsSmallMuted(this PB_ParagraphBuilder builder)
            => builder.AddClass("small text-muted");

        /// <summary>
        ///     Removes paragraph bottom margin.
        /// </summary>
        /// <param name="builder">The paragraph builder to configure.</param>
        /// <returns>The configured <see cref="PB_ParagraphBuilder"/>.</returns>
        public static PB_ParagraphBuilder AsNoMargin(this PB_ParagraphBuilder builder)
            => builder.AddClass("mb-0");

        /// <summary>
        ///     Adds section heading spacing to a title.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsSectionHeading(this TitleBuilder builder)
            => builder.AddClass("mb-4");

        /// <summary>
        ///     Adds subsection heading spacing to a title.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsSubSectionHeading(this TitleBuilder builder)
            => builder.AddClass("mb-4 mt-5");

        /// <summary>
        ///     Removes title bottom margin.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsCompact(this TitleBuilder builder)
            => builder.AddClass("mb-0");

        /// <summary>
        ///     Adds Bootstrap alert heading styling to a title.
        /// </summary>
        /// <param name="builder">The title builder to configure.</param>
        /// <returns>The configured <see cref="TitleBuilder"/>.</returns>
        public static TitleBuilder AsAlertHeading(this TitleBuilder builder)
            => builder.AddClass("alert-heading");

        /// <summary>
        ///     Adds monospace styling to a span.
        /// </summary>
        /// <param name="builder">The span builder to configure.</param>
        /// <returns>The configured <see cref="PB_SpanBuilder"/>.</returns>
        public static PB_SpanBuilder AsMonospace(this PB_SpanBuilder builder)
            => builder.AddClass("font-monospace");

        /// <summary>
        ///     Adds small monospace styling to a span.
        /// </summary>
        /// <param name="builder">The span builder to configure.</param>
        /// <returns>The configured <see cref="PB_SpanBuilder"/>.</returns>
        public static PB_SpanBuilder AsMonospaceSmall(this PB_SpanBuilder builder)
            => builder.AddClass("font-monospace small");

        /// <summary>
        ///     Adds bold styling to a span.
        /// </summary>
        /// <param name="builder">The span builder to configure.</param>
        /// <returns>The configured <see cref="PB_SpanBuilder"/>.</returns>
        public static PB_SpanBuilder AsBold(this PB_SpanBuilder builder)
            => builder.AddClass("fw-bold");

        /// <summary>
        ///     Adds semibold styling to a span.
        /// </summary>
        /// <param name="builder">The span builder to configure.</param>
        /// <returns>The configured <see cref="PB_SpanBuilder"/>.</returns>
        public static PB_SpanBuilder AsSemiBold(this PB_SpanBuilder builder)
            => builder.AddClass("fw-semibold");

        #endregion
    }
}
