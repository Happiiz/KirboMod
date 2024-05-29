using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace KirboMod.ItemDropRules.DropConditions
{
    // Checks if in post-Golem Hardmode
    public class PostGolemHardmodeCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) {
			if (!info.IsInSimulation) {
				return Main.hardMode && NPC.downedGolemBoss;
			}
			return false;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return "In Hardmode, post-Golem";
		}
	}
}
