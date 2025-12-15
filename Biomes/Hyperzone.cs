using KirboMod.NPCs;
using KirboMod.NPCs.PureDarkMatterRematch;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace KirboMod.Biomes
{
    public class Hyperzone : ModSceneEffect
    {
        public const string BiomeName = "KirboMod:HyperZone";
        public override bool IsSceneEffectActive(Player player)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Zero>()) || NPC.AnyNPCs(ModContent.NPCType<ZeroEye>()) || NPC.AnyNPCs(ModContent.NPCType<PureDarkMatterRematch>()))
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
            player.ManageSpecialBiomeVisuals(BiomeName, isActive, default);
        }
    }
}