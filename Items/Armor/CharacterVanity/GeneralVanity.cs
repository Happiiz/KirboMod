using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using KirboMod.Items.Armor.AbilityHats;
using KirboMod.Tiles;

namespace KirboMod.Items.Armor.CharacterVanity
{
    [AutoloadEquip(EquipType.Head)]
    public class MetaKnightMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 4;
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class DededeRobes : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = FireAbilityHat.AbilityHatPrice * 4;
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddRecipeGroup("Gold", 10);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class BandanaDeeBandana : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 4;
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddIngredient(ItemID.Silk, 30);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class DarkMetaKnightMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 4;
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MetaKnightMask>());
            recipe.AddTile<DimensionMirror>();
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class TaranzaWig : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 4;
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            recipe.AddIngredient(ItemID.SpiderFang, 4);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class MasterCrown : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 6;
            Item.rare = ItemRarityID.Lime;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DarkMaterial>(), 5);
            recipe.AddRecipeGroup("Gold", 10);
            recipe.AddIngredient(ItemID.Emerald, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class DaroachHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 4;
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            recipe.AddIngredient(ItemID.Ruby, 5);
            recipe.AddIngredient(ItemID.Silk, 30);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class SusieHat : ModItem
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
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 100);
            recipe.AddIngredient(ModContent.ItemType<KrackoSpikeItem>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class AdeleineBeret : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 4;
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            recipe.AddIngredient(ItemID.Paintbrush);
            recipe.AddIngredient(ModContent.ItemType<CrystalShard>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class RibbonWig : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice * 4;
            Item.rare = ItemRarityID.LightPurple;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DreamEssence>(), 20);
            recipe.AddIngredient(ItemID.PixieDust, 10);
            recipe.AddIngredient(ModContent.ItemType<CrystalShard>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}