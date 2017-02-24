using Terraria;
using Terraria.ModLoader;


namespace Durability {
	class DurabilityGlobalProjectile : GlobalProjectile {
		public override void UseGrapple( Player player, ref int type ) {
			Item grapple = null;
			if( Main.projHook[player.miscEquips[4].shoot] ) {
				grapple = player.miscEquips[4];
			}
			if( grapple == null ) {
				for( int i = 0; i < 58; i++ ) {
					if( Main.projHook[player.inventory[i].shoot] ) {
						grapple = player.inventory[i];
						break;
					}
				}
			}

			if( grapple != null ) {
				DurabilityItemInfo info = grapple.GetModInfo<DurabilityItemInfo>(this.mod);
				info.Use( grapple, 1, 1 );
//Main.NewText(grapple.name+" "+info.Durability+" "+ grapple.useStyle+" "+grapple.knockBack);
			}
		}

		//public override bool OnTileCollide( Projectile projectile, Vector2 oldVelocity ) {
		//	if( projectile.owner == Main.myPlayer ) {
		//		Player player = Main.player[Main.myPlayer];
		//		Item item = player.inventory[player.selectedItem];

		//		if( item.melee ) {
		//			DurabilityModPlayer p_info = player.GetModPlayer<DurabilityModPlayer>(this.mod);
		//			p_info.UseCurrentItem();
		//		}
		//	}

		//	return base.OnTileCollide(projectile, oldVelocity);
		//}

		//		public override void OnHitNPC( Projectile projectile, NPC target, int damage, float knockback, bool crit ) {
		//Main.NewText(projectile.name + " OnHitNPC, is melee: "+ projectile.melee);
		//			if( projectile.owner == Main.myPlayer ) {
		//				Player player = Main.player[Main.myPlayer];
		//				Item item = player.inventory[player.selectedItem];

		//				if( item.melee ) {
		//					DurabilityModPlayer p_info = player.GetModPlayer<DurabilityModPlayer>(this.mod);
		//					p_info.UseCurrentItem();
		//				}
		//			}
		//		}
	}
}
