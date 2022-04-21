using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Rilisoft.NullExtensions;
using UnityEngine;

public class FreeAwardShowHandler : MonoBehaviour
{
	private enum SkipReason
	{
		None,
		CameraTouchOverGui,
		FriendsInterfaceEnabled,
		BankInterfaceEnabled,
		ShopInterfaceEnabled,
		RewardedVideoInterfaceEnabled,
		BannerEnabled,
		MainMenuComponentEnabled,
		LeaderboardEnabled,
		ProfileEnabled,
		NewsEnabled,
		LevelUpShown,
		AskNameWindow
	}

	public GameObject chestModelCoins;

	public GameObject chestModelGems;

	public GameObject freeAwardGuiPrefab;

	public LeaderboardsView leaderboardsView;

	public LeaderboardsView leaderboardsViewOld;

	public static FreeAwardShowHandler Instance;

	private NickLabelController nickController;

	private bool clicked;

	private bool inside;

	public bool IsInteractable
	{
		get
		{
			return base.gameObject.GetComponent<Collider>() != null && base.gameObject.GetComponent<Collider>().enabled;
		}
		set
		{
			if (base.gameObject.GetComponent<Collider>() != null)
			{
				base.gameObject.GetComponent<Collider>().enabled = value;
			}
		}
	}

	private void Awake()
	{
		Instance = this;
		if (FreeAwardController.Instance == null && freeAwardGuiPrefab != null)
		{
			UnityEngine.Object @object = UnityEngine.Object.Instantiate(freeAwardGuiPrefab, Vector3.zero, Quaternion.identity);
		}
		LocalizationManager.OnLocalizeEvent += OnLocalizationChanged;
	}

	private void OnDestroy()
	{
		LocalizationManager.OnLocalizeEvent -= OnLocalizationChanged;
		Instance = null;
	}

	private void OnLocalizationChanged()
	{
		SetFreeAwardLocalization();
	}

	private NickLabelController GetNickController()
	{
		return (!(MainMenuController.sharedController != null)) ? null : MainMenuController.sharedController.persNickLabel;
	}

	private bool NeedToSkip()
	{
		SkipReason skipReason = NeedToSkipCore();
		if (Defs.IsDeveloperBuild && skipReason != 0)
		{
			Debug.Log("Skipping free award chest: " + skipReason);
		}
		return skipReason != SkipReason.None;
	}

	private SkipReason NeedToSkipCore()
	{
		if (UICamera.currentTouch.Map((UICamera.MouseOrTouch t) => t.isOverUI))
		{
			return SkipReason.CameraTouchOverGui;
		}
		if (FriendsWindowGUI.Instance.InterfaceEnabled)
		{
			return SkipReason.FriendsInterfaceEnabled;
		}
		if (BankController.Instance != null && BankController.Instance.InterfaceEnabled)
		{
			return SkipReason.BankInterfaceEnabled;
		}
		if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
		{
			return SkipReason.ShopInterfaceEnabled;
		}
		if (FreeAwardController.Instance != null && !FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>())
		{
			return SkipReason.RewardedVideoInterfaceEnabled;
		}
		if (BannerWindowController.SharedController != null && BannerWindowController.SharedController.IsAnyBannerShown)
		{
			return SkipReason.BannerEnabled;
		}
		if (AskNameManager.instance != null && !AskNameManager.isComplete)
		{
			return SkipReason.AskNameWindow;
		}
		MainMenuController sharedController = MainMenuController.sharedController;
		if (sharedController != null && (sharedController.RentExpiredPoint.Map((Transform r) => r.childCount > 0) || sharedController.SettingsJoysticksPanel.activeSelf || sharedController.supportPanel.activeSelf || sharedController.settingsPanel.activeSelf || sharedController.freePanel.activeSelf || sharedController.singleModePanel.activeSelf))
		{
			return SkipReason.MainMenuComponentEnabled;
		}
		if (leaderboardsView.Map((LeaderboardsView l) => l.isActiveAndEnabled))
		{
			return SkipReason.LeaderboardEnabled;
		}
		if (leaderboardsViewOld.Map((LeaderboardsView l) => l.isActiveAndEnabled))
		{
			return SkipReason.LeaderboardEnabled;
		}
		if (FriendsController.sharedController.Map((FriendsController c) => c.ProfileInterfaceActive))
		{
			return SkipReason.ProfileEnabled;
		}
		if (NewsLobbyController.sharedController != null && NewsLobbyController.sharedController.gameObject.activeSelf)
		{
			return SkipReason.NewsEnabled;
		}
		if (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown)
		{
			return SkipReason.LevelUpShown;
		}
		return SkipReason.None;
	}

