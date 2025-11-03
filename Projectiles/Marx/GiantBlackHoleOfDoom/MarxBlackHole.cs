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
        public static float ScaleUpDuration => 20f;// 10f;
        static float MaxScale => 2f;
        static float SuckStrengthMin => 8f * SuckStrengthMult;
        static float SuckStrengthMax => 60f * SuckStrengthMult;
        static float SuckStrengthMult => Main.expertMode ? 1.7f : 1.05f;
        static float SuckStrengthMinDist => 120f;
        static float SuckStrengthMaxDist => 2000f;
        static float NoMoreSuckDist => 2500f;
        static float HitboxRadius => 100f;
        public static float SuckDuration => 260f;
        public static SoundStyle SuckSFX => new("KirboMod/Sounds/NPC/Marx/BlackHoleSuck");
        static bool[] playersHit;
        PurpleSmoke[] particles;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.hostile = true;
            Projectile.scale = MaxScale / ScaleUpDuration;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            playersHit = new bool[Main.maxPlayers];
            int maxParticles = 300;
            particles = new PurpleSmoke[maxParticles];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new()
                {
                    Active = false,
                    index = (short)i
                };
            }
        }
        public ref float Timer => ref Projectile.localAI[0];
        public override void AI()
        {
            Projectile.frameCounter++;
            Timer++;
            ScalingAndSFX();
            SpawnDust();
            UpdateParticles();
            if (Timer > SuckDuration)
            {
                if (Projectile.scale <= 0 && NoActiveParticles())
                {
                    Projectile.Kill();
                }
                return;
            }
            KillHooks();
            SuckPlayers();

            //DebugDisplays();
        }
        bool NoActiveParticles()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].Active)
                {
                    return false;
                }
            }
            return true;
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
                    Vector2 newVel = Vector2.Lerp(plr.velocity, plr.DirectionTo(center) * MathF.Max(plr.velocity.Length(), suckStrength), 0.05f);
                    if (!newVel.HasNaNs())
                    {
                        plr.velocity = newVel;
                    }
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
        void SpawnDust()
        {
            float spread = 0.35f;
            //proj is 1 hitbox width height
            Vector2 center = Projectile.position;
            float spawnDist = 1000;
            float spawnDistSpread = 100;
            //+3 so it's a bit in advance
            int initialTimerValue = (int)MathF.Max(0, Timer - SuckDuration + PurpleSmoke.TotalLifetime - ScaleUpDuration);
            float particlePerFrame = 3;
            for (int i = 0; i < particlePerFrame; i++)
            {
                float finalSpawnDist = spawnDist + Main.rand.NextFloat(-spawnDistSpread / 2, spawnDistSpread / 2);
                Vector2 offsetDirVec = (Timer + i / particlePerFrame + Main.rand.NextFloat(spread)).ToRotationVector2();
                Vector2 spawnPos = center + offsetDirVec * finalSpawnDist;
                Vector2 targetPos = center - offsetDirVec.RotatedBy(MathF.PI * .5f) * 190;
                Vector2 vel = (targetPos - spawnPos) / PurpleSmoke.TotalLifetime;
                SpawnPurpleSmoke(spawnPos, vel, initialTimerValue);
                //calculate if the dust will be sucked in depending on a time treshold? if so set custom data to be a flag that indicates it will scale down near the end of its lifetime?
                //or maybe just set a position as customdata and make dust scale down in update depending on proximity to it
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
            DrawParticles(Main.spriteBatch, Main.screenPosition);
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
        void SpawnPurpleSmoke(Vector2 pos, Vector2 vel, float initialTimerVal = 0)
        {
            if (Timer >= SuckDuration)
            {
                return;
            }
            for (int i = 0; i < particles.Length; i++)
            {
                PurpleSmoke smoke = particles[i];
                if (smoke.Active)
                {
                    continue;
                }
                smoke.Spawn(pos, vel, initialTimerVal);
                break;
            }
        }
        void UpdateParticles()
        {
            int activeParticleCount = 0;
            for (int i = 0; i < particles.Length; i++)
            {
                PurpleSmoke smoke = particles[i];
                if (smoke.Active)
                {
                    activeParticleCount++;
                }
                smoke.Update();
            }
        }
        void DrawParticles(SpriteBatch sb, Vector2 screenPos)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                PurpleSmoke smoke = particles[i];
                if (smoke.Active)
                {
                    smoke.Draw(sb, screenPos);
                }
            }
        }
        class PurpleSmoke
        {
            public static int TimeBeforeFadingOut => 60;
            public static int FadeOutDuration => 15;
            public static int FadeInDuration => 25;
            public static int TotalLifetime => TimeBeforeFadingOut + FadeOutDuration;
            public Vector2 pos;
            public Vector2 vel;
            public float timer;
            public short index;
            public short alpha;
            public Color color;
            public float rotation;
            public bool Active 
            {
                get => timer <= TimeBeforeFadingOut + FadeOutDuration; 
                set => timer = value ? 0 : TimeBeforeFadingOut + FadeOutDuration + 1; 
            }
            public void Spawn(Vector2 pos, Vector2 vel, float initialTimerVal = 0)
            {
                timer = 0;
                color = Color.Lerp(new Color(91, 0, 181), new Color(139, 0, 181), Main.rand.NextFloat());
                alpha = 255;
                this.pos = pos;
                this.vel = vel;
                timer = initialTimerVal;
            }
            public void Update()
            {
                if (!Active)
                {
                    return;
                }
                pos += vel;
                timer++;
                alpha -= (short)(255 / FadeInDuration);
                rotation += 0.05f;
                if (alpha < 0)
                {
                    alpha = 0;
                }
                if (timer >= TimeBeforeFadingOut)
                {
                    alpha = (short)Utils.Remap(timer, TimeBeforeFadingOut, TimeBeforeFadingOut + FadeOutDuration, 0, 255, false);
                }
            }
            public Color GetColor()
            {
                return color * (1f - alpha / 255f);
            }
            public void Draw(SpriteBatch sb, Vector2 screenPos)
            {
                int indexMod3 = index % 3;
                Texture2D tex = (indexMod3 == 0 ? Dusts.MarxPurpleSmoke.PurpleSmoke.cloud1 : indexMod3 == 1 ? Dusts.MarxPurpleSmoke.PurpleSmoke.cloud2 : Dusts.MarxPurpleSmoke.PurpleSmoke.cloud3).Value;
                sb.Draw(tex, pos - screenPos, null, GetColor(), rotation, tex.Size() / 2, 0.5f, (SpriteEffects)(index % 2), 0f);
            }
        }
    }
}
