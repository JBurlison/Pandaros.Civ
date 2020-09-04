using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.StoneAge.Items
{

    public class SturdyCrate : CSType, ICrate
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "SturdyCrate");
        public int MaxCrateStackSize { get; set; } = 200;
        public int MaxNumberOfStacks { get; set; } = 10;

        public SturdyCrate()
        {
            name = Name;
            sideall = "planks";
            categories = new List<string>()
            {
                "essential",
                "storage",
                "crate",
                "ba",
                "stoneage",
                GameSetup.NAMESPACE
            };
            isSolid = true;
            colliders = new Colliders()
            {
                collidePlayer = true,
                collideSelection = true,
                boxes = new List<Colliders.Boxes>()
                {
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0.5f, -0.4f }, new List<float>(){ -0.5f, -0.5f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0.5f, 0.5f }, new List<float>(){ -0.5f, -0.5f, 0.4f }),
                    new Colliders.Boxes(new List<float>(){ -0.4f, 0.5f, 0.5f }, new List<float>(){ -0.5f, -0.5f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, 0.5f, 0.5f }, new List<float>(){ 0.4f, -0.5f, -0.5f }),
                    new Colliders.Boxes(new List<float>(){ 0.5f, -0.4f, 0.5f }, new List<float>(){ -0.5f, -0.5f, -0.5f })
                }
            };
            onPlaceAudio = "woodPlace";
            onRemoveAudio = "woodDeleteLight";
            icon = GameSetup.Textures.GetPath(TextureType.icon, "SturdyCrate.png");
            mesh = GameSetup.MESH_PATH + "SturdyCrate.ply";
        }
    }

    public class SturdyCrateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.PLANKS.Id, 10),
            new RecipeItem(ColonyBuiltIn.ItemTypes.LEAVESTEMPERATE.Id, 5),
            new RecipeItem(PreHistory.Items.BasicCrate.Name)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(SturdyCrate.Name)
        };

        public bool isOptional => false;

        public string name => SturdyCrate.Name;
    }
}
