using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace KirboMod.Projectiles
{
    public class DarkMatterShot : ModProjectile
    {
        int initalDir = 1;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80; //not actual size just hitbox
            Projectile.height = 80;
            DrawOriginOffsetY = -3;
            DrawOffsetX = -28;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 500;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.hide = true;
        }

        public override string Texture => "KirboMod/NPCs/PureDarkMatter";

        public static float TimeBeforeAccelerating => 45;
        ref float SpawnPosX => ref Projectile.localAI[1];
        ref float SpawnPosY => ref Projectile.localAI[2];
        ref Vector2 TargetPos => ref Projectile.velocity;
        public static float Acceleration => 0.3f;
        ref float Timer => ref Projectile.localAI[0];
        //call this
        public static void AccountForSpeed(ref Vector2 offset, Player target)
        {
            offset.X += target.velocity.X * TimeBeforeAccelerating;
            offset.Y += target.velocity.Y * 0.75f * TimeBeforeAccelerating;
        }
        public static void NewDarkMatterShot(NPC zero, Vector2 target, Vector2 from, int damage, float directionY)
        {
            Projectile.NewProjectile(zero.GetSource_FromAI(), from, target, ModContent.ProjectileType<DarkMatterShot>(), damage, 0, -1, from.X, from.Y, directionY);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);
        public override void AI()
        {
            Projectile.Opacity += .1f;
            if (Timer == 0)
            {
                SpawnPosX = Projectile.Center.X;
                SpawnPosY = Projectile.Center.Y;
            }
            if (Timer > TimeBeforeAccelerating + 1)
            {
                if(Timer == TimeBeforeAccelerating + 2)
                {
                    Projectile.velocity = default;
                }
                Projectile.velocity.Y -= Acceleration * Projectile.ai[2];
            }
            else
            {
                float progress = Utils.GetLerpValue(0, TimeBeforeAccelerating, Timer, true);
                UnifiedRandom rnd = new UnifiedRandom(Projectile.identity);
                Projectile.position.X = MathHelper.Lerp(SpawnPosX, TargetPos.X, RandomEasing(rnd.Next(3), progress, 1)) - Projectile.width / 2;
                Projectile.position.Y = MathHelper.Lerp(SpawnPosY, TargetPos.Y, RandomEasing(rnd.Next(3), progress, 1)) - Projectile.height / 2;
                Projectile.position -= Projectile.velocity;
            }
            Projectile.spriteDirection = (int)Projectile.ai[2];
            Projectile.rotation = MathF.PI / 2 * Projectile.ai[2];
            if (Projectile.spriteDirection < 0)
            {
                Projectile.rotation += MathF.PI;
            }
            Timer++;

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Timer > TimeBeforeAccelerating && projHitbox.Intersects(targetHitbox);
        }
        public override bool ShouldUpdatePosition()
        {
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; // Makes it uneffected by light
        }
        static float RandomEasing(int index, float progress, float exponent)
        {
            return index switch
            {
                0 => Easings.InOutCirc(progress),
                1 => Easings.EaseInOutSine(progress),
                _ => Easings.BackInOut(progress),
            };
        }
    }
}