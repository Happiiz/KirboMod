using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MetaKnightSwing : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 5;
        }

		public override void SetDefaults()
		{
			Projectile.width = 272;
			Projectile.height = 176;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.localNPCHitCooldown = 10; //time before hit again
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true; //check if owner has line of sight to hit
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.spriteDirection = Projectile.direction; 

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - new Vector2(0, 66f);

            //death
            if (player.itemTime == 1 || player.active == false || player.dead == true) //done/can't attack
            {
                Projectile.Kill(); //kill projectile
            }

            //animation
            Projectile.frameCounter++; //slash
            if (Projectile.frameCounter < 2)
            {
                Projectile.frame = 0; //start
            }
            else if (Projectile.frameCounter < 4)
            {
                Projectile.frame = 1; //half slash
            }
            else if (Projectile.frameCounter < 6)
            {
                Projectile.frame = 2; //full slash
            }
            else if (Projectile.frameCounter < 8)
            {
                Projectile.frame = 3; //half and quarter slash
            }
            else 
            {
                Projectile.frame = 4; //tail end slash
            }
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}

        /*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];

            float rotation = (player.itemTime * (MathF.Tau / player.itemTimeMax)) * player.direction;

            return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, 150, rotation, MathF.PI); 
        }*/
    }
}