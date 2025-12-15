using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts.MarxSparks
{
    public class MarxSparks : ModDust
    {
        public static Asset<Texture2D> dustSheet;
        public static readonly Rectangle Small1 = new(23, 76, 8, 8);
        public static readonly Rectangle Small2 = new(31, 104, 10, 10);
        public static readonly Rectangle Small3 = new(39, 103, 8, 8);
        public static readonly Rectangle Medium1 = new(0, 99, 22, 22);
        public static readonly Rectangle Medium2 = new(0, 76, 23, 23);
        public static readonly Rectangle Medium3 = new(99, 36, 29, 29);
        public static readonly Rectangle Medium4 = new(31, 76, 24, 22);
        public static readonly Rectangle Big1 = new(0, 0, 72, 76);
        public static readonly Rectangle Big2 = new(72, 0, 56, 36);
        public static readonly Rectangle Big3 = new(62, 67, 127, 127);

        public static bool IsBig(Dust dust) => dust.frame == Big1 || dust.frame == Big2 || dust.frame == Big3;
        public static bool IsSmall(Dust dust) => dust.frame == Small1 || dust.frame == Small2 || dust.frame == Small3;
        public static bool IsMedium(Dust dust) => dust.frame == Medium1 || dust.frame == Medium2 || dust.frame == Medium3 || dust.frame == Medium4;

        //CALL ON MARX BOSS SETDEFAULT
        public static void LoadTextureIfNeeded()
        {
            if (Main.dedServ || dustSheet != null)
            {
                return;
            }
            dustSheet = ModContent.Request<Texture2D>("KirboMod/Dusts/MarxSparks/MarxSparks");
        }
        public override void OnSpawn(Dust dust)
        {
            dust.rotation = Main.rand.NextFloat( MathF.Tau);
            dust.customData = (SpriteEffects)Main.rand.Next(2);
            dust.alpha = 1;
            float smallChance = 0.625f;
            float mediumChance = 0.325f;
            float bigChance = 0.05f;
            float total = smallChance + mediumChance + bigChance;
            float rnd = Main.rand.NextFloat(total);
            if (rnd < smallChance)
            {
                dust.frame = Main.rand.NextFromList(Small1, Small2, Small3);
            }
            else if (rnd < smallChance + mediumChance)
            {
                dust.frame = Main.rand.NextFromList(Medium1, Medium2, Medium3, Medium4);
            }
            else
            {
                dust.frame = Main.rand.NextFromList(Big1, Big2, Big3);
            }
        }
        public override bool PreDraw(Dust dust)
        {
            LoadTextureIfNeeded();
            Texture2D tex = dustSheet.Value;
            Vector2 origin = dust.frame.Size() / 2;
            Vector2 drawPos = dust.position - Main.screenPosition;
            SpriteEffects fx = SpriteEffects.None;
            if (dust.customData is SpriteEffects newFX)
            {
                fx = newFX;
            }
            float opacity = 1f - dust.alpha / 255f;
            Main.EntitySpriteDraw(tex, drawPos, dust.frame, dust.GetAlpha(Color.White), dust.rotation, origin, dust.scale * 2f, fx);
            return false;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.9f;
            float rotationMult = 1f;
                //rotation looks really goofy on these 2 big ones, so make them have reduced rotation changes
            if(dust.frame == Big2 || dust.frame == Big3)
            {
                rotationMult = 0.06f;
            }
            dust.rotation += rotationMult * (dust.velocity.X * 0.052f + dust.velocity.Y * 0.04f);
            bool shouldDecay = dust.velocity.LengthSquared() <= 1f;
            if (IsBig(dust))
            { 
                if (shouldDecay)
                {
                    dust.alpha += 255 / 15;
                    if(dust.alpha >= 255)
                    {
                        dust.active = false;
                    }
                }
            }
            else
            {
                if(shouldDecay)
                {
                    dust.scale -= 0.05f * Utils.Remap(dust.scale, 1f, .9f, 0.05f, 1f);
                    if(dust.scale < 0)
                    {
                        dust.active = false;
                    }
                }
            }
            return false;
        }
    }
}
