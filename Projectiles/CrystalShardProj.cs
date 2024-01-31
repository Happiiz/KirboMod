using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace KirboMod.Projectiles
{
    public class CrystalShardProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Shard"); 
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {

            Projectile.height = 22;
            Projectile.width = 22;
            DrawOffsetX = -21;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.alpha = 50;
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 1000 * Projectile.MaxUpdates;
        }
        int TargetIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        ref float InitialVelLength { get => ref Projectile.ai[1]; }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item11);
                InitialVelLength = Projectile.velocity.Length();
                TargetIndex = -1;
                Projectile.rotation = MathF.PI / 2;
                if (Main.gfxQuality == 1)
                {
                    //doing it like this for multiplayer reasons
                    Vector2 mousePos = Projectile.Center + Projectile.velocity * Projectile.ai[2] / InitialVelLength;
                    float diamondWidth = 4;
                    float spacingMultiplier = .4f;
                    DiamondShapeDustEffect(mousePos, diamondWidth, spacingMultiplier);
                }
                Projectile.localAI[0]++;
            }
            float rangeSQ = 1500 * 1500;
            if (!Helper.ValidIndexedTarget(TargetIndex, Projectile, out _))
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * InitialVelLength, 0.1f);
                int closestNPC = -1;
                Vector2 center = Projectile.Center;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC potentialTarget = Main.npc[i];
                    float distToClosestPointInPotentialTargetHitbox = center.DistanceSQ(potentialTarget.Hitbox.ClosestPointInRect(center));
                    bool notValidTarget = !Helper.ValidHomingTarget(potentialTarget, Projectile);
                    if (notValidTarget || distToClosestPointInPotentialTargetHitbox > rangeSQ)
                        continue;
                    if (!Main.npc.IndexInRange(closestNPC) || center.DistanceSQ(potentialTarget.Hitbox.ClosestPointInRect(center)) < center.DistanceSQ(Main.npc[closestNPC].Hitbox.ClosestPointInRect(center)))
                        closestNPC = i;
                }
                TargetIndex = closestNPC;
                Projectile.localAI[0] = 1;
            }
            if (Helper.ValidIndexedTarget(TargetIndex, Projectile, out NPC target))
            {
                Projectile.localAI[0]++;
                float homingStrength = Helper.RemapEased(Projectile.localAI[0], 1, 20, 0, .1f, Easings.EaseInOutSine);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * InitialVelLength, homingStrength);
            }

            Rectangle dustBox = Projectile.Hitbox;
            ModifyDamageHitbox(ref dustBox);
            if (Main.rand.NextFloat() < Utils.Remap(Main.maxDustToDraw, 0, 6000, 0, 1))
            {
                if (Main.rand.NextBool(3))
                {
                    Color col = Color.Lerp(Color.Cyan, Color.Purple, Main.rand.NextFloat());
                    Vector2 scale = new Vector2(1, 3) * .7f;
                    Vector2 fatness = new Vector2(2, 2) * .7f;
                    Sparkle sp = new Sparkle(Main.rand.NextVector2FromRectangle(dustBox), col, Main.rand.NextVector2Circular(30,30) * .2f, scale, fatness, 40);
                    sp.Confirm();
                    sp.velocity += Projectile.velocity * .1f;
                    sp.fadeOutTime = 20;
                    sp.friction = .98f;
                }
                int index = Dust.NewDust(dustBox.TopLeft(), dustBox.Width, dustBox.Height, ModContent.DustType<Dusts.RainbowSparkle>(), 0f, 0f, 200, default, 1f); //dust
                Main.dust[index].velocity *= 0.3f;
            }
        }

        private void DiamondShapeDustEffect(Vector2 pos, float diamondWidth, float spacingMultiplier)
        {
            spacingMultiplier *= Utils.Remap(Main.maxDustToDraw, 0, Main.maxDust, 0, 1);
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = Utils.Remap(i, 0, 4, 0, MathF.Tau, false).ToRotationVector2();
                offset.Y *= 3;
                offset *= diamondWidth;
                Vector2 oldOffset = Utils.Remap(i - 1, 0, 4, 0, MathF.Tau, false).ToRotationVector2();
                oldOffset.Y *= 3;
                oldOffset *= diamondWidth;
                DustLine(oldOffset, offset, spacingMultiplier, pos);
            }
            Color col = Color.Lerp(Color.Cyan, Color.Purple, Main.rand.NextFloat());
            Vector2 scale = new Vector2(3, 6);
            Vector2 fatness = new Vector2(3, 3);
            Sparkle sp = new Sparkle(pos, col, null, scale, fatness, 20);
            sp.Confirm();
        }

        void DustLine(Vector2 from, Vector2 to, float spacingMultiplier, Vector2 centerRef)
        {
            from += centerRef;
            to += centerRef;
            float dist = from.Distance(to);
            for (float i = 0; i < 1; i += spacingMultiplier / dist)
            {
                Vector2 vel = Vector2.Lerp(from, to, i) - centerRef;
                Dust d = Dust.NewDustPerfect(Vector2.Lerp(from, to, i), DustID.RainbowMk2, vel);
                d.color = Color.Lerp(Color.Cyan, Color.Purple, Main.rand.NextFloat());
                d.scale *= 1.2f;
                d.noGravity = true;
                if (d.dustIndex != 6000)
                {
                    d = Dust.CloneDust(d);
                    d.scale *= .6f;
                    d.color = Color.White;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center); //crystal break

            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ModContent.DustType<Dusts.RainbowSparkle>(), speed, 0, default, Scale: 1f); //Makes dust in a messy circle
            }
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ModContent.DustType<Dusts.CrystalBit>(), speed, 0, default, Scale: 1f); //Makes dust in a messy circle
            }
            Projectile.Hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(100, 500));
            Projectile.penetrate = -1;
            Projectile.Damage();
            DiamondShapeDustEffect(Projectile.Center, 8, .4f);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D tex = Projectile.MyTexture(out Vector2 origin, out SpriteEffects fx);
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                float opacity = Utils.GetLerpValue(Projectile.oldPos.Length, 0, i);
                Main.EntitySpriteDraw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.White * opacity, Projectile.rotation, origin, Projectile.scale, fx);
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, fx);
            return false;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(22, 48));
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}