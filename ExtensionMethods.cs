using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace KirboMod
{
    public static class ExtensionMethods
    {
        public static Texture2D MyTexture(this Projectile proj, out Vector2 origin, out SpriteEffects fx)
        {
            fx = proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = TextureAssets.Projectile[proj.type].Value;
            origin = tex.Size() / 2;
            return tex;
        }
    }
}
