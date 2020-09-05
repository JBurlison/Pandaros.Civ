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

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    [ModLoader.ModManager]
    public static class RockThrowerModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.RockThrowerModEntries")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<GuardJobInstance>(
                    new RockThrower(),
                    (setting, pos, type, bytedata) => new RockThrower(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new RockThrower(setting, pos, type, colony)
                )
            );
        }
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

    public class RockThrower : PandaGuardJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(RockThrower));

        public RockThrower() : base()
        {
            NPCType.AddSettings(new NPCTypeStandardSettings
            {
                keyName = Name,
                printName = "Rock Thrower",
                maskColor1 = new UnityEngine.Color32(51, 51, 77, 255),
                type = NPCTypeID.GetNextID(),
                inventoryCapacity = 500f
        });

            NPCType = NPCType.GetByKeyNameOrDefault(Name);
        }
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
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(RockThrower.Name)
        };

        public bool isOptional => false;

        public string name => RockThrower.Name;
    }
}
