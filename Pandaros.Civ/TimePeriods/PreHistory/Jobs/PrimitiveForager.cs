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

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    [ModLoader.ModManager]
    public static class PrimitiveForagerModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.PrimitiveForagerModEntry")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<PandaGoalJob>(new PrimitiveForager())
            );
        }
    }

    public class PrimitiveForagerNPCSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = PrimitiveForager.Name;
        public string printName { get; set; } = "Primitive Forager";
        public float inventoryCapacity { get; set; } = 150f;
        public float movementSpeed { get; set; } = 2f;
        public UnityEngine.Color32 maskColor1 { get; set; } = new UnityEngine.Color32(66, 255, 129, 255);
        public UnityEngine.Color32 maskColor0 { get; set; }
    }


    public class PrimitiveForager : ForagingJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(PrimitiveForager));
        public static LootTable SharedLootTable { get; set; } = new LootTable()
        {
            name = Name,
            LootPoolList = new List<LootPoolEntry>()
            {
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.BERRY, 10, 20),
                new LootPoolEntry(Rock.NAME, 5, 10),
                new LootPoolEntry(Wood.NAME, 6, 15, 0.15f),
                new LootPoolEntry(Stick.NAME, 5, 20, 0.4f),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE, 5, 10, 0.5f),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LEAVESTAIGA, 5, 10, 0.5f)

            }
        };

        public PrimitiveForager() : base(Name, Name, SharedLootTable, 20, 25, 2)
        {

        }
    }

    public class PrimitiveForagerJobType : CSGenerateType
    {
        public override string typeName => PrimitiveForager.Name;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#00FF00";
    }
}
