using KirboMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace KirboMod.Projectiles
{
    public class PlasmaZap : BadPlasmaZap
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true; //make collide
        }
    }
}