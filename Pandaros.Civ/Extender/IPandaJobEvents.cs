using NPC;
using Pandaros.API.Extender;
using Pandaros.Civ.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Extender
{
    public interface IPandaJobEventsExtender : IPandarosExtention
    {
        void GoalChanged(IPandaJob job, INpcGoal oldGold, INpcGoal newGoal);
    }

    public interface IPandaJobEvents
    {
        void GoalChanged(IPandaJob job, INpcGoal oldGold, INpcGoal newGoal);
    }
}
