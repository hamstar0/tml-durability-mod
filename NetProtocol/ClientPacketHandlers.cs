using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Durability.NetProtocol {
	static class ClientPacketHandlers {
		public static void HandlePacket( BinaryReader reader ) {
			DurabilityNetProtocolTypes protocol = (DurabilityNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case DurabilityNetProtocolTypes.ModSettings:
				ClientPacketHandlers.ReceiveSettingsOnClient( reader );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}



		////////////////////////////////
		// Senders (Client)
		////////////////////////////////

		public static void SendSettingsRequestFromClient( Player player ) {
			// Clients only
			if( Main.netMode != 1 ) { return; }

			var mymod = DurabilityMod.Instance;
			ModPacket packet = mymod.GetPacket();
			packet.Write( (byte)DurabilityNetProtocolTypes.ModSettingsRequest );
			packet.Send();
		}



		////////////////////////////////
		// Recipients (Clients)
		////////////////////////////////

		private static void ReceiveSettingsOnClient( BinaryReader reader ) {
			// Clients only
			if( Main.netMode != 1 ) { return; }

			var mymod = DurabilityMod.Instance;
			bool success;

			mymod.ConfigJson.DeserializeMe( reader.ReadString(), out success );
		}
	}
}
