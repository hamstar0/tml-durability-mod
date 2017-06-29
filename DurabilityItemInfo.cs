using HamstarHelpers.ItemHelpers;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace Durability {
	public class DurabilityItemInfo : GlobalItem {
		public override bool InstancePerEntity { get { return true; } }
		//public override bool CloneNewInstances { get { return true; } }

		
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

		public static int CalculateFullDurability( DurabilityMod mymod, Item item ) {
			var data = mymod.Config.Data;
			double val = item.value;
			double mul = data.DurabilityMultiplier;
			double add = data.DurabilityAdditive;
			double hits_per_sec = 60d / (double)ItemHelpers.CalculateStandardUseTime( item );
			bool is_armor = ItemIdentityHelpers.IsArmor( item );
			bool is_tool = ItemIdentityHelpers.IsTool( item );

			if( is_armor && !is_tool ) { hits_per_sec = 1d; }

			if( val == 0 ) {
				val = ((double)item.damage * (hits_per_sec/4d) * 1000d) + ((double)item.defense * 1000d);

				if( item.rare > 1 ) {
					val *= (double)item.rare;
				}
				if( val <= 0 ) {
					val = 100 * (item.rare > 0 ? item.rare : 1);    // Fallback
				}
			}

			double pow = Math.Pow( val, data.DurabilityExponent );
			double durability = mul * ( ((hits_per_sec / 4d) * pow) / (5d + val) ) + add;
			
			if( is_armor ) {
				durability *= data.ArmorDurabilityMultiplier;
			}
			if( is_tool ) {
				durability *= data.ToolDurabilityMultiplier;
			}
			if( !is_tool && !is_armor ) {
				durability *= data.NonToolOrArmorDurabilityMultiplier;
			}

			if( data.CustomDurabilityMultipliers.Keys.Contains(item.Name) ) {
				durability *= data.CustomDurabilityMultipliers[item.Name];
			}

			return (int)durability;
		}

		public int CalculateDurabilityLoss( DurabilityMod mymod ) {
			return (int)((float)this.Repairs * mymod.Config.Data.MaxDurabilityLostPerRepair);
		}
		

		public bool HasDurability( Item item ) {
			bool is_handy = ItemIdentityHelpers.IsTool(item) || ItemIdentityHelpers.IsArmor(item) || ItemIdentityHelpers.IsGrapple(item);
			return is_handy && !this.IsUnbreakable && !item.consumable;
		}



		////////////////

		public void Initialize( DurabilityMod mymod, Item item, double wear, int repairs ) {
			this.WearAndTear = wear;
			this.Repairs = repairs;
			this.IsBroken = wear >= DurabilityItemInfo.CalculateFullDurability( mymod, item );

			this.IsInitialized = true;
		}


		public override GlobalItem Clone( Item item, Item item_clone ) {
			var clone = (DurabilityItemInfo)base.Clone( item, item_clone );
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
			return this.HasDurability( item ) ? true : base.NeedsSaving( item );
		}

		public override void LoadLegacy( Item item, BinaryReader reader ) {
			this.Initialize( (DurabilityMod)this.mod, item, (int)reader.ReadInt32(), 0 );
		}

		public override void Load( Item item, TagCompound tag ) {
			double wear = tag.GetDouble( "wear_and_tear_d" );
			int repairs = tag.GetInt( "repairs" );
			this.Initialize( (DurabilityMod)this.mod, item, wear, repairs );
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
			writer.Write( this.WearAndTear );
			writer.Write( this.IsUnbreakable );
			writer.Write( this.Repairs );
			writer.Write( this.IsCritical );
			writer.Write( this.IsInitialized );
		}
		
		////////////////


		public void AddWearAndTear( DurabilityMod mymod, Item item, int hits = 1, double multiplier = 1d ) {
			if( !this.HasDurability( item ) || this.ConcurrentUses >= DurabilityItemInfo.MaxConcurrentUses ) { return; }

			this.AddWearAndTearForMe( mymod, item, hits, multiplier );

			// Propagate effect to mouse item, if applicable
			if( Main.netMode != 2 && item.owner == Main.myPlayer ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir && !Main.mouseItem.IsNotTheSameAs( item ) ) {
					var mouse_item_info = Main.mouseItem.GetGlobalItem<DurabilityItemInfo>( mymod );
					mouse_item_info.AddWearAndTearForMe( mymod, Main.mouseItem, hits, multiplier );
				}
			}
		}

		private void AddWearAndTearForMe( DurabilityMod mymod, Item item, int hits, double multiplier ) {
			this.WearAndTear += (double)hits * (double)mymod.Config.Data.GeneralWearAndTearMultiplier * multiplier;
			this.ConcurrentUses++;
			this.RecentUseDisplayBarAnimate = 8;
			
			if( !this.IsBroken && this.IsNowBroken(mymod, item) ) {
				this.KillMe( mymod, item );
			}
		}


		public bool RemoveWearAndTear( DurabilityMod mymod, Item item, int amount ) {
			double wear = this.WearAndTear - amount;
			
			this.WearAndTear = Math.Max( wear, this.CalculateDurabilityLoss( mymod ) );

			bool is_broken = this.IsNowBroken( mymod, item );
			if( !this.IsBroken && is_broken ) {
				this.KillMe( mymod, item );
			}
			return !is_broken;
		}

		////////////////
		
		public bool IsNowBroken( DurabilityMod mymod, Item item ) {
			int max = DurabilityItemInfo.CalculateFullDurability( mymod, item );
			return this.WearAndTear >= max;
		}


		public bool CanRepair( DurabilityMod mymod, Item item ) {
			ConfigurationData data = mymod.Config.Data;
			bool can_repair_broken = !this.IsBroken || (data.CanRepairBroken && this.IsBroken);

			return data.CanRepair && can_repair_broken && this.WearAndTear > this.CalculateDurabilityLoss( mymod );
		}


		public void RepairMe( DurabilityMod mymod, Item item ) {
			this.Repairs++;

			if( this.RemoveWearAndTear( mymod, item, mymod.Config.Data.RepairAmount ) ) {
				Main.PlaySound( SoundID.Item37, item.position );
				this.IsBroken = false;
			}
		}


		public void UpdateCriticalState( DurabilityMod mymod, Item item ) {
			double max = DurabilityItemInfo.CalculateFullDurability( mymod, item );
			double ratio = (max - this.WearAndTear) / max;

			if( !this.IsCritical ) {
				if( ratio <= mymod.Config.Data.CriticalWarningPercent ) {
					this.IsCritical = true;

					var player = Main.player[Main.myPlayer];

					int ct = CombatText.NewText( player.getRect(), Color.Yellow, item.Name + " damaged!" );
					Main.combatText[ct].lifeTime = 100;
					Main.PlaySound( SoundID.NPCHit18, player.position );
				}
			} else {
				if( ratio > mymod.Config.Data.CriticalWarningPercent ) {
					this.IsCritical = false;
				}
			}
		}


		public void KillMe( DurabilityMod mymod, Item item ) {
			this.IsBroken = true;
			this.WearAndTear = DurabilityItemInfo.CalculateFullDurability( mymod, item );

			string item_name = item.Name;
			Player player = Main.player[ item.owner ];
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
			
			int ct = CombatText.NewText( player.getRect(), Color.DarkGray, item_name + " has broken!", false, true );
			Main.combatText[ct].lifeTime = 80;
		}
	}
}
