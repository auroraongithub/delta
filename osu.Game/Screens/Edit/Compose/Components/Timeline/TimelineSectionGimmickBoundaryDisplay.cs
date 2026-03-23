// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Specialized;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.Edit.Components.Timelines.Summary.Parts;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.Compose.Components.Timeline
{
    /// <summary>
    /// Draws section boundary indicators across the full timeline height,
    /// so section starts/ends are visible in both song and hitobject timeline lanes.
    /// </summary>
    public partial class TimelineSectionGimmickBoundaryDisplay : TimelinePart<TimelineSectionGimmickBoundaryDisplay.SectionBoundaryMarker>
    {
        private readonly Cached displayCache = new Cached();

        private readonly BindableList<SectionGimmickSection> sections = new BindableList<SectionGimmickSection>();

        [Resolved(canBeNull: true)]
        private SectionGimmickEditorModel? editorModel { get; set; }

        protected override void LoadBeatmap(EditorBeatmap beatmap)
        {
            base.LoadBeatmap(beatmap);

            if (editorModel == null)
                return;

            sections.UnbindAll();
            sections.BindTo(editorModel.Sections);
            sections.BindCollectionChanged((_, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Replace)
                    displayCache.Invalidate();
            });

            editorModel.SelectedSectionId.BindValueChanged(_ => updateSelectionState());

            displayCache.Invalidate();
        }

        protected override void Update()
        {
            base.Update();

            if (!displayCache.IsValid)
            {
                recreateDisplay();
                displayCache.Validate();
            }
        }

        private void recreateDisplay()
        {
            Clear();

            if (editorModel == null)
                return;

            foreach (var section in sections.OrderBy(s => s.StartTime))
            {
                Add(new SectionBoundaryMarker(section)
                {
                    OnSelected = id => editorModel.SelectedSectionId.Value = id,
                });
            }

            updateSelectionState();
        }

        private void updateSelectionState()
        {
            if (editorModel == null)
                return;

            foreach (var marker in AliveChildren)
                marker.SetSelected(marker.Section.Id == editorModel.SelectedSectionId.Value);
        }

        public partial class SectionBoundaryMarker : CompositeDrawable
        {
            public SectionGimmickSection Section { get; }

            public Action<int>? OnSelected { get; init; }

            private Box startLine = null!;
            private Box endLine = null!;
            private OsuSpriteText label = null!;

            private Color4 baseColour;
            private bool isSelected;

            public SectionBoundaryMarker(SectionGimmickSection section)
            {
                Section = section;
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                RelativePositionAxes = Axes.X;
                RelativeSizeAxes = Axes.Y;

                X = (float)Section.StartTime;
                Width = (float)Math.Max(1, Section.EndTime < 0 ? 1 : Section.EndTime - Section.StartTime);

                Color4 display = Section.Settings.DisplayColor;
                baseColour = display == Color4.White ? colours.Pink2 : display;

                InternalChildren = new Drawable[]
                {
                    startLine = new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                        Colour = baseColour,
                        Alpha = 0.65f,
                    },
                    endLine = new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Colour = baseColour,
                        AlwaysPresent = Section.EndTime >= 0,
                        Alpha = Section.EndTime >= 0 ? 0.5f : 0,
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        Y = 2,
                        X = 4,
                        Child = label = new OsuSpriteText
                        {
                            Font = OsuFont.GetFont(size: 11, weight: FontWeight.SemiBold),
                            Colour = baseColour,
                            Text = string.IsNullOrWhiteSpace(Section.Settings.SectionName)
                                ? $"S{Section.Id}"
                                : $"S{Section.Id} {Section.Settings.SectionName}",
                            Shadow = true,
                        }
                    }
                };

                updateSelectedState();
            }

            protected override bool OnClick(osu.Framework.Input.Events.ClickEvent e)
            {
                OnSelected?.Invoke(Section.Id);
                return true;
            }

            protected override void Update()
            {
                base.Update();

                // Keep labels from cluttering at very narrow zoom levels.
                label.Alpha = DrawWidth >= 80 ? 1 : 0;
            }

            public void SetSelected(bool selected)
            {
                if (isSelected == selected)
                    return;

                isSelected = selected;
                updateSelectedState();
            }

            private void updateSelectedState()
            {
                if (startLine == null)
                    return;

                float thickness = isSelected ? 3f : 2f;
                var colour = isSelected ? baseColour.Lighten(0.35f) : baseColour;

                startLine.Width = thickness;
                endLine.Width = thickness;

                startLine.FadeColour(colour, 180, Easing.OutQuint);
                endLine.FadeColour(colour, 180, Easing.OutQuint);
                label.FadeColour(colour, 180, Easing.OutQuint);

                startLine.FadeTo(isSelected ? 0.95f : 0.65f, 180, Easing.OutQuint);
                endLine.FadeTo(isSelected ? 0.8f : 0.5f, 180, Easing.OutQuint);
            }
        }
    }
}
