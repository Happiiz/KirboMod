using KirboMod.Bestiary;
using KirboMod.Items;
using KirboMod.Items.Accesories;
using KirboMod.Items.Ammo;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace KirboMod.NPCs
{
	public class Wheelie : ModNPC
	{
        public override void SetStaticDefaults() {
			Main.npcFrameCount[NPC.type] = 3;

            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        bool turning = false; //for visual

        ref float turnCounter => ref NPC.localAI[0];
        ref float wallTimer => ref NPC.localAI[1];
        ref float DropStarTimer => ref NPC.localAI[2];
        public override void SetDefaults() {
			NPC.width = 30;
			NPC.height = 28;
			NPC.lifeMax = 300;
			NPC.damage = 40;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath3;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.knockBackResist = 0f; //How much of the knockback it receives will actually apply
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.WheelieBanner>();
            NPC.aiStyle = -1;
			NPC.noGravity = false;
            NPC.rarity = 1;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
            if (Main.hardMode)
            {
                return SpawnCondition.GoblinScout.Chance * 1.2f; //a bit more common as a goblin scout
            }
            else
            {
                return 0f;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
			//uses AddRange to add multiple things instead of Add for simplicity
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				//set spawning conditions of NPC in bestiary
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				//bestiary description
				new FlavorTextBestiaryInfoElement("Stay out! This is Wheelie territory, and you're not fast enough for these rambunctious racers! But perhaps if you can show them up, one of them will help you ride around town...")
			}); 
        }

        public override void AI()
        {
            DropStarTimer++;
            if (DropStarTimer % 20 == 0)
            {
                KirbyTransformationModCompatibilityHelper.SpawnSingleDropStar(NPC.GetSource_FromAI(), NPC.Center, 150);
            }
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];

            float speed = 15;

            float turnRange = 400;

            //checks if Wheelie is facing away from the player from a far position
            bool behind = (NPC.Center.X < player.Center.X && NPC.direction == -1) || (NPC.Center.X > player.Center.X && NPC.direction == 1);

            bool onGround = NPC.velocity.Y == 0;

            float Xdistance = MathF.Abs(NPC.Center.X - player.Center.X);

            if (turnCounter < 0) //slow down while turning
            {
                turning = true;
                NPC.velocity.X *= 0.9f;
            }
            else //go at a set speed
            {
                if (turnCounter == 0)
                {
                    NPC.TargetClosest(true);
                }

                turning = false;
                NPC.velocity.X = NPC.direction * speed;

                if (NPC.Center.Y > player.Center.Y + 100 && onGround && !behind && Xdistance <= turnRange)
                {
                    float YDistance = player.Center.Y - NPC.Center.Y;

                    float XTime = Xdistance / MathF.Abs(NPC.velocity.X);

                    //jump with gravity accounted for
                    NPC.velocity.Y = Math.Clamp(YDistance / XTime - (NPC.gravity * XTime * .5f), -20, -3);
                }

                if (NPC.collideX)
                {
                    wallTimer++;
                    if (onGround)
                    {
                        NPC.velocity.Y = -10;
                    }
                    if (wallTimer > 120)
                    {
                        NPC.direction *= -1;
                        wallTimer = 0;
                    }
                }
                else
                {
                    wallTimer = 0;
                }

                //if passed player and hasn't turned recently
                if (behind && Xdistance > turnRange && onGround && !player.dead && turnCounter >= 20)
                {
                    turnCounter = -20;
                }
            }

            turnCounter++;

            //for stepping up tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WheelieLicense>(), 20));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DreamEssence>(), 1, 2, 4));
        }

        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (turning)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else
            {
                if (NPC.frameCounter < 5.0)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frameCounter < 10.0)
                {
                    NPC.frame.Y = frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
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

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.instance.LoadProjectile(NPC.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawOrigin = NPC.frame.Size() / 2;
                Vector2 drawPos = (NPC.oldPos[k] - Main.screenPosition) + new Vector2(0, NPC.gfxOffY) + drawOrigin;

                SpriteEffects direction = SpriteEffects.FlipHorizontally;
                if (NPC.direction == -1)
                {
                    direction = SpriteEffects.None;
                }

                Color color = drawColor * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, 1, direction, 0);
            }

            return true;
        }
	}
}
