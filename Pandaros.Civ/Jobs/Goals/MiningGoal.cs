using BlockTypes;
using Jobs;
using Jobs.Implementations;
using NPC;
using Pandaros.API;
using Pandaros.Civ.Jobs.BaseReplacements;
using Pandaros.Civ.Storage;
using Pipliz;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class MiningGoal : INpcGoal
    {
		public MiningGoal(BlockJobInstance job, PandaMiningJobSettings settings)
        {
			MinerSettings = settings;
			Job = job;
			BlockJobInstance = job;
			JobSettings = settings;
		}

		protected static List<ItemTypes.ItemTypeDrops> GatherResults = new List<ItemTypes.ItemTypeDrops>();
		public Vector3Int ClosestCrate { get; set; }
		public MinerJobSettings MinerSettings { get; set; }
		public BlockJobInstance BlockJobInstance { get; set; }
        public IJob Job { get; set; }
        public IPandaJobSettings JobSettings { get; set; }
        public string Name { get; set; }
        public string LocalizationKey { get; set; }

        public Vector3Int GetPosition()
        {
			if (StorageFactory.CrateLocations.TryGetValue(Job.Owner, out var crateLocs) &&
				(ClosestCrate == default(Vector3Int) || !crateLocs.ContainsKey(ClosestCrate)))
				ClosestCrate = BlockJobInstance.Position.GetClosestPosition(crateLocs.Keys.ToList());

			return BlockJobInstance.Position;
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
			MinerJobInstance instance = (MinerJobInstance)BlockJobInstance;
			if (instance.BlockTypeBelow == null || instance.BlockTypeBelow == BuiltinBlocks.Types.air)
			{
				if (!World.TryGetTypeAt(instance.Position.Add(0, -1, 0), out ItemTypes.ItemType foundType))
				{
					state.SetCooldown(5.0, 7.0);
					return;
				}
				if (foundType == BuiltinBlocks.Types.air)
				{
					ThreadManager.InvokeOnMainThread(delegate
					{
						ServerManager.TryChangeBlock(instance.Position, instance.BlockType, BuiltinBlocks.Types.air, instance.Owner);
					});
					state.SetCooldown(2.5, 3.5);
					return;
				}
				instance.BlockTypeBelow = foundType;
			}
			if (instance.MiningCooldown <= 0f)
			{
				float cooldown = 0f;
				if (instance.BlockTypeBelow.CustomDataNode?.TryGetAs("minerMiningTime", out cooldown) ?? false)
				{
					instance.MiningCooldown = cooldown;
				}
				if (instance.MiningCooldown <= 0f)
				{
					ThreadManager.InvokeOnMainThread(delegate
					{
						ServerManager.TryChangeBlock(instance.Position, instance.BlockType, BuiltinBlocks.Types.air, instance.Owner);
					});
					state.SetCooldown(2.5, 5.0);
					return;
				}
			}
			if (MinerSettings.BlockTypes.ContainsByReference(instance.BlockType, out int index))
			{
                UnityEngine.Vector3 rotate = instance.NPC.Position.Vector;
				switch (index)
				{
					case 1:
						rotate.x += 1f;
						break;
					case 2:
						rotate.x -= 1f;
						break;
					case 3:
						rotate.z += 1f;
						break;
					case 4:
						rotate.z -= 1f;
						break;
				}
				instance.NPC.LookAt(rotate);
			}
			AudioManager.SendAudio(instance.Position.Vector, "stoneDelete");
			GatherResults.Clear();
			List<ItemTypes.ItemTypeDrops> itemList = instance.BlockTypeBelow.OnRemoveItems;
			for (int i = 0; i < itemList.Count; i++)
			{
				GatherResults.Add(itemList[i]);
			}
			ModLoader.Callbacks.OnNPCGathered.Invoke(instance, instance.Position.Add(0, -1, 0), GatherResults);
			InventoryItem toShow = ItemTypes.ItemTypeDrops.GetWeightedRandom(GatherResults);
			float cd = Pipliz.Random.NextFloat(0.9f, 1.1f) * instance.MiningCooldown;
			if (toShow.Amount > 0)
			{
				state.SetIndicator(new IndicatorState(cd, toShow.Type));
			}
			else
			{
				state.SetCooldown(cd);
			}
			state.Inventory.Add(GatherResults);
			instance.GatheredItemCount++;
			if (instance.GatheredItemCount >= MinerSettings.MaxCraftsPerRun)
			{
				JobSettings.SetGoal(BlockJobInstance, new PutItemsInCrateGoal(BlockJobInstance, JobSettings, this, state.Inventory.Inventory, this), ref state);
				state.Inventory.Inventory.Clear();
			}
		}

        public void SetAsGoal()
        {
            
        }
    }
}
