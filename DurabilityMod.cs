using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using System;
using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.TModLoader.Mods;


namespace Durability {
	partial class DurabilityMod : Mod {
		public static DurabilityMod Instance { get; private set; }



		////////////////

		public DurabilityConfig Config => this.GetConfig<DurabilityConfig>();

		public Texture2D DestroyedTex { get; private set; }



		////////////////

		public DurabilityMod() {
			this.DestroyedTex = null;
		}

		////////////////

		public override void Load() {
			DurabilityMod.Instance = this; 

			if( Main.netMode != 2 ) {   // Not server
				this.DestroyedTex = ModContent.GetTexture( "Terraria/MapDeath" );
			}

			AchievementsHelper.OnTileDestroyed += this.MyOnTileDestroyedEvent;
		}

		public override void Unload() {
			DurabilityMod.Instance = null;
			AchievementsHelper.OnTileDestroyed -= this.MyOnTileDestroyedEvent;
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
