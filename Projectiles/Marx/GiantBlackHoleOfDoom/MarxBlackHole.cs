using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.GiantBlackHoleOfDoom
{
    public class MarxBlackHole : ModProjectile
    {
        public static float ScaleUpDuration => 10f;
        static float MaxScale => 1f;
        static float SuckStrengthMin => 8f * SuckStrengthMult;
        static float SuckStrengthMax => 60f * SuckStrengthMult;
        static float SuckStrengthMult => Main.expertMode ? 1.3f : 1.05f;
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
            Timer++;
            if(Timer == 1)
            {
                SoundEngine.PlaySound(SuckSFX.WithVolumeScale(0.8f), Projectile.position, null);
            }
            Projectile.scale = Helper.RemapEased(Timer + 1, 0, ScaleUpDuration, 0, MaxScale, Easings.EaseOutSquare);
            Projectile.scale *= Helper.RemapEased(Timer + 1, SuckDuration + ScaleUpDuration, SuckDuration, 0, MaxScale, Easings.EaseOutSquare);
            if(Timer > SuckDuration)
            {
                if(Projectile.scale <= 0)
                {
                    Projectile.Kill();
                }
                return;
            }

            //proj is 1 hitbox width height so can use position instead of center
            Vector2 center = Projectile.position;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player plr = Main.player[i];
                if (plr.active && !plr.dead)
                {
                    float dist = plr.Distance(center);
                    if(dist > NoMoreSuckDist)
                    {
                        continue;
                    }
                    float suckStrength = Helper.Remap(dist, SuckStrengthMinDist, SuckStrengthMaxDist, SuckStrengthMin, SuckStrengthMax);
                    plr.velocity = Vector2.Lerp(plr.velocity, plr.DirectionTo(center) * MathF.Max(plr.velocity.Length(), suckStrength), 0.05f);
                }
            }
            //DebugDisplays();
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
            return Helper.CheckCircleCollision(targetHitbox, Projectile.position, HitboxRadius);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawSelf();
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
