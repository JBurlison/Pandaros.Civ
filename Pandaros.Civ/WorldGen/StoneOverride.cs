using Pandaros.Civ.Storage;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.WorldGen
{
    public class StoneOverride : IMineTypeOverride
    {
        public string[] BlockNames => new[]
        {
            "darkstone",
            "infinitestone",
            "stoneblock",
            "stonebricks"
        };

        public string HoldingItemType => "air";

        public StoredItem[] Replacement => new[]
        {
            new StoredItem(Rock.NAME, 4)
        };
    }
}
