using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Security.Policy;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace KirboMod.Projectiles
{
	public class CresentSlash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			DrawOriginOffsetY = -38;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.timeLeft = 24;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesIDStaticNPCImmunity = true; 
			Projectile.idStaticNPCHitCooldown = 10; //wait 10 frames before dealing damage again for all cresent slashes
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 200, Color.White, 2f); //dust
			Main.dust[dustnumber].velocity *= 0.3f;
			Main.dust[dustnumber].noGravity = true;

			//For making sure it can't go through completely closed gaps

			for (int i = 0; i < 32; i++) //X
			{
				for (int j = 0; j < 32; j++) //Y
				{
					Point tilePoint = new Point(((int)Projectile.position.X / 16) + 1, ((int)Projectile.position.Y / 16) + 2);

					Tile tile = Main.tile[tilePoint.X + (i / 16), tilePoint.Y + (i / 16)];

					if (WorldGen.SolidOrSlopedTile(tile))
					{
						Projectile.Kill();
					}
				}
			}
        }

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 36; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, speed * 2, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = true;
			}
            for (int i = 0; i < 3; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
				speed.Normalize(); //unit of 1
				speed *= 10;
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.MetaBat>(), speed); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.damage = (int)(Projectile.damage * 0.75f); //reduce
		}

        public override bool? CanHitNPC(NPC target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }
        public override bool CanHitPvp(Player target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //make collision into a half circle
            return Utils.IntersectsConeFastInaccurate(targetHitbox, projHitbox.Center(), 100, Projectile.rotation, MathF.PI / 2);
        } 
    }
}