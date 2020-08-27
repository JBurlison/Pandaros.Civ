using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pandaros.Civ.Extender;
using Pipliz;
using Pipliz.JSON;

namespace Pandaros.Civ.Jobs
{
    public class PandaJobFactory : IOnTryChangeBlock, IOnNPCDied
    {
        public static Dictionary<string, IPandaJobType> JobTypes { get; set; } = new Dictionary<string, IPandaJobType>();

        public void OnNPCDied(NPCBase npc)
        {
            if (npc.Job != null && npc.Job is PandaJob pandaJob)
            {
                pandaJob.GoalChanged -= Job_GoalChanged;
            }
        }

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData data)
        {
            var colony = data?.RequestOrigin.AsPlayer?.ActiveColony;

            if (data.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player &&
                colony != null)
            {
                if (JobTypes.TryGetValue(data.TypeNew.Name, out var jobType))
                {
                    var job = new PandaJob(colony, data.Position, jobType.NPCTypeName, jobType.RecruitmentItem, jobType.JobBlock, jobType.StartingGoal, jobType.SleepNight);

                    job.GoalChanged += Job_GoalChanged;

                    colony.JobFinder.Add(job);
                }
            }
        }

        private void Job_GoalChanged(object sender, (INpcGoal, INpcGoal) e)
        {
            foreach (var cb in PandarosAPIExtender.GetCallbacks<IPandaJobEventsExtender>())
            {
                cb.GoalChanged((IPandaJob)sender, e.Item1, e.Item2);
            }
        }
    }
}
