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
    public class SharpRock : CSType 
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(SharpRock));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "SharpRock.png");
        public override bool? isPlaceable => false;
        public override int? maxStackSize => 300;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Essential,
            CommonCategories.Ammo,
            "ab",
            nameof(TimePeriod.PreHistory),
            CommonCategories.Stone,
            GameSetup.NAMESPACE
        };
    }

    public class SharpRockRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Rock.NAME, 2)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SharpRock.NAME, 2)
        };

        public bool isOptional => false;

        public string name => SharpRock.NAME;
    }
}
