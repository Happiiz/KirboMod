using KirboMod.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Globals
{
    public class KirbNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int finalCutteredCounter = 0;//
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {

        }
        public void StartFinalCutter(int duration)
        {
            finalCutteredCounter = duration;
            //do netcode stuff here later
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if(finalCutteredCounter > 0)
            {
                return false;
            }
            return true;
        }
        public override bool PreAI(NPC npc)
        {
            finalCutteredCounter--;
            if(finalCutteredCounter > 0)
            {
                //maybe do this in modplayer update instead
                //npc.position -= npc.velocity;
                return false;
            }
            return true;
        }
    }
}
