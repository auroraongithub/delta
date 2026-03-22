// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Rulesets.Osu;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Compose;

namespace osu.Game.Tests.Editing
{
    [TestFixture]
    public class SectionGimmickEditorModelTest
    {
        [Test]
        public void TestAddSectionCreatesSectionFirstFlow()
        {
            var editorBeatmap = createEditorBeatmap();
            var model = new SectionGimmickEditorModel(editorBeatmap);

            model.AddSection(5000);

            Assert.That(model.Sections.Count, Is.EqualTo(1));
            Assert.That(model.Sections[0].StartTime, Is.EqualTo(5000));
            Assert.That(model.Sections[0].EndTime, Is.EqualTo(-1));
            Assert.That(model.SelectedSectionId.Value, Is.EqualTo(model.Sections[0].Id));
        }

        [Test]
        public void TestCopyPasteSettingsDoesNotOverwriteRange()
        {
            var editorBeatmap = createEditorBeatmap();
            var model = new SectionGimmickEditorModel(editorBeatmap);

            model.AddSection(0);
            model.SetSelectedEndTime(1000);
            model.SetSelectedSetting(s =>
            {
                s.EnableNoMiss = true;
                s.EnableCountLimits = true;
                s.Max100s = 3;
            });

            int sourceId = model.SelectedSectionId.Value;

            model.AddSection(2000);
            int targetId = model.SelectedSectionId.Value;

            double targetStart = model.Sections.First(s => s.Id == targetId).StartTime;
            double targetEnd = model.Sections.First(s => s.Id == targetId).EndTime;

            model.SelectedSectionId.Value = sourceId;
            model.CopySelectedSettings();
            model.PasteSettingsTo(new[] { targetId });

            var target = model.Sections.First(s => s.Id == targetId);

            Assert.That(target.StartTime, Is.EqualTo(targetStart));
            Assert.That(target.EndTime, Is.EqualTo(targetEnd));
            Assert.That(target.Settings.EnableNoMiss, Is.True);
            Assert.That(target.Settings.EnableCountLimits, Is.True);
            Assert.That(target.Settings.Max100s, Is.EqualTo(3));
        }

        [Test]
        public void TestCloneGimmicksCreatesDeepCopy()
        {
            var source = new BeatmapSectionGimmicks();
            source.Sections.Add(new SectionGimmickSection
            {
                Id = 1,
                StartTime = 0,
                EndTime = 1500,
                Settings = new SectionGimmickSettings
                {
                    EnableNoMiss = true,
                    Max100s = 2,
                }
            });

            var clone = SectionGimmickEditorModel.CloneGimmicks(source);

            Assert.That(clone.Sections.Count, Is.EqualTo(1));
            Assert.That(clone.Sections[0].Settings.EnableNoMiss, Is.True);

            clone.Sections[0].Settings.EnableNoMiss = false;

            Assert.That(source.Sections[0].Settings.EnableNoMiss, Is.True);
        }

        private static EditorBeatmap createEditorBeatmap()
            => new EditorBeatmap(new Beatmap
            {
                BeatmapInfo =
                {
                    Ruleset = new OsuRuleset().RulesetInfo,
                }
            });
    }
}
