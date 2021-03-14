using JustinsASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Search
{
    public class Utils
    {
        public static bool AllowsDuplicates(ArmorSlot slot)
        {
            switch (slot)
            {
                // TODO add the others
                case ArmorSlot.Head:
                    return false;
                case ArmorSlot.Deco:
                    return true;
                default:
                    throw new ArgumentException($"Unsupported slot type called to AllowsDuplicates: {slot}");
            }
        }
    }
}
