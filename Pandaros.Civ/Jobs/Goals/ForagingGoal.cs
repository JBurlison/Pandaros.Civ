using AI;
using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.API.Items;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Pandaros.Civ.TimePeriods.PreHistory.Items;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class ForagingGoal : IPandaNpcGoal
    {
        public enum ForagingState
        {
            HeadingOut,
            HeadingBack
        }

        public ForagingGoal(IJob job,  Vector3Int pos, ILootTable lootTable, int foragingTimeMinSec, int foragingTimeMaxSec, float lootLuckModifier = 0f)
        {
            Job = job;
            JobPos = pos;
            LootTable = lootTable;
            ForagingTimeMaxSec = foragingTimeMaxSec;
            ForagingTimeMinSec = foragingTimeMinSec;
            LuckMod = lootLuckModifier;
        }

        public int ForagingTimeMinSec { get; set; }
        public int ForagingTimeMaxSec { get; set; }
        public float LuckMod { get; set; }
        public ILootTable LootTable { get; set; }
        public Vector3Int JobPos { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(ForagingGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Jobs.Goals", nameof(ForagingGoal));
        public Vector3Int ClosestCrate { get; set; }
        public Vector3Int ForagingPos { get; set; }
        public bool Foraging { get; set; } = false;
      
        public ServerTimeStamp ForageEndTime { get; set; }

        public Vector3Int GetCrateSearchPosition()
        {
            return JobPos;
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            return null;
        }

        public Vector3Int GetPosition()
        {
            if (StorageFactory.CrateLocations.TryGetValue(Job.Owner, out var crateLocs) &&
               (ClosestCrate == default(Vector3Int) || !crateLocs.ContainsKey(ClosestCrate)))
                ClosestCrate = StorageFactory.GetClosestCrateLocation(JobPos, Job.Owner);

            if (!Foraging)
            {
                var radius = Job.Owner.BannerSafeRadius - 1;

                var distances = new List<Vector3Int>()
                {
                    Job.Owner.Banners[0].Position.Add(radius, 0, 0),
                    Job.Owner.Banners[0].Position.Add(radius * -1, 0, 0),
                    Job.Owner.Banners[0].Position.Add(0, 0, radius),
                    Job.Owner.Banners[0].Position.Add(0, 0, radius * -1)
                };

                bool posFound = false;
                distances.Shuffle();
                foreach (var pos in distances)
                {
                    var getEdge = pos.GetClosestPositionWithinY(pos, 6);
                    if (getEdge != Vector3Int.invalidPos && getEdge != default(Vector3Int) && getEdge != pos)
                    {
                        ForagingPos = getEdge;
                        posFound = true;
                        break;
                    }
                }

                if (!posFound)
                    ForagingPos = JobPos;

                RandomizePos();
            }

            return ForagingPos;
        }

        public void LeavingGoal()
        {
            
        }

        public void LeavingJob()
        {

        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            state.JobIsDone = true;

            if (!Foraging)
            {
                Foraging = true;
                var nextTime = Pipliz.Random.Next(ForagingTimeMinSec, ForagingTimeMaxSec);
                ForageEndTime = ServerTimeStamp.Now.Add(nextTime * 1000);
                state.SetCooldown(1, 2);
                state.SetIndicator(new Shared.IndicatorState(Pipliz.Random.NextFloat(1, 2), LootTable.LootPoolList.GetRandomItem().Item));
            }
            else if (ForageEndTime.IsPassed)
            {
                var items = LootTable.GetDrops(LuckMod);

                foreach (var item in items)
                    Job.NPC.Inventory.Add(item.Key, item.Value);

                state.SetCooldown(1);

                Foraging = false;
                PandaJobFactory.SetActiveGoal(Job, new PutItemsInCrateGoal(Job, JobPos, this, Job.NPC.Inventory.Inventory, this), ref state);
                Job.NPC.Inventory.Inventory.Clear();
            }
            else
            {
                RandomizePos();
                var nextTime = Pipliz.Random.Next(5, 7);
                state.SetCooldown(nextTime);
                state.SetIndicator(new Shared.IndicatorState(nextTime, LootTable.LootPoolList.GetRandomItem().Item));
            }
        }

        private void RandomizePos()
        {
            var newPos = ForagingPos;
            int blocks = Pipliz.Random.Next(1, 6);
            

            switch (Pipliz.Random.Next(1, 5))
            {
                case 1:
                    newPos = newPos.Add(blocks, 0, 0);
                    break;
                case 2:
                    newPos = newPos.Add(blocks * -1, 0, 0);
                    break;
                case 3:
                    newPos = newPos.Add(0, 0, blocks);
                    break;
                case 4:
                    newPos = newPos.Add(0, 0, blocks * -1);
                    break;

            }

            if (newPos != ForagingPos)
                ForagingPos = newPos.GetClosestPositionWithinY(Job.NPC.Position, 6);
        }

        public void SetAsGoal()
        {
            
        }
    }
}
