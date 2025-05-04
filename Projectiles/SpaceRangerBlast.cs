using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class SpaceRangerBlast : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Space Blast");
		}

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			DrawOffsetX = -18; //make hitbox line up with sprite middle
			Projectile.friendly = false; //can't damage enemies(directly)
			Projectile.hostile = false;
			Projectile.timeLeft = 1000;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = Main.rand.Next(10);
			}
			Projectile.localAI[0]++;
			if (Projectile.localAI[0] % 10 == 0)
			{
				int dustCount = 30;
				for (int i = 0; i < dustCount; i++)
				{
					Vector2 vel = Utils.Remap(i, 0, dustCount, 0, MathF.Tau).ToRotationVector2();
					vel.X *= 0.5f;
					vel = vel.RotatedBy(Projectile.velocity.ToRotation());
					Dust d = Dust.NewDustPerfect(Projectile.Center + vel * 3 + Projectile.velocity * 2, DustID.Electric, vel * 3);
					d.noGravity = true;
					d.scale -= 0.2f;
				}
			}
			//projectile.spriteDirection = projectile.direction;
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Projectile.localAI[0] % 2 == 0) 
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}
            //explode when in contact with npc
            for (int i = 0; i < Main.maxNPCs; i++) //loop statement that cycles completely every tick
            {
                NPC npc = Main.npc[i]; //any npc

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.active && !npc.dontTakeDamage) //hitboxes touching
                {
                    Projectile.Kill();
                }
            }

            //player here too incase pvp
            for (int i = 0; i < Main.maxPlayers; i++) //loop statement that cycles completely every tick
            {
                Player player = Main.player[i]; //any player

                //hitboxes touching and player is on opposing team
                if (player.Hitbox.Intersects(Projectile.Hitbox) && player.InOpposingTeam(Main.player[Projectile.owner]))
                {
                    Projectile.Kill();
                }
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void OnKill(int timeLeft) //when the projectile dies
		{
			if (Main.myPlayer == Projectile.owner)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity *= 0, ModContent.ProjectileType<Projectiles.SpaceRangerBlastExplosion>(), Projectile.damage, 2f, Projectile.owner);
			}
		}
	}
}