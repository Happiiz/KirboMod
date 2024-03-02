using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Security.Policy;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace KirboMod.Projectiles.SwordAuras
{
    public class CresentSlash : SwordSlash
    {
        public override float ScaleMultiplier => 1.4f;
        public override Color[] Palette => new Color[3] { new Color(150, 100, 255), Color.Black, new Color(0, 0, 255) };
    }
}