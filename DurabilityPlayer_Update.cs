using HamstarHelpers.Helpers.ItemHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using System.Linq;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	partial class DurabilityPlayer : ModPlayer {
		public override void PreUpdate() {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			Player player = this.player;

			Item curr_item = player.inventory[player.selectedItem];
			Item head_item = player.armor[0];
			Item body_item = player.armor[1];
			Item legs_item = player.armor[2];

			if( curr_item != null && !curr_item.IsAir ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir ) { curr_item = Main.mouseItem; }

				var item_info = curr_item.GetGlobalItem<DurabilityItemInfo>( mymod );
				item_info.ConcurrentUses = 0;
				if( !item_info.IsBroken ) {
					item_info.UpdateCriticalState( mymod, curr_item );
				}
			}

			if( !head_item.IsAir ) {
				var head_item_info = head_item.GetGlobalItem<DurabilityItemInfo>( mymod );
				head_item_info.ConcurrentUses = 0;

				if( head_item_info.IsBroken ) {
					int who = ItemHelpers.CreateItem( player.position, head_item.type, 1, head_item.width, head_item.height, head_item.prefix );
					ItemHelpers.DestroyItem( head_item );

					var new_item = Main.item[who];
					var new_item_info = new_item.GetGlobalItem<DurabilityItemInfo>( mymod );
					new_item_info.KillMe( mymod, new_item );
					player.armor[0] = new Item();
				}
			}

			if( !body_item.IsAir ) {
				var body_item_info = body_item.GetGlobalItem<DurabilityItemInfo>( mymod );
				body_item_info.ConcurrentUses = 0;

				if( body_item_info.IsBroken ) {
					int who = ItemHelpers.CreateItem( player.position, body_item.type, 1, body_item.width, body_item.height, body_item.prefix );
					ItemHelpers.DestroyItem( body_item );

					var new_item = Main.item[who];
					var new_item_info = new_item.GetGlobalItem<DurabilityItemInfo>( mymod );
					new_item_info.KillMe( mymod, new_item );
					player.armor[1] = new Item();
				}
			}

			if( !legs_item.IsAir ) {
				var legs_item_info = legs_item.GetGlobalItem<DurabilityItemInfo>( mymod );
				legs_item_info.ConcurrentUses = 0;

				if( legs_item_info.IsBroken ) {
					int who = ItemHelpers.CreateItem( player.position, legs_item.type, 1, legs_item.width, legs_item.height, legs_item.prefix );
					ItemHelpers.DestroyItem( legs_item );

					var new_item = Main.item[who];
					var new_item_info = new_item.GetGlobalItem<DurabilityItemInfo>( mymod );
					new_item_info.KillMe( mymod, new_item );
					player.armor[2] = new Item();
				}
			}

			this.CheckPurchaseChanges();
		}

		////////////////

		private void CheckPurchaseChanges() {
			if( Main.netMode == 2 ) { return; }	// Not server
			if( this.player.whoAmI != Main.myPlayer ) { return; }	// Current player

			// Note: Due to a (tML?) bug, this method of copying seems necessary?
			if( this.ForceCopy != null ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir ) {
					var item_info = Main.mouseItem.GetGlobalItem<DurabilityItemInfo>( this.mod );
					item_info.CopyToMe( this.ForceCopy );
				}
				this.ForceCopy = null;
			}

			long money = PlayerItemHelpers.CountMoney( this.player );
			long spent = this.LastMoney - money;

			if( this.player.talkNPC != -1 ) {
				var shop = Main.instance.shop;
				if( Main.npcShop >= 0 && Main.npcShop < shop.Length && shop[Main.npcShop] != null ) {
					if( spent > 0 ) {
						this.FixPurchasedItemInfo();
					}
					this.UpdateInventoryAndShopSnapshots();
				}
				this.LastMoney = money;
			}
		}


		private void UpdateInventoryAndShopSnapshots() {
			int len = this.player.inventory.Length + 1;
			Item[] stock = Main.instance.shop[Main.npcShop].item;

			// Get snapshop of inventory
			if( this.PrevInventory == null || this.PrevInventory.Length != len ) {
				this.PrevInventory = new Item[len];
			}
			for( int i = 0; i < len - 1; i++ ) {
				this.PrevInventory[i] = this.player.inventory[i].Clone();
			}
			this.PrevInventory[len - 1] = Main.mouseItem.Clone();

			// Get snapshot of shop
			if( this.PrevShop == null || this.PrevShop.Length != stock.Length ) {
				this.PrevShop = new Item[stock.Length];
			}
			for( int i = 0; i < stock.Length; i++ ) {
				if( stock[i] != null ) {
					this.PrevShop[i] = stock[i].Clone();
				} else {
					this.PrevShop[i] = new Item();
				}
			}
		}

		
		private void FixPurchasedItemInfo() {
			Item[] curr_shop = Main.instance.shop[Main.npcShop].item;
			Item[] curr_inv = (Item[])this.player.inventory.Clone();
			curr_inv = curr_inv.Concat<Item>( new Item[] { Main.mouseItem.Clone() } ).ToArray();

			var shop_changes = ItemFinderHelpers.FindChanges( this.PrevShop, curr_shop );
			var inv_changes = ItemFinderHelpers.FindChanges( this.PrevInventory, curr_inv );

			if( shop_changes.Count != 1 || inv_changes.Count != 1 ) { return; }

			int prev_item_idx = 0;
			Item prev_item = null;
			Item curr_item = null;

			foreach( var kv in shop_changes ) {
				if( shop_changes[kv.Key] ) { return; }
				prev_item = this.PrevShop[ kv.Key ];
				prev_item_idx = kv.Key;
			}
			foreach( var kv in inv_changes ) {
				if( !inv_changes[kv.Key] ) { return; }
				curr_item = curr_inv[kv.Key];
			}

			var prev_info = prev_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
			var curr_info = curr_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
			
			this.ForceCopy = prev_info;
		}
	}
}
