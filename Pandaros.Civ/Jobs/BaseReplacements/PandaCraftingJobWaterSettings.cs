using Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.BaseReplacements
{
    public class PandaCraftingJobWaterSettings : CraftingJobWaterSettings
    {
        public PandaCraftingJobWaterSettings(string blockType, string npcType, float cooldown, int maxCraftsPerHaul, string onCraftedAudio) : 
            base(blockType, npcType, cooldown, maxCraftsPerHaul, onCraftedAudio)
        {
        }
    }
}
