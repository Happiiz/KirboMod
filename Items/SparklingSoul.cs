using KirboMod.Projectiles;
using KirboMod.Projectiles.Marx;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
	public class SparklingSoul : ModItem
	{
		public override void SetStaticDefaults() 
		{
			ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 4; //go to *this* spot in boss spawn group
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 4, false));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() 
		{
			Item.width = 40;
			Item.height = 40;
			Item.value = Item.buyPrice(0, 0, 10, 0);
			Item.rare = ItemRarityID.Lime;
			Item.maxStack = 1;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.UseSound = SoundID.Item1;
			Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<FlyingSparklingSoul>();
        }

        public override bool CanUseItem(Player player)
        {
            //WIP

            bool anySoul = false;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.type == ModContent.ProjectileType<FlyingSparklingSoul>() && proj.active)
                {
                    anySoul = true;
                    break;
                }
            }

            if (NPC.AnyNPCs(ModContent.NPCType<NPCs.Marx.MarxBoss>()) || NPC.AnyNPCs(ModContent.NPCType<NPCs.Marx.Townie.MarxPrelude>()) || anySoul)
				return false;

            return true;
        }

        /*public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer) //if the player using the item is the client
            {
                if (Main.netMode != NetmodeID.MultiplayerClient) // If the player is not in multiplayer, spawn directly
                {
                    //NPC.SpawnOnPlayer(player.whoAmI, NPC.AnyNPCs(ModContent.NPCType<NPCs.Marx.MarxTownie.MarxPrelude>()));
                }
                else // If the player is in multiplayer, request a spawn
                {
                    //this will only work if NPCID.Sets.MPAllowedEnemies[type] is set in boss
                    //NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<NPCs.DarkMatter.DarkMatter>());
                }
                SoundEngine.PlaySound(SoundID.Roar, player.position);
            }
            return true;
        }*/

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.Y = player.Center.Y - 50;
            position.X = player.Center.X + player.direction * 50;

            velocity.X = 0;
            velocity.Y = -20;
        }

        /*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position.Y = player.Center.Y - 50;
            position.X = player.Center.X + player.direction * 50;

            velocity.X = 0;
            velocity.Y = -10;

            Projectile.NewProjectile(source, position, velocity, type, 0, 0, player.whoAmI, ai1: )

            return false;
        }*/

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White; // Makes it uneffected by light
        }

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();//the result is dark mirror
			recipe.AddIngredient(ModContent.ItemType<HeartMatter>(), 3);
			recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.SoulofFlight, 5);
            recipe.AddIngredient(ItemID.SoulofFright, 2);
            recipe.AddIngredient(ItemID.SoulofMight, 2);
            recipe.AddIngredient(ItemID.SoulofSight, 2);
            recipe.AddIngredient(ItemID.Ectoplasm, 10);
            recipe.AddTile(TileID.MythrilAnvil); //crafted at hardmode anvil
			recipe.Register(); //adds this recipe to the game
		}
    }
}