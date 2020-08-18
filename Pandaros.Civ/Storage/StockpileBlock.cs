using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BlockTypes;
using Newtonsoft.Json.Linq;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.API.localization;
using Pandaros.API.Models;
using Pipliz;
using Pipliz.JSON;

namespace Pandaros.Civ.Storage
{
    public class StockpileBlockTexture : CSTextureMapping
    {
        public override string name => StockpileBlock.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "StockpileBlock.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "StockpileBlock.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "StockpileBlock.png");
    }

    public class StockpileBlock : StorageUpgradeBlockBase, IAfterItemTypesDefined, IOnSendAreaHighlights, IOnTryChangeBlock, IOnLoadingColony, IOnSavingColony
    {
        public static string Name { get; } = GameSetup.GetNamespace("Storage", "StockpileBlock");
        public static LocalizationHelper LocalizationHelper { get; set; } = new LocalizationHelper(GameSetup.NAMESPACE, "Stockpile");
        public static Dictionary<TimePeriods, (Vector3Int, Vector3Int)> StockpileSizes { get; set; } = new Dictionary<TimePeriods, (Vector3Int, Vector3Int)>()
        {
            {
                TimePeriods.PreHistory, (new Vector3Int(2,0,2), new Vector3Int(-2, 1, -2))
            },
            {
                TimePeriods.StoneAge, (new Vector3Int(2,0,2), new Vector3Int(-2, 1, -2))
            },
            {
                TimePeriods.BronzeAge, (new Vector3Int(3,0,3), new Vector3Int(-3, 2, -3))
            },
            {
                TimePeriods.IronAge, (new Vector3Int(5,0,3), new Vector3Int(-5, 3, -3))
            },
            {
                TimePeriods.IndustrialAge, (new Vector3Int(7,0,4), new Vector3Int(-7, 3, -4))
            },
            {
                TimePeriods.AtomicAge, (new Vector3Int(8,0,4), new Vector3Int(-8, 4, -4))
            },
            {
                TimePeriods.InformationAge, (new Vector3Int(9,0,4), new Vector3Int(-9, 5, -4))
            },
            {
                TimePeriods.RenewablesAge, (new Vector3Int(10,0,5), new Vector3Int(-10, 5, -5))
            },
            {
                TimePeriods.FusionAge, (new Vector3Int(10,0,7), new Vector3Int(-10, 7, -7))
            }
        };
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
            GlobalUpgrade = 32;
            CategoryUpgrades.Add("food", 32);
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
                    list.Add(new AreaJobTracker.AreaHighlight(pos.Add(StockpileSizes[TimePeriods.PreHistory].Item1), pos.Add(StockpileSizes[TimePeriods.PreHistory].Item2), Shared.EAreaMeshType.ThreeDActive, Shared.EServerAreaType.Default));
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

                if (cs.Positions.ContainsKey(Name))
                {
                    tryChangeBlockData.CallbackConsumedResult = EServerChangeBlockResult.CancelledByCallback;
                    tryChangeBlockData.TypeNew = ColonyBuiltIn.ItemTypes.AIR;
                    PandaChat.Send(tryChangeBlockData.RequestOrigin.AsPlayer, LocalizationHelper, "StockpileAlreadyPlaced", ChatColor.red);
                }
                else
                    cs.Positions[Name] = tryChangeBlockData.Position;
            }
            else if (tryChangeBlockData.TypeOld.Name == Name &&
                tryChangeBlockData.TypeNew == ColonyBuiltIn.ItemTypes.AIR &&
                tryChangeBlockData.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player &&
                tryChangeBlockData.RequestOrigin.AsPlayer.ActiveColony != null)
            {
                //TODO: handle moving of stockpile
            }
        }

        public void OnLoadingColony(Colony c, JSONNode n)
        {
            throw new NotImplementedException();
        }

        public void OnSavingColony(Colony c, JSONNode n)
        {
            throw new NotImplementedException();
        }
    }
}
