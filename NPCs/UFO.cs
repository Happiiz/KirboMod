using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	public class UFO : ModNPC
	{
        public ref float Movement => ref NPC.ai[2];
        ref float MovementTimer => ref NPC.ai[0];
        ref float AttackTimer => ref NPC.ai[1];
        private bool seen = false; //determines if the ufo was in range of the player's sight

		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("UFO");
			Main.npcFrameCount[NPC.type] = 5;
		}

		public override void SetDefaults() {
			NPC.width = 46;
			NPC.height = 44;
			//drawOffsetY = -18; //make sprite line up with hitbox
			NPC.damage = 90;
			NPC.lifeMax = 4000;
			NPC.defense = 30;
			NPC.HitSound = SoundID.NPCHit4; //metal
			NPC.DeathSound = SoundID.NPCDeath14; //mech explode
			NPC.value = Item.buyPrice(5, 0, 0, 0); // money it drops
			NPC.rarity = 4; //1 is dungeon slime, 4 is mimic
			NPC.knockBackResist = 0; //How much of the knockback it receives will actually apply
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.UFOBanner>();
			NPC.noGravity = true;
			NPC.noTileCollide = true;
            NPC.aiStyle = -1;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Rarely some may see mysterious constructs floating around the atmosphere. They tend to open fire on suspicious life forms that get too close.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.Player.ZoneSkyHeight && Main.hardMode) //if player is within space height and world is in hardmode
			{	
				return 0.025f; //return spawn rate
			}
			else
			{
				return 0f; //no spawn rate
			}
		}

		public override void AI() //constantly cycles each time
        {
            Lighting.AddLight(NPC.Center, Vector3.One);
            int attackRate = Main.expertMode ? 90 : 120;
            NPC.spriteDirection = NPC.direction;
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            Vector2 distance = player.Center - NPC.Center;
            //this is here so players don't get shot from where they can't see
            //within dimensions, not in unaccessible area and player not dead
            if (MathF.Abs(distance.Y) < 400 && MathF.Abs(distance.X) < 800 && !player.dead && player.active)
                seen = true;//prepare attack
            if (seen) //prepare attack
            {
                AttackTimer++; //prepare attack
            }
            else
            {
                AttackTimer = 0; //float around 
            }

            if (AttackTimer >= attackRate / 2) //warning
            {
                //Makes dust slightly above UFO
                Dust d = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -12), ModContent.DustType<Dusts.CyborgArcherLaser>(), Vector2.Zero, Scale: 1f);
                d.noGravity = true;
            }

            CheckAttack(attackRate, player);

            //how often it should switch directions
            MovementTimer++;
            if (MovementTimer >= attackRate)
            {
                MovementTimer = 0f;
            }
            else
            {
                return;
            }
            float movementSpeed = 3;
            if (Main.expertMode)
                movementSpeed *= 1.3333f;
            bool moveVertically = MathF.Abs(NPC.Center.Y - player.Center.Y) > MathF.Abs(NPC.Center.X - player.Center.X);
            if (distance.Length() < 600)
                moveVertically = Main.rand.NextBool();
            NPC.velocity = Vector2.Zero;
            if (moveVertically)
            {
                NPC.velocity.Y = MathF.Sign(player.Center.Y - NPC.Center.Y);
            }
            else
            {
                NPC.velocity.X = MathF.Sign(player.Center.X - NPC.Center.X);
            }
            NPC.netUpdate = true;
            NPC.velocity *= movementSpeed;
        }

        void CheckAttack(int attackRate, Player player)
        {
            if (AttackTimer >= attackRate) //shoot
            {
                float projVelocity = Main.expertMode ? 20 : 13;
                Vector2 projshoot = player.Center - NPC.Center;
                projshoot.Normalize(); //make into 1
                projshoot *= projVelocity;//projectile velocity.
                ShootProj(projshoot);
                SoundEngine.PlaySound(SoundID.Item158 with { Pitch = .7f }, NPC.Center);
                //shoot an additional projectile
                if (Main.expertMode && player.velocity.LengthSquared() > 0)
                {
                    //predicts player movement
                    Utils.ChaseResults result = Utils.GetChaseResults(NPC.Center, projVelocity, player.Center, player.velocity);
                    result.ChaserVelocity = result.InterceptionHappens ? result.ChaserVelocity : (Vector2.Normalize(player.velocity) * projVelocity);
                    ShootProj(result.ChaserVelocity);
                }
                //reset
                AttackTimer = 0;
            }
        }

        private void ShootProj(Vector2 velocity)
        {
            float projMaxUpdates = ContentSamples.ProjectilesByType[ModContent.ProjectileType<Projectiles.UFOLaser>()].MaxUpdates;
            Particles.Ring.ShotRing(NPC.Center, Color.Red, velocity);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity / projMaxUpdates, ModContent.ProjectileType<Projectiles.UFOLaser>(), 80 / 2, 0);
            }
        }

        public override void FindFrame(int frameHeight) // animation
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 10.0)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 20.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 30.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < 40.0)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else
            {
                NPC.frameCounter = 0.0;
            }
        }
        public override bool PreKill()
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.TheDestroyer).noGravity = true;
            }
            return true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (NPC.life <= 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle edge
                        Gore.NewGorePerfect(NPC.GetSource_FromAI(), NPC.Center, speed, Main.rand.Next(16, 18));
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                        Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, speed, Main.rand.Next(11, 13), Scale: 1f); //double jump smoke
                    }
                }
            }
        }

		/*public override void NPCLoot()
		{
			Item.NewItem(npc.getRect(), ModContent.ItemType<Items.DreamEssence>(), Main.rand.Next(2, 4));

			if (Main.expertMode)
			{
				if (Main.rand.NextBool(2))
				{
					Item.NewItem(npc.getRect(), ItemID.SoulofFlight, 1);
				}
			}
			else
			{
				if (Main.rand.NextBool(4))
				{
					Item.NewItem(npc.getRect(), ItemID.SoulofFlight, 1);
				}
			}
		}*/
	}
}
