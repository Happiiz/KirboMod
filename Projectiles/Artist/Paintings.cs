using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Artist
{
	public class Paintings : ModProjectile
    {
        List<Color> availableColors = [Color.Red, Color.Orange, Color.Yellow, Color.DeepPink, Color.Lime, Color.Blue, Color.BlueViolet];

        int character = 0;
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 8;

            //Cultist takes 75% damage from homing projectiles
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
		public override void SetDefaults()
		{
			Projectile.width = 56;
			Projectile.height = 48;
			Projectile.friendly = false;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.ai[0]++;

			if (Projectile.ai[0] == 1)
            {
				character = Main.rand.Next(4); //choose random character
			}

            if (Projectile.frameCounter++ < 30)
            {
                if (Projectile.frameCounter < 15)
                {
                    Projectile.frame = character > 0 ? character * 2 : character; //if above zero then multiply to adjust
                }
                else
                {
                    Projectile.frame = (character > 0 ? character * 2 : character) + 1;
                }
            }
            else
            {
                Projectile.frameCounter = 0;
            }

            if (Projectile.ai[0] >= 60)
            {
                Projectile.friendly = true;
                Helper.Homing(Projectile, 30, ref Projectile.ai[1], ref Projectile.localAI[0], 0.5f, 2000);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
        }

        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(6f, 6f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Marble, speed,
                    Scale: 1f, newColor: availableColors[Main.rand.Next(availableColors.Count)]); //Makes dust in a messy circle
            }
        }
    }
}