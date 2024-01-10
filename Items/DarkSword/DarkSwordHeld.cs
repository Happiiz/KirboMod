using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.DarkSword
{
    public class DarkSwordHeld : ModProjectile, ITrailedHeldProjectile
    {
        ref float Timer { get => ref Projectile.localAI[0]; }
        ref float TrailCancelCount { get => ref Projectile.localAI[1]; }
        ref float UseTime { get => ref Projectile.ai[0]; }
        ref float SwingAngle { get => ref Projectile.ai[1]; }
        ref float SwingDir { get => ref Projectile.ai[2]; }
        bool Dead { get => Progress > 1; }
        float VisualRotation { get => Projectile.rotation - MathF.PI / 4; }
        float Progress { get => Utils.GetLerpValue(0, Projectile.ai[0], Projectile.localAI[0]); }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new(30);
            Projectile.extraUpdates = 8;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
            Projectile.hide = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale *= Main.player[Projectile.owner].GetAdjustedItemScale(Main.player[Projectile.owner].HeldItem);
            UseTime *= Projectile.MaxUpdates;
            Projectile.velocity.Normalize();
            Main.instance.LoadProjectile(Type);
        }
        static float EaseBackOut(float progress)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * MathF.Pow(progress - 1, 3) + c1 * MathF.Pow(progress - 1, 2);
        }
        static float AngleLerpLongWay(float curAngle, float targetAngle, float amount)
        {
            float angle;
            if (targetAngle < curAngle)
            {
                float num = targetAngle + MathF.Tau;
                angle = ((num - curAngle > curAngle - targetAngle) ? MathHelper.Lerp(curAngle, num, amount) : MathHelper.Lerp(curAngle, targetAngle, amount));
            }
            else
            {
                if (!(targetAngle > curAngle))
                {
                    return curAngle;
                }
                float num = targetAngle - MathF.Tau;
                angle = ((targetAngle - curAngle > curAngle - num) ? MathHelper.Lerp(curAngle, targetAngle, amount) : MathHelper.Lerp(curAngle, num, amount));
            }
            return MathHelper.WrapAngle(angle);
        }
        public override void AI()
        {
            if (Dead)
            {
                Projectile.timeLeft = 100;
                if (TrailCancelCount > Projectile.oldPos.Length)
                    Projectile.Kill();
                TrailCancelCount += 1;
                return;
            }
            float progress = EaseBackOut(Progress);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (SwingDir == -1)
                progress = 1 - progress;
            Projectile.rotation += MathF.PI / 4;
            if (SwingAngle >= MathF.PI)
            {
                Projectile.rotation = AngleLerpLongWay(Projectile.rotation - SwingAngle / 2f, Projectile.rotation + SwingAngle / 2f, progress);
            }
            else
            {
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation - SwingAngle / 2f, Projectile.rotation + SwingAngle / 2f, progress);
            }
            MakePlayerHoldMe();
            Timer++;
        }
        void MakePlayerHoldMe()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            if (UseTime - Timer > 1)
                player.SetDummyItemTime(2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, VisualRotation - MathF.PI / 2 - player.fullRotation);
            Vector2 dirToPlayer = VisualRotation.ToRotationVector2();
            Projectile.position = GetCenterPoint();
            Projectile.position += dirToPlayer * 117 * Projectile.scale;
        }
        Vector2 GetCenterPoint()
        {
            Player player = Main.player[Projectile.owner];
            return player.RotatedRelativePoint(player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation)) - Projectile.Size / 2f + GetExtraUpdateAdjustmentOffset(player);
        }
        Vector2 GetExtraUpdateAdjustmentOffset(Player player)
        {
            return Utils.GetLerpValue(0, Projectile.MaxUpdates, Projectile.numUpdates + 1) * -player.velocity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Dead)
                return false;
            float timeLeft = UseTime - Timer;
            timeLeft /= Projectile.MaxUpdates;
            float fade = Utils.GetLerpValue(0, 5, timeLeft, true);
            fade *= Projectile.Opacity;
            Texture2D texMain = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texMain, Projectile.Center - Main.screenPosition, null, Color.White * fade, Projectile.rotation, texMain.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            Vector2 offset = VisualRotation.ToRotationVector2() * 118;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - offset * Projectile.scale, Projectile.Center + offset * Projectile.scale, 70, ref a);
        }

        public void AddTrail()
        {
            float timeLeft = UseTime - Timer;
            timeLeft /= Projectile.MaxUpdates;
            float fade = Utils.GetLerpValue(0, 5, timeLeft, true);
            fade *= Projectile.Opacity;
            HeldProjTrailSystem.Trail.AddSubtractive(Projectile, 200, Color.Green * fade, Color.Gray * fade);
        }
    }
}
