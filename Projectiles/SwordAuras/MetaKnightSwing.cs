using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.SwordAuras
{
    public class MetaKnightSwing : SwordAura
    {
        public override float BaseScale => 1.6f;

        public override float ScaleIncrease => .7f;

        public override Color[] Palette => new Color[3] { new Color(150, 100, 255), Color.Black, new Color(0, 0, 255) };
    }
}