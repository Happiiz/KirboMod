using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Flames
{
    public class BatafireFire : ModProjectile //used star code as base
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public static void SpawnWithGravity(IEntitySource source, Vector2 from, Vector2 initialVel, int damage, float gravity)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            Projectile.NewProjectile(source, from, initialVel, ModContent.ProjectileType<BatafireFire>(), damage, 0f, -1, gravity);
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
           
        }
        ref float Gravity => ref Projectile.ai[0];
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(MathF.Tau);
                Projectile.localAI[0] = 1;
                if (Gravity != 0)//can'	t do in setdefaults
                {
                    Projectile.timeLeft = 5000;
                }
            }

            Projectile.velocity.Y += Gravity;
            int cap = Projectile.height;
            //cap velocity so it doesn't phase through players
            if (Projectile.velocity.Y > cap)
            {
                Projectile.velocity.Y = cap;
            }
            Lighting.AddLight(Projectile.Center, TorchID.Torch);

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;

            if (Main.rand.NextBool(2)) // happens 1/2 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 50, 50, DustID.Torch, Scale: 2f);
                Main.dust[dustnumber].noGravity = true;
            }
        }
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            VFX.DrawProjWithStarryTrail(Projectile, Color.Orange, Color.Yellow * 0.4f, Color.Orange, 0.4f, 0, 0, (byte)(128 * Projectile.Opacity));
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}