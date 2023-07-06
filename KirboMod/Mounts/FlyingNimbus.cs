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
	public class FlyingNimbus : ModMount
	{
		public override void SetStaticDefaults() {
			MountData.spawnDust = DustID.SandstormInABottle;
			MountData.buff = ModContent.BuffType<Buffs.FlyingNimbusMount>();
			MountData.heightBoost = 55; //how high is the mount and item player is holding
			MountData.flightTimeMax = 20; //how long can fly
			MountData.fatigueMax = 80; //maximum speed for fly timer before falling
			MountData.fallDamage = 0f; //percent of fall damage received in mount
			MountData.usesHover = true; //hover like ufo
			MountData.runSpeed = 8f; //move speed
			MountData.dashSpeed = 8f; //idk honestly
			MountData.acceleration = 0.16f; //acceleration
			MountData.jumpHeight = 1; //how high it jumps
			MountData.jumpSpeed = 8f; //how fast it jumps
			MountData.blockExtraJumps = true; //no accesory double jumps
			MountData.totalFrames = 1; //frames for animation
			//mountData.constantJump = true; //can hold jump I guess
			int[] array = new int[MountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 45;
			}
			MountData.playerYOffsets = array; //player offset
			MountData.xOffset = 0; //x offset
			MountData.bodyFrame = 3; //which frame will player be in(3 is mount sitting)
			MountData.yOffset = 12; //y offset
			MountData.playerHeadOffset = 60; //offset of player head on map
			if (Main.netMode != NetmodeID.Server) 
			{
				MountData.textureWidth = MountData.frontTexture.Width(); //get front texture (mountname_front)
				MountData.textureHeight = MountData.frontTexture.Height(); //get front texture (mountname_front)
			}
			
			//animation stuff
			MountData.standingFrameCount = 1;
			MountData.standingFrameDelay = 0;
			MountData.standingFrameStart = 0;
			MountData.runningFrameCount = 1;
			MountData.runningFrameDelay = 0;
			MountData.runningFrameStart = 0;
			MountData.flyingFrameCount = 1;
			MountData.flyingFrameDelay = 0;
			MountData.flyingFrameStart = 0;
			MountData.inAirFrameCount = 1;
			MountData.inAirFrameDelay = 0;
			MountData.inAirFrameStart = 0;
			MountData.idleFrameCount = 1;
			MountData.idleFrameDelay = 0;
			MountData.idleFrameStart = 0;
			MountData.idleFrameLoop = true;
			MountData.swimFrameCount = 1;
			MountData.swimFrameDelay = 0;
			MountData.swimFrameStart = 0;
		}

		public override void UpdateEffects(Player player) 
		{
			Rectangle rect = player.getRect();
			// This code spawns some dust if we are moving fast enough.
			if ((Math.Abs(player.velocity.X) > 6f) || (Math.Abs(player.velocity.Y) > 6f)) //gets absolute value
			{
				Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.SandstormInABottle);
			}
		}
	}
}