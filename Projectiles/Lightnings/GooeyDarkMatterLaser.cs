using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Lightnings
{
    public class GooeyDarkMatterLaser : LightningProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            outerColor = Color.RoyalBlue;
            innerColor = Color.Black;
            width = 15;
            Projectile.scale = 1.5f;
            opacityFunction = OpacityFunction;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ArmorPenetration = 30;
            SetAmountOfLightingSegments(7, Projectile.type);
        }
        float OpacityFunction(float progress)
        {
            return 1;
        }
    }
}
