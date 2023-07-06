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

            // The "LogBoss" method requires many parameters, defined separately below:

            // The name used for the title of the page
            string bossName = "Whispy Woods";

            // The NPC type of the boss
            int bossType = ModContent.NPCType<NPCs.Whispy>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 0.5f; //before king slime

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedWhispyBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.WhispyWoodsRelic>(),
                ModContent.ItemType<Items.WhispyWoods.WhispyPetItem>(),
                ModContent.ItemType<Items.WhispyWoods.WhispyTrophy>(),
                ModContent.ItemType<Items.WhispyWoods.WhispyMask>()
            };

            // The item used to summon the boss with (if available)
            int summonItem = ModContent.ItemType<Items.WhispySeed>();

            // Information for the player so they know how to encounter the boss
           string spawnInfo = $"Use a [i:{summonItem}].";

            // Custom despawn message
            string despawnInfo = "Whispy Woods blends back in with it's bretheren...";

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            /*var customBossPortrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("ExampleMod/Assets/Textures/Bestiary/MinionBoss_Preview").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };*/
            
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                nameof(Whispy),
                0.5f, //before King Slime
                () => DownedBossSystem.downedWhispyBoss,
                ModContent.NPCType<NPCs.Whispy>(),
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = summonItem,
                }
            );

            // Other bosses or additional Mod.Call can be made after every call.

            // The name used for the title of the page
            string bossName2 = "Kracko";

            // The NPC type of the boss
            int bossType2 = ModContent.NPCType<NPCs.Kracko>();

            // Value inferred from boss progression, see the wiki for details
            float weight2 = 3.5f; //before old ones army

            // Used for tracking checklist progress
            Func<bool> downed2 = () => DownedBossSystem.downedKrackoBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available2 = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection2 = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.KrackoRelic>(),
                ModContent.ItemType<Items.Kracko.KrackoPetItem>(),
                ModContent.ItemType<Items.Kracko.KrackoTrophy>(),
                ModContent.ItemType<Items.Kracko.KrackoMask>()
            };

            // The item used to summon the boss with (if available)
            int summonItem2 = ModContent.ItemType<Items.SkyBlanket>();

            // Information for the player so they know how to encounter the boss
            string spawnInfo2 = $"Kill a natural Kracko Jr. or use a [i:{summonItem2}] to summon one.";

            // Custom despawn message
            string despawnInfo2 = "Kracko floats back up into the heavens...";

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            var customBossPortrait2 = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/KrackoPortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                bossName2,
                bossType2,
                weight2,
                downed2,
                available2,
                collection2,
                summonItem2,
                spawnInfo2,
                despawnInfo2,
                customBossPortrait2
            );

            // The name used for the title of the page
            string bossName3 = "King Dedede";

            // The NPC type of the boss
            int bossType3 = ModContent.NPCType<NPCs.KingDedede>();

            // Value inferred from boss progression, see the wiki for details
            float weight3 = 6.5f; //before wall of flesh

            // Used for tracking checklist progress
            Func<bool> downed3 = () => DownedBossSystem.downedKingDededeBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available3 = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection3 = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.KingDededeRelic>(),
                ModContent.ItemType<Items.KingDedede.KingDededePetItem>(),
                ModContent.ItemType<Items.KingDedede.KingDededeTrophy>(),
                ModContent.ItemType<Items.KingDedede.KingDededeMask>()
            };

            // The item used to summon the boss with (if available)
            int summonItem3 = ModContent.ItemType<Items.DededeBrooch>();

            // Information for the player so they know how to encounter the boss
            string spawnInfo3 = $"Use a [i:{summonItem3}].";

            // Custom despawn message
            string despawnInfo3 = "The gluttonous king gives up the chase...";

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            var customBossPortrait3 = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/KingDededePortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                bossName3,
                bossType3,
                weight3,
                downed3,
                available3,
                collection3,
                summonItem3,
                spawnInfo3,
                despawnInfo3,
                customBossPortrait3
            );

            // The name used for the title of the page
            string bossName4 = "Nightmare";

            // The NPC type of the boss
            int bossType4 = ModContent.NPCType<NPCs.NightmareWizard>();

            // Value inferred from boss progression, see the wiki for details
            float weight4 = 7.8f; //before queen slime

            // Used for tracking checklist progress
            Func<bool> downed4 = () => DownedBossSystem.downedNightmareBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available4 = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection4 = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.NightmareRelic>(),
                ModContent.ItemType<Items.Nightmare.NightmarePetItem>(),
                ModContent.ItemType<Items.Nightmare.NightmareTrophy>(),
                ModContent.ItemType<Items.Nightmare.NightmareMask>()
            };

            // The item used to summon the boss with (if available)
            int summonItem4 = ModContent.ItemType<Items.Weapons.StarRod>();

            // Information for the player so they know how to encounter the boss
            string spawnInfo4 = $"Use a [i:{summonItem4}] at a Fountain of Dreams during night.";

            // Custom despawn message
            string despawnInfo4 = "It slips back into the shadows...";

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            /*var customBossPortrait4 = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/KingDededePortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };*/

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                bossName4,
                bossType4,
                weight4,
                downed4,
                available4,
                collection4,
                summonItem4,
                spawnInfo4,
                despawnInfo4
                //customBossPortrait4
            );

            // The name used for the title of the page
            string bossName5 = "Dark Matter";

            // The NPC type of the boss
            int bossType5 = ModContent.NPCType<NPCs.DarkMatter>();

            // Value inferred from boss progression, see the wiki for details
            float weight5 = 13.5f; //before besty

            // Used for tracking checklist progress
            Func<bool> downed5 = () => DownedBossSystem.downedDarkMatterBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available5 = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection5 = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.DarkMatterRelic>(),
                ModContent.ItemType<Items.DarkMatter.DarkMatterPetItem>(),
                ModContent.ItemType<Items.DarkMatter.DarkMatterTrophy>(),
                ModContent.ItemType<Items.DarkMatter.DarkMatterMask>()
            };

            // The item used to summon the boss with (if available)
            int summonItem5 = ModContent.ItemType<Items.DarkMirror>();
            int summonItem5two = ModContent.ItemType<Items.Weapons.RainbowSword>();

            // Information for the player so they know how to encounter the boss
            string spawnInfo5 = $"Throw out a [i:{summonItem5}] and right click towards it with a [i:{summonItem5two}].";

            // Custom despawn message
            string despawnInfo5 = "The invader disappears to which it came...";

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            /*var customBossPortrait5 = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/KingDededePortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };*/

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                bossName5,
                bossType5,
                weight5,
                downed5,
                available5,
                collection5,
                summonItem5,
                spawnInfo5,
                despawnInfo5
            //customBossPortrait5
            );

            // The name used for the title of the page
            string bossName6 = "Zero";

            // The NPC type of the boss
            int bossType6 = ModContent.NPCType<NPCs.Zero>();

            // Value inferred from boss progression, see the wiki for details
            float weight6 = 18.1f; //right after moon lord

            // Used for tracking checklist progress
            Func<bool> downed6 = () => DownedBossSystem.downedZeroBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available6 = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection6 = new List<int>()
            {
                ModContent.ItemType<Items.Placeables.BossRelics.ZeroRelic>(),
                ModContent.ItemType<Items.Zero.ZeroPetItem>(),
                ModContent.ItemType<Items.Zero.ZeroTrophy>(),
                ModContent.ItemType<Items.Zero.ZeroMask>()
            };

            // The item used to summon the boss with (if available)
            int summonItem6 = ModContent.ItemType<Items.PillarOfLight>();

            // Information for the player so they know how to encounter the boss
            string spawnInfo6 = $"Use a [i:{summonItem6}] and prepare for what comes next.";

            // Custom despawn message
            string despawnInfo6 = "The darkness clears...";

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            var customBossPortrait6 = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("KirboMod/NPCs/BestiaryTextures/ZeroPortrait").Value;
                Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 4), rect.Y + (rect.Height / 2) - (texture.Height / 4));
                sb.Draw(texture, centered, default, color, 0, default, 0.5f, SpriteEffects.None, default); //half size
            };

            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                bossName6,
                bossType6,
                weight6,
                downed6,
                available6,
                collection6,
                summonItem6,
                spawnInfo6,
                despawnInfo6,
                customBossPortrait6
            );
        }
    }
}