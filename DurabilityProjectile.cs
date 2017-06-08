using HamstarHelpers.ItemHelpers;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
	class DurabilityProjectile : GlobalProjectile {
		public override void UseGrapple( Player player, ref int type ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			Item grapple = ItemHelpers.GetGrappleItem( player );
			
			if( grapple != null ) {
				var item_info = grapple.GetGlobalItem<DurabilityItemInfo>( this.mod );
				item_info.AddWearAndTear( (DurabilityMod)this.mod, grapple, 1, mymod.Config.Data.GrappleWearAndTearMultiplier );
			}
		}
	}
}
