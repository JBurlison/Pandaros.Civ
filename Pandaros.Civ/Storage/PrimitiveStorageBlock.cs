using Newtonsoft.Json.Linq;
using Pandaros.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class PrimitiveStorageBlockTexture : CSTextureMapping
    {
        public override string name => PrimitiveStorageBlock.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "StockpileBlock.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "StockpileBlock.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "StockpileBlock.png");
    }

    public class PrimitiveStorageBlock : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "PrimitiveStorageBlock");
        public Dictionary<string, int> CategoryUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalUpgrade { get; set; }

        public PrimitiveStorageBlock()
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
            icon = GameSetup.Textures.GetPath(TextureType.icon, "StockpileBlock.png");
        }
    }
}
