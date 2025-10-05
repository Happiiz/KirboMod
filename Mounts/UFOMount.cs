using KirboMod.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace KirboMod.Mounts
{
	public class UFOMount : ModMount
	{
		public override void SetStaticDefaults() {
			MountData.spawnDust = ModContent.DustType<Dusts.RainbowSparkle>();
			MountData.buff = ModContent.BuffType<UFOMountBuff>();

			MountData.heightBoost = 0; //how high is the mount from the ground

			MountData.flightTimeMax = 9999; //how long can fly
			MountData.fatigueMax = 0; //maximum speed for fly timer before falling
			MountData.fallDamage = 0f; //percent of fall damage received in mount

			MountData.usesHover = true; //hover like ufo

            MountData.runSpeed = 3f; //move speed
            MountData.dashSpeed = 3f; //speed of mount while dashing
            MountData.acceleration = 0f; //acceleration

			MountData.jumpHeight = 0; //how high it jumps
			MountData.jumpSpeed = 0f; //how fast it jumps
			MountData.blockExtraJumps = true; //no accesory double jumps
			MountData.constantJump = true; //if can hold jump

			MountData.totalFrames = 6; //frames for animation

			int[] array = new int[MountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 10;
			}
			MountData.playerYOffsets = array; //player offset
			MountData.xOffset = 0; //x offset
			MountData.yOffset = 0; //y offset
            MountData.bodyFrame = 5; //which frame will player be in (3 is mount sitting)
            MountData.playerHeadOffset = 0; //offset of player head on map

			if (Main.netMode != NetmodeID.Server) 
			{
				MountData.textureWidth = MountData.frontTexture.Width(); //get front texture (mountname_front)
				MountData.textureHeight = MountData.frontTexture.Height(); //get front texture (mountname_front)
			}
			
			/*MountData.standingFrameCount = 6;
			MountData.standingFrameDelay = 5;
			MountData.standingFrameStart = 0;

			MountData.inAirFrameCount = MountData.standingFrameCount;
			MountData.inAirFrameDelay = MountData.standingFrameDelay;
			MountData.inAirFrameStart = MountData.standingFrameStart;

			MountData.idleFrameCount = MountData.standingFrameCount;
			MountData.idleFrameDelay = MountData.standingFrameDelay;
			MountData.idleFrameStart = MountData.standingFrameStart;
			MountData.idleFrameLoop = true;

			MountData.swimFrameCount = MountData.standingFrameCount;
			MountData.swimFrameDelay = MountData.standingFrameDelay;
			MountData.swimFrameStart = MountData.standingFrameStart;

            MountData.runningFrameCount = MountData.standingFrameCount;
            MountData.runningFrameDelay = MountData.standingFrameDelay;
            MountData.runningFrameStart = MountData.standingFrameStart;

            MountData.flyingFrameCount = MountData.standingFrameCount;
            MountData.flyingFrameDelay = MountData.standingFrameDelay;
            MountData.flyingFrameStart = MountData.standingFrameStart;

			MountData.dashingFrameCount = MountData.standingFrameCount;
			MountData.dashingFrameCount = MountData.standingFrameDelay;
            MountData.dashingFrameStart = MountData.standingFrameStart;*/
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
			skipDust = true;

			for (int i = 0; i < 30; i++)
			{
				Rectangle mountArea = Utils.CenteredRectangle(player.MountedCenter, new Vector2(MountData.textureWidth, MountData.textureHeight / MountData.totalFrames));

				Dust.NewDust(mountArea.TopLeft(), mountArea.Width, mountArea.Height, MountData.spawnDust, Scale: 0.5f);
            }
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            skipDust = true;

            for (int i = 0; i < 30; i++)
            {
                Rectangle mountArea = Utils.CenteredRectangle(player.MountedCenter, new Vector2(MountData.textureWidth, MountData.textureHeight / MountData.totalFrames));

                Dust.NewDust(mountArea.TopLeft(), mountArea.Width, mountArea.Height, MountData.spawnDust, Scale: 0.5f);
            }
        }

        public override void UpdateEffects(Player player) 
		{
			player.ignoreWater = true;
            player.blockExtraJumps = true;

            player.velocity *= 0.01f; //stop velocity completely if not moving

            if (player.controlDown)
			{
				player.velocity.Y = MountData.runSpeed;
			}
            if (player.controlUp)
            {
                player.velocity.Y = -MountData.runSpeed;
            }
            if (player.controlRight)
            {
                player.velocity.X = MountData.runSpeed;
				player.direction = 1;
            }
            if (player.controlLeft)
            {
                player.velocity.X = -MountData.runSpeed;
                player.direction = -1;
            }

			player.blockRange += 10;

            KirbPlayer kplr = player.GetModPlayer<KirbPlayer>();

            if (kplr.ufoMountShootTimer >= 120)
            {
                ShootLaser(player);
                kplr.ufoMountShootTimer = 0;
            }

            player.DryCollision(true, true); //doubles speed for some reason
        }

        void ShootLaser(Player player) //referenced from UFO and Personal Cloud code
        {
            //targeting
            int targetIndex = -1;
            const float attackRangeSQ = 1000 * 1000;
            Vector2 center = player.MountedCenter + Vector2.UnitY * 40;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC compare = Main.npc[i];
                if (!compare.CanBeChasedBy())
                    continue;
                if (targetIndex == -1 || compare.DistanceSQ(center) < Main.npc[targetIndex].DistanceSQ(center))
                {
                    targetIndex = i;
                }
            }

            if (targetIndex != -1)
            {
                if (Main.npc[targetIndex].DistanceSQ(center) <= attackRangeSQ && Main.npc[targetIndex].active)
                {
                    NPC target = Main.npc[targetIndex];

                    float vel = 20;

                    //predicts player movement
                    Utils.ChaseResults result = Utils.GetChaseResults(center, vel, target.Center, target.velocity);
                    result.ChaserVelocity = result.InterceptionHappens ? result.ChaserVelocity : (Vector2.Normalize(target.velocity) * vel);

                    float projMaxUpdates = ContentSamples.ProjectilesByType[ModContent.ProjectileType<Projectiles.UFOLaser>()].MaxUpdates;
                    Particles.Ring.ShotRing(center, Color.Red, result.ChaserVelocity);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), center, result.ChaserVelocity / projMaxUpdates, ModContent.ProjectileType<Projectiles.UFOMountLaser>(), 100, 7, player.whoAmI);
                    }
                }
            }
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            if (mountedPlayer.mount._frameCounter++ >= 5)
            {
                mountedPlayer.mount._frame++;
                mountedPlayer.mount._frameCounter = 0;
            }
            if (mountedPlayer.mount._frame > MountData.totalFrames - 1)
            {
                mountedPlayer.mount._frame = 0;
            }

			return false;
        }
	}
}