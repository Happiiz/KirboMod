using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items.Kracko;
using KirboMod.Items.Armor.AirWalker;
using KirboMod.Items.Accesories;
using ReLogic.Content;
using KirboMod.Systems;
using Terraria.DataStructures;
using System.IO;
using KirboMod.Projectiles;
namespace KirboMod.NPCs
{
    [AutoloadBossHead]
    public partial class Kracko : ModNPC
    {
        enum KrackoAttackType : byte
        {
			DecideNext,//0
            SpinningBeamOrbs,//1
            Sweep,//2
            Dash,//3
			Lightning//4
        }

		public override void FindFrame(int frameHeight) // animation
		{
			if (animation == 0) //slow
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 12)
				{
					NPC.frame.Y = 0; //normal
				}
				else
				{
					NPC.frame.Y = frameHeight; //pulse
				}
				if (NPC.frameCounter >= 24)
				{
					NPC.frameCounter = 0;
				}
			}
			if (animation == 1) //fast
			{
				NPC.frameCounter++;
				if (NPC.frameCounter < 4)
				{
					NPC.frame.Y = 0; //normal
				}
				else
				{
					NPC.frame.Y = frameHeight; //pulse
				}
				if (NPC.frameCounter >= 8)
				{
					NPC.frameCounter = 0;
				}
			}
		}
		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedKrackoBoss, -1);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<KrackoBag>())); //only drops in expert

			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
			LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.Cloud, 1, 50, 50)); //50 clouds

			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<KrackoMask>(), 7));

			// Drop one of these 3 items with 100% chance
			notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<AirWalkerHelmet>(), ModContent.ItemType<AirWalkerBreastplate>(), ModContent.ItemType<AirWalkerLeggings>()));

			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PeeWeePole>(), 4));

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KrackoTrophy>(), 10)); //drop trophy

			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.KrackoRelic>()));

			masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Kracko.KrackoPetItem>(), 4));

			// add the rules
			npcLoot.Add(notExpertRule);
			npcLoot.Add(masterMode);
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "Kracko"; //_ has been defeated!
			potionType = ItemID.LesserHealingPotion; //potion it drops
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return true;
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(20f, 20f); //circle
					Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.LilStar>(), speed, Scale: 2f); //Makes dust in a messy circle
					d.noGravity = true;
				}
				for (int i = 0; i < 15; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
					Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(3f, 3f); //circle
					Dust d = Dust.NewDustPerfect(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)), DustID.Cloud, speed, Scale: 1f); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			 if (attacktype == KrackoAttackType.Lightning) //frenzy
			{
				int darkness = (int)Utils.Remap(NPC.ai[0], 30, 60, 255, 60);
				return new Color(darkness, darkness, darkness); //darken for thunder
			}
			else
			{
				return Color.White; //make it unaffected by light
			}
		}
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Kracko");
			Main.npcFrameCount[NPC.type] = 2;

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				CustomTexturePath = "KirboMod/NPCs/BestiaryTextures/KrackoPortrait",
				PortraitScale = 0.75f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,
				Position = new Vector2(2, 6), //Center the eye
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

		public override void SetDefaults()
		{
			NPC.width = 219; //hitbox less than sprite
			NPC.height = 150; //hitbox less than sprite
			DrawOffsetY = 29; //line up with middle
			NPC.damage = 30;
			NPC.defense = 12;
			NPC.noTileCollide = true;
			NPC.lifeMax = 4000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = Item.buyPrice(0, 5, 0, 0); // money it drops
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.lavaImmune = true;
			Music = MusicID.Boss1;
			//bossBag/* tModPorter Note: Removed. Spawn the treasure bag alongside other loot via npcLoot.Add(ItemDropRule.//bossBag(type)) */ = ModContent.ItemType<Items.Kracko.KrackoBag>(); //the expert mode treasure bag it drops
			NPC.friendly = false;
			NPC.npcSlots = 4;
			NPC.buffImmune[BuffID.Confused] = true;
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
			NPC.damage = (int)(NPC.damage * 0.6f);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Kracko is a living cloud that has lingered in the sky since ancient times, and thus has mastered the art of thunder!")
			});
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((byte)attacktype); //send non NPC.ai array info to servers
			writer.Write(doodelay); //send non NPC.ai array info to servers
			writer.Write(attackDirection); //send non NPC.ai array info to servers
			writer.Write((byte)lastattacktype); //send non NPC.ai array info to servers
			writer.Write(sweepheight); //send non NPC.ai array info to servers
			writer.Write(sweepX); //send non NPC.ai array info to servers
			writer.Write(sweepY); //send non NPC.ai array info to servers
			writer.Write(transitioning); //send non NPC.ai array info to servers
			writer.Write(frenzy); //send non NPC.ai array info to servers
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			attacktype = (KrackoAttackType)reader.ReadByte(); //sync in multiplayer
			doodelay = reader.ReadInt32(); //sync in multiplayer
			attackDirection = reader.ReadSByte(); //sync in multiplayer
			lastattacktype = (KrackoAttackType)reader.ReadByte(); //sync in multiplayer
			sweepheight = reader.ReadSingle(); //sync in multiplayer
			sweepX = reader.ReadSingle(); //sync in multiplayer
			sweepY = reader.ReadSingle(); //sync in multiplayer
			transitioning = reader.ReadBoolean(); //sync in multiplayer
			frenzy = reader.ReadBoolean(); //sync in multiplayer
		}
	}
}
