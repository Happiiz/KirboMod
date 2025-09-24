using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace KirboMod.Projectiles.Marx.Vine
{
    public class MarxVine : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
        ref float Timer => ref Projectile.ai[1];
        ref float LeftToSpawn => ref Projectile.ai[2];
        static int VineProjSpacing => 40;
        public static void SpawnVine(IEntitySource source, Vector2 start, int length, int damage)
        {
            Vector2 velocity = -Vector2.UnitY;
            length /= VineProjSpacing;
            Projectile.NewProjectile(source, start, velocity, ModContent.ProjectileType<MarxVine>(), damage, 0f, -1, Main.rand.Next(1, 4), 0f, length);
        }
        public override void AI()
        {
            Timer++;
            if (Timer > 100)
            {
                Projectile.alpha += 255 / 10;
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.alpha -= 255 / 10;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            Projectile.hostile = Timer > 5 && Projectile.alpha < 200;
            if (Timer == 5 && LeftToSpawn > 0)
            {
                LeftToSpawn--;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity.Normalized(VineProjSpacing), Projectile.velocity, Type, Projectile.damage, 0f, -1, Main.rand.Next(1, 4), 0f, LeftToSpawn - 1);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI * .5f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI * .5f;

            return Projectile.DrawSelf(Color.White);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
