using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.DotNET.Extensions;


namespace Durability.Helpers.Items {
	public partial class ItemFinderHelpers {
		/// <summary>
		/// Finds changes between 2 same-sized arrays of items.
		/// </summary>
		/// <param name="prevItems"></param>
		/// <param name="currItems"></param>
		/// <param name="skipCoins"></param>
		/// <returns>Set of array indices of changed items between the 2 collections. Mapped to a "direction" indicator of
		/// changes.</returns>
		public static IDictionary<int, int> FindChanges( Item[] prevItems, Item[] currItems, bool skipCoins=true ) {
			if( prevItems.Length != currItems.Length ) {
				throw new ModHelpersException( "Mismatched item array sizes." );
			}

			IDictionary<int, int> changes = new Dictionary<int, int>();
			int len = currItems.Length;
			Item prevItem, currItem;

			for( int i = 0; i < len; i++ ) {
				prevItem = prevItems[i];
				currItem = currItems[i];
				bool prevItemOn = prevItem?.IsAir == false;
				bool currItemOn = currItem?.IsAir == false;

				if( prevItemOn != currItemOn ) {
					changes[i] = currItemOn
						? 1
						: -1;

					continue;
				} else if( !currItemOn ) {	// both not active
					continue;
				}

				if( prevItem.type != currItem.type || prevItem.stack != currItem.stack ) {
					changes[i] = prevItem.stack < currItem.stack
						? 1
						: -1;
				}
			}

			if( skipCoins ) {
				foreach( (int itemIdx, int changeDir) in changes.ToArray() ) {
					currItem = currItems[itemIdx];
					prevItem = prevItems[itemIdx];
					bool prevItemOn = prevItem?.IsAir == false;
					bool currItemOn = currItem?.IsAir == false;

					if( !currItemOn ) {	// is not active
						if( prevItemOn ) {
							if( prevItem.type >= ItemID.CopperCoin && prevItem.type <= ItemID.PlatinumCoin ) {	// was money
								changes.Remove( itemIdx );
							}
						}
					} else {	// is active
						if( currItem.type >= ItemID.CopperCoin || currItem.type <= ItemID.PlatinumCoin ) {	// is money
							// was not active or was money:
							if( !prevItemOn || (prevItem.type >= ItemID.CopperCoin && prevItem.type <= ItemID.PlatinumCoin) ) {
								changes.Remove( itemIdx );
							}
						}
					}
				}
			}

			return changes;
		}
	}
}
