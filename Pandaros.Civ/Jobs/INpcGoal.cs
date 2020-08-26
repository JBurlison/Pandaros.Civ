using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public interface INpcGoal
    {
        string GoalName { get; set; }
        IPandaJob Job { get; set; }
        string Name { get; set; }
        string LocalizationKey { get; set; }
        Vector3Int Location { get; set; }

        void AtGoal();

        void PerformGoal();

        void LeavingGoal();

        INpcGoal NewGoalInstance();
    }
}
