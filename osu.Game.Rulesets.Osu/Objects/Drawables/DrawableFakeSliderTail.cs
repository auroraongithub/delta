// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    public partial class DrawableFakeSliderTail : DrawableSliderTail
    {
        private DrawableFakeSlider FakeSlider => (DrawableFakeSlider)ParentHitObject;

        public DrawableFakeSliderTail()
        {
        }

        public DrawableFakeSliderTail(FakeSliderTailCircle tail)
            : base(tail)
        {
            SamplePlaysOnlyOnHit = false;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered && FakeSlider.SliderInputManager.Tracking)
                FakeSlider.RegisterInteraction(fromHead: false);

            if (timeOffset < SliderEventGenerator.TAIL_LENIENCY)
                return;

            if (!Judged)
                ApplyResult(static (r, _) => r.Type = HitResult.IgnoreHit, 0);
        }

        public override void PlaySamples()
        {
            if (FakeSlider.HitObject.FakePlayHitsound)
                base.PlaySamples();
        }
    }
}
