using KirboMod.Projectiles.NightmareLightningOrb;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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

        ref float deathCounter { get => ref NPC.ai[2]; }

        int tpEffectCounter = 12;
        Vector2 tpEffectPos;

        public override void AI() //constantly cycles each time
        {
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction; //face whatever direction

            tpEffectCounter++;

            //Despawn

            if (deathCounter > 0)
            {
                DoDeathAnimation();
            }
            else if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active || Main.dayTime) //Despawn
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
                    NPC.Center += new Vector2(0, -2000);
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
                NightmareAttackType.Swoop, NightmareAttackType.Tornado, NightmareAttackType.Stoop, NightmareAttackType.LightningOrbsPentagon, NightmareAttackType.LightningOrbsHoming };

            possibleAttacks.Remove(lastattacktype);

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
            else
            {
                if (NPC.ai[0] == 120) //beginning of cycle
                {
                    animation = 0;

                    NPC.damage = 0; //reset

                    NPC.velocity *= 0.01f; //slow

                    NPC.TargetClosest(false);

                    AttackDecideNext();

                    if (NPC.GetLifePercent() < 0.5f)
                    {
                        phase = 2; //change phase
                    }
                }

                switch (attacktype)
                {
                    case NightmareAttackType.SpreadStars:
                        if (phase == 1)
                        {
                            AttackSpreadStars(NPC.ai[0] - 120, player);
                        }
                        else
                        {
                            EnrageSpreadStars(NPC.ai[0] - 120, player);
                        }
                        break;
                    case NightmareAttackType.RingStars:
                        if (phase == 1)
                        {
                            AttackRingStars(NPC.ai[0] - 120, player);
                        }
                        else
                        {
                            EnrageRingStars(NPC.ai[0] - 120, player);
                        }
                        break;
                    case NightmareAttackType.Swoop:
                        if (phase == 1)
                        {
                            AttackSwoop(NPC.ai[0] - 120, player);
                        }
                        else
                        {
                            EnrageSwoop(NPC.ai[0] - 120, player);
                        }
                        break;
                    case NightmareAttackType.Tornado:
                        if (phase == 1)
                        {
                            AttackTornado(NPC.ai[0] - 120, player);
                        }
                        else
                        {
                            EnrageTornado(NPC.ai[0] - 120, player);
                        }
                        break;
                    case NightmareAttackType.Stoop:
                        if (phase == 1)
                        {
                            AttackStoop(NPC.ai[0] - 120, player);
                        }
                        else
                        {
                            EnrageStoop(NPC.ai[0] - 120, player);
                        }
                        break;
                    case NightmareAttackType.LightningOrbsPentagon:
                        AttackLightningOrbPentagon(NPC.ai[0] - 120, player);
                        break;
                    case NightmareAttackType.LightningOrbsHoming:
                        AttackLightningOrbHoming(NPC.ai[0] - 120, player);
                        break;
                }
            }

            if (tpEffectCounter <= 12) //teleporting
            {
                animation = 2;
            }
        }

        private void AttackLightningOrbPentagon(float timer, Player player)
        {
            int firerate = 30;
            int numOrbs = 5;
            int start = 60;
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -100) + player.velocity * (start + 10));
            if (timer > start)
            {
                animation = 5; //damageable
            }

            if (CheckShouldShoot(firerate, numOrbs, start) && NetmodeID.MultiplayerClient != Main.netMode)
            {
                NightmareLightningOrb.GetShootStats(firerate, numOrbs, start, (int)timer, NPC.target, out float ai0, out float ai1, out float ai2, out Vector2 velocity);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<NightmareLightningOrb>(), 40, 0, -1, ai0, ai1, ai2);
            }
            if (timer > firerate * (numOrbs + 1))
            {
                animation = 0;
            }
            float extraTime = 500;
            if (Main.expertMode)
                extraTime *= 0.65f;
            if (Main.getGoodWorld)
                extraTime = 0;
            if (timer > firerate * numOrbs + extraTime)
            {
                NPC.ai[0] = 119;
            }
        }

        private void AttackLightningOrbHoming(float timer, Player player)
        {
            int firerate = 40;
            int numOrbs = 4;
            int start = 60;
            float extraTime = 200;
            if (Main.expertMode)
                extraTime *= 0.65f;
            if (Main.getGoodWorld)
                extraTime = 0;
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -100) + player.velocity * (start + 5));
            if (timer > start)
            {
                animation = 5; //damageable
            }

            if (CheckShouldShoot(firerate, numOrbs, start) && NetmodeID.MultiplayerClient != Main.netMode)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, default, ModContent.ProjectileType<NightmareLightningOrbHoming>(), 40, 0, -1, NPC.target, NightmareLightningOrbHoming.GetMaxSpeed(firerate, numOrbs, start, (int)timer));
            }
            if (timer > firerate * numOrbs)
            {
                animation = 0;
            }
            if (timer > firerate * numOrbs + extraTime)
            {
                NPC.ai[0] = 119;
            }
        }

        private void AttackSpreadStars(float timer, Player player) //NPC.ai[0] subtracted by 60 to make counting more simple
        {
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            bool phase2 = phase != 1;
            int delayBeforeSlide = phase2 ? /*20 : 30*/ 60 : 90;
            int slideDuration = 30;
            int extraWaitTime = phase2 ? 70 : 100;
            if (Main.getGoodWorld)
                extraWaitTime = 0;
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -100) + player.velocity * (delayBeforeSlide + 5));
            if (timer == delayBeforeSlide) //move to side
            {
                animation = 1;
                if (player.Center.X < NPC.Center.X)
                {
                    NPC.velocity.X = 20;
                    NPC.direction = -1;
                }
                else
                {
                    NPC.velocity.X = -20;
                    NPC.direction = 1;
                }
            }
            if (timer >= delayBeforeSlide + slideDuration)
            {
                animation = 3; //damageable
                NPC.velocity *= 0.9f;

                //spawn on top of hand
                Vector2 startpos = NPC.Center + new Vector2(NPC.direction * -50, -90);
                int firerate = Main.expertMode ? 9 : 12;
                if (phase2)
                {
                    firerate = (int)(firerate * 0.8f);
                }
                Vector2 direction = player.Center - startpos;
                direction.Normalize();
                direction *= phase2 ? 30 : 25;


                if (timer % (firerate * 2) == firerate)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), startpos, direction,
                        ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);
                }
                else if (timer % (firerate * 2) == 0)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), startpos, direction.RotatedBy(-(MathF.PI / 8)),
                            ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), startpos, direction.RotatedBy(MathF.PI / 8),
                            ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);
                }
            }

            if (timer >= delayBeforeSlide + slideDuration + extraWaitTime)
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
                if (timer > 80 && timer % 20 == 0)
                {
                    int starsInRing = Main.expertMode ? 15 : 9;

                    for (float i = 0; i < starsInRing; i += 1f)
                    {
                        float angle = ((timer - 60) * 2f) + (i * (MathF.PI / 6));

                        Vector2 vel = new Vector2(MathF.Cos(angle) * 15, MathF.Sin(angle) * 15);

                        SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel,
                            ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);
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

            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));

            if (timer >= 60)
            {
                animation = 4; //swoop

                if (timer == 60)
                {
                    if (playerDistance.X <= 0) //if player is right of enemy
                    {
                        NPC.velocity.X = 30f;
                    }
                    else
                    {
                        NPC.velocity.X = -30f;
                    }

                    NPC.TargetClosest(true); //face player only for inital backing up
                }

                NPC.velocity.X += NPC.direction * 1f;

                float speed = 20f;
                float inertia = 15f;

                playerDistance.Normalize();
                playerDistance *= speed;
                NPC.velocity.Y = (NPC.velocity.Y * (inertia - 1) + playerDistance.Y) / inertia;
            }

            if (timer > 180)
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

                    float speed = 30;
                    float inertia = 60;

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
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));

            if (timer >= 12)
            {
                animation = 7;

                if (timer < 60) //rise
                {
                    //hover over player
                    Vector2 playerDistance = player.Center + new Vector2(0, -400) - NPC.Center;

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

        //EXPERT LOW HEALTH ATTACKS

        private void EnrageSpreadStars(float timer, Player player)
        {
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));

            if (timer == 40) //move to side
            {
                animation = 1;
                if (NPC.Center.X > player.Center.X)
                {
                    NPC.velocity.X = 20;
                    NPC.direction = -1;
                }
                else
                {
                    NPC.velocity.X = -20;
                    NPC.direction = 1;
                }
            }
            if (timer >= 90)
            {
                animation = 3; //damageable
                NPC.velocity *= 0.9f;

                //spawn on top of hand
                Vector2 startpos = NPC.Center + new Vector2(NPC.direction * -50, -100);

                Vector2 direction = player.Center - startpos;

                direction.Normalize();
                direction *= 25;


                if (timer % 8 == 0 && timer % 16 != 0)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), startpos, direction,
                        ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);
                }

                if (timer % 16 == 0)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), startpos, direction.RotatedBy(-(MathF.PI / 9)),
                            ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), startpos, direction.RotatedBy(MathF.PI / 9),
                            ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);
                }
            }

            if (timer >= 210)
            {
                NPC.ai[0] = 119; //restart
            }
        }

        private void EnrageRingStars(float timer, Player player)
        {
            //teleport
            if (timer < 240)
            {
                //teleport throughout attack
                if (timer % 40 == 0 || timer == 1)
                {
                    NPC.ai[1] = timer; //time to start teleport
                }
                Teleport(timer - NPC.ai[1], player, new Vector2(Main.rand.Next(-300, 300), -400));

                animation = 5; //damageable

                if ((timer + 20) % 40 == 0)
                {
                    for (float i = 0; i < 13; i += 1f)
                    {
                        float angle = ((timer - 60) * 2f) + (i * (MathF.PI / 6));

                        Vector2 vel = new Vector2(MathF.Cos(angle) * 10, MathF.Sin(angle) * 10);

                        SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel,
                            ModContent.ProjectileType<Projectiles.BadStar>(), 50 / 2, 6);
                    }
                }
            }

            if (timer >= 300)
            {
                NPC.ai[0] = 119; //restart
            }
        }

        private void EnrageSwoop(float timer, Player player)
        {

            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            if (timer < 60)
            {
                Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));
            }
            float side = Main.rand.NextBool() == true ? 1 : -1;

            //teleport again
            if (timer == 60)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            Teleport(timer - NPC.ai[1], player, new Vector2(side * 600, 0));

            if (timer > 60)
            {
                Vector2 playerDistance = player.Center - NPC.Center;

                animation = 4; //swoop

                if (timer == 61)
                {
                    if (playerDistance.X <= 0) //if player is right of enemy
                    {
                        NPC.velocity.X = 20f;
                    }
                    else
                    {
                        NPC.velocity.X = -20f;
                    }

                    NPC.TargetClosest(true); //face player only for inital backing up
                }

                NPC.velocity.X += NPC.direction * 1f;

                float speed = 20f;
                float inertia = 15f;

                playerDistance.Normalize();
                playerDistance *= speed;
                NPC.velocity.Y = (NPC.velocity.Y * (inertia - 1) + playerDistance.Y) / inertia;


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

            if (timer > 180)
            {
                NPC.velocity *= 0.01f;

                NPC.ai[0] = 119; //restart
            }
        }

        private void EnrageTornado(float timer, Player player)
        {
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            if (timer < 30)
            {
                Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));
            }
            else if (timer >= 30) //initiate animation
            {
                animation = 6;

                if (timer >= 60) //glide towards player
                {
                    Vector2 playerDistance = player.Center - NPC.Center;

                    float speed = 30;
                    float inertia = 50;

                    playerDistance.Normalize();
                    playerDistance *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + playerDistance) / inertia;

                    if ((timer % 120 == 0) && timer != 120) //every 120 excluding 120
                    {
                        NPC.ai[1] = timer; //time to start teleport
                        NPC.velocity *= -1;
                    }

                    Teleport(timer - NPC.ai[1], player, Vector2.Normalize(NPC.velocity) * 500 + player.velocity * (500 / speed));
                    //teleport 'round a circle
                }
            }

            if (timer >= 480)
            {
                NPC.velocity *= 0.01f;

                NPC.ai[0] = 119; //restart
            }
        }

        private void EnrageStoop(float timer, Player player)
        {
            //teleport 
            if (timer == 1)
            {
                NPC.ai[1] = timer; //time to start teleport
            }
            if (timer < 12)
            {
                Teleport(timer - NPC.ai[1], player, new Vector2(0, -200));
            }
            else if (timer >= 12)
            {
                animation = 7;

                if (timer < 60) //follow
                {
                    //hover over player
                    Vector2 playerDistance = player.Center + new Vector2(0, -400) - NPC.Center;

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

                //AGAIN!

                //teleport 
                if (timer == 120)
                {
                    NPC.ai[1] = timer; //time to start teleport
                }
                Teleport(timer - NPC.ai[1], player, new Vector2(-300, 0));

                if (timer >= 132)
                {
                    animation = 7;

                    if (timer < 180) //follow but with slightly more time to escape
                    {
                        //hover beside player
                        Vector2 playerDistance = player.Center + new Vector2(-400, 0) - NPC.Center;
                        NPC.rotation = -MathF.PI / 2; //facing right

                        float speed = 15f;
                        float inertia = 10f;

                        playerDistance.Normalize();
                        playerDistance *= speed;
                        NPC.velocity = (NPC.velocity * (inertia - 1) + playerDistance) / inertia;
                    }
                    else if (timer < 270)
                    {
                        NPC.velocity.X *= 0.9f; //slow while stooping

                        NPC.velocity.Y *= 0.01f;

                        if (timer == 180) //stoop
                        {
                            NPC.velocity.X = (player.Center.X - NPC.Center.X) / 5; //distance depends on player distance

                            //caps

                            if (NPC.velocity.X < 30)
                            {
                                NPC.velocity.X = 30;
                            }

                            if (NPC.velocity.X > 60)
                            {
                                NPC.velocity.X = 60;
                            }
                        }
                    }
                }
            }

            if (timer >= 270)
            {
                NPC.rotation = 0;
                NPC.ai[0] = 119; //restart
            }
        }

        private void Teleport(float timer, Player player, Vector2 location)
        {
            if (timer == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) //have this here because teleport is random sometimes
                {
                    tpEffectPos = NPC.Center;
                }

                tpEffectCounter = 4;//animSpeed

                NPC.Center = player.Center + location;
                NPC.frameCounter = 0;

                NPC.TargetClosest(false); //just in case
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
            Rectangle frame = sheet.Frame(1, Main.npcFrameCount[NPC.type], 0, frameY);
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);
            Main.EntitySpriteDraw(sheet, tpEffectPos - Main.screenPosition, frame, Color.White, 0, origin, 1f, SpriteEffects.None);
        }
        public override bool CheckDead()
        {
            if (deathCounter < 360)
            {
                NPC.active = true;
                NPC.life = 1;
                deathCounter += 1; //go up
                return false;
            }
            return true;
        }

        private void DoDeathAnimation()
        {
            NPC.ai[0] = 0; //don't attack
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.active = true;
            NPC.velocity *= 0.01f;
            NPC.rotation = 0;

            if (deathCounter % 10 == 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit2, NPC.Center);
            }

            deathCounter++; //go up

            Vector2 speed = Main.rand.NextVector2Circular(40f, 40f); //circle
            Dust d = Dust.NewDustPerfect(NPC.Center, DustID.DemonTorch, speed, Scale: 2f); //Makes dust in a messy circle
            d.noGravity = true;

            if (deathCounter < 120)
            {
                animation = 9;
            }
            else if (deathCounter < 240)
            {
                animation = 10;
            }
            else if (deathCounter < 360)
            {
                animation = 11;
            }
            else
            {
                NPC.SimpleStrikeNPC(999999, 1, false, 0, null, false, 0, false);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            TpEffectDraw();

            return true;
        }
        bool CheckShouldShoot(int fireRate, int numberOfShots, int start)
        {
            int timer = (int)NPC.ai[0] - 120;
            return (timer - start) % fireRate == 0 && timer < (start + fireRate * numberOfShots) && timer >= start;
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

                NPC.damage = NPC.defDamage; //set to inital damage
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

            if (animation == 9) // death 1
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = frameHeight * 19;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 20;
                }
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                }
            }
            if (animation == 10) // death 2
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = frameHeight * 21;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 22;
                }
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                }
            }
            if (animation == 11) // death 3
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = frameHeight * 23;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 24;
                }
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                }
            }
        }
    }
}