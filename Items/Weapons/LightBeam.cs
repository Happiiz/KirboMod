using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Weapons
{
	public class LightBeam : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Light Beam Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Rains holy rays down on your opponents" +
				"\n'I'm with you in the dark...'"); */
			Item.staff[Item.type] = true; //staff not gun
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults()
		{
			Item.damage = 90;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 48;
			Item.height = 62;
			Item.useTime = 3; //lower than use animation to repeat projectiles
			Item.useAnimation = 60;
			Item.reuseDelay = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.value = Item.buyPrice(0, 0, 30, 5);
			Item.rare = ItemRarityID.Yellow;
			//item.UseSound = SoundID.Item92; //electrosphere launcher
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.LightBeamLaser>();
			Item.shootSpeed = 30f;
			Item.mana = 12;
		}
		int FindTarget(Vector2 position)
        {
			Rectangle areaToCheck = Utils.CenteredRectangle(position, new Vector2(400, 900));
			List<int> npcsInRect = new();
			int closestIndex = -1;
			for (int i = 0; i < Main.maxNPCs; i++)
            {
				if (!Main.npc[i].CanBeChasedBy(null, Main.npc[i].type == NPCID.HallowBoss && (Main.npc[i].ai[0] == 8 || Main.npc[i].ai[0] == 9)) || !Main.npc[i].Hitbox.Intersects(areaToCheck))
					continue;
				npcsInRect.Add(i);
            }
            for (int i = 0; i < npcsInRect.Count; i++)
            {
				if (closestIndex == -1 || Main.npc[closestIndex].DistanceSQ(position) > Main.npc[i].DistanceSQ(position))
					closestIndex = i;
            }
			return closestIndex;
        }
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			int target = FindTarget(Main.MouseWorld);
			if (target == -1)
				position = Main.MouseWorld + new Vector2(0, -1000);
			else 
				position = Main.npc[target].Center + new Vector2(0, -1000);		
			position.X += Main.rand.Next(-300, 300);
			Vector2 targetPos = target == -1 ? Main.MouseWorld : Main.npc[target].Center;		
			velocity = Vector2.Normalize(targetPos - position) * 30;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			SoundEngine.PlaySound(SoundID.Item92.WithVolumeScale(0.3f) with { MaxInstances = 0}, player.Center); //electrosphere launcher
			return true;
		}

        /*public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }*/

		//draw light
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("KirboMod/Items/Weapons/LightBeam_Glowmask").Value; //GlowMask (light)

            spriteBatch.Draw
            (
                texture,
                new Vector2
               (
                        Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                        Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height * 0.5f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                rotation,
                texture.Size() * 0.5f,
                1f, //size depends on size variable
                SpriteEffects.None,
                0f
            );
        }

        public override void AddRecipes()
		{
			Recipe recipe1 = CreateRecipe();//the result is gigantsword
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.LaserBeam>()); //Laser Beam Staff
			recipe1.AddIngredient(ItemID.RainbowRod); //Rainbow Rod
            recipe1.AddIngredient(ItemID.FairyQueenMagicItem); //Nightglow
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 5); //5 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}