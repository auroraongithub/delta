// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Osu.Judgements;
using osu.Game.Rulesets.Osu.UI;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    public partial class DrawableFakeSliderHead : DrawableSliderHead
    {
        private bool displayMissSymbol;
        private bool visualNoHitTriggered;

        private DrawableFakeSlider FakeSlider => (DrawableFakeSlider)ParentHitObject;

        public DrawableFakeSliderHead()
        {
        }

        public DrawableFakeSliderHead(FakeSliderHeadCircle head)
            : base(head)
        {
        }

        public override bool DisplayResult => displayMissSymbol;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (!userTriggered)
            {
                if (!visualNoHitTriggered && !HitObject.HitWindows.CanBeHit(timeOffset))
                {
                    visualNoHitTriggered = true;
                    UpdateState(ArmedState.Hit, true);
                }

                return;
            }

            var result = ResultFor(timeOffset);
            var clickAction = CheckHittable?.Invoke(this, Time.Current, result);

            if (clickAction == ClickAction.Shake)
                Shake();

            if (result == HitResult.None || clickAction != ClickAction.Hit)
                return;

            if (!FakeSlider.IsMissPunishMode)
            {
                displayMissSymbol = false;
                ApplyResult(static (r, _) => ((OsuHitCircleJudgementResult)r).Type = HitResult.IgnoreHit, 0);
                FakeSlider.RegisterInteraction(fromHead: true);
                return;
            }

            FakeSlider.RegisterInteraction(fromHead: true);
        }

        internal void ApplyPunishMiss(bool fromHead)
        {
            if (Judged)
                return;

            displayMissSymbol = true;
            ApplyResult(static (r, _) => ((OsuHitCircleJudgementResult)r).Type = HitResult.Miss, 0);

            if (fromHead)
                UpdateState(ArmedState.Hit, true);
        }

        internal void FinaliseNoInteractionResult()
        {
            if (Judged)
                return;

            displayMissSymbol = false;
            ApplyResult(static (r, _) => ((OsuHitCircleJudgementResult)r).Type = HitResult.IgnoreHit, 0);
        }

        public override void PlaySamples()
        {
            if (FakeSlider.HitObject.FakePlayHitsound)
                base.PlaySamples();
        }

        protected override void OnFree()
        {
            displayMissSymbol = false;
            visualNoHitTriggered = false;
            base.OnFree();
        }
    }
}
