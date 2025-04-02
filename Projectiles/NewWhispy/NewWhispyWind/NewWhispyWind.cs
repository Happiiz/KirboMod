using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NewWhispy.NewWhispyWind
{
    public class NewWhispyWind : Whisp
    {
        public override string Texture => "KirboMod/Projectiles/NewWhispy/NewWhispyWind/NewWhispyWindSmall";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 30;
            Projectile.hostile = true;
            DrawOriginOffsetY = -10;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        int TargetIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public static void GetAIValues(int targetIndex, out float ai0)
        {
            ai0 = targetIndex;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if(TargetIndex != Main.myPlayer)
            {
                Projectile.alpha = 128;
            }
            return true;
        }
        public override bool CanHitPlayer(Player target)
        {
            return target.whoAmI == TargetIndex;
        }
        public override void PostAI()
        {
            //localai0 is distance travelled
            //Projectile.localAI[0] += Projectile.velocity.X;
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
            }
            float homingStrength = 0.05f;

            //if (Projectile.localAI[0] > 16 * 45)
            //{
            //    homingStrength = 0.15f;
            //}
            if (TargetIndex >= 0 && TargetIndex < Main.maxPlayers)
            {
                Player target = Main.player[TargetIndex];
                if (Projectile.position.Y + Projectile.height > target.position.Y && Projectile.position.Y < target.position.Y + target.height)
                {
                    Projectile.velocity.Y *= 0.95f;//slow down
                }
                else
                {
                    if (Projectile.Center.Y < target.Center.Y)//above player
                    {
                        Projectile.velocity.Y += homingStrength;//fall to match
                    }
                    else
                    {
                        Projectile.velocity.Y -= homingStrength;
                    }
                }
            }
        }

    }
}
