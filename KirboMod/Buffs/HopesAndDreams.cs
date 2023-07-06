using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Buffs
{
	public class HopesAndDreams : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hopes & Dreams");
			// Description.SetDefault("The hopes and dreams of all your friends empower you!");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing
		}

		public override void Update(Player player, ref int buffIndex)
		{
			//player.minionDamage *= 2; //this times two
		    //player.minionDamageMult += 3f;
			player.GetDamage(DamageClass.Summon) += 0.25f; //increases minion damage by 25%
			player.statDefense += 10; //defense up by 10
		}
	}
}
