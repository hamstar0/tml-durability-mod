using System.Collections.Generic;
using Terraria;

namespace Utils {
	public static class ItemHelper {
		public static bool IsTool( Item item ) {
			return (item.useStyle > 0 ||
					item.damage > 0 ||
					item.crit > 0 ||
					item.knockBack > 0 ||
					item.melee ||
					item.magic ||
					item.ranged ||
					item.thrown ||
					item.summon ||
					item.pick > 0 ||
					item.hammer > 0 ||
					item.axe > 0) &&
				!item.accessory &&
				!item.potion &&
				!item.consumable &&
				!item.vanity;
		}

		public static bool IsArmor( Item item ) {
			return (item.headSlot != -1 ||
					item.bodySlot != -1 ||
					item.legSlot != -1) &&
				!item.accessory &&
				!item.potion &&
				!item.consumable &&
				!item.vanity;
		}

		public static bool IsGrapple( Item item ) {
			return Main.projHook[ item.shoot ];
		}


		public static Item FindFirstPlayerItemOfType( Player player, int item_type ) {
			Item item = ItemHelper.FindFirstItemOfType( player.inventory, item_type );
			if( item != null ) { return item; }

			if( player.chest >= 0 ) {	// Player's current chest
				item = ItemHelper.FindFirstItemOfType( Main.chest[player.chest].item, item_type );
				if( item != null ) { return item; }
			}
			if( player.chest == -2 ) {  // Piggy bank
				item = ItemHelper.FindFirstItemOfType( player.bank.item, item_type );
				if( item != null ) { return item; }
			}
			if( player.chest == -3 ) {  // Safe
				item = ItemHelper.FindFirstItemOfType( player.bank2.item, item_type );
				if( item != null ) { return item; }
			}
			if( player.chest == -4 ) {  // ..whatever this is
				item = ItemHelper.FindFirstItemOfType( player.bank3.item, item_type );
				if( item != null ) { return item; }
			}

			return null;
		}

		public static Item FindFirstItemOfType( Item[] items, int item_type ) {
			for( int i = 0; i < items.Length; i++ ) {
				Item item = items[i];
				if( item.type == item_type && item.stack > 0 ) {
					return item;
				}
			}
			return null;
		}


		public static IDictionary<int, bool> FindChanges( Item[] prev_items, Item[] curr_items ) {
			IDictionary<int, bool> changes = new Dictionary<int, bool>();
			int len = curr_items.Length;

			for( int i = 0; i < len; i++ ) {
				Item prev_item = prev_items[i];
				Item curr_item = curr_items[i];

				if( prev_item == null ) {
					if( curr_item != null ) { changes[i] = true; }
					continue;
				} else if( curr_item == null ) {
					if( prev_item != null ) { changes[i] = false; }
					continue;
				}

				if( prev_item.type != curr_item.type || prev_item.stack != curr_item.stack ) {
					changes[i] = prev_item.type == 0 || prev_item.stack < curr_item.stack;

					// Skip coins
					if( changes[i] ) {
						if( curr_item.type == 71 || curr_item.type == 72
						|| curr_item.type == 73 || curr_item.type == 74 ) {
							changes.Remove( i );
						}
					} else {
						if( prev_item.type == 71 || prev_item.type == 72
						|| prev_item.type == 73 || prev_item.type == 74 ) {
							changes.Remove( i );
						}
					}
				}
			}

			return changes;
		}


		private static IDictionary<int, int> ProjPene = new Dictionary<int, int>();

		public static bool IsPenetrator( Item item ) {
			if( item.shoot <= 0 ) { return false; }

			if( !ItemHelper.ProjPene.Keys.Contains( item.shoot ) ) {
				var proj = new Projectile();
				proj.SetDefaults( item.shoot );
				ItemHelper.ProjPene[item.shoot] = proj.penetrate;
			}

			return ItemHelper.ProjPene[item.shoot] == -1 || ItemHelper.ProjPene[item.shoot] >= 3;	// 3 seems fair?
		}


		public static int CalculateStandardUseTime( Item item ) {
			int use_time;

			// No exact science for this one (Note: No accommodations made for other mods' non-standard use of useTime!)
			if( item.melee || item.useTime == 0 ) {
				use_time = item.useAnimation;
			} else {
				use_time = item.useTime;
				if( item.reuseDelay > 0 ) { use_time = (use_time + item.reuseDelay) / 2; }

				if( item.useTime <= 0 || item.useTime == 100 ) {    // 100 = default amount
					if( item.useAnimation > 0 && item.useAnimation != 100 ) {   // 100 = default amount
						use_time = item.useAnimation;
					}
				}
			}

			return use_time;
		}


		public static Item GetSelectedItem( Player player ) {
			if( Main.mouseItem != null && !Main.mouseItem.IsAir ) {
				return Main.mouseItem;
			}
			return player.inventory[player.selectedItem];
		}


		public static Item GetGrappleItem( Player player ) {
			if( ItemHelper.IsGrapple( player.miscEquips[4] ) ) {
				return player.miscEquips[4];
			}
			for( int i = 0; i < 58; i++ ) {
				if( Main.projHook[ player.inventory[i].shoot ] ) {
					return player.inventory[i];
				}
			}
			return null;
		}
	}
}
