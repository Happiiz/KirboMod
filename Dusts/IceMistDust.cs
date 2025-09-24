using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
    public class IceMistDust : ModDust
    {
        public override string Texture => "KirboMod/Projectiles/IceMist/IceMist1";
        static void GetCustomData(Dust dust, out float innerMistRotationOffset, out SpriteEffects spriteFXOuter, out SpriteEffects spriteFXInner, out int style)
        {
            (float innerRot, byte spriteFXData) = ((float innerRot, byte spriteFXData))dust.customData;
            innerMistRotationOffset = innerRot;

            // high 4 bits = style
            style = spriteFXData >> 4;

            // low 2 bits = sprite effects
            int fxBits = spriteFXData & 0b11;
            spriteFXOuter = fxBits >= 2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteFXInner = fxBits % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }
        public override void OnSpawn(Dust dust)
        {
            byte spriteFXData = (byte)Main.rand.Next(4); // store in low bits
            spriteFXData |= (byte)(Main.rand.Next(1, 4) << 4); // store style in high 4 bits
            dust.customData = (Main.rand.NextFloat(MathF.Tau), spriteFXData);

        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.97f;
            if(dust.velocity.LengthSquared() < 4*4)
            {
                dust.alpha += 255 / 30;
                if(dust.alpha >= 230)
                {
                    dust.active = false;
                } 
            }
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            GetCustomData(dust, out float innerMistRotationOffset, out SpriteEffects fxOuter, out SpriteEffects fxInner, out int style);
            Texture2D iceMist = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist" + style).Value;

            Color col = dust.color;
            col *= (255 - dust.alpha) / 255f;
            float scale = dust.scale;
            Vector2 drawPos = dust.position - Main.screenPosition;
            Vector2 origin = iceMist.Size() / 2;
           // drawPos += origin;
            Main.EntitySpriteDraw(iceMist, drawPos, null, col, dust.rotation, origin, scale * 0.5f, fxOuter);
            Main.EntitySpriteDraw(iceMist, drawPos, null, col, dust.rotation + innerMistRotationOffset, origin, scale * 0.25f, fxInner);
            return false;
        }
    }
}
