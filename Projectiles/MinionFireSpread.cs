using KirboMod.NPCs;
using KirboMod.Projectiles.Flames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace KirboMod.Projectiles
{
	public class MinionFireSpread : FlameProj
	{
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.MinionShot[Type] = true;
        }
        protected override void FlamethrowerStats()
        {
            smokeColor = (Color.DarkGray) * .6f;
            startColor = Color.YellowGreen with { A = 158 };
            middleColor = Color.Orange with { A = 158 };
            endColor = Color.OrangeRed with { A = 158 };
            startScale = .4f;
            endScale = .7f;
            dustID = DustID.Torch;
            dustRadius = 50;
            dustChance = .5f;
            debuffID = BuffID.OnFire;
            debuffDuration = 120;
            duration = 20;
            fadeOutDuration /= 4;
            whiteInsideOpacity = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.friendly = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
            Projectile.damage = (int)(Projectile.damage * 0.5);//decay damage on hit
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
    }
}