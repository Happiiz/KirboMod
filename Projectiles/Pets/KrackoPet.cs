using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Pets
{
	public class KrackoPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lil' Krackle");
			Main.projFrames[Projectile.type] = 2;
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.height = 26;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
		}

		// Here you can decide if your pet breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		public override void AI()
		{
            Projectile.spriteDirection = Projectile.direction;

            Player player = Main.player[Projectile.owner];

            KirbPlayer modPlayer = player.GetModPlayer<KirbPlayer>();
            if (player.dead)
            {
                modPlayer.krackoPet = false;
            }
            if (modPlayer.krackoPet)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 IdlePosition = player.Center;
            //float petPositionOffsetX = (40 + Projectile.minionPos * 40) * -player.direction; //behind player depending on order summoned
            //IdlePosition.X += petPositionOffsetX;

            Vector2 vectorToIdlePosition = IdlePosition - Projectile.Center; //distance from idle
            float distanceToIdlePosition = vectorToIdlePosition.Length(); //aboslute distance from idle

            if (distanceToIdlePosition <= 1000f) //move within this range
            {
                float speed = 6f;
                float inertia = 12f;

                Projectile.tileCollide = false;

                Vector2 direction = player.Center - Projectile.Center; //start - end

                float distance = Vector2.Distance(player.Center, Projectile.Center);

                if (distance <= 60) //close to player
                {
                    Projectile.velocity *= 0.8f;
                }
                else
                {
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;  //fly towards player
                }
            }
            else //teleport
            {
                Projectile.Center = player.Center;
            }

            if (++Projectile.frameCounter >= 30) //changes frames every 10 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 1) //higher than 1
                {
                    Projectile.frame = 0; //start of bob
                }
            }

            //crying water
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 600)
            {
                if (Projectile.ai[0] % 10 == 0)
                {
                    Dust.NewDustPerfect(Projectile.Center, Dust.dustWater(), Scale: 1.5f);
                }
            }
            if (Projectile.ai[0] >= 900)
            {
                Projectile.ai[0] = 0; //reset
            }
        }
    }
}