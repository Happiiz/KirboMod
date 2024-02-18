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
            base.SetDefaults();
            Projectile.hostile = true;
            innerColor = Color.White;
            outerColor = Color.Lerp(Color.White, VFX.RndElectricCol, 0.4f);
            width = 10;
            maxDeviation = 150;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            outerColor = Color.Lerp(Color.White, VFX.RndElectricCol, 0.4f);
            return base.PreDraw(ref lightColor);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            int duration = 120;
            if (Main.masterMode)
            {
                duration *= 3;
            }
            else if (Main.expertMode)
            {
                duration *= 2;
            }
            target.AddBuff(BuffID.Electrified, duration);
        }
    }
}
