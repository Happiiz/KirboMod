using KirboMod.Projectiles.Tornadoes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class TornadoNado : Tornado
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tornado");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ArmorPenetration = 9999;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        int ManaToUse => (int)Projectile.ai[1];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;
            player.manaRegenDelay = 20;
            player.manaRegenCount = 0;
            bool manaIsAvailable = player.CheckMana(ManaToUse);
            bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;
            if (Projectile.ai[0] % 20 == 1 && Projectile.ai[0] != 1)//don't use mana the first cycle because the item already used it
            {
                player.CheckMana(ManaToUse, true); //consume ManaToUse mana every 20 frames, affected by player's mana reduction stat
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (stillInUse) //HOMING
                {
                    float speed = 23; //top speed
                    float inertia = 9f; //influences acceleration and decceleration

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 direction = Main.MouseWorld - Projectile.Center; //start - end 

                        direction.Normalize();
                        direction *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; //movement
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMethods.SyncProjPosition(Projectile, (byte)Projectile.owner);
                    }
                }
                else
                {
                    Projectile.Kill();
                }

            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(8f, 10f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, speed * 2, Scale: 3f); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(90, 100)).Intersects(targetHitbox);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Collision.CanHit(Projectile, target))
            {
                return null;
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) //bounce
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y) //bounce
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}