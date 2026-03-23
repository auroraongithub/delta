// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Screens.Edit.Components.Timelines.Summary.Visualisations;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.Components.Timelines.Summary.Parts
{
    /// <summary>
    /// Displays section gimmick boundaries on the summary timeline (playback seeker area).
    /// </summary>
    public partial class SectionGimmickBoundaryPart : TimelinePart
    {
        private string? lastSignature;

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        protected override void Update()
        {
            base.Update();

            string signature = buildSignature();
            if (signature == lastSignature)
                return;

            lastSignature = signature;
            recreate();
        }

        private void recreate()
        {
            Clear();

            var sections = EditorBeatmap.SectionGimmicks.Sections;
            if (sections.Count == 0)
                return;

            var usedEnds = new HashSet<double>();

            foreach (var section in sections.OrderBy(s => s.StartTime))
            {
                addLine(section.StartTime, colours.Yellow, 1f, PointVisualisation.MAX_WIDTH + 1);

                if (section.EndTime >= 0 && usedEnds.Add(section.EndTime))
                    addLine(section.EndTime, colours.Orange1, 0.9f, PointVisualisation.MAX_WIDTH);
            }
        }

        private void addLine(double time, Color4 colour, float alpha, float width)
        {
            Add(new Container
            {
                RelativePositionAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                X = (float)time,
                Width = 1,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = width + 2,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Color4.Black,
                        Alpha = 0.55f,
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = width,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = colour,
                        Alpha = alpha,
                    },
                }
            });
        }

        private string buildSignature()
        {
            var sections = EditorBeatmap.SectionGimmicks.Sections;

            if (sections.Count == 0)
                return string.Empty;

            return string.Join("|", sections.OrderBy(s => s.StartTime)
                                              .Select(s => $"{s.Id}:{s.StartTime:F3}:{s.EndTime:F3}"));
        }
    }
}
