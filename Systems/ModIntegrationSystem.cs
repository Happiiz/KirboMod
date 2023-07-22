using KirboMod.NPCs;
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
            // Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees various data is initialized and set up properly

            // Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
            // https://forums.terraria.org/index.php?threads/.50668/
            DoBossChecklistIntegration();
        }
        private void DoBossChecklistIntegration()
        {
            // The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/Support-using-Mod-Call
            // If we navigate the wiki, we can find the "LogBoss" method, which we want in this case

            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.3.1
            // Usually mods either provide that information themselves in some way, or it's found on the github through commit history/blame
            if (bossChecklistMod.Version < new Version(1, 3, 1))
            {
                return;
            }

            //Arguments Layout:

            /*bossChecklistMod.Call(
	        "LogBoss",
	        Mod,
	        nameof(Boss),
	        float that represents rarity (reference here: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4%5D-Boss-Progression-Values),
	        () => DownedBossSystem.downedMyBoss,
	        ModContent.NPCType<MyBoss>(),
	        new Dictionary<string, object>() 
            {
		        ["spawnItems"] = ModContent.ItemType<ItemUsedToSpawnBoss>(),
		        //other arguments
	        }
            );*/


            //Whispy

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(Whispy),
                0.5f, //before King Slime
                () => DownedBossSystem.downedWhispyBoss,
                ModContent.NPCType<NPCs.Whispy>(),
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<Items.WhispySeed>(),
                }
            );

            // Other bosses or additional Mod.Call can be made after every call.

            // Custom despawn message
            string despawnInfo2 = "Kracko floats back up into the heavens...";

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            var customBossPortrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/KrackoPortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };

            //Kracko

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(Kracko),
                3.5f, //before Old One's Army
                () => DownedBossSystem.downedKrackoBoss,
                ModContent.NPCType<NPCs.Kracko>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Kill a naturally spawning Kracko Jr. or use a [i:{ModContent.ItemType<Items.SkyBlanket>()}] to summon one."),
                    ["spawnItems"] = ModContent.ItemType<Items.SkyBlanket>(),
                    ["customPortrait"] = customBossPortrait,
                }
            );

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            var customBossPortrait2 = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/KingDededePortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };

            //King Dedede

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(KingDedede),
                6.5f, //before Wall of Flesh
                () => DownedBossSystem.downedKingDededeBoss,
                ModContent.NPCType<NPCs.KingDedede>(),
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<Items.DededeBrooch>(),
                    ["customPortrait"] = customBossPortrait2,
                }
            );

            //Nightmare

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(NightmareWizard),
                7.8f, //before Queen Slime
                () => DownedBossSystem.downedNightmareBoss,
                ModContent.NPCType<NPCs.NightmareWizard>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Use a [i:{ModContent.ItemType<Items.Weapons.StarRod>()}] at a Fountain of Dreams during night."),
                    ["spawnItems"] = ModContent.ItemType<Items.Weapons.StarRod>(),
                }
            );

            //Dark Matter

            // The item used to summon the boss with (if available)
            int summonItem2 = ModContent.ItemType<Items.DarkMirror>();
            int summonItem2two = ModContent.ItemType<Items.Weapons.RainbowSword>();

            List<int> spawnItems = new List<int>()
            {
                summonItem2,
                summonItem2two,
            };

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(DarkMatter),
                13.5f, //before Betsy
                () => DownedBossSystem.downedDarkMatterBoss,
                ModContent.NPCType<NPCs.DarkMatter>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Throw out a [i:{summonItem2}] and right click towards it with a [i:{summonItem2two}]."),
                    ["spawnItems"] = spawnItems,
                }
            );

            //Zero

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            var customBossPortrait3 = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/ZeroPortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 4), rect.Y + (rect.Height / 2) - (texture.Height / 4));
                sb.Draw(texture, centered, default, color, 0, default, 0.5f, SpriteEffects.None, default); //half size
            };

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(Zero),
                18.1f, //Right after Moon Lord
                () => DownedBossSystem.downedZeroBoss,
                ModContent.NPCType<NPCs.Zero>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetText($"Use a [i:{ModContent.ItemType<Items.PillarOfLight>()}] and prepare for what comes next."),
                    ["spawnItems"] = ModContent.ItemType<Items.PillarOfLight>(),
                    ["customPortrait"] = customBossPortrait3,
                }
            );
        }
    }
}