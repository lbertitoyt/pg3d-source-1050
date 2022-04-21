using UnityEngine;

public class loaddex
{
	private static AndroidJavaObject _plugin;

	static loaddex()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.appodeal.loaddex.LoadDex"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static void loadDex()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				androidJavaObject.Call("runOnUiThread", new AndroidJavaRunnable(load));
			}
		}
	}

	private static void load()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("loadDex");
		}
	}
}
