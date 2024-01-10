using KirboMod.Items.Weapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class PlasmaShield : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 140;
			Projectile.height = 140;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = int.MaxValue;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
            Projectile.hide = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
			return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return Helper.CheckCircleCollision(targetHitbox, Projectile.Center, Projectile.ai[0]);
        }
        public override void AI()
		{
		
			Player player = Main.player[Projectile.owner];
			if(player.HeldItem.type != ModContent.ItemType<Plasma>())
            {
				Projectile.Kill();
				return;
            }
			KirbPlayer mplr = player.GetModPlayer<KirbPlayer>();
			if(mplr.PlasmaShieldLevel < 1)
            {
				Projectile.Kill();
				return;
            }
			Projectile.damage = mplr.PlasmaShieldLevel == 1 ? 50 : 100;
			Projectile.ai[0] = mplr.PlasmaShieldRadius;
			Projectile.Center = player.Center;
			Lighting.AddLight(Projectile.Center, 0, 1, 0);
		}
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (target.boss || target.type == NPCID.TargetDummy)
				return;
			Vector2 push = Vector2.Normalize(target.Center - Projectile.Center) * 40;
			push *= target.knockBackResist < .1f ? .1f : target.knockBackResist;
			target.velocity = push;
			NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, target.whoAmI);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			modifiers.Knockback *= 0;
        }
    }
}