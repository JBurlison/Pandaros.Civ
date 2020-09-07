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
    public class SpearThrowerGuardSettingsNight : ICSGuardJobSettings
    {
        public string blockType { get; set; } = SpearThrower.NameNight;
        public int cooldownShot { get; set; } = 6;
        public int damage { get; set; } = 50;
        public string jobType { get; set; } = "guard";
        public string npcType { get; set; } = SpearThrower.NameDay;
        public string onHitAudio { get; set; } = "fleshHit";
        public string onShootAudio { get; set; } = "sling";
        public int range { get; set; } = 5;
        public IRecruitmentitem recruitmentItem { get; set; } = new Recruitmentitem() { type = LeafSash.NAME };
        public IShootrequirement[] shootRequirements { get; set; } = new[] { new Shootrequirement() { type = WoodenSpear.NAME } };
        public string sleepType { get; set; } = "Day"; 
    }
    public class SpearThrowerGuardSettingsDay : ICSGuardJobSettings
    {
        public string blockType { get; set; } = SpearThrower.NameDay;
        public int cooldownShot { get; set; } = 6;
        public int damage { get; set; } = 30;
        public string jobType { get; set; } = "guard";
        public string npcType { get; set; } = SpearThrower.NameDay;
        public string onHitAudio { get; set; } = "fleshHit";
        public string onShootAudio { get; set; } = "sling";
        public int range { get; set; } = 10;
        public IRecruitmentitem recruitmentItem { get; set; } = new Recruitmentitem() { type = LeafSash.NAME };
        public IShootrequirement[] shootRequirements { get; set; } = new[] { new Shootrequirement() { type = WoodenSpear.NAME } };
        public string sleepType { get; set; } = "Night";
    }

    public class SpearThrowerSettingsNight : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = SpearThrower.NameNight;
        public string printName { get; set; } = "Spear Thrower Night Guard";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 2.3f;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }
    public class SpearThrowerSettingsDay : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = SpearThrower.NameDay;
        public string printName { get; set; } = "Spear Thrower Day Guard";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 1.5f;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }

    public class SpearThrower
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(SpearThrower));
        public static string NameDay = Name + "Day";
        public static string NameNight = Name + "Night";
        public static int range = 8;
        public static int damage = 20;
        public static int cooldown = 3;
    }

    public class SpearThrowerTypeNigt : CSGenerateType
    {
        public override string typeName => SpearThrower.NameNight;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#474747";
    }
    public class SpearThrowerTypeDay : CSGenerateType
    {
        public override string typeName => SpearThrower.NameDay;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#474747";
    }

    public class SpearThrowerRecipeNight : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(WoodenSpear.NAME)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SpearThrower.NameNight)
        };

        

        public string name => SpearThrower.NameNight;
    }
    public class SpearThrowerRecipeDay : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(WoodenSpear.NAME)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SpearThrower.NameDay)
        };

        

        public string name => SpearThrower.NameDay;
    }
}
