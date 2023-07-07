using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Pets
{
	public class ZeroPet : ModProjectile
	{
        int follower1;
        int follower2;
        int follower3;
        int follower4;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Light Wanderer");
			Main.projFrames[Projectile.type] = 2;
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, 2, 20)
                .WithOffset(-4f, -2f)
                .WithCode(CharacterPreviewCustomization);
        }
        public static void CharacterPreviewCustomization(Projectile proj, bool walking)
        {

        }

        public sealed override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 54;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
		}

		// Here you can decide if your pet breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		public override void AI()
		{
            Projectile.spriteDirection = Projectile.direction;

            Player player = Main.player[Projectile.owner];

            KirbPlayer modPlayer = player.GetModPlayer<KirbPlayer>();
            if (player.dead)
            {
                modPlayer.zeroPet = false;
            }
            if (modPlayer.zeroPet)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 IdlePosition = player.Center;
            //float petPositionOffsetX = (40 + Projectile.minionPos * 40) * -player.direction; //behind player depending on order summoned
            //IdlePosition.X += petPositionOffsetX;

            Vector2 vectorToIdlePosition = IdlePosition - Projectile.Center; //distance from idle
            float distanceToIdlePosition = vectorToIdlePosition.Length(); //aboslute distance from idle

            if (distanceToIdlePosition <= 1000f) //move within this range
            {
                float speed = 4.5f;
                float inertia = 12f;

                Projectile.tileCollide = false;

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

            if (++Projectile.frameCounter >= 20) //changes frames every 20 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 1) //higher than 1
                {
                    Projectile.frame = 0; //start of bob
                }
            }

            if (Projectile.ai[0] == 0)
            {
                follower1 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero,
                ModContent.ProjectileType<DarkFollower>(), 0, 0, Projectile.owner, Projectile.whoAmI);
                follower2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 48, Projectile.position.Y, 0, 0,
                ModContent.ProjectileType<DarkFollower>(), 0, 0, Projectile.owner, Projectile.whoAmI);
                follower3 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y + 54, 0, 0,
                ModContent.ProjectileType<DarkFollower>(), 0, 0, Projectile.owner, Projectile.whoAmI);
                follower4 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 48, Projectile.position.Y + 54, 0, 0,
                ModContent.ProjectileType<DarkFollower>(), 0, 0, Projectile.owner, Projectile.whoAmI);
            }
            Projectile.ai[0]++;

            if (Projectile.active) //keep setting to 2 if alive
            {
                Main.projectile[follower1].timeLeft = 2;
                Main.projectile[follower2].timeLeft = 2;
                Main.projectile[follower3].timeLeft = 2;
                Main.projectile[follower4].timeLeft = 2;
            }

            //leave dust
            if (Projectile.frameCounter % 30 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Redsidue>(), Vector2.Zero, 0, default, 0.5f);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}