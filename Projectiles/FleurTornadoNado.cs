using KirboMod.Projectiles.Tornadoes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class FleurTornadoNado : Tornado
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fleur Tornado");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 110;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.scale = 1.2f;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override int HeightForVisual => 120;
        public override int WidthForVisual => 150;
        public override Color[] SetPalette()
        {
            Color[] palette = { new(154, 212, 255), Color.White, new(105, 226, 255) };
            return palette;
        }
        //must be like this so that the item is affected by modifiers
        int ManaToUse => (int)Projectile.ai[1];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;


            player.manaRegenDelay = 20;
            player.manaRegenCount = 0;
            bool manaIsAvailable = player.CheckMana(10);
            bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;
            if (Projectile.ai[0] % 20 == 1 && Projectile.ai[0] != 1)//don't use mana the first cycle because the item already used it
            {
                player.CheckMana(ManaToUse, true); //consume ManaToUse mana every 20 frames, affected by player's mana reduction stat
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (stillInUse) //HOMING
                {
                    float speed = 40f; //top speed
                    float inertia = 15f; //influences acceleration and decceleration

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 direction = Main.MouseWorld - Projectile.Center; //start - end 

                        direction.Normalize();
                        direction *= speed;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; //movement
                    }
                }
                else
                {
                    Projectile.Kill();
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMethods.SyncProjPosition(Projectile, (byte)Projectile.owner);
                }
            }
           
            if (++Projectile.frameCounter >= 5) //changes frames every 5 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            //Feathers
            if (Projectile.ai[0] % 10 == 0)
            {
                Vector2 velocity = Main.rand.BetterNextVector2Circular(37f); //circle
                Feathers(velocity);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(12f, 15f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, speed * 2, Scale: 3f); //Makes dust in a messy circle
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item20 with { Pitch = 2, Volume = 2 }, Projectile.Center);
            for (int i = 0; i < 10; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 velocity = Main.rand.NextVector2Circular(25f, 25f); //circle
                Feathers(velocity);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(108, 120)).Intersects(targetHitbox);
        }

        private void Feathers(Vector2 velocity)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.BetterNextVector2Circular(16 * 3), velocity, ModContent.ProjectileType<Projectiles.FleurTornadoFeather>(), Projectile.damage / 2, 5f, Projectile.owner);
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