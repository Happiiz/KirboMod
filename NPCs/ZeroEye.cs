using KirboMod.Items.Zero;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using KirboMod.Systems;
using KirboMod.Bestiary;
using System.Collections.Specialized;
using System.IO;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public class ZeroEye : ModNPC
	{
		private int deathcounter = 0; //for death animation

		Vector2 chargedirection = new Vector2(1, 1);

		//default speed, acceleration and charge speed
        private float speed = 40f;
        private float inertia = 50f;
		private float chargespeed = 30;

		//shortens the time between charges and the time of charges
		private float chargereduce = 0;

		private bool spewFaster = false; //decide if to spew blood faster (whether or not eye is low)
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Eye of Zero");
			Main.npcFrameCount[NPC.type] = 1;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true
            };
        }

		public override void SetDefaults()
		{
			NPC.width = 110;
			NPC.height = 110;
            NPC.defense = 60;
            NPC.lifeMax = 20000;
			NPC.damage = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 38, 18, 10); // money it drops
			NPC.knockBackResist = 0f; //how much knockback applies
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.boss = true;
			NPC.npcSlots = 16;

			NPC.lavaImmune = true;

			Music = MusicID.Boss2;
			SceneEffectPriority = SceneEffectPriority.BossHigh; // By default, musicPriority is BossLow		
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
		{
			//half it so when it scales 2x it will stay the same
			NPC.lifeMax = (int)(NPC.lifeMax * 0.5 * balance);
			NPC.damage = (int)(NPC.damage * 0.5);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new HyperzoneBackgroundProvider(), //I totally didn't reference the vanilla code what no way

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("No one knows where the source of this pure evil came from. A physical manifestation of one half of the battle between good and evil? But then what good counters evil of this magnitude?")
            });
        }

        public override void SendExtraAI(BinaryWriter writer) //syncing stuff
        {
			writer.Write(deathcounter);
			writer.WriteVector2(chargedirection);
			writer.Write(speed);
            writer.Write(inertia);
            writer.Write(chargespeed);
            writer.Write(chargereduce);
            writer.Write(spewFaster);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			deathcounter = reader.ReadInt32();
            chargedirection = reader.ReadVector2();
            speed = reader.ReadSingle();
            inertia = reader.ReadSingle();
            chargespeed = reader.ReadSingle();
            chargereduce = reader.ReadSingle();
            spewFaster = reader.ReadBoolean();
        }

        public override void AI() //constantly cycles each time
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            if (NPC.ai[1] <= 60) //rise
            {
                NPC.ai[1]++;
                NPC.velocity.Y = -10;
                NPC.velocity.X = 0;
                NPC.rotation = MathHelper.ToRadians(270);
                NPC.damage = 0;
            }
            else if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
			{
				NPC.velocity.Y = NPC.velocity.Y - 0.2f;

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
			else
			{
				AttackPattern();
				NPC.damage = 200;
			}
		}

		private void AttackPattern()
		{
			Player player = Main.player[NPC.target];

            if (Vector2.Distance(player.Center, NPC.Center) > 2000) //player is far away
            {
                speed = 100;
            }
            else if (NPC.life <= NPC.lifeMax * 0.25 && NPC.ai[0] == 0) //low
			{
				speed = 60; //25% faster speed
				inertia = 62.5f; //25% faster acceleration

				chargespeed = 40; //33.3& faster charge speed
				chargereduce = 150; //300 - 150 = 150 ticks before charge if low (150 / 3 is 30 less for charge)

				spewFaster = true;
            }
			else
			{
                speed = 40f;
                inertia = 50f;
            }

			Vector2 moveTo = player.Center;
			Vector2 direction = moveTo - NPC.Center; //start - end
			direction.Normalize();
			direction *= speed;

			if (NPC.ai[0] < 300 - chargereduce)
			{
				NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia; //follow player
			}

			NPC.ai[0]++;
			if (NPC.ai[0] % (spewFaster ? 7 : 10) == 0 && NPC.ai[0] < 300 - chargereduce) //only do this if less than 300(or less)
			{
				//divide damage by 4 because so it doesn't scale in expert mode because npc damage doesnt scale in expert mode
				//and so it doesn't scale normally
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity * 0, Mod.Find<ModProjectile>("ZeroEyeBlood").Type, NPC.damage / 4, 2f, Main.myPlayer);
				}
			}

			if (NPC.ai[0] >= 300 - chargereduce)
			{
				if (NPC.life <= NPC.lifeMax * 0.5) //initate dash
				{
					if (NPC.ai[0] <= 360 - (chargereduce + chargereduce / 5)) //stop
					{
						NPC.velocity *= 0.01f; //freeze to warn player (but not too much else it might disappear)
                    }
					else if (NPC.ai[0] <= 390 - (chargereduce + chargereduce / 5)) //initiate dash
					{
						if (NPC.ai[0] == 361 - (chargereduce + chargereduce / 5))
						{
							chargedirection = player.Center - NPC.Center; //start - end
						}
						chargedirection.Normalize();
						chargedirection *= chargespeed; //changes depending whether or not eye has less than 25% health
						NPC.velocity = chargedirection; //charge

						if (NPC.ai[0] % 2 == 0)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity * 0, Mod.Find<ModProjectile>("ZeroEyeBlood").Type, NPC.damage / 4, 2f, Main.myPlayer);
							}
						}
					}
					else
					{
						NPC.ai[0] = 0;
					}
				}
				else //restart
				{
					NPC.ai[0] = 0;
				}
			}

			//rotato
			// First, calculate a Vector pointing towards what you want to look at
			Vector2 distance = player.Center - NPC.Center;
			// Second, use the ToRotation method to turn that Vector2 into a float representing a rotation in radians.
			float desiredRotation = distance.ToRotation();

			NPC.rotation = desiredRotation;
		}

		public override void FindFrame(int frameHeight) // animation
		{

		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed, Scale: 1f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "Zero"; //_ has been defeated!
			potionType = ItemID.SuperHealingPotion; //potion it drops
		}

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedZeroBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ZeroBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ZeroMask>(), 7));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.MiracleMatter>(), 1, 2, 2));

            ItemDropRule.Common(ModContent.ItemType<Items.Zero.ZeroTrophy>(), 10); //drop trophy

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.ZeroRelic>()));

			masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Kracko.KrackoPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

        public override Color? GetAlpha(Color drawColor)
		{
			return Color.White; //make it unaffected by light
		}
		public override bool CheckDead()
		{
			if (deathcounter < 300)
			{
				NPC.active = true;
				NPC.life = 1;
				deathcounter += 1; //go up
				return false;
			}
			return true;
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
				NPC.velocity *= 0.01f;

				NPC.rotation += MathHelper.ToRadians(15); //rotate
				
				if (deathcounter % 5 == 0) //effects
				{
					int randomX = Main.rand.Next(0, NPC.width);
					int randomY = Main.rand.Next(0, NPC.height);
					SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);
				}

				for (int i = 0; i < 3; i++) //first section makes variable //second declares the conditional // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(20f, 20f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed, Scale: 2); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
			else if (deathcounter > 0) //death
			{
				NPC.dontTakeDamage = false;
                NPC.HideStrikeDamage = true;
                NPC.SimpleStrikeNPC(999999, 1, false, 0, null, false, 0, false);
                for (int i = 0; i < 60; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(40f, 40f);
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed, Scale: 3); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}

		}
	}
}
