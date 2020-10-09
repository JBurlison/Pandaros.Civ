using Jobs;
using ModLoaderInterfaces;
using NetworkUI;
using NetworkUI.AreaJobs;
using NetworkUI.Items;
using Pandaros.API.Entities;
using Pandaros.API.Questing;
using Pandaros.API.Questing.Models;
using Pandaros.Civ.Quests;
using Pandaros.Civ.TimePeriods.PreHistory.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pandaros.Civ.TimePeriods
{
    public class PeriodFactory : IOnConstructCommandTool, IOnPlayerPushedNetworkUIButton, IAfterWorldLoad
    {
        public static Dictionary<TimePeriod, Action<Players.Player>> GenerateTimePeriodCommandToolMenu { get; set; } = new Dictionary<TimePeriod, Action<Players.Player>>()
        {
            { TimePeriod.PreHistory, SendPreHistroyMenu }
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
            CommandToolManager.AddButtonTooltip(TimePeriod.PreHistory.ToString(), TimePeriod.PreHistory.ToString(), TimePeriod.PreHistory.ToString() + "Tip");

            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(RockThrower.NameDay, RockThrower.NameDay, RockThrower.NameDay, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(RockThrower.NameNight, RockThrower.NameNight, RockThrower.NameNight, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(SpearThrower.NameDay, SpearThrower.NameDay, SpearThrower.NameDay, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(SpearThrower.NameNight, SpearThrower.NameNight, SpearThrower.NameNight, EBlockToolHoverType.GreenIfNPCCanStand));
            CommandToolManager.AddAreaJobSettings(new BlockToolDescriptionSettings(PrimitiveForager.Name, PrimitiveForager.Name, PrimitiveForager.Name, EBlockToolHoverType.GreenIfNPCCanStand));

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
            CommandToolManager.GenerateFourColumnTwoGroupRow(menu,
                GetButtonTool(p, RockThrower.NameDay, RockThrower.Name),
                GetButtonTool(p, SpearThrower.NameDay, SpearThrower.Name),
                GetButtonTool(p, RockThrower.NameNight, RockThrower.Name),
                GetButtonTool(p, SpearThrower.NameNight, SpearThrower.Name)) ;
            menu.Items.Add(new EmptySpace(10));
            CommandToolManager.GenerateThreeColumnCenteredRow(menu,
                CommandToolManager.GetButtonTool(p, CommandToolManager.AreaType.FORESTER, CommandToolManager.LocalizationKey.FORESTER),
                GetButtonTool(p, PrimitiveForager.Name, PrimitiveForager.Name));

            ModLoader.Callbacks.OnConstructCommandTool.Invoke(p, menu, TimePeriod.PreHistory.ToString());
            NetworkMenuManager.SendServerPopup(p, menu);
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
    }
}
