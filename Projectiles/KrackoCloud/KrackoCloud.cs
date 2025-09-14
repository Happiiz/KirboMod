using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.KrackoCloud
{
    //vilethorn-esque logic, 
    public class KrackoCloud : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/IceMist/IceMist1";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.alpha = 255 - 255 / 15;
        }
        ref float Style => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        ref float LeftToSpawn => ref Projectile.ai[2];
        static int CloudProjSpacing => 60;
        public static void SpawnCloudLine(IEntitySource source, Vector2 start, Vector2 velocity, int length, int damage)
        {
            velocity.SafeNormalize(Vector2.UnitX);
            velocity *= 0.001f;
            length /= CloudProjSpacing;
            Projectile.NewProjectile(source, start, velocity, ModContent.ProjectileType<KrackoCloud>(), damage, 0f, -1, Main.rand.Next(1, 4), 0f, length);
        }
        public override void AI()
        {
            Timer++;
            if (Timer > 15)
            {
                Projectile.alpha += 255 / 30;
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
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity.Normalized(CloudProjSpacing), Projectile.velocity, Type, Projectile.damage, 0f, -1, Main.rand.Next(1, 4), 0f, LeftToSpawn - 1);
                }
            }

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextFloat(MathF.Tau);
                Projectile.localAI[1] = Main.rand.NextFloat(MathF.Tau);
                Projectile.localAI[2] = Main.rand.Next(0, 4);
            }
            Texture2D iceMist = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist" + Style).Value;
            Color color = Color.White * Projectile.Opacity;

            float scale = Projectile.scale * .75f;
            SpriteEffects fx = Projectile.localAI[2] < 2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(iceMist, Projectile.Center - Main.screenPosition, null, color, Projectile.localAI[1], iceMist.Size() / 2, scale * 0.5f, fx);
            fx = Projectile.localAI[2] % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(iceMist, Projectile.Center - Main.screenPosition, null, color, Projectile.localAI[0], iceMist.Size() / 2, scale * 0.25f, fx);
            return false;
        }
    }
}
