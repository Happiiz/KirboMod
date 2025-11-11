using KirboMod.Items.Weapons;
using KirboMod.NPCs.MidBosses;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx.Townie
{
    public class MarxPrelude : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        const int transPoint = 300;

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 36;
            DrawOffsetY = 40;
            NPC.damage = 0;
            NPC.lifeMax = 20;
            NPC.friendly = false;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.velocity.Y = 15;

        }

        public override void AI()
        {
            NPC.ai[0]++;

            NPC.TargetClosest(false);

            NPC.Opacity = Utils.Remap(NPC.ai[0], 0, 30, 0, 1, true);

            NPC.velocity *= 0.9f;


            if (MarxSpawningSystem.UnlockedMarx)
            {
                if (NPC.ai[0] == 1)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.KirboMod.NPCs.MarxPrelude.Dialogue.1"), Color.Violet);
                }

                if (NPC.ai[0] == transPoint * 0.5f)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.KirboMod.NPCs.MarxPrelude.Dialogue.2"), Color.Violet);
                }

                if (NPC.ai[0] == transPoint)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.KirboMod.NPCs.MarxPrelude.Dialogue.3"), Color.Violet);
                }
            }
            else
            {
                if (NPC.ai[0] == 1)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.KirboMod.NPCs.MarxPrelude.Dialogue.1Stranger"), Color.Violet);
                }

                if (NPC.ai[0] == transPoint * 0.5f)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.KirboMod.NPCs.MarxPrelude.Dialogue.2Stranger"), Color.Violet);
                }
            }

            if (NPC.ai[0] >= transPoint)
            {
                NPC.active = false; //disable NPC

                if (Main.netMode != NetmodeID.MultiplayerClient) // If not a client
                {
                    NPC.SpawnBoss((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MarxBoss>(), NPC.target);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.ai[0] < transPoint - 20)
            {
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
        }
    }
}