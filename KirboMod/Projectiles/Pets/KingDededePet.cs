using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Pets
{
	public class KingDededePet : ModProjectile
	{
		bool flying = false; //checks if flying
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dee");
			Main.projFrames[Projectile.type] = 9;
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(4, 4, 10)
                .WithOffset(-2, 2f)
                .WithCode(CharacterPreviewCustomization);
        }
        public static void CharacterPreviewCustomization(Projectile proj, bool walking)
        {
            
        }

        public sealed override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 30;
            DrawOriginOffsetY = 1;
			Projectile.tileCollide = true;
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
                modPlayer.kingDededePet = false;
            }
            if (modPlayer.kingDededePet)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 vectorToPlayer = player.Center - Projectile.Center; //distance from idle
            float distanceToPlayer = vectorToPlayer.Length(); //aboslute distance from idle

            if (distanceToPlayer <= 1000f) //move within this range
            {
                if (distanceToPlayer > 300) //fly if far away 
                {
                    flying = true;
                }

                if (flying == true & Projectile.position.Y <= player.Center.Y - 50f && player.velocity.Y == 0) //stop flying after 
                {
                    flying = false;
                }

				//flying
                if (flying == true)
                {
                    float speed = 10f;
                    float inertia = 18f;

                    Projectile.tileCollide = false;

                    Vector2 direction = player.Center - Projectile.Center; //start - end

                    float distance = Vector2.Distance(player.Center, Projectile.Center);

                    if (distance <= 60) //close to player
                    {
                        Projectile.velocity = Projectile.velocity;
                    }
                    else
                    {
                        direction.Y -= 50f;
                        direction.Normalize();
                        direction *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;  //fly towards player
                    }

                    //flying
                    Projectile.frameCounter = 0;
                    Projectile.frame = 8; //fly   
                }
                else //walk
                {
                    //Gravity
                    Projectile.velocity.Y += 0.4f;
                    if (Projectile.velocity.Y >= 6f)
                    {
                        Projectile.velocity.Y = 6f;
                    }

                    float speed = 5f;
                    float inertia = 6f;

                    Projectile.tileCollide = true;

                    if (Math.Abs(vectorToPlayer.X) < 100f) //near idle position
                    {
                        Projectile.velocity.X *= 0.8f; //slow
                    }
                    else
                    {
                        Vector2 direction = player.Center - Projectile.Center; //start - end
                        int pseudoDirection = 1;
                        if (direction.X < 0) //enemy is behind
                        {
                            pseudoDirection = -1; //change direction so it will go towards enemy
                        }
                        //we put this instead of player.Center so it will always be moving top speed instead of slowing down when enemy is near but unreachable
                        //A "carrot on a stick" if you will

                        Vector2 carrotDirection = Projectile.Center + new Vector2(pseudoDirection * 50, 0) - Projectile.Center; //start - end 
                        carrotDirection.Normalize();
                        carrotDirection *= speed;

                        //use .X so it only effects horizontal movement
                        Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + carrotDirection.X) / inertia;
                    }

                    if (Math.Abs(vectorToPlayer.X) < 100f) //near idle position X
                    {
                        Projectile.velocity.X *= 0.8f; //slow

                        if (++Projectile.frameCounter >= 10) //changes frames every 10 ticks 
                        {
                            Projectile.frameCounter = 0;
                            if (++Projectile.frame >= 3)
                            {
                                Projectile.frame = 0; //start of idle
                            }
                        }
                    }
                    else //walk
                    {
                        if (++Projectile.frameCounter >= 10) //changes frames every 10 ticks 
                        {
                            Projectile.frameCounter = 0;
                            if (++Projectile.frame >= 7)
                            {
                                Projectile.frame = 4; //start of walk
                            }
                        }
                    }

                    //for stepping up tiles
                    Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                }
            }
            else //teleport
            {
                Projectile.Center = player.Center;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			Player player = Main.player[Projectile.owner];

            Vector2 toPlayer = player.Center - Projectile.Center;
            if (flying == true)
            {
                fallThrough = true;
            }
            else
            {
                if (toPlayer.Y > 1 && Math.Abs(toPlayer.X) < 300)
                {
                    fallThrough = true;
                }
                else
                {
                    fallThrough = false;
                }
            }

            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
		}
    }
}