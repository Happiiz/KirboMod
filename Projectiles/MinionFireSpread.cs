using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MinionFireSpread : ModProjectile
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
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
		}

		public override void AI()
        {
            BurningLeoMinion leoOwner = Main.projectile[(int)Projectile.ai[0]].ModProjectile as BurningLeoMinion;

            if (leoOwner.Projectile.active == false)
            {
                Projectile.Kill(); //kill
                return;
            }

            Projectile.Center = leoOwner.Projectile.Center;

            Vector2 direction = GetSpreadDirection(leoOwner); //start - end

            float directionRotation = direction.ToRotation();

            Projectile.rotation = directionRotation; //offset a bit

            for (int i = 0; i < 1; i++)
            {
                Vector2 speed = Main.rand.NextVector2Unit(directionRotation - (MathF.Tau * 50 / 360 / 2), MathF.Tau * 50/360); //circle

                if (Projectile.ai[1] == 0) //which fire type?
                {
                    speed *= 20;
                }
                else
                {
                    speed *= 40;
                }
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed, Scale: 2);
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
            if (Projectile.ai[1] == 0) //which fire type?
            {
                return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, 200, Projectile.rotation, MathHelper.ToRadians(25));
            }
            else
            {
                return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, 400, Projectile.rotation, MathHelper.ToRadians(25));
            }
        }

        private Vector2 GetSpreadDirection(BurningLeoMinion leoOwner)
        {
            return leoOwner.aggroTarget.Center - leoOwner.Projectile.Center; //start - end;
        }
    }
}