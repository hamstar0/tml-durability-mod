using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Utils;


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
					var item_info = craft_item.GetModInfo<DurabilityItemInfo>( this.mod );
					if( item_info == null ) { continue; }

					item_info.IsUnbreakable = true;
				} catch(Exception _) {}
			}
		}

		////////////////

		
		
		//public override void PostHurt( bool pvp, bool quiet, double damage, int hitDirection, bool crit ) {
		public override void Hurt( bool pvp, bool quiet, double damage, int hitDirection, bool crit ) {
			if( quiet ) { return; }

			var mymod = (DurabilityMod)this.mod;
			damage = Main.CalculateDamage( (int)damage, this.player.statDefense );
			if( crit ) { damage *= 2; }

			int dmg = (int)(damage / 10);
			if( dmg <= 0 ) { dmg = 1; }

			Item armor1 = player.armor[0];
			if( armor1.type != 0 ) {
				var armor1_info = armor1.GetModInfo<DurabilityItemInfo>(this.mod);
				armor1_info.AddWearAndTear( mymod, armor1, dmg );
			}

			Item armor2 = player.armor[1];
			if( armor2.type != 0 ) {
				var armor2_info = armor2.GetModInfo<DurabilityItemInfo>(this.mod);
				armor2_info.AddWearAndTear( mymod, armor2, dmg );
			}

			Item armor3 = player.armor[2];
			if( armor3.type != 0 ) {
				var armor3_info = armor3.GetModInfo<DurabilityItemInfo>(this.mod);
				armor3_info.AddWearAndTear( mymod, armor3, dmg );
			}
		}


		private void onHitWithProj( Player player, Projectile proj ) {
			Item item = ItemHelper.GetSelectedItem( this.player );

			if( !item.IsAir && !proj.minion && item.shoot != 0 ) {
				if( ( item.melee || item.thrown || proj.type == ProjectileID.Harpoon) && item.noMelee
						&& proj.type != ProjectileID.NorthPoleSnowflake
						&& proj.type != ProjectileID.SporeCloud
						&& proj.type != ProjectileID.ShadowFlameKnife ) {
					if( item.shoot == proj.type ) {
						var mymod = (DurabilityMod)this.mod;
						var item_info = item.GetModInfo<DurabilityItemInfo>( mymod );
						
						item_info.AddWearAndTear( mymod, item, 1, mymod.Config.Data.WeaponWearAndTearMultiplier );
					}
				}
			}
		}

		public override void OnHitNPCWithProj( Projectile proj, NPC target, int damage, float knockback, bool crit ) {
			this.onHitWithProj( this.player, proj );
		}

		public override void OnHitPvpWithProj( Projectile proj, Player target, int damage, bool crit ) {
			this.onHitWithProj( this.player, proj );
		}

		public override void OnHitNPC( Item item, NPC target, int damage, float knockback, bool crit ) {
			if( item.type == 0 ) { return; }
			var item_info = item.GetModInfo<DurabilityItemInfo>(this.mod);
			item_info.AddWearAndTear( (DurabilityMod)this.mod, item );
		}

		public override void OnHitPvp( Item item, Player target, int damage, bool crit ) {
			if( item.type == 0 ) { return; }
			var item_info = item.GetModInfo<DurabilityItemInfo>(this.mod);
			item_info.AddWearAndTear( (DurabilityMod)this.mod, item );
		}

		public override void CatchFish( Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk ) {
			if( fishingRod.type == 0 ) { return; }
			var item_info = fishingRod.GetModInfo<DurabilityItemInfo>(this.mod);
			var mymod = (DurabilityMod)this.mod;

			item_info.AddWearAndTear( (DurabilityMod)this.mod, fishingRod, 1, mymod.Config.Data.FishingWearAndTearMultiplier );
		}

		public override bool Shoot( Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack ) {
			if( item.IsAir ) { return false; }

			var mymod = (DurabilityMod)this.mod;
			var item_info = item.GetModInfo<DurabilityItemInfo>( mymod );
			if( !item_info.HasDurability( item ) ) { return true; }

			ConfigurationData data = mymod.Config.Data;
			if( item.melee && !item.noMelee && !item.noUseGraphic ) { // Any projectile generating melee
				item_info.AddWearAndTear( mymod, item, 1, mymod.Config.Data.MeleeProjectileWearAndTearMultiplier );
			}
			return true;
		}


		public override bool PreItemCheck() {
			Player player = this.player;
			Item item = ItemHelper.GetSelectedItem( player );
			
			if( !item.IsAir && !player.noItems ) {
				var mymod = (DurabilityMod)this.mod;
				var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );

				if( item_info.HasDurability(item) ) {
					bool is_harpoon = item.type == 160;
					bool has_ammo = player.HasAmmo( item, true );	// Somehow works with harpoons and magic (?)
					bool cant_magic = /*item.magic &&*/ player.statMana < item.mana;
					bool trigger = (player.itemTime == 0 && player.controlUseItem && player.releaseUseItem) ||
						(player.itemTime == 1 && player.controlUseItem && item.autoReuse);

					if( trigger && !item.melee && has_ammo && !is_harpoon && !cant_magic ) {
						double scale = mymod.Config.Data.WeaponWearAndTearMultiplier;

						if( item.summon ) {
							scale = mymod.Config.Data.SummonWearAndTearMultiplier;
						} else if( item.mech ) {
							scale = mymod.Config.Data.WireWearAndTearMultiplier;
						}
							
						item_info.AddWearAndTear( (DurabilityMod)this.mod, item, 1, scale );
					}
				}
			}

			return base.PreItemCheck();
		}

		public override void PostItemCheck() {
			Item item = ItemHelper.GetSelectedItem( this.player );
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			int max = DurabilityItemInfo.CalculateFullDurability( (DurabilityMod)this.mod, item );

			if( item_info.WearAndTear >= max ) {
				item_info.KillMe( item );
			}
		}

		////////////////

		public override void PreUpdate() {
			Item item = ItemHelper.GetSelectedItem( this.player );
			Item armor1 = player.armor[0];
			Item armor2 = player.armor[1];
			Item armor3 = player.armor[2];

			if( item.type != 0 ) {
				var item_info = item.GetModInfo<DurabilityItemInfo>(this.mod);
				item_info.ConcurrentUses = 0;
			}

			if( armor1.type != 0 ) {
				var armor1_info = armor1.GetModInfo<DurabilityItemInfo>(this.mod);
				armor1_info.ConcurrentUses = 0;
			}

			if( armor2.type != 0 ) {
				var armor2_info = armor2.GetModInfo<DurabilityItemInfo>(this.mod);
				armor2_info.ConcurrentUses = 0;
			}

			if( armor3.type != 0 ) {
				var armor3_info = armor3.GetModInfo<DurabilityItemInfo>(this.mod);
				armor3_info.ConcurrentUses = 0;
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
					var item_info = Main.mouseItem.GetModInfo<DurabilityItemInfo>( this.mod );
					item_info.CopyToMe( this.ForceCopy );
				}
				this.ForceCopy = null;
			}

			long money = PlayerHelper.CountMoney( this.player );
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

			var shop_changes = ItemHelper.FindChanges( this.PrevShop, curr_shop );
			var inv_changes = ItemHelper.FindChanges( this.PrevInventory, curr_inv );

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

			var prev_info = prev_item.GetModInfo<DurabilityItemInfo>( this.mod );
			var curr_info = curr_item.GetModInfo<DurabilityItemInfo>( this.mod );
//Main.NewText( "DONE! prev wear: "+ prev_info.WearAndTear+" ("+prev_item.type+"), curr wear: "+ curr_info.WearAndTear+" ("+curr_item.type+")");

			this.ForceCopy = prev_info;
		}
	}
}
