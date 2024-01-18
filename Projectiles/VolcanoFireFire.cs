using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class VolcanoFireFire : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false; //don't deal damage to enemies, only explosion
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
		public override void AI()
		{
			Projectile.rotation += Projectile.velocity.X * 0.02f;
			Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
			if (Projectile.velocity.Y >= 10f)
			{
				Projectile.velocity.Y = 10f;
			}

			if (Main.rand.NextBool(2)) // happens 1/2 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 50, 50, DustID.Torch, 0f, 0f, 200, default, 1.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

            //explode when in contact with npc
            for (int i = 0; i < Main.maxNPCs; i++) //loop statement that cycles completely every tick
            {
                NPC npc = Main.npc[i]; //any npc

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.active) //hitboxes touching
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
         public override void OnKill(int timeLeft) //when the projectile dies
         {
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.01f, //no zero else it won't launch right
				ModContent.ProjectileType<Projectiles.VolcanoFireExplode>(), Projectile.damage, 7f, Projectile.owner);
         }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}