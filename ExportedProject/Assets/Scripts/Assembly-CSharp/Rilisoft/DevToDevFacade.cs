using System;
using System.Collections.Generic;
using DevToDev;

namespace Rilisoft
{
	internal sealed class DevToDevFacade
	{
		private readonly string _appKey;

		public static string Version
		{
			get
			{
				return Analytics.SDKVersion;
			}
		}

		public bool UserIsCheater
		{
			set
			{
				if (!string.IsNullOrEmpty(_appKey))
				{
				}
			}
		}

		public static bool LoggingEnabled
		{
			set
			{
				Analytics.SetActiveLog(value);
			}
		}

		public DevToDevFacade(string appKey, string secretKey)
		{
			if (appKey == null)
			{
				throw new ArgumentNullException("appKey");
			}
			if (secretKey == null)
			{
				throw new ArgumentNullException("secretKey");
			}
			_appKey = appKey;
			Analytics.Initialize(appKey, secretKey);
		}

		public void SendCustomEvent(string eventName)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("Event name must not be empty.", "eventName");
			}
			if (!string.IsNullOrEmpty(_appKey))
			{
				Analytics.CustomEvent(eventName);
			}
		}

		public void InAppPurchase(string purchaseId, string purchaseType, int purchaseAmount, int purchasePrice, string purchaseCurrency)
		{
			purchaseId = purchaseId.Substring(0, Math.Min(purchaseId.Length, 32));
			purchaseType = purchaseType.Substring(0, Math.Min(purchaseType.Length, 96));
			Analytics.InAppPurchase(purchaseId, purchaseType, purchaseAmount, purchasePrice, purchaseCurrency);
		}

		public void RealPayment(string paymentId, float inAppPrice, string inAppName, string inAppCurrencyISOCode)
		{
			Analytics.RealPayment(paymentId, inAppPrice, inAppName, inAppCurrencyISOCode);
		}

		public void CurrencyAccrual(int amount, string currencyName, AccrualType accrualType)
		{
			Analytics.CurrencyAccrual(amount, currencyName, accrualType);
		}

		public void LevelUp(int level, Dictionary<string, int> resources)
		{
			Analytics.LevelUp(level, resources);
		}

		public void Tutorial(int step)
		{
			Analytics.Tutorial(step);
		}

		public void SendCustomEvent(string eventName, IDictionary<string, object> eventParams)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("Event name must not be empty.", "eventName");
			}
			if (string.IsNullOrEmpty(_appKey))
			{
				return;
			}
			CustomEventParams customEventParams = new CustomEventParams();
			foreach (KeyValuePair<string, object> eventParam in eventParams)
			{
				string text = eventParam.Value as string;
				if (text != null)
				{
					customEventParams.AddParam(eventParam.Key, text);
				}
				else if (eventParam.Value is int)
				{
					customEventParams.AddParam(eventParam.Key, (int)eventParam.Value);
				}
				else if (eventParam.Value is long)
				{
					customEventParams.AddParam(eventParam.Key, (long)eventParam.Value);
				}
				else if (eventParam.Value is float || eventParam.Value is double)
				{
					customEventParams.AddParam(eventParam.Key, Convert.ToDouble(eventParam.Value));
				}
				else if (eventParam.Value is DateTimeOffset)
				{
					customEventParams.AddParam(eventParam.Key, (DateTimeOffset)eventParam.Value);
				}
				else
				{
					customEventParams.AddParam(eventParam.Key, Convert.ToString(eventParam.Value));
				}
			}
			Analytics.CustomEvent(eventName, customEventParams);
		}

		public void SendBufferedEvents()
		{
			if (!string.IsNullOrEmpty(_appKey))
			{
				Analytics.SendBufferedEvents();
			}
		}
	}
}
