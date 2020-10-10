using Pandaros.API;
using Pandaros.API.Models;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;

namespace Pandaros.Civ.TimePeriods.StoneAge.Items
{
    public class LeafBasket : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.StoneAge.Items", nameof(LeafBasket));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.PreHistory) + "/" + "LeafBasket.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 5;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Essential,
            CommonCategories.Storage,
            CommonCategories.Bag,
            nameof(TimePeriod.PreHistory),
            GameSetup.NAMESPACE
        };
    }

    public class LeafBasketTemperateJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 1),
            new RecipeItem(SharpRock.NAME),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBasket.NAME)
        };

        

        public string name => LeafBasket.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string>();
    }

    public class LeafBasketTaigaJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 1),
            new RecipeItem(SharpRock.NAME),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBasket.NAME)
        };

        

        public string name => LeafBasket.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string>();
    }
}
