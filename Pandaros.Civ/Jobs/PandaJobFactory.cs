using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLoaderInterfaces;
using NPC;
using Pandaros.API.Entities;
using Pipliz;
using Pipliz.JSON;

namespace Pandaros.Civ.Jobs
{
    public class PandaJobFactory : IOnNPCRecruited, IOnNPCDied, IOnNPCLoaded, IOnNPCSaved, IOnSavingColony, IOnLoadingColony
    {
        public static Dictionary<string, INpcGoal> NPCGoals { get; set; } = new Dictionary<string, INpcGoal>();
        public static Dictionary<IPandaJob, Vector3Int> JobLocations { get; set; } = new Dictionary<IPandaJob, Vector3Int>();
        public static List<IPandaJob> AvailableJobs { get; set; } = new List<IPandaJob>();
        public static List<IPandaJob> TakenJobs { get; set; } = new List<IPandaJob>();


        public void OnNPCDied(NPCBase npc)
        {
            
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
    }
}
