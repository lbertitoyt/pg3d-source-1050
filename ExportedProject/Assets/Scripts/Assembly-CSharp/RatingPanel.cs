using System;
using Rilisoft;
using UnityEngine;

public class RatingPanel : MonoBehaviour
{
	public GameObject leaguePanel;

	public UISprite cup;

	public UILabel leagueLabel;

	public UILabel ratingLabel;

	[SerializeField]
	private ButtonHandler _btnOpenProfile;

	private void OnEnable()
	{
		UpdateInfo();
		if (FriendsController.isUseRatingSystem)
		{
			RatingSystem instance = RatingSystem.instance;
			instance.OnRatingUpdate = (RatingSystem.RatingUpdate)Delegate.Combine(instance.OnRatingUpdate, new RatingSystem.RatingUpdate(UpdateInfo));
		}
	}

	private void UpdateInfo()
	{
		if (!FriendsController.isUseRatingSystem || !TrainingController.TrainingCompleted)
		{
			leaguePanel.SetActive(false);
			return;
		}
		leaguePanel.SetActive(true);
		cup.spriteName = RatingSystem.instance.currentLeague.ToString() + " " + (3 - RatingSystem.instance.currentDivision);
		if (RatingSystem.instance.currentLeague != RatingSystem.RatingLeague.Adamant)
		{
			leagueLabel.text = LocalizationStore.Get(RatingSystem.leagueLocalizations[(int)RatingSystem.instance.currentLeague]) + " " + RatingSystem.divisionByIndex[RatingSystem.instance.currentDivision];
		}
		else
		{
			leagueLabel.text = LocalizationStore.Get(RatingSystem.leagueLocalizations[(int)RatingSystem.instance.currentLeague]);
		}
		ratingLabel.text = RatingSystem.instance.currentRating.ToString();
		if (_btnOpenProfile != null)
		{
			_btnOpenProfile.Clicked += OnBtnOpenProfileClicked;
		}
	}

	private void OnDisable()
	{
		if (_btnOpenProfile != null)
		{
			_btnOpenProfile.Clicked -= OnBtnOpenProfileClicked;
		}
		if (FriendsController.isUseRatingSystem)
		{
			RatingSystem instance = RatingSystem.instance;
			instance.OnRatingUpdate = (RatingSystem.RatingUpdate)Delegate.Remove(instance.OnRatingUpdate, new RatingSystem.RatingUpdate(UpdateInfo));
		}
	}

	private void OnBtnOpenProfileClicked(object sender, EventArgs e)
	{
		if (MainMenuController.sharedController != null)
		{
			MainMenuController.sharedController.GoToProfile();
		}
		if (ProfileController.Instance != null)
		{
			ProfileController.Instance.SetStaticticTab(StatisticHUD.TypeOpenTab.leagues);
		}
	}
}
