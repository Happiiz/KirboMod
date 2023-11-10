using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ChainBombProj : ModProjectile
	{
        ref float Power => ref Projectile.ai[0];
        private List<int> Connected = new List<int>(); //track how much bombs are connected in a singular web
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Chain bomb");
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 34;
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

            Connected.Clear(); //reset

            Power = Connected.Count; //power equals the length of Connected


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

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.friendly == false && npc.active == true) //hitboxes touching
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
        }
         public override void OnKill(int timeLeft) //when the projectile dies
         {
            //damage is 10% of projectile damage times amount of chains made
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.01f, //no zero else it won't launch right
                ModContent.ProjectileType<Projectiles.ChainBombExplosion>(), Projectile.damage + (int)((Projectile.damage * 0.2) * Power), 12, Projectile.owner, 0, Power);
         }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false; //can't cut grass and pots
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Power == 1)
            {
                return Color.Red;
            }
            else if (Power == 2)
            {
                return Color.Yellow;
            }
            else if (Power == 3)
            {
                return Color.Lime;
            }
            else if (Power == 4)
            {
                return Color.Blue;
            }
            else if (Power > 4)
            {
                return Color.Cyan;
            }
            else //anything else
            {
                return Color.GreenYellow;
            }
        }

        public static Asset<Texture2D> ChainBombChain;

        public override bool PreDraw(ref Color lightColor)
        {
            ChainBombChain = ModContent.Request<Texture2D>("KirboMod/Projectiles/ChainBombChain");

            
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            //Vector2 center = Projectile.Center;

            Power = 0; //reset power

            for (int i = 0; i <= Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (Vector2.Distance(Projectile.Center, proj.Center) < 200 && proj.type == ModContent.ProjectileType<ChainBombProj>() 
                    && Projectile.whoAmI != proj.whoAmI && proj.active)
                {
                    Power += 1; //add 1 to power

                    Vector2 directionToBomb = Projectile.Center - proj.Center;
                    Vector2 center = proj.Center;
                    float projRotation = directionToBomb.ToRotation() - MathHelper.PiOver2;
                    float distance = directionToBomb.Length();
                    while (distance > 15f && !float.IsNaN(distance)) //draw only while 15 units away from source bomb (and an actual number I guess)
                    {
                        directionToBomb.Normalize();                   //get unit vector
                        directionToBomb *= ChainBombChain.Height();     // multiply by chain link length
                        center += directionToBomb;                   //update draw position
                        directionToBomb = Projectile.Center - center;    //update distance
                        distance = directionToBomb.Length();
                        Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                        //Draw chain
                        Main.EntitySpriteDraw(ChainBombChain.Value, center - Main.screenPosition,
                            ChainBombChain.Value.Bounds, Color.White, projRotation,
                            ChainBombChain.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                    }
                }
            }

            return true;
        }

        public static Asset<Texture2D> ChainBombKnobs; //Drawing knobs seperately from glowing body

        public static Asset<Texture2D> ChainBombDefaultBody; //Body it uses when not charged

        public override void PostDraw(Color lightColor)
        {
            ChainBombKnobs = ModContent.Request<Texture2D>("KirboMod/Projectiles/ChainBombKnobs");

            ChainBombDefaultBody = ModContent.Request<Texture2D>("KirboMod/Projectiles/ChainBombDefault");

            if (Power < 1)
            {
                Main.EntitySpriteDraw(ChainBombDefaultBody.Value, Projectile.Center - Main.screenPosition,
                            ChainBombDefaultBody.Value.Bounds, Color.White, Projectile.rotation,
                            ChainBombDefaultBody.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            //offset origin position a smidge cause it's bigger than projectile
            Main.EntitySpriteDraw(ChainBombKnobs.Value, Projectile.Center - Main.screenPosition,
                            ChainBombKnobs.Value.Bounds, Color.White, Projectile.rotation,
                            ChainBombKnobs.Size() * 0.5f, 1f, SpriteEffects.None, 0);
        }
    }
}