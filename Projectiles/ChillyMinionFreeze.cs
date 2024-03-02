using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
			Projectile.timeLeft = 12;
			Projectile.tileCollide = false;
            Projectile.penetrate = 6;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 12; //doesn't wait for global npc cooldown
			Projectile.hide = true; //here so drawbehind works
            Projectile.stopsDealingDamageAfterPenetrateHits = true; //cancels out damage without killing projectile
        }

		public override void AI()
		{
			Projectile chillyOwner = Main.projectile[(int)Projectile.ai[1]];
			Projectile.Center = chillyOwner.Center;

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

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			Main.instance.DrawCacheProjsBehindProjectiles.Add(index);
		}
    }
}