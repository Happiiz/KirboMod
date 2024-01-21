using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MinionBeamSpread : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override string Texture => "KirboMod/NothingTexture";

        public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.timeLeft = 2;
			Projectile.tileCollide = false;
			Projectile.penetrate = 5;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 15;
			Projectile.stopsDealingDamageAfterPenetrateHits = true; //cancels out damage without killing projectile
		}

		public override void AI()
        {
            WaddleDooMinion dooOwner = Main.projectile[(int)Projectile.ai[0]].ModProjectile as WaddleDooMinion;

            if (dooOwner.Projectile.active == false)
            {
                Projectile.Kill(); //kill
                return;
            }

            Projectile.Center = dooOwner.Projectile.Center;

            Vector2 direction = GetSpreadDirection(dooOwner);

            float directionRotation = direction.ToRotation();

            Projectile.rotation = directionRotation - (MathF.Tau * 50 / 360 / 2); //offset a bit

            if (dooOwner.attacking) //attacking
			{
                Projectile.timeLeft = 2; //keep alive
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override bool? CanHitNPC(NPC target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }
        public override bool CanHitPvp(Player target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }

        public override void PostDraw(Color lightColor)
        {
            WaddleDooMinion dooOwner = Main.projectile[(int)Projectile.ai[0]].ModProjectile as WaddleDooMinion;

            Vector2 direction = GetSpreadDirection(dooOwner);

            direction.Normalize();

            for (int i = 1; i < 6; i++)
            {
                VFX.DrawElectricOrb(dooOwner.Projectile.Center + direction.RotatedBy(MathF.Sin((Main.GlobalTimeWrappedHourly * 10) - MathHelper.ToRadians(10 * i))) * (30 * i),
                        new Vector2(1.2f), Projectile.Opacity, Projectile.rotation);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, 180, Projectile.rotation + MathHelper.ToRadians(30), MathHelper.ToRadians(60));
        }

        private Vector2 GetSpreadDirection(WaddleDooMinion dooOwner)
        {
            return dooOwner.aggroTarget.Center - dooOwner.Projectile.Center; //start - end;
        }
    }
}