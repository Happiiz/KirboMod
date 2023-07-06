using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Buffs.Pets
{
	public class ZeroEyePetBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			// DisplayName and Description are automatically set from the .lang files, but below is how it is done normally.
			// DisplayName.SetDefault("Zero's Eye");
			// Description.SetDefault("\"Is it in constant pain of not being able to blink...?\"");
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.buffTime[buffIndex] = 18000;
			player.GetModPlayer<KirbPlayer>().zeroEyePet = true;
			bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.ZeroEyePetProj>()] <= 0;
			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer) {
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<Projectiles.Pets.ZeroEyePetProj>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}