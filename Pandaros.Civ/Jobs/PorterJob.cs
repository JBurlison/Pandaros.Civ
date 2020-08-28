using Pandaros.Civ.Jobs.Goals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pandaros.Civ.Jobs
{
    public class PorterJob : IPandaJobSettings, INPCTypeStandardSettings
    {
        public InventoryItem RecruitmentItem { get; set; }
        public string JobBlock { get; set; }
        public bool SleepNight { get; set; } = true;
        public INpcGoal StartingGoal { get; set; } = new StockpikeToCrateGoal();
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Jobs", nameof(PorterJob));
        public string keyName { get; set; }
        public string printName { get; set; }
        public float inventoryCapacity { get; set; }
        public float movementSpeed { get; set; }
        public Color32 maskColor1 { get; set; }
        public Color32 maskColor0 { get; set; }
    }
}
