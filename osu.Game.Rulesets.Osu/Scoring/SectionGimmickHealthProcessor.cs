// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Scoring
{
    public partial class SectionGimmickHealthProcessor : OsuHealthProcessor
    {
        private readonly SectionGimmickCountTracker countTracker = new SectionGimmickCountTracker();
        private BeatmapSectionGimmicks gimmicks = new BeatmapSectionGimmicks();
        private SectionGimmickSection? activeSection;

        public SectionGimmickSection? ActiveSection => activeSection;

        public SectionGimmickHealthProcessor(double drainStartTime)
            : base(drainStartTime)
        {
        }

        public override void ApplyBeatmap(IBeatmap beatmap)
        {
            gimmicks = beatmap.SectionGimmicks ?? new BeatmapSectionGimmicks();
            SectionGimmicksValidator.Validate(gimmicks);
            base.ApplyBeatmap(beatmap);
        }

        protected override void Update()
        {
            base.Update();

            var section = resolveSection(Time.Current);
            if (section == null)
                return;

            var settings = section.Settings;

            if ((settings.EnableHPGimmick && settings.NoDrain) || settings.EnableGreatOffsetPenalty)
            {
                // cancel out frame drain while this mode is active.
                // this intentionally keeps behaviour section-scoped.
                Health.Value += DrainRate * Time.Elapsed;
            }
        }

        protected override void ApplyResultInternal(JudgementResult result)
        {
            var section = resolveSection(result.HitObject.StartTime);
            if (section != null)
            {
                var settings = section.Settings;

                if (settings.EnableNoMiss && result.Type == HitResult.Miss)
                {
                    TriggerFailure();
                    return;
                }

                if (settings.EnableNoMissedSliderEnd && result.HitObject is SliderTailCircle && result.Type == HitResult.IgnoreMiss)
                {
                    TriggerFailure();
                    return;
                }

                countTracker.Record(result.Type);
                if (countTracker.Exceeds(settings, result.Type))
                {
                    TriggerFailure();
                    return;
                }
            }

            base.ApplyResultInternal(result);

            // Apply additional offset penalty after base judgement application.
            if (section != null)
            {
                var settings = section.Settings;
                if (settings.EnableGreatOffsetPenalty && result.Type == HitResult.Great)
                {
                    if (Math.Abs(result.TimeOffset) > settings.GreatOffsetThresholdMs)
                        Health.Value += settings.GreatOffsetPenaltyHP;
                }
            }
        }

        protected override double GetHealthIncreaseFor(JudgementResult result)
        {
            var section = resolveSection(result.HitObject.StartTime);
            if (section == null)
                return base.GetHealthIncreaseFor(result);

            var settings = section.Settings;
            if (!settings.EnableHPGimmick)
                return base.GetHealthIncreaseFor(result);

            float hpValue = result.Type switch
            {
                HitResult.Great => settings.HP300,
                HitResult.Ok => settings.HP100,
                HitResult.Meh => settings.HP50,
                HitResult.Miss => settings.HPMiss,
                _ => float.NaN
            };

            if (float.IsNaN(hpValue))
                return base.GetHealthIncreaseFor(result);

            // When ReverseHP is false, positive HP values should drain (subtract from health)
            // When ReverseHP is true, positive HP values should heal (add to health)
            return settings.ReverseHP ? hpValue : -hpValue;
        }

        private SectionGimmickSection? resolveSection(double time)
        {
            var section = SectionGimmickSectionResolver.Resolve(gimmicks, time);

            if (section == null)
            {
                activeSection = null;
                return null;
            }

            if (activeSection?.Id != section.Id)
            {
                activeSection = section;
                countTracker.EnterSection(section.Id);

                var settings = section.Settings;
                if ((settings.EnableHPGimmick && settings.NoDrain && !settings.ReverseHP) && settings.EnableGreatOffsetPenalty)
                {
                    // both enabled: no-op special case, handled by per-hit logic.
                }
            }

            return section;
        }
    }
}
