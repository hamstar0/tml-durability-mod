using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Utils {
	// Ugliest hack ever!
	class ItemSetChangeTracker {
		private Item[] PrevItemSet;

		private Mod Mod;

		public IDictionary<int, KeyValuePair<bool, Item>> Changes { get; private set; }



		public ItemSetChangeTracker( Mod mod ) {
			this.Mod = mod;
		}
		
		public void FindChanges( Item[] items ) {
			// If shop opened anew, setup
			if( this.PrevItemSet == null ) {
				this.PrevItemSet = new Item[items.Length];
			} else {
				for( int i = 0; i < items.Length; i++ ) {
					Item prev = this.PrevItemSet[i];
					Item curr = items[i];

					if( prev != null && prev.type == curr.type && prev.stack == curr.stack ) {
						continue;
					}

					if( this.Changes == null ) {
						this.Changes = new Dictionary<int, KeyValuePair<bool, Item>>( 1 );
					}
					
					if( prev != null && prev.type != 0 ) {
						if( curr.type == 0 || prev.stack > curr.stack ) {
							this.Changes[prev.type] = new KeyValuePair<bool, Item>( false, prev.Clone() );
						} else {
							this.Changes[curr.type] = new KeyValuePair<bool, Item>( true, curr.Clone() );
						}
					} else {
						if( curr != null && curr.type != 0 && curr.stack != 0 ) {
							this.Changes[curr.type] = new KeyValuePair<bool, Item>( true, curr.Clone() );
						}
					}
				}
			}

			// Refresh list
			for( int i = 0; i < items.Length; i++ ) {
				this.PrevItemSet[i] = items[i];
			}
		}

		public IDictionary<int, KeyValuePair<bool, Item>> Compare( ItemSetChangeTracker tracker ) {
			var intersect = new Dictionary<int, KeyValuePair<bool, Item>>();
			
			var matches = this.Changes.Keys.Intersect( tracker.Changes.Keys );
			foreach( var m in matches ) {
				intersect.Add( m, this.Changes[m] );
			}
			return intersect;
		}
	}
}
