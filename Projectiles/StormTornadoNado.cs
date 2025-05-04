using KirboMod.Projectiles.Lightnings;
using KirboMod.Projectiles.Tornadoes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class StormTornadoNado : Tornado
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Storm Tornado");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 216;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            //uses own immunity frames
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 1.6f;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override int HeightForVisual => 110;
        public override int WidthForVisual => 240;
        public static int LightningRate => 10;
        int ManaToUse => (int)Projectile.ai[1];
        static float StormCloudDamageMult => 0.5f;
        static float ThunderDamageMult => 1;
        public override Color[] SetPalette()
        {
            Color[] palette = { Color.White, Color.Black, Color.MediumPurple };
            return palette;
        }

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
                    float speed = 60f; //top speed
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
            //Storm Cloud
            if (Projectile.ai[0] % 5 == 0 && Main.myPlayer == Projectile.owner)
            {
                Vector2 offset = Main.rand.BetterNextVector2Circular(40);
                offset.Y *= 1.5f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity * 0.01f, ModContent.ProjectileType<Projectiles.StormTornadoCloud>(), (int)(Projectile.damage * StormCloudDamageMult), 0f, Projectile.owner);
            }

            //"Thunder"
            if (Projectile.ai[0] % 60 == 1) 
            {
                SoundEngine.PlaySound(SoundID.Item69.WithVolumeScale(1.5f).WithPitchOffset(-20f), Projectile.Center); //staff of earth
            }

            if (Projectile.ai[0] % LightningRate == 0)
            {
                SpawnLightningBolt();
            }
        }

        private void SpawnLightningBolt()
        {
            SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
            if (Main.myPlayer != Projectile.owner)
            {
                return;
            }
            Vector2 speed = Main.rand.NextVector2CircularEdge(15f, 15f); //circle 
            int target = -1;
            Vector2 center = Projectile.Center;
            float minRange = 800f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy())
                {
                    continue;
                }
                if (target == -1)
                {
                    if (npc.DistanceSQ(center) < minRange * minRange)
                    {
                        target = i;
                    }
                    continue;
                }
                NPC current = Main.npc[target];
                if (current.DistanceSQ(center) > npc.DistanceSQ(center))
                {
                    target = i;
                }
            }
            Vector2 projSpawnPos = center + Main.rand.BetterNextVector2Circular(32f);
            if (target != -1)
            {
                NPC targetNPC = Main.npc[target];
                Utils.ChaseResults results = Utils.GetChaseResults(projSpawnPos, speed.Length(), targetNPC.Center, targetNPC.velocity);
                if (results.InterceptionHappens)
                {
                    speed = results.ChaserVelocity;
                }
            }
            LightningProj.GetSpawningStats(speed, out float ai0, out float ai1);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(),projSpawnPos, speed,
                ModContent.ProjectileType<StormTornadoLightning>(), (int)(Projectile.damage * ThunderDamageMult) , 0f, Projectile.owner, ai0, ai1);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 60; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(24f, 30f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainCloud, speed, Scale: 5f, newColor: new Color(80, 80, 80)); //Makes dust in a messy circle
                d.noGravity = true;
            }
            for (int i = 0; i < 6; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(15f, 15f); //circle 
                LightningProj.GetSpawningStats(speed, out float ai0, out float ai1);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed,
                    ModContent.ProjectileType<StormTornadoLightning>(), Projectile.damage / 2, 0f, Projectile.owner, ai0, ai1);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(180, 200)).Intersects(targetHitbox);
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