using Pandaros.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.PreHistory.Items
{
    public class StoneAgeIcon : CSType
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(StoneAgeIcon));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "Pandaros.Civ.TimePeriods.PreHistory.Research.StoneAgeResearch.png");
        public override bool? isPlaceable => false;
    }

    public class Crates_tutorialIcon : CSType
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(Crates_tutorialIcon));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "crates_tutorial.png");
        public override bool? isPlaceable => false;
    }

    public class stockpile_tutorialIcon : CSType
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(stockpile_tutorialIcon));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "stockpile_tutorial.png");
        public override bool? isPlaceable => false;
    }
}
