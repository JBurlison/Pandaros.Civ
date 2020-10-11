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
    public static class PrimitiveRockForagerModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.PrimitiveRockForagerModEntry")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<PandaGoalJob>(
                    new PrimitiveRockForager(),
                    (setting, pos, type, bytedata) => new PandaGoalJob(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new PandaGoalJob(setting, pos, type, colony)
                )
            );
        }
    }

    public class PrimitiveRockForagerSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = PrimitiveRockForager.Name;
        public string printName { get; set; } = "Primitive Rock Forager";
        public float inventoryCapacity { get; set; } = 150f;
        public float movementSpeed { get; set; } = 2f;
        public UnityEngine.Color32 maskColor1 { get; set; } = new UnityEngine.Color32(130, 123, 109, 255);
        public UnityEngine.Color32 maskColor0 { get; set; }
    }


    public class PrimitiveRockForager : ForagingJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(PrimitiveRockForager));
        public static LootTable SharedLootTable { get; set; } = new LootTable()
        {
            name = Name,
            LootPoolList = new List<LootPoolEntry>()
            {
                new LootPoolEntry(Rock.NAME, 10, 20),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.DIRT, 1, 2, 0.4f)
            }
        };

        public PrimitiveRockForager() : base(Name, Name, SharedLootTable, 30, 40, 0)
        {

        }
    }

    public class PrimitiveRockForagerJobType : CSGenerateType
    {
        public override string typeName => PrimitiveRockForager.Name;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#827b6d";
    }
}
