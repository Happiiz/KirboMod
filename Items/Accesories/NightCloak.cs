using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Items.Accesories
{
   // [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class NightCloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Night Cloak");
            // Tooltip.SetDefault("Shoots stars in eight directions upon being hit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
            Main.RegisterItemAnimation(Type, new VFX.NightmareItemAnimation());
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
            Item.width = 66;
            Item.height = 58;
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 3, 25, 0);
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<KirbPlayer>().nightcloak = true;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            // When the item is given a prefix, only roll the best modifiers for accessories
            return 0;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StarCloak);
            recipe.AddIngredient(ModContent.ItemType<Items.NightCloth>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }


    }
    public class NightmareCloakBackDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<KirbPlayer>().nightcloak;
        }
        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.BackAcc);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {

            Vector2 zero = Vector2.Zero;
            Vector2 vector = new Vector2(0f, 8f);
            Vector2 vec = zero + drawInfo.Position - Main.screenPosition + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height / 2) + new Vector2(0f, -4f) + vector;
            vec = vec.Floor();
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Items/Accesories/NightCloak_Back").Value;
            Rectangle frame = drawInfo.drawPlayer.bodyFrame;
            int frameCount = 64;
            float animSpeed = 1f;
            int frameIndex = (int)((Main.timeForVisualEffects * animSpeed) % frameCount);
            frame.X += frameIndex * frame.Width;
            drawInfo.DrawDataCache.Add(new(texture, vec, frame, drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect
                ));
        }
    }
    public class NightmareCloakFrontDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<KirbPlayer>().nightcloak;
        }
        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.FrontAccFront);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Vector2 zero = Vector2.Zero;
            Vector2 vector = new Vector2(0f, 8f);
            Vector2 vec = zero + drawInfo.Position - Main.screenPosition + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height / 2) + new Vector2(0f, -4f) + vector;
            vec = vec.Floor();
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Items/Accesories/NightCloak_Front").Value;
            Rectangle frame = drawInfo.drawPlayer.bodyFrame;
            int frameCount = 64;
            float animSpeed = 1f;
            int frameIndex = (int)((Main.timeForVisualEffects * animSpeed) % frameCount);
            frame.X += frameIndex * frame.Width;
            drawInfo.DrawDataCache.Add(new(texture, vec, frame, drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect
                ));
        }
    }
}
