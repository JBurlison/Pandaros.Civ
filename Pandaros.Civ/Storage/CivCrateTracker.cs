using AI;
using BlockEntities;
using BlockEntities.Helpers;
using BlockEntities.Implementations;
using Pipliz;
using Pipliz.Collections.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlockEntities.Implementations.BannerTracker;

namespace Pandaros.Civ.Storage
{

	[BlockEntityAutoLoader]
	public class CivCrateTracker : ILoadedWithDataByPositionType, IChangedWithType, IMultiBlockEntityMapping
	{
		public ItemTypes.ItemType[] Types { get; }
		public InstanceTracker<CivCrate> Positions { get; protected set; }

		public IEnumerable<ItemTypes.ItemType> TypesToRegister { get { return Types; } }

		public CivCrateTracker()
		{
			Positions = new InstanceTracker<CivCrate>(7);

			Types = ItemTypes._TypeByUShort.Where(pair => StorageFactory.CrateTypes.ContainsKey(pair.Value.Name) || pair.Value.Name == StockpileBlock.Name).Select(pair => pair.Value).ToArray();
		}

		public void OnLoadedWithDataPosition(Chunk chunk, Vector3Int blockPosition, ushort type, ByteReader reader)
		{
			Colony colony = null;
			if (reader != null)
			{
				colony = ServerManager.ColonyTracker.Get(reader.ReadVariableInt());
			}
			CivCrate crate = new CivCrate(colony);
			if (chunk.GetOrAllocateEntities().Add(blockPosition, crate))
			{
				Positions.TryAdd(blockPosition, crate);
			}
		}

		public void OnChangedWithType(Chunk chunk, BlockChangeRequestOrigin origin, Vector3Int blockPosition, ItemTypes.ItemType typeOld, ItemTypes.ItemType typeNew)
		{
			if (Types.ContainsByReference(typeOld, out int idxOld))
			{
				if (Positions.TryRemove(blockPosition, out CivCrate instance))
				{
					chunk.GetEntities()?.Remove(blockPosition);
				}
			}

			if (Types.ContainsByReference(typeNew, out int idxNew))
			{
				Colony colony = null;
				switch (origin.Type)
				{
					case BlockChangeRequestOrigin.EType.Colony:
						colony = origin.AsColony;
						break;
					case BlockChangeRequestOrigin.EType.Player:
						Players.Player play = origin.AsPlayer;
						colony = play.ActiveColony;
						break;
				}

				CivCrate crate = new CivCrate(colony);
				if (chunk.GetOrAllocateEntities().Add(blockPosition, crate))
				{
					Positions.TryAdd(blockPosition, crate);
				}
			}
		}

		public bool TryGetClosestInColony(Colony colony, Vector3Int desiredPosition, out Vector3Int found, int maxBoxRadius = 200)
		{
			var tuple = (colony.Banners, ServerManager.ServerSettings.Colony.ExclusiveRadius, colony);
			return Positions.TryGetClosestWhere(desiredPosition, IsCloseAndHasColony, ref tuple, out found, out CivCrate crate, maxBoxRadius);
		}

		static bool IsCloseAndHasColony(Vector3Int position, CivCrate crate, ref (Banner[] banners, int range, Colony colony) data)
		{
			return (crate.Colony == null || data.colony == crate.Colony)
				&& data.banners.HasWithinRange(position, data.range);
		}

		public class CivCrate : IBlockEntityKeepLoaded, IBlockEntitySerializable
		{
			public Colony Colony { get; }

			public CivCrate(Colony colony)
			{
				Colony = colony;
			}

			public EKeepChunkLoadedResult OnKeepChunkLoaded(Vector3Int blockPosition)
			{
				return EKeepChunkLoadedResult.YesLong;
			}

			public ESerializeEntityResult SerializeToBytes(Chunk chunk, Vector3Byte blockPosition, ByteBuilder builder)
			{
				builder.WriteVariable(Colony != null ? Colony.ColonyID : 0);
				return ESerializeEntityResult.LoadChunkOnStartup | ESerializeEntityResult.WroteData;
			}
		}

		public void FilterLandmarks(
			Vector3Int chunk,
			Colony colony,
			ref UnsafeNativeSortedList<ushort, PlausibleLandmark, LandmarkSorter, UnsafePersistentAllocator> plausibleLandmarks)
		{
			Positions.FilterLandmarks(chunk, colony, ref plausibleLandmarks, FilterLandmarkToColony);
		}

		public static bool FilterLandmarkToColony(Colony colony, CivCrate crate)
		{
			return crate != null && crate.Colony == colony;
		}
	}
}
