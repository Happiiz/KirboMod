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
			Projectile.timeLeft = 15;
			Projectile.tileCollide = false;
			Projectile.penetrate = 99;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
			Projectile.scale = Projectile.scale + 0.025f;
			if (Main.rand.NextBool(75)) // happens 1/75 times
			{
				//swap X vel and Y vel(also make them negative)
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToDegrees(5)) / 2, ModContent.ProjectileType<Projectiles.FrostySculpture>(), Projectile.damage, 0.1f, Projectile.owner);
			}
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