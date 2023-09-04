using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
namespace KirboMod.Projectiles
{
	public class CrystalShardProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Crystal Shard"); 
		}
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			DrawOffsetX = -21;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
			Projectile.alpha = 50;
		}
		int TargetIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
		ref float InitialVelLength { get => ref Projectile.ai[1]; }
        public override void OnSpawn(IEntitySource source)
        {
			InitialVelLength = Projectile.velocity.Length();
			TargetIndex = -1;
			Projectile.rotation = MathF.PI / 4;
		}
		public override void AI()
		{

			const float rangeSQ = 600 * 600;
			if(!Main.npc.IndexInRange(TargetIndex)|| !AIUtils.ValidHomingTarget(Main.npc[TargetIndex], Projectile))
            {
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * InitialVelLength, 0.1f);
				int closestNPC = -1;
				Vector2 center = Projectile.Center;
				for(int i = 0; i < Main.npc.Length; i++)
                {
					NPC npc = Main.npc[i];
					if (!AIUtils.ValidHomingTarget(npc, Projectile) || npc.DistanceSQ(center) > rangeSQ || !(Main.npc.IndexInRange(TargetIndex) && npc.DistanceSQ(center) < Main.npc[TargetIndex].DistanceSQ(center)))
						continue;
					closestNPC = i;
                }
				TargetIndex = closestNPC;
            }
            else
            {
				NPC target = Main.npc[TargetIndex];
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * InitialVelLength, 0.1f);
            }
			if (Main.rand.NextBool(5)) // happens 1/5 times
			{
				int dustnumber = Dust.NewDust(Projectile.position, 22, 22, ModContent.DustType<Dusts.RainbowSparkle>(), 0f, 0f, 200, default, 1f); //dust
				Main.dust[dustnumber].velocity *= 0.3f;
			}
		}

        public override void Kill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center); //crystal break

			for (int i = 0; i < 5; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ModContent.DustType<Dusts.RainbowSparkle>(), speed, 0, default, Scale: 1f); //Makes dust in a messy circle
			}
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), ModContent.DustType<Dusts.CrystalBit>(), speed, 0, default, Scale: 1f); //Makes dust in a messy circle
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = Projectile.MyTexture(out Vector2 origin, out var fx);

			Main.EntitySpriteDraw(tex, Projectile.Center, null, Color.White, Projectile.rotation, origin, Projectile.scale, fx);
			return false;
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White; // Makes it uneffected by light
		}
	}
}