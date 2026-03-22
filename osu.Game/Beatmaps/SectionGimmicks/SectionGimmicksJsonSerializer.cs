// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.Json;

namespace osu.Game.Beatmaps.SectionGimmicks
{
    public static class SectionGimmicksJsonSerializer
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

        public static string Serialize(BeatmapSectionGimmicks gimmicks)
            => JsonSerializer.Serialize(gimmicks, options);

        public static BeatmapSectionGimmicks Deserialize(string json)
            => JsonSerializer.Deserialize<BeatmapSectionGimmicks>(json, options) ?? new BeatmapSectionGimmicks();
    }
}
