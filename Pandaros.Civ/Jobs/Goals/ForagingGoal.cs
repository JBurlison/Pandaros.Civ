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
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class ForagingGoal : INpcGoal
    {
        public ForagingGoal(IJob job, IPandaJobSettings settings, Vector3Int pos, ILootTable lootTable, int foragingTimeMinSec, int foragingTimeMaxSec, float lootLuckModifier = 0f)
        {
            Job = job;
            JobSettings = settings;
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
        public IPandaJobSettings JobSettings { get; set; }
        public string Name { get; set; }
        public string LocalizationKey { get; set; }
        public Vector3Int ClosestCrate { get; set; }
        public Vector3Int EdgeOfColony { get; set; }
        public Vector3Int ForagingPos { get; set; }
        public bool Foraging { get; set; } = false;
      
        public ServerTimeStamp ForageEndTime { get; set; }

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
            
        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            if (!Foraging)
            {
                ForagingPos = EdgeOfColony.Add(0, -100, 0);
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
                Job.NPC.SetPosition(EdgeOfColony);
                Foraging = false;
                JobSettings.SetGoal(Job, new PutItemsInCrateGoal(Job, JobSettings, this, Job.NPC.Inventory.Inventory, this), ref state);
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
