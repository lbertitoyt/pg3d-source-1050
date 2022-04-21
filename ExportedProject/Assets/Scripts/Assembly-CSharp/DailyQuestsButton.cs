using System;
using UnityEngine;

public class DailyQuestsButton : MonoBehaviour
{
	public bool inBannerSystem = true;

	public DailyQuestsBannerController controller;

	public GameObject rewardIndicator;

	private void Awake()
	{
		if (inBannerSystem)
		{
			QuestSystem.Instance.Updated += HandleQuestSystemUpdate;
		}
		else if (Defs.isDaterRegim)
		{
			base.gameObject.SetActive(false);
		}
		if (QuestSystem.Instance.QuestProgress != null)
		{
			CheckUnrewarded();
		}
	}

	private void CheckUnrewardedEvent(object sender, EventArgs e)
	{
		CheckUnrewarded();
	}

	public void CheckUnrewarded()
	{
		if (rewardIndicator != null && QuestSystem.Instance.QuestProgress != null)
		{
			bool active = QuestSystem.Instance.QuestProgress.HasUnrewaredAccumQuests();
			rewardIndicator.SetActive(active);
		}
	}

	private void OnDestroy()
	{
		if (inBannerSystem)
		{
			QuestSystem.Instance.Updated -= HandleQuestSystemUpdate;
		}
	}

	private void OnClick()
	{
		ButtonClickSound.TryPlayClick();
		if ((BankController.Instance != null && BankController.Instance.InterfaceEnabled) || (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown) || ShopNGUIController.GuiActive || ExperienceController.sharedController.isShowNextPlashka)
		{
			return;
		}
		if (inBannerSystem)
		{
			BannerWindowController sharedController = BannerWindowController.SharedController;
			if (!(sharedController == null))
			{
				sharedController.ForceShowBanner(BannerWindowType.DailyQuests);
			}
		}
		else if (!LoadingInAfterGame.isShowLoading)
		{
			controller.OnOpenClick();
		}
	}

	private void HandleQuestSystemUpdate(object sender, EventArgs e)
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Refreshing after quest system update.");
		}
		CheckUnrewarded();
	}
}
