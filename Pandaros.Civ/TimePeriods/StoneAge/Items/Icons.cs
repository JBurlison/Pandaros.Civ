using Pandaros.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.StoneAge.Items
{
    public class BronzeAgeIcon : CSType
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.StoneAge.Items", nameof(BronzeAgeIcon));
        public override string name { get; set; } = NAME;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "Pandaros.Civ.TimePeriods.PreHistory.Research.BronzeAgeResearch.png");
        public override bool? isPlaceable => false;
    }
}
