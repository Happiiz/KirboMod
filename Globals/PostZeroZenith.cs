using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using static Terraria.Graphics.FinalFractalHelper;
using KirboMod.Items.Weapons;
using System.Linq;
using Terraria.Graphics.Shaders;
using Terraria.Graphics;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;
using KirboMod.Items.DarkSword;

namespace KirboMod.Globals
{
	public class PostZeroZenith : GlobalItem
	{
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
			return entity.type == ItemID.Zenith;
        }
        private static Dictionary<int, FinalFractalProfile> _fractalProfiles = new Dictionary<int, FinalFractalProfile>
		{
	{
		65,
		new FinalFractalProfile(48f, new Color(236, 62, 192))
	},
	{
		1123,
		new FinalFractalProfile(48f, Main.OurFavoriteColor)
	},
	{
		46,
		new FinalFractalProfile(48f, new Color(122, 66, 191))
	},
	{
		121,
		new FinalFractalProfile(76f, new Color(254, 158, 35))
	},
	{
		190,
		new FinalFractalProfile(70f, new Color(107, 203, 0))
	},
	{
		368,
		new FinalFractalProfile(70f, new Color(236, 200, 19))
	},
	{
		674,
		new FinalFractalProfile(70f, new Color(236, 200, 19))
	},
	{
		273,
		new FinalFractalProfile(70f, new Color(179, 54, 201))
	},
	{
		675,
		new FinalFractalProfile(70f, new Color(179, 54, 201))
	},
	{
		2880,
		new FinalFractalProfile(70f, new Color(84, 234, 245))
	},
	{
		989,
		new FinalFractalProfile(48f, new Color(91, 158, 232))
	},
	{
		1826,
		new FinalFractalProfile(76f, new Color(252, 95, 4))
	},
	{
		3063,
		new FinalFractalProfile(76f, new Color(254, 194, 250))
	},
	{
		3065,
		new FinalFractalProfile(70f, new Color(237, 63, 133))
	},
	{
		757,
		new FinalFractalProfile(70f, new Color(80, 222, 122))
	},
	{
		155,
		new FinalFractalProfile(70f, new Color(56, 78, 210))
	},
	{
		795,
		new FinalFractalProfile(70f, new Color(237, 28, 36))
	},
	{
		3018,
		new FinalFractalProfile(80f, new Color(143, 215, 29))
	},
	{
		4144,
		new FinalFractalProfile(45f, new Color(178, 255, 180))
	},
	{
		3507,
		new FinalFractalProfile(45f, new Color(235, 166, 135))
	},
	{
		4956,
		new FinalFractalProfile(86f, new Color(178, 255, 180))
	},
	{
		ModContent.ItemType<MasterSword>(),
		new FinalFractalProfile(79, Color.Yellow)
	},
	{
	ModContent.ItemType<HeroSword>(),
		new FinalFractalProfile(50, new Color(107,202,255))
	},
	{
		ModContent.ItemType<GigantSword>(),
		new FinalFractalProfile(104, new Color(193,145,187))
	},
	{
		ModContent.ItemType<MetaKnightSword>(),
		new FinalFractalProfile(84, Color.Gold)
	},
	{
		ModContent.ItemType<Items.RainbowSword.RainbowSword>(),
		new FinalFractalProfile(50, Main.DiscoColor)
	},
	{
		ModContent.ItemType<DarkSword>(),
		new FinalFractalProfile(107, Color.Purple)
	}
};
		//todo: fix rainbow sword item not displaying correctly
		public override void Load()
		{
			Terraria.Graphics.On_FinalFractalHelper.GetFinalFractalProfile += GetFinalFractalProfilePostZero;
            Terraria.Graphics.On_FinalFractalHelper.GetRandomProfileIndex += GetRandomProfileIndexPostZero;
            Terraria.Graphics.On_FinalFractalHelper.Draw += DrawRainbowTrailIfRainbowSword;
		}
        private static VertexStrip _vertexStrip = new();
		Color ColorFunction(float progress)
		{
			return Main.hslToRgb((float)((progress * 3 + Main.timeForVisualEffects / 30) % 1), 1, 0.5f);
		}
		private void DrawRainbowTrailIfRainbowSword(On_FinalFractalHelper.orig_Draw orig, ref Terraria.Graphics.FinalFractalHelper self, Projectile proj)
        {		
			if((int)proj.ai[1] != ModContent.ItemType<Items.RainbowSword.RainbowSword>())
            {
				orig.Invoke(ref self, proj);
				return;
            }
			FinalFractalProfile finalFractalProfile = GetFinalFractalProfile((int)proj.ai[1]);
			MiscShaderData miscShaderData = GameShaders.Misc["FinalFractal"];
			int num = 4;
			int num2 = 0;
			int num3 = 0;
			int num4 = 4;
			miscShaderData.UseShaderSpecificData(new Vector4(num, num2, num3, num4));
			miscShaderData.UseImage0("Images/Extra_" + (short)201);
			miscShaderData.UseImage1("Images/Extra_" + (short)193);
			miscShaderData.Apply();
			_vertexStrip.PrepareStrip(proj.oldPos, proj.oldRot, ColorFunction, finalFractalProfile.widthMethod, -Main.screenPosition + proj.Size / 2f, proj.oldPos.Length, includeBacksides: true);
			_vertexStrip.DrawTrail();
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();	
		}

        private int GetRandomProfileIndexPostZero(On_FinalFractalHelper.orig_GetRandomProfileIndex orig)
        {
			List<int> list = _fractalProfiles.Keys.ToList();
			int index = Main.rand.Next(list.Count);
			if (list[index] == 4956)
			{
				list.RemoveAt(index);
				index = Main.rand.Next(list.Count);
			}
			return list[index];
		}

        private FinalFractalProfile GetFinalFractalProfilePostZero(On_FinalFractalHelper.orig_GetFinalFractalProfile orig, int usedSwordId)
		{
			if (!_fractalProfiles.TryGetValue(usedSwordId, out var value))
			{
				return new FinalFractalProfile(50f, Color.White);
			}
			return value;
		}

        public override void Unload()
        {
			On_FinalFractalHelper.Draw -= DrawRainbowTrailIfRainbowSword;
			On_FinalFractalHelper.GetFinalFractalProfile -= GetFinalFractalProfilePostZero;
			On_FinalFractalHelper.GetRandomProfileIndex -= GetRandomProfileIndexPostZero;
			_vertexStrip = null;
			_fractalProfiles = null;
		}
	}
	public class PostZeroZenithRecipe : ModSystem
    {
		public override void PostAddRecipes()
		{
			for (int i = 0; i < Recipe.numRecipes; i++)
			{
				Recipe recipe = Main.recipe[i];

				if (recipe.TryGetResult(ItemID.Zenith, out _))
				{
					recipe.AddIngredient<MasterSword>();
				}
			}
		}
	}
}
