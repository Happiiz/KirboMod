using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.HammerSwings
{
    public class HammerSwing : ModProjectile
    {
        int spinDirection = 1;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override string Texture => "KirboMod/NothingTexture";

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
            Projectile.localAI[1]++;
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            Projectile.Opacity = Utils.GetLerpValue(1, 7, Projectile.timeLeft, true) * Utils.GetLerpValue(90, 90 - 7, Projectile.timeLeft, true);
            //Animation
            if (++Projectile.frameCounter >= 3) //changes frames every 3 ticks 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.timeLeft == 1)
            {
                player.HeldItem.noUseGraphic = false;
            }
            player.SetDummyItemTime(2);
            player.ChangeDir(((Projectile.localAI[1] % 8) < 4) ? -1 : 1);
            for (int i = 0; i < 6; i++)
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 200, default, 2f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            player.immuneTime = player.itemAnimationMax + 10;
            player.immune = true;
            player.SetImmuneTimeForAllTypes(10);
            player.mount.Dismount(player); //dismount mounts


            if (Main.myPlayer == player.whoAmI)
            {
                float dashSteeringRate = .07f;
                Vector2 targetVelocity = player.Center.DirectionTo(Main.MouseWorld) * 15;
                player.velocity = Vector2.Lerp(player.velocity, targetVelocity, dashSteeringRate);
                if (Projectile.localAI[0] == 0)//first frame
                {
                    spinDirection = Math.Sign(Main.MouseWorld.X - player.Center.X);

                    player.velocity.X = spinDirection * 15;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (Projectile.timeLeft > 1 && Projectile.localAI[0] == 1)
                    {
                        NetMethods.SyncPlayerPosition(player);
                    }
                    else
                    {
                        Vector2 velocity = player.velocity;
                        player.velocity = Vector2.Zero;
                        NetMethods.SyncPlayerPositionAndVelocity(player);
                        player.velocity = velocity;
                    }
                }
            }
            else
            {
                player.velocity = Vector2.Zero;
            }
            Projectile.localAI[0] = 1;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300); //inflict with fire for 5 seconds 
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player plr = Main.player[Projectile.owner];
            Vector2 drawPos = plr.Center - Main.screenPosition;
            DrawHammers(drawPos);

            return false;
        }

        void DrawHammers(Vector2 drawPos)
        {
            Texture2D tex = ModContent.Request<Texture2D>("KirboMod/Items/Weapons/HammerHeld").Value;
            for (int i = 0; i < 2; i++)
            {
                float timer = MathF.Cos(i * MathF.PI + Projectile.localAI[1] * 0.4f);
                Vector2 scale = new Vector2(timer, 1);
                SpriteEffects fx = SpriteEffects.None;
                Vector2 origin = new Vector2(0, tex.Height / 2);
                if (scale.X < 0)
                {
                    scale.X *= -1;
                    fx = SpriteEffects.FlipHorizontally;
                    origin = new Vector2(tex.Width, tex.Height / 2);
                }
                float rotationOffset = MathF.Sin(Projectile.localAI[1] * .2f) * .2f;
                Main.EntitySpriteDraw(tex, drawPos + new Vector2(timer * 10, -2), null, Color.White * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, scale, fx);
            }
        }
    }
}