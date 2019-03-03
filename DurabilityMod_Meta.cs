using Terraria.ModLoader;
using Terraria;
using System.IO;
using System;
using HamstarHelpers.Components.Config;


namespace Durability {
	partial class DurabilityMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-durability-mod";

		public static string ConfigFileRelativePath {
			get { return ConfigurationDataBase.RelativePath + Path.DirectorySeparatorChar + DurabilityConfigData.ConfigFileName; }
		}
		public static void ReloadConfigFromFile() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reload configs outside of single player." );
			}
			if( DurabilityMod.Instance != null ) {
				if( !DurabilityMod.Instance.ConfigJson.LoadFile() ) {
					DurabilityMod.Instance.ConfigJson.SaveFile();
				}
			}
		}

		public static void ResetConfigFromDefaults() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reset to default configs outside of single player." );
			}

			var newConfig = new DurabilityConfigData();
			newConfig.SetDefaults();

			DurabilityMod.Instance.ConfigJson.SetData( newConfig );
			DurabilityMod.Instance.ConfigJson.SaveFile();
		}
	}
}
