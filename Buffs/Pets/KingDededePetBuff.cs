using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Buffs.Pets
{
	public class KingDededePetBuff : ModBuff
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Dee");
			// Description.SetDefault("\"The one to rule them all... one day....\"");
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.buffTime[buffIndex] = 18000;

			player.GetModPlayer<KirbPlayer>().kingDededePet = true;

			bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.KingDededePet>()] <= 0;

			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer) {
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<Projectiles.Pets.KingDededePet>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}