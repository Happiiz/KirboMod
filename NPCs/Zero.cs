using KirboMod.Bestiary;
using KirboMod.Items.Zero;
using KirboMod.Projectiles;
using KirboMod.Projectiles.ZeroDashHitbox;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    [AutoloadBossHead]
    public class Zero : ModNPC
    {
        public enum ZeroAttackType : byte
        {
            DecideNext,//0
            BloodShots,//1
            Dash,//2
            DarkMatterShots,//3
            Sparks,//4
            ThornTail,//5
            BackgroundShots,//6
        }
        public static bool calamityEnabled;
        static int AttackCooldown => Main.expertMode ? 20 : 100;
        private short animation = 1; //frame cycles

        private ZeroAttackType lastattacktype = ZeroAttackType.DecideNext; //sets last attack type
        public ZeroAttackType attacktype = ZeroAttackType.DecideNext; //decides the attack
                                                                      //public to check zero's dash
        private float backupoffset = 0; //offset goto position when backing up

        static int BloodDamage => (calamityEnabled ? 180 : 100) / 2;
        static int DarkMatterDamage => (calamityEnabled ? 180 : 100) / 2;
        public static int SparkDamage => (calamityEnabled ? 180 : 100) / 2;
        static int ThornDamage => (calamityEnabled ? 180 : 100) / 2;
        public static int BGShotDamage => (calamityEnabled ? 180 : 100) / 2;
        static int DashDamage => (calamityEnabled ? 180 : 100) / 2;

        private int deathcounter = 0; //for death animation

        private int backgroundAttackCountDown = 0; //cycles through background attack at 10% health every 3 attacks

        private int animationXframeOffset = 0; //changes the sprite column depending on what animation is playing

        public override void Load()
        {
            calamityEnabled = ModLoader.TryGetMod("CalamityMod", out _);
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Zero");
            Main.npcFrameCount[NPC.type] = 8; //kinda pointless as the drawing is done manually

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/ZeroPortrait",
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0,
                PortraitPositionXOverride = 120,
                Position = new Vector2(100, 0),
                Scale = 0.75f,

            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCDebuffImmunityData debuffData = new()
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
            };
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement

        }

        public override void SetDefaults()
        {
            NPC.width = 398;
            NPC.height = 398;
            DrawOffsetY = 193;
            NPC.damage = 1; //dash damage in dash projectile. Needs to be >0 damage or else ApplyDifficultyAndPlayerScaling won't be called
            NPC.noTileCollide = true;
            if (!calamityEnabled)
            {
                //regular stats
                NPC.defense = 86;
                NPC.lifeMax = 280000;
            }
            else
            {
                NPC.defense = 200;
                NPC.lifeMax = 4_000_000;
            }
            NPC.HitSound = SoundID.NPCHit1; //slime
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 38, 18, 10); // money it drops
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 16;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.dontTakeDamage = true; //only initally
            /*Music = MusicID.Boss2;*/
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/Photonic0_ZeroTwo");
                //Music = MusicID.Boss2;
            }

            SceneEffectPriority = SceneEffectPriority.BossHigh; // By default, musicPriority is BossLow
            NPC.alpha = 255; //initally
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */ //damage is automatically doubled in expert, use this to reduce it
        {
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new HyperzoneBackgroundProvider(), //I totally didn't reference the vanilla code what no way

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The ultimate leader of all dark matter. Uses its leigon to blanket entire worlds in darkness.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer) //for syncing non NPC.ai[] stuff
        {
            writer.Write(animation);
            writer.Write((byte)lastattacktype);
            writer.Write((byte)attacktype);
            writer.Write(backupoffset);
            writer.Write(deathcounter);
            writer.Write(backgroundAttackCountDown);
            writer.Write(animationXframeOffset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animation = reader.ReadInt16();

            lastattacktype = (ZeroAttackType)reader.ReadByte();
            attacktype = (ZeroAttackType)reader.ReadByte();

            backupoffset = reader.ReadSingle();
            deathcounter = reader.ReadInt32();

            backgroundAttackCountDown = reader.ReadInt32();

            animationXframeOffset = reader.ReadInt32();
        }

        public override void AI() //constantly cycles each time
        {
            NPC.damage = 0;//set to 0 because can't set in SetDefaults
            Player playerstate = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            //DESPAWNING
            if (NPC.target < 0 || NPC.target == 255 || playerstate.dead || !playerstate.active)
            {
                NPC.TargetClosest(false);

                NPC.velocity.Y -= 0.2f;

                if (NPC.timeLeft > 60)
                {
                    NPC.timeLeft = 60;
                    return;
                }
            }
            else if (deathcounter > 0)
            {
                DoDeathAnimation();
            }
            else if (NPC.ai[1] < 390)
            {
                animation = 1;
                NPC.ai[1]++;

                if (NPC.ai[1] < 60) //rise up gang
                {
                    NPC.velocity.Y = -2;
                    NPC.alpha -= 5; //get visible
                }
                else
                {
                    NPC.velocity *= 0;
                }
                NPC.life = (int)Utils.Remap(NPC.ai[1], 0, 380, 1, NPC.lifeMax);

                if (NPC.ai[1] == 389) //fight start effect
                {
                    for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 20, Scale: 2); //Makes dust in a messy circle
                        d.noGravity = true;
                    }
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
            Vector2 playerDistance = player.Center - NPC.Center;

            bool bloodShots = attacktype == ZeroAttackType.BloodShots;
            Vector2 playerRightDistance = player.Center + new Vector2((bloodShots ? 1000 : 500) + backupoffset, 0) - NPC.Center;
            Vector2 playerLeftDistance = player.Center + new Vector2((bloodShots ? -1000 : -500) - backupoffset, 0) - NPC.Center;
            Vector2 playerAboveDistance = player.Center + new Vector2(0, -700) - NPC.Center;

            if (NPC.ai[0] == 0) //restart stats
            {
                animation = 0;
                NPC.frameCounter = 0; //reset animation

                NPC.dontTakeDamage = false;

                backupoffset = 0;
            }

            NPC.ai[0]++;

            // float timer = NPC.GetLifePercent() > 0.5 ? NPC.ai[0] : NPC.ai[0] + 60;

            float speed = 16;
            float inertia = 20;

            //far away
            if (playerDistance.Length() > 5000)
            {
                speed += 10 + playerDistance.Length() - 5000; //make Zero move faster 
                if (speed > 100)
                    speed = 100;
            }


            if (NPC.ai[0] <= AttackCooldown)
            {
                NPC.TargetClosest(true); //face player before attacking

                DefaultMovement(playerDistance, playerRightDistance, playerLeftDistance, speed, inertia);

                if (NPC.ai[0] == AttackCooldown)
                {
                    if (NPC.GetLifePercent() <= 0.55) //50%
                    {
                        //background attack is done every 3 attacks when zero's health gets low enough
                        //(does it the next cycle upon initally dropping low enough)

                        if (backgroundAttackCountDown <= 0) //equal or less than zero
                        {
                            attacktype = ZeroAttackType.BackgroundShots; //background attack
                            lastattacktype = ZeroAttackType.BackgroundShots;
                            backgroundAttackCountDown = 2; //restart
                        }
                        else //do regular attack
                        {
                            backgroundAttackCountDown--; //go down by 1
                            AttackDecideNext();
                        }
                    }
                    else
                    {
                        AttackDecideNext();
                    }
                }
            }
            else //attacking
            {
                switch (attacktype) //blood shots
                {
                    case ZeroAttackType.BloodShots:
                        Attack_BloodShots(player, playerRightDistance, playerLeftDistance, speed);
                        break;
                    case ZeroAttackType.Dash:
                        Attack_Dash(player);
                        break;
                    case ZeroAttackType.DarkMatterShots:
                        Attack_DarkMatterShots(playerDistance, playerRightDistance, playerLeftDistance, speed, inertia);
                        break;
                    case ZeroAttackType.Sparks:
                        Attack_Sparks(player, playerDistance, playerRightDistance, playerLeftDistance, speed, inertia);
                        break;
                    case ZeroAttackType.ThornTail:
                        Attack_ThornTail(ref playerAboveDistance, ref speed, inertia);
                        break;
                    case ZeroAttackType.BackgroundShots:
                        Attack_BGShots(player);
                        break;
                }
            }
        }

        private void Attack_BGShots(Player player)
        {
            NPC.velocity.Y *= 0.98f;

            if (NPC.ai[0] <= AttackCooldown + 60) //backup
            {
                NPC.TargetClosest(false); //don't face player during attack

                NPC.scale = 1 - (Utils.GetLerpValue(AttackCooldown, AttackCooldown + 60, NPC.ai[0]) * 0.6f); //get smaller

                NPC.behindTiles = true;
                NPC.damage = 0;
                NPC.dontTakeDamage = true;

                if (NPC.ai[0] == AttackCooldown + 1)
                {
                    NPC.velocity.X *= 0; //reset momentum
                }
                else
                {
                    NPC.velocity.X -= NPC.direction * 0.2f; //back up
                }

                animation = 5;
            }
            else if (NPC.ai[0] <= AttackCooldown + 360) //zoom
            {
                NPC.TargetClosest(false);

                if (NPC.ai[0] % 9 == 0 && NPC.ai[0] < AttackCooldown + 360) //shoot projectiles before getting big again
                {
                    Vector2 BackgroundplayerDistance = player.Center + (player.velocity * 5) - NPC.Center;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, BackgroundplayerDistance / 60, ModContent.ProjectileType<ZeroScreenBlood>(), BGShotDamage, 1f, Main.myPlayer, 0, NPC.ai[0] % 2 == 0 ? 0 : 1);
                    }
                    SoundEngine.PlaySound(SoundID.NPCHit9.WithVolumeScale(0.8f), NPC.Center); //leech hit

                }

                NPC.velocity.X += NPC.direction * 0.2f;

                if (NPC.velocity.X > 10)
                {
                    NPC.velocity.X = 10;
                }
                if (NPC.velocity.X < -10)
                {
                    NPC.velocity.X = -10;
                }

                if (NPC.ai[0] >= 420)
                {
                    NPC.scale += 0.01f; //get bigger
                }
            }
            else //reset
            {
                NPC.TargetClosest(true);
                NPC.ai[0] = 0;
                backupoffset = 0;
                NPC.scale = 1; //just in case
                NPC.behindTiles = false;
            }
        }

        private void Attack_ThornTail(ref Vector2 playerAboveDistance, ref float speed, float inertia)
        {
            if (NPC.ai[0] < AttackCooldown + 20)
            {
                animation = 2;
                NPC.velocity *= 0;

                if (NPC.ai[0] == AttackCooldown + 10)
                {
                    SoundEngine.PlaySound(SoundID.Item56.WithVolumeScale(1.6f).WithPitchOffset(-0.1f), NPC.Center);
                }
            }
            else if (NPC.ai[0] < AttackCooldown + 300)
            {
                animation = 3;

                speed += 8; //go slightly faster

                playerAboveDistance.Normalize();
                playerAboveDistance *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + playerAboveDistance) / inertia; //fly towards player
                int projRate = 25;
                if ((NPC.ai[0] - AttackCooldown) % projRate == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int count = 15;
                    int index = (int)(NPC.ai[0] - AttackCooldown) / projRate;
                    if (index % 3 == 1)
                        count++;
                    for (int i = 0; i < count; i++)
                    {

                        float horizontalVelocity;
                        float verticalVelocity;
                        switch (index % 3)
                        {
                            case 0:
                                horizontalVelocity = Utils.Remap(i, 0, count - 1, -30, 30);
                                verticalVelocity = Utils.GetLerpValue(0, (count - 1) / 2f, i, true) * Utils.GetLerpValue(count - 1, (count - 1) / 2f, i, true) * -10 + 15;
                                break;
                            case 1:
                                Vector2 vel = new Vector2(0, 15).RotatedBy(Utils.Remap(i, 0, count - 1, -2, 2));
                                horizontalVelocity = vel.X;
                                verticalVelocity = vel.Y;
                                break;
                            default:
                                horizontalVelocity = Utils.Remap(i, 0, count - 1, -20, 20);
                                verticalVelocity = 10;
                                break;
                        }

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 290),
                      new Vector2(horizontalVelocity, verticalVelocity), ModContent.ProjectileType<ZeroThornJuice>(), ThornDamage, 1f, Main.myPlayer, 0);
                    }

                }
                if (NPC.ai[0] % 25 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item17.WithVolumeScale(1.2f), NPC.Center); //stinger
                }
            }
            else if (NPC.ai[0] < AttackCooldown + 320)
            {
                if (NPC.ai[0] == AttackCooldown + 300)
                {
                    NPC.frameCounter = 0; //reset animation
                }
                animation = 4;
                NPC.velocity *= 0;
            }
            else
            {
                NPC.ai[0] = 0;
            }
        }

        private void Attack_Sparks(Player player, Vector2 playerDistance, Vector2 playerRightDistance, Vector2 playerLeftDistance, float speed, float inertia)
        {
            NPC.TargetClosest(true); //face player during attack

            int burstInterval = 3;
            int burstIndex = (int)((NPC.ai[0] - AttackCooldown) / burstInterval);

            if (Main.netMode != NetmodeID.MultiplayerClient && (NPC.ai[0] - AttackCooldown) % burstInterval == 0)
            {
                List<Player> players = new(8);
                foreach (Player plr in Main.ActivePlayers)//need to index this and count length
                {
                    players.Add(plr);
                }
                player = players[burstIndex % players.Count];
                for (int i = 0; i < 2 * burstInterval; i++)
                {

                    Vector2 velocity = Main.rand.NextVector2Unit();
                    float range = Utils.GetLerpValue(0, 60, (NPC.ai[0] / burstInterval) % 60);
                    if (i % 2 == 0)
                    {
                        range = 1 - range;
                    }
                    range = MathF.Sqrt(range);

                    float deviation = range;
                    deviation *= 1000;
                    Vector2 from = NPC.Center + new Vector2(NPC.direction * 150, 0);
                    //0.043f is what made it behave right
                    velocity *= deviation;
                    velocity = (ZeroSpark.AccountForVelocity(player.Center + velocity, player.velocity) - from) * .043f;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), from,
                        velocity,
                        ModContent.ProjectileType<ZeroSpark>(), SparkDamage, 0, Main.myPlayer, 0, 0);
                }
            }
            if ((NPC.ai[0] - AttackCooldown) % 5 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item39.WithVolumeScale(1.4f), NPC.Center);
            }

            backupoffset = 500;

            DefaultMovement(playerDistance, playerRightDistance, playerLeftDistance, speed, inertia);

            if ((NPC.ai[0] - AttackCooldown) >= 240)
            {
                NPC.ai[0] = 0;
            }
        }

        private void Attack_DarkMatterShots(Vector2 playerDistance, Vector2 playerRightDistance, Vector2 playerLeftDistance, float speed, float inertia)
        {
            NPC.TargetClosest(true); //face player during attack

            if ((NPC.ai[0] - AttackCooldown) % 80 == 0 && NPC.ai[0] < AttackCooldown + 300) //on multiples of 80
            {
                CreateDarkMatterFloorAndCeiling();

                SoundEngine.PlaySound(SoundID.Item104, NPC.Center);
            }
            DefaultMovement(playerDistance * 2, playerRightDistance, playerLeftDistance, speed, inertia);

            if (NPC.ai[0] >= AttackCooldown + 360)
            {
                NPC.ai[0] = 0;
            }
        }

        private void Attack_Dash(Player player)
        {
            NPC.TargetClosest(false); //don't face player during attack

            if (NPC.ai[0] <= AttackCooldown + 40) //backup
            {
                if (NPC.ai[0] == AttackCooldown + 1)
                {
                    int dmgHitboxType = ModContent.ProjectileType<ZeroDamageHitbox>();
                    bool shouldSpawnDamageHitbox = true;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile compare = Main.projectile[i];
                        if (compare.active && compare.type == dmgHitboxType && compare.ai[0] == NPC.whoAmI)
                        {
                            shouldSpawnDamageHitbox = false;
                            break;
                        }
                    }
                    if (shouldSpawnDamageHitbox)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, default, dmgHitboxType, DashDamage, 0, Main.myPlayer, NPC.whoAmI);
                    }
                    NPC.velocity *= 0.01f; //freeze to warn player (but not too much else it might disappear)

                    SoundEngine.PlaySound(SoundID.Item113.WithPitchOffset(-0.5f).WithVolumeScale(6f), NPC.Center); //deep, loud roaaar
                }
                else
                {
                    NPC.velocity.X -= NPC.direction * 0.5f; //back up
                }
            }
            else if (NPC.ai[0] <= AttackCooldown + 180) //dash
            {
                NPC.velocity.X += NPC.direction * 0.6f;
            }//IF YOU CHANGE THE DURATION OF THE DASH REMEMBER TO CHANGE IT IN THE ZERODAMAGEHITBOX PROJECTILE!!
            else //reset
            {
                NPC.ai[0] = 0;
                backupoffset = 0;
            }

            //go up or down
            if (player.Center.Y < NPC.Center.Y)
            {
                NPC.velocity.Y -= 0.35f;
            }
            else
            {
                NPC.velocity.Y += 0.35f;
            }

            //cap
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -7.5f, 7.5f);
        }

        private void Attack_BloodShots(Player player, Vector2 playerRightDistance, Vector2 playerLeftDistance, float speed)
        {
            NPC.TargetClosest(true); //face player during attack
            speed += 8;

            if (NPC.ai[0] > AttackCooldown + 30)
            {
                //blood effect for attack
                if (NPC.ai[0] % 5 == 0) //every 5
                {
                    int d = Dust.NewDust(NPC.position + new Vector2(NPC.direction == 1 ? NPC.width - 200 : 0, NPC.height / 4), 200, 200, ModContent.DustType<Dusts.Redsidue>());
                    Main.dust[d].velocity *= 0;
                }

                if (NPC.ai[0] % 10 == 0) //every 10 ticks
                {
                    Vector2 bloodlocation = new(NPC.Center.X + NPC.width / 3 * NPC.direction, NPC.Center.Y + Main.rand.Next(-150, 150));

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 spread = Main.rand.BetterNextVector2Circular(10f); //circle
                        Dust.NewDustPerfect(bloodlocation, ModContent.DustType<Dusts.Redsidue>(), spread, Scale: 1f); //Makes dust in a messy circle
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            Player target = Main.player[i];
                            if (target.dead || !target.active)
                                continue;
                            if (NPC.direction * (target.Center.X - NPC.Center.X) > 0 && target.Distance(NPC.Center) < 16 * 100)
                            {
                                ZeroBloodShot.GetAIValues(i, out float ai0);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), bloodlocation, new Vector2(NPC.direction, 0), ModContent.ProjectileType<ZeroBloodShot>(), BloodDamage, 6f, Main.myPlayer, ai0);
                            }
                        }
                    }
                    SoundStyle SkinTear = new("KirboMod/Sounds/Item/SkinTear");
                    SoundEngine.PlaySound(SkinTear, NPC.Center);
                }
            }

            Vector2 dist = playerRightDistance;
            if (NPC.direction == 1)
            {
                dist = playerLeftDistance;
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + dist * 1000) * speed, 0.1f);
            if (NPC.ai[0] >= AttackCooldown + 90 + 30)
            {
                NPC.ai[0] = 0;
            }

        }

        //call this on the frame you want to create the setup
        //probably snaps to the position too quickly, but idk how to calculate the numbers to fix it so it gets there in fewer frames
        void CreateDarkMatterFloorAndCeiling()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            Player player = Main.player[NPC.target];
            //tweak these parameters to your liking for balancing
            float ySpacing = 1000;
            float xSpacing = 130;
            int mattersInWall = 20;

            for (int i = 0; i < mattersInWall; i++)
            {
                //if i is even, mod will result in 0, multiplied by 2 becomes 0, then subtract one to become -1
                //if i is odd, mod will result in 1, multiplied by 2 becomes 2, then subtract one to become 1
                int directionY = i % 2 * 2 - 1;
                Vector2 offset = default;
                offset.Y = directionY * ySpacing / 2;
                offset.X = Utils.Remap(i, 0, mattersInWall - 1, -xSpacing * mattersInWall / 2, xSpacing * mattersInWall / 2) - NPC.direction * 64;
                DarkMatterShot.AccountForSpeed(ref offset, player);
                Vector2 from = NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height) / 3f;
                DarkMatterShot.NewDarkMatterShot(NPC, offset + player.Center, from, DarkMatterDamage, directionY);
            }
        }

        void AttackDecideNext()
        {
            List<ZeroAttackType> possibleAttacks = new() { ZeroAttackType.BloodShots, ZeroAttackType.Dash, ZeroAttackType.DarkMatterShots, ZeroAttackType.Sparks, ZeroAttackType.ThornTail };

            possibleAttacks.Remove(lastattacktype);

            if (NPC.GetLifePercent() >= 0.97f)
            {
                attacktype = ZeroAttackType.BloodShots;
            }
            else
            {
                attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
                NPC.netUpdate = true;
            }
            lastattacktype = attacktype;
        }
        void DefaultMovement(Vector2 playerDistance, Vector2 playerRightDistance, Vector2 playerLeftDistance, float speed, float inertia)
        {
            if (playerDistance.X <= 0)
            {
                playerRightDistance.Normalize();
                playerRightDistance *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + playerRightDistance) / inertia; //fly towards player
            }
            else
            {
                playerLeftDistance.Normalize();
                playerLeftDistance *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + playerLeftDistance) / inertia; //fly towards player
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            int r;
            int g;
            int b;
            r = 255 - NPC.alpha;
            g = 255 - NPC.alpha;
            b = 255 - NPC.alpha;
            return new Color(r, g, b, 0); //fade in 
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;

            position.Y = NPC.position.Y + NPC.height + 40;

            if (NPC.life == 1 && deathcounter > 0)
            {
                return false; //no health bar
            }
            else //health bar
                return true;
        }

        public override bool CheckDead()
        {
            if (deathcounter <= 100000)
            {
                NPC.active = true;
                NPC.life = 1;
                deathcounter += 1; //go up
                return false;
            }
            return true;
        }

        public override bool PreKill()
        {
            return !Main.expertMode;
        }

        private void DoDeathAnimation()
        {
            if (deathcounter > 0 && deathcounter < 300)
            {
                NPC.ai[0] = 0; //don't attack
                NPC.dontTakeDamage = true;
                deathcounter += 1; //go up
                NPC.damage = 0;
                NPC.active = true;
                NPC.velocity *= 0.95f;
                animation = 0; //regular

                if (Main.expertMode)
                {
                    NPC.rotation = MathHelper.ToRadians(90); //up
                    NPC.direction = -1; //to make sure it doesn't face down
                }
                else
                {
                    NPC.rotation += MathHelper.ToRadians(5); //rotate
                }

                if (deathcounter % 5 == 0) //effects
                {
                    int randomX = Main.rand.Next(0, NPC.width);
                    int randomY = Main.rand.Next(0, NPC.height);
                    SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);

                    for (int i = 0; i < 10; i++) //first section makes variable //second declares the conditional // third declares the loop
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
                        Dust d = Dust.NewDustPerfect(NPC.position + new Vector2(randomX, randomY), ModContent.DustType<Dusts.Redsidue>(), speed, Scale: 2); //Makes dust in a messy circle
                        d.noGravity = true;
                    }
                }

                if ((deathcounter == 260 || deathcounter == 280) && !Main.expertMode) //only in normal mode
                {
                    for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed * 20, Scale: 4); //Makes dust in a messy circle
                        d.noGravity = true;
                    }

                    SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
                }
            }
            else if (deathcounter > 0)
            {
                deathcounter = 99999999;
                if (Main.expertMode)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                        Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -200), ModContent.DustType<Dusts.Redsidue>(), speed * 20, Scale: 4); //Makes dust in a messy circle
                        d.noGravity = true;
                    }
                    Dust.NewDustPerfect(NPC.position + new Vector2(-200, -200), ModContent.DustType<Dusts.ZeroEyeless>(), new Vector2(0, 5), 0);
                    if (Main.netMode != NetmodeID.MultiplayerClient) //don't use SpawnBoss() as we need the special status message
                    {
                        ZeroEye.GetAIValues(out float[] ai2s);
                        for (int i = 0; i < ai2s.Length; i++)
                        {
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 15, ModContent.NPCType<ZeroEye>(), ai2: ai2s[i]);
                        }

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText("Zero's eye has ejected from it's body!", 175, 75);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Zero's eye has ejected from it's body!"), new Color(175, 75, 255));
                        }
                    }
                }
                NPC.dontTakeDamage = false;
                NPC.HideStrikeDamage = true;
                NPC.SimpleStrikeNPC(999999, 1, false, 0, null, false, 0, false);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Zero"; //_ has been defeated!
            potionType = ItemID.SuperHealingPotion; //potion it drops
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (deathcounter >= 300 && !Main.expertMode) //only in normal mode
            {
                for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed * 20, Scale: 4); //Makes dust in a messy circle
                    d.noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
            }
            else
            {
                for (int i = 0; i < 2; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.Redsidue>());
                }
            }
        }

        public override void OnKill()
        {
            if (!Main.expertMode && !Main.masterMode) //only register when not expert mode and master mode
            {
                NPC.SetEventFlagCleared(ref DownedBossSystem.downedZeroBoss, -1);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ZeroBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ZeroMask>(), 7));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.MiracleMatter>(), 1, 2, 2));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Zero.ZeroTrophy>(), 10)); //drop trophy

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.ZeroRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Zero.ZeroPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

        public override void DrawBehind(int index)
        {
            if (attacktype == ZeroAttackType.BackgroundShots & NPC.ai[0] > 120)
            {
                NPC.hide = true;
                Main.instance.DrawCacheNPCsMoonMoon.Add(index);//be drawn behind things like moonlord(?)
            }
            else
            {
                NPC.hide = false;
            }
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) //box where NPC name and health is shown
        {
            boundingBox = NPC.Hitbox;
        }



        //MANUAL DRAWING INBOUND!


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D zero = TextureAssets.Npc[Type].Value; //get the texture but actually

            //fade in stuff (fades in from black but it looks cool so I'm keeping it)
            int r;
            int g;
            int b;
            r = 255 - NPC.alpha;
            g = 255 - NPC.alpha;
            b = 255 - NPC.alpha;

            SpriteEffects direction = SpriteEffects.FlipHorizontally; //face right
            if (NPC.direction == -1)
            {
                direction = SpriteEffects.None; //face left
            }

            //draw!
            spriteBatch.Draw(zero, NPC.Center - Main.screenPosition, Animation(), new Color(r, g, b), NPC.rotation,
                new Vector2(400, 400), NPC.scale, direction, 0f);
            if (attacktype == ZeroAttackType.Dash)
            {
                float progress = Utils.GetLerpValue(0, 240, NPC.ai[0]);
                float size = Easings.EaseOut(progress, 3) * 5 + 1;
                float opacity = Easings.RemapProgress(0, 10, 300, 310, NPC.ai[0]);
                Vector2 drawpos = NPC.Center + new Vector2(NPC.direction * 135, 2) - Main.screenPosition;
                float rotation = Easings.InOutCirc(progress) * MathF.Tau * 6;
                VFX.DrawPrettyStarSparkle(opacity, drawpos, Color.Black, Color.Black, 1, 0, .9f, 1.1f, 2, rotation, new Vector2(size) * 2, new Vector2(4));
                VFX.DrawPrettyStarSparkle(opacity, drawpos, new Color(255, 255, 255, 0), Color.Red, 1, 0, .9f, 1.1f, 2, rotation, new Vector2(size), new Vector2(2));
            }
            return false;
        }
        private Rectangle Animation() //make the dimensions for the frames
        {
            //inital
            int Xframe = 0;
            int Yframe = 0;

            if (animation == 0) //regular
            {
                if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
                {
                    NPC.frameCounter++;
                }
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0; //attacks 
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 0; //attacks 
                    Yframe = 800;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 0; //attacks 
                    Yframe = 1600;
                }
                else if (NPC.frameCounter < 40)
                {
                    Xframe = 0; //attacks 
                    Yframe = 800;
                }
                else
                {
                    Xframe = 0; //attacks 
                    Yframe = 0;
                    NPC.frameCounter = 0; //reset
                }
            }
            if (animation == 1) //intro
            {
                if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
                {
                    NPC.frameCounter++;
                }
                if (NPC.frameCounter < 60) //change texture
                {
                    Xframe = 1; //intro 1
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 70)
                {
                    Xframe = 1; //intro 1
                    Yframe = 800;
                }
                else if (NPC.frameCounter < 80)
                {
                    Xframe = 1; //intro 1
                    Yframe = 1600;
                }
                else if (NPC.frameCounter < 90)
                {
                    Xframe = 1; //intro 1
                    Yframe = 2400;
                }
                else if (NPC.frameCounter < 150)
                {
                    Xframe = 1; //intro 1
                    Yframe = 3200;
                }
                else if (NPC.frameCounter < 160)
                {
                    Xframe = 1; //intro 1
                    Yframe = 4000;
                }
                else if (NPC.frameCounter < 170)
                {
                    Xframe = 1; //intro 1
                    Yframe = 4800;
                }
                else if (NPC.frameCounter < 180) //change texture
                {
                    Xframe = 2; //intro 2
                    Yframe = 0;
                }
                else if (NPC.frameCounter < 240)
                {
                    Xframe = 2; //intro 2
                    Yframe = 800;
                }
                else if (NPC.frameCounter < 250)
                {
                    Xframe = 2; //intro 2
                    Yframe = 1600;
                }
                else if (NPC.frameCounter < 310)
                {
                    Xframe = 2; //intro 2
                    Yframe = 2400;
                }
                else if (NPC.frameCounter < 320)
                {
                    Xframe = 2; //intro 2
                    Yframe = 3200;
                }
                else
                {
                    Xframe = 2; //intro 2
                    Yframe = 4000;
                }
            }
            if (animation == 2) //thorns open
            {
                if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
                {
                    NPC.frameCounter++;
                }
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0; //attacks 
                    Yframe = 2400;
                }
                else
                {
                    Xframe = 0; //attacks 
                    Yframe = 3200;
                }
            }
            if (animation == 3) //thorns loop
            {
                Xframe = 0; //attacks 
                if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
                {
                    NPC.frameCounter++;
                }
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0; //attacks 
                    Yframe = 3200;
                }
                else if (NPC.frameCounter < 20)
                {
                    Xframe = 0; //attacks 
                    Yframe = 4000;
                }
                else if (NPC.frameCounter < 30)
                {
                    Xframe = 0; //attacks 
                    Yframe = 3200;
                }
                else if (NPC.frameCounter < 40)
                {
                    Xframe = 0; //attacks 
                    Yframe = 4800;
                }
                else
                {
                    Yframe = 3200;
                    NPC.frameCounter = 0; //reset
                }
            }
            if (animation == 4) //thorns close
            {
                if (!Main.gamePaused && Main.hasFocus) //not paused or tabbed out
                {
                    NPC.frameCounter++;
                }
                if (NPC.frameCounter < 10)
                {
                    Xframe = 0; //attacks 
                    Yframe = 2400;
                }
                else
                {
                    Xframe = 0; //attacks 
                    Yframe = 5600;
                }
            }
            if (animation == 5) //front face
            {
                Xframe = 0; //attacks 
                Yframe = 5600;
            }

            return new Rectangle(Xframe * 800, Yframe, 800, 800);
        }
    }
}
