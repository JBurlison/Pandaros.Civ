using AI;
using BlockEntities;
using BlockEntities.Helpers;
using BlockEntities.Implementations;
using ModLoaderInterfaces;
using Pandaros.API;
using Pandaros.API.Models;
using Pandaros.Civ.Jobs.Goals;
using Pipliz;
using Pipliz.Collections.Native;
using Pipliz.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BlockEntities.Implementations.BannerTracker;

namespace Pandaros.Civ.Storage
{

	[BlockEntityAutoLoader]
	public class CivCrateTracker : ILoadedWithDataByPositionType, IChangedWithType, IMultiBlockEntityMapping, IOnSavingColony, IOnLoadingColony, IOnCreatedColony
	{
		public static HashSet<ItemTypes.ItemType> Types { get; set; } = new HashSet<ItemTypes.ItemType>();

		public InstanceTracker<CivCrate> Positions { get; protected set; }
		public Dictionary<Colony, Dictionary<ushort, List<Vector3Int>>> ItemCrateLocations { get; set; } = new Dictionary<Colony, Dictionary<ushort, List<Vector3Int>>>();
		public IEnumerable<ItemTypes.ItemType> TypesToRegister { get { return Types; } }

		public CivCrateTracker()
		{
			Positions = new InstanceTracker<CivCrate>(7);
		}

		public void OnLoadedWithDataPosition(Chunk chunk, Vector3Int blockPosition, ushort type, ByteReader reader)
		{
			Colony colony = null;
			if (reader != null)
			{
				colony = ServerManager.ColonyTracker.Get(reader.ReadVariableInt());
			}

			CivCrate crate = new CivCrate(colony, reader);
			if (chunk.GetOrAllocateEntities().Add(blockPosition, crate))
			{
				Positions.TryAdd(blockPosition, crate);
			}
		}

		public void OnChangedWithType(Chunk chunk, BlockChangeRequestOrigin origin, Vector3Int blockPosition, ItemTypes.ItemType typeOld, ItemTypes.ItemType typeNew)
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

			if (Types.Contains(typeOld))
			{
				if (Positions.TryRemove(blockPosition, out CivCrate instance))
				{
					chunk.GetEntities()?.Remove(blockPosition);
				}

				if (!ItemCrateLocations.ContainsKey(colony))
					ItemCrateLocations.Add(colony, new Dictionary<ushort, List<Vector3Int>>());

				foreach (var item in ItemCrateLocations[colony])
					item.Value.Remove(blockPosition);
			}

			if (Types.Contains(typeNew) && StorageFactory.CrateTypes.TryGetValue(typeNew.Name, out var icrate))
			{
				CivCrate crate = new CivCrate(colony, blockPosition, icrate);
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
			public CrateInventory Inventory { get; }
			public Vector3Int Position { get; }

			public CivCrate(Colony colony, Vector3Int position, ICrate crate)
			{
				Colony = colony;
				Position = position;
				Inventory = new CrateInventory(crate, position, colony);
			}

			public CivCrate(Colony colony, ByteReader reader)
			{
				Colony = colony;
				Position = reader.ReadVector3Int();
				Inventory = new CrateInventory(reader);
			}

			public EKeepChunkLoadedResult OnKeepChunkLoaded(Vector3Int blockPosition)
			{
				return EKeepChunkLoadedResult.YesLong;
			}

			public ESerializeEntityResult SerializeToBytes(Chunk chunk, Vector3Byte blockPosition, ByteBuilder builder)
			{
				builder.WriteVariable(Colony != null ? Colony.ColonyID : 0);
				builder.Write(Position);
				Inventory.Serialize(builder);
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


		public void OnSavingColony(Colony colony, JSONNode data)
		{
			if (ItemCrateLocations.TryGetValue(colony, out var icl))
			{
				if (!data.HasChild(nameof(ItemCrateLocations)))
					data[nameof(ItemCrateLocations)] = new JSONNode();

				var itemsLocs = new JSONNode();

				foreach (var kvp in icl)
				{
					var locs = new JSONNode(NodeType.Array);

					foreach (var l in kvp.Value)
						locs.AddToArray((JSONNode)l);

					itemsLocs[kvp.Key.ToString()] = locs;
				}

				data[nameof(ItemCrateLocations)] = itemsLocs;
			}
		}

		public void OnLoadingColony(Colony colony, JSONNode data)
		{
			if (!ItemCrateLocations.ContainsKey(colony))
				ItemCrateLocations.Add(colony, new Dictionary<ushort, List<Vector3Int>>());

			if (data.TryGetAs<JSONNode>(nameof(ItemCrateLocations), out var icl))
			{
				foreach (var kvp in icl.LoopObject())
				{
					List<Vector3Int> locs = new List<Vector3Int>();

					foreach (var l in kvp.Value.LoopArray())
						locs.Add((Vector3Int)l);

					ItemCrateLocations[colony][Convert.ToUInt16(kvp.Key)] = locs;
				}
			}
		}

        public void OnCreatedColony(Colony colony)
        {
			if (!ItemCrateLocations.ContainsKey(colony))
				ItemCrateLocations.Add(colony, new Dictionary<ushort, List<Vector3Int>>());
		}
    }
}
