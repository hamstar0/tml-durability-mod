using System;
using System.Collections.Generic;


namespace Durability {
	// Clarification of terms:
	//	'wear and tear'		Accumulated damage to tool or armor.
	//	'durability'		Amount of wear and tear before tool or armor breaks.
	public class DurabilityConfigData {
		public readonly static string ConfigFileName = "Durability Config.json";


		////////////////

		public string VersionSinceUpdate = "";

		public bool DebugModeInfo = false;

		public bool Enabled = true;

		public int DurabilityAdditive = 50;
		public float DurabilityMultiplier = 0.5f;
		public float DurabilityExponent = 1.56f;

		public bool CanRepair = true;
		public bool CanRepairBroken = true;
		public int RepairAmount = 250;

		public float NonToolOrArmorDurabilityMultiplier = 1f;
		public float ArmorDurabilityMultiplier = 2f;
		public float ToolDurabilityMultiplier = 2f;

		public float MaxDurabilityLostPerRepair = 25f;

		public float CriticalWarningPercent = 0.2f;

		public IDictionary<string, float> CustomDurabilityMultipliers = new Dictionary<string, float>(); 

		public bool ShowNumbers = true;
		public bool ShowBar = true;

		public float GeneralWearAndTearMultiplier = 1f;
		public float ArmorWearAndTearMultiplier = 1f;
		public float ToolWearAndTearMultiplier = 0.5f;
		public float WeaponWearAndTearMultiplier = 1f;
		public float FishingWearAndTearMultiplier = 10f;
		public float GrappleWearAndTearMultiplier = 1f;
		public float SummonWearAndTearMultiplier = 40f;
		public float WireWearAndTearMultiplier = 1f;
		public float MeleeProjectileWearAndTearMultiplier = 1f;


		public string _OLD_SETTINGS_BELOW = "";
		public float NonMaxToolOrArmorDurabilityMultiplier = 1f;
		public float ArmorMaxDurabilityMultiplier = 2f;
		public float ToolMaxDurabilityMultiplier = 2f;



		////////////////

