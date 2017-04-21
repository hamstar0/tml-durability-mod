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

		public int ConcurrentUses;
		public int RecentUseDisplayBarAnimate;

		private bool IsInitialized = false;
		
		////////////////

		public static int CalculateFullDurability( DurabilityMod mymod, Item item ) {
			ConfigurationData data = mymod.Config.Data;
			double val = item.value;
			double mul = data.DurabilityMultiplier;
			double add = data.DurabilityAdditive;
			double hits_per_sec = 60d / ItemHelper.CalculateStandardUseTime( item );
			bool is_armor = ItemHelper.IsArmor( item );
			bool is_tool = ItemHelper.IsTool( item );

			if( is_armor && !is_tool ) { hits_per_sec = 1d; }

			if( val == 0 ) {
				val = ((double)item.damage * hits_per_sec * 1000d) + ((double)item.defense * 1000d);

				if( item.rare > 1 ) { val *= (double)item.rare; }
				if( val <= 0 ) {
					return 100 * (item.rare > 0 ? item.rare : 1);    // Fallback
				}
			}

			double pow = Math.Pow( val, data.DurabilityExponent );
			double durability = (((hits_per_sec / 4d) * mul * pow) / (5d + val)) + add;
			
			if( is_armor ) {
				durability *= data.ArmorDurabilityMultiplier;
			}
			if( is_tool ) {
				durability *= data.ToolDurabilityMultiplier;
			}
			if( data.CustomDurabilityMultipliers.Keys.Contains(item.name) ) {
				durability *= data.CustomDurabilityMultipliers[item.name];
			}

			return (int)durability;
		}


		////////////////
		
		public bool HasDurability( Item item ) {
			return (ItemHelper.IsTool(item) || ItemHelper.IsArmor(item) || ItemHelper.IsGrapple(item))
				&& !this.IsUnbreakable;
		}

		////////////////

		public override ItemInfo Clone() {
			var clone = (DurabilityItemInfo)base.Clone();
			clone.WearAndTear = this.WearAndTear;
			clone.Repairs = this.Repairs;
			clone.IsUnbreakable = this.IsUnbreakable;
			clone.IsInitialized = this.IsInitialized;

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
		}

		////////////////

		public void NetReceive( Item item, BinaryReader reader ) {
			this.WearAndTear = reader.ReadDouble();
			this.IsUnbreakable = reader.ReadBoolean();
			this.Repairs = reader.ReadInt32();
			this.IsInitialized = true;
		}

		public void NetSend( Item item, BinaryWriter writer ) {
			writer.Write( this.WearAndTear );
			writer.Write( this.IsUnbreakable );
			writer.Write( this.Repairs );
		}

		////////////////

		public void AddWearAndTear( DurabilityMod mymod, Item item, int hits = 1, double scale_override = 1d ) {
			if( scale_override == 0 ) { return; }
			if( !this.HasDurability( item ) || this.ConcurrentUses >= DurabilityItemInfo.MaxConcurrentUses ) { return; }

			ConfigurationData data = mymod.Config.Data;
			int max = DurabilityItemInfo.CalculateFullDurability( mymod, item );

			this.WearAndTear += (double)hits * (double)data.GeneralWearAndTearMultiplier * scale_override;
			this.ConcurrentUses++;
			this.RecentUseDisplayBarAnimate = 8;
//Debug.Display[ item.name ] = this.WearAndTear.ToString("N2")+" : "+max.ToString("N2") + " (" + wear.ToString("N2") + ":"+ data.WearMultiplier+":"+ scale_override.ToString("N2") + ")";

			if( this.WearAndTear >= max ) {
				this.WearAndTear = max;
				this.KillMe( item );
			}
		}


		public void RepairMe( DurabilityMod mymod, Item item ) {
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
		}
	}
}
