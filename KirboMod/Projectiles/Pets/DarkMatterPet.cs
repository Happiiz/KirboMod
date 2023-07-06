using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Pets
{
	public class DarkMatterPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Wanderer");
			Main.projFrames[Projectile.type] = 2;
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, 2, 10)
                .WithOffset(-4, -20f)
                .WithCode(CharacterPreviewCustomization);
        }
        public static void CharacterPreviewCustomization(Projectile proj, bool walking)
        {

        }

        public sealed override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 34;
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
                modPlayer.darkMatterPet = false;
            }
            if (modPlayer.darkMatterPet)
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
                float speed = 9f;
                float inertia = 12f;

                Projectile.tileCollide = false;

                Vector2 direction = player.Center - Projectile.Center; //start - end

                float distance = Vector2.Distance(player.Center, Projectile.Center);

                if (distance <= 60) //close to player
                {
                    Projectile.velocity = Projectile.velocity;
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

            if (++Projectile.frameCounter >= 10) //changes frames every 10 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 1) //higher than 1
                {
                    Projectile.frame = 0; //start of bob
                }
            }

            //leave dust
            if (Projectile.frameCounter % 5 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), Vector2.Zero, 0, default, 0.5f);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}