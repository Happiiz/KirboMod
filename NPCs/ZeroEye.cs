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
using KirboMod.Projectiles;
using System.Collections.Generic;
using Terraria.Localization;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public class ZeroEye : ModNPC
	{
		private int deathcounter = 0; //for death animation

        SoundStyle Death = new("KirboMod/Sounds/NPC/ZeroDeathSound");

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Eye of Zero");
			Main.npcFrameCount[NPC.type] = 1;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new()
			{
				CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/ZeroPortrait",
				PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0,
				PortraitPositionXOverride = 120,
				Position = new Vector2(100, 0),
				Scale = 0.75f,

			};
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

		public override void SetDefaults()
		{
			NPC.width = 110;
			NPC.height = 110;
            NPC.defense = 60;
            NPC.lifeMax = 40000;
			NPC.damage = Zero.calamityEnabled ? 360 : 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = Death;
			NPC.value = Item.buyPrice(1, 0, 0, 0); // money it drops
			NPC.knockBackResist = 0f; //how much knockback applies
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.boss = true;
			NPC.npcSlots = 16;

			NPC.lavaImmune = true;

            Music = MusicLoader.GetMusicSlot(Mod, "Music/02NewerWithMetadata");
            SceneEffectPriority = SceneEffectPriority.BossHigh; // By default, musicPriority is BossLow
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
		{
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, 1);//spawn 1 eyeball for every player
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            //uses AddRange to add multiple things instead of Add for simplicity
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new HyperzoneBackgroundProvider(), //I totally didn't reference the vanilla code what no way

				//bestiary description
				new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.KirboMod.NPCs.Bestiary.Zero")),
            });
        }

        public override void SendExtraAI(BinaryWriter writer) //syncing stuff
        {
			writer.Write(deathcounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			deathcounter = reader.ReadInt32();
        }
		public static void GetAIValues(out float[] ai2s)
		{
			List<float> targets = new List<float>();
			foreach (var item in Main.ActivePlayers)
			{
				targets.Add(item.whoAmI);
			}
			ai2s = targets.ToArray();
		}
        public override void AI() //constantly cycles each time
		{
            NPC.target = (int)NPC.ai[2];
			Player player = Main.player[NPC.target];
            

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
            float speed = 40f;
			float inertia = 50f;
			float chargespeed = 40;
			float chargereduce = 0; //shortens the time between charges and the time of charges
			float chargePoint = 300;

            bool spewFaster = false; //decide if to spew blood faster (whether or not eye is low)

            if (Vector2.Distance(player.Center, NPC.Center) > 2000) //player is far away
            {
                speed *= 2; //double speed
            }
            else if (NPC.GetLifePercent() <= 0.3f && Main.expertMode) //low & in expert mode
			{
				speed *= 1.25f; //25% faster speed
				inertia *= 1.5f; //50% faster acceleration

                chargereduce = chargePoint * 0.75f;
                chargespeed += 20;

				spewFaster = true;
            }

			Vector2 moveTo = player.Center;
			Vector2 direction = moveTo - NPC.Center; //start - end
			direction.Normalize();
			direction *= speed;

            NPC.ai[0]++;

            if (NPC.ai[0] < chargePoint - chargereduce)
			{
				NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia; //follow player

                if (NPC.ai[0] % (spewFaster ? 7 : 10) == 0) //only do this if less than 300 (or less)
                {
                    SpewEyeBlood();
                }
            }

			if (NPC.ai[0] >= chargePoint - chargereduce)
			{
                Vector2 chargedirection = player.Center - NPC.Center; //start - end

                if (NPC.GetLifePercent() <= 0.6f) //initate dash
				{
					if (NPC.ai[0] < chargePoint + 60 - chargereduce) //stop
					{
						NPC.velocity *= 0.01f; //freeze to warn player
                    }
					else if (NPC.ai[0] < chargePoint + 90 - chargereduce) //initiate dash
					{
						if (NPC.ai[0] == chargePoint + 60 - chargereduce)
						{
                            chargedirection.Normalize();
                            chargedirection *= chargespeed; //changes depending whether or not eye has less than 25% health
                            NPC.velocity = chargedirection; //charge
                        }

						if (NPC.ai[0] % 2 == 0)
						{
                            SpewEyeBlood();
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
			float desiredRotation = direction.ToRotation();

			NPC.rotation = desiredRotation;
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

		private int? SpewEyeBlood()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				return Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ZeroEyeBlood>(), NPC.damage / 4, 2f, Main.myPlayer);
			}

			return null;
        }

		public override void BossLoot(ref int potionType)
		{
			potionType = ItemID.SuperHealingPotion; //potion it drops
		}

        public override void OnKill()
        {
			if(!NPC.AnyNPCs(ModContent.NPCType<ZeroEye>()))
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedZeroBoss, -1);
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
			if (AnyZeroEyesAsideMe())
			{
                for (int i = 0; i < 60; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(40f, 40f);
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed, Scale: 3); //Makes dust in a messy circle
                    d.noGravity = true;
                }
				NPC.active = false;//kinda die but do not drop loot or anything like that
                return false;
			}
			return true;
		}
		bool AnyZeroEyesAsideMe()
		{
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC compare = Main.npc[i];
				if(compare.active && compare.type == ModContent.NPCType<ZeroEye>() && i != NPC.whoAmI)
				{
					return true;
				}
			}
			return false;
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
					Vector2 speed = Main.rand.NextVector2Circular(40f, 40f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed, Scale: 2.5f); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
			else if (deathcounter > 0) //death
			{
                NPC.HideStrikeDamage = true;
                NPC.SimpleStrikeNPC(999999, 1, false, 0, null, false, 0, false);
                for (int i = 0; i < 120; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(60f, 60f);
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.Redsidue>(), speed, Scale: 3.5f); //Makes dust in a messy circle
					d.noGravity = true;
				}
            }

		}
	}
}
