using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class BankController : MonoBehaviour
{
	public const int InitialIosGems = 0;

	public const int InitialIosCoins = 0;

	public BankView bankView_AB_NoGrid_Common;

	public BankView bankView_AB_NoGrid_X3;

	public BankView bankViewCommon;

	public BankView bankViewX3;

	public GameObject uiRoot;

	public ChestBonusView bonusDetailView;

	public static bool canShowIndication = true;

	private BankView _bankViewCurrent;

	private bool firsEnterToBankOccured;

	private float tmOfFirstEnterTheBank;

	private bool _lockInterfaceEnabledCoroutine;

	private int _counterEn;

	private IDisposable _backSubscription;

	private readonly System.Lazy<bool> _timeTamperingDetected = new System.Lazy<bool>(delegate
	{
		bool flag = FreeAwardController.Instance.TimeTamperingDetected();
		if (flag)
		{
		}
		return flag;
	});

	private static float _lastTimePurchaseButtonPressed;

	private bool _debugOptionsEnabled;

	private string _debugMessage = string.Empty;

	private bool _escapePressed;

	private static BankController _instance;

	public static bool ABTestStaticBank { get; private set; }

	public BankView bankView
	{
		get
		{
			if (PromoActionsManager.sharedManager == null)
			{
				Debug.LogWarning("PromoActionsManager.sharedManager == null");
				return (!ABTestStaticBank) ? bankViewCommon : bankView_AB_NoGrid_Common;
			}
			return PromoActionsManager.sharedManager.IsEventX3Active ? ((!ABTestStaticBank) ? bankViewX3 : bankView_AB_NoGrid_X3) : ((!ABTestStaticBank) ? bankViewCommon : bankView_AB_NoGrid_Common);
		}
	}

	public bool InterfaceEnabled
	{
		get
		{
			return bankView != null && bankView.interfaceHolder != null && bankView.interfaceHolder.gameObject.activeInHierarchy;
		}
		set
		{
			if (value)
			{
				ABTestStaticBank = FriendsController.isShowStaticBank;
			}
			StartCoroutine(InterfaceEnabledCoroutine(value));
		}
	}

	public bool InterfaceEnabledCoroutineLocked
	{
		get
		{
			return _lockInterfaceEnabledCoroutine;
		}
	}

	public static BankController Instance
	{
		get
		{
			return _instance;
		}
	}

	public static event Action onUpdateMoney;

	public event EventHandler BackRequested
	{
		add
		{
			if (bankViewCommon != null)
			{
				bankViewCommon.BackButtonPressed += value;
			}
			if (bankViewX3 != null)
			{
				bankViewX3.BackButtonPressed += value;
			}
			if (bankView_AB_NoGrid_Common != null)
			{
				bankView_AB_NoGrid_Common.BackButtonPressed += value;
			}
			if (bankView_AB_NoGrid_X3 != null)
			{
				bankView_AB_NoGrid_X3.BackButtonPressed += value;
			}
			this.EscapePressed = (EventHandler)Delegate.Combine(this.EscapePressed, value);
		}
		remove
		{
			if (bankViewCommon != null)
			{
				bankViewCommon.BackButtonPressed -= value;
			}
			if (bankViewX3 != null)
			{
				bankViewX3.BackButtonPressed -= value;
			}
			if (bankView_AB_NoGrid_Common != null)
			{
				bankView_AB_NoGrid_Common.BackButtonPressed -= value;
			}
			if (bankView_AB_NoGrid_X3 != null)
			{
				bankView_AB_NoGrid_X3.BackButtonPressed -= value;
			}
			this.EscapePressed = (EventHandler)Delegate.Remove(this.EscapePressed, value);
		}
	}

	private event EventHandler EscapePressed;

	public static void UpdateAllIndicatorsMoney()
	{
		if (BankController.onUpdateMoney != null)
		{
			BankController.onUpdateMoney();
		}
	}

	public static void GiveInitialNumOfCoins()
	{
		if (!Storager.hasKey("Coins"))
		{
			Storager.setInt("Coins", 0, false);
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				PlayerPrefs.Save();
			}
		}
		if (!Storager.hasKey("GemsCurrency"))
		{
			switch (BuildSettings.BuildTargetPlatform)
			{
			case RuntimePlatform.IPhonePlayer:
				Storager.setInt("GemsCurrency", 0, false);
				break;
			case RuntimePlatform.Android:
				Storager.setInt("GemsCurrency", 0, false);
				break;
			}
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				PlayerPrefs.Save();
			}
		}
	}

	private IEnumerator InterfaceEnabledCoroutine(bool value)
	{
		if (!value && _backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
		_lockInterfaceEnabledCoroutine = true;
		int cnt = _counterEn++;
		Debug.Log("InterfaceEnabledCoroutine " + cnt + " start: " + value);
		try
		{
			if (value && !firsEnterToBankOccured)
			{
				firsEnterToBankOccured = true;
				tmOfFirstEnterTheBank = Time.realtimeSinceStartup;
			}
			if (!value)
			{
				yield return null;
				yield return null;
			}
			if (bankView != _bankViewCurrent && _bankViewCurrent != null && _bankViewCurrent.interfaceHolder != null)
			{
				_bankViewCurrent.interfaceHolder.gameObject.SetActive(false);
				_bankViewCurrent = null;
			}
			if (bankView != null && bankView.interfaceHolder != null)
			{
				bankView.interfaceHolder.gameObject.SetActive(value);
				_bankViewCurrent = ((!value) ? null : bankView);
				if (value)
				{
					bankView.LoadCurrencyIcons(value);
				}
			}
			uiRoot.SetActive(value);
			if (!value)
			{
				ActivityIndicator.IsActiveIndicator = false;
				bankViewCommon.LoadCurrencyIcons(value);
				bankViewX3.LoadCurrencyIcons(value);
				bankView_AB_NoGrid_Common.LoadCurrencyIcons(value);
				bankView_AB_NoGrid_X3.LoadCurrencyIcons(value);
			}
			FreeAwardShowHandler.CheckShowChest(value);
			if (value)
			{
				coinsShop.thisScript.RefreshProductsIfNeed();
				OnEventX3AmazonBonusUpdated();
			}
		}
		finally
		{
			if (value)
			{
				if (_backSubscription != null)
				{
					_backSubscription.Dispose();
				}
				_backSubscription = BackSystem.Instance.Register(HandleEscape, "Bank");
			}
			if (Device.IsLoweMemoryDevice)
			{
				ActivityIndicator.ClearMemory();
			}
			_lockInterfaceEnabledCoroutine = false;
			Debug.Log("InterfaceEnabledCoroutine " + cnt + " stop: " + value);
		}
	}

	private void HandleEscape()
	{
		if (FreeAwardController.Instance != null && !FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>())
		{
			FreeAwardController.Instance.HandleClose();
			_escapePressed = false;
		}
		else
		{
			_escapePressed = true;
		}
	}

	private void Awake()
	{
		GiveInitialNumOfCoins();
	}

	private void Start()
	{
		_instance = this;
		PromoActionsManager.EventX3Updated += OnEventX3Updated;
		if (bankViewCommon != null)
		{
			bankViewCommon.PurchaseButtonPressed += HandlePurchaseButtonPressed;
		}
		if (bankViewX3 != null)
		{
			bankViewX3.PurchaseButtonPressed += HandlePurchaseButtonPressed;
		}
		if (bankView_AB_NoGrid_Common != null)
		{
			bankView_AB_NoGrid_Common.PurchaseButtonPressed += HandlePurchaseButtonPressed;
		}
		if (bankView_AB_NoGrid_X3 != null)
		{
			bankView_AB_NoGrid_X3.PurchaseButtonPressed += HandlePurchaseButtonPressed;
		}
		PromoActionsManager.EventAmazonX3Updated += OnEventX3AmazonBonusUpdated;
		HashSet<string> hashSet = new HashSet<string>();
		hashSet.Add("7FFC6ACA-F568-46C3-86AD-8A4FA2DF4401");
		HashSet<string> hashSet2 = hashSet;
		_debugOptionsEnabled = hashSet2.Contains(SystemInfo.deviceUniqueIdentifier);
		bankView.freeAwardButton.gameObject.SetActive(false);
	}

	private void OnEventX3Updated()
	{
		if (_bankViewCurrent != null)
		{
			StartCoroutine(InterfaceEnabledCoroutine(true));
		}
	}

	private void OnEventX3AmazonBonusUpdated()
	{
		if (_bankViewCurrent == null || _bankViewCurrent.eventX3AmazonBonusWidget == null)
		{
			return;
		}
		GameObject gameObject = _bankViewCurrent.eventX3AmazonBonusWidget.gameObject;
		gameObject.SetActive(PromoActionsManager.sharedManager.IsAmazonEventX3Active);
		UILabel[] componentsInChildren = gameObject.GetComponentsInChildren<UILabel>();
		UILabel uILabel = bankView.Map((BankView b) => b.amazonEventCaptionLabel) ?? componentsInChildren.FirstOrDefault((UILabel l) => "CaptionLabel".Equals(l.name, StringComparison.OrdinalIgnoreCase));
		PromoActionsManager.AmazonEventInfo o = PromoActionsManager.sharedManager.Map((PromoActionsManager p) => p.AmazonEvent);
		if (uILabel != null)
		{
			uILabel.text = o.Map((PromoActionsManager.AmazonEventInfo e) => e.Caption) ?? string.Empty;
		}
		UILabel o2 = bankView.Map((BankView b) => b.amazonEventTitleLabel) ?? componentsInChildren.FirstOrDefault((UILabel l) => "TitleLabel".Equals(l.name, StringComparison.OrdinalIgnoreCase));
		UILabel[] array = o2.Map((UILabel t) => t.GetComponentsInChildren<UILabel>()) ?? new UILabel[0];
		float num = o.Map((PromoActionsManager.AmazonEventInfo e) => e.Percentage);
		string text = LocalizationStore.Get("Key_1672");
		UILabel[] array2 = array;
		foreach (UILabel uILabel2 in array2)
		{
			uILabel2.text = ("Key_1672".Equals(text, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(text, num));
		}
	}

	private void Update()
	{
		if (!InterfaceEnabled)
		{
			_escapePressed = false;
			return;
		}
		if (FreeAwardController.Instance == null)
		{
			bankView.freeAwardButton.gameObject.SetActive(false);
		}
		else if (!Defs.MainMenuScene.Equals(Application.loadedLevelName, StringComparison.Ordinal))
		{
			bankView.freeAwardButton.gameObject.SetActive(false);
		}
		else if (MobileAdManager.AdIsApplicable(MobileAdManager.Type.Video) && !_timeTamperingDetected.Value && FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>())
		{
			bool active = FreeAwardController.Instance.AdvertCountLessThanLimit();
			bankView.freeAwardButton.gameObject.SetActive(active);
		}
		UpdateBankView(bankViewCommon);
		UpdateBankView(bankViewX3);
		UpdateBankView(bankView_AB_NoGrid_Common);
		UpdateBankView(bankView_AB_NoGrid_X3);
		EventHandler escapePressed = this.EscapePressed;
		if (_escapePressed && escapePressed != null)
		{
			escapePressed(this, EventArgs.Empty);
			_escapePressed = false;
		}
	}

	private void LateUpdate()
	{
		if (InterfaceEnabled && ExperienceController.sharedController != null && !_lockInterfaceEnabledCoroutine)
		{
			ExperienceController.sharedController.isShowRanks = false;
		}
	}

	private void UpdateBankView(BankView bankView)
	{
		if (bankView == null || !bankView.gameObject.activeSelf)
		{
			return;
		}
		if (coinsShop.IsWideLayoutAvailable)
		{
			bankView.ConnectionProblemLabelEnabled = false;
			bankView.CrackersWarningLabelEnabled = true;
			bankView.NotEnoughCoinsLabelEnabled = false;
			bankView.NotEnoughGemsLabelEnabled = false;
			bankView.PurchaseButtonsEnabled = false;
			bankView.PurchaseSuccessfulLabelEnabled = false;
		}
		else
		{
			if (!(coinsShop.thisScript != null))
			{
				return;
			}
			bankView.NotEnoughCoinsLabelEnabled = coinsShop.thisScript.notEnoughCurrency != null && coinsShop.thisScript.notEnoughCurrency.Equals("Coins");
			bankView.NotEnoughGemsLabelEnabled = coinsShop.thisScript.notEnoughCurrency != null && coinsShop.thisScript.notEnoughCurrency.Equals("GemsCurrency");
			ActivityIndicator.IsActiveIndicator = StoreKitEventListener.purchaseInProcess;
			if (coinsShop.IsNoConnection)
			{
				if (Time.realtimeSinceStartup - tmOfFirstEnterTheBank > 3f)
				{
					bankView.ConnectionProblemLabelEnabled = true;
				}
				bankView.NotEnoughCoinsLabelEnabled = false;
				bankView.NotEnoughGemsLabelEnabled = false;
				bankView.PurchaseButtonsEnabled = false;
				bankView.PurchaseSuccessfulLabelEnabled = false;
			}
			else
			{
				bankView.ConnectionProblemLabelEnabled = false;
				bankView.PurchaseButtonsEnabled = true;
			}
			bankView.PurchaseSuccessfulLabelEnabled = coinsShop.thisScript.ProductPurchasedRecently;
		}
	}

	private void OnDestroy()
	{
		PromoActionsManager.EventX3Updated -= OnEventX3Updated;
		PromoActionsManager.EventAmazonX3Updated -= OnEventX3AmazonBonusUpdated;
		if (bankViewCommon != null)
		{
			bankViewCommon.PurchaseButtonPressed -= HandlePurchaseButtonPressed;
		}
		if (bankViewX3 != null)
		{
			bankViewX3.PurchaseButtonPressed -= HandlePurchaseButtonPressed;
		}
		if (bankView_AB_NoGrid_Common != null)
		{
			bankView_AB_NoGrid_Common.PurchaseButtonPressed -= HandlePurchaseButtonPressed;
		}
		if (bankView_AB_NoGrid_X3 != null)
		{
			bankView_AB_NoGrid_X3.PurchaseButtonPressed -= HandlePurchaseButtonPressed;
		}
	}

	private void HandlePurchaseButtonPressed(object sender, PurchaseEventArgs e)
	{
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64 && Time.realtimeSinceStartup - _lastTimePurchaseButtonPressed < 1f)
		{
			Debug.Log("Bank button pressed but ignored: " + e);
			return;
		}
		_lastTimePurchaseButtonPressed = Time.realtimeSinceStartup;
		Debug.Log("Bank button pressed: " + e);
		if (StoreKitEventListener.purchaseInProcess)
		{
			Debug.Log("Cannot perform request while purchase is in progress.");
		}
		if (coinsShop.thisScript != null)
		{
			coinsShop.thisScript.HandlePurchaseButton(e.Index, e.Currency);
		}
	}

	private static string ClampCoinCount(int coinCount)
	{
		return coinCount.ToString();
	}

	public static void AddCoins(int count, bool needIndication = true, AnalyticsConstants.AccrualType accrualType = AnalyticsConstants.AccrualType.Earned)
	{
		int @int = Storager.getInt("Coins", false);
		Storager.setInt("Coins", @int + count, false);
		AnalyticsFacade.CurrencyAccrual(count, "Coins", accrualType);
		if (needIndication)
		{
			CoinsMessage.FireCoinsAddedEvent();
		}
	}

	public static void AddGems(int count, bool needIndication = true, AnalyticsConstants.AccrualType accrualType = AnalyticsConstants.AccrualType.Earned)
	{
		int @int = Storager.getInt("GemsCurrency", false);
		Storager.setInt("GemsCurrency", @int + count, false);
		AnalyticsFacade.CurrencyAccrual(count, "GemsCurrency", accrualType);
		if (needIndication)
		{
			CoinsMessage.FireCoinsAddedEvent(true);
		}
	}

	public static IEnumerator WaitForIndicationGems(bool isGems)
	{
		while (!canShowIndication)
		{
			yield return null;
		}
		CoinsMessage.FireCoinsAddedEvent(isGems);
	}

	public void FreeAwardButtonClick()
	{
		ButtonClickSound.TryPlayClick();
		if (FreeAwardController.Instance == null || !FreeAwardController.Instance.AdvertCountLessThanLimit())
		{
			return;
		}
		List<double> list = ((!MobileAdManager.IsPayingUser()) ? PromoActionsManager.MobileAdvert.RewardedVideoDelayMinutesNonpaying : PromoActionsManager.MobileAdvert.RewardedVideoDelayMinutesPaying);
		if (list.Count != 0)
		{
			DateTime date = DateTime.UtcNow.Date;
			KeyValuePair<int, DateTime> keyValuePair = FreeAwardController.Instance.LastAdvertShow(date);
			int num = Math.Max(0, keyValuePair.Key + 1);
			if (num <= list.Count)
			{
				DateTime dateTime = ((!(keyValuePair.Value < date)) ? keyValuePair.Value : date);
				FreeAwardController.Instance.SetWatchState(dateTime + TimeSpan.FromMinutes(list[num]));
			}
		}
	}
}
