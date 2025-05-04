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
            Projectile.penetrate = 6;

            //waits for its own hit cooldown
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextFloat(MathF.Tau);
                Projectile.localAI[1] = Main.rand.NextFloat(MathF.Tau);
                Projectile.localAI[2] = Main.rand.Next(0, 4);
            }
            Projectile.velocity *= 0.9f; //slow
            Projectile.rotation += Projectile.direction * 0.04f; // rotates projectile depending on direction it's facing

            Projectile.Opacity = Utils.GetLerpValue(180, 175, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 40, Projectile.timeLeft, true);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Cloud = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist" + style).Value;
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = Main.rand.Next(1, 4);
            }
            int style2 = (int)Projectile.ai[0];
            Texture2D Cloud2 = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist" + style2).Value;

            //Color color = new Color(44, 0, 44) * Projectile.Opacity;
            Color color = Color.Black * Projectile.Opacity;
            SpriteEffects fx = Projectile.localAI[2] < 2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(Cloud, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation + Projectile.localAI[0], Cloud.Size() / 2, 0.25f, fx);
            fx = Projectile.localAI[2] % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(Cloud2, Projectile.Center - Main.screenPosition, null, color, -Projectile.rotation + Projectile.localAI[1], Cloud.Size() / 2, 0.25f, fx);

            return false;
        }
    }
}