using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public interface ICraftingJobSettings
    {
        string RecipieKey { get; set; }
        string OnCraftedAudio { get; set; }
        float CraftingCooldown { get; set; }
    }
}
