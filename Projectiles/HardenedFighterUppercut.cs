using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class HardenedFighterUppercut : FighterUppercut
    {
        public override int AnimationDuration => 9;
        public override float HighSpeed => 40;
        public override float LowSpeed => 24f;
        public override Color EndColor => new Color(129, 90, 44) * 0.8f;
        public override Color StartColor => new(185, 129, 64);
        public override Color InnerStartColor => Color.Black with { A = 0 };
        public override Color InnerEndColor => Color.White;
        public override float YMult => 3f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.myPlayer == Projectile.owner)
            {
                int projCount = 9;
                float shootSpeed = 30;
                int type = ModContent.ProjectileType<HardenedPebble>();
                shootSpeed /= ContentSamples.ProjectilesByType[type].MaxUpdates;
                for (int i = 0; i < projCount; i++)
                {
                    float progress = (float)i / projCount;
                    float angle = progress * MathF.Tau;
                    Vector2 vel = angle.ToRotationVector2() * shootSpeed;
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center - vel, vel, type, (int)MathF.Max(1, (Projectile.damage * 0.1f)), Projectile.knockBack);
                }
            }
        }

    }
}