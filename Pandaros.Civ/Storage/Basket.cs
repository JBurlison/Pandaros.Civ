using Newtonsoft.Json.Linq;
using Pandaros.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class BasketTexture : CSTextureMapping
    {
        public override string name => Basket.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "basket.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "basket.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "basket.png");
    }

    public class Basket : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "Basket");
        public Dictionary<string, int> CategoryUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalUpgrade { get; set; }

        public Basket()
        {
            name = Name;
            GlobalUpgrade = 8;
            CategoryUpgrades.Add("food", 8);
            sideall = GameSetup.Textures.SELF;
            categories = new List<string>()
            {
                "storage"
            };
            isSolid = true;
            customData = JObject.Parse("{ \"attachBehaviour\" : [{\"id\":\"crate\"}] }");
            icon = GameSetup.Textures.GetPath(TextureType.icon, "basket.png");
        }
    }
}
