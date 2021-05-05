using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.FrontEndInterface;
using JustinsASS.Gui.DataModel;

namespace JustinsASS.Gui
{
    public class Helper
    {
        public static Dictionary<string, SkillLvlMax> GetSkillsWithMax(IList<SkillValue> skills, ASS ass)
        {
            Dictionary<string, SkillLvlMax> skillsWithMax = new Dictionary<string, SkillLvlMax>();

            foreach (SkillValue skill in skills)
            {
                IDictionary<string, int> temp = ass.GetSkillNamesToMaxLevelMapping();
                SkillLvlMax slm;
                // TODO: CHANGE WHEN ALL THE DATA IS UP TO DATE, currently need to check if skill exists since data is incomplete
                if (temp.ContainsKey(skill.SkillId))
                {
                    slm = new SkillLvlMax(skill.Points, ass.GetSkillNamesToMaxLevelMapping()[skill.SkillId]);
                }
                else
                {
                    slm = new SkillLvlMax(skill.Points, 0);
                }
                skillsWithMax.Add(skill.SkillId, slm);
            }

            return skillsWithMax;
        }
    }
}
