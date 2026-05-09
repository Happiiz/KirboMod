using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class MiracleMatter : ModItem
	{
		byte useCounter;
		public static SoundStyle SingleHit => new SoundStyle("KirboMod/Sounds/Item/MiracleMatter/MiracleMatterSingleOrchHit") with { MaxInstances = 0 };
		public static SoundStyle DoubleHit => new SoundStyle("KirboMod/Sounds/Item/MiracleMatter/MiracleMatterDoubleOrchHit") with { MaxInstances = 0 };
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Miracle Matter");
			//Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(10, 2)); //ticks per frame, frame count
			// Tooltip.SetDefault("Matter straight from a fallen angel");
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 1005; //go to *this* spot in material group
			ItemID.Sets.ItemNoGravity[Item.type] = true; //float like soul

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // Configure the amount of this item that's needed to research it in Journey mode.
        }
        public override void UseAnimation(Player player)
        {
			useCounter++;
			useCounter %= 2;
			if(useCounter == 0)
			{
				Item.UseSound = DoubleHit;
			}
			else
			{
				Item.UseSound = SingleHit;
			}
        }
        public override void SetDefaults() 
		{
			Item.width = 66;
			Item.height = 70;
			Item.value = Item.buyPrice(0, 0, 25, 0);
			Item.rare = ItemRarityID.Purple; //post moon lord tier
			Item.maxStack = 9999;
			Item.UseSound = SingleHit;
			useCounter = 0;
			Item.useAnimation = Item.useTime = 60;
			Item.useStyle = ItemUseStyleID.HoldUp;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}