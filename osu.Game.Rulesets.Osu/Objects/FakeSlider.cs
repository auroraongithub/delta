// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps.HitObjectGimmicks;
using System;
using System.Threading;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Judgements;

namespace osu.Game.Rulesets.Osu.Objects
{
    public class FakeSlider : Slider
    {
        public override Judgement CreateJudgement() => new OsuIgnoreJudgement();

        public FakePunishMode FakePunishMode { get; set; }
        public bool FakePlayHitsound { get; set; }
        public bool FakeRevealEnabled { get; set; } = true;
        public float FakeRevealRed { get; set; } = 1f;
        public float FakeRevealGreen { get; set; } = 0.3019608f;
        public float FakeRevealBlue { get; set; } = 0.3019608f;
        public float FakeRevealStrength { get; set; } = HitObjectGimmickSettings.DEFAULT_FAKE_REVEAL_STRENGTH;
        public float FakeRevealLeadInStartMs { get; set; } = HitObjectGimmickSettings.DEFAULT_FAKE_REVEAL_LEAD_IN_START_MS;
        public float FakeRevealLeadInLengthMs { get; set; } = HitObjectGimmickSettings.DEFAULT_FAKE_REVEAL_LEAD_IN_LENGTH_MS;
        public float FakeRevealFadeOutStartMs { get; set; } = HitObjectGimmickSettings.DEFAULT_FAKE_REVEAL_FADE_OUT_START_MS;
        public float FakeRevealFadeOutLengthMs { get; set; } = HitObjectGimmickSettings.DEFAULT_FAKE_REVEAL_FADE_OUT_LENGTH_MS;

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            HeadCircle = null!;
            TailCircle = null!;
            LastRepeat = null!;

            var sliderEvents = SliderEventGenerator.Generate(StartTime, SpanDuration, Velocity, TickDistance, Path.Distance, this.SpanCount(), cancellationToken);

            foreach (var e in sliderEvents)
            {
                switch (e.Type)
                {
                    case SliderEventType.Tick:
                        AddNested(new FakeSliderTick
                        {
                            SpanIndex = e.SpanIndex,
                            SpanStartTime = e.SpanStartTime,
                            StartTime = e.Time,
                            Position = Position + Path.PositionAt(e.PathProgress),
                            PathProgress = e.PathProgress,
                            StackHeight = StackHeight,
                        });
                        break;

                    case SliderEventType.Head:
                        AddNested(HeadCircle = new FakeSliderHeadCircle
                        {
                            StartTime = e.Time,
                            Position = Position,
                            StackHeight = StackHeight,
                            ClassicSliderBehaviour = ClassicSliderBehaviour,
                        });
                        break;

                    case SliderEventType.Tail:
                        AddNested(TailCircle = new FakeSliderTailCircle(this)
                        {
                            RepeatIndex = e.SpanIndex,
                            StartTime = e.Time,
                            Position = EndPosition,
                            StackHeight = StackHeight,
                            ClassicSliderBehaviour = ClassicSliderBehaviour,
                        });
                        break;

                    case SliderEventType.Repeat:
                        AddNested(LastRepeat = new FakeSliderRepeat(this)
                        {
                            RepeatIndex = e.SpanIndex,
                            StartTime = StartTime + (e.SpanIndex + 1) * SpanDuration,
                            Position = Position + Path.PositionAt(e.PathProgress),
                            StackHeight = StackHeight,
                            PathProgress = e.PathProgress,
                        });
                        break;
                }
            }

            if (HeadCircle == null)
            {
                AddNested(HeadCircle = new FakeSliderHeadCircle
                {
                    StartTime = StartTime,
                    Position = Position,
                    StackHeight = StackHeight,
                    ClassicSliderBehaviour = ClassicSliderBehaviour,
                });
            }

            if (TailCircle == null)
            {
                AddNested(TailCircle = new FakeSliderTailCircle(this)
                {
                    RepeatIndex = Math.Max(0, RepeatCount),
                    StartTime = EndTime,
                    Position = EndPosition,
                    StackHeight = StackHeight,
                    ClassicSliderBehaviour = ClassicSliderBehaviour,
                });
            }

            UpdateNestedSamples();
        }
    }
}
