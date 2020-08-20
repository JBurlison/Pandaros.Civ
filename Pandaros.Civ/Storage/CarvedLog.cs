﻿using Newtonsoft.Json.Linq;
using Pandaros.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Storage
{
    public class CarvedLogTexture : CSTextureMapping
    {
        public override string name => CarvedLog.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "CarvedLog.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "CarvedLog.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "CarvedLog.png");
    }

    public class CarvedLog : CSType, IStorageUpgradeBlock
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "CarvedLog");
        public Dictionary<string, int> CategoryUpgrades { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ItemUpgrades { get; set; } = new Dictionary<string, int>();
        public int GlobalUpgrade { get; set; }

        public CarvedLog()
        {
            name = Name;
            GlobalUpgrade = 4;
            CategoryUpgrades.Add("food", 4);
            sideall = GameSetup.Textures.SELF;
            categories = new List<string>()
            {
                "storage"
            };
            isSolid = true;
            customData = JObject.Parse("{ \"attachBehaviour\" : [{\"id\":\"crate\"}] }");
            icon = GameSetup.Textures.GetPath(TextureType.icon, "CarvedLog.png");
        }
    }
}
