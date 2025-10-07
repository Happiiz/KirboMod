using KirboMod.NPCs;
using KirboMod.NPCs.NewWhispy;
using Terraria;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace KirboMod.Achievements;

public class WhispyAchievement : ModAchievement
{
	public override void SetStaticDefaults() {
		//slayer is default achievement, but put code here anyway for referencial purposes
		Achievement.SetCategory(AchievementCategory.Slayer);

		AddNPCKilledCondition(ModContent.NPCType<NewWhispyBoss>());
	}

	//put achievement after/before specified achievement
	public override Position GetDefaultPosition() => new Before("EYE_ON_YOU");
}

public class KrackoAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Slayer);

        AddNPCKilledCondition(ModContent.NPCType<Kracko>());
    }

    public override Position GetDefaultPosition() => new After("MASTERMIND");
}

public class KingDededeAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Slayer);

        AddNPCKilledCondition(ModContent.NPCType<KingDedede>());
    }

    public override Position GetDefaultPosition() => new After("DUNGEON_HEIST");
}

public class NightmareAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Slayer);

        AddNPCKilledCondition(ModContent.NPCType<NightmareWizard>());
    }

    public override Position GetDefaultPosition() => new After("DRAX_ATTAX");
}

//WAIT 'TIL AFTER MARX HAS BEEN ADDED

/*public class MarxAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Slayer);

        AddNPCKilledCondition(ModContent.NPCType<NPCs.Marx.MarxBoss>());
    }

    public override Position GetDefaultPosition() => new After("THE_GREAT_SOUTHERN_PLANTKILL");
}*/

public class DarkMatterAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Slayer);

        AddNPCKilledCondition(ModContent.NPCType<PureDarkMatter>());
    }

    public override Position GetDefaultPosition() => new After("FISH_OUT_OF_WATER");
}

public class ZeroAchievement : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Slayer);

        if (Main.expertMode)
        {
            AddNPCKilledCondition(ModContent.NPCType<ZeroEye>());
        }
        else
        {
            AddNPCKilledCondition(ModContent.NPCType<Zero>());
        }
    }

    public override Position GetDefaultPosition() => new After("CHAMPION_OF_TERRARIA");
}
