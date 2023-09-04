using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Globals
{
    internal class BuffWeaponsIfCalamityIsEnabled : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.damage > 0;
        }
        public override void SetDefaults(Item entity)
        {
            if(entity.ModItem.Mod != null && entity.ModItem.Mod.Name == "KirboMod")
            {
                entity.damage = (int)(entity.damage * 1.35f);
            }
        }
    }
}
