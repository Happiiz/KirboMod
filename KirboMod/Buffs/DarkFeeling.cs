using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Buffs
{
	public class DarkFeeling : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Feeling");
			// Description.SetDefault("This mirror is giving bad vibes...");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.velocity.X *= 0.98f;

			player.statDefense -= 1; //subtract 1 defense
			player.bleed = true; //no life regen
		}
	}
}
