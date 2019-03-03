using HamstarHelpers.Helpers.PlayerHelpers;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	class DurabilityProjectile : GlobalProjectile {
		public override bool? CanUseGrapple( int type, Player player ) {
			Item grapple = PlayerItemHelpers.GetGrappleItem( player );
			if( grapple != null ) {
				var itemInfo = grapple.GetGlobalItem<DurabilityItemInfo>();
				if( itemInfo.IsBroken ) {
					return false;
				}
			}

			return null;
		}


		public override void UseGrapple( Player player, ref int type ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }

			Item grapple = PlayerItemHelpers.GetGrappleItem( player );
			
			if( grapple != null ) {
				var itemInfo = grapple.GetGlobalItem<DurabilityItemInfo>();
				itemInfo.AddWearAndTear( grapple, 1, mymod.Config.GrappleWearAndTearMultiplier );
			}
		}
	}
}
