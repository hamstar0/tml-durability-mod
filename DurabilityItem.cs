using HamstarHelpers.HudHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
    class DurabilityItem : GlobalItem {
		public override void PostDrawInInventory( Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color draw_color, Color item_color, Vector2 origin, float scale ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }
			if( item == null || item.IsAir ) { return; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>( this.mod );
			if( !item_info.HasDurability( item ) ) { return; }
			
			if( !item_info.IsBroken ) {
				int max = DurabilityItemInfo.CalculateFullDurability( mymod, item );
				int hp = max - (int)item_info.WearAndTear;

				float alpha = 0.6f + (0.05f * item_info.RecentUseDisplayBarAnimate);
				Color color = HudHealthBarHelpers.GetHealthBarColor( hp, max, alpha );
				float pos_x = position.X + (((float)frame.Width / 2f) * scale);
				float pos_y = position.Y + (((float)frame.Height / 2f) * scale);

				if( item_info.RecentUseDisplayBarAnimate > 0 ) {
					item_info.RecentUseDisplayBarAnimate--;
				}

				if( mymod.Config.Data.ShowBar ) {
					HudHealthBarHelpers.DrawHealthBar( sb, pos_x + 1f, pos_y + 5f, max - (int)item_info.WearAndTear, max, color, 0.8f );
				}

				if( mymod.Config.Data.ShowNumbers ) {
					var player = Main.player[ Main.myPlayer ];
					Item hover_item = Main.HoverItem;
					Item selected = player.inventory[ player.selectedItem ];
					Item mouse = Main.mouseItem;

					if( (hover_item != null && !hover_item.IsAir && !hover_item.IsNotTheSameAs(item))
							|| (selected != null && !selected.IsAir && !selected.IsNotTheSameAs(item))
							|| (mouse != null && !mouse.IsAir && !mouse.IsNotTheSameAs(item)) ) {
						Color t_color = new Color( Math.Min(color.R + 64, 255), Math.Min(color.G + 64, 255), Math.Min(color.B + 64, 255), color.A );
						HudHealthBarHelpers.DrawHealthText( sb, pos_x, pos_y, hp, t_color );
					}
				}
			} else {
				float pos_x = position.X + (((float)frame.Width / 2f) * scale) - (((float)mymod.DestroyedTex.Width / 2f) * scale);
				float pos_y = position.Y + (((float)frame.Height / 2f) * scale) - (((float)mymod.DestroyedTex.Height / 2f) * scale);

				sb.Draw( mymod.DestroyedTex, new Vector2(pos_x, pos_y), Color.White );
			}
		}

		
		public override void ModifyTooltips( Item item, List<TooltipLine> tooltips ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>( mymod );
			int max_loss = item_info.CalculateDurabilityLoss( mymod );
			if( max_loss == 0 ) { return; }

			var tip = new TooltipLine( mymod, "max_durability_loss", "Durability lost to repairs: "+max_loss );
			tip.overrideColor = Color.Red;
			tooltips.Add( tip );
		}

		////////////////

		private static DurabilityItemInfo CurrentReforgeDurability = null;

		public override void PreReforge( Item item ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>(this.mod);
			if( item_info.HasDurability( item ) ) {
				DurabilityItem.CurrentReforgeDurability = item_info;
			}
		}

		public override void PostReforge( Item item ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Data.Enabled ) { return; }

			var item_info = item.GetGlobalItem<DurabilityItemInfo>( this.mod );

			if( DurabilityItem.CurrentReforgeDurability != null && item_info.HasDurability(item) ) {
				item_info.CopyToMe( DurabilityItem.CurrentReforgeDurability );

				if( mymod.Config.Data.CanRepair ) {
					item_info.RepairMe( mymod, item );
				}
			}
			DurabilityItem.CurrentReforgeDurability = null;
		}

		public override void OnCraft( Item item, Recipe recipe ) {
			DurabilityItemInfo item_info = item.GetGlobalItem<DurabilityItemInfo>( this.mod );

			item_info.IsUnbreakable = false;
			//info.IsUnbreakable = info.HasDurability( item );	// Doesn't work?
		}
	}

}
