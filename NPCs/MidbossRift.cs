using KirboMod.Items.Weapons;
using KirboMod.NPCs.MidBosses;
using KirboMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KirboMod.NPCs
{
    public class MidbossRift : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] != 1) //spawned naturally instead of with DD
            {
                string text = "A dimensional rift has appeared with a challenging foe!";

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(text, 175, 75);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(175, 75, 255));
                }
            }

            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.Center);
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[0] == 180) //summon
            {
                int index;

                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (player.ZoneSnow) //Mr. Frosty
                    {
                        index = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y,
                            ModContent.NPCType<MrFrosty>(), Target: Projectile.owner);
                    }
                    else //Bonkers
                    {
                        index = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y,
                            ModContent.NPCType<Bonkers>(), Target: Projectile.owner);
                    }

                    //if (index < Main.maxNPCs && index >= 0)
                    //{
                    //    NetMessage.SendData(MessageID.SyncNPC, number: index);
                    //}
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(8f, 8f); //circle

                    Vector2 yOffset = new Vector2(0, -20);

                    Dust.NewDustPerfect(Projectile.Center + yOffset, 15, speed, Scale: 1.5f); //fallen star dust
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter < 6)
            {
                Projectile.frame = 0;
            }
            else if (Projectile.frameCounter < 12)
            {
                Projectile.frame = 1;
            }
            else if (Projectile.frameCounter < 18)
            {
                Projectile.frame = 2;
            }
            else if (Projectile.frameCounter < 24)
            {
                Projectile.frame = 3;
            }
            else if (Projectile.frameCounter < 30)
            {
                Projectile.frame = 4;
            }
            else
            {
                Projectile.frameCounter = 0;
            }
            if (Projectile.ai[0] >= 240) //disappear
            {
                Projectile.Kill();
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public static Asset<Texture2D> Rift;

        public override bool PreDraw(ref Color drawColor)
        {

            Texture2D rift = TextureAssets.Projectile[Type].Value;

            Vector2 yOffset = new Vector2(0, -20); //move down with scale to match up with center

            float Xscale = Utils.GetLerpValue(0, 40, Projectile.ai[0], true) * Utils.GetLerpValue(240, 200, Projectile.ai[0], true);

            VFX.DrawGlowBallAdditive(Projectile.Center + yOffset, Xscale * 1.5f, Color.DeepSkyBlue, Color.White);

            Vector2 scale = new Vector2(Xscale, 1);
            Rectangle frame = rift.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Main.EntitySpriteDraw(rift, Projectile.Center - Main.screenPosition, frame, Color.White, 0, frame.Size() / 2, scale, SpriteEffects.None);

            return false;
        }
    }
}