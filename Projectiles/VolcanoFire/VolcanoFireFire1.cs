using KirboMod.Gores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.VolcanoFire
{
    public class VolcanoFireFire1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = false; //don't deal damage to enemies, only explosion
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;//actually why does this even have iframes? it doesn't deal damage
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;//denser afterimages
            SetStaticDefaults();
        }
        public static int RandomType => Main.rand.NextBool(3) ? ModContent.ProjectileType<VolcanoFireFire1>() : Main.rand.NextBool() ? ModContent.ProjectileType<VolcanoFireFire2>() : ModContent.ProjectileType<VolcanoFireFire3>();
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.01f;
            Projectile.velocity.Y += 0.12f;
            if (Main.rand.NextBool(3)) // happens 1/3 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 50, 50, DustID.Torch, 0f, 0f, 200, default, 1.5f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            //explode when in contact with npc
            for (int i = 0; i < Main.maxNPCs; i++) //loop statement that cycles completely every tick
            {
                NPC npc = Main.npc[i]; //any npc

                if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.active) //hitboxes touching
                {
                    Projectile.Kill();
                }
            }

            //player here too incase pvp
            for (int i = 0; i < Main.maxPlayers; i++) //loop statement that cycles completely every tick
            {
                Player player = Main.player[i]; //any player

                //hitboxes touching and player is on opposing team
                if (!player.active || player.dead || !player.Hitbox.Intersects(Projectile.Hitbox) || !player.InOpposingTeam(Main.player[Projectile.owner]))
                {
                    continue;
                }
                Projectile.Kill();
            }
            Projectile.Opacity += .2f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                for (int j = 2; j < 5; j += 2)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector2 offset = Utils.Remap(k, 0, 4, 0, MathF.Tau).ToRotationVector2() * j;
                        offset = offset.RotatedBy(Projectile.rotation);
                        Color edgeColor = lightColor;
                        edgeColor.A = 0;
                        edgeColor *= 0.25f;
                        edgeColor *= Utils.GetLerpValue(Projectile.oldPos.Length, 0, i);
                        Main.EntitySpriteDraw(tex, Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + offset, null, edgeColor * Projectile.Opacity, Projectile.oldRot[i], tex.Size() / 2, Projectile.scale, fx);
                    }
                }
            }


            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale, fx);
            for (int j = 2; j < 5; j += 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = Utils.Remap(i, 0, 4, 0, MathF.Tau).ToRotationVector2() * j;
                    offset = offset.RotatedBy(Projectile.rotation);
                    Color edgeColor = lightColor;
                    edgeColor.A = 0;
                    edgeColor *= 0.25f;
                    Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + offset, null, edgeColor * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale, fx);
                }
            }
            
            return false;
        }
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.01f, //no zero else it won't launch right
                ModContent.ProjectileType<VolcanoFireExplode>(), Projectile.damage, 7f, Projectile.owner);

            //don't spawn gores if on server
            if (Main.netMode == NetmodeID.Server)
                return;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2;
                    oldPos += Main.rand.NextVector2Circular(Projectile.width * .5f, Projectile.height * .5f);
                    Dust d = Dust.NewDustPerfect(oldPos, DustID.Torch);
                    d.scale *= 2;
                    d.noGravity = true;
                    d.velocity += Projectile.oldVelocity * .5f;
                }
            }
            for (int i = 0; i < 2; i++)
            {
                int type = Main.rand.NextBool(5) ? ModContent.DustType<VolcanoFireFragment1>() : 
                    Main.rand.NextBool(4) ? ModContent.DustType<VolcanoFireFragment2>() :
                    Main.rand.NextBool(3) ? ModContent.DustType<VolcanoFireFragment3>() : 
                    Main.rand.NextBool() ? ModContent.DustType<VolcanoFireFragment4>() : 
                    ModContent.DustType<VolcanoFireFragment5>();
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, Projectile.velocity.X * .5f, Projectile.velocity.Y *.5f - 16, 0, default);
            }
        }
    }
    public class VolcanoFireFire2 : VolcanoFireFire1 { }
    public class VolcanoFireFire3 : VolcanoFireFire1 { }
}