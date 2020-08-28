using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public class PandaJobFactory : IOnTryChangeBlock
    {
        public static Dictionary<string, IPandaJobSettings> JobTypes { get; set; } = new Dictionary<string, IPandaJobSettings>();
        public static Dictionary<Colony, Dictionary<Vector3Int, IPandaJob>> JobsByLocation { get; set; } = new Dictionary<Colony, Dictionary<Vector3Int, IPandaJob>>();

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData data)
        {
            var colony = data?.RequestOrigin.AsPlayer?.ActiveColony;

            if (data.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player &&
                colony != null)
            {
                if (!JobsByLocation.ContainsKey(colony))
                    JobsByLocation.Add(colony, new Dictionary<Vector3Int, IPandaJob>());

                if (JobTypes.TryGetValue(data.TypeNew.Name, out var jobType))
                {
                    var job = new PandaJob(colony, data.Position, jobType.keyName, jobType.RecruitmentItem, jobType.JobBlock, jobType.StartingGoal, jobType.SleepNight);

                    job.GoalChanged += Job_GoalChanged;
                    JobsByLocation[colony][data.Position] = job;

                    colony.JobFinder.Add(job);
                }
                else if (JobsByLocation[colony].TryGetValue(data.Position, out var job))
                {
                    job.GoalChanged -= Job_GoalChanged;
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
