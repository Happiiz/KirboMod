using Terraria;
using Terraria.ModLoader;

namespace KirboMod.NPCs.MidBosses
{
    public class MidbossMusic : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        short curMusic = -1;
        public override int Music => curMusic;
        public override bool IsSceneEffectActive(Player player)
        {
            const string path = "KirboMod/Music/";
            if (NPC.AnyNPCs(ModContent.NPCType<Batafire>()))
            {
                short musicSlot = (short)MusicLoader.GetMusicSlot(path + "Photonic0_BatafireBattle_WithLoopMetadata");
                if(curMusic == -1)//only if just changed the song
                {
                    ChangeMusicTo(musicSlot);
                }
                curMusic = musicSlot;
                return true;
            }
            if(NPC.AnyNPCs(ModContent.NPCType<Bonkers>()))
            {
                //curMusic = (short)MusicLoader.GetMusicSlot(path + "Happiz_BonkersBattle");
                //curMusic = (short)MusicLoader.GetMusicSlot(path + "Happiz_BonkersBattle_WithLoopMetadata");
                //return true;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<MrFrosty>()))
            {
                //curMusic = (short)MusicLoader.GetMusicSlot(path + "Photonic0_FrostyBattle_WithLoopMetadata");
                //return true;
            }
            curMusic = -1;
            return false;
        }
        void ChangeMusicTo(int musicSlot)
        {
            for (int i = 0; i < MusicLoader.MusicCount; i++)
            {
                Main.musicFade[i] = 0f;
            }
            Main.musicFade[musicSlot] = 1f;
        }
    }
}
