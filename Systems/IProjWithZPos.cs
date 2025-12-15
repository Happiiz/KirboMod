using KirboMod.Systems;
using Terraria;

namespace KirboMod.Systems
{
    public interface IProjWithZPos
    {
        public float ZPos { get; set; }
        public Projectile Projectile { get; }
    }
}
