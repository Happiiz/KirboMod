using KirboMod.Dusts;
using KirboMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.On_Actions.NPCs;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace KirboMod.NPCs
{
    [AutoloadBossHead]
	public partial class DarkMatter : ModNPC
	{
		private int animation = 0; //frame cycles

		private int phase = 1; //phase 1 = blade, phase 2 = transition, phase 3 = true form

		private bool frenzy = false; 

        private Vector2 playerTargetArea = Vector2.Zero;

        private DarkMatterAttackType attacktype = DarkMatterAttackType.DarkBeams;

        private DarkMatterAttackType lastattacktype = DarkMatterAttackType.Orbs;

        public override void AI() //constantly cycles each time
		{
			Player playerstate = Main.player[NPC.target];

			//cap life
			if (NPC.life >= NPC.lifeMax)
			{
				NPC.life = NPC.lifeMax;
			}
			//DESPAWNING
			if (NPC.target < 0 || NPC.target == 255 || playerstate.dead || !playerstate.active)
			{
				NPC.TargetClosest(false);

				if (phase == 1 || phase == 2) //rise into the sky to never be seen again!
				{
					NPC.velocity.Y = NPC.velocity.Y - 0.4f;
				}
				else //rise phaster
                {
					NPC.velocity.Y = NPC.velocity.Y - 0.2f;
				}

                NPC.ai[0] = 0;

				if (NPC.timeLeft > 60)
				{
					NPC.timeLeft = 60;
					return;
				}
			}
			else //regular attack
            {
				AttackPattern();
			}
		}
		private void AttackPattern()
		{
            NPC.ai[0]++;

            NPC.spriteDirection = NPC.direction;

            if (phase == 1)
            {
                //passive effects
                if (Main.rand.NextBool(4)) //1/4 chance
                {
                    Dust.NewDust(NPC.position, NPC.width, 40, ModContent.DustType<DarkResidue>(), 0f, -2f, 200, default, 0.75f); //dust
                }

                if (NPC.ai[0] < 30)
                {
                    NPC.velocity *= 0.9f; //slow during intermission

                    animation = 0;

                    NPC.TargetClosest();

                    if (NPC.ai[0] == 1)
                    {
                        AttackDecideNext();

                        if (NPC.GetLifePercent() < 0.75f && Main.expertMode) //Enrage
                        {
                            frenzy = true;
                        }
                    }
                }
                else
                {
                    if (attacktype == DarkMatterAttackType.DarkBeams)
                    {
                        if (frenzy == true)
                        {
                            EnrageDarkBeams();
                        }
                        else
                        {
                            AttackDarkBeams();
                        }
                    }
                    if (attacktype == DarkMatterAttackType.Dash)
                    {
                        if (frenzy == true)
                        {
                            EnrageDash();
                        }
                        else
                        {
                            AttackDash();
                        }
                    }
                    if (attacktype == DarkMatterAttackType.Orbs)
                    {
                        if (frenzy == true)
                        {
                            EnrageOrbs();
                        }
                        else
                        {
                            AttackOrbs();
                        }
                    }
                    /*switch (attacktype)
                    {
                        case DarkMatterAttackType.DarkBeams:
                            AttackDarkBeams();
                            break;
                        case DarkMatterAttackType.Dash:
                            AttackDash();
                            break;
                        case DarkMatterAttackType.Orbs:
                            AttackOrbs();
                            break;
                    }
                    AttackDash();*/
                }
            }
        }

        void AttackDecideNext()
        {
            List<DarkMatterAttackType> possibleAttacks = new() { DarkMatterAttackType.DarkBeams, DarkMatterAttackType.Dash, DarkMatterAttackType.Orbs };

            /*if (NPC.GetLifePercent() < 0.75f && Main.expertMode) //Enrage
                possibleAttacks = new() { DarkMatterAttackType.EnrageDarkBeams, DarkMatterAttackType.EnrageDash, DarkMatterAttackType.EnrageOrbs };*/
            possibleAttacks.Remove(lastattacktype);
            possibleAttacks.TrimExcess();
            attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
            lastattacktype = attacktype;
            NPC.netUpdate = true;
        }

        void AttackDarkBeams()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;
            NPC.TargetClosest();
            animation = 0;

            float xOffset = 400;

            if (playerDistance.X <= 0) //if player is behind enemy
            {
                xOffset = 400; // go in front of player 
            }
            else
            {
                xOffset = -400; // go behind player
            }

            if (NPC.ai[0] % 30 == 0) //every 30 ticks
            {
                playerTargetArea = player.Center + player.velocity * 30;
            }

            Vector2 playerXOffest = player.Center + new Vector2(xOffset, -20f); //go in front of player
            Vector2 move = playerXOffest - NPC.Center;

            Vector2 velocity = new Vector2(NPC.direction * 30, 0);

            Vector2 position = NPC.Center + new Vector2(0, 30f);

            if (NPC.ai[0] > 30 && NPC.ai[0] < 50)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 50)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 60 && NPC.ai[0] < 80)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 80)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 90 && NPC.ai[0] < 110)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 110)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 120 && NPC.ai[0] < 140)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 140)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 150 && NPC.ai[0] < 170)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 170)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 180 && NPC.ai[0] < 200)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 200)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser

                if (NPC.GetLifePercent() > 0.75f && !Main.expertMode) //restart if high enough (and not expert mode)
                {
                    NPC.ai[0] = 0; //restart
                }
            }

            //keep going if low enough or expert mode

            if (NPC.ai[0] > 210 && NPC.ai[0] < 230)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 230)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 240 && NPC.ai[0] < 260)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 260)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 270 && NPC.ai[0] < 290)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 290)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 300 && NPC.ai[0] < 320)
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 320)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser

                NPC.ai[0] = 0; //restart
            }
        }

        void AttackDash()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;

            float speed = 30f;
            float inertia = 10f;

            if (NPC.ai[0] < 120) //ready charge
            {
                animation = 1; //dash

                float xOffset = 400;

                if (playerDistance.X <= 0) //if player is behind enemy
                {
                    xOffset = 400; // go in front of player 
                }
                else
                {
                    xOffset = -400; // go behind player
                }

                Vector2 playerXOffest = player.Center + new Vector2(xOffset, 0f); //go in front of player
                Vector2 move = playerXOffest - NPC.Center;

                NPC.TargetClosest(); //face player only for charge

                move.Normalize();
                move *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia; 
            }
            else if (NPC.ai[0] == 120) //stop X velocity for a moment to make the backup stand out more
            {
                NPC.velocity.X = 0.01f; //backup
            }
            else if (NPC.ai[0] < 150) //backup
            {
                NPC.velocity.X += -NPC.direction * 0.5f; //backup

                speed = 20f; //make smaller

                playerDistance.Normalize();
                playerDistance *= speed;
                NPC.velocity.Y = (NPC.velocity.Y * (inertia - 1) + playerDistance.Y) / inertia; //only change Y
            }
            else if (NPC.ai[0] < 210) //dash
            {
                NPC.velocity.X += NPC.direction * 1.5f;

                playerDistance.Normalize();
                playerDistance *= speed;
                NPC.velocity.Y = (NPC.velocity.Y * (inertia - 1) + playerDistance.Y) / inertia;

                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
            }

            if (NPC.GetLifePercent() > 0.75f && !Main.expertMode) //restart if high enough (and not expert mode)
            {
                if (NPC.ai[0] == 210)
                {
                    animation = 2; //lean back
                    NPC.velocity.X = -NPC.direction * 20; //jolt back
                    NPC.velocity.Y = 0.01f;
                }
                else if (NPC.ai[0] > 210)
                {
                    NPC.velocity.X *= 0.95f; //slow
                    NPC.velocity.Y = 0.01f;
                }

                if (NPC.ai[0] >= 240)
                {
                    NPC.ai[0] = 0; //restart
                }
            }

            else //below 75% or expert mode
            {
                if (NPC.ai[0] == 210) //stop a bit later
                {
                    animation = 2; //lean back
                    NPC.velocity.X = 0.01f; //stop
                }
                else if (NPC.ai[0] > 210) //back up
                {
                    NPC.velocity.X += -NPC.direction * 1.5f;

                    speed = 30f; //make smaller

                    playerDistance.Normalize();
                    playerDistance *= speed;
                    NPC.velocity.Y = (NPC.velocity.Y * (inertia - 1) + playerDistance.Y) / inertia;

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Shadowflame);
                }

                if (NPC.ai[0] >= 260)
                {
                    NPC.ai[0] = 0; //restart
                }
            }
        }

        void AttackOrbs()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            animation = 0;

            NPC.velocity *= 0.9f; //slow

            if (NPC.ai[0] < 40)
            {
                NPC.alpha += 30; //lemme be clear
            }
            else if (NPC.ai[0] == 40)
            {
                playerTargetArea = player.Center; //set dash target 

                float xlocation = -500;

                if (Main.rand.NextBool(2))
                {
                    xlocation = 500; 
                }

                //teleport to either side of the player
                NPC.Center = player.Center + new Vector2(xlocation, Main.rand.Next(-200, 200));
            }
            if (NPC.ai[0] == 70) //shoot
            {
                Vector2 Yoffset = new Vector2(0, -170);

                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Yoffset, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), 80 / 2, 6, default, 0, player.whoAmI);
            }

            if (NPC.ai[0] > 40)
            {
                NPC.alpha -= 30;
            }
            
            if (NPC.ai[0] > 130)
            {
                NPC.alpha = 0;
                NPC.ai[0] = 0;
            }
        }

        //Enraged attacks

        void EnrageDarkBeams()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;
            NPC.TargetClosest();
            animation = 0;

            float xOffset = 400;

            if (playerDistance.X <= 0) //if player is behind enemy
            {
                xOffset = 400; // go in front of player 
            }
            else
            {
                xOffset = -400; // go behind player
            }

            if (NPC.ai[0] % 30 == 0) //every 30 ticks
            {
                playerTargetArea = player.Center + player.velocity * 30;
            }

            Vector2 playerXOffest = player.Center + new Vector2(xOffset, -20f); //go in front of player
            Vector2 move = playerXOffest - NPC.Center;

            Vector2 velocity = new Vector2(NPC.direction * 30, 0);

            Vector2 position = NPC.Center + new Vector2(0, 30f);

            if (NPC.ai[0] > 20 && NPC.ai[0] < 30) //1
            {
                move /= 2;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 30)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 40 && NPC.ai[0] < 50) //2
            {
                move /= 2;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 50)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 60 && NPC.ai[0] < 70) //3
            {
                move /= 2;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 70)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 80 && NPC.ai[0] < 90) //4
            {
                move /= 2;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 90)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 100 && NPC.ai[0] < 110) //5
            {
                move /= 2;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 110)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 120 && NPC.ai[0] < 130) //6
            {
                move /= 2;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 130)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 140 && NPC.ai[0] < 150) //7
            {
                move /= 2;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 150)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            if (NPC.ai[0] > 160 && NPC.ai[0] < 170) //8
            {
                move /= 3;

                NPC.velocity = move;
            }
            if (NPC.ai[0] == 170)
            {
                NPC.velocity *= 0.01f;

                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            velocity = new Vector2(NPC.direction * 20, 0);

            Vector2 move2 = playerXOffest + new Vector2(0, -500)  - NPC.Center;

            if (NPC.ai[0] >= 180 && NPC.ai[0] < 190) //spread 1
            {
                move2 /= 3;

                NPC.velocity = move2;
            }
            if (NPC.ai[0] >= 190 && NPC.ai[0] < 240) //move down
            {
                NPC.velocity.Y = 20;

                NPC.velocity.X *= 0.01f;

                if (NPC.ai[0] % 5 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                    SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
                }
            }

            Vector2 move3 = playerXOffest + new Vector2(0, 500) - NPC.Center;

            if (NPC.ai[0] >= 240 && NPC.ai[0] < 250) //spread 2
            {
                move3 /= 3;

                NPC.velocity = move3;
            }
            if (NPC.ai[0] >= 250 && NPC.ai[0] < 300) //move up
            {
                NPC.velocity.Y = -20;

                NPC.velocity.X *= 0.01f;

                if (NPC.ai[0] % 5 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);

                    SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
                }
            }
            
            if (NPC.ai[0] > 300)
            {
                NPC.ai[0] = 0; //restart
            }
        }
        void EnrageDash()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;

            Vector2 targetDistance = playerTargetArea - NPC.Center;

            float speed = 30f;
            float inertia = 10f;


            if (NPC.ai[0] < 40)
            {
                NPC.alpha += 30; //lemme be clear
            }
            else if (NPC.ai[0] == 40)
            {
                playerTargetArea = player.Center; //set dash target 

                NPC.Center = player.Center + Main.rand.NextVector2CircularEdge(500, 500);

                //NPC.rotation = targetDistance.ToRotation(); //rotate towards target

                animation = 1;

                NPC.TargetClosest(); //flip direction once
            }
            else if (NPC.ai[0] < 60) //small backup
            {
                NPC.alpha -= 30; 

                targetDistance.Normalize();
                targetDistance *= -speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + targetDistance) / inertia;

                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
            }
            else if (NPC.ai[0] >= 60 && NPC.ai[0] < 80) //increase velocity
            {
                NPC.alpha -= 30;
                speed = 60f;

                targetDistance.Normalize();
                targetDistance *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + targetDistance) / inertia;

                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
            }

            if (NPC.ai[0] > 100)
            {
                NPC.rotation = 0; //reset
                NPC.alpha = 0;
                NPC.ai[0] = 0; //restart
            }
        }
        void EnrageOrbs()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            animation = 0;

            NPC.velocity *= 0.9f; //slow

            if (NPC.ai[0] < 40)
            {
                NPC.alpha += 30; //lemme be clear
            }
            else if (NPC.ai[0] == 40)
            {
                playerTargetArea = player.Center; //set dash target 

                float xlocation = -500;

                if (Main.rand.NextBool(2))
                {
                    xlocation = 500;
                }

                //teleport to either side of the player
                NPC.Center = player.Center + new Vector2(xlocation, Main.rand.Next(-200, 200));
            }
            if (NPC.ai[0] >= 70 && NPC.ai[0] <= 160) //shoot
            {
                if (NPC.ai[0] % 30 == 0) //shoot
                {
                    Vector2 Yoffset = new Vector2(0, -170);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Yoffset, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), 80 / 2, 6, default, 0, player.whoAmI, NPC.whoAmI);
                }
            }

            if (NPC.ai[0] > 40)
            {
                NPC.alpha -= 30;
            }
            
            if (NPC.ai[0] > 190)
            {
                NPC.alpha = 0;
                NPC.ai[0] = 0;
            }
        }

        public override void FindFrame(int frameHeight) // animation
        {
			if (animation == 0) //phase 1
			{
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 12.0)
                {
                    NPC.frame.Y = 0; //robe swish
                }
                else if (NPC.frameCounter < 24.0)
                {
                    NPC.frame.Y = frameHeight; //robe swoosh
                }
                else if (NPC.frameCounter < 36.0)
                {
                    NPC.frame.Y = frameHeight * 2; //robe swash
                }
                else if (NPC.frameCounter < 48.0)
                {
                    NPC.frame.Y = frameHeight * 3; //robe swush
                }
                else
                {
                    NPC.frameCounter = 0.0; //reset
                }
            }
            if (animation == 1) //dash
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 6.0)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                else if (NPC.frameCounter < 12.0)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
                else if (NPC.frameCounter < 18.0)
                {
                    NPC.frame.Y = frameHeight * 6;
                }
                else if (NPC.frameCounter < 24.0)
                {
                    NPC.frame.Y = frameHeight * 7;
                }
                else
                {
                    NPC.frameCounter = 0.0; //reset
                }
            }
            if (animation == 2) //retreat
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 6.0)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
                else if (NPC.frameCounter < 12.0)
                {
                    NPC.frame.Y = frameHeight * 9;
                }
                else if (NPC.frameCounter < 18.0)
                {
                    NPC.frame.Y = frameHeight * 10;
                }
                else if (NPC.frameCounter < 24.0)
                {
                    NPC.frame.Y = frameHeight * 11;
                }
                else
                {
                    NPC.frameCounter = 0.0; //reset
                }
            }
            if (animation == 3) //phase transition
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 6.0)
                {
                    NPC.frame.Y = frameHeight * 12; 
                }
                else if (NPC.frameCounter < 12.0)
                {
                    NPC.frame.Y = frameHeight * 13; 
                }
                else if (NPC.frameCounter < 18.0)
                {
                    NPC.frame.Y = frameHeight * 14; 
                }
                else if (NPC.frameCounter < 24.0)
                {
                    NPC.frame.Y = frameHeight * 15; 
                }
                else
                {
                    NPC.frameCounter = 0.0; //reset
                }
            }
			if (animation == 4) //phase 2
			{
				NPC.frameCounter += 1.0;
				if (NPC.frameCounter < 6.0)
				{
					NPC.frame.Y = frameHeight * 16; //idle
				}
				else if (NPC.frameCounter < 12.0)
				{
					NPC.frame.Y = frameHeight * 17; //lil' stretch
				}
				else
				{
					NPC.frameCounter = 0.0; //reset
				}
			}
		}

		// This npc uses additional textures for drawing
		public static Asset<Texture2D> DarkBlade;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			DarkBlade = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkBlade");

            float rotation = MathHelper.ToRadians(90);
            SpriteEffects direction = SpriteEffects.None;

            Vector2 offset = new Vector2(50, 30);  

            if (NPC.direction == -1) 
            {
                direction = SpriteEffects.FlipVertically;
                offset = new Vector2(-50, 30);
            }

            if ((attacktype == DarkMatterAttackType.Orbs && frenzy) && NPC.ai[0] >= 70 && NPC.ai[0] <= 160) //enraged
            {
                rotation = MathHelper.ToRadians(0);
                direction = SpriteEffects.None;
                offset = new Vector2(0, -20);
            }
            else if ((attacktype == DarkMatterAttackType.Orbs) && NPC.ai[0] >= 70 && NPC.ai[0] <= 90)  //regular
            {
                rotation = MathHelper.ToRadians(0);
                direction = SpriteEffects.None;
                offset = new Vector2(0, -20);
            }

            if ((attacktype == DarkMatterAttackType.Dash && frenzy) && NPC.ai[0] > 40) //change rotation for enrage dash
            {
                rotation += NPC.rotation;
            }

            if (phase == 1) //sword
			{
                Texture2D blade = DarkBlade.Value; 
                spriteBatch.Draw(blade, NPC.Center - Main.screenPosition + offset, null, new Color(255, 255, 255) * NPC.Opacity, rotation, new Vector2(19, 64), 1f, direction, 0f);
            }
        }
        // This npc uses additional textures for drawing
        public static Asset<Texture2D> afterimage;

        /*public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.instance.LoadNPC(NPC.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < NPC.oldPos.Length; k++) //start at 1 so not ontop of actual npc
            {
                Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 drawPos = (NPC.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, NPC.gfxOffY);

                SpriteEffects direction = SpriteEffects.None;

                if (NPC.direction == -1) //reverse
                {
                    direction = SpriteEffects.FlipHorizontally;
                }

                Color color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);

                Main.EntitySpriteDraw(texture, drawPos, null, color, NPC.rotation, drawOrigin, 1, direction);
            }

            return true;
        }*/

        public override void BossHeadSlot(ref int index)
        {
            if (phase == 1 || phase == 2) //if in first phase or transitioning
			{
				index = ModContent.GetModBossHeadSlot("KirboMod/NPCs/DarkMatter_Head_Boss");
			}
			else //second phase
            {
				index = ModContent.GetModBossHeadSlot("KirboMod/NPCs/DarkMatter_Head_Boss2");
			}
        }
    }
}
