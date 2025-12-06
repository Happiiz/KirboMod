using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Gores
{
    public class KrackoEye : ModProjectile
    {
        public override string Texture => "KirboMod/NPCs/KrackoEyeBase";
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

        }
        public static int DecelerateDuration => 40;
        public static int LookLeftDuration => 45;
        public static int LookRightDuration => 45;
        public static int GoUpDuration => 60;
        public static int TotalAnimTime => DecelerateDuration + LookLeftDuration + LookRightDuration + GoUpDuration;
        public ref float Timer => ref Projectile.localAI[0];
        public override void AI()
        {
            Timer++;
            if(Timer >= TotalAnimTime)
            {
                Projectile.Kill();
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
