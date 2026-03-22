// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Specialized;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Screens.Edit.Components.Timelines.Summary.Parts;

namespace osu.Game.Screens.Edit.Compose.Components.Timeline
{
    public partial class TimelineSectionGimmickDisplay : TimelinePart<TimelineSectionGimmick>
    {
        private readonly Cached displayCache = new Cached();

        private readonly BindableList<SectionGimmickSection> sections = new BindableList<SectionGimmickSection>();

        [Resolved]
        private SectionGimmickEditorModel editorModel { get; set; } = null!;

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

            foreach (var section in sections.OrderBy(s => s.StartTime))
            {
                if (section.EndTime < 0)
                    continue;

                Add(new TimelineSectionGimmick(section)
                {
                    OnSelected = id => editorModel.SelectedSectionId.Value = id,
                });
            }

            updateSelectionState();
        }

        private void updateSelectionState()
        {
            foreach (var section in AliveChildren)
                section.SetSelected(section.Section.Id == editorModel.SelectedSectionId.Value);
        }
    }
}
