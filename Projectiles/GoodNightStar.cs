using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class GoodNightStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Night Star");
            Main.projFrames[Projectile.type] = 1;
        }

        static int DefaultPenetrate => 4;
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.penetrate = DefaultPenetrate;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ArmorPenetration = 40;
            Projectile.alpha = 255;
        }
        int TargetIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        bool PVPStar { get => Projectile.ai[2] == 0; set => Projectile.ai[2] = value ? 0 : 1; }
        ref float InitialVelLength { get => ref Projectile.ai[1]; }
        public override void AI()
        {
            if (PVPStar && Projectile.timeLeft % 4 == 0)
            {
                Projectile.timeLeft--;
            }
            if (TargetIndex != -1 && Projectile.timeLeft <= 30 && !PVPStar)
            {
                Projectile.timeLeft = 31;
            }
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

            if (Main.rand.NextBool(5)) // happens 1/5 times
            {
                int dustnumber = Dust.NewDust(Projectile.position, 50, 50, DustID.Shadowflame, 0f, 0f, 200, default, 1f); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            float rangeSQ = 1500 * 1500;
            if(Projectile.penetrate != DefaultPenetrate)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * InitialVelLength, 0.1f);
                return;
            }
            if (!PVPStar)
            {
                if (!Helper.ValidIndexedTarget(TargetIndex, Projectile, out _, false))
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * InitialVelLength, 0.1f);
                    int closestNPC = -1;
                    Vector2 center = Projectile.Center;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC potentialTarget = Main.npc[i];
                        float distToClosestPointInPotentialTargetHitbox = center.DistanceSQ(potentialTarget.Hitbox.ClosestPointInRect(center));
                        if (distToClosestPointInPotentialTargetHitbox > rangeSQ)
                            continue;
                        bool notValidTarget = !Helper.ValidHomingTarget(potentialTarget, Projectile, false);
                        if (notValidTarget)
                            continue;
                        if ((closestNPC < 0 || closestNPC >= Main.maxNPCs) || center.DistanceSQ(potentialTarget.Hitbox.ClosestPointInRect(center)) < center.DistanceSQ(Main.npc[closestNPC].Hitbox.ClosestPointInRect(center)))
                            closestNPC = i;
                    }
                    TargetIndex = closestNPC;
                    Projectile.localAI[0] = 1;
                }
                if (Helper.ValidIndexedTarget(TargetIndex, Projectile, out NPC target, false))
                {
                    Projectile.localAI[0]++;
                    float homingStrength = Utils.Remap(Projectile.localAI[0], 1, 20, 0, .05f, false);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * InitialVelLength, homingStrength);
                }
                else
                {
                    TargetIndex = -1;
                }
            }
            else
            {
                if (TargetIndex < 0 || TargetIndex >= Main.maxPlayers || !Main.player[TargetIndex].InOpposingTeam(Main.player[Projectile.owner]))
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * InitialVelLength, 0.1f);
                    int closestPlayer = -1;
                    Vector2 center = Projectile.Center;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (i == Projectile.owner)
                            continue;
                        Player potentialTarget = Main.player[i];
                        float distToClosestPointInPotentialTargetHitbox = center.DistanceSQ(potentialTarget.Hitbox.ClosestPointInRect(center));
                        if (distToClosestPointInPotentialTargetHitbox > rangeSQ)
                            continue;
                        bool notValidTarget = potentialTarget.dead || !potentialTarget.active || !potentialTarget.InOpposingTeam(Main.player[Projectile.owner]);
                        if (notValidTarget)
                            continue;
                        if (closestPlayer < 0 || closestPlayer >= Main.maxPlayers || center.DistanceSQ(potentialTarget.Hitbox.ClosestPointInRect(center)) < center.DistanceSQ(Main.player[closestPlayer].Hitbox.ClosestPointInRect(center)))
                        {
                            closestPlayer = i;
                        }
                    }
                    TargetIndex = closestPlayer;
                }
                if (TargetIndex >= 0)
                {
                    Player target = Main.player[TargetIndex];
                    if (!target.dead && target.active && target.InOpposingTeam(Main.player[Projectile.owner]))
                    {
                        Projectile.localAI[0]++;
                        float homingStrength = Utils.Remap(Projectile.localAI[0], 1, 20, 0, .05f, false);
                        if (homingStrength > 1)
                            homingStrength = 1;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * InitialVelLength, homingStrength);
                    }
                    else
                    {
                        Projectile.localAI[0] = 1;
                        TargetIndex = -1;
                    }
                }
               
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * Projectile.Opacity; // Makes it uneffected by light
        }

        // This projectile uses additional textures for drawing
        public static Asset<Texture2D> StarTexture;

        public override bool PreDraw(ref Color lightColor) //blue "afterimage" thing
        {
            StarTexture = ModContent.Request<Texture2D>("KirboMod/Projectiles/BadStar");

            Texture2D star = StarTexture.Value;

            for (int i = 0; i <= 2; i++)
            {
                Main.EntitySpriteDraw(star,
                    new Vector2
               (
                        (Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f) - Projectile.velocity.X * (i * 1.5f),
                        (Projectile.position.Y - Main.screenPosition.Y + Projectile.height - star.Height * 0.5f + 2f) - Projectile.velocity.Y * (i * 1.5f)
                ),
                    new Rectangle(0, 0, star.Width, star.Height),
                    new Color(0, 0, 100, 0) * Projectile.Opacity,
                    Projectile.rotation,
                    star.Size() * 0.5f,
                    1f,
                    SpriteEffects.None,
                    0);
            }
            VFX.DrawProjWithStarryTrail(Projectile, new Color(173, 245, 255) * .15f, Color.White * .35f * Projectile.Opacity, default, Projectile.Opacity);
            return true;
        }
        public override bool CanHitPvp(Player target)
        {
            return PVPStar;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return !PVPStar;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}