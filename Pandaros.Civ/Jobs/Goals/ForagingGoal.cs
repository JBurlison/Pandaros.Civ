using Jobs;
using NPC;
using Pandaros.API.Items;
using Pandaros.API.Models;
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
        public ForagingGoal(IJob job, IPandaJobSettings settings, Vector3Int pos, ILootTable lootTable)
        {
            Job = job;
            JobSettings = settings;
            JobPos = pos;
            LootTable = lootTable;
        }

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

        public Vector3Int GetPosition()
        {
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
            }


        }

        public void SetAsGoal()
        {
            
        }
    }
}
