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
    public class Wood : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(Wood));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "Wood.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 200;
        public override List<string> categories { get; set; } = new List<string>()
        {
            "ingredient",
            "wood",
            "aa",
            "prehistory", 
            GameSetup.NAMESPACE
        };
    }

    public class WoodTemperateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Wood.NAME, 4)
        };

        public bool isOptional => false;

        public string name => Wood.NAME;
    }
    public class WoodTaigaRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTAIGA.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Wood.NAME, 4)
        };

        public bool isOptional => false;

        public string name => Wood.NAME;
    }
}
