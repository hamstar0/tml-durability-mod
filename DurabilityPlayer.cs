using HamstarHelpers.Helpers.PlayerHelpers;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	partial class DurabilityPlayer : ModPlayer {
		private long LastMoney = 0;
		private Item[] PrevInventory;
		private Item[] PrevShop;
		private DurabilityItemInfo ForceCopy = null;


		////////////////

		public override bool CloneNewInstances { get { return false; } }

		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );
			var myclone = (DurabilityPlayer)clone;

			myclone.LastMoney = this.LastMoney;
			myclone.PrevInventory = this.PrevInventory;
			myclone.PrevShop = this.PrevShop;
			myclone.ForceCopy = this.ForceCopy;
		}


		////////////////

		public override void SyncPlayer( int to_who, int from_who, bool new_player ) {
			var mymod = (DurabilityMod)this.mod;

			if( Main.netMode == 2 ) {
				if( to_who == -1 && from_who == this.player.whoAmI ) {
					this.OnServerConnect();
				}
			}
		}

		public override void OnEnterWorld( Player player ) {
			if( player.whoAmI != this.player.whoAmI ) { return; }

			var mymod = (DurabilityMod)this.mod;

			if( Main.netMode == 0 ) {
				if( !mymod.ConfigJson.LoadFile() ) {
					mymod.ConfigJson.SaveFile();
					ErrorLogger.Log( "Durability config " + DurabilityConfigData.ConfigVersion.ToString() + " created (ModPlayer.OnEnterWorld())." );
				}
			}

			if( mymod.Config.DebugModeInfo ) {
				bool _;
				ErrorLogger.Log( "ResetMode.ResetModePlayer.OnEnterWorld - " + player.name + " joined (" + PlayerIdentityHelpers.GetUniqueId( player, out _ ) + ")" );
			}

			if( Main.netMode == 0 ) {
				this.OnSingleConnect();
			}
			if( Main.netMode == 1 ) {
				this.OnClientConnect();
			}
		}
	}
}
