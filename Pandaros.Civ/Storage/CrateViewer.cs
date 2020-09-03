using NetworkUI;
using NetworkUI.Items;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.localization;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs.Goals;
using Pipliz;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    [ModLoader.ModManager]
    public class CrateViewer
    {
        static readonly LocalizationHelper _localizationHelper = new LocalizationHelper(GameSetup.NAMESPACE, "storage");
        public static Dictionary<Players.Player, Vector3Int> LastCrateClick { get; set; } = new Dictionary<Players.Player, Vector3Int>();

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerClicked, GameSetup.NAMESPACE + ".Items.Crate.OpenMenu")]
        public static void OpenMenu(Players.Player player, PlayerClickedData playerClickData)
        {
            //Only launch on RIGHT click
            if (player == null || playerClickData.ClickType != PlayerClickedData.EClickType.Right || playerClickData.HitType != PlayerClickedData.EHitType.Block || player.ActiveColony == null)
                return;

            var voxel = playerClickData.GetVoxelHit();
            var itemHit = ItemId.GetItemId(voxel.TypeHit);

            if (StorageFactory.CrateTypes.ContainsKey(itemHit.Name) &&
                StorageFactory.CrateLocations.TryGetValue(player.ActiveColony, out var positions) && 
                positions.ContainsKey(voxel.BlockHit))
            {
                LastCrateClick[player] = voxel.BlockHit;
                NetworkMenuManager.SendServerPopup(player, MainMenu(player));
            }
        }

        public static bool TryGetPlayersCurrentCrate(Players.Player player, out CrateInventory crateInventory)
        {
            crateInventory = null;
            return player.ActiveColony != null &&
                    LastCrateClick.TryGetValue(player, out var cratePos) &&
                    StorageFactory.CrateLocations.TryGetValue(player.ActiveColony, out var crateLocs) &&
                    crateLocs.TryGetValue(cratePos, out crateInventory);
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerPushedNetworkUIButton, GameSetup.NAMESPACE + ".Items.Crate.PressButton")]
        public static void PressButton(ButtonPressCallbackData data)
        {
            if (data.Player == null || data.Player.ActiveColony == null)
                return;

            if (data.ButtonIdentifier == "Crate.GetItemsFromStockpile")
            {
                NetworkMenu menu = StockpileMenu(data);
                NetworkMenuManager.SendServerPopup(data.Player, menu);
            }
            else if (data.ButtonIdentifier == "Crate.MainMenu")
            {
                NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player));
            }
            else if (data.ButtonIdentifier == "Crate.GetItemsFromToolbar")
            {
                NetworkMenuManager.SendServerPopup(data.Player, ToolbarMenu(data));
            }
            else if (data.ButtonIdentifier == "Crate.SelectAllInCrate")
            {
                NetworkMenuManager.SendServerPopup(data.Player, StockpileMenu(data, false, true));
            }
            else if (data.ButtonIdentifier == "Crate.SelectNoneInCrate")
            {
                NetworkMenuManager.SendServerPopup(data.Player, StockpileMenu(data, false, false));
            }
            else if (data.ButtonIdentifier == "Crate.SelectAllInToolbar")
            {
                NetworkMenuManager.SendServerPopup(data.Player, ToolbarMenu(data, false, true));
            }
            else if (data.ButtonIdentifier == "Crate.SelectNoneInToolbar")
            {
                NetworkMenuManager.SendServerPopup(data.Player, ToolbarMenu(data, false, false));
            }
            else if (data.ButtonIdentifier == "Crate.SelectNoneInCrateMain")
            {
                NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player, false, false));
            }
            else if (data.ButtonIdentifier == "Crate.SelectAllInCrateMain")
            {
                NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player, false, true));
            }
            else if (data.ButtonIdentifier == "Crate.SelectNoneInCrateToolbar")
            {
                NetworkMenuManager.SendServerPopup(data.Player, ToolbarMenu(data, false, false));
            }
            else if (data.ButtonIdentifier == "Crate.SelectAllInCrateToolbar")
            {
                NetworkMenuManager.SendServerPopup(data.Player, ToolbarMenu(data, false, true));
            }
            else if (data.ButtonIdentifier == "Crate.MoveItemsToStockpile")
            {
                if (data.Storage.TryGetAs("Crate.NumberOfItems", out string strNumItems) && 
                    int.TryParse(strNumItems, out int numItems) &&
                    TryGetPlayersCurrentCrate(data.Player, out var crateInventory))
                {
                    List<InventoryItem> itemsMoved = new List<InventoryItem>();

                    foreach (var itemKvp in crateInventory.ContentCopy)
                    {
                        if (data.Storage.TryGetAs("Crate." + itemKvp.Key + ".ItemSelected", out bool selected) && selected)
                        {
                            int takeNum = System.Math.Min(numItems, itemKvp.Value);
                            var item = new StoredItem(itemKvp.Value.Id, takeNum);
                            itemsMoved.Add(item);
                            crateInventory.TryTake(item);
                        }
                    }

                    data.Player.ActiveColony.Stockpile.Add(itemsMoved);

                    NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player));
                }
                else
                    NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player, true));

            }
            else if (data.ButtonIdentifier == "Crate.MoveItemsToToolbar")
            {
                if (data.Storage.TryGetAs("Crate.NumberOfItems", out string strNumItems) && 
                    int.TryParse(strNumItems, out int numItems) &&
                    TryGetPlayersCurrentCrate(data.Player, out var crateInventory))
                {
                    List<StoredItem> itemsMoved = new List<StoredItem>();
                    var invRef = data.Player.Inventory;

                    foreach (var itemKvp in crateInventory.ContentCopy)
                    {
                        if (data.Storage.TryGetAs("Crate." + itemKvp.Key + ".ItemSelected", out bool selected) && selected)
                        {
                            int takeNum = System.Math.Min(numItems, itemKvp.Value);
                            var item = new StoredItem(itemKvp.Value.Id, takeNum);
                            itemsMoved.Add(item);
                        }
                    }

                    foreach (var item in itemsMoved)
                    {
                        if (invRef.TryAdd(item.Id, item.Amount))
                        {
                            crateInventory.TryTake(item);
                        }
                        else
                            break;
                    }

                    NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player));
                }
                else
                    NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player, true));
            }
            else if (data.ButtonIdentifier == "Crate.MoveItemsToCrateFromStockpile")
            {
                if (data.Storage.TryGetAs("Crate.NumberOfItems", out string strNumItems) && 
                    int.TryParse(strNumItems, out int numItems) &&
                    TryGetPlayersCurrentCrate(data.Player, out var crateInventory))
                {
                    List<StoredItem> itemsMoved = new List<StoredItem>();

                    foreach (var itemKvp in data.Player.ActiveColony.Stockpile.Items)
                    {
                        if (data.Storage.TryGetAs("Crate." + itemKvp.Key + ".ItemSelected", out bool selected) && selected)
                        {
                            int takeNum = System.Math.Min(numItems, itemKvp.Value);
                            var item = new StoredItem(itemKvp.Key, takeNum);
                            itemsMoved.Add(item);
                        }
                    }

                    foreach (var item in itemsMoved)
                    {
                        data.Player.ActiveColony.Stockpile.TryRemove(item);
                        var leftover = crateInventory.TryAdd(item);
                        data.Player.ActiveColony.Stockpile.Add(leftover);
                    }

                    NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player));
                }
                else
                    NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player, true));
            }
            else if (data.ButtonIdentifier == "Crate.MoveItemsToCrateFromToolbar")
            {
                if (data.Storage.TryGetAs("Crate.NumberOfItems", out string strNumItems) && 
                    int.TryParse(strNumItems, out int numItems) &&
                    TryGetPlayersCurrentCrate(data.Player, out var crateInventory))
                {
                    List<StoredItem> itemsMoved = new List<StoredItem>();
                    var invRef = data.Player.Inventory;

                    foreach (var itemKvp in invRef.Items)
                    {
                        if (data.Storage.TryGetAs("Crate." + itemKvp.Type + ".ItemSelected", out bool selected) && selected)
                        {
                            int takeNum = System.Math.Min(numItems, itemKvp.Amount);
                            var item = new StoredItem(itemKvp.Type, takeNum);
                            itemsMoved.Add(item);
                        }
                    }

                    foreach (var item in itemsMoved)
                    {
                        if (invRef.TryRemove(item.Id, item.Amount))
                        {
                            invRef.Add(crateInventory.TryAdd(item));
                        }
                    }

                    NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player));
                }
                else
                    NetworkMenuManager.SendServerPopup(data.Player, MainMenu(data.Player, true));
            }
        }

        private static NetworkMenu StockpileMenu(ButtonPressCallbackData data, bool error = false, bool? selectAll = null)
        {
            NetworkMenu menu = new NetworkMenu();
            menu.LocalStorage.SetAs("header", _localizationHelper.LocalizeOrDefault("MoveItemsToCrate", data.Player));
            menu.Width = 1000;
            menu.Height = 600;

            try
            {
                if (error)
                    menu.Items.Add(new Label(new LabelData(_localizationHelper.GetLocalizationKey("invalidNumber"), UnityEngine.Color.red)));

                List<ValueTuple<IItem, int>> headerItems = new List<ValueTuple<IItem, int>>();
                headerItems.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(_localizationHelper.GetLocalizationKey("Numberofitems"))), 333));
                headerItems.Add(ValueTuple.Create<IItem, int>(new InputField("Crate.NumberOfItems"), 333));
                headerItems.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.MoveItemsToCrateFromStockpile", new LabelData(_localizationHelper.GetLocalizationKey("MoveItemsToCrate"))), 333));
                menu.Items.Add(new HorizontalRow(headerItems));
                menu.Items.Add(new Line(UnityEngine.Color.black));

                List<ValueTuple<IItem, int>> items = new List<ValueTuple<IItem, int>>();
                items.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.MainMenu", new LabelData(_localizationHelper.GetLocalizationKey("Back"))), 250));
                items.Add(ValueTuple.Create<IItem, int>(new EmptySpace(), 250));
                items.Add(ValueTuple.Create<IItem, int>(new EmptySpace(), 250));

                bool selected = false;

                if (selectAll == true)
                    selected = true;
                else if (selectAll == false)
                    selected = false;

                if (selected)
                    items.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.SelectNoneInCrate", new LabelData(_localizationHelper.GetLocalizationKey("SelectNone"))), 250));
                else
                    items.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.SelectAllInCrate", new LabelData(_localizationHelper.GetLocalizationKey("SelectAll"))), 250));

                menu.Items.Add(new HorizontalRow(items));
                menu.Items.Add(new Line(UnityEngine.Color.black));

                foreach (var itemKvp in data.Player.ActiveColony.Stockpile.Items)
                {
                    items = new List<ValueTuple<IItem, int>>();
                    items.Add(ValueTuple.Create<IItem, int>(new ItemIcon(itemKvp.Key), 250));
                    items.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(ItemId.GetItemId(itemKvp.Key), UnityEngine.TextAnchor.MiddleLeft, 18, LabelData.ELocalizationType.Type)), 250));
                    items.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("Crate", data.Player) + ": " + itemKvp.Value.ToString())), 250));
                    items.Add(ValueTuple.Create<IItem, int>(new Toggle(new LabelData(_localizationHelper.LocalizeOrDefault("Select", data.Player)), "Crate." + itemKvp.Key + ".ItemSelected"), 250));

                    if (selectAll == null)
                        menu.LocalStorage.TryGetAs("Crate." + itemKvp.Key + ".ItemSelected", out selected);

                    menu.LocalStorage.SetAs("Crate." + itemKvp.Key + ".ItemSelected", selected);
                    menu.Items.Add(new HorizontalRow(items));
                    
                }
            }
            catch (Exception ex)
            {
                CivLogger.LogError(ex);
            }

            return menu;
        }

        public static NetworkMenu ToolbarMenu(ButtonPressCallbackData data, bool error = false, bool? selectAll = null)
        {
            NetworkMenu menu = new NetworkMenu();
            menu.LocalStorage.SetAs("header", _localizationHelper.LocalizeOrDefault("MoveItemsToCrate", data.Player));
            menu.Width = 1000;
            menu.Height = 600;

            try
            {
                if (error)
                    menu.Items.Add(new Label(new LabelData(_localizationHelper.GetLocalizationKey("invalidNumber"), UnityEngine.Color.red)));

                List<ValueTuple<IItem, int>> headerItems = new List<ValueTuple<IItem, int>>();
                headerItems.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(_localizationHelper.GetLocalizationKey("Numberofitems"))), 333));
                headerItems.Add(ValueTuple.Create<IItem, int>(new InputField("Crate.NumberOfItems"), 333));
                headerItems.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.MoveItemsToCrateFromToolbar", new LabelData(_localizationHelper.GetLocalizationKey("MoveItemsToCrate"))), 333));
                menu.Items.Add(new HorizontalRow(headerItems));
                menu.Items.Add(new Line(UnityEngine.Color.black));

                List<ValueTuple<IItem, int>> items = new List<ValueTuple<IItem, int>>();
                items.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.MainMenu", new LabelData(_localizationHelper.GetLocalizationKey("Back"))), 250));
                items.Add(ValueTuple.Create<IItem, int>(new EmptySpace(), 250));
                items.Add(ValueTuple.Create<IItem, int>(new EmptySpace(), 250));

                bool selected = false;

                if (selectAll == true)
                    selected = true;
                else if (selectAll == false)
                    selected = false;

                if (selected)
                    items.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.SelectNoneInCrateToolbar", new LabelData(_localizationHelper.GetLocalizationKey("SelectNone"))), 250));
                else
                    items.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.SelectAllInCrateToolbar", new LabelData(_localizationHelper.GetLocalizationKey("SelectAll"))), 250));

                menu.Items.Add(new HorizontalRow(items));
                menu.Items.Add(new Line(UnityEngine.Color.black));
                var invRef = data.Player.Inventory;

                foreach (var itemKvp in invRef.Items)
                {
                    if (itemKvp.Type != ColonyBuiltIn.ItemTypes.AIR.Id)
                    {
                        items = new List<ValueTuple<IItem, int>>();
                        items.Add(ValueTuple.Create<IItem, int>(new ItemIcon(itemKvp.Type), 250));
                        items.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(ItemId.GetItemId(itemKvp.Type), UnityEngine.TextAnchor.MiddleLeft, 18, LabelData.ELocalizationType.Type)), 250));
                        items.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("Toolbar", data.Player) + ": " + itemKvp.Amount.ToString())), 250));
                        items.Add(ValueTuple.Create<IItem, int>(new Toggle(new LabelData(_localizationHelper.LocalizeOrDefault("Select", data.Player)), "Crate." + itemKvp.Type + ".ItemSelected"), 250));

                        if (selectAll == null)
                            menu.LocalStorage.TryGetAs("Crate." + itemKvp.Type + ".ItemSelected", out selected);

                        menu.LocalStorage.SetAs("Crate." + itemKvp.Type + ".ItemSelected", selected);
                        menu.Items.Add(new HorizontalRow(items));
                    }
                }
            }
            catch (Exception ex)
            {
                CivLogger.LogError(ex);
            }

            return menu;
        }

        public static NetworkMenu MainMenu(Players.Player player, bool error = false, bool? selectAll = null)
        {
            var ps = PlayerState.GetPlayerState(player);

            NetworkMenu menu = new NetworkMenu();
            menu.LocalStorage.SetAs("header", _localizationHelper.LocalizeOrDefault("Crate", player));
            menu.Width = 1000;
            menu.Height = 600;

            if (TryGetPlayersCurrentCrate(player, out var crateInventory))
            {
                if (error)
                    menu.Items.Add(new Label(new LabelData(_localizationHelper.GetLocalizationKey("invalidNumber"), UnityEngine.Color.red)));

                if (player.ActiveColony != null)
                    menu.Items.Add(new HorizontalSplit(new ButtonCallback("Crate.GetItemsFromStockpile", new LabelData(_localizationHelper.GetLocalizationKey("GetItemsFromStockpile"))),
                                                       new ButtonCallback("Crate.GetItemsFromToolbar", new LabelData(_localizationHelper.GetLocalizationKey("GetItemsFromToolbar")))));
                else
                    menu.Items.Add(new ButtonCallback("Crate.GetItemsFromToolbar", new LabelData(_localizationHelper.GetLocalizationKey("GetItemsFromToolbar"))));

                menu.Items.Add(new Line(UnityEngine.Color.black));

                List<ValueTuple<IItem, int>> headerItems = new List<ValueTuple<IItem, int>>();
                headerItems.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(_localizationHelper.GetLocalizationKey("Numberofitems"))), 250));
                headerItems.Add(ValueTuple.Create<IItem, int>(new InputField("Crate.NumberOfItems"), 250));

                if (player.ActiveColony != null)
                    headerItems.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.MoveItemsToStockpile", new LabelData(_localizationHelper.GetLocalizationKey("MoveItemsToStockpile"))), 250));

                headerItems.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.MoveItemsToToolbar", new LabelData(_localizationHelper.GetLocalizationKey("MoveItemsToToolbar"))), 250));

                menu.Items.Add(new HorizontalRow(headerItems));
                menu.Items.Add(new Line(UnityEngine.Color.black));

                List<ValueTuple<IItem, int>> items = new List<ValueTuple<IItem, int>>();
                items.Add(ValueTuple.Create<IItem, int>(new EmptySpace(), 250));
                items.Add(ValueTuple.Create<IItem, int>(new EmptySpace(), 250));
                items.Add(ValueTuple.Create<IItem, int>(new EmptySpace(), 250));

                bool selected = false;

                if (selectAll == true)
                    selected = true;
                else if (selectAll == false)
                    selected = false;

                if (selected)
                    items.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.SelectNoneInCrateMain", new LabelData(_localizationHelper.GetLocalizationKey("SelectNone"))), 250));
                else
                    items.Add(ValueTuple.Create<IItem, int>(new ButtonCallback("Crate.SelectAllInCrateMain", new LabelData(_localizationHelper.GetLocalizationKey("SelectAll"))), 250));

                menu.Items.Add(new HorizontalRow(items));
                menu.Items.Add(new Line(UnityEngine.Color.black));


                foreach (var itemKvp in crateInventory.ContentCopy)
                {
                    items = new List<ValueTuple<IItem, int>>();
                    items.Add(ValueTuple.Create<IItem, int>(new ItemIcon(itemKvp.Key), 250));
                    items.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(ItemId.GetItemId(itemKvp.Key), UnityEngine.TextAnchor.MiddleLeft, 18, LabelData.ELocalizationType.Type)), 250));
                    items.Add(ValueTuple.Create<IItem, int>(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("Crate", player) + ": " + itemKvp.Value.Amount.ToString())), 250));
                    items.Add(ValueTuple.Create<IItem, int>(new Toggle(new LabelData(_localizationHelper.LocalizeOrDefault("Select", player)), "Crate." + itemKvp.Key + ".ItemSelected"), 250));

                    if (selectAll == null)
                        menu.LocalStorage.TryGetAs("Crate." + itemKvp.Key + ".ItemSelected", out selected);

                    menu.LocalStorage.SetAs("Crate." + itemKvp.Key + ".ItemSelected", selected);
                    menu.Items.Add(new HorizontalRow(items));
                }
            }
            else
                menu.Items.Add(new Label(new LabelData(_localizationHelper.GetLocalizationKey("CannotLocateCrateData"), UnityEngine.Color.red)));

            return menu;
        }
    }
}
