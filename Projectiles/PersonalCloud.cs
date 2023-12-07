using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PersonalCloud : ModProjectile
	{
		private int animation = 1;

        private List<float> Targetdistances = new List<float>(); //targeting
        private NPC aggroTarget = null; //target the minion is currently focused on

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 42;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.minion = true; //deals minion damage
		}

        public override bool? CanCutTiles() //can cut foliage?
        {
            return false;
        }
        public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.ai[0]++;

			Projectile.Center = player.Center + new Vector2(0, -50); //stay above player

			//equipping accesory
			if (player.GetModPlayer<KirbPlayer>().personalcloud == true)
            {
				Projectile.timeLeft = 2; //keep being on the brink of death until accesory is no longer equipped
			}

			//TARGETING
			float distanceFromTarget = 800f;

            if (aggroTarget == null || !aggroTarget.active || aggroTarget.dontTakeDamage) //search target
            {
                animation = 1;
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
                animation = 2;

				if (Projectile.ai[0] >= 60) //shoot
                {
					Vector2 distance = aggroTarget.Center - Projectile.Center;
					distance.Normalize();
					distance *= 10;
					int damage = 30;
                    if (Main.masterMode)
                    {
						damage = ((int)(damage * 1.5f));
                    }
					if(ModLoader.TryGetMod("CalamityMod", out _))
                    {
						damage = (int)(damage * 2f);
                    }
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, distance, ModContent.ProjectileType<PersonalCloudBeam>(), 30, 4, Projectile.owner);

					Projectile.ai[0] = 0;
				}
			}

			//animation
			Projectile.frameCounter++; 

			if (animation == 1)
			{
				if (Projectile.frameCounter < 30)
				{
					Projectile.frame = 0;
				}
				else if (Projectile.frameCounter < 60)
				{
					Projectile.frame = 1;
				}
				else
				{
					Projectile.frameCounter = 0; 
				}
			}
			if (animation == 2)
			{
				if (Projectile.frameCounter < 30)
				{
					Projectile.frame = 2;
				}
				else if (Projectile.frameCounter < 60)
				{
					Projectile.frame = 3;
				}
				else
				{
					Projectile.frameCounter = 0;
				}
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}