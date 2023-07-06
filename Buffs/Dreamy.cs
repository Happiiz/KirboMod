using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Buffs
{
	public class Dreamy : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dreamy");
			// Description.SetDefault("The fountain puts you at ease");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.lifeRegen += 1;
            player.statDefense += 5; //add 5 defense
            player.moveSpeed += 0.05f; //5%
			player.maxRunSpeed += 0.05f; //5%
		}
	}
}
