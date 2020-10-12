using Pandaros.API;
using Pandaros.API.Models;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pipliz.JSON;

namespace Pandaros.Civ.TimePeriods.PreHistory.Items
{ 
    public class LeafBedBase : CSType
    {
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, nameof(TimePeriod.PreHistory) + "/" + "LeafBed.png");
        public override bool? isPlaceable => true;
        public override int? maxStackSize => 200;
        public override List<string> categories { get; set; } = new List<string>()
        {
            CommonCategories.Ingredient,
            CommonCategories.Wood,
            nameof(TimePeriod.PreHistory),
            GameSetup.NAMESPACE
        };
        public override Colliders colliders => new Colliders()
        {
            collidePlayer = false,
            collideSelection = true,
            boxes = new List<Colliders.Boxes>()
            {
                new Colliders.Boxes(new List<float>(){ 0.5f, 0f, 0.5f }, new List<float>(){ -0.5f, -0.5f, -0.5f })
            }
        };
        public override string onPlaceAudio => "woodPlace";
        public override string onRemoveAudio => "woodDeleteLight";
        public override bool? isSolid => false;
        public override string mesh => GameSetup.MESH_PATH + "SlabDown.ply";
        public override string sideall => ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Name;
        public override JObject customData => JsonConvert.DeserializeObject<JObject>("{ \"bedpart\": \"head\", \"useHeightMap\": true, \"useNormalMap\": true }");
    }
    public class LeafBed : CSGenerateType
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(LeafBed));
        public LeafBed()
                {
                    typeName = NAME;
                    generateType = "rotateBlock";
                    baseType = new LeafBedBase();
                }
    }

    public class LeafBedHeadTexture : CSTextureMapping
    {
        public override string name => LeafBed.NAME + "BedHead";
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "BedHead.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "BedHead.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "BedHead.png");
    }

    public class LeafBedFootTexture : CSTextureMapping
    {
        public override string name => LeafBed.NAME + "BedFoot";
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "BedFoot.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "BedFoot.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "BedFoot.png");
    }

    public class LeafBedFootBase : CSType
    {
        public override string parentType => LeafBed.NAME;
        public override JObject customData  => JsonConvert.DeserializeObject<JObject>("{ \"bedpart\": \"foot\", \"useHeightMap\": true, \"useNormalMap\": true }");
        public override string sideall => ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Name;
        public override string sideyp { get; set; } = LeafBed.NAME + "BedFoot";
        public override Colliders colliders => new Colliders()
        {
            collidePlayer = false,
            collideSelection = true,
            boxes = new List<Colliders.Boxes>()
            {
                new Colliders.Boxes(new List<float>(){ 0.5f, 0f, 0.5f }, new List<float>(){ -0.5f, -0.5f, -0.5f })
            }
        };
    }
    public class LeafBedFoot : CSGenerateType
    {
        public static string NAME = GameSetup.GetNamespace("TimePeriods.PreHistory.Items", nameof(LeafBedFoot));
        public LeafBedFoot()
        {
            typeName = NAME;
            generateType = "rotateBlock";
            baseType = new LeafBedFootBase();
        }
    }

    public class LeafBedTaigaRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBed.NAME, 1)
        };



        public string name => Wood.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Name + "player";
    }

    public class LeafBedTemperateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBed.NAME, 1)
        };


        public string name => Wood.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Name + "player";
    }

    public class LeafBedTaigaJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBed.NAME, 1)
        };

        

        public string name => Wood.NAME + ColonyBuiltIn.ItemTypes.LEAVESTAIGA.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 5;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }

    public class LeafBedTemperateJobRecipe : ICSRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(LeafBed.NAME, 1)
        };

        
        public string name => Wood.NAME + ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Name;
        public CraftPriority defaultPriority => CraftPriority.Medium;
        public int defaultLimit => 5;

        public string Job => Jobs.WoodWorker.Name;

        public List<string> JobBlock => new List<string>() { Jobs.WoodWorker.Name };
    }
}
