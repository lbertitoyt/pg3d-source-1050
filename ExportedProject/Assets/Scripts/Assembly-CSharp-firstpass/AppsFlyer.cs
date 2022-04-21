using System;
using System.Collections.Generic;
using UnityEngine;

public class AppsFlyer : MonoBehaviour
{
	private static AndroidJavaClass obj = new AndroidJavaClass("com.appsflyer.AppsFlyerLib");

	private static AndroidJavaObject cls_AppsFlyer = obj.CallStatic<AndroidJavaObject>("getInstance", new object[0]);

	private static AndroidJavaClass cls_AppsFlyerHelper = new AndroidJavaClass("com.appsflyer.AppsFlyerUnityHelper");

	private static string devKey;

	public static void trackEvent(string eventName, string eventValue)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				cls_AppsFlyer.Call("trackEvent", androidJavaObject, eventName, eventValue);
			}
		}
	}

	public static void setCurrencyCode(string currencyCode)
	{
		cls_AppsFlyer.Call("setCurrencyCode", currencyCode);
	}

	public static void setCustomerUserID(string customerUserID)
	{
		cls_AppsFlyer.Call("setAppUserId", customerUserID);
	}

	public static void loadConversionData(string callbackObject, string callbackMethod, string callbackFailedMethod)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				cls_AppsFlyerHelper.CallStatic("createConversionDataListener", androidJavaObject, callbackObject, callbackMethod, callbackFailedMethod);
			}
		}
	}

	public static void setCollectIMEI(bool shouldCollect)
	{
		cls_AppsFlyer.Call("setCollectIMEI", shouldCollect);
	}

	public static void setCollectAndroidID(bool shouldCollect)
	{
		MonoBehaviour.print("AF.cs setCollectAndroidID");
		cls_AppsFlyer.Call("setCollectAndroidID", shouldCollect);
	}

	public static void init(string key)
	{
		MonoBehaviour.print("AF.cs init");
		devKey = key;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				androidJavaObject.Call("runOnUiThread", new AndroidJavaRunnable(init_cb));
			}
		}
	}

	private static void init_cb()
	{
		MonoBehaviour.print("AF.cs init_cb");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				cls_AppsFlyer.Call("init", androidJavaObject, devKey);
			}
		}
	}

	public static void setAppsFlyerKey(string key)
	{
		MonoBehaviour.print("AF.cs setAppsFlyerKey");
		init(key);
	}

	public static void trackAppLaunch()
	{
		MonoBehaviour.print("AF.cs trackAppLaunch");
		trackEvent(null, null);
	}

	public static void setAppID(string packageName)
	{
		cls_AppsFlyer.Call("setAppId", packageName);
	}

	public static void createValidateInAppListener(string aObject, string callbackMethod, string callbackFailedMethod)
	{
		MonoBehaviour.print("AF.cs createValidateInAppListener called");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				cls_AppsFlyerHelper.CallStatic("createValidateInAppListener", androidJavaObject, aObject, callbackMethod, callbackFailedMethod);
			}
		}
	}

	public static void validateReceipt(string publicKey, string purchaseData, string signature, string price, string currency)
	{
		MonoBehaviour.print("AF.cs validateReceipt pk = " + publicKey + " data = " + purchaseData + "sig = " + signature);
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				MonoBehaviour.print("inside cls_activity");
				cls_AppsFlyer.Call("validateAndTrackInAppPurchase", androidJavaObject, publicKey, signature, purchaseData, price, currency, null);
			}
		}
	}

	public static void trackRichEvent(string eventName, Dictionary<string, string> eventValues)
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap"))
		{
			IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
			object[] array = new object[2];
			foreach (KeyValuePair<string, string> eventValue in eventValues)
			{
				using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.String", eventValue.Key))
				{
					using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.String", eventValue.Value))
					{
						array[0] = androidJavaObject2;
						array[1] = androidJavaObject3;
						AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
					}
				}
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject4 = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					cls_AppsFlyer.Call("trackEvent", androidJavaObject4, eventName, androidJavaObject);
				}
			}
		}
	}

	public static void setImeiData(string imeiData)
	{
		MonoBehaviour.print("AF.cs setImeiData");
		cls_AppsFlyer.Call("setImeiData", imeiData);
	}

	public static void setAndroidIdData(string androidIdData)
	{
		MonoBehaviour.print("AF.cs setImeiData");
		cls_AppsFlyer.Call("setAndroidIdData", androidIdData);
	}

	public static void setIsDebug(bool isDebug)
	{
		MonoBehaviour.print("AF.cs setDebugLog");
		cls_AppsFlyer.Call("setDebugLog", isDebug);
	}

	public static void setIsSandbox(bool isSandbox)
	{
	}

	public static void getConversionData()
	{
	}

	public static void handleOpenUrl(string url, string sourceApplication, string annotation)
	{
	}

	public static string getAppsFlyerId()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				return cls_AppsFlyer.Call<string>("getAppsFlyerUID", new object[1] { androidJavaObject });
			}
		}
	}
}
