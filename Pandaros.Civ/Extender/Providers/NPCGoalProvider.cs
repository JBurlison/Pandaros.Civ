using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Extender;
using Pandaros.Civ.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Extender.Providers
{
    public class NPCGoalProvider : IAfterModsLoadedExtention
    {
        public List<Type> LoadedAssembalies { get; } = new List<Type>();

        public string InterfaceName => nameof(INpcGoal);

        public Type ClassType => null;

        public void AfterModsLoaded(List<ModLoader.ModDescription> list)
        {
            foreach (var jobExtender in LoadedAssembalies)
                if (Activator.CreateInstance(jobExtender) is INpcGoal pandaGoal)
                {
                    PandaJobFactory.NPCGoals[pandaGoal.GoalName] = pandaGoal;
                }
        }
    }
}
