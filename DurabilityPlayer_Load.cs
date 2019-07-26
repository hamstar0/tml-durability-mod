using System;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	partial class DurabilityPlayer : ModPlayer {
		private void OnLocalConnect() {
			// Because crafting recipe items showing a durability bar is confusing
			for( int i = 0; i < Main.recipe.Length; i++ ) {
				if( Main.recipe[i] == null ) { continue; }
				Item craftItem = Main.recipe[i].createItem;
				if( craftItem == null || craftItem.IsAir ) { continue; }

				try {
					var itemInfo = craftItem.GetGlobalItem<DurabilityItemInfo>();
					if( itemInfo == null ) { continue; }

					itemInfo.IsUnbreakable = true;
				} catch( Exception ) { }
			}
		}

		private void OnSingleConnect() {
			this.OnLocalConnect();
		}

		private void OnClientConnect() {
			this.OnLocalConnect();
		}

		private void OnServerConnect() { }
	}
}
