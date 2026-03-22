// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Beatmaps.SectionGimmicks
{
    public class BeatmapSectionGimmicks
    {
        public List<SectionGimmickSection> Sections { get; set; } = new List<SectionGimmickSection>();

        public SectionGimmickSection? FindSectionAt(double time)
            => Sections.OrderBy(s => s.StartTime).FirstOrDefault(s => s.Contains(time));
    }
}
