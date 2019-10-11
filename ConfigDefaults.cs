using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;


namespace Durability {
	public class DurabilityConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////////////////

		[Label("Enable debug info mode")]
		public bool DebugModeInfo = false;


		[Label("Enable mod (otherwise library mode)")]
		[DefaultValue( true )]
		public bool Enabled = true;


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
			var item = new Item();
			item.SetDefaults( ItemID.CopperPickaxe, true );
			return DurabilityItemInfo.CalculateFullDurability( item );
		} }
		[Label( "Computed durability of a plain Copper Shortsword" )]
		public float ComputedCopperShortswordDurability { get {
			var item = new Item();
			item.SetDefaults( ItemID.CopperShortsword, true );
			return DurabilityItemInfo.CalculateFullDurability( item );
		} }

		////
		
		[Header("Durability formula factors")]
		[Label( "Durability calculation factor (additive)" )]
		[Range(0, 1000)]
		[DefaultValue( 50 )]
		public int DurabilityAdditive = 50;

		[Label( "Durability calculation factor (multiplier)" )]
		[Range(0f, 1000)]
		[DefaultValue( 0.5f )]
		public float DurabilityMultiplier = 0.5f;

		[Label( "Durability calculation factor (exponent)" )]
		[Range( -10f, 10f )]
		[DefaultValue( 1.56f )]
		public float DurabilityExponent = 1.56f;


		[Label( "Non-tool-or-armor durability calculation factor (multiplier)" )]
		[Range( 0f, 1000f )]
		[DefaultValue( 1f )]
		public float NonToolOrArmorDurabilityMultiplier = 1f;

		[Label( "Armor durability calculation factor (multiplier)" )]
		[Range( 0f, 1000f )]
		[DefaultValue( 2f )]
		public float ArmorDurabilityMultiplier = 2f;


		[Label( "Tool durability calculation factor (multiplier)" )]
		[Range( 0f, 1000f )]
		[DefaultValue( 2f )]
		public float ToolDurabilityMultiplier = 2f;

		[Label( "Custom item durability calculation factors (multipliers)" )]
		public Dictionary<ItemDefinition, float> CustomDurabilityMultipliers;


		[Header( "Durability settings" )]
		[Label( "Can items be repaired" )]
		[DefaultValue( true )]
		public bool CanRepair = true;

		[Label( "Can broken items be repaired" )]
		[DefaultValue( true )]
		public bool CanRepairBroken = true;

		[Label( "Amount per repair" )]
		[Range( 0, 10000 )]
		[DefaultValue( 250 )]
		public int RepairAmount = 250;

		[Label( "Max durability lost per repair" )]
		[Range( 0f, 1000f )]
		[DefaultValue( 25f )]
		public float MaxDurabilityLostPerRepair = 25f;


		[Label( "Durability percent before critical warning" )]
		[Range( 0f, 1f )]
		[DefaultValue( 0.2f )]
		public float CriticalWarningPercent = 0.2f;


		[Label( "Show durability quantity" )]
		[DefaultValue( true )]
		public bool ShowNumbers = true;

		[Label( "Show durability gauge bar" )]
		[DefaultValue( true )]
		public bool ShowBar = true;


		[Label( "General item wear and tear calculation factor (multiplier)" )]
		[Tooltip("'Wear and tear' is to durability loss what armor is to hp.")]
		[Range( 0f, 1000f )]
		[DefaultValue( 1f )]
		public float GeneralWearAndTearMultiplier = 1f;

		[Label( "Armor item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 1000f )]
		[DefaultValue( 1f )]
		public float ArmorWearAndTearMultiplier = 1f;

		[Label( "Tool item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 1000f )]
		[DefaultValue( 0.5f )]
		public float ToolWearAndTearMultiplier = 0.5f;

		[Label( "Weapon item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 1000f )]
		[DefaultValue( 1f )]
		public float WeaponWearAndTearMultiplier = 1f;

		[Label( "Fishing item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 1000f )]
		[DefaultValue( 10f )]
		public float FishingWearAndTearMultiplier = 10f;

		[Label( "Grapple item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 1000f )]
		[DefaultValue( 1f )]
		public float GrappleWearAndTearMultiplier = 1f;

		[Label( "Summon item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 1000f )]
		[DefaultValue( 40f )]
		public float SummonWearAndTearMultiplier = 40f;

		[Label( "Wire item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 1000f )]
		[DefaultValue( 1f )]
		public float WireWearAndTearMultiplier = 1f;

		[Label( "Melee projectile item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, 1000f )]
		[DefaultValue( 1f )]
		public float MeleeProjectileWearAndTearMultiplier = 1f;



		////////////////

		public DurabilityConfig() {
			this.CustomDurabilityMultipliers = new Dictionary<ItemDefinition, float> {
				// Ore items get durability boost
				{ new ItemDefinition(ItemID.IronHelmet), 1.15f },
				{ new ItemDefinition(ItemID.IronChainmail), 1.15f },
				{ new ItemDefinition(ItemID.IronGreaves), 1.15f },
				{ new ItemDefinition(ItemID.IronAxe), 1.15f },
				{ new ItemDefinition(ItemID.IronHammer), 1.15f },
				{ new ItemDefinition(ItemID.IronPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.IronShortsword), 1.15f },
				{ new ItemDefinition(ItemID.IronBroadsword), 1.15f },
				//{ "Iron Bow", 1.15f },
				{ new ItemDefinition(ItemID.LeadHelmet), 1.15f },
				{ new ItemDefinition(ItemID.LeadChainmail), 1.15f },
				{ new ItemDefinition(ItemID.LeadGreaves), 1.15f },
				{ new ItemDefinition(ItemID.LeadAxe), 1.15f },
				{ new ItemDefinition(ItemID.LeadHammer), 1.15f },
				{ new ItemDefinition(ItemID.LeadPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.LeadShortsword), 1.15f },
				{ new ItemDefinition(ItemID.LeadBroadsword), 1.15f },
				//{ new ItemDefinition(ItemID.LeadBow), 1.15f },
				{ new ItemDefinition(ItemID.SilverHelmet), 1.15f },
				{ new ItemDefinition(ItemID.SilverChainmail), 1.15f },
				{ new ItemDefinition(ItemID.SilverGreaves), 1.15f },
				{ new ItemDefinition(ItemID.SilverAxe), 1.15f },
				{ new ItemDefinition(ItemID.SilverHammer), 1.15f },
				{ new ItemDefinition(ItemID.SilverPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.SilverShortsword), 1.15f },
				{ new ItemDefinition(ItemID.SilverBroadsword), 1.15f },
				//{ new ItemDefinition(ItemID.SilverBow), 1.15f },
				{ new ItemDefinition(ItemID.TungstenHelmet), 1.15f },
				{ new ItemDefinition(ItemID.TungstenChainmail), 1.15f },
				{ new ItemDefinition(ItemID.TungstenGreaves), 1.15f },
				{ new ItemDefinition(ItemID.TungstenAxe), 1.15f },
				{ new ItemDefinition(ItemID.TungstenHammer), 1.15f },
				{ new ItemDefinition(ItemID.TungstenPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.TungstenShortsword), 1.15f },
				{ new ItemDefinition(ItemID.TungstenBroadsword), 1.15f },
				//{ new ItemDefinition(ItemID.TungstenBow), 1.15f },
				{ new ItemDefinition(ItemID.GoldHelmet), 1.15f },
				{ new ItemDefinition(ItemID.GoldChainmail), 1.15f },
				{ new ItemDefinition(ItemID.GoldGreaves), 1.15f },
				{ new ItemDefinition(ItemID.GoldAxe), 1.15f },
				{ new ItemDefinition(ItemID.GoldHammer), 1.15f },
				{ new ItemDefinition(ItemID.GoldPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.GoldShortsword), 1.15f },
				{ new ItemDefinition(ItemID.GoldBroadsword), 1.15f },
				//{ new ItemDefinition(ItemID.GoldBow), 1.15f },
				{ new ItemDefinition(ItemID.PlatinumHelmet), 1.15f },
				{ new ItemDefinition(ItemID.PlatinumChainmail), 1.15f },
				{ new ItemDefinition(ItemID.PlatinumGreaves), 1.15f },
				{ new ItemDefinition(ItemID.PlatinumAxe), 1.15f },
				{ new ItemDefinition(ItemID.PlatinumHammer), 1.15f },
				{ new ItemDefinition(ItemID.PlatinumPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.PlatinumShortsword), 1.15f },
				{ new ItemDefinition(ItemID.PlatinumBroadsword), 1.15f },
				//{ new ItemDefinition(ItemID.PlatinumBow), 1.15f },
				{ new ItemDefinition(ItemID.CobaltHelmet), 1.15f },
				{ new ItemDefinition(ItemID.CobaltMask), 1.15f },
				{ new ItemDefinition(ItemID.CobaltHat), 1.15f },
				{ new ItemDefinition(ItemID.CobaltBreastplate), 1.15f },
				{ new ItemDefinition(ItemID.CobaltLeggings), 1.15f },
				{ new ItemDefinition(ItemID.CobaltChainsaw), 1.15f },
				{ new ItemDefinition(ItemID.CobaltWaraxe), 1.15f },
				{ new ItemDefinition(ItemID.CobaltPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.CobaltDrill), 1.15f },
				{ new ItemDefinition(ItemID.CobaltSword), 1.15f },
				{ new ItemDefinition(ItemID.CobaltNaginata), 1.15f },
				//{ new ItemDefinition(ItemID.CobaltRepeater), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumHelmet), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumMask), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumHeadgear), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumBreastplate), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumLeggings), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumChainsaw), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumWaraxe), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumDrill), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumSword), 1.15f },
				{ new ItemDefinition(ItemID.PalladiumPike), 1.15f },
				//{ new ItemDefinition(ItemID.PalladiumRepeater), 1.15f },
				{ new ItemDefinition(ItemID.MythrilHelmet), 1.15f },
				{ new ItemDefinition(ItemID.MythrilHat), 1.15f },
				{ new ItemDefinition(ItemID.MythrilHood), 1.15f },
				{ new ItemDefinition(ItemID.MythrilChainmail), 1.15f },
				{ new ItemDefinition(ItemID.MythrilGreaves), 1.15f },
				{ new ItemDefinition(ItemID.MythrilChainsaw), 1.15f },
				{ new ItemDefinition(ItemID.MythrilWaraxe), 1.15f },
				{ new ItemDefinition(ItemID.MythrilPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.MythrilDrill), 1.15f },
				{ new ItemDefinition(ItemID.MythrilSword), 1.15f },
				{ new ItemDefinition(ItemID.MythrilHalberd), 1.15f },
				//{ new ItemDefinition(ItemID.MythrilRepeater), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumHelmet), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumMask), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumHeadgear), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumBreastplate), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumLeggings), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumChainsaw), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumWaraxe), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumDrill), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumSword), 1.15f },
				{ new ItemDefinition(ItemID.OrichalcumHalberd), 1.15f },
				//{ new ItemDefinition(ItemID.OrichalcumRepeater), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteHelmet), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteMask), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteHeadgear), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteBreastplate), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteLeggings), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteChainsaw), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteWaraxe), 1.15f },
				{ new ItemDefinition(ItemID.AdamantitePickaxe), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteDrill), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteSword), 1.15f },
				{ new ItemDefinition(ItemID.AdamantiteGlaive), 1.15f },
				//{ new ItemDefinition(ItemID.AdamantiteRepeater), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumHelmet), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumMask), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumHeadgear), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumBreastplate), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumLeggings), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumChainsaw), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumWaraxe), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumPickaxe), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumDrill), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumSword), 1.15f },
				{ new ItemDefinition(ItemID.TitaniumTrident), 1.15f },
				//{ new ItemDefinition(ItemID.TitaniumRepeater), 1.15f },
				{ new ItemDefinition(ItemID.FrostHelmet), 1.15f },
				{ new ItemDefinition(ItemID.FrostBreastplate), 1.15f },
				{ new ItemDefinition(ItemID.AncientBattleArmorPants), 1.15f },
				{ new ItemDefinition(ItemID.AncientBattleArmorHat), 1.15f },
				{ new ItemDefinition(ItemID.AncientBattleArmorShirt), 1.15f },
				//{ new ItemDefinition(ItemID.TitaniumRepeater), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteHelmet), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteMask), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteHeadgear), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophytePlateMail), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteGreaves), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophytePickaxe), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteDrill), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteChainsaw), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteGreataxe), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteJackhammer), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteWarhammer), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteSaber), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophyteClaymore), 1.15f },
				{ new ItemDefinition(ItemID.ChlorophytePartisan), 1.15f },
				{ new ItemDefinition(ItemID.TurtleHelmet), 1.15f },
				{ new ItemDefinition(ItemID.TurtleScaleMail), 1.15f },
				{ new ItemDefinition(ItemID.TurtleLeggings), 1.15f },
				{ new ItemDefinition(ItemID.VenomStaff), 1.15f },
				//{ new ItemDefinition(ItemID.TitaniumRepeater), 1.15f },
				{ new ItemDefinition(ItemID.SpectreMask), 1.15f },
				{ new ItemDefinition(ItemID.SpectreHood), 1.15f },
				{ new ItemDefinition(ItemID.SpectreRobe), 1.15f },
				{ new ItemDefinition(ItemID.SpectrePants), 1.15f },
				{ new ItemDefinition(ItemID.SpectrePickaxe), 1.15f },
				{ new ItemDefinition(ItemID.SpectreHamaxe), 1.15f },
				{ new ItemDefinition(ItemID.ShroomiteMask), 1.15f },
				{ new ItemDefinition(ItemID.ShroomiteHeadgear), 1.15f },
				{ new ItemDefinition(ItemID.ShroomiteHelmet), 1.15f },
				{ new ItemDefinition(ItemID.ShroomiteBreastplate), 1.15f },
				{ new ItemDefinition(ItemID.ShroomiteLeggings), 1.15f },
				{ new ItemDefinition(ItemID.ShroomiteDiggingClaw), 1.15f },
			
				// Cactus stuffs very easily acquired
				{ new ItemDefinition(ItemID.CactusSword), 0.85f },
				{ new ItemDefinition(ItemID.CactusPickaxe), 0.85f },
				{ new ItemDefinition(ItemID.CactusHelmet), 0.85f },
				{ new ItemDefinition(ItemID.CactusBreastplate), 0.85f },
				{ new ItemDefinition(ItemID.CactusLeggings), 0.85f },

				// Meteor stuffs easily (albeit unpredictably) renewable
				{ new ItemDefinition(ItemID.MeteorHelmet), 0.85f },
				{ new ItemDefinition(ItemID.MeteorSuit), 0.85f },
				{ new ItemDefinition(ItemID.MeteorLeggings), 0.85f },
				{ new ItemDefinition(ItemID.SpaceGun), 0.85f },
				{ new ItemDefinition(ItemID.MeteorHamaxe), 0.85f },

				// Demonite stuffs farmable
				{ new ItemDefinition(ItemID.DemonBow), 0.85f },
				{ new ItemDefinition(ItemID.WarAxeoftheNight), 0.85f },
				{ new ItemDefinition(ItemID.LightsBane), 0.85f },
				{ new ItemDefinition(ItemID.ShadowHelmet), 0.85f },
				{ new ItemDefinition(ItemID.ShadowScalemail), 0.85f },
				{ new ItemDefinition(ItemID.ShadowGreaves), 0.85f },
				{ new ItemDefinition(ItemID.TheBreaker), 0.85f },
				{ new ItemDefinition(ItemID.CorruptYoyo), 0.85f },
				{ new ItemDefinition(ItemID.NightmarePickaxe), 1f }, //0.85f },
			
				// Crimtain stuffs farmable
				{ new ItemDefinition(ItemID.TendonBow), 0.85f },
				{ new ItemDefinition(ItemID.BloodLustCluster), 0.85f },
				{ new ItemDefinition(ItemID.BloodButcherer), 0.85f },
				{ new ItemDefinition(ItemID.CrimsonHelmet), 0.85f },
				{ new ItemDefinition(ItemID.CrimsonScalemail), 0.85f },
				{ new ItemDefinition(ItemID.CrimsonGreaves), 0.85f },
				{ new ItemDefinition(ItemID.FleshGrinder), 0.85f },
				{ new ItemDefinition(ItemID.TheMeatball), 0.85f },
				{ new ItemDefinition(ItemID.DeathbringerPickaxe), 1f },	//0.85f },
			
				// Hallowed stuffs (semi) farmable
				{ new ItemDefinition(ItemID.HallowedMask), 0.9f },
				{ new ItemDefinition(ItemID.HallowedHelmet), 0.9f },
				{ new ItemDefinition(ItemID.HallowedHeadgear), 0.9f },
				{ new ItemDefinition(ItemID.HallowedPlateMail), 0.9f },
				{ new ItemDefinition(ItemID.HallowedGreaves), 0.9f },
				{ new ItemDefinition(ItemID.Drax), 1f },	//0.9f },
				{ new ItemDefinition(ItemID.PickaxeAxe), 1f },	//0.9f },
				{ new ItemDefinition(ItemID.HallowedRepeater), 0.9f },
				{ new ItemDefinition(ItemID.Excalibur), 0.9f },	//0.9f },
				{ new ItemDefinition(ItemID.Gungnir), 0.9f },

				// 2 shots per fire
				{ new ItemDefinition(ItemID.VortexBeater), 0.5f }
			};
		}

		////

		public override ModConfig Clone() {
			var clone = (DurabilityConfig)base.Clone();

			clone.CustomDurabilityMultipliers = this.CustomDurabilityMultipliers
				.ToDictionary( kv => kv.Key, kv => kv.Value );

			return clone;
		}
	}
}
