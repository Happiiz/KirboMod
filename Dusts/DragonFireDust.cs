using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Dusts
{
    internal class DragonFireDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(1, 3, 0, Main.rand.Next(3));
            dust.customData = MathHelper.Lerp(-.1f, .1f, Main.rand.NextFloat());
            dust.alpha = 0;
        }
        public override bool Update(Dust dust)
        {        
            if(dust.scale > 2f)
            {
                dust.scale = 2;
            }
            dust.scale *= .98f;
            dust.velocity *= 0.98f;
            if (dust.customData is float accel)
            {
                dust.velocity.X += accel;
                accel *= .9f;
                dust.customData = accel;
            }
            if (!dust.noGravity) 
            {
                dust.velocity.Y -= .1f;
            }
            if (!dust.noLightEmittence)
            {
                float lightIntensity = dust.scale * 1.4f;
                if (lightIntensity > 0.6f)
                {
                    lightIntensity = 0.6f;
                }
                Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), lightIntensity, lightIntensity * 0.44f, lightIntensity * .72f);
            }
            dust.rotation += dust.velocity.X * 0.1f;
            if(dust.scale < .5f)
            {
                dust.alpha += 20;
                dust.active = dust.alpha < 255;

            }
            dust.position += dust.velocity;
            return false;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor) => Color.White * (1 - dust.alpha / 255f);
    }
}
