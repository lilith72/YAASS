using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    public class SearchTarget
    {
        private IList<SkillValue> desiredSkills;


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

            // Always want to consider sets with vacant slots.
            if (contributor is VacantSlot)
            {
                return true;
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
            return true;
        }

        private Dictionary<string, int> GetRemainingSkillPointsGivenSolution(Solution solution)
        {
            Dictionary<string, int> remainingSkillPoints = new Dictionary<string, int>();
            foreach (SkillValue desiredSkill in desiredSkills)
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
