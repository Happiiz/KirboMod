using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.BandanaDee
{
	public class BandanaDeeSpearThrown : ModProjectile //referenced blizzard icicle code
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 6;
			ProjectileID.Sets.TrailingMode[Type] = 2;

            ProjectileID.Sets.MinionShot[Type] = true;
        }

		public static float Gravity = 0.2f;

        public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
			DrawOffsetX = -12;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.timeLeft = 600; //10 seconds
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
				Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
				Main.EntitySpriteDraw(texture, drawPos, null, lightColor * (1 - (float)i / Projectile.oldPos.Length), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            }

			return true;
        }
        public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			Projectile.velocity.Y += Gravity;
			if (Projectile.velocity.Y > 30f)
            {
				Projectile.velocity.Y = 30f;
            }
		}
    }
}