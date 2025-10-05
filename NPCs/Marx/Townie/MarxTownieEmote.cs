using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace KirboMod.NPCs.Marx.Townie
{
	public class MarxTownieEmote : ModEmoteBubble
	{
		public override void SetStaticDefaults() {

			AddToCategory(EmoteID.Category.Town);
		}
	}
}
