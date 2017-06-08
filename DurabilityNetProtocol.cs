﻿using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	public enum DurabilityNetProtocolTypes : byte {
		ModSettingsRequest,
		ModSettings
	}


	public class DurabilityNetProtocol {
		public static void RoutePacket( DurabilityMod mymod, BinaryReader reader ) {
			DurabilityNetProtocolTypes protocol = (DurabilityNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case DurabilityNetProtocolTypes.ModSettingsRequest:
				DurabilityNetProtocol.ReceiveSettingsRequestOnServer( mymod, reader );
				break;
			case DurabilityNetProtocolTypes.ModSettings:
				DurabilityNetProtocol.ReceiveSettingsOnClient( mymod, reader );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}



		////////////////////////////////
		// Senders (Client)
		////////////////////////////////

		public static void RequestSettingsFromServer( DurabilityMod mymod, Player player ) {
			// Clients only
			if( Main.netMode != 1 ) { return; }

			ModPacket packet = mymod.GetPacket();
			packet.Write( (byte)DurabilityNetProtocolTypes.ModSettingsRequest );
			packet.Write( (int)player.whoAmI );
			packet.Send();
		}

		////////////////////////////////
		// Senders (Server)
		////////////////////////////////

		public static void SendSettingsFromServer( DurabilityMod mymod, Player player ) {
			// Server only
			if( Main.netMode != 2 ) { return; }

			ModPacket packet = mymod.GetPacket();
			packet.Write( (byte)DurabilityNetProtocolTypes.ModSettings );
			packet.Write( (string)mymod.Config.SerializeMe() );

			packet.Send( (int)player.whoAmI );
		}



		////////////////////////////////
		// Recipients (Clients)
		////////////////////////////////

		private static void ReceiveSettingsOnClient( DurabilityMod mymod, BinaryReader reader ) {
			// Clients only
			if( Main.netMode != 1 ) { return; }

			mymod.Config.DeserializeMe( reader.ReadString() );
		}

		////////////////////////////////
		// Recipients (Server)
		////////////////////////////////

		private static void ReceiveSettingsRequestOnServer( DurabilityMod mymod, BinaryReader reader ) {
			// Server only
			if( Main.netMode != 2 ) { return; }

			int who = reader.ReadInt32();

			if( who < 0 || who >= Main.player.Length || Main.player[who] == null ) {
				ErrorLogger.Log( "DurabilityNetProtocol.ReceiveSettingsRequestOnServer - Invalid player whoAmI. " + who );
				return;
			}
			
			DurabilityNetProtocol.SendSettingsFromServer( mymod, Main.player[who] );
		}
	}
}
