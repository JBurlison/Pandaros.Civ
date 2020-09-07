using AI;
using Jobs;
using NPC;
using Pandaros.API;
using Pandaros.API.Items;
using Pandaros.API.Models;
using Pandaros.Civ.Storage;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class ForagingGoal : INpcGoal
    {
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
        public Vector3Int EdgeOfColony { get; set; }
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
                ClosestCrate = JobPos.GetClosestPosition(crateLocs.Keys.ToList());

            if (!Foraging)
            {
                var radius = Job.Owner.BannerSafeRadius + 15;
                var xp = Job.Owner.Banners[0].Position.Add(radius, 0, 0);
                var xn = Job.Owner.Banners[0].Position.Add(radius * -1, 0, 0);
                var zp = Job.Owner.Banners[0].Position.Add(0, 0, radius);
                var zn = Job.Owner.Banners[0].Position.Add(0, 0, radius * -1);

                var distances = new Dictionary<Vector3Int, float>()
                {
                    { xp, UnityEngine.Vector3Int.Distance(JobPos, xp) },
                    { xn, UnityEngine.Vector3Int.Distance(JobPos, xn) },
                    { zp, UnityEngine.Vector3Int.Distance(JobPos, zp) },
                    { zn, UnityEngine.Vector3Int.Distance(JobPos, zn) }
                };

                EdgeOfColony = distances.OrderBy(kvp => kvp.Value).First().Key;
            }
            else
            {
                return ForagingPos;
            }

            return EdgeOfColony;
        }

        public void LeavingGoal()
        {
            
        }

        public void LeavingJob()
        {
            ComeHome();
        }

        private void ComeHome()
        {
            if (PathingManager.TryCanStandNearNotAt(EdgeOfColony, out var canStand, out var pos))
                Job.NPC.SetPosition(pos);
            else if (PathingManager.TryCanStandNearNotAt(JobPos, out canStand, out pos))
                Job.NPC.SetPosition(pos);
            else
                Job.NPC.SetPosition(Job.Owner.Banners.FirstOrDefault().Position);
        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            state.JobIsDone = true;

            if (!Foraging)
            {
                ForagingPos = EdgeOfColony.Add(0, -60, 0);
                Job.NPC.SetPosition(ForagingPos);
                Foraging = true;
                var nextTime = Pipliz.Random.Next(ForagingTimeMinSec, ForagingTimeMaxSec);
                ForageEndTime = ServerTimeStamp.Now.Add(nextTime * 1000);
                state.SetCooldown(nextTime);
            }
            else if (ForageEndTime.IsPassed)
            {
                var items = LootTable.GetDrops(LuckMod);

                foreach (var item in items)
                    Job.NPC.Inventory.Add(item.Key, item.Value);

                state.SetCooldown(1);
                ComeHome();
                Foraging = false;
                PandaJobFactory.SetActiveGoal(Job, new PutItemsInCrateGoal(Job, JobPos, this, Job.NPC.Inventory.Inventory, this), ref state);
                Job.NPC.Inventory.Inventory.Clear();
            }
            else
            {
                state.SetCooldown(5);
            }
        }

        public void SetAsGoal()
        {
            
        }
    }
}
