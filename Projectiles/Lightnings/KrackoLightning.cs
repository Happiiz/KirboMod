using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Lightnings
{
    public class KrackoLightning : LightningProj
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetStaticDefaults();

            base.SetDefaults();
            Projectile.hostile = true;
            innerColor = Color.White;
            outerColor = Color.Lerp(Color.White, VFX.RndElectricCol, 0.4f);
            width = 10;
            maxDeviation = 200;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 90);
        }
    }
}
