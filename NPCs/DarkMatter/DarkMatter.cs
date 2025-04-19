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

namespace KirboMod.NPCs.DarkMatter
{
    [AutoloadBossHead]
    public partial class DarkMatter : ModNPC
    {
        private int animation = 0; //frame cycles

        private int phase = 1; //phase 1 = regular, phase 2 = harder, phase 3 = enraged, phase 4 = transition

        private Vector2 playerTargetArea = Vector2.Zero;

        private DarkMatterAttackType attacktype = DarkMatterAttackType.DarkBeams;

        private DarkMatterAttackType lastattacktype = DarkMatterAttackType.Orbs;

        public override void AI() //constantly cycles each time
        {
            Player player = Main.player[NPC.target];

            //cap life
            if (NPC.life >= NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
            }

            if (phase == 4)
            {
                Transition();
            }
            //INTRO
            else if (NPC.ai[1] < 60)
            {
                if (NPC.ai[1] == 30) //teleport to player
                {
                    NPC.Center = player.Center + new Vector2(MathF.Cos((float)Utils.Lerp(0, MathF.Tau, Main.rand.NextFloat())) * 500,
                        MathF.Sin((float)Utils.Lerp(0, MathF.Tau, Main.rand.NextFloat())) * 500);

                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(20f, 20f); //circle
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 3); //Makes dust in a messy circle
                        d.noGravity = true;
                    }
                }

                NPC.velocity *= 0.98f;

                NPC.ai[1]++;

                NPC.TargetClosest();

