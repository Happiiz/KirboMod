using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Lightnings
{
    public class GoodDarkMatterLaser : LightningProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            outerColor = Color.MediumSlateBlue;
            innerColor = Color.Black;
            width = 15;
            Projectile.scale = 1.5f;
            opacityFunction = OpacityFunction;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }
        float OpacityFunction(float progress)
        {
            return 1;
        }
    }
}
