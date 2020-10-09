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

namespace Pandaros.Civ.TimePeriods.StoneAge.Items
{
    public class WovenBasketTexture : CSTextureMapping
    {
        public override string name => WovenBasket.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "WovenBasket.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "WovenBasket.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "WovenBasket.png");
    }

    public class WovenBasket : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("TimePeriods.StoneAge.Items", nameof(WovenBasket));
        public Dictionary<string, int> CategoryStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalStorageUpgrade { get; set; }

        public WovenBasket()
        {
            name = Name;
            GlobalStorageUpgrade = 16;
            CategoryStorageUpgrades.Add("food", 16);
            sideall = GameSetup.Textures.SELF;
            categories = new List<string>()
            {
                CommonCategories.Essential,
                CommonCategories.Storage,
                CommonCategories.StockpileUpgrade,
                "ba",
                nameof(TimePeriod.StoneAge),
                GameSetup.NAMESPACE
            };
            onPlaceAudio = CommonSounds.WoodPlace;
            onRemoveAudio = CommonSounds.WoodDeleteLight;
            isSolid = true;
            icon = GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.StoneAge) + "/" + "WovenBasket.png");
        }
    }

    public class WovenBasketRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.PLANKS.Id),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(WovenBasket.Name)
        };

        public bool isOptional => true;

        public string name => WovenBasket.Name;
    }
}
