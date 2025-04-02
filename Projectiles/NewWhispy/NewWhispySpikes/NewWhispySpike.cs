using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NewWhispy.NewWhispySpikes
{
    public class NewWhispySpike : ModProjectile
    {
        int Length => (int)Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.hide = true;
        }
        public override void AI()
        {
            if (Timer < 10)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.WoodFurniture, 0, 0, 100);
                d.velocity *= .3f;
                d.velocity += Vector2.Normalize(Projectile.velocity) * 3 * Length;
                d.scale += .6f;
                d.position.X += Main.rand.NextFloat(-20, 20);
                d.noGravity = true;
            }
            if(Timer == 10)
            {
                SoundEngine.PlaySound(SoundID.Item45 with { Pitch = -0.6f, MaxInstances = 0 }, Projectile.Center);
            }
            Timer++;
            if (Timer > 100)
            {
                Projectile.Opacity -= 0.1f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2;
        }
        public static void SpawnRoot(IEntitySource source, Vector2 pos, Vector2 direction, int damage, int length, int telegraphDuration = 40)
        {
            direction.Normalize();
            direction *= 40;//40 is height of sprite
            Projectile.NewProjectile(source, pos, direction, ModContent.ProjectileType<NewWhispySpike>(), damage, 0, Main.myPlayer, length, -telegraphDuration);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tip = ModContent.Request<Texture2D>("KirboMod/Projectiles/NewWhispy/NewWhispySpikes/NewWhispySpikeTip").Value;
            Texture2D root = TextureAssets.Projectile[Type].Value;
            int length = Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 drawPos = Projectile.Center + Projectile.velocity * i;
                Color drawColor = Lighting.GetColor((int)(drawPos.X / 16), (int)(drawPos.Y / 16));
                Texture2D texture = i == length - 1 ? tip : root;
                float opacity = Utils.GetLerpValue(0, 5, Timer - i * 5, true);
                Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, drawColor * Projectile.Opacity * opacity, Projectile.rotation, root.Size() / 2, Projectile.scale, default);
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        void GetCollisionEdges(out Vector2 bottom, out Vector2 tip)
        {
            bottom = Projectile.Center - Projectile.velocity / 2;
            tip = bottom + Projectile.velocity * (Length - .5f) * Utils.GetLerpValue(0, Length * 5, Timer, true);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            GetCollisionEdges(out Vector2 bottom, out Vector2 tip);
            float unused = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), bottom, tip, 25, ref unused);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