                NPC.spriteDirection = NPC.direction;
            }
            //DESPAWNING
            else if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
            {
                NPC.TargetClosest(false);

                NPC.velocity.Y = NPC.velocity.Y - 0.2f; //rise

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

                    //Make attacks slightly harder
                    if (NPC.GetLifePercent() < 0.5f || Main.expertMode)
                    {
                        phase = 2;
                    }

                    //Enrage
                    if (NPC.GetLifePercent() < 0.5f && Main.expertMode)
                    {
                        phase = 3;
                    }
                }
            }
            else
            {
                if (attacktype == DarkMatterAttackType.DarkBeams)
                {
                    if (phase == 3)
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
                    if (phase == 3)
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
                    if (phase == 3)
                    {
                        EnrageOrbs();
                    }
                    else
                    {
                        AttackOrbs();
                    }
                }
            }
        }
        void AttackDecideNext()
        {
            List<DarkMatterAttackType> possibleAttacks = new() { DarkMatterAttackType.DarkBeams, DarkMatterAttackType.Dash, DarkMatterAttackType.Orbs };
            possibleAttacks.Remove(lastattacktype);
            attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
            lastattacktype = attacktype;
            NPC.netUpdate = true;
        }
        void AttackDarkBeams()
        {
            SetPlayerTargetAreaPhase2();
            DarkBeamAttackSetup();
            if (NPC.ai[0] >= 150 && phase == 1)
            {
                //restart if high enough (and not expert mode)
                NPC.ai[0] = 0; //restart         
            }
            for (int i = 0; i < 8; i++)
            {
                TripleShot(40 + i * 30);
            }
            if (NPC.ai[0] >= 270)
            {
                NPC.ai[0] = 0; //restart (2)
            }
        }
        private void DarkBeamAttackSetup()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;
            animation = 0;
            float xOffset = MathF.CopySign(400, -playerDistance.X);
            Vector2 playerXOffest = playerTargetArea + new Vector2(xOffset, -20f); //go in front of player
            NPC.velocity = (playerXOffest - NPC.Center) / 3;
        }
        private void SetPlayerTargetAreaPhase2()
        {
            Player player = Main.player[NPC.target];
            SetPlayerTargetArea();
            if (phase != 2)
                return;
            switch (NPC.ai[0]) //put extra checks here because this is also the first stopping point
            {
                case 150 or 210:
                    playerTargetArea = player.Center + new Vector2(0, 0);
                    break;
                case 180:
                    playerTargetArea = player.Center + new Vector2(0, -200);
                    break;
                case 240:
                    playerTargetArea = player.Center + new Vector2(0, 200);
                    break;
            }
        }
        private void SetPlayerTargetArea()
        {
            Player player = Main.player[NPC.target];
            switch (NPC.ai[0])
            {
                case 30 or 90:
                    playerTargetArea = player.Center + new Vector2(0, 0);
                    break;
                case 60:
                    playerTargetArea = player.Center + new Vector2(0, -200);
                    break;
                case 120:
                    playerTargetArea = player.Center + new Vector2(0, 200);
                    break;
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

                float xOffset = -MathF.CopySign(400, playerDistance.X);
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

            if (phase == 1) //restart if high enough (and not expert mode)
            {
                if (NPC.ai[0] == 210)
                {
                    animation = 2; //lean back
                    NPC.velocity.X = -NPC.direction * 20; //jolt back
                    NPC.velocity.Y *= 0.01f;
                }
                else if (NPC.ai[0] > 210)
                {
                    NPC.velocity.X *= 0.95f; //slow
                    NPC.velocity.Y *= 0.01f;
                }

                if (NPC.ai[0] >= 240)
                {
                    NPC.ai[0] = 0; //restart
                }
            }
            else //below half or expert mode
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
                NPC.netUpdate = true; //sync random movement
            }


            if (NPC.ai[0] > 40)
            {
                NPC.alpha -= 30;
            }

            //normal mode above half
            if (phase == 1)
            {
                if (NPC.ai[0] == 70) //shoot
                {
                    Vector2 Yoffset = new(0, -170);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Yoffset, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), 80 / 2, 6, default, 0, player.whoAmI);
                    }
                    PlayBallChargeSoundEffect(NPC.Center + Yoffset);
                }

                if (NPC.ai[0] > 130)
                {
                    NPC.alpha = 0;
                    NPC.ai[0] = 0;
                }
            }
            //normal mode below half or expert mode above half
            else
            {
                if (NPC.ai[0] >= 70 && NPC.ai[0] <= 160) //shoot
                {
                    if (NPC.ai[0] % 30 == 0) //shoot
                    {
                        Vector2 Yoffset = new(0, -170);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Yoffset, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), 80 / 2, 6, default, 0, player.whoAmI, NPC.whoAmI);
                        }
                        PlayBallChargeSoundEffect(NPC.Center + Yoffset);
                    }
                }

                if (NPC.ai[0] > 190)
                {
                    NPC.alpha = 0;
                    NPC.ai[0] = 0;
                }
            }
        }
        void EnrageDarkBeams()
        {
            SetPlayerTargetArea();

            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;
            animation = 0;
            float xOffset = MathF.CopySign(400, -playerDistance.X);
            Vector2 playerXOffest = playerTargetArea + new Vector2(xOffset, -20f); //go in front of player
            Vector2 position = NPC.Center + new Vector2(0, 30f);
            if (NPC.ai[0] < 160) //not doing spread attack
            {
                NPC.velocity = (playerXOffest - NPC.Center) / 3;
            }
            for (int i = 0; i < 4; i++)
            {
                TripleShot(40 + i * 30);
            }
            Vector2 velocity = new(NPC.direction * 20, 0);

            if (NPC.ai[0] == 160)
            {
                playerTargetArea = player.Center + new Vector2(xOffset, -525);
            }

            Vector2 move2 = playerTargetArea - NPC.Center;
            if (NPC.ai[0] >= 160 && NPC.ai[0] < 170) //spread 1
            {
                move2 /= 3;

                NPC.velocity = move2;
            }
            float start = 170;
            if (NPC.ai[0] >= start && NPC.ai[0] < start + 50) //move down
            {
                NPC.velocity.Y = 20;
                NPC.velocity.X *= 0.01f;
                if (NPC.ai[0] % 5 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);
                    SfxShoot();
                }
            }

            if (NPC.ai[0] == 220)
            {
                playerTargetArea = player.Center + new Vector2(xOffset, 475);
            }
            if (NPC.ai[0] >= 220 && NPC.ai[0] < 230) //spread 2
            {
                NPC.velocity = (playerTargetArea - NPC.Center) / 3;
            }

            if (NPC.ai[0] >= 230 && NPC.ai[0] < 280) //move up
            {
                NPC.velocity.Y = -20;
                NPC.velocity.X *= 0.01f;
                if (NPC.ai[0] % 5 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);
                    }
                    SfxShoot();
                }
            }

            if (NPC.ai[0] > 280)
            {
                NPC.ai[0] = 0; //restart
            }
        }

        private void TripleShot(float start)
        {
            Vector2 position = NPC.Center + new Vector2(0, 30f);
            Vector2 velocity = new(NPC.direction * 40, 0);
            if (NPC.ai[0] > start && NPC.ai[0] < start + 20)
            {
                if (NPC.ai[0] % 5 == 0) //every 5 ticks
                {
                    NPC.velocity *= 0.01f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<DarkBeam>(), 60 / 2, 6);
                    }
                    SfxShoot();
                }
            }
        }
        void EnrageDash()
        {
            Player player = Main.player[NPC.target];

            float speed = 30f;
            float inertia = 10f;

            Vector2 targetDistance = playerTargetArea - NPC.Center;

            if (NPC.ai[0] < 40)
            {
                NPC.alpha += 30; //lemme be clear
            }
            else if (NPC.ai[0] == 40)
            {
                NPC.Center = player.Center + Main.rand.NextVector2CircularEdge(500, 500);
                animation = 1;
                NPC.TargetClosest(); //flip direction once
                playerTargetArea = player.Center; //set dash target 
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
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            animation = 0;

            NPC.velocity *= 0.9f; //slow

            if (NPC.ai[0] < 40)
            {
                NPC.alpha += 30; //lemme be clear
            }
            else if (NPC.ai[0] < 100)
            {
                //teleport to left side of the player
                NPC.Center = player.Center + new Vector2(500, 0);
            }
            if (NPC.ai[0] >= 100 && NPC.ai[0] <= 280) //shoot
            {
                if (NPC.ai[0] % 20 == 0) //shoot
                {

                    Vector2 Yoffset = new(0, -170);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Yoffset, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), 80 / 2, 6, default, 0, player.whoAmI, 0);
                    }
                    PlayBallChargeSoundEffect(NPC.Center + Yoffset);
                }
                //spin around player
                NPC.Center = player.Center + new Vector2(MathF.Cos((NPC.ai[0] - 100) / 30) * 500, MathF.Sin((NPC.ai[0] - 100) / 30) * 500);
            }

            playerTargetArea = player.Center; //set dash target 

            if (NPC.ai[0] > 40)
            {
                NPC.alpha -= 30;
            }

            if (NPC.ai[0] > 310)
            {
                NPC.alpha = 0;
                NPC.ai[0] = 0;
            }
        }
        private void Transition() //transition to phase 2
        {
            NPC.ai[0]++;
            animation = 3;
            NPC.velocity *= 0.95f;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.rotation = 0;
            Vector2 speed = Main.rand.NextVector2Circular(15f, 15f); //circle
            Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 1); //Makes dust in a messy circle
            if (NPC.ai[0] == 240) //summon phase two
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PureDarkMatter>());
                }
                NPC.active = false;
            }
        }
        void SfxShoot()
        {
            SoundEngine.PlaySound(DarkBeamShoot, NPC.Center);
        }
        public override void FindFrame(int frameHeight) // animation
        {
            if (animation == 0) //idle
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
        }
        public override bool CheckDead()
        {
            NPC.active = true;
            NPC.life = 1;
            NPC.ai[0] = 0; //reset
            phase = 4; //transition into second phase (pure dark matter)
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }
        // This npc uses additional textures for drawing
        public static Asset<Texture2D> darkBlade;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            darkBlade = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkMatter/DarkBlade");

            float rotation = MathF.PI / 2;
            SpriteEffects direction = NPC.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 offset = new(50 * NPC.direction, 30);

            //orb animation
            if (attacktype == DarkMatterAttackType.Orbs)
            {
                if (phase == 3 && NPC.ai[0] >= 100) //second phase
                {
                    rotation = 0;
                    direction = SpriteEffects.None;
                    offset = new Vector2(0, -20);
                }
                else if (phase == 2 && NPC.ai[0] >= 70 && NPC.ai[0] <= 190) //buffed phase
                {
                    rotation = 0;
                    direction = SpriteEffects.None;
                    offset = new Vector2(0, -20);
                }
                else if (phase == 1 && NPC.ai[0] >= 70 && NPC.ai[0] <= 90)  //regular
                {
                    rotation = 0;
                    direction = SpriteEffects.None;
                    offset = new Vector2(0, -20);
                }
            }

            if ((attacktype == DarkMatterAttackType.Dash && phase == 2) && NPC.ai[0] > 40) //change rotation for enrage dash
            {
                rotation += NPC.rotation;
            }

            Texture2D blade = darkBlade.Value;

            if (phase != 4) //draw when not transitioning
            {
                spriteBatch.Draw(blade, NPC.Center - Main.screenPosition + offset, null, new Color(255, 255, 255) * NPC.Opacity, rotation, new Vector2(19, 64), 1f, direction, 0f);
            }
        }
    }
}
