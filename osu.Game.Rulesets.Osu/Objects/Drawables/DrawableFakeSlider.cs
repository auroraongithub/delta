// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps.HitObjectGimmicks;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    public partial class DrawableFakeSlider : DrawableSlider
    {
        private bool interactionRegistered;
        private bool wasTracking;

        public new FakeSlider HitObject => (FakeSlider)base.HitObject;

        public new DrawableFakeSliderHead HeadCircle => (DrawableFakeSliderHead)base.HeadCircle;

        public DrawableFakeSlider()
            : this(null)
        {
        }

        public DrawableFakeSlider(FakeSlider? slider)
            : base(slider)
        {
        }

        public override bool DisplayResult => false;

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case FakeSliderHeadCircle head:
                    return new DrawableFakeSliderHead(head);

                case FakeSliderTailCircle tail:
                    return new DrawableFakeSliderTail(tail);

                case FakeSliderTick tick:
                    return new DrawableFakeSliderTick(tick);

                case FakeSliderRepeat repeat:
                    return new DrawableFakeSliderRepeat(repeat);
            }

            return base.CreateNestedHitObject(hitObject);
        }

        internal bool IsMissPunishMode => HitObject.FakePunishMode == FakePunishMode.Miss;

        internal void RegisterInteraction(bool fromHead)
        {
            if (interactionRegistered)
                return;

            interactionRegistered = true;

            if (IsMissPunishMode)
                HeadCircle.ApplyPunishMiss(fromHead);
        }

        protected override void Update()
        {
            base.Update();

            if (!wasTracking && Tracking.Value && Time.Current >= HitObject.StartTime && Time.Current <= HitObject.EndTime)
                RegisterInteraction(fromHead: false);

            wasTracking = Tracking.Value;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered && Time.Current >= HitObject.StartTime && Time.Current <= HitObject.EndTime && SliderInputManager.Tracking)
                RegisterInteraction(fromHead: false);

            if (userTriggered || !TailCircle.Judged || Time.Current < HitObject.EndTime)
                return;

            HeadCircle.FinaliseNoInteractionResult();

            ApplyResult(static (r, _) => r.Type = HitResult.IgnoreHit, 0);
        }

        public override void PlaySamples()
        {
            if (HitObject.FakePlayHitsound)
                base.PlaySamples();
        }

        protected override void OnFree()
        {
            interactionRegistered = false;
            wasTracking = false;
            base.OnFree();
        }
    }
}
