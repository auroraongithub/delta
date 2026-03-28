// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Beatmaps.HitObjectGimmicks
{
    public static class HitObjectGimmickBindingUtils
    {
        private static long nextObjectId;

        public static long GenerateNewObjectId()
            => Interlocked.Increment(ref nextObjectId);

        public static void EnsureObjectIds(IEnumerable<HitObject> hitObjects)
        {
            foreach (var hitObject in hitObjects)
            {
                if (!hitObject.GimmickObjectId.HasValue)
                    hitObject.GimmickObjectId = GenerateNewObjectId();
            }
        }

        public static void SynchroniseEntriesWithHitObjects(IBeatmap beatmap)
        {
            EnsureObjectIds(beatmap.HitObjects);

            var gimmicks = beatmap.HitObjectGimmicks;

            if (gimmicks == null || gimmicks.Entries.Count == 0)
                return;

            var byObjectId = beatmap.HitObjects
                                     .Where(h => h.GimmickObjectId.HasValue)
                                     .ToDictionary(h => h.GimmickObjectId!.Value, h => h);

            var byLegacyKey = beatmap.HitObjects
                                     .OfType<IHasComboInformation>()
                                     .GroupBy(h => (((HitObject)h).StartTime, h.ComboIndexWithOffsets))
                                     .ToDictionary(g => g.Key, g => new Queue<HitObject>(g.Cast<HitObject>()));

            var byStartTime = beatmap.HitObjects
                                     .OfType<IHasComboInformation>()
                                     .Select(h => (hitObject: (HitObject)h, combo: h.ComboIndexWithOffsets))
                                     .GroupBy(v => v.hitObject.StartTime)
                                     .ToDictionary(g => g.Key, g => new Queue<HitObject>(g.OrderBy(v => v.combo).Select(v => v.hitObject)));

            var entries = gimmicks.Entries;

            foreach (var entry in entries)
            {
                if (entry == null)
                    continue;

                if (entry.ObjectId.HasValue)
                    continue;

                if (byLegacyKey.TryGetValue((entry.StartTime, entry.ComboIndexWithOffsets), out Queue<HitObject>? legacyCandidates)
                    && legacyCandidates.Count > 0)
                {
                    HitObject matched = legacyCandidates.Dequeue();
                    entry.ObjectId = matched.GimmickObjectId;
                    continue;
                }

                if (byStartTime.TryGetValue(entry.StartTime, out Queue<HitObject>? timeCandidates)
                    && timeCandidates.Count > 0)
                {
                    HitObject matched = timeCandidates.Dequeue();
                    entry.ObjectId = matched.GimmickObjectId;
                }
            }

            foreach (var entry in entries)
            {
                if (entry == null)
                    continue;

                if (!entry.ObjectId.HasValue)
                    continue;

                if (!byObjectId.TryGetValue(entry.ObjectId.Value, out HitObject? hitObject))
                    continue;

                entry.StartTime = hitObject.StartTime;

                if (hitObject is IHasComboInformation comboInformation)
                    entry.ComboIndexWithOffsets = comboInformation.ComboIndexWithOffsets;
            }
        }

        public static Dictionary<long, HitObjectGimmickSettings> CreateLookupByObjectId(BeatmapHitObjectGimmicks gimmicks)
        {
            var lookup = new Dictionary<long, HitObjectGimmickSettings>();

            foreach (var entry in gimmicks.Entries)
            {
                if (!entry.ObjectId.HasValue)
                    continue;

                lookup[entry.ObjectId.Value] = entry.Settings ?? new HitObjectGimmickSettings();
            }

            return lookup;
        }

        public static Dictionary<(double StartTime, int ComboIndexWithOffsets), HitObjectGimmickSettings> CreateLookupByLegacyKey(BeatmapHitObjectGimmicks gimmicks)
        {
            var lookup = new Dictionary<(double StartTime, int ComboIndexWithOffsets), HitObjectGimmickSettings>();

            foreach (var entry in gimmicks.Entries)
                lookup[(entry.StartTime, entry.ComboIndexWithOffsets)] = entry.Settings ?? new HitObjectGimmickSettings();

            return lookup;
        }

        public static bool TryGetSettings(HitObject hitObject,
                                          Dictionary<long, HitObjectGimmickSettings> objectIdLookup,
                                          Dictionary<(double StartTime, int ComboIndexWithOffsets), HitObjectGimmickSettings> legacyLookup,
                                          out HitObjectGimmickSettings settings)
        {
            if (hitObject.GimmickObjectId.HasValue
                && objectIdLookup.TryGetValue(hitObject.GimmickObjectId.Value, out settings!))
            {
                return true;
            }

            if (hitObject is IHasComboInformation comboInformation
                && legacyLookup.TryGetValue((hitObject.StartTime, comboInformation.ComboIndexWithOffsets), out settings!))
            {
                return true;
            }

            settings = null!;
            return false;
        }

        public static HitObjectGimmickSettings CloneSettings(HitObjectGimmickSettings source)
            => new HitObjectGimmickSettings
            {
                EnableHPGimmick = source.EnableHPGimmick,
                EnableNoMiss = source.EnableNoMiss,
                EnableCountLimits = source.EnableCountLimits,
                EnableGreatOffsetPenalty = source.EnableGreatOffsetPenalty,

                Max300s = source.Max300s,
                Max100s = source.Max100s,
                Max50s = source.Max50s,
                MaxMisses = source.MaxMisses,

                HP300 = source.HP300,
                HP100 = source.HP100,
                HP50 = source.HP50,
                HPMiss = source.HPMiss,

                GreatOffsetThresholdMs = source.GreatOffsetThresholdMs,
                GreatOffsetPenaltyHP = source.GreatOffsetPenaltyHP,

                EnableDifficultyOverrides = source.EnableDifficultyOverrides,
                AllowUnsafeDifficultyOverrideValues = source.AllowUnsafeDifficultyOverrideValues,
                SectionCircleSize = source.SectionCircleSize,
                SectionApproachRate = source.SectionApproachRate,
                SectionOverallDifficulty = source.SectionOverallDifficulty,
                AllowUnsafeStackLeniencyOverrideValues = source.AllowUnsafeStackLeniencyOverrideValues,
                SectionStackLeniency = source.SectionStackLeniency,
                AllowUnsafeTickRateOverrideValues = source.AllowUnsafeTickRateOverrideValues,
                SectionTickRate = source.SectionTickRate,

                ForceHidden = source.ForceHidden,
                ForceNoApproachCircle = source.ForceNoApproachCircle,
                ForceHardRock = source.ForceHardRock,
                ForceFlashlight = source.ForceFlashlight,
                FlashlightRadius = source.FlashlightRadius,
            };

        public static BeatmapHitObjectGimmicks CloneGimmicks(BeatmapHitObjectGimmicks source)
            => new BeatmapHitObjectGimmicks
            {
                Entries = source.Entries.Select(e => new HitObjectGimmickEntry
                {
                    ObjectId = e.ObjectId,
                    StartTime = e.StartTime,
                    ComboIndexWithOffsets = e.ComboIndexWithOffsets,
                    Settings = CloneSettings(e.Settings ?? new HitObjectGimmickSettings()),
                }).ToList(),
            };
    }
}
