using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using KirboMod.Items.DarkMatter;
using KirboMod.Items.Accesories;
using KirboMod.Items.Accesories.Wings;

namespace KirboMod.Items.Marx
{
	public class MarxBag : DarkMatterBag
	{

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MarxMask>(), 7));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulMatter>(), 1, 20, 20));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MarxWings>(), 10));

            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<StarshipSummon>()));

            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<NPCs.Marx.MarxBoss>())); // drop money
        }
    }
}