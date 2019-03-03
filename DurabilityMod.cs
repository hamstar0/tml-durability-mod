using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Durability.NetProtocol;
using System;
using HamstarHelpers.Components.Config;
using HamstarHelpers.Helpers.TmlHelpers.ModHelpers;
using HamstarHelpers.Helpers.TmlHelpers;
using HamstarHelpers.Components.Errors;


namespace Durability {
	partial class DurabilityMod : Mod {
		public static DurabilityMod Instance { get; private set; }
		


		////////////////

		public JsonConfig<DurabilityConfigData> ConfigJson { get; private set; }
		public DurabilityConfigData Config => this.ConfigJson.Data;

		public Texture2D DestroyedTex { get; private set; }



		////////////////

		public DurabilityMod() {
			this.DestroyedTex = null;
			this.ConfigJson = new JsonConfig<DurabilityConfigData>( DurabilityConfigData.ConfigFileName, ConfigurationDataBase.RelativePath,
				new DurabilityConfigData() );
		}

		////////////////

		public override void Load() {
			string depErr = TmlHelpers.ReportBadDependencyMods( this );
			if( depErr != null ) { throw new HamstarException( depErr ); }

			DurabilityMod.Instance = this; 

			if( Main.netMode != 2 ) {   // Not server
				this.DestroyedTex = ModLoader.GetTexture( "Terraria/MapDeath" );
			}

			this.LoadConfig();
			AchievementsHelper.OnTileDestroyed += this.MyOnTileDestroyedEvent;
		}
		
		private void LoadConfig() {
			if( !this.ConfigJson.LoadFile() ) {
				this.ConfigJson.SaveFile();
			}

			if( this.Config.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Durability updated to " + this.Version.ToString() );
				this.ConfigJson.SaveFile();
			}
		}

		public override void Unload() {
			DurabilityMod.Instance = null;
			AchievementsHelper.OnTileDestroyed -= this.MyOnTileDestroyedEvent;
		}
		

		////////////////

		public override void HandlePacket( BinaryReader reader, int whoAmI ) {
			if( Main.netMode == 1 ) {
				ClientPacketHandlers.HandlePacket( reader );
			} else if( Main.netMode == 2 ) {
				ServerPacketHandlers.HandlePacket( reader, whoAmI );
			}
		}


		////////////////

		public override object Call( params object[] args ) {
			return ModBoilerplateHelpers.HandleModCall( typeof(DurabilityAPI), args );
		}


		////////////////

		public void MyOnTileDestroyedEvent( Player player, ushort itemId ) {
			if( !this.Config.Enabled ) { return; }

			if( player.itemAnimation > 0 && player.toolTime == 0 && player.controlUseItem ) {
				Item item = player.inventory[ player.selectedItem ];

				if( item != null && !item.IsAir ) {
					if( item.pick > 0 || item.axe > 0 || item.hammer > 0 ) {
						DurabilityItemInfo itemInfo = item.GetGlobalItem<DurabilityItemInfo>();
						itemInfo.AddWearAndTear( item, 1, this.Config.ToolWearAndTearMultiplier );
					}
				}
			}
		}
	}
}
