using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
    public class Starbit : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Starbit");
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 1000; //go to *this* spot in material group
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 8, true));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25; // Configure the amount of this item that's needed to research it in Journey mode.
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Vector2 drawPos = Item.Center - Main.screenPosition;
            Texture2D tex = TextureAssets.Item[Type].Value;

            float frameIndex = (int)Main.timeForVisualEffects / 7;
            //probably not a good idea but should be fine hopefully probably maybe
            frameIndex = MathF.Acos(MathF.Cos((MathF.PI * frameIndex) / 7f)) * (7 / MathF.PI);
            frameIndex = MathF.Round(frameIndex);
            Rectangle frame = tex.Frame(1, 8, 0, (int)frameIndex);
            //copied from fallen star code
            float rotationTimeOffset = Item.timeSinceItemSpawned / 240f + Main.GlobalTimeWrappedHourly * 0.04f;
            float timer = Main.GlobalTimeWrappedHourly;
            timer %= 5f;
            timer /= 2.5f;
            if (timer >= 1f)
            {
                timer = 2f - timer;
            }
            timer = timer * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                spriteBatch.Draw(tex, drawPos + new Vector2(0f, 8f).RotatedBy((i + rotationTimeOffset) * MathF.Tau) * timer, frame, new Color(50, 50, 255, 50), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
            }
            for (float i = 0f; i < 1f; i += 0.34f)
            {
                spriteBatch.Draw(tex, drawPos + new Vector2(0f, 4f).RotatedBy((i + rotationTimeOffset) * MathF.Tau) * timer, frame, new Color(120, 120, 255, 127), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
            }
            Main.EntitySpriteDraw(tex, drawPos, frame, Color.White, rotation, frame.Size() / 2, scale, SpriteEffects.None);
            return false;
        }
        public override void PostUpdate()
        {
            //once gain copied from fallen star
            Lighting.AddLight(Item.Center, 0.8f, 0.7f, 0.1f);
            if (Item.timeSinceItemSpawned % 12 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Item.Center + new Vector2(0f, Item.height * 0.2f) + Main.rand.NextVector2CircularEdge(Item.width, Item.height * 0.6f) * (0.3f + Main.rand.NextFloat() * 0.5f), 228, new Vector2(0f, (0f - Main.rand.NextFloat()) * 0.3f - 1.5f), 127);
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
            }
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.value = 3;
            Item.rare = ItemRarityID.White;
            Item.maxStack = 9999;
        }

        public override void AddRecipes()
        {
            Recipe starbit = CreateRecipe(20);//the result is 20 starbits
            starbit.AddIngredient(ModContent.ItemType<DreamEssence>(), 10); //10 dream essence
            starbit.AddIngredient(ItemID.FallenStar); //1 fallen star
            starbit.AddTile(TileID.Anvils); //crafted at any anvil
            starbit.Register(); //adds this recipe to the game
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}