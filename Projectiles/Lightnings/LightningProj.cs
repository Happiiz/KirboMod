using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Projectiles.Lightnings
{
    public abstract class LightningProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossLightningOrbArc;
        public override void SetStaticDefaults()
        {
            //MAKE SURE THE OLD POS LENGTH IS A MULTIPLE OF MAXUPDATES + 1
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
        }
        /// <summary>
        /// SHOULD BE CALLED IN SetStaticDefaults !!!!!!!
        /// </summary>
        public static void SetAmountOfLightingSegments(int amount, int projID)
        {
            ProjectileID.Sets.TrailCacheLength[projID] = amount * 3 + 1;
            ProjectileID.Sets.DrawScreenCheckFluff[projID] = 1500;
            ProjectileID.Sets.TrailingMode[projID] = 2;
        }
        public override void SetDefaults()
        {
            maxDeviation = 40;
            width = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 2 * 60 * Projectile.MaxUpdates;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.Size = new(1);//tile collide hitbox
            opacityFunction = DefaultOpacityFunction;
        }
        protected Color outerColor;
        protected Color innerColor;
        protected float width;
        protected float maxDeviation = 40;
        protected Func<float, float> opacityFunction;
        float DefaultOpacityFunction(float progress)
        {
            return Utils.GetLerpValue(0, .25f, progress, true);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);
        public override void AI()
        {
            if (Projectile.localAI[2] == 0)
            {
                Projectile.oldPos[0] = Projectile.position;
                RandomizeDirection();
            }
            Projectile.localAI[2]++;
            Projectile.frameCounter++;
            Lighting.AddLight(Projectile.Center, outerColor.ToVector3());
            if (Projectile.velocity == Vector2.Zero)
            {
                if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
                {
                    Projectile.frameCounter = 0;
                    bool killProj = true;
                    for (int i = 1; i < Projectile.oldPos.Length; i++)
                    {
                        if (Projectile.oldPos[i] != Projectile.oldPos[0])
                        {
                            killProj = false;
                        }
                    }
                    if (killProj)
                    {
                        Projectile.Kill();
                        return;
                    }
                }

                if (Main.rand.NextBool(Projectile.MaxUpdates))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float dustAngle = Projectile.rotation + ((Main.rand.NextBool(2)) ? (-1f) : 1f) * ((float)Math.PI / 2f);
                        float dustVelLength = (float)Main.rand.NextDouble() * 0.8f + 1f;
                        Vector2 dustVel = new Vector2((float)Math.Cos(dustAngle) * dustVelLength, (float)Math.Sin(dustAngle) * dustVelLength);
                        int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric, dustVel.X, dustVel.Y);
                        Main.dust[dustIndex].noGravity = true;
                        Main.dust[dustIndex].scale = 1.2f;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        Vector2 dustPosOffset = Projectile.velocity.RotatedBy(1.5707963705062866) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                        int dustIndex = Dust.NewDust(Projectile.Center + dustPosOffset - Vector2.One * 4f, 8, 8, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                        Dust dust = Main.dust[dustIndex];
                        dust.velocity *= 0.5f;
                        Main.dust[dustIndex].velocity.Y = 0f - Math.Abs(Main.dust[dustIndex].velocity.Y);
                    }
                }
            }
            else
            {
                if (Projectile.frameCounter < Projectile.extraUpdates * 2)
                {
                    return;
                }
                RandomizeDirection();
            }
        }
        void RandomizeDirection()
        {
           
            Projectile.frameCounter = 0;
            float projVelLength = Projectile.velocity.Length();
            UnifiedRandom unifiedRandom = new UnifiedRandom((int)Projectile.ai[1]);
            int loopCounter = 0;
            Vector2 finalLightningDirection = -Vector2.UnitY;
            while (true)
            {
                int seed = unifiedRandom.Next();
                Projectile.ai[1] = seed;
                seed %= 100;
                float branchDirection = seed / 100f * MathF.Tau;
                Vector2 lightningDirection = branchDirection.ToRotationVector2();
                if (lightningDirection.Y > 0f)
                {
                    lightningDirection.Y *= -1f;
                }
                bool increaseCounter = false;
                if (lightningDirection.Y > -0.02f)
                {
                    increaseCounter = true;
                }
                if (lightningDirection.X * (Projectile.extraUpdates + 1) * 2f * projVelLength + Projectile.localAI[0] > maxDeviation)
                {
                    increaseCounter = true;
                }
                if (lightningDirection.X * (Projectile.extraUpdates + 1) * 2f * projVelLength + Projectile.localAI[0] < -maxDeviation)
                {
                    increaseCounter = true;
                }
                if (increaseCounter)
                {
                    if (loopCounter++ >= 100)
                    {
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localAI[1] = 1f;
                        break;
                    }
                    continue;
                }
                finalLightningDirection = lightningDirection;
                break;
            }
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.localAI[0] += finalLightningDirection.X * (Projectile.extraUpdates + 1) * 2f * projVelLength;
                Projectile.velocity = finalLightningDirection.RotatedBy(Projectile.ai[0] + MathF.PI / 2f) * projVelLength;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D laserTex = TextureAssets.Extra[33].Value;
            Vector2 halfSize = new Vector2(Projectile.scale) / 2f;
            Vector2 gfxOff = Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            for (int i = 0; i < 3; i++)
            {
                DelegateMethods.f_1 = 1f;
                for (int j = Projectile.oldPos.Length - 1; j > 0; j -= Projectile.MaxUpdates)
                {
                    SetColorVariable(i, ref halfSize, opacityFunction(Utils.GetLerpValue(Projectile.oldPos.Length, 0, j)));
                    //MAKE SURE THE OLD POS LENGTH IS A MULTIPLE OF MAXUPDATES + 1
                    if (Projectile.oldPos[j] == Vector2.Zero)
                    {
                        continue;
                    }
                    Vector2 start = Projectile.oldPos[j] + new Vector2(Projectile.width, Projectile.height) / 2f + gfxOff;
                    Vector2 end = Projectile.oldPos[j - Projectile.MaxUpdates] + new Vector2(Projectile.width, Projectile.height) / 2f + gfxOff;
                    if (start == end || Projectile.oldPos[j - Projectile.MaxUpdates] == Vector2.Zero)
                    {
                        continue;
                    }

                    Utils.DrawLaser(Main.spriteBatch, laserTex, start, end, halfSize, DelegateMethods.LightningLaserDraw);
                }
                SetColorVariable(i, ref halfSize, 1);
                if (Projectile.oldPos[0] == Vector2.Zero)
                {
                    continue;
                }
                DelegateMethods.f_1 = 1f;
                Vector2 lastPos = Projectile.oldPos[Projectile.MaxUpdates] + Projectile.Size / 2 + gfxOff;

               // if (Projectile.oldPos[Projectile.MaxUpdates] != Vector2.Zero)
               // {
                   // Utils.DrawLaser(Main.spriteBatch, laserTex, lastPos, drawPos, halfSize, DelegateMethods.LightningLaserDraw);
                //}
            }
            if (Projectile.velocity == Vector2.Zero)
            {
                VFX.DrawGlowBallAdditive(Projectile.Center, 1, outerColor, Color.Transparent, true);
            }
            if (Projectile.localAI[2] <= Projectile.oldPos.Length - 1)
            {
                VFX.DrawGlowBallAdditive(Projectile.oldPos[(int)Projectile.localAI[2]], 1, outerColor, Color.Transparent, true);
            }
            return false;
        }
        static void DrawLaserLine(Vector2 from, Vector2 to, float width, Color color)
        {
            Texture2D line = VFX.GlowLine;
            Texture2D cap = VFX.GlowLineCap;
            Vector2 origin = line.Size() / 2;
            Vector2 lineScale = new Vector2(width, from.Distance(to) / line.Height);
            Vector2 pos = (from + to) / 2;
            float rotation = (to - from).ToRotation() + MathF.PI;
            Main.EntitySpriteDraw(line, pos, null, color, rotation, origin, lineScale, SpriteEffects.None);
        }
        public static void GetSpawningStats(Vector2 velocity, out float ai0, out float ai1)
        {
            ai0 = velocity.ToRotation();
            ai1 = Main.rand.Next(100);
        }
        void SetColorVariable(int i, ref Vector2 halfSize, float opacityMultiplier)
        {
            switch (i)
            {
                case 0:
                    halfSize = new Vector2(Projectile.scale) * 0.6f;
                    DelegateMethods.c_1 = outerColor * 0.5f * opacityMultiplier;
                    break;
                case 1:
                    halfSize = new Vector2(Projectile.scale) * 0.4f;
                    DelegateMethods.c_1 = Color.Lerp(outerColor, innerColor, 0.5f) * 0.5f * opacityMultiplier;
                    break;
                default:
                    halfSize = new Vector2(Projectile.scale) * 0.2f;
                    DelegateMethods.c_1 = innerColor * 0.5f * opacityMultiplier;
                    break;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            Vector2 pos = Projectile.Center;
            pos.X = MathF.Floor(pos.X / 16) * 16 + 8;//truncate 
            pos.Y = MathF.Floor(pos.Y / 16) * 16 + 8;//truncate  
            if (SolidTile(0, 1))
            {
                pos.Y += 8;
            }
            if (SolidTile(0, -1))
            {
                pos.Y -= 8;
            }
            if (SolidTile(1))
            {
                pos.X += 8;
            }
            if (SolidTile(-1))
            {
                pos.X -= 8;
            }
            Projectile.Center = pos;
            Projectile.tileCollide = false;
            return false;
        }
        
        bool SolidTile(int xOffset = 0, int yOffset = 0) => Collision.SolidTiles(Projectile.Center + new Vector2(xOffset, yOffset) * 16, 1, 1);
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Rectangle hitbox = new Rectangle((int)Projectile.oldPos[i].X, (int)Projectile.oldPos[i].Y, Projectile.width, Projectile.height);
                ModifyDamageHitbox(ref hitbox);
                if (targetHitbox.Intersects(hitbox))
                    return true;
            }   
            return projHitbox.Intersects(targetHitbox);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            return true;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float size = Projectile.friendly ? 46 : 32;//bigger hitboxes if it's friendly so it feels better for the player
            hitbox = Utils.CenteredRectangle(hitbox.Center(), new Vector2(size));
        }
    }
}
