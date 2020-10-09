using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.StoneAge.Items
{
    public class WoodenCart : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.StoneAge.Items", nameof(WoodenCart));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.StoneAge) + "/" + "WoodenCart.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 1;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Ingredient,
            CommonCategories.Stone,
            nameof(TimePeriod.StoneAge),
            GameSetup.NAMESPACE
        };
    }

    public class WoodenCartRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(StoneWheel.NAME, 2),
            new RecipeItem(ColonyBuiltIn.ItemTypes.PLANKS.Name, 10),
            new RecipeItem(Wood.NAME, 5),
            new RecipeItem(LeafRope.NAME, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(WoodenCart.NAME, 1)
        };

        public string name => WoodenCart.NAME;

        public CraftPriority defaultPriority => CraftPriority.Medium;

        public int defaultLimit => 5;

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string> { WoodWorker.Name };
    }
}
