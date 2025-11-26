using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace KirboMod.Items.Accesories
{
	public class UFOMountSummon : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Peewee Pole");
			// Tooltip.SetDefault("Summons a rideable nimbus");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 10;
			Item.height = 10;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(0, 10, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item4;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.UFOMount>();
		}
	}
}