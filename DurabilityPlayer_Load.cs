using Durability.NetProtocol;
using System;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	partial class DurabilityPlayer : ModPlayer {
		private void onLocalConnect() {
			// Because crafting recipe items showing a durability bar is confusing
			for( int i = 0; i < Main.recipe.Length; i++ ) {
				if( Main.recipe[i] == null ) { continue; }
				Item craft_item = Main.recipe[i].createItem;
				if( craft_item == null || craft_item.IsAir ) { continue; }

				try {
					var item_info = craft_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
					if( item_info == null ) { continue; }

					item_info.IsUnbreakable = true;
				} catch( Exception ) { }
			}
		}

		private void OnSingleConnect() {
			this.onLocalConnect();
		}

		private void OnClientConnect() {
			var mymod = (DurabilityMod)this.mod;
			ClientPacketHandlers.SendSettingsRequestFromClient( mymod, player );

			this.onLocalConnect();
		}

		private void OnServerConnect() { }
	}
}
