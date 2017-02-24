using Terraria;
using Terraria.ModLoader;

namespace Durability {
	class DurabilityGlobalNPC : GlobalNPC {
		public override void SetupShop( int type, Chest shop, ref int nextSlot ) {
			//Player player = Main.player[ Main.myPlayer ];
			//var modplayer = player.GetModPlayer<DurabilityModPlayer>( this.mod );
			//modplayer.TakeInventorySnapshot();
			
			if( type != 107 ) { return; }	// Goblin only

			Item item = new Item();
			item.SetDefaults( this.mod.ItemType("SmithingHammerItem") );

			shop.item[nextSlot++] = item;
		}
	}
}
