// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Reflection;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Rulesets.Osu.UI;

namespace osu.Game.Rulesets.Osu.Tests
{
    [TestFixture]
    public class SectionGimmickDetailsDisplayTest
    {
        [Test]
        public void TestBuildDetailsLabelSingleLineNoHpPrefixes()
        {
            var section = new SectionGimmickSection
            {
                Id = 1,
                StartTime = 0,
                EndTime = 1000,
                Settings = new SectionGimmickSettings
                {
                    EnableHPGimmick = true,
                    HP300 = 0,
                    HP100 = -0.2f,
                    HP50 = 0.1f,
                    HPMiss = -0.5f,
                }
            };

            string label = invokeBuildLabel(section, false);

            Assert.That(label, Does.Contain("300: +0"));
            Assert.That(label, Does.Contain("100: -0.2"));
            Assert.That(label, Does.Contain("50: +0.1"));
            Assert.That(label, Does.Contain("Miss: -0.5"));

            Assert.That(label, Does.Not.Contain("HP300"));
            Assert.That(label, Does.Not.Contain("HP100"));
            Assert.That(label, Does.Not.Contain("HP50"));
            Assert.That(label, Does.Not.Contain("HPMiss"));

            Assert.That(label, Does.Contain(" | "));
        }

        [Test]
        public void TestBuildDetailsLabelMultiLineMode()
        {
            var section = new SectionGimmickSection
            {
                Id = 1,
                StartTime = 0,
                EndTime = 1000,
                Settings = new SectionGimmickSettings
                {
                    EnableHPGimmick = true,
                    HP300 = 0,
                    HP100 = -0.2f,
                    HP50 = 0.1f,
                }
            };

            string label = invokeBuildLabel(section, true);

            Assert.That(label, Does.Contain("300: +0\n"));
            Assert.That(label, Does.Contain("100: -0.2\n"));
            Assert.That(label, Does.Not.Contain(" | "));
        }

        private static string invokeBuildLabel(SectionGimmickSection section, bool multiline)
        {
            var method = typeof(SectionGimmickDetailsDisplay).GetMethod("BuildDetailsLabelForTest", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null);

            var bindable = new BindableBool(multiline);
            return (string)method!.Invoke(null, new object[] { section, bindable })!;
        }
    }
}
