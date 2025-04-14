using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class HardenedPebble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 600; //10 seconds
            Projectile.tileCollide = false;
            Projectile.penetrate = 6;
            Projectile.usesLocalNPCImmunity = true; //doesn't wait for other projectiles to hit again
            Projectile.localNPCHitCooldown = 10; //time until able to hit npc even if npc has just been struck (default)

        }
        public override void AI()
        {
            //Gravity
            Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            if (Projectile.velocity.Y >= 22f)
            {
                Projectile.velocity.Y = 22f;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 1)
            {
                Projectile.tileCollide = true;
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Projectile.velocity * 0); //Makes dust
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(MathF.Max(1, Projectile.damage * 0.6f));
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 circle = Main.rand.BetterNextVector2Circular(1f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, circle, Scale: 1f); //Makes dust in a messy circle
            }
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }
    }
}