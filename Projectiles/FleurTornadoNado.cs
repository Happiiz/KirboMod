using KirboMod.Projectiles.Tornadoes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace KirboMod.Projectiles
{
	public class FleurTornadoNado : Tornado
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fleur Tornado");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
            Projectile.ArmorPenetration = 9999;
        }
        public override Color[] SetPalette()
        {
            Color[] palette = { new Color(154, 212, 255), Color.White, new Color(105, 226, 255) };
            return palette;
        }

        public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

			Projectile.scale = 1.2f;

            //stuff here so it doesn't oppose drain
            player.manaRegenDelay = 20;
            player.manaRegenCount = 0;
            player.manaCost = 1;

            //Tooken from old Example Mod (Bad idea)
            bool manaIsAvailable = player.CheckMana(1, true, false); //consume 1 mana per tick
            bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;

            if (stillInUse) //HOMING
            {
                float speed = 40f; //top speed
				float inertia = 20f; //acceleration and decceleration speed

				if (Projectile.owner == Main.myPlayer)
				{
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

			//Feathers
			if (Projectile.ai[0] % 5 == 0)
			{
				Vector2 velocity = Main.rand.NextVector2Circular(25f, 25f); //circle
                Feathers(velocity);
            }
		}

        private void UpdateDamageForManaSickness(Player player) //Tooken from old Example Mod
        {
            float ownerCurrentMagicDamage = player.GetDamage(DamageClass.Generic).Multiplicative + (player.GetDamage(DamageClass.Magic).Multiplicative - 1f);
            Projectile.damage = (int)(player.HeldItem.damage * ownerCurrentMagicDamage);
        }

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 30; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(12f, 15f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, speed * 2, Scale: 3f); //Makes dust in a messy circle
				d.noGravity = true;
			}

			for (int i = 0; i < 10; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 velocity = Main.rand.NextVector2Circular(25f, 25f); //circle
                Feathers(velocity);
			}
		}
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(108, 120)).Intersects(targetHitbox);
        }

		private void Feathers(Vector2 velocity)
		{
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<Projectiles.FleurTornadoFeather>(), Projectile.damage / 2, 5f, Projectile.owner);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Collision.CanHit(Projectile, target);
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