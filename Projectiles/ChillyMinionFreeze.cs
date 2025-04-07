using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class ChillyMinionFreeze : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 118;
			Projectile.height = 102;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.timeLeft = ChillyMinion.FireRate + 4;
			Projectile.tileCollide = false;
            Projectile.penetrate = 6;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = Projectile.timeLeft + 1; //doesn't wait for global npc cooldown
			Projectile.hide = true; //here so drawbehind works

			//don't need this because of the addition of local iframes
            //Projectile.stopsDealingDamageAfterPenetrateHits = true; //cancels out damage without killing projectile
        }

		public override void AI()
		{
			Projectile chillyOwner = Main.projectile.FirstOrDefault(i => i.identity == Projectile.ai[1]);
			if (chillyOwner == null || chillyOwner == default)
			{
				Projectile.Kill();
				return;
			}
			Projectile.Center = chillyOwner.Center;
			Lighting.AddLight(Projectile.Center, Vector3.One);
            //Animation
            if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
   //     public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
   //     {
			//Projectile.damage = (int)(Projectile.damage * 0.5);//decay damage on hit
			//target.AddBuff(BuffID.Frostburn, 60 * 3);
   //     }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			Main.instance.DrawCacheProjsBehindProjectiles.Add(index);
		}
    }
}