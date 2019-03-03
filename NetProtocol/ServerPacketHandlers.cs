using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Durability.NetProtocol {
	static class ServerPacketHandlers {
		public static void HandlePacket( BinaryReader reader, int whoAmI ) {
			DurabilityNetProtocolTypes protocol = (DurabilityNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case DurabilityNetProtocolTypes.ModSettingsRequest:
				ServerPacketHandlers.ReceiveSettingsRequestOnServer( reader, whoAmI );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}


		
		////////////////////////////////
		// Senders (Server)
		////////////////////////////////

		public static void SendSettingsFromServer( Player player ) {
			if( Main.netMode != 2 ) { return; } // Server only

			var mymod = DurabilityMod.Instance;
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)DurabilityNetProtocolTypes.ModSettings );
			packet.Write( (string)mymod.ConfigJson.SerializeMe() );

			packet.Send( (int)player.whoAmI );
		}


		
		////////////////////////////////
		// Recipients (Server)
		////////////////////////////////

		private static void ReceiveSettingsRequestOnServer( BinaryReader reader, int whoAmI ) {
			if( Main.netMode != 2 ) { return; } // Server only

			var mymod = DurabilityMod.Instance;
			ServerPacketHandlers.SendSettingsFromServer( Main.player[whoAmI] );
		}
	}
}
