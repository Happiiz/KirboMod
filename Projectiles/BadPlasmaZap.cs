using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Projectiles
{
    public class BadPlasmaZap : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = true;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
        }
        static int KeyPosRandomizationInterval => 10;
        float KeyPosSpreadSize => Projectile.friendly ? 16f * 5f : 16 * 2f;//SHAPE IS A SQUARE
        ref float Timer => ref Projectile.ai[0];
        Vector2 PreviousKeyPos { get => new Vector2(Projectile.ai[1], Projectile.ai[2]); set { Projectile.ai[1] = value.X; Projectile.ai[2] = value.Y; } }
        Vector2 KeyPos { get => new Vector2(Projectile.localAI[1], Projectile.localAI[2]); set { Projectile.localAI[1] = value.X; Projectile.localAI[2] = value.Y; } }
        public override void AI()
        {
            if (Timer % KeyPosRandomizationInterval == 0)
            {
                if (Timer == 0)
                {
                    PreviousKeyPos = Projectile.Center;
                }
                else
                {
                    //UnifiedRandom rnd = new UnifiedRandom(Projectile.identity + (int)Timer - KeyPosRandomizationInterval);
                    PreviousKeyPos = KeyPos;
                }
                UnifiedRandom rnd = Main.rand;
                KeyPos = new Vector2(rnd.NextFloat() * KeyPosSpreadSize * 2 - KeyPosSpreadSize, rnd.NextFloat() * KeyPosSpreadSize * 2 - KeyPosSpreadSize) + Projectile.Center + Projectile.velocity * (KeyPosRandomizationInterval - 1);//Maybe it should be subtracted by 1? not sure. Shouldn't make that big of a difference either way
            }
            Timer++;
            float keyPosChangeProgress = (Timer % KeyPosRandomizationInterval) / KeyPosRandomizationInterval;
            float prevKeyPosChangeProgress = ((Timer - 1) % KeyPosRandomizationInterval) / KeyPosRandomizationInterval;
            Vector2 previousKeyPos = PreviousKeyPos;
            Vector2 currentKeyPos = KeyPos;
            Vector2 dustPos = Vector2.Lerp(previousKeyPos, currentKeyPos, keyPosChangeProgress);
            Vector2 prevDustPos = Vector2.Lerp(previousKeyPos, currentKeyPos, prevKeyPosChangeProgress);
            float increment = 1f/Vector2.Distance(prevDustPos, dustPos);
            int dustInterval = 17;
            for (float i = 0; i < 1f; i+= increment)
            {
                Projectile.frameCounter++;//using the frame counter NOT to count frames... and instead to count interval in ticks for dust spawning
                if (Projectile.frameCounter <= dustInterval)
                {
                    continue;
                }
                Projectile.frameCounter = 0;
                Vector2 finalDustPos = Vector2.Lerp(prevDustPos, dustPos, i);

                Vector2 dustVel = Main.rand.BetterNextVector2Circular(1f);
                Dust dust = Dust.NewDustPerfect(finalDustPos, DustID.TerraBlade, dustVel);
                dust.color = VFX.RndElectricCol;
                dust.noLight = true;
                dust.noLightEmittence = true;
                dust.scale *= 2f;
                dust.noGravity = true;
            }

            //Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade);
            //dust.noGravity = true;
            //dust.velocity *= .5f;
            //dust.fadeIn = 1;
            //Projectile.spriteDirection = Projectile.direction;
            //Projectile.rotation = Projectile.velocity. ToRotation();
            //Lighting.AddLight(Projectile.Center, Color.Green.ToVector3());
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 visualPos = Vector2.Lerp(PreviousKeyPos, KeyPos, (Timer % KeyPosRandomizationInterval) / KeyPosRandomizationInterval);
            
            return projHitbox.Intersects(targetHitbox) || Utils.CenteredRectangle(visualPos, new Vector2(8)).Intersects(targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 scale = new Vector2(Projectile.scale, Projectile.scale * 1.5f);
            Vector2 visualPos = Vector2.Lerp(PreviousKeyPos, KeyPos, ((Timer - 1) % KeyPosRandomizationInterval) / KeyPosRandomizationInterval);

            VFX.DrawPrettyStarSparkle(Projectile.Opacity, visualPos - Main.screenPosition, Color.White with { A = 0 }, Color.Lime, 1f, 0f, 0.5f, 2f, 3f, 0f, scale, scale);
            return false;//Projectile.DrawSelf(lightColor);
        }
    }
}