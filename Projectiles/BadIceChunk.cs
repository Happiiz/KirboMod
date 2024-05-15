using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class BadIceChunk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Cube");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            //spawning dust on bottom
            if (Projectile.velocity.Y == 0)
            {
                if (Projectile.ai[0] % 5 == 0) //make dust every 5 ticks
                {
                    Dust.NewDustPerfect(new Vector2(Projectile.Center.X + Projectile.direction * -20, Projectile.position.Y + 50), DustID.GemDiamond,
                        new Vector2(Projectile.velocity.X * -1, Main.rand.Next(-3, -1))); //Makes dust
                }
            }
            int mistInterval = 10;
            if (Main.hardMode)
            {
                mistInterval = 7;
                if (Main.expertMode)
                {
                    mistInterval = 5;
                }

            }
            if(MathF.Abs( Projectile.velocity.Y) > 1)
            {
                mistInterval /= 2;
            }
            if (Projectile.ai[0] % mistInterval == 0) //spawn mist every 10 ticks
            {
                int type = ModContent.ProjectileType<BadIceChunkMist>();
                Vector2 position = Projectile.Bottom - new Vector2(0, ContentSamples.ProjectilesByType[type].height + 8);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, Projectile.velocity * 0.01f, type,
                    Projectile.damage / 2, 2f);
            }

            //keep it from stopping
            if (Projectile.velocity.X > 0)
            {
                Projectile.velocity.X = GetIceChunkXVelocity(1);
            }
            else
            {
                Projectile.velocity.X = GetIceChunkXVelocity(-1);
            }

            //Gravity
            Projectile.velocity.Y += 0.3f;
            if(Main.expertMode && Main.hardMode)
            {
                Projectile.velocity.Y += 0.4f;//stronger gravity
            }
            if (Projectile.velocity.Y >= 12f && !Main.expertMode)
            {
                Projectile.velocity.Y = 12f;
            }

            ClimbTiles();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false; //don't die 
        }


        public static float GetIceChunkXVelocity(int direction)
        {
            float velX = (Main.expertMode ? 8 : 5) * direction;
            if (Main.hardMode)
                velX *= 2;
            return velX;
        }
        private void ClimbTiles()
        {
            bool climableTiles = false;

            for (int i = 0; i < 48; i++)
            {
                if (Projectile.direction == 1)
                {
                    //checks for tiles on right side of NPC
                    Tile tile = Main.tile[(new Vector2(Projectile.Right.X + 16, Projectile.position.Y + i)).ToTileCoordinates()];
                    climableTiles = WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType] || tile.IsHalfBlock;
                }
                else
                {
                    //checks for tiles on left side of NPC
                    Tile tile = Main.tile[(new Vector2(Projectile.Left.X - 16, Projectile.position.Y + i)).ToTileCoordinates()];
                    climableTiles = WorldGen.SolidOrSlopedTile(tile) || TileID.Sets.Platforms[tile.TileType] || tile.IsHalfBlock;
                }

                if (climableTiles || Projectile.velocity.X == 0)
                {
                    Projectile.tileCollide = false;

                    Projectile.velocity.Y = -2f;

                    break;
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false; //don't pass through platforms

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position); //crystal smash
            for (int i = 0; i < 15; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
                Vector2 speed = Main.rand.NextVector2Circular(2f, 2f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Flake>(), speed * 2, Scale: 1f); //Makes dust in a messy circle
                d.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}