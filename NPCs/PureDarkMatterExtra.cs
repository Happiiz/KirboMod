using KirboMod.Bestiary;
using KirboMod.Items.DarkMatter;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public partial class PureDarkMatter : ModNPC
    {
        public static SoundStyle LaserSFX => new SoundStyle("KirboMod/Sounds/NPC/DarkMatter/PureDarkMatterLaser") with { MaxInstances = 0 };
        public static SoundStyle PetalThrowSFX => new SoundStyle("KirboMod/Sounds/NPC/DarkMatter/PureDarkMatterPetalThrow");
        public static SoundStyle DashSFX => new SoundStyle("KirboMod/Sounds/NPC/DarkMatter/PureDarkMatterDash") with { Volume = 2f };
        public static SoundStyle BeamSFX => new SoundStyle("KirboMod/Sounds/NPC/DarkMatter/PureDarkMatterBeam") with { Volume = 0.8f };
        enum DarkMatterAttackType : byte
        {
            Petals,//1
            Dash,//2
            Lasers,//3
            Spin,//4
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dark Matter");
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                //CustomTexturePath = ,
                PortraitScale = 1, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0,
                PortraitPositionXOverride = 20,
                Position = new Vector2(20, 0),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true; //immune to all buffs that aren't whips
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
            NPC.value = Item.buyPrice(0, 19, 9, 5); // money it drops
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
            Music = MusicID.Boss1;
            NPC.buffImmune[BuffID.Confused] = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            Helper.BossHpScalingForHigherDifficulty(ref NPC.lifeMax, balance);
            //NPC.damage = (int)(NPC.damage * 0.6);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //send non NPC.ai array info to servers
            writer.Write((byte)attacktype);
            writer.Write((byte)lastattacktype);

            writer.Write(attackTurn);
            writer.Write(phase);

            writer.WriteVector2(spot);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //sync in multiplayer
            attacktype = (DarkMatterAttackType)reader.ReadByte();
            lastattacktype = (DarkMatterAttackType)reader.ReadByte();

            attackTurn = reader.ReadInt32();
            phase = reader.ReadInt32();

            spot = reader.ReadVector2();
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

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDarkMatterBoss, -1);
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
                for (int i = 0; i < 80; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 4, 10, Scale: 2); //Makes dust in a messy circle
                    d.noGravity = true;
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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DarkMatterBag>())); //only drops in expert

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert()); //checks if not expert
            LeadingConditionRule masterMode = new(new Conditions.IsMasterMode()); //checks if master mode

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.DarkMaterial>(), 1, 30, 30));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DarkMatterMask>(), 7));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkMatterTrophy>(), 10)); //drop trophy

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.BossRelics.DarkMatterRelic>()));

            masterMode.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.DarkMatter.DarkMatterPetItem>(), 4));

            // add the rules
            npcLoot.Add(notExpertRule);
            npcLoot.Add(masterMode);
        }
        void PlayBeamSFX()
        {
            SoundEngine.PlaySound(SoundID.Item158, NPC.Center);
        }
    }
}