using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Utils;


namespace Durability {
	public class DurabilityItemInfo : ItemInfo {
		private static int MaxConcurrentUses = 5;
		
		public double WearAndTear { get; private set; }
		public bool IsUnbreakable = false;
		public int Repairs = 0;
		public bool IsCritical = false;

		public int ConcurrentUses;
		public int RecentUseDisplayBarAnimate;

		private bool IsInitialized = false;
		
		////////////////

		public static int CalculateFullDurability( DurabilityMod mymod, Item item ) {
			ConfigurationData data = mymod.Config.Data;
			double val = item.value;
			double mul = data.DurabilityMultiplier;
			double add = data.DurabilityAdditive;
			double hits_per_sec = 60d / (double)ItemHelper.CalculateStandardUseTime( item );
			bool is_armor = ItemHelper.IsArmor( item );
			bool is_tool = ItemHelper.IsTool( item );

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
			double durability = (((hits_per_sec / 4d) * mul * pow) / (5d + val)) + add;
			
			if( is_armor ) {
				durability *= data.ArmorMaxDurabilityMultiplier;
			}
			if( is_tool ) {
				durability *= data.ToolMaxDurabilityMultiplier;
			}
			if( !is_tool && !is_armor ) {
				durability *= data.NonToolOrArmorMaxDurabilityMultiplier;
			}

			if( data.CustomDurabilityMultipliers.Keys.Contains(item.name) ) {
				durability *= data.CustomDurabilityMultipliers[item.name];
			}

			return (int)durability;
		}


		////////////////
		
		public bool HasDurability( Item item ) {
			bool is_handy = ItemHelper.IsTool( item ) || ItemHelper.IsArmor( item ) || ItemHelper.IsGrapple( item );
			return is_handy && !this.IsUnbreakable && !item.consumable;
		}

		////////////////

		public override ItemInfo Clone() {
			var clone = (DurabilityItemInfo)base.Clone();
			clone.WearAndTear = this.WearAndTear;
			clone.Repairs = this.Repairs;
			clone.IsUnbreakable = this.IsUnbreakable;
			clone.IsInitialized = this.IsInitialized;
			clone.IsCritical = this.IsCritical;

			return clone;
		}

		public void Initialize( Item item, double wear ) {
			if( this.IsInitialized ) {
				ErrorLogger.Log( "Item already initialized: "+item.name );
			} else {
				this.WearAndTear = wear;
				this.IsInitialized = true;
			}
		}

		public void CopyToMe( DurabilityItemInfo info ) {
			this.WearAndTear = info.WearAndTear;
			this.IsUnbreakable = info.IsUnbreakable;
			this.Repairs = info.Repairs;
			this.IsInitialized = true;
			this.IsCritical = info.IsCritical;
		}

		////////////////

		public void NetReceive( Item item, BinaryReader reader ) {
			this.WearAndTear = reader.ReadDouble();
			this.IsUnbreakable = reader.ReadBoolean();
			this.Repairs = reader.ReadInt32();
			this.IsInitialized = true;
			this.IsCritical = reader.ReadBoolean();
		}

		public void NetSend( Item item, BinaryWriter writer ) {
			writer.Write( this.WearAndTear );
			writer.Write( this.IsUnbreakable );
			writer.Write( this.Repairs );
			writer.Write( this.IsCritical );
		}

		////////////////

		public void AddWearAndTear( DurabilityMod mymod, Item item, int hits = 1, double multiplier = 1d ) {
			if( !this.HasDurability( item ) || this.ConcurrentUses >= DurabilityItemInfo.MaxConcurrentUses ) { return; }

			this.AddWearAndTearForMe( mymod, item, hits, multiplier );

			// Propagate effect to mouse item, if applicable
			if( Main.netMode != 2 && item.owner == Main.myPlayer ) {
				if( Main.mouseItem != null && !Main.mouseItem.IsAir && !Main.mouseItem.IsNotTheSameAs( item ) ) {
					DurabilityItemInfo mouse_item_info = Main.mouseItem.GetModInfo<DurabilityItemInfo>( mymod );
					mouse_item_info.AddWearAndTearForMe( mymod, Main.mouseItem, 1, multiplier );
				}
			}
		}

		private void AddWearAndTearForMe( DurabilityMod mymod, Item item, int hits, double multiplier ) {
			ConfigurationData data = mymod.Config.Data;
			int max = DurabilityItemInfo.CalculateFullDurability( mymod, item );

			this.WearAndTear += (double)hits * (double)data.GeneralWearAndTearMultiplier * multiplier;
			this.ConcurrentUses++;
			this.RecentUseDisplayBarAnimate = 8;
			
			if( this.WearAndTear >= max ) {
				this.WearAndTear = max;
				this.KillMe( item );
			}
		}


		public void UpdateCriticalState( DurabilityMod mymod, Item item ) {
			double max = DurabilityItemInfo.CalculateFullDurability( mymod, item );
			double ratio = (max - this.WearAndTear) / max;

			if( !this.IsCritical ) {
				if( ratio <= mymod.Config.Data.CriticalWarningPercent ) {
					this.IsCritical = true;

					var player = Main.player[Main.myPlayer];
					
					int ct = CombatText.NewText( player.getRect(), Color.Yellow, item.name+" damaged!" );
					Main.combatText[ct].lifeTime = 100;
					Main.PlaySound( SoundID.NPCHit18, player.position );
				}
			} else {
				if( ratio > mymod.Config.Data.CriticalWarningPercent ) {
					this.IsCritical = false;
				}
			}
		}


		public void RemoveWearAndTear( DurabilityMod mymod, Item item ) {
			ConfigurationData data = mymod.Config.Data;
			double wear = this.WearAndTear - data.RepairAmount;
			int max = DurabilityItemInfo.CalculateFullDurability( mymod, item );

			this.Repairs++;
			this.WearAndTear = Math.Max( wear, (double)this.Repairs * data.MaxDurabilityLostPerRepair );

			if( this.WearAndTear >= max ) {
				this.WearAndTear = max;
				this.KillMe( item );
			} else {
				Main.PlaySound( SoundID.Item37, item.position );
			}
		}


		public void KillMe( Item item ) {
			string item_name = item.name;
			Player player = Main.player[item.owner];
			player.AddBuff(23, 1);
			player.noItems = true;

			//int soundSlot = mod.GetSoundSlot(SoundType.Item, "Sounds/Item/ItemBreak");
			//Main.PlaySound( 2, player.position, soundSlot );
			Main.PlaySound( 13, player.position );
			Main.PlaySound( 3, player.position, 18 );

			item.SetDefaults( 0, false );
			item.netID = 0;
			item.type = 0;
			item.stack = 0;
			item.name = "";
			item.useStyle = 0;
			item.useTime = 0;
			item.useAnimation = 0;
			
			int ct = CombatText.NewText( player.getRect(), Color.DarkGray, item_name + " has broken!", false, true );
			Main.combatText[ct].lifeTime = 80;
		}
	}
}
