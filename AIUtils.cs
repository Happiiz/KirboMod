using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace KirboMod
{
    public static class AIUtils
    {
        public static bool ValidHomingTarget(NPC npc, Projectile proj, bool includeImmuneNPCs = true)
        {
            bool npcImmuneToProj = false;
            if (!includeImmuneNPCs)
            {
                if (proj.usesLocalNPCImmunity)
                {
                    npcImmuneToProj = proj.localNPCImmunity[npc.whoAmI] > 0;
                }
                else if (proj.usesIDStaticNPCImmunity)
                {
                    npcImmuneToProj = Projectile.perIDStaticNPCImmunity[proj.type][npc.type] > 0;
                }
                else
                {
                    npcImmuneToProj = npc.immune[proj.owner] > 0;
                }
            }//eol becomes invincible during dash, phasde transition and spawn animation, so ignore donttake damage if EoL to avoid some weirdness
            //      ai0 as 8 or 9 is if she's dashng, so that she doesn't get targeted during the phase transition or spawn animation
            return npc.CanBeChasedBy(null, npc.type == NPCID.HallowBoss && (npc.ai[0] == 8 || npc.ai[0] == 9)) && !npcImmuneToProj;
        }
        /// <summary>
        /// used by the nightglow. very aggressive
        /// </summary>
        public static void SmoothStepHoming(ref Projectile Projectile, float homingRange = 700, float maxVel = 10, float homingStrengthIncrease = 0.1f, float maxHomingStrength = 1, bool includeImmuneNPCs = true)
        {
            float amountToCurve = 0;
            float maxRangeSQ = homingRange * homingRange;
            NPC target = null;//this searches for a target every frame, not very efficient, as it would be ideal to store a found target, but this is a method for generalizing things
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (ValidHomingTarget(Main.npc[i], Projectile, includeImmuneNPCs) && Main.npc[i].DistanceSQ(Projectile.Center) <= maxRangeSQ && (target == null || (Main.npc[i].DistanceSQ(Projectile.Center) < target.DistanceSQ(Projectile.Center))))
                {
                    target = Main.npc[i];
                }
            }

            if (target != null)
            {
                amountToCurve = MathHelper.Clamp(amountToCurve + homingStrengthIncrease, float.MinValue, maxHomingStrength);
                Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * maxVel, amountToCurve);
            }
        }
    }
}
