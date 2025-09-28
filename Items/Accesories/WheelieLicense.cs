using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Accesories
{
	public class WheelieLicense : ModItem
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
			Item.value = 10;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item22;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.WheelieBike>();
		}
	}
}