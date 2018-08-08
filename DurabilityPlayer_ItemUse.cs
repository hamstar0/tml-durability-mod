using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Durability {
	partial class DurabilityPlayer : ModPlayer {
		public override void Hurt( bool pvp, bool quiet, double damage, int hitDirection, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			if( quiet ) { return; }
			
			Item head_item = player.armor[0];
			Item body_item = player.armor[1];
			Item legs_item = player.armor[2];

			damage = Main.CalculateDamage( (int)damage, this.player.statDefense );
			if( crit ) { damage *= 2; }

			int dmg = (int)(damage / 10);
			if( dmg <= 0 ) { dmg = 1; }


			if( !head_item.IsAir ) {
				var head_item_info = head_item.GetGlobalItem<DurabilityItemInfo>( mymod );
				head_item_info.AddWearAndTear( mymod, head_item, dmg );
				head_item_info.UpdateCriticalState( (DurabilityMod)this.mod, head_item );
			}

			if( !body_item.IsAir ) {
				var body_item_info = body_item.GetGlobalItem<DurabilityItemInfo>( mymod );
				body_item_info.AddWearAndTear( mymod, body_item, dmg );
				body_item_info.UpdateCriticalState( (DurabilityMod)this.mod, body_item );
			}

			if( !legs_item.IsAir ) {
				var legs_item_info = legs_item.GetGlobalItem<DurabilityItemInfo>( mymod );
				legs_item_info.AddWearAndTear( mymod, legs_item, dmg );
				legs_item_info.UpdateCriticalState( (DurabilityMod)this.mod, legs_item );
			}
		}


		private void onHitWithProj( Projectile proj ) {
			Item item = this.player.inventory[ this.player.selectedItem ];

			if( item != null && !item.IsAir && !proj.minion && item.shoot != 0 ) {
				if( (item.melee || item.thrown || proj.type == ProjectileID.Harpoon) && item.noMelee
						&& proj.type != ProjectileID.NorthPoleSnowflake
						&& proj.type != ProjectileID.SporeCloud
						&& proj.type != ProjectileID.ShadowFlameKnife ) {
					if( item.shoot == proj.type ) {
						var mymod = (DurabilityMod)this.mod;
						var item_info = item.GetGlobalItem<DurabilityItemInfo>( mymod );
						
						item_info.AddWearAndTear( mymod, item, 1, mymod.Config.WeaponWearAndTearMultiplier );
					}
				}
			}
		}

		public override void OnHitNPCWithProj( Projectile proj, NPC target, int damage, float knockback, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }

			this.onHitWithProj( proj );
		}

		public override void OnHitPvpWithProj( Projectile proj, Player target, int damage, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }

			this.onHitWithProj( proj );
		}

		public override void OnHitNPC( Item item, NPC target, int damage, float knockback, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			if( item == null || item.IsAir ) { return; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>(this.mod);
			item_info.AddWearAndTear( (DurabilityMod)this.mod, item );
		}

		public override void OnHitPvp( Item item, Player target, int damage, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			if( item == null || item.IsAir ) { return; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>(this.mod);
			item_info.AddWearAndTear( (DurabilityMod)this.mod, item );
		}

		public override void CatchFish( Item fishing_rod, Item bait, int power, int liquid_type, int pool_size, int world_layer, int quest_fish, ref int caught_type, ref bool junk ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			if( fishing_rod == null || fishing_rod.IsAir ) { return; }

			var item_info = fishing_rod.GetGlobalItem<DurabilityItemInfo>( mymod );

			item_info.AddWearAndTear( mymod, fishing_rod, 1, mymod.Config.FishingWearAndTearMultiplier );
		}

		public override bool Shoot( Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return base.Shoot(item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack); }
			if( item == null || item.IsAir ) { return false; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>( mymod );
			if( !item_info.HasDurability( item ) ) { return true; }
			
			if( item.melee && !item.noMelee && !item.noUseGraphic ) { // Any projectile generating melee
				item_info.AddWearAndTear( mymod, item, 1, mymod.Config.MeleeProjectileWearAndTearMultiplier );
			}
			return true;
		}


		public override bool PreItemCheck() {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return base.PreItemCheck(); }

			Player player = this.player;
			Item item = player.inventory[player.selectedItem];

			if( !item.IsAir && !player.noItems ) {
				var item_info = item.GetGlobalItem<DurabilityItemInfo>( this.mod );

				if( item_info.IsBroken ) {
					player.noItems = true;
					player.noBuilding = true;
				} else if( item_info.HasDurability(item) ) {
					bool is_harpoon = item.type == 160;
					bool is_fishing_pole = item.fishingPole > 0;
					bool has_ammo = player.HasAmmo( item, true );	// Somehow works with harpoons and magic (?)
					bool cant_magic = /*item.magic &&*/ player.statMana < item.mana;
					bool trigger = (player.itemTime == 0 && player.controlUseItem && player.releaseUseItem) ||
						(player.itemTime == 1 && player.controlUseItem && item.autoReuse);

					if( trigger && !item.melee && has_ammo && !is_fishing_pole && !is_harpoon && !cant_magic ) {
						double scale = mymod.Config.WeaponWearAndTearMultiplier;

						if( item.summon ) {
							scale = mymod.Config.SummonWearAndTearMultiplier;
						} else if( item.mech ) {
							scale = mymod.Config.WireWearAndTearMultiplier;
						}

						item_info.AddWearAndTear( (DurabilityMod)this.mod, item, 1, scale );
						if( Main.mouseItem != null && !Main.mouseItem.IsAir ) {
							var mouse_item_info = Main.mouseItem.GetGlobalItem<DurabilityItemInfo>( this.mod );
							mouse_item_info.AddWearAndTear( (DurabilityMod)this.mod, Main.mouseItem, 1, scale );
						}
					}
				}
			}

			return base.PreItemCheck();
		}

		public override void PostItemCheck() {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }

			Item item = this.player.inventory[this.player.selectedItem];
			var item_info = item.GetGlobalItem<DurabilityItemInfo>( this.mod );

			if( !item_info.IsBroken && item_info.IsNowBroken(mymod, item) ) {
				item_info.KillMe( mymod, item );
			}
		}
	}
}
