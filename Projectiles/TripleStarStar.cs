using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class TripleStarStar : ModProjectile
	{
        enum TripleStarBehaviourMode
        {
            CirclingPlayer,
            GoingForwards,
            ReturningToPlayer
        }

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star");
			Main.projFrames[Projectile.type] = 1;

            //for drawing afterimages and stuff alike
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 500;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.ignoreWater = true;

            //doesn't wait for no one!
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
		}
        static int FindTarget()
        {
            int target = -1;
            float maxRange = 400;
            Vector2 searchCenter = Main.MouseWorld;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (!Main.npc[i].CanBeChasedBy() || Main.npc[i].DistanceSQ(searchCenter) > maxRange * maxRange || target == -1 || Main.npc[i].DistanceSQ(searchCenter) > Main.npc[target].DistanceSQ(searchCenter))
                    continue;
                target = i;
            }
            return target;
        }
        public void Shoot()
        {
            BehaviourMode = TripleStarBehaviourMode.GoingForwards;
            Projectile.ai[0] = 0;
            if(Main.myPlayer == Projectile.owner)
            {
                int targetIndex = FindTarget();
                Vector2 targetPos = targetIndex == -1 ? Main.MouseWorld : Main.npc[targetIndex].Center;
                float velLength = MathF.Max((targetPos - Projectile.Center).Length() / 20, 50f);
                Projectile.velocity = Vector2.Normalize(targetPos - Projectile.Center) * velLength;
                Projectile.netUpdate = true;
            }
            SoundEngine.PlaySound(SoundID.Item9, Projectile.position); //star sound       
        }
        int AfterImgDrawCancelCount { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public bool AvailableForUse { get => BehaviourMode == TripleStarBehaviourMode.CirclingPlayer; }
        TripleStarBehaviourMode BehaviourMode { get => (TripleStarBehaviourMode)(int)Projectile.ai[1]; set => Projectile.ai[1] = (int)value; }
        public override void AI()
		{
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, 0.255f, 0.255f, 0f);
			Projectile.rotation += 0.3f; // rotates projectile
            int dustIndex = Dust.NewDust(Projectile.position, 50, 50, DustID.BlueTorch, Scale: 2f); //dust
            Main.dust[dustIndex].velocity *= 0.2f;
            Main.dust[dustIndex].noGravity = true;

            Projectile.ai[0]++;
            Vector2 targetPos = GetWeirdAsfPosOffsetForCirclingPlayer();
            float dist = Projectile.Distance(targetPos);
            float speed = Utils.Remap(dist, 200, 30, 60, player.velocity.Length() + 3f, true);
            float inertia = Utils.Remap(dist, 200, 30, 10, 1, true);
            switch (BehaviourMode)
            {
                case TripleStarBehaviourMode.CirclingPlayer:
                    AfterImgDrawCancelCount++;
                    if (AfterImgDrawCancelCount > Projectile.oldPos.Length)
                        AfterImgDrawCancelCount = Projectile.oldPos.Length;
                    speed = 70f;
                    inertia = MathHelper.Clamp(40f - player.velocity.Length() * 0.5f, 1, 1000);
                    break;
                case TripleStarBehaviourMode.GoingForwards:
                    AfterImgDrawCancelCount--;
                    if (AfterImgDrawCancelCount < 0)
                        AfterImgDrawCancelCount = 0;
                    if (Projectile.ai[0] >= 25)//time before returning to player
                    {
                        BehaviourMode = TripleStarBehaviourMode.ReturningToPlayer;
                    }
                    break;
                case TripleStarBehaviourMode.ReturningToPlayer:
                    AfterImgDrawCancelCount--;
                    if (AfterImgDrawCancelCount < 0)
                        AfterImgDrawCancelCount = 0;
                    if (Projectile.DistanceSQ(targetPos) < 16 * 16 * 3 * 3) //if at least less than 3 tile away from player
                    {
                        BehaviourMode = TripleStarBehaviourMode.CirclingPlayer;
                    }
                    break;
            }
            if (BehaviourMode != TripleStarBehaviourMode.GoingForwards)
            {
                Vector2 direction = targetPos - Projectile.Center; //start - end 																	
                direction.Normalize();
                direction *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
            }
        }
        Vector2 GetWeirdAsfPosOffsetForCirclingPlayer()
        {
            //this is copied from EoL's dash effect lol
            float offsetDistanceMultiplier = 200;//default value 200
            float timer = (float)Main.timeForVisualEffects / 60f * (Projectile.whoAmI % 2 * 2 - 1) + Projectile.whoAmI;
            Vector3 offset3D = Vector3.Transform(Vector3.Forward,
                Matrix.CreateRotationX((timer - 0.3f * Projectile.whoAmI * 1f) * 0.7f * MathF.Tau) *
                Matrix.CreateRotationY((timer - 0.8f * Projectile.whoAmI * 3f) * 0.7f * MathF.Tau) *
                Matrix.CreateRotationZ(timer * Projectile.whoAmI * 5 * 0.1f * MathF.Tau));
            offsetDistanceMultiplier += Utils.GetLerpValue(-1f, 1f, offset3D.Z, clamped: true) * 150f;
            offsetDistanceMultiplier = Utils.GetLerpValue(100, 150, offsetDistanceMultiplier, true) * 80f + 80f;
            Vector2 offset = new Vector2(offset3D.X, offset3D.Y) * offsetDistanceMultiplier;
            return Main.player[Projectile.owner].Center + offset.RotatedBy((float)Main.timeForVisualEffects / 180f * MathF.Tau) * 0.3f;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White; 
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Projectiles/TripleStarStarAfterimage").Value;

            // Redraw the projectile with the color not influenced by light
            int endCount = AfterImgDrawCancelCount;
            int startCount = Projectile.oldPos.Length - 1;
            if(BehaviourMode == TripleStarBehaviourMode.GoingForwards)
            {
                 startCount = Projectile.oldPos.Length - AfterImgDrawCancelCount;
                startCount = (int)MathHelper.Clamp(startCount, 0, Projectile.oldPos.Length - 1);
                endCount = 0; ;
            }
            for (int k = startCount; k >= endCount; k--)
            {                      
                Vector2 drawOrigin = new Vector2(25, 25);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                Color color = Color.DodgerBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            return true; //draw og
        }
    }
}