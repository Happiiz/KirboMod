using KirboMod.NPCs;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.ZeroOrbs
{
    public class ZeroOrbWindmill : ModProjectile
    {
        //managing everything in this 1 projectile to make sure Z Layering works properly
        public override string Texture => "KirboMod/Projectiles/ZeroOrbs/ZeroBGBallBlue";
        public static int OrbCountPerArm => Main.expertMode ? 20 : 10;
        public static int OrbHitboxSideLength => 50;
        public static float OrbSpacing => Main.expertMode ? 90 : 80;
        public static int WindmillSpawnRate => 5;
        public static int WindmillSpawnCount => 9;
        public static int ArmAmount => Main.expertMode ? 5 : 4;
        public float GetSpiralingSpeed()
        {
            return StartingZPos / TimeToReach;
        }
        public static float SpiralingSpeed => .03f;
        public ref float Timer => ref Projectile.localAI[0];
        public ref float TimeToReach => ref Projectile.ai[0];
        public ref float StartingZPos => ref Projectile.ai[1];
        public ref float RotationOffset => ref Projectile.ai[2];
        public ref float DrawPassCounter => ref Projectile.localAI[1];
        public bool CurrentDrawPassIsBehindNPCsAndTiles => DrawPassCounter % 2 == 0;
        public bool CurrentDrawPassIsOverPlayers => DrawPassCounter % 2 == 1;
        public static float FadeInDuration => 10;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 50;
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = OrbHitboxSideLength;
            Projectile.height = OrbHitboxSideLength;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }
        public override void AI()
        {
            FrameCounting();
            Timer++;
            float lastWaveTimer = Timer - WindmillSpawnCount * WindmillSpawnRate;
            float lastWaveZPos = GetZPos(lastWaveTimer, TimeToReach, StartingZPos);
            float scale = Draw3D.GetScaleFor3D(lastWaveZPos);
            if (scale < float.Epsilon)
            {
                Projectile.Kill();
            }
        }
        /// <summary>
        /// leave rotation offset as -1 for randomized.
        /// </summary>
        public static void SpawnOrbWindmillAt(IEntitySource source, Vector2 pos, float timeToReach, float startingZPos, int damage, float rotationOffset = -1)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            if (rotationOffset == -1)
            {
                rotationOffset = MathF.Tau * Main.rand.NextFloat();
            }
            Projectile.NewProjectile(source, pos, Vector2.Zero, ModContent.ProjectileType<ZeroOrbWindmill>(), damage, 1, -1, timeToReach, startingZPos, rotationOffset);
        }
        private void FrameCounting()
        {
            int frameSpeed = 3;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }
        }
        public float GetZPos()
        {
            return GetZPos(Timer, TimeToReach, StartingZPos);
        }
        public static float GetZPos(float timer, float timeToReach, float startingZPos)
        {
            return Utils.Remap(timer, 0, timeToReach, startingZPos, 0, false);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //this is not a mistake!!!
            behindNPCsAndTiles.Add(index);
            overPlayers.Add(index);
        }
        public static Vector2[] GetOrbPositions(Vector2 center, float timer, float startingZPos, float timeToReach, float rotation)
        {
            int totalOrbCount = ArmAmount * (OrbCountPerArm - 1) + 1;
            Vector2[] result = new Vector2[totalOrbCount];
            int index = 0;
            result[0] = center;
            for (int i = 0; i < ArmAmount; i++)
            {
                for (int j = 1; j < OrbCountPerArm; j++)
                {
                    float angle = Utils.Remap(i, 0, ArmAmount, 0, MathF.Tau, false) + timer * SpiralingSpeed + rotation;
                    Vector2 offset = angle.ToRotationVector2() * j * OrbSpacing;
                    index++;
                    result[index] = offset + center;
                }
            }
            return result;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer >= TimeToReach  && Timer - WindmillSpawnCount * WindmillSpawnRate < TimeToReach && (Timer - TimeToReach) % WindmillSpawnRate == 0)
            {
                Rectangle[] hitboxes = GetHitboxes(GetOrbPositions(Projectile.Center, TimeToReach, StartingZPos, TimeToReach, Projectile.rotation));
                for (int i = 0; i < hitboxes.Length; i++)
                {
                    if (targetHitbox.Intersects(hitboxes[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void PreviewHitboxes()
        {
            if (Timer >= TimeToReach && Timer - WindmillSpawnCount * WindmillSpawnRate < TimeToReach && (Timer - TimeToReach) % WindmillSpawnRate == 0)
            {
                Rectangle[] hitboxes = GetHitboxes(GetOrbPositions(Projectile.Center, TimeToReach, StartingZPos, TimeToReach, Projectile.rotation));
                for (int i = 0; i < hitboxes.Length; i++)
                {
                    Helper.RectVisualizer(hitboxes[i]);
                }
            }
        }
        public static Rectangle[] GetHitboxes(Vector2[] hitboxCenters)
        {
            Rectangle[] result = new Rectangle[hitboxCenters.Length];
            int width = OrbHitboxSideLength;
            int height = OrbHitboxSideLength;
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            for (int i = 0; i < result.Length; i++)
            {
                Vector2 center = hitboxCenters[i];
                result[i] = new Rectangle((int)center.X - halfWidth, (int)center.Y - halfHeight, width, height);
            }
            return result;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            //just fade them in
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() / 2;
            Color color = Projectile.GetAlpha(lightColor);
            Vector2 center = Projectile.Center;
            Vector2 screenCenter = new(Main.screenWidth / 2, Main.screenHeight / 2);
            for (int i = WindmillSpawnCount - 1; i >= 0; i--)
            {
                float timer = Timer - i * WindmillSpawnRate;
                if (timer < 0)
                {
                    continue;
                }
                Vector2[] orbPositions = GetOrbPositions(center, timer, StartingZPos, TimeToReach, Projectile.rotation);
                float zPos = GetZPos(timer, TimeToReach, StartingZPos);
                float scale3D = Draw3D.GetScaleFor3D(zPos);
                if ((scale3D < 1 && CurrentDrawPassIsBehindNPCsAndTiles) || (scale3D >= 1 && CurrentDrawPassIsOverPlayers))
                {
                    for (int j = 0; j < orbPositions.Length; j++)
                    {
                        Vector2 drawPos = Vector2.Lerp(screenCenter, orbPositions[j] - Main.screenPosition, scale3D);
                        //so it starts with some opacity already, instead of the first frame being 0% opacity
                        float opacity = Utils.GetLerpValue(0, FadeInDuration, timer + 1, true);
                        // mult by 2 because sprite is not upscaled
                        Main.EntitySpriteDraw(texture, drawPos, frame, color * opacity, Projectile.rotation, origin, Projectile.scale * scale3D * 2f, SpriteEffects.None);
                    }
                }
            }
            DrawPassCounter++;
            //PreviewHitboxes();
            return false;
        }
    }
}
