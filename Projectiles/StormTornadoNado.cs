using KirboMod.Particles;
using KirboMod.Projectiles.Lightnings;
using KirboMod.Projectiles.Tornadoes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
            Projectile.localNPCHitCooldown = 5;
            Projectile.ArmorPenetration = 9999;
        }

        public override Color[] SetPalette()
        {
            Color[] palette = { Color.Black, Color.Gray, Color.LightGray };
            return palette;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

            Projectile.scale = 1.6f;

            //stuff here so it doesn't oppose drain
            player.manaRegenDelay = 20;
            player.manaRegenCount = 0;
            player.manaCost = 1;

            //Tooken from old Example Mod (Bad idea)
            bool manaIsAvailable = player.CheckMana(1, true, false); //consume 1 mana per tick
            bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;

            if (stillInUse) //HOMING
            {
                float speed = 40f; //top speed
                float inertia = 30f; //acceleration and decceleration speed

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 direction = Main.MouseWorld - Projectile.Center; //start - end 

                    direction.Normalize();
                    direction *= speed;
                    if (Projectile.ai[0] % 3 == 0)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; //movement
                }
                UpdateDamageForManaSickness(player); //Tooken from old Example Mod (Bad idea)
            }
            else
            {
                Projectile.Kill();
            }

            if (++Projectile.frameCounter >= 4) //changes frames every 4 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            //Storm Cloud
            if (Projectile.ai[0] % 5 == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0, ModContent.ProjectileType<Projectiles.StormTornadoCloud>(), Projectile.damage / 2, 0f, Projectile.owner);
            }

            //"Thunder"
            if (Projectile.ai[0] % 60 == 0 || Projectile.ai[0] == 1) //remainder is 0 or 1
            {
                SoundEngine.PlaySound(SoundID.Item69.WithVolumeScale(1.5f).WithPitchOffset(-20f), Projectile.Center); //staff of earth
            }

            if (Main.rand.NextBool(20)) //1/20 every tick
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(15f, 15f); //circle 
                LightningProj.GetSpawningStats(speed, out float ai0, out float ai1);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, 
                    ModContent.ProjectileType<StormTornadoLightning>(), Projectile.damage / 2, 0f, Projectile.owner, ai0, ai1);
                SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
            }
        }

        private void UpdateDamageForManaSickness(Player player) //Tooken from old Example Mod
        {
            float ownerCurrentMagicDamage = player.GetDamage(DamageClass.Generic).Multiplicative + (player.GetDamage(DamageClass.Magic).Multiplicative - 1f);
            Projectile.damage = (int)(player.HeldItem.damage * ownerCurrentMagicDamage);
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