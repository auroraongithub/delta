// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Game.Audio;
using osu.Game.Rulesets.Osu.Skinning.Legacy;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Osu.Tests
{
    [TestFixture]
    public class OsuLegacySkinTransformerTest
    {
        [Test]
        public void TestSectionGimmickStatusTextLookupDoesNotThrow()
        {
            var transformer = new OsuLegacySkinTransformer(new EmptySkin());

            Drawable? component = null;

            Assert.DoesNotThrow(() => component = transformer.GetDrawableComponent(new OsuSkinComponentLookup(OsuSkinComponents.SectionGimmickStatusText)));
            Assert.That(component, Is.Null);
        }

        private class EmptySkin : ISkin
        {
            public Drawable? GetDrawableComponent(ISkinComponentLookup lookup) => null;

            public Texture? GetTexture(string componentName, WrapMode wrapModeS, WrapMode wrapModeT) => null;

            public ISample? GetSample(ISampleInfo sampleInfo) => null;

            public IBindable<TValue>? GetConfig<TLookup, TValue>(TLookup lookup)
                where TLookup : notnull
                where TValue : notnull
                => null;
        }
    }
}
