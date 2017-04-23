using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( item_info != null ) {
				item_info.Initialize( item, (int)reader.ReadInt32() );
			}
		}

		public override void Load( Item item, TagCompound tag ) {
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( item_info != null ) {
				item_info.Initialize( item, tag.GetDouble("wear_and_tear_d") );
			}
		}

		public override TagCompound Save( Item item ) {
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( item_info == null ) {
				return new TagCompound { };
			}
			return new TagCompound {
				{"wear_and_tear_d", (double)item_info.WearAndTear}
			};
		}

		public override void NetReceive( Item item, BinaryReader reader ) {
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			item_info.NetReceive( item, reader );
		}

		public override void NetSend( Item item, BinaryWriter writer ) {
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			item_info.NetSend( item, writer );
		}

		////////////////

		public override void PostDrawInInventory( Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color draw_color, Color item_color, Vector2 origin, float scale ) {
			if( item == null || item.IsAir ) { return; }

			var mymod = (DurabilityMod)this.mod;
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			if( !item_info.HasDurability( item ) ) { return; }

			int max = DurabilityItemInfo.CalculateFullDurability( mymod, item );
			int hp = max - (int)item_info.WearAndTear;
			float alpha = 0.6f + (0.05f * item_info.RecentUseDisplayBarAnimate);
			Color color = UI.GetHealthBarColor( hp, max, alpha );
			float pos_x = position.X + (((float)frame.Width / 2f) * scale);
			float pos_y = position.Y + (((float)frame.Height / 2f) * scale);

			if( item_info.RecentUseDisplayBarAnimate > 0 ) {
				item_info.RecentUseDisplayBarAnimate--;
			}

			if( mymod.Config.Data.ShowBar ) {
				UI.DrawHealthBar( sb, pos_x + 1f, pos_y + 5f, max - (int)item_info.WearAndTear, max, color, 0.8f );
			}

			if( mymod.Config.Data.ShowNumbers ) {
				var player = Main.player[ Main.myPlayer ];
				Item tooltip = Main.toolTip;
				Item selected = player.inventory[ player.selectedItem ];
				Item mouse = Main.mouseItem;

				if( (tooltip != null && !tooltip.IsAir && !tooltip.IsNotTheSameAs(item))
						|| (selected != null && !selected.IsAir && !selected.IsNotTheSameAs(item))
						|| (mouse != null && !mouse.IsAir && !mouse.IsNotTheSameAs(item)) ) {
					Color t_color = new Color( Math.Min(color.R + 64, 255), Math.Min(color.G + 64, 255), Math.Min(color.B + 64, 255), color.A );
					UI.DrawHealthText( sb, pos_x, pos_y, hp, t_color );
				}
			}
		}

		////////////////

		private DurabilityItemInfo CurrentReforgeDurability = null;

		public override void PreReforge( Item item ) {
			var item_info = item.GetModInfo<DurabilityItemInfo>(this.mod);
			if( item_info.HasDurability( item ) ) {
				this.CurrentReforgeDurability = item_info;
			}
		}

		public override void PostReforge( Item item ) {
			var mymod = (DurabilityMod)this.mod;
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );

			if( this.CurrentReforgeDurability != null && item_info.HasDurability(item) ) {
				item_info.CopyToMe( this.CurrentReforgeDurability );

				if( mymod.Config.Data.CanRepair ) {
					item_info.RemoveWearAndTear( mymod, item );
				}
			}
			this.CurrentReforgeDurability = null;
		}

		public override void OnCraft( Item item, Recipe recipe ) {
			DurabilityItemInfo item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );

			item_info.IsUnbreakable = false;
			//info.IsUnbreakable = info.HasDurability( item );	// Doesn't work?
		}
	}

}
