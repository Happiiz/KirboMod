using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace KirboMod.Systems
{
    public class MarxSpawningSystem : ModSystem
    {
        public static bool CanMarxAppear = false;
        public static bool MarxHasAppeared = false;
        public static bool UnlockedMarx = false;
        /// <summary>
        /// Checks whether or not Marx Boss has been summoned and not defeated in that world before
        /// </summary>
        public static bool MarxActive = false;

        public override void PostUpdateWorld()
        {
            if (!MarxHasAppeared && !UnlockedMarx && (DownedBossSystem.downedWhispyBoss || DownedBossSystem.downedKrackoBoss || DownedBossSystem.downedKingDededeBoss || Main.hardMode))
            {
                CanMarxAppear = true;
            }
            else
            {
                CanMarxAppear = false;
            }

            if (DownedBossSystem.downedMarxBoss)
            {
                MarxActive = false; //an extra measure just in case it doesn't disable in boss code for whatever reason
            }

            if (CanMarxAppear && !NPC.AnyDanger()) //rift spawning
            {
                bool anyRifts = false;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile starRift = Main.projectile[i];

                    if (starRift.type == ModContent.ProjectileType<MidbossRift>())
                    {
                        anyRifts = true;
                        break;
                    }
                }
                
                Player player = Main.LocalPlayer; //initial set for singleplayer

                bool foundPlayer = true; //always have a player in singleplayer

                if (Main.netMode == NetmodeID.Server) //if being executed on the server
                {
                    foundPlayer = false; //the server now needs to find a player

                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player Itplayer = Main.player[i];

                        if (!Itplayer.dead && Itplayer.active) //first check if the player isn't dead and is active...
                        {
                            if (Itplayer.ZoneForest) //...then check if they're in the right place. If so, then focus on that player and stop the loop
                            {
                                foundPlayer = true;
                                player = Itplayer;
                                break;
                            }
                        }
                    }
                }

                if (!anyRifts && player.ZoneForest && !player.dead && foundPlayer) //no marx rifts and a player is in a forest
                {
                    for (int w = 20; w > 0; w--)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            Vector2 riftLocation = new Vector2(player.Center.X - 300 + 600 / w, player.Center.Y - 30 * l);
                            Point riftTileLocation = riftLocation.ToTileCoordinates();

                            if (!Main.tile[riftTileLocation].HasTile && Main.rand.NextFloat() < 0.001f) //summon ze rift after a random amount of time
                            {
                                Projectile.NewProjectile(new EntitySource_WorldEvent(), riftLocation - Vector2.UnitY * 200, default, ModContent.ProjectileType<MidbossRift>(), -1, 0, Main.myPlayer, ai1: 0);
                                break;
                            }
                        }
                    }
                }
            }
        }

        //pretty much doing the same thing as downedbosssystem vvv

        public override void SaveWorldData(TagCompound tag)
        {
            if (CanMarxAppear)
            {
                tag.Add("CanMarxAppear", CanMarxAppear);
            }
            if (MarxHasAppeared)
            {
                tag.Add("MarxHasAppeared", MarxHasAppeared);
            }
            if (UnlockedMarx)
            {
                tag.Add("UnlockedMarx", UnlockedMarx);
            }
            if (MarxActive)
            {
                tag.Add("MarxActive", MarxActive);
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("CanMarxAppear"))
            {
                CanMarxAppear = tag.Get<bool>("CanMarxAppear");
            }
            if (tag.ContainsKey("MarxHasAppeared"))
            {
                MarxHasAppeared = tag.Get<bool>("MarxHasAppeared");
            }
            if (tag.ContainsKey("UnlockedMarx"))
            {
                UnlockedMarx = tag.Get<bool>("UnlockedMarx");
            }
            if (tag.ContainsKey("MarxActive"))
            {
                MarxActive = tag.Get<bool>("MarxActive");
            }
        }


        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = CanMarxAppear;
            flags[1] = MarxHasAppeared;
            flags[2] = UnlockedMarx;
            flags[3] = MarxActive;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            CanMarxAppear = flags[0];
            MarxHasAppeared = flags[1];
            UnlockedMarx = flags[2];
            MarxActive = flags[3];
        }
    }
}