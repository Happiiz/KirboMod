using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Systems
{
    public class KirbyTransformationModCompatibilityHelper : ModSystem
    {
        static int DroppedStarProjID = -1;
        static Mod transfMod;

        //values given by the transformations mod dev
        const float SlowerDropRotation = MathF.Tau / 85;
        const float FasterDropRotation = MathF.Tau / 80;
        const float DroppedStarAI1 = 0.8f / 20;
        public override void SetStaticDefaults()
        {
            DroppedStarProjID = -1;
            //this is the kirby transformation mod's internal name
            ModLoader.TryGetMod("SmallPinkCreature", out transfMod);
            CacheDroppedStarProjIDIfNeeded();
        }

        private static void CacheDroppedStarProjIDIfNeeded()
        {
            //already got proj id
            if (DroppedStarProjID > 0)
            {
                return;
            }
            //transformations mod not loaded
            if (transfMod == null)
            {
                return;
            }
            transfMod.TryFind("DroppedStar", out ModProjectile sampleProj);//.TryFind("DroppedStar", out Projectile sampleProj);  
            if (sampleProj != null)
            {
                DroppedStarProjID = sampleProj.Type;
            }
        }

        public static void BlacklistProjIDFromSpawningDroppedStar(int projID)
        {
            if (transfMod != null)
            {
                transfMod.Call("ProjectileCantSpawnDroppedStar", projID);
            }
        }
        public static void BlacklistNPCIDFromSpawningDroppedStar(int npcID)
        {
            if (transfMod != null)
            {
                transfMod.Call("NPCCantSpawnDroppedStar", npcID);
            }
        }
        public static void SpawnSingleDropStar(IEntitySource source, Vector2 pos, float damage)
        {

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            CacheDroppedStarProjIDIfNeeded();
            if(DroppedStarProjID  <= 0)
            {
                return;
            }
            Vector2 offset = new(12, 12);
            pos += offset;
            Projectile.NewProjectile(source, pos, Vector2.Zero, DroppedStarProjID, 1, 0f, -1, FasterDropRotation, DroppedStarAI1, damage);
        }
        public static void SpawnDropStarLine(IEntitySource source, Vector2 start, Vector2 end, int starCount, float damagePerDrop)
        {
            for (int i = 0; i < starCount; i++)
            {
                Vector2 pos = Vector2.Lerp(start, end, Utils.GetLerpValue(0, starCount - 1, i));
                SpawnSingleDropStar(source, pos, damagePerDrop);
            }
        }
        public static void Spawn5Drop(IEntitySource source, Vector2 pos, int combinedDamage)
        {
            if(Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            float ai2 = combinedDamage / 5f;
            SpawnSingleDropStar(source, pos, ai2);
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Utils.Remap(i, 0, 4, 0, MathF.Tau).ToRotationVector2() * 20;
                SpawnSingleDropStar(source, pos + offset, ai2);
            }
        }
    }
}
