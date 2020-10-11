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
    public static class PrimitiveBerryForagerModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.PrimitiveBerryForagerModEntry")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<PandaGoalJob>(
                    new PrimitiveBerryForager(),
                    (setting, pos, type, bytedata) => new PandaGoalJob(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new PandaGoalJob(setting, pos, type, colony)
                )
            );
        }
    }

    public class PrimitiveBerryForagerSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = PrimitiveBerryForager.Name;
        public string printName { get; set; } = "Primitive Berry Forager";
        public float inventoryCapacity { get; set; } = 150f;
        public float movementSpeed { get; set; } = 2f;
        public UnityEngine.Color32 maskColor1 { get; set; } = new UnityEngine.Color32(161, 58, 47, 255);
        public UnityEngine.Color32 maskColor0 { get; set; }
    }


    public class PrimitiveBerryForager : ForagingJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(PrimitiveBerryForager));
        public static LootTable SharedLootTable { get; set; } = new LootTable()
        {
            name = Name,
            LootPoolList = new List<LootPoolEntry>()
            {
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.BERRY, 4, 8),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.BERRY, 2, 5, 0.4f),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.BERRY, 2, 4, 0.4f),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.BERRY, 2, 3, 0.1f)
            }
        };

        public PrimitiveBerryForager() : base(Name, Name, SharedLootTable, 40, 60, 0)
        {

        }
    }

    public class PrimitiveBerryForagerJobType : CSGenerateType
    {
        public override string typeName => PrimitiveBerryForager.Name;
        public override string generateType => "jobOutline";
        public override string outlineColor => "#a13a2f";
    }
}
