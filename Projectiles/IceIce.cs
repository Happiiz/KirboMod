using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class IceIce : ModProjectile
	{
        readonly int style = Main.rand.Next(1, 4);

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 20;
			Projectile.tileCollide = true;
			Projectile.penetrate = 2;
			Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
		public override void AI()
		{
            //scale with timeLeft
            Projectile.scale = 1f + Utils.GetLerpValue(20, 0, Projectile.timeLeft, true);
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
            Projectile.damage = (int)(Projectile.damage * .8f);
            target.AddBuff(BuffID.Frostburn, 60);
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
            Texture2D iceMist = ModContent.Request<Texture2D>("KirboMod/Projectiles/IceMist/IceMist" + style).Value;

            Color color = Color.LightCyan * Utils.GetLerpValue(0, 5, Projectile.timeLeft, true);

            float scale = Utils.GetLerpValue(20, 15, Projectile.timeLeft, true);

            Main.EntitySpriteDraw(iceMist, Projectile.Center - Main.screenPosition, null, color, 0, iceMist.Size() / 2, scale * 0.25f, SpriteEffects.None);

            color = Color.White * Utils.GetLerpValue(0, 5, Projectile.timeLeft, true);

            Main.EntitySpriteDraw(iceMist, Projectile.Center - Main.screenPosition, null, color, 0, iceMist.Size() / 2, scale * 0.125f, SpriteEffects.None);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float scaler = Utils.GetLerpValue(20, 15, Projectile.timeLeft, true);

            return Utils.CenteredRectangle(Projectile.Center, new Vector2(100 * scaler)).Intersects(targetHitbox);
        }
    }
}