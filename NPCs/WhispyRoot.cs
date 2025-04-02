using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	public class WhispyRoot : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[NPC.type] = 1;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

		public override void SetDefaults()
		{
			NPC.width = 60;
			NPC.height = 150;
			NPC.life = 80; 
			NPC.lifeMax = 80; 
			NPC.defense = 10;
			NPC.knockBackResist = 0.00f; //recieves 0% of knockback
			NPC.friendly = false;
			NPC.dontTakeDamage = true;
			NPC.timeLeft = 120;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.hide = true; //for drawing behind stuff
			NPC.damage = 0; //won't last
			NPC.HitSound = SoundID.Dig;
			NPC.DeathSound = SoundID.Dig;
            NPC.dontCountMe = true; //Don't count towards npc total
        }

		private int TileDetection(int missedTiles)
		{
			if (NPC.IsABestiaryIconDummy)
				return 0;
            missedTiles = 0;

            int max = NPC.width / 16;

            for (int i = 0; i < max; i++) //tile width
            {
                Point tileposition = NPC.position.ToTileCoordinates() + new Point(i, (int)(NPC.position.Y / 16));

                Tile tile = Main.tile[tileposition];

                if (!tile.HasTile) //no tile on top
                {
                    missedTiles++;

                    continue;
                }
            }

			return missedTiles;
        }

		public override void AI()
		{
			NPC.spriteDirection = (int)NPC.ai[1]; //ai 1 is equal to whispy direction

			if (NPC.ai[0] == 59) //tile checking
            {
				int missedTiles = 0;
                missedTiles = TileDetection(missedTiles);

                if (missedTiles >= 3) //break if too many missing tiles
				{
					//bark
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, -10, Mod.Find<ModProjectile>("WhispyBark").Type, 20 / 2, 5, Main.myPlayer, 0, 0);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, -1, -10, Mod.Find<ModProjectile>("WhispyBark").Type, 20 / 2, 5, Main.myPlayer, 0, 0);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 1, -10, Mod.Find<ModProjectile>("WhispyBark").Type, 20 / 2, 5, Main.myPlayer, 0, 0);
                    }

                    NPC.life = 0;
                }

            }

			NPC.ai[0]++;

			if (NPC.ai[0] < 60)
            {
				Dust.NewDust(NPC.position, NPC.width / 2, NPC.height, DustID.Dirt, 0, -3, 0, default, 1f); //warning
				if (NPC.ai[0] % 5 == 0) //multiple of 5
				{
					SoundEngine.PlaySound(SoundID.WormDig.WithVolumeScale(1.2f), NPC.Center); //worm dig warning(style is 1 so not roar)
				}
			}

			if (NPC.ai[0] == 60) 
			{
				SoundEngine.PlaySound(SoundID.Dig, NPC.position); //dig
				NPC.dontTakeDamage = false;
				NPC.damage = 40; //deal damage(high because expert mode exclusive)
			}

			//go up
			if (NPC.ai[0] >= 60 && NPC.ai[0] <= 70) //10 ticks
			{
				NPC.velocity.Y = -10; //go up
			}
			else
            {
				NPC.velocity.Y = 0;
            }

			if (NPC.ai[0] >= 780 || !NPC.AnyNPCs(Mod.Find<ModNPC>("Whispy").Type)) //stay for 12 more seconds after coming up(kill if whispy gone)
            {
				NPC.life = 0; //kill
				for (int i = 0; i < 25; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Dirt, 0, 0, 0, default, 1f);
				}
			}
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
			if (NPC.life <= 0)
			{
				for (int i = 0; i < 25; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Dirt, 0, 0, 0, default, 1f);
				}
			}
			else //less if above death
			{
				for (int i = 0; i < 3; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Dirt, 0, 0, 0, default, 1f);
				}
			}
		}

        public override void DrawBehind(int index)
        {
			Point tileposition = NPC.position.ToTileCoordinates();

			if (WorldGen.SolidTile(tileposition.X, tileposition.Y) == false & NPC.ai[0] < 59) //check if no tile in npc position(top left corner of npc)
			{
				Main.instance.DrawCacheNPCsOverPlayers.Add(index); //draw over stuff
			}
			else
			{
				Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index); //draw under stuff
			}
		}

        public override Color? GetAlpha(Color drawColor)
        {
            int missedTiles = 0;
            missedTiles = TileDetection(missedTiles);

            if (missedTiles >= 3 && NPC.ai[0] < 60) //too many missing tiles
            {
                return Color.Red; //will explode
            }
			else
            {
                return null; //will rise
            }
        }
    }
}