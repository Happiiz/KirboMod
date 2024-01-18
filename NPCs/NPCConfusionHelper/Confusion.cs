using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace KirboMod.NPCs.NPCConfusionHelper
{
    public static class Confusion
    {
        public static void InvertDirection(NPC npc)
        {
            if(!npc.confused)
            {
                return;
            }
            npc.direction *= -1;
            npc.spriteDirection *= -1;
        }
        public static float AwayFromPlayer(NPC npc)
        {
            return npc.Center.X - Main.player[npc.target].Center.X < 0 ? -1 : 1;
        }
    }
}
