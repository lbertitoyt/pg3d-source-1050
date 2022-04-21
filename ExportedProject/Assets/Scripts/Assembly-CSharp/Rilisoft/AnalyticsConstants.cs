namespace Rilisoft
{
	public sealed class AnalyticsConstants
	{
		public enum AccrualType
		{
			Earned,
			Purchased
		}

		public enum TutorialState
		{
			Started,
			Controls_Overview,
			Controls_Move,
			Controls_Jump,
			Kill_Enemy,
			Portal,
			Rewards,
			Open_Shop,
			Category_Sniper,
			Equip_Sniper,
			Category_Armor,
			Equip_Armor,
			Back_Shop,
			Connect_Scene,
			Table_Deathmatch,
			Play_Deathmatch,
			Deathmatch_Completed,
			Finished
		}

		public const string LevelUp = "LevelUp";

		public const string ViralityEvent = "Virality";

		public const string SocialEvent = "Social";
	}
}
