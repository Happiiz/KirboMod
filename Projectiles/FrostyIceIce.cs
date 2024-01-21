using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class FrostyIceIce : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 20;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
		public override void AI()
        {
            //scale with timeLeft
            Projectile.scale = 1 + 0.05f * (10 - Projectile.timeLeft) < 1.5f ? 1 + 0.05f * (10 - Projectile.timeLeft) : 1.5f;

            if (Main.rand.NextBool(100)) // happens 1/100 times
			{
				//swap X vel and Y vel(also make them negative)
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity / 2, ModContent.ProjectileType<Projectiles.FrostySculpture>(), Projectile.damage, 0f, Projectile.owner);
			}

            if (Projectile.timeLeft <= 5) //fade when close to death
            {
                Projectile.alpha += 51;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //stop projectile
            Projectile.velocity *= 0.1f;

            Projectile.timeLeft = 5;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (target.life <= 0 & target.boss == false) //checks if the npc is dead
            {
                SoundEngine.PlaySound(SoundID.Item46, Projectile.position); //ice hydra
                for (int i = 0; i < 8; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(4f, 4f); //circle
                    Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, Scale: 1f); //Makes dust in a messy circle
                    d.noGravity = false;
                }

                Player player = Main.player[Projectile.owner]; //spawns ice chunk 
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position, new Vector2(player.direction * 5, 0), ModContent.ProjectileType<Projectiles.IceChunk>(), Projectile.damage * 2, 6, Projectile.owner);
            }
		}
	}
}