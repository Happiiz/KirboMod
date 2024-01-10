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
        public static KirboMod instance;
        internal FighterComboMeter fighterComboMeter;
        internal UserInterface fighterComboMeterInterface;
        public enum ModPacketType : byte
        {
            //byte: playerWhoAmI
            StartFinalCutter = 0,
            //byte: playerWhoAmI, byte: number of npcs, bytes: indexes of npcs caught in effect
            StartFinalCutterMultiNPC = 1,
            /// <summary>
            /// increases the player's plasma charge by 1.
            /// byte: player whoAmI
            /// </summary>
            PlasmaCharge1 = 2,
            /// <summary>
            /// increases the player's plasma charge by 2.
            /// byte: player whoAmI
            /// </summary>
            PlasmaCharge2 = 3,
            /// <summary>
            /// increases the player's plasma charge by 3.
            /// byte: player whoAmI
            /// </summary>
            PlasmaCharge3 = 4,
            /// <summary>
            /// increases the player's plasma charge by 4.
            /// byte: player whoAmI
            /// </summary>
            PlasmaCharge4 = 5,
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
                case ModPacketType.PlasmaCharge1 or ModPacketType.PlasmaCharge2 or ModPacketType.PlasmaCharge3 or ModPacketType.PlasmaCharge4:
                    Player plr = Main.player[reader.ReadByte()];
                    KirbPlayer mplr = plr.GetModPlayer<KirbPlayer>();
                    mplr.plasmaCharge++;
                    if (packetType >= ModPacketType.PlasmaCharge2)
                        mplr.plasmaCharge++;
                    if (packetType >= ModPacketType.PlasmaCharge3)
                        mplr.plasmaCharge++;
                    if (packetType == ModPacketType.PlasmaCharge4)
                        mplr.plasmaCharge++;
                    mplr.plasmaTimer = 0;
                    if(mplr.plasmaCharge > 20)
                    {
                        mplr.plasmaCharge = 20;
                    }
                    break;
                case ModPacketType.PlayerRightClickFalse:
                    KirbPlayer.playerRightClicks[reader.ReadByte()] = false;
                    break;
                case ModPacketType.PlayerRightClickTrue:
                    KirbPlayer.playerRightClicks[reader.ReadByte()] = true;
                    break;
                case ModPacketType.PlayerPosition or ModPacketType.PlayerPositionAndVelocity:
                    plr = Main.player[reader.ReadByte()];
                    plr.position = reader.ReadVector2();
                    if(packetType == ModPacketType.PlayerPositionAndVelocity)
                    {
                        plr.velocity = reader.ReadVector2();
                    }
                    break;

            }
        }
        public override void Unload()
        {
            instance = null;
        }

        public override void Load()
        {
            instance = this;
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