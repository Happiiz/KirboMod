using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
    public class NightCloth : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Night Cloth");
            // Tooltip.SetDefault("The stuff nightmares are made of");
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 1007; //go to *this* spot in material group
            Main.RegisterItemAnimation(Type, new VFX.NightmareItemAnimation());
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5; // Configure the amount of this item that's needed to research it in Journey mode.
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return true;
            //return VFX.NightmareItemsPreDrawPreDrawInInventory(Item, spriteBatch, position, drawColor, scale);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return true;
            //return VFX.NightmareItemsPreDrawInWorld(Item, spriteBatch, ref rotation, ref scale);
        }

        public override void SetDefaults()
        {
            Item.width = 36 * 2;
            Item.height = 32 * 2;
            Item.value = Item.buyPrice(0, 0, 7, 0);
            Item.rare = ItemRarityID.Pink;
            Item.maxStack = 9999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}