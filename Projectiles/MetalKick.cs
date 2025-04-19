using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class MetalKick : FighterUppercut
    {
        public override Color StartColor => Color.LightGray;
        public override Color EndColor => Color.DarkGray;
        public override int AnimationDuration => 20;
        public override float LowSpeed => 40;
        public override float HighSpeed => 70;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0]++;
            if (Timer > AnimationDuration)
            {
                Projectile.damage = -1;
                if (Timer > AnimationDuration * 2)
                {
                    Projectile.Kill();
                }
                return;
            }
            Projectile.spriteDirection = player.direction; //player speed will be updated before projectile AI so this is fine I think?
            Projectile.rotation = player.velocity.ToRotation() + MathF.PI / 2;
            Projectile.Center = player.MountedCenter + new Vector2(player.direction * 12, 10);
            if (MakePlayerInvincible)
            {
                ClampIframes(player);
            }
        }
        public static MetalKick SampleInstance => ContentSamples.ProjectilesByType[ModContent.ProjectileType<MetalKick>()].ModProjectile as MetalKick;
        public static Vector2 GetSpeed(Player player, bool touchingTiles, MetalKick sampleInstance, float timer)
        {
            float angle = .8f;
            if (touchingTiles)
            {
                angle = 0f;
            }
            Vector2 dir = new(MathF.Cos(angle), MathF.Sin(angle));
            dir.X *= player.direction;
            float decelerate = Utils.Remap(sampleInstance.AnimationDuration - timer, sampleInstance.AnimationDuration - sampleInstance.DecelerateDuration, sampleInstance.AnimationDuration, 1f, 0.2f);
            int whoAmI = player.whoAmI;
            int type = sampleInstance.Type;
            bool makeInvincible = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == type && proj.owner == whoAmI && proj.ai[1] == 1)
                {
                    makeInvincible = true;
                }
            }
            float speed = makeInvincible ? sampleInstance.HighSpeed : sampleInstance.LowSpeed;
            return dir * speed * decelerate;
        }
        public Vector2 ScanUntilHitTiles(Vector2 vel, out int iterations, out int iterationsPassed)
        {
            Vector2 dir = vel.SafeNormalize(Vector2.Zero);
            iterations = (int)MathF.Ceiling(vel.Length());
            iterationsPassed = 0;
            Player plr = Main.player[Projectile.owner];
            int width = plr.width;
            int height = plr.height;
            Vector2 topLeft = plr.position;
            Vector2 prevPos = topLeft;
            for (int i = 0; i < iterations; i++)
            {
                Vector2 position = dir * i + topLeft;
                if (Collision.SolidTiles(position, width, height))
                {
                    return dir * (i - 1);// + new Vector2(MathF.Sign(dir.X * (iterations - i)), 0);
                }
                iterationsPassed++;
            }
            return vel;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.frame >= 4 && Projectile.ai[0] == 0) //fist on bottom
            {
                if (player.velocity.Y != 0f) //in air
                {
                    Projectile.ai[0]++; //disable stomping

                    player.velocity.X = player.direction * -5;
                    player.velocity.Y = -8;

                    player.immuneTime = 8;
                    player.immune = true;
                    player.immuneNoBlink = true;

                    SlamSpikes(player);
                }
            }
        }
        /// <summary>
        /// Custom method used for the spikes that shoot out if a slam hits the ground or an NPC. (This was written just to test summaries)
        /// </summary>

        private void SlamSpikes(Player player) //custom method for 
        {
            //spikes
            for (int i = 0; i < 16; i++)
            {
                float rotationalOffset = MathHelper.ToRadians(i * 22.5f); //convert degrees to radians

                //use sine and cosine for circular formation
                float projX = Projectile.Center.X + MathF.Cos(rotationalOffset) * 5; //angle = rotation offset
                float projY = Projectile.Center.Y + MathF.Sin(rotationalOffset) * 5; //multiplier = spread

                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), projX + player.direction * 45, projY + 45, 0, 0,
                    ModContent.ProjectileType<MetalFighterSpike>(), Projectile.damage / 2, 6, Projectile.owner, 0, 0);

                Vector2 direction = Main.projectile[proj].Center - (Projectile.Center + new Vector2(player.direction * 45, 45));
                direction.Normalize();
                direction *= 25;
                Main.projectile[proj].velocity = direction;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity; //independent from light level while still being affected by opacity
        }
    }
}