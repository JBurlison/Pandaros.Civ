using NPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public class PandaJob : IPandaJob
    {
        public INpcGoal CurrentGoal { get; set; }

        public Colony Owner { get; set; }
        public string LocalizationKey { get; set; }
        public NPCBase NPC { get; set; }
        public NPCType NPCType { get; set; }

        public event EventHandler<(NPCBase, NPCBase)> NPCSet;
        public event EventHandler<(INpcGoal, INpcGoal)> GoalChanged;

        public virtual void OnUpdate()
        {
           
        }

        public void SetGoal(INpcGoal npcGoal)
        {
            
            if (NPCSet != null)
            {

            }
        }

        public void SetNPC(NPCBase npc)
        {
            
        }
    }
}
