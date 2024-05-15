using KirboMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Twister
{
    public class Twister : ModNPC
    {
        private float counting;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Twister");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 34;
            NPC.damage = 30;
            NPC.defense = 14;
            NPC.lifeMax = 70;
            NPC.HitSound = SoundID.NPCHit4; //metal
            NPC.DeathSound = SoundID.NPCDeath14; //also metal
            NPC.value = Item.buyPrice(0, 0, 0, 10);
            NPC.knockBackResist = 0f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.TwisterBanner>();
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.GravityIgnoresLiquid = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneDirtLayerHeight || spawnInfo.Player.ZoneRockLayerHeight) //if player is within cave height
            {
                return spawnInfo.SpawnTileType == TileID.Marble ? .3f : 0f; //functions like a mini if else statement
            }
            else
            {
                return 0f; //no spawn rate
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Marble,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Look out! This little yellow top has the knack to spin violently towards intruders! One could wonder how it goes about doing so without eyes.")
            });
        }

        public override void AI() //constantly cycles each time
        {
            NPC.spriteDirection = 1; //only spin in 1 direction
            Player player = Main.player[NPC.target];
            NPC.localAI[0]++;
            if (!player.dead)
            {
                NPC.TargetClosest(true);
            }

            float speed = 10f; //top speed
            float inertia = 80f; //acceleration and decceleration speed

            Vector2 direction = new Vector2(NPC.direction * 50, 0); //start - end 
                                                                    //we put this instead of player.Center so it will always be moving top speed instead of slowing down when player is near

            direction.Normalize();
            direction *= speed;
            NPC.velocity.X = (NPC.velocity.X * (inertia - 1) + direction.X) / inertia; //use .X so it only effects horizontal movement

            //for stepping up tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.localAI[0]++;
            }
            counting += 1;
            if (counting < 5)
            {
                NPC.frame.Y = 0;
            }
            else if (counting < 10)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (counting < 15)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (counting < 20)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else
            {
                counting = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Tornado>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starbit>(), 1, 2, 4));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (NPC.life <= 0)
                {
                    for (int i = 0; i < 10; i++)
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
        }

        public static Asset<Texture2D> Twisty;
        public static Asset<Texture2D> lilHandBallThing;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.instance.LoadNPC(NPC.type);
            Twisty ??= ModContent.Request<Texture2D>(Texture);
            lilHandBallThing ??= ModContent.Request<Texture2D>("KirboMod/NPCs/Twister/TwisterDot");
            Texture2D texture = Twisty.Value;

            Vector2 drawOrigin = new Vector2(texture.Width / 2, 40);
            Vector2 drawPos = NPC.Center - Main.screenPosition + new Vector2(0f, 20 + NPC.gfxOffY);
            float handSpinRadius = 28;
            float handSpinSpeed = .09f;
            float handYOffset = -22;
            float rotation = MathHelper.ToRadians(NPC.velocity.X * 2);
            for (int i = 0; i < 2; i++)
            {
                float timer = NPC.localAI[0] * handSpinSpeed + i * MathF.PI;
                bool layerFront = (timer - MathF.PI / 2) % (MathF.Tau) > MathF.PI;
                if (layerFront)
                    continue;
                Vector2 handOffset = new Vector2(MathF.Sin(timer) * handSpinRadius, handYOffset).RotatedBy(rotation);
                Main.EntitySpriteDraw(lilHandBallThing.Value, drawPos + handOffset, null, drawColor, rotation, lilHandBallThing.Size() / 2, 1, default);

            }

            Main.EntitySpriteDraw(texture, drawPos, NPC.frame, drawColor, rotation, drawOrigin, 1f, SpriteEffects.None);

            for (int i = 0; i < 2; i++)
            {
                float timer = NPC.localAI[0] * handSpinSpeed + i * MathF.PI;
                bool layerFront = (timer - MathF.PI / 2) % (MathF.Tau) > MathF.PI;
                if (layerFront)
                {
                    Vector2 handOffset = new Vector2(MathF.Sin(timer) * handSpinRadius, handYOffset).RotatedBy(rotation);
                    Main.EntitySpriteDraw(lilHandBallThing.Value, drawPos + handOffset, null, drawColor, rotation, lilHandBallThing.Size() / 2, 1, default);
                }
            }

            return false;
        }
        public override void Unload()
        {
            Twisty = null;
            lilHandBallThing = null;
        }

    }
}
