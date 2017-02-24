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
	public class ConfigurationData {
		public string VersionSinceUpdate = "";
		public float WearMultiplier = 1f;
		public int DurabilityAdditive = 50;
		public float DurabilityMultiplier = 1f;
		public float DurabilityExponent = 1.54f;
		public bool CanRepair = true;
		public int RepairAmount = 250;
		//public float ToolDurabilityMultiplier = 1.5f;
		public float ToolUseMultiplier = 0.5f;
		public float ArmorDurabilityMultiplier = 2f;
		public float ToolDurabilityMultiplier = 2f;
		public int MaxMeteors = 2;
		public IDictionary<string, float> CustomDurabilityMultipliers = new Dictionary<string, float> {
			{ "Demon Bow", 0.6f },
			{ "War Axe of the Night", 0.6f },
			{ "Light's Bane", 0.6f },
			{ "Shadow Helmet", 0.6f },
			{ "Shadow Scalemail", 0.6f },
			{ "Shadow Greaves", 0.6f },
			//{ "Nightmare Pickaxe", 0.0.6f },
			{ "The Breaker", 0.6f },
			{ "Malaise", 0.6f },

			{ "Tendon Bow", 0.6f },
			{ "Blood Lust Cluster", 0.6f },
			{ "Blood Butcherer", 0.6f },
			{ "Crimson Helmet", 0.6f },
			{ "Crimson Scalemail", 0.6f },
			{ "Crimson Greaves", 0.6f },
			//{ "Deathbringer Pickaxe", 00.6f },
			{ "Flesh Grinder", 0.6f },
			{ "The Meatball", 0.6f },

			{ "Hallowed Mask", 0.6f },
			{ "Hallowed Helmet", 0.6f },
			{ "Hallowed Headgear", 0.6f },
			{ "Hallowed Plate Mail", 0.6f },
			{ "Hallowed Greaves", 0.6f },
			{ "Drax", 0.6f },
			{ "Pickaxe Axe", 0.6f },
			{ "Hallowed Repeater", 0.6f },
			{ "Excalibur", 0.6f },
			{ "Gungnir", 0.6f }
		};
		public bool ShowNumbers = true;
		public bool ShowBar = true;
		public float RepairDegradationMultiplier = 25f;
	}



	public class DurabilityMod : Mod {
		public static Version ConfigVersion = new Version(1, 9, 0);
		public static JsonConfig<ConfigurationData> Config { get; private set; }


		public DurabilityMod() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};

			string filename = "Durability Config.json";
			DurabilityMod.Config = new JsonConfig<ConfigurationData>( filename, new ConfigurationData() );
		}

		public override void Load() {
			AchievementsHelper.OnTileDestroyed += this.MyOnTileDestroyedEvent;

			if( !DurabilityMod.Config.Load() ) {
				DurabilityMod.Config.Save();
			} else {
				Version vers_since = DurabilityMod.Config.Data.VersionSinceUpdate != "" ?
					new Version( DurabilityMod.Config.Data.VersionSinceUpdate ) :
					new Version();

				if( vers_since < DurabilityMod.ConfigVersion ) {
					ErrorLogger.Log( "Durability config updated to " + DurabilityMod.ConfigVersion.ToString() );

					DurabilityMod.Config.Data.VersionSinceUpdate = DurabilityMod.ConfigVersion.ToString();
					DurabilityMod.Config.Save();
				}
			}
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
				Item item = player.inventory[player.selectedItem];

				if( item.pick > 0 || item.axe > 0 || item.hammer > 0 ) {
//Main.NewText("Mod.OnTileDestroyed " + item.name);
					DurabilityItemInfo info = item.GetModInfo<DurabilityItemInfo>(this);
					info.Use( item, 1, DurabilityMod.Config.Data.ToolUseMultiplier );

					//int tileX = (int)(((float)Main.mouseX + Main.screenPosition.X) / 16f);
					//int tileY = (int)(((float)Main.mouseY + Main.screenPosition.Y) / 16f);
					//
					//float a = player.position.X / 16f - (float)tileX - (float)item.tileBoost;
					//float b = player.position.X + (float)player.width / 16f + (float)tileX + (float)item.tileBoost - 1f;
					//float c = player.position.Y / 16f - (float)tileY - (float)item.tileBoost;
					//float d = (player.position.Y + (float)player.height) / 16f + (float)tileY + (float)item.tileBoost - 2f;
					//bool withinRange = a <= (float)tileX && b >= (float)tileX && c <= (float)tileY && d >= (float)tileY;
					//
					//Tile tile = Main.tile[tileX, tileY];
					//if( withinRange && Main.tile[tileX, tileY].active() ) {
					//}
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
