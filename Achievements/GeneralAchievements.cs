using KirboMod.Items;
using KirboMod.Items.RainbowSword;
using KirboMod.Items.Weapons;
using KirboMod.NPCs;
using KirboMod.NPCs.MidBosses;
using KirboMod.NPCs.NewWhispy;
using KirboMod.NPCs.PlasmaWisp;
using KirboMod.NPCs.Twister;
using System.Collections.Generic;
using Terraria;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace KirboMod.Achievements;

public class MidBossAchievement : ModAchievement
{
	public override void SetStaticDefaults() {
		//slayer is default achievement, but put code here anyway for referencial purposes
		Achievement.SetCategory(AchievementCategory.Slayer);

        int[] midBosses = { ModContent.NPCType<Bonkers>(), ModContent.NPCType<MrFrosty>(), ModContent.NPCType<Batafire>() };

		AddNPCKilledCondition(midBosses);
    }

    public override Position GetDefaultPosition() => new After("MASTERMIND");

    public override IEnumerable<Position> GetModdedConstraints() //method used for ordering modded achievements adjacent to other modded achievements
    {
        yield return new After(ModContent.GetInstance<KrackoAchievement>());
    }
}

public class EnemiesAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Slayer);

        //have to kill every hostile("can technically hurt you but won't do so purposefully" for some) NPC in the mod

        int[] allNPCs = { ModContent.NPCType<WaddleDee>(), ModContent.NPCType<WaddleDoo>(), ModContent.NPCType<ParosolDee>(), 
            ModContent.NPCType<BladeKnight>(), ModContent.NPCType<SirKibble>(), 
            ModContent.NPCType<BrontoBurt>(), ModContent.NPCType<Cappy>(),
            ModContent.NPCType<PoppyBrosJr>(), ModContent.NPCType<Chilly>(), ModContent.NPCType<BurningLeo>(), 
            ModContent.NPCType<BroomHatter>(), ModContent.NPCType<Kabu>(), ModContent.NPCType<KnuckleJoe>(), ModContent.NPCType<Scarfy>(),
            ModContent.NPCType<Birdon>(), ModContent.NPCType<BioSpark>(), ModContent.NPCType<PlasmaWisp>(), ModContent.NPCType<Twister>(),
            ModContent.NPCType<Wheelie>(), ModContent.NPCType<Bomber>(), ModContent.NPCType<UFO>(),
            ModContent.NPCType<Bonkers>(), ModContent.NPCType<MrFrosty>(), ModContent.NPCType<Batafire>()};

        AddManyNPCKilledCondition(allNPCs);
    }

    public override Position GetDefaultPosition() => new After("GELATIN_WORLD_TOUR");
}

public class RareStoneAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Collector);

        int[] rareStone = { ModContent.ItemType<RareStone>(), ModContent.ItemType<TreasureStone>(), ModContent.ItemType<MysticalStone>() };

        AddItemPickupCondition(rareStone);
    }

    public override Position GetDefaultPosition() => new After("HEAD_IN_THE_CLOUDS");
}

public class EvoWeaponAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Collector);

        //second tiers
        int[] evoWeapons = { ModContent.ItemType<VolcanoFire>(), ModContent.ItemType<FrostyIce>(), ModContent.ItemType<LaserBeam>(),
        ModContent.ItemType<FleurTornado>(), ModContent.ItemType<ToyHammer>(), ModContent.ItemType<HardenedFighter>(),
        ModContent.ItemType<GigantSword>(), ModContent.ItemType<ChainBomb>(), ModContent.ItemType<ChakramCutter>(),
        ModContent.ItemType<ClutterNeedle>(), ModContent.ItemType<HunterArcherBow>(), ModContent.ItemType<NobleRangerGun>(),

        //third tiers
        ModContent.ItemType<DragonFire>(), ModContent.ItemType<BlizzardIce>(), ModContent.ItemType<LightBeam>(),
        ModContent.ItemType<StormTornado>(), ModContent.ItemType<WildHammer>(), ModContent.ItemType<MetalFighter>(),
        ModContent.ItemType<MetaKnightSword>(), ModContent.ItemType<HomingBomb>(), ModContent.ItemType<BuzzCutter>(),
        ModContent.ItemType<CrystalNeedle>(), ModContent.ItemType<CyborgArcherBow>(), ModContent.ItemType<SpaceRangerGun>(),

        //extra
        ModContent.ItemType<MaskedHammer>(), ModContent.ItemType<TripleStar>()};


        AddItemCraftCondition(evoWeapons);
    }

    public override Position GetDefaultPosition() => new After("HEAD_IN_THE_CLOUDS");

    public override IEnumerable<Position> GetModdedConstraints()
    {
        yield return new After(ModContent.GetInstance<RareStoneAchievement>());
    }
}

public class ThirdTierAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Collector);

        int[] thirdTiers = { ModContent.ItemType<DragonFire>(), ModContent.ItemType<BlizzardIce>(), ModContent.ItemType<LightBeam>(),
        ModContent.ItemType<StormTornado>(), ModContent.ItemType<WildHammer>(), ModContent.ItemType<MetalFighter>(),
        ModContent.ItemType<MetaKnightSword>(), ModContent.ItemType<HomingBomb>(), ModContent.ItemType<BuzzCutter>(),
        ModContent.ItemType<CrystalNeedle>(), ModContent.ItemType<CyborgArcherBow>(), ModContent.ItemType<SpaceRangerGun>()};

        AddItemCraftCondition(thirdTiers);
    }

    public override Position GetDefaultPosition() => new After("BIG_BOOTY");
}

public class RainbowSwordAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Collector);

        //so it activates after the animation
        AddItemPickupCondition(ModContent.ItemType<RainbowSword>());
    }

    public override Position GetDefaultPosition() => new After("TOPPED_OFF");
}

public class ZeroWeaponAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Collector);

        int[] zeroWeapon = { ModContent.ItemType<LoveLoveStick>(), ModContent.ItemType<CrystalGun>(), ModContent.ItemType<MasterSword>(),
        ModContent.ItemType<DreamRod>()};

        AddItemCraftCondition(zeroWeapon);
    }
    public override Position GetDefaultPosition() => new After("CHAMPION_OF_TERRARIA");

    public override IEnumerable<Position> GetModdedConstraints()
    {
        yield return new After(ModContent.GetInstance<ZeroAchievement>());
    }
}
