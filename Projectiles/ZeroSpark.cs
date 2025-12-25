using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Projectiles
{
    /// <summary>
    /// All the sparks and explosions are a single projectile
    /// this is to avoid spawning 1 bajillion projectiles and potentially filling up the projectile cap
    /// and it also saves up on sending a lot of projectile sync packets by using deterministic RNG like this
    /// </summary>
    public class ZeroSpark : ModProjectile
    {
      
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spark");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 7000;
        }
        public static int Lifetime => 120;
        public static int ExplosionDuration => 10;
        bool Exploded { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }
        public int RNGSeed => (int)Projectile.ai[2];
        public static int SparkRings => 20;
        public static int SparksPerRing => 50;
        public static int SparkRowsAndColumns => Main.getGoodWorld ? 22 : Main.expertMode ? 18 : 14;
        public static float SquareSideLength => 3000;
        public static float CircleRadius => 2000;
        //+1 for the one that always spawns in the middle
        public static int SparkCount => SparkRowsAndColumns * SparkRowsAndColumns + 1;
        public Vector2[] sparkTargetPositions;
        public float[] rotationDirSigns;
        public float[] rotationSpeeds;
        public Rectangle[] hitboxes;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = Lifetime + ExplosionDuration;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Projectile.localAI[1]++;

            InitializeArraysIfNeeded();
            if (Projectile.localAI[1] == Lifetime)
            {
                Exploded = true;
                for (int i = 0; i < SparkCount; i++)
                {
                    hitboxes[i] = Utils.CenteredRectangle(sparkTargetPositions[i], new Vector2(100));
                }
            }
        }

        private void AI_Old()
        {
            Projectile.velocity *= 0.96f;
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] >= Lifetime && !Exploded)
            {
                Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(100));
                Projectile.friendly = false;
                Projectile.hostile = true;
                Projectile.tileCollide = false;
                Projectile.penetrate = -1;
                Projectile.scale = 1f;
                Projectile.alpha = 50;
                Exploded = true;
                Projectile.velocity = default;
                SoundEngine.PlaySound(SoundID.Item11.WithVolumeScale(0.8f), Projectile.Center);//boom
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public static Vector2 AccountForVelocity(Vector2 targetPos, Vector2 targetVelocity)
        {
            return targetPos + targetVelocity * Lifetime;
        }
        void InitializeArraysIfNeeded()
        {
            if (sparkTargetPositions == null)
            {
                sparkTargetPositions = new Vector2[SparkCount];
                rotationDirSigns = new float[SparkCount];
                rotationSpeeds = new float[SparkCount];
                hitboxes = new Rectangle[SparkCount];
                UnifiedRandom rnd = new(RNGSeed);
                for (int i = 0; i < SparkCount; i++)
                {
                    Vector2 offset = Vector2.Zero;
                    if (i != SparkRowsAndColumns - 1 || SparkRowsAndColumns % 2 == 0)
                    {
                        if (i != SparkRowsAndColumns - 1)
                        {
                            int indexInColumn = i / SparkRowsAndColumns;
                            int indexInRow = i % SparkRowsAndColumns;
                            offset.Y = Utils.Remap(indexInColumn, 0, SparkRowsAndColumns, -SquareSideLength / 2f, SquareSideLength / 2f);
                            offset.X = Utils.Remap(indexInRow, 0, SparkRowsAndColumns, -SquareSideLength / 2f, SquareSideLength / 2f);
                            float individualSquareSideLength = SquareSideLength / SparkRowsAndColumns;
                            individualSquareSideLength *= 0.5f;
                            offset += Main.rand.NextVector2Square(-individualSquareSideLength, individualSquareSideLength);
                        }
                    }
                    sparkTargetPositions[i] = Projectile.velocity + offset;
                    rotationDirSigns[i] = rnd.NextBool() ? -1 : 1;
                    rotationSpeeds[i] = rnd.NextFloat(-.1f, .1f);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            InitializeArraysIfNeeded();
            for (int i = 0; i < sparkTargetPositions.Length; i++)
            {
                Vector2 targetPos = sparkTargetPositions[i];
                Vector2 center = Vector2.Lerp(Projectile.Center, targetPos, Easings.EaseOutSine(Utils.GetLerpValue(0, 30f, Projectile.localAI[1], true)));

                if (Exploded)
                {
                    Main.instance.LoadProjectile(ModContent.ProjectileType<ZeroSparkExplosion>());
                    Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<ZeroSparkExplosion>()].Value;
                    Projectile.scale = Utils.GetLerpValue(ExplosionDuration, 0, Projectile.timeLeft, true);
                    Projectile.scale = Easings.EaseOut(Projectile.scale, 2);
                    Projectile.scale = MathHelper.Lerp(1, 1 + 0.05f * ExplosionDuration, Projectile.scale);
                    Projectile.Opacity = Utils.Remap(Projectile.timeLeft, ExplosionDuration * .7f, 0, 0.8f, 0);
                    Lighting.AddLight(center, 1f, 0.9f, 0);
                    Main.EntitySpriteDraw(texture, center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
                    continue;
                }

                Projectile.localAI[0] = rotationSpeeds[i];// Main.rand.NextFloat(-.1f, .1f);
                Projectile.localAI[2] = rotationDirSigns[i];// Main.rand.NextBool() ? -1 : 1;

                float scale = Utils.GetLerpValue(Lifetime * .3f, Lifetime * .8f, Projectile.localAI[1]);
                if (scale < 0)
                {
                    scale = 0;
                }
                scale = Easings.EaseInOutSine(scale);
                scale *= 2;
                Vector2 scaleVec = new(scale);
                float rotation = Utils.GetLerpValue(Lifetime * .4f, Lifetime, Projectile.localAI[1], true);
                rotation = Easings.EaseIn(rotation, 4);
                rotation *= MathF.PI;
                rotation *= Projectile.localAI[2];
                rotation += Projectile.localAI[0];
                Color whiteAdditive = new(255, 255, 255, 0);
                VFX.DrawGlowBallDiffuse(center, scale * 1.25f, Color.Black * .5f, default);

                VFX.DrawPrettyStarSparkle(1, center - Main.screenPosition, whiteAdditive, Color.Blue with { A = 0 }, Projectile.localAI[1],
                    0, 5, Lifetime - 0.001f, Lifetime, rotation, scaleVec, scaleVec / Helper.Phi);
                if (Projectile.localAI[1] < Lifetime / 2)
                    VFX.DrawGlowBallAdditive(center, 0.4f, Color.Blue, Color.White, false);
            }
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return Exploded;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Exploded)
            {
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
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}