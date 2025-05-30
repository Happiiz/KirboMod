using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class BigRangerStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.255f, 0.255f, 0f);
            Projectile.rotation += 0.25f * Projectile.direction; // rotates projectile
            if (Main.rand.NextBool(3)) // happens 1/3 times
            {
                int dustnumber = Dust.NewDust(Projectile.position + Projectile.velocity, 74, 74, DustID.Enchanted_Gold, 0f, 0f, 200, default, 1f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1) //if ai equal 1
            {
                SoundEngine.PlaySound(SoundID.MaxMana, Projectile.position); //star sound
            }

            //explode when in contact with npc
            for (int i = 0; i < Main.maxNPCs; i++) //loop statement that cycles completely every tick
            {
                NPC npc = Main.npc[i]; //any npc

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.active) //hitboxes touching
                {
                    Explode();
                    return;
                }
            }

            //player here too incase pvp
            for (int i = 0; i < Main.maxPlayers; i++) //loop statement that cycles completely every tick
            {
                Player player = Main.player[i]; //any player

                //hitboxes touching and player is on opposing team
                if (player.Hitbox.Intersects(Projectile.Hitbox) && player.InOpposingTeam(Main.player[Projectile.owner]))
                {
                    Explode();
                    return;
                }
            }
        }
        void Explode()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.01f, //no zero else it won't launch right
    ModContent.ProjectileType<Projectiles.RangerStarExplode>(), Projectile.damage, 8, Projectile.owner);
            }
            Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //collision
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}