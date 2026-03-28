// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps.HitObjectGimmicks;

namespace osu.Game.Beatmaps.SectionGimmicks
{
    public static class SectionGimmickValueClamper
    {
        public static void ClampSectionSettingsInPlace(SectionGimmickSettings settings)
        {
            settings.Max300s = clampCountLimit(settings.Max300s);
            settings.Max100s = clampCountLimit(settings.Max100s);
            settings.Max50s = clampCountLimit(settings.Max50s);
            settings.MaxMisses = clampCountLimit(settings.MaxMisses);

            settings.HP300 = clampHpDelta(settings.HP300);
            settings.HP100 = clampHpDelta(settings.HP100);
            settings.HP50 = clampHpDelta(settings.HP50);
            settings.HPMiss = clampHpDelta(settings.HPMiss);

            settings.HPStart = clampZeroToOne(settings.HPStart);
            settings.HPCap = clampZeroToOne(settings.HPCap);

            settings.GreatOffsetThresholdMs = clampMin(settings.GreatOffsetThresholdMs, 0);
            settings.GreatOffsetPenaltyHP = clampMax(settings.GreatOffsetPenaltyHP, 0);

            if (!settings.AllowUnsafeDifficultyOverrideValues)
            {
                settings.SectionCircleSize = ClampCircleSize(settings.SectionCircleSize);
                settings.SectionApproachRate = ClampApproachRate(settings.SectionApproachRate);
                settings.SectionOverallDifficulty = ClampOverallDifficulty(settings.SectionOverallDifficulty);
            }

            if (!settings.AllowUnsafeStackLeniencyOverrideValues)
                settings.SectionStackLeniency = ClampStackLeniency(settings.SectionStackLeniency);

            if (!settings.AllowUnsafeTickRateOverrideValues)
                settings.SectionTickRate = ClampTickRate(settings.SectionTickRate);

            settings.GradualDifficultyChangeEndTimeMs = clampMin(settings.GradualDifficultyChangeEndTimeMs, 0);
            settings.SectionCircleSizeStartTimeMs = clampMin(settings.SectionCircleSizeStartTimeMs, -1);
            settings.SectionCircleSizeEndTimeMs = clampMin(settings.SectionCircleSizeEndTimeMs, -1);
            settings.SectionApproachRateStartTimeMs = clampMin(settings.SectionApproachRateStartTimeMs, -1);
            settings.SectionApproachRateEndTimeMs = clampMin(settings.SectionApproachRateEndTimeMs, -1);
            settings.SectionOverallDifficultyStartTimeMs = clampMin(settings.SectionOverallDifficultyStartTimeMs, -1);
            settings.SectionOverallDifficultyEndTimeMs = clampMin(settings.SectionOverallDifficultyEndTimeMs, -1);
            settings.FlashlightRadius = clampMinMax(settings.FlashlightRadius, 20f, 400f);
            settings.GradualFlashlightRadiusEndTimeMs = clampMin(settings.GradualFlashlightRadiusEndTimeMs, 0);

            settings.WiggleStrength = clampMinMax(settings.WiggleStrength, 0.1f, 2f);
            settings.GrowStartScale = clampMinMax(settings.GrowStartScale, 0f, 0.99f);
            settings.DeflateStartScale = clampMinMax(settings.DeflateStartScale, 1f, 25f);
            settings.ApproachDifferentScale = clampMinMax(settings.ApproachDifferentScale, 1.5f, 10f);
            settings.NoScopeHiddenComboCount = clampMinMax(settings.NoScopeHiddenComboCount, 0, 50);
            settings.MagnetisedAttractionStrength = clampMinMax(settings.MagnetisedAttractionStrength, 0.05f, 1f);
            settings.RepelRepulsionStrength = clampMinMax(settings.RepelRepulsionStrength, 0.05f, 1f);
            settings.DepthMaxDepth = clampMinMax(settings.DepthMaxDepth, 50f, 200f);
            settings.BloomMaxSizeComboCount = clampMinMax(settings.BloomMaxSizeComboCount, 5, 100);
            settings.BloomMaxCursorSize = clampMinMax(settings.BloomMaxCursorSize, 5f, 15f);
            settings.BarrelRollSpinSpeed = clampMinMax(settings.BarrelRollSpinSpeed, 0.02, 12);
            settings.MutedMuteComboCount = clampMinMax(settings.MutedMuteComboCount, 0, 500);
        }

        public static void ClampHitObjectSettingsInPlace(HitObjectGimmickSettings settings)
        {
            settings.Max300s = clampCountLimit(settings.Max300s);
            settings.Max100s = clampCountLimit(settings.Max100s);
            settings.Max50s = clampCountLimit(settings.Max50s);
            settings.MaxMisses = clampCountLimit(settings.MaxMisses);

            settings.HP300 = clampHpDelta(settings.HP300);
            settings.HP100 = clampHpDelta(settings.HP100);
            settings.HP50 = clampHpDelta(settings.HP50);
            settings.HPMiss = clampHpDelta(settings.HPMiss);

            settings.GreatOffsetThresholdMs = clampMin(settings.GreatOffsetThresholdMs, 0);
            settings.GreatOffsetPenaltyHP = clampMax(settings.GreatOffsetPenaltyHP, 0);

            if (!settings.AllowUnsafeDifficultyOverrideValues)
            {
                settings.SectionCircleSize = ClampCircleSize(settings.SectionCircleSize);
                settings.SectionApproachRate = ClampApproachRate(settings.SectionApproachRate);
                settings.SectionOverallDifficulty = ClampOverallDifficulty(settings.SectionOverallDifficulty);
            }

            if (!settings.AllowUnsafeStackLeniencyOverrideValues)
                settings.SectionStackLeniency = ClampStackLeniency(settings.SectionStackLeniency);

            if (!settings.AllowUnsafeTickRateOverrideValues)
                settings.SectionTickRate = ClampTickRate(settings.SectionTickRate);

            settings.FlashlightRadius = clampMinMax(settings.FlashlightRadius, 20f, 400f);
        }

        public static float ClampCircleSize(float value)
            => clampMinMax(value, 0, 11);

        public static float ClampApproachRate(float value)
            => clampMinMax(value, -20, 11);

        public static float ClampOverallDifficulty(float value)
            => clampMinMax(value, 0, 11);

        public static float ClampStackLeniency(float value)
            => clampMinMax(value, 0, 1);

        public static double ClampTickRate(double value)
            => double.IsNaN(value) ? value : Math.Max(0, value);

        private static int clampCountLimit(int value)
            => Math.Max(-1, value);

        private static float clampHpDelta(float value)
            => clampMinMax(value, -2, 2);

        private static float clampZeroToOne(float value)
            => clampMinMax(value, 0, 1);

        private static float clampMin(float value, float min)
            => float.IsNaN(value) ? value : Math.Max(min, value);

        private static float clampMax(float value, float max)
            => float.IsNaN(value) ? value : Math.Min(max, value);

        private static float clampMinMax(float value, float min, float max)
            => float.IsNaN(value) ? value : Math.Clamp(value, min, max);

        private static int clampMinMax(int value, int min, int max)
            => Math.Clamp(value, min, max);

        private static double clampMinMax(double value, double min, double max)
            => double.IsNaN(value) ? value : Math.Clamp(value, min, max);
    }
}
