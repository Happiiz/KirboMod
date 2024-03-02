using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class MinionIce : IceIce
	{
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.MinionShot[Type] = true;
        }
	}
}