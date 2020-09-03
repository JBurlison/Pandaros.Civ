using Pandaros.API;
using Pandaros.API.Models;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Items
{
    public class LeafBag : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(LeafBag));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "Backpack.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 5;
        public override List<string> categories { get; set; } = new List<string>()
        {
            "essential",
            "storage",
            "bag",
            "backpack", 
            "aa",
            nameof(TimePeriod.PreHistory),
            GameSetup.NAMESPACE
        };
    }

    public class LeafBagRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 1),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 10)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBag.NAME)
        };

        public bool isOptional => false;

        public string name => LeafBag.NAME;
    }
}
