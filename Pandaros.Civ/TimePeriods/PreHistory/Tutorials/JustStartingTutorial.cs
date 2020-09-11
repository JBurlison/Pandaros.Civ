using BlockTypes;
using NetworkUI;
using NetworkUI.Items;
using Pandaros.API;
using Pandaros.API.localization;
using Pandaros.API.Tutorials.Models;
using Pandaros.API.Tutorials.Prerequisites;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Tutorials
{
    public class JustStartingTutorial : ITutorial
    {
        public string Name => nameof(JustStartingTutorial);

        public List<ITutorialPrerequisite> Prerequisites => new List<ITutorialPrerequisite>()
        {
            new ItemPlacedPrerequisite(ColonyBuiltIn.ItemTypes.BANNER, 1)
        };

        static readonly LocalizationHelper _localizationHelper = new LocalizationHelper(GameSetup.NAMESPACE, "Tutorials");

        public NetworkMenu ShowTutorial(Players.Player p)
        {
            NetworkMenu menu = new NetworkMenu();

            menu.LocalStorage.SetAs("header", _localizationHelper.LocalizeOrDefault("Welcome", p));
            menu.Width = 1000;
            menu.Height = 600;
            menu.ForceClosePopups = true;
            menu.Items.Add(new ItemIcon(stockpile_tutorialIcon.NAME, 800, 70, -150));
            menu.Items.Add(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("WelcomeText", p)), -1, 0, -300));
            menu.Items.Add(new EmptySpace(10));
            menu.Items.Add(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("StockpileBlockHeader", p), ELabelAlignment.MiddleCenter, 24)));
            menu.Items.Add(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("StockpileBlock", p))));
            menu.Items.Add(new EmptySpace(10));
            menu.Items.Add(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("CrateHeader", p), ELabelAlignment.MiddleCenter, 24)));
            menu.Items.Add(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("Crate", p))));
            menu.Items.Add(new EmptySpace(10));
            menu.Items.Add(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("PortersHeader", p), ELabelAlignment.MiddleCenter, 24)));
            menu.Items.Add(new Label(new LabelData(_localizationHelper.LocalizeOrDefault("Porters", p))));
            return menu;
        }
    }
}
