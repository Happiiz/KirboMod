using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.Vine
{
    public class MarxVine : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 60;
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
        ref float Timer => ref Projectile.ai[1];
        ref float LeftToSpawn => ref Projectile.ai[2];
        static int VineProjSpacing => 40;
        public static int TimeToStartDecay => 40;
        public static int TimeUntilNewSegmentIsSpawned => 3;

        public static void SpawnVine(IEntitySource source, Vector2 start, int length, int damage, Vector2 direction)
        {
            Vector2 velocity = direction;
            velocity.Normalize();//just in case
            int p = Projectile.NewProjectile(source, start, velocity, ModContent.ProjectileType<MarxVine>(), damage, 0f, -1, Main.rand.Next(1, 4), 0f, length);

        }
        public override void AI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI * .5f;
            if (Timer > LingerDuration + LeftToSpawn / TravelSpeed)
            {
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI * .5f;

            Projectile.GetAlpha(lightColor);

            Vector2 origin = Projectile.Center;
            Vector2 start = origin;
            Vector2 travelDirection = Vector2.Normalize(Projectile.velocity) * Projectile.scale;
            float maxDistTravelled = (int)Timer * TravelSpeed;
            float originPoint = maxDistTravelled - LingerDuration * TravelSpeed;
            if (originPoint > 0)
            {
                start += travelDirection * originPoint;
            }
            else
            {
                originPoint = 0;
            }
            if (maxDistTravelled > LeftToSpawn)
            {
                maxDistTravelled = LeftToSpawn;
            }
            Vector2 end = origin + travelDirection * maxDistTravelled;
            Vector2 screenPos = Main.screenPosition;
            float rotation = Projectile.rotation;
            for (float i = 0; i < maxDistTravelled;)
            {

                Vector2 drawPos = origin + travelDirection * i;
                i += tex.Height;
                if (i < originPoint)
                {
                    continue;
                }
                if (!IsOnScreen(drawPos, tex.Width / 2, tex.Height / 2))
                {
                    continue;
                }
                float opacity = MathF.Abs(originPoint - tex.Height - i) / (tex.Height * 2);
                
                Color drawColor = Projectile.GetAlpha(Lighting.GetColor((int)(drawPos.X / 16), (int)(drawPos.Y / 16)));
                drawColor *= opacity;
                drawPos -= screenPos;
                Rectangle frame = tex.Frame(2, 1, i < maxDistTravelled ? 0 : 1, 0);
                Main.EntitySpriteDraw(tex, drawPos, frame, drawColor, rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            //GetCollisionParams(out start, out end, out float width);
            //Helper.AABBvLineVisualizer(start, end, width);
            return false;
        }

        static bool IsOnScreen(Vector2 drawPos, int frameWidth, int frameHeight)
        {
            if (drawPos.X - frameWidth > Main.screenPosition.X + Main.screenWidth)
            {
                return false;
            }
            if (drawPos.X + frameWidth < Main.screenPosition.X)
            {
                return false;
            }
            if (drawPos.Y - frameHeight > Main.screenPosition.Y + Main.screenHeight)
            {
                return false;
            }
            return drawPos.Y + frameHeight > Main.screenPosition.Y;
        }

        public static int TravelSpeed => 20;
        public static int LingerDuration => 60;
        public static int MaxDist => 600;
        void GetCollisionParams(out Vector2 start, out Vector2 end, out float width)
        {
            width = 20;
            Vector2 origin = Projectile.Center;
            start = origin;
            Vector2 travelDirection = Vector2.Normalize(Projectile.velocity) * Projectile.scale;
            float distTravelled = (int)Timer * TravelSpeed;
            float originPoint = distTravelled - LingerDuration * TravelSpeed;
            if (originPoint > 0)
            {
                start += travelDirection * originPoint;
            }
            if (distTravelled > LeftToSpawn)
            {
                distTravelled = LeftToSpawn;
            }
            end = origin + travelDirection * distTravelled;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            GetCollisionParams(out Vector2 start, out Vector2 end, out float width);
            float colPoint = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref colPoint);
        }
    }
}
