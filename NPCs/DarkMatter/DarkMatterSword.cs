using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.NPCs.DarkMatter
{
    public partial class DarkMatter : ModNPC
    {
        SwordDisplay sword;
        private struct SwordDisplay
        {
            public SwordDisplay(NPC darkMatter)
            {
                rainbowStrength = 0;
                darkStrength = 0;
                rotation = 0;
                this.darkMatter = darkMatter;
                swordTexture = ModContent.Request<Texture2D>("KirboMod/NPCs/DarkMatter/DarkBlade").Value;
            }
            public void RotateTo(float angle)
            {
                rotation = rotation.AngleLerp(angle, .1f);
            }
            public void Draw()
            {
                SpriteEffects direction = darkMatter.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
                Vector2 offset = new Vector2(50 * darkMatter.direction, 30); 
                Main.spriteBatch.Draw(swordTexture, darkMatter.Center - Main.screenPosition + offset, null, new Color(255, 255, 255) * darkMatter.Opacity, rotation, new Vector2(19, 64), 1f, direction, 0f);
                if(rainbowStrength > 0)
                {
                    int frameX = (int)(Main.timeForVisualEffects % 60);
                    Rectangle frame = rainbowEffectTexture.Frame(60, 1, frameX);
                    Main.spriteBatch.Draw(rainbowEffectTexture, darkMatter.Center - Main.screenPosition + offset, frame, new Color(255, 255, 255) * darkMatter.Opacity * rainbowStrength, rotation, new Vector2(19, 64), 1f, direction, 0f);
                }
                if(darkStrength > 0)
                {
                    Main.spriteBatch.Draw(darkEffectTexture, darkMatter.Center - Main.screenPosition + offset, null, new Color(255, 255, 255) * darkMatter.Opacity * darkStrength, rotation, new Vector2(19, 64), 1f, direction, 0f);
                }
            }
            public void Update()
            {
                rainbowStrength -= .05f;//decay strengths for fadeouts
                darkStrength -= .05f;
            }
            public void TurnRainbow()
            {
                if (rainbowStrength > 1)
                    rainbowStrength = 1;//purposefully checks before the increase to counteract the rainbow strength being decreased every frame
                //play some sound effect if rainbowstrength == 0 probably
                rainbowStrength += .1f;//net increase of .05 every frame
            }
            public void ShootDarkBeamEffect()
            {
                darkStrength = 1.05f;
                Particles.Ring.DarkMatterSwordShot(darkMatter.Center + new Vector2(darkMatter.direction * 40, 30));
            }
            NPC darkMatter;
            public float rainbowStrength;//opacity of the rainbow effect
            public float darkStrength;//opacity of the darkness effect
            public static Texture2D swordTexture;
            public static Texture2D darkEffectTexture;
            public static Texture2D rainbowEffectTexture;//make additive perhaps? probably will look bad though...
            public float rotation;
        }
    }
}
