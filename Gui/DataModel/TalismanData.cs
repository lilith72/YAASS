using YAASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Gui.DataModel
{
    class TalismanData
    {
        public string Name { get; private set; }
        public IList<SkillValue> Skills { get; private set; }
        public IList<int> Slots { get; private set; }
        public TalismanData(string name, SkillContributor talisman)
        {
            Name = name;
            Skills = talisman.ProvidedSkillValues;
            Slots = talisman.ProvidedDecoSlots;
        }
    }
}
