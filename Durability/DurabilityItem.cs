using HamstarHelpers.Helpers.HUD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace Durability {
    class DurabilityItem : GlobalItem {
		public override void PostDrawInInventory( Item item, SpriteBatch sb, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }
			if( item == null || item.IsAir ) { return; }

			var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();
			if( !itemInfo.HasDurability( item ) ) { return; }
			
			if( !itemInfo.IsBroken ) {
				int max = DurabilityItemInfo.CalculateFullDurability( item );
				int hp = max - (int)itemInfo.WearAndTear;

				float alpha = 0.6f + (0.05f * itemInfo.RecentUseDisplayBarAnimate);
				Color color = HUDHealthBarHelpers.GetHealthBarColor( hp, max, alpha );
				float posX = position.X + (((float)frame.Width / 2f) * scale);
				float posY = position.Y + (((float)frame.Height / 2f) * scale);

				if( itemInfo.RecentUseDisplayBarAnimate > 0 ) {
					itemInfo.RecentUseDisplayBarAnimate--;
				}

				if( mymod.Config.ShowBar ) {
					HUDHealthBarHelpers.DrawHealthBar( sb, posX + 1f, posY + 5f, max - (int)itemInfo.WearAndTear, max, color, 0.8f );
				}

				if( mymod.Config.ShowNumbers ) {
					var player = Main.player[ Main.myPlayer ];
					Item hoverItem = Main.HoverItem;
					Item selected = player.inventory[ player.selectedItem ];
					Item mouse = Main.mouseItem;

					if( (hoverItem != null && !hoverItem.IsAir && !hoverItem.IsNotTheSameAs(item))
							|| (selected != null && !selected.IsAir && !selected.IsNotTheSameAs(item))
							|| (mouse != null && !mouse.IsAir && !mouse.IsNotTheSameAs(item)) ) {
						Color tColor = new Color( Math.Min(color.R + 64, 255), Math.Min(color.G + 64, 255), Math.Min(color.B + 64, 255), color.A );
						HUDHealthBarHelpers.DrawHealthText( sb, posX, posY, hp, tColor );
					}
				}
			} else {
				float posX = position.X + (((float)frame.Width / 2f) * scale) - (((float)mymod.DestroyedTex.Width / 2f) * scale);
				float posY = position.Y + (((float)frame.Height / 2f) * scale) - (((float)mymod.DestroyedTex.Height / 2f) * scale);

				sb.Draw( mymod.DestroyedTex, new Vector2(posX, posY), Color.White );
			}
		}

		
		public override void ModifyTooltips( Item item, List<TooltipLine> tooltips ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }

			var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();
			int maxLoss = itemInfo.CalculateDurabilityLoss();
			if( maxLoss == 0 ) { return; }

			var tip = new TooltipLine( mymod, "max_durability_loss", "Durability lost to repairs: " + maxLoss ) {
				overrideColor = Color.Red
			};
			tooltips.Add( tip );
		}

		////////////////

		private static DurabilityItemInfo CurrentReforgeDurability = null;

		public override bool NewPreReforge( Item item ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return true; }

			var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();
			if( itemInfo.HasDurability( item ) ) {
				DurabilityItem.CurrentReforgeDurability = itemInfo;
			}
			return true;
		}

		public override void PostReforge( Item item ) {
			var mymod = (DurabilityMod)this.mod;
			if( !mymod.Config.Enabled ) { return; }

			var itemInfo = item.GetGlobalItem<DurabilityItemInfo>();

			if( DurabilityItem.CurrentReforgeDurability != null && itemInfo.HasDurability(item) ) {
				itemInfo.CopyToMe( DurabilityItem.CurrentReforgeDurability );

				if( mymod.Config.CanRepair ) {
					itemInfo.RepairMe( item );
				}
			}
			DurabilityItem.CurrentReforgeDurability = null;
		}

		public override void OnCraft( Item item, Recipe recipe ) {
			DurabilityItemInfo itemInfo = item.GetGlobalItem<DurabilityItemInfo>();

			itemInfo.IsUnbreakable = false;
			//info.IsUnbreakable = info.HasDurability( item );	// Doesn't work?
		}
	}

}
