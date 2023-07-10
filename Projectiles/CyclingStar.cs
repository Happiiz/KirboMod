using KirboMod.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class CyclingStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star");
			Main.projFrames[Projectile.type] = 1;

            //for drawing afterimages and stuff alike
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 2;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, 0.255f, 0.255f, 0f);

			Projectile.rotation = player.GetModPlayer<KirbPlayer>().tripleStarRotationCounter * 0.25f - Projectile.whoAmI; // rotates projectile (final result if 0.3, same as triple star

            if (player.GetModPlayer<KirbPlayer>().HasTripleStars[(int)Projectile.ai[1]] == true &&
                player.HeldItem.type == ModContent.ItemType<TripleStar>()) //ai 1 now being i in Projectile summoning code in KirbPlayer
            {
                Projectile.timeLeft = 2; //suspend death
            }

            Projectile.ai[0] = player.GetModPlayer<KirbPlayer>().tripleStarRotationCounter * 0.1f; //grow

            float rotationalOffset = Projectile.ai[0] + Projectile.ai[1] * MathF.Tau / 3; //offset so three stars are evenly spaced out

            //set a point(circleCenter) and then make the projectile spiral in a growing circle around that (starting at the center)
            Projectile.Center = player.MountedCenter + (rotationalOffset).ToRotationVector2() * 40;
        }

        public override bool? CanCutTiles()
        {
            return false; //no destroy grass or pots
        }

        public override Color? GetAlpha(Color lightColor)
        {
			return Color.White; // Makes it uneffected by light
        }
    }
}