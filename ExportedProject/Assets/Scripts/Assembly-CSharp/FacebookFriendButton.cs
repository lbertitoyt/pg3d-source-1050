using Rilisoft;
using UnityEngine;

public class FacebookFriendButton : MonoBehaviour
{
	private void Start()
	{
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnClick()
	{
		ButtonClickSound.Instance.PlayClick();
	}
}
