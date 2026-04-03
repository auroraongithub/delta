// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    public partial class DrawableFakeSliderRepeat : DrawableSliderRepeat
    {
        private DrawableFakeSlider FakeSlider => (DrawableFakeSlider)ParentHitObject;

        public DrawableFakeSliderRepeat()
        {
        }

        public DrawableFakeSliderRepeat(FakeSliderRepeat repeat)
            : base(repeat)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered && FakeSlider.SliderInputManager.Tracking)
                FakeSlider.RegisterInteraction(fromHead: false);

            if (timeOffset < 0)
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
