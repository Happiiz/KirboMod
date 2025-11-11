using Microsoft.Xna.Framework;
using KirboMod.Items.Armor;
using KirboMod.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using KirboMod.Particles;
using Terraria.DataStructures;
using KirboMod.Projectiles.Marx.Cutter;
using KirboMod.NPCs.Marx;
using Microsoft.Xna.Framework.Graphics;
using KirboMod.Systems;

namespace KirboMod.Items.DebugItems
{
	public class Unmarxify : ModItem
	{
		public override void SetStaticDefaults()
		{

        }

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 420, 69, 21);
			Item.rare = ItemRarityID.Cyan;
			Item.useAnimation = 1;
            Item.useTime = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.consumable = false;
            MarxWingRenderer.Initialize();
        }

        public override bool? UseItem(Player player)
        {
            MarxSpawningSystem.UnlockedMarx = false;
            MarxSpawningSystem.MarxHasAppeared = false;
            MarxSpawningSystem.CanMarxAppear = false;
            DownedBossSystem.downedMarxBoss = false;
            Main.NewText("Marx has been reborn", Color.Yellow);

            return true;
        }
    }
}