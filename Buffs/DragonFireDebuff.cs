using KirboMod.Dusts;
using KirboMod.Globals;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Buffs
{
    internal class DragonFireDebuff : ModBuff
    {
        public const int damagePerSecond = 100;
        public const int defenseLoss = 26;
        //reminder lightning check on dark matter laser
        public override void Update(NPC npc, ref int buffIndex)
        {
          
            npc.GetGlobalNPC<KirbNPC>().dragonFire = true;
        }
        public static void DustEffect(Entity entity)
        {

            Dust d = Dust.NewDustDirect(entity.position, entity.width, entity.height, ModContent.DustType<DragonFireDust>());
            d.scale = 1.4f;
            d.noGravity = false;
            
        }
    }
}
