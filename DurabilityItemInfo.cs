using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Items;
using HamstarHelpers.Helpers.Items.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;


namespace Durability {
	class DurabilityItemInfo : GlobalItem {
		public override bool InstancePerEntity => true;
		//public override bool CloneNewInstances => true;

		
		public void Initialize( Item item, double wear, int repairs ) {
			this.WearAndTear = wear;
			this.Repairs = repairs;
			this.IsBroken = wear >= DurabilityItemInfo.CalculateFullDurability( item );

			this.IsInitialized = true;
		}

		public override GlobalItem Clone( Item item, Item itemClone ) {
			var clone = (DurabilityItemInfo)base.Clone( item, itemClone );
			clone.WearAndTear = this.WearAndTear;
			clone.Repairs = this.Repairs;
			clone.IsUnbreakable = this.IsUnbreakable;
			clone.IsCritical = this.IsCritical;
			clone.IsBroken = this.IsBroken;
			clone.IsInitialized = this.IsInitialized;

			return clone;
		}
		public void CopyToMe( DurabilityItemInfo info ) {
			this.WearAndTear = info.WearAndTear;
			this.IsUnbreakable = info.IsUnbreakable;
			this.Repairs = info.Repairs;
			this.IsCritical = info.IsCritical;
			this.IsBroken = info.IsBroken;

			this.IsInitialized = true;
		}

		////////////////	

		public override bool NeedsSaving( Item item ) {
			return this.IsHandy(item) && !item.consumable ? true : base.NeedsSaving( item );
		}

		public override void LoadLegacy( Item item, BinaryReader reader ) {
			this.Initialize( item, (int)reader.ReadInt32(), 0 );
		}

		public override void Load( Item item, TagCompound tag ) {
			double wear = tag.GetDouble( "wear_and_tear_d" );
			int repairs = tag.GetInt( "repairs" );
			this.Initialize( item, wear, repairs );
		}

		public override TagCompound Save( Item item ) {
			return new TagCompound {
				{"wear_and_tear_d", (double)this.WearAndTear},
				{"repairs", (int)this.Repairs }
			};
		}

		////////////////

		public override void NetReceive( Item item, BinaryReader reader ) {
			this.WearAndTear = reader.ReadDouble();
			this.IsUnbreakable = reader.ReadBoolean();
			this.Repairs = reader.ReadInt32();
			this.IsCritical = reader.ReadBoolean();
			this.IsInitialized = reader.ReadBoolean();
		}

		public override void NetSend( Item item, BinaryWriter writer ) {
			writer.Write( (double)this.WearAndTear );
			writer.Write( (bool)this.IsUnbreakable );
			writer.Write( (int)this.Repairs );
			writer.Write( (bool)this.IsCritical );
			writer.Write( (bool)this.IsInitialized );
		}

		////////////////



		private static int MaxConcurrentUses = 5;

		public bool IsBroken { get; private set; }
		public double WearAndTear { get; private set; }
		public bool IsUnbreakable = false;
		public int Repairs = 0;
		public bool IsCritical = false;

		public int ConcurrentUses;
		public int RecentUseDisplayBarAnimate;

		private bool IsInitialized = false;
		


		////////////////

		public static int CalculateFullDurability( Item item, DurabilityConfig overrideConfig=null ) {
			var data = overrideConfig ?? DurabilityMod.Instance?.Config;
			if( data == null ) {
				return 0;	// !
			}

			double val = item.value;
			double mul = data.DurabilityMultiplier;
			double add = data.DurabilityAdditive;
			double hitsPerSec = 60d / (double)ItemHelpers.CalculateStandardUseTime( item );
			bool isArmor = ItemAttributeHelpers.IsArmor( item );
			bool isTool = ItemAttributeHelpers.IsTool( item );

			if( isArmor && !isTool ) { hitsPerSec = 1d; }

			if( val == 0 ) {
				val = ((double)item.damage * (hitsPerSec/4d) * 1000d) + ((double)item.defense * 1000d);

				if( item.rare > 1 ) {
					val *= (double)item.rare;
				}
				if( val <= 0 ) {
					val = 100 * (item.rare > 0 ? item.rare : 1);    // Fallback
				}
			}

			double pow = Math.Pow( val, data.DurabilityExponent );
			double durability = mul * ( ((hitsPerSec / 4d) * pow) / (5d + val) ) + add;
			
			if( isArmor ) {
				durability *= data.ArmorDurabilityMultiplier;
			}
			if( isTool ) {
				durability *= data.ToolDurabilityMultiplier;
			}
			if( !isTool && !isArmor ) {
				durability *= data.NonToolOrArmorDurabilityMultiplier;
			}

			var itemDef = new ItemDefinition( item.type );

			if( data.PerItemDurabilityMultipliers.ContainsKey( itemDef ) ) {
				durability *= data.PerItemDurabilityMultipliers[itemDef].Multiplier;
			}

			return (int)durability;
		}

