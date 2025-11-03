using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using KirboMod.Items.Armor.AbilityHats;

namespace KirboMod.Items.Armor.CharacterVanity.Hyness
{
    [AutoloadEquip(EquipType.Head)]
    public class HynessHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 5;
            Item.rare = ItemRarityID.Red;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<HeartMatter>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 3);
            recipe.AddIngredient(ItemID.BlueLunaticHood, 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class HynessRobes : ModItem
    {
        public override void Load()
        {
            //can't run on server
            if (Main.netMode == NetmodeID.Server)
                return;

            EquipLoader.AddEquipTexture(Mod, $"{Texture}_Legs", EquipType.Legs, this);
        }

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = FireAbilityHat.AbilityHatPrice * 5;
            Item.rare = ItemRarityID.Red;
            Item.vanity = true;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<HeartMatter>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 3);
            recipe.AddIngredient(ItemID.BlueLunaticRobe, 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}