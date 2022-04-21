using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	public class AnalyticsStuff
	{
		public enum LogTrafficForwardingMode
		{
			Show,
			Press
		}

		private const string eventNameBase = "Daily Gift";

		private const string WeaponsSpecialOffersEvent = "Weapons Special Offers";

		private static int trainingStep = -1;

		private static bool trainingStepLoaded = false;

		private static string trainingProgressKey = "TrainingStepKeyAnalytics";

		private static string[] trainingCustomEventStepValues = new string[19]
		{
			"1_First Launch", "2_Controls_Overview", "3_Controls_Move", "4_Controls_Jump", "5_Kill_Enemy", "6_Portal", "7_Rewards", "8_Open Shop", "9_Category_Sniper", "10_Equip Sniper",
			"11_Category_Armor", "12_Equip Armor", "13_Back Shop", "14_Connect Scene", "15_Table Deathmatch", "16_Play Deathmatch", "17(0)_Deathmatch Win", "18_Level Up (Finished)", "17(1)_Deathmatch Lose"
		};

		public static int TrainingStep
		{
			get
			{
				LoadTrainingStep();
				return trainingStep;
			}
		}

		internal static void TrySendOnceToAppsFlyer(string eventName, Lazy<Dictionary<string, string>> eventParams, Version excludeVersion)
		{
			//Discarded unreachable code: IL_0060
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			if (excludeVersion == null)
			{
				throw new ArgumentNullException("excludeVersion");
			}
			try
			{
				Version version = new Version(Switcher.InitialAppVersion);
				if (version <= excludeVersion)
				{
					return;
				}
			}
			catch
			{
				return;
			}
			string key = "Analytics:" + eventName;
			if (!Storager.hasKey(key) || string.IsNullOrEmpty(Storager.getString(key, false)))
			{
				Storager.setString(key, Json.Serialize(eventParams), false);
				AnalyticsFacade.SendCustomEventToAppsFlyer(eventName, eventParams.Value);
			}
		}

		public static void TrySendOnceToAppsFlyer(string eventName)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			string key = "Analytics:" + eventName;
			if (!Storager.hasKey(key) || string.IsNullOrEmpty(Storager.getString(key, false)))
			{
				Storager.setString(key, "{}", false);
				AnalyticsFacade.SendCustomEventToAppsFlyer(eventName, new Dictionary<string, string>());
			}
		}

		public static void LogCampaign(string map, string boxName)
		{
			try
			{
				if (string.IsNullOrEmpty(map))
				{
					Debug.LogError("LogCampaign string.IsNullOrEmpty(map)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Maps", map);
				Dictionary<string, object> dictionary2 = dictionary;
				if (boxName != null)
				{
					dictionary2.Add("Boxes", boxName);
				}
				AnalyticsFacade.SendCustomEvent("Campaign", dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogCampaign: " + ex);
			}
		}

		public static void LogMultiplayer()
		{
			try
			{
				string text = FlurryPluginWrapper.ModeNameForPurchasesAnalytics();
				if (text == null)
				{
					Debug.LogError("LogMultiplayer modeName == null");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Game Modes", text);
				dictionary.Add(text + " By Tier", ExpController.OurTierForAnyPlace() + 1);
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Multiplayer Total", eventParams);
				AnalyticsFacade.SendCustomEvent("Multiplayer" + FlurryPluginWrapper.GetPayingSuffixNo10(), eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogMultiplayer: " + ex);
			}
		}

		public static void LogSandboxTimeGamePopularity(int timeGame, bool isStart)
		{
			try
			{
				string key = ((timeGame != 5 && timeGame != 10 && timeGame != 15) ? "Other" : ("Time " + timeGame));
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(key, (!isStart) ? "End" : "Start");
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Sandbox", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Sandbox exception: " + ex);
			}
		}

		public static void LogFirstBattlesKillRate(int battleIndex, float killRate)
		{
			try
			{
				string empty = string.Empty;
				empty = ((killRate < 0.4f) ? "<0,4" : ((killRate < 0.6f) ? "0,4 - 0,6" : ((killRate < 0.8f) ? "0,6 - 0,8" : ((killRate < 1f) ? "0,8 - 1" : ((killRate < 1.2f) ? "1 - 1,2" : ((killRate < 1.5f) ? "1,2 - 1,5" : ((killRate < 2f) ? "1,5 - 2" : ((!(killRate < 3f)) ? ">3" : "2 - 3"))))))));
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Battle " + battleIndex, empty);
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("First Battles KillRate", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogFirstBattlesKillRate: " + ex);
			}
		}

		public static void LogFirstBattlesResult(int battleIndex, bool winner)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Battle " + battleIndex, (!winner) ? "Lose" : "Win");
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("First Battles Result", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogFirstBattlesResult: " + ex);
			}
		}

		public static void LogABTest(string nameTest, string nameCohort, bool isStart = true)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(nameTest, (!isStart) ? ("Excluded " + nameCohort) : nameCohort);
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("A/B Test", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("A/B Test exception: " + ex);
			}
		}

		public static void LogArenaWavesPassed(int countWaveComplite)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Waves Passed", (countWaveComplite >= 9) ? ">=9" : countWaveComplite.ToString());
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Arena", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("ArenaFirst  exception: " + ex);
			}
		}

		public static void LogArenaFirst(bool isPause, bool isMoreOneWave)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("First", isPause ? "Quit" : ((!isMoreOneWave) ? "Fail" : "Complete"));
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Arena", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("ArenaFirst  exception: " + ex);
			}
		}

		public static void Tutorial(AnalyticsConstants.TutorialState step, bool winDeathmatch = true)
		{
			try
			{
				LoadTrainingStep();
				if ((int)step > trainingStep)
				{
					trainingStep = (int)step;
					AnalyticsFacade.Tutorial(step);
					string value = ((!winDeathmatch) ? trainingCustomEventStepValues[trainingCustomEventStepValues.Length - 1] : trainingCustomEventStepValues[(int)step]);
					AnalyticsFacade.SendCustomEvent("Tutorial", new Dictionary<string, object> { { "Progress", value } });
					if (step > AnalyticsConstants.TutorialState.Portal)
					{
						SaveTrainingStep();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in Tutorial: " + ex);
			}
		}

		public static void SaveTrainingStep()
		{
			if (trainingStepLoaded)
			{
				Storager.setInt(trainingProgressKey, trainingStep, false);
			}
		}

		public static void LogDailyGiftPurchases(string packId)
		{
			try
			{
				if (string.IsNullOrEmpty(packId))
				{
					Debug.LogError("LogDailyGiftPurchases: string.IsNullOrEmpty(packId)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Purchases", ReadableNameForInApp(packId));
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Daily Gift Total", eventParams);
				AnalyticsFacade.SendCustomEvent("Daily Gift" + FlurryPluginWrapper.GetPayingSuffixNo10(), eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogDailyGiftPurchases: " + ex);
			}
		}

		public static void LogDailyGift(string giftId, int count, bool isForMoneyGift)
		{
			try
			{
				if (string.IsNullOrEmpty(giftId))
				{
					Debug.LogError("LogDailyGift: string.IsNullOrEmpty(giftId)");
					return;
				}
				if (SkinsController.shopKeyFromNameSkin.ContainsKey(giftId))
				{
					giftId = "Skin";
				}
				giftId = giftId + "_" + count;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Chance", giftId);
				dictionary.Add("Spins", (!isForMoneyGift) ? "Free" : "Paid");
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Daily Gift Total", eventParams);
				AnalyticsFacade.SendCustomEvent("Daily Gift" + FlurryPluginWrapper.GetPayingSuffixNo10(), eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogDailyGift: " + ex);
			}
		}

		public static void LogTrafficForwarding(LogTrafficForwardingMode mode)
		{
			try
			{
				string text = ((mode != 0) ? "Button Pressed" : "Button Show");
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Conversion", text);
				dictionary.Add(text + " Levels", (!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
				dictionary.Add(text + " Tiers", ExpController.OurTierForAnyPlace() + 1);
				dictionary.Add(text + " Paying", (!FlurryPluginWrapper.IsPayingUser()) ? "FALSE" : "TRUE");
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Pereliv Button", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogTrafficForwarding: " + ex);
			}
		}

		public static void LogWEaponsSpecialOffers_MoneySpended(string packId)
		{
			try
			{
				if (string.IsNullOrEmpty(packId))
				{
					Debug.LogError("LogWEaponsSpecialOffers_MoneySpended: string.IsNullOrEmpty(packId)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Money Spended", ReadableNameForInApp(packId));
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Weapons Special Offers Total", eventParams);
				AnalyticsFacade.SendCustomEvent("Weapons Special Offers" + FlurryPluginWrapper.GetPayingSuffixNo10(), eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogWEaponsSpecialOffers_MoneySpended: " + ex);
			}
		}

		public static void LogWEaponsSpecialOffers_Conversion(bool show, string weaponId = null)
		{
			try
			{
				if (!show && string.IsNullOrEmpty(weaponId))
				{
					Debug.LogError("LogWEaponsSpecialOffers_Conversion: string.IsNullOrEmpty(weaponId)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Conversion", (!show) ? "Buy" : "Show");
				Dictionary<string, object> dictionary2 = dictionary;
				try
				{
					float num = ((!FriendsController.useBuffSystem) ? KillRateCheck.instance.GetKillRate() : BuffSystem.instance.GetKillrateByInteractions());
					string arg = ((num <= 0.5f) ? "Weak" : ((!(num <= 1.2f)) ? "Strong" : "Normal"));
					string key = string.Format("Conversion {0} Players", arg);
					if (!show)
					{
						dictionary2.Add("Currency Spended", weaponId);
						dictionary2.Add("Buy (Tier)", ExpController.OurTierForAnyPlace() + 1);
						dictionary2.Add("Buy (Level)", (!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
						dictionary2.Add(key, "Buy");
					}
					else
					{
						dictionary2.Add("Show (Tier)", ExpController.OurTierForAnyPlace() + 1);
						dictionary2.Add("Show (Level)", (!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
						dictionary2.Add(key, "Show");
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in LogWEaponsSpecialOffers_Conversion adding paramters: " + ex);
				}
				AnalyticsFacade.SendCustomEvent("Weapons Special Offers Total", dictionary2);
				AnalyticsFacade.SendCustomEvent("Weapons Special Offers" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex2)
			{
				Debug.LogError("Exception in LogWEaponsSpecialOffers_Conversion: " + ex2);
			}
		}

		public static void LogSpecialOffersPanel(string efficiencyPArameter, string efficiencyValue, string additionalParameter = null, string additionalValue = null)
		{
			try
			{
				if (string.IsNullOrEmpty(efficiencyPArameter) || string.IsNullOrEmpty(efficiencyValue))
				{
					Debug.LogError("LogSpecialOffersPanel:  string.IsNullOrEmpty(efficiencyPArameter) || string.IsNullOrEmpty(efficiencyValue)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(efficiencyPArameter, efficiencyValue);
				Dictionary<string, object> dictionary2 = dictionary;
				if (additionalParameter != null && additionalValue != null)
				{
					dictionary2.Add(additionalParameter, additionalValue);
				}
				AnalyticsFacade.SendCustomEvent("Special Offers Banner Total", dictionary2);
				AnalyticsFacade.SendCustomEvent("Special Offers Banner" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogSpecialOffersPanel: " + ex);
			}
		}

		public static void LogSales(string itemId, string categoryParameterName, bool isDaterWeapon = false)
		{
			try
			{
				if (string.IsNullOrEmpty(itemId))
				{
					Debug.LogError("LogSales: string.IsNullOrEmpty(itemId)");
					return;
				}
				if (string.IsNullOrEmpty(categoryParameterName))
				{
					Debug.LogError("LogSales: string.IsNullOrEmpty(categoryParameterName)");
					return;
				}
				string[] source = new string[11]
				{
					"Stickers", "Premium Maps", "Gear", "Premium Account", "Skins", "Armor", "Boots", "Capes", "Hats", "Starter Pack",
					"Masks"
				};
				string[] array = new string[6] { "Primary", "Back Up", "Melee", "Special", "Sniper", "Premium" };
				string text = ((!source.Contains(categoryParameterName)) ? "Weapons Sales" : "Equipment Sales");
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(categoryParameterName, itemId);
				Dictionary<string, object> dictionary2 = dictionary;
				if (isDaterWeapon)
				{
					dictionary2.Add("Dater Weapons", itemId);
				}
				AnalyticsFacade.SendCustomEvent(text + " Total", dictionary2);
				AnalyticsFacade.SendCustomEvent(text + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogSales: " + ex);
			}
		}

		public static void RateUsFake(bool rate, int stars, bool sendNegativFeedback = false)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Efficiency", (!rate) ? "Later" : "Rate");
				if (rate)
				{
					dictionary.Add("Rating (Stars)", stars);
				}
				if (stars > 0 && stars < 4)
				{
					dictionary.Add("Negative Feedback", (!sendNegativFeedback) ? "Not sended" : "Sended");
				}
				AnalyticsFacade.SendCustomEvent("Rate Us Fake", dictionary);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in RateUsFake: " + ex);
			}
		}

		public static string ReadableNameForInApp(string purchaseId)
		{
			return (!StoreKitEventListener.inAppsReadableNames.ContainsKey(purchaseId)) ? purchaseId : StoreKitEventListener.inAppsReadableNames[purchaseId];
		}

		private static void LoadTrainingStep()
		{
			if (!trainingStepLoaded)
			{
				if (!Storager.hasKey(trainingProgressKey))
				{
					trainingStep = -1;
					Storager.setInt(trainingProgressKey, trainingStep, false);
				}
				else
				{
					trainingStep = Storager.getInt(trainingProgressKey, false);
				}
				trainingStepLoaded = true;
			}
		}
	}
}
