using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CrystalClutter : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = true;
        }
		public override void AI()
		{
			Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
			if (Projectile.velocity.Y >= 3f)
            {
				Projectile.velocity.Y = 3f;
            }
			Projectile.rotation += 0.03f * (float)Projectile.direction * Projectile.velocity.X; // rotates projectile
		}
         public override void OnKill(int timeLeft) //when the projectile dies
         {
             for (int i = 0; i < 20; i++)
             {
                 Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                 Dust d = Dust.NewDustPerfect(Projectile.position, 91, speed * 3, 0, new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255)), Scale: 1f); //Makes dust in a messy circle
             }
			
			SoundEngine.PlaySound(SoundID.Item27 with {Volume = 0.5f, MaxInstances = 0}, Projectile.Center); //quiet crystal break
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return false;
		}
        public override bool PreDraw(ref Color lightColor)
        {
			return Projectile.DrawSelf(Color.White);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through platforms
            return true;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
			hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(58));
        }
	}
}