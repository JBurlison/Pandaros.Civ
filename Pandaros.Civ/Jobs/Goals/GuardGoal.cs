using BlockTypes;
using Jobs;
using Jobs.Implementations;
using Monsters;
using NPC;
using Pandaros.API;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pandaros.Civ.Jobs.Goals;
using Pandaros.Civ.Storage;
using Pipliz;
using Shared;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
 
    public class GuardGoal : IPandaNpcGoal
    {
        public static List<GuardGoal> CurrentGuards { get; set; } = new List<GuardGoal>();

        public GuardGoal(GuardJobInstance job, PandaGuardJobSettings settings)
        {
            GuardJob = job;
            Job = job;
            GuardSettings = settings;
            CurrentGuards.Add(this);
        }

        public Vector3Int ClosestCrate { get; set; }
        public GuardJobInstance GuardJob { get; set; }
        public GuardJobSettings GuardSettings { get; set; }
        public IJob Job { get; set; }
        public string Name { get; set; } = nameof(GuardGoal);
        public string LocalizationKey { get; set; } = GameSetup.GetNamespace("Jobs.Goals", nameof(GuardGoal));

        public Pipliz.Vector3Int GetPosition()
        {
            if (ClosestCrate == default(Vector3Int) || !StorageFactory.CrateTracker.Positions.TryGetValue(ClosestCrate, out var crate))
                ClosestCrate =StorageFactory.GetClosestCrateLocation(GuardJob.Position, Job.Owner);

            return GuardJob.Position;
        }

        public void LeavingGoal()
        {
           
        }

        public void LeavingJob()
        {
            CurrentGuards.Remove(this);
        }

        public void PerformGoal(ref NPCBase.NPCState state)
        {
            Pipliz.Vector3Int position = GuardJob.Position;
            state.SetCooldown(1);
            state.JobIsDone = true;

            if (!Job.NPC.Inventory.Contains(GuardSettings.ShootItem))
            {
                Shop(Job, ref Job.NPC.state);
                return;
            }

            if (GuardJob.HasTarget)
            {
                UnityEngine.Vector3 npcPos = position.Add(0, 1, 0).Vector;
                UnityEngine.Vector3 targetPos = GuardJob.Target.PositionToAimFor;
                if (VoxelPhysics.CanSee(npcPos, targetPos))
                {
                    GuardJob.NPC.LookAt(targetPos);
                    ShootAtTarget(GuardJob, ref state);
                    return;
                }
            }

            GuardJob.Target = MonsterTracker.Find(position.Add(0, 1, 0), GuardSettings.Range, GuardSettings.Damage);

            if (GuardJob.HasTarget)
            {
                GuardJob.NPC.LookAt(GuardJob.Target.PositionToAimFor);
                ShootAtTarget(GuardJob, ref state);
                return;
            }

            state.SetCooldown(GuardSettings.CooldownSearchingTarget * Pipliz.Random.NextFloat(0.9f, 1.1f));
            UnityEngine.Vector3 pos = GuardJob.NPC.Position.Vector;

            if (GuardSettings.BlockTypes.ContainsByReference(GuardJob.BlockType, out int index))
            {
                switch (index)
                {
                    case 1:
                        pos.x += 1f;
                        break;
                    case 2:
                        pos.x -= 1f;
                        break;
                    case 3:
                        pos.z += 1f;
                        break;
                    case 4:
                        pos.z -= 1f;
                        break;
                }
            }
            GuardJob.NPC.LookAt(pos);
        }

        public void SetAsGoal()
        {
            if (!Job.NPC.Inventory.Contains(GuardSettings.ShootItem))
            {
                Shop(Job, ref Job.NPC.state);
            }
        }

        public virtual void ShootAtTarget(GuardJobInstance instance, ref NPCBase.NPCState state)
        {
            if (state.Inventory.TryRemove(GuardSettings.ShootItem))
            {
                if (GuardSettings.OnShootAudio != null)
                {
                    AudioManager.SendAudio(instance.Position.Vector, GuardSettings.OnShootAudio);
                }
                if (GuardSettings.OnHitAudio != null)
                {
                    AudioManager.SendAudio(instance.Target.PositionToAimFor, GuardSettings.OnHitAudio);
                }
                UnityEngine.Vector3 start = instance.Position.Add(0, 1, 0).Vector;
                UnityEngine.Vector3 end = instance.Target.PositionToAimFor;
                UnityEngine.Vector3 dirNormalized = (end - start).normalized;
                ServerManager.SendParticleTrail(start + dirNormalized * 0.15f, end - dirNormalized * 0.15f, Pipliz.Random.NextFloat(1.5f, 2.5f));
                instance.Target.OnHit(GuardSettings.Damage, instance.NPC, ModLoader.OnHitData.EHitSourceType.NPC);
                state.SetIndicator(new IndicatorState(GuardSettings.CooldownShot, GuardSettings.ShootItem[0].Type));
                if (GuardSettings.OnShootResultItem.item.Type > 0 && Pipliz.Random.NextDouble(0.0, 1.0) <= (double)GuardSettings.OnShootResultItem.chance)
                {
                    state.Inventory.Add(GuardSettings.OnShootResultItem.item);
                }
            }
            else
            {
                Shop(instance, ref state);
            }
        }

        private void Shop(IJob instance, ref NPCBase.NPCState state)
        {
            var items = GuardSettings.ShootItem.Select(i => new StoredItem(i.Type, i.Amount * 50)).ToArray();
            var getitemsfromCrate = new GetItemsFromCrateGoal(instance, GuardJob.Position, this, items, this);
            PandaJobFactory.SetActiveGoal(instance, new PutItemsInCrateGoal(Job, GuardJob.Position, getitemsfromCrate, state.Inventory.Inventory, this), ref state);
            state.Inventory.Inventory.Clear();
            state.Inventory.Add(items);
        }

        public Vector3Int GetCrateSearchPosition()
        {
            return GuardJob.Position;
        }

        public Dictionary<ushort, StoredItem> GetItemsNeeded()
        {
            var items = new Dictionary<ushort, StoredItem>();

            if (GuardSettings?.ShootItem != null)
                items.AddRange(GuardSettings.ShootItem);

            return items;
        }
    }
}
