using KirboMod.Projectiles.Marx.IceBombFrag;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.Marx.IceBomb
{
    public class MarxIceBomb : ModProjectile
    {
        public static SoundStyle SpitSFX => new("KirboMod/Sounds/NPC/Marx/IceBombSpit");
        public static SoundStyle IceBombBreakSFX => new("KirboMod/Sounds/NPC/Marx/IceBombBreak");
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;

        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 60;
            Projectile.extraUpdates = 1;//ensure collision works fine at high speeds
            Projectile.scale = 1f / 7f;
        }
        int TargetPlayerIndex => (int)Projectile.ai[0];
        public static float Gravity => 0.1f;
        public override void AI()
        {
            Projectile.velocity.Y += Gravity;
            Player target = Main.player[TargetPlayerIndex];
            if (Projectile.Center.Y >= target.Center.Y)//if below player or leveled with played center
            {
                SoundEngine.PlaySound(IceBombBreakSFX, Projectile.Center);
                Projectile.Kill();
            }
            Projectile.localAI[0]++;
            Projectile.scale = Helper.RemapEased(Projectile.localAI[0] + 1, 0, 7, 0, 1, Easings.EaseOutSquare);

        }
        public override void OnKill(int timeLeft)
        {
            SpawnFrag();
        }
        /// <summary>
        /// use this one for the boss
        /// </summary>
        public static void SpawnBombsForEveryPlayerAndPlaySFX(NPC marx, int damage)
        {
            SoundEngine.PlaySound(SpitSFX, marx.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            foreach (Player plr in Main.ActivePlayers)
            {
                //if within 200 tiles of marx
                if (plr.Distance(marx.position) < 16 * 200)
                {
                    SpawnBombForPlayer(marx, damage, plr.whoAmI);
                }
            }
        }
        /// <summary>
        /// use this if you want the bomb attack to only attack 1 player in multiplayer
        /// DOESN'T PLAY SFX
        /// </summary>
        public static void SpawnBombForPlayer(NPC marx, int damage, int target)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            Projectile.NewProjectile(marx.GetSource_FromAI(), marx.Center + marx.velocity * 2f, Vector2.Zero, ModContent.ProjectileType<MarxIceBomb>(), damage, 0f, -1, target);
        }
        private void SpawnFrag()
        {

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            int id = ModContent.ProjectileType<MarxIceBombFrag>();
            int fragCountPerSide = Main.getGoodWorld ? 5 : Main.expertMode ? 3 : 1;
            float spread = (fragCountPerSide - 1) * 0.5f;
            float shootSpeed = 17;
            if (Main.expertMode)
            {
                shootSpeed *= 1.3f;
            }
            if (Main.getGoodWorld)
            {
                shootSpeed *= 1.3f;
            }

            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = 0; j < fragCountPerSide; j++)
                {
                    //avoid div by 0 inside remap function
                    float angle = fragCountPerSide == 1 ? 0 : Utils.Remap(j, 0, fragCountPerSide - 1, -spread / 2f, spread / 2f);
                    Vector2 vel = new Vector2(i * shootSpeed, 0).RotatedBy(angle);
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, vel, id, Projectile.damage, 0f, -1, TargetPlayerIndex, j * i);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawSelf(lightColor);
        }
    }
}