		public int CalculateDurabilityLoss() {
			var mymod = DurabilityMod.Instance;
			return (int)((float)this.Repairs * mymod.Config.MaxDurabilityLostPerRepair);
		}
		

		public bool IsHandy( Item item ) {
			return ItemAttributeHelpers.IsTool( item ) || ItemAttributeHelpers.IsArmor( item ) || ItemAttributeHelpers.IsGrapple( item );
		}
		public bool HasDurability( Item item ) {
			return this.IsHandy(item) && !this.IsUnbreakable && !item.consumable;
		}



		////////////////


		public void AddWearAndTear( Item item, int hits = 1, double multiplier = 1d ) {
			if( !this.HasDurability( item ) || this.ConcurrentUses >= DurabilityItemInfo.MaxConcurrentUses ) { return; }

			var mymod = DurabilityMod.Instance;

			this.AddWearAndTearForMe( item, hits, multiplier );

			// Propagate effect to mouse item, if applicable
			if( Main.netMode != 2 && item.owner == Main.myPlayer ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir && !Main.mouseItem.IsNotTheSameAs( item ) ) {
					var mouseItemInfo = Main.mouseItem.GetGlobalItem<DurabilityItemInfo>();
					mouseItemInfo.AddWearAndTearForMe( Main.mouseItem, hits, multiplier );
				}
			}
		}

		private void AddWearAndTearForMe( Item item, int hits, double multiplier ) {
			var mymod = DurabilityMod.Instance;

			this.WearAndTear += (double)hits * (double)mymod.Config.GeneralWearAndTearMultiplier * multiplier;
			this.ConcurrentUses++;
			this.RecentUseDisplayBarAnimate = 8;
			
			if( !this.IsBroken && this.IsNowBroken(item) ) {
				this.KillMe( item );
			}
		}


		public bool RemoveWearAndTear( Item item, int amount ) {
			double wear = this.WearAndTear - amount;
			
			this.WearAndTear = Math.Max( wear, this.CalculateDurabilityLoss() );

			bool isBroken = this.IsNowBroken( item );
			if( !this.IsBroken && isBroken ) {
				this.KillMe( item );
			}
			return !isBroken;
		}

		////////////////
		
		public bool IsNowBroken( Item item ) {
			var mymod = DurabilityMod.Instance;
			int max = DurabilityItemInfo.CalculateFullDurability( item );
			return this.WearAndTear >= max;
		}


		public bool CanRepair( Item item ) {
			var mymod = DurabilityMod.Instance;
			DurabilityConfig data = mymod.Config;
			bool can_repair_broken = !this.IsBroken || (data.CanRepairBroken && this.IsBroken);

			return data.CanRepair && can_repair_broken && this.WearAndTear > this.CalculateDurabilityLoss();
		}


		public void RepairMe( Item item ) {
			var mymod = DurabilityMod.Instance;

			this.Repairs++;

			if( this.RemoveWearAndTear( item, mymod.Config.RepairAmount ) ) {
				Main.PlaySound( SoundID.Item37, item.position );
				this.IsBroken = false;
			}
		}


		public void UpdateCriticalState( Item item ) {
			var mymod = DurabilityMod.Instance;
			double max = DurabilityItemInfo.CalculateFullDurability( item );
			double ratio = (max - this.WearAndTear) / max;

			if( !this.IsCritical ) {
				if( ratio <= mymod.Config.CriticalWarningPercent ) {
					this.IsCritical = true;

					if( Main.netMode != 2 ) {
						var player = Main.player[Main.myPlayer];

						int ct = CombatText.NewText( player.getRect(), Color.Yellow, item.Name + " damaged!" );
						Main.combatText[ct].lifeTime = 100;
						Main.PlaySound( SoundID.NPCHit18, player.position );
					}
				}
			} else {
				if( ratio > mymod.Config.CriticalWarningPercent ) {
					this.IsCritical = false;
				}
			}
		}


		public void KillMe( Item item ) {
			if( item.owner < 0 || item.owner > 255 ) {
				LogHelpers.Warn( "Invalid item owner." );
				return;
			}

			var mymod = DurabilityMod.Instance;

			this.IsBroken = true;
			this.WearAndTear = DurabilityItemInfo.CalculateFullDurability( item );

			if( item.owner != 255 ) {
				string itemName = item.Name;
				Player player = Main.player[item.owner];
				player.AddBuff( 23, 1 );
				player.noItems = true;

				Main.PlaySound( 13, player.position );
				Main.PlaySound( 3, player.position, 18 );

				/*item.SetDefaults( 0, false );
				item.netID = 0;
				item.type = 0;
				item.stack = 0;
				//item.name = "";
				item.useStyle = 0;
				item.useTime = 0;
				item.useAnimation = 0;*/

				int ct = CombatText.NewText( player.getRect(), Color.DarkGray, itemName + " has broken!", false, true );
				Main.combatText[ct].lifeTime = 80;
			}
		}
	}
}
