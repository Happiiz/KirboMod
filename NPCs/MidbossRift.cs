using KirboMod.Items.Weapons;
using KirboMod.NPCs.MidBosses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace KirboMod.NPCs
{
    public class MidbossRift : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[NPC.type] = 1;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true
            };

            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 104;
            NPC.height = 142;
            NPC.life = 5;
            NPC.lifeMax = 5;
            NPC.defense = 0;
            NPC.knockBackResist = 0.00f; //recieves 0% of knockback
            NPC.friendly = false;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.damage = 0;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.Dig;
            NPC.dontCountMe = true; //I guess don't count towards npc total
            NPC.hide = true; //for drawing behind things
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            bool noOtherMidbosses = !NPC.AnyNPCs(Type) && !NPC.AnyNPCs(ModContent.NPCType<Bonkers>()) && !NPC.AnyNPCs(ModContent.NPCType<MrFrosty>());

            if (!spawnInfo.Invasion && !spawnInfo.Water && noOtherMidbosses) //no invasion, water, or other midbosses
            {
                if (spawnInfo.Player.ZoneSnow)
                {
                    return 0.01f; //spawn in snow biome with rare chance
                }
                else
                {
                    return SpawnCondition.GoblinScout.Chance * 0.1f; //spawn in outer sixths of world with the same chance of spawning as a goblin scout
                }
            }
            else
            {
                return 0f; //too far
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (NPC.ai[1] != 1) //spawned naturally instead of with DD
            {
                string text = "A dimensional rift has appeared with a challenging foe!";

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(text, 175, 75);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(175, 75, 255));
                }
            }

            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
        }

        public override void AI()
        {
            NPC.ai[0]++;

            NPC.TargetClosest(true); //target

            Player player = Main.player[NPC.target];

            if (NPC.ai[0] == 180) //summon
            {
                int index;

                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient) //multiplayer stuff
                {
                    if (player.ZoneSnow) //Mr. Frosty
                    {
                        index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y,
                            ModContent.NPCType<MrFrosty>(), Target: NPC.target);
                    }
                    else //Bonkers
                    {
                        index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y,
                            ModContent.NPCType<Bonkers>(), Target: NPC.target);
                    }

                    if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: index);
                    }
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle

                    Vector2 yOffset = new Vector2(0, -20);

                    Dust.NewDustPerfect(NPC.Center + yOffset, 15, speed, Scale: 1.5f); //fallen star dust
                }
            }

            if (NPC.ai[0] >= 240) //disappear
            {
                NPC.life = 0; //kill
            }
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index); //draw under tiles
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public static Asset<Texture2D> Rift;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Rift = ModContent.Request<Texture2D>(Texture);

            Texture2D rift = Rift.Value;

            Vector2 yOffset = new Vector2(0, -20); //move down with scale to match up with center

            float Xscale = Utils.GetLerpValue(0, 40, NPC.ai[0], true) * Utils.GetLerpValue(240, 200, NPC.ai[0], true);

            VFX.DrawGlowBallAdditive(NPC.Center + yOffset, Xscale * 1.5f, Color.DeepSkyBlue, Color.White);

            Vector2 scale = new Vector2(Xscale, 1);

            Main.EntitySpriteDraw(rift, NPC.Center - Main.screenPosition, NPC.frame, Color.White, 0, NPC.frame.Size() / 2, scale, SpriteEffects.None);

            return false;
        }
        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter++;
            if (NPC.frameCounter < 6)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 12)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 18)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 24)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = frameHeight * 4;
            }
            else
            {
                NPC.frameCounter = 0;
            }
        }
    }
}