using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Backgrounds
{
	public class HyperZoneBg : ModSurfaceBackgroundStyle
	{
		//public override bool ChooseBgStyle()/* tModPorter Note: Removed. Create a ModBiome (or ModSceneEffect) class and override SurfaceBackgroundStyle property to return this object through Mod/ModContent.Find, then move this code into IsBiomeActive (or IsSceneEffectActive) */ {
		//	return !Main.gameMenu && Main.LocalPlayer.GetModPlayer<KirbPlayer>().hyperzone;
		//}

		// Use this to keep far Backgrounds like the [dark clouds].
		public override void ModifyFarFades(float[] fades, float transitionSpeed) {
			for (int i = 0; i < fades.Length; i++) {
				if (i == Slot) {
					fades[i] += transitionSpeed;
					if (fades[i] > 1f) {
						fades[i] = 1f;
					}
				}
				else {
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f) {
						fades[i] = 0f;
					}
				}
			}
		}

		/*public override int ChooseFarTexture() {
			return mod.GetBackgroundSlot("Backgrounds/DarkClouds");
		}*/

		private static int SurfaceFrameCounter;
		private static int SurfaceFrame;
		public override int ChooseMiddleTexture() {
			if (++SurfaceFrameCounter > 12) {
				SurfaceFrame = (SurfaceFrame + 1) % 4;
				SurfaceFrameCounter = 0;
			}
			switch (SurfaceFrame) {
				case 0:
					return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/DarkClouds");
				case 1:
					return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/DarkClouds");
				case 2:
					return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/DarkClouds");
				case 3:
					return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/DarkClouds");
				default:
					return -1;
			}
		}
		/*public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) {
			return mod.GetBackgroundSlot("Backgrounds/DarkClouds");
		}*/
	}
}