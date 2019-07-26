using HamstarHelpers.Helpers.Items;
using HamstarHelpers.Helpers.Players;
using System.Linq;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	partial class DurabilityPlayer : ModPlayer {
		public override void PreUpdate() {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			Player player = this.player;

			Item currItem = player.inventory[player.selectedItem];
			Item headItem = player.armor[0];
			Item bodyItem = player.armor[1];
			Item legsItem = player.armor[2];

			if( currItem != null && !currItem.IsAir ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir ) { currItem = Main.mouseItem; }

				var itemInfo = currItem.GetGlobalItem<DurabilityItemInfo>();
				itemInfo.ConcurrentUses = 0;
				if( !itemInfo.IsBroken ) {
					itemInfo.UpdateCriticalState( currItem );
				}
			}

			if( !headItem.IsAir ) {
				var head_item_info = headItem.GetGlobalItem<DurabilityItemInfo>();
				head_item_info.ConcurrentUses = 0;

				if( head_item_info.IsBroken ) {
					int who = ItemHelpers.CreateItem( player.position, headItem.type, 1, headItem.width, headItem.height, headItem.prefix );
					ItemHelpers.DestroyItem( headItem );

					var newItem = Main.item[who];
					var newItemInfo = newItem.GetGlobalItem<DurabilityItemInfo>();
					newItemInfo.KillMe( newItem );
					player.armor[0] = new Item();
				}
			}

			if( !bodyItem.IsAir ) {
				var bodyItemInfo = bodyItem.GetGlobalItem<DurabilityItemInfo>();
				bodyItemInfo.ConcurrentUses = 0;

				if( bodyItemInfo.IsBroken ) {
					int who = ItemHelpers.CreateItem( player.position, bodyItem.type, 1, bodyItem.width, bodyItem.height, bodyItem.prefix );
					ItemHelpers.DestroyItem( bodyItem );

					var newItem = Main.item[who];
					var newItemInfo = newItem.GetGlobalItem<DurabilityItemInfo>();
					newItemInfo.KillMe( newItem );
					player.armor[1] = new Item();
				}
			}

			if( !legsItem.IsAir ) {
				var legsItemInfo = legsItem.GetGlobalItem<DurabilityItemInfo>();
				legsItemInfo.ConcurrentUses = 0;

				if( legsItemInfo.IsBroken ) {
					int who = ItemHelpers.CreateItem( player.position, legsItem.type, 1, legsItem.width, legsItem.height, legsItem.prefix );
					ItemHelpers.DestroyItem( legsItem );

					var newItem = Main.item[who];
					var newItemInfo = newItem.GetGlobalItem<DurabilityItemInfo>();
					newItemInfo.KillMe( newItem );
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
					var itemInfo = Main.mouseItem.GetGlobalItem<DurabilityItemInfo>();
					itemInfo.CopyToMe( this.ForceCopy );
				}
				this.ForceCopy = null;
			}

			long money = PlayerItemHelpers.CountMoney( this.player, false );
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
				if( this.player.inventory[i] == null || this.player.inventory[i].IsAir ) {
					this.PrevInventory[i] = new Item();
				} else {
					this.PrevInventory[i] = this.player.inventory[i].Clone();
				}
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
			Item[] currShop = Main.instance.shop[Main.npcShop].item;
			Item[] currInv = (Item[])this.player.inventory.Clone();
			currInv = currInv.Concat<Item>( new Item[] { Main.mouseItem.Clone() } ).ToArray();

			var shopChanges = ItemFinderHelpers.FindChanges( this.PrevShop, currShop );
			var invChanges = ItemFinderHelpers.FindChanges( this.PrevInventory, currInv );

			if( shopChanges.Count != 1 || invChanges.Count != 1 ) { return; }

			int prevItemIdx = 0;
			Item prevItem = null;
			Item currItem = null;

			foreach( var kv in shopChanges ) {
				int itemIdx = kv.Key;
				int changeAmt = kv.Value;

				if( changeAmt < 0 ) { return; }
				prevItem = this.PrevShop[itemIdx];
				prevItemIdx = itemIdx;
			}
			foreach( var kv in invChanges ) {
				int itemIdx = kv.Key;
				int changeAmt = kv.Value;

				if( changeAmt >= 0 ) { return; }
				currItem = currInv[itemIdx];
			}

			var prevInfo = prevItem.GetGlobalItem<DurabilityItemInfo>();
			var currInfo = currItem.GetGlobalItem<DurabilityItemInfo>();
			
			this.ForceCopy = prevInfo;
		}
	}
}
