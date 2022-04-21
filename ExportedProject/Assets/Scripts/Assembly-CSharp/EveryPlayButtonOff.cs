using Rilisoft;
using UnityEngine;

public class EveryPlayButtonOff : MonoBehaviour
{
	private void Start()
	{
		if ((BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon) || BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
		{
			base.gameObject.SetActive(false);
		}
	}
}
