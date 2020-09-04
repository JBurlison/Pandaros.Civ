using Science;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Research
{
    public class FoodStoredCondition : IResearchableCondition
    {
        public float CalorieCount { get; set; }

        public FoodStoredCondition(float calorieCount)
        {
            CalorieCount = calorieCount;
        }

        public bool IsConditionMet(AbstractResearchable researchable, ColonyScienceState manager)
        {
            return CalorieCount >= manager.Colony.Stockpile.TotalFood;
        }
    }
}
