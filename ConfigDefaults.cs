using HamstarHelpers.Helpers.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
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


		[Label( "Durability calculation factor (additive)" )]
		[Range(0, Int32.MaxValue)]
		[DefaultValue( 50 )]
		public int DurabilityAdditive = 50;

		[Label( "Durability calculation factor (multiplier)" )]
		[Range(0f, Single.MaxValue)]
		[DefaultValue( 0.5f )]
		public float DurabilityMultiplier = 0.5f;

		[Label( "Durability calculation factor (exponent)" )]
		[Range( Single.MinValue, Single.MaxValue )]
		[DefaultValue( 1.56f )]
		public float DurabilityExponent = 1.56f;


		[Label( "Non-tool-or-armor durability calculation factor (multiplier)" )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 1f )]
		public float NonToolOrArmorDurabilityMultiplier = 1f;

		[Label( "Armor durability calculation factor (multiplier)" )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 2f )]
		public float ArmorDurabilityMultiplier = 2f;


		[Label( "Tool durability calculation factor (multiplier)" )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 2f )]
		public float ToolDurabilityMultiplier = 2f;

		[Label( "Custom item durability calculation factors (multipliers)" )]
		public Dictionary<string, float> CustomDurabilityMultipliers;


		[Label( "Can items be repaired" )]
		[DefaultValue( true )]
		public bool CanRepair = true;

		[Label( "Can broken items be repaired" )]
		[DefaultValue( true )]
		public bool CanRepairBroken = true;

		[Label( "Amount per repair" )]
		[Range( 0, Int32.MaxValue )]
		[DefaultValue( 250 )]
		public int RepairAmount = 250;

		[Label( "Max durability lost per repair" )]
		[Range( 0f, Single.MaxValue )]
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
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 1f )]
		public float GeneralWearAndTearMultiplier = 1f;

		[Label( "Armor item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 1f )]
		public float ArmorWearAndTearMultiplier = 1f;

		[Label( "Tool item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 0.5f )]
		public float ToolWearAndTearMultiplier = 0.5f;

		[Label( "Weapon item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 1f )]
		public float WeaponWearAndTearMultiplier = 1f;

		[Label( "Fishing item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 10f )]
		public float FishingWearAndTearMultiplier = 10f;

		[Label( "Grapple item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 1f )]
		public float GrappleWearAndTearMultiplier = 1f;

		[Label( "Summon item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 40f )]
		public float SummonWearAndTearMultiplier = 40f;

		[Label( "Wire item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 1f )]
		public float WireWearAndTearMultiplier = 1f;

		[Label( "Melee projectile item wear and tear calculation factor (multiplier)" )]
		[Tooltip( "'Wear and tear' is to durability loss what armor is to hp." )]
		[Range( 0f, Single.MaxValue )]
		[DefaultValue( 1f )]
		public float MeleeProjectileWearAndTearMultiplier = 1f;



		////////////////

		[OnDeserialized]
		internal void OnDeserializedMethod( StreamingContext context ) {
			if( this.CustomDurabilityMultipliers != null ) {
				return;
			}

			this.CustomDurabilityMultipliers = new Dictionary<string, float> {
				// Ore items get durability boost
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.IronHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.IronChainmail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.IronGreaves), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.IronAxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.IronHammer), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.IronPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.IronShortsword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.IronBroadsword), 1.15f },
				//{ "Iron Bow", 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadChainmail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadGreaves), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadAxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadHammer), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadShortsword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadBroadsword), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.LeadBow), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverChainmail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverGreaves), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverAxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverHammer), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverShortsword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverBroadsword), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.SilverBow), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenChainmail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenGreaves), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenAxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenHammer), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenShortsword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenBroadsword), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.TungstenBow), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldChainmail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldGreaves), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldAxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldHammer), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldShortsword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldBroadsword), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.GoldBow), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumChainmail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumGreaves), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumAxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumHammer), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumShortsword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumBroadsword), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.PlatinumBow), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltMask), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltHat), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltBreastplate), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltLeggings), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltChainsaw), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltWaraxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltDrill), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltSword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltNaginata), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.CobaltRepeater), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumMask), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumHeadgear), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumBreastplate), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumLeggings), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumChainsaw), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumWaraxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumDrill), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumSword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumPike), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.PalladiumRepeater), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilHat), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilHood), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilChainmail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilGreaves), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilChainsaw), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilWaraxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilDrill), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilSword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilHalberd), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.MythrilRepeater), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumMask), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumHeadgear), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumBreastplate), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumLeggings), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumChainsaw), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumWaraxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumDrill), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumSword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumHalberd), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.OrichalcumRepeater), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteMask), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteHeadgear), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteBreastplate), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteLeggings), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteChainsaw), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteWaraxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantitePickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteDrill), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteSword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteGlaive), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.AdamantiteRepeater), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumMask), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumHeadgear), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumBreastplate), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumLeggings), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumChainsaw), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumWaraxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumPickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumDrill), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumSword), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumTrident), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumRepeater), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.FrostHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.FrostBreastplate), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AncientBattleArmorPants), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AncientBattleArmorHat), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.AncientBattleArmorShirt), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumRepeater), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteMask), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteHeadgear), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophytePlateMail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteGreaves), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophytePickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteDrill), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteChainsaw), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteGreataxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteJackhammer), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteWarhammer), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteSaber), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophyteClaymore), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ChlorophytePartisan), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TurtleHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TurtleScaleMail), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TurtleLeggings), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.VenomStaff), 1.15f },
				//{ ItemIdentityHelpers.GetUniqueKey(ItemID.TitaniumRepeater), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SpectreMask), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SpectreHood), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SpectreRobe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SpectrePants), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SpectrePickaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SpectreHamaxe), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShroomiteMask), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShroomiteHeadgear), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShroomiteHelmet), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShroomiteBreastplate), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShroomiteLeggings), 1.15f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShroomiteDiggingClaw), 1.15f },
			
				// Cactus stuffs very easily acquired
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CactusSword), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CactusPickaxe), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CactusHelmet), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CactusBreastplate), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CactusLeggings), 0.85f },

				// Meteor stuffs easily (albeit unpredictably) renewable
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MeteorHelmet), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MeteorSuit), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MeteorLeggings), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.SpaceGun), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.MeteorHamaxe), 0.85f },

				// Demonite stuffs farmable
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.DemonBow), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.WarAxeoftheNight), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.LightsBane), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShadowHelmet), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShadowScalemail), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.ShadowGreaves), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TheBreaker), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CorruptYoyo), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.NightmarePickaxe), 1f }, //0.85f },
			
				// Crimtain stuffs farmable
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TendonBow), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.BloodLustCluster), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.BloodButcherer), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CrimsonHelmet), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CrimsonScalemail), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.CrimsonGreaves), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.FleshGrinder), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.TheMeatball), 0.85f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.DeathbringerPickaxe), 1f },	//0.85f },
			
				// Hallowed stuffs (semi) farmable
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.HallowedMask), 0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.HallowedHelmet), 0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.HallowedHeadgear), 0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.HallowedPlateMail), 0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.HallowedGreaves), 0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.Drax), 1f },	//0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.PickaxeAxe), 1f },	//0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.HallowedRepeater), 0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.Excalibur), 0.9f },	//0.9f },
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.Gungnir), 0.9f },

				// 2 shots per fire
				{ ItemIdentityHelpers.GetUniqueKey(ItemID.VortexBeater), 0.5f }
			};
		}
	}
}
