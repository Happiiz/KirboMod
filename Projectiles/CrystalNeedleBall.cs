using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class CrystalNeedleBall : ModProjectile
    {
        public static int DistRequiredForTrap => 16 * 3;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            //modified damaged hitbox.
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 7;
            Projectile.usesLocalNPCImmunity = true; //wait for no one
            Projectile.localNPCHitCooldown = 3;
        }
        public override void AI()
        {
            Projectile.ai[0]+= Projectile.position.Distance(Projectile.oldPosition);
            if (Projectile.ai[0] >= DistRequiredForTrap && Projectile.velocity.Y == 0 && Main.myPlayer == Projectile.owner) //every 14 ticks and on the ground
            {
                Projectile.ai[0] %= DistRequiredForTrap;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Bottom.X - MathF.Sign(Projectile.velocity.X) * Projectile.ai[0] + Main.rand.NextFloat(-8,8), Projectile.Bottom.Y - 16, Projectile.velocity.X * 0.01f, Projectile.velocity.Y * 0.01f, ModContent.ProjectileType<CrystalTrap>(), Projectile.damage / 5, Projectile.knockBack, Projectile.owner, 0, 0, 0);
            }

            //Gravity
            Projectile.velocity.Y = Projectile.velocity.Y + 0.6f;
            if (Projectile.velocity.Y >= 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            //Rotation
            Projectile.rotation += Projectile.velocity.X * 0.02f;

            if (Projectile.velocity.Y == 0)
            {
                Projectile.velocity.X *= 0.992f; //slow down
            }

            if (Main.rand.NextBool(3)) // happens 1/3 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 102, 102, ModContent.DustType<Dusts.RainbowSparkle>(), 0f, 0f, 200, default, 1f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
            }

            //for stepping up tiles
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            //crystal
            for (int i = 0; i < 30; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, 91, speed * 3, 0, new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255)), Scale: 1.5f); //Makes dust in a messy circle
            }
            if (Main.myPlayer == Projectile.owner)
            {
                //crystal clutter projectiles
                for (int i = 0; i < 3; i++)
                {
                    float topVal = 35;
                    Vector2 vel = new Vector2(Projectile.direction * topVal, Main.rand.NextFloat(-topVal, 20 -topVal));
                    vel.Normalize();
                    vel *= 15f;
                    vel += Main.rand.BetterNextVector2Circular(3);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<Projectiles.CrystalClutter>(), Projectile.damage, 10f, Projectile.owner, 0, 0);
                }
            }
            SoundEngine.PlaySound(SoundID.Item27 with { MaxInstances = 0}, Projectile.Center); //crystal break
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) //bounce
            {
                Projectile.velocity.X = -oldVelocity.X;
                Projectile.ai[0]++;//avoid stacking spike shards
            }
            return false; //dont die
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through platforms
            return true;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(100));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawSelf(Color.White);
        }
    }
}