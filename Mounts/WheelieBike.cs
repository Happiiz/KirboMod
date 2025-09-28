using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Mounts
{
	public class WheelieBike : ModMount
	{
		public override void SetStaticDefaults() {
			MountData.spawnDust = DustID.Smoke;
			MountData.buff = ModContent.BuffType<Buffs.WheelieBikeMount>();
			MountData.heightBoost = 55; //how high is the mount and item player is holding
			MountData.flightTimeMax = 0; //how long can fly
			MountData.fatigueMax = 0; //maximum speed for fly timer before falling
			MountData.fallDamage = 0f; //percent of fall damage received in mount
			MountData.runSpeed = 20f; //move speed
			MountData.dashSpeed = 20f; //idk honestly
			MountData.acceleration = 0.04f; //acceleration
			MountData.jumpHeight = 15; //how high it jumps
			MountData.jumpSpeed = 8f; //how fast it jumps
			MountData.blockExtraJumps = false; //no accesory double jumps
			MountData.totalFrames = 1; //frames for animation

			int[] array = new int[MountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 36;
			}
			MountData.playerYOffsets = array; //player offset
			MountData.bodyFrame = 3; //which frame will player be in(3 is mount sitting)
			MountData.yOffset = 20; //y offset
			MountData.playerHeadOffset = 60; //offset of player head on map

			if (Main.netMode != NetmodeID.Server) 
			{
				MountData.textureWidth = MountData.frontTexture.Width(); //get front texture (mountname_front)
				MountData.textureHeight = MountData.frontTexture.Height(); //get front texture (mountname_front)
			}
		}

		public override void UpdateEffects(Player player) 
		{
			Vector2 bottom = player.Center + Vector2.UnitY * 50;

			player.fullRotationOrigin = player.mount.Origin + new Vector2(-22, 100);

            player.fullRotation = player.velocity.X * 0.03f;

            if (Math.Abs(player.velocity.X) > MountData.runSpeed - 3f && player.velocity.Y == 0)
			{
				Vector2 velocity = new Vector2(-player.velocity.X * 0.1f, -5 * Main.rand.NextFloat(2));

				Vector2 velocity2 = new Vector2(-player.velocity.X * 0.1f, -5 * Main.rand.NextFloat(2));

                Dust.NewDustPerfect(bottom, DustID.Torch, velocity, Scale: 2f);

                Dust.NewDustPerfect(bottom, DustID.Smoke, velocity2, Scale: 2f);
            }
		}
	}
}