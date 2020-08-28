using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.JobSettings
{
    public interface ICraftingJobSettings : IPandaJobSettings
    {
        string RecipieKey { get; set; }
        string OnCraftedAudio { get; set; }
        float CraftingCooldown { get; set; }
    }
}
