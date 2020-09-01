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

namespace Pandaros.Civ.BronzeAge.Items
{
    public class ShelvingTexture : CSTextureMapping
    {
        public override string name => Shelving.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "Shelving.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "Shelving.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "Shelving.png");
    }

    public class Shelving : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "Shelving");
        public Dictionary<string, int> CategoryStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalStorageUpgrade { get; set; }

        public Shelving()
        {
            name = Name;
            GlobalStorageUpgrade = 32;
            CategoryStorageUpgrades.Add("food", 32);
            sideall = GameSetup.Textures.SELF;
            categories = new List<string>()
            {
                "storage",
                GameSetup.NAMESPACE
            };
            isSolid = true;
            icon = GameSetup.Textures.GetPath(TextureType.icon, "Shelving.png");
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
            new RecipeResult(Shelving.Name)
        };

        public bool isOptional => false;

        public string name => Shelving.Name;
    }
}
