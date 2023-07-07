using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Pets
{
	public class NightmarePet : ModProjectile
	{
		bool flying = false; //checks if flying
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shade Mage");
			Main.projFrames[Projectile.type] = 5;
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(3, 2, 10)
                .WithOffset(-4, -20f)
                .WithCode(CharacterPreviewCustomization);
        }
        public static void CharacterPreviewCustomization(Projectile proj, bool walking)
        {

        }

        public sealed override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 38;
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
            Player player = Main.player[Projectile.owner];

            KirbPlayer modPlayer = player.GetModPlayer<KirbPlayer>();
            if (player.dead)
            {
                modPlayer.nightmarePet = false;
            }
            if (modPlayer.nightmarePet)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 IdlePosition = player.Center;

            Vector2 vectorToIdlePosition = IdlePosition - Projectile.Center; //distance from idle
            float distanceToIdlePosition = vectorToIdlePosition.Length(); //aboslute distance from idle

            if (distanceToIdlePosition <= 1000f) //move within this range
            {
                if (distanceToIdlePosition > 600) //fly if far away 
                {
                    flying = true;
                }
                if (distanceToIdlePosition <= 60) //close to player
                {
                    flying = false;
                }

				//dashing
                if (flying == true) //stretch and go faster
                {
                    Projectile.spriteDirection = Projectile.direction;
                    if (Projectile.direction == -1)
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180); //rotate towards velocity with offset
                    }
                    else
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation(); //rotate towards velocity
                    }

                    float speed = 9f;
                    float inertia = 6f;

                    Vector2 direction = player.Center - Projectile.Center; //start - end

                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;  //fly towards player   

                    //animation
                    if (++Projectile.frameCounter >= 10) //changes frames every 10 ticks 
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame > 4)
                        {
                            Projectile.frame = 3; //start of dash
                        }
                    }

                    //leave dust
                    if (Projectile.frameCounter % 2 == 0)
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Vector2.Zero, 0, default, 1f);
                    }
                }
                else //hover 
                {
                    Projectile.spriteDirection = Projectile.direction; //only face direction when not stretched
                    Projectile.rotation = MathHelper.ToRadians(0 + (Projectile.velocity.X * 0.5f) * Projectile.direction);

                    float speed = 5f;
                    float inertia = 8f;

                    Vector2 direction = IdlePosition - Projectile.Center; //start - end

                    if (distanceToIdlePosition <= 60) //close to player
                    {
                        Projectile.velocity = Projectile.velocity;
                    }
                    else
                    {
                        direction.Normalize();
                        direction *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; 
                    }

                    //animation (CHANGE THIS SO IT REVERSES INSTEAD OF LOOPING)
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter < 10) //changes frames every 10 ticks 
                    {
                        Projectile.frame = 0; //start of bob
                    }
                    else if (Projectile.frameCounter < 20) 
                    {
                        Projectile.frame = 1; //middle of bob
                    }
                    else if (Projectile.frameCounter < 30) 
                    {
                        Projectile.frame = 2; //end of bob
                    }
                    else if (Projectile.frameCounter < 40) 
                    {
                        Projectile.frame = 1; //middle of bob
                    }
                    if (Projectile.frameCounter >= 50)
                    {
                        Projectile.frameCounter = 0; //reset
                    }
                }
            }
            else //teleport
            {
                Projectile.Center = player.Center;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}