using Newtonsoft.Json.Linq;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.PreHistory.Items
{
    public class CarvedLogTexture : CSTextureMapping
    {
        public override string name => CarvedLog.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "CarvedLog.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "CarvedLog.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "CarvedLog.png");
    }

    public class CarvedLog : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "CarvedLog");
        public Dictionary<string, int> CategoryStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalStorageUpgrade { get; set; }

        public CarvedLog()
        {
            name = Name;
            GlobalStorageUpgrade = 8;
            CategoryStorageUpgrades.Add("food", 8);
            sideall = GameSetup.Textures.SELF;
            categories = new List<string>()
            {
                "storage", 
                GameSetup.NAMESPACE
            };
            isSolid = true;
            icon = GameSetup.Textures.GetPath(TextureType.icon, "CarvedLog.png");
        }
    }

    public class CarvedLogRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LOGTAIGA.Id),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 1)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(CarvedLog.Name)
        };

        public bool isOptional => false;

        public string name => CarvedLog.Name;
    }
}
