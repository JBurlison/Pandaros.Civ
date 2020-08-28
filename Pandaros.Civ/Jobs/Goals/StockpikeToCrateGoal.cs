using NPC;
using Pandaros.API;
using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class StockpikeToCrateGoal : INpcGoal
    {
        public static List<Vector3Int> InProgress { get; set; } = new List<Vector3Int>();

        public StockpikeToCrateGoal(IPandaJob job)
        {
            Job = job;
        }

        public IPandaJob Job { get; set; }
        public string Name { get; set; }
        public string LocalizationKey { get; set; }
        public Vector3Int CurrentCratePosition { get; set; } = Vector3Int.invalidPos;
        public List<Vector3Int> LastCratePosition { get; set; } = new List<Vector3Int>();

        public Vector3Int GetPosition()
        {
            var locations = Job.NPC.Position.SortClosestPositions(StorageFactory.CrateLocations[Job.Owner].Keys.ToList());

            foreach (var location in locations)
                if (!LastCratePosition.Contains(location) && 
                    !InProgress.Contains(location))
                {
                    CurrentCratePosition = location;
                    InProgress.Add(location);
                    break;
                }

            if (CurrentCratePosition == Vector3Int.invalidPos)
            {
                CurrentCratePosition = Job.Position;
            }

            return CurrentCratePosition;
        }

        public void LeavingGoal()
        {
           
        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            
        }
    }
}
