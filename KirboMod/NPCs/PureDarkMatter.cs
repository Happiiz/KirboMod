using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public class PureDarkMatter : ModNPC
	{
		private int phase2special = 1; //for deciding which special is used in phase 2

		private int frenzydashamount = 2; //not 3 because the inital dash

		private bool frenzy = false;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Matter");
			Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused, // Most NPCs have this
		            BuffID.Poisoned,
                    BuffID.Venom,
                    BuffID.OnFire,
                    BuffID.CursedInferno,
                    BuffID.ShadowFlame,
                }
            };
            NPCID.Sets.DebuffImmunitySets[Type] = debuffData;
        }

		public override void SetDefaults()
		{
			NPC.width = 130;
			NPC.height = 130;
			NPC.damage = 100;
			NPC.noTileCollide = true;
			NPC.defense = 35;
			NPC.lifeMax = 30000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice( 0, 19, 9, 5); // money it drops
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.npcSlots = 16;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			NPC.buffImmune[BuffID.Poisoned] = true;
			NPC.buffImmune[BuffID.Venom] = true;
			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.buffImmune[BuffID.CursedInferno] = true;
			NPC.buffImmune[BuffID.ShadowFlame] = true;
			Music = MusicID.Boss4;
            NPC.buffImmune[BuffID.Confused] = true;
        }

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75 * balance); // (npc.lifeMax * 2) * reducer == expert health
			NPC.damage = (int)(NPC.damage * 1);
		}
		public override void AI() //constantly cycles each time
		{
			Player playerstate = Main.player[NPC.target];

			NPC.netAlways = true;
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

				if (NPC.ai[3] < 30) //rise up gang
				{
					NPC.velocity.Y = -3;
					NPC.ai[3]++;
				}
				else if (NPC.ai[3] >= 30)//start cycle
				{

					if (NPC.ai[3] < 360) //Movement before special attack (Alot of this code was taken from ExampleMod's capitive element(2)) so I don't fully understand this yet, but it makes my video game reference go smooth so I don't mind it
                    {
						float minX = moveTo.X - 50f;
						float maxX = moveTo.X + 50f;
						float minY = moveTo.Y;
						float maxY = moveTo.Y;

						if (playerDistance.X <= 0) //if player is behind enemy
						{
							move = move + new Vector2(400f, 0); // go in front of player 
						}
						else
						{
							move = move + new Vector2(-400f, 0); // go behind player 
						}

						if (NPC.Center.X >= minX && NPC.Center.X <= maxX && NPC.Center.Y >= minY && NPC.Center.Y <= maxY) //certain range
						{
							NPC.velocity *= 0.98f; //slow
						}
						else
						{
							float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
							float speed = 28; //speed I think
							if (magnitude > speed)
							{
								move *= speed / magnitude;
							}
							float inertia = 10f; //Ok so like I'm pretty sure this is supposed to be how wibbly wobbly you want the npc to be before it reaches its destination
							NPC.velocity = (inertia * NPC.velocity + move) / (inertia + 1);
							magnitude = (float)Math.Sqrt(NPC.velocity.X * NPC.velocity.X + NPC.velocity.Y + NPC.velocity.Y);
							if (magnitude > speed)
							{
								NPC.velocity *= speed / magnitude;
							}
						}
						NPC.TargetClosest(true);
					}

					if (!Main.expertMode) //if not Expert Mode
					{
						NPC.damage = 120; 
					}
					else
                    {
						NPC.damage = 240; 
                    }

					NPC.defense = 0; //no defense

					if (NPC.ai[3] == 30) //checks if cycle has restarted
					{
						if (NPC.life <= NPC.lifeMax * 0.50) //checks if npc has 5/10 of life in any difficulty
						{
							frenzy = true;
						}
						else
						{
							frenzy = false;
						}
					}

					NPC.ai[3]++; //phase 2 cycle

                if (NPC.ai[3] >= 90 && NPC.ai[3] <= 120) //Matter Orb
                {
                    if (NPC.ai[3] == 90)
                    {
                        //up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 100)
                    {
                        //down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 110)
                    {
                        //diagonal up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 120)
                    {
                        //diagonal down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                }
                if (NPC.ai[3] >= 130 && NPC.ai[3] <= 170 && frenzy == true) //Matter Orb (enraged)
                {
                    if (NPC.ai[3] == 130)
                    {
                        //up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 140)
                    {
                        //down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 150)
                    {
                        //diagonal up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 160)
                    {
                        //diagonal down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 170)
                    {
                        //center
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 0, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                }
                if (NPC.ai[3] >= 180 && NPC.ai[3] <= 210) //Matter Orb
                {
                    if (NPC.ai[3] == 180)
                    {
                        //up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 190)
                    {
                        //down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 200)
                    {
                        //diagonal up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 210)
                    {
                        //diagonal down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                }
                if (NPC.ai[3] >= 220 && NPC.ai[3] <= 260 && frenzy == true) //Matter Orb (enraged)
                {
                    if (NPC.ai[3] == 220)
                    {
                        //up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 230)
                    {
                        //down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 240)
                    {
                        //diagonal up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 250)
                    {
                        //diagonal down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 260)
                    {
                        //center
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 0, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                }
                if (NPC.ai[3] >= 270 && NPC.ai[3] <= 300) //Matter Orb
                {
                    if (NPC.ai[3] == 270)
                    {
                        //up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y - 10, 0, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 280)
                    {
                        //down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 0, NPC.Center.Y + 10, 0, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 290)
                    {
                        //diagonal up
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * -10, NPC.Center.Y + NPC.direction * -5, NPC.direction * -10, -10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                    else if (NPC.ai[3] == 300)
                    {
                        //diagonal down
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.direction * 10, NPC.Center.Y + NPC.direction * 5, NPC.direction * -10, 10, ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 2f, Main.myPlayer, 0, NPC.target);
                        }
                        SoundEngine.PlaySound(SoundID.SplashWeak, NPC.Center);

                        for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                        {
                            Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                            Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed, 0, default, 1f); //Makes dust in a messy circle
                            d.noGravity = true;
                        }
                    }
                }

                //SPECIAL ATTACKS
                if (NPC.ai[3] == 360 & phase2special == 3) //reset because there is no special 3
                {
                    phase2special = 1;
                }

                if (NPC.ai[3] > 360 & NPC.ai[3] < 420 & phase2special == 1) //DASH START //Backup
                {
                    NPC.TargetClosest(true); //face player

                    float speed = 28f;
                    float inertia = 10f;

                    Vector2 direction = player.Center - NPC.Center; //start - end

                    if (playerDistance.X <= 0) //if player is behind enemy
                    {
                        direction.X += 400f + (NPC.ai[3] - 360) * 3; // go in front of player (and backup!)
                    }
                    else
                    {
                        direction.X -= 400f + (NPC.ai[3] - 360) * 3; // go behind player (and backup!)
                    }

                    direction.Y += player.velocity.Y * 30; //read player

                    direction.Normalize();
                    direction *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;  //fly to area of aim
                }

                if (NPC.ai[3] >= 420 & phase2special == 1) //dash for 60 ticks
                {
                    NPC.TargetClosest(false);
                    NPC.velocity.X += NPC.direction * 1.04f;
                    NPC.velocity.Y = 0;

                    if (NPC.ai[3] % 2 == 0) //every multiple of 2 place a dust
                    {
                        Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.Next(-70, 70), Main.rand.Next(-70, 70)), ModContent.DustType<Dusts.DarkResidue>(), NPC.velocity * 0, 2); //Makes dust i
                        d.noGravity = true;
                    }

                    if (NPC.ai[3] == 420) //upon dash start
                    {
                        SoundEngine.PlaySound(SoundID.Roar, player.position); //do da roar
                    }
                }

                if (NPC.ai[3] > 360 & NPC.ai[3] <= 540 & phase2special == 2) //DARK LASERS
                {
                    NPC.velocity *= 0;

                    if (NPC.ai[3] % 60 == 0 & phase2special == 2) //shoots only if ai 3 is a multiple of 60
                    {
                        NPC.TargetClosest(true);
                        Vector2 lasershoot = player.Center - NPC.Center;
                        lasershoot.Normalize(); //reduces it to a value of 1
                        lasershoot *= 32f; //inital projectile speed

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 5, 0), lasershoot, Mod.Find<ModProjectile>("DarkLaser").Type, 120 / 2, 10f, Main.myPlayer);
                        }
                        SoundEngine.PlaySound(SoundID.Item12);
                    }
                }

                if (NPC.ai[3] == 660 && phase2special == 2 & frenzy == true) //for spin beam
                {
                    NPC.rotation = MathHelper.ToRadians(-90f); //face up
                    NPC.direction = 1; //face correctly(idk which way but it's right*get it?*)
                }

                if (NPC.ai[3] > 660 & NPC.ai[3] <= 680 && phase2special == 2 & frenzy == true) // also for spin beam
                {
                    NPC.rotation += MathHelper.ToRadians(18);
                }

                if (NPC.ai[3] >= 740 & NPC.ai[3] <= 980 & phase2special == 2 && frenzy == true) //SPIN BEAM (expert frenzy)
                {
                    NPC.velocity *= 0; //stop

                    if (NPC.ai[3] == 680) //face up
                    {
                        NPC.rotation = MathHelper.ToRadians(-90f);
                        NPC.direction = 1; //face correctly(idk which way but it's right*get it?*)
                    }

                    if (NPC.ai[3] >= 740 & NPC.ai[3] < 980 & phase2special == 2) //4 seconds
                    {
                        NPC.rotation += MathHelper.ToRadians(3);
                        NPC.TargetClosest(false); //don't face player

                        if (NPC.ai[3] % 3 == 0)//only shoots if ai 3 is a multiple of 3
                        {
                            float spinshoot = 18f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 10 * (float)Math.Cos(NPC.rotation), NPC.Center.Y + 10 * (float)Math.Cos(NPC.rotation), spinshoot * (float)Math.Cos(NPC.rotation), spinshoot * (float)Math.Sin(NPC.rotation), Mod.Find<ModProjectile>("AngledDarkBeam").Type, 60 / 2, 10f, Main.myPlayer);
                            }
                            SoundEngine.PlaySound(SoundID.Item12);
                        }
                    }
                }


                if (NPC.ai[3] >= 480 & phase2special == 1) //dash end
                {
                    if (!frenzy) //not frenzying
                    {
                        NPC.ai[3] = 30; //don't make it zero or it will rise again
                        phase2special += 1; //next attack
                    }
                    else //frenzying
                    {
                        if (frenzydashamount > 0)
                        {
                            frenzydashamount -= 1;
                            NPC.velocity *= 0; //cut velocity
                            NPC.ai[3] = 361; //restart dash
                        }
                        else //end dash for real
                        {
                            NPC.ai[3] = 30; //don't make it zero or it will rise again
                            phase2special += 1; //next attack
                            frenzydashamount = 2; //reset frenzy dash amount for next dash 
                        }
                    }
                }

                if (NPC.ai[3] >= 660 & phase2special == 2 & frenzy == false) //laser end
                {
                    NPC.ai[3] = 30; //don't make it zero or it will rise again
                    phase2special += 1; //next attack
                }
                else if (NPC.ai[3] >= 1100 & phase2special == 2 & frenzy == true) //spin beam end
                {
                    NPC.ai[3] = 30; //don't make it zero or it will rise again
                    phase2special += 1; //next attack
                    NPC.rotation = 0;
                }
            }
            NPC.spriteDirection = NPC.direction;
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
				NPC.frame.Y = frameHeight; //lil' stretch
			}
			else
			{
				NPC.frameCounter = 0.0; //reset
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
			if (NPC.ai[3] > 360 & NPC.ai[3] < 540 & phase2special == 2) //laser attack
			{
				if ((NPC.ai[3] % 60 <= 5) == false) //doesn't glow purple on the shot(or momentaraily after)
				{
					return Color.Purple; //make it ourple
				}
				else
				{
					return Color.White; //make it unaffected by light
				}
			}
			else
            {
				return Color.White; //make it unaffected by light
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return true;
		}

		public override bool PreKill() //don't drop anything
		{
			return false;
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 4, 10); //Makes dust in a messy circle
					d.noGravity = true;
				}

				Player player = Main.player[NPC.target];

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnBoss((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Zero>(), player.whoAmI); //different from SpawnOnPlayer()
                }
            }
			else
            {
				for (int i = 0; i < 5; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 2, 2); //Makes dust in a messy circle
					d.noGravity = false;
				}
			}
		}
    }

}
