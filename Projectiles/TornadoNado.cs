using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class TornadoNado : ModProjectile
	{
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tornado");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 90;
			Projectile.height = 100;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

			//stuff here so it doesn't oppose drain
			player.manaRegenDelay = 20;
            player.manaRegenCount = 0;
            player.manaCost = 1;

            //Altered from old Example Mod (Bad idea)
            bool manaIsAvailable = player.CheckMana(1, true, false); //consume 1 mana per tick 
            bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;

            if (stillInUse) //HOMING
            {
				if (Projectile.owner == Main.myPlayer)
				{
					float speed = 40f; //top speed
					float inertia = 35f; //acceleration and decceleration speed

					Vector2 direction = Main.MouseWorld - Projectile.Center; //start - end 

					direction.Normalize();
					direction *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; //movement
				}
                UpdateDamageForManaSickness(player); //Tooken from old Example Mod (Bad idea)
            }
            else
            {
                Projectile.Kill();
            }

            if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}

        private void UpdateDamageForManaSickness(Player player) //Tooken from old Example Mod
        {
            float ownerCurrentMagicDamage = player.GetDamage(DamageClass.Generic).Multiplicative + (player.GetDamage(DamageClass.Magic).Multiplicative - 1f);
            Projectile.damage = (int)(player.HeldItem.damage * ownerCurrentMagicDamage);
        }

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, speed * 2, Scale: 3f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X) //bounce
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y) //bounce
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}
	}
}