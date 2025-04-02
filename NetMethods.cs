using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }


        public static void SyncPlayerRightClick(Player plr)
        {
            if (plr.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient && KirbPlayer.playerRightClicks[plr.whoAmI] != Main.mouseRight)
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
        public static void SyncPlasmaChargeChange(Player plr, sbyte amountToChange, bool writeDebugMessage = false)
        {
            ModPacket p = KirboMod.instance.GetPacket();
            p.Write((byte)ModPacketType.PlasmaChargeChange);
            p.Write((byte)plr.whoAmI);
            p.Write(amountToChange);
            p.Send(-1, plr.whoAmI);
            if (writeDebugMessage)
            {
                Main.NewText("sent packet: sync plasma charge change.");
            }
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
                    Player plr = Main.player[reader.ReadByte()];
                    sbyte amountToChange = reader.ReadSByte();   
                    KirbPlayer mplr = plr.GetModPlayer<KirbPlayer>();
                    mplr.plasmaCharge += amountToChange;
                    mplr.plasmaTimer = 0;
                    Main.NewText($"received packet: plasma charge change. player {plr.whoAmI} changed plasma charge by {amountToChange}");
                    break;
                case ModPacketType.PlayerRightClickFalse:
                    byte index = reader.ReadByte();
                    KirbPlayer.playerRightClicks[index] = false;
                    Main.NewText($"received packet: player {index}  right click false");
                    break;
                case ModPacketType.PlayerRightClickTrue:
                    index = reader.ReadByte();
                    KirbPlayer.playerRightClicks[index] = true;
                    Main.NewText($"received packet: player {index}  right click true");
                    break;
                case ModPacketType.PlayerPosition or ModPacketType.PlayerPositionAndVelocity:
                    plr = Main.player[reader.ReadByte()];
                    plr.position = reader.ReadVector2();
                    if (packetType == ModPacketType.PlayerPositionAndVelocity)
                    {
                        plr.velocity = reader.ReadVector2();
                    }
                    break;

            }
        }
    }
}
