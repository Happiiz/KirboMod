﻿using KirboMod.Items.Accesories;
using KirboMod.Items.Armor.AirWalker;
using KirboMod.Items.Kracko;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
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
        public static void AddElectrifiedDebuff(Player plr)
        {
            int duration = 30;
            if (Main.masterMode)
            {
                duration *= 3;
            }
            else if (Main.expertMode)
            {
                duration *= 2;
            }
            plr.AddBuff(BuffID.Electrified, duration);
        }
        public static SoundStyle ElecOrbsSFX => new SoundStyle("KirboMod/Sounds/NPC/Kracko/KrackoElecOrbs").WithVolumeScale(0.3f);
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

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new(new Conditions.IsMasterMode()); //checks if master mode

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
                for (int i = 0; i < 8; i++)
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 25, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 25);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.BoldStar>(), speed, Scale: 3f); //Makes dust in a messy circle
                    d.noGravity = true;
                }
                for (int i = 0; i < 20; i++)
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
        float DarknessProgressForThunder => Utils.GetLerpValue(15, 40, NPC.ai[0], true);
        public override Color? GetAlpha(Color lightColor)
        {

            if (attacktype == KrackoAttackType.Lightning)
            {
                int darkness = (int)MathHelper.Lerp(255, 60, DarknessProgressForThunder);
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

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }

        public override void SetDefaults()
        {
            NPC.width = 219; //hitbox less than sprite
            NPC.height = 150; //hitbox less than sprite
            DrawOffsetY = 29; //line up with middle
            NPC.damage = 27;
            NPC.defense = 12;
            NPC.noTileCollide = true;
            NPC.lifeMax = 5000;
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
            if (!Main.dedServ)//if not dedicated server
            {
                int musicSlot = MusicLoader.GetMusicSlot("KirboMod/Music/Evobyte_K1Boss");
                Music = musicSlot;
                Main.musicFade[musicSlot] = 1;
                Main.musicNoCrossFade[musicSlot] = true;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);
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
            writer.Write((byte)doodelay); //send non NPC.ai array info to servers
            writer.Write((byte)nextAttackType); //send non NPC.ai array info to servers
            BitsByte b = new(transitioning, frenzy, attackDirection);
            writer.Write(b); //send non NPC.ai array info to servers
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attacktype = (KrackoAttackType)reader.ReadByte(); //sync in multiplayer
            doodelay = reader.ReadByte(); //sync in multiplayer
            nextAttackType = (KrackoAttackType)reader.ReadByte(); //sync in multiplayer
            BitsByte b = reader.ReadByte(); //sync in multiplayer
            b.Retrieve(ref transitioning, ref frenzy, ref attackDirection);
        }
    }
}
