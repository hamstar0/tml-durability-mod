using Terraria;
using Terraria.ModLoader;
using Utils;


namespace Durability {
	class DurabilityGlobalProjectile : GlobalProjectile {
		public override void UseGrapple( Player player, ref int type ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			Item grapple = ItemHelper.GetGrappleItem( player );
			
			if( grapple != null ) {
				var item_info = grapple.GetModInfo<DurabilityItemInfo>( this.mod );
				item_info.AddWearAndTear( (DurabilityMod)this.mod, grapple, 1, mymod.Config.Data.GrappleWearAndTearMultiplier );
			}
		}
	}
}
