using KirboMod.Bestiary;
using KirboMod.Dusts;
using KirboMod.Items.DarkMatter;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
	[AutoloadBossHead]
	public partial class PureDarkMatter : ModNPC
	{
		private int phase = 1;

        private int dashesleft = 0;

        private DarkMatterAttackType attacktype = DarkMatterAttackType.Petals;

        private DarkMatterAttackType lastattacktype = DarkMatterAttackType.Dash;

        public override void AI() //constantly cycles each time
		{
			Player playerstate = Main.player[NPC.target];

			//cap life
			if (NPC.life >= NPC.lifeMax)
			{
				NPC.life = NPC.lifeMax;
			}

			//DESPAWNING
			if (NPC.target < 0 || NPC.target == 255 || playerstate.dead || !playerstate.active)
			{
				NPC.TargetClosest(false);

				NPC.velocity.Y = NPC.velocity.Y - 0.2f;

                NPC.ai[0] = 0;

                if (NPC.timeLeft > 60)
				{
					NPC.timeLeft = 60;
					return;
				}
			}
			else //regular attack
            {
				AttackPattern();
			}
		}
		private void AttackPattern()
		{
			Player player = Main.player[NPC.target];
			Vector2 moveTo = player.Center; 
			Vector2 playerDistance = player.Center - NPC.Center;
			Vector2 move = player.Center - NPC.Center;

            NPC.ai[0]++;

            NPC.spriteDirection = NPC.direction;

            if (NPC.ai[0] < 30) //rise up gang
            {
                NPC.velocity.Y = -3;

				if (NPC.ai[0] == 29) //last rise up frame
				{
                    for (int i = 0; i < 20; i++) 
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(10, 10); //circle edge
                        Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.DarkResidue>(), speed); //Makes dust in a messy circle
                        d.noGravity = true;
                    }
                }
            }
			else //main loop
			{
				if (NPC.ai[0] == 30)
				{
                    //AttackDecideNext();
                    attacktype = DarkMatterAttackType.Dash;

                    //Make attacks slightly harder
                    if (NPC.GetLifePercent() <= 0.5f || Main.expertMode)
                    {
                        phase = 2;
                    }

                    //Enrage
                    if (NPC.GetLifePercent() < 0.5f && Main.expertMode)
                    {
                        phase = 3;
                    }
                }

                if (attacktype == DarkMatterAttackType.Petals)
                {
                    if (phase == 3)
                    {
                        EnragePetals();
                    }
                    else
                    {
                        AttackPetals();
                    }
                }
                if (attacktype == DarkMatterAttackType.Dash)
                {
                    if (phase == 3)
                    {
                        EnrageDash();
                    }
                    else
                    {
                        AttackDash();
                    }
                }
                if (attacktype == DarkMatterAttackType.Lasers)
                {
                    if (phase == 3)
                    {
                        EnrageLasers();
                    }
                    else
                    {
                        AttackLasers();
                    }
                }
            }
        }

        void AttackDecideNext()
        {
            List<DarkMatterAttackType> possibleAttacks = new() { DarkMatterAttackType.Petals, DarkMatterAttackType.Dash, DarkMatterAttackType.Lasers };

            possibleAttacks.Remove(lastattacktype);
            possibleAttacks.TrimExcess();
            attacktype = possibleAttacks[Main.rand.Next(possibleAttacks.Count)];
            lastattacktype = attacktype;
            NPC.netUpdate = true;
        }

        void AttackPetals()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;
            NPC.TargetClosest();

            //deciding which side
            float xOffset = 400;

            if (playerDistance.X <= 0) //if player is behind enemy
            {
                xOffset = 400; // go in front of player 
            }
            else
            {
                xOffset = -400; // go behind player
            }

            //movement
            Vector2 playerXOffest = player.Center + new Vector2(xOffset, 0f); //go ahead of player
            Vector2 move = playerXOffest - NPC.Center;


            float speed = 20f;
            float inertia = 10f;

            move.Normalize();
            move *= speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;

            //main attack
            if (NPC.ai[0] % 30 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.direction * -10, Main.rand.Next(-10, 10)),
                    ModContent.ProjectileType<Projectiles.MatterOrb>(), 60 / 2, 4, default, 0, player.whoAmI);
            }

            //reset
            if (NPC.ai[0] > 270) 
            {
                NPC.ai[0] = 30;
            }
        }

        void AttackDash()
        {
            Player player = Main.player[NPC.target];
            Vector2 playerDistance = player.Center - NPC.Center;

            if (NPC.ai[0] < 150) //follow predicted player y for 120 ticks
            {
                NPC.TargetClosest(); //face player only for 60 ticks

                if (NPC.ai[0] % 10 == 0) //little dust to warn player
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
                }

                //deciding which side
                float xOffset = 400;

                if (playerDistance.X <= 0) //if player is behind enemy
                {
                    xOffset = 300; // go in front of player 
                }
                else
                {
                    xOffset = -300; // go behind player
                }
                //movement
                Vector2 playerXOffest = player.Center + new Vector2(xOffset + ((NPC.ai[0] - 30) * -NPC.direction * 5), player.velocity.Y * 20); //go ahead of player and backup a bit
                Vector2 move = playerXOffest - NPC.Center;

                float speed = 60f;
                float inertia = 10f;

                move.Normalize();
                move *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + move) / inertia;
            }
            else if (NPC.ai[0] < 180) //go forth
            {
                NPC.velocity = new Vector2(NPC.velocity.X + (2.5f * NPC.direction), 0f);

                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkResidue>());
            }
            else //slow
            {
                NPC.velocity.X *= 0.9f;
            }
            
            if (NPC.ai[0] >= 210)
            {
                if (phase != 1) //restart if high enough (and not expert mode)
                {
                    NPC.ai[0] = 0; //restart
                }
            }
            //continue if conditions are met


        }

        void AttackLasers()
        {

        }

        //ENRAGED

        void EnragePetals()
        {

        }

        void EnrageDash()
        {

        }

        void EnrageLasers()
        {

        }

        public override void FindFrame(int frameHeight) // animation
        {
			NPC.frameCounter += 1.0;
			if (NPC.frameCounter < 8.0)
			{
				NPC.frame.Y = 0; //idle
			}
			else if (NPC.frameCounter < 16.0)
			{
				NPC.frame.Y = frameHeight; 
			}
            else if (NPC.frameCounter < 24.0)
            {
                NPC.frame.Y = frameHeight * 2; 
            }
            else if (NPC.frameCounter < 32.0)
            {
                NPC.frame.Y = frameHeight * 3; 
            }
            else
			{
				NPC.frameCounter = 0.0; //reset
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
            return Color.White; //make it unaffected by light
        }
    }

}
