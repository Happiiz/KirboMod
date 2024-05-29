using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class BadIceChunkMist : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Cube");
            Main.projFrames[Projectile.type] = 1;
        }

        public override string Texture => "KirboMod/NothingTexture";

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = IceChunkMistDuration;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }
        public static int IceChunkMistDuration
        {
            get
            {
                int result = 180;
                if (!Main.hardMode && !Main.expertMode)
                {
                    result = 50;
                }
                if (Main.getGoodWorld)
                {
                    result = 360;
                }
                return result;
            }
        }
        public override void AI()
        {


            if (!Main.hardMode)
            {
                if (Main.rand.NextFloat() < Utils.GetLerpValue(0, Main.maxDust, Main.maxDustToDraw) && Main.rand.NextBool(2,3))
                {
                    int dustnumber = Dust.NewDust(Projectile.position, 50, Projectile.height, ModContent.DustType<Dusts.Flake>(), 0f, 0f, 0, default, 1); //dust
                    Main.dust[dustnumber].velocity *= 0.3f;
                    Main.dust[dustnumber].noGravity = true;
                }
            }
            else if (Main.rand.NextBool(3))
            {
                int dustnumber = Dust.NewDust(Projectile.position + Projectile.velocity * 2, 50, Projectile.height, ModContent.DustType<Dusts.Flake>(), 0f, 0f, 0, default, 1); //dust
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            //Go down in prehardmode, accelerate up in hardmode
            if (Main.hardMode)
            {
                if (NPC.downedGolemBoss) //go up slightly slower to tighten the area more
                {
                    Projectile.velocity.Y -= .05f;
                    if (Projectile.velocity.Y < -20)
                        Projectile.velocity.Y = -20;
                }
                else
                {
                    Projectile.velocity.Y -= .1f;
                    if (Projectile.velocity.Y < -40)
                        Projectile.velocity.Y = -40;
                }
            }
            else
            {
                Projectile.velocity.Y = 2f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //don't die 
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't pass through platforms

            return true;
        }
    }
}