using KirboMod.Items.Zero;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items;
using KirboMod.Items.Nightmare;
using KirboMod.Systems;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Timers;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public partial class NightmareWizard : ModNPC
	{
		private int animation = 0; //which frames to cycle through
		private int attack = 0; //timer that counts through attack cycle
		private int despawntimer = 0; //for despawning
		private NightmareAttackType attacktype = NightmareAttackType.SpreadStars; //sets attack type
		private NightmareAttackType lastattacktype = NightmareAttackType.Swoop; //sets last attack type

		private int phase = 1; //decides what kind of attack cycle

        int tpEffectCounter = 12;
        Vector2 tpEffectPos;

        public override void AI() //constantly cycles each time
		{
			Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction; //face whatever direction

            tpEffectCounter++;

            //Despawn
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active || Main.dayTime) //Despawn
			{
				NPC.TargetClosest(false);
				animation = 8; //despawn
                NPC.velocity *= 0.01f;

                if (despawntimer == 0) //reset animation
                {
                    NPC.frameCounter = 0;
                }

                despawntimer++;

				if (despawntimer > 12)//teleport away
				{
					NPC.Center += new Vector2( 0, -2000);
                }

				if (NPC.timeLeft > 12)
				{
					NPC.timeLeft = 12;
					return;
				}
			}
			else //regular attack
			{
				AttackCycle();
				despawntimer = 0;
			}
		}
        void AttackDecideNext()
        {
            List<NightmareAttackType> possibleAttacks = new() { NightmareAttackType.SpreadStars, NightmareAttackType.RingStars, 
                NightmareAttackType.Swoop, NightmareAttackType.Tornado, NightmareAttackType.Stoop };

            possibleAttacks.Remove(lastattacktype);
            possibleAttacks.TrimExcess();
            attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
            lastattacktype = attacktype;
            NPC.netUpdate = true;
        }


        private void AttackCycle()
	    {
			Player player = Main.player[NPC.target];

            NPC.ai[0]++;

			if (NPC.ai[0] < 120) //intro
			{
                NPC.TargetClosest(false);

                animation = 0;
            }
			else //beginning of cycle
			{
				if (NPC.ai[0] == 120)
				{
					animation = 0;

					NPC.damage = 0; //reset

                    NPC.velocity *= 0.01f; //slow

                    NPC.TargetClosest(false);

					AttackDecideNext();
                }

                if (attacktype == NightmareAttackType.SpreadStars)
                {
					AttackSpreadStars(NPC.ai[0] - 120, player);
                }
                if (attacktype == NightmareAttackType.RingStars)
                {
                    AttackRingStars(NPC.ai[0] - 120, player);
                }
                if (attacktype == NightmareAttackType.Swoop)
                {
					AttackSwoop(NPC.ai[0] - 120, player);
                }
                if (attacktype == NightmareAttackType.Tornado)
                {
                    AttackTornado(NPC.ai[0] - 120, player);
                }
                if (attacktype == NightmareAttackType.Stoop)
                {
                    AttackStoop(NPC.ai[0] - 120, player);
                }
            }
		}

		private void AttackSpreadStars(float timer, Player player) //NPC.ai[0] subtracted by 60 to make counting more simple
		{
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -200)); 

			if (timer == 90) //move to side
            {
				animation = 1; 
				if (NPC.Center.X > player.Center.X)
				{
					NPC.velocity.X = 10;
					NPC.direction = -1;
				}
                else
                {
                    NPC.velocity.X = -10;
                    NPC.direction = 1;
                }
            }
			if (timer >= 120)
			{
				if (timer == 120)
				{
					animation = 3; //damageable
                    NPC.velocity /= 2;
                }


                if (timer % 10 == 0)
				{
                    Vector2 direction = player.Center - NPC.Center;

                    direction.Normalize();
                    direction *= 20;

                    SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedByRandom(MathHelper.ToRadians(30)),
                        ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);
                }
            }

			if (timer >= 360)
			{
				NPC.ai[0] = 119; //restart
			}

        }

        private void AttackRingStars(float timer, Player player)
        {
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));

			if (timer > 40)
            {
				animation = 5; //damageable

				//a little delay before actual attack
				if (timer > 80)
				{
                    if (timer % 20 == 0)
                    {
                        for (float i = 0; i < 12; i += 1f)
                        {
							float angle = ((timer - 60) * 2f) + (i * (MathF.PI / 6));

                            Vector2 vel = new Vector2(MathF.Cos(angle) * 15, MathF.Sin(angle) * 15);

                            SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel,
                                ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);
                        }
                    }
                }
            }

            if (timer >= 240)
            {
                NPC.ai[0] = 119; //restart
            }
        }

        private void AttackSwoop(float timer, Player player)
        {
            Vector2 playerDistance = player.Center - NPC.Center;

            float speed = 30f;
            float inertia = 15f;

            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));

			if (timer >= 60 && timer < 180)
            {
                animation = 4; //swoop

                float xOffset = 800;

                if (playerDistance.X <= 0) //if player is behind enemy
                {
                    xOffset = 800; // go in front of player 
                }
                else
                {
                    xOffset = -800; // go behind player
                }

                Vector2 playerXOffest = player.Center + new Vector2(xOffset, 0f); //go in front of player
                Vector2 move = playerXOffest - NPC.Center;

                NPC.TargetClosest(true); //face player only for charge

                move.Normalize();
                move *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;
            }
			else if (timer >= 180)
			{
                NPC.velocity.X += NPC.direction * 1.5f;

                speed = 10f; //make slower

                playerDistance.Normalize();
                playerDistance *= speed;
                NPC.velocity.Y = (NPC.velocity.Y * (inertia - 1) + playerDistance.Y) / inertia;

                if (NPC.ai[0] == 180) //reset velocity for sweep
                {
                    NPC.velocity.X = 0;
                }

                //make hitbox
                int x = (int)NPC.position.X;
                int y = (int)NPC.position.Y - 75; //move up a bit 

                
                Rectangle hitbox = new Rectangle(x, y, NPC.width, NPC.height);

                //hurt
                if (hitbox.Intersects(player.getRect()) && player.immune == false)
				{
					player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 70, NPC.direction);
				}
            }

			if (timer > 240)
            {
                NPC.velocity *= 0.01f;

                NPC.ai[0] = 119; //restart
            }
        }

		private void AttackTornado(float timer, Player player)
		{
			//teleport 
            if (timer == 1)
			{
				NPC.ai[1] = timer; //time to start teleport
            }
			Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));

			if (timer >= 60) //initiate animation
			{
				animation = 6;

                if (timer >= 90) //glide towards player
                {
                    Vector2 playerDistance = player.Center - NPC.Center;

                    float speed = 18f;
                    float inertia = 20f;

                    playerDistance.Normalize();
                    playerDistance *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + playerDistance) / inertia;
                }
            }

			if (timer >= 270)
			{
                NPC.velocity *= 0.01f;

                NPC.ai[0] = 119; //restart
            }
        }

		private void AttackStoop(float timer, Player player)
		{
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -400));

			if (timer >= 12)
			{
                animation = 7;

				if (timer < 60) //rise
                {
                    //hover over player
                    Vector2 playerDistance = player.Center + new Vector2(0, -200) - NPC.Center;

                    float speed = 15f;
                    float inertia = 10f;

                    playerDistance.Normalize();
                    playerDistance *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + playerDistance) / inertia;
                }
				else if (timer < 120)
                {
                    NPC.velocity.Y *= 0.9f; //slow while stooping

                    NPC.velocity.X *= 0.01f;

                    if (timer == 60) //stoop
					{
						NPC.velocity.Y = (player.Center.Y - NPC.Center.Y) / 5; //distance depends on player distance

                        //caps

                        if (NPC.velocity.Y < 30)
                        {
                            NPC.velocity.Y = 30;
                        }

                        if (NPC.velocity.Y > 60)
                        {
                            NPC.velocity.Y = 60;
                        }
                    }
                }
            }

			if (timer >= 120)
            {
                NPC.ai[0] = 119; //restart
            }
        }

		private void Teleport(float timer, Player player, Vector2 location)
		{
            if (timer == 1)
            {
                tpEffectPos = NPC.Center;
                tpEffectCounter = 4;//animSpeed

                NPC.Center = player.Center + location;
				NPC.frameCounter = 0;
            }
            if (timer < 12)
			{
				animation = 2;
			}
			else if (timer == 12)
            {
                animation = 0;
            }
        }
        void TpEffectDraw()
        {
            if (tpEffectCounter > 12)
            {
                return;
            }
            int animSpeed = 4;//change this to whatever nightmare uses
            Texture2D sheet = TextureAssets.Npc[Type].Value;
            //3 frames on the tp anim
            int frameY = (int)Utils.Remap(tpEffectCounter, animSpeed, animSpeed * 3, 10, 12, true); 
            Rectangle frame = sheet.Frame(1, 19, 0, frameY);
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);
            Main.EntitySpriteDraw(sheet, tpEffectPos - Main.screenPosition, frame, Color.White, 0, origin, 1f, SpriteEffects.None);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			TpEffectDraw();

			return true;
        }

        public override void FindFrame(int frameHeight) // animation
        {
			if (animation == 0) //idle
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 12)
				{
					NPC.frame.Y = 0; 
				}
				else
				{
					NPC.frame.Y = frameHeight; 
				}
				if (NPC.frameCounter >= 24)
				{
					NPC.frameCounter = 0;
				}

                NPC.dontTakeDamage = true;
            }
            if (animation == 1) //moving side
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 7)
				{
					NPC.frame.Y = frameHeight * 2; 
				}
				else
				{
					NPC.frame.Y = frameHeight * 3; 
				}
				if (NPC.frameCounter >= 14)
				{
					NPC.frameCounter = 0;
				}

                NPC.dontTakeDamage = true;

            }

            if (animation == 2) //teleport
            {
				NPC.frameCounter++;
				if (NPC.frameCounter < 4)
				{
					NPC.frame.Y = frameHeight * 12; 
				}
				else if (NPC.frameCounter < 8)
				{
					NPC.frame.Y = frameHeight * 11; 
				}
				else
				{
					NPC.frame.Y = frameHeight * 10; 
				}
				if (NPC.frameCounter >= 12)
				{
					NPC.frameCounter = 0;
				}

                NPC.dontTakeDamage = true;
            }

            if (animation == 3) //open robe one side
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 4; 
				}
				else
				{
					NPC.frame.Y = frameHeight * 5; 
				}
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;
				}

                NPC.dontTakeDamage = false;

                NPC.damage = NPC.defDamage; //set to inital damage
            }

            if (animation == 4) //swoop 
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 6; 
				}
				else
				{
					NPC.frame.Y = frameHeight * 7; 
				}
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;
				}

                NPC.dontTakeDamage = true;
            }

            if (animation == 5) //fully open robe
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 8; 
				}
				else
				{
					NPC.frame.Y = frameHeight * 9; 
				}
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;
				}

                NPC.dontTakeDamage = false;

                NPC.damage = NPC.defDamage; //set to inital damage
            }

            if (animation == 6) //tornado attack
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 3)
				{
					NPC.frame.Y = frameHeight * 13; 
				}
				else if (NPC.frameCounter < 6)
				{
					NPC.frame.Y = frameHeight * 14; 
				}
				else if (NPC.frameCounter < 9)
				{
					NPC.frame.Y = frameHeight * 15; 
				}
				else
				{
					NPC.frame.Y = frameHeight * 16; 
				}
				if (NPC.frameCounter >= 12)
				{
					NPC.frameCounter = 0;
                }

                NPC.dontTakeDamage = false;

                NPC.damage = NPC.defDamage; //set to inital damage
            }

            if (animation == 7) // stoop attack
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 5)
				{
					NPC.frame.Y = frameHeight * 17; 
				}
				else
				{
					NPC.frame.Y = frameHeight * 18; 
				}
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;
				}

                NPC.dontTakeDamage = false;

                NPC.damage = NPC.defDamage; //set to inital damage
            }

            if (animation == 8) //despawn teleport
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 4)
                {
                    NPC.frame.Y = frameHeight * 10;
                }
                else if (NPC.frameCounter < 8)
                {
                    NPC.frame.Y = frameHeight * 11;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 12;
                }
                if (NPC.frameCounter >= 12)
                {
                    NPC.frameCounter = 0;
                }

                NPC.dontTakeDamage = true;
            }
        }
	}
}