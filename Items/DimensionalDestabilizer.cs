using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items
{
    public class DimensionalDestabilizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            Item.staff[Type] = true; //hold like staff
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.value = Item.buyPrice(0, 0, 2, 50);
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 1;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        static bool AnyProjs(int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == type)
                    return true;
            }
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            Point mouselocation = Main.MouseWorld.ToTileCoordinates();

            List<int> midbossIDs = [ModContent.NPCType<NPCs.MidBosses.Bonkers>(), ModContent.NPCType<NPCs.MidBosses.MrFrosty>()
            ,ModContent.NPCType<NPCs.MidBosses.Batafire>()];


            bool usableLocation = !player.ZoneRockLayerHeight && !player.ZoneDirtLayerHeight;

            for (int i = 0; i < midbossIDs.Count; i++)
            {
                if (NPC.AnyNPCs(midbossIDs[i]))
                {
                    return false;
                }
            }

            if (AnyProjs(ModContent.ProjectileType<MidbossRift>())
                     || player.CountItem(ModContent.ItemType<Starbit>()) < 25
                     || WorldGen.SolidOrSlopedTile(Main.tile[mouselocation.X, mouselocation.Y])
                     || !usableLocation)
            {
                return false;
            }

            return true;
        }
        public override bool? UseItem(Player player)
        {

            for (int i = 0; i < 25; i++)
            {
                player.ConsumeItem(ModContent.ItemType<Starbit>());
            }

            if (player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(Item.GetSource_FromThis(), Main.MouseWorld, default, ModContent.ProjectileType<MidbossRift>(), -1, 0, player.whoAmI, ai1: 1);
            }
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//the result is dimen destabilizer
            recipe.AddIngredient(ItemID.FallenStar, 5); //5 fallen stars
            recipe.AddIngredient(ItemID.MeteoriteBar, 20); //20 meteorite bars
            recipe.AddTile(TileID.Anvils); //crafted at anvil
            recipe.Register(); //adds this recipe to the game
        }
    }
}