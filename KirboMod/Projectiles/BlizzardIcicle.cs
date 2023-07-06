using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BlizzardIcicle : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			DrawOffsetX = -6;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 1200; //20 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.alpha = 50;
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			Projectile.velocity.Y += 0.4f;
			if (Projectile.velocity.Y > 16f)
            {
				Projectile.velocity.Y = 16f;
            }

			if (Main.rand.NextBool(8)) // happens 1/8 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, -(Projectile.velocity.X) * 0.2f, -(Projectile.velocity.Y) * 0.2f, 0, default, 1f); //dust
				Main.dust[dustnumber].noGravity = true;
			}
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item27, Projectile.position); //crystal smash

			for (int i = 0; i < 15; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, speed * 2, Scale: 1f); //Makes dust in a messy circle
				d.noGravity = true;
			}
			
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (target.active) //checks if the npc is active
			{
				//spawns body ice on npc 
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<BodyIce>(), Projectile.damage / 4, 0, Projectile.owner, target.whoAmI); 
			}

            if (target.life <= 0 & target.boss == false) //checks if the npc is dead
            {
                SoundEngine.PlaySound(SoundID.Item46, Projectile.position); //ice hydra
                for (int i = 0; i < 8; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(4f, 4f); //circle
                    Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, Scale: 1f); //Makes dust in a messy circle
                    d.noGravity = false;
                }

                //spawns ice chunk 
                Player player = Main.player[Projectile.owner];
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position, new Vector2(player.direction * 5, 0), ModContent.ProjectileType<Projectiles.IceChunk>(), Projectile.damage * 2, 6, Projectile.owner);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.rand.NextBool(5)) //chance of making a blizzard formation
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<BlizzardFormation>(), Projectile.damage / 2, 4f, Projectile.owner);
            }
			return true;
        }
    }
}