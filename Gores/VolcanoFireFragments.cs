using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace KirboMod.Gores
{
    public class VolcanoFireFragment1 : ModDust
    {
        public virtual int TextureX => 32;
        public virtual int TextureY => 26;
        public override void OnSpawn(Dust dust)
        {
            dust.customData = 0;
            dust.alpha = 0;
            dust.frame = new Rectangle(0, 0, TextureX, TextureY);           
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = Texture2D.Value;
            Color lightColor = Lighting.GetColor(dust.position.ToTileCoordinates());
            float opacity = (1 - (dust.alpha / 255f));
            lightColor *= opacity;
            SpriteEffects dir = (SpriteEffects)(dust.dustIndex % 3);
            Main.EntitySpriteDraw(Texture2D.Value, dust.position - Main.screenPosition, null, lightColor, dust.rotation, Texture2D.Size() / 2, dust.scale, dir);
            for (int j = 2; j < 5; j += 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = Utils.Remap(i, 0, 4, 0, MathF.Tau).ToRotationVector2() * j;
                    offset = offset.RotatedBy(dust.rotation);
                    Color edgeColor = lightColor;
                    edgeColor.A = 0;
                    edgeColor *= 0.25f;
                    Main.EntitySpriteDraw(tex, dust.position - Main.screenPosition + offset, null, edgeColor, dust.rotation, tex.Size() / 2, dust.scale, dir);
                }
            }
            return false;
        }
        public override bool Update(Dust dust)
        {
            if (dust.customData is not int)
            {
                dust.customData = 0;
            }
            int timer = (int)dust.customData;
            dust.velocity.X *= .98f;
            dust.velocity.Y += .3f;

            if (!dust.noLightEmittence)
            {
                Lighting.AddLight(dust.position, Color.OrangeRed.ToVector3() * .5f);
            }
            if (timer > 60)
                dust.alpha += 255 / 15;
            if (dust.alpha > 255)
            {
                dust.active = false;
                return false;
            }
            dust.customData = timer + 1;
            dust.firstFrame = false;
            dust.rotation += (dust.dustIndex % 10 - 5) * .1f * Utils.GetLerpValue(10, 0, dust.velocity.X);
            //                                                                add 8 to round to nearest instead of round down
            dust.velocity = Collision.TileCollision(dust.position, dust.velocity, (TextureX + 8) / 16, (TextureY + 8) / 16);
            dust.position += dust.velocity;
            return false;
        }
    }
    public class VolcanoFireFragment2 : VolcanoFireFragment1 
    {
        public override int TextureX => 32;
        public override int TextureY => 30;
    }
    public class VolcanoFireFragment3 : VolcanoFireFragment1 
    {
        public override int TextureX => 38;
        public override int TextureY => 32;
    }
    public class VolcanoFireFragment4 : VolcanoFireFragment1 
    {
        public override int TextureX => 30;
        public override int TextureY => 26;
    }
    public class VolcanoFireFragment5 : VolcanoFireFragment1
    {
        public override int TextureX => 26;
        public override int TextureY => 22;
    }
}
