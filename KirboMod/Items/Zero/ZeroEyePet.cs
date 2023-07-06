using KirboMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Zero
{
	public class ZeroEyePet : ModItem //light pet status declared in other parts
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Zero's Eye");
			// Tooltip.SetDefault("Summons Zero's eye to provide light");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ZephyrFish);
			Item.width = 15;
			Item.height = 15;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.expert = true;
			Item.value = 5997; //half of 1, 9, 97
			Item.shoot = ModContent.ProjectileType<Projectiles.Pets.ZeroEyePetProj>();
			Item.buffType = ModContent.BuffType<Buffs.Pets.ZeroEyePetBuff>();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0) {
				player.AddBuff(Item.buffType, 3600, true);
			}
		}
	}
}