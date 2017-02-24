using System;
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

		public static int CalculateWearAndTear( Item item ) {
			int val = item.value;

			if( val == 0 ) {
				val = (item.damage * 1000) + (item.defense * 1000);
				if( val <= 0 ) {
					return 100;
				}
				if( item.rare > 0) {
					val *= item.rare;
				}
			}
			
			ConfigurationData data = DurabilityMod.Config.Data;
			float mul = data.DurabilityMultiplier;
			float exp = data.DurabilityExponent;
			int add = data.DurabilityAdditive;
			int durability = (int)(mul * Math.Pow((float)val, exp) / (5 + val)) + add;
			
			if( ItemHelper.IsArmor(item) ) {
				durability = (int)((float)durability * data.ArmorDurabilityMultiplier);
			}
			if( ItemHelper.IsTool(item) ) {
				durability = (int)((float)durability * data.ToolDurabilityMultiplier);
			}
			if( data.CustomDurabilityMultipliers.Keys.Contains(item.name) ) {
				durability = (int)( (float)durability * data.CustomDurabilityMultipliers[item.name] );
			}

			return durability;
		}


		////////////////
		
		public bool HasDurability( Item item ) {
			return (ItemHelper.IsTool( item ) || ItemHelper.IsArmor( item )) && !this.IsUnbreakable;
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

		public void Use( Item item, int hits = 1, double scale_override = 0d ) {
			if( !this.HasDurability( item ) || this.ConcurrentUses >= DurabilityItemInfo.MaxConcurrentUses ) { return; }

			ConfigurationData data = DurabilityMod.Config.Data;
			double wear = (double)hits * (double)data.WearMultiplier;
			int max = DurabilityItemInfo.CalculateWearAndTear( item );

			// If we're not using a pick, scale according to use time (supplements item.value)
			if( scale_override == 0 ) {
				if( ItemHelper.IsPenetratorMelee(item) ) {
					scale_override = 1d;
				} else {
					scale_override = (double)ItemHelper.CalculateStandardUseTime( item ) / 16d;
					scale_override = scale_override <= 0d ? 1d : scale_override;
				}
			}
			wear *= scale_override;

			this.WearAndTear += wear;
			this.ConcurrentUses++;
			this.RecentUseDisplayBarAnimate = 8;
//Debug.Display[ item.name ] = this.WearAndTear.ToString("N2")+" : "+max.ToString("N2") + " (" + wear.ToString("N2") + ":"+ data.WearMultiplier+":"+ scale_override.ToString("N2") + ")";

			if( this.WearAndTear >= max ) {
				this.WearAndTear = max;
				this.KillMe( item );
			}
		}


		public void RepairMe( Item item ) {
			ConfigurationData data = DurabilityMod.Config.Data;
			double wear = this.WearAndTear - data.RepairAmount;
			int max = DurabilityItemInfo.CalculateWearAndTear( item );

			this.Repairs++;
			this.WearAndTear = Math.Max( wear, (double)this.Repairs * data.RepairDegradationMultiplier );

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
