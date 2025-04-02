using KirboMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Projectiles.ZeroDashHitbox
{
    public class ZeroDamageHitbox : ModProjectile
    {
        public override string Texture => "KirboMod/NothingTexture";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 250;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300; //duration of zero's dash
            Projectile.hostile = true;
            Helper.DealDefenseDamageInCalamity(Projectile);
        }
        public override void AI()
        {
            int zeroID = ModContent.NPCType<Zero>();
            NPC zero = Main.npc[(int)Projectile.ai[0]];
            //if index is not correct for whatever reason, maybe on multiplayer
            //then search if there is a zero npc active
            if(zero.type != zeroID || !zero.active)
            {
                zero = null;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC compare = Main.npc[i];
                    if (compare.active && compare.type == zeroID)
                    {
                        zero = compare;
                        Projectile.ai[0] = i;
                    }
                }
            }
            //if it can't find it, then kill itself and
            //return so we don't get null reference exception
            if (zero == null)
            {
                Projectile.Kill();
                return;
            }
            Projectile.ai[1] = 0;//1 if zero is on the background
            if (zero.ModNPC is Zero zeroModNPC)
            {
                Projectile.ai[1] = zeroModNPC.attacktype == Zero.ZeroAttackType.BackgroundShots ? 1 : 0;

            }
            else//more failsafe
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = zero.Center;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[1] == 1)//1 if zero is on the background
                return false;
            return Helper.CheckCircleCollision(targetHitbox, Projectile.Center, 190);//398 is zero's width and height. it's 380/2 (/2 because it's radius not diameter) to be a bit more forgiving
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
