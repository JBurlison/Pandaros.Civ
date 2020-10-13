using Jobs;
using Jobs.Implementations;
using ModLoaderInterfaces;
using NetworkUI;
using NetworkUI.AreaJobs;
using NetworkUI.Items;
using NPC;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.Models;
using Pandaros.API.Questing;
using Pandaros.API.Questing.Models;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pandaros.Civ.Quests;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using Pandaros.Civ.TimePeriods.StoneAge.Jobs;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pandaros.Civ.TimePeriods
{
    public class PeriodFactory : IOnConstructCommandTool, IOnPlayerPushedNetworkUIButton, IAfterWorldLoad, IOnCreatedColony, IOnConstructTooltipUI
    {
        public const string TOOLTIP = "Tip";

        public static Dictionary<TimePeriod, Action<Players.Player>> GenerateTimePeriodCommandToolMenu { get; set; } = new Dictionary<TimePeriod, Action<Players.Player>>()
        {
            { TimePeriod.PreHistory, SendPreHistroyMenu },
            { TimePeriod.StoneAge, SendStoneAgeyMenu }
        };

        public static TimePeriod GetTimePeriod(Colony c)
        {
            var cs = ColonyState.GetColonyState(c);

            if(cs.Stats.TryGetValue(nameof(TimePeriod), out var val))
                return (TimePeriod)val;
            else
            {
                cs.Stats[nameof(TimePeriod)] = (double)TimePeriod.PreHistory;
                return TimePeriod.PreHistory;
            }
        }

        public void AfterWorldLoad()
        {
            // Pre History
            CommandToolManager.AddButtonTooltip(TimePeriod.PreHistory.ToString(), TimePeriod.PreHistory.ToString(), TimePeriod.PreHistory.ToString() + TOOLTIP);

            CommandToolManager.AddButtonTooltip(PrimitiveBerryForager.Name, PrimitiveBerryForager.Name, PrimitiveBerryForager.Name + TOOLTIP);
            CommandToolManager.AddButtonTooltip(PrimitiveRockForager.Name, PrimitiveRockForager.Name, PrimitiveRockForager.Name + TOOLTIP);
            CommandToolManager.AddButtonTooltip(PrimitiveWoodForager.Name, PrimitiveWoodForager.Name, PrimitiveWoodForager.Name + TOOLTIP);
            CommandToolManager.AddButtonTooltip(RockThrower.NameDay, RockThrower.NameDay, RockThrower.Name + TOOLTIP);
            CommandToolManager.AddButtonTooltip(SpearThrower.NameDay, SpearThrower.NameDay, SpearThrower.Name + TOOLTIP);
            CommandToolManager.AddButtonTooltip(RockThrower.NameNight, RockThrower.NameNight, RockThrower.Name + TOOLTIP);
            CommandToolManager.AddButtonTooltip(SpearThrower.NameNight, SpearThrower.NameNight, SpearThrower.Name + TOOLTIP);

            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(RockThrower.NameDay, RockThrower.NameDay, RockThrower.NameDay, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(RockThrower.NameNight, RockThrower.NameNight, RockThrower.NameNight, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(SpearThrower.NameDay, SpearThrower.NameDay, SpearThrower.NameDay, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(SpearThrower.NameNight, SpearThrower.NameNight, SpearThrower.NameNight, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(PrimitiveBerryForager.Name, PrimitiveBerryForager.Name, PrimitiveBerryForager.Name, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(PrimitiveRockForager.Name, PrimitiveRockForager.Name, PrimitiveRockForager.Name, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(PrimitiveWoodForager.Name, PrimitiveWoodForager.Name, PrimitiveWoodForager.Name, EBlockToolHoverType.GreenIfNPCCanStand));


            // Stone Age
            CommandToolManager.AddButtonTooltip(TimePeriod.StoneAge.ToString(), TimePeriod.StoneAge.ToString(), TimePeriod.StoneAge.ToString() + TOOLTIP);

            CommandToolManager.AddButtonTooltip(StoneMiner.Name, StoneMiner.Name, StoneMiner.Name + TOOLTIP);

            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(StoneMiner.Name, StoneMiner.Name, StoneMiner.Name, EBlockToolHoverType.GreenIfMineable));


            CommandToolManager.AddButtonTooltip(TimePeriod.BronzeAge.ToString(), TimePeriod.BronzeAge.ToString(), TimePeriod.BronzeAge.ToString() + TOOLTIP);
            CommandToolManager.AddButtonTooltip(TimePeriod.IronAge.ToString(), TimePeriod.IronAge.ToString(), TimePeriod.IronAge.ToString() + TOOLTIP);
            CommandToolManager.AddButtonTooltip(TimePeriod.IndustrialAge.ToString(), TimePeriod.IndustrialAge.ToString(), TimePeriod.IndustrialAge.ToString() + TOOLTIP);
            CommandToolManager.AddButtonTooltip(TimePeriod.AtomicAge.ToString(), TimePeriod.AtomicAge.ToString(), TimePeriod.AtomicAge.ToString() + TOOLTIP);
            CommandToolManager.AddButtonTooltip(TimePeriod.InformationAge.ToString(), TimePeriod.InformationAge.ToString(), TimePeriod.InformationAge.ToString() + TOOLTIP);
            CommandToolManager.AddButtonTooltip(TimePeriod.RenewablesAge.ToString(), TimePeriod.RenewablesAge.ToString(), TimePeriod.RenewablesAge.ToString() + TOOLTIP);
            CommandToolManager.AddButtonTooltip(TimePeriod.FusionAge.ToString(), TimePeriod.FusionAge.ToString(), TimePeriod.FusionAge.ToString() + TOOLTIP);
        }

        public void OnPlayerPushedNetworkUIButton(ButtonPressCallbackData data)
        {
            if (!string.IsNullOrEmpty(data.ButtonIdentifier) && Enum.TryParse<TimePeriod>(data.ButtonIdentifier, out var tp) && GenerateTimePeriodCommandToolMenu.TryGetValue(tp, out var generateMenu))
            {
                generateMenu(data.Player);
            }
        }

        public void OnConstructCommandTool(Players.Player p, NetworkMenu menu, string menuName)
        {
            if (menuName == CommandToolManager.Menus.MAIN_MENU)
            {
                var newMenu = CommandToolManager.GenerateMenuBase(p, false);
                menu.Items.Clear();
                menu.Items.AddRange(newMenu.Items);

                if (p.ActiveColony != null)
                {
                    var currentPeriod = GetTimePeriod(p.ActiveColony);

                    CommandToolManager.GenerateThreeColumnCenteredRow(menu,
                        CommandToolManager.GetButtonMenu(p, TimePeriod.PreHistory.ToString(), TimePeriod.PreHistory.ToString()),
                        CommandToolManager.GetButtonMenu(p, TimePeriod.StoneAge.ToString(), TimePeriod.StoneAge.ToString(), currentPeriod >= TimePeriod.StoneAge),
                        CommandToolManager.GetButtonMenu(p, TimePeriod.BronzeAge.ToString(), TimePeriod.BronzeAge.ToString(), currentPeriod >= TimePeriod.BronzeAge)
                        );
                    menu.Items.Add(new EmptySpace(10));
                    CommandToolManager.GenerateThreeColumnCenteredRow(menu,
                        CommandToolManager.GetButtonMenu(p, TimePeriod.IronAge.ToString(), TimePeriod.IronAge.ToString(), currentPeriod >= TimePeriod.IronAge),
                        CommandToolManager.GetButtonMenu(p, TimePeriod.IndustrialAge.ToString(), TimePeriod.IndustrialAge.ToString(), currentPeriod >= TimePeriod.IndustrialAge),
                        CommandToolManager.GetButtonMenu(p, TimePeriod.AtomicAge.ToString(), TimePeriod.AtomicAge.ToString(), currentPeriod >= TimePeriod.AtomicAge)
                        );
                    menu.Items.Add(new EmptySpace(10));
                    CommandToolManager.GenerateThreeColumnCenteredRow(menu,
                        CommandToolManager.GetButtonMenu(p, TimePeriod.InformationAge.ToString(), TimePeriod.InformationAge.ToString(), currentPeriod >= TimePeriod.InformationAge),
                        CommandToolManager.GetButtonMenu(p, TimePeriod.RenewablesAge.ToString(), TimePeriod.RenewablesAge.ToString(), currentPeriod >= TimePeriod.RenewablesAge),
                        CommandToolManager.GetButtonMenu(p, TimePeriod.FusionAge.ToString(), TimePeriod.FusionAge.ToString(), currentPeriod >= TimePeriod.FusionAge)
                        );
                }
            }
        }

        public static void SendPreHistroyMenu(Players.Player p)
        {
            var menu = CommandToolManager.GenerateMenuBase(p, true);

            menu.Items.Add(new BackgroundColor(null, 255, 75, 5, 0, 4, 4, new UnityEngine.Color32(96, 79, 73, 255)));
            menu.Items.Add(new BackgroundColor(null, 255, 75, 295, 0, 4, 4, new UnityEngine.Color32(96, 79, 73, 255)));
            menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
            {
                (new Label(new LabelData("popup.tooljob.guardsday", ELabelAlignment.UpperCenter), 20) { Width = 270 }, 270),
                (new Label(new LabelData("")) { Width = 20 }, 20),
                (new Label(new LabelData("popup.tooljob.guardsnight", ELabelAlignment.UpperCenter), 20) { Width = 270 }, 270)
            }));

            GenerateGuardsBox(menu,
                GetButtonTool(p, RockThrower.NameDay, RockThrower.Name),
                GetButtonTool(p, SpearThrower.NameDay, SpearThrower.Name),
                GetButtonTool(p, RockThrower.NameNight, RockThrower.Name),
                GetButtonTool(p, SpearThrower.NameNight, SpearThrower.Name));
            CommandToolManager.GenerateThreeColumnCenteredRow(menu,
                GetButtonTool(p, PrimitiveBerryForager.Name, PrimitiveBerryForager.Name),
                GetButtonTool(p, PrimitiveRockForager.Name, PrimitiveRockForager.Name),
                GetButtonTool(p, PrimitiveWoodForager.Name, PrimitiveWoodForager.Name));

            ModLoader.Callbacks.OnConstructCommandTool.Invoke(p, menu, TimePeriod.PreHistory.ToString());
            NetworkMenuManager.SendServerPopup(p, menu);
        }

        public static void SendStoneAgeyMenu(Players.Player p)
        {
            var menu = CommandToolManager.GenerateMenuBase(p, true);
            GenerateGuardsBox(menu);

            menu.Items.Add(new EmptySpace(10));
            CommandToolManager.GenerateThreeColumnCenteredRow(menu,
                GetButtonTool(p, StoneMiner.Name, StoneMiner.Name));

            ModLoader.Callbacks.OnConstructCommandTool.Invoke(p, menu, TimePeriod.PreHistory.ToString());
            NetworkMenuManager.SendServerPopup(p, menu);
        }

        /// <summary>
        ///     Takes 4 guard buttons, 2 day, 2 night. paird as seen on the UI. 2 day guards then 2 night guards.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="guardButtons"></param>
        private static void GenerateGuardsBox(NetworkMenu menu, params IItem[] guardButtons)
        {
            menu.Items.Add(new BackgroundColor(null, 255, 75, 5, 0, 4, 4, new UnityEngine.Color32(96, 79, 73, 255)));
            menu.Items.Add(new BackgroundColor(null, 255, 75, 295, 0, 4, 4, new UnityEngine.Color32(96, 79, 73, 255)));
            menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
            {
                (new Label(new LabelData("popup.tooljob.guardsday", ELabelAlignment.UpperCenter), 20) { Width = 270 }, 270),
                (new Label(new LabelData("")) { Width = 20 }, 20),
                (new Label(new LabelData("popup.tooljob.guardsnight", ELabelAlignment.UpperCenter), 20) { Width = 270 }, 270)
            }));
            CommandToolManager.GenerateFourColumnTwoGroupRow(menu, guardButtons);
            menu.Items.Add(new EmptySpace(10));
        }

        public static IItem GetButtonTool(Players.Player p, string id, string labelLocalizationKey, int width = 120, bool triggerHover = true)
        {
            bool enabled = true;
            
            if (JobReward.JobToQuestMappings.TryGetValue(id, out string quest) && QuestingSystem.CompletedQuests.TryGetValue(p.ActiveColony, out var completed))
            {
                enabled = completed.Contains(quest);
            }
            return new ButtonCallback(id, new LabelData(labelLocalizationKey), width, 45, ButtonCallback.EOnClickActions.ClosePopup)
            {
                TriggerHoverCallback = triggerHover,
                Enabled = enabled
            };
        }

        public void OnCreatedColony(Colony colony)
        {
            var cs = ColonyState.GetColonyState(colony);
            cs.MonstersEnabled = false;
        }

        public void OnConstructTooltipUI(Players.Player player, ConstructTooltipUIData data)
        {
            if (data.hoverType == ETooltipHoverType.NetworkUiButton && 
                CommandToolManager.AreaDescriptions.TryGetValue(data.hoverKey, out var toolDescription))
            {
                if (PandaJobFactory.GuardJobsSettings.TryGetValue(toolDescription.NPCTypeKey, out var guardJobSettings))
                {
                    if (NPCType.NPCTypes.TryGetValue(guardJobSettings.NPCType, out var nPCTypeSettings) && nPCTypeSettings is NPCTypeStandardSettings standardSettings)
                        data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("InventorySize"))), 125),
                                                        (new Label(new LabelData(standardSettings.inventoryCapacity.ToString())), 125)
                                                    }));

                    data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("Damage"))), 125),
                                                        (new Label(new LabelData(guardJobSettings.Damage.ToString())), 125)
                                                    }));
                    data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("Range"))), 125),
                                                        (new Label(new LabelData(guardJobSettings.Range.ToString() + Localization.GetSentence(player.LastKnownLocale, GameSetup.GetNamespace("Blocks")))), 125)
                                                    }));
                    data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("ReloadSpeed"))), 125),
                                                        (new Label(new LabelData(guardJobSettings.CooldownShot.ToString() + Localization.GetSentence(player.LastKnownLocale, GameSetup.GetNamespace("Seconds")))), 125)
                                                    }));
                    data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                     {
                                                        (new Label(new LabelData(GameSetup.GetNamespace("RecruitmentItem"))), 125),
                                                        (new Label(new LabelData(ItemId.GetItemId(guardJobSettings.RecruitmentItem.Type), ELabelAlignment.Default, 16, LabelData.ELocalizationType.Type)), 125)
                                                    }));
                }


                if (PandaJobFactory.MineJobsSettings.TryGetValue(toolDescription.NPCTypeKey, out var miningJobSettings))
                {
                    if (miningJobSettings.RecruitmentItem != null && miningJobSettings.RecruitmentItem.Type != ColonyBuiltIn.ItemTypes.AIR)
                        data.menu.Items.Add(new HorizontalRow(new List<(IItem, int)>()
                                                         {
                                                            (new Label(new LabelData(GameSetup.GetNamespace("RecruitmentItem"))), 125),
                                                            (new Label(new LabelData(ItemId.GetItemId(miningJobSettings.RecruitmentItem.Type), ELabelAlignment.Default, 16, LabelData.ELocalizationType.Type)), 125)
                                                        }));

                    if (miningJobSettings.MinableTypes != null && miningJobSettings.MinableTypes.Count != 0)
                    {
                        data.menu.Items.Add(new Label(new LabelData(GameSetup.GetNamespace("MinableTypes"))));

                        StringBuilder buildableTypes = new StringBuilder();
                        var i = 0;

                        foreach (var item in miningJobSettings.MinableTypes)
                        {
                            i++;
                            buildableTypes.Append(Localization.GetType(player.LastKnownLocale, ItemId.GetItemId(item)));

                            if (i != miningJobSettings.MinableTypes.Count)
                                buildableTypes.Append(", ");
                        }

                        data.menu.Items.Add(new Label(new LabelData(buildableTypes.ToString())));
                    }
                }

            }
        }
    }
}
