using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using KirboMod.NPCs;

namespace KirboMod.Dusts
{
    public class KrackoDust : ModDust
    {
        public static float GetZPos(Dust dust) => ((float[])dust.customData)[0];
        public static float GetZVel(Dust dust) => ((float[])dust.customData)[1];
        public static void SetZPos(Dust dust, float val) => ((float[])dust.customData)[0] = val;
        public static void SetZVel(Dust dust, float val) => ((float[])dust.customData)[1] = val;
        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(1, 3, 0, Main.rand.Next(3));
            dust.customData = MathHelper.Lerp(-.1f, .1f, Main.rand.NextFloat());
            dust.alpha = 0;
            dust.scale *= 2f;
        }
        public static void NewKrackoDust(Vector2 Position, int Width, int Height, float SpeedX = 0f, float SpeedY = 0f, float Scale = 1f, float SpeedZ = 0f, float zPos = 0f)
        {
            Dust dust = Dust.NewDustDirect(Position, Width, Height, ModContent.DustType<KrackoDust>(), SpeedX, SpeedY, 0, Color.White, Scale);
            float[] zParams = new float[2];
            zParams[0] = zPos;
            zParams[1] = SpeedZ;
            dust.customData = zParams;
            dust.velocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedX;
            dust.velocity.Y = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedY;
        }
        public override bool Update(Dust dust)
        {
            float velDecay = 0.98f;
            dust.scale *= .98f;
            dust.velocity *= velDecay;
            float zVel = GetZVel(dust);
            zVel *= velDecay;
            SetZVel(dust, zVel);
         
            dust.rotation += dust.velocity.X * 0.1f;
            if (dust.scale < .5f)
            {
                dust.alpha += 20;
                dust.active = dust.alpha < 255;

            }
            dust.position += dust.velocity;
            float zPos = GetZPos(dust);
            zPos += GetZVel(dust);
            SetZPos(dust, zPos);
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = Texture2D.Value;
            Vector2 drawPos = dust.position;
            Rectangle frame = dust.frame;
            drawPos.X -= frame.Width / 2;
            drawPos.Y -= frame.Height / 2;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);
            Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            Color col = dust.GetAlpha(Color.White);
            float scaleMult = Kracko.GetScaleFor3D(GetZPos(dust));
            drawPos = Vector2.Lerp(screenCenter, drawPos, scaleMult);
            drawPos -= Main.screenPosition;
            Main.EntitySpriteDraw(tex, drawPos, frame, col, dust.rotation, origin, dust.scale * scaleMult, (SpriteEffects)(dust.dustIndex % 2));
            return false;
        }
    }
}