	private void OnMouseDown()
	{
		clicked = true;
		inside = true;
	}

	private void OnMouseExit()
	{
		inside = false;
	}

	private void OnMouseEnter()
	{
		if (clicked)
		{
			inside = true;
		}
	}

	private void OnMouseUp()
	{
		clicked = false;
		if (!inside || NeedToSkip())
		{
			return;
		}
		inside = false;
		if (!FreeAwardController.Instance.AdvertCountLessThanLimit())
		{
			return;
		}
		List<double> list = ((!MobileAdManager.IsPayingUser()) ? PromoActionsManager.MobileAdvert.RewardedVideoDelayMinutesNonpaying : PromoActionsManager.MobileAdvert.RewardedVideoDelayMinutesPaying);
		if (list.Count == 0)
		{
			return;
		}
		DateTime date = DateTime.UtcNow.Date;
		KeyValuePair<int, DateTime> keyValuePair = FreeAwardController.Instance.LastAdvertShow(date);
		int num = Math.Max(0, keyValuePair.Key + 1);
		if (num <= list.Count)
		{
			DateTime dateTime = ((!(keyValuePair.Value < date)) ? keyValuePair.Value : date);
			TimeSpan timeSpan = TimeSpan.FromMinutes(list[num]);
			DateTime watchState = dateTime + timeSpan;
			FreeAwardController.Instance.SetWatchState(watchState);
			if (ButtonClickSound.Instance != null)
			{
				ButtonClickSound.Instance.PlayClick();
			}
		}
	}

	private void OnEnable()
	{
		StartCoroutine(ShowChestCoroutine());
	}

	private void PlayeAnimationTitle(bool isReverse, GameObject titleLabel)
	{
		UIPlayTween component = titleLabel.GetComponent<UIPlayTween>();
		component.resetOnPlay = true;
		component.tweenGroup = (isReverse ? 1 : 0);
		component.Play(true);
	}

	private void CheckShowFreeAwardTitle(bool isEnable, bool needExitAnim = false)
	{
		if (nickController == null)
		{
			nickController = GetNickController();
		}
		if (isEnable && nickController != null)
		{
			SetFreeAwardLocalization();
			nickController.freeAwardTitle.gameObject.SetActive(true);
			PlayeAnimationTitle(false, nickController.freeAwardTitle);
		}
		else if (needExitAnim)
		{
			PlayeAnimationTitle(true, nickController.freeAwardTitle);
		}
	}

	private void SetFreeAwardLocalization()
	{
		if (nickController == null)
		{
			nickController = GetNickController();
		}
		if (!(nickController == null))
		{
			UILabel uILabel = nickController.freeAwardTitle.GetComponent<UILabel>() ?? nickController.freeAwardTitle.GetComponentInChildren<UILabel>();
			uILabel.text = ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? ScriptLocalization.Get("Key_1155") : ScriptLocalization.Get("Key_2046"));
		}
	}

	private IEnumerator ShowChestCoroutine()
	{
		yield return null;
		PlayAnimationShowChest(false);
		chestModelCoins.SetActive(FreeAwardController.Instance.CurrencyForAward == "Coins");
		chestModelGems.SetActive(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency");
		CheckShowFreeAwardTitle(true);
	}

	private IEnumerator HideChestCoroutine()
	{
		PlayAnimationShowChest(true);
		CheckShowFreeAwardTitle(false, true);
		GameObject obj = ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? chestModelCoins : chestModelGems);
		Animation animationSystem = obj.GetComponent<Animation>();
		if (animationSystem != null)
		{
			yield return new WaitForSeconds(animationSystem["Animate"].length);
		}
		base.gameObject.SetActive(false);
	}

	private void PlayAnimationShowChest(bool isReserse)
	{
		GameObject gameObject = ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? chestModelCoins : chestModelGems);
		Animation component = gameObject.GetComponent<Animation>();
		if (!(component == null))
		{
			if (isReserse)
			{
				component["Animate"].speed = -1f;
				component["Animate"].time = component["Animate"].length;
			}
			else
			{
				component["Animate"].speed = 1f;
				component["Animate"].time = 0f;
			}
			component.Play();
		}
	}

	public void HideChestWithAnimation()
	{
		StartCoroutine(HideChestCoroutine());
	}

	public void HideChestTitle()
	{
		if (!(nickController == null))
		{
			nickController.freeAwardTitle.gameObject.SetActive(false);
		}
	}

	public static void CheckShowChest(bool interfaceEnabled)
	{
		if (!(Instance == null) && interfaceEnabled && Instance.gameObject.activeSelf)
		{
			Instance.HideChestTitle();
			Instance.gameObject.SetActive(false);
		}
	}
}
