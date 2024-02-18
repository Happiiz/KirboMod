using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace KirboMod
{
    public static class Helper
    {
        public static bool SpawnInfoNotInAnySpecialBiome(NPCSpawnInfo info)
        {
            if (info.Player.ZoneJungle)
            {
                return false;
            }
            if (info.Player.ZoneSnow)
            {
                return false;
            }
            if (info.Player.ZoneBeach) //don't spawn on beach
            {
                return false;
            }
            if (info.Player.ZoneDesert) //don't spawn on beach
            {
                return false;
            }
            if (info.Player.ZoneCorrupt) //don't spawn on beach
            {
                return false;
            }
            if (info.Player.ZoneCrimson) //don't spawn on beach
            {
                return false;
            }
            if (info.Player.ZoneDungeon) //don't spawn in dungeon
            {
                return false;
            }
            if (info.Water) //don't spawn in water
            {
                return false;
            }
            return true;
        }
        public static void DustExplosion(Entity entity, int type, bool noGravity = false, float dustAmountMultiplier = 1f)
        {
            int count = (int)(entity.width * entity.height * .08f * dustAmountMultiplier);
            for (int i = 0; i < count; i++)
            {
                Dust.NewDustDirect(entity.position, entity.width, entity.height, type).noGravity = noGravity;
            }
        }
        public static Color Remap(float fromValue, float fromMin, float fromMax, Color toMin, Color toMax, bool clamped = true)
        {
            return Color.Lerp(toMin, toMax, Utils.GetLerpValue(fromMin, fromMax, fromValue, clamped));
        }
        public static float RemapEased(float fromValue, float fromMin, float fromMax, float toMin, float toMax, Func<float, float> easingFunction, bool clamp = true)
        {
            return MathHelper.Lerp(toMin, toMax, easingFunction(Utils.GetLerpValue(fromMin, fromMax, fromValue, clamp)));
        }
        public static void DustCircle(int dustAmount, float radius, Vector2 circleOrigin, int dustID = DustID.MagnetSphere)
        {
            for (float i = 0; i < 1; i+= 1f / dustAmount)
            {
                Dust.NewDustPerfect(circleOrigin + (i * MathF.Tau).ToRotationVector2() * radius, dustID, Vector2.Zero);
            }
        }
        public static bool CheckCircleCollision(Rectangle targetHitbox, Vector2 circleOrigin, float radius)
        {
            return circleOrigin.DistanceSQ(targetHitbox.ClosestPointInRect(circleOrigin)) < radius * radius;
        }
        /// <summary>
        /// THIS DOESN'T WORK WITH MINIONS BECAUSE OF WHIP TARGETING
        /// </summary>
        public static int FindHomingTarget(Projectile proj, float maxRange, bool includeNPCsImmuneToThis = true)
        {
            int target = -1;
            float maxRangeSQ = maxRange * maxRange;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC potentialTarget = Main.npc[i];
                if (!ValidHomingTarget(potentialTarget, proj, includeNPCsImmuneToThis) || potentialTarget.DistanceSQ(proj.Center) > maxRangeSQ || i == -1 || (potentialTarget.DistanceSQ(proj.Center) > Main.npc[target].DistanceSQ(proj.Center)))
                    continue;
                target = i;
            }
            return target;
        }
        public static bool ValidIndexedTarget(int targetIndex, Projectile proj, out NPC target, bool includeImmuneNPCs = true)
        {
            bool invalid = !Main.npc.IndexInRange(targetIndex) || !ValidHomingTarget(Main.npc[targetIndex], proj);
            target = invalid ? null : Main.npc[targetIndex];
            return !invalid;

        }
        public static bool ValidHomingTarget(NPC npc, Projectile proj, bool includeImmuneNPCs = true)
        {
            bool npcImmuneToProj = false;
            if (!includeImmuneNPCs)
            {
                if (proj.usesLocalNPCImmunity)
                {
                    npcImmuneToProj = proj.localNPCImmunity[npc.whoAmI] != 0;
                }
                else if (proj.usesIDStaticNPCImmunity)
                {
                    npcImmuneToProj = Projectile.perIDStaticNPCImmunity[proj.type][npc.type] != 0;
                }
                else
                {
                    npcImmuneToProj = npc.immune[proj.owner] != 0;
                }
            }//eol becomes invincible during dash, phasde transition and spawn animation, so ignore donttake damage if EoL to avoid some weirdness
            //      ai0 as 8 or 9 is if she's dashng, so that she doesn't get targeted during the phase transition or spawn animation
            return npc.CanBeChasedBy(null, npc.type == NPCID.HallowBoss && (npc.ai[0] == 8 || npc.ai[0] == 9)) && (includeImmuneNPCs || !npcImmuneToProj);
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
        public static Vector2 GetPredictiveAimVelocity(Vector2 shotOrigin, float shotVelLength, Vector2 aimedTargetPos, Vector2 aimedTargetVel)
        {
            return GetPredictiveAimRotation(shotOrigin, shotVelLength, aimedTargetPos, aimedTargetVel).ToRotationVector2() * shotVelLength;
        }
        public static float GetPredictiveAimRotation(Vector2 shotOrigin, float shotVelLength, Vector2 aimedTargetPos, Vector2 aimedTargetVel)
        {
            float angleToTarget = (aimedTargetPos - shotOrigin).ToRotation();
            float targetTraj = aimedTargetVel.ToRotation();
            float aimedTargetVelLength = aimedTargetVel.Length();
            float z = MathF.PI + targetTraj - angleToTarget;
            return angleToTarget - MathF.Asin(aimedTargetVelLength * MathF.Sin(z) / shotVelLength);
        }
    }
}
