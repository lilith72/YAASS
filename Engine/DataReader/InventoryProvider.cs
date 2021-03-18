using JustinsASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.DataReader
{
    public class InventoryProvider
    {
        // #Fields:ItemUniqueName,armorPoints,slotType,size1DecoSlots,size2DecoSlots,size3DecoSlots,size4DecoSlots,Skill1Name,Skill1Points,Skill2Name,Skill2Points,Skill3Name,Skill3Points,Skill4Name,Skill4Points,Skill5Name,Skill5Points
        List<SkillContributor> ReadSkillContributorsFromCSV()
        {
            throw new NotImplementedException();
        }

        // #Fields:SkillName,MaxLevel
        Dictionary<string, int> ReadSkillNameToMaxLevelFromCSV()
        {
            throw new NotImplementedException();
        }
    }
}
