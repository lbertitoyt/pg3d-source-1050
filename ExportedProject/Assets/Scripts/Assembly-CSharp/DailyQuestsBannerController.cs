using System;
using System.Linq;
using Rilisoft;
using UnityEngine;

public class DailyQuestsBannerController : BannerWindow
{
	public UISprite blockingFon;

	public DailyQuestItem[] DailyQuests;

	public GameObject noQuestsLabel;

	public DailyQuestsButton questsButton;

	public UITable questsTable;

	public UILabel skipHint;

	public bool inBannerSystem = true;

	public static DailyQuestsBannerController Instance;

	private IDisposable _backSubscription;

	private void Awake()
	{
		QuestSystem.Instance.Updated += HandleQuestSystemUpdate;
		Instance = this;
	}

	private void OnDestroy()
	{
		QuestSystem.Instance.Updated -= HandleQuestSystemUpdate;
	}

	private void HandleQuestSystemUpdate(object sender, EventArgs e)
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Refreshing after quest system update.");
		}
		UpdateItems();
	}

	public void OnCloseClick()
	{
		if (inBannerSystem)
		{
			BannerWindowController.SharedController.HideBannerWindow();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnOpenClick()
	{
		base.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		ExpController.LevelUpShown += HandleLevelUpShown;
		UpdateItems();
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(OnCloseClick, "Quest Banner");
	}

	private void OnDisable()
	{
		ExpController.LevelUpShown -= HandleLevelUpShown;
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void Update()
	{
		if (blockingFon != null)
		{
			blockingFon.depth = ((!(ExpController.Instance != null) || !ExpController.Instance.WaitingForLevelUpView) ? 100 : 100000);
		}
	}

	public void UpdateItems()
	{
		QuestProgress questProgress = QuestSystem.Instance.QuestProgress;
		bool flag = TrainingController.TrainingCompleted && questProgress != null && questProgress.GetActiveQuests().Values.Count((QuestBase q) => q != null && !q.Rewarded) > 0;
		bool flag2 = false;
		for (int i = 0; i < DailyQuests.Length; i++)
		{
			DailyQuestItem dailyQuestItem = DailyQuests[i];
			if (flag)
			{
				if (!dailyQuestItem.gameObject.activeSelf)
				{
					dailyQuestItem.gameObject.SetActive(true);
				}
				dailyQuestItem.FillData(i);
			}
			else if (dailyQuestItem.gameObject.activeSelf)
			{
				dailyQuestItem.gameObject.SetActive(false);
			}
			flag2 = flag2 || dailyQuestItem.CanSkip;
		}
		if (skipHint != null)
		{
			skipHint.gameObject.SetActive(flag2);
		}
		QuestSystem.Instance.SaveQuestProgressIfDirty();
		if (flag)
		{
			noQuestsLabel.SetActive(false);
			questsTable.Reposition();
		}
		else
		{
			noQuestsLabel.SetActive(true);
		}
		if (questsButton != null)
		{
			questsButton.CheckUnrewarded();
		}
	}

	private void HandleLevelUpShown()
	{
		if (inBannerSystem)
		{
			if (BannerWindowController.SharedController != null)
			{
				BannerWindowController.SharedController.HideBannerWindowNoShowNext();
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
