using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace KirboMod.NPCs
{
	public class OrangeButterfly : ModNPC
	{
		public override void SetStaticDefaults() {
			Main.npcFrameCount[NPC.type] = 3;
		}

		public override void SetDefaults() {
			NPC.width = 24;
			NPC.height = 24;
			NPC.defense = 0; 
            //underscore just for clarity (86 million HP)
			NPC.lifeMax = 86_000_000;
			NPC.damage = 0;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 0f; // money it drops
			NPC.knockBackResist = 0f; //how much knockback applies
			Banner = NPC.type;
			NPC.aiStyle = NPCAIStyleID.Butterfly;
			AIType = NPCID.Butterfly;
			NPC.noGravity = true;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
            return SpawnCondition.OverworldDayGrassCritter.Chance * 0.01f; //spawn with 1/100th the chance of a regular forest critter
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            //uses AddRange to add multiple things instead of Add for simplicity
            bestiaryEntry.Info.AddRange(
            [
				//set spawning conditions of NPC in bestiary
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				//bestiary description
				new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.KirboMod.NPCs.Bestiary.OrangeButterfly"))
            ]);
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
        }

        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 10.0)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 20.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 30.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 40.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else
            {
                NPC.frameCounter = 0.0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
		{
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Lava, speed); //Makes dust in a messy circle
                    d.noGravity = false;
                }
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Dust d = Dust.NewDustPerfect(NPC.Center, DustID.Lava, speed); //Makes dust in a messy circle
                d.noGravity = false;
            }
        }
        public override void OnCaughtBy(Player player, Item item, bool failed)
        {

            if(Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMethods.SyncMorphoButterflyVanish((byte)NPC.whoAmI);
            }
            FailedCatchDust(NPC.Center);
            NPC.active = false;
        }
        public static void FailedCatchDust(Vector2 position)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
                Dust d = Dust.NewDustPerfect(position, ModContent.DustType<Dusts.RainbowSparkle>(), speed, Scale: 0.5f); //Makes dust in a messy circle
            }
        }
	}
}
