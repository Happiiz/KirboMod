using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Pets
{
	public class ZeroEyePetProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Zero's Eye"); // Automatic from .lang files
			Main.projFrames[Projectile.type] = 1;
			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.LightPet[Projectile.type] = true;
            //no character preview because it light pet
        }

        public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			Projectile.width = 42;
			Projectile.height = 42;
			Projectile.tileCollide = false;
			Projectile.aiStyle = -1;
		}

		public override bool PreAI() 
		{
			Player player = Main.player[Projectile.owner];
			//player.zephyrfish = false; // Relic from aiType
			return true;
		}

		public override void AI() 
		{
			Player player = Main.player[Projectile.owner];
			KirbPlayer modPlayer = player.GetModPlayer<KirbPlayer>();

			if (!Main.dedServ)
			{
				Lighting.AddLight(Projectile.Center, 2f, 0, 0); //red light
			}

			if (player.dead) {
				modPlayer.zeroEyePet = false;
			}
			if (modPlayer.zeroEyePet) {
				Projectile.timeLeft = 2;
			}

			Projectile.rotation = Projectile.velocity.ToRotation();

			float speed = 20f;
			float inertia = 25f;

			Vector2 moveTo = player.Center;
			Vector2 direction = moveTo - Projectile.Center; //start - end

			float distance = Vector2.Distance(player.Center, Projectile.Center);

			if (distance <= 60) //idle
			{
				Projectile.velocity = Projectile.velocity;
			}
			else if (distance <= 1000f) //move within this range
            {
				direction.Normalize();
				direction *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; //follow player
            }
            else //teleport
            {
                Projectile.Center = player.Center;
            }
        }

		public override Color? GetAlpha(Color drawColor)
		{
			return Color.White; //make it unaffected by light
		}
	}
}