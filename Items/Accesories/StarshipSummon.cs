using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace KirboMod.Items.Accesories
{
	public class StarshipSummon : ModItem
	{
		public override void SetStaticDefaults() 
		{
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 10;
			Item.height = 10;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.buyPrice(0, 4, 30, 0);
            Item.rare = ItemRarityID.Lime;
			Item.expert = true;
			Item.UseSound = SoundID.Item4;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.Starship>();
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}