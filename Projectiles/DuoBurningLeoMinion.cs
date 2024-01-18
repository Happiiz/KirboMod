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
	public class DuoBurningLeoMinion : BurningLeoMinion //code inherited from leo minion
    {
        public override string Texture => "KirboMod/Projectiles/BurningLeoMinion";

        public override string Buff => "LeoAndChillyBuff";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.minionSlots = 0.5f;
        }
    }
}
