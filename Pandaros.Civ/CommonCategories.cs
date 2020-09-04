using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ
{
    public static class CommonCategories
    {
        public static string Ingredient = "ingredient";
        public static string Wood = "wood";
        public static string Stone = "stone";
        public static string HighPriority = "aa";
        public static string MediumPriority = "aa";
        public static string Crate = "crate";
        public static string Storage = "storage";
        public static string Essential = "essential";
        public static string StockpileUpgrade = "upgrade";
        public static string RecruitmentItem = "recruitment";
        public static string Job = "job";
        public static string Food = "food";

        public static List<string> PreHistory = new List<string>
        {
            "aa",
            nameof(TimePeriod.PreHistory)
        };
        public static List<string> StoneAge = new List<string>
        {
            "ba",
            nameof(TimePeriod.StoneAge)
        };
        public static List<string> BronzeAge = new List<string>
        {
            "ca",
            nameof(TimePeriod.BronzeAge)
        };
    }
}
