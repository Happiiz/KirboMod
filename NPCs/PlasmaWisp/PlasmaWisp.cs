using KirboMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using SoundEngine = Terraria.Audio.SoundEngine;

namespace KirboMod.NPCs.PlasmaWisp
{
    public class PlasmaWisp : ModNPC
    {
        private struct PlasmaWispFlame
        {
            float rotation;
            Vector2 positionOffset;
            NPC npc;
            Asset<Texture2D> texture;
            int timer;
            int randomNumber;
            Texture2D Tex { get => texture.Value; }
            Vector2 Origin { get => texture.Size() / 2f; }
            bool MoveUp { get => randomNumber % 3 == 0; }
            public PlasmaWispFlame(ModNPC mnpc)
            {
                randomNumber = Main.rand.Next(int.MaxValue);
                rotation = Main.rand.NextFloat(MathF.Tau);
                timer = 0;
                npc = mnpc.NPC;
                if (randomNumber % 3 == 0)
                {
                    positionOffset = Main.rand.NextVector2Circular(30, 20);
                }
                else
                {
                    positionOffset = Main.rand.NextVector2Circular(40, 30);
                }
                positionOffset.Y += 7;
                string texturePath = "KirboMod/NPCs/PlasmaWisp/";
                if (Main.rand.NextBool())
                {
                    texturePath += "fire_0";
                    texturePath += Main.rand.Next(1, 3);
                }
                else
                {
                    texturePath += "flame_0";
                    texturePath += Main.rand.Next(1, 5);
                }
                texture = ModContent.Request<Texture2D>(texturePath);
            }
            public static void Draw(ref List<PlasmaWispFlame> flames, Vector2 screenPos)
            {
                if (flames == null)
                {
                    return;
                }
                for (int i = 0; i < flames.Count; i++)
                {
                    flames[i].Draw(screenPos);
                }
                for (int i = 0; i < flames.Count; i++)
                {
                    flames[i].DrawWhite(screenPos);
                }
            }
            void DrawWhite(Vector2 screenPos)
            {
                GetDrawValues(out float opacity, out float scale, out _);
                scale *= .5f;
                Main.EntitySpriteDraw(Tex, npc.Center + positionOffset - screenPos, null, Color.Gray with { A = 0 } * opacity, rotation, Origin, scale, SpriteEffects.None);
            }

            private void GetDrawValues(out float opacity, out float scale, out float t)
            {
                opacity = Utils.GetLerpValue(0, 8, timer, true);
                t = MathF.Abs(positionOffset.X);
                if (!MoveUp)
                {
                    opacity *= Utils.GetLerpValue(0, 7, t, true);
                    scale = Utils.GetLerpValue(-6, 7, t, true);
                }
                else
                {
                    opacity = Utils.GetLerpValue(70, 50, timer, true);
                    scale = opacity * .5f;
                }
                Easing(ref scale);
                Easing(ref opacity);
                scale *= 0.2f;
                Easing(ref t);
            }

            void Draw(Vector2 screenPos)
            {
                GetDrawValues(out float opacity, out float scale, out float t);
                Main.EntitySpriteDraw(Tex, npc.Center + positionOffset - screenPos, null, Color.Lerp(new Color(41, 255, 90), Color.White, Utils.GetLerpValue(2, 6, t, true)) with { A = 128 } * opacity, rotation, Origin, scale, SpriteEffects.None);
            }
            public static void Update(ref List<PlasmaWispFlame> flames)
            {
                if (flames == null)
                {
                    return;
                }
                for (int i = 0; i < flames.Count; i++)
                {
                    PlasmaWispFlame flame = flames[i];
                    flame.Update(out bool remove);
                    flames[i] = flame;
                    if (remove)
                    {
                        flames.RemoveAt(i);
                        i--;
                    }
                }
            }
            void Update(out bool remove)
            {
                remove = timer > 69;
                if (timer > 2)
                {
                    float amountToMove = MoveUp ? .1f : .8f;
                    if (!MoveUp)
                    {
                        amountToMove += Utils.Remap(positionOffset.Y, 0, -4, 0, .2f, false);
                    }
                    positionOffset.X -= MathF.CopySign(amountToMove, positionOffset.X);
                }
                positionOffset.Y -= .7f;
                if (MoveUp)
                {
                    positionOffset.Y -= .4f;
                }
                rotation += ((npc.position.X + positionOffset.X) % 2) * .1f;
                timer++;
            }
            static void Easing(ref float progress)
            {
                progress = .5f - MathF.Cos(progress * MathF.PI) * .5f;
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            if (eyes == null)
            {
                eyes = ModContent.Request<Texture2D>("KirboMod/NPCs/PlasmaWisp/PlasmaWispEyes", AssetRequestMode.ImmediateLoad);
            }
            if (hand == null)
            {
                hand = ModContent.Request<Texture2D>("KirboMod/NPCs/PlasmaWisp/PlasmaWispHand", AssetRequestMode.ImmediateLoad);
            }
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true; //immune to not mess up movement
        }
        List<PlasmaWispFlame> flames = null;
        public override void SetDefaults()
        {
            NPC.width = 82;
            NPC.height = 70;
            DrawOffsetY = -2; //make sprite line up with hitbox
            NPC.damage = 60;
            NPC.defense = 25;
            NPC.lifeMax = 500;
            NPC.HitSound = SoundID.NPCHit5; //pixie
            NPC.DeathSound = SoundID.NPCDeath7; //pixie
            NPC.value = Item.buyPrice(0, 0, 4, 0); // money it drops
            NPC.knockBackResist = 0f; //how much knockback is applied
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<Items.Banners.PlasmaWispBanner>();
            NPC.aiStyle = -1;
            NPC.noGravity = true; //not effected by gravity
            NPC.noTileCollide = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneDirtLayerHeight & Main.hardMode || spawnInfo.Player.ZoneRockLayerHeight & Main.hardMode) //if player is within cave height
            {
                return spawnInfo.SpawnTileType == TileID.Dirt || spawnInfo.SpawnTileType == TileID.Stone ? .03f : 0f; //functions like a mini if else statement
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A wisp sparking high amounts of electricity and light. Tries to sniff out intruders of its domain.")
            });
        }
        void Particles()
        {
            if (flames == null)
            {
                flames = new();
            }
            if (Main.rand.NextBool())
                flames.Add(new(this));
            flames.Add(new(this));
            PlasmaWispFlame.Update(ref flames);
        }
        public override void AI() //constantly cycles each time
        {

            Particles();
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            //passive effects
            if (Main.rand.NextBool(4)) //1/4 chance
            {
                int index = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, -20f, 200, default, 1.5f); //dust
                Main.dust[index].velocity *= 0.3f;
                Main.dust[index].noGravity = true;
            }
            float speed = Main.expertMode ? 4 : 2; //top speed
            float inertia = 10f; //acceleration and decceleration speed

