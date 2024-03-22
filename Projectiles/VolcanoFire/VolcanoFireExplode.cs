using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.VolcanoFire
{
    public class VolcanoFireExplode : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Volcanic Explosion");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound
                for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 velocity1 = Main.rand.NextVector2Circular(3f, 3f); //circle
                    velocity1.Y -= 3;
                    Gore gore = Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(30,30), velocity1, Main.rand.Next(61, 63), Scale: 1f); //smoke
                    gore.position.X -= gore.Width / 2;
                    gore.position.Y -= gore.Height / 2;//center the gore because it wasn't being centered for some reason
                    gore.rotation = Main.rand.NextFloat() * MathF.Tau;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }
    }
}