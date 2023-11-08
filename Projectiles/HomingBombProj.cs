using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class HomingBombProj : ModProjectile
	{
		private bool groundcollide;
		private bool awake = false;
        ref float Power => ref Projectile.ai[1];

        private List<float> Targetdistances = new List<float>(); //targeting
        private NPC aggroTarget = null; //target the minion is currently focused on
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Homing bomb");
			Main.projFrames[Projectile.type] = 5;

            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.height = 38;
			DrawOriginOffsetY = -18;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
            if (Projectile.velocity.Y == 0)
            {
                awake = true;
            }

            //slow down when on ground
            if (Projectile.velocity.Y == 0 && awake)
            {
                Projectile.velocity.X *= 0.96f; //slow
            }

            //Gravity
            Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            if (Projectile.velocity.Y >= 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            //Animation
            if (awake)
			{
				if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
				{
					Projectile.frameCounter = 0;
					if (++Projectile.frame >= Main.projFrames[Projectile.type])
					{
						Projectile.frame = 1; //start on frame 1 instead of 0 as it's the beginning of this animation loop
					}
				}
			}
			else //still rolling
			{
                Projectile.frame = 0;
            }

            //explode when in contact with npc
            for (int i = 0; i < Main.maxNPCs; i++) //loop statement that cycles completely every tick
            {
                NPC npc = Main.npc[i]; //any npc

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.friendly == false && npc.active == true) //hitboxes touching
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


			//HOMING

			if (awake) //start targeting if hit the ground
			{
				Projectile.ai[0]++; //for nothing except radar
                Projectile.rotation = 0; //up straight

                Projectile.spriteDirection = Projectile.direction;

                //Targeting
				float distanceFromTarget = 500f;

                if (aggroTarget == null || !aggroTarget.active || aggroTarget.dontTakeDamage) //search target
                {
                    //start each number with a very big number so they can't be targeted if their npc doesn't exist
                    Targetdistances = Enumerable.Repeat(999999f, Main.maxNPCs).ToList();

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        float distance = Vector2.Distance(Projectile.Center, npc.Center);

                        if (npc.CanBeChasedBy()) //checks if targetable
                        {
                            Vector2 positionOffset = new Vector2(0, -5);
                            bool inView = Collision.CanHitLine(Projectile.position + positionOffset, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

                            //close, hittable, hostile and can see target
                            if (inView && !npc.friendly && !npc.dontTakeDamage && !npc.dontCountMe && distance < distanceFromTarget && npc.active)
                            {
                                Targetdistances.Insert(npc.whoAmI, (int)distance); //add to list of potential targets
                            }
                        }

                        if (i == Main.maxNPCs - 1)
                        {
                            int theTarget = -1;

                            //count up 'til reached maximum distance
                            for (float j = 0; j < distanceFromTarget; j++)
                            {
                                int Aha = Targetdistances.FindIndex(a => a == j); //count up 'til a target is found in that range

                                if (Aha > -1) //found target
                                {
                                    theTarget = Aha;

                                    break;
                                }
                            }

                            if (theTarget > -1) //exists
                            {
                                NPC npc2 = Main.npc[theTarget];

                                if (npc2 != null) //exists
                                {
                                    aggroTarget = npc2;
                                }
                            }
                            else
                            {
                                break; //just in case
                            }
                        }
                    }
                }
                else if (aggroTarget != null && aggroTarget.active && !aggroTarget.dontTakeDamage) //ATTACK
                {
                    Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end
					float speed = 10f;
					float inertia = 15f;

					//Jumping
					Vector2 jumprange = aggroTarget.Center - Projectile.Center;
					float distance = Math.Abs(direction.X); //get absolute
					if (jumprange.Y <= -20 && groundcollide == true && Math.Abs(jumprange.X) < 200 || (Projectile.velocity.X == 0 && groundcollide == true)) //jump when a little close and no attack cycle and when touching ground
					{
						Projectile.velocity.Y = -10; //jump
						groundcollide = false; //wait to touch floor again
					}

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

                    //Direction
                    if (direction.X >= 0)
					{
						Projectile.direction = 1;
					}
					else
					{
						Projectile.direction = -1;
					}
				}
			}
        }
        public override void OnKill(int timeLeft) //when the projectile dies
        {
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.01f, //no zero else it won't launch right
                ModContent.ProjectileType<Projectiles.HomingBombExplosion>(), Projectile.damage + (int)((Projectile.damage * 0.4) * Power), 12, Projectile.owner, 0, Power);
        }

        public override bool? CanCutTiles()
        {
            return false; //can't cut grass and pots
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (Projectile.oldVelocity.Y > 0f) //if was falling
			{
				groundcollide = true;
			}
			return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public static Asset<Texture2D> ChainBombChain;
        public static Asset<Texture2D> Radar;

        public override bool PreDraw(ref Color lightColor)
        {
            ChainBombChain = ModContent.Request<Texture2D>("KirboMod/Projectiles/ChainBombChain");
            Radar = ModContent.Request<Texture2D>("KirboMod/Projectiles/HomingBombRadar");

            int r;
            int g;
            int b;
            r = 255 - (int)(Projectile.ai[0] * 600);
            g = 255 - (int)(Projectile.ai[0] * 600);
            b = 255 - (int)(Projectile.ai[0] * 600);
			
            if (Projectile.ai[0] > 0 && Projectile.ai[0] < 30) //make radar
			{
                Main.EntitySpriteDraw(Radar.Value, Projectile.Center - Main.screenPosition,
                            Radar.Value.Bounds, (Projectile.GetAlpha(Color.White) * Projectile.Opacity) * ((255 - (Projectile.ai[0] * 10)) / 255), 
                            MathHelper.ToRadians(Projectile.ai[0] * 10), Radar.Size() / 2, 1f + (Projectile.ai[0] * 0.05f), SpriteEffects.None, 0);
            }

            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            //Vector2 center = Projectile.Center;

            Power = 0; //reset power

            for (int i = 0; i <= Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (Vector2.Distance(Projectile.Center, proj.Center) < 200 && proj.type == ModContent.ProjectileType<HomingBombProj>()
                    && Projectile.whoAmI != proj.whoAmI && proj.active)
                {
                    Power += 1; //add 1 to power

                    Vector2 directionToBomb = Projectile.Center - proj.Center;
                    Vector2 center = proj.Center;
                    float projRotation = directionToBomb.ToRotation() - MathHelper.PiOver2;
                    float distance = directionToBomb.Length();
                    while (distance > 15f && !float.IsNaN(distance)) //draw only while 15 units away from source bomb (and an actual number I guess)
                    {
                        directionToBomb.Normalize();                   //get unit vector
                        directionToBomb *= ChainBombChain.Height();     // multiply by chain link length
                        center += directionToBomb;                   //update draw position
                        directionToBomb = Projectile.Center - center;    //update distance
                        distance = directionToBomb.Length();
                        Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                        //Draw chain
                        Main.EntitySpriteDraw(ChainBombChain.Value, center - Main.screenPosition,
                            ChainBombChain.Value.Bounds, Color.White, projRotation,
                            ChainBombChain.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                    }
                }
            }

            return true;
        }
    }
}