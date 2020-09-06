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

namespace Pandaros.Civ.TimePeriods.IronAge.Items
{
    public class StorageDrawersTexture : CSTextureMapping
    {
        public override string name => StorageDrawers.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "StorageDrawers.png");
        //public override string height => GameSetup.Textures.GetPath(TextureType.height, "StorageDrawers.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "StorageDrawers.png");
    }

    public class StorageDrawers : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("TimePeriods.IronAge.Items", nameof(StorageDrawers));
        public Dictionary<string, int> CategoryStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalStorageUpgrade { get; set; }

        public StorageDrawers()
        {
            name = Name;
            GlobalStorageUpgrade = 64;
            CategoryStorageUpgrades.Add("food", 64);
            sidexp = StorageDrawers.Name;
            sideall = "planks";
            categories = new List<string>()
            {
                CommonCategories.Essential,
                CommonCategories.Storage,
                CommonCategories.StockpileUpgrade,
                "da",
                nameof(TimePeriod.IronAge),
                GameSetup.NAMESPACE
            };
            onPlaceAudio = "woodPlace";
            onRemoveAudio = "woodDeleteHeavy";
            isSolid = true;
            icon = GameSetup.Textures.GetPath(TextureType.icon, "StorageDrawers.png");
        }
    }

    public class ShelvingRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.IRONINGOT.Id, 5),
            new RecipeItem(ColonyBuiltIn.ItemTypes.COATEDPLANKS.Id, 6)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(StorageDrawers.Name)
        };

        public bool isOptional => true;

        public string name => StorageDrawers.Name;
    }
}
