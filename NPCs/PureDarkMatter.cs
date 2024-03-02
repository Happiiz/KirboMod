using KirboMod.Dusts;
using KirboMod.Projectiles;
using KirboMod.Projectiles.Lightnings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    [AutoloadBossHead]
    public partial class PureDarkMatter : ModNPC
    {
        private int phase = 1;

        private Vector2 spot = new Vector2(0, 0);

        private int attackTurn = 2; //start at two so last expert phase starts on spin move

        private DarkMatterAttackType attacktype = DarkMatterAttackType.Petals;

        private DarkMatterAttackType lastattacktype = DarkMatterAttackType.Lasers;



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

                NPC.velocity.Y = NPC.velocity.Y - 0.2f;

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
            Player player = Main.player[NPC.target];
            Vector2 moveTo = player.Center;
            Vector2 playerDistance = player.Center - NPC.Center;
            Vector2 move = player.Center - NPC.Center;

            NPC.ai[0]++;

            NPC.spriteDirection = NPC.direction;

            if (NPC.ai[0] < 30) //rise up gang
            {
                NPC.velocity.Y = -3;

                if (NPC.ai[0] == 1) //inital frame
                {
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(20, 20); //circle edge
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed); //Makes dust in a messy circle
                        d.noGravity = true;

                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center); //OOooAAAHHRrrr
                    }
                }
            }
            else //main loop
            {
                if (NPC.ai[0] == 30)
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

                        //Spin Move Percent
                        if (NPC.GetLifePercent() < 0.25f && Main.expertMode)
                        {
                            phase = 4;
                        }
                    }
                }
                else if (NPC.ai[0] > 30)
                {
                    if (attacktype == DarkMatterAttackType.Petals)
                    {
                        if (phase >= 3)
                        {
                            EnragePetals();
                        }
                        else
                        {
                            AttackPetals();
                        }
                    }
                    if (attacktype == DarkMatterAttackType.Dash)
                    {
                        if (phase >= 3)
                        {
                            EnrageDash();
                        }
                        else
                        {
                            AttackDash();
                        }
                    }
                    if (attacktype == DarkMatterAttackType.Lasers)
                    {
                        if (phase >= 3)
                        {
                            EnrageLasers();
                        }
                        else
                        {
                            AttackLasers();
                        }
                    }
                    if (attacktype == DarkMatterAttackType.Spin)
                    {
                        AttackSpin();
                    }
                }
            }
        }

        void AttackDecideNext()
        {
            List<DarkMatterAttackType> possibleAttacks = new() { DarkMatterAttackType.Petals, DarkMatterAttackType.Dash, DarkMatterAttackType.Lasers };

            possibleAttacks.Remove(lastattacktype);

            if (attackTurn > 1 && phase == 4) //below 25% on expert mode and did 2 attacks
            {
                attacktype = DarkMatterAttackType.Spin; //spin move
                attackTurn = 0; //reset
            }
            else
            {
                attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
                attackTurn += 1; //add every turn 

                NPC.netUpdate = true;
                lastattacktype = attacktype;
            }
        }

        void AttackPetals()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;
            NPC.TargetClosest();

            //deciding which side
            float xOffset;
            if (playerDistance.X <= 0) //if player is behind enemy
            {
                xOffset = 400; // go in front of player 
            }
            else
            {
                xOffset = -400; // go behind player
            }

            //movement
            Vector2 playerXOffest = player.Center + new Vector2(xOffset, 0f); //go ahead of player
            Vector2 move = playerXOffest - NPC.Center;


            float speed = 20f;
            float inertia = 10f;

            move.Normalize();
            move *= speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;

            //main attack
            if (phase == 1)
            {
                if (NPC.ai[0] % 15 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.direction * -10, Main.rand.Next(-10, 10)),
                        ModContent.ProjectileType<MatterOrb>(), 60 / 2, 4, default, 0, player.whoAmI, NPC.ai[0] / 60f);
                    }
                    SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center); //blub
                }
            }
            else
            {
                if (NPC.ai[0] % 10 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.direction * -10, Main.rand.Next(-10, 10)),
                        ModContent.ProjectileType<MatterOrb>(), 60 / 2, 4, default, 0, player.whoAmI, NPC.ai[0] / 60f);
                    }
                    SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center); //blub blub
                }
            }

            //reset
            if (NPC.ai[0] >= 270 && phase == 1)
            {
                NPC.ai[0] = 29;
            }
            else if (NPC.ai[0] >= 360)
            {
                NPC.ai[0] = 29;
            }
        }

        void AttackDash()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;

            //set dash amount
            if (NPC.ai[0] == 31 && phase != 1)
            {
                NPC.ai[1] = 2; //actually 3
            }

            if (NPC.ai[0] < 150) //follow predicted player y for 120 ticks
            {
                NPC.TargetClosest(); //face player only for 60 ticks

                if (NPC.ai[0] % 10 == 0) //little dust to warn player
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
                }

                //deciding which side
                float xOffset = 400;

                if (playerDistance.X <= 0) //if player is behind enemy
                {
                    xOffset = 500; // go in front of player 
                }
                else
                {
                    xOffset = -500; // go behind player
                }
                //movement
                Vector2 playerXOffest = player.Center + new Vector2(xOffset + ((NPC.ai[0] - 30) * -NPC.direction * 5), player.velocity.Y * 20); //go ahead of player and backup a bit
                Vector2 move = playerXOffest - NPC.Center;

                float speed = 60f;
                float inertia = 10f;

                move.Normalize();
                move *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;
            }
            else if (NPC.ai[0] < 180) //go forth
            {
                NPC.velocity = new Vector2(NPC.velocity.X + (2.5f * NPC.direction), 0f);

                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());

                if (NPC.ai[0] == 150)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center); //OOooAAAHHRrrr
                }
            }
            else //slow
            {
                NPC.velocity.X *= 0.95f;
            }

            if (NPC.ai[0] >= 210)
            {
                if (NPC.ai[1] > 0) //can dash again
                {
                    NPC.ai[1] -= 1;
                    NPC.ai[0] = 90; //restart from attack beginning
                }
                else
                {
                    NPC.ai[0] = 29; //restart
                    NPC.ai[1] = 0;
                }
            }
        }

        void AttackLasers()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;

            NPC.TargetClosest();

            //deciding which side
            float xOffset = 400;

            if (playerDistance.X <= 0) //if player is behind enemy
            {
                xOffset = 500; // go in front of player 
            }
            else
            {
                xOffset = -500; // go behind player
            }

            //movement
            Vector2 playerXOffest = player.Center + new Vector2(xOffset, MathF.Sin((NPC.ai[0] - 30) / 30) * 300); //go ahead of player
            Vector2 move = playerXOffest - NPC.Center;

            NPC.rotation = MathHelper.ToRadians(MathF.Sin((NPC.ai[0] - 30) / 30) * 60);

            float speed = 10f;
            float inertia = 10f;

            move.Normalize();
            move *= speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;

            Vector2 turned = NPC.direction == 1 ? NPC.rotation.ToRotationVector2() : NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(180));
            Vector2 posOffset = turned * 45;
            turned *= 20;
            //main attack
            if (NPC.ai[0] >= 90)
            {
                if (phase == 1)
                {
                    if (NPC.ai[0] % 40 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            LightningProj.GetSpawningStats(turned, out float ai0, out float ai1);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + posOffset, turned,
                                ModContent.ProjectileType<DarkMatterLaser>(), 60 / 2, 4, default, ai0, ai1);
                        }
                        SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
                    }
                }
                else
                {
                    if (NPC.ai[0] % 20 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            LightningProj.GetSpawningStats(turned, out float ai0, out float ai1);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + posOffset, turned,
                                ModContent.ProjectileType<DarkMatterLaser>(), 60 / 2, 4, default, ai0, ai1);
                        }
                        SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
                    }
                }
            }

            Vector2 offset = new Vector2(45, 0).RotatedBy(NPC.rotation) * NPC.direction;

            Dust.NewDustPerfect(NPC.Center + offset, DustID.VilePowder, Main.rand.NextVector2Circular(10, 10));

            //reset
            if (NPC.ai[0] >= 400)
            {
                NPC.rotation = 0;
                NPC.ai[0] = 29;
            }
        }

        //ENRAGED

        void EnragePetals()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;

            //set dash amount
            if (NPC.ai[0] == 31)
            {
                NPC.ai[1] = 3;
            }

            if (NPC.ai[0] < 90) //follow predicted player y for 120 ticks
            {
                NPC.TargetClosest(); //face player only for 60 ticks

                if (NPC.ai[0] % 10 == 0) //little dust to warn player
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
                }

                //deciding which side
                float xOffset = 400;

                if (playerDistance.X <= 0) //if player is behind enemy
                {
                    xOffset = 500; // go in front of player 
                }
                else
                {
                    xOffset = -500; // go behind player
                }

                float yoffset = 300;

                if (NPC.ai[1] % 2 == 0) //check if even
                {
                    yoffset = -300; //go up
                }

                //movement
                Vector2 playerXOffest = player.Center + new Vector2(xOffset + ((NPC.ai[0] - 30) * -NPC.direction * 5), yoffset); //go ahead of player and backup a bit
                Vector2 move = playerXOffest - NPC.Center;

                float speed = 60f;
                float inertia = 10f;

                move.Normalize();
                move *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;
            }
            else if (NPC.ai[0] < 110) //go forth
            {
                NPC.velocity = new Vector2(80f * NPC.direction, 0f);

                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());

                if (NPC.ai[0] == 90)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center); //OOooAAAHHRrrr

                    for (int i = 0; i < 20; i++) //Dust in an arc behind Dark Matter
                    {
                        Dust.NewDustPerfect(NPC.Center, ModContent.DustType<DarkResidue>(), Main.rand.Next(5, 10) * -NPC.direction * Main.rand.NextVector2Unit(-45, MathHelper.ToRadians(90)));
                    }
                }

                if (NPC.ai[0] % 2 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<Projectiles.MatterOrbVertical>(), 60 / 2, 4, default, 0, NPC.ai[1] % 2 == 0 ? 0 : 1);
                    }
                    SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center); //blub blub blub
                }
            }

            if (NPC.ai[0] >= 110)
            {
                if (NPC.ai[1] > 0) //can dash again
                {
                    NPC.ai[1] -= 1;

                    NPC.velocity.X *= 0.01f;

                    NPC.ai[0] = 32;
                }
                else
                {
                    NPC.ai[0] = 29; //restart
                    NPC.ai[1] = 0;
                }
            }
        }

        void EnrageDash()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;

            //set dash amount
            if (NPC.ai[0] == 31)
            {
                NPC.ai[1] = 7;
            }

            NPC.direction = 1;

            if (NPC.ai[0] < 120) //charge up dash
            {
                if (NPC.ai[0] % 10 == 0) //little dust to warn player
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
                }

                NPC.rotation = playerDistance.ToRotation() + MathHelper.ToRadians(MathF.Sin(Main.GlobalTimeWrappedHourly * 5) * 45);

                NPC.velocity *= 0.9f;
            }
            else if (NPC.ai[0] < 170) //go forth
            {
                if (NPC.ai[0] == 120)
                {
                    NPC.rotation = playerDistance.ToRotation();

                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center); //pitched roar

                    for (int i = 0; i < 20; i++) //Dust in an arc behind Dark Matter
                    {
                        Dust.NewDustPerfect(NPC.Center, ModContent.DustType<DarkResidue>(), Main.rand.Next(5, 10) * -Main.rand.NextVector2Unit(NPC.rotation + MathHelper.ToRadians(-45), MathHelper.ToRadians(90)));
                    }

                    Vector2 move = playerDistance;
                    move.Normalize();
                    NPC.velocity = move * 40;
                }

                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
            }

            if (NPC.ai[0] >= 170)
            {
                if (NPC.ai[1] > 0) //can dash again
                {
                    NPC.ai[1] -= 1;

                    NPC.ai[0] = 119;
                }
                else
                {
                    NPC.ai[0] = 29; //restart
                    NPC.ai[1] = 0;
                    NPC.rotation = 0;
                }
            }
        }

        void EnrageLasers()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;

            NPC.TargetClosest();

            //deciding which side
            float xOffset;

            if (playerDistance.X <= 0) //if player is behind enemy
            {
                xOffset = 500; // go in front of player 
            }
            else
            {
                xOffset = -500; // go behind player
            }

            float interval = 40;

            if (NPC.ai[0] > 150)
            {
                interval = 30;
            }
            if (NPC.ai[0] > 240)
            {
                interval = 20;
            }

            if (NPC.ai[0] == 31) //move intially
            {
                spot = player.Center + new Vector2(xOffset, 0); //go ahead of player
            }

            Vector2 move = spot - NPC.Center;

            float speed = 60f;
            float inertia = 5f;

            move.Normalize();
            move *= speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;

            float tradjectory = MathHelper.ToRadians(45);

            //main attack
            if (NPC.ai[0] % interval == 0 && NPC.ai[0] >= 90 && NPC.ai[0] < 440)
            {
                if (NPC.ai[1] == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -2; i < 1; i++)
                        {
                            tradjectory = MathHelper.ToRadians(45 * i);
                            Vector2 vel = NPC.direction * tradjectory.ToRotationVector2().RotatedBy(MathHelper.ToRadians(45)) * 20;

                            LightningProj.GetSpawningStats(vel, out float ai0, out float ai1);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 45, 0), vel, ModContent.ProjectileType<DarkMatterLaser>(), 60 / 2, 4, -1, ai0, ai1);
                        }
                    }
                    NPC.ai[1] = 1;
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -1; i < 1; i++)
                        {
                            tradjectory = MathHelper.ToRadians(-22.5f + (45 * i));
                            Vector2 vel = NPC.direction * tradjectory.ToRotationVector2().RotatedBy(MathHelper.ToRadians(45)) * 20;
                            LightningProj.GetSpawningStats(vel, out float ai0, out float ai1);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 45, 0), vel,
                                ModContent.ProjectileType<DarkMatterLaser>(), 60 / 2, 4, Main.myPlayer, ai0, ai1);
                        }
                    }
                    NPC.ai[1] = 0;
                }

                spot = player.Center + new Vector2(xOffset, 0); //move ahead of player

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }

            Vector2 offset = new Vector2(45, 0).RotatedBy(NPC.rotation) * NPC.direction;

            Dust.NewDustPerfect(NPC.Center + offset, DustID.VilePowder, Main.rand.NextVector2Circular(10, 10));

            //reset
            if (NPC.ai[0] >= 480)
            {
                NPC.ai[0] = 29;
            }
        }

        private void AttackSpin() //"...Then I get it with my spin move"
        {
            Player player = Main.player[NPC.target];
            float speed = 30f;
            float inertia = 10f;

            Vector2 move = player.Center + new Vector2(0, -200) - NPC.Center; //move above

            float rotationSpeed = 2.8f;
            float rotationDuration = 720 / rotationSpeed;//2 full spins
            NPC.direction = -1;

            if (NPC.ai[0] < 120)
            {
                NPC.rotation = MathHelper.ToRadians(90);

                move.Normalize();
                move *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;
            }
            else if (NPC.ai[0] < 240)
            {
                if (NPC.ai[0] < 180) //full rotation once
                {
                    NPC.rotation -= MathHelper.ToRadians(12f);
                }

                NPC.velocity *= 0.01f;
            }
            else if (NPC.ai[0] < 240 + rotationDuration)
            {
                NPC.rotation -= MathHelper.ToRadians(rotationSpeed);

                Vector2 velocity = NPC.rotation.ToRotationVector2() * 30;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - velocity, -velocity, ModContent.ProjectileType<AngledDarkBeam>(), 60 / 2, 4, Main.myPlayer);

                SoundEngine.PlaySound(SoundID.Item33, NPC.Center); //boss laser
            }
            else if (NPC.ai[0] > 240 + rotationDuration + 50) //reset with cooldown of 50 frames after finishing attack
            {
                NPC.rotation = 0;
                NPC.ai[0] = 29;
            }
        }

        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 8.0)
            {
                NPC.frame.Y = 0; //idle
            }
            else if (NPC.frameCounter < 16.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 24.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 32.0)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else
            {
                NPC.frameCounter = 0.0; //reset
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }


        bool ShouldDrawLaserTelegraph()
        {
            if (attacktype == DarkMatterAttackType.Lasers)
            {
                return (phase >= 3 && NPC.ai[0] < 440) || (phase < 3);
            }
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!ShouldDrawLaserTelegraph())
            {
                return;
            }

            Texture2D telegraph = TextureAssets.Extra[ExtrasID.FairyQueenLance].Value;//the texture used for eol's ethereal lance telegraph
            Vector2 origin = new Vector2(0, telegraph.Height / 2);
            Vector2 drawPos = NPC.Center + new Vector2(NPC.direction * 45, 0) - Main.screenPosition;
            float direction;
            float extraRot = NPC.direction == -1 ? 180 : 0;
            if (phase >= 3)
            {
                if (NPC.ai[1] == 0)
                {
                    for (int i = -2; i < 1; i++)
                    {
                        direction = MathHelper.ToRadians(45 * i + 45 + extraRot);
                        Main.EntitySpriteDraw(telegraph, drawPos, null, Color.Purple, direction, origin, 4, SpriteEffects.None);
                    }
                    return;
                }
                for (int i = -1; i < 1; i++)
                {
                    direction = MathHelper.ToRadians(-22.5f + (45 * i) + 45 + extraRot);
                    Main.EntitySpriteDraw(telegraph, drawPos, null, Color.Purple, direction, origin, 4, SpriteEffects.None);

                }
                return;
            }
            direction = NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathF.PI;
            drawPos = NPC.Center + direction.ToRotationVector2() * 45 - Main.screenPosition;
          
            Main.EntitySpriteDraw(telegraph, drawPos, null, Color.Purple, direction, origin, 4, SpriteEffects.None);

        }
    }

}
