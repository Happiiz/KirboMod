using KirboMod.Projectiles.Lightnings;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Flames
{
    public class BadFire : FlameProj
    {
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
            Projectile.hostile = true;
        }
    }
}