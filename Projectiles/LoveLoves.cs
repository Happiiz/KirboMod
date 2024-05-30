using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class LoveLoves : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
		public override void SetDefaults()
		{
			Projectile.width = 58;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.extraUpdates = 3;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 20; //time before hit again
			Projectile.ignoreWater = true; //it looks ugly when in water
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.direction = Main.rand.Next ((Projectile.direction - 5), (Projectile.direction + 5));
			if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}
		static List<Vector2> Heart()
		{
			List<Vector2> heartOffsets = new();
			for (float i = 0; i < 1; i += 2f / 30f)
			{
				Vector2 offset = Vector2.Lerp(new Vector2(0, 100), new Vector2(100, 0), i);
				heartOffsets.Add(offset);
				offset.X *= -1f;
				heartOffsets.Add(offset);
			}
			for (float i = MathF.PI * 0.75f; i < MathF.Tau * 0.89f; i += 1f / MathF.Tau)
			{
				Vector2 offset = (i).ToRotationVector2() * 70 + new Vector2(-50, -50);
				heartOffsets.Add(offset);
				offset.X *= -1;
				heartOffsets.Add(offset);
			}
			return heartOffsets;
		}
		public override void OnKill(int timeLeft)
        {
			List<Vector2> offsets = Heart();
            for (int i = 0; i < offsets.Count; i++)
            {
				Vector2 offset = offsets[i];
				Dust dust = Dust.NewDustPerfect(Projectile.Center + offset * 0.1f, DustID.TheDestroyer, offset * 0.05f, 0, default, 2);
				dust.noGravity = true;
            }

			SoundEngine.PlaySound(SoundID.Item67 with { MaxInstances = 0}, Projectile.Center); //rainbow gun
            for (float i = 0; i < 1; i += 1f / 20f)
            { Vector2 offset = Main.rand.NextVector2Circular(1000, 1000) / 100;
				Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.RainbowMk2, offset * 0.3f, 0, Color.Lerp(Color.Red, Color.HotPink, Main.rand.NextFloat()), 1 + Main.rand.NextFloat());
				dust.noGravity = true;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LoveDot>(), Projectile.damage / 10, 0, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, i * MathF.Tau);		
			}
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            // Redraw the projectile with the color not influenced by light
            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual projectile
            {
				Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                Vector2 drawOrigin = frame.Size() / 2;

                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                float scale = 1 - (0.05f * k);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }
            return true; //draw og
        }
    }
}