using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Buffs
{
	public class Nightmare : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Nightmare!");
			// Description.SetDefault("You are feeling bad thoughts");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.velocity.X *= 0.98f;

			player.statDefense -= 5; //subtract 5 defense
			player.bleed = true; //no life regen
			player.blackout = true; //severely decreased vision

		}
	}
}
