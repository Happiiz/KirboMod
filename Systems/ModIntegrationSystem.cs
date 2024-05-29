using KirboMod.NPCs;
using KirboMod.NPCs.DarkMatter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.Systems
{
    //This for integrating features of other mods into this one
    public class ModIntegrationSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            DoBossChecklistIntegration();
        }
        private void DoBossChecklistIntegration()
        {
            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            if (bossChecklistMod.Version < new Version(1, 3, 1))
            {
                return;
            }

            //Arguments Layout:

            /*bossChecklistMod.Call(
	        "LogBoss",
	        Mod,
	        nameof(Boss),
	        float that represents progression (reference here: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4%5D-Boss-Progression-Values),
	        () => DownedBossSystem.downedMyBoss,
	        ModContent.NPCType<MyBoss>(),
	        new Dictionary<string, object>() 
            {
		        ["spawnItems"] = ModContent.ItemType<ItemUsedToSpawnBoss>(),
		        //other arguments
	        }
            );*/


            //WHISPY
            

            List<int> whispycollectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.WhispyWoodsRelic>(),
                ModContent.ItemType<Items.WhispyWoods.WhispyPetItem>(),
                ModContent.ItemType<Items.WhispyWoods.WhispyTrophy>(),
                ModContent.ItemType<Items.WhispyWoods.WhispyMask>()
            };

            int summonItem = ModContent.ItemType<Items.WhispySeed>();

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(Whispy),
                0.75f, //before King Slime
                () => DownedBossSystem.downedWhispyBoss,
                ModContent.NPCType<NPCs.Whispy>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Mods.KirboMod.ProgressionDescription.Whispy").WithFormatArgs(summonItem),
                    ["spawnItems"] = ModContent.ItemType<Items.WhispySeed>(),
                    ["despawnMessage"] = Language.GetText("Mods.KirboMod.DespawnMessage.Whispy"),
                    ["collectibles"] = whispycollectibles
                }
            );

            // Other bosses or additional Mod.Call can be made after every call.


            //KRACKO


            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            var krackoPortrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/KrackoPortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };

            List<int> krackoCollectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.KrackoRelic>(),
                ModContent.ItemType<Items.Kracko.KrackoPetItem>(),
                ModContent.ItemType<Items.Kracko.KrackoTrophy>(),
                ModContent.ItemType<Items.Kracko.KrackoMask>()
            };

            summonItem = ModContent.ItemType<Items.SkyBlanket>();

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(Kracko),
                3.5f, //before Old One's Army
                () => DownedBossSystem.downedKrackoBoss,
                ModContent.NPCType<NPCs.Kracko>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Mods.KirboMod.ProgressionDescription.Kracko").WithFormatArgs(summonItem),
                    ["spawnItems"] = ModContent.ItemType<Items.SkyBlanket>(),
                    ["customPortrait"] = krackoPortrait,
                    ["despawnMessage"] = Language.GetText("Mods.KirboMod.DespawnMessage.Kracko"),
                    ["collectibles"] = krackoCollectibles
                }
            );

            var dededePortrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/KingDededePortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };


            //KING DEDEDE


            List<int> dededeCollectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.KingDededeRelic>(),
                ModContent.ItemType<Items.KingDedede.KingDededePetItem>(),
                ModContent.ItemType<Items.KingDedede.KingDededeTrophy>(),
                ModContent.ItemType<Items.KingDedede.KingDededeMask>()
            };

            summonItem = ModContent.ItemType<Items.DededeBrooch>();

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(KingDedede),
                6.5f, //before Wall of Flesh
                () => DownedBossSystem.downedKingDededeBoss,
                ModContent.NPCType<NPCs.KingDedede>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Mods.KirboMod.ProgressionDescription.Dedede").WithFormatArgs(summonItem),
                    ["spawnItems"] = ModContent.ItemType<Items.DededeBrooch>(),
                    ["customPortrait"] = dededePortrait,
                    ["despawnMessage"] = Language.GetText("Mods.KirboMod.DespawnMessage.Dedede"),
                    ["collectibles"] = dededeCollectibles
                }
            );


            //NIGHTMARE


            List<int> nightmareCollectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.NightmareRelic>(),
                ModContent.ItemType<Items.Nightmare.NightmarePetItem>(),
                ModContent.ItemType<Items.Nightmare.NightmareTrophy>(),
                ModContent.ItemType<Items.Nightmare.NightmareMask>()
            };

            summonItem = ModContent.ItemType<Items.Weapons.StarRod>();

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(NightmareWizard),
                11.75f, //before Plantera
                () => DownedBossSystem.downedNightmareBoss,
                ModContent.NPCType<NPCs.NightmareWizard>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Mods.KirboMod.ProgressionDescription.Nightmare").WithFormatArgs(summonItem),
                    ["spawnItems"] = ModContent.ItemType<Items.Weapons.StarRod>(),
                    ["despawnMessage"] = Language.GetText("Mods.KirboMod.DespawnMessage.Nightmare"),
                    ["collectibles"] = nightmareCollectibles
                }
            );


            //DARK MATTER


            List<int> darkMatterCollectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.DarkMatterRelic>(),
                ModContent.ItemType<Items.DarkMatter.DarkMatterPetItem>(),
                ModContent.ItemType<Items.DarkMatter.DarkMatterTrophy>(),
                ModContent.ItemType<Items.DarkMatter.DarkMatterMask>()
            };

            summonItem = ModContent.ItemType<Items.DarkMirror>();

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(PureDarkMatter),
                13.8f, //before Duke Fishron
                () => DownedBossSystem.downedDarkMatterBoss,
                ModContent.NPCType<DarkMatter>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Mods.KirboMod.ProgressionDescription.DarkMatter").WithFormatArgs(summonItem),
                    ["spawnItems"] = ModContent.ItemType<Items.DarkMirror>(),
                    ["despawnMessage"] = Language.GetText("Mods.KirboMod.DespawnMessage.DarkMatter"),
                    ["collectibles"] = darkMatterCollectibles
                }
            );


            //ZERO


            var zeroPortrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/ZeroPortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 4), rect.Y + (rect.Height / 2) - (texture.Height / 4));
                sb.Draw(texture, centered, default, color, 0, default, 0.5f, SpriteEffects.None, default); //half size
            };

            List<int> zeroCollectibles = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.ZeroRelic>(),
                ModContent.ItemType<Items.Zero.ZeroPetItem>(),
                ModContent.ItemType<Items.Zero.ZeroTrophy>(),
                ModContent.ItemType<Items.Zero.ZeroMask>()
            };

            summonItem = ModContent.ItemType<Items.PillarOfLight>();

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(Zero),
                22f, //after Moon Lord
                () => DownedBossSystem.downedZeroBoss,
                ModContent.NPCType<NPCs.Zero>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Mods.KirboMod.ProgressionDescription.Zero").WithFormatArgs(summonItem),
                    ["spawnItems"] = ModContent.ItemType<Items.PillarOfLight>(),
                    ["customPortrait"] = zeroPortrait,
                    ["despawnMessage"] = Language.GetText("Mods.KirboMod.DespawnMessage.Zero"),
                    ["collectibles"] = zeroCollectibles
                }
            );
        }
    }
}

//All the vanilla progression values

/*KingSlime = 1f;
TorchGod = 1.5f;
EyeOfCthulhu = 2f;
BloodMoon = 2.5f;
EaterOfWorlds = 3f;
GoblinArmy = 3.33f;
OldOnesArmy = 3.66f;
DarkMage = 3.67f;
QueenBee = 4f;
Skeletron = 5f;
DeerClops = 6f;
WallOfFlesh = 7f;
FrostLegion = 7.33f;
PirateInvasion = 7.66f;
PirateShip = 7.67f;
QueenSlime = 8f;
TheTwins = 9f;
TheDestroyer = 10f;
SkeletronPrime = 11f;
Ogre = 11.01f;
SolarEclipse = 11.5f;
Plantera = 12f;
Golem = 13f;
PumpkinMoon = 13.25f;
MourningWood = 13.26f;
Pumpking = 13.27f;
FrostMoon = 13.5f;
Everscream = 13.51f;
SantaNK1 = 13.52f;
IceQueen = 13.53f;
MartianMadness = 13.75f;
MartianSaucer = 13.76f;
DukeFishron = 14f;
EmpressOfLight = 15f;
Betsy = 16f;
LunaticCultist = 17f;
LunarEvent = 17.01f;
Moonlord = 18f;*/