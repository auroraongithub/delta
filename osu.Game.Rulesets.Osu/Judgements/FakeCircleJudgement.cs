// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Judgements
{
    /// <summary>
    /// Fake circle judgement supports both ignore results and miss for punishment mode.
    /// </summary>
    public class FakeCircleJudgement : Judgement, ICustomJudgementResultValidator
    {
        public override HitResult MaxResult => HitResult.IgnoreHit;

        public override HitResult MinResult => HitResult.IgnoreMiss;

        public bool IsResultAllowed(HitResult result)
            => result == HitResult.Miss;

        protected override double HealthIncreaseFor(HitResult result)
            => result == HitResult.Miss
                ? base.HealthIncreaseFor(HitResult.Miss)
                : 0;
    }
}
