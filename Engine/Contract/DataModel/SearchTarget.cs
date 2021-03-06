using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Contract.DataModel
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
            if (!partialSolution.CanFitNewPiece(contributor)
                && !(partialSolution.Contributors.Any(contr => contr is VacantSlot) && contributor is Decoration)) // Don't discard decos we could potentially fit later
            {
                return false;
            }

            if (contributor.ProvidedSkillValues.Any(sv => sv.SkillId.Equals("Stormsoul", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

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

            return true;
        }

        public Dictionary<string, int> GetRemainingSkillPointsGivenSolution(Solution solution)
        {
            // TODO: precalculate and cache this, needs a refactor into solution though
            Dictionary<string, int> remainingSkillPoints = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (SkillValue desiredSkill in this.DesiredSkills)
            {
                remainingSkillPoints.Add(desiredSkill.SkillId, desiredSkill.Points);
            }

            foreach (KeyValuePair<string, int> skillPoints in solution.GetSkillValuesPrecomputed())
            {
                if (remainingSkillPoints.ContainsKey(skillPoints.Key))
                {
                    remainingSkillPoints[skillPoints.Key] = remainingSkillPoints[skillPoints.Key] - skillPoints.Value;
                }
            }

            return remainingSkillPoints;
        }
    }
}
