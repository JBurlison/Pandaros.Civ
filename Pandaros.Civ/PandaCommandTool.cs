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

namespace Pandaros.Civ.CommandTool
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
            //UI Settings
            NetworkMenu commandUI = new NetworkMenu();
            commandUI.Identifier = "CommandToolUI";
            commandUI.LocalStorage.SetAs("header", _localizationHelper.LocalizeOrDefault("CommandTool", player));
            commandUI.Width = 500;
            commandUI.Height = 600;

            int LabelSize = 150;
            int ButtonSize = 75;
            if (!commandUIInteraction.item_placer_option_dict.ContainsKey(player))
            {
                commandUIInteraction.item_placer_option_dict[player] = "";
            }
            if (commandUIInteraction.item_placer_option_dict[player] == "Guards")
            {
                    Label Guardslabel = new Label(_localizationHelper.LocalizeOrDefault("Label.Guards", player));
                    ButtonCallback BackButton = new ButtonCallback(GameSetup.NAMESPACE + ".UIButton.Back", new LabelData(_localizationHelper.LocalizeOrDefault("Button.Back", player), Color.black));
                    List<(IItem, int)> GuardsHeaderHorizontalItems = new List<(IItem, int)>();

                    GuardsHeaderHorizontalItems.Add((Guardslabel, LabelSize));
                    GuardsHeaderHorizontalItems.Add((BackButton, ButtonSize));

                    HorizontalRow GuardsHeaderHorizontalRow = new HorizontalRow(GuardsHeaderHorizontalItems);
                    commandUI.Items.Add(GuardsHeaderHorizontalRow);


                    foreach (string guard in commandUIInteraction.commandUIGuardTypes)
                    {
                        Label GuardLabel = new Label(_localizationHelper.LocalizeOrDefault("Label." + guard, player));
                        ButtonCallback GuardButtonNight = new ButtonCallback(GameSetup.NAMESPACE + ".UIButton." + guard + ".Night", new LabelData(_localizationHelper.LocalizeOrDefault("Button.Night", player), Color.black));
                        ButtonCallback GuardButtonDay = new ButtonCallback(GameSetup.NAMESPACE + ".UIButton." + guard + ".Day", new LabelData(_localizationHelper.LocalizeOrDefault("Button.Day", player), Color.black));
                        List<(IItem, int)> GuardHorizontalItems = new List<(IItem, int)>();

                        GuardHorizontalItems.Add((GuardLabel, LabelSize));
                        GuardHorizontalItems.Add((GuardButtonNight, ButtonSize));
                        GuardHorizontalItems.Add((GuardButtonDay, ButtonSize));

                        HorizontalRow GuardHorizontalRow = new HorizontalRow(GuardHorizontalItems);

                        commandUI.Items.Add(GuardHorizontalRow);
                    }
                
            }
            else
            {
                foreach (string category in commandUIInteraction.commandUICategories)
                {
                    ButtonCallback CategoryButton = new ButtonCallback(GameSetup.NAMESPACE + ".UIButton." + category, new LabelData(_localizationHelper.LocalizeOrDefault("Button." + category, player), Color.black));
                    commandUI.Items.Add(CategoryButton);
                    commandUI.Items.Add(new EmptySpace(5));
                }
            }
            commandUI.Items.Add(new EmptySpace(35));

            /*//if (player == null && player.ConnectionState != Players.EConnectionState.Connected || player.ActiveColony == null || player.ActiveColony.ScienceData == null)
            //   return;

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



            //sends ui
            NetworkMenuManager.SendServerPopup(player, commandUI);
        }
    }

    [ModLoader.ModManager]
    public static class commandUIInteraction
    {
        public static Dictionary<Players.Player, string> item_placer_dict = new Dictionary<Players.Player, string>();
        public static Dictionary<Players.Player, string> item_placer_option_dict = new Dictionary<Players.Player, string>();
        public static List<string> commandUICategories = new List<string>
        {
            "Guards"
        };
        public static List<string> commandUIGuardTypes = new List<string>
        {
            "RockThrower",
            "SpearThrower",
            "StoneSpearThrower"
        };

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerPushedNetworkUIButton, GameSetup.NAMESPACE + ".UIButton.OnPlayerPushedNetworkUIButton")]
        public static void OnPlayerPushedNetworkUIButton(ButtonPressCallbackData data)
        {
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
                    case GameSetup.NAMESPACE + ".UIButton.StoneSpearThrower.Night":
                        item_placer_dict[data.Player] = TimePeriods.PreHistory.Jobs.SpearThrower.NameNight;
                        return;
                    case GameSetup.NAMESPACE + ".UIButton.StoneSpearThrower.Day":
                        item_placer_dict[data.Player] = TimePeriods.PreHistory.Jobs.SpearThrower.NameDay;
                        return;
                }
            }

        }
    }
    public class CommandToolType : CSType
    {
        public static string NAME = GameSetup.GetNamespace("CommandTool");
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
        public override int? maxStackSize => 1;
    }
    [ModLoader.ModManager]
    public class UIManageing
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerClicked, GameSetup.NAMESPACE + ".CommandTool.OnPlayerClick")]
        public static void UIManagement(Players.Player player, PlayerClickedData data)
        {
            if (data.TypeSelected == ItemTypes.GetType(CommandToolType.NAME).ItemIndex)
            {
                //PlayerClickedData.VoxelHit voxelData = data.GetVoxelHit();
                if (data.ClickType == PlayerClickedData.EClickType.Left)
                {
                    SendCommandUI.SendUI(player);
                }
                else if (data.ClickType == PlayerClickedData.EClickType.Right && commandUIInteraction.item_placer_dict.ContainsKey(player))
                {
                    PlayerClickedData.VoxelHit voxelData = data.GetVoxelHit();
                    if (PlayerClickedData.EHitType.Block == data.HitType && voxelData.SideHit == VoxelSide.yPlus)
                    {
                        ServerManager.TryChangeBlock(voxelData.PositionBuild, ItemTypes.GetType(commandUIInteraction.item_placer_dict[player]).ItemIndex, player);

                    }
                }
            }
        }
        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnSendAreaHighlights, GameSetup.NAMESPACE + ".CommandTool.OnSendAreaHighlights")]
        private static void OnSendAreaHighlights(Players.Player goal, List<AreaJobTracker.AreaHighlight> list, List<ushort> showWhileHoldingTypes)
        {
            showWhileHoldingTypes.Add(ItemTypes.GetType(CommandToolType.NAME).ItemIndex);
        }
    }
}
