using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ZeroBloodShot : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Blood Shot");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		ref float Timer => ref Projectile.localAI[2];
		int TargetIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
		public static void GetAIValues(int targetIndex, out float ai0)
		{
			ai0 = targetIndex;
		}

		public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
			Timer++;
			Projectile.velocity.X += Projectile.direction * .3f;
			if (TargetIndex >= 0 && TargetIndex < Main.maxPlayers)
			{
				Player target = Main.player[TargetIndex];
				if (Projectile.position.Y + Projectile.height > target.position.Y && Projectile.position.Y < target.position.Y + target.height)
				{
					Projectile.velocity.Y *= 0.98f;//slow down
				}
				else
				{
					if (Projectile.Center.Y < target.Center.Y)//above player
					{
						Projectile.velocity.Y += 0.25f;//fall to match
					}
					else
					{
						Projectile.velocity.Y -= 0.25f;
					}
				}
			}
            if (Main.rand.NextBool(5)) // happens 1/5 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Redsidue>(), 0f, 0f, 0, Color.White, 0.5f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
                Main.dust[dustnumber].GetColor(Color.White);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {		
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 scale = new Vector2(Utils.Remap(MathF.Abs(Projectile.velocity.X), 0, 20, .3f, 3), 1) * Projectile.scale;
		    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2, scale, direction);
			return false;
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}