		public void SetDefaults() {
			this.CustomDurabilityMultipliers = new Dictionary<string, float> {
				// Ore items get durability boost
				{ "Iron Helmet", 1.15f },
				{ "Iron Chainmail", 1.15f },
				{ "Iron Greaves", 1.15f },
				{ "Iron Axe", 1.15f },
				{ "Iron Hammer", 1.15f },
				{ "Iron Pickaxe", 1.15f },
				{ "Iron Shortsword", 1.15f },
				{ "Iron Broadsword", 1.15f },
				//{ "Iron Bow", 1.15f },
				{ "Lead Helmet", 1.15f },
				{ "Lead Chainmail", 1.15f },
				{ "Lead Greaves", 1.15f },
				{ "Lead Axe", 1.15f },
				{ "Lead Hammer", 1.15f },
				{ "Lead Pickaxe", 1.15f },
				{ "Lead Shortsword", 1.15f },
				{ "Lead Broadsword", 1.15f },
				//{ "Lead Bow", 1.15f },
				{ "Silver Helmet", 1.15f },
				{ "Silver Chainmail", 1.15f },
				{ "Silver Greaves", 1.15f },
				{ "Silver Axe", 1.15f },
				{ "Silver Hammer", 1.15f },
				{ "Silver Pickaxe", 1.15f },
				{ "Silver Shortsword", 1.15f },
				{ "Silver Broadsword", 1.15f },
				//{ "Silver Bow", 1.15f },
				{ "Tungsten Helmet", 1.15f },
				{ "Tungsten Chainmail", 1.15f },
				{ "Tungsten Greaves", 1.15f },
				{ "Tungsten Axe", 1.15f },
				{ "Tungsten Hammer", 1.15f },
				{ "Tungsten Pickaxe", 1.15f },
				{ "Tungsten Shortsword", 1.15f },
				{ "Tungsten Broadsword", 1.15f },
				//{ "Tungsten Bow", 1.15f },
				{ "Gold Helmet", 1.15f },
				{ "Gold Chainmail", 1.15f },
				{ "Gold Greaves", 1.15f },
				{ "Gold Axe", 1.15f },
				{ "Gold Hammer", 1.15f },
				{ "Gold Pickaxe", 1.15f },
				{ "Gold Shortsword", 1.15f },
				{ "Gold Broadsword", 1.15f },
				//{ "Gold Bow", 1.15f },
				{ "Platinum Helmet", 1.15f },
				{ "Platinum Chainmail", 1.15f },
				{ "Platinum Greaves", 1.15f },
				{ "Platinum Axe", 1.15f },
				{ "Platinum Hammer", 1.15f },
				{ "Platinum Pickaxe", 1.15f },
				{ "Platinum Shortsword", 1.15f },
				{ "Platinum Broadsword", 1.15f },
				//{ "Platinum Bow", 1.15f },
				{ "Cobalt Helmet", 1.15f },
				{ "Cobalt Mask", 1.15f },
				{ "Cobalt Hat", 1.15f },
				{ "Cobalt Breastplate", 1.15f },
				{ "Cobalt Leggings", 1.15f },
				{ "Cobalt Chainsaw", 1.15f },
				{ "Cobalt Waraxe", 1.15f },
				{ "Cobalt Hammer", 1.15f },
				{ "Cobalt Pickaxe", 1.15f },
				{ "Cobalt Drill", 1.15f },
				{ "Cobalt Sword", 1.15f },
				{ "Cobalt Naginata", 1.15f },
				//{ "Cobalt Repeater", 1.15f },
				{ "Palladium Helmet", 1.15f },
				{ "Palladium Mask", 1.15f },
				{ "Palladium Headgear", 1.15f },
				{ "Palladium Breastplate", 1.15f },
				{ "Palladium Leggings", 1.15f },
				{ "Palladium Chainsaw", 1.15f },
				{ "Palladium Waraxe", 1.15f },
				{ "Palladium Hammer", 1.15f },
				{ "Palladium Pickaxe", 1.15f },
				{ "Palladium Drill", 1.15f },
				{ "Palladium Sword", 1.15f },
				{ "Palladium Pike", 1.15f },
				//{ "Palladium Repeater", 1.15f },
				{ "Mythril Helmet", 1.15f },
				{ "Mythril Hat", 1.15f },
				{ "Mythril Hood", 1.15f },
				{ "Mythril Chainmail", 1.15f },
				{ "Mythril Greaves", 1.15f },
				{ "Mythril Chainsaw", 1.15f },
				{ "Mythril Waraxe", 1.15f },
				{ "Mythril Hammer", 1.15f },
				{ "Mythril Pickaxe", 1.15f },
				{ "Mythril Drill", 1.15f },
				{ "Mythril Sword", 1.15f },
				{ "Mythril Halberd", 1.15f },
				//{ "Mythril Repeater", 1.15f },
				{ "Orichalcum Helmet", 1.15f },
				{ "Orichalcum Mask", 1.15f },
				{ "Orichalcum Headgear", 1.15f },
				{ "Orichalcum Breastplate", 1.15f },
				{ "Orichalcum Leggings", 1.15f },
				{ "Orichalcum Chainsaw", 1.15f },
				{ "Orichalcum Waraxe", 1.15f },
				{ "Orichalcum Hammer", 1.15f },
				{ "Orichalcum Pickaxe", 1.15f },
				{ "Orichalcum Drill", 1.15f },
				{ "Orichalcum Sword", 1.15f },
				{ "Orichalcum Halberd", 1.15f },
				//{ "Orichalcum Repeater", 1.15f },
				{ "Adamantite Helmet", 1.15f },
				{ "Adamantite Mask", 1.15f },
				{ "Adamantite Headgear", 1.15f },
				{ "Adamantite Breastplate", 1.15f },
				{ "Adamantite Leggings", 1.15f },
				{ "Adamantite Chainsaw", 1.15f },
				{ "Adamantite Waraxe", 1.15f },
				{ "Adamantite Hammer", 1.15f },
				{ "Adamantite Pickaxe", 1.15f },
				{ "Adamantite Drill", 1.15f },
				{ "Adamantite Sword", 1.15f },
				{ "Adamantite Glaive", 1.15f },
				//{ "Adamantite Repeater", 1.15f },
				{ "Titanium Helmet", 1.15f },
				{ "Titanium Mask", 1.15f },
				{ "Titanium Headgear", 1.15f },
				{ "Titanium Breastplate", 1.15f },
				{ "Titanium Leggings", 1.15f },
				{ "Titanium Chainsaw", 1.15f },
				{ "Titanium Waraxe", 1.15f },
				{ "Titanium Hammer", 1.15f },
				{ "Titanium Pickaxe", 1.15f },
				{ "Titanium Drill", 1.15f },
				{ "Titanium Sword", 1.15f },
				{ "Titanium Trident", 1.15f },
				//{ "Titanium Repeater", 1.15f },
				{ "Frost Helmet", 1.15f },
				{ "Frost Breastplate", 1.15f },
				{ "Frost Leggings", 1.15f },
				{ "Forbidden Mask", 1.15f },
				{ "Forbidden Robes", 1.15f },
				{ "Forbidden Treads", 1.15f },
				//{ "Titanium Repeater", 1.15f },
				{ "Chlorophyte Helmet", 1.15f },
				{ "Chlorophyte Mask", 1.15f },
				{ "Chlorophyte Headgear", 1.15f },
				{ "Chlorophyte Plate Mail", 1.15f },
				{ "Chlorophyte Greaves", 1.15f },
				{ "Chlorophyte Pickaxe", 1.15f },
				{ "Chlorophyte Drill", 1.15f },
				{ "Chlorophyte Chainsaw", 1.15f },
				{ "Chlorophyte Greataxe", 1.15f },
				{ "Chlorophyte Jackhammer", 1.15f },
				{ "Chlorophyte Warhammer", 1.15f },
				{ "Chlorophyte Saber", 1.15f },
				{ "Chlorophyte Claymore", 1.15f },
				{ "Chlorophyte Partisan", 1.15f },
				{ "Turtle Helmet", 1.15f },
				{ "Turtle Scale Mail", 1.15f },
				{ "Turtle Leggings", 1.15f },
				{ "Venom Staff", 1.15f },
				//{ "Titanium Repeater", 1.15f },
				{ "Spectre Mask", 1.15f },
				{ "Spectre Hood", 1.15f },
				{ "Spectre Robe", 1.15f },
				{ "Spectre Pants", 1.15f },
				{ "Spectre Pickaxe", 1.15f },
				{ "Spectre Hamaxe", 1.15f },
				{ "Shroomite Mask", 1.15f },
				{ "Shroomite Headgear", 1.15f },
				{ "Shroomite Helmet", 1.15f },
				{ "Shroomite Breastplate ", 1.15f },
				{ "Shroomite Leggings", 1.15f },
				{ "Shroomite Digging Claw", 1.15f },
				{ "Shroomite Hamaxe", 1.15f },
			
				// Cactus stuffs very easily acquired
				{ "Cactus Sword", 0.85f },
				{ "Cactus Pickaxe", 0.85f },
				{ "Cactus Helmet", 0.85f },
				{ "Cactus Breastplate", 0.85f },
				{ "Cactus Leggings", 0.85f },

				// Meteor stuffs easily (albeit unpredictably) renewable
				{ "Meteor Helmet", 0.85f },
				{ "Meteor Suit", 0.85f },
				{ "Meteor Leggings", 0.85f },
				{ "Space Gun", 0.85f },
				{ "Meteor Hamaxe", 0.85f },

				// Demonite stuffs farmable
				{ "Demon Bow", 0.85f },
				{ "War Axe of the Night", 0.85f },
				{ "Light's Bane", 0.85f },
				{ "Shadow Helmet", 0.85f },
				{ "Shadow Scalemail", 0.85f },
				{ "Shadow Greaves", 0.85f },
				{ "The Breaker", 0.85f },
				{ "Malaise", 0.85f },
				{ "Nightmare Pickaxe", 1f }, //0.85f },
			
				// Crimtain stuffs farmable
				{ "Tendon Bow", 0.85f },
				{ "Blood Lust Cluster", 0.85f },
				{ "Blood Butcherer", 0.85f },
				{ "Crimson Helmet", 0.85f },
				{ "Crimson Scalemail", 0.85f },
				{ "Crimson Greaves", 0.85f },
				{ "Flesh Grinder", 0.85f },
				{ "The Meatball", 0.85f },
				{ "Deathbringer Pickaxe", 1f },	//0.85f },
			
				// Hallowed stuffs (semi) farmable
				{ "Hallowed Mask", 0.9f },
				{ "Hallowed Helmet", 0.9f },
				{ "Hallowed Headgear", 0.9f },
				{ "Hallowed Plate Mail", 0.9f },
				{ "Hallowed Greaves", 0.9f },
				{ "Drax", 1f },	//0.9f },
				{ "Pickaxe Axe", 1f },	//0.9f },
				{ "Hallowed Repeater", 0.9f },
				{ "Excalibur", 0.9f },	//0.9f },
				{ "Gungnir", 0.9f },

				// 2 shots per fire
				{ "Vortex Beater", 0.5f }
			};
		}


		public bool UpdateToLatestVersion() {
			var newConfig = new DurabilityConfigData();
			var versSince = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();

			if( versSince >= DurabilityMod.Instance.Version ) {
				return false;
			}

			this.SetDefaults();

			this.VersionSinceUpdate = DurabilityMod.Instance.Version.ToString();

			return true;
		}
	}
}
