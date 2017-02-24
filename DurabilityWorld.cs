using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Durability {
	class DurabilityWorld : ModWorld {
		private bool NowTrackingMeteor = false;
		private int MeteorCount = 0;

		private bool IsLoaded = false;


		////////////////

		public override void Load( TagCompound tags ) {
			this.NowTrackingMeteor = tags.GetBool( "now_tracking_meteor" );
			this.MeteorCount = tags.GetInt( "meteor_count" );

			this.IsLoaded = true;
		}

		public override TagCompound Save() {
			var tags = new TagCompound {
				{"now_tracking_meteor", this.NowTrackingMeteor},
				{"meteor_count", this.MeteorCount }
			};
			return tags;
		}

		////////////////

		public override void PreUpdate() {
			if( this.IsLoaded && DurabilityMod.Config.Data.MaxMeteors > 0 ) {
				if( this.MeteorCount < DurabilityMod.Config.Data.MaxMeteors ) {
					if( !this.NowTrackingMeteor && WorldGen.spawnMeteor ) {
						this.NowTrackingMeteor = true;
					}
					if( this.NowTrackingMeteor && !WorldGen.spawnMeteor ) {
						this.NowTrackingMeteor = false;
						this.MeteorCount++;
					}
				} else {
					WorldGen.spawnMeteor = false;
				}
			}
		}
	}
}
