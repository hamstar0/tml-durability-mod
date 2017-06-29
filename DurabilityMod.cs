using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Terraria;
using System.IO;
using HamstarHelpers.Utilities.Config;
using Microsoft.Xna.Framework.Graphics;


namespace Durability {
	public class DurabilityMod : Mod {
		public JsonConfig<ConfigurationData> Config { get; private set; }
		public Texture2D DestroyedTex = null;


		public DurabilityMod() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			string filename = "Durability Config.json";
			this.Config = new JsonConfig<ConfigurationData>( filename, "Mod Configs", new ConfigurationData() );
		}


		private void LoadConfig() {
			// Update old config to new location
			var old_config = new JsonConfig<ConfigurationData>( "Durability 1.6.0.json", "", new ConfigurationData() );
			if( old_config.LoadFile() ) {
				old_config.DestroyFile();
				old_config.SetFilePath( this.Config.FileName, "Mod Configs" );
				old_config.Data.VersionSinceUpdate = "1.6.0";
				old_config.SaveFile();
			}

			if( !this.Config.LoadFile() ) {
				this.Config.SaveFile();
			}

			if( this.Config.Data.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Durability updated to " + ConfigurationData.CurrentVersion.ToString() );
				this.Config.SaveFile();
			}
		}


		public override void Load() {
			if( Main.netMode != 2 ) {   // Not server
				this.DestroyedTex = ModLoader.GetTexture( "Terraria/MapDeath" );
			}

			this.LoadConfig();
			AchievementsHelper.OnTileDestroyed += this.MyOnTileDestroyedEvent;
		}

		public override void Unload() {
			AchievementsHelper.OnTileDestroyed -= this.MyOnTileDestroyedEvent;
		}



		////////////////

		public override void HandlePacket( BinaryReader reader, int whoAmI ) {
			DurabilityNetProtocol.RoutePacket( this, reader );
		}


		////////////////

		public void MyOnTileDestroyedEvent( Player player, ushort item_id ) {
			if( !this.Config.Data.Enabled ) { return; }

			if( player.itemAnimation > 0 && player.toolTime == 0 && player.controlUseItem ) {
				Item item = player.inventory[ player.selectedItem ];

				if( item != null && !item.IsAir ) {
					if( item.pick > 0 || item.axe > 0 || item.hammer > 0 ) {
						DurabilityItemInfo item_info = item.GetGlobalItem<DurabilityItemInfo>( this );
						item_info.AddWearAndTear( this, item, 1, this.Config.Data.ToolWearAndTearMultiplier );
					}
				}
			}
		}
	}
}
