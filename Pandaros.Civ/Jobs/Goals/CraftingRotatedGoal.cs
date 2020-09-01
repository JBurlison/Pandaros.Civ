using Jobs;
using Pipliz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs.Goals
{
    public class CraftingRotatedGoal : CraftingGoal
    {
        public CraftingRotatedGoal(IJob job, IPandaJobSettings jobSettings, CraftingJobSettings settings) : base(job, jobSettings, settings)
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
                        ++position.x;
                        break;
                    case 2:
                        --position.x;
                        break;
                    case 3:
                        ++position.z;
                        break;
                    case 4:
                        --position.z;
                        break;
                }
            }
            return position;
        }
    }
}
