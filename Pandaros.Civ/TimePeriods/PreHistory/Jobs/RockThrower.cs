using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pandaros.Civ.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Pandaros.Civ.TimePeriods.PreHistory.Items;

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    public class RockThrowerGuardSettingsDay : ICSGuardJobSettings
    {
        public string blockType { get; set; } = RockThrower.Name;
        public int cooldownShot { get; set; } = 6;
        public int damage { get; set; } = 30;
        public string jobType { get; set; } = "guard";
        public string npcType { get; set; } = RockThrower.Name;
        public string onHitAudio { get; set; } = "fleshHit";
        public string onShootAudio { get; set; } = "sling";
        public int range { get; set; } = 10;
        public IRecruitmentitem recruitmentItem { get; set; } = new Recruitmentitem() { type = Rock.NAME };
        public IShootrequirement[] shootRequirements { get; set; } = new[] { new Shootrequirement() { type = Rock.NAME } };
        public string sleepType { get; set; } = "day"; 
    }

    public class RockThrowerSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = RockThrower.Name;
        public string printName { get; set; } = "Rock Thrower";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 1.5f;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }

    public class RockThrower
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(RockThrower));
    }

    public class RockThrowerType : CSGenerateType
    {
        public override string typeName => RockThrower.Name;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#474747";
    }

    public class RockThrowerRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(RockThrower.Name)
        };

        public bool isOptional => false;

        public string name => RockThrower.Name;
    }
}
