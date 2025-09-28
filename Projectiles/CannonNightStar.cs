using KirboMod.NPCs;
using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class CannonNightStar : GoodNightStar //(mostly) uses good night star code
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        float initialVelLength = 30; //own initial vel length

        public override string Texture => "KirboMod/Projectiles/GoodNightStar";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.ArmorPenetration = 0;
            Projectile.tileCollide = true;
        }

        public override void AI() //adapted version of good night star (also uses personal cloud targeting)
        {
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

            if (Projectile.localAI[1] < 1)
            {
                NightmareWizard.PlayBodyStarSoundEffect(Projectile.Center);
                initialVelLength = Projectile.velocity.Length();
                Projectile.localAI[1]++;
            }

            if (Main.rand.NextBool(5)) // happens 1/5 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 50, 50, DustID.Shadowflame, 0f, 0f, 200, default, 1f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
            }

            int targetIndex = -1;
            Vector2 center = Projectile.Center;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC compare = Main.npc[i];
                if (!compare.CanBeChasedBy())
                    continue;
                if (targetIndex == -1 || compare.DistanceSQ(center) < Main.npc[targetIndex].DistanceSQ(center))
                {
                    targetIndex = i;
                }
            }

            float homingRange = 2000;

            if (Helper.ValidIndexedTarget(targetIndex, Projectile, out NPC target, false))
            {
                if (Main.npc[targetIndex].Distance(center) <= homingRange)
                {
                    Projectile.localAI[0]++;
                    float homingStrength = Utils.Remap(Projectile.localAI[0], 1, 20, 0, .03f, false);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * initialVelLength, homingStrength);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //collision
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(10, 10); //burst of sparkles
                Sparkle.NewSparkle(Projectile.Center + Projectile.velocity, Main.rand.NextBool(3, 5) ? Color.LightSkyBlue : Color.MediumSlateBlue,
                    new Vector2(1, 1f), velocity, 40, new Vector2(2, 2));
            }
        }

        public override bool CanHitPvp(Player target)
        {
            return true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return true;
        }
        public override bool? CanCutTiles()
        {
            return null;
        }
    }
}