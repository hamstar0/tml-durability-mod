using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Utils;

namespace Durability.Items {
	public class SmithingHammerItem : ModItem {
		public override void SetDefaults() {
			this.item.name = "Smithing Hammer";
			this.item.damage = 7;
			this.item.melee = true;
			this.item.width = 40;
			this.item.height = 40;
			this.item.toolTip = "Specialized hammer for repairing armor at an anvil.";
			this.item.useTime = 15;
			this.item.useAnimation = 15;
			this.item.hammer = 40;
			this.item.useStyle = 1;
			this.item.knockBack = 6;
			this.item.value = 20000;
			this.item.rare = 1;
			this.item.UseSound = SoundID.Item1;
			this.item.autoReuse = false;
		}

		public override void AddRecipes() {
			for( int i = 1; i < Main.itemName.Length; i++ ) {   // Main.itemTexture?
				Item item = new Item();
				item.SetDefaults( i );

				if( !ItemHelper.IsArmor(item) ) { continue; }

				var recipe = new SmithedArmorRecipe( (DurabilityMod)this.mod, item );
				recipe.AddRecipe();
			}
		}
	}



	class SmithedArmorRecipe : ModRecipe {
		public int ItemType { get; private set; }
		private DurabilityItemInfo PrevInfo = null;

		////////////////

		public SmithedArmorRecipe( DurabilityMod mymod, Item item ) : base( mymod ) {
			this.ItemType = item.type;

			this.AddIngredient( mymod, "SmithingHammerItem", 1 );
			this.AddTile( 16 );   // Anvil

			//if( item.modItem != null ) {
			//	this.AddIngredient( item.modItem, 1 );
			//} else {
			//	this.AddIngredient( item.name, 1 );
			//}
			this.AddIngredient( item.type, 1 );
			this.SetResult( item.type );
		}

		////////////////

		public override int ConsumeItem( int item_type, int quantity ) {
			var mymod = (DurabilityMod)this.mod;
			Player player = Main.player[ Main.myPlayer ];
			Item item = ItemHelper.FindFirstPlayerItemOfType( player, item_type );
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );

			if( this.mod.ItemType("SmithingHammerItem") == item_type ) {
				int max_wear = DurabilityItemInfo.CalculateFullDurability( mymod, item );
				int wear = (max_wear / 3) + 1;

				item_info.AddWearAndTear( mymod, item, wear, 1 );
				return 0;
			} else if( item != null ) {
				this.PrevInfo = (DurabilityItemInfo)item_info.Clone();
			}

			return quantity;
		}

		public override bool RecipeAvailable() {
			var mymod = (DurabilityMod)this.mod;
			bool can_repair = mymod.Config.Data.CanRepair;

			if( can_repair && Main.netMode != 2 ) {
				Player player = Main.player[Main.myPlayer];
				Item item = ItemHelper.FindFirstPlayerItemOfType( player, this.ItemType );

				if( item != null && !item.IsAir ) {
					var item_info = item.GetModInfo<DurabilityItemInfo>( mymod );
					can_repair = item_info.CanRepair( mymod, item );
				}
			}
			return can_repair;
		}

		public override void OnCraft( Item item ) {
			if( this.PrevInfo == null ) {
				ErrorLogger.Log( "Could not find previous WearAndTear info for " + item.name + " (" + item.type + ")" );
				return;
			}
			if( item.type != this.ItemType ) {
				ErrorLogger.Log( "Mismatched item type for " + item.name + ": Found " + item.type + ", expected "+this.ItemType );
				return;
			}

			var mymod = (DurabilityMod)this.mod;
			var item_info = item.GetModInfo<DurabilityItemInfo>( this.mod );
			item_info.CopyToMe( this.PrevInfo );

			item_info.RepairMe( mymod, item );
		}
	}
}