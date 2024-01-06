using KirboMod.NPCs;
using KirboMod.UI;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace KirboMod
{
	public class KirboMod : Mod
	{
        internal FighterComboMeter fighterComboMeter;
        internal UserInterface fighterComboMeterInterface;

        public enum ModPacketType : byte
        {
            //byte: playerWhoAmI
            StartFinalCutter = 0,
            //byte: playerWhoAmI, byte: number of npcs, bytes: indexes of npcs caught in effect
            StartFinalCutterMultiNPC = 1,
            /// <summary>
            /// byte: player whoAmI
            /// </summary>
            PlasmaChargeUp = 2,
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
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
                case ModPacketType.PlasmaChargeUp:
                    Player plr = Main.player[reader.ReadByte()];
                    KirbPlayer mplr = plr.GetModPlayer<KirbPlayer>();
                    mplr.plasmaCharge++;
                    mplr.plasmaTimer = 0;
                    if(mplr.plasmaCharge > 20)
                    {
                        mplr.plasmaCharge = 20;
                    }
                    break;

            }
        }
        public override void Unload()
        {
             
        }

        public override void Load()
        {
            // All code below runs only if we're not loading on a server
            if (!Main.dedServ)
            {
                // Create new filters
                Filters.Scene["KirboMod:HyperZone"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.4f, 0.9f).UseOpacity(0), EffectPriority.High);
                SkyManager.Instance["KirboMod:HyperZone"] = new ZeroSky();

                // Custom timer
                fighterComboMeter = new FighterComboMeter();
                fighterComboMeter.Activate();
                fighterComboMeterInterface = new UserInterface();
                fighterComboMeterInterface.SetState(fighterComboMeter);
            }
        }
    }
}