using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.StoneAge.Items
{
    public class LeafRope : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.StoneAge.Items", nameof(LeafRope));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.StoneAge) + "/" + "LeafRope.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 50;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Ingredient,
            CommonCategories.Stone,
            nameof(TimePeriod.StoneAge),
            GameSetup.NAMESPACE
        };
    }

    public class LeafRopeTemperateRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Name, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafRope.NAME, 1)
        };

        public string name => LeafRope.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE;

        public CraftPriority defaultPriority => CraftPriority.Medium;

        public int defaultLimit => 10;

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string> { WoodWorker.Name };
    }
    public class LeafRopeTaigaRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Name, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafRope.NAME, 1)
        };

        public string name => LeafRope.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA;

        public CraftPriority defaultPriority => CraftPriority.Medium;

        public int defaultLimit => 10;

        public string Job => WoodWorker.Name;

        public List<string> JobBlock => new List<string> { WoodWorker.Name };
    }
}
