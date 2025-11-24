using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using KirboMod.Items.Weapons;

namespace KirboMod.Items.Armor.AbilityHats
{
	[AutoloadEquip(EquipType.Head)]
	public class FireAbilityHat : ModItem
	{
        public static int AbilityHatPrice => 500; //5 silver

		public override void SetStaticDefaults() 
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

		public override void SetDefaults() {
			Item.width = 10;
			Item.height = 10;
			Item.value = AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
			Item.vanity = true;
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddRecipeGroup("Gold", 5);
            recipe.AddIngredient(ModContent.ItemType<Fire>());
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class IceAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddRecipeGroup("Gold", 5);
            recipe.AddIngredient(ModContent.ItemType<Ice>());
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class TornadoAbilityHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; //amount needed to research
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddRecipeGroup("Gold", 5);
            recipe.AddIngredient(ModContent.ItemType<Tornado>());
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class PlasmaAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddRecipeGroup("Gold", 5);
            recipe.AddIngredient(ModContent.ItemType<Plasma>());
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class BombAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddIngredient(ItemID.Silk, 30);
            recipe.AddIngredient(ModContent.ItemType<Bomb>(), 100);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class FighterAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddIngredient(ItemID.Silk, 30);
            recipe.AddIngredient(ModContent.ItemType<FighterGlove>());
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class CutterAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddIngredient(RecipeGroupID.IronBar, 10);
            recipe.AddIngredient(ModContent.ItemType<Cutter>());
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class BeamAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 20);
            recipe.AddIngredient(ItemID.Silk, 30);
            recipe.AddIngredient(ModContent.ItemType<BeamStaff>());
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class WaterAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 10);
            recipe.AddRecipeGroup("Gold", 5);
            recipe.AddIngredient(ItemID.WaterBolt);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class LeafAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 10);
            recipe.AddRecipeGroup("Gold", 5);
            recipe.AddIngredient(ItemID.BladeofGrass);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class MirrorAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 10);
            recipe.AddIngredient(ItemID.Silk, 30);
            recipe.AddIngredient(ItemID.MagicMirror);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<Starbit>(), 10);
            recipe2.AddIngredient(ItemID.Silk, 30);
            recipe2.AddIngredient(ItemID.IceMirror);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class YoyoAbilityHat : ModItem
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
            Item.value = FireAbilityHat.AbilityHatPrice;
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Starbit>(), 10);
            recipe.AddIngredient(ItemID.Silk, 30);
            recipe.AddIngredient(ItemID.CorruptYoyo);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<Starbit>(), 10);
            recipe2.AddIngredient(ItemID.Silk, 30);
            recipe2.AddIngredient(ItemID.CrimsonYoyo);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}