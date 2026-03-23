// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Localisation.SkinComponents;

namespace osu.Game.Skinning.Components
{
    [UsedImplicitly]
    public partial class TextElement : FontAdjustableSkinComponent
    {
        [SettingSource(typeof(SkinnableComponentStrings), nameof(SkinnableComponentStrings.TextElementText))]
        public Bindable<string> Text { get; } = new Bindable<string>("Circles!");

        private readonly OsuSpriteText text;

        public TextElement()
        {
            AutoSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                text = new OsuSpriteText
                {
                    // Keep internal text top-left so component anchor/origin behaviour is intuitive
                    // and not biased toward centre.
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    AllowMultiline = true,
                    Font = OsuFont.Default.With(size: 40)
                }
            };

            // Settings text boxes are single-line, so users commonly type "\n" to represent new lines.
            // Convert escaped line breaks to actual new lines for preview/output.
            Text.BindValueChanged(v => text.Current.Value = (v.NewValue ?? string.Empty).Replace("\\n", "\n"), true);
        }

        protected override void SetFont(FontUsage font) => text.Font = font.With(size: 40);

        protected override void SetTextColour(Colour4 textColour) => text.Colour = textColour;
    }
}
