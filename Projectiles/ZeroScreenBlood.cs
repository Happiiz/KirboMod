using KirboMod.NPCs;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class ZeroScreenBlood : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Shot");
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = false;
            //Projectile.hostile = true;//dont deal damage
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.hide = true;
            Projectile.scale = 1.2f;
        }
        public ref float ZPos => ref Projectile.ai[2];
        public ref float MaxZPos => ref Projectile.localAI[2];
        public static int TimeToFly => 60;
        public override void AI()
        {
            if (MaxZPos == 0)
            {
                Projectile.timeLeft = TimeToFly + 2;
                MaxZPos = ZPos;
            }
            ZPos = Utils.Remap(Projectile.timeLeft, TimeToFly, 2, MaxZPos, 0);      
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Redsidue>(), -speed); //Makes dust in a circle
                d.noGravity = true;
            }

            //summon projectiles in 8 directions
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Projectile.ai[1] == 0) //normal circle
                {
                    for (int i = 0; i < 16; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        float rotationalOffset = MathHelper.ToRadians(i * 22.5f); //convert degrees to radians

                        float projX = Projectile.Center.X + (float)Math.Cos(rotationalOffset) * 2;
                        float projY = Projectile.Center.Y + (float)Math.Sin(rotationalOffset) * 2;
                        Vector2 direction = new Vector2(projX, projY) - Projectile.Center;
                        direction.Normalize(); //unit of 1
                        direction *= 35; //speed of 35
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), projX, projY, direction.X, direction.Y, ModContent.ProjectileType<ZeroBloodPellet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    }
                }
                else //offset circle
                {
                    for (int i = 0; i < 16; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
                    {
                        float rotationalOffset = MathHelper.ToRadians((i * 22.5f) + 22.5f / 2f); //convert degrees to radians

                        float projX = Projectile.Center.X + (float)Math.Cos(rotationalOffset) * 2;
                        float projY = Projectile.Center.Y + (float)Math.Sin(rotationalOffset) * 2;

                        Vector2 direction = new Vector2(projX, projY) - Projectile.Center;
                        direction.Normalize(); //unit of 1
                        direction *= 35; //speed of 35
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), projX, projY, direction.X, direction.Y, ModContent.ProjectileType<ZeroBloodPellet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);


                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 origin = tex.Size() / 2;
            Vector2 drawPos = Projectile.Center;
            Vector2 screenCenter = Main.screenPosition;
            screenCenter.X += Main.screenWidth / 2;
            screenCenter.Y += Main.screenHeight / 2;
            float scaleMult = Draw3D.GetScaleFor3D(ZPos);
            drawPos = Vector2.Lerp(screenCenter, drawPos, scaleMult);
            drawPos -= Main.screenPosition;
            Main.EntitySpriteDraw(tex, drawPos, null, Color.White, Projectile.rotation, origin, Projectile.scale * scaleMult, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; // Makes it uneffected by light
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
}