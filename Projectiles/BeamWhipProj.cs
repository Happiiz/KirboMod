using KirboMod.Items.Weapons;
using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class BeamWhipProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] += 16 * 30;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.WhipSettings.RangeMultiplier = 2;
            Projectile.WhipSettings.Segments = 10;
            Projectile.extraUpdates = 3;
        }
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public static int StaffLength => 60;
        public override bool PreAI()
        {
            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;//leaving it like this incase vanilla code uses it
            Timer += 1f;
            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * (Timer - 1f);
            Projectile.spriteDirection = (!(Vector2.Dot(Projectile.velocity, Vector2.UnitX) < 0f)) ? 1 : (-1);
            if (Timer % (4 * Projectile.MaxUpdates) == 1 && Timer < timeToFlyOut - (4 * Projectile.MaxUpdates))
            {
                if (Timer + (4 * Projectile.MaxUpdates) >= timeToFlyOut - (4 * Projectile.MaxUpdates))
                {
                    SoundEngine.PlaySound(WaddleDoo.BeamAttackEnd, Projectile.Center);
                }
                else
                {
                    SoundEngine.PlaySound(WaddleDoo.BeamAttackLoop, Projectile.Center);
                }
            }
            if (Timer >= timeToFlyOut)
            {
                Projectile.Kill();
                return false;
            }

            player.heldProj = Projectile.whoAmI;
            player.MatchItemTimeToItemAnimation();
            return false; // Prevent the vanilla whip AI from running.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f); // Multihit penalty. Decrease the damage the more enemies the whip hits.
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            Vector2 pos = list[0];
            float prevDrawnRadius = -1;
            Vector2 prevDrawnPos = pos;
            Texture2D staff = TextureAssets.Item[ModContent.ItemType<BeamStaff>()].Value;
            Vector2 origin = new Vector2(8, staff.Height - 8);//almost bottom left of sprite as origin
            float staffRotation = (list[4] - pos).ToRotation();

            for (int i = 0; i < list.Count; i++)
            {
                pos = list[i];
                if (prevDrawnRadius == -1)
                {
                    prevDrawnRadius = Utils.Remap(i, 0, list.Count - 1, 0.3f, 1f);
                }
                if(pos.Distance(prevDrawnPos) > prevDrawnRadius)
                {
                    prevDrawnPos = pos;
                    prevDrawnRadius = Utils.Remap(i, 0, list.Count - 1, 0.3f, 1f);
                    VFX.DrawWaddleDooBeam(pos, prevDrawnRadius, 1f);
                }
            }
            Main.EntitySpriteDraw(staff, list[0] - Main.screenPosition, null, Color.White, staffRotation + MathF.PI / 4, origin, 1f, SpriteEffects.None);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Projectile.WhipPointsForCollision.Clear();
            Projectile.FillWhipControlPoints(Projectile, Projectile.WhipPointsForCollision);
            for (int m = 0; m < Projectile.WhipPointsForCollision.Count; m++)
            {
                Point point = Projectile.WhipPointsForCollision[m].ToPoint();
                projHitbox.Location = new Point(point.X - projHitbox.Width / 2, point.Y - projHitbox.Height / 2);
                if (projHitbox.Intersects(targetHitbox))
                {
                    return true;
                }
            }
            return false;
        }
    }
}