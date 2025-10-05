using KirboMod.Projectiles;
using KirboMod.Projectiles.Marx;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace KirboMod.NPCs.Marx.Townie
{
	public class MarxTownieDown : ModNPC
	{
		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 1;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true,
            };

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults() {
			NPC.friendly = true;
			NPC.width = 36;
			NPC.height = 36;
            DrawOffsetY = 32;
			NPC.aiStyle = 0;
			NPC.damage = 0;
			NPC.defense = 30;
			NPC.lifeMax = 25000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.knockBackResist = 0.5f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            MarxSpawningSystem.MarxHasAppeared = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneForest && !spawnInfo.Water && MarxSpawningSystem.MarxHasAppeared && NPC.AnyNPCs(Type))
            {
                return 0.1f;
            }
            else
            {
                return 0f;
            }
        }

        public override bool CanChat()
        {
            return true;
        }

        public override void AI() //referenced from vanilla source code
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && Main.player[i].talkNPC == NPC.whoAmI) //player interacts with NPC
                {
                    NPC.Transform(ModContent.NPCType<MarxTownie>()); //turn into Marx
                    Main.BestiaryTracker.Chats.RegisterChatStartWith(NPC); //register chat in Bestiary (I honestly don't know what this really does)
                    Main.player[i].SetTalkNPC(NPC.whoAmI); //talk
                    MarxSpawningSystem.UnlockedMarx = true; //secure the deal

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncTalkNPC, -1, -1, null, i);
                    }

                    break;
                }
            }
        }

        public override string GetChat()
        {
            return Language.GetTextValue("Mods.KirboMod.NPCs.MarxTownie.Dialogue.DInitial");
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position = NPC.Bottom + Vector2.UnitY * 10;
            return true;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
    }
}