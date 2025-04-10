using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class CyborgArcherArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laser Arrow");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            DrawOriginOffsetX = -9;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;//don't initially collide with tile. set in AI
            Projectile.penetrate = 10;
            Projectile.scale = 1f;
            Projectile.aiStyle = 0;
            Projectile.light = 0.4f;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 40 * Projectile.MaxUpdates; //40  frames
            //Doesn't wait for any immunity cooldown
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ArmorPenetration = 25;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (!Projectile.tileCollide && !Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.tileCollide = true;
            }
            int dustnumber = Dust.NewDust(Projectile.position, 20, 20, DustID.GemRuby, 0f, 0f, 0, default, 1f); //dust
            Main.dust[dustnumber].velocity *= 0.0f;
            Main.dust[dustnumber].noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.BetterNextVector2Circular(2f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, speed * 2, Scale: 1f); //Makes dust in a messy circle
                d.noGravity = true;
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10.WithVolumeScale(0.5f), Projectile.position); //impact
            return true; //collision
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; //make it unaffected by light
        }
    }
}