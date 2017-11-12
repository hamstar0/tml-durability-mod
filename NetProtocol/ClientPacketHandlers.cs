using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Durability.NetProtocol {
	public static class ClientPacketHandlers {
		public static void RoutePacket( DurabilityMod mymod, BinaryReader reader ) {
			DurabilityNetProtocolTypes protocol = (DurabilityNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case DurabilityNetProtocolTypes.ModSettings:
				ClientPacketHandlers.ReceiveSettingsOnClient( mymod, reader );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}



		////////////////////////////////
		// Senders (Client)
		////////////////////////////////

		public static void SendSettingsRequestFromClient( DurabilityMod mymod, Player player ) {
			// Clients only
			if( Main.netMode != 1 ) { return; }

			ModPacket packet = mymod.GetPacket();
			packet.Write( (byte)DurabilityNetProtocolTypes.ModSettingsRequest );
			packet.Send();
		}



		////////////////////////////////
		// Recipients (Clients)
		////////////////////////////////

		private static void ReceiveSettingsOnClient( DurabilityMod mymod, BinaryReader reader ) {
			// Clients only
			if( Main.netMode != 1 ) { return; }

			mymod.Config.DeserializeMe( reader.ReadString() );
		}
	}
}
