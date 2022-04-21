public class BtnBannerSocialGun : ButtonBannerBase
{
	public override bool BannerIsActive()
	{
		return false;
	}

	public override void OnClickButton()
	{
		MainMenuController.sharedController.OnSocialGunEventButtonClick();
	}

	public override void OnHide()
	{
	}

	public override void OnShow()
	{
	}

	public override void OnChangeLocalize()
	{
	}
}
