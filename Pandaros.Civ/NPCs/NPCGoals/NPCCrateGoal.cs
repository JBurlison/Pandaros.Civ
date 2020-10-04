using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI;
using BlockTypes;
using Jobs;
using NPC;
using Pandaros.Civ.Storage;
using Pipliz.Collections.Native;
using UnityEngine.Assertions;
using static AI.PathingManager;
using Vector3Int = Pipliz.Vector3Int;

namespace Pandaros.Civ.NPCs.NPCGoals
{
	public class NPCCrateGoal : GoalBase, IPathingThreadAction
	{
		public static readonly StockpileFilter StockpileFilterInstance = new StockpileFilter();

		public Vector3Int? CrateLocation;
		public Vector3Int? crateLocationStanding;

		public NPCBase.NPCPath pathToCrate;

		public Colony pathingThreadContext;
		public Vector3Int pathingThreadPathStart;
		public bool pathingThreadIsQueued;
		public EPathResult pathingThreadResult;

		public override void Setup(NPCBase npc)
		{
			Assert.IsFalse(pathingThreadIsQueued);
		}

		public override void Clear()
		{
			if (pathingThreadIsQueued)
			{
				// ensure we can cancel/stop/await the thread action so we can safely clear the path
				pathingThreadIsQueued = false;
				System.Threading.Thread.MemoryBarrier();
				int sleeps = 0;
				while (pathingThreadResult == EPathResult.InProgress && sleeps++ < 500)
				{
					ThreadManager.Sleep(10);
				}
				if (sleeps >= 500)
				{
					Pipliz.Log.WriteError($"Thread action took more than 500 ms to complete while clearing this goal?");
				}
			}

			pathToCrate.Clear();
		}

		public override void OnNPCUpdate(NPCBase npc)
		{
			Assert.IsFalse(pathingThreadIsQueued);
			if (pathingThreadResult != EPathResult.None)
			{
				var resultRead = pathingThreadResult;
				pathingThreadResult = EPathResult.None;
				switch (resultRead)
				{
					case EPathResult.AdjustPosition:
						npc.SetPosition(pathingThreadPathStart);
						npc.state.SetCooldown(npc.oneOverSpeed);
						return;
					case EPathResult.NotFoundPath:
						OnFailGoal(npc);
						return;
					case EPathResult.NotFoundViaNavMesh:
						{
							// try to find it via the direct method instead
							if (StorageFactory.CrateTracker.TryGetClosestInColony(npc.Colony, npc.Position, out var directGoal))
							{
								CrateLocation = directGoal;
								if (PathingManager.TryGetClosestPositionWorldNotAt(directGoal, npc.Position, out bool result, out var directGoalStanding) && result)
								{
									crateLocationStanding = directGoalStanding;
									QueuePathThreadActionWithGoal(npc);
								}
								else
								{
									OnFailGoal(npc, false);
								}
							}
							else
							{
								if (ServerManager.BlockEntityTracker.BannerTracker.TryGetClosest(npc.Position, out var banner))
								{
									CrateLocation = banner.Position;
									crateLocationStanding = banner.Position;
									QueuePathThreadActionWithGoal(npc);
								}
								else
								{
									OnFailGoal(npc);
								}
							}
							return;
						}
					case EPathResult.WaitUnloaded:
						npc.state.SetCooldown(3.0, 6.0);
						return;
					case EPathResult.Success:
						Assert.IsTrue(pathToCrate.Exists);
						break; // yey; path should be applied
				}
			}

			if (!CrateLocation.HasValue || !crateLocationStanding.HasValue)
			{
				QueuePathThreadActionSearchMesh(npc);
				return;
			}

			if (npc.Position == crateLocationStanding.Value)
			{
				OnReachedGoal(npc);
			}
			else if (!pathToCrate.Exists || !pathToCrate.HasGoal(crateLocationStanding.Value))
			{
				OnResetPath(npc);
			}
			else
			{
				OnTryMovePath(npc);
			}
		}

		void OnTryMovePath(NPCBase npc)
		{
			Assert.IsTrue(!pathToCrate.IsDone); // if we have a path, the end is our goal, but we're not there; surely we have movement to do
			switch (pathToCrate.TryMoveOne(out Vector3Int moved))
			{
				case NPCBase.NPCPath.EMoveResult.NotLoaded:
				case NPCBase.NPCPath.EMoveResult.PathInvalid:
					OnResetPath(npc);
					break;
				case NPCBase.NPCPath.EMoveResult.Success:
				case NPCBase.NPCPath.EMoveResult.AtEnd:
					Vector3Int pos = npc.Position;
					npc.SetPosition(moved);
					npc.SetDirection((npc.Position - pos).Vector);
					npc.state.SetCooldown(npc.oneOverSpeed);
					break;
			}
		}

		void OnFailGoal(NPCBase npc, bool resetGoalPrimary = true)
		{
			npc.SetIndicatorState(new Shared.IndicatorState(Pipliz.Random.NextFloat(12f, 18f), BuiltinBlocks.Indices.crate, true, false));
			crateLocationStanding = default;
			if (resetGoalPrimary)
			{
				CrateLocation = default;
			}
		}

