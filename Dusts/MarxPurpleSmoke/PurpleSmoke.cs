using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Dusts.MarxPurpleSmoke
{
    public class PurpleSmoke : ModDust
    {
        public override string Texture => "KirboMod/Projectiles/IceMist/IceMist1";

        public static Asset<Texture2D> cloud1;
        public static Asset<Texture2D> cloud2;
        public static Asset<Texture2D> cloud3;
        public static int TimeBeforeFadingOut => 60;
        public static int FadeOutDuration => 15;
        public static int TotalLifetime => TimeBeforeFadingOut + FadeOutDuration;
        public override void SetStaticDefaults()
        {
            cloud1 = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist1");
            cloud2 = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist2");
            cloud3 = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist3");
        }
        public override void OnSpawn(Dust dust)
        {
            dust.frame.X = Main.rand.Next(3);
            dust.alpha = 255;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.frame.Y++;
            dust.alpha -= 255 / 15;
            if(dust.alpha < 0)
            {
                dust.alpha = 0;
            }
            if (dust.frame.Y >= TimeBeforeFadingOut)
            {
                dust.alpha = (int)Utils.Remap(dust.frame.Y, TimeBeforeFadingOut, TimeBeforeFadingOut + FadeOutDuration, 0, 255, false);
            }
            if(dust.alpha >= 255)
            {
                dust.active = false;
            }
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = (dust.frame.X == 0 ? cloud1 : dust.frame.X == 1 ? cloud2 : cloud3).Value;
            Main.EntitySpriteDraw(tex, dust.position - Main.screenPosition, null, dust.color * (1f - dust.alpha / 255f), dust.rotation, tex.Size() / 2, dust.scale, (SpriteEffects)(dust.dustIndex % 2));
            return false;
        }
        public static Dust NewPurpleSmokeDust(Vector2 pos, Vector2 vel, float scale, int initialTimerValue = 0)
        {
            Color purple = Color.Lerp(new Color(91, 0, 181), new Color(139, 0, 181), Main.rand.NextFloat());
            Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<PurpleSmoke>(), vel, 0, purple, scale);
            d.frame.Y = initialTimerValue;
            return d;
        }
    }
}
