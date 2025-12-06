using Terraria.GameContent;
using Terraria.ModLoader;

namespace KirboMod.Gores
{
    public class CappyCap : ModGore
    {
        public override void SetStaticDefaults()
        {
            ChildSafety.SafeGore[Type] = true;
        }
    }
}