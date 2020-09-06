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
    public class LeafBasket : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(LeafBasket));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "LeafBasket.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 5;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Essential,
            CommonCategories.Storage,
            CommonCategories.Bag,
            "aa",
            nameof(TimePeriod.PreHistory),
            GameSetup.NAMESPACE
        };
    }

    public class LeafBasketTemperateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 1),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 10)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBasket.NAME)
        };

        

        public string name => LeafBasket.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE;
    }
    public class LeafBasketTaigaRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 1),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Id, 10)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBasket.NAME)
        };

        

        public string name => LeafBasket.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA;
    }
    public class LeafBasketTemperateJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 1),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 10)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBasket.NAME)
        };

        

        public string name => LeafBasket.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE + Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }
    public class LeafBasketTaigaJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 1),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Id, 10)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBasket.NAME)
        };

        

        public string name => LeafBasket.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA + Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }
}
