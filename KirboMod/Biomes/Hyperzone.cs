using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics;
using Terraria.Graphics.Effects;

namespace KirboMod.Biomes
{
    public class Hyperzone : ModSceneEffect
    {
        public override void SetStaticDefaults()
        {
          
        }

        public override bool IsSceneEffectActive(Player player)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Zero>()) || NPC.AnyNPCs(ModContent.NPCType<PureDarkMatter>()) || NPC.AnyNPCs(ModContent.NPCType<ZeroEye>()))
            {
                //Disable these specific sky effects
                SkyManager.Instance["Party"].Deactivate();
                SkyManager.Instance["Ambience"].Deactivate();

                return true;
            }
            else
            {
                return false;
            }
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("KirboMod:HyperZone", isActive, default);
        }
    }
}