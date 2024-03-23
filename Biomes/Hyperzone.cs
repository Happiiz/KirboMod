using KirboMod.NPCs;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace KirboMod.Biomes
{
    public class Hyperzone : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Zero>()) || NPC.AnyNPCs(ModContent.NPCType<ZeroEye>()))
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