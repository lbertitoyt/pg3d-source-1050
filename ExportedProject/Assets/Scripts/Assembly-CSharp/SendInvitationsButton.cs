using UnityEngine;

public class SendInvitationsButton : MonoBehaviour
{
	private void OnClick()
	{
		if (FacebookController.FacebookSupported)
		{
			FacebookController.sharedController.InvitePlayer();
		}
		ButtonClickSound.Instance.PlayClick();
	}
}
