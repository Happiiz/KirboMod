using KirboMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
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
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = Lifetime;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			Projectile.velocity *= 0.96f;
			Projectile.localAI[1]++;
        }
         public override void OnKill(int timeLeft) //when the projectile dies
         {
            SoundEngine.PlaySound(SoundID.Item11.WithVolumeScale(0.8f), Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity *= 0.01f, ModContent.ProjectileType<ZeroSparkExplosion>(), 100 / 2, 12f, Main.myPlayer);
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

			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = Main.rand.NextFloat(-.1f, .1f);
				Projectile.localAI[2] = Main.rand.NextBool() ? -1 : 1;
			}
			float scale = Utils.GetLerpValue(Lifetime * .3f, Lifetime * .8f, Projectile.localAI[1], true);
			scale = Easings.EaseInOutSine(scale);
			scale *= 2;
			Vector2 scaleVec = new Vector2(scale);
			float rotation = Utils.GetLerpValue(Lifetime * .4f, Lifetime, Projectile.localAI[1], true);
			rotation = Easings.EaseIn(rotation, 4);
			rotation *= MathF.PI;
            rotation *= Projectile.localAI[2];
            rotation += Projectile.localAI[0];
			Color whiteAdditive = new Color(255, 255, 255, 0);
			VFX.DrawGlowBallDiffuse(Projectile.Center, scale, Color.Black * .5f, default);
      
            VFX.DrawPrettyStarSparkle(1, Projectile.Center - Main.screenPosition, whiteAdditive, Color.Blue with { A = 0 }, Projectile.localAI[1],
				0, 5, Lifetime - 0.001f, Lifetime, rotation, scaleVec , scaleVec / Helper.Phi);
			if (Projectile.localAI[1] < Lifetime / 2)
            VFX.DrawGlowBallAdditive(Projectile.Center, 0.4f, Color.Blue, Color.White, false);
			return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}