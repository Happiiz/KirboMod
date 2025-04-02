using Microsoft.Xna.Framework;
using KirboMod.Items.Armor;
using KirboMod.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using KirboMod.Particles;
using Terraria.DataStructures;

namespace KirboMod.Items
{
	public class Changaroonie : ModItem
	{
		public override void SetStaticDefaults()
		{
			/*Tooltip.SetDefault("Dev item" +
                "\nLeft click to cycle through difficulties" +
                "\nRight click to cycle through progression");*/
        }

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 420, 69, 21);
			Item.rare = ItemRarityID.Cyan;
			Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.consumable = false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.altFunctionUse == 2)
            {
                Main.getGoodWorld = !Main.getGoodWorld;
                Main.NewText("for the worthy is now " + Main.getGoodWorld, Color.Yellow);
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.hardMode && !NPC.downedGolemBoss) //hardmode
            {
                Main.hardMode = true;
                Main.NewText("Difficulty set to Hardmode", Color.Yellow);
            }
            else if (Main.hardMode && !NPC.downedGolemBoss) //post golem
            {
                NPC.downedGolemBoss = true;
                Main.NewText("Difficulty set to post-Golem", Color.Yellow);
            }
            else if (NPC.downedGolemBoss) //pre hardmode
            {
                Main.hardMode = false;
                NPC.downedGolemBoss = false;
                Main.NewText("Difficulty set to pre-Hardmode", Color.Yellow);
            }

            return true;
        }
    }
}