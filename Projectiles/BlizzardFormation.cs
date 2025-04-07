using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BlizzardFormation : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			DrawOffsetX = -30;
			DrawOriginOffsetY = -30;
			Projectile.friendly = true;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				SoundEngine.PlaySound(SoundID.Item46, Projectile.position); //ice hydra

				for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
					Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, Scale: 1f); //Makes dust in a messy circle
					d.noGravity = true;
				}
			}
			float radius = 40f;
			float radiusSQ = radius * radius;
			int type = Type;
			int whoAmI = Projectile.whoAmI;
			float pushMult = 0.5f;
			Vector2 pos = Projectile.position;//use position and not center because they're all the same difference, as it's the same proj type
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if(i == whoAmI)
				{
					continue;
				}
				Projectile p = Main.projectile[i];
				if (!p.active || p.type != type)
				{
					continue;
				}
				Vector2 deltaPos = (p.position - pos);
				float distSQ = deltaPos.LengthSquared();
				if(distSQ < radiusSQ)
				{
					Vector2 push = deltaPos.Normalized(MathF.Sqrt(distSQ / 2f) * pushMult);
					Vector2 nextPos = Projectile.position - push;
					Tile tile = Main.tile[nextPos.ToTileCoordinates()];
					int tileID = tile.TileType;
					bool inTiles = true;
                    if (!tile.HasUnactuatedTile && (Main.tileSolid[tileID] && !Main.tileSolidTop[tileID]))
					{
						Projectile.position = nextPos;
                        pos = Projectile.position;
                        inTiles = false;
                    }
					//double if the other was in tiles
					if (inTiles)
					{
						push *= 2;
					}
					inTiles = true;
                    nextPos = p.position + push;
                    tile = Main.tile[nextPos.ToTileCoordinates()];
                    tileID = tile.TileType;
                    if (!tile.HasUnactuatedTile && (Main.tileSolid[tileID] && !Main.tileSolidTop[tileID]))
                    {
                        p.position = nextPos;
						inTiles = false;
                    }
					if (inTiles) //push is already doubled by now so don't double it again
                    {
						Projectile.position -= push;
						pos -= push;
					}
				}
			}
			Projectile.velocity *= 0.96f;
		}

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 15; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, default, Scale: 1f); //Makes dust in a messy circle
				d.noGravity = true;
			}
		}

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //make hitbox into a centered rectangle
        {
            Rectangle hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(70, 70));

            return hitbox.Intersects(targetHitbox);
        }

        public override bool? CanHitNPC(NPC target) //can hit only if there's a line of sight
        {
            if (Collision.CanHit(Projectile, target))
            {
                return null;
            }
            return false;
        }
        public override bool CanHitPvp(Player target) //can hit only if there's a line of sight
        {
            return Collision.CanHit(Projectile, target);
        }
    }
}