using Jobs;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class CraftingLitGoal : CraftingGoal
    {
        public CraftingLitGoal(IJob job, IPandaJobSettings jobSettings, CraftingJobSettings settings) : base(job, jobSettings, settings)
        {
        }

        public override Vector3Int GetPosition()
        {
            var position = base.GetPosition();
            int index;
            if (CraftingJobSettings.BlockTypes.ContainsByReference<ItemTypes.ItemType>(CraftingJobInstance.BlockType, out index))
            {
                switch (index)
                {
                    case 1:
                    case 6:
                        ++position.x;
                        break;
                    case 2:
                    case 7:
                        --position.x;
                        break;
                    case 3:
                    case 8:
                        ++position.z;
                        break;
                    case 4:
                    case 9:
                        --position.z;
                        break;
                }
            }
            return position;
        }

        public override void OnStartCrafting()
        {
            int index;
            if (!CraftingJobSettings.BlockTypes.ContainsByReference<ItemTypes.ItemType>(CraftingJobInstance.BlockType, out index))
                return;
            switch (index)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    ItemTypes.ItemType blockType = CraftingJobSettings.BlockTypes[index + 5];
                    ESetBlockFlags flags = ESetBlockFlags.None;
                    if (ServerManager.TryChangeBlock(CraftingJobInstance.Position, CraftingJobInstance.BlockType, blockType, (BlockChangeRequestOrigin)CraftingJobInstance.Owner, flags) == EServerChangeBlockResult.Success)
                    {
                        CraftingJobInstance.BlockType = blockType;
                        break;
                    }
                    break;
            }
        }

        public override void OnStopCrafting()
        {
            int index;
            if (!CraftingJobSettings.BlockTypes.ContainsByReference<ItemTypes.ItemType>(CraftingJobInstance.BlockType, out index))
                return;
            switch (index)
            {
                case 6:
                case 7:
                case 8:
                case 9:
                    ItemTypes.ItemType blockType = CraftingJobSettings.BlockTypes[index - 5];
                    ESetBlockFlags flags = ESetBlockFlags.None;
                    if (ServerManager.TryChangeBlock(CraftingJobInstance.Position, CraftingJobInstance.BlockType, blockType, (BlockChangeRequestOrigin)CraftingJobInstance.Owner, flags) == EServerChangeBlockResult.Success)
                    {
                        CraftingJobInstance.BlockType = blockType;
                        break;
                    }
                    break;
            }
        }
    }
}
