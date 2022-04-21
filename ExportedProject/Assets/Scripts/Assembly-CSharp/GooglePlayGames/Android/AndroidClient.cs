using System;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidClient : IClientImpl
	{
		internal const string BridgeActivityClass = "com.google.games.bridge.NativeBridgeActivity";

		private const string LaunchBridgeMethod = "launchBridgeIntent";

		private const string LaunchBridgeSignature = "(Landroid/app/Activity;Landroid/content/Intent;)V";

		private TokenClient tokenClient;

		public PlatformConfiguration CreatePlatformConfiguration()
		{
			AndroidPlatformConfiguration androidPlatformConfiguration = AndroidPlatformConfiguration.Create();
			using (AndroidJavaObject androidJavaObject = AndroidTokenClient.GetActivity())
			{
				androidPlatformConfiguration.SetActivity(androidJavaObject.GetRawObject());
				androidPlatformConfiguration.SetOptionalIntentHandlerForUI(delegate(IntPtr intent)
				{
					IntPtr intentRef = AndroidJNI.NewGlobalRef(intent);
					PlayGamesHelperObject.RunOnGameThread(delegate
					{
						try
						{
							LaunchBridgeIntent(intentRef);
						}
						finally
						{
							AndroidJNI.DeleteGlobalRef(intentRef);
						}
					});
				});
				return androidPlatformConfiguration;
			}
		}

		public TokenClient CreateTokenClient(string playerId, bool reset)
		{
			if (tokenClient == null || reset)
			{
				tokenClient = new AndroidTokenClient(playerId);
			}
			return tokenClient;
		}

		private static void LaunchBridgeIntent(IntPtr bridgedIntent)
		{
			object[] args = new object[2];
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.games.bridge.NativeBridgeActivity"))
				{
					using (AndroidJavaObject androidJavaObject = AndroidTokenClient.GetActivity())
					{
						IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(androidJavaClass.GetRawClass(), "launchBridgeIntent", "(Landroid/app/Activity;Landroid/content/Intent;)V");
						array[0].l = androidJavaObject.GetRawObject();
						array[1].l = bridgedIntent;
						AndroidJNI.CallStaticVoidMethod(androidJavaClass.GetRawClass(), staticMethodID, array);
					}
				}
			}
			catch (Exception ex)
			{
				GooglePlayGames.OurUtils.Logger.e("Exception launching bridge intent: " + ex.Message);
				GooglePlayGames.OurUtils.Logger.e(ex.ToString());
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}
	}
}
