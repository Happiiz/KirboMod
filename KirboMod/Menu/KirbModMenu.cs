using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;

namespace KirboMod.Menu
{
	public class KirbModMenu : ModMenu
	{
		private const string menuAssetPath = "KirboMod/Menu"; // Creates a constant variable representing the texture path, so we don't have to write it out multiple times

		public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{menuAssetPath}/MenuLogo");

        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/Sun");

		public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/Moon");

		//public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");

		//public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<ExampleSurfaceBackgroundStyle>();

		public override string DisplayName => "Kirby's Terraria Mod";

		public override void OnSelected() 
		{
            SoundStyle menuSound = new SoundStyle("KirboMod/Menu/1up");
            SoundEngine.PlaySound(menuSound); // Plays a Kirby Super Star 1up sound when this ModMenu is selected
		}

		public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor) {
			drawColor = Color.White; // Changes the draw color of the logo
			return true;
		}
	}
}
