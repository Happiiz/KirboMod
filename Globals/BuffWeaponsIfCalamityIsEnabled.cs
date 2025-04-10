using KirboMod.Items.Weapons;
using KirboMod.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Globals
{
    public class TemporaryDefenseDamageThing : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return ModLoader.TryGetMod("CalamityMod", out _) && entity.ModProjectile != null && entity.ModProjectile.Mod.Name == "KirboMod" ;
        }
        public override void SetDefaults(Projectile entity)
        {
            Helper.DealDefenseDamageInCalamity(entity);

        }
    }
    public class BuffNPCsIfCalamityIsEnabled : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {           
            entity.lifeMax = (int)(entity.lifeMax * 1.35f);
            entity.defense = (int)(entity.defense * 1.35f);
            entity.knockBackResist = entity.knockBackResist / 1.35f;
            entity.damage = (int)(entity.damage * 1.2f);
            Helper.DealDefenseDamageInCalamity(entity);
        }
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type != ModContent.NPCType<Zero>() && ModLoader.TryGetMod("CalamityMod", out _) && entity.ModNPC != null && entity.ModNPC.Mod.Name == "KirboMod";
        }
        public override void Load()
        {
            On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += IncreaseKirboModHostileProjDamage;
        }

        private int IncreaseKirboModHostileProjDamage(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, Terraria.DataStructures.IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
        {
            Projectile sample = ContentSamples.ProjectilesByType[Type];
            if(sample.ModProjectile != null && sample.ModProjectile.Mod.Name == "KirboMod")
            {
                Damage = (int)(Damage * 1.15f);
            }
            return orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
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
            if (entity.type == ModContent.ItemType<ChakramCutter>())
            {
                //calamity buffs light disc by around this much.
                //Chakram cutter uses light disc in its recipe
                entity.damage = (int)(entity.damage * 2.4f);
            }
            else
            {
                entity.damage = (int)(entity.damage * 1.2f);
            }
            if(entity.type == ModContent.ItemType<MasterSword>())
            {
                entity.damage = 1000;
            }else if(entity.type == ModContent.ItemType<DreamRod>())
            {
                entity.damage = 1000;
            }else if(entity.type == ModContent.ItemType<LoveLoveStick>())
            {
                entity.damage = 1400;
            }else if(entity.type == ModContent.ItemType<CrystalGun>())
            {
                entity.damage = 3500;
            }
        }
    }
}
