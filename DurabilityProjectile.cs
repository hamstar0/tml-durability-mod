using HamstarHelpers.Helpers.PlayerHelpers;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	class DurabilityProjectile : GlobalProjectile {
		public override bool? CanUseGrapple( int type, Player player ) {
			Item grapple = PlayerItemHelpers.GetGrappleItem( player );
			if( grapple != null ) {
				var item_info = grapple.GetGlobalItem<DurabilityItemInfo>( this.mod );
				if( item_info.IsBroken ) {
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
				var item_info = grapple.GetGlobalItem<DurabilityItemInfo>( this.mod );
				item_info.AddWearAndTear( (DurabilityMod)this.mod, grapple, 1, mymod.Config.GrappleWearAndTearMultiplier );
			}
		}
	}
}
