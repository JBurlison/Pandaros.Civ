using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.API.Models;

namespace Pandaros.Civ.Storage
{
    public class StockpileBlockTexture : CSTextureMapping
    {
        public override string name => StockpileBlock.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "StockpileBlock.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "StockpileBlock.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "StockpileBlock.png");
    }

    public class StockpileBlock : StorageUpgradeBlockBase, IAfterItemTypesDefined, IOnSendAreaHighlights, IOnTryChangeBlock
    {
        public static string Name { get; } = GameSetup.GetNamespace("Storage", "StockpileBlock");

        public StockpileBlock()
        {
            name = Name;
            sideall = GameSetup.Textures.SELF;
            isSolid = true;
            categories = new List<string>()
            {
                "essential"
            };
            customData = JObject.Parse("{ \"attachBehaviour\" : [{\"id\":\"crate\"}] }");
            icon = GameSetup.Textures.GetPath(TextureType.icon, "StockpileBlock.png");
            GlobalUpgrade = 36;
            CategoryUpgrades.Add("food", 100);
        }

        public void AfterItemTypesDefined()
        {
            StarterPacks.Manager.PrimaryStockpileStart.Items.Add(new InventoryItem(name));
        }

        public void OnSendAreaHighlights(Players.Player player, List<AreaJobTracker.AreaHighlight> list, List<ushort> showWhileHoldingTypes)
        {
            if (player.ActiveColony != null)
            {
                var cs = ColonyState.GetColonyState(player.ActiveColony);

                if (cs.Positions.TryGetValue(Name, out var pos))
                {
                    list.Add(new AreaJobTracker.AreaHighlight(pos.Add(2, 0, 2), pos.Add(-2, 2, -2), Shared.EAreaMeshType.ThreeD, Shared.EServerAreaType.Default));
                }
            }

        }

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData tryChangeBlockData)
        {
            if (tryChangeBlockData.TypeNew.Name == Name &&
                tryChangeBlockData.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player &&
                tryChangeBlockData.RequestOrigin.AsPlayer.ActiveColony != null)
            {
                var cs = ColonyState.GetColonyState(tryChangeBlockData.RequestOrigin.AsPlayer.ActiveColony);
                cs.Positions[Name] = tryChangeBlockData.Position;
            }
        }
    }
}
