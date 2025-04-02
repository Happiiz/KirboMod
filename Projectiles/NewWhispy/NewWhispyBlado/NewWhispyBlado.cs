using KirboMod.NPCs.NewWhispy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NewWhispy.NewWhispyBlado
{
    internal class NewWhispyBlado : ModProjectile
    {
        public override string Texture => "KirboMod/Projectiles/BladoProj";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blado");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        bool TouchedGround { get => Projectile.localAI[1] == 1; set => Projectile.localAI[1] = value ? 1 : 0; }
        ref float RotationSpeed => ref Projectile.localAI[2];
        ref float YPosToBecomeSolid => ref Projectile.ai[1];
        ref float AccelerationDirection => ref Projectile.ai[2];
        public static void GetAIValues(float telegraphTime, float yPosThresholdToBecomeSolid, float directionX, out float ai0, out float ai1, out float ai2)
        {
            ai0 = -telegraphTime;
            ai1 = yPosThresholdToBecomeSolid;
            if (directionX == 0)
                directionX = 1;
            ai2 = directionX;
        }
        public override bool PreAI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Projectile.ai[0];
            }
            if (Projectile.ai[0] < 0)
                Projectile.ai[0]++;
            Projectile.scale = Utils.GetLerpValue(Projectile.localAI[0], Projectile.localAI[0] + 10, Projectile.ai[0], true);
            return Projectile.ai[0] >= 0;
        }
        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 9999;
                SoundEngine.PlaySound(NewWhispyBoss.ObjFallSFX, Projectile.Center);
            }
            Projectile.tileCollide = Projectile.position.Y + Projectile.height / 2 > YPosToBecomeSolid;
            if (Projectile.velocity.Y == 0)
            {
                TouchedGround = true; //touch ground
            }

            Projectile.velocity.Y += 0.4f;
            if (Projectile.velocity.Y >= 12f)
            {
                Projectile.velocity.Y = 12f;
            }

            if (!TouchedGround)
            {
                RotationSpeed = Projectile.velocity.X * 0.05f; // rotates projectile
                Projectile.velocity.X *= 0.98f;

            }
            else //touched ground
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] < 5)
                {
                    RotationSpeed *= 0.95f;
                    Projectile.velocity.X *= 0.95f;
                }
                if (Projectile.ai[0] >= 5)
                {
                    if (AccelerationDirection == 0)
                    {
                        AccelerationDirection = 1;
                    }
                    RotationSpeed += MathHelper.ToRadians(AccelerationDirection * 0.5f);

                    if (RotationSpeed >= MathHelper.ToRadians(20))
                    {
                        RotationSpeed = MathHelper.ToRadians(20);
                    }
                    if (RotationSpeed <= MathHelper.ToRadians(-20))
                    {
                        RotationSpeed = MathHelper.ToRadians(-20);

                    }
                    if (Projectile.ai[0] == 5)
                    {
                        SoundEngine.PlaySound(SoundID.Item22, Projectile.Center); //motor loop
                    }
                    Point rightbelow = new Vector2(Projectile.Center.X, Projectile.position.Y + Projectile.height).ToTileCoordinates();
                    if (Projectile.ai[0] % 2 == 0 && Main.tile[rightbelow.X, rightbelow.Y].HasTile)
                    {
                        int dust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.position.Y + Projectile.height), 1, 1, DustID.Smoke, Projectile.direction * -0.4f, -2f, 0, default, 2f); //dust
                        Main.dust[dust].noGravity = true;
                    }
                }

                if (Projectile.ai[0] >= 5)
                {
                    Projectile.velocity.X += AccelerationDirection * 0.2f;
                }
            }

            Projectile.rotation += RotationSpeed; //rotation
            if (Projectile.tileCollide)
            {
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawSelf(false);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.CheckCircleCollision(targetHitbox, Projectile.Center, 32);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //doesn't die
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {

            fallThrough = false;
            return true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 100);
        }
    }
}
