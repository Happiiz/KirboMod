using KirboMod.Items.DarkMatter;
using KirboMod.Items.Zero;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Bestiary;
using KirboMod.Systems;
using Terraria.DataStructures;
using System.IO;

namespace KirboMod.NPCs
{
	public partial class DarkMatter : ModNPC
	{
        enum DarkMatterAttackType : byte
        {
            DarkBeams,//1
            Dash,//2
            Orbs,//3
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Matter");
            Main.npcFrameCount[NPC.type] = 18;

            // Add this in for bosses that have a summon item, requires corresponding code in the item
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true, //hide from bestiary
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true; //immune to all buffs that aren't whips

            //for drawing afterimages and stuff alike
            ProjectileID.Sets.TrailCacheLength[NPC.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[NPC.type] = 0; // The recording mode
        }

        public override void Load() //I use this to allow me to use the boss head
        {
            Mod.AddBossHeadTexture("KirboMod/NPCs/DarkMatter_Head_Boss2");
        }

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 130;
            DrawOffsetY = 30;
            NPC.damage = 100;
            NPC.noTileCollide = true;
            NPC.defense = 50;
            NPC.lifeMax = 52000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 19, 9, 5); // money it drops
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 12;
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
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7 * balance);
            NPC.damage = (int)(NPC.damage * 0.6);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new SurfaceBackgroundProvider(), //I totally didn't steal this code
				// Sets the spawning conditions of this NPC that is listed in the bestiary.

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A mysterious monocular invader with a body as black as darkness itself. What hidden motives does it have descending upon this world?")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //send non NPC.ai array info to servers
            writer.Write((byte)attacktype); 
            writer.Write((byte)lastattacktype); 

            writer.Write(animation);
            writer.Write(phase);

            writer.WriteVector2(playerTargetArea);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //sync in multiplayer
            attacktype = (DarkMatterAttackType)reader.ReadByte(); 
            lastattacktype = (DarkMatterAttackType)reader.ReadByte();

            animation = reader.ReadInt32();
            phase = reader.ReadInt32();

            playerTargetArea = reader.ReadVector2();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;

            position.Y = NPC.position.Y + NPC.height + 40;

            if (NPC.life == 1)
            {
                return false; //no health bar
            }
            else //health bar
            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Dark Matter"; //_ has been defeated!
            potionType = ItemID.GreaterHealingPotion; //potion it drops
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (phase == 1)
            {
                for (int i = 0; i < 1; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 3, 1); //Makes dust in a messy circle
                    d.noGravity = true;
                }
            }
            if (phase == 3)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 2, 2); //Makes dust in a messy circle
                    d.noGravity = false;
                }
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 40; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 4, 10); //Makes dust in a messy circle
                    d.noGravity = true;
                }
            }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDarkMatterBoss, -1);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * NPC.Opacity; //make it unaffected by light
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DarkMatterBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new LeadingConditionRule(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.DarkMaterial>(), 1, 30, 30));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DarkMatterMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkMatterTrophy>(), 10)); //drop trophy

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.DarkMatterRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.DarkMatter.DarkMatterPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }
    }
}
