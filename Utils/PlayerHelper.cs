using System.Collections.Generic;
using Terraria;

namespace Utils {
	public static class PlayerHelper {
		public static long CountMoney( Player player ) {
			bool _;
			long inv_count = Terraria.Utils.CoinsCount( out _, player.inventory, new int[] { 58, 57, 56, 55, 54 } );
			long bank_count = Terraria.Utils.CoinsCount( out _, player.bank.item, new int[0] );
			long bank2_count = Terraria.Utils.CoinsCount( out _, player.bank2.item, new int[0] );
			long bank3_count = Terraria.Utils.CoinsCount( out _, player.bank3.item, new int[0] );
			return Terraria.Utils.CoinsCombineStacks( out _, new long[] { inv_count, bank_count, bank2_count, bank3_count } );
		}
	}
}
