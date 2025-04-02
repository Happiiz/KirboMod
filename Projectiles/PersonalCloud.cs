using Microsoft.Xna.Framework;
using Mono.Cecil;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class PersonalCloud : ModProjectile
    {
        private int animation = 1;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 2;
        }

        public override bool? CanCutTiles() //can cut foliage?
        {
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

            Projectile.Center = player.Center + new Vector2(0, -50); //stay above player

            if (player.whoAmI == Main.myPlayer && player.oldPosition != player.position) //if client player has changed position
            {
                Projectile.netUpdate = true; //sync projectile position change due to player
            }

            //equipping accesory
            if (player.GetModPlayer<KirbPlayer>().personalcloud)
            {
                Projectile.timeLeft = 2; //keep being on the brink of death until accesory is no longer equipped
            }

            //TARGETING
            int targetIndex = -1;
            const float attackRangeSQ = 700 * 700;
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
            if (targetIndex > 0 && Main.npc[targetIndex].DistanceSQ(center) <= attackRangeSQ) //ATTACK
            {
                animation = 2;

                if (Projectile.ai[0] >= 60) //shoot
                {
                    NPC target = Main.npc[targetIndex];

                    Vector2 distance = target.Center - Projectile.Center;
                    distance.Normalize();
                    distance *= 17;
                    int damage = 30;
                    if (Main.expertMode)
                    {
                        damage = 60;
                    }
                    if (Main.masterMode)
                    {
                        damage = 90;
                    }
                    if (ModLoader.TryGetMod("CalamityMod", out _))
                    {
                        damage = (int)(damage * 1.2f);
                    }
                    int direction = target.Center.X - center.X < 0 ? -1 : 1;
                    //damage = (int)player.GetTotalDamage(DamageClass.Generic).ApplyTo(damage);
                    bool crit = Main.rand.NextFloat() > player.GetTotalCritChance(DamageClass.Generic);
                    target.SimpleStrikeNPC(damage, direction, crit, 3, DamageClass.Generic, false, player.luck);
                    //Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, distance, ModContent.ProjectileType<PersonalCloudBeam>(), damage, 4, Projectile.owner, target.whoAmI);
                    LightningZap(target.Center, center);
                    Projectile.ai[0] = 0;
                }
            }

            //animation
            Projectile.frameCounter++;

            if (animation == 1)
            {
                if (Projectile.frameCounter < 30)
                {
                    Projectile.frame = 0;
                }
                else if (Projectile.frameCounter < 60)
                {
                    Projectile.frame = 1;
                }
                else
                {
                    Projectile.frameCounter = 0;
                }
            }
            if (animation == 2)
            {
                if (Projectile.frameCounter < 30)
                {
                    Projectile.frame = 2;
                }
                else if (Projectile.frameCounter < 60)
                {
                    Projectile.frame = 3;
                }
                else
                {
                    Projectile.frameCounter = 0;
                }
            }
        }
        static void LightningZap(Vector2 from, Vector2 to)
        {        
            Vector2 tangent = from - to;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();
            List<float> positions = [0];
            for (int i = 0; i < length / 64; i++)
            {
                positions.Add(Main.rand.NextFloat());
            }
            positions.Sort();
            const float Sway = 80;
            const float Jaggedness = 1 / Sway;
            Vector2 prevPoint = to;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {

                float pos = positions[i];
                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);
                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;
                float displacement = Main.rand.NextFloat(-Sway, Sway);
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;
                Vector2 point = to + pos * tangent + displacement * normal;
                DustLine(prevPoint, point, 5);
                prevPoint = point;
                prevDisplacement = displacement;
            }
            DustLine(prevPoint, from, 5);
        }
        static void DustLine(Vector2 from, Vector2 to, float dustSpacing)
        {
            float dist = from.Distance(to);
            for (float i = 0; i < 1; i += dustSpacing / dist)
            {
                if (Main.rand.NextFloat() > Utils.Remap(Main.maxDustToDraw, 0, Main.maxDust, .5f, 1))//less dust on lower quality
                    continue;
                Vector2 dustPos = Vector2.Lerp(from, to, i);
                Dust d = Dust.NewDustPerfect(dustPos, DustID.Electric, null, 0, VFX.RndElectricCol, .75f);
                d.scale += Main.rand.NextFloat(-.1f, .1f);
                d.position -= d.velocity * 3;
                d.noLight = true;
                d.noLightEmittence = true;
                d.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }
    }
}