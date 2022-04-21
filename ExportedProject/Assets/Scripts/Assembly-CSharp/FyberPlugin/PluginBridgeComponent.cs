using UnityEngine;

namespace FyberPlugin
{
	internal class PluginBridgeComponent : IPluginBridge
	{
		static PluginBridgeComponent()
		{
			FyberGameObject.Init();
		}

		public void StartSDK(string json)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.fyber.mediation.MediationAdapterStarter"))
			{
				FyberSettings instance = FyberSettings.Instance;
				androidJavaClass.CallStatic("setup", instance.BundlesInfoJson(), instance.BundlesCount());
			}
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.fyber.mediation.MediationConfigProvider"))
			{
				FyberSettings instance2 = FyberSettings.Instance;
				androidJavaClass2.CallStatic("setup", instance2.BundlesConfigJson());
			}
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.FyberPlugin"))
			{
				androidJavaObject.CallStatic("setPluginParameters", "8.1.4", Application.unityVersion);
				androidJavaObject.CallStatic("start", json);
			}
		}

		public void Cache(string action)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.cache.CacheWrapper"))
			{
				androidJavaObject.CallStatic(action);
			}
		}

		public void Request(string json)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.requesters.RequesterWrapper"))
			{
				androidJavaObject.CallStatic("request", json);
			}
		}

		public void StartAd(string json)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.ads.AdWrapper"))
			{
				androidJavaObject.CallStatic("start", json);
			}
		}

		public void Report(string json)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.reporters.ReporterWrapper"))
			{
				androidJavaObject.CallStatic("report", json);
			}
		}

		public void Settings(string json)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.settings.SettingsWrapper"))
			{
				androidJavaObject.CallStatic("perform", json);
			}
		}

		public void EnableLogging(bool shouldLog)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.utils.FyberLogger"))
			{
				androidJavaObject.CallStatic<bool>("enableLogging", new object[1] { shouldLog });
			}
		}
	}
}
