using KirboMod.NPCs;
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
	public class DuoMinionFireSpread : ModProjectile
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
			Projectile.penetrate = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
			Projectile.stopsDealingDamageAfterPenetrateHits = true; //cancels out damage without killing projectile
		}

		public override void AI()
        {
            DuoBurningLeoMinion leoOwner = Main.projectile[(int)Projectile.ai[0]].ModProjectile as DuoBurningLeoMinion;

            if (leoOwner.Projectile.active == false)
            {
                Projectile.Kill(); //kill
                return;
            }

            Projectile.Center = leoOwner.Projectile.Center;

            Vector2 direction = GetSpreadDirection(leoOwner); //start - end

            float directionRotation = direction.ToRotation();

            Projectile.rotation = directionRotation; //offset a bit

            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = Main.rand.NextVector2Unit(directionRotation - (MathF.Tau * 50 / 360 / 2), MathF.Tau * 50/360); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed * 20, Scale: 2);
				d.noGravity = true;
            }

			if (leoOwner.attacking) //attacking
			{
                Projectile.timeLeft = 2; //keep alive
            }
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }

        public override bool? CanHitNPC(NPC target) //can hit only if there's a line of sight
        {
            if (Collision.CanHit(Projectile, target))
            {
                return null;
            }
            return false;
        }
        public override bool CanHitPvp(Player target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, 200, Projectile.rotation, MathHelper.ToRadians(25));
        }

        private Vector2 GetSpreadDirection(DuoBurningLeoMinion leoOwner)
        {
            return leoOwner.aggroTarget.Center - leoOwner.Projectile.Center; //start - end;
        }
    }
}