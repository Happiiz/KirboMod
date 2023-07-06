using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Buffs.Pets
{
	public class KrackoPetBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			// DisplayName and Description are automatically set from the .lang files, but below is how it is done normally.
			// DisplayName.SetDefault("Lil' Krackle");
			// Description.SetDefault("\"It cries water every once in a while.\"");
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.buffTime[buffIndex] = 18000;

			player.GetModPlayer<KirbPlayer>().krackoPet = true;

			bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.KrackoPet>()] <= 0;

			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer) {
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<Projectiles.Pets.KrackoPet>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}