using System;
using System.Collections.Generic;
using System.Globalization;
using DevToDev;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class AnalyticsFacade
	{
		private static bool _initialized = false;

		private static DevToDevFacade _devToDevFacade;

		private static AppsFlyerFacade _appsFlyerFacade;

		private static readonly Lazy<string> _simpleEventFormat = new Lazy<string>(InitializeSimpleEventFormat);

		private static readonly Lazy<string> _parametrizedEventFormat = new Lazy<string>(InitializeParametrizedEventFormat);

		public static bool DuplicateToConsoleByDefault { get; set; }

		public static bool LoggingEnabled
		{
			set
			{
				DevToDevFacade.LoggingEnabled = value;
				AppsFlyerFacade.LoggingEnabled = value;
			}
		}

		internal static DevToDevFacade DevToDevFacade
		{
			get
			{
				return _devToDevFacade;
			}
		}

		internal static AppsFlyerFacade AppsFlyerFacade
		{
			get
			{
				return _appsFlyerFacade;
			}
		}

		public static void Initialize()
		{
			if (_initialized)
			{
				return;
			}
			if (MiscAppsMenu.Instance == null)
			{
				Debug.LogError("MiscAppsMenu.Instance == null");
				return;
			}
			if (MiscAppsMenu.Instance.misc == null)
			{
				Debug.LogError("MiscAppsMenu.Instance.misc == null");
				return;
			}
			try
			{
				HiddenSettings misc = MiscAppsMenu.Instance.misc;
				DuplicateToConsoleByDefault = Defs.IsDeveloperBuild;
				LoggingEnabled = Defs.IsDeveloperBuild;
				string text = string.Empty;
				string text2 = string.Empty;
				if (Defs.IsDeveloperBuild || Application.isEditor)
				{
					switch (BuildSettings.BuildTargetPlatform)
					{
					case RuntimePlatform.Android:
						text = "8517441f-d330-04c5-b621-5d88e92f50e3";
						text2 = "xkjaPTLIgGQKs5MftquXrEHDW0y8OBAS";
						break;
					case RuntimePlatform.IPhonePlayer:
						text = "92002d69-82d8-067e-997d-88d1c5e804f7";
						text2 = "tQ4zhKGBvyFVObPUofaiHj7pSAcWn3Mw";
						break;
					}
				}
				else
				{
					switch (BuildSettings.BuildTargetPlatform)
					{
					case RuntimePlatform.Android:
						if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
						{
							text = "8d1482db-5181-0647-a80e-decf21db619f";
							text2 = misc.devtodevSecretGoogle;
						}
						else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
						{
							text = "531e6d54-b959-06c1-8a38-6dfdfbf309eb";
							text2 = misc.devtodevSecretAmazon;
						}
						break;
					case RuntimePlatform.IPhonePlayer:
						text = "3c77b196-8042-0dab-a5dc-92eb4377aa8e";
						text2 = misc.devtodevSecretIos;
						break;
					case RuntimePlatform.MetroPlayerX64:
						text = "cd19ad66-971e-09b2-b449-ba84d3fb52d8";
						text2 = misc.devtodevSecretWsa;
						break;
					}
				}
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Initializing DevtoDev {0}; appId: '*{1}', appSecret: '*{2}'...", DevToDevFacade.Version, text.Substring(Math.Max(text.Length - 4, 0)), text2.Substring(Math.Max(text2.Length - 4, 0)));
				}
				InitializeDevToDev(text, text2);
				string text3 = string.Empty;
				string appsFlyerAppKey = misc.appsFlyerAppKey;
				if (!Defs.IsDeveloperBuild && !Application.isEditor)
				{
					switch (BuildSettings.BuildTargetPlatform)
					{
					case RuntimePlatform.Android:
						if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
						{
							text3 = "com.pixel.gun3d";
						}
						else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
						{
							text3 = "com.PixelGun.a3D";
						}
						break;
					case RuntimePlatform.IPhonePlayer:
						text3 = "ecd1e376-8e2f-45e4-a9dc-9e938f999d20";
						break;
					}
				}
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Initializing AppsFlyer; appsFlyerAppKey: '*{0}', appsFlyerAppId: '*{1}'...", appsFlyerAppKey.Substring(Math.Max(appsFlyerAppKey.Length - 4, 0)), text3.Substring(Math.Max(text3.Length - 4, 0)));
				}
				InitializeAppsFlyer(appsFlyerAppKey, text3);
				_initialized = true;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in AnalyticsFacade.Initialize: " + ex);
			}
		}

		public static void SendCustomEvent(string eventName)
		{
			Initialize();
			SendCustomEvent(eventName, DuplicateToConsoleByDefault);
		}

		public static void SendCustomEvent(string eventName, IDictionary<string, object> eventParams)
		{
			Initialize();
			SendCustomEvent(eventName, eventParams, DuplicateToConsoleByDefault);
		}

		public static void SendCustomEvent(string eventName, bool duplicateToConsole)
		{
			Initialize();
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (_devToDevFacade != null)
			{
				_devToDevFacade.SendCustomEvent(eventName);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_simpleEventFormat.Value, eventName);
			}
		}

		public static void SendCustomEvent(string eventName, IDictionary<string, object> eventParams, bool duplicateToConsole)
		{
			Initialize();
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			if (_devToDevFacade != null)
			{
				_devToDevFacade.SendCustomEvent(eventName, eventParams);
			}
			if (duplicateToConsole)
			{
				string text = Json.Serialize(eventParams);
				Debug.LogFormat(_parametrizedEventFormat.Value, eventName, text);
			}
		}

		public static void SendCustomEventToAppsFlyer(string eventName, Dictionary<string, string> eventParams)
		{
			Initialize();
			SendCustomEventToAppsFlyer(eventName, eventParams, DuplicateToConsoleByDefault);
		}

		public static void SendCustomEventToAppsFlyer(string eventName, Dictionary<string, string> eventParams, bool duplicateToConsole)
		{
			Initialize();
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			if (_appsFlyerFacade != null)
			{
				_appsFlyerFacade.TrackRichEvent(eventName, eventParams);
			}
			if (duplicateToConsole)
			{
				string text = Json.Serialize(eventParams);
				Debug.LogFormat(_parametrizedEventFormat.Value, eventName, text);
			}
		}

		public static void Flush()
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				_devToDevFacade.SendBufferedEvents();
			}
		}

		public static void Tutorial(AnalyticsConstants.TutorialState step)
		{
			Initialize();
			Tutorial(step, DuplicateToConsoleByDefault);
		}

		public static void Tutorial(AnalyticsConstants.TutorialState step, bool duplicateToConsole)
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				int step2;
				switch (step)
				{
				case AnalyticsConstants.TutorialState.Started:
					step2 = TutorialState.Start;
					break;
				case AnalyticsConstants.TutorialState.Finished:
					step2 = TutorialState.Finish;
					break;
				default:
					step2 = (int)step;
					break;
				}
				_devToDevFacade.Tutorial(step2);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "TUTORIAL_BUILTIN", Json.Serialize(new Dictionary<string, object> { 
				{
					"step",
					step.ToString()
				} }));
			}
		}

		public static void LevelUp(int level)
		{
			Initialize();
			LevelUp(level, DuplicateToConsoleByDefault);
		}

		public static void LevelUp(int level, bool duplicateToConsole)
		{
			Initialize();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("Coins", Storager.getInt("Coins", false));
			dictionary.Add("GemsCurrency", Storager.getInt("GemsCurrency", false));
			if (_devToDevFacade != null)
			{
				_devToDevFacade.LevelUp(level, dictionary);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "LEVELUP_BUILTIN", Json.Serialize(new Dictionary<string, object>
				{
					{
						"level",
						level.ToString()
					},
					{ "resources", dictionary }
				}));
			}
		}

		public static void CurrencyAccrual(int amount, string currencyName, AnalyticsConstants.AccrualType accrualType = AnalyticsConstants.AccrualType.Earned)
		{
			Initialize();
			CurrencyAccrual(amount, currencyName, accrualType, DuplicateToConsoleByDefault);
		}

		public static void CurrencyAccrual(int amount, string currencyName, AnalyticsConstants.AccrualType accrualType, bool duplicateToConsole)
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				AccrualType accrualType2 = AccrualType.Earned;
				if (accrualType == AnalyticsConstants.AccrualType.Purchased)
				{
					accrualType2 = AccrualType.Purchased;
				}
				_devToDevFacade.CurrencyAccrual(amount, currencyName, accrualType2);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "CURRENCY_ACCRUAL_BUILTIN", Json.Serialize(new Dictionary<string, object>
				{
					{
						"amount",
						amount.ToString()
					},
					{ "currencyName", currencyName },
					{
						"accrualType",
						accrualType.ToString()
					}
				}));
			}
		}

		public static void RealPayment(string paymentId, float inAppPrice, string inAppName, string currencyIsoCode)
		{
			Initialize();
			RealPayment(paymentId, inAppPrice, inAppName, currencyIsoCode, DuplicateToConsoleByDefault);
		}

		public static void RealPayment(string paymentId, float inAppPrice, string inAppName, string currencyIsoCode, bool duplicateToConsole)
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				_devToDevFacade.RealPayment(paymentId, inAppPrice, inAppName, currencyIsoCode);
			}
			Lazy<Dictionary<string, string>> lazy = new Lazy<Dictionary<string, string>>(delegate
			{
				string value = inAppPrice.ToString("0.00", CultureInfo.InvariantCulture);
				return new Dictionary<string, string>(4)
				{
					{ "af_revenue", value },
					{ "af_content_id", inAppName },
					{ "af_currency", currencyIsoCode },
					{ "af_receipt_id", paymentId }
				};
			});
			if (_appsFlyerFacade != null)
			{
				_appsFlyerFacade.TrackRichEvent("af_purchase", lazy.Value);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "REAL_PAYMENT_BUILTIN", Json.Serialize(lazy.Value));
			}
		}

		public static void SendFirstTimeRealPayment(string paymentId, float inAppPrice, string inAppName, string currencyIsoCode)
		{
			Initialize();
			SendFirstTimeRealPayment(paymentId, inAppPrice, inAppName, currencyIsoCode, DuplicateToConsoleByDefault);
		}

		public static void SendFirstTimeRealPayment(string paymentId, float inAppPrice, string inAppName, string currencyIsoCode, bool duplicateToConsole)
		{
			Initialize();
			Lazy<Dictionary<string, string>> lazy = new Lazy<Dictionary<string, string>>(delegate
			{
				string value = inAppPrice.ToString("0.00", CultureInfo.InvariantCulture);
				return new Dictionary<string, string>(4)
				{
					{ "af_revenue", value },
					{ "af_content_id", inAppName },
					{ "af_currency", currencyIsoCode },
					{ "af_receipt_id", paymentId }
				};
			});
			if (_appsFlyerFacade != null)
			{
				_appsFlyerFacade.TrackRichEvent("first_buy", lazy.Value);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "First time real payment", Json.Serialize(lazy.Value));
			}
		}

		public static void InAppPurchase(string purchaseId, string purchaseType, int purchaseAmount, int purchasePrice, string purchaseCurrency)
		{
			Initialize();
			InAppPurchase(purchaseId, purchaseType, purchaseAmount, purchasePrice, purchaseCurrency, DuplicateToConsoleByDefault);
		}

		public static void InAppPurchase(string purchaseId, string purchaseType, int purchaseAmount, int purchasePrice, string purchaseCurrency, bool duplicateToConsole)
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				_devToDevFacade.InAppPurchase(purchaseId, purchaseType, purchaseAmount, purchasePrice, purchaseCurrency);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "IN_APP_PURCHASE_BUILTIN", Json.Serialize(new Dictionary<string, object>
				{
					{ "purchaseId", purchaseId },
					{ "purchaseType", purchaseType },
					{
						"purchaseAmount",
						purchaseAmount.ToString()
					},
					{
						"purchasePrice",
						purchasePrice.ToString()
					},
					{ "purchaseCurrency", purchaseCurrency }
				}));
			}
		}

		private static void InitializeDevToDev(string appKey, string secretKey)
		{
			_devToDevFacade = new DevToDevFacade(appKey, secretKey);
		}

		private static void InitializeAppsFlyer(string appKey, string appId)
		{
			_appsFlyerFacade = new AppsFlyerFacade(appKey, appId);
			_appsFlyerFacade.TrackAppLaunch();
		}

		private static string InitializeSimpleEventFormat()
		{
			return (!Application.isEditor) ? "\"{0}\"" : "<color=magenta>\"{0}\"</color>";
		}

		private static string InitializeParametrizedEventFormat()
		{
			return (!Application.isEditor) ? "\"{0}\": {1}" : "<color=magenta>\"{0}\": {1}</color>";
		}
	}
}
