// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps.SectionGimmicks;
using osu.Game.Rulesets.Edit.Checks.Components;

namespace osu.Game.Rulesets.Edit.Checks
{
    public class CheckSectionGimmicks : ICheck
    {
        public CheckMetadata Metadata => new CheckMetadata(CheckCategory.Settings, "Section gimmicks validity");

        public IEnumerable<IssueTemplate> PossibleTemplates => new[]
        {
            new IssueTemplateInvalidSectionGimmicks(this),
        };

        public IEnumerable<Issue> Run(BeatmapVerifierContext context)
        {
            try
            {
                SectionGimmicksValidator.Validate(context.CurrentDifficulty.Playable.SectionGimmicks);
                return Enumerable.Empty<Issue>();
            }
            catch (Exception e)
            {
                return new[]
                {
                    new Issue(new IssueTemplateInvalidSectionGimmicks(this), e.Message),
                };
            }
        }

        public class IssueTemplateInvalidSectionGimmicks : IssueTemplate
        {
            public IssueTemplateInvalidSectionGimmicks(ICheck check)
                : base(check, IssueType.Problem, "Invalid section gimmicks: {0}")
            {
            }
        }
    }
}
