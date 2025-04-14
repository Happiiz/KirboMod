using KirboMod.Items.NewWhispy;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            /// changes the player's plasma charge.
            /// byte: player whoAmI
            /// sbyte: amount to change
            /// </summary>
            PlasmaChargeChange = 2,
            /// <summary>
            /// sets the player's right click bool in the array to false.
            /// byte: player whoAmI
            /// </summary>
            PlayerRightClickFalse = 6,
            /// <summary>
            /// sets the player's right click bool in the array to true.
            /// byte: player whoAmI
            /// /// </summary>
            PlayerRightClickTrue = 7,
            /// <summary>
            /// updates the player's position.
            /// byte: player whoAmI. Vector2: player position (not center!).
            /// </summary>
            PlayerPosition = 8,
            /// <summary>
            /// updates the player's position and velocity.
            /// byte: player whoAmI. Vector2: player position(not center!). Vector2 player velocity.
            /// </summary>
            PlayerPositionAndVelocity = 9,
            /// <summary>
            /// spawns whispy woods boss
            /// byte: player index, int: tileX, int: tileY
            /// </summary>
            SpawnWhispy = 10,
            /// <summary>
            /// syncs a projectile's position
            /// byte: projectile.identity of the projectile to sync, Vector2: projectile.position(not center!), byte: player whoAmI of client that called the method
            /// </summary>
            ProjectilePosition = 11,
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

            }
        }

        private static void ReadSpawnWhispy(BinaryReader reader)
        {
            // don't need to re-send packet because the server will be responsible for spawning the NPC
            int playerIndex = reader.ReadByte();
            int i = reader.ReadInt32();
            int j = reader.ReadInt32();
            NewWhispySummonTile.SpawnWhispyAt(playerIndex, i, j);
        }
    }
}
