using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Utils;


namespace Durability {
	class DurabilityPlayer : ModPlayer {
		private long LastMoney = 0;
		private Item[] PrevInventory;
		private Item[] PrevShop;
		private DurabilityItemInfo ForceCopy = null;


		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );
			var myclone = (DurabilityPlayer)clone;

			myclone.LastMoney = this.LastMoney;
			myclone.PrevInventory = this.PrevInventory;
			myclone.PrevShop = this.PrevShop;
			myclone.ForceCopy = this.ForceCopy;
		}

		public override void OnEnterWorld( Player player ) {
			if( player == null ) { return; }	// Wtf #1

			if( player.whoAmI == this.player.whoAmI ) { // Current player
				if( !DurabilityMod.Config.Load() ) {
					DurabilityMod.Config.Save();
				}

				if( Main.netMode == 1 ) {   // Client
					DurabilityNetProtocol.RequestSettingsFromServer( this.mod, player );
				}
			}

			// Because crafting recipe items showing a durability bar is confusing
			for( int i = 0; i < Main.recipe.Length; i++ ) {
				if( Main.recipe[i] == null ) { continue; }	// Wtf #2

				Item craft_item = Main.recipe[i].createItem;
				if( craft_item == null || craft_item.IsAir ) { continue; }	// Wtf #3

				try {
					var info = craft_item.GetModInfo<DurabilityItemInfo>( this.mod );
					if( info == null ) { continue; }

					info.IsUnbreakable = true;
				} catch(Exception _) {}
			}
		}

		////////////////

		
		
		//public override void PostHurt( bool pvp, bool quiet, double damage, int hitDirection, bool crit ) {
		public override void Hurt( bool pvp, bool quiet, double damage, int hitDirection, bool crit ) {
			if( quiet ) { return; }

			damage = Main.CalculateDamage( (int)damage, this.player.statDefense );
			if( crit ) { damage *= 2; }

			int dmg = (int)(damage / 10);
			if( dmg <= 0 ) { dmg = 1; }

			Item armor1 = player.armor[0];
			if( armor1.type != 0 ) {
				DurabilityItemInfo armor1_info = armor1.GetModInfo<DurabilityItemInfo>(this.mod);
				armor1_info.Use( armor1, dmg );
			}

			Item armor2 = player.armor[1];
			if( armor2.type != 0 ) {
				DurabilityItemInfo armor2_info = armor2.GetModInfo<DurabilityItemInfo>(this.mod);
				armor2_info.Use( armor2, dmg );
			}

			Item armor3 = player.armor[2];
			if( armor3.type != 0 ) {
				DurabilityItemInfo armor3_info = armor3.GetModInfo<DurabilityItemInfo>(this.mod);
				armor3_info.Use( armor3, dmg );
			}
		}


		private void onHitWithProj( Player player, Projectile proj ) {
			Item item = player.inventory[player.selectedItem];
			if( item.type != 0 ) {
				if( item.melee && item.shoot == proj.type ) {
					DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(this.mod);
					info.Use( item ); //, 1, 0.5d );
				}
			}
		}

		public override void OnHitNPCWithProj( Projectile proj, NPC target, int damage, float knockback, bool crit ) {
			this.onHitWithProj( this.player, proj );
		}

		public override void OnHitPvpWithProj( Projectile proj, Player target, int damage, bool crit ) {
			this.onHitWithProj( this.player, proj );
		}

		public override void OnHitNPC( Item item, NPC target, int damage, float knockback, bool crit ) {
			if( item.type == 0 ) { return; }
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(this.mod);
			info.Use( item );
		}

		public override void OnHitPvp( Item item, Player target, int damage, bool crit ) {
			if( item.type == 0 ) { return; }
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(this.mod);
			info.Use( item );
		}

		public override void CatchFish( Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk ) {
			if( fishingRod.type == 0 ) { return; }
			DurabilityItemInfo info = fishingRod.GetModInfo<DurabilityItemInfo>(this.mod);
			info.Use( fishingRod, 1, 1 );
		}
		
		public override bool PreItemCheck() {
			Item item = player.inventory[player.selectedItem];
			if( item.type != 0 && !player.noItems ) {
				DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(this.mod);

				if( info.HasDurability(item) ) {
					if( !item.melee && player.HasAmmo(item, true) ) {   // || item.shoot > 0)
						if( (player.itemTime == 0 && player.controlUseItem && player.releaseUseItem) ||
							(player.itemTime == 1 && player.controlUseItem && item.autoReuse) ) {
							info.Use( item );
						}
					}
				}
			}

			return base.PreItemCheck();
		}

		public override void PreUpdate() {
			Item item = player.inventory[player.selectedItem];
			if( item.type != 0 ) {
				var info = item.GetModInfo<DurabilityItemInfo>(this.mod);
				info.ConcurrentUses = 0;
			}

			Item armor1 = player.armor[0];
			if( armor1.type != 0 ) {
				var info = armor1.GetModInfo<DurabilityItemInfo>(this.mod);
				info.ConcurrentUses = 0;
			}

			Item armor2 = player.armor[1];
			if( armor2.type != 0 ) {
				var info = armor2.GetModInfo<DurabilityItemInfo>(this.mod);
				info.ConcurrentUses = 0;
			}

			Item armor3 = player.armor[2];
			if( armor3.type != 0 ) {
				var info = armor3.GetModInfo<DurabilityItemInfo>(this.mod);
				info.ConcurrentUses = 0;
			}
			
			this.CheckPurchaseChanges();
		}

		////////////////

		private void CheckPurchaseChanges() {
			if( Main.netMode == 2 ) { return; }	// Not server
			if( this.player.whoAmI != Main.myPlayer ) { return; }	// Current player

			// Note: Due to a (tML?) bug, this method of copying is necessary
			if( this.ForceCopy != null ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir ) {
					var info = Main.mouseItem.GetModInfo<DurabilityItemInfo>( this.mod );
					info.CopyToMe( this.ForceCopy );
				}
				this.ForceCopy = null;
			}

			long money = PlayerHelper.CountMoney( this.player );
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

			var shop_changes = ItemHelper.FindChanges( this.PrevShop, curr_shop );
			var inv_changes = ItemHelper.FindChanges( this.PrevInventory, curr_inv );

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

			var prev_info = prev_item.GetModInfo<DurabilityItemInfo>( this.mod );
			var curr_info = curr_item.GetModInfo<DurabilityItemInfo>( this.mod );
//Main.NewText( "DONE! prev wear: "+ prev_info.WearAndTear+" ("+prev_item.type+"), curr wear: "+ curr_info.WearAndTear+" ("+curr_item.type+")");

			this.ForceCopy = prev_info;
		}
	}
}
