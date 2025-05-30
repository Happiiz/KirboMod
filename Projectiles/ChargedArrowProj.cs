using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ChargedArrowProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Charged Star Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			DrawOriginOffsetX = -9;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 120; //2 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = 3;
			Projectile.scale = 1f;
			Projectile.alpha = 50;
			Projectile.aiStyle = 0;
			Projectile.light = 0.4f;
			Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 1;
		}
        ref float MaxVel => ref Projectile.localAI[0];
        ref float HomingTimer => ref Projectile.localAI[1];
        ref float HomingLockoutTimer => ref Projectile.ai[1];
        ref float HomingTargetIndex => ref Projectile.ai[0];
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
            if(MaxVel == 0)
            {
                MaxVel = Projectile.velocity.Length();
            }
			int dustnumber = Dust.NewDust(Projectile.position, 18, 18, DustID.IcyMerman, 0f, 0f, 0, default, 1f); //dust
			Main.dust[dustnumber].velocity *= 0.2f;
			Main.dust[dustnumber].noGravity = true;
            Main.dust[dustnumber].scale *= 1.2f;
            if (HomingLockoutTimer > 0)
            {
                HomingLockoutTimer--;
                Projectile.velocity.Normalize();
                Projectile.velocity *= MaxVel;
            }
            else
            {
                float prevAi0 = HomingTargetIndex;
                Helper.Homing(Projectile, MaxVel, ref HomingTargetIndex, ref HomingTimer, 0.1f, 100f);
                if (prevAi0 != HomingTargetIndex)// if changed homing target, sync.
                {
                    Projectile.netUpdate = true;
                }
            }
		}

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(Projectile.velocity.Length() / 2, Projectile.velocity.Length() / 2); //circle
                Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch,
                    velocity, Scale: 1f);
            }


            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(Projectile.velocity.Length() / 2, Projectile.velocity.Length() / 2); //circle
                Sparkle.NewSparkle(Projectile.Center, Color.Blue, new Vector2(1, 1f),
                velocity, 40, new Vector2(1f, 1f));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //collision

        }

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual projectile
            {
                Vector2 drawOrigin = texture.Size() / 2;
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(-8f, Projectile.gfxOffY);

                Color color = Color.CornflowerBlue;
                color.A = 128;//make it blend with the background a bit.
                color *= ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }
            
            return Projectile.DrawSelf(Color.White);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HomingLockoutTimer = 10;
            HomingTargetIndex = -1;
            Projectile.netUpdate = true;
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }
        public override void PostDraw(Color lightColor)
        {
            //add glow ball at tip of arrow
            VFX.DrawGlowBallAdditive(Projectile.Center + Projectile.velocity * 0.5f, .8f, Color.Blue * .5f, Color.Transparent, true);
        }
    }
}