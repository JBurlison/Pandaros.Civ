using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods.BronzeAge.Items
{

    public class PaddedCrate : CSType, ICrate
    {
        public static string Name { get; private set; } = GameSetup.GetNamespace("Storage", "PaddedCrate");
        public int MaxCrateStackSize { get; set; } = 300;
        public int MaxNumberOfStacks { get; set; } = 15;

        public PaddedCrate()
        {
            name = Name;
            sideall = "coatedplanks";
            categories = new List<string>()
            {
                CommonCategories.Essential,
                CommonCategories.Storage,
                CommonCategories.Crate,
                "ca",
                nameof(TimePeriod.BronzeAge),
                GameSetup.NAMESPACE
            };
            onPlaceAudio = "woodPlace";
            onRemoveAudio = "woodDeleteLight";
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
            icon = GameSetup.Textures.GetPath(TextureType.icon, "PaddedCrate.png");
            mesh = GameSetup.MESH_PATH + "SturdyCrate.ply";
        }
    }

    public class PaddedCrateRecipe : ICSPlayerRecipe
    {
        public List<RecipeItem> requires => new List<RecipeItem>()
        {
            new RecipeItem(ColonyBuiltIn.ItemTypes.COATEDPLANKS.Id, 6),
            new RecipeItem(ColonyBuiltIn.ItemTypes.COPPERPARTS.Id, 3),
            new RecipeItem(StoneAge.Items.SturdyCrate.Name)
        };

        public List<RecipeResult> results => new List<RecipeResult>()
        {
            new RecipeResult(PaddedCrate.Name)
        };

        

        public string name => PaddedCrate.Name;
    }
}
