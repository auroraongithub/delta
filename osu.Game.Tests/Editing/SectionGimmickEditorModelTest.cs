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
                s.MaxMisses = 1;
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
            Assert.That(target.Settings.MaxMisses, Is.EqualTo(1));
        }

        [Test]
        public void TestAddSectionBeforeExistingUsesTimelinePositionAndEndsAtNextSection()
        {
            var editorBeatmap = createEditorBeatmap();
            var model = new SectionGimmickEditorModel(editorBeatmap);

            model.AddSection(5000);
            model.AddSection(2000);

            var sectionAt2000 = model.Sections.Single(s => s.StartTime == 2000);
            var sectionAt5000 = model.Sections.Single(s => s.StartTime == 5000);

            Assert.That(sectionAt2000.EndTime, Is.EqualTo(5000));
            Assert.That(sectionAt5000.EndTime, Is.EqualTo(-1));
        }

        [Test]
        public void TestAddSectionCapsPreviousOpenEndedSectionAtInsertionPoint()
        {
            var editorBeatmap = createEditorBeatmap();
            var model = new SectionGimmickEditorModel(editorBeatmap);

            model.AddSection(1000);
            model.AddSection(3000);

            var first = model.Sections.Single(s => s.StartTime == 1000);
            Assert.That(first.EndTime, Is.EqualTo(3000));
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

        [Test]
        public void TestSetSelectedSettingClampsOutOfRangeValues()
        {
            var editorBeatmap = createEditorBeatmap();
            var model = new SectionGimmickEditorModel(editorBeatmap);

            model.AddSection(0);
            model.SetSelectedSetting(s =>
            {
                s.SectionCircleSize = 500;
                s.SectionApproachRate = 500;
                s.SectionOverallDifficulty = -50;
                s.HP300 = 999;
                s.HPStart = -2;
                s.HPCap = 3;
                s.WiggleStrength = -10;
                s.BloomMaxCursorSize = 999;
            });

            var settings = model.Sections[0].Settings;

            Assert.That(settings.SectionCircleSize, Is.EqualTo(11));
            Assert.That(settings.SectionApproachRate, Is.EqualTo(11));
            Assert.That(settings.SectionOverallDifficulty, Is.EqualTo(0));
            Assert.That(settings.HP300, Is.EqualTo(2));
            Assert.That(settings.HPStart, Is.EqualTo(0));
            Assert.That(settings.HPCap, Is.EqualTo(1));
            Assert.That(settings.WiggleStrength, Is.EqualTo(0.1f));
            Assert.That(settings.BloomMaxCursorSize, Is.EqualTo(15));
        }

        [Test]
        public void TestSetSelectedSettingAllowsUnsafeDifficultyOverrideValues()
        {
            var editorBeatmap = createEditorBeatmap();
            var model = new SectionGimmickEditorModel(editorBeatmap);

            model.AddSection(0);
            model.SetSelectedSetting(s =>
            {
                s.AllowUnsafeDifficultyOverrideValues = true;
                s.SectionCircleSize = 500;
                s.SectionApproachRate = 500;
                s.SectionOverallDifficulty = -500;
            });

            var settings = model.Sections[0].Settings;

            Assert.That(settings.AllowUnsafeDifficultyOverrideValues, Is.True);
            Assert.That(settings.SectionCircleSize, Is.EqualTo(500));
            Assert.That(settings.SectionApproachRate, Is.EqualTo(500));
            Assert.That(settings.SectionOverallDifficulty, Is.EqualTo(-500));
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
