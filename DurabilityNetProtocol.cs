using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Durability {
	public enum DurabilityNetProtocolTypes : byte {
		SendSettingsRequest,
		SendSettings
	}


	public class DurabilityNetProtocol {
		public static void RoutePacket( Mod mod, BinaryReader reader ) {
			DurabilityNetProtocolTypes protocol = (DurabilityNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case DurabilityNetProtocolTypes.SendSettingsRequest:
				DurabilityNetProtocol.ReceiveSettingsRequestOnServer( mod, reader );
				break;
			case DurabilityNetProtocolTypes.SendSettings:
				DurabilityNetProtocol.ReceiveSettingsOnClient( mod, reader );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}



		////////////////////////////////
		// Senders (Client)
		////////////////////////////////

		public static void RequestSettingsFromServer( Mod mod, Player player ) {
			// Clients only
			if( Main.netMode != 1 ) { return; }

			ModPacket packet = mod.GetPacket();
			packet.Write( (byte)DurabilityNetProtocolTypes.SendSettingsRequest );
			packet.Write( (int)player.whoAmI );
			packet.Send();
		}

		////////////////////////////////
		// Senders (Server)
		////////////////////////////////

		public static void SendSettingsFromServer( Mod mod, Player player ) {
			// Server only
			if( Main.netMode != 2 ) { return; }

			ModPacket packet = mod.GetPacket();
			packet.Write( (byte)DurabilityNetProtocolTypes.SendSettings );

			packet.Write( (float)DurabilityMod.Config.Data.WearMultiplier );
			packet.Write( (int)DurabilityMod.Config.Data.DurabilityAdditive );
			packet.Write( (float)DurabilityMod.Config.Data.DurabilityMultiplier );
			packet.Write( (float)DurabilityMod.Config.Data.DurabilityExponent );
			packet.Write( (bool)DurabilityMod.Config.Data.CanRepair );
			packet.Write( (int)DurabilityMod.Config.Data.RepairAmount );
			packet.Write( (float)DurabilityMod.Config.Data.ToolUseMultiplier );
			packet.Write( (float)DurabilityMod.Config.Data.ArmorDurabilityMultiplier );
			packet.Write( (float)DurabilityMod.Config.Data.ToolDurabilityMultiplier );
			packet.Write( (int)DurabilityMod.Config.Data.MaxMeteors );
			packet.Write( (bool)DurabilityMod.Config.Data.ShowNumbers );
			packet.Write( (bool)DurabilityMod.Config.Data.ShowBar );
			packet.Write( (float)DurabilityMod.Config.Data.RepairDegradationMultiplier );
			packet.Write( (int)DurabilityMod.Config.Data.CustomDurabilityMultipliers.Count );

			foreach( var kv in DurabilityMod.Config.Data.CustomDurabilityMultipliers ) {
				packet.Write( (string)kv.Key );
				packet.Write( (float)kv.Value );
			}

			packet.Send( (int)player.whoAmI );
		}



		////////////////////////////////
		// Recipients (Clients)
		////////////////////////////////

		private static void ReceiveSettingsOnClient( Mod mod, BinaryReader reader ) {
			// Clients only
			if( Main.netMode != 1 ) { return; }

			float wear_mul = (float)reader.ReadSingle();
			int dura_add = (int)reader.ReadInt32();
			float dura_mul = (float)reader.ReadSingle();
			float dura_exp = (float)reader.ReadSingle();
			bool can_repair = (bool)reader.ReadBoolean();
			int repair_amt = (int)reader.ReadInt32();
			float tool_use_mul = (float)reader.ReadSingle();
			float armor_dura_mul = (float)reader.ReadSingle();
			float tool_dura_mul = (float)reader.ReadSingle();
			int max_meteors = (int)reader.ReadInt32();
			bool show_numbers = (bool)reader.ReadBoolean();
			bool show_bar = (bool)reader.ReadBoolean();
			float repair_deg_mul = (float)reader.ReadSingle();
			int customs = (int)reader.ReadInt32();

			DurabilityMod.Config.Data.WearMultiplier = wear_mul;
			DurabilityMod.Config.Data.DurabilityAdditive = dura_add;
			DurabilityMod.Config.Data.DurabilityMultiplier = dura_mul;
			DurabilityMod.Config.Data.DurabilityExponent = dura_exp;
			DurabilityMod.Config.Data.CanRepair = can_repair;
			DurabilityMod.Config.Data.RepairAmount = repair_amt;
			DurabilityMod.Config.Data.ToolUseMultiplier = tool_use_mul;
			DurabilityMod.Config.Data.ArmorDurabilityMultiplier = armor_dura_mul;
			DurabilityMod.Config.Data.ToolDurabilityMultiplier = tool_dura_mul;
			DurabilityMod.Config.Data.MaxMeteors = max_meteors;
			DurabilityMod.Config.Data.ShowNumbers = show_numbers;
			DurabilityMod.Config.Data.ShowBar = show_bar;
			DurabilityMod.Config.Data.RepairDegradationMultiplier = repair_deg_mul;
			DurabilityMod.Config.Data.CustomDurabilityMultipliers = new Dictionary<string, float>( customs );

			for( int i=0; i<customs; i++ ) {
				string item_name = reader.ReadString();
				float item_mul = reader.ReadSingle();
				DurabilityMod.Config.Data.CustomDurabilityMultipliers[ item_name ] = item_mul;
			}
		}

		////////////////////////////////
		// Recipients (Server)
		////////////////////////////////

		private static void ReceiveSettingsRequestOnServer( Mod mod, BinaryReader reader ) {
			// Server only
			if( Main.netMode != 2 ) { return; }

			int who = reader.ReadInt32();

			if( who < 0 || who >= Main.player.Length || Main.player[who] == null ) {
				ErrorLogger.Log( "DurabilityNetProtocol.ReceiveSettingsRequestOnServer - Invalid player whoAmI. " + who );
				return;
			}
			
			DurabilityNetProtocol.SendSettingsFromServer( mod, Main.player[who] );
		}
	}
}
