using HamstarHelpers.ItemHelpers;
using HamstarHelpers.PlayerHelpers;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Durability {
	class DurabilityPlayer : ModPlayer {
		private long LastMoney = 0;
		private Item[] PrevInventory;
		private Item[] PrevShop;
		private DurabilityItemInfo ForceCopy = null;


		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );
			var myclone = (DurabilityPlayer)clone;

			myclone.LastMoney = this.LastMoney;
			myclone.PrevInventory = this.PrevInventory;
			myclone.PrevShop = this.PrevShop;
			myclone.ForceCopy = this.ForceCopy;
		}

		public override void OnEnterWorld( Player player ) {
			if( player == null ) { return; }	// Wtf #1

			if( player.whoAmI == this.player.whoAmI ) { // Current player
				var mymod = (DurabilityMod)this.mod;

				if( Main.netMode != 2 ) {	// Not server
					if( !mymod.Config.LoadFile() ) {
						mymod.Config.SaveFile();
					}
				}

				if( Main.netMode == 1 ) {   // Client
					DurabilityNetProtocol.RequestSettingsFromServer( mymod, player );
				}
			}

			// Because crafting recipe items showing a durability bar is confusing
			for( int i = 0; i < Main.recipe.Length; i++ ) {
				if( Main.recipe[i] == null ) { continue; }	// Wtf #2

				Item craft_item = Main.recipe[i].createItem;
				if( craft_item == null || craft_item.IsAir ) { continue; }	// Wtf #3

				try {
					var item_info = craft_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
					if( item_info == null ) { continue; }

					item_info.IsUnbreakable = true;
				} catch(Exception _) {}
			}
		}

		////////////////

		
		
		//public override void PostHurt( bool pvp, bool quiet, double damage, int hitDirection, bool crit ) {
		public override void Hurt( bool pvp, bool quiet, double damage, int hitDirection, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }
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
						
						item_info.AddWearAndTear( mymod, item, 1, mymod.Config.Data.WeaponWearAndTearMultiplier );
					}
				}
			}
		}

		public override void OnHitNPCWithProj( Projectile proj, NPC target, int damage, float knockback, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			this.onHitWithProj( proj );
		}

		public override void OnHitPvpWithProj( Projectile proj, Player target, int damage, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			this.onHitWithProj( proj );
		}

		public override void OnHitNPC( Item item, NPC target, int damage, float knockback, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }
			if( item == null || item.IsAir ) { return; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>(this.mod);
			item_info.AddWearAndTear( (DurabilityMod)this.mod, item );
		}

		public override void OnHitPvp( Item item, Player target, int damage, bool crit ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }
			if( item == null || item.IsAir ) { return; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>(this.mod);
			item_info.AddWearAndTear( (DurabilityMod)this.mod, item );
		}

		public override void CatchFish( Item fishing_rod, Item bait, int power, int liquid_type, int pool_size, int world_layer, int quest_fish, ref int caught_type, ref bool junk ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }
			if( fishing_rod == null || fishing_rod.IsAir ) { return; }

			var item_info = fishing_rod.GetGlobalItem<DurabilityItemInfo>( mymod );

			item_info.AddWearAndTear( mymod, fishing_rod, 1, mymod.Config.Data.FishingWearAndTearMultiplier );
		}

		public override bool Shoot( Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return base.Shoot(item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack); }
			if( item == null || item.IsAir ) { return false; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>( mymod );
			if( !item_info.HasDurability( item ) ) { return true; }

			ConfigurationData data = mymod.Config.Data;
			if( item.melee && !item.noMelee && !item.noUseGraphic ) { // Any projectile generating melee
				item_info.AddWearAndTear( mymod, item, 1, mymod.Config.Data.MeleeProjectileWearAndTearMultiplier );
			}
			return true;
		}


		public override bool PreItemCheck() {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return base.PreItemCheck(); }

			Player player = this.player;
			Item item = player.inventory[player.selectedItem];

			if( !item.IsAir && !player.noItems ) {
				var item_info = item.GetGlobalItem<DurabilityItemInfo>( this.mod );

				if( item_info.HasDurability(item) ) {
					bool is_harpoon = item.type == 160;
					bool is_fishing_pole = item.fishingPole > 0;
					bool has_ammo = player.HasAmmo( item, true );	// Somehow works with harpoons and magic (?)
					bool cant_magic = /*item.magic &&*/ player.statMana < item.mana;
					bool trigger = (player.itemTime == 0 && player.controlUseItem && player.releaseUseItem) ||
						(player.itemTime == 1 && player.controlUseItem && item.autoReuse);

					if( trigger && !item.melee && has_ammo && !is_fishing_pole && !is_harpoon && !cant_magic ) {
						double scale = mymod.Config.Data.WeaponWearAndTearMultiplier;

						if( item.summon ) {
							scale = mymod.Config.Data.SummonWearAndTearMultiplier;
						} else if( item.mech ) {
							scale = mymod.Config.Data.WireWearAndTearMultiplier;
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
			if( !mymod.Config.Data.Enabled ) { return; }

			Item item = this.player.inventory[this.player.selectedItem];
			var item_info = item.GetGlobalItem<DurabilityItemInfo>( this.mod );
			int max = DurabilityItemInfo.CalculateFullDurability( (DurabilityMod)this.mod, item );

			if( item_info.WearAndTear >= max ) {
				item_info.KillMe( item );
			}
		}

		////////////////
		
		public override void PreUpdate() {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			Item curr_item = this.player.inventory[this.player.selectedItem];
			Item head_item = player.armor[0];
			Item body_item = player.armor[1];
			Item legs_item = player.armor[2];

			if( curr_item != null && !curr_item.IsAir ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir ) { curr_item = Main.mouseItem; }

				var item_info = curr_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
				item_info.ConcurrentUses = 0;

				item_info.UpdateCriticalState( (DurabilityMod)this.mod, curr_item );
			}

			if( !head_item.IsAir ) {
				var head_item_info = head_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
				head_item_info.ConcurrentUses = 0;
			}

			if( !body_item.IsAir ) {
				var body_item_info = body_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
				body_item_info.ConcurrentUses = 0;
			}

			if( !legs_item.IsAir ) {
				var legs_item_info = legs_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
				legs_item_info.ConcurrentUses = 0;
			}

			this.CheckPurchaseChanges();
		}

		////////////////

		private void CheckPurchaseChanges() {
			if( Main.netMode == 2 ) { return; }	// Not server
			if( this.player.whoAmI != Main.myPlayer ) { return; }	// Current player

			// Note: Due to a (tML?) bug, this method of copying is necessary
			if( this.ForceCopy != null ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir ) {
					var item_info = Main.mouseItem.GetGlobalItem<DurabilityItemInfo>( this.mod );
					item_info.CopyToMe( this.ForceCopy );
				}
				this.ForceCopy = null;
			}

			long money = PlayerItemHelpers.CountMoney( this.player );
			long spent = this.LastMoney - money;

			if( this.player.talkNPC != -1 ) {
				var shop = Main.instance.shop;
				if( Main.npcShop >= 0 && Main.npcShop < shop.Length && shop[Main.npcShop] != null ) {
					if( spent > 0 ) {
						this.FixPurchasedItemInfo();
					}
					this.UpdateInventoryAndShopSnapshots();
				}
				this.LastMoney = money;
			}
		}


		private void UpdateInventoryAndShopSnapshots() {
			int len = this.player.inventory.Length + 1;
			Item[] stock = Main.instance.shop[Main.npcShop].item;

			// Get snapshop of inventory
			if( this.PrevInventory == null || this.PrevInventory.Length != len ) {
				this.PrevInventory = new Item[len];
			}
			for( int i = 0; i < len - 1; i++ ) {
				this.PrevInventory[i] = this.player.inventory[i].Clone();
			}
			this.PrevInventory[len - 1] = Main.mouseItem.Clone();

			// Get snapshot of shop
			if( this.PrevShop == null || this.PrevShop.Length != stock.Length ) {
				this.PrevShop = new Item[stock.Length];
			}
			for( int i = 0; i < stock.Length; i++ ) {
				if( stock[i] != null ) {
					this.PrevShop[i] = stock[i].Clone();
				} else {
					this.PrevShop[i] = new Item();
				}
			}
		}

		
		private void FixPurchasedItemInfo() {
			Item[] curr_shop = Main.instance.shop[Main.npcShop].item;
			Item[] curr_inv = (Item[])this.player.inventory.Clone();
			curr_inv = curr_inv.Concat<Item>( new Item[] { Main.mouseItem.Clone() } ).ToArray();

			var shop_changes = ItemFinderHelpers.FindChanges( this.PrevShop, curr_shop );
			var inv_changes = ItemFinderHelpers.FindChanges( this.PrevInventory, curr_inv );

			if( shop_changes.Count != 1 || inv_changes.Count != 1 ) { return; }

			int prev_item_idx = 0;
			Item prev_item = null;
			Item curr_item = null;

			foreach( var kv in shop_changes ) {
				if( shop_changes[kv.Key] ) { return; }
				prev_item = this.PrevShop[ kv.Key ];
				prev_item_idx = kv.Key;
			}
			foreach( var kv in inv_changes ) {
				if( !inv_changes[kv.Key] ) { return; }
				curr_item = curr_inv[kv.Key];
			}

			var prev_info = prev_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
			var curr_info = curr_item.GetGlobalItem<DurabilityItemInfo>( this.mod );
			
			this.ForceCopy = prev_info;
		}
	}
}
