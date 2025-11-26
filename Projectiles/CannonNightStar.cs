using KirboMod.Items.Ammo;
using KirboMod.NPCs;
using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    //IF YOU OVERRIDE ONHIT REMEMBER TO CALL BASE!!!
    public class CannonNightStar : GoodNightStar //(mostly) uses good night star code
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override string Texture => "KirboMod/Projectiles/NightStarWhite";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged; 
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 0;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 60 * 20;
        }
        public static float StarCannonSlashDamageMult => 2f;
        public static int HomingRange => 350;
        public int ItemIDUsedToShoot => (int)Projectile.ai[1];
        public bool ShotFromSuperStarShooter => Projectile.ai[1] == ItemID.SuperStarCannon;
        public override void AI() //adapted version of good night star (also uses personal cloud targeting)
        {
            if (InitialVelLength == 0)
            {
                SoundEngine.PlaySound(NightmareOrb.StarSpreadShotSFX with { MaxInstances = 0, Volume = .6f}, Projectile.Center);
                //NightmareWizard.PlayBodyStarSoundEffect(Projectile.Center);
                InitialVelLength = Projectile.velocity.Length();
            }

            if (Projectile.timeLeft <= 30)
            {
                Projectile.Opacity = Utils.GetLerpValue(1, 30, Projectile.timeLeft);
            }
            else
            {
                Projectile.Opacity += 1 / 5f;
            }
            Lighting.AddLight(Projectile.Center, 0.255f, 0f, 0.255f);

            if (Projectile.velocity.X >= 0)
            {
                Projectile.rotation += 0.3f;
            }
            else
            {
                Projectile.rotation -= 0.3f;
            }



            //if (Main.rand.NextBool(5)) // happens 1/5 times
            {
                int dustType = DustID.Shadowflame;
                if (ShotFromSuperStarShooter)
                {
                    dustType = DustID.GemRuby;
                }
                int dustnumber = Dust.NewDust(Projectile.position, 50, 50, dustType, 0f, 0f, 200, default, 1.5f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            int targetIndex = -1;
            Vector2 center = Projectile.Center;
            float homingRange = HomingRange;
            float rangeSQ = homingRange * homingRange;
            //if invalid target
            if (!Helper.ValidIndexedTarget(TargetIndex, Projectile, out _, false))
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC compare = Main.npc[i];
                    if (!compare.CanBeChasedBy())
                        continue;
                    if (compare.DistanceSQ(center) < rangeSQ && (targetIndex == -1 || compare.DistanceSQ(center) < Main.npc[targetIndex].DistanceSQ(center)))
                    {
                        targetIndex = i;
                    }
                }
                TargetIndex = targetIndex;
            }
            targetIndex = TargetIndex;

            if (Helper.ValidIndexedTarget(targetIndex, Projectile, out NPC target, false))
            {
                Projectile.localAI[0]++;
                float homingStrength = Utils.Remap(Projectile.localAI[0], 1, 40, 0, .06f, false);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * InitialVelLength, homingStrength);
            }
            else
            {
                TargetIndex = -1;
                Projectile.localAI[0] = 0;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * InitialVelLength, .06f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //collision
        }

        public override void OnKill(int timeLeft)
        {
            Color slightlyMoreCommonColor = Color.LightSkyBlue;
            Color slightlyLessCommonColor = Color.MediumSlateBlue;
            if (ShotFromSuperStarShooter)
            {
                slightlyMoreCommonColor = Color.Red;
                slightlyLessCommonColor = Color.White;
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(10, 10); //burst of sparkles
                Sparkle.NewSparkle(Projectile.Center + Projectile.velocity, Main.rand.NextBool(3, 5) ? slightlyMoreCommonColor : slightlyLessCommonColor,
                    new Vector2(1, 1f), velocity, 40, new Vector2(2, 2));
            }
        }
        //copied vanilla function
        private void SummonSuperStarSlash(Vector2 target)
        {
            Vector2 v = Main.rand.NextVector2CircularEdge(200f, 200f);
            if (v.Y < 0f)
            {
                v.Y *= -1f;
            }
            v.Y += 100f;
            Vector2 spawnVel = v.SafeNormalize(Vector2.UnitY) * 6f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target - spawnVel * 20f, spawnVel, ProjectileID.SuperStarSlash, (int)(Projectile.damage * StarCannonSlashDamageMult), 0f, Projectile.owner, 0f, target.Y);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //KEEP BASE CALL!!
            base.OnHitNPC(target, hit, damageDone);
            if (ShotFromSuperStarShooter)
            {
                SummonSuperStarSlash(target.Center + target.velocity * 2f);
            }
        }
        public override bool? CanCutTiles()
        {
            return null;
        }
        public override bool CanHitPvp(Player target)
        {
            return true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }

        /// <summary>
        /// replaces Shoot()
        /// </summary>
        /// <returns>if it should cancel out usual shooting for the specialized shoot code</returns>
        public static bool TryShootWithSpecialData(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (source.AmmoItemIdUsed != ModContent.ItemType<NightStarAmmo>())
            {
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, -1, item.type);
            return true;
        }
        //Good night star color =>
        //new Color(116,128,255)

        public override Color? GetAlpha(Color lightColor)
        {
            Color resultColor = new(116, 128, 255, 0);
            if (ShotFromSuperStarShooter)
            {
                resultColor = Color.White;// new Color(195, 90, 61, 0);
            }
            return resultColor * Projectile.Opacity; // Makes it uneffected by light
        }
        public override bool PreDraw(ref Color lightColor) //blue "afterimage" thing
        {
            Texture2D star = TextureAssets.Projectile[Type].Value;

            Color[] afterimageColors = [new Color(0, 0, 100, 0), new Color(0, 0, 100, 0), new Color(0, 0, 100, 0)];
            Color outerTrailColor = new Color(173, 245, 255) * .15f;
            Color innerTrailColor = Color.White * 0.35f;
            float trailDistancing = 1.5f;
            int distancingOffset = 0;
            if (ShotFromSuperStarShooter)
            {

                afterimageColors = new Color[] { new(255, 131, 181), Color.Red, Color.Red * .5f };
                outerTrailColor = new Color(255, 50, 50) * .25f;
                innerTrailColor = new Color(255, 166, 89) * 0.35f;
                trailDistancing = 0.8f;
                distancingOffset = 1;
            }
            VFX.DrawProjWithStarryTrail(Projectile, outerTrailColor, innerTrailColor, default, Projectile.Opacity);
            for (int i = 0; i < afterimageColors.Length; i++)
            {
                Main.EntitySpriteDraw(star,
                    new Vector2
               (
                        (Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f) - Projectile.velocity.X * ((distancingOffset + i) * trailDistancing),
                        (Projectile.position.Y - Main.screenPosition.Y + Projectile.height - star.Height * 0.5f + 2f) - Projectile.velocity.Y * ((distancingOffset + i) * trailDistancing)
                ),
                    new Rectangle(0, 0, star.Width, star.Height),
                    afterimageColors[i] * Projectile.Opacity,
                    Projectile.rotation,
                    star.Size() * 0.5f,
                    1f,
                    SpriteEffects.None,
                    0);
            }
            // DebugDraw();
            return true;
        }
        void DebugDraw()
        {

            float dots = 40;
            Vector2 center = Projectile.Center;
            float range = HomingRange;
            Texture2D dot = TextureAssets.MagicPixel.Value;
            for (int i = 0; i < dots; i++)
            {
                Vector2 scale = new(2f, 2f / dot.Height);
                float angle = (i / dots) * MathF.Tau;
                Vector2 offset = angle.ToRotationVector2() * range;
                Vector2 next = (((i - 1) / dots) * MathF.Tau).ToRotationVector2() * range;
                float rotation = (next - offset).ToRotation();
                scale.X = (offset.Distance(next));
                Main.EntitySpriteDraw(dot, center + offset - Main.screenPosition, null, Color.White, rotation + (MathF.PI / dots), dot.Size() / 2, scale, SpriteEffects.None);
            }

            int targetIndex = TargetIndex;
            if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
            {
                return;
            }
            NPC npc = Main.npc[targetIndex];
            Utils.DrawLine(Main.spriteBatch, center, npc.Center, Color.Red, Color.Red, 4f);
        }
    }
}