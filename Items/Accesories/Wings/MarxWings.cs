using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KirboMod.Items.Accesories.Wings
{
	[AutoloadEquip(EquipType.Wings)]
	public class MarxWings : ModItem
	{
		public override void SetStaticDefaults() {
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(360, 10f, 1f); //high flight time but low acceleration
		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.buyPrice(gold: 8);
			Item.rare = ItemRarityID.Yellow;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			//referenced from Hadarian Wings in Calamity source code (because wow who knew a haphazard vanilla Terraria mechanic would be so finicky)

			bool usingWings = player.controlJump && player.wingTime > 0f && player.velocity.Y != 0;
            bool hovering = player.TryingToHoverDown && !player.merman;

            if (usingWings && hovering)
			{
                player.velocity.Y *= 0.2f;
                if (player.velocity.Y > -2f && player.velocity.Y < 2f)
                {
                    player.velocity.Y = 0.105f;
                }

                player.wingTime += 0.5f; //double flight time while hovering because why not (inspired by hadarian wings)
            }
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
			acceleration *= 1.5f;
        }

        public override bool WingUpdate(Player player, bool inUse)
		{
			bool descending = player.controlJump && player.velocity.Y != 0 && player.wingTime <= 0;

            if (inUse || descending)
			{
				player.wingFrameCounter++;

				if (player.wingFrameCounter++ > 10)
				{
					player.wingFrame++;
					player.wingFrameCounter = 0;
				}

				if (player.wingFrame > 3)
				{
					player.wingFrame = 1; //skip the first frame
				}
			}
			else
			{
                player.wingFrame = 0;
            }

			Lighting.AddLight(player.Center, TorchID.Rainbow);

			return true;
		}

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, TorchID.Rainbow);
        }
	}
}
