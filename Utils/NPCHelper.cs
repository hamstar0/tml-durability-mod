using Terraria;
using Terraria.ModLoader;

namespace Utils {
	public static class NPCHelper {
		public static Chest GetShop( int npc_type ) {
			if( Main.instance == null ) {
				ErrorLogger.Log( "No main instance." );
				return null;
			}

			switch( npc_type ) {
			case 17:
				return Main.instance.shop[1];
			case 19:
				return Main.instance.shop[2];
			case 20:
				return Main.instance.shop[3];
			case 38:
				return Main.instance.shop[4];
			case 54:
				return Main.instance.shop[5];
			case 107:
				return Main.instance.shop[6];
			case 108:
				return Main.instance.shop[7];
			case 124:
				return Main.instance.shop[8];
			case 142:
				return Main.instance.shop[9];
			case 160:
				return Main.instance.shop[10];
			case 178:
				return Main.instance.shop[11];
			case 207:
				return Main.instance.shop[12];
			case 208:
				return Main.instance.shop[13];
			case 209:
				return Main.instance.shop[14];
			case 227:
				return Main.instance.shop[15];
			case 228:
				return Main.instance.shop[16];
			case 229:
				return Main.instance.shop[17];
			case 353:
				return Main.instance.shop[18];
			case 368:
				return Main.instance.shop[19];
			case 453:
				return Main.instance.shop[20];
			case 550:
				return Main.instance.shop[21];
			}

			return null;
		}
	}
}
