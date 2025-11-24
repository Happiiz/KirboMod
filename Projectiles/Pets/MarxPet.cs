using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Pets
{
	public class MarxPet : ModProjectile
	{
        public override void SetStaticDefaults()
		{
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, 2)
                .WithOffset(-32f, 8f)
                .WithCode(CharacterPreviewCustomization);
        }
        public static void CharacterPreviewCustomization(Projectile proj, bool walking)
        {
            proj.scale = 0.6f;

            proj.ai[0]++;

            if (walking)
            {
                proj.position.Y += MathF.Cos(MathF.Tau / 180 * proj.ai[0]) * 2;
            }
        }

        public sealed override void SetDefaults()
		{
			Projectile.width = 76;
			Projectile.height = 88;
            DrawOriginOffsetY = -26;
            DrawOffsetX = -28;
            Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public override void AI()
		{
            Projectile.spriteDirection = Projectile.direction;

            Projectile.rotation = Projectile.velocity.X * 0.08f;


            Player player = Main.player[Projectile.owner];

            KirbPlayer modPlayer = player.GetModPlayer<KirbPlayer>();
            if (player.dead)
            {
                modPlayer.marxPet = false;
            }
            if (modPlayer.marxPet)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 IdlePosition = player.Center;

            Vector2 vectorToIdlePosition = IdlePosition - Projectile.Center; //distance from idle
            float distanceToIdlePosition = vectorToIdlePosition.Length(); //aboslute distance from idle

            if (distanceToIdlePosition <= 1000f) //move within this range
            {
                float speed = 5f;
                float inertia = 6f;

                Vector2 direction = player.Center - Projectile.Center; //start - end

                float distance = Vector2.Distance(player.Center, Projectile.Center);

                if (distance <= 60) //close to player
                {
                    Projectile.velocity = Projectile.velocity;
                }
                else
                {
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;  //fly towards player
                }
            }
            else //teleport
            {
                Projectile.Center = player.Center;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, TorchID.Yellow);
            return Color.White; // Makes it uneffected by light
        }
    }
}