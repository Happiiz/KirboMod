using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KirboMod.Items.RainbowDrops;
using ReLogic.Content;
using Terraria.ID;
using System.Linq;
using Terraria.Audio;

namespace KirboMod.Items.RainbowSword
{
    internal class RainbowSwordCraftingAnimation  : ModProjectile
    {
        public override string Texture => "KirboMod/Items/RainbowSword/RainbowSword";
        public override void SetDefaults()
        {
            
        }
        static float Easing(float progress)
        {
            return 0.5f - MathF.Cos(progress * MathF.PI) * 0.5f;
        }
        ref float Timer { get => ref Projectile.localAI[0]; }
        public override void AI()
        {
            if (Timer == 0)
            {
                Vector2 targetPos = Main.myPlayer == Projectile.owner ? Main.MouseWorld : Main.player[Projectile.owner].Center;

                float dist = targetPos.Distance(Projectile.Center);
                for (float i = 0.5f; i < 1; i+= 4/dist)
                {
                    Dust dust = Dust.NewDustPerfect(Vector2.Lerp(Projectile.Center, targetPos, i), DustID.RainbowMk2, null, 0, Main.hslToRgb(i, 1, 0.5f), 2);
                    dust.noGravity = true;
                    dust.velocity *= 0.2f;
                    dust.velocity -= (targetPos - Projectile.Center) / 18.6f;
                }
            }
            Timer++;
            float range = MathHelper.Lerp(700, 0, Easing(Utils.GetLerpValue(0, 640, Timer)));
            for (float i = 0; i < MathF.Tau; i += MathF.Tau / 5f)
            {
                if (Main.rand.NextFloat() < Easing( Utils.GetLerpValue(150, 50, Timer, true)))
                    continue;
                Vector2 offset = i.ToRotationVector2().RotatedByRandom(1) * MathHelper.Lerp(0.4f, 0.9f, Main.rand.NextFloat());
                offset *= range;
                Color col = Main.hslToRgb(Utils.GetLerpValue(-MathF.PI, MathF.PI, (offset.ToRotation() + Main.GlobalTimeWrappedHourly * 0.3f)) % 1, 1, 0.5f);
                Dust dust = Dust.NewDustPerfect(offset + Projectile.Center, DustID.RainbowMk2, -offset.RotatedBy(1.5f) * 0.04f,0,col, MathHelper.Lerp(2, 3, Main.rand.NextFloat()));
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.noGravity = true;
                dust = Dust.CloneDust(dust);
                dust.color = Color.White;
                dust.scale *= 0.7f;
                dust.noGravity = true;
            }
            if (Timer > 590)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(Projectile.GetSource_Death(), Projectile.Center, ModContent.ItemType<RainbowSword>());
                Array.ForEach(Main.dust, dust => dust.active = false);
                for (int i = 0; i < 400; i++)
                {
                    Vector2 posOffset = Main.rand.NextVector2Circular(1000, 1000) / 20;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + posOffset, DustID.RainbowMk2, posOffset * 0.2f, 0, Main.hslToRgb(Utils.GetLerpValue(-MathF.PI, MathF.PI, posOffset.ToRotation()), 1, 0.5f), MathHelper.Lerp(2,3,Main.rand.NextFloat()));
                    dust.noGravity = true;
                    dust = Dust.CloneDust(dust);
                    dust.color = Color.White;
                    dust.scale *= 0.7f;
                    dust.noGravity = true;
                }
                Projectile.Kill();
            }
        }
        static Asset<Texture2D> RainbowDropTexture(int index, out Vector2 origin, out SpriteEffects fx)
        {
            fx = SpriteEffects.None;
            Asset<Texture2D> tex = TextureAssets.Item[rainbowDrops[index]];
            origin = tex.Size() / 2;
            return tex;
        }
        static int[] rainbowDrops = new int[6] { ModContent.ItemType<DesertDrop>(), ModContent.ItemType<EvilDrop>(), ModContent.ItemType<HellDrop>(), ModContent.ItemType<JungleDrop>(), ModContent.ItemType<OceanDrop>(), ModContent.ItemType<SnowDrop>() }; 
        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < rainbowDrops.Length; i++)
            {
                int rainbowDropSpacing = 55;
                int frameIndexOffset = rainbowDrops[i] * rainbowDrops[i];//idk

                float rainbowDropDist = 300 + rainbowDropSpacing * i;//max dist
                rainbowDropDist *= Utils.GetLerpValue(rainbowDropSpacing * i + 100, rainbowDropSpacing * i, Timer);
                if (rainbowDropDist < 0)
                    rainbowDropDist = 0;
                float rainbowDropOpacity = Utils.GetLerpValue(450, 300, rainbowDropDist, true) * Utils.GetLerpValue(0, 20, rainbowDropDist, true);
                Vector2 offset = GetRainbowDropAngle(i).ToRotationVector2() * rainbowDropDist;
                Asset<Texture2D> tex = RainbowDropTexture(i, out _, out SpriteEffects spriteEffects);
                Rectangle frame = Utils.Frame(tex, 1, 6, 0, (int)(frameIndexOffset + Timer / 5) % 6);

                Main.EntitySpriteDraw(tex.Value, Projectile.Center + offset - Main.screenPosition, frame, Color.White * rainbowDropOpacity, 0, frame.Size() / 2, 1, spriteEffects);
            }
            float glowOpacity = Utils.GetLerpValue(0, 20, Timer, true) * Utils.GetLerpValue(-10, 150, 590 - Timer, true);
            glowOpacity = Easing(glowOpacity);
            VFX.DrawGlowBallDiffuse(Projectile.Center, Utils.Remap(Timer, 0, 700, 0.7f, 4), Main.DiscoColor * glowOpacity, Color.White * glowOpacity);
            VFX.DrawGlowBallDiffuse(Projectile.Center, Utils.Remap(Timer, 0, 700, 0.7f, 4) * 0.5f, Color.White * glowOpacity, Color.White * glowOpacity);

            return false;
        }
        static float GetRainbowDropAngle(int index)
        {
            return index switch
            {
                0 => 0,
                1 => (2 * MathF.Tau) / 6f,
                2 => (4 * MathF.Tau) / 6f,
                3 => (1 * MathF.Tau) / 6f,
                4 => MathF.PI,
                _ => 5 * MathF.Tau / 6,
            };
        }
    }
    public partial class RainbowSword : ModItem
    {
        static bool IsARainbowDropID(int id)
        {
            int[] rainbowDrops = new int[6] { ModContent.ItemType<DesertDrop>(), ModContent.ItemType<EvilDrop>(), ModContent.ItemType<HellDrop>(), ModContent.ItemType<JungleDrop>(), ModContent.ItemType<OceanDrop>(), ModContent.ItemType<SnowDrop>() };
            return rainbowDrops.Contains(id);
        }
        private void SpawnRainbowSwordCraftAnimation(On_Main.orig_CraftItem orig, Recipe r)
        {
            if(r.TryGetResult(Type, out _))
            {
                List<int> rainbowDropIDsTaken = new List<int>();
                for (int i = 0; i < Main.LocalPlayer.inventory.Length; i++)
                {
                    for (int j = 0; j < Main.maxProjectiles; j++)
                    {
                        Projectile projToCheck = Main.projectile[j];
                        if (!projToCheck.active || projToCheck.type != ModContent.ProjectileType<RainbowSwordCraftingAnimation>())
                            continue;
                        return;
                    }
                    Item item = Main.LocalPlayer.inventory[i];
                    if (rainbowDropIDsTaken.Contains(item.type) || !IsARainbowDropID(item.type))
                        continue;
                    if (item.stack > 1)
                    {
                        rainbowDropIDsTaken.Add(item.type);
                        item.stack--;
                        continue;
                    }
                    rainbowDropIDsTaken.Add(item.type);
                    item.TurnToAir();
                }
                SoundEngine.PlaySound(SoundID.Item4);
                SoundEngine.PlaySound(SoundID.DD2_WinScene);
                Recipe.FindRecipes();
                Projectile.NewProjectile(Item.GetSource_ReleaseEntity(), Main.LocalPlayer.Center - new Vector2(0, 150), Vector2.Zero, ModContent.ProjectileType<RainbowSwordCraftingAnimation>(), -1, 0, 255);
                return;
            }
            orig(r);
        }
    }
}
