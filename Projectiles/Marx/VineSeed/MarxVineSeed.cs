using KirboMod.NPCs.Marx;
using KirboMod.Projectiles.Marx.Vine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.VineSeed
{
    public class MarxVineSeed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }
        public static int StayStillDuration => 120;
        public static int TimerValExplodeStart => 2000;
        public ref float Timer => ref Projectile.localAI[0];
        public ref float TargetExplodeYPos => ref Projectile.ai[1];
        public override void AI()
        {
            Projectile.velocity.Y += 0.03f;
            Projectile.rotation += Projectile.velocity.Y * 0.03f * Projectile.spriteDirection;
            Timer++;
            if (Projectile.Center.Y > TargetExplodeYPos)
            {
                Projectile.velocity = Vector2.Zero;
                if (Timer < TimerValExplodeStart)
                {
                    Timer = TimerValExplodeStart;
                }
                if(Timer > TimerValExplodeStart + StayStillDuration)
                {
                    Projectile.Kill();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SpawnThorns();
                    }
                }
               
            }
        }
        public Vector2[] GetThornParams(out float thornLength)
        {
            int thornCount = 2;
            thornLength = 2000;
            if (Main.getGoodWorld)
            {
                thornLength *= 1.6f;
                thornCount = 6;
            }
            else if (Main.masterMode)
            {
                thornLength *= 1.2f;
                thornCount = 5;
            }
            else if (Main.expertMode)
            {
                thornLength *= 1.1f;
                thornCount = 4;
            }
            float rotationOffset = MathF.PI / thornCount;
            if (Main.rand.NextBool())
            {
                // rotationOffset += MathF.PI;
            }
            Vector2[] directions = new Vector2[thornCount];
            for (int i = 0; i < directions.Length; i++)
            {
                float angle = Utils.Remap(i, 0, directions.Length, 0, MathF.Tau, false) + rotationOffset;
                directions[i] = angle.ToRotationVector2();
            }
            return directions;
        }
        private void SpawnThorns()
        {
            Vector2[] directions = GetThornParams(out float thornLength);
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2 vel = directions[i];
                MarxVine.SpawnVine(Projectile.GetSource_Death(), Projectile.Center, (int)thornLength, MarxBoss.VineDamage, vel);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //impossible for rotation to be 0 again, so can use this as a check just fine
            if (Projectile.rotation == 0f)
            {
                Projectile.rotation = Main.rand.NextFloat(MathF.Tau) + float.Epsilon;
                Projectile.spriteDirection = Main.rand.Next(2) * 2 - 1;
            }
            //line telegraph
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            if (Timer > TimerValExplodeStart)
            {
                DrawLineTelegraph(drawPos);

            }
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, drawPos, null, lightColor * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            return false;
        }

        private void DrawLineTelegraph(Vector2 drawPos)
        {
            Texture2D lance = TextureAssets.Extra[ExtrasID.FairyQueenLance].Value;
            Vector2[] directions = GetThornParams(out _);
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2 origin = new(0, lance.Height);
                Main.EntitySpriteDraw(lance, drawPos, null, Color.White, directions[i].ToRotation(), origin, 2f, SpriteEffects.None);
            }
        }

        public override void OnKill(int timeLeft)
        {
        }
    }
}
