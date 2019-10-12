using HamstarHelpers.Classes.UI.ModConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;


namespace Durability {
	public class DurabilityMultiplierConfigEntry {
		[Range( 0f, 100f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float Multiplier { get; set; } = 1f;
	}




	public class DurabilityConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////////////////

		[Label( "Enable debug info mode" )]
		public bool DebugModeInfo { get; set; } = false;


		[Label("Enable mod (otherwise library mode)")]
		[DefaultValue( true )]
		public bool Enabled { get; set; } = true;


		////

		[Header( "Formula for computing durability:\n" +
			"  x = Sell value,\n" +
			"  y = Avg hits per second\n" +
			"  m = Multiply value (default 0.5)\n" +
			"  e = Exponent value (default 1.56)\n" +
			"  a = Addition value (default 50)\n \n" +
			"m * ( (y/4 * x^e) / (5 + x) ) + a" )]
		[JsonIgnore]
		[Label( "Computed durability of a plain Copper Pickaxe" )]
		public float ComputedCopperPickaxeDurability { get {
			if( DurabilityMod.Instance.Config == null ) { return 0f; }
			var item = new Item();
			item.SetDefaults( ItemID.CopperPickaxe, true );
			return DurabilityItemInfo.CalculateFullDurability( item, this );
		} }
		[Label( "Computed durability of a plain Copper Shortsword" )]
		public float ComputedCopperShortswordDurability { get {
			if( DurabilityMod.Instance.Config == null ) { return 0f; }
			var item = new Item();
			item.SetDefaults( ItemID.CopperShortsword, true );
			return DurabilityItemInfo.CalculateFullDurability( item, this );
		} }

		////
		
		[Header("Durability formula factors")]
		[Label( "Durability calculation factor (additive)" )]
		[Range(0, 100)]
		[DefaultValue( 50 )]
		public int DurabilityAdditive { get; set; } = 50;

		[Label( "Durability calculation factor (multiplier)" )]
		[Range(0f, 100)]
		[DefaultValue( 0.5f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float DurabilityMultiplier { get; set; } = 0.5f;

		[Label( "Durability calculation factor (exponent)" )]
		[Range( -10f, 10f )]
		[DefaultValue( 1.56f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float DurabilityExponent { get; set; } = 1.56f;


		[Label( "Non-tool-or-armor durability calculation factor (multiplier)" )]
		[Range( 0f, 100f )]
		[DefaultValue( 1f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float NonToolOrArmorDurabilityMultiplier { get; set; } = 1f;

		[Label( "Armor durability calculation factor (multiplier)" )]
		[Range( 0f, 1000f )]
		[DefaultValue( 2f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float ArmorDurabilityMultiplier { get; set; } = 2f;


		[Label( "Tool durability calculation factor (multiplier)" )]
		[Range( 0f, 100f )]
		[DefaultValue( 2f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float ToolDurabilityMultiplier { get; set; } = 2f;

		[Label( "Custom item durability calculation factors (multipliers)" )]
		public Dictionary<ItemDefinition, DurabilityMultiplierConfigEntry> PerItemDurabilityMultipliers { get; set; }


		[Header( "Durability settings" )]
		[Label( "Can items be repaired" )]
		[DefaultValue( true )]
		public bool CanRepair { get; set; } = true;

		[Label( "Can broken items be repaired" )]
		[DefaultValue( true )]
		public bool CanRepairBroken { get; set; } = true;

		[Label( "Amount per repair" )]
		[Range( 0, 10000 )]
		[DefaultValue( 250 )]
		public int RepairAmount { get; set; } = 250;

		[Label( "Max durability lost per repair" )]
		[Range( 0f, 1000f )]
		[DefaultValue( 25f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float MaxDurabilityLostPerRepair { get; set; } = 25f;


		[Label( "Durability percent before critical warning" )]
		[Range( 0f, 1f )]
		[DefaultValue( 0.2f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float CriticalWarningPercent { get; set; } = 0.2f;


		[Label( "Show durability quantity" )]
		[DefaultValue( true )]
		public bool ShowNumbers { get; set; } = true;

		[Label( "Show durability gauge bar" )]
		[DefaultValue( true )]
		public bool ShowBar { get; set; } = true;


		[Label( "General item wear and tear calculation factor (multiplier)" )]
		[Tooltip("'Wear and tear' is to durability loss what armor is to hp.")]
		[Range( 0f, 100f )]
		[DefaultValue( 1f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float GeneralWearAndTearMultiplier { get; set; } = 1f;

		[Label( "Armor item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 100f )]
		[DefaultValue( 1f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float ArmorWearAndTearMultiplier { get; set; } = 1f;

		[Label( "Tool item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 100f )]
		[DefaultValue( 0.5f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float ToolWearAndTearMultiplier { get; set; } = 0.5f;

		[Label( "Weapon item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 100f )]
		[DefaultValue( 1f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float WeaponWearAndTearMultiplier { get; set; } = 1f;

		[Label( "Fishing item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 100f )]
		[DefaultValue( 10f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float FishingWearAndTearMultiplier { get; set; } = 10f;

		[Label( "Grapple item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 100f )]
		[DefaultValue( 1f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float GrappleWearAndTearMultiplier { get; set; } = 1f;

		[Label( "Summon item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 100f )]
		[DefaultValue( 40f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float SummonWearAndTearMultiplier { get; set; } = 40f;

		[Label( "Wire item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 100f )]
		[DefaultValue( 1f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float WireWearAndTearMultiplier { get; set; } = 1f;

		[Label( "Melee projectile item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 100f )]
		[DefaultValue( 1f )]
		[CustomModConfigItem( typeof( FloatInputElement ) )]
		public float MeleeProjectileWearAndTearMultiplier { get; set; } = 1f;


		////

		[Header( "OLD SETTINGS BEYOND THIS POINT" )]
		public Dictionary<ItemDefinition, float> CustomDurabilityMultipliers { get; set; } = new Dictionary<ItemDefinition, float>();



		////////////////

		public DurabilityConfig() {
			this.PerItemDurabilityMultipliers = new Dictionary<ItemDefinition, DurabilityMultiplierConfigEntry> {
				// Ore items get durability boost
				{ new ItemDefinition(ItemID.IronHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.IronChainmail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.IronGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.IronAxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.IronHammer), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.IronPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.IronShortsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.IronBroadsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ "Iron Bow", 1.15f },
				{ new ItemDefinition(ItemID.LeadHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.LeadChainmail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.LeadGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.LeadAxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.LeadHammer), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.LeadPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.LeadShortsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.LeadBroadsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.LeadBow), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SilverHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SilverChainmail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SilverGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SilverAxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SilverHammer), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SilverPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SilverShortsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SilverBroadsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.SilverBow), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TungstenHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TungstenChainmail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TungstenGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TungstenAxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TungstenHammer), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TungstenPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TungstenShortsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TungstenBroadsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.TungstenBow), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.GoldHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.GoldChainmail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.GoldGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.GoldAxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.GoldHammer), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.GoldPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.GoldShortsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.GoldBroadsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.GoldBow), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PlatinumHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PlatinumChainmail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PlatinumGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PlatinumAxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PlatinumHammer), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PlatinumPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PlatinumShortsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PlatinumBroadsword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.PlatinumBow), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltMask), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltHat), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltBreastplate), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltChainsaw), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltWaraxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltDrill), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltSword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.CobaltNaginata), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.CobaltRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumMask), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumHeadgear), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumBreastplate), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumChainsaw), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumWaraxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumDrill), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumSword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.PalladiumPike), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.PalladiumRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilHat), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilHood), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilChainmail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilChainsaw), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilWaraxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilDrill), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilSword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.MythrilHalberd), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.MythrilRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumMask), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumHeadgear), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumBreastplate), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumChainsaw), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumWaraxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumDrill), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumSword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.OrichalcumHalberd), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.OrichalcumRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteMask), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteHeadgear), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteBreastplate), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteChainsaw), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteWaraxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantitePickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteDrill), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteSword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AdamantiteGlaive), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.AdamantiteRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumMask), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumHeadgear), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumBreastplate), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumChainsaw), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumWaraxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumDrill), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumSword), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TitaniumTrident), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.TitaniumRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.FrostHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.FrostBreastplate), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AncientBattleArmorPants), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AncientBattleArmorHat), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.AncientBattleArmorShirt), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.TitaniumRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteMask), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteHeadgear), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophytePlateMail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophytePickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteDrill), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteChainsaw), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteGreataxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteJackhammer), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteWarhammer), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteSaber), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophyteClaymore), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ChlorophytePartisan), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TurtleHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TurtleScaleMail), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.TurtleLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.VenomStaff), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				//{ new ItemDefinition(ItemID.TitaniumRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SpectreMask), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SpectreHood), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SpectreRobe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SpectrePants), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SpectrePickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.SpectreHamaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ShroomiteMask), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ShroomiteHeadgear), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ShroomiteHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ShroomiteBreastplate), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ShroomiteLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
				{ new ItemDefinition(ItemID.ShroomiteDiggingClaw), new DurabilityMultiplierConfigEntry { Multiplier = 1.15f } },
			
				// Cactus stuffs very easily acquired
				{ new ItemDefinition(ItemID.CactusSword), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.CactusPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.CactusHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.CactusBreastplate), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.CactusLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },

				// Meteor stuffs easily (albeit unpredictably) renewable
				{ new ItemDefinition(ItemID.MeteorHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.MeteorSuit), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.MeteorLeggings), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.SpaceGun), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.MeteorHamaxe), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },

				// Demonite stuffs farmable
				{ new ItemDefinition(ItemID.DemonBow), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.WarAxeoftheNight), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.LightsBane), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.ShadowHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.ShadowScalemail), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.ShadowGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.TheBreaker), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.CorruptYoyo), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.NightmarePickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1f } }, //0.85f } },
			
				// Crimtain stuffs farmable
				{ new ItemDefinition(ItemID.TendonBow), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.BloodLustCluster), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.BloodButcherer), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.CrimsonHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.CrimsonScalemail), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.CrimsonGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.FleshGrinder), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.TheMeatball), new DurabilityMultiplierConfigEntry { Multiplier = 0.85f } },
				{ new ItemDefinition(ItemID.DeathbringerPickaxe), new DurabilityMultiplierConfigEntry { Multiplier = 1f } },	//0.85f } },
			
				// Hallowed stuffs (semi) farmable
				{ new ItemDefinition(ItemID.HallowedMask), new DurabilityMultiplierConfigEntry { Multiplier = 0.9f } },
				{ new ItemDefinition(ItemID.HallowedHelmet), new DurabilityMultiplierConfigEntry { Multiplier = 0.9f } },
				{ new ItemDefinition(ItemID.HallowedHeadgear), new DurabilityMultiplierConfigEntry { Multiplier = 0.9f } },
				{ new ItemDefinition(ItemID.HallowedPlateMail), new DurabilityMultiplierConfigEntry { Multiplier = 0.9f } },
				{ new ItemDefinition(ItemID.HallowedGreaves), new DurabilityMultiplierConfigEntry { Multiplier = 0.9f } },
				{ new ItemDefinition(ItemID.Drax), new DurabilityMultiplierConfigEntry { Multiplier = 1f } },	//0.9f } },
				{ new ItemDefinition(ItemID.PickaxeAxe), new DurabilityMultiplierConfigEntry { Multiplier = 1f } },	//0.9f } },
				{ new ItemDefinition(ItemID.HallowedRepeater), new DurabilityMultiplierConfigEntry { Multiplier = 0.9f } },
				{ new ItemDefinition(ItemID.Excalibur), new DurabilityMultiplierConfigEntry { Multiplier = 0.9f } },	//0.9f } },
				{ new ItemDefinition(ItemID.Gungnir), new DurabilityMultiplierConfigEntry { Multiplier = 0.9f } },

				// 2 shots per fire
				{ new ItemDefinition(ItemID.VortexBeater), new DurabilityMultiplierConfigEntry { Multiplier = 0.5f } }
			};
		}

		////

		public override ModConfig Clone() {
			var clone = (DurabilityConfig)base.Clone();

			clone.PerItemDurabilityMultipliers = this.PerItemDurabilityMultipliers
				.ToDictionary( kv => kv.Key, kv => kv.Value );

			return clone;
		}
	}
}
