using KirboMod.NPCs;
using KirboMod.NPCs.PureDarkMatterRematch;
using KirboMod.Projectiles;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace KirboMod.Biomes
{
    public class Hyperzone : ModSceneEffect
    {
        public const string BiomeName = "KirboMod:HyperZone";
        public bool FlyingPillarOfLightExists;
        public override bool IsSceneEffectActive(Player player)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Zero>()) || NPC.AnyNPCs(ModContent.NPCType<ZeroEye>()) || NPC.AnyNPCs(ModContent.NPCType<PureDarkMatterRematch>()) || AnyProjs(ModContent.ProjectileType<FlyingPillarOfLight>()))
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
            FlyingPillarOfLightExists = false;
        }

        private static bool AnyProjs(int v)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if(p.active && p.type == v && (KirboWorld.summonedDarkMatterRematchBefore || p.ai[0] > 50))
                {
                    return true;
                }
            }
            return false;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals(BiomeName, isActive, default);
        }
    }
}