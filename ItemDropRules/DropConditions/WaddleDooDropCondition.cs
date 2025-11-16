using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace KirboMod.ItemDropRules.DropConditions
{
	// Checks if npc ai not equal to 1 because 1 means kracko doo
	public class WaddleDooDropCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) {
			if (!info.IsInSimulation) {
				return info.npc.ai[1] != 1;
			}
			return false;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return Language.GetTextValue("Mods.KirboMod.DropConditions.WaddleDoo");
		}
	}
}
