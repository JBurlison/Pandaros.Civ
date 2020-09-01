using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.TimePeriods
{
    public class PeriodFactory
    {
        public static TimePeriod GetTimePeriod(Colony c)
        {
            return TimePeriod.IndustrialAge;
        }
    }
}
