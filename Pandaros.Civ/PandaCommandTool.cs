using Pipliz;

using Chatting;
using NetworkUI;
using NetworkUI.Items;
using Science;
using System;
using Shared;

using System.Collections.Generic;
using System.Linq;
using Pipliz.JSON;
using System.IO;
using UnityEngine;
using Pandaros.API.localization;
using Pandaros.API.Models;
using Pandaros.API.Items;

namespace Pandaros.Civ
{
    //open ui with command
    [ChatCommandAutoLoader]
    public class Command : IChatCommand
    {
        public bool TryDoCommand(Players.Player player, string chat, List<string> splits)
        {
            if (player == null)
                return false;

            if (!chat.Equals("?commandtool"))
                return false;

            //Sends the UI to the player
            SendCommandUI.SendUI(player);

            return true;
        }
    }

    //draws ui
    [ModLoader.ModManager]
    public class SendCommandUI
    {
        static readonly LocalizationHelper _localizationHelper = new LocalizationHelper(GameSetup.NAMESPACE, "CommandTool");
        public static void SendUI(Players.Player player)
        {
            NetworkMenu commandUI = new NetworkMenu();
            commandUI.Identifier = "CommandToolUI";
            commandUI.LocalStorage.SetAs("header", _localizationHelper.LocalizeOrDefault("CommandTool", player));
            commandUI.Width = 500;
            commandUI.Height = 600;


            ButtonCallback GuardsButton = new ButtonCallback(GameSetup.NAMESPACE + ".UIButton.Guards", new LabelData(_localizationHelper.LocalizeOrDefault("Button.Guards", player), Color.black));


            Label Guardslabel = new Label(_localizationHelper.LocalizeOrDefault("Label.Guards", player));
            ButtonCallback BackButton = new ButtonCallback(GameSetup.NAMESPACE + ".UIButton.Back", new LabelData(_localizationHelper.LocalizeOrDefault("Button.Back", player), Color.black));
            List<(IItem, int)> GuardsHeaderHorizontalItems = new List<(IItem, int)>();

            GuardsHeaderHorizontalItems.Add((Guardslabel, 150));
            GuardsHeaderHorizontalItems.Add((BackButton, 75));

            HorizontalRow GuardsHeaderHorizontalRow = new HorizontalRow(GuardsHeaderHorizontalItems);

            Label RockThrowerLabel = new Label(_localizationHelper.LocalizeOrDefault("Label.RockThrower", player));
            ButtonCallback RockThrowerButtonNight = new ButtonCallback(GameSetup.NAMESPACE + ".UIButton.RockThrower.Night", new LabelData(_localizationHelper.LocalizeOrDefault("Button.Night", player), Color.black));
            ButtonCallback RockThrowerButtonDay = new ButtonCallback(GameSetup.NAMESPACE + ".UIButton.RockThrower.Day", new LabelData(_localizationHelper.LocalizeOrDefault("Button.Day", player), Color.black));
            List<(IItem, int)> RockThrowerHorizontalItems = new List<(IItem, int)>();

            RockThrowerHorizontalItems.Add((RockThrowerLabel, 150));
            RockThrowerHorizontalItems.Add((RockThrowerButtonNight, 75));
            RockThrowerHorizontalItems.Add((RockThrowerButtonDay, 75));

            HorizontalRow RockThrowerHorizontalRow = new HorizontalRow(RockThrowerHorizontalItems);

            if (commandUIInteraction.item_placer_option_dict.ContainsKey(player))
            {
                if (commandUIInteraction.item_placer_option_dict[player].Equals("Guards"))
                {
                    commandUI.Items.Add(GuardsHeaderHorizontalRow);
                    if (player == null && player.ConnectionState != Players.EConnectionState.Connected || player.ActiveColony == null || player.ActiveColony.ScienceData == null)
                        return;
                    commandUI.Items.Add(RockThrowerHorizontalRow);

                    /*Science.ScienceKey SlingShotScienceKey = new Science.ScienceKey(Nach0Config.ResearchPrefix + "Slingshot");
                    Science.ScienceKey CompoundBowScienceKey = new Science.ScienceKey(Nach0Config.ResearchPrefix + "CompoundBow");
                    Science.ScienceKey SwordScienceKey = new Science.ScienceKey(Nach0Config.ResearchPrefix + "SwordGuard");
                    Science.ScienceKey SniperScienceKey = new Science.ScienceKey(Nach0Config.ResearchPrefix + "Sniper");
                    Science.ScienceKey BallistaScienceKey = new Science.ScienceKey(Nach0Config.ResearchPrefix + "Ballista");*/

                    /*if (SlingShotScienceKey.IsCompleted(player.ActiveColony.ScienceData))
                        commandUI.Items.Add(SlingshotHorizontalRow);
                    if (CompoundBowScienceKey.IsCompleted(player.ActiveColony.ScienceData))
                        commandUI.Items.Add(compoundBowHorizontalRow);
                    if (SwordScienceKey.IsCompleted(player.ActiveColony.ScienceData))
                        commandUI.Items.Add(swordHorizontalRow);
                    if (SniperScienceKey.IsCompleted(player.ActiveColony.ScienceData))
                        commandUI.Items.Add(sniperHorizontalRow);
                    if (BallistaScienceKey.IsCompleted(player.ActiveColony.ScienceData))
                        commandUI.Items.Add(ballistaHorizontalRow);*/
                }
                else
                {
                    commandUI.Items.Add(GuardsButton);
                    commandUI.Items.Add(new EmptySpace(5));
                    /*if (Nach0Config.GuardsMod)
                    {
                        commandUI.Items.Add(GuardsButton);
                        commandUI.Items.Add(new EmptySpace(5));
                    }*/
                }
            }
            else
            {
                commandUI.Items.Add(GuardsButton);
                commandUI.Items.Add(new EmptySpace(5));
                /*if (Nach0Config.GuardsMod)
                {
                    commandUI.Items.Add(GuardsButton);
                    commandUI.Items.Add(new EmptySpace(5));
                }*/

            }
            commandUI.Items.Add(new EmptySpace(35));

            //sends ui
            NetworkMenuManager.SendServerPopup(player, commandUI);
        }
    }

