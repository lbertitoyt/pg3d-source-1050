using Rilisoft;
using UnityEngine;

public class EnderButton : MonoBehaviour
{
	private void Start()
	{
		if ((BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer && (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android || Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)) || !Defs.EnderManAvailable)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
	}
}
