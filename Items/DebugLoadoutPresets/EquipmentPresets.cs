//using Microsoft.Xna.Framework.Graphics;
//using System;
//using Terraria;
//using Terraria.GameContent.UI.Elements;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria.UI;

//namespace KirboMod.Items.DebugLoadoutPresets
//{
//    public class EquipmentPresets : ModItem
//    {
//        //make something that just linearly increments (left click) or decemrents (right click) an index and sets them every time cuz don't want to make UI. if hold shift multiply the change by 4,
//        public bool presetWindowOpen = false;
//        public override string Texture => "Terraria/Images/Item_" + ItemID.Cog;
//        public override void SetDefaults()
//        {
//            Item.useTime = 1;
//            Item.useAnimation = 1;
//            Item.useStyle = 1;
//            Item.width = 15;
//            Item.height = 15;
//        }
//        public override bool? UseItem(Player player)
//        {
//            presetWindowOpen = !presetWindowOpen;
//            return true;
//        }
//        static int[] GetPreset(int i)
//        {
//            //ARRAY ORDER:
//            //MAX MELEE, RANGED, MAGIC, SUMMON
//            //ITEM ID ORDER:
//            //4 WEAPONS
//            //THE 3 ARMOR PIECES
//            //7 ACCESSORIES (LEAVE BLANK/DEFAULT ITEM ON PREHARDMODE STUFF
//            //3 VANITY ARMOR
//            //7 VANITY ACCESSORIES
//            int[][] presets = new int[][]
//            {
//                [ItemID.BladeofGrass, ItemID.Starfury, ItemID.JungleYoyo, ItemID.ThornChakram, ItemID.GoldHelmet, ItemID.Gi, ItemID.GoldGreaves, ItemID.HermesBoots, ItemID.BlizzardinaBottle, ItemID.FeralClaws, ItemID.SharkToothNecklace, ItemID.MagmaStone, 0,0, 0, ItemID.GoldChainmail,0, ItemID.AmphibianBoots, ItemID.YellowHorseshoeBalloon, ItemID.WhiteString, ItemID.Magiluminescence, ItemID.LuckyHorseshoe],//REMOVE LUCKY HORSESHOE HERE, REPLACE WITH SOMETHING ELSE
//                [ItemID.Boomstick, ItemID.PlatinumBow, ItemID.Minishark, ItemID.SnowballCannon, ItemID.GoldHelmet, ItemID.Gi, ItemID.GoldGreaves, ItemID.HermesBoots, ItemID.BlizzardinaBottle, ItemID.FeralClaws, ItemID.SharkToothNecklace, ItemID.MagmaStone, 0,0, 0, ItemID.GoldChainmail,0, ItemID.AmphibianBoots, ItemID.YellowHorseshoeBalloon, ItemID.WhiteString, ItemID.Magiluminescence, ItemID.LuckyHorseshoe],
//                [ItemID.Boomstick, ItemID.PlatinumBow, ItemID.Minishark, ItemID.SnowballCannon, ItemID.GoldHelmet, ItemID.Gi, ItemID.GoldGreaves, ItemID.HermesBoots, ItemID.BlizzardinaBottle, ItemID.ManaFlower, ItemID.SharkToothNecklace, ItemID.BandofRegeneration, 0,0, 0, ItemID.GoldChainmail,0, ItemID.AmphibianBoots, ItemID.YellowHorseshoeBalloon, ItemID.WhiteString, ItemID.Magiluminescence, ItemID.LuckyHorseshoe],
//                [ItemID.DemonScythe, ItemID.DiamondStaff, ItemID.Vilethorn, ItemID.CrimsonRod, ItemID.WizardHat, ItemID.GypsyRobe, ItemID.JunglePants, ItemID.HermesBoots, ItemID.BlizzardinaBottle, ItemID.FeralClaws, ItemID.SharkToothNecklace, ItemID.MagmaStone, 0,0, ItemID.GoldHelmet, ItemID.GoldChainmail,ItemID.GoldGreaves, ItemID.AmphibianBoots, ItemID.YellowHorseshoeBalloon, ItemID.WhiteString, ItemID.Magiluminescence, ItemID.LuckyHorseshoe],

//            };
//            return Array.Empty<int>();
//        }
//    }
//    public class PresetUI : UIElement
//    {
//        private UIText text;
//        private UIElement mainSelect;
//        private UIImage golemBossHead;
//        UIImage rangerEmblem;
//        //first indexer: tier, second indexer: class type. add third indexer later if needed
//        bool[][] activeStates;
//        const int TiersCount = 10;
//        public override void OnInitialize()
//        {
//            InitializeActiveStates();
//            // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
//            // UIElement is invisible and has no padding.
//            mainSelect = new UIElement();
//            mainSelect.Left.Set(-mainSelect.Width.Pixels - 600, 1f); // Place the resource bar to the left of the hearts.
//            mainSelect.Top.Set(30, 0f); // Placing it just a bit below the top of the screen.
//            mainSelect.Width.Set(182, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
//            mainSelect.Height.Set(60, 0f);

//        }

//        private void InitializeActiveStates()
//        {
//            activeStates = new bool[TiersCount][];
//            for (int i = 0; i < activeStates.Length; i++)
//            {
//                activeStates[i] = new bool[5];
//            }
//        }

//        public override void Draw(SpriteBatch spriteBatch)
//        {
//            Item item = Main.LocalPlayer.HeldItem;
//            EquipmentPresets moditem = item.ModItem as EquipmentPresets;
//            if (moditem is null || moditem == null || item == null || item.IsAir)
//            {
//                return;
//            }
//            if (!moditem.presetWindowOpen)
//            {
//                return;
//            }
//            base.Draw(spriteBatch);
//        }
//        public override void LeftClick(UIMouseEvent evt)
//        {
//            //mainSelect.
//            //evt.MousePosition
//        }
//    }
//}
