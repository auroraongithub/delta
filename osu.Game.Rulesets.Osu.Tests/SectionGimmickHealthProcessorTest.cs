// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Judgements;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Tests
{
    [TestFixture]
    public class SectionGimmickHealthProcessorTest
    {
        [Test]
        public void TestNoMissFailsOnMiss()
        {
            var hit = new HitCircle { StartTime = 1000 };
            var beatmap = new OsuBeatmap();
            beatmap.HitObjects.Add(hit);
            beatmap.SectionGimmicks.Sections.Add(new SectionGimmickSection
            {
                Id = 0,
                StartTime = 0,
                EndTime = -1,
                Settings = new SectionGimmickSettings
                {
                    EnableNoMiss = true
                }
            });

            var hp = new SectionGimmickHealthProcessor(0);
            hp.ApplyBeatmap(beatmap);

            var result = new OsuJudgementResult(hit, hit.CreateJudgement()) { Type = HitResult.Miss };
            hp.ApplyResult(result);

            Assert.That(hp.HasFailed, Is.True);
        }

        [Test]
        public void TestCountLimitFailsOnExceed()
        {
            var hit1 = new HitCircle { StartTime = 1000 };
            var hit2 = new HitCircle { StartTime = 2000 };
            var beatmap = new OsuBeatmap();
            beatmap.HitObjects.Add(hit1);
            beatmap.HitObjects.Add(hit2);
            beatmap.SectionGimmicks.Sections.Add(new SectionGimmickSection
            {
                Id = 0,
                StartTime = 0,
                EndTime = -1,
                Settings = new SectionGimmickSettings
                {
                    EnableCountLimits = true,
                    Max100s = 0
                }
            });

            var hp = new SectionGimmickHealthProcessor(0);
            hp.ApplyBeatmap(beatmap);

            hp.ApplyResult(new OsuJudgementResult(hit1, hit1.CreateJudgement()) { Type = HitResult.Ok });
            hp.ApplyResult(new OsuJudgementResult(hit2, hit2.CreateJudgement()) { Type = HitResult.Ok });

            Assert.That(hp.HasFailed, Is.True);
        }

        [Test]
        public void TestReverseHPGreatUsesConfiguredHP300()
        {
            var hit = new HitCircle { StartTime = 1000 };
            var beatmap = new OsuBeatmap();
            beatmap.HitObjects.Add(hit);
            beatmap.SectionGimmicks.Sections.Add(new SectionGimmickSection
            {
                Id = 0,
                StartTime = 0,
                EndTime = -1,
                Settings = new SectionGimmickSettings
                {
                    EnableHPGimmick = true,
                    NoDrain = true,
                    ReverseHP = true,
                    HP100 = 0.1f,
                    HP50 = 0.1f,
                    HPMiss = 0.1f,
                    HP300 = -0.2f,
                }
            });

            var hp = new SectionGimmickHealthProcessor(0);
            hp.ApplyBeatmap(beatmap);
            hp.Health.Value = 0.5;

            hp.ApplyResult(new OsuJudgementResult(hit, hit.CreateJudgement()) { Type = HitResult.Great });

            Assert.That(hp.Health.Value, Is.EqualTo(0.3).Within(0.0001));
        }
    }
}
