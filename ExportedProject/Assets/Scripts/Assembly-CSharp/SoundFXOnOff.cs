using Rilisoft;
using UnityEngine;

public class SoundFXOnOff : MonoBehaviour
{
	private GameObject soundFX;

	private bool _isWeakdevice;

	private void Start()
	{
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			_isWeakdevice = Device.isWeakDevice;
		}
		else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android || BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
		{
			_isWeakdevice = true;
		}
		else
		{
			_isWeakdevice = Device.IsLoweMemoryDevice;
		}
		if (_isWeakdevice && !Application.isEditor)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		soundFX = base.transform.GetChild(0).gameObject;
		if (Defs.isSoundFX)
		{
			soundFX.SetActive(true);
		}
	}

	private void FixedUpdate()
	{
		if (!_isWeakdevice && soundFX.activeSelf != Defs.isSoundFX)
		{
			soundFX.SetActive(Defs.isSoundFX);
		}
	}
}