    [ModLoader.ModManager]
    public static class commandUIInteraction
    {
        public static Dictionary<Players.Player, string> item_placer_dict = new Dictionary<Players.Player, string>();
        public static Dictionary<Players.Player, string> item_placer_option_dict = new Dictionary<Players.Player, string>();

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerPushedNetworkUIButton, GameSetup.NAMESPACE + ".UIButton.OnPlayerPushedNetworkUIButton")]
        public static void OnPlayerPushedNetworkUIButton(ButtonPressCallbackData data)
        {
            /*string itemPrefix = "NACH0.Types.";
            string guard = ".Guard";
            string night = guard + ".Nightx+";
            string day = guard + ".Dayx+";*/
            if (data.ButtonIdentifier.StartsWith(GameSetup.NAMESPACE + ".UIButton."))
            {
                switch (data.ButtonIdentifier)
                {
                    case GameSetup.NAMESPACE + ".UIButton.Guards":
                        item_placer_option_dict[data.Player] = "Guards";
                        SendCommandUI.SendUI(data.Player);
                        item_placer_option_dict[data.Player] = "";
                        return;
                    case GameSetup.NAMESPACE + ".UIButton.Back":
                        item_placer_option_dict[data.Player] = "";
                        SendCommandUI.SendUI(data.Player);
                        return;
                    case GameSetup.NAMESPACE + ".UIButton.RockThrower.Night":
                        item_placer_dict[data.Player] = TimePeriods.PreHistory.Jobs.RockThrower.NameNight;
                        return;
                    case GameSetup.NAMESPACE + ".UIButton.RockThrower.Day":
                        item_placer_dict[data.Player] = TimePeriods.PreHistory.Jobs.RockThrower.NameDay;
                        return;
                    case GameSetup.NAMESPACE + ".UIButton.SpearThrower.Night":
                        item_placer_dict[data.Player] = TimePeriods.PreHistory.Jobs.SpearThrower.NameNight;
                        return;
                    case GameSetup.NAMESPACE + ".UIButton.SpearThrower.Day":
                        item_placer_dict[data.Player] = TimePeriods.PreHistory.Jobs.SpearThrower.NameDay;
                        return;
                }
            }

        }
        /*public static void AfterItemTypeChanged(Players.Player player)
        {
            //Chat.Send(player, "<color=blue>Item_Placer Type set to: " + commandUIInteraction.item_placer_dict[player] + "</color>");
        }*/
    }
    public class CommandTool : CSType
    {
        public static string NAME = GameSetup.GetNamespace(nameof(CommandTool));
        public override string name { get; set; } = NAME;
        public override StaticItems.StaticItem StaticItemSettings => new StaticItems.StaticItem() { Name = NAME };
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "CommandTool.png");
        public override bool? isPlaceable => false;
        public override List<string> categories => new List<string>()
            {
                CommonCategories.Essential,
                "AAA",
                GameSetup.NAMESPACE
            };
    }
    [ModLoader.ModManager]
    public class UIManageing
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerClicked, GameSetup.NAMESPACE + ".CommandTool.OnPlayerClick")]
        public static void UIManagement(Players.Player player, PlayerClickedData data)
        {
            if (data.TypeSelected == ItemTypes.GetType(CommandTool.NAME).ItemIndex)
            {
                //PlayerClickedData.VoxelHit voxelData = data.GetVoxelHit();
                if (data.ClickType == PlayerClickedData.EClickType.Left)
                {
                    SendCommandUI.SendUI(player);
                }
                else if (data.ClickType == PlayerClickedData.EClickType.Right)
                {
                    PlayerClickedData.VoxelHit voxelData = data.GetVoxelHit();
                    if (PlayerClickedData.EHitType.Block == data.HitType && voxelData.SideHit == VoxelSide.yPlus)
                    {
                        if (commandUIInteraction.item_placer_dict.ContainsKey(player))
                        {
                            ServerManager.TryChangeBlock(voxelData.PositionBuild, ItemTypes.GetType(commandUIInteraction.item_placer_dict[player]).ItemIndex, player);
                        }
                    }
                    /*AreaJobTracker.CommandToolTypeData Data = new AreaJobTracker.CommandToolTypeData();


                    switch (commandUIInteraction.item_placer_dict[player])
                    {
                        //case TimePeriods.PreHistory.Jobs.RockThrower.NameNight:
                        //    break;
                        case GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs." + nameof(TimePeriods.PreHistory.Jobs.RockThrower) + "Night":
                            Data.LocaleEntry = "popup.tooljob.RockThrowerNight";
                            Data.AreaType = TimePeriods.PreHistory.Jobs.RockThrower.NameNight;
                            break;
                        case GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs." + nameof(TimePeriods.PreHistory.Jobs.RockThrower) + "Day":
                            Data.LocaleEntry = "popup.tooljob.RockThrowerDay";
                            Data.AreaType = TimePeriods.PreHistory.Jobs.RockThrower.NameDay;
                            break;
                    }
                    AreaJobTracker.StartCommandToolSelection(player, Data);*/

                }
                /*else if (data.ClickType == PlayerClickedData.EClickType.Right)
                {
                    if (PlayerClickedData.EHitType.Block == data.HitType && voxelData.SideHit == VoxelSide.yPlus)
                    {
                        if (commandUIInteraction.item_placer_dict.ContainsKey(player))
                        {
                            ServerManager.TryChangeBlock(voxelData.PositionBuild, ItemTypes.GetType(commandUIInteraction.item_placer_dict[player]).ItemIndex, player);
                        }
                    }
                }*/
            }
        }
        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnSendAreaHighlights, GameSetup.NAMESPACE + ".CommandTool.OnSendAreaHighlights")]
        private static void OnSendAreaHighlights(Players.Player goal, List<AreaJobTracker.AreaHighlight> list, List<ushort> showWhileHoldingTypes)
        {
            showWhileHoldingTypes.Add(ItemTypes.GetType(CommandTool.NAME).ItemIndex);
        }
    }
}
