// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps.SectionGimmicks;

namespace osu.Game.Rulesets.Osu.Scoring
{
    public static class SectionGimmickSectionResolver
    {
        public static SectionGimmickSection? Resolve(BeatmapSectionGimmicks gimmicks, double time)
            => gimmicks.FindSectionAt(time);
    }
}