            if (player.dead == false) //player is alive
            {
                Vector2 direction = player.Center - NPC.Center; //start - end 

                direction.Normalize();
                direction *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia; //move
            }
            else //player is dead
            {
                Vector2 direction = NPC.Center + new Vector2(0, 50) - NPC.Center; //start - end 
                direction.Normalize();
                direction *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia; //go down
            }

            Vector2 direction2 = player.Center - NPC.Center; //start - end 
            bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            //checks if the plasmama is in range (or if already attacking)
            if (NPC.Distance(player.Center) <= 150 && NPC.ai[0] < 299)
            {
                NPC.ai[0] = 299;
            }
            if ((Math.Abs(direction2.X) < 1280 && direction2.Y < 640 && direction2.Y > -720) || NPC.ai[0] >= 300)
            {
                if (NPC.ai[0] == 299) //almost at 300
                {
                    if (lineOfSight && !player.dead) //if in line of sight and player is alive
                    {
                        NPC.ai[0]++; //go to 300
                    }
                    else
                    {
                        NPC.ai[0] = 299; //freeze until in line of sight
                    }
                }
                else
                {
                    NPC.ai[0]++;
                }
            }
            if (NPC.ai[0] >= 300) //attack phase
            {
                NPC.velocity *= 0f;
                if (NPC.ai[0] == 375 || NPC.ai[0] == 450 || NPC.ai[0] == 525)
                {
                    //setting projectiles
                    GetProjShootData(out int proj, out int projdamage, out Vector2 velocity, out float ai0, out SoundStyle soundToPlay);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, proj, projdamage / 2, 10, Main.myPlayer, ai0);            
                    }
                    for (int i = 0; i < 40; i++)
                    {
                        Vector2 speed2 = Main.rand.NextVector2CircularEdge(20, 20); //circle
                        Dust d = Dust.NewDustPerfect(NPC.Center, DustID.TerraBlade, speed2, Scale: 1f); //Makes dust in a messy circle
                        d.noGravity = true;
                    }
                    SoundEngine.PlaySound(soundToPlay, NPC.Center);
                }
            }
            if (NPC.ai[0] >= 600) //limit
            {
                NPC.ai[0] = 0;
            }
        }

        private void GetProjShootData(out int type, out int damage, out Vector2 velocity, out float ai0, out SoundStyle soundToPlay)
        {
            ai0 = 0;
            Player player = Main.player[NPC.target];
            velocity = Vector2.Normalize(player.Center - NPC.Center); //start - end 
            if (NPC.ai[0] == 375)
            {
                soundToPlay = SoundID.Item12;
                velocity *= 20;
                damage = 30;
                type = ModContent.ProjectileType<Projectiles.BadPlasmaZap>();
            }
            else if (NPC.ai[0] == 450)
            {
                soundToPlay = SoundID.Item33;
                velocity *= 30;
                damage = 40;
                type = ModContent.ProjectileType<Projectiles.BadPlasmaLaser>();
            }
            else
            {
                soundToPlay = SoundID.Item117;
                damage = 120;
                float shootSpeed = 13;
                velocity *= shootSpeed;
                Utils.ChaseResults results = Utils.GetChaseResults(NPC.Center, shootSpeed, player.Center, player.velocity);
                if (results.InterceptionHappens)
                {
                    ai0 = results.InterceptionTime;
                    velocity = results.ChaserVelocity;
                }
                else
                {
                    ai0 = NPC.Distance(player.Center) / shootSpeed;
                }
                type = ModContent.ProjectileType<Projectiles.BadPlasmaBlast>();
            }
            velocity /= ContentSamples.ProjectilesByType[type].MaxUpdates;
        }

        static Asset<Texture2D> eyes;
        static Asset<Texture2D> hand;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            PlasmaWispFlame.Draw(ref flames, screenPos);

            float time = (float)Main.timeForVisualEffects * .07f + NPC.whoAmI * 20392;
            GetHandPositionAndRotationOffsets(out Vector2 leftHand, out Vector2 rightHand, out float rotLeftHand, out float rotRightHand, out float xOffset, out SpriteEffects leftHandFx, out SpriteEffects rightHandFx);
            Vector2 offset = new Vector2(xOffset, MathF.Sin(time) * 7);
            Main.EntitySpriteDraw(eyes.Value, NPC.Center - screenPos + offset, null, Color.White * NPC.Opacity, NPC.rotation, eyes.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            Main.EntitySpriteDraw(hand.Value, NPC.Center - screenPos + rightHand, null, Color.White * NPC.Opacity, NPC.rotation + rotRightHand, eyes.Size() / 2, NPC.scale, rightHandFx);

            Main.EntitySpriteDraw(hand.Value, NPC.Center - screenPos + leftHand, null, Color.White * NPC.Opacity, NPC.rotation + rotLeftHand, eyes.Size() / 2, NPC.scale, leftHandFx);
            return false;
        }
        static void Easing(ref float t)
        {
            t = 0.5f - 0.5f * MathF.Cos(t * MathF.PI);
        }
        void GetHandPositionAndRotationOffsets(out Vector2 leftHand, out Vector2 rightHand, out float rotLeftHand, out float rotRightHand, out float xOffset, out SpriteEffects leftHandFx, out SpriteEffects rightHandFx)
        {
            leftHandFx = SpriteEffects.FlipHorizontally;
            rightHandFx = SpriteEffects.None;
            Vector2 offset;
            Vector2 toPlayer = Vector2.Zero;
            xOffset = 0;
            if (Main.player.IndexInRange(NPC.target))
            {
                Player plr = Main.player[NPC.target];
                toPlayer = NPC.DirectionTo(plr.Center);
                xOffset = MathHelper.Clamp((plr.Center.X - NPC.Center.X) * .2f, -16, 16);
            }
            float time = (float)Main.timeForVisualEffects * .11f + NPC.whoAmI * 20392;
            offset = new Vector2(32 + xOffset, 10);
            offset += time.ToRotationVector2() * 4;
            rightHand = offset;
            offset = new Vector2(xOffset - 32, 10);
            offset += (time + 1).ToRotationVector2() * 4;
            leftHand = offset;
            //600
            float endTimer = Utils.GetLerpValue(570, 530, NPC.ai[0], true);
            Easing(ref endTimer);
            //375 || 450 || 525
            float attackTimer = Utils.GetLerpValue(300, 350, NPC.ai[0], true);
            attackTimer *= attackTimer * endTimer;
            Easing(ref attackTimer);
            float handWobbleIntensity = Utils.GetLerpValue(330, 360, NPC.ai[0], true) * endTimer;
            Easing(ref handWobbleIntensity);
            rotLeftHand = -MathF.PI / 2;
            rotLeftHand = Utils.AngleLerp(0, rotLeftHand, attackTimer);
            rotRightHand = MathF.PI / 2;
            rotRightHand = Utils.AngleLerp(0, rotRightHand, attackTimer);
            float handWobble = MathF.Sin((float)Main.timeForVisualEffects * .4f) * 3 * handWobbleIntensity;
            offset = new Vector2(10 + xOffset, -35 + handWobble);
            rightHand = Vector2.Lerp(rightHand, offset, attackTimer);
            offset = new Vector2(xOffset - 10, -35 + handWobble);
            leftHand = Vector2.Lerp(leftHand, offset, attackTimer);

            //todo: lerp offset of hands towards direction to player * some magnitude that looks good
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                Particles();
            }
            if (NPC.ai[0] < 300) //float
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 7.0)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frameCounter < 14.0)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.frameCounter < 21.0)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (NPC.frameCounter < 28.0)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
            else //attack
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter < 5.0)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                else if (NPC.frameCounter < 10.0)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                }
            }
        }
        /*public override void OnKill()
		{
			if (Main.rand.NextBool(Main.expertMode ? 10 : 20))
			{
				Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Plasma>());
			}
			Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.DreamEssence>(), Main.rand.Next(5, 10));
		}*/

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Items.Weapons.Plasma>(), 20, 10)); // 1 in 20 (5%) chance in Normal. 1 in 10 (10%) chance in Expert

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DreamEssence>(), 1, 4, 8));
        }

        public override void HitEffect(NPC.HitInfo hit)
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
        public override void Unload()
        {
            eyes = null;
            hand = null;
        }
    }
}
