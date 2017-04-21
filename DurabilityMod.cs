using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Terraria;
using Utils;
using Utils.JsonConfig;
using System;
using System.IO;
using System.Collections.Generic;


namespace Durability {
	// Clarification of terms:
	//	'wear and tear'		Accumulated damage to tool or armor.
	//	'durability'		Damage before tool or armor breaks.
	public class ConfigurationData {
		public string VersionSinceUpdate = "";
		
		public int DurabilityAdditive = 50;
		public float DurabilityMultiplier = 1f;
		public float DurabilityExponent = 1.54f;

		public bool CanRepair = true;
		public int RepairAmount = 250;

		public float ArmorDurabilityMultiplier = 2f;
		public float ToolDurabilityMultiplier = 2f;

		public float MaxDurabilityLostPerRepair = 25f;

		public IDictionary<string, float> CustomDurabilityMultipliers = new Dictionary<string, float> {
			// Cactus geat very easily acquired
			{ "Cactus Sword", 0.7f },
			{ "Cactus Pickaxe", 0.6f },
			//{ "Cactus Helmet", 0.7f },
			//{ "Cactus Breastplate", 0.7f },
			//{ "Cactus Leggings", 0.7f },

			// Meteor stuffs easily (but not readily) renewable
			{ "Meteor Helmet", 0.7f },
			{ "Meteor Suit", 0.7f },
			{ "Meteor Leggings", 0.7f },
			{ "Space Gun", 0.7f },
			{ "Meteor Hamaxe", 0.7f },

			// Demonite stuffs readily renewable
			{ "Demon Bow", 0.7f },
			{ "War Axe of the Night", 0.7f },
			{ "Light's Bane", 0.7f },
			{ "Shadow Helmet", 0.7f },
			{ "Shadow Scalemail", 0.7f },
			{ "Shadow Greaves", 0.7f },
			{ "The Breaker", 0.7f },
			{ "Malaise", 0.7f },
			//{ "Nightmare Pickaxe", 0.7f },
			
			// Crimtain stuffs readily renewable
			{ "Tendon Bow", 0.7f },
			{ "Blood Lust Cluster", 0.7f },
			{ "Blood Butcherer", 0.7f },
			{ "Crimson Helmet", 0.7f },
			{ "Crimson Scalemail", 0.7f },
			{ "Crimson Greaves", 0.7f },
			{ "Flesh Grinder", 0.7f },
			{ "The Meatball", 0.7f },
			//{ "Deathbringer Pickaxe", 0.7f },
			
			// Hallowed stuffs readily renewable
			{ "Hallowed Mask", 0.7f },
			{ "Hallowed Helmet", 0.7f },
			{ "Hallowed Headgear", 0.7f },
			{ "Hallowed Plate Mail", 0.7f },
			{ "Hallowed Greaves", 0.7f },
			{ "Drax", 0.7f },
			{ "Pickaxe Axe", 0.7f },
			{ "Hallowed Repeater", 0.7f },
			{ "Excalibur", 0.7f },
			{ "Gungnir", 0.7f },

			// 2 shots per fire
			{ "Vortex Beater", 0.5f }
		};
		public bool ShowNumbers = true;
		public bool ShowBar = true;

		public float GeneralWearAndTearMultiplier = 1f;
		public float ArmorWearAndTearMultiplier = 1f;
		public float ToolWearAndTearMultiplier = 0.5f;
		public float WeaponWearAndTearMultiplier = 1f;
		public float FishingWearAndTearMultiplier = 1f;
		public float GrappleWearAndTearMultiplier = 1f;
		public float SummonWearAndTearMultiplier = 10f;
		public float WireWearAndTearMultiplier = 1f;
		public float MeleeProjectileWearAndTearMultiplier = 1f;
	}



	public class DurabilityMod : Mod {
		public readonly static Version ConfigVersion = new Version( 2, 1, 0 );
		public JsonConfig<ConfigurationData> Config { get; private set; }


		public DurabilityMod() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			string filename = "Durability Config.json";
			this.Config = new JsonConfig<ConfigurationData>( filename, "Mod Configs", new ConfigurationData() );
		}


		private void LoadConfig() {
			var old_config = new JsonConfig<ConfigurationData>( this.Config.FileName, "", new ConfigurationData() );

			// Update old config to new location
			if( old_config.LoadFile() ) {
				old_config.DestroyFile();
				old_config.SetFilePath( this.Config.FileName, "Mod Configs" );
				this.Config = old_config;
			} else if( !this.Config.LoadFile() ) {
				this.Config.SaveFile();
			}

			Version vers_since = this.Config.Data.VersionSinceUpdate != "" ?
				new Version( this.Config.Data.VersionSinceUpdate ) :
				new Version();

			if( vers_since < DurabilityMod.ConfigVersion ) {
				ErrorLogger.Log( "Durability updated to " + DurabilityMod.ConfigVersion.ToString() );

				if( vers_since < new Version( 2, 1, 0 ) ) {
					var custom_muls = new ConfigurationData().CustomDurabilityMultipliers;
					foreach( var kv in custom_muls ) {
						this.Config.Data.CustomDurabilityMultipliers[kv.Key] = kv.Value;
					}
				}

				this.Config.Data.VersionSinceUpdate = DurabilityMod.ConfigVersion.ToString();
				this.Config.SaveFile();
			}
		}


		public override void Load() {
			this.LoadConfig();
			AchievementsHelper.OnTileDestroyed += this.MyOnTileDestroyedEvent;
		}

		public override void Unload() {
			AchievementsHelper.OnTileDestroyed -= this.MyOnTileDestroyedEvent;
		}



		////////////////

		public override void HandlePacket( BinaryReader reader, int whoAmI ) {
			DurabilityNetProtocol.RoutePacket( this, reader );
		}


		////////////////

		public void MyOnTileDestroyedEvent( Player player, ushort itemId ) {
			if( player.itemAnimation > 0 && player.toolTime == 0 && player.controlUseItem ) {
				Item item = ItemHelper.GetSelectedItem( player );

				if( item.pick > 0 || item.axe > 0 || item.hammer > 0 ) {
					DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(this);
					info.AddWearAndTear( this, item, 1, this.Config.Data.ToolWearAndTearMultiplier );
				}
			}
		}

		////////////////

		public override void PostDrawInterface( SpriteBatch sb ) {
			Debug.PrintToBatch( sb );
			Debug.Once = false;
			Debug.OnceInAWhile--;
		}
	}
}
