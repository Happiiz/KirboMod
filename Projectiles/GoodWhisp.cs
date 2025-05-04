using KirboMod.Items.Weapons;
using KirboMod.NPCs.NewWhispy;
using KirboMod.Projectiles.NewWhispy.NewWhispyWind;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class GoodWhisp : NewWhispySplittingWind
    {
        void Split()
        {
            if (Main.myPlayer != Projectile.owner)
                return;
            float randOffset = Main.rand.NextFloat(MathF.Tau);
            for (int i = 0; i < WindPipe.SplitProjCount; i++)
            {
                Puff.GetAIValues(i, WindPipe.SplitProjCount, WindPipe.SplitRadius, WindPipe.SplitProjDuration, out float ai0, out float ai1, out float ai2, out Vector2 velocity, randOffset);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocity, ModContent.ProjectileType<Puff>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0, ai1, ai2);
            }
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override void AI()
        {
            //Rectangle projHitbox = Projectile.Hitbox;
            //for (int i = 0; i < Main.maxNPCs; i++)
            //{
            //    NPC npc = Main.npc[i];
            //    if (npc.Hitbox.Intersects(projHitbox))
            //    {
            //        Projectile.Kill();
            //        return;
            //    }
            //}
            base.AI();
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(NewWhispyBoss.AirShotSplitSFX, Projectile.Center);
            Split();
        }

    }
}