		void OnResetPath(NPCBase npc)
		{
			pathToCrate.Clear();
			crateLocationStanding = default;
			CrateLocation = default;
			npc.state.SetCooldown(0.1, 0.4);
		}

		void OnReachedGoal(NPCBase npc)
		{
			pathToCrate.Clear();
			npc.Job.OnNPCAtStockpile(ref npc.state);
			npc.SendIndicatorIfSet();
		}

		void QueuePathThreadActionWithGoal(NPCBase npc)
		{
			// so we failed to find a crate via the navmesh-landmark way, but found one in the straight line way
			// path to that instead
			pathingThreadIsQueued = true;
			pathingThreadPathStart = npc.Position;
			pathingThreadContext = npc.Colony;
			ServerManager.PathingManager.QueueAction(this);
			npc.state.SetCooldown(0.5, 1.5);
		}

		void QueuePathThreadActionSearchMesh(NPCBase npc)
		{
			CrateLocation = default; // clear them so the thread isn't confused and thinks we provided a location
			crateLocationStanding = default;
			QueuePathThreadActionWithGoal(npc);
		}

		public override ENPCGoalState GetState(NPCBase npc)
		{
			if (pathingThreadIsQueued)
			{
				return ENPCGoalState.WaitingOnPathing;
			}
			else
			{
				return ENPCGoalState.Okay;
			}
		}

		public virtual void PathingThreadAction(PathingManager.PathingContext context)
		{
			if (!pathingThreadIsQueued)
			{
				pathingThreadResult = EPathResult.Cancelled;
				return;
			}

			pathingThreadResult = EPathResult.InProgress;
			Assert.IsTrue(!pathToCrate.Exists);
			Assert.IsTrue(pathingThreadIsQueued);

			if (!CrateLocation.HasValue)
			{
				// find a location via the landmark system
				switch (GetLandmarkResult(ref context, out Vector3Int foundLandmark, out Vector3Int foundStandPosition))
				{
					case PathFinderLandmarks.EPathFindingResult.Fail:
					case PathFinderLandmarks.EPathFindingResult.None:
					default:
						pathingThreadResult = EPathResult.NotFoundViaNavMesh;
						pathingThreadIsQueued = false;
						return;
					case PathFinderLandmarks.EPathFindingResult.OutOfRange:
					case PathFinderLandmarks.EPathFindingResult.NotLoaded:
						pathingThreadResult = EPathResult.WaitUnloaded;
						pathingThreadIsQueued = false;
						return;
					case PathFinderLandmarks.EPathFindingResult.Success:
						CrateLocation = foundLandmark;
						crateLocationStanding = foundStandPosition;
						break;
				}
			}

			Assert.IsTrue(crateLocationStanding.HasValue);

			var result = context.Pathing.TryFindPath(ref context, pathingThreadPathStart, crateLocationStanding.Value, out Path path, 2 * 1000 * 1000 * 1000);
			switch (result)
			{
				case PathFinder.EPathFindingResult.NotFoundGoal:
					// not loaded / invalid goal (?) wait a bit and retry
					Path.Return(path);
					pathingThreadResult = EPathResult.WaitUnloaded;
					break;
				case PathFinder.EPathFindingResult.Fail:
				case PathFinder.EPathFindingResult.OutOfRange:
					Path.Return(path);
					pathingThreadResult = EPathResult.NotFoundPath;
					break;
				// no path, wait a longer bit and retry, indicate fail
				case PathFinder.EPathFindingResult.NotFoundStart:
					Path.Return(path);
					HandleStartNotFound(ref context, ref pathingThreadPathStart, ref pathingThreadResult);
					break;
				// not loaded / invalid start (?) check which
				case PathFinder.EPathFindingResult.Success:
					pathingThreadResult = EPathResult.Success;
					pathToCrate.SetPath(path);
					break;
				default:
					Assert.IsTrue(false, "Unexpected EPathFindingResult");
					pathingThreadResult = EPathResult.None;
					break;
			}

			pathingThreadIsQueued = false;
		}


		private PathFinderLandmarks.EPathFindingResult GetLandmarkResult(ref PathingManager.PathingContext context, out Vector3Int foundLandmark, out Vector3Int foundStandPosition)
		{
			return context.PathingLandmarks.TryFindClosestLandmark(
								ref context,
								pathingThreadPathStart,
								LandmarkManager.CrateType,
								StockpileFilterInstance,
								pathingThreadContext,
								out foundLandmark,
								out foundStandPosition);
		}

		public class StockpileFilter : ILandmarkFilter
		{
			public void CheckLandmarks(Vector3Int chunkPosition, object context, ref UnsafeNativeSortedList<ushort, PlausibleLandmark, LandmarkSorter, UnsafePersistentAllocator> plausibleLandmarks)
			{
				if (!(context is Colony colony))
				{
					plausibleLandmarks.Clear();
					return;
				}

				ServerManager.BlockEntityTracker.CrateTracker.FilterLandmarks(chunkPosition, colony, ref plausibleLandmarks);
			}
		}
	}
}
