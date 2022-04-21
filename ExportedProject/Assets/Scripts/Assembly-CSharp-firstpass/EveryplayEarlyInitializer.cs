using UnityEngine;

public class EveryplayEarlyInitializer : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod]
	private static void InitializeEveryplayOnStartup()
	{
		EveryplaySettings everyplaySettings = (EveryplaySettings)Resources.Load("EveryplaySettings");
		if (everyplaySettings != null && everyplaySettings.earlyInitializerEnabled && everyplaySettings.IsEnabled && everyplaySettings.IsValid)
		{
			Everyplay.Initialize();
		}
	}
}
