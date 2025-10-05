using KirboMod.Projectiles;
using KirboMod.Projectiles.Marx;
using KirboMod.Systems;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace KirboMod.NPCs.Marx.Townie
{
	//Load NPC head icon
	[AutoloadHead]
	public class MarxTownie : ModNPC
	{
		public int hint = -1; //start at -1 so when it increases for the first time it starts at 0

		private static Profiles.StackedNPCProfile NPCProfile;

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 26;

			NPCID.Sets.ExtraFramesCount[Type] = 10;
			NPCID.Sets.AttackFrameCount[Type] = 5;
			NPCID.Sets.DangerDetectRange[Type] = 200;
            NPCID.Sets.PrettySafe[Type] = NPCID.Sets.DangerDetectRange[Type] / 2;
            NPCID.Sets.AttackType[Type] = 0;
			NPCID.Sets.AttackTime[Type] = 60;
			NPCID.Sets.AttackAverageChance[Type] = 3;

			NPCID.Sets.ShimmerTownTransform[Type] = true; 

			NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<MarxTownieEmote>();

			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Velocity = 1f,
				Direction = -1
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			NPC.Happiness
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Love)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Nurse, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.SantaClaus, AffectionLevel.Hate);

			//different profiles for different textures
			NPCProfile = new Profiles.StackedNPCProfile(
				new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
				new Profiles.DefaultNPCProfile(Texture + "_Shimmer", NPCHeadLoader.GetHeadSlot(HeadTexture))
			);
		}

		public override void SetDefaults() {
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 36;
			NPC.height = 36;
            DrawOffsetY = 32;
			NPC.aiStyle = 7;
			NPC.damage = 0;
			NPC.defense = 30;
			NPC.lifeMax = 25000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6; //ghastly sound (For playing up the "-has left!" effect)
			NPC.knockBackResist = 0.5f;

			AnimationType = NPCID.Guide;
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,

				//Bestiary flavor text
				new FlavorTextBestiaryInfoElement("A friendly face has appeared from a land of dreams to help you figure out his " +
                "world's weird visitors! He's a little weird, himself, but there's nothing wrong with that... right?"),

			});
		}

		public override void HitEffect(NPC.HitInfo hit) 
		{
			if (Main.netMode != NetmodeID.Server && NPC.life <= 0) //use smoke to signal "escape" instead of "death"
			{
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Main.rand.Next(11, 14), Main.rand.NextFloat() * 0.5f + 0.5f);
                }
            }
		}

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_SpawnNPC)
            {
                MarxSpawningSystem.UnlockedMarx = true; //secure the deal
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return MarxSpawningSystem.UnlockedMarx && !NPC.AnyNPCs(ModContent.NPCType<MarxBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<MarxPrelude>());
        }

		public override ITownNPCProfile TownNPCProfile() {
			return NPCProfile;
		}

		public override List<string> SetNPCNameList() 
		{
			return default; //use type name
		}

		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

            for (int i = 0; i < 5; i++) //more common than others
            {
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D1"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D2"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D3"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D4"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.D5"));
            }

            if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon)
            {
                for (int i = 0; i < 10; i++) //much more common
                {
                    chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DMoon"));
                }
            }

            if (NPC.IsShimmerVariant)
            {
                for (int i = 0; i < 5; i++)
                {
                    chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DMarxolor"));
                }
            }

            chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DShutTheHellUp"));
            chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DReference"));

            if (Main.hardMode)
            {
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby1"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby2"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby3"));
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby4"));
            }

            if (Main.LocalPlayer.wings == 45) //celestial starboard
            {
                chat.Add(Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DKirby5"));
            }

            string chosenChat = chat;

			if (!Main.LocalPlayer.GetModPlayer<KirbPlayer>().talkedToMarx) //first time chatting
			{
				chosenChat = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DInitial"); //guaranteed
            }

            Main.LocalPlayer.GetModPlayer<KirbPlayer>().talkedToMarx = true;

            return chosenChat;
		}

		public override void SetChatButtons(ref string button, ref string button2) { // What the chat buttons are when you open up the chat UI
			button = "Help";
		}
		
		public override void OnChatButtonClicked(bool firstButton, ref string shop) {
			if (firstButton) 
			{
                GetHint();

                if (hint > 15)  //restart if too high
                {
                    hint = -1;
                    GetHint(); //try again
                }
            }
		}

        private void GetHint()
        {
            Player player = Main.LocalPlayer;

            string? chosenText = null;

            hint++;

            #region Help Text

            while (chosenText == null)
            {
                if (hint == 0)
                {
                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HStarbits");
                }
                else if (hint == 1)
                {
                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HWeapons");
                }
                else if (hint == 2)
                {
                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HBosses");
                }
                else if (hint == 3)
                {
                    if (!DownedBossSystem.downedWhispyBoss)
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HWhispyWoods");
                    else
                        hint++;
                }
                else if (hint == 4)
                {
                    if (!DownedBossSystem.downedKrackoBoss)
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HKracko");
                    else
                        hint++;
                }
                else if (hint == 5)
                {
                    if (!DownedBossSystem.downedKingDededeBoss)
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HKingDedede");
                    else
                        hint++;
                }
                else if (hint == 6)
                {
                    if (NPC.downedBoss2)
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HMidbosses");
                    else
                        hint++;
                }
                else if (Main.hardMode)
                {
                    if (hint == 7)
                    {
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HDreamMatter");
                    }
                    else if (hint == 8)
                    {
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HRareStones");
                    }
                    else if (hint == 9)
                    {
                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HEvoWeapons");
                    }
                    else if (hint == 10)
                    {
                        if (!DownedBossSystem.downedNightmareBoss)
                            chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HNightmare");
                        else
                            hint++;
                    }
                    else if (NPC.downedPlantBoss)
                    {
                        if (hint == 11)
                        {
                            chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HHeartMatter");
                        }
                        else if (hint == 12)
                        {
                            //extra code is WIP

                            //if (!DownedBossSystem.downedMarxBoss)
                            chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HMarx");
                            //else
                            //hint++;
                        }
                        else if (NPC.downedGolemBoss)
                        {
                            if (hint == 13)
                            {
                                if (!DownedBossSystem.downedDarkMatterBoss)
                                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HDarkMatter");
                                else
                                    hint++;
                            }
                            else if (NPC.downedMoonlord)
                            {
                                if (hint == 14)
                                {
                                    if (!DownedBossSystem.downedZeroBoss)
                                        chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HZero");
                                    else
                                        hint++;
                                }
                                else if (hint == 15)
                                {
                                    chosenText = Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Help.HPostZero");
                                }
                                else
                                {
                                    hint = 0;
                                }
                            }
                            else
                            {
                                hint = 0;
                            }
                        }
                        else
                        {
                            hint = 0;
                        }
                    }
                    else
                    {
                        hint = 0;
                    }
                }
                else
                {
                    hint = 0;
                }
            }

            Main.npcChatText = chosenText;

            #endregion Help Text
        }

        public override bool CheckDead()
        {
            if (NPC.life <= 0)
            {
                NPCID.Sets.IsTownPet[Type] = true; //set to pet last second to make the "-has left!" text appear
            }
            return base.CheckDead();
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;


		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 30;
			knockback = 7f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
			projType = ModContent.ProjectileType<MarxBall>();
			attackDelay = 28; //7 fps
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 12f;
			randomOffset = 2f;
			gravityCorrection = -6;
		}

        public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
        {
            int food = Main.rand.NextFromList<int>(EmoteID.ItemCookedFish, EmoteID.ItemSoup, EmoteID.PartyCake, EmoteID.Hungry);

            if (!Main.afterPartyOfDoom)
            {
                for (int i = 0; i < 5; i++) //more likely
                {
                    emoteList.Add(food);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    emoteList.Add(EmoteID.EmoteEating);
                }
            }

            int reaction = EmoteID.EmoteHappiness;

            if (otherAnchor.entity is NPC { type: NPCID.SantaClaus }) //checks another NPC it's conversating with
            {
                return EmoteID.EmotionAnger;
            }
            else if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon)
            {
                reaction = EmoteID.WeatherRainbow;
            }
            else if (otherAnchor.entity is Player)
            {
                reaction = Main.rand.NextFromList<int>(EmoteID.EmoteWink, EmoteID.EmotionLove, EmoteID.EmoteNote, EmoteID.EmoteSilly);

                //vvv to be implemented vvv

                /* if (DownedBossSystem.DownedMarxBoss)
                 * {
                 *     for (int i = 0; i < 5; i++)
                       {
                           emoteList.Add(EmoteID.EmotionCry);
                       }
                 * }
                */
            }

            for (int i = 0; i < 5; i++) //more likely
            {
                emoteList.Add(reaction);
            }

            return null;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position = NPC.Bottom + Vector2.UnitY * 10;
            return true;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
    }
}