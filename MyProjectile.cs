using HamstarHelpers.PlayerHelpers;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	class MyProjectile : GlobalProjectile {
		public override bool? CanUseGrapple( int type, Player player ) {
			Item grapple = PlayerItemHelpers.GetGrappleItem( player );
			if( grapple != null ) {
				var item_info = grapple.GetGlobalItem<MyItemInfo>( this.mod );
				if( item_info.IsBroken ) {
					return false;
				}
			}

			return null;
		}


		public override void UseGrapple( Player player, ref int type ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			Item grapple = PlayerItemHelpers.GetGrappleItem( player );
			
			if( grapple != null ) {
				var item_info = grapple.GetGlobalItem<MyItemInfo>( this.mod );
				item_info.AddWearAndTear( (DurabilityMod)this.mod, grapple, 1, mymod.Config.Data.GrappleWearAndTearMultiplier );
			}
		}
	}
}
