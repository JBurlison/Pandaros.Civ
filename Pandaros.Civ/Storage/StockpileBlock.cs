using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BlockTypes;
using ModLoaderInterfaces;
using Newtonsoft.Json.Linq;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.API.localization;
using Pandaros.API.Models;
using Pandaros.Civ.TimePeriods;
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

    public class StockpileBlock : CSType, IStorageUpgradeBlock, IAfterItemTypesDefined, IOnSendAreaHighlights, IOnTryChangeBlock, IOnLoadingColony, IOnSavingColony
    {
        public static string Name { get; } = GameSetup.GetNamespace("Storage", "StockpileBlock");
        public static LocalizationHelper LocalizationHelper { get; set; } = new LocalizationHelper(GameSetup.NAMESPACE, "Stockpile");
        public static Dictionary<TimePeriod, (Vector3Int, Vector3Int)> StockpileSizes { get; set; } = new Dictionary<TimePeriod, (Vector3Int, Vector3Int)>()
        {
            {
                TimePeriod.PreHistory, (new Vector3Int(2,0,2), new Vector3Int(-2, 1, -2))
            },
            {
                TimePeriod.StoneAge, (new Vector3Int(2,0,2), new Vector3Int(-2, 1, -2))
            },
            {
                TimePeriod.BronzeAge, (new Vector3Int(3,0,3), new Vector3Int(-3, 2, -3))
            },
            {
                TimePeriod.IronAge, (new Vector3Int(5,0,3), new Vector3Int(-5, 3, -3))
            },
            {
                TimePeriod.IndustrialAge, (new Vector3Int(7,0,4), new Vector3Int(-7, 3, -4))
            },
            {
                TimePeriod.AtomicAge, (new Vector3Int(8,0,4), new Vector3Int(-8, 4, -4))
            },
            {
                TimePeriod.InformationAge, (new Vector3Int(9,0,4), new Vector3Int(-9, 5, -4))
            },
            {
                TimePeriod.RenewablesAge, (new Vector3Int(10,0,5), new Vector3Int(-10, 5, -5))
            },
            {
                TimePeriod.FusionAge, (new Vector3Int(10,0,7), new Vector3Int(-10, 7, -7))
            }
        };
        public Dictionary<string, int> CategoryStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemStorageUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalStorageUpgrade { get; set; }

        public StockpileBlock()
        {
            name = Name;
            sideall = GameSetup.Textures.SELF;
            isSolid = true;
            categories = new List<string>()
            {
                "essential",
                "storage"
            };
            icon = GameSetup.Textures.GetPath(TextureType.icon, "StockpileBlock.png");
            GlobalStorageUpgrade = 100;
            CategoryStorageUpgrades.Add("food", 50);
        }

        public void AfterItemTypesDefined()
        {
            StarterPacks.Manager.PrimaryStockpileStart.Items.Add(new InventoryItem(name)); // = new List<InventoryItem>() { new InventoryItem(name) };
        }

        public void OnSendAreaHighlights(Players.Player player, List<AreaJobTracker.AreaHighlight> list, List<ushort> showWhileHoldingTypes)
        {
            if (player.ActiveColony != null)
            {
                var cs = ColonyState.GetColonyState(player.ActiveColony);
                
                if (cs.Positions.TryGetValue(Name, out var pos))
                {
                    var currentPeriod = PeriodFactory.GetTimePeriod(player.ActiveColony);
                    list.Add(new AreaJobTracker.AreaHighlight(pos.Add(StockpileSizes[currentPeriod].Item1), 
                                                              pos.Add(StockpileSizes[currentPeriod].Item2), Shared.EAreaMeshType.ThreeD, Shared.EServerAreaType.Default));
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
                AreaJobTracker.SendData(tryChangeBlockData.RequestOrigin.AsPlayer.ActiveColony);
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
                var cs = ColonyState.GetColonyState(tryChangeBlockData.RequestOrigin.AsPlayer.ActiveColony);
                cs.Positions.Remove(Name);
                AreaJobTracker.SendData(tryChangeBlockData.RequestOrigin.AsPlayer.ActiveColony);
                //TODO: handle moving of stockpile
            }
        }

        public void OnLoadingColony(Colony c, JSONNode n)
        {
            
        }

        public void OnSavingColony(Colony c, JSONNode n)
        {
            
        }
    }
}
