using KirboMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.IceBombFrag
{
    internal class MarxIceBombFrag : ModProjectile
    {
        public static SoundStyle IceBombRightRollSFX => new("KirboMod/Sounds/NPC/Marx/IceBombRightRoll");
        public static SoundStyle IceBombLeftRollSFX => new("KirboMod/Sounds/NPC/Marx/IceBombRightRoll");

        public override void SetDefaults()
        {
            Projectile.coldDamage = true;
            Projectile.width = Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 30;
            Projectile.scale = 2f;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
            Main.projFrames[Type] = 4;
        }
        ref float Timer => ref Projectile.localAI[0];
        ref float DistTravelled => ref Projectile.localAI[1];
        int TargetPlayerIndex => (int)Projectile.ai[0];
        bool IsProjForClient => TargetPlayerIndex == Main.myPlayer;
        bool PlaySFX => MathF.Abs(Projectile.ai[1]) == 1;
        public override void AI()
        {
            DistTravelled += Projectile.velocity.Length();
            Timer++;
            float dustInterval = 40f;
            if (Projectile.frameCounter++ >= 2)
            {
                Projectile.frameCounter %= 2;
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Type];
            }

            if (DistTravelled > dustInterval)
            {
                DistTravelled %= dustInterval;
                Vector2 offset = -Projectile.velocity.Normalized(DistTravelled + 10);
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, ModContent.DustType<IceMistDust>(), Projectile.velocity * 0.25f, 0, Color.White, 0.45f);
                if (!IsProjForClient)
                {
                    d.alpha = 200;
                }
            }
            if (Timer == 1)
            {
                if (PlaySFX)
                {
                    SoundEngine.PlaySound(Projectile.ai[1] >= 0 ? IceBombRightRollSFX : IceBombLeftRollSFX, Projectile.Center, SoundUpdateCallback);
                }
            }
        }

        bool SoundUpdateCallback(ActiveSound soundInstance)
        {
            soundInstance.Position = Projectile.position;
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!IsProjForClient)
            {
                Projectile.alpha = 200;
            }
            return Projectile.DrawSelf(Color.White with { A = 128 });
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!IsProjForClient)
            {
                return false;
            }
            return null;
        }
    }
}
