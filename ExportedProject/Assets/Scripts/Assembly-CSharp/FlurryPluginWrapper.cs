using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class FlurryPluginWrapper : MonoBehaviour
{
	public const string BackToMainMenu = "Back to Main Menu";

	public const string UnlockHungerMoney = "Enable_Deadly Games";

	public const string AppsFlyerInitiatedCheckout = "af_initiated_checkout";

	private const string TeamBattleForPurchasesAnalytics = "Team Battle";

	private const string DeathmatchForPurchasesAnalytics = "Deathmatch";

	public static string ModeEnteredEvent = "ModeEnteredEvent";

	public static string MapEnteredEvent = "MapEnteredEvent";

	public static string MapNameParameter = "MapName";

	public static string ModeParameter = "Mode";

	public static string ModePressedEvent = "ModePressed";

	public static string SocialEventName = "Post to Social";

	public static string SocialParName = "Service";

	public static string AppVersionParameter = "App_version";

	public static string MultiplayeLocalEvent = "Local Button Pressed";

	public static string MultiplayerWayDeaathmatchEvent = "Way_to_start_multiplayer_DEATHMATCH";

	public static string MultiplayerWayCOOPEvent = "Way_to_start_multiplayer_COOP";

	public static string MultiplayerWayCompanyEvent = "Way_to_start_multiplayer_Company";

	public static string WayName = "Button";

	public static readonly string HatsCapesShopPressedEvent = "Hats_Capes_Shop";

	public static string FreeCoinsEv = "FreeCoins";

	public static string FreeCoinsParName = "type";

	public static string RateUsEv = "Rate_Us";

	private float _startSession;

	private static bool _sessionStarted = false;

	private static Dictionary<string, int> antiCheatLimitsPaying = new Dictionary<string, int>
	{
		{ "Coins", 14000 },
		{ "GemsCurrency", 11000 }
	};

	private static Dictionary<string, int> antiCheatLimitsNonPaying = new Dictionary<string, int>
	{
		{ "Coins", 50000 },
		{ "GemsCurrency", 900 }
	};

	public static Dictionary<string, string> LevelAndTierParameters
	{
		get
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Levels", ((ExperienceController.sharedController != null) ? ExperienceController.sharedController.currentLevel : 0).ToString());
			dictionary.Add("Tiers", (((ExpController.Instance != null) ? ExpController.Instance.OurTier : 0) + 1).ToString());
			return dictionary;
		}
	}

	public static string MultiplayerWayEvent
	{
		get
		{
			return Defs.isCOOP ? MultiplayerWayCOOPEvent : ((!Defs.isCompany) ? MultiplayerWayDeaathmatchEvent : MultiplayerWayCompanyEvent);
		}
	}

	private static bool UserIsCheaterByCurrenciesCount()
	{
		Dictionary<string, int> dictionary = ((!IsPayingUser()) ? antiCheatLimitsNonPaying : antiCheatLimitsPaying);
		return Storager.getInt("Coins", false) >= dictionary["Coins"] || Storager.getInt("GemsCurrency", false) >= dictionary["GemsCurrency"];
	}

	private static void FlurryLogEventCore(string eventName, bool isTimed = false)
	{
	}

	public static string ConvertFromBase64(string s)
	{
		byte[] array = Convert.FromBase64String(s);
		return Encoding.UTF8.GetString(array, 0, array.Length);
	}

	public static string ConvertToBase64(string s)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
	}

	public static bool IsAdditionalLoggingAvailable()
	{
		//Discarded unreachable code: IL_008e, IL_00ab
		try
		{
			if (BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer)
			{
				return false;
			}
			return File.Exists(ConvertFromBase64("L0FwcGxpY2F0aW9ucy9DeWRpYS5hcHA=")) || File.Exists(ConvertFromBase64("L0xpYnJhcnkvTW9iaWxlU3Vic3RyYXRlL01vYmlsZVN1YnN0cmF0ZS5keWxpYg==")) || File.Exists(ConvertFromBase64("L2Jpbi9iYXNo")) || File.Exists(ConvertFromBase64("L3Vzci9zYmluL3NzaGQ=")) || File.Exists(ConvertFromBase64("L2V0Yy9hcHQ=")) || Directory.Exists(ConvertFromBase64("L3ByaXZhdGUvdmFyL2xpYi9hcHQv"));
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Exception in IsAdditionalLoggingAvailable: " + ex);
			return false;
		}
	}

	public static bool IsLoggingFlurryAnalyticsSupported()
	{
		//Discarded unreachable code: IL_00d9, IL_00f6
		try
		{
			if (BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer)
			{
				return false;
			}
			string path = ConvertFromBase64("L0xpYnJhcnkvTW9iaWxlU3Vic3RyYXRlL0R5bmFtaWNMaWJyYXJpZXM=");
			if (File.Exists(Path.Combine(path, ConvertFromBase64("TG9jYWxJQVBTdG9yZS5keWxpYg=="))) || File.Exists(Path.Combine(path, ConvertFromBase64("TG9jYWxsQVBTdG9yZS5keWxpYg=="))))
			{
				Debug.LogWarning("IsLoggingFlurryAnalyticsSupported: logging supported");
				return true;
			}
			if (File.Exists(Path.Combine(path, ConvertFromBase64("aWFwLmR5bGli"))))
			{
				Debug.LogWarning("IsLoggingFlurryAnalyticsSupported: logging_supported");
				return true;
			}
			if (File.Exists(Path.Combine(path, ConvertFromBase64("aWFwZnJlZS5jb3JlLmR5bGli"))) || File.Exists(Path.Combine(path, ConvertFromBase64("SUFQRnJlZVNlcnZpY2UuZHlsaWI="))))
			{
				Debug.LogWarning("IsLoggingFlurryAnalyticsSupported: logging__supported");
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Exception in IsLoggingFlurryAnalyticsSupported: " + ex);
			return false;
		}
	}

	public static void LogLevelPressed(string n)
	{
		FlurryLogEventCore(n + "_Pressed");
	}

	public static void LogBoxOpened(string nm)
	{
		LogEvent(nm + "_Box_Opened");
	}

	private static void FlurryLogEventWithParametersCore(string ev, Dictionary<string, string> parameters, bool isTimed = false)
	{
	}

	public static void LogEventWithParameterAndValue(string ev, string pat, string val)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(pat, val);
		dictionary.Add("Paying User", IsPayingUser().ToString());
		Dictionary<string, string> parameters = dictionary;
		FlurryLogEventWithParametersCore(ev, parameters);
	}

	public static void LogEvent(string eventName)
	{
		FlurryLogEventCore(eventName);
	}

	public static void LogTimedEvent(string eventName)
	{
		FlurryLogEventCore(eventName, true);
	}

	public static void LogEvent(string eventName, Dictionary<string, string> parameters, bool addPaying = true)
	{
		if (addPaying && !parameters.ContainsKey("Paying User"))
		{
			parameters.Add("Paying User", IsPayingUser().ToString());
		}
		FlurryLogEventWithParametersCore(eventName, parameters);
	}

	public static void LogTimedEvent(string eventName, Dictionary<string, string> parameters)
	{
		if (!parameters.ContainsKey("Paying User"))
		{
			parameters.Add("Paying User", IsPayingUser().ToString());
		}
		FlurryLogEventWithParametersCore(eventName, parameters, true);
	}

	public static void EndTimedEvent(string eventName)
	{
	}

	public static void LogEventAndDublicateToConsole(string eventName, Dictionary<string, string> parameters, bool addPaying = true)
	{
		LogEvent(eventName, parameters, addPaying);
		if (Defs.IsDeveloperBuild)
		{
			string text = ((parameters == null) ? "{}" : Json.Serialize(parameters));
			if (Application.isEditor)
			{
				Debug.LogFormat("<color=lightblue>{0}: {1}</color>", eventName, text);
			}
			else
			{
				Debug.LogFormat("{0}: {1}", eventName, text);
			}
		}
	}

	public static void LogEventAndDublicateToEditor(string eventName, Dictionary<string, string> parameters, [Optional] Color32 color)
	{
		LogEvent(eventName, parameters);
		if (Application.isEditor)
		{
			string text = ((parameters == null) ? "{ }" : Json.Serialize(parameters));
			string message = ((!(color == default(Color))) ? string.Format("<color=#{2:X2}{3:X2}{4:X2}>{0}: {1}</color>", eventName, text, color.r, color.g, color.b) : (eventName + ": " + text));
			Debug.Log(message);
		}
	}

	public static void LogFastPurchase(string purchaseKind)
	{
		if (ExperienceController.sharedController != null)
		{
			int currentLevel = ExperienceController.sharedController.currentLevel;
			int num = (currentLevel - 1) / 9;
			string arg = string.Format("[{0}, {1})", num * 9 + 1, (num + 1) * 9 + 1);
			string eventName = string.Format("Shop Purchases On Level {0} ({1}){2}", arg, (!IsPayingUser()) ? "Non Paying" : "Paying", string.Empty);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Level " + currentLevel, purchaseKind);
			Dictionary<string, string> parameters = dictionary;
			LogEventAndDublicateToConsole(eventName, parameters);
		}
		else
		{
			Debug.LogWarning("ExperienceController.sharedController == null");
		}
	}

	public static void LogMatchCompleted(string mode)
	{
		if (ExperienceController.sharedController != null)
		{
			string eventName = string.Format("Match Completed ({0})", (!IsPayingUser()) ? "Non Paying" : "Paying");
			LogEventAndDublicateToConsole(eventName, new Dictionary<string, string>
			{
				{
					"Rank",
					ExperienceController.sharedController.currentLevel.ToString()
				},
				{ "Mode", mode }
			});
		}
	}

	public static void LogWinInMatch(string mode)
	{
		if (ExperienceController.sharedController != null)
		{
			string eventName = string.Format("Win In Match ({0})", (!IsPayingUser()) ? "Non Paying" : "Paying");
			LogEventAndDublicateToConsole(eventName, new Dictionary<string, string>
			{
				{
					"Rank",
					ExperienceController.sharedController.currentLevel.ToString()
				},
				{ "Mode", mode }
			});
		}
	}

	public static void LogTimedEventAndDublicateToConsole(string eventName)
	{
		LogTimedEvent(eventName);
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log(eventName);
		}
	}

	public static void LogModeEventWithValue(string val)
	{
		if (!PlayerPrefs.HasKey("Mode Pressed First Time"))
		{
			PlayerPrefs.SetInt("Mode Pressed First Time", 0);
			LogEventWithParameterAndValue("Mode Pressed First Time", ModeParameter, val);
		}
		else
		{
			LogEventWithParameterAndValue(ModePressedEvent, ModeParameter, val);
		}
	}

	public static void LogMultiplayerWayStart()
	{
		LogEventWithParameterAndValue(MultiplayerWayEvent, WayName, "Start");
		LogEvent("Start");
	}

	public static void LogMultiplayerWayQuckRandGame()
	{
		LogEventWithParameterAndValue(MultiplayerWayEvent, WayName, "Quick_rand_game");
		LogEvent("Random");
	}

	public static void LogMultiplayerWayCustom()
	{
		LogEventWithParameterAndValue(MultiplayerWayEvent, WayName, "Custom");
		LogEvent("Custom");
	}

	public static void LogDeathmatchModePress()
	{
		LogModeEventWithValue("Deathmatch");
		LogEvent("Deathmatch");
	}

	public static void LogCampaignModePress()
	{
		LogModeEventWithValue("Survival");
		LogEvent("Campaign");
	}

	public static void LogTrueSurvivalModePress()
	{
		LogModeEventWithValue("Arena_Survival");
		LogEvent("Survival");
	}

	public static void LogCooperativeModePress()
	{
		LogModeEventWithValue("COOP");
		LogEvent("Cooperative");
	}

	public static void LogSkinsMakerModePress()
	{
		LogEvent("Skins Maker");
	}

	public static void LogTwitter()
	{
		LogEventWithParameterAndValue(SocialEventName, SocialParName, "Twitter");
	}

	public static void LogFacebook()
	{
		LogEventWithParameterAndValue(SocialEventName, SocialParName, "Facebook");
	}

	public static void LogGamecenter()
	{
		LogEvent("Game Center");
	}

	public static void LogFreeCoinsFacebook()
	{
		LogEventWithParameterAndValue(FreeCoinsEv, FreeCoinsParName, "Facebook");
		LogEvent("Facebook");
	}

	public static void LogFreeCoinsTwitter()
	{
		LogEventWithParameterAndValue(FreeCoinsEv, FreeCoinsParName, "Twitter");
		LogEvent("Twitter");
	}

	public static void LogFreeCoinsYoutube()
	{
		LogEventWithParameterAndValue(FreeCoinsEv, FreeCoinsParName, "Youtube");
		LogEvent("YouTube");
	}

	public static void LogCoinEarned()
	{
		LogEvent("Earned Coin Survival");
	}

	public static void LogCoinEarned_COOP()
	{
		LogEvent("Earned Coin COOP");
	}

	public static void LogCoinEarned_Deathmatch()
	{
		LogEvent("Earned Coin Deathmatch");
	}

	public static void LogFreeCoinsRateUs()
	{
		LogEvent(RateUsEv);
	}

	public static void LogSkinsMakerEnteredEvent()
	{
		LogEvent("SkinsMaker");
	}

	public static void LogAddYourSkinTriedToBoughtEvent()
	{
		LogEvent("AddYourSkin_TriedToBought");
	}

	public static void LogAddYourSkinUsedEvent()
	{
		LogEvent("AddYourSkin_Used");
	}

	public static void LogMultiplayeLocalEvent()
	{
		LogEvent(MultiplayeLocalEvent);
	}

	public static void LogMultiplayeWorldwideEvent()
	{
		LogEvent("Worldwide");
	}

	public static void LogCategoryEnteredEvent(string catName)
	{
		LogEventWithParameterAndValue("Dhop_Category", "Category_name", catName);
	}

	public static void LogEnteringMap(int typeConnect, string mapName)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(MapNameParameter, mapName);
		Dictionary<string, string> parameters = dictionary;
		string ev = (Defs.isCOOP ? "COOP" : "Deathmatch_WorldWide");
		FlurryLogEventWithParametersCore(ev, parameters);
	}

	public static void DoWithResponse(HttpWebRequest request, Action<HttpWebResponse> responseAction)
	{
		Action action = delegate
		{
			request.BeginGetResponse((AsyncCallback)delegate(IAsyncResult iar)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Expected O, but got Unknown
				HttpWebResponse obj = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
				responseAction(obj);
			}, (object)request);
		};
		action.BeginInvoke(delegate(IAsyncResult iar)
		{
			Action action2 = (Action)iar.AsyncState;
			action2.EndInvoke(iar);
		}, action);
	}

	public static HttpWebRequest RequestAppWithID(string _id)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		return (HttpWebRequest)WebRequest.Create("http://itunes.apple.com/lookup?id=" + _id);
	}

	public static bool IsPayingUser()
	{
		return Storager.getInt("PayingUser", true) > 0;
	}

	public static string GetEventX3State()
	{
		if (PromoActionsManager.sharedManager.IsEventX3Active)
		{
			if (PromoActionsManager.sharedManager.IsNewbieEventX3Active)
			{
				return "Newbie";
			}
			return "Common";
		}
		return "None";
	}

	private void CheckForEdnermanApp()
	{
	}

	private IEnumerator OnApplicationPause(bool pause)
	{
		if (pause)
		{
			EndSession();
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				FlurryLogEventCore("Application Paused", true);
			}
			yield break;
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			EndTimedEvent("Application Paused");
		}
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		CheckForEdnermanApp();
		StartSession();
	}

	private void StartSession()
	{
		_startSession = Time.realtimeSinceStartup;
		int @int = PlayerPrefs.GetInt("AppsFlyer.SessionIndex", 0);
		int value = @int + 1;
		PlayerPrefs.SetInt("AppsFlyer.SessionIndex", value);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Session count", value.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("Timestamp (UTC)", DateTime.UtcNow.ToString("s"));
		Dictionary<string, string> attributes = dictionary;
		LogEventToAppsFlyer("Start session", attributes);
	}

	private void EndSession()
	{
		float num = Time.realtimeSinceStartup - _startSession;
		_startSession += num;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Duration (s)", num.ToString("F1", CultureInfo.InvariantCulture));
		dictionary.Add("Timestamp (UTC)", DateTime.UtcNow.ToString("s"));
		Dictionary<string, string> attributes = dictionary;
		LogEventToAppsFlyer("End session", attributes);
	}

	private void Start()
	{
		FriendsController.NewCheaterDetectParametersAvailable += FriendsController_NewCheaterDetectParametersAvailable;
		StartSession();
		CheckForEdnermanApp();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_sessionStarted = true;
	}

	private void FriendsController_NewCheaterDetectParametersAvailable(int coinsLimitPaying, int gemsLimitPaying, int coinsLimitNonPaying, int gemsLimitNonPaying)
	{
		antiCheatLimitsPaying["Coins"] = coinsLimitPaying;
		antiCheatLimitsPaying["GemsCurrency"] = gemsLimitPaying;
		antiCheatLimitsNonPaying["Coins"] = coinsLimitNonPaying;
		antiCheatLimitsNonPaying["GemsCurrency"] = gemsLimitNonPaying;
	}

	private void OnApplicationQuit()
	{
		EndSession();
	}

	private void OnDestroy()
	{
		FriendsController.NewCheaterDetectParametersAvailable -= FriendsController_NewCheaterDetectParametersAvailable;
	}

	internal static void LogEventToAppsFlyer(string eventName, Dictionary<string, string> attributes)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			Debug.LogError("Event name should not be empty.");
			return;
		}
		if (attributes == null)
		{
			Debug.LogError("Event values should not be null.");
			return;
		}
		if (!attributes.ContainsKey("deviceModel"))
		{
			attributes["deviceModel"] = SystemInfo.deviceModel;
		}
		if (ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel > 0 && !attributes.ContainsKey("level"))
		{
			attributes["level"] = ExperienceController.sharedController.currentLevel.ToString();
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			AppsFlyer.trackRichEvent(eventName, attributes);
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.LogFormat("{0}: {1}", eventName, Json.Serialize(attributes));
		}
	}

	public static string PlaceForPurchasesAnalytics(bool fromBottomPanel = false)
	{
		string result = "None";
		try
		{
			if (fromBottomPanel)
			{
				if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
				{
					if (!Defs.isMulti)
					{
						result = "Single Bottom Panel";
						return result;
					}
					result = "Bottom Panel";
					return result;
				}
				result = "Bottom Panel";
				return result;
			}
			if (Application.loadedLevelName == Defs.MainMenuScene)
			{
				result = "Lobby";
				return result;
			}
			if (Defs.inRespawnWindow)
			{
				result = "Killcam";
				return result;
			}
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				if (!Defs.isMulti)
				{
					result = "Single Battle";
					return result;
				}
				result = "Battle";
				return result;
			}
			if (Application.loadedLevelName == "LevelComplete")
			{
				result = "Single Score (End)";
				return result;
			}
			if (NetworkStartTableNGUIController.IsEndInterfaceShown())
			{
				result = "Score (End)";
				return result;
			}
			if (Application.loadedLevelName == "ChooseLevel")
			{
				result = "Single Score (Start)";
				return result;
			}
			if (NetworkStartTableNGUIController.IsStartInterfaceShown())
			{
				result = "Score (Start)";
				return result;
			}
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in PlaceForPurchasesAnalytics: " + ex);
			return result;
		}
	}

	public static string ModeNameForPurchasesAnalytics(bool forNormalMultyModesUseMultyplayer = false)
	{
		string result = null;
		try
		{
			if (!Defs.IsSurvival && !Defs.isMulti)
			{
				result = "Campaign";
				return result;
			}
			if (Defs.IsSurvival && !Defs.isMulti)
			{
				result = "Arena";
				return result;
			}
			if (Defs.isMulti)
			{
				if (Application.loadedLevelName != Defs.MainMenuScene)
				{
					if (Application.loadedLevelName != "Clans")
					{
						if (Defs.isDaterRegim)
						{
							result = "Sandbox";
							return result;
						}
						if (forNormalMultyModesUseMultyplayer)
						{
							result = "Multiplayer";
							return result;
						}
						if (Defs.isCompany)
						{
							result = "Team Battle";
							return result;
						}
						if (Defs.isCapturePoints)
						{
							result = "Point Capture";
							return result;
						}
						if (Defs.isCOOP)
						{
							result = "COOP Survival";
							return result;
						}
						if (Defs.isFlag)
						{
							result = "Flag Capture";
							return result;
						}
						if (Defs.isHunger)
						{
							result = "Deadly Games";
							return result;
						}
						result = "Deathmatch";
						return result;
					}
					return result;
				}
				return result;
			}
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ModeNameForPurchasesAnalytics: " + ex);
			return result;
		}
	}

	public static void LogGearPurchases(string gearId, int gearCount, bool fromBottomPanel)
	{
		try
		{
			if (gearId == null)
			{
				Debug.LogError("LogGearPurchases: gearId = null");
				return;
			}
			string text = string.Format("Gear Purchases{0}", GetPayingSuffixNo10());
			string text2 = PlaceForPurchasesAnalytics(fromBottomPanel);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Total", gearId);
			Dictionary<string, string> dictionary2 = dictionary;
			if (!string.IsNullOrEmpty(text2))
			{
				dictionary2.Add(text2, gearId);
			}
			for (int i = 0; i < gearCount; i++)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>GearPurchasesEventName = " + text + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
			}
			for (int j = 0; j < gearCount; j++)
			{
				LogEventAndDublicateToConsole(text, dictionary2);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in LogGearPurchases: " + ex);
		}
	}

	public static void LogPurchasesPoints(bool isWeaponEvent)
	{
		try
		{
			string value = PlaceForPurchasesAnalytics();
			string text = PurchasesPointsEventName();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Total", value);
			Dictionary<string, string> dictionary2 = dictionary;
			if (isWeaponEvent)
			{
				dictionary2.Add("Total Weapons", value);
			}
			string text2 = null;
			text2 = ModeNameForPurchasesAnalytics();
			if (text2 != null)
			{
				dictionary2.Add(text2, value);
			}
			if (Debug.isDebugBuild)
			{
				Debug.Log("<color=green>PurchasesPointsEventName = " + text + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
			}
			LogEventAndDublicateToConsole(text, dictionary2);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in LogPurchasesPoints: " + ex);
		}
	}

	public static void LogPurchaseByPoints(ShopNGUIController.CategoryNames category, string itemId, int count)
	{
		try
		{
			string arg = PlaceForPurchasesAnalytics().Replace("Single ", string.Empty);
			string text = ModeNameForPurchasesAnalytics(true);
			string text2 = string.Format("Purchases {0} {1}{2}", text ?? string.Empty, arg, GetPayingSuffixNo10()).Replace("Multiplayer Lobby", "Lobby");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("All Categories", category.ToString());
			dictionary.Add(category.ToString(), itemId);
			Dictionary<string, string> dictionary2 = dictionary;
			for (int i = 0; i < count; i++)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>PurchaseInModeEventName = " + text2 + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
			}
			for (int j = 0; j < count; j++)
			{
				LogEventAndDublicateToConsole(text2, dictionary2);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in LogPurchaseByPoints: " + ex);
		}
	}

	public static void LogPurchaseByModes(ShopNGUIController.CategoryNames category, string itemId, int count, bool UNUSED_fromBottomPanel)
	{
		try
		{
			string text = ModeNameForPurchasesAnalytics();
			if (text == null || Application.loadedLevelName == Defs.MainMenuScene)
			{
				return;
			}
			string text2 = string.Format("Purchases {0}{1}", text, GetPayingSuffixNo10());
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("All Categories", category.ToString());
			dictionary.Add(category.ToString(), itemId);
			Dictionary<string, string> dictionary2 = dictionary;
			for (int i = 0; i < count; i++)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>EventName = " + text2 + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
			}
			for (int j = 0; j < count; j++)
			{
				LogEventAndDublicateToConsole(text2, dictionary2);
			}
			if (Defs.isInet || (!(text == "Team Battle") && !(text == "Deathmatch")))
			{
				return;
			}
			string text3 = string.Format("Purchases Local{0}", GetPayingSuffixNo10());
			for (int k = 0; k < count; k++)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>EventName = " + text3 + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
			}
			for (int l = 0; l < count; l++)
			{
				LogEventAndDublicateToConsole(text3, dictionary2);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in LogPurchaseByModes: " + ex);
		}
	}

	public static string GetEventName(string eventName)
	{
		return eventName + GetPayingSuffix();
	}

	public static string GetPayingSuffix()
	{
		return GetPayingSuffixNo10();
	}

	public static string PurchasesPointsEventName()
	{
		return string.Format("{0}{1}", "Purchases Points", GetPayingSuffixNo10());
	}

	public static string GetPayingSuffixNo10()
	{
		if (!IsPayingUser())
		{
			return " (Non Paying)";
		}
		return " (Paying)";
	}
}
