using Jobs;
using Pandaros.API;
using Pandaros.API.Items;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
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
                new BlockJobManager<PandaGoalJob>(
                    new PrimitiveForager(),
                    (setting, pos, type, bytedata) => new PandaGoalJob(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new PandaGoalJob(setting, pos, type, colony)
                )
            );
        }
    }

    public class PrimitiveForagerSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = PrimitiveForager.Name;
        public string printName { get; set; } = "Primitive Forager";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 2.3f;
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
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.DIRT, 5, 10),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE, 5, 10),
                new LootPoolEntry(ColonyBuiltIn.ItemTypes.BERRY, 10, 20),
                new LootPoolEntry(Rock.NAME, 5, 10)
            }
        };

        public PrimitiveForager() : base(Name, Name, SharedLootTable, 20, 25, 2)
        {

        }
    }

    public class PrimitiveForagerJobType : CSType
    {
        public override string name => PrimitiveForager.Name;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "PrimitiveForager.png");
        public override string onPlaceAudio => CommonSounds.WoodPlace;
        public override string onRemoveAudio => CommonSounds.WoodDeleteLight;
        public override string sideall => RoughWoodenBoard.NAME;
        public override string mesh { get; set; } = GameSetup.MESH_PATH + "PrimitiveForager.ply";
        public override List<string> categories => new List<string>()
            {
                CommonCategories.Job,
                "aa",
                nameof(TimePeriod.PreHistory),
                CommonCategories.Forager,
                GameSetup.NAMESPACE
            };
    }

    public class PrimitiveForagerRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(RoughWoodenBoard.NAME, 4),
            new RecipeItem(LeafBasket.NAME),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 5),
             new RecipeItem(Rock.NAME, 4)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(PrimitiveForager.Name)
        };

        public string name => PrimitiveForager.Name + "player";
    }

    public class PrimitiveForagerJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(RoughWoodenBoard.NAME, 4),
            new RecipeItem(LeafBasket.NAME),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 5),
            new RecipeItem(Rock.NAME, 4)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(PrimitiveForager.Name)
        };

        public string name => PrimitiveForager.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }
}
