// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Scoring
{
    public class SectionGimmickCountTracker
    {
        private int currentSectionId = -1;
        private int count300;
        private int count100;
        private int count50;
        private int countMiss;

        public void EnterSection(int sectionId)
        {
            if (currentSectionId == sectionId)
                return;

            currentSectionId = sectionId;
            count300 = 0;
            count100 = 0;
            count50 = 0;
            countMiss = 0;
        }

        public void Record(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                    count300++;
                    break;

                case HitResult.Ok:
                    count100++;
                    break;

                case HitResult.Meh:
                    count50++;
                    break;

                case HitResult.Miss:
                    countMiss++;
                    break;
            }
        }

        public bool Exceeds(SectionGimmickSettings settings, HitResult result)
        {
            if (!settings.EnableCountLimits)
                return false;

            return result switch
            {
                HitResult.Great => settings.Max300s >= 0 && count300 > settings.Max300s,
                HitResult.Ok => settings.Max100s >= 0 && count100 > settings.Max100s,
                HitResult.Meh => settings.Max50s >= 0 && count50 > settings.Max50s,
                HitResult.Miss => settings.MaxMisses >= 0 && countMiss > settings.MaxMisses,
                _ => false
            };
        }
    }
}
