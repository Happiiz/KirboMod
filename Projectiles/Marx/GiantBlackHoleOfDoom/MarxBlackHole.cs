using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.GiantBlackHoleOfDoom
{
    public class MarxBlackHole : ModProjectile
    {
        public static float ScaleUpDuration => 10f;
        static float MaxScale => 2f;
        static float SuckStrengthMin => 8f * SuckStrengthMult;
        static float SuckStrengthMax => 60f * SuckStrengthMult;
        static float SuckStrengthMult => Main.expertMode ? 1.7f : 1.05f;
        static float SuckStrengthMinDist => 120f;
        static float SuckStrengthMaxDist => 2000f;
        static float NoMoreSuckDist => 2500f;
        static float HitboxRadius => 100f;
        public static float SuckDuration => 260f;
        public static SoundStyle SuckSFX => new SoundStyle("KirboMod/Sounds/NPC/Marx/BlackHoleSuck");
        static bool[] playersHit;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.hostile = true;
            Projectile.scale = MaxScale / ScaleUpDuration;
            Projectile.tileCollide = false;
            playersHit = new bool[Main.maxPlayers];
        }
        public ref float Timer => ref Projectile.localAI[0];
        public override void AI()
        {
            Projectile.frameCounter++;
            Timer++;
            ScalingAndSFX();
            if (Timer > SuckDuration)
            {
                if (Projectile.scale <= 0)
                {
                    Projectile.Kill();
                }
                return;
            }
            KillHooks();
            SuckPlayers();

            //DebugDisplays();
        }

        private void ScalingAndSFX()
        {
            if (Timer == 1)
            {
                SoundEngine.PlaySound(SuckSFX.WithVolumeScale(0.8f), Projectile.position, null);
            }
            Projectile.scale = Helper.RemapEased(Timer + 1, 0, ScaleUpDuration, 0, 1f, Easings.EaseOutSquare);
            Projectile.scale *= Helper.RemapEased(Timer + 1, SuckDuration + ScaleUpDuration, SuckDuration, 0, 1f, Easings.EaseOutSquare);
            Projectile.scale *= MaxScale;
        }

        private void SuckPlayers()
        {
            //proj is 1 hitbox width height so can use position instead of center
            Vector2 center = Projectile.position;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player plr = Main.player[i];
                if (plr.active && !plr.dead)
                {
                    float dist = plr.Distance(center);
                    if (dist > NoMoreSuckDist)
                    {
                        continue;
                    }
                    float suckStrength = Helper.Remap(dist, SuckStrengthMinDist, SuckStrengthMaxDist, SuckStrengthMin, SuckStrengthMax);
                    plr.velocity = Vector2.Lerp(plr.velocity, plr.DirectionTo(center) * MathF.Max(plr.velocity.Length(), suckStrength), 0.05f);
                }
            }
        }

        void KillHooks()
        {
            if (Main.expertMode)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.aiStyle == ProjAIStyleID.Hook)
                    {
                        p.Kill();
                    }
                }
            }
        }
        void DebugDisplays()
        {
            Helper.DustCircle(100, SuckStrengthMaxDist, Projectile.position);
            Helper.DustCircle(50, SuckStrengthMinDist, Projectile.position);
            Helper.DustCircle(200, NoMoreSuckDist, Projectile.position);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //proj is 1 hitbox width height so can use position instead of center
            return Helper.CheckCircleCollision(targetHitbox, Projectile.position, HitboxRadius * Projectile.scale * .5f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int type = ModContent.ProjectileType<MarxBlackHole>();
            Main.instance.LoadProjectile(type);
            Texture2D texture = TextureAssets.Projectile[type].Value;
            int spinFPS = 1;
            int framesX = 5;
            int framesY = 30;
            int frameX = Projectile.frameCounter / spinFPS;
            int frameY = Projectile.frameCounter;
            frameX %= framesX;
            frameY %= framesY;
            Rectangle frame = texture.Frame(framesX, framesY, frameX, frameY);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, 0, frame.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            playersHit[target.whoAmI] = true;
        }
        public override bool CanHitPlayer(Player target)
        {
            return !playersHit[target.whoAmI];
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
}
