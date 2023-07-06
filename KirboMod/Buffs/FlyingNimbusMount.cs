using Terraria;
using Terraria.ModLoader;

namespace KirboMod.Buffs
{
	public class FlyingNimbusMount : ModBuff
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Flying Nimbus");
			// Description.SetDefault("'How is this a Kirby reference!?'");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.mount.SetMount(ModContent.MountType<Mounts.FlyingNimbus>(), player);
			player.buffTime[buffIndex] = 10;
		}
	}
}
