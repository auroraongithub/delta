// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Beatmaps.HitObjectGimmicks;
using osu.Game.Beatmaps.SectionGimmicks;

namespace osu.Game.Tests.Beatmaps.SectionGimmicks
{
    [TestFixture]
    public class SectionGimmickValueClamperTest
    {
        [Test]
        public void TestClampSectionDifficultyAndHpRanges()
        {
            var settings = new SectionGimmickSettings
            {
                HP300 = 5,
                HP100 = -5,
                HP50 = 3,
                HPMiss = -3,
                HPStart = 2,
                HPCap = -1,
                SectionCircleSize = 500,
                SectionApproachRate = 500,
                SectionOverallDifficulty = -50,
            };

            SectionGimmickValueClamper.ClampSectionSettingsInPlace(settings);

            Assert.That(settings.HP300, Is.EqualTo(2));
            Assert.That(settings.HP100, Is.EqualTo(-2));
            Assert.That(settings.HP50, Is.EqualTo(2));
            Assert.That(settings.HPMiss, Is.EqualTo(-2));
            Assert.That(settings.HPStart, Is.EqualTo(1));
            Assert.That(settings.HPCap, Is.EqualTo(0));
            Assert.That(settings.SectionCircleSize, Is.EqualTo(11));
            Assert.That(settings.SectionApproachRate, Is.EqualTo(11));
            Assert.That(settings.SectionOverallDifficulty, Is.EqualTo(0));
        }

        [Test]
        public void TestClampSectionFunModRanges()
        {
            var settings = new SectionGimmickSettings
            {
                WiggleStrength = -1,
                GrowStartScale = -10,
                DeflateStartScale = 200,
                ApproachDifferentScale = -20,
                NoScopeHiddenComboCount = 999,
                MagnetisedAttractionStrength = -5,
                RepelRepulsionStrength = 999,
                DepthMaxDepth = 1,
                BloomMaxSizeComboCount = -50,
                BloomMaxCursorSize = 999,
                BarrelRollSpinSpeed = 999,
                MutedMuteComboCount = -5,
            };

            SectionGimmickValueClamper.ClampSectionSettingsInPlace(settings);

            Assert.That(settings.WiggleStrength, Is.EqualTo(0.1f));
            Assert.That(settings.GrowStartScale, Is.EqualTo(0));
            Assert.That(settings.DeflateStartScale, Is.EqualTo(25));
            Assert.That(settings.ApproachDifferentScale, Is.EqualTo(1.5f));
            Assert.That(settings.NoScopeHiddenComboCount, Is.EqualTo(50));
            Assert.That(settings.MagnetisedAttractionStrength, Is.EqualTo(0.05f));
            Assert.That(settings.RepelRepulsionStrength, Is.EqualTo(1));
            Assert.That(settings.DepthMaxDepth, Is.EqualTo(50));
            Assert.That(settings.BloomMaxSizeComboCount, Is.EqualTo(5));
            Assert.That(settings.BloomMaxCursorSize, Is.EqualTo(15));
            Assert.That(settings.BarrelRollSpinSpeed, Is.EqualTo(12));
            Assert.That(settings.MutedMuteComboCount, Is.EqualTo(0));
        }

        [Test]
        public void TestClampHitObjectRanges()
        {
            var settings = new HitObjectGimmickSettings
            {
                Max300s = -999,
                HP300 = 9,
                GreatOffsetThresholdMs = -100,
                GreatOffsetPenaltyHP = 3,
                SectionCircleSize = -10,
                SectionApproachRate = -50,
                SectionOverallDifficulty = 200,
            };

            SectionGimmickValueClamper.ClampHitObjectSettingsInPlace(settings);

            Assert.That(settings.Max300s, Is.EqualTo(-1));
            Assert.That(settings.HP300, Is.EqualTo(2));
            Assert.That(settings.GreatOffsetThresholdMs, Is.EqualTo(0));
            Assert.That(settings.GreatOffsetPenaltyHP, Is.EqualTo(0));
            Assert.That(settings.SectionCircleSize, Is.EqualTo(0));
            Assert.That(settings.SectionApproachRate, Is.EqualTo(-20));
            Assert.That(settings.SectionOverallDifficulty, Is.EqualTo(11));
        }

        [Test]
        public void TestUnsafeDifficultyOverrideSkipsDifficultyClamps()
        {
            var settings = new SectionGimmickSettings
            {
                AllowUnsafeDifficultyOverrideValues = true,
                SectionCircleSize = 500,
                SectionApproachRate = 500,
                SectionOverallDifficulty = -200,
            };

            SectionGimmickValueClamper.ClampSectionSettingsInPlace(settings);

            Assert.That(settings.SectionCircleSize, Is.EqualTo(500));
            Assert.That(settings.SectionApproachRate, Is.EqualTo(500));
            Assert.That(settings.SectionOverallDifficulty, Is.EqualTo(-200));
        }

        [Test]
        public void TestClampFlashlightRadiusRanges()
        {
            var section = new SectionGimmickSettings
            {
                FlashlightRadius = 999,
                GradualFlashlightRadiusEndTimeMs = -100,
            };

            var hitObject = new HitObjectGimmickSettings
            {
                FlashlightRadius = -20,
            };

            SectionGimmickValueClamper.ClampSectionSettingsInPlace(section);
            SectionGimmickValueClamper.ClampHitObjectSettingsInPlace(hitObject);

            Assert.That(section.FlashlightRadius, Is.EqualTo(400));
            Assert.That(section.GradualFlashlightRadiusEndTimeMs, Is.EqualTo(0));
            Assert.That(hitObject.FlashlightRadius, Is.EqualTo(20));
        }
    }
}
