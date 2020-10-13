using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.StoneAge.Items
{
    public class SharpRock : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.StoneAge.Items", nameof(SharpRock));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.StoneAge) + "/" + "SharpRock.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 300;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Ingredient,
            CommonCategories.Stone,
            nameof(TimePeriod.StoneAge),
            GameSetup.NAMESPACE
        };
    }

    public class SharpRockRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME, 2)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SharpRock.NAME, 1)
        };

        public string name => SharpRock.NAME;

        public CraftPriority defaultPriority => CraftPriority.Medium;

        public int defaultLimit => 10;

        public string Job => StoneShaper.Name;

        public List<string> JobBlock => new List<string> { StoneShaper.Name };
    }

    public class SharpRockPlayerRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME, 2)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SharpRock.NAME, 1)
        };

        public string name => SharpRock.NAME + "player";
    }
}
