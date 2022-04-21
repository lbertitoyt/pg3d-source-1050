using System;
using System.Collections.Generic;
using I2.Loc;
using Rilisoft;
using UnityEngine;

internal sealed class FreeAwardView : MonoBehaviour
{
	public GameObject backgroundPanel;

	public GameObject waitingPanel;

	public GameObject watchForCoinsPanel;

	public GameObject watchForGemsPanel;

	public GameObject connectionPanel;

	public GameObject awardPanelCoins;

	public GameObject awardPanelGems;

	public GameObject closePanel;

	public UILabel watchHeader;

	public UILabel watchTimer;

	public UIButton nguiWatchButton;

	public UIButton devSkipButton;

	public UILabel prizeMoneyLabel;

	public UISprite currencySprite;

	public UITexture loadingSpinner;

	public UILabel awardOuterLabelCoins;

	public UILabel awardOuterLabelGems;

	private FreeAwardController.State _currentState;

	private readonly System.Lazy<UILabel[]> _watchTimerLabels;

	internal FreeAwardController.State CurrentState
	{
		private get
		{
			return _currentState;
		}
		set
		{
			if (value != _currentState)
			{
				FreeAwardController.WatchState watchState = value as FreeAwardController.WatchState;
				if (watchState != null)
				{
					TimeSpan estimatedTimeSpan = watchState.GetEstimatedTimeSpan();
					SetWatchButtonEnabled(estimatedTimeSpan <= TimeSpan.FromMinutes(0.0), estimatedTimeSpan);
				}
				else
				{
					SetWatchButtonEnabled(false);
				}
				RefreshAwardLabel(watchState != null);
			}
			if (backgroundPanel != null)
			{
				backgroundPanel.SetActive(!(value is FreeAwardController.IdleState));
			}
			if (waitingPanel != null)
			{
				waitingPanel.SetActive(value is FreeAwardController.WaitingState);
			}
			if (connectionPanel != null)
			{
				connectionPanel.SetActive(value is FreeAwardController.ConnectionState);
			}
			if (closePanel != null)
			{
				closePanel.SetActive(value is FreeAwardController.CloseState);
			}
			if (value is FreeAwardController.WatchState)
			{
				if (FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")
				{
					watchForGemsPanel.SetActive(true);
					watchForCoinsPanel.SetActive(false);
				}
				else
				{
					watchForGemsPanel.SetActive(false);
					watchForCoinsPanel.SetActive(true);
				}
			}
			else
			{
				watchForGemsPanel.SetActive(false);
				watchForCoinsPanel.SetActive(false);
			}
			if (value is FreeAwardController.AwardState)
			{
				if (FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")
				{
					awardPanelGems.SetActive(true);
				}
				else
				{
					awardPanelCoins.SetActive(true);
				}
			}
			else
			{
				awardPanelCoins.SetActive(false);
				awardPanelGems.SetActive(false);
			}
			_currentState = value;
		}
	}

	public FreeAwardView()
	{
		_watchTimerLabels = new System.Lazy<UILabel[]>(InitializeWatchTimerLabels);
	}

	private void RefreshAwardLabel(bool visible)
	{
		if (!visible)
		{
			return;
		}
		string text = LocalizationStore.Get(ScriptLocalization.Key_0291);
		if (PromoActionsManager.MobileAdvert == null)
		{
			text += " 1";
		}
		else
		{
			int num = ((!MobileAdManager.IsPayingUser()) ? PromoActionsManager.MobileAdvert.AwardCoinsNonpaying : PromoActionsManager.MobileAdvert.AwardCoinsPaying);
			text += ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? num.ToString() : string.Format(" [c][50CEFFFF]{0}[-][/c]", num));
		}
		List<UILabel> list = new List<UILabel>();
		list.AddRange(awardOuterLabelCoins.gameObject.GetComponentsInChildren<UILabel>(true));
		list.AddRange(awardOuterLabelGems.gameObject.GetComponentsInChildren<UILabel>(true));
		foreach (UILabel item in list)
		{
			item.text = text;
		}
		currencySprite.spriteName = ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? "ingame_coin" : "gem_znachek");
	}

	private void Start()
	{
		if (devSkipButton != null)
		{
			devSkipButton.gameObject.SetActive(Application.isEditor || (Defs.IsDeveloperBuild && BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64));
		}
	}

	private void Update()
	{
		FreeAwardController.WaitingState waitingState = CurrentState as FreeAwardController.WaitingState;
		if (waitingState != null && loadingSpinner != null)
		{
			float num = Time.realtimeSinceStartup - waitingState.StartTime;
			int num2 = Convert.ToInt32(Mathf.Floor(num));
			loadingSpinner.invert = num2 % 2 == 0;
			loadingSpinner.fillAmount = ((!loadingSpinner.invert) ? (1f - num + (float)num2) : (num - (float)num2));
		}
		FreeAwardController.WatchState watchState = CurrentState as FreeAwardController.WatchState;
		if (watchState != null && Time.frameCount % 10 == 0)
		{
			TimeSpan estimatedTimeSpan = watchState.GetEstimatedTimeSpan();
			SetWatchButtonEnabled(estimatedTimeSpan <= TimeSpan.FromMinutes(0.0), estimatedTimeSpan);
		}
	}

	private void SetWatchButtonEnabled(bool enabled, TimeSpan nextTimeAwailable)
	{
		if (nguiWatchButton != null)
		{
			nguiWatchButton.isEnabled = enabled;
		}
		if (watchHeader != null)
		{
			watchHeader.gameObject.SetActive(enabled);
		}
		if (!(watchTimer != null))
		{
			return;
		}
		watchTimer.transform.parent.gameObject.SetActive(!enabled);
		if (!enabled)
		{
			string text = ((nextTimeAwailable.Hours <= 0) ? string.Format("{0}:{1:D2}", nextTimeAwailable.Minutes, nextTimeAwailable.Seconds) : string.Format("{0}:{1:D2}:{2:D2}", nextTimeAwailable.Hours, nextTimeAwailable.Minutes, nextTimeAwailable.Seconds));
			UILabel[] value = _watchTimerLabels.Value;
			foreach (UILabel uILabel in value)
			{
				uILabel.text = text;
			}
		}
	}

	private void SetWatchButtonEnabled(bool enabled)
	{
		SetWatchButtonEnabled(enabled, default(TimeSpan));
	}

	private UILabel[] InitializeWatchTimerLabels()
	{
		if (watchTimer == null)
		{
			return new UILabel[0];
		}
		List<UILabel> list = new List<UILabel>(3);
		list.Add(watchTimer);
		List<UILabel> list2 = list;
		watchTimer.GetComponentsInChildren(true, list2);
		return list2.ToArray();
	}
}
