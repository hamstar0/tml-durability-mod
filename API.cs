namespace Durability {
	public static class DurabilityAPI {
		public static DurabilityConfigData GetModSettings() {
			return DurabilityMod.Instance.Config;
		}
	}
}
