using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BadIceChunkMist : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ice Cube");
			Main.projFrames[Projectile.type] = 1;
		}

        public override string Texture => "KirboMod/NothingTexture";

        public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
            int dustnumber = Dust.NewDust(Projectile.position, 50, 10, DustID.Frost, 0f, 0f, 0, default, 2f); //dust
            Main.dust[dustnumber].velocity *= 0.3f;
            Main.dust[dustnumber].noGravity = true;

            //Go down
            Projectile.velocity.Y = 2f;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //don't die 
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			fallThrough = false; //don't pass through platforms

            return true;
        }
    }
}