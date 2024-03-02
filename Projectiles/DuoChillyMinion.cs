using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Projectiles
{
	public class DuoChillyMinion : ChillyMinion //code inherited from chilly minion
	{
        public override string Texture => "KirboMod/Projectiles/ChillyMinion";

        public override string Buff => "LeoAndChillyBuff";

        public override void SetDefaults()
		{
            base.SetDefaults();
            Projectile.minionSlots = 0.5f;
            Projectile.ArmorPenetration = 12; //add here since crown of climate itself doesn't have armor penetration
        }
    }
}