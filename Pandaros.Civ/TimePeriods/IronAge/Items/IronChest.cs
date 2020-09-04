﻿using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.IronAge.Items
{
    public class IronChestTexture : CSTextureMapping
    {
        public override string name => IronChest.Name;
        public override string albedo => GameSetup.Textures.GetPath(TextureType.aldebo, "IronChest.png");
        public override string height => GameSetup.Textures.GetPath(TextureType.height, "IronChest.png");
        public override string normal => GameSetup.Textures.GetPath(TextureType.normal, "IronChest.png");
    }
    public class IronChestBase : CSType, ICrate
    {
        public int MaxCrateStackSize { get; set; } = 300;
        public int MaxNumberOfStacks { get; set; } = 15;
        public override string sideall => IronChest.Name;
        public override List<string> categories => new List<string>()
        {
            "essential",
            "storage",
            "crate",
            "da",
            "ironage",
            GameSetup.NAMESPACE
        };
        public override Colliders colliders => new Colliders()
        {
            collidePlayer = true,
            collideSelection = true,
            boxes = new List<Colliders.Boxes>()
            {
                new Colliders.Boxes(new List<float>(){ 0.5f, 0.18f, 0.28f }, new List<float>(){ -0.5f, -0.5f, -0.28f })
            }
        };
        public override bool? isSolid => true;
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "IronChest.png");
        public override string mesh => GameSetup.MESH_PATH + "IronChest.ply";

    }

    public class IronChest : CSGenerateType
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "IronChest");
        public IronChest()
        {
            typeName = Name;
            generateType = "rotateBlock";
            baseType = new IronChestBase();
        }
    }


    public class IronChestRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.COATEDPLANKS.Id, 10),
            new RecipeItem(ColonyBuiltIn.ItemTypes.IRONINGOT.Id, 3),
            new RecipeItem(ColonyBuiltIn.ItemTypes.COPPERPARTS.Id, 5)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(IronChest.Name)
        };

        public bool isOptional => false;

        public string name => IronChest.Name;
    }
}