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

namespace KirboMod.NPCs
{
    [AutoloadBossHead]
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
        }

		public override void SetDefaults()
		{
			NPC.width = 104;
			NPC.height = 154;
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
            Vector2 NPCSpawnLocation = new Vector2(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY);
            Vector2 distance = spawnInfo.Player.Center - NPCSpawnLocation;

            bool noOtherMidbosses = !NPC.AnyNPCs(Type) && !NPC.AnyNPCs(ModContent.NPCType<Bonkers>()) && !NPC.AnyNPCs(ModContent.NPCType<MrFrosty>());

            if (!spawnInfo.Invasion && !spawnInfo.Water && noOtherMidbosses) //no invasion, water, or other midbosses
            {
                if (spawnInfo.Player.ZoneSnow)
                {
                    return 0.2f; //spawn in snow biome with rare chance
                }
                else
                {
                    return SpawnCondition.GoblinScout.Chance * 1f; //spawn in outer sixths of world with the same chance of spawning as a goblin scout
                }
            }
            else
			{
				return 0f; //too far
			}
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("A dimensional rift has appeared with a challenging foe!", 175, 75);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("A dimensional rift has appeared with a challenging foe!"), new Color(175, 75, 255));
            }

            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
        }

        public override void AI()
		{
            NPC.ai[0]++;

            NPC.TargetClosest(true); //target
			NPC.spriteDirection = -1; //face same direction

            Player player = Main.player[NPC.target];

            /*if (NPC.ai[0] % 20 == 0) 
            {
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop, NPC.Center);
            }*/

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
            }

			if (NPC.ai[0] >= 240) //disappear
			{
                NPC.life = 0; //kill
            }
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index); //draw under stuff
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White; //unaffected by light
        }  

		/*// This npc uses additional textures for drawing
        public static Asset<Texture2D> Rift;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Rift = ModContent.Request<Texture2D>(NPC.ModNPC.Texture);
            
            if (NPC.ai[0] < 60)
            {
                Texture2D rift = Rift.Value;
                rift.Width = 1;
            }

            return false;
        }*/
    }
}