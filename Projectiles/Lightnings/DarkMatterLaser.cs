using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Lightnings
{
    public class DarkMatterLaser : LightningProj
    {
        //Vector2 initalposition = new Vector2(0,0);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            outerColor = Color.Purple;
            innerColor = Color.Black;
            width = 20;
            Projectile.scale = 2;
            opacityFunction = OpacityFunction;
        }
        float OpacityFunction(float progress)
        {
            return 1;
        }
        //public override void AI()
        //{
        //	Projectile.ai[1]++;
        //          if (Projectile.ai[1] == 1)
        //          {
        //		initalposition = Projectile.Center;
        //	}

        //	SetLaserPosition();
        //}
        //public override bool PreDraw(ref Color lightColor)
        //{
        //	// We start drawing the laser
        //	DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Projectile.type].Value, initalposition,
        //	Projectile.velocity, 1, Projectile.damage, 0f, 1f, 2000f, Color.White, 0);
        //	return false;
        //}
        //public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        //{
        //	// Draws the laser 'body'
        //	for (float i = transDist; i <= Projectile.ai[0]; i += step)
        //	{
        //		var origin = start + i * unit;
        //		spriteBatch.Draw(texture, origin - Main.screenPosition,
        //			new Rectangle(0, 0, 14, 14), color, Projectile.velocity.ToRotation(),
        //			new Vector2(Projectile.width * .5f, Projectile.height * .5f), scale, 0, 0);
        //	}

        //	/*// Draws the laser end
        //	spriteBatch.Draw(texture, start + (projectile.ai[0] + step) * unit - Main.screenPosition,
        //		new Rectangle(0, projectile.frame == 1 ? 168 : 56, 16, 56), color, 0,
        //		new Vector2(16 * .5f, 56 * .5f), scale, 0, 0);*/
        //}

        //// Change the way of collision check of the projectile
        //public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        //{
        //	Vector2 unit = Projectile.velocity;
        //	float point = 0f;

        //	// Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
        //	// It will look for collisions on the given line using AABB
        //	return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), initalposition,
        //		initalposition + unit * Projectile.ai[0], 14, ref point);
        //}

        //private void SetLaserPosition()
        //{
        //	for (Projectile.ai[0] = 0; Projectile.ai[0] <= 2000f; Projectile.ai[0] += 5f)
        //	{
        //		var start = initalposition + Projectile.velocity * Projectile.ai[0]; 
        //		if (!Collision.CanHit(initalposition, 1, 1, start, 1, 1))
        //		{
        //			Projectile.ai[0] -= 5f; 
        //			break;
        //		}
        //	}
        //}

        //      public override void OnKill(int timeLeft) //Dust across laser
        //      {
        //          for (int i = 0; i < Projectile.ai[0]; i += 5)
        //	{
        //		Dust.NewDustPerfect(initalposition + Projectile.velocity * i, DustID.VilePowder);
        //	}
        //      }

        //      public override bool ShouldUpdatePosition() => false;
    }
}
