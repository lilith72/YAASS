using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    public class SearchTarget
    {
        private IList<SkillValue> DesiredSkills { get; set; }

        public SearchTarget(
            List<SkillValue> desiredSkills)
        {
            this.DesiredSkills = desiredSkills;
        }

        /// <summary>
        /// Filters out:
        /// - armors for which the slot is full
        /// - armors without skills which help or gem slots
        /// </summary>
        /// <param name="contributor"></param>
        /// <param name="partialSolution"></param>
        /// <returns></returns>
        public bool SkillContributorHelpsTarget(SkillContributor contributor, Solution partialSolution)
        {
            if (!partialSolution.CanFitNewPiece(contributor))
            {
                return false;
            }

            // TODO: take armors that don't help skills but do add skill slots

            Dictionary<string, int> remainingSkillPoints = GetRemainingSkillPointsGivenSolution(partialSolution);
            return contributor.ProvidedSkillValues.Any((skillValue) => 
                remainingSkillPoints.ContainsKey(skillValue.SkillId)
                && remainingSkillPoints[skillValue.SkillId] > 0);
        }

        public bool SolutionFulfillsTarget(Solution solution)
        {
            // skillId -> remaining points
            Dictionary<string, int> remainingSkillPoints = GetRemainingSkillPointsGivenSolution(solution);

            foreach (KeyValuePair<string, int> pointsRemaining in remainingSkillPoints)
            {
                if (pointsRemaining.Value > 0)
                {
                    return false;
                }
            }

            // TODO: If user doesn't want vacant slots shown, then check against vacant non-deco slots here.

            return true;
        }

        private Dictionary<string, int> GetRemainingSkillPointsGivenSolution(Solution solution)
        {
            Dictionary<string, int> remainingSkillPoints = new Dictionary<string, int>();
            foreach (SkillValue desiredSkill in this.DesiredSkills)
            {
                remainingSkillPoints.Add(desiredSkill.SkillId, desiredSkill.Points);
            }

            foreach (SkillContributor contributor in solution.Contributors)
            {
                foreach (SkillValue skillPoints in contributor.ProvidedSkillValues)
                {
                    if (remainingSkillPoints.ContainsKey(skillPoints.SkillId))
                    {
                        remainingSkillPoints[skillPoints.SkillId] = remainingSkillPoints[skillPoints.SkillId] - skillPoints.Points;
                    }
                }
            }

            return remainingSkillPoints;
        }
    }
}
