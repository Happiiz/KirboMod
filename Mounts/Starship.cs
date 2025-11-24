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
	public class Starship : ModMount
	{
		public override void SetStaticDefaults() {
			MountData.spawnDust = DustID.WhiteTorch;
			MountData.buff = ModContent.BuffType<Buffs.StarshipMount>();
			MountData.heightBoost = 20; //height of mount from ground
			MountData.flightTimeMax = int.MaxValue; //how long can fly
			MountData.fallDamage = 0f; //percent of fall damage received in mount
			MountData.usesHover = true; //hover like ufo
			MountData.runSpeed = 5f; //move speed reduced to what it would be because using DryCollision() dramatically increases
			MountData.dashSpeed = MountData.runSpeed * 3/4; //idk honestly but i use it to spawn dust if speed is past this threshold
			MountData.acceleration = 0.20f; //acceleration
			MountData.jumpHeight = 0; //how high it jumps
			MountData.jumpSpeed = 0; //how fast it jumps
			MountData.blockExtraJumps = true; //no accesory double jumps
			MountData.totalFrames = 1; //frames for animation
			int[] array = new int[MountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 14;
			}
			MountData.playerYOffsets = array; //player offset
			MountData.xOffset = -20; //x offset
			MountData.bodyFrame = 3; //which frame will player be in(3 is mount sitting)
			MountData.yOffset = 6; //y offset
			MountData.playerHeadOffset = 20; //offset of player head on map
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
			player.mount._flyTime = MountData.flightTimeMax;

			player.velocity.Y *= 0.92f; //limit vertical speed

            player.ignoreWater = true;
            player.DryCollision(true, true); //for stepping down tiles

			Lighting.AddLight(player.Center, TorchID.White);

            Rectangle rect = player.getRect();
			if (Math.Abs(player.velocity.X) > MountData.dashSpeed) //spawn dust if moving fast enough
			{
				Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.WhiteTorch, Scale: 1.5f, SpeedX: -player.velocity.X);
			}

			KirbPlayer kirbPlayer = player.GetModPlayer<KirbPlayer>();

            if (player.ItemAnimationActive) //in process of using item
			{
                kirbPlayer.starshipShootTimer++; //count up to save across uses
            }

            if (kirbPlayer.starshipShootTimer > 20) //after 1/3 of a second fire star horizontally forward
            {
                Vector2 velocity = Vector2.UnitX * player.direction * 50;
                Vector2 mountCenter = player.Center + velocity;
                Projectile.NewProjectile(player.GetSource_FromThis(), mountCenter, velocity, ModContent.ProjectileType<Projectiles.StarshipStar>(), 100, 5, player.whoAmI);

                kirbPlayer.starshipShootTimer = 0; //reset

            }
        }
	}
}