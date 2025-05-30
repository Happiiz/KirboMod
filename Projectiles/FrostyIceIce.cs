using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class FrostyIceIce : ModProjectile
    {
        readonly int style = Main.rand.Next(1, 4);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.scale = 0.1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            //scale with timeLeft
            Projectile.scale = Easings.EaseOutSquare(Utils.GetLerpValue(40, 30, Projectile.timeLeft, true));
            if (Main.rand.NextBool(100)) // happens 1/100 times
            {
                //swap X vel and Y vel(also make them negative)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity / 2, ModContent.ProjectileType<Projectiles.FrostySculpture>(), Projectile.damage / 2, 0f, Projectile.owner);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //stop projectile
            Projectile.velocity *= 0.1f;

            Projectile.timeLeft = 5;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0) //checks if the npc is dead
            {
                SoundEngine.PlaySound(SoundID.Item46, Projectile.position); //ice hydra
                for (int i = 0; i < 8; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(4f, 4f); //circle
                    Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, Scale: 1f); //Makes dust in a messy circle
                    d.noGravity = false;
                }

                Player player = Main.player[Projectile.owner]; //spawns ice chunk 
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position, new Vector2(player.direction * 5, 0), ModContent.ProjectileType<Projectiles.IceChunk>(), Projectile.damage * 2, 6, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextFloat(MathF.Tau);
                Projectile.localAI[1] = Main.rand.NextFloat(MathF.Tau);
                Projectile.localAI[2] = Main.rand.Next(0, 4);
            }
            Texture2D iceMist = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist" + style).Value;

            Color color = Color.LightCyan * Utils.GetLerpValue(0, 10, Projectile.timeLeft, true);

            float scale = Projectile.scale;
            SpriteEffects fx = Projectile.localAI[2] < 2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(iceMist, Projectile.Center - Main.screenPosition, null, color, Projectile.localAI[1], iceMist.Size() / 2, scale * 0.5f, fx);
            fx = Projectile.localAI[2] % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            color = Color.White * Utils.GetLerpValue(0, 5, Projectile.timeLeft, true);
            Main.EntitySpriteDraw(iceMist, Projectile.Center - Main.screenPosition, null, color, Projectile.localAI[0], iceMist.Size() / 2, scale * 0.25f, fx);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.CheckCircleCollision(targetHitbox, Projectile.Center, Projectile.scale * 90);
        }
    }
}