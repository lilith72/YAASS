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
        public static readonly int MAX_SLOT_SIZE = 3;
        public static readonly int MAX_WEAPON_SLOTS = 3;
        public static readonly int MAX_SLOTS = 3;

        public static Dictionary<string, SkillLvlMax> GetSkillsWithMax(IList<SkillValue> skills)
        {
            Dictionary<string, SkillLvlMax> skillsWithMax = new Dictionary<string, SkillLvlMax>();

            foreach (SkillValue skill in skills)
            {
                IDictionary<string, int> temp = ASS.Instance.GetSkillNamesToMaxLevelMapping();
                SkillLvlMax slm;
                // TODO: CHANGE WHEN ALL THE DATA IS UP TO DATE, currently need to check if skill exists since data is incomplete
                if (temp.ContainsKey(skill.SkillId))
                {
                    slm = new SkillLvlMax(skill.Points, ASS.Instance.GetSkillNamesToMaxLevelMapping()[skill.SkillId]);
                }
                else
                {
                    slm = new SkillLvlMax(skill.Points, 0);
                }
                skillsWithMax.Add(skill.SkillId, slm);
            }

            return skillsWithMax;
        }

        public static IList<int> DecorationArrayToList(int[] inDecos)
        {
            IList<int> outDecos = new List<int>();
            for (int i = 0; i < inDecos.Length; i++)
            {
                for (int j = 0; j < inDecos[i]; j++)
                {
                    outDecos.Add(i + 1);
                }
            }
            return outDecos;
        }
    }
}
