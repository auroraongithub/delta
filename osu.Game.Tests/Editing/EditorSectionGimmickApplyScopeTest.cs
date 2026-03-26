// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Screens.Edit;

namespace osu.Game.Tests.Editing
{
    [TestFixture]
    public class EditorSectionGimmickApplyScopeTest
    {
        [Test]
        public void TestApplySectionGimmicksToTargetBeatmapSupportsRulesetBeatmapTypes()
        {
            IBeatmap target = new OsuBeatmap();

            var source = new BeatmapSectionGimmicks();
            source.Sections.Add(new SectionGimmickSection
            {
                Id = 7,
                StartTime = 1234,
                EndTime = 5678,
                Settings = new SectionGimmickSettings
                {
                    EnableNoMiss = true,
                    SectionName = "copied",
                }
            });

            bool applied = Editor.TryApplySectionGimmicksToTargetBeatmap(target, source);

            Assert.That(applied, Is.True);
            Assert.That(target.SectionGimmicks.Sections.Count, Is.EqualTo(1));
            Assert.That(target.SectionGimmicks.Sections[0].Id, Is.EqualTo(7));
            Assert.That(target.SectionGimmicks.Sections[0].StartTime, Is.EqualTo(1234));
            Assert.That(target.SectionGimmicks.Sections[0].EndTime, Is.EqualTo(5678));
            Assert.That(target.SectionGimmicks.Sections[0].Settings.EnableNoMiss, Is.True);
            Assert.That(target.SectionGimmicks.Sections[0].Settings.SectionName, Is.EqualTo("copied"));

            target.SectionGimmicks.Sections[0].Settings.SectionName = "changed";
            Assert.That(source.Sections[0].Settings.SectionName, Is.EqualTo("copied"));
        }

        [Test]
        public void TestApplySectionGimmicksToTargetBeatmapRejectsNullInputs()
        {
            var target = new OsuBeatmap();
            var source = new BeatmapSectionGimmicks();

            Assert.That(Editor.TryApplySectionGimmicksToTargetBeatmap(null!, source), Is.False);
            Assert.That(Editor.TryApplySectionGimmicksToTargetBeatmap(target, null!), Is.False);
        }
    }
}
