using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
    public class BeamStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Beam Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Item.staff[Item.type] = true; //staff not gun
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            //low range so it has high damage value
            Item.DefaultToWhip(ModContent.ProjectileType<Projectiles.BeamWhipProj>(), 32, 6.5f, 2.2f, 30);
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.knockBack = -3;
            Item.value = Item.buyPrice(0, 0, 3);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.mana = 5;
            Item.UseSound = default;
        }

        public override void AddRecipes()
        {
            Recipe beamstaff = CreateRecipe();//the result is staff
            beamstaff.AddIngredient(ModContent.ItemType<Starbit>(), 10); //10 starbits
            beamstaff.AddRecipeGroup(RecipeGroupID.IronBar, 10); //10 iron/lead bars
            beamstaff.AddTile(TileID.Anvils); //crafted at anvil
            beamstaff.Register(); //adds this recipe to the game
        }

    }
}