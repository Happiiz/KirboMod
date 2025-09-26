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

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(20f, 20f); //circle
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.MetaBat>(), speed); //Makes dust in a messy circle
            }
        }
    }
}