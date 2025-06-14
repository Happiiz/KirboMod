using KirboMod.Particles;
using KirboMod.Projectiles.KrackoJrBomb;
using KirboMod.Projectiles.KrackoJrCannonball;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public class KrackoJr : ModNPC
    {
        enum KrackoJrAttackType
        {
            Spawn,
            Bombs,
            CannonBall,
            Dash
        }
        private class KrackoJrCloud
        {
            public int timeLeft;
            public Vector2 position;
            public float rotation;
            public KrackoJrCloud(Vector2 pos, float rotation)
            {
                timeLeft = 13;
                this.rotation = rotation;
                position = pos;
            }
            public void Draw(Texture2D tex)
            {
                Main.spriteBatch.Draw(tex, position - Main.screenPosition, null, Color.White * Utils.GetLerpValue(0, 4, timeLeft, true), rotation, tex.Size() / 2, 1, SpriteEffects.None, 0); ;
            }
            public static void SpawnCloud(KrackoJr kracko)
            {
                if (kracko.trail.Count < 13)
                {
                    kracko.trail.Add(new KrackoJrCloud(kracko.NPC.Center + kracko.NPC.netOffset, kracko.NPC.rotation));
                }
            }
        }
        List<KrackoJrCloud> trail = new(13);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Kracko Jr.");
            Main.npcFrameCount[NPC.type] = 1;
            // Add this in for bosses(in this case minibosses) that have a summon item, requires corresponding code in the item
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 58;
            NPC.height = 58;
            NPC.damage = 26;
            NPC.defense = 6;
            NPC.noTileCollide = true;
            NPC.lifeMax = 800;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.friendly = false;
            NPC.rarity = 2; //groom/pinky rarity
            NPC.boss = true;
            if (!Main.dedServ)//if not dedicated server
            {
                int musicSlot = MusicLoader.GetMusicSlot("KirboMod/Music/Evobyte_KrackoJr");
                Music = musicSlot;
                Main.musicFade[musicSlot] = 1;
                Main.musicNoCrossFade[musicSlot] = true;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            //if player is within space height, not in hardmode, defeated evil boss but not Kracko
            if (spawnInfo.Player.ZoneSkyHeight && NPC.downedBoss2 && !DownedBossSystem.downedKrackoBoss && !Main.hardMode) 
            {
                return (NPC.AnyNPCs(ModContent.NPCType<KrackoJr>()) || NPC.AnyNPCs(ModContent.NPCType<NPCs.Kracko>())) ? 0 : 0.05f; //return spawn rate if kracko isn't here
            }
            else
            {
                return 0f; //no spawn rate
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A Kracko that shrunk itself in size to be more mobile, but will grow and become stronger if threatened!")
            });
        }

        ref float Timer { get => ref NPC.ai[0]; }
        KrackoJrAttackType CurrentAttack { get => (KrackoJrAttackType)NPC.ai[1]; set => NPC.ai[1] = (float)value; }

        public override void AI() //constantly cycles each time
        {
            //will be used as the rotation of the cloud sprite
            NPC.rotation += .1f;

            //DESPAWNING
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
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
            Timer++;
            switch (CurrentAttack)
            {
                case KrackoJrAttackType.Spawn:
                    NPC.velocity = NPC.velocity.MoveTowards(NPC.DirectionTo(player.Center - new Vector2(0, 128)) * 30, 1);
                    NPC.velocity *= Utils.GetLerpValue(16, 256, NPC.Distance(player.Center), true);
                    if (Timer > 120)
                    {
                        CurrentAttack = KrackoJrAttackType.Bombs;
                        Timer = 0;
                    }
                    break;
                case KrackoJrAttackType.Bombs:

                    float flyEnd = 190;
                    float flyStart = 100;

                    if (Timer == 30)
                    {
                        FacePlayer();
                        Vector2 targetOffset = new Vector2(500 * NPC.spriteDirection, -300);
                        NPC.velocity = player.Center + targetOffset - NPC.Center;
                        NPC.velocity *= .177f;//these numbers are just what worked idk why
                    }
                    NPC.velocity *= .85f;

                    if (Timer >= flyStart - 10 && Timer <= flyEnd)
                    {
                        float progress = Utils.GetLerpValue(flyStart - 10, flyStart, Timer, true) * Utils.GetLerpValue(flyEnd, flyEnd - 10, Timer, true);
                        progress = Easings.EaseInOutSine(progress);
                        NPC.velocity.X = -NPC.spriteDirection * progress * 20;
                    }

                    if (CheckShouldShoot(20, 5, (int)flyStart))
                    {
                        SoundEngine.PlaySound(SoundID.Item73 with { MaxInstances = 0, Pitch = -1, PitchVariance = .15f }, NPC.Center);
                        SoundEngine.PlaySound(SoundID.Item73 with { MaxInstances = 0, Pitch = 0, PitchVariance = .15f }, NPC.Center);
                        SpawnBomb();
                    }
                    if (Timer > flyStart + 20 * 5 + GetExtraAttackWaitTime())
                    {
                        Timer = 0;
                        CurrentAttack = KrackoJrAttackType.CannonBall;
                    }
                    break;
                case KrackoJrAttackType.CannonBall:
                    NPC.damage = 0;
                    int spinStart = 10;
                    int spinDuration = 120;
                    int decelerationDuration = 8;
                    if (Timer == spinStart)
                    {
                        FacePlayer();
                    }
                    if (Timer >= spinStart && Timer < spinStart + spinDuration + decelerationDuration)
                    {
                        float progress = Utils.GetLerpValue(spinStart, spinStart + spinDuration, Timer, true);
                        float progressWithDeceleration = progress * Utils.GetLerpValue(spinStart + spinDuration + decelerationDuration, spinStart + spinDuration, Timer, true);
                        progress = Easings.EaseInOutSine(progress);
                        progressWithDeceleration = Easings.EaseInOutSine(progressWithDeceleration);
                        Vector2 offset = -Vector2.UnitY.RotatedBy(NPC.spriteDirection * progress * MathF.Tau) * MathHelper.Lerp(90, 260, progress);
                        offset.X += progress * progress * progress * progress * progress * 400 * NPC.spriteDirection;
                        NPC.Center = Vector2.Lerp(NPC.Center, player.Center + offset, progressWithDeceleration);
                    }
                    if (CheckShouldShoot(1, 1, spinStart + spinDuration + decelerationDuration))
                    {
                        SoundEngine.PlaySound(SoundID.Item45 with { Volume = 2, MaxInstances = 0, Pitch = 0f }, NPC.Center);
                        SoundEngine.PlaySound(SoundID.Item45 with { Volume = 2, MaxInstances = 0, Pitch = -.2f }, NPC.Center);
                        SoundEngine.PlaySound(SoundID.Item66 with { Volume = 2, MaxInstances = 0, Pitch = .8f }, NPC.Center);
                        SoundEngine.PlaySound(SoundID.Item66 with { Volume = 2, MaxInstances = 0, Pitch = 1f }, NPC.Center);
                        ShootCannonBalls();
                    }
                    if (Timer > spinStart + spinDuration + decelerationDuration + GetExtraAttackWaitTime())//change 14 to a higher number to increase how much cooldown between attacks
                    {
                        Timer = 0;
                        CurrentAttack = KrackoJrAttackType.Dash;
                    }
                    break;
                case KrackoJrAttackType.Dash:
                    int decelerateStart = 300;
                    int decelerateDuration = 80;
                    float maxSpeed = Utils.GetLerpValue(0, 100, Timer, true) * Utils.GetLerpValue(decelerateStart + decelerateDuration / 2f, decelerateStart, Timer, true);
                    maxSpeed = Easings.EaseInOutSine(maxSpeed) * 30;//30 is the actual speed
                    float steeringSpeed = .01f + Utils.Remap(Timer, decelerateStart + decelerateDuration, decelerateStart, .2f, 0f);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center) * maxSpeed, steeringSpeed);
                    NPC.damage = Timer < decelerateStart ? NPC.defDamage : 0;
                    if (Timer > decelerateStart + decelerateDuration + GetExtraAttackWaitTime())
                    {
                        CurrentAttack = KrackoJrAttackType.Bombs;
                        Timer = 0;
                    }
                    break;
            }


            MakeClouds();
        }

        private void FacePlayer()
        {
            NPC.spriteDirection = MathF.Sign(NPC.Center.X - Main.player[NPC.target].Center.X);
        }

        private void MakeClouds()
        {
            if ((NPC.position - NPC.oldPosition).LengthSquared() > 2 && Main.timeForVisualEffects % 4 == 0)
            {
                KrackoJrCloud.SpawnCloud(this);
            }
            for (int i = 0; i < trail.Count; i++)
            {
                KrackoJrCloud cld = trail[i];
                cld.timeLeft--;
                if (cld.timeLeft <= 0)
                {
                    trail.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Cloud, speed * 20, Scale: 3f); //Makes dust in a circle
                    d.noGravity = true;
                }

                //summon kracko
               
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Cloud, speed * 2, Scale: 1.5f); //Makes dust in a messy circle
                    d.noGravity = false;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }

        // This npc uses an additional texture for drawing
        public static Asset<Texture2D> Clouds;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Clouds = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoJrClouds");
            //Draw clouds
            Texture2D clouds = Clouds.Value;
            for (int i = trail.Count - 1; i >= 0; i--)
            {
                trail[i].Draw(clouds);
            }
            spriteBatch.Draw(clouds, NPC.Center - screenPos, null, new Color(255, 255, 255), NPC.rotation, new Vector2(55, 55), 1f, SpriteEffects.None, 0f);
            Texture2D eye = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyeBase").Value;
            Texture2D pupil = ModContent.Request<Texture2D>("KirboMod/NPCs/KrackoEyePupil").Value;
            Player player = Main.player[NPC.target];
            float offsetLength = 6.5f;
            Vector2 pupilOffset = Vector2.Normalize(player.Center - NPC.Center) * offsetLength;//the multipier is just what looks good

            if (NPC.IsABestiaryIconDummy)
            {
                pupilOffset = (Main.MouseScreen - NPC.Center);//the multipier is just what looks good
                if(pupilOffset.Length() > offsetLength)
                {
                    pupilOffset.Normalize();
                    pupilOffset *= offsetLength;
                }
            }
            pupilOffset /= 2;
            pupilOffset = pupilOffset.Floor() * 2 + Vector2.One;
            spriteBatch.Draw(eye, NPC.Center - screenPos, null, new Color(255, 255, 255), 0, new Vector2(29, 29), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(pupil, NPC.Center - screenPos + pupilOffset, null, Color.White, 0, pupil.Size() / 2, 1, SpriteEffects.None, 0);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.rotation += .1f;
                //MakeClouds();
            }

        }

        bool CheckShouldShoot(int fireRate, int numberOfShots, int start)
        {
            return (NPC.ai[0] - start) % fireRate == 0 && NPC.ai[0] < (start + fireRate * numberOfShots) && NPC.ai[0] >= start;
        }
        void ShootCannonBalls()
        {
            float numCannonballs = 5;
            if (Main.expertMode)
            {
                numCannonballs += 2;
            }
            if (Main.getGoodWorld)
            {
                numCannonballs += 2;
            }

            float totalSpread = 2.5f;
            float shootSpeed = 7;
            if (Main.getGoodWorld)
            {
                shootSpeed *= 1.15f;
            }
            if (Main.expertMode)
            {
                shootSpeed *= 1.5f;
            }
            Player plr = Main.player[NPC.target];
            for (float i = 0; i < numCannonballs; i ++)
            {
                float t = i / (numCannonballs - 1);
                Vector2 velocity = NPC.DirectionTo(plr.Center);
                velocity = velocity.RotatedBy(MathHelper.Lerp(-totalSpread / 2, totalSpread / 2, t)) * shootSpeed;
                for (int j = 0; j < 20; j++)
                {
                    Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(32, 32), DustID.Asphalt, velocity.RotatedByRandom(.1f) * Utils.Remap(j, 0, 20, 3, 6), 0, default, 1.4f);
                    dust.noGravity = true;
                    dust.velocity *= Main.rand.NextFloat() * .2f + .2f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<KrackoJrCannonball>(), 26 / 2, 0);
            }
        }
        int GetExtraAttackWaitTime()
        {
            if (Main.getGoodWorld)
                return 0;
            if (Main.expertMode)
                return NPC.GetLifePercent() < .5f ? 20 : 10;
            return 40;
        }
        void SpawnBomb()
        {
            Sparkle[] sparkles = Sparkle.EyeShine(NPC.Center, Color.Orange, duration: 14);
            foreach (Sparkle sparkle in sparkles)
            {
                sparkle.scale *= 2f;
                sparkle.fatness.Y *= 3;
                sparkle.fadeOutTime = 4;
            }
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player plr = Main.player[i];
                if (plr.dead || !plr.active)
                {
                    continue;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<KrackoJrBomb>(), 30 / 2, 0, Main.myPlayer, plr.whoAmI);
                }
            }
        }
        public override bool CheckDead()
        {
            int boss = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 3, ModContent.NPCType<NPCs.Kracko>(), 0, 0, 0, 0, 0, NPC.target);
            //TEST IF NEEDS SYNCNPC MESSAGE TO WORK ON MP
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Main.npc[boss].TypeName), 175, 75);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[boss].GetTypeNetName()), new Color(175, 75, 255));
            }
            NPC.active = false;
            return false;
        }
    }
}
