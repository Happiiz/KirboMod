using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class BuzzCutterProj : ModProjectile
	{
		private int lives = 8; //eight chances to bounce

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
			// DisplayName.SetDefault("Buzz Cutter");
		}
        public int NPCToStickTo { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
		Vector2 posOffsetToNPCCenter =Vector2.Zero;
        public override void SetDefaults()
		{
			Projectile.width = 73;
			Projectile.height = 73;
			DrawOffsetX = -24;
			DrawOriginOffsetY = -24;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 3600;
			Projectile.tileCollide = false; //inital so doesn't collide with tiles upon spawn
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true; //uses own immunity frames
			Projectile.extraUpdates = 3;//detect collision more often to more accurately get a collision point
			Projectile.localNPCHitCooldown = 6; //time before hit again, very short to really get the feeling that this grinds enemies
		}
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return AIUtils.CheckCircleCollision(targetHitbox, Projectile.Center, 60);
        }
        int amountOfTimesToGrindTarget = 30;
        public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.rotation -= 0.07f; // rotates projectile(in the teeth direction now)
			if (Projectile.ai[0] == 0)
				NPCToStickTo = -1;
			Projectile.ai[0]++;
			if(NPCToStickTo != -1)
            {
				Projectile.Center = Main.npc[NPCToStickTo].Center + posOffsetToNPCCenter;
            }
			if (Projectile.ai[0] >= 5)
            {
				Projectile.tileCollide = true; //now collide
			}
			if (Projectile.ai[0] >= 50) //not colliding with tiles
            {
				NPCToStickTo = -1;
                float speed = 20; //top speed(original shoot speed)
              
				float inertia = 2.5f; //acceleration and decceleration speed

				Vector2 direction = player.Center - Projectile.Center; //start - end 																	
				direction.Normalize();
				direction *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

				Rectangle box = Projectile.Hitbox;
				if (box.Intersects(player.Hitbox)) //if touching player
				{
					Projectile.Kill(); //KILL
				}
			}

			if (Projectile.ai[1] >= amountOfTimesToGrindTarget)
			{
				Projectile.ai[0] = 50;
				Projectile.ai[1] = -30;
			}
			if (Projectile.ai[1] < 0) //go up until 0
			{
                Projectile.ai[1]++;
            }

			if (lives <= 0) //enough bouncing
            {
				Projectile.Kill(); //KILL

                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, speed, Main.rand.Next(61, 63), Scale: 1f); //smoke
                }
            }
		}
        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(posOffsetToNPCCenter.X);
			writer.Write(posOffsetToNPCCenter.Y);
		}
        public override void ReceiveExtraAI(BinaryReader reader)
        {
			posOffsetToNPCCenter.X = reader.ReadSingle();
			posOffsetToNPCCenter.Y = reader.ReadSingle();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (Projectile.ai[1] >= 0 && Projectile.ai[1] < amountOfTimesToGrindTarget) //can grind again
			{
				Projectile.ai[1]++;
				Projectile.ai[0] = 39;
				if (NPCToStickTo == -1)
				{
					Projectile.velocity *= 0;
					NPCToStickTo = target.whoAmI;
					posOffsetToNPCCenter = Projectile.Center - target.Center;

					Projectile.netUpdate = true;
				}
			}

            //dust
            for (int i = 0; i < 3; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
            {
				Vector2 speed = Vector2.Normalize(posOffsetToNPCCenter).RotatedBy(MathF.PI / 2);
				speed = speed.RotatedByRandom(0.3f) *( 10 + Main.rand.NextFloat() * 5f);
                Dust d = Dust.NewDustPerfect(Projectile.Center - Vector2.Normalize(posOffsetToNPCCenter) * 55, DustID.Torch, speed, Scale: 2f);
                d.noGravity = false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			lives--;

			//dust
			for (int i = 0; i < 3; i++) //first semicolon makes inital statement once //second declares the conditional they must follow // third declares the loop
			{
				Vector2 speed = Main.rand.NextVector2Circular(5f, 5f); //circle
				Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed, Scale: 2f); //Makes dust in a messy circle
				d.noGravity = false;
			}
			//thump
			SoundEngine.PlaySound(SoundID.Item40, Projectile.Center); //sniper shot

			if (Projectile.velocity.X != oldVelocity.X) //bounce
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y) //bounce
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			Projectile.ai[0] = 5; //reset timer
			return false;
		}
        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = TextureAssets.Projectile[Type].Value;
			if(NPCToStickTo == -1)
            for (float i = -10; i < 11; i+= 1f/4f)//motion blur effect
            {
				Vector2 posOffset = Projectile.velocity * i * 0.5f - Main.screenPosition;
				float opacity = 1 - MathF.Abs(i) / 10;
				Main.EntitySpriteDraw(tex, Projectile.Center + posOffset, null, Projectile.GetAlpha(Color.White) * opacity * 0.125f, Projectile.rotation, tex.Size() / 2, Projectile.scale , SpriteEffects.None);
                }
            else
            {
				Vector2 grindOffset = -Vector2.Normalize(posOffsetToNPCCenter) * 55;
				Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
				Main.EntitySpriteDraw(VFX.GlowBall, Projectile.Center - Main.screenPosition + grindOffset, null, Color.OrangeRed with { A = 0 }, 0, VFX.GlowBall.Size() / 2, Projectile.scale * 3f, SpriteEffects.None);
				Main.EntitySpriteDraw(VFX.GlowBall, Projectile.Center - Main.screenPosition + grindOffset, null, Color.Yellow with { A = 0 }, 0, VFX.GlowBall.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None);
				Main.EntitySpriteDraw(VFX.GlowBall, Projectile.Center - Main.screenPosition + grindOffset, null, Color.White with { A = 0 }, 0, VFX.GlowBall.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None);

			}
			return false;
        }
    }
}