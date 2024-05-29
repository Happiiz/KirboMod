using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace KirboMod.ItemDropRules.DropConditions
{
	// Checks if in pre-Golem Hardmode
	public class PreGolemHardmodeCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) {
			if (!info.IsInSimulation) {
				return Main.hardMode && !NPC.downedGolemBoss;
			}
			return false;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return "In Hardmode, pre-Golem";
        }
	}
}
