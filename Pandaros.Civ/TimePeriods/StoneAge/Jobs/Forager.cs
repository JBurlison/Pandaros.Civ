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
        public string printName { get; set; } = "Forager";
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
    /*public class ForagerJobType : CSType
    {
        public override string name => Forager.Name;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.StoneAge) + "/" + "Forager.png");
        public override string onPlaceAudio => CommonSounds.WoodPlace;
        public override string onRemoveAudio => CommonSounds.WoodDeleteLight;
        public override string sideall => Forager.Name;
        public override string mesh { get; set; } = GameSetup.MESH_PATH + "Forager.ply";
        public override List<string> categories => new List<string>()
            {
                CommonCategories.Job,
                "aa",
                nameof(TimePeriod.StoneAge),
                CommonCategories.Forager,
                GameSetup.NAMESPACE
            };
    }
    public class RoughWoodenBoardTexture : CSTextureMapping
    {
        public override string name => Forager.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "Forager.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "Forager.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "Forager.png");
    }*/

    public class ForagerRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.PLANKS.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Forager.Name)
        };

        public string name => Forager.Name + "player";
    }

    /*public class ForagerJobRecipe : ICSRecipe
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
            new RecipeResult(Forager.Name)
        };

        public string name => Forager.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => PreHistory.Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { PreHistory.Jobs.WoodWorker.Name };
    }*/
}
