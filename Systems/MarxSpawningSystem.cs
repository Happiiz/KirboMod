using System;
using System.Collections.Generic;
using System.Security.Policy;
using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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

        public override void PostUpdateWorld()
        {
            if (!MarxHasAppeared && !UnlockedMarx && (DownedBossSystem.downedWhispyBoss || DownedBossSystem.downedKrackoBoss || DownedBossSystem.downedKrackoBoss || Main.hardMode))
            {
                CanMarxAppear = true;
            }
            else
            {
                CanMarxAppear = false;
            }

            if (CanMarxAppear && NPC.AnyDanger()) //rift spawning
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

                Player player = Main.LocalPlayer;

                if (!anyRifts && player.ZoneForest) //no marx rifts and the player is in a forest
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
        }
    }
}