using KirboMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.WingedBow
{
	public class WingedSlash : ModProjectile
	{
        public int initialShootDirection;

		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Charged Star Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }
		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			DrawOriginOffsetX = -10;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 240; //4 seconds
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0, 20, 0f, 1f, true) * Utils.Remap(Projectile.localAI[0], 220, 240, 1f, 0f, true);

            int spinDuration = 50;

            int range1 = 10;
            int range2 = range1 + spinDuration;

            if (Projectile.localAI[0] > range1 && Projectile.localAI[0] <= range2)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Tau / spinDuration * -initialShootDirection);
            }
            else if (Projectile.localAI[0] > range2)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Tau / 600 * -initialShootDirection);
            }

            Projectile.localAI[0]++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); //impact
            return true; //collision

        }

        public static Asset<Texture2D> afterimage;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            afterimage = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = afterimage.Value;

            for (int k = 1; k < Projectile.oldPos.Length; k++) //start at 1 so not ontop of actual projectile
            {
                Vector2 drawOrigin = texture.Size() / 2;
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(-8f, Projectile.gfxOffY);

                Color color = Color.White * Projectile.Opacity;
                color.A = 128;//make it blend with the background a bit.
                color *= ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }
            
            return Projectile.DrawSelf(Color.White * Projectile.Opacity);
        }
    }
}