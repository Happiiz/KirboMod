using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;

namespace KirboMod.NPCs.NPCConfusionHelper
{
    public static class Confusion
    {
        public static void DrawConfusedIndicator(NPC npc, Vector2 screenPos, SpriteBatch sb, float YOffset = 0)
        {
            if (!npc.confused)
            {
                return;
            }
            Asset<Texture2D> tex = TextureAssets.Npc[npc.type];
            Vector2 halfSize = new(tex.Width() / 2, tex.Height() / Main.npcFrameCount[npc.type] / 2);
            //ugly code yes but it's copy pasted from vanilla
            sb.Draw(TextureAssets.Confuse.Value, new Vector2(npc.position.X - screenPos.X + npc.width / 2 - TextureAssets.Npc[npc.type].Width() * npc.scale / 2f + halfSize.X * npc.scale, npc.position.Y - screenPos.Y + npc.height - tex.Height() * npc.scale / Main.npcFrameCount[npc.type] + 4f + halfSize.Y * npc.scale + YOffset - TextureAssets.Confuse.Height() - 20f), new Rectangle(0, 0, TextureAssets.Confuse.Width(), TextureAssets.Confuse.Height()), npc.GetShimmerColor(new Color(250, 250, 250, 70)), npc.velocity.X * -0.05f, new Vector2(TextureAssets.Confuse.Width() / 2, TextureAssets.Confuse.Height() / 2), Main.essScale + 0.2f, SpriteEffects.None, 0f);

        }
        public static void InvertDirection(NPC npc)
        {
            if (!npc.confused)
            {
                return;
            }
            npc.direction *= -1;
            npc.spriteDirection *= -1;
        }
        public static float AwayFromPlayer(NPC npc)
        {
            return npc.Center.X - Main.player[npc.target].Center.X < 0 ? -1 : 1;
        }
    }
}
