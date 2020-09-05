﻿using Pandaros.API.Models;
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
}