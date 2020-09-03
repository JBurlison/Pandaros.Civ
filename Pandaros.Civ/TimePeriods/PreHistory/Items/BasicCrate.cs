using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.PreHistory.Items
{

    public class BasicCrate : CSType, ICrate
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "BasicCrate");
        public int MaxCrateStackSize { get; set; } = 100;
        public int MaxNumberOfStacks { get; set; } = 5;

        public BasicCrate()
        {
            name = Name;
            sideall = "planks";
            categories = new List<string>()
            {
                CommonCategories.Essential,
                CommonCategories.Storage,
                CommonCategories.Crate,
                CommonCategories.HighPriority,
                nameof(TimePeriod.PreHistory),
                GameSetup.NAMESPACE
            };
            isSolid = true;
            colliders = new Colliders()
            {
                collidePlayer = true,
                collideSelection = true,
                boxes = new List<Colliders.Boxes>()
                {
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0.5f, -0.4f }, new List<float>(){ -0.5f, -0.5f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0.5f, 0.5f }, new List<float>(){ -0.5f, -0.5f, 0.4f }),
                    new Colliders.Boxes(new List<float>(){ -0.4f, 0.5f, 0.5f }, new List<float>(){ -0.5f, -0.5f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0.5f, 0.5f }, new List<float>(){ 0.4f, -0.5f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, -0.4f, 0.5f }, new List<float>(){ -0.5f, -0.5f, -0.5f })
                }
            };
            onPlaceAudio = "woodPlace";
            onRemoveAudio = "woodDeleteLight";
            icon = GameSetup.Textures.GetPath(TextureType.icon, "BasicCrate.png");
            mesh = GameSetup.MESH_PATH + "crate.obj";
        }
    }

    public class BasicCrateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 2),
            new RecipeItem(RoughWoodenBoard.NAME, 5),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(BasicCrate.Name)
        };

        public bool isOptional => false;

        public string name => BasicCrate.Name;
    }

    public class BasicCrateWoodWorkerRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 2),
            new RecipeItem(RoughWoodenBoard.NAME, 5),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(BasicCrate.Name)
        };

        public bool isOptional => false;

        public string name => BasicCrate.Name + TimePeriods.PreHistory.Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Low;
        public int defaultLimit => 5;

        public string Job => TimePeriods.PreHistory.Jobs.WoodWorker.Name;

        public string JobBlock => TimePeriods.PreHistory.Jobs.WoodWorker.Name;
    }
}
