using KirboMod.NPCs.Marx.Townie;
using KirboMod.Projectiles;
using KirboMod.Projectiles.Marx;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
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

            if (NPC.AnyNPCs(ModContent.NPCType<NPCs.Marx.MarxBoss>()) || NPC.AnyNPCs(ModContent.NPCType<MarxPrelude>()) || anySoul)
				return false;

            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (!DownedBossSystem.downedMarxBoss)
            {
                return null;
            }

            //just spawns marx regularly

            int index = -1;

            if (Main.netMode != NetmodeID.MultiplayerClient) // If not a client
            {
                index = NPC.NewNPC(Item.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y - 300, ModContent.NPCType<MarxPrelude>());
            }

            if (index != -1)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
            }

            return true;
        }

        public override bool CanShoot(Player player)
        {
            if (DownedBossSystem.downedMarxBoss)
            {
                return false;
            }

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.Y = player.Center.Y - 50;
            position.X = player.Center.X + player.direction * 50;

            velocity.X = 0;
            velocity.Y = -20;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;

            if (DownedBossSystem.downedMarxBoss)
            {
                tooltips.Add(new TooltipLine(Mod, "TooltipLine1", Language.GetTextValue("Mods.KirboMod.Items.SparklingSoul.Summon")));
            }
        }

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