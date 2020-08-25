using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLoaderInterfaces;
using NPC;
using Pipliz;

namespace Pandaros.Civ.Jobs
{
    public class PandaJobFactory : IOnNPCRecruited, IOnNPCDied
    {
        public Dictionary<IPandaJob, Vector3Int> JobLocations { get; set; } = new Dictionary<IPandaJob, Vector3Int>();
        public List<IPandaJob> AvailableJobs { get; set; } = new List<IPandaJob>();

        public void OnNPCDied(NPCBase npc)
        {
            
        }

        public void OnNPCRecruited(NPCBase npc)
        {
            
        }
    }
}
