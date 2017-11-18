using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Terraria;
using System.IO;
using HamstarHelpers.Utilities.Config;
using Microsoft.Xna.Framework.Graphics;
using Durability.NetProtocol;
using System;


namespace Durability {
	class DurabilityMod : Mod {
		public static DurabilityMod Instance { get; private set; }

		public static string GithubUserName { get { return "hamstar0"; } }
		public static string GithubProjectName { get { return "tml-durability-mod"; } }

		public static string ConfigFileRelativePath {
			get { return ConfigurationDataBase.RelativePath + Path.DirectorySeparatorChar + DurabilityConfigData.ConfigFileName; }
		}
		public static void ReloadConfigFromFile() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reload configs outside of single player." );
			}
			if( DurabilityMod.Instance != null ) {
				DurabilityMod.Instance.Config.LoadFile();
			}
		}


		////////////////

		public JsonConfig<DurabilityConfigData> Config { get; private set; }
		public Texture2D DestroyedTex { get; private set; }


		////////////////

		public DurabilityMod() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			this.DestroyedTex = null;
			this.Config = new JsonConfig<DurabilityConfigData>( DurabilityConfigData.ConfigFileName, ConfigurationDataBase.RelativePath, new DurabilityConfigData() );
		}

		////////////////

		public override void Load() {
			DurabilityMod.Instance = this;

			var hamhelpmod = ModLoader.GetMod( "HamstarHelpers" );
			var min_vers = new Version( 1, 2, 0 );
			if( hamhelpmod.Version < min_vers ) {
				throw new Exception( "Hamstar Helpers must be version " + min_vers.ToString() + " or greater." );
			}

			if( Main.netMode != 2 ) {   // Not server
				this.DestroyedTex = ModLoader.GetTexture( "Terraria/MapDeath" );
			}

			this.LoadConfig();
			AchievementsHelper.OnTileDestroyed += this.MyOnTileDestroyedEvent;
		}
		
		private void LoadConfig() {
			// Update old config to new location
			var old_config = new JsonConfig<DurabilityConfigData>( "Durability 1.6.0.json", "", new DurabilityConfigData() );
			if( old_config.LoadFile() ) {
				old_config.DestroyFile();
				old_config.SetFilePath( this.Config.FileName, ConfigurationDataBase.RelativePath );
				old_config.Data.VersionSinceUpdate = "1.6.0";
				old_config.SaveFile();
				this.Config = old_config;
			}

			if( !this.Config.LoadFile() ) {
				this.Config.SaveFile();
			}

			if( this.Config.Data.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Durability updated to " + DurabilityConfigData.ConfigVersion.ToString() );
				this.Config.SaveFile();
			}
		}

		public override void Unload() {
			DurabilityMod.Instance = null;
			AchievementsHelper.OnTileDestroyed -= this.MyOnTileDestroyedEvent;
		}
		

		////////////////

		public override void HandlePacket( BinaryReader reader, int who_am_i ) {
			if( Main.netMode == 1 ) {
				ClientPacketHandlers.HandlePacket( this, reader );
			} else if( Main.netMode == 2 ) {
				ServerPacketHandlers.HandlePacket( this, reader, who_am_i );
			}
		}



		////////////////

		public void MyOnTileDestroyedEvent( Player player, ushort item_id ) {
			if( !this.Config.Data.Enabled ) { return; }

			if( player.itemAnimation > 0 && player.toolTime == 0 && player.controlUseItem ) {
				Item item = player.inventory[ player.selectedItem ];

				if( item != null && !item.IsAir ) {
					if( item.pick > 0 || item.axe > 0 || item.hammer > 0 ) {
						MyItemInfo item_info = item.GetGlobalItem<MyItemInfo>( this );
						item_info.AddWearAndTear( this, item, 1, this.Config.Data.ToolWearAndTearMultiplier );
					}
				}
			}
		}
	}
}
