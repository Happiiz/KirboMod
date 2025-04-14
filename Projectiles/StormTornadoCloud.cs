using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class StormTornadoCloud : ModProjectile
    {
        readonly int style = Main.rand.Next(1, 4);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            //waits for its own hit cooldown
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] += 1;
                Projectile.rotation += MathF.Tau * Main.rand.NextFloat();
            }
            Projectile.velocity *= 0.9f; //slow
            Projectile.rotation += Projectile.direction * 0.04f; // rotates projectile depending on direction it's facing

            Projectile.Opacity = Utils.GetLerpValue(180, 140, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 40, Projectile.timeLeft, true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Cloud = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist" + style).Value;

            Color color = new Color(44, 0, 44) * Projectile.Opacity;

            Main.EntitySpriteDraw(Cloud, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, Cloud.Size() / 2, 0.25f, SpriteEffects.None);

            color = new Color(44, 0, 44) * Projectile.Opacity;

            Main.EntitySpriteDraw(Cloud, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, Cloud.Size() / 2, 0.25f, SpriteEffects.None);

            return false;
        }
    }
}