using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BioSparkSlashHitbox : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 20;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			NPC originNPC = Main.npc[(int)Projectile.ai[0]];

			if (originNPC.type == ModContent.NPCType<BioSpark>())
			{
				Projectile.Center = originNPC.Center + new Vector2(originNPC.direction * 20, 2);
			}
            else if (originNPC.type == ModContent.NPCType<BladeKnight>())
            {
                Projectile.Center = originNPC.Center + new Vector2(originNPC.direction * 30, 2);
            }

        }
    }
}