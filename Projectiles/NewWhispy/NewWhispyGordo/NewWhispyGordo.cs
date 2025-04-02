using KirboMod.NPCs.NewWhispy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.NewWhispy.NewWhispyGordo
{
    public class NewWhispyGordo : ModProjectile //gordo projectile used by Whispy Woods
    {
        public override string Texture => "KirboMod/Projectiles/BouncyGordo";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }
        public override bool PreAI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Projectile.ai[0];
            }
            if (Projectile.ai[0] < 0)
                Projectile.ai[0]++;
            Projectile.scale = Utils.GetLerpValue(Projectile.localAI[0], Projectile.localAI[0] + 10, Projectile.ai[0], true);
            return Projectile.ai[0] >= 0;
        }
        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 9999;
                SoundEngine.PlaySound(NewWhispyBoss.ObjFallSFX, Projectile.Center);
            }
            Projectile.velocity.Y += 0.4f;//
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawSelf();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }
        public override void OnKill(int timeLeft) //when the projectile dies
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.BetterNextVector2Circular(8); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center + speed, DustID.Obsidian, speed, Scale: 2f); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }
    }
}