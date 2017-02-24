using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Utils;

namespace Durability {
    class DurabilityGlobalItem : GlobalItem {
		public override bool NeedsSaving( Item item ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(mod);
			return info != null && info.HasDurability(item) ? true : base.NeedsSaving(item);
		}


		public override void LoadLegacy( Item item, BinaryReader reader ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( info != null ) {
				info.Initialize( item, (int)reader.ReadInt32() );
			}
		}

		public override void Load( Item item, TagCompound tag ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( info != null ) {
				info.Initialize( item, tag.GetDouble("wear_and_tear_d") );
			}
		}

		public override TagCompound Save( Item item ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( info == null ) {
				return new TagCompound { };
			}
			return new TagCompound {
				{"wear_and_tear_d", (double)info.WearAndTear}
			};
		}

		/*public override void NetReceive( Item item, BinaryReader reader ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( info.HasDurability(item) ) {
				info.WearAndTear = reader.ReadInt32();
			}
		}

		public override void NetSend( Item item, BinaryWriter writer ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( info.HasDurability( item ) ) {
				writer.Write( info.WearAndTear );
			}
		}*/

		////////////////

		public override void PostDrawInInventory( Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale ) {
			if( item.type == 0 ) { return; }
			
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(this.mod);
			if( !info.HasDurability( item ) ) { return; }

			int max = DurabilityItemInfo.CalculateWearAndTear( item );
			Vector2 pos = new Vector2( position.X + ((frame.Width / 2) * scale), position.Y + ((frame.Height - 6) * scale) );
			float alpha = 0.6f + (0.05f * info.RecentUseDisplayBarAnimate);
			if( info.RecentUseDisplayBarAnimate > 0 ) {
				info.RecentUseDisplayBarAnimate--;
			}

			if( DurabilityMod.Config.Data.ShowBar ) {
				UI.DrawHealthBar( sb, pos.X, pos.Y, max - (int)info.WearAndTear, max, alpha, scale );
			}
			if( DurabilityMod.Config.Data.ShowNumbers ) {
				UI.DrawHealthText( sb, pos.X, pos.Y, max - (int)info.WearAndTear, alpha );
			}
		}

		////////////////

		private DurabilityItemInfo CurrentReforgeDurability = null;

		public override void PreReforge( Item item ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(this.mod);
			if( info.HasDurability( item ) ) {
				this.CurrentReforgeDurability = info;
			}
		}

		public override void PostReforge( Item item ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( this.CurrentReforgeDurability != null && info.HasDurability(item) ) {
				info.CopyToMe( this.CurrentReforgeDurability );

				if( DurabilityMod.Config.Data.CanRepair ) {
					info.RepairMe( item );
				}
			}
			this.CurrentReforgeDurability = null;
		}

		public override void OnCraft( Item item, Recipe recipe ) {
			DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>( this.mod );

			info.IsUnbreakable = false;
			//info.IsUnbreakable = info.HasDurability( item );	// Doesn't work?
		}
	}

}
