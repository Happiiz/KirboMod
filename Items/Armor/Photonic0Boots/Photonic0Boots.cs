using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Armor.Photonic0Boots
{
    [AutoloadEquip(EquipType.Legs)]
    public class Photonic0Boots : ModItem
    {
        public override void SetDefaults()
        {
            Item.vanity = true;
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SilverGreaves)
                .AddIngredient(ItemID.WispDye)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SilverGreaves)
                .AddIngredient(ItemID.CyanDye)
                .AddIngredient(ItemID.Torch, 10)
                .Register();

            CreateRecipe()
               .AddIngredient(ItemID.SilverGreaves)
               .AddIngredient(ItemID.UltrabrightTorch, 10)
               .Register();
        }
    }
    public class Photonic0BootsDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            int type = ModContent.ItemType<Photonic0Boots>();
            bool result = drawInfo.drawPlayer.armor[2].type == type || drawInfo.drawPlayer.armor[12].type == type;
            return result;
        }
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Leggings);
        }
        static Asset<Texture2D> bootsTexture;
        public override void Unload()
        {
            bootsTexture = null;
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (bootsTexture == null)
            {
                bootsTexture = ModContent.Request<Texture2D>("KirboMod/Items/Armor/Photonic0Boots/Photonic0BootsGlow");
            }

            int num = drawInfo.armorAdjust;
            if (drawInfo.drawPlayer.direction == -1)
            {
                num = 0;
            }
            Texture2D tex = bootsTexture.Value;
            Color color = new(100, 100, 100, 0);
            ulong seed = (ulong)(drawInfo.drawPlayer.miscCounter / 4 + 210470);
            int num2 = 4;
            Player plr = drawInfo.drawPlayer;
            plr.sitting.GetSittingOffsetInfo(plr, out Vector2 posOffset, out float seatAdjustment);
            posOffset += drawInfo.legsOffset;
            posOffset.Y += drawInfo.seatYOffset + seatAdjustment;
            if (plr.sitting.isSitting || (plr.mount != null && plr.mount.Type == MountID.WitchBroom || plr.mount.Type == MountID.DarkMageBook || plr.mount.Type == MountID.SpookyWood))
            {
                for (int i = 0; i < num2; i++)
                {
                    float num3 = Utils.RandomInt(ref seed, -10, 11) * 0.2f;
                    float num4 = Utils.RandomInt(ref seed, -11, 1) * 0.15f;
                    // num4 = -11 * 0.15f;//TEMP DEBUGGING VALUE
                    DrawSittingLegs(ref drawInfo, tex, color, new Vector2(num3, num4));
                }
                return;
            }
            for (int i = 0; i < num2; i++)
            {
                float num3 = Utils.RandomInt(ref seed, -10, 11) * 0.2f;
                float num4 = Utils.RandomInt(ref seed, -10, 1) * 0.15f;

                DrawData item = new(tex, new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.legFrame.Width / 2 + drawInfo.drawPlayer.width / 2) + num,
                    (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.legFrame.Height + 18)) + drawInfo.drawPlayer.legPosition + new Vector2(drawInfo.drawPlayer.legFrame.Width / 2 + num3, drawInfo.drawPlayer.legFrame.Height / 2 + num4) + posOffset,
                    drawInfo.drawPlayer.legFrame, color, drawInfo.drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect);
                item.shader = drawInfo.cLegs;
                drawInfo.DrawDataCache.Add(item);
            }
        }


        static void DrawSittingLegs(ref PlayerDrawSet drawinfo, Texture2D textureToDraw, Color matchingColor, Vector2 posOffset, int shaderIndex = 0, bool glowmask = false)
        {
            Vector2 legsOffset = drawinfo.legsOffset;
            Vector2 vector = new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - drawinfo.drawPlayer.legFrame.Width / 2 + drawinfo.drawPlayer.width / 2), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.legFrame.Height + 4f)) + drawinfo.drawPlayer.legPosition + drawinfo.legVect;
            Rectangle legFrame = drawinfo.drawPlayer.legFrame;
            vector.Y -= 2f;
            vector.Y += drawinfo.seatYOffset;
            vector += legsOffset;
            int num = 2;
            int num2 = 42;
            int num3 = 2;
            int num4 = 2;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            bool flag = drawinfo.drawPlayer.legs == 101 || drawinfo.drawPlayer.legs == 102 || drawinfo.drawPlayer.legs == 118 || drawinfo.drawPlayer.legs == 99;
            if (drawinfo.drawPlayer.wearsRobe && !flag)
            {
                num = 0;
                num4 = 0;
                num2 = 6;
                vector.Y += 4f;
                legFrame.Y = legFrame.Height * 5;
            }
            switch (drawinfo.drawPlayer.legs)
            {
                case 214:
                case 215:
                case 216:
                    num = -6;
                    num4 = 2;
                    num5 = 2;
                    num3 = 4;
                    num2 = 6;
                    legFrame = drawinfo.drawPlayer.legFrame;
                    vector.Y += 2f;
                    break;
                case 106:
                case 143:
                case 226:
                    num = 0;
                    num4 = 0;
                    num2 = 6;
                    vector.Y += 4f;
                    legFrame.Y = legFrame.Height * 5;
                    break;
                case 132:
                    num = -2;
                    num7 = 2;
                    break;
                case 193:
                case 194:
                    if (drawinfo.drawPlayer.body == 218)
                    {
                        num = -2;
                        num7 = 2;
                        vector.Y += 2f;
                    }
                    break;
                case 210:
                    if (glowmask)
                    {
                        Vector2 vector2 = new(Main.rand.Next(-10, 10) * 0.125f, Main.rand.Next(-10, 10) * 0.125f);
                        vector += vector2;
                    }
                    break;
            }
            //starting for loop value should be num3
            //0 rn for debugging
            for (int i = num3; i >= 0; i--)
            {
                Vector2 position = vector + new Vector2(num, 2f) * new Vector2(drawinfo.drawPlayer.direction, 1f);
                Rectangle value = legFrame;
                value.Y += i * 2;
                value.Y += num2;
                value.Height -= num2;
                value.Height -= i * 2;
                if (i != num3)
                {
                    value.Height = 2;
                }
                position.X += drawinfo.drawPlayer.direction * num4 * i + num6 * drawinfo.drawPlayer.direction;
                if (i != 0)
                {
                    position.X += num7 * drawinfo.drawPlayer.direction;
                }

                //fix didn't work for some reason 
                //if (i < 2 && drawinfo.drawPlayer.body == ArmorIDs.Body.ApprenticeDark)
                //{
                //    if (posOffset.Y < 0)//would clip into a pixel gap of the robe and look ugly
                //    {
                //        continue;
                //    }
                //}

                position.Y += num2;
                position.Y += num5;


                DrawData item = new(textureToDraw, position + posOffset, value, matchingColor, drawinfo.drawPlayer.legRotation, drawinfo.legVect, 1f, drawinfo.playerEffect);
                item.shader = shaderIndex;
                drawinfo.DrawDataCache.Add(item);
            }
        }

        public static void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            int type = ModContent.ItemType<Photonic0Boots>();
            if (drawInfo.drawPlayer.armor[2].type == type || drawInfo.drawPlayer.armor[12].type == type)
            {
                drawInfo.colorArmorLegs = Color.White;
            }
        }
    }
}
