using ModLoaderInterfaces;
using NPC;
using Pandaros.Civ.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Extender.Providers
{
    public class PandaJobEventsProvider : IPandaJobEventsExtender, IAfterWorldLoad
    {
        public List<Type> LoadedAssembalies { get; } = new List<Type>();

        public string InterfaceName => nameof(IPandaJobEvents);

        public Type ClassType => null;

        public List<IPandaJobEvents> LoadedInstances { get; set; } = new List<IPandaJobEvents>();

        public void AfterWorldLoad()
        {
            if (LoadedInstances.Count == 0)
                foreach (var jobExtender in LoadedAssembalies)
                    if (Activator.CreateInstance(jobExtender) is IPandaJobEvents pandaJobEvent)
                        LoadedInstances.Add(pandaJobEvent);
        }

        public void GoalChanged(IPandaJob job, INpcGoal oldGold, INpcGoal newGoal)
        {
            foreach (var instance in LoadedInstances)
                instance.GoalChanged(job, oldGold, newGoal);
        }

        public void NPCSet(IPandaJob job, NPCBase oldNpc, NPCBase newNpc)
        {
            foreach (var instance in LoadedInstances)
                instance.NPCSet(job, oldNpc, newNpc);
        }
    }
}
