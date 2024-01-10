using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static KirboMod.KirboMod;

namespace KirboMod
{
    public static class NetMethods
    {
        public static void SyncPlayerPosition(Player plr)
        {
            ModPacket packet = KirboMod.instance.GetPacket();
            packet.Write((byte)ModPacketType.PlayerPosition);
            packet.Write((byte)plr.whoAmI);
            packet.WriteVector2(plr.position);
            packet.Send();
        }
        public static void SyncPlayerPosition(int whoAmI)
        {
            SyncPlayerPosition(Main.player[whoAmI]);
        }

        public static void SyncPlayerPositionAndVelocity(Player plr)
        {
            ModPacket packet = KirboMod.instance.GetPacket();
            packet.Write((byte)ModPacketType.PlayerPositionAndVelocity);
            packet.Write((byte)plr.whoAmI);
            packet.WriteVector2(plr.position);
            packet.WriteVector2(plr.velocity);
            packet.Send();
        }
    }
}
