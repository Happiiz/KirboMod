using KirboMod.NPCs;
using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroSpark : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spark");
			Main.projFrames[Projectile.type] = 1;
		}
		static int Lifetime => 90;
		static int ExplosionDuration => 20;
		bool Exploded { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = Lifetime + ExplosionDuration;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			Projectile.velocity *= 0.96f;
			Projectile.localAI[1]++;
			if (Projectile.localAI[1] >= Lifetime && !Exploded)
			{
                Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(100));
                Projectile.friendly = false;
                Projectile.hostile = true;
                Projectile.tileCollide = false;
                Projectile.penetrate = -1;
                Projectile.scale = 1f;
                Projectile.alpha = 50;
                Exploded = true;
				Projectile.velocity = default;
                SoundEngine.PlaySound(SoundID.Item11.WithVolumeScale(0.8f), Projectile.Center);//boom
            }
        }
      

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return false;
        }

		public static Vector2 AccountForVelocity(Vector2 targetPos, Vector2 targetVelocity)
		{
			return targetPos + targetVelocity * Lifetime;
		}
        public override bool PreDraw(ref Color lightColor)
        {
			if (Exploded)
			{
				Main.instance.LoadProjectile(ModContent.ProjectileType<ZeroSparkExplosion>());
				Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<ZeroSparkExplosion>()].Value;
				Projectile.scale = Utils.GetLerpValue(ExplosionDuration, 0, Projectile.timeLeft, true);
                Projectile.scale = Easings.EaseOut(Projectile.scale, 2);
                Projectile.scale = MathHelper.Lerp(1, 1 + 0.05f * ExplosionDuration, Projectile.scale);
                Projectile.Opacity = Utils.Remap(Projectile.timeLeft, ExplosionDuration * .7f, 0, 0.8f, 0);
                Lighting.AddLight(Projectile.Center, 1f, 0.9f, 0);
                Projectile.velocity = default;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
				return false;
			}
			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = Main.rand.NextFloat(-.1f, .1f);
				Projectile.localAI[2] = Main.rand.NextBool() ? -1 : 1;
			}
			float scale = Utils.GetLerpValue(Lifetime * .3f, Lifetime * .8f, Projectile.localAI[1]);
			scale = Easings.EaseInOutSine(scale);
			scale *= 2;
			Vector2 scaleVec = new Vector2(scale);
			float rotation = Utils.GetLerpValue(Lifetime * .4f, Lifetime, Projectile.localAI[1], true);
			rotation = Easings.EaseIn(rotation, 4);
			rotation *= MathF.PI;
            rotation *= Projectile.localAI[2];
            rotation += Projectile.localAI[0];
			Color whiteAdditive = new Color(255, 255, 255, 0);
			VFX.DrawGlowBallDiffuse(Projectile.Center, scale * 1.25f, Color.Black * .5f, default);
      
            VFX.DrawPrettyStarSparkle(1, Projectile.Center - Main.screenPosition, whiteAdditive, Color.Blue with { A = 0 }, Projectile.localAI[1],
				0, 5, Lifetime - 0.001f, Lifetime, rotation, scaleVec , scaleVec / Helper.Phi);
			if (Projectile.localAI[1] < Lifetime / 2)
            VFX.DrawGlowBallAdditive(Projectile.Center, 0.4f, Color.Blue, Color.White, false);
			return false;
        }
        public override bool CanHitPlayer(Player target)
        {
			return Exploded;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}