﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Zero
{
	internal class BloodyTendrils : ModItem
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Bloody Tendrils");
			// Tooltip.SetDefault("'Feels and looks bad'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			/*
				this.noUseGraphic = true;
				this.damage = 0;
				this.knockBack = 7f;
				this.useStyle = 5;
				this.name = "Amethyst Hook";
				this.shootSpeed = 10f;
				this.shoot = 230;
				this.width = 18;
				this.height = 28;
				this.useSound = 1;
				this.useAnimation = 20;
				this.useTime = 20;
				this.rare = 1;
				this.noMelee = true;
				this.value = 20000;
			*/
			// Instead of copying these values, we can clone and modify the ones we want to copy
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.expert = true;
			Item.value = 5997; //half of 1, 9, 97
			Item.shootSpeed = 18f; // how quickly the hook is shot.
			Item.shoot = ModContent.ProjectileType<BloodyTendrilsProj>();
		}
	}


	//THE PROJECTILE ITSELF


	internal class BloodyTendrilsProj : ModProjectile
	{
        private static Asset<Texture2D> chainTexture;

        public override void Load()
        { // This is called once on mod (re)load when this piece of content is being loaded.
          // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
            chainTexture = ModContent.Request<Texture2D>("KirboMod/Items/Zero/BloodyTendrilsChain");
        }

        public override void Unload()
        { // This is called once on mod reload when this piece of content is being unloaded.
          // It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.
            chainTexture = null;
        }

        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Bloody Tendrils");
		}

		public override void SetDefaults() 
		{
			/*	this.netImportant = true;
				this.name = "Gem Hook";
				this.width = 18;
				this.height = 18;
				this.aiStyle = 7;
				this.friendly = true;
				this.penetrate = -1;
				this.tileCollide = false;
				this.timeLeft *= 10;
			*/
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
		}

		// Use this hook for hooks that can have multiple hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook
		public override bool? CanUseGrapple(Player player) 
		{
			int hooksOut = 0;
			for (int l = 0; l < 1000; l++) {
				if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type) {
					hooksOut++;
				}
			}
			if (hooksOut > 5) // This hook can have 6 hooks out.
			{
				return false;
			}
			return true;
		}

		// Return true if it is like: Hook, CandyCaneHook, BatHook, GemHooks
		//public override bool? SingleGrappleHook(Player player)
		//{
		//	return true;
		//}

		// Use this to kill oldest hook. For hooks that kill the oldest when shot, not when the newest latches on: Like SkeletronHand
		// You can also change the projectile like: Dual Hook, Lunar Hook
		//public override void UseGrapple(Player player, ref int type)
		//{
		//	int hooksOut = 0;
		//	int oldestHookIndex = -1;
		//	int oldestHookTimeLeft = 100000;
		//	for (int i = 0; i < 1000; i++)
		//	{
		//		if (Main.projectile[i].active && Main.projectile[i].owner == projectile.whoAmI && Main.projectile[i].type == projectile.type)
		//		{
		//			hooksOut++;
		//			if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
		//			{
		//				oldestHookIndex = i;
		//				oldestHookTimeLeft = Main.projectile[i].timeLeft;
		//			}
		//		}
		//	}
		//	if (hooksOut > 1)
		//	{
		//		Main.projectile[oldestHookIndex].Kill();
		//	}
		//}

		// Amethyst Hook is 300, Static Hook is 600
		public override float GrappleRange() 
		{
			return 500f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) 
		{
			numHooks = 2;
		}

		// default is 11, Lunar is 24
		public override void GrappleRetreatSpeed(Player player, ref float speed) 
		{
			speed = 20f;
		}

		public override void GrapplePullSpeed(Player player, ref float speed) 
		{
			speed = 15;
		}

		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) 
		{
			Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
			//float hangDist = 50f;
			grappleX += dirToPlayer.X/* * hangDist*/;
			grappleY += dirToPlayer.Y/* * hangDist*/;
		}

		public override bool PreDrawExtras() 
		{
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float projRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distance = directionToPlayer.Length();
			while (distance > 15f && !float.IsNaN(distance)) //draw only while 15 units away from player (and an actual number I guess)
			{
                directionToPlayer.Normalize();                   //get unit vector
                directionToPlayer *= chainTexture.Height();     // multiply by chain link length
                center += directionToPlayer;                   //update draw position
                directionToPlayer = playerCenter - center;    //update distance
				distance = directionToPlayer.Length();
				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                //Draw chain
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, projRotation,
                    chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
            // Stop vanilla from drawing the default chain
            return false;
		}
	}

	// Animated hook example
	// Multiple, 
	// only 1 connected, spawn mult
	// Light the path
	// Gem Hooks: 1 spawn only
	// Thorn: 4 spawns, 3 connected
	// Dual: 2/1 
	// Lunar: 5/4 -- Cycle hooks, more than 1 at once
	// AntiGravity -- Push player to position
	// Static -- move player with keys, don't pull to wall
	// Christmas -- light ends
	// Web slinger -- 9/8, can shoot more than 1 at once
	// Bat hook -- Fast reeling

}
