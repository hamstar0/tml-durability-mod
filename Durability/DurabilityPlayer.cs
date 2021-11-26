using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Players;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	partial class DurabilityPlayer : ModPlayer {
		private long LastMoney = 0;
		private Item[] PrevInventory;
		private Item[] PrevShop;
		private DurabilityItemInfo ForceCopy = null;



		////////////////

		public override bool CloneNewInstances => false;

		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );
			var myclone = (DurabilityPlayer)clone;

			myclone.LastMoney = this.LastMoney;
			myclone.PrevInventory = this.PrevInventory;
			myclone.PrevShop = this.PrevShop;
			myclone.ForceCopy = this.ForceCopy;
		}


		////////////////

		public override void SyncPlayer( int toWho, int fromWho, bool newPlayer ) {
			var mymod = (DurabilityMod)this.mod;

			if( Main.netMode == 2 ) {
				if( toWho == -1 && fromWho == this.player.whoAmI ) {
					this.OnServerConnect();
				}
			}
		}

		public override void OnEnterWorld( Player player ) {
			if( player.whoAmI != Main.myPlayer ) { return; }
			if( this.player.whoAmI != Main.myPlayer ) { return; }

			var mymod = (DurabilityMod)this.mod;

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( player.name + " joined (" + PlayerIdentityHelpers.GetUniqueId( player ) + ")" );
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
