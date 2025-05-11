using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class MaskedFireTornado : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            // DisplayName.SetDefault("Fire Spin");
        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 90;
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
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 200, default, 1.2f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            

            Lighting.AddLight(Projectile.Center, 0.9f, 0.5f, 0f); //orange light
            player.SetImmuneTimeForAllTypes(40);
            //extra...
            player.maxFallSpeed = 0;
            player.noKnockback = true;
            player.dashType = 0;

            player.canRocket = false;
            player.carpet = false;
            player.carpetFrame = -1;

            //disable kirby balloon
            player.GetModPlayer<KirbPlayer>().kirbyballoon = false;
            player.GetModPlayer<KirbPlayer>().kirbyballoonwait = 1;

            //double jump effects
            player.blockExtraJumps = true;
            player.DryCollision(true, true); //fall through platforms
            player.mount.Dismount(player); //dismount mounts

            player.velocity.Y -= player.gravity;
            if (Main.myPlayer == player.whoAmI)
            {
                float dashTopSpeed = Utils.Remap(Projectile.localAI[1], 0, 80, 12, 22);
                float dashSteeringRate = .07f;
                Vector2 targetVelocity = player.Center.DirectionTo(Main.MouseWorld) * dashTopSpeed;
                player.velocity = Vector2.Lerp(player.velocity, targetVelocity, dashSteeringRate);
                if (Projectile.localAI[0] == 0)//first frame
                {
                    player.velocity = targetVelocity;
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
            target.AddBuff(BuffID.Daybreak, 1800);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int frames = Main.projFrames[Type];
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Player plr = Main.player[Projectile.owner];
            Vector2 drawPos = plr.Center - Main.screenPosition;
            DrawHammers(drawPos);
            Rectangle frame = tex.Frame(1, frames, 0, Projectile.frame);
            int frameHeight = tex.Height / Main.projFrames[Type];
            DrawSegmentedTornado(tex, drawPos, frame, frameHeight, default, 1, Color.White, .7f, 1.2f);

            frame = tex.Frame(1, frames, 0, (Projectile.frame + 2) % frames);
            Color col = (Color.White with { A = 128 }) * .3f;
            DrawSegmentedTornado(tex, drawPos, frame, frameHeight, default, 1.5f, col, .7f, 1.2f);

            return false;
        }

        private void DrawSegmentedTornado(Texture2D tex, Vector2 drawPos, Rectangle frame, int segments, int frameHeight, float drawScale, Color col, float bottomXScale, float topXScale)
        {
            frameHeight = (tex.Height / Main.projFrames[Type]) / segments;
            for (int i = 0; i < segments; i++)
            {
                Rectangle f = frame;
                f.Height /= segments;
                f.Y = (int)Utils.Remap(i, 0, segments - 1, f.Y, f.Y + (segments - 1) * frameHeight);
                Vector2 offset = new Vector2(0, frameHeight * i - (frameHeight * (segments - 1) * .5f)) * drawScale;
                Main.EntitySpriteDraw(tex, drawPos + offset, f, col * Projectile.Opacity, Projectile.rotation, f.Size() / 2, Projectile.scale * new Vector2(Utils.Remap(i, 0, segments, topXScale, bottomXScale), 1) * drawScale, SpriteEffects.None);
            }
        }

        void DrawHammers(Vector2 drawPos)
        {
            Texture2D tex = ModContent.Request<Texture2D>("KirboMod/Items/Weapons/MaskedHammerHeld").Value;
            for (int i = 0; i < 2; i++)
            {
                float timer = MathF.Cos(i * MathF.PI + Projectile.localAI[1] * 0.4f);
                Vector2 scale = new Vector2(timer, 1) * .7f;
                SpriteEffects fx = SpriteEffects.None;
                Vector2 origin = new Vector2(0, tex.Height / 2);
                if (scale.X < 0)
                {
                    scale.X *= -1;
                    fx = SpriteEffects.FlipHorizontally;
                    origin = new Vector2(tex.Width, tex.Height / 2);
                }
                float rotationOffset = MathF.Sin(Projectile.localAI[1] * .2f) * .2f;
                Main.EntitySpriteDraw(tex, drawPos + new Vector2(timer * 10, -2), null, Color.White * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, scale * 0.3f, fx);
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsOverWiresUI.Add(index); //go in front of players
        }
    }
}