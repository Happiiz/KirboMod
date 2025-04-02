using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class DreamedFriend : ModProjectile
    {
        private List<float> Targetdistances = new List<float>(); //targeting
        private NPC aggroTarget = null; //target the minion is currently focused on
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;

            //Cultist takes 75% damage from homing projectiles
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Type] = true;
        }
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			DrawOffsetX = -40;
			DrawOriginOffsetY = -40;
			Projectile.friendly = true;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 3;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Player player = Main.player[Projectile.owner];
            Projectile.spriteDirection = Projectile.direction;

            if (Projectile.direction == -1) //facing left
            {
                Projectile.rotation += MathHelper.ToRadians(180); //rotate by 180 degrees after turning to velocity rotation to make upright
            }

			Projectile.ai[0]++;

			if (Projectile.ai[0] == 1)
            {
				Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]); //choose random character
			}

			if (Main.rand.NextBool(2)) // happens 1/2 times
			{
				int d = Dust.NewDust(Projectile.position, 24, 24, DustID.PurpleCrystalShard, 0f, 0f, 200, default, 1.5f); //dust
				Main.dust[d].velocity *= 0.3f;
				Main.dust[d].noGravity = true;

                int d2 = Dust.NewDust(Projectile.position, 24, 24, DustID.BlueCrystalShard, 0f, 0f, 200, default, 1.5f); //dust
                Main.dust[d2].velocity *= 0.3f;
                Main.dust[d2].noGravity = true;
            }
            Helper.Homing(Projectile, 30, ref Projectile.ai[1], ref Projectile.localAI[0], 0.2f, 2000);
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(15f, 15f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleCrystalShard, velocity, Scale: 2f, Alpha: 200); //Makes dust in a messy circle
                d.noGravity = true;

                Vector2 velocity2 = Main.rand.NextVector2Circular(15f, 15f); //circle
                Dust d2 = Dust.NewDustPerfect(Projectile.Center, DustID.BlueCrystalShard, velocity2, Scale: 2f, Alpha: 200); //Makes dust in a messy circle
                d2.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            VFX.DrawGlowBallAdditive(Projectile.Center, 1.4f, Color.DeepSkyBlue, Color.MediumSlateBlue);
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
    }
}