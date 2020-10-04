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
using Pandaros.Civ.TimePeriods.StoneAge.Items;

namespace Pandaros.Civ.TimePeriods.StoneAge.Jobs
{
    public class StoneSpearThrowerGuardSettingsNight : ICSGuardJobSettings
    {
        public string blockType { get; set; } = StoneSpearThrower.NameNight;
        public int cooldownShot { get; set; } = StoneSpearThrower.cooldown;
        public int damage { get; set; } = StoneSpearThrower.damage;
        public string jobType { get; set; } = "guard";
        public string npcType { get; set; } = StoneSpearThrower.NameDay;
        public string onHitAudio { get; set; } = "fleshHit";
        public string onShootAudio { get; set; } = "sling";
        public int range { get; set; } = StoneSpearThrower.range;
        public IRecruitmentitem recruitmentItem { get; set; } = new Recruitmentitem() { type = LeafSash.NAME };
        public IShootrequirement[] shootRequirements { get; set; } = new[] { new Shootrequirement() { type = StoneSpear.NAME } };
        public string sleepType { get; set; } = "Day"; 
    }
    public class StoneSpearThrowerGuardSettingsDay : ICSGuardJobSettings
    {
        public string blockType { get; set; } = StoneSpearThrower.NameDay;
        public int cooldownShot { get; set; } = StoneSpearThrower.cooldown;
        public int damage { get; set; } = StoneSpearThrower.damage;
        public string jobType { get; set; } = "guard";
        public string npcType { get; set; } = StoneSpearThrower.NameDay;
        public string onHitAudio { get; set; } = "fleshHit";
        public string onShootAudio { get; set; } = "sling";
        public int range { get; set; } = StoneSpearThrower.range;
        public IRecruitmentitem recruitmentItem { get; set; } = new Recruitmentitem() { type = LeafSash.NAME };
        public IShootrequirement[] shootRequirements { get; set; } = new[] { new Shootrequirement() { type = StoneSpear.NAME } };
        public string sleepType { get; set; } = "Night";
    }

    public class StoneSpearThrowerSettingsNight : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = StoneSpearThrower.NameNight;
        public string printName { get; set; } = "Spear Thrower Night Guard";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 2.3f;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }
    public class StoneSpearThrowerSettingsDay : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = StoneSpearThrower.NameDay;
        public string printName { get; set; } = "Spear Thrower Day Guard";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 1.5f;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }

    public class StoneSpearThrower
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.StoneAge.Jobs", nameof(StoneSpearThrower));
        public static string NameDay = Name + "Day";
        public static string NameNight = Name + "Night";
        public static int range = 10;
        public static int damage = 25;
        public static int cooldown = 3;
    }

    public class StoneSpearThrowerTypeNigt : CSGenerateType
    {
        public override string typeName => StoneSpearThrower.NameNight;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#474747";
    }
    public class StoneSpearThrowerTypeDay : CSGenerateType
    {
        public override string typeName => StoneSpearThrower.NameDay;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#474747";
    }

    /*public class StoneSpearThrowerRecipeNight : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(WoodenSpear.NAME)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(StoneSpearThrower.NameNight)
        };

        

        public string name => StoneSpearThrower.NameNight;
    }
    public class StoneSpearThrowerRecipeDay : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(WoodenSpear.NAME)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(StoneSpearThrower.NameDay)
        };

        

        public string name => StoneSpearThrower.NameDay;
    }*/
}
