using KirboMod.Items.Zero;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items;
using KirboMod.Items.Nightmare;
using KirboMod.Systems;
using System.IO;

namespace KirboMod.NPCs
{
	public partial class NightmareWizard : ModNPC
	{
        enum NightmareAttackType : byte
        {
            SpreadStars,//1
            RingStars,//2
            Swoop,//3
            Tornado,//4
            Stoop,//5
            LightningOrbsPentagon,
            LightningOrbsHoming
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nightmare");
            Main.npcFrameCount[NPC.type] = 25;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 70f,
                Position = new Vector2(0, 80),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        }

        public override void SetDefaults()
        {
            NPC.width = 114;
            NPC.height = 114;
            DrawOffsetY = 20;
            NPC.damage = 70;
            NPC.noTileCollide = true;
            NPC.defense = 15;
            NPC.lifeMax = 18000;
            NPC.HitSound = SoundID.NPCHit2; //bone
            NPC.DeathSound = SoundID.NPCDeath2; //undead
            NPC.value = Item.buyPrice(0, 5, 0, 0); // money it drops
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            Music = MusicID.Boss1;
            NPC.friendly = false;
            NPC.dontTakeDamage = true;
            NPC.npcSlots = 6;
            NPC.buffImmune[BuffID.Confused] = true;
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("An evil wizard bent on spreading nightmares to all that can dream. Its magic can twist the mind and body of those who are cursed with it.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(animation);
            writer.Write(attack);
            writer.Write(despawntimer);
            writer.Write((byte)attacktype);
            writer.Write((byte)lastattacktype);

            writer.Write(phase);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animation = reader.ReadInt32();
            attack = reader.ReadInt32();
            despawntimer = reader.ReadInt32();
            attacktype = (NightmareAttackType)reader.ReadByte();
            lastattacktype = (NightmareAttackType)reader.ReadByte();

            phase = reader.ReadByte();
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedNightmareBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<NightmareBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<NightCloth>(), 1, 15, 15));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<NightmareMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NightmareTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.NightmareRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Nightmare.NightmarePetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Nightmare"; //_ has been defeated!
            potionType = ItemID.GreaterHealingPotion; //potion it drops
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (deathCounter >= 360 && NPC.life <= 0)
            {
                for (int i = 0; i < 8; i++) 
                {
                    // go around in a octogonal pattern
                    Vector2 speed = new Vector2((float)Math.Cos(MathHelper.ToRadians(i * 45)) * 20, (float)Math.Sin(MathHelper.ToRadians(i * 45)) * 20);

                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.NightStar>(), speed, Scale: 3f); //Makes dust in an octogonal formation
                    d.noGravity = true;
                }

                for (int i = 0; i < 20; i++) 
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 2f); //double jump smoke
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}