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
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.PreHistory) + "/" + "Wood.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 200;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Ingredient,
            CommonCategories.Wood,
            nameof(TimePeriod.PreHistory),
            GameSetup.NAMESPACE
        };
    }

    public class WoodTaigaJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTAIGA.Id, 1),
            new RecipeItem(Rock.NAME)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Wood.NAME, 4)
        };

        

        public string name => Wood.NAME + ColonyBuiltIn.ItemTypes.LOGTAIGA.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }

    public class WoodTemperateJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Id, 1),
            new RecipeItem(Rock.NAME)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Wood.NAME, 4)
        };

        
        public string name => Wood.NAME + ColonyBuiltIn.ItemTypes.LOGTEMPERATE.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }
}
