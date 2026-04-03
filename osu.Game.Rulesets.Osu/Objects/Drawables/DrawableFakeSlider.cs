// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    public partial class DrawableFakeSlider : DrawableSlider
    {
        public DrawableFakeSlider()
            : this(null)
        {
        }

        public DrawableFakeSlider(FakeSlider? slider)
            : base(slider)
        {
        }
    }
}
