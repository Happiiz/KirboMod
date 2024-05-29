using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PlasmaLaser : BadPlasmaLaser
	{
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.width = 11;
            Projectile.width = 11;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //multiply by 2 to make equal to bad hitbox
            return Utils.CenteredRectangle(Projectile.Center, Projectile.Size * 2).Intersects(targetHitbox);
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Projectile.velocity * Main.rand.NextFloat(1f, 2f); //spread
                Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade,
                    velocity.RotatedByRandom(Math.PI / 8), Scale: 1f);
            }
        }
    }
}