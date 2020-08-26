using NPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandaros.Civ.Jobs
{
    public abstract class PandaJob : IPandaJob
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

        public virtual void SetGoal(INpcGoal npcGoal)
        {
            var oldGoal = CurrentGoal;
            CurrentGoal = npcGoal;

            if (GoalChanged != null)
                GoalChanged.Invoke(this, (oldGoal, npcGoal));
        }

        public virtual void SetNPC(NPCBase npc)
        {
            var oldNpc = NPC;
            NPC = npc;

            if (NPCSet != null)
                NPCSet.Invoke(this, (oldNpc, npc));
        }
    }
}
