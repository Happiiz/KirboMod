using KirboMod.Items.NewWhispy;
using KirboMod.Items.RainbowSword;
using KirboMod.NPCs;
using KirboMod.Projectiles;
using KirboMod.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KirboMod
{
    public static class NetMethods
    {
        private enum ModPacketType : byte
        {
            //byte: playerWhoAmI
            StartFinalCutter = 0,
            //byte: playerWhoAmI, byte: number of npcs, bytes: indexes of npcs caught in effect
            StartFinalCutterMultiNPC = 1,
            /// <summary>
            /// changes the player's plasma charge.<br/>
            /// byte: player whoAmI<br/>
            /// sbyte: amount to change
            /// </summary>
            PlasmaChargeChange = 2,
            /// <summary>
            /// sets the player's right click bool in the array to false.<br/>
            /// byte: player whoAmI
            /// </summary>
            PlayerRightClickFalse = 6,
            /// <summary>
            /// sets the player's right click bool in the array to true.<br/>
            /// byte: player whoAmI
            /// /// </summary>
            PlayerRightClickTrue = 7,
            /// <summary>
            /// updates the player's position.<br/>
            /// byte: player whoAmI.<br/>
            /// Vector2: player position (not center!).
            /// </summary>
            PlayerPosition = 8,
            /// <summary>
            /// updates the player's position and velocity.<br/>
            /// byte: player whoAmI. Vector2: player position(not center!). Vector2 player velocity.
            /// </summary>
            PlayerPositionAndVelocity = 9,
            /// <summary>
            /// spawns whispy woods boss<br/>
            /// byte: player index<br/>
            /// int: tileX<br/>
            /// int: tileY
            /// </summary>
            SpawnWhispy = 10,
            /// <summary>
            /// syncs a projectile's position<br/>
            /// byte: projectile.identity of the projectile to sync<br/>
            /// Vector2: projectile.position(not center!)<br/>
            /// byte: player whoAmI of client that called the method
            /// </summary>
            ProjectilePosition = 11,
            /// <summary>
            /// spawnsn nightmare power orb<br/>
            /// byte: player index<br/>
            /// int: tileX<br/>
            /// int: tileY
            /// </summary>
            SpawnNightmareOrb = 12,
            /// <summary>
            /// byte: butterfly NPC whoAmI
            /// </summary>
            MorphoButterflyVanish = 13,
            //short: proj identity
            //byte: npc hit index
            /// <summary>
            /// for projectiles that implement IHitCdSync<br/>
            /// short: proj identity of the projectile being synced<br/>
            /// byte: whoAmI of the npc hit<br/>
            /// </summary>
            ProjHitCdSync = 14,
            /// <summary>
            /// byte: proj owner
            /// short: proj identity
            /// </summary>
            NewTripleStarStarIdentity = 15,
            /// <summary>
            /// byte: proj owner
            /// short: proj identity
            /// </summary>
            ClearTripleStarStarIdentity = 16,
            /// <summary>
            /// byte: client to reset the identities of
            /// </summary>
            BugfixResetTripleStarStarIdentities = 17,
            /// <summary>
            /// vector2 = hit effect position<br></br>
            /// sbyte = sword hit direction sign<br></br>
            /// byte = player whoAmI that is using the sword
            /// </summary>
            RainbowSwordHit = 18,
        }
        //initially called on the client that owns the projectile
        public static void SyncProjPosition(Projectile proj, byte playerWhoAmI)
        {
            ModPacket packet = KirboMod.instance.GetPacket();
            packet.Write((byte)ModPacketType.ProjectilePosition);
            packet.Write((short)proj.identity);
            packet.WriteVector2(proj.position);
            packet.Write(playerWhoAmI);
            packet.Send(-1, playerWhoAmI);
        }
        static void ReadSyncProjPosition(BinaryReader reader)
        {
            int identity = reader.ReadInt16();
            Vector2 pos = reader.ReadVector2();
            byte projOwner = reader.ReadByte();
            Projectile proj = Main.projectile.FirstOrDefault(p => p.identity == identity && p.active && p.owner == projOwner);
            if (proj != default)
            {
                proj.position = pos;
                if (Main.dedServ)//if server, re-send the packet to the other clients
                {
                    SyncProjPosition(proj, projOwner);
                }
            }
        }
        public static void SpawnWhispy(int tileX, int tileY)
        {
            ModPacket packet = KirboMod.instance.GetPacket();
            packet.Write((byte)ModPacketType.SpawnWhispy);
            packet.Write((byte)Main.myPlayer);
            packet.Write(tileX);
            packet.Write(tileY);
            packet.Send();
        }
        public static void SpawnNightmareOrb(int tileX, int tileY)
        {
            ModPacket packet = KirboMod.instance.GetPacket();
            packet.Write((byte)ModPacketType.SpawnNightmareOrb);
            packet.Write((byte)Main.myPlayer);
            packet.Write(tileX);
            packet.Write(tileY);
            packet.Send();
        }
        public static void SyncPlayerRightClick(Player plr)
        {
            if (plr.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient && KirbPlayer.playerRightClicks[plr.whoAmI] != Main.mouseRight && (Main.HoverItem.IsAir))
            {
                KirbPlayer.playerRightClicks[plr.whoAmI] = Main.mouseRight;
                ModPacket p = KirboMod.instance.GetPacket();
                p.Write((byte)(Main.mouseRight ? ModPacketType.PlayerRightClickTrue : ModPacketType.PlayerRightClickFalse));
                p.Write(plr.whoAmI);
                p.Send();
            }
        }
        public static void SyncPlayerPosition(Player plr)
        {
            ModPacket packet = KirboMod.instance.GetPacket();
            packet.Write((byte)ModPacketType.PlayerPosition);
            packet.Write((byte)plr.whoAmI);
            packet.WriteVector2(plr.position);
            packet.Send(-1, plr.whoAmI);
        }
        public static void SyncPlayerPosition(int whoAmI)
        {
            SyncPlayerPosition(Main.player[whoAmI]);
        }
        public static void SyncPlasmaChargeChange(Player plr, sbyte amountToChange)
        {
            ModPacket p = KirboMod.instance.GetPacket();
            p.Write((byte)ModPacketType.PlasmaChargeChange);
            p.Write((byte)plr.whoAmI);
            p.Write(amountToChange);
            p.Send(-1, plr.whoAmI);
        }
        public static void SyncPlayerPositionAndVelocity(Player plr)
        {
            ModPacket packet = KirboMod.instance.GetPacket();
            packet.Write((byte)ModPacketType.PlayerPositionAndVelocity);
            packet.Write((byte)plr.whoAmI);
            packet.WriteVector2(plr.position);
            packet.WriteVector2(plr.velocity);
            packet.Send(-1, plr.whoAmI);
        }



        public static void HandlePacket(BinaryReader reader)
        {
            ModPacketType packetType = (ModPacketType)reader.ReadByte();
            switch (packetType)
            {
                //case ModPacketType.StartFinalCutter:
                //    if (npcsInFinalCutter.Count == 1)
                //    {
                //        packet = Mod.GetPacket(3);
                //        packet.Write((byte)KirboMod.ModPacketType.StartFinalCutter);
                //        packet.Write((byte)Main.myPlayer);
                //        packet.Write((byte)npcsInFinalCutter[0].whoAmI);
                //        packet.Send(-1, Main.myPlayer);
                //        return true;
                //    }
                //    Player plr = Main.player[reader.ReadByte()];
                //    KirbPlayer kPlr = plr.GetModPlayer<KirbPlayer>();
                //    kPlr.TryStartingFinalCutter
                //    break;
                //case ModPacketType.StartFinalCutterMultiNPC:
                //    packet = Mod.GetPacket();
                //    packet.Write((byte)KirboMod.ModPacketType.StartFinalCutterMultiNPC);
                //    packet.Write((byte)Main.myPlayer);
                //    packet.Write((byte)npcsInFinalCutter.Count);
                //    for (int i = 0; i < npcsInFinalCutter.Count; i++)
                //    {
                //        packet.Write((byte)npcsInFinalCutter[i].whoAmI);
                //    }
                //    packet.Send(-1, Main.myPlayer);
                //    break;
                case ModPacketType.PlasmaChargeChange:
                    //code is executed here once for the server, and then again on the other clients.
                    byte plrWhoAmI = reader.ReadByte();
                    Player plr = Main.player[plrWhoAmI];
                    sbyte amountToChange = reader.ReadSByte();
                    KirbPlayer mplr = plr.GetModPlayer<KirbPlayer>();
                    mplr.ModifyPlasmaChargeAndResetPlasmaChargeDecayTimer_NoNetMessageSend(amountToChange);
                    if (Main.dedServ)
                    {
                        //ModPacket packet = KirboMod.instance.GetPacket();
                        //packet.Write((byte)ModPacketType.PlasmaChargeChange);
                        //packet.Write(plrWhoAmI);
                        //packet.Write(amountToChange);
                        //packet.Send(-1, plrWhoAmI);
                        SyncPlasmaChargeChange(plr, amountToChange);
                    }
                    break;
                case ModPacketType.PlayerRightClickFalse:
                    byte index = reader.ReadByte();
                    KirbPlayer.playerRightClicks[index] = false;
                    if (Main.dedServ)
                    {
                        ModPacket p = KirboMod.instance.GetPacket();
                        p.Write((byte)(ModPacketType.PlayerRightClickFalse));
                        p.Write(index);
                        p.Send();
                    }
                    break;
                case ModPacketType.PlayerRightClickTrue:
                    index = reader.ReadByte();
                    KirbPlayer.playerRightClicks[index] = true;
                    if (Main.dedServ)
                    {
                        ModPacket p = KirboMod.instance.GetPacket();
                        p.Write((byte)(ModPacketType.PlayerRightClickTrue));
                        p.Write(index);
                        p.Send();
                    }
                    break;
                case ModPacketType.PlayerPosition or ModPacketType.PlayerPositionAndVelocity:
                    byte plrIndex = reader.ReadByte();
                    plr = Main.player[plrIndex];
                    Vector2 pos = reader.ReadVector2();
                    plr.position = pos;
                    if (packetType == ModPacketType.PlayerPositionAndVelocity)
                    {
                        Vector2 velocity = reader.ReadVector2();
                        plr.velocity = velocity;
                        if (Main.dedServ)
                        {
                            SyncPlayerPositionAndVelocity(plr);
                        }
                    }
                    else if (Main.dedServ)
                    {
                        SyncPlayerPosition(plrIndex);
                    }

                    break;
                case ModPacketType.SpawnWhispy:
                    ReadSpawnWhispy(reader);
                    break;
                case ModPacketType.ProjectilePosition:
                    ReadSyncProjPosition(reader);
                    break;
                case ModPacketType.SpawnNightmareOrb:
                    ReadSpawnNightmareOrb(reader);
                    break;
                case ModPacketType.MorphoButterflyVanish:
                    ReadMorphoButterflyVanishDust(reader);
                    break;
                case ModPacketType.ProjHitCdSync:
                    ReadProjHitCdSync(reader);
                    break;
                case ModPacketType.ClearTripleStarStarIdentity:
                    ReadClearTripleStarStarIdentity(reader);
                    break;
                case ModPacketType.NewTripleStarStarIdentity:
                    ReadNewTripleStarStarIdentity(reader);
                    break;
                case ModPacketType.BugfixResetTripleStarStarIdentities:
                    ReadBugfixTripleStarStarIdentities(reader);
                    break;
                case ModPacketType.RainbowSwordHit:
                    ReadRainbowSwordHit(reader);
                    break;
            }
        }



        private static void ReadMorphoButterflyVanishDust(BinaryReader reader)
        {
            byte npcIndex = reader.ReadByte();
            NPC butterfly = Main.npc[npcIndex];
            if (butterfly.active && butterfly.type == ModContent.NPCType<OrangeButterfly>())
            {
                butterfly.active = false;
                OrangeButterfly.FailedCatchDust(butterfly.Center);
            }
            if (Main.dedServ)
            {
                SyncMorphoButterflyVanish(npcIndex);
            }
        }

        private static void ReadSpawnNightmareOrb(BinaryReader reader)
        {
            // don't need to re-send packet because the server will be responsible for spawning the NPC
            int playerIndex = reader.ReadByte();
            int i = reader.ReadInt32();
            int j = reader.ReadInt32();
            FountainOfDreams.SpawnNightmareOrbAt(playerIndex, i, j);
        }
        private static void ReadSpawnWhispy(BinaryReader reader)
        {
            // don't need to re-send packet because the server will be responsible for spawning the NPC
            int playerIndex = reader.ReadByte();
            int i = reader.ReadInt32();
            int j = reader.ReadInt32();
            NewWhispySummonTile.SpawnWhispyAt(playerIndex, i, j);
        }

        public static void SendMorphoButterflyVanish(NPC butterfly)
        {
            SyncMorphoButterflyVanish((byte)butterfly.whoAmI);
        }
        public static void SyncMorphoButterflyVanish(byte butterflyWhoAmI)
        {
            ModPacket p = KirboMod.instance.GetPacket();
            p.Write((byte)ModPacketType.MorphoButterflyVanish);
            p.Write(butterflyWhoAmI);
            p.Send();
        }
        public static void SyncProjHitCd(Projectile proj, NPC npcHit)
        {
            SyncProjHitCd(proj, npcHit.whoAmI);
        }
        public static void SyncProjHitCd(Projectile proj, int npcHitIndex)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return;
            }
            if (npcHitIndex < 0 || npcHitIndex >= Main.maxNPCs)
            {
                return;
            }

            ModPacket p = KirboMod.instance.GetPacket();
            p.Write((byte)ModPacketType.ProjHitCdSync);
            p.Write((short)proj.identity);
            p.Write((byte)npcHitIndex);
            //don't need to write iframe amount because that can be gathered from the projectile from the other client
            p.Send();
        }
        private static void ReadProjHitCdSync(BinaryReader reader)
        {
            int projIdentity = reader.ReadInt16();
            int npcHitIndex = reader.ReadByte();
            Projectile proj = Main.projectile.FirstOrDefault(x => (x.active && x.identity == projIdentity));
            if (proj == null)
            {
                return;
            }
            proj.localNPCImmunity[npcHitIndex] = proj.localNPCHitCooldown;
            //re-send to other clients
            if (Main.dedServ)
            {
                SyncProjHitCd(proj, npcHitIndex);
            }
        }

        internal static void SendNewTripleStarStarIdentity(byte owner, short identity)
        {
            ModPacket p = KirboMod.instance.GetPacket();
            p.Write((byte)ModPacketType.NewTripleStarStarIdentity);
            p.Write(owner);
            p.Write(identity);
            p.Send();
        }

        internal static void SendClearTripleStarStarIdentity(byte owner, short identity)
        {
            ModPacket p = KirboMod.instance.GetPacket();
            p.Write((byte)ModPacketType.ClearTripleStarStarIdentity);
            p.Write(owner);
            p.Write(identity);
            p.Send();
        }
        static void ReadClearTripleStarStarIdentity(BinaryReader reader)
        {
            byte owner = reader.ReadByte();
            short identity = reader.ReadInt16();
            Player plr = Main.player[owner];
            Projectile proj = Main.projectile.FirstOrDefault(p => p.active && p.identity == identity && p.type == ModContent.ProjectileType<TripleStarStar>() && p.owner == owner);
            KirbPlayer kplr = plr.GetModPlayer<KirbPlayer>();
            for (int i = 0; i < kplr.tripleStarIdentities.Length; i++)
            {
                if (kplr.tripleStarIdentities[i] == identity)
                {
                    kplr.tripleStarIdentities[i] = -1;
                    break;
                }
            }
            if (proj != null)
            {
                proj.Kill();
            }
            if (Main.dedServ)
            {
                SendClearTripleStarStarIdentity(owner, identity);
            }
        }
        static void ReadNewTripleStarStarIdentity(BinaryReader reader)
        {
            byte owner = reader.ReadByte();
            short identity = reader.ReadInt16();
            Player plr = Main.player[owner];
            Projectile proj = Main.projectile.FirstOrDefault(p => p.active && p.identity == identity && p.type == ModContent.ProjectileType<TripleStarStar>() && p.owner == plr.whoAmI);
            KirbPlayer kplr = plr.GetModPlayer<KirbPlayer>();
            for (int i = 0; i < kplr.tripleStarIdentities.Length; i++)
            {
                if (kplr.tripleStarIdentities[i] == -1)
                {
                    kplr.tripleStarIdentities[i] = identity;
                    break;
                }
            }
            if (Main.dedServ)
            {
                SendNewTripleStarStarIdentity(owner, identity);
            }
        }

        public static void SendBugfixResetTripleStarStarIdentities(Player player)
        {

            if(player  != null && player.whoAmI < Main.maxPlayers && player.whoAmI >= 0)
            {
                SendBugfixResetTripleStarStarIdentities(player.whoAmI);
            }

        }
        public static void SendBugfixResetTripleStarStarIdentities(int plrIndex)
        {
            if(plrIndex < 0 || plrIndex >= Main.maxPlayers)
            {
                return;
            }
            ModPacket p = KirboMod.instance.GetPacket();
            p.Write((byte)ModPacketType.BugfixResetTripleStarStarIdentities);
            p.Write(plrIndex);
            p.Send();
        }
        public static void ReadBugfixTripleStarStarIdentities(BinaryReader reader)
        {
            int plrIndex = reader.ReadByte();
            if(plrIndex < 0 || plrIndex >= Main.maxPlayers || plrIndex == Main.myPlayer)
            {
                return;
            }
            Player plr = Main.player[plrIndex];
            KirbPlayer kplr = plr.GetModPlayer<KirbPlayer>();
            for (int i = 0; i < kplr.tripleStarIdentities.Length; i++)
            {
                kplr.tripleStarIdentities[i] = -1;
            }
            if (Main.dedServ)
            {
                SendBugfixResetTripleStarStarIdentities(plrIndex);
            }
            else
            {
                Main.NewText(plr.name + "'s triple star identities have been reset! Tell them this message appeared for you!", Color.Magenta);
            }
        }

        public static void SendRainbowSwordHit(Vector2 targetPos, sbyte swingDir, byte projOwner, float progress)
        {
            if (projOwner < 0 || projOwner >= Main.maxPlayers)
            {
                return;
            }
            ModPacket p = KirboMod.instance.GetPacket();
            p.Write((byte)ModPacketType.RainbowSwordHit);
            p.WriteVector2(targetPos);
            p.Write(swingDir);
            p.Write(projOwner);
            p.Write(progress);
            p.Send(-1, projOwner);
        }
        static void ReadRainbowSwordHit(BinaryReader reader)
        {
            Vector2 targetPos = reader.ReadVector2();
            sbyte swingDir = reader.ReadSByte();
            byte projOwner = reader.ReadByte();
            float progress = reader.ReadSingle();
            if(Main.dedServ)
            {
                SendRainbowSwordHit(targetPos, swingDir, projOwner, progress);
            } 
            RainbowSwordHeld.HitEffect(targetPos, swingDir, projOwner, progress);
        }
    }
}
