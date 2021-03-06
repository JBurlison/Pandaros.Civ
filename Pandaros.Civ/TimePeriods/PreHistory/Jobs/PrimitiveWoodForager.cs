﻿using Jobs;
using Pandaros.API;
using Pandaros.API.Items;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.StoneAge.Items;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    [ModLoader.ModManager]
    public static class PrimitiveWoodForagerModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.PrimitiveWoodForagerModEntry")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<PandaGoalJob>(
                    new PrimitiveWoodForager(),
                    (setting, pos, type, bytedata) => new PandaGoalJob(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new PandaGoalJob(setting, pos, type, colony)
                )
            );
        }
    }

    public class PrimitiveWoodForagerSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = PrimitiveWoodForager.Name;
        public string printName { get; set; } = "Primitive Wood Forager";
        public float inventoryCapacity { get; set; } = 150f;
        public float movementSpeed { get; set; } = 2f;
        public UnityEngine.Color32 maskColor1 { get; set; } = new UnityEngine.Color32(66, 255, 129, 255);
        public UnityEngine.Color32 maskColor0 { get; set; }
    }


    public class PrimitiveWoodForager : ForagingJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(PrimitiveWoodForager));
        public static LootTable SharedLootTable { get; set; } = new LootTable()
        {
            name = Name,
            LootPoolList = new List<LootPoolEntry>()
            {
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LOGTAIGA, 1, 5),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LOGTEMPERATE, 1, 5),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE, 1, 3), // in random the max cannot be rolled. https://docs.microsoft.com/en-us/dotnet/api/system.random.next?view=netcore-3.1#System_Random_Next_System_Int32_System_Int32_
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LEAVESTAIGA, 1, 3), // in random the max cannot be rolled. 
                new LootPoolEntry(Stick.NAME, 1, 3, 0.8f) // these are crafted
            }
        };

        public PrimitiveWoodForager() : base(Name, Name, SharedLootTable, 30, 40, 0)
        {

        }
    }

    public class PrimitiveWoodForagerJobType : CSGenerateType
    {
        public override string typeName => PrimitiveWoodForager.Name;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#00FF00";
    }
}
