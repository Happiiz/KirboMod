using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class PoppyBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            DrawOriginOffsetY = -12;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
        }
        ref float YVel { get => ref Projectile.ai[1]; }
        public static float BombAcceleration { get => Main.expertMode ? .4f : .25f; }
        public static float BombMaxSpeed { get => Main.expertMode ? 10 : 6f; }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.04f;
            float terminalVel = BombMaxSpeed;
            if (YVel >= terminalVel)
            {
                Projectile.position.Y += terminalVel;
            }
            else
            {
                Projectile.position.Y += YVel / 2;
                YVel += BombAcceleration;
                Projectile.position.Y += YVel / 2;
            }
            Dust dust = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation - MathF.PI / 2).ToRotationVector2() * 26, DustID.Torch);
            dust.noGravity = true;
            dust.velocity *= .5f;
            dust.scale *= 2;
            dust.velocity += Projectile.velocity;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill(); //ensuring that it'll detonate upon player contact
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, Projectile.Size * 2);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position); //bomb sound
            for (int i = 0; i < 20; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, speed, Main.rand.Next(16, 18), 1);
                speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
            }
            for (int i = 0; i < 35; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 0, default, 2);
                d.velocity.Y -= 1;
            }
        }
    }
}