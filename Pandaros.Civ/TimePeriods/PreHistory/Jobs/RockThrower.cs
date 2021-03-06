﻿using Jobs;
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
using Steamworks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    public class RockThrowerGuardSettingsNight : ICSGuardJobSettings
    {
        public string blockType { get; set; } = RockThrower.NameNight;
        public int cooldownShot { get; set; } = RockThrower.cooldown;
        public int damage { get; set; } = RockThrower.damage;
        public string jobType { get; set; } = "guard";
        public string npcType { get; set; } = RockThrower.NameNight;
        public string onHitAudio { get; set; } = "fleshHit";
        public string onShootAudio { get; set; } = "sling";
        public int range { get; set; } = RockThrower.range;
        public IRecruitmentitem recruitmentItem { get; set; } = new Recruitmentitem() { type = LeafSash.NAME };
        public IShootrequirement[] shootRequirements { get; set; } = new[] { new Shootrequirement() { type = Rock.NAME } };
        public string sleepType { get; set; } = "Day"; 
    }
    public class RockThrowerGuardSettingsDay : ICSGuardJobSettings
    {
        public string blockType { get; set; } = RockThrower.NameDay;
        public int cooldownShot { get; set; } = RockThrower.cooldown;
        public int damage { get; set; } = RockThrower.damage;
        public string jobType { get; set; } = "guard";
        public string npcType { get; set; } = RockThrower.NameDay;
        public string onHitAudio { get; set; } = "fleshHit";
        public string onShootAudio { get; set; } = "sling";
        public int range { get; set; } = RockThrower.range;
        public IRecruitmentitem recruitmentItem { get; set; } = new Recruitmentitem() { type = LeafSash.NAME };
        public IShootrequirement[] shootRequirements { get; set; } = new[] { new Shootrequirement() { type = Rock.NAME } };
        public string sleepType { get; set; } = "Night";
    }

    public class RockThrowerSettingsNight : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = RockThrower.NameNight;
        public string printName { get; set; } = "Rock Thrower Night Guard";
        public float inventoryCapacity { get; set; } = RockThrower.inventory;
        public float movementSpeed { get; set; } = RockThrower.movement;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }
    public class RockThrowerSettingsDay : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = RockThrower.NameDay;
        public string printName { get; set; } = "Rock Thrower Day Guard";
        public float inventoryCapacity { get; set; } = RockThrower.inventory;
        public float movementSpeed { get; set; } = RockThrower.movement;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }

    public class RockThrower
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(RockThrower));
        public static string NameDay = Name + "Day";
        public static string NameNight = Name + "Night";
        public static int range = 6;
        public static int damage = 60;
        public static int cooldown = 6;
        public static float inventory = 25f;
        public static float movement = 2.5f;
    }

    public class RockThrowerTypeNigt : CSGenerateType
    {
        public override string typeName => RockThrower.NameNight;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#474747";
    }
    public class RockThrowerTypeDay : CSGenerateType
    {
        public override string typeName => RockThrower.NameDay;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#474747";
    }
}
