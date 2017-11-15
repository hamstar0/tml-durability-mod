using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Durability.NetProtocol {
	static class ServerPacketHandlers {
		public static void HandlePacket( DurabilityMod mymod, BinaryReader reader, int who_am_i ) {
			DurabilityNetProtocolTypes protocol = (DurabilityNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case DurabilityNetProtocolTypes.ModSettingsRequest:
				ServerPacketHandlers.ReceiveSettingsRequestOnServer( mymod, reader, who_am_i );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}


		
		////////////////////////////////
		// Senders (Server)
		////////////////////////////////

		public static void SendSettingsFromServer( DurabilityMod mymod, Player player ) {
			if( Main.netMode != 2 ) { return; }	// Server only

			ModPacket packet = mymod.GetPacket();
			packet.Write( (byte)DurabilityNetProtocolTypes.ModSettings );
			packet.Write( (string)mymod.Config.SerializeMe() );

			packet.Send( (int)player.whoAmI );
		}


		
		////////////////////////////////
		// Recipients (Server)
		////////////////////////////////

		private static void ReceiveSettingsRequestOnServer( DurabilityMod mymod, BinaryReader reader, int who_am_i ) {
			if( Main.netMode != 2 ) { return; } // Server only

			ServerPacketHandlers.SendSettingsFromServer( mymod, Main.player[who_am_i] );
		}
	}
}
