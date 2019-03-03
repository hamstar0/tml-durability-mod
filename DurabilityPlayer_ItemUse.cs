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
			
			Item headItem = player.armor[0];
			Item bodyItem = player.armor[1];
			Item legsItem = player.armor[2];

			damage = Main.CalculateDamage( (int)damage, this.player.statDefense );
			if( crit ) { damage *= 2; }

			int dmg = (int)(damage / 10);
			if( dmg <= 0 ) { dmg = 1; }


			if( !headItem.IsAir ) {
				var head_item_info = headItem.GetGlobalItem<DurabilityItemInfo>();
				head_item_info.AddWearAndTear( headItem, dmg );
				head_item_info.UpdateCriticalState( headItem );
			}

			if( !bodyItem.IsAir ) {
				var body_item_info = bodyItem.GetGlobalItem<DurabilityItemInfo>();
				body_item_info.AddWearAndTear( bodyItem, dmg );
				body_item_info.UpdateCriticalState( bodyItem );
			}

			if( !legsItem.IsAir ) {
				var legs_item_info = legsItem.GetGlobalItem<DurabilityItemInfo>();
				legs_item_info.AddWearAndTear( legsItem, dmg );
				legs_item_info.UpdateCriticalState( legsItem );
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
						var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();
						
						itemInfo.AddWearAndTear( item, 1, mymod.Config.WeaponWearAndTearMultiplier );
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

			var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();
			itemInfo.AddWearAndTear( item );
		}

		public override void OnHitPvp( Item item, Player target, int damage, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			if( item == null || item.IsAir ) { return; }

			var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();
			itemInfo.AddWearAndTear( item );
		}

		public override void CatchFish( Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			if( fishingRod == null || fishingRod.IsAir ) { return; }

			var item_info = fishingRod.GetGlobalItem<DurabilityItemInfo>();

			item_info.AddWearAndTear( fishingRod, 1, mymod.Config.FishingWearAndTearMultiplier );
		}

		public override bool Shoot( Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return base.Shoot(item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack); }
			if( item == null || item.IsAir ) { return false; }

			var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();
			if( !itemInfo.HasDurability( item ) ) { return true; }
			
			if( item.melee && !item.noMelee && !item.noUseGraphic ) { // Any projectile generating melee
				itemInfo.AddWearAndTear( item, 1, mymod.Config.MeleeProjectileWearAndTearMultiplier );
			}
			return true;
		}


		public override bool PreItemCheck() {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return base.PreItemCheck(); }

			Player player = this.player;
			Item item = player.inventory[player.selectedItem];

			if( item != null && !item.IsAir && !player.noItems ) {
				var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();

				if( itemInfo.IsBroken ) {
					player.noItems = true;
					player.noBuilding = true;
				} else if( itemInfo.HasDurability(item) ) {
					bool isHarpoon = item.type == 160;
					bool isFishingPole = item.fishingPole > 0;
					bool hasAmmo = player.HasAmmo( item, true );	// Somehow works with harpoons and magic (?)
					bool cantMagic = /*item.magic &&*/ player.statMana < item.mana;
					bool trigger = (player.itemTime == 0 && player.controlUseItem && player.releaseUseItem) ||
						(player.itemTime == 1 && player.controlUseItem && item.autoReuse);

					if( trigger && !item.melee && hasAmmo && !isFishingPole && !isHarpoon && !cantMagic ) {
						double scale = mymod.Config.WeaponWearAndTearMultiplier;

						if( item.summon ) {
							scale = mymod.Config.SummonWearAndTearMultiplier;
						} else if( item.mech ) {
							scale = mymod.Config.WireWearAndTearMultiplier;
						}

						itemInfo.AddWearAndTear( item, 1, scale );
						if( Main.mouseItem != null && !Main.mouseItem.IsAir ) {
							var mouseItemInfo = Main.mouseItem.GetGlobalItem<DurabilityItemInfo>();
							mouseItemInfo.AddWearAndTear( Main.mouseItem, 1, scale );
						}
					}
				}
			}

			return base.PreItemCheck();
		}

		public override void PostItemCheck() {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }

			Item item = this.player.inventory[ this.player.selectedItem ];
			if( item != null && !item.IsAir ) {
				var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();

				if( !itemInfo.IsBroken && itemInfo.IsNowBroken( item ) ) {
					itemInfo.KillMe( item );
				}
			}
		}
	}
}
