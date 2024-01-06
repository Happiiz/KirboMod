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
        public static Vector2 Normalized(this Vector2 vec, float lengthMultiplier = 1)
        {
            vec.Normalize();
            return vec * lengthMultiplier;
        }
        public static bool DrawSelf(this Projectile proj, bool fullbright = true)
        {
            Main.instance.LoadProjectile(proj.type);
            SpriteEffects fx = proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = TextureAssets.Projectile[proj.type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[proj.type], 0, proj.frame);
            Color col = fullbright ? Color.White : Lighting.GetColor(proj.Center.ToTileCoordinates());
            Main.EntitySpriteDraw(tex, proj.Center - Main.screenLastPosition, frame, col * proj.Opacity, proj.rotation, tex.Size() / 2, proj.scale, fx);
            return false;
        }
        public static Texture2D MyTexture(this Projectile proj, out Vector2 origin, out SpriteEffects fx)
        {
            fx = proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = TextureAssets.Projectile[proj.type].Value;
            origin = tex.Size() / 2;
            return tex;
        }
    }
}
