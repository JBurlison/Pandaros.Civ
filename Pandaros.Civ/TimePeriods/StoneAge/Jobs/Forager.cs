using Jobs;
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

namespace Pandaros.Civ.TimePeriods.StoneAge.Jobs
{
    [ModLoader.ModManager]
    public static class ForagerModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.StoneAge.Jobs.ForagerModEntry")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<PandaGoalJob>(
                    new Forager(),
                    (setting, pos, type, bytedata) => new PandaGoalJob(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new PandaGoalJob(setting, pos, type, colony)
                )
            );
        }
    }

    public class ForagerSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = Forager.Name;
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 2.3f;
        public UnityEngine.Color32 maskColor1 { get; set; } = new UnityEngine.Color32(66, 255, 129, 255);
        public UnityEngine.Color32 maskColor0 { get; set; }
    }


    public class Forager : ForagingJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.StoneAge.Jobs", nameof(Forager));
        public static LootTable SharedLootTable { get; set; } = new LootTable()
        {
            name = Name,
            LootPoolList = new List<LootPoolEntry>()
            {
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.BERRY, 15, 30),
                new LootPoolEntry(Rock.NAME, 10, 15),
                new LootPoolEntry(Wood.NAME, 10, 25, 0.15f),
                new LootPoolEntry(Stick.NAME, 10, 30, 0.4f),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE, 10, 15, 0.5f),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LEAVESTAIGA, 10, 15, 0.5f)
            }
        };
        //public static InventoryItem Recruitmentitem { get; set; } = new InventoryItem(WoodenCart.NAME);
        public Forager() : base(Name, Name, SharedLootTable, 20, 25, 2)
        {

        }
    }

    public class ForagerJobType : CSGenerateType
    {
        public override string typeName => Forager.Name;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#00FF00";
    }
}
