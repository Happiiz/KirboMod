using KirboMod.Buffs.MinionBuffs;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
    public class DuoChillyMinion : ChillyMinion //code inherited from chilly minion
    {
        public override string Texture => "KirboMod/Projectiles/ChillyMinion";

        public override int BuffID => ModContent.BuffType<LeoAndChillyBuff>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.minionSlots = 0.5f;
            Projectile.ArmorPenetration = 12; //add here since crown of climate itself doesn't have armor penetration
        }
    }
}