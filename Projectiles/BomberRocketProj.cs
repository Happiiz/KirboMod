using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BomberRocketProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{

        }

        bool detonated = false;

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			DrawOffsetX = -8;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 1200; //20 seconds
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.aiStyle = 0;
			Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.damage = 0; //hold off damage until hit

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i]; //any npc

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.active && !npc.friendly && !npc.dontTakeDamage && !detonated) //hitboxes touching
                {
                    detonated = true;
                    Explode();
                }
            }

            //player here too incase pvp
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i]; //any player

                //hitboxes touching and player is on opposing team
                if (player.Hitbox.Intersects(Projectile.Hitbox) && player.InOpposingTeam(Main.player[Projectile.owner]) && !detonated)
                {
                    detonated = true;
                    Explode();
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!detonated)
            {
                Explode();
            }
            return true; //collision
        }

		void Explode() //used from regular Bomber code
		{
            SoundEngine.PlaySound(SoundID.Item38 with { MaxInstances = 0 }, Projectile.Center);

            float sizeMult = 10;
            Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, Projectile.Size * sizeMult);

            Projectile.Kill();

            for (int j = 0; j < 200; j++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                Vector2 speed = (pos - Projectile.Center) * Main.rand.NextFloat(0.02f, 0.1f); //spread outward

                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), pos, speed, Main.rand.Next(61, 64), Scale: 1f); //bomb smoke

                //reroll
                pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                speed = (pos - Projectile.Center) * 0.02f;

                Dust d = Dust.NewDustPerfect(pos, DustID.Torch, speed, Scale: 3);

                Dust d2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: 2);
            }

            Player player = Main.player[Projectile.owner];

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.active && npc.Hitbox.Intersects(Projectile.Hitbox) && !npc.friendly && !npc.dontTakeDamage)
                {
                    bool crit = Main.rand.Next(100) < Projectile.CritChance;
                    int damage = (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo(Projectile.originalDamage);

                    npc.SimpleStrikeNPC(damage, MathF.Sign(npc.Center.X - Projectile.Center.X), 
                        crit, Projectile.knockBack, Projectile.DamageType, true);
                }
            }
        }
	}
}