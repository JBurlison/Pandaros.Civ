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

namespace Pandaros.Civ.IndustrialAge.Items
{
    /*public class PalletTexture : CSTextureMapping
    {
        public override string name => Pallet.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "Pallet.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "Pallet.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "Pallet.png");
    }*/

    public class Pallet : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "Pallet");
        public Dictionary<string, int> CategoryStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalStorageUpgrade { get; set; }

        public Pallet()
        {
            name = Name;
            GlobalStorageUpgrade = 128;
            CategoryStorageUpgrades.Add("food", 128);
            mesh = "gamedata/meshes/trader.ply";
            //sideall = GameSetup.Textures.SELF;
            categories = new List<string>()
            {
                "essential",
                "storage",
                "upgrade",
                "ea",
                "industrialage",
                GameSetup.NAMESPACE
            };
            isSolid = true;
            icon = "gamedata/textures/icons/trader.png";
            //icon = GameSetup.Textures.GetPath(TextureType.icon, "Pallet.png");
        }
    }

    public class ShelvingRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.PLANKS.Id, 5),
            new RecipeItem(ColonyBuiltIn.ItemTypes.COPPERNAILS.Id, 6)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(Pallet.Name)
        };

        public bool isOptional => false;

        public string name => Pallet.Name;
    }
}
