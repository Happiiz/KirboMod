using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Globals
{
    public class BuffNPCsIfCalamityIsEnabled : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {
            entity.lifeMax = (int)(entity.lifeMax * 1.35f);
            entity.defense = (int)(entity.defense * 1.35f);
            entity.knockBackResist = entity.knockBackResist / 1.35f;
        }
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return ModLoader.TryGetMod("CalamityMod", out _) && entity.ModNPC != null && entity.ModNPC.Mod.Name == "KirboMod";
        }
    }
    internal class BuffWeaponsIfCalamityIsEnabled : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            bool calam = ModLoader.TryGetMod("CalamityMod", out _);
            return entity.damage > 0 && calam && entity.ModItem != null && entity.ModItem.Mod.Name == "KirboMod";
        }
        public override void SetDefaults(Item entity)
        {
            entity.damage = (int)(entity.damage * 1.35f);
        }
    }
}
