using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class PrimitiveStorageBlock : StorageUpgradeBlockBase
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "PrimitiveStorageBlock");

        public PrimitiveStorageBlock()
        {
            name = Name;
            GlobalUpgrade = 8;
            CategoryUpgrades.Add("food", 8);
            sideall = StockpileBlock.Name;
            isSolid = true;
            customData = JObject.Parse("{ \"attachBehaviour\" : [{\"id\":\"crate\"}] }");
            icon = GameSetup.Textures.GetPath(TextureType.icon, "StockpileBlock.png");
        }
    }
}
