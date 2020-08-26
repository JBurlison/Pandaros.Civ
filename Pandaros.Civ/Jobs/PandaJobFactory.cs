using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Entities;
using Pandaros.API.Extender;
using Pipliz;
using Pipliz.JSON;

namespace Pandaros.Civ.Jobs
{
    public class PandaJobFactory : IOnNPCRecruited, IOnNPCDied, IOnNPCLoaded, IOnNPCSaved, IOnSavingColony, IOnLoadingColony, IOnTimedUpdate, IOnTryChangeBlock
    {
        public static Dictionary<string, INpcGoal> NPCGoals { get; set; } = new Dictionary<string, INpcGoal>();
        public static Dictionary<Colony, Dictionary<Vector3Int, int>> JobLocations { get; set; } = new Dictionary<Colony, Dictionary<Vector3Int, int>>();
        public static Dictionary<Colony, Dictionary<int, IPandaJob>> AvailableJobs { get; set; } = new Dictionary<Colony, Dictionary<int, IPandaJob>>();
        public static Dictionary<Colony, Dictionary<int, IPandaJob>> TakenJobs { get; set; } = new Dictionary<Colony, Dictionary<int, IPandaJob>>();
        public static Dictionary<Colony, Dictionary<int, IPandaJob>> AllJobs { get; set; } = new Dictionary<Colony, Dictionary<int, IPandaJob>>();
        public static Dictionary<Colony, int> JobIndexs { get; set; } = new Dictionary<Colony, int>();

        public int NextUpdateTimeMinMs => 5000;

        public int NextUpdateTimeMaxMs => 10000;

        public ServerTimeStamp NextUpdateTime { get; set; }

        public static int GetNextIndex(Colony c)
        {
            int index = 0;

            lock (JobIndexs)
            {
                if (!JobIndexs.TryGetValue(c, out index))
                    JobIndexs[c] = index;

                index++;
                JobIndexs[c] = index;
            }

            return index;
        }

        public void OnNPCDied(NPCBase npc)
        {
            var job = npc.GetPandaJob();

            if (job != null)
            {
                TakenJobs[npc.Colony].Remove(job.JobId);
                AvailableJobs[npc.Colony][job.JobId] = job;
            }
        }

        public void OnNPCLoaded(NPCBase npc, JSONNode savedData)
        {
            var job = npc.GetPandaJob();

        }

        public void OnNPCRecruited(NPCBase npc)
        {
            
        }

        public void OnNPCSaved(NPCBase npc, JSONNode data)
        {
            
        }

        public void OnSavingColony(Colony colony, JSONNode data)
        {
            if (AllJobs.TryGetValue(colony, out var jobs))
                foreach (var kvp in jobs)
                {

                }
        }

        public void OnLoadingColony(Colony colony, JSONNode data)
        {
            if (!AllJobs.ContainsKey(colony))
                AllJobs.Add(colony, new Dictionary<int, IPandaJob>());
        }

        public void OnTimedUpdate()
        {
            
        }

        public void OnTryChangeBlock(ModLoader.OnTryChangeBlockData data)
        {
            
        }
    }
}
