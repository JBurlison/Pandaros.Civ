using Pandaros.API.Entities;
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
            var cs = ColonyState.GetColonyState(c);

            if(cs.Stats.TryGetValue(nameof(TimePeriod), out var val))
                return (TimePeriod)val;
            else
            {
                cs.Stats[nameof(TimePeriod)] = (double)TimePeriod.PreHistory;
                return TimePeriod.PreHistory;
            }
        }
    }
}
