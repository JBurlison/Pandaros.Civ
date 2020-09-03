using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Items
{
    public class RoughWoodenBoard : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(RoughWoodenBoard));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "RoughWoodBoard.png");
        public override int? maxStackSize => 300;
        public override string sideall { get; set; } = NAME;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Ingredient,
            CommonCategories.Wood,
            nameof(TimePeriod.PreHistory),
            GameSetup.NAMESPACE
        };
    }

    public class RoughWoodenBoardTexture : CSTextureMapping
    {
        public override string name => RoughWoodenBoard.NAME;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "RoughWoodBoard.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "RoughWoodBoard.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "RoughWoodBoard.png");
    }

    public class RoughWoodenBoardRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(RoughWoodenBoard.NAME, 1)
        };

        public bool isOptional => false;

        public string name => RoughWoodenBoard.NAME;

        public CraftPriority defaultPriority => CraftPriority.Medium;

        public int defaultLimit => 10;

        public string Job => WoodWorker.Name;

        public string JobBlock => WoodWorker.Name;
    }
}
