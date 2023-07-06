using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MiniWhispy : ModProjectile
	{
		private int animation = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mini Whispy");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 4;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			//Main.projPet[projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			//ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			//ProjectileID.Sets.Homing[projectile.type] = true;

		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 76;
			Projectile.height = 142;
			DrawOriginOffsetY = 2; //touch floor
			// Makes the sentry unable to go through tiles
			Projectile.tileCollide = true;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.friendly = true;
			// Only determines the damage type
			Projectile.sentry = true;
			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			// Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.penetrate = -1;

			Projectile.damage = 0;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			return false;
		}

		public override void AI()
		{
			Projectile.damage = 0;
			Player player = Main.player[Projectile.owner];

			Projectile.velocity.Y += 0.2f;
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}

			Vector2 targetCenter = Projectile.position;
			float distanceFromTarget = 200f;
			bool foundTarget = false;

			//Right click to target an npc
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float distance = Vector2.Distance(npc.Center, Projectile.Center);
				// Reasonable distance away so it doesn't target across multiple screens
				if (distance < 100f)
				{
					distanceFromTarget = distance;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) //search target
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy())
					{
						float distance = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, npc.Center) > distance;
						bool inRange = distance < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = distance < 100f;
						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
						{
							distanceFromTarget = distance;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}

			if (foundTarget) //spew apples
			{
				animation = 1; //shake

				Projectile.ai[0]++;
				if (Projectile.ai[0] % 30 == 0) //every multiple of 30
				{
					Vector2 speed = Main.rand.NextVector2Circular(3.2f, 3f); //circle
				    //add more commas then specified if you want to specify things like x and y
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 50f, speed.X * 3, speed.Y * 3, ModContent.ProjectileType<SmullApple>(), Projectile.damage, 3, player.whoAmI, 0, 0);
					for (int i = 0; i < 5; i++) //spawns 5 grassblades
					{
						Vector2 yoffset = new Vector2(0f, -50); //50 up
						Dust.NewDustPerfect(Projectile.Center + yoffset, DustID.GrassBlades, speed, 0, default, 1);
					}
				}
			}
			else //not found target
            {
				Projectile.ai[0] = 0;
				animation = 0; //idle
            }

			//Animation
			Projectile.frameCounter++;
			if (animation == 0)
			{
				if (Projectile.frameCounter >= 180 & Projectile.frameCounter <= 190)
				{
					Projectile.frame = 1; //blink
				}
				else
				{
					Projectile.frame = 0; //eyes open
				}
				if (Projectile.frameCounter > 190)
				{
					Projectile.frameCounter = 0;
				}
			}
			if (animation == 1)
            {
				if (Projectile.frameCounter <= 10)
				{
					Projectile.frame = 2; //shake left
				}
				else //higher than 10, lower than 20
				{
					Projectile.frame = 3; //shake right
				}
				if (Projectile.frameCounter >= 20)
				{
					Projectile.frameCounter = 2;
				}
			}
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
			fallThrough = false;
			return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return false; //don't die when touching tiles
        }
    }
}