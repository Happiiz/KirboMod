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
using Terraria.Utilities;

namespace KirboMod.Projectiles.ZeroOrbs
{
    public class ZeroBGOrbRings : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/ZeroOrbs/ZeroBGBallGreen";
        public static int OrbHitboxSideLength => 50;
        public static int RingSpawnRate => 1;
        public static int RingSpawnCount => 40;
        public static int OrbsPerRing => 9;
        public static int OrbRingRadius => 140;
        public float GetSpiralingSpeed()
        {
            return StartingZPos / TimeToReach;
        }
        public static float RotationSpeed => .04f;
        public ref float Timer => ref Projectile.localAI[0];
        public ref float TimeToReach => ref Projectile.ai[0];
        public ref float StartingZPos => ref Projectile.ai[1];
        public int RandomSeed => (int)Projectile.ai[2];

        public ref float DrawPassCounter => ref Projectile.localAI[1];
        public bool CurrentDrawPassIsBehindNPCsAndTiles => DrawPassCounter % 2 == 0;
        public bool CurrentDrawPassIsOverPlayers => DrawPassCounter % 2 == 1;
        public static float FadeInDuration => 10;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 80;
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
            if(RandomSeed == -1)
            {
                Projectile.ai[2] = Main.rand.Next(int.MaxValue);
            }
            FrameCounting();
            Timer++;
            float lastWaveTimer = Timer - RingSpawnCount * RingSpawnRate;
            float lastWaveZPos = GetZPos(lastWaveTimer, TimeToReach, StartingZPos);
            float scale = Draw3D.GetScaleFor3D(lastWaveZPos);
            if (scale < float.Epsilon)
            {
                Projectile.Kill();
            }
        }
        public static void SpawnOrbRingsAt(IEntitySource source, Vector2 pos, float timeToReach, float startingZPos, int damage)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Projectile.NewProjectile(source, pos, Vector2.Zero, ModContent.ProjectileType<ZeroBGOrbRings>(), damage, 1, -1, timeToReach, startingZPos, Main.rand.Next(int.MaxValue));
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
        public static Vector2[] GetOrbPositions(Vector2 center, float timer, int randomSeed, int i)
        {
            int totalOrbCount = RingSpawnCount * OrbsPerRing;
            Vector2[] result = new Vector2[totalOrbCount];
            int index = -1;
            UnifiedRandom rnd = new(randomSeed);
            for (int j = 0; j < i; j++)
            {
                rnd.Next();//advance the RNG
            }
            rnd = new(rnd.Next(int.MaxValue));
            
            float rotationSpeed = rnd.Next(2) * 2 - 1;
            rotationSpeed *= RotationSpeed;
            float rectHalfWidth = 1300;
            float rectHalfHeight = 1300;
            Vector2 curRingOffset;
            //yes, should be i and not j
            if (i == 0)
            {
                curRingOffset = rnd.NextFloat(MathF.Tau).ToRotationVector2() * OrbRingRadius;
            }
            else
            {
                curRingOffset = new(rnd.NextFloat(-rectHalfWidth, rectHalfWidth), rnd.NextFloat(-rectHalfHeight, rectHalfHeight));
            }
            for (int j = 0; j < OrbsPerRing; j++)
            {
    
                float angle = Utils.Remap(j, 0, OrbsPerRing, 0, MathF.Tau, false) + timer * rotationSpeed;
                Vector2 offset = angle.ToRotationVector2() * OrbRingRadius;
                index++;
                result[index] = offset + center + curRingOffset;
            }

            return result;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer >= TimeToReach && Timer - RingSpawnCount * RingSpawnRate < TimeToReach && (Timer - TimeToReach) % RingSpawnRate == 0)
            {
                int curIndex = (int)(Timer - TimeToReach) / RingSpawnRate;
                for (int j = 0; j < RingSpawnCount; j++)
                {
                    Rectangle[] hitboxes = GetHitboxes(GetOrbPositions(Projectile.Center, Timer - curIndex, RandomSeed, curIndex));
                    for (int i = 0; i < hitboxes.Length; i++)
                    {
                        if (targetHitbox.Intersects(hitboxes[i]))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        void PreviewHitboxes()
        {
            int timer = (int)Timer;
            if (timer >= TimeToReach && timer - RingSpawnCount * RingSpawnRate < TimeToReach && (timer - TimeToReach) % RingSpawnRate == 0)
            {
                int curIndex = (int)(timer - TimeToReach) / RingSpawnRate;
                for (int j = 0; j < RingSpawnCount; j++)
                {
                    Rectangle[] hitboxes = GetHitboxes(GetOrbPositions(Projectile.Center, Timer - curIndex, RandomSeed, curIndex));
                    for (int i = 0; i < hitboxes.Length; i++)
                    {
                        Helper.RectVisualizer(hitboxes[i]);
                    }
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
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            //just fade them in
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() / 2;
            Color color = Projectile.GetAlpha(lightColor);
            Vector2 center = Projectile.Center;
            Vector2 screenCenter = new(Main.screenWidth / 2, Main.screenHeight / 2);
            for (int i = RingSpawnCount - 1; i >= 0; i--)
            {
                float timer = Timer - i * RingSpawnRate;
                if (timer < 0)
                {
                    continue;
                }

                float zPos = GetZPos(timer, TimeToReach, StartingZPos);
                float scale3D = Draw3D.GetScaleFor3D(zPos);
                if ((scale3D < 1 && CurrentDrawPassIsBehindNPCsAndTiles) || (scale3D >= 1 && CurrentDrawPassIsOverPlayers))
                {
                    Vector2[] orbPositions = GetOrbPositions(center, timer, RandomSeed, i);
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
            if (CurrentDrawPassIsBehindNPCsAndTiles)
            {
          //      PreviewHitboxes();
            }
            return false;
        }
    }
}
