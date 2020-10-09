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
    public class StoneWheel : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.StoneAge.Items", nameof(StoneWheel));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.StoneAge) + "/" + "StoneWheel.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 25;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Ingredient,
            CommonCategories.Stone,
            nameof(TimePeriod.StoneAge),
            GameSetup.NAMESPACE
        };
    }

    public class StoneWheelRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(StoneBrick.NAME, 2),
            new RecipeItem(LeafRope.NAME, 1),
            new RecipeItem(Stick.NAME, 3)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(StoneWheel.NAME, 1)
        };

        public string name => StoneWheel.NAME;

        public CraftPriority defaultPriority => CraftPriority.Medium;

        public int defaultLimit => 5;

        public string Job => StoneShaper.Name;

        public List<string> JobBlock => new List<string> { StoneShaper.Name };
    }
}
