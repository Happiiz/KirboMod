using KirboMod.Bestiary;
using KirboMod.Items;
using KirboMod.Items.Ammo;
using KirboMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace KirboMod.NPCs
{
	public class Bomber : ModNPC
	{
        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Waddle Dee");
			Main.npcFrameCount[NPC.type] = 2;
		}

        bool turnRed = false; //purely visual

		public override void SetDefaults() {
			NPC.width = 36;
			NPC.height = 32;
			NPC.lifeMax = 600;
			NPC.damage = 5;
            NPC.HitSound = SoundID.NPCHit4; //metal
            NPC.DeathSound = SoundID.NPCDeath14; //explosive metal
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.knockBackResist = 0f; //How much of the knockback it receives will actually apply
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.BomberBanner>();
            NPC.aiStyle = -1;
			NPC.noGravity = false;
            NPC.rarity = 5;

            NPC.direction = Main.rand.NextBool() ? 1 : -1;
            NPC.netUpdate = true;
            NPC.GravityIgnoresLiquid = true;
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
            if (Main.hardMode)
            {
                if (spawnInfo.Player.ZoneRockLayerHeight) //if player is within cave height
                {
                    return SpawnCondition.Cavern.Chance * 0.01f;
                }
                else
                {
                    return 0f; //no spawn rate
                }
            }

            return 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Don't let its cutesy, dinky appearance fool you. If it looks like a bomb, falls like a bomb, and explodes like a bomb... it's a bomb!")
			}); 
        }

        public override void AI() //constantly cycles each time
        {
            NPC.spriteDirection = NPC.direction;
            NPC.TargetClosest(false);

            bool falling = false;

            if (NPC.velocity.Y == 0)
            {
                NPC.velocity.X = NPC.direction * 2;

                NPC.localAI[0] = 0;
            }
            else
            {
                NPC.velocity.X *= 0.9f;

                NPC.localAI[0]++;

                if (NPC.localAI[0] > 30)
                {
                    falling = true;
                }
            }

            if (falling)
            {
                turnRed = true;
                NPC.rotation = MathF.PI / 2 * NPC.direction;

                for (int i = 0; i < NPC.width; i++) //checks if tiles are below Bomber
                {
                    Point tileLocation = (NPC.BottomLeft + new Vector2(i, 8)).ToTileCoordinates();

                    Tile tile = Main.tile[tileLocation];

                    if (WorldGen.SolidOrSlopedTile(tile) || Main.tileSolidTop[tile.TileType] || NPC.velocity.Y == 0 && NPC.localAI[2] == 0)
                    {
                        Explode();
                        NPC.localAI[2] = 1; //increase to prevent from expanding again

                        break;
                    }
                }
            }
            else
            {
                //turn around if touching wall
                if (NPC.collideX && NPC.localAI[1] > 0)
                {
                    NPC.direction *= -1;
                    NPC.localAI[1] = -10;
                }
            }

            NPC.localAI[1]++;

            //for stepping up tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }

        void Explode() //referenced from Scarfy code
        {
            //explode

            NPC.active = false;
            SoundEngine.PlaySound(SoundID.Item38 with { MaxInstances = 0 }, NPC.Center);

            float sizeMult = 10;
            NPC.Hitbox = Utils.CenteredRectangle(NPC.Center, NPC.Size * sizeMult);

            for (int j = 0; j < 200; j++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(NPC.Hitbox);
                Vector2 speed = (pos - NPC.Center) * Main.rand.NextFloat(0.02f, 0.1f); //spread outward

                Gore.NewGorePerfect(NPC.GetSource_FromAI(), pos, speed, Main.rand.Next(61, 64), Scale: 1f); //bomb smoke

                //reroll
                pos = Main.rand.NextVector2FromRectangle(NPC.Hitbox);
                speed = (pos - NPC.Center) * 0.02f;

                Dust d = Dust.NewDustPerfect(pos, DustID.Torch, speed, Scale: 3);

                Dust d2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke, Scale: 2);
            }

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player plr = Main.player[k];
                if (plr.active && !plr.dead && plr.Hitbox.Intersects(NPC.Hitbox))
                    plr.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 200, MathF.Sign(plr.Center.X - NPC.Center.X));
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.BomberRocket>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DreamEssence>(), 1, 24, 30));
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
            else
            {
                NPC.frameCounter = 0.0;
            }
        }

		public override void HitEffect(NPC.HitInfo hit)
		{
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++) //first section makes inital statement once //second declares the conditional they must follow // third declares the loop
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle edge
                    Gore.NewGorePerfect(NPC.GetSource_FromAI(), NPC.Center, speed, Main.rand.Next(16, 18));
                }
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (turnRed)
            {
                Lighting.AddLight(NPC.Center, TorchID.Red);
                return Color.Red;
            }

            Lighting.AddLight(NPC.Center, TorchID.Torch);
            return null;
        }
	}
}
