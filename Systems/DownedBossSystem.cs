using System.Collections;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KirboMod.Systems
{
	public class DownedBossSystem : ModSystem
	{
		public static bool downedWhispyBoss = false;
        public static bool downedKrackoBoss = false;
        public static bool downedKingDededeBoss = false;
        public static bool downedNightmareBoss = false;
        public static bool downedDarkMatterBoss = false;
        public static bool downedZeroBoss = false;
        // public static bool downedOtherBoss = false;

        public override void OnWorldLoad() {
            downedWhispyBoss = false;
            downedKrackoBoss = false;
            downedKingDededeBoss = false;
            downedNightmareBoss = false;
            downedDarkMatterBoss = false;
            downedZeroBoss = false;
            // downedOtherBoss = false;
        }

		public override void OnWorldUnload() {
            downedWhispyBoss = false;
            downedKrackoBoss = false;
            downedKingDededeBoss = false;
            downedNightmareBoss = false;
            downedDarkMatterBoss = false;
            downedZeroBoss = false;
            // downedOtherBoss = false;
        }

		// We save our data sets using TagCompounds.
		// NOTE: The tag instance provided here is always empty by default.
		public override void SaveWorldData(TagCompound tag) {
			if (downedWhispyBoss) {
				tag["downedWhispyBoss"] = true;
			}
            if (downedKrackoBoss)
            {
                tag["downedKrackoBoss"] = true;
            }
            if (downedKingDededeBoss)
            {
                tag["downedKingDededeBoss"] = true;
            }
            if (downedNightmareBoss)
            {
                tag["downedNightmareBoss"] = true;
            }
            if (downedDarkMatterBoss)
            {
                tag["downedDarkMatterBoss"] = true;
            }
            if (downedZeroBoss)
            {
                tag["downedZeroBoss"] = true;
            }
            // if (downedOtherBoss) {
            //	tag["downedOtherBoss"] = true;
            // }
        }

		public override void LoadWorldData(TagCompound tag) {
            downedWhispyBoss = tag.ContainsKey("downedWhispyBoss");
            downedKrackoBoss = tag.ContainsKey("downedKrackoBoss");
            downedKingDededeBoss = tag.ContainsKey("downedKingDededeBoss");
            downedNightmareBoss = tag.ContainsKey("downedNightmareBoss");
            downedDarkMatterBoss = tag.ContainsKey("downedDarkMatterBoss");
            downedZeroBoss = tag.ContainsKey("downedZeroBoss");
            // downedOtherBoss = tag.ContainsKey("downedOtherBoss");
        }

		public override void NetSend(BinaryWriter writer) {
			// Order of operations is important and has to match that of NetReceive
			var flags = new BitsByte();
			flags[0] = downedWhispyBoss;
            flags[1] = downedKrackoBoss;
            flags[2] = downedKingDededeBoss;
            flags[3] = downedNightmareBoss;
            flags[4] = downedDarkMatterBoss;
            flags[5] = downedZeroBoss;
            // flags[6] = downedOtherBoss;
            //when up to [7], create a new BitsByte
            writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader) {
			// Order of operations is important and has to match that of NetSend
			BitsByte flags = reader.ReadByte();
            downedWhispyBoss = flags[0];
            downedKrackoBoss = flags[1];
            downedKingDededeBoss = flags[2];
            downedNightmareBoss = flags[3];
            downedDarkMatterBoss = flags[4];
            downedZeroBoss = flags[5];
            // downedOtherBoss = flags[6];
            //when up to [7], create a new BitsByte
        }
	}
}
