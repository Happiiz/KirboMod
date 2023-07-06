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

            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			DrawOffsetX = -40;
			DrawOriginOffsetY = -40;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;
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
				int dustnumber = Dust.NewDust(Projectile.position, 24, 24, DustID.PurpleCrystalShard, 0f, 0f, 200, default, 1.5f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}

			//TARGETING AND HOMING
			if (Projectile.ai[0] >= 5 - (player.maxMinions * 0.5f)) //stats depend on minion slots
			{
				//Targeting
				float distanceFromTarget = 500f + player.maxMinions * 100;
				if (distanceFromTarget > 2000)
				{
					distanceFromTarget = 2000; //cap
                }

				if (player.HasMinionAttackTargetNPC) //Right click targeting
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					float distance = Vector2.Distance(npc.Center, Projectile.Center);
					// Reasonable distance away so it doesn't target across multiple screens
					if (distance < 2000f)
					{
						aggroTarget = npc;
					}
				}

                if (aggroTarget == null || !aggroTarget.active || aggroTarget.dontTakeDamage) //search target
                {
                    //start each number with a very big number so they can't be targeted if their npc doesn't exist
                    Targetdistances = Enumerable.Repeat(999999f, Main.maxNPCs).ToList();

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        float distance = Vector2.Distance(Projectile.Center, npc.Center);

                        if (npc.CanBeChasedBy()) //checks if targetable
                        {
                            Vector2 positionOffset = new Vector2(0, -5);
                            bool inView = Collision.CanHitLine(Projectile.position + positionOffset, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

                            //close, hittable, hostile and can see target
                            if (inView && !npc.friendly && !npc.dontTakeDamage && !npc.dontCountMe && distance < distanceFromTarget && npc.active)
                            {
                                Targetdistances.Insert(npc.whoAmI, (int)distance); //add to list of potential targets
                            }
                        }

                        if (i == Main.maxNPCs - 1)
                        {
                            int theTarget = -1;

                            //count up 'til reached maximum distance
                            for (float j = 0; j < distanceFromTarget; j++)
                            {
                                int Aha = Targetdistances.FindIndex(a => a == j); //count up 'til a target is found in that range

                                if (Aha > -1) //found target
                                {
                                    theTarget = Aha;

                                    break;
                                }
                            }

                            if (theTarget > -1) //exists
                            {
                                NPC npc2 = Main.npc[theTarget];

                                if (npc2 != null) //exists
                                {
                                    aggroTarget = npc2;
                                }
                            }
                            else
                            {
                                break; //just in case
                            }
                        }
                    }
                }
                else if (aggroTarget != null && aggroTarget.active && !aggroTarget.dontTakeDamage) //ATTACK
                {
                    Vector2 direction = aggroTarget.Center - Projectile.Center; //start - end
					float speed = 25f + player.maxMinions * 2.5f; //stats depend on minion slots
					float inertia = 10f;

                    direction.Normalize();
					direction *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;  //fly towards enemy
				}
				else //fly straight
				{
					Projectile.velocity = Projectile.velocity; //keep going the way it's going
				}
			}
        }

        public override void Kill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(10f, 10f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleCrystalShard, speed, Scale: 1.5f, Alpha: 200); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}