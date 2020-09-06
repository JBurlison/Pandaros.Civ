using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ;
using Pandaros.Civ.Jobs;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pandaros.Civ.TimePeriods.PreHistory.Jobs
{
    [ModLoader.ModManager]
    public static class SlowPorterModEntries
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, GameSetup.NAMESPACE + ".TimePeriods.PreHistory.Jobs.SlowPorterModEntry")]
        [ModLoader.ModCallbackProvidesFor("create_savemanager")]
        public static void AfterDefiningNPCTypes()
        {
            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
                new BlockJobManager<PandaGoalJob>(
                    new SlowPorterToCrate(),
                    (setting, pos, type, bytedata) => new PandaGoalJob(setting, pos, type, bytedata),
                    (setting, pos, type, colony) => new PandaGoalJob(setting, pos, type, colony)
                )
            );

            ServerManager.BlockEntityCallbacks.RegisterEntityManager(
               new BlockJobManager<PandaGoalJob>(
                   new SlowPorterFromCrate(),
                   (setting, pos, type, bytedata) => new PandaGoalJob(setting, pos, type, bytedata),
                   (setting, pos, type, colony) => new PandaGoalJob(setting, pos, type, colony)
               )
           );
        }
    }

    public class SlowPorterToCrateSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = SlowPorterToCrate.Name;
        public string printName { get; set; } = "Pre-History Porter to Crate";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 1.5f;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(66, 255, 129, 255);
        public Color32 maskColor0 { get; set; }
    }


    public class SlowPorterToCrate : PorterJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(SlowPorterToCrate));

        public SlowPorterToCrate() : base(Name, Name, PandaGoalJob.PorterJobType.ToCrate)
        {

        }
    }

    public class SlowPorterToCrateTexture : CSTextureMapping
    {
        public override string name => SlowPorterToCrate.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "PorterToCrate.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "PorterToCrate.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "PorterToCrate.png");
    }

    public class SlowPorterJobType : CSType
    {
        public override string name => SlowPorterToCrate.Name;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "SlowPorterToCrate.png");
        public override string onPlaceAudio => CommonSounds.WoodPlace;
        public override string onRemoveAudio => CommonSounds.WoodDeleteLight;
        public override string sideall => SlowPorterToCrate.Name;
        public override string sideyp => RoughWoodenBoard.NAME;
        public override string sideyn => RoughWoodenBoard.NAME;
        public override List<string> categories => new List<string>()
            {
                CommonCategories.Essential,
                CommonCategories.Storage,
                CommonCategories.Porter,
                "aa",
                nameof(TimePeriod.PreHistory),
                CommonCategories.Job,
                GameSetup.NAMESPACE
            };
    }

    public class SlowPorterToCrateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 2),
            new RecipeItem(RoughWoodenBoard.NAME, 4),
            new RecipeItem(LeafBasket.NAME),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SlowPorterToCrate.Name)
        };

        public string name => SlowPorterToCrate.Name;
    }

    public class SlowPorterToCrateJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 2),
            new RecipeItem(RoughWoodenBoard.NAME, 4),
            new RecipeItem(LeafBasket.NAME),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SlowPorterToCrate.Name)
        };

        public string name => SlowPorterToCrate.Name + Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }

    public class SlowPorterFromCrate : PorterJobSettings
    {
        public static string Name = GameSetup.GetNamespace("TimePeriods.PreHistory.Jobs", nameof(SlowPorterFromCrate));

        public SlowPorterFromCrate() : base(Name, Name, PandaGoalJob.PorterJobType.FromCrate)
        {

        }
    }

    public class SlowPorterFromCrateSettings : INPCTypeStandardSettings
    {
        public string keyName { get; set; } = SlowPorterFromCrate.Name;
        public string printName { get; set; } = "Pre-History Porter from Crate";
        public float inventoryCapacity { get; set; } = 300f;
        public float movementSpeed { get; set; } = 1.5f;
        public Color32 maskColor1 { get; set; } = new UnityEngine.Color32(37, 64, 31, 255);
        public Color32 maskColor0 { get; set; }
    }

    public class SlowPorterFromCrateTexture : CSTextureMapping
    {
        public override string name => SlowPorterFromCrate.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "PorterFromCrate.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "PorterFromCrate.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "PorterFromCrate.png");
    }

    public class PorterJobFromCrateType : CSType
    {
        public override string name => SlowPorterFromCrate.Name;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "SlowPorterFromCrate.png");
        public override string onPlaceAudio => CommonSounds.WoodPlace;
        public override string onRemoveAudio => CommonSounds.WoodDeleteLight;
        public override string sideall => SlowPorterFromCrate.Name;
        public override string sideyp => RoughWoodenBoard.NAME;
        public override string sideyn => RoughWoodenBoard.NAME;
        public override List<string> categories => new List<string>()
            {
                CommonCategories.Essential,
                CommonCategories.Storage,
                CommonCategories.Porter,
                "ab",
                nameof(TimePeriod.PreHistory),
                CommonCategories.Job,
                GameSetup.NAMESPACE
            };
    }

    public class SlowPorterFromCrateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 2),
            new RecipeItem(RoughWoodenBoard.NAME, 4),
            new RecipeItem(LeafBasket.NAME),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SlowPorterFromCrate.Name)
        };

        

        public string name => SlowPorterFromCrate.Name;
    }
    public class SlowPorterFromCrateJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(Wood.NAME, 2),
            new RecipeItem(RoughWoodenBoard.NAME, 4),
            new RecipeItem(LeafBasket.NAME),
            new RecipeItem(ColonyBuiltIn.ItemTypes.DIRT.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SlowPorterFromCrate.Name)
        };

        

        public string name => SlowPorterFromCrate.Name + Jobs.WoodWorker.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 10;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }
}

