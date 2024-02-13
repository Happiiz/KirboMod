using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BombProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jolly Bomb");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.height = 38;
			DrawOriginOffsetY = -12;
			Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}
		public override void AI()
		{
			Projectile.rotation += Projectile.velocity.X * 0.02f;

            //slow
            if (Projectile.velocity.Y == 0)
            {
                Projectile.velocity.X *= 0.96f;
            }

            //Gravity
            Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
			if (Projectile.velocity.Y >= 10f)
			{
				Projectile.velocity.Y = 10f;
			}


            //explode when in contact with npc
            for (int i = 0; i < Main.maxNPCs; i++) //loop statement that cycles completely every tick
            {
                NPC npc = Main.npc[i]; //any npc

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.active && !npc.friendly) //hitboxes touching
                {
                    Projectile.Kill();
                }
            }

			//player here too incase pvp
            for (int i = 0; i < Main.maxPlayers; i++) //loop statement that cycles completely every tick
            {
                Player player = Main.player[i]; //any player

                //hitboxes touching and player is on opposing team
                if (player.Hitbox.Intersects(Projectile.Hitbox) && player.InOpposingTeam(Main.player[Projectile.owner])) 
                {
                    Projectile.Kill();
                }
            }

            //Step up half tiles
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
		}
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.01f, //no zero else it won't launch right
                ModContent.ProjectileType<Projectiles.BombExplosion>(), Projectile.damage, 12, Projectile.owner);
        }

        public override bool? CanCutTiles()
        {
            return false; //can't cut grass and pots
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't fall through platforms
            return true;
        }
    }
}