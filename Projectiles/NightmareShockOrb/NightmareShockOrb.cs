using KirboMod.NPCs;
using KirboMod.NPCs.PlasmaWisp;
using KirboMod.Particles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NightmareShockOrb
{
	public class NightmareShockOrb : ModProjectile 
	{
		private struct Effects
        {
            Vector2 Origin { get => texture.Size() / 2f; }
            float rotation;
			Projectile proj;
            Asset<Texture2D> texture;
            Color color;
            int type = Main.rand.Next(10) < 9 ? 2 : Main.rand.Next(0, 2); //mostly flames
            static int maxtimer = 30;
            public int timer = maxtimer;
            float positionOffset = 0;
            
            public Effects(ModProjectile modproj)
			{
				proj = modproj.Projectile;
				rotation = Main.rand.NextFloat(MathF.Tau);

                string texturePath = "KirboMod/Projectiles/NightmareShockOrb/twirl";

                if (type == 0)
                {
                    texturePath = "KirboMod/Projectiles/NightmareShockOrb/";
                    if (Main.rand.NextBool())
                    {
                        texturePath += "spark1";
                    }
                    else
                    {
                        texturePath += "spark2";
                    }
                }
                else if (type == 2)
                {
                    texturePath = "KirboMod/NPCs/PlasmaWisp/flame_0" + $"{Main.rand.Next(1, 5)}";
                }
                texture = ModContent.Request<Texture2D>(texturePath);

                if (type == 2)
                {
                    maxtimer = 15;
                }

                timer = maxtimer; //update again

                if (Main.rand.Next(1, 4) == 1)
                {
                    color = Color.White;
                }
                else if(Main.rand.Next(1, 4) == 2)
                {
                    color = Color.Yellow;
                }
                else
                {
                    color = Color.Magenta;
                }
            }

            public static void Draw(ref List<Effects> effects, Vector2 screenPos)
            {
                if (effects == null)
                {
                    return;
                }
                for (int i = 0; i < effects.Count; i++)
                {
                    if (effects[i].type < 2)
                    {
                        effects[i].Draw(screenPos);
                    }
                    else //draws flames around the circle
                    {
                        effects[i].DrawFlame(screenPos, Utils.Remap(Main.rand.Next(0, effects.Count), 0, effects.Count, 0, MathF.Tau, true));
                    }
                }
            }

            void Draw(Vector2 screenPos)
            {
                if (type == 0) //sparks
                {
                    Main.EntitySpriteDraw(texture.Value, proj.Center + (rotation.ToRotationVector2() * 55) - screenPos, null,
                        color * Utils.GetLerpValue(0, maxtimer, timer), rotation, Origin, 0.25f, SpriteEffects.None);
                }
                else if (type == 1) //twirls
                {
                    Main.EntitySpriteDraw(texture.Value, proj.Center + (rotation.ToRotationVector2() * positionOffset) - screenPos, 
                        null, Color.White * Utils.GetLerpValue(0, maxtimer, timer), rotation, Origin, 0.075f, SpriteEffects.None);
                }
            }

            void DrawFlame(Vector2 screenPos, float rotation)
            {
                Main.EntitySpriteDraw(texture.Value, proj.Center + rotation.ToRotationVector2() * 115 - screenPos,
                        null, Color.Magenta * 0.6f, rotation, Origin, 0.12f, SpriteEffects.None);
            }

            void Update(out bool remove)
            {
                remove = timer < 1;

                timer--;

                positionOffset += 2;
            }

            public static void Update(ref List<Effects> effects)
            {
                for (int i = 0; i < effects.Count; i++)
                {
                    Effects effect = effects[i];
                    effect.Update(out bool remove);
                    effects[i] = effect;
                    if (remove)
                    {
                        effects.RemoveAt(i);
                        i--;
                    }
                }
            }

            public static void SetToFlame(Effects effect)
            {
                effect.type = 2;
            }
        }

        List<Effects> effects = null;

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dark Orb");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 242;
			Projectile.height = 242;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			
		}

		public override void AI()
		{
            if (effects == null)
            {
                effects = new();
            }
            if (Main.rand.NextBool(5)) //make effects
            {
                effects.Add(new(this));
            }

            if (Projectile.ai[0] % 10 == 0)
            {
                for (float j = 0; j < MathF.Tau; j += MathF.Tau / 100)
                {
                    effects.Add(new(this));
                    Effects.SetToFlame(effects[effects.Count - 1]); //set latest one to a flame
                }
            }
            Effects.Update(ref effects);

            Projectile.ai[0]++;
        }

        public override void OnKill(int timeLeft)
		{
			/*for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Unit(); //circle edge
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.DarkResidue>(), speed * 10, 10); //Makes dust in a messy circle
				d.noGravity = true;
			}*/
		}

        Vector2 pos;

        Asset<Texture2D> ring;
        Asset<Texture2D> flash;

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            //flash
            flash = ModContent.Request<Texture2D>("KirboMod/Projectiles/NightmareShockOrb/NightmareShockOrbFlash");
            Texture2D texture = flash.Value;

            Vector2 drawOrigin = flash.Size() / 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color colorType = Color.Black * 0.3f;

            if (Main.rand.Next(3) == 0)
            {
                colorType = Color.Black * 0.75f;
            }
            else if (Main.rand.Next(3) == 1)
            {
                colorType = Color.Black * 0.5f;
            }
            else
            {
                colorType = Color.Maroon * 0.5f;
            }

            Main.EntitySpriteDraw(texture, drawPos, null, colorType, 0, drawOrigin, 1f, SpriteEffects.None);

            //spinning ring
            ring = ModContent.Request<Texture2D>("KirboMod/Projectiles/NightmareShockOrb/twirl");
            float rotation = Projectile.ai[0] * MathF.Tau / 10;

            Texture2D texture2 = ring.Value;

            Vector2 drawOrigin2 = ring.Size() / 2;
            Vector2 drawPos2 = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Main.EntitySpriteDraw(texture2, drawPos2, null, Color.DarkRed, rotation, drawOrigin2, 0.35f, SpriteEffects.None);

            Main.EntitySpriteDraw(texture2, drawPos2, null, Color.DarkRed, rotation, drawOrigin2, 0.5f, SpriteEffects.None);

            //effects
            VFX.DrawGlowBallAdditive(pos, 0.4f, Color.Yellow, Color.White);

            pos = new Vector2(Projectile.Center.X + Main.rand.Next(-80, 80), Projectile.Center.Y + Main.rand.Next(-80, 80));

            //effects
            Effects.Draw(ref effects, Main.screenPosition);

            //glow in center
            VFX.DrawGlowBallAdditive(Projectile.Center, 1.7f, Color.White, Color.White);

            //ring
            ring = ModContent.Request<Texture2D>("KirboMod/ExtraTextures/RingShinePremultiplied");
            texture2 = ring.Value;
            drawOrigin2 = ring.Size() / 2;

            Main.EntitySpriteDraw(texture2, drawPos2, null, Color.Magenta, 0, drawOrigin2, 2.25f, SpriteEffects.None);
        }
    }
}