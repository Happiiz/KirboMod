using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Prefixes.PrefixLegacy;

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
			Item.damage = 40;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.width = 48;
			Item.height = 62;
			Item.useTime = 5;
			Item.useAnimation = Item.useTime;
			//Item.reuseDelay = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.value = Item.buyPrice(0, 0, 30, 5);
			Item.rare = ItemRarityID.Yellow;
			//item.UseSound = SoundID.Item92; //electrosphere launcher
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.LightBeamLaser>();
			Item.shootSpeed = 30f;
			Item.mana = 15;
		}
		int FindTarget(Vector2 position)
        {
			Rectangle areaToCheck = Utils.CenteredRectangle(position, new Vector2(200, 900));
			List<int> npcsInRect = new();
			int closestIndex = -1;
			for (int i = 0; i < Main.maxNPCs; i++)
            {
				if (!Main.npc[i].CanBeChasedBy(null, Main.npc[i].type == NPCID.HallowBoss && (Main.npc[i].ai[0] == 8 || Main.npc[i].ai[0] == 9)) || !Main.npc[i].Hitbox.Intersects(areaToCheck))
					continue;
				npcsInRect.Add(i); //adds the ID of the NPC as an element at the end of the list (highest index)
            }
            for (int i = 0; i < npcsInRect.Count; i++)
            {
				if (closestIndex == -1 || Main.npc[closestIndex].DistanceSQ(position) > Main.npc[i].DistanceSQ(position))
					closestIndex = npcsInRect.ElementAt(i); //get the ID of the NPC at the specified index
            }
			return closestIndex;
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			SoundEngine.PlaySound(SoundID.Item92.WithVolumeScale(0.3f) with { MaxInstances = 0}, player.Center); //electrosphere launcher

			//spawn projectile manually so velocity doesn't interfere with hold direction (make sure staff doesn't point down)

            int target = FindTarget(Main.MouseWorld);

            if (target == -1)
                position = Main.MouseWorld;
            else
                position = Main.npc[target].Center;
            position.X += Main.rand.Next(-300, 300);
            position.Y -= 1000;
            Vector2 targetPos = target == -1 ? Main.MouseWorld : Main.npc[target].Center;
            velocity = Vector2.Normalize(targetPos - position) * 30;

			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
		}

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
			Recipe recipe1 = CreateRecipe();
			recipe1.AddIngredient(ModContent.ItemType<Items.Weapons.LaserBeam>()); //Laser Beam Staff
			recipe1.AddIngredient(ItemID.RainbowRod); //Rainbow Rod
            recipe1.AddIngredient(ItemID.FairyQueenMagicItem); //Nightglow
			recipe1.AddIngredient(ModContent.ItemType<Items.Starbit>(), 100); //100 starbits
			recipe1.AddIngredient(ModContent.ItemType<Items.RareStone>(), 2); //2 rare stones
			recipe1.AddTile(TileID.MythrilAnvil); //crafted at mythril/orichalcum anvil
			recipe1.Register(); //adds this recipe to the game
		}
	}
}