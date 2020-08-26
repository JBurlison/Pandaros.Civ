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
    public class PandaJobFactory : IOnNPCRecruited, IOnNPCDied, IOnNPCLoaded, IOnNPCSaved, IOnSavingColony, IOnLoadingColony, IOnTimedUpdate
    {
        public static Dictionary<string, INpcGoal> NPCGoals { get; set; } = new Dictionary<string, INpcGoal>();
        public static Dictionary<IPandaJob, Vector3Int> JobLocations { get; set; } = new Dictionary<IPandaJob, Vector3Int>();
        public static List<IPandaJob> AvailableJobs { get; set; } = new List<IPandaJob>();
        public static Dictionary<int, IPandaJob> TakenJobs { get; set; } = new Dictionary<int, IPandaJob>();

        public int NextUpdateTimeMinMs => 5000;

        public int NextUpdateTimeMaxMs => 10000;

        public ServerTimeStamp NextUpdateTime { get; set; }

        public void OnNPCDied(NPCBase npc)
        {
            var job = npc.GetPandaJob();

            if (job != null)
            {
                TakenJobs.Remove(job.JobId);
                AvailableJobs.Add(job);
            }
        }

        public void OnNPCLoaded(NPCBase npc, JSONNode savedData)
        {

        }

        public void OnNPCRecruited(NPCBase npc)
        {
            
        }

        public void OnNPCSaved(NPCBase npc, JSONNode data)
        {
            
        }

        public void OnSavingColony(Colony colony, JSONNode data)
        {
            
        }

        public void OnLoadingColony(Colony colony, JSONNode data)
        {

        }

        public void OnTimedUpdate()
        {
            
        }
    }
}
