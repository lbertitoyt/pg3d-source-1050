using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using Rilisoft;
using UnityEngine;

internal sealed class BankView : MonoBehaviour, IDisposable
{
	public List<BankViewItem> coinItemsABStatic;

	public List<BankViewItem> gemItemsABStatic;

	public GameObject goldContainerABStatic;

	public GameObject gemsContainerABStatic;

	public ButtonHandler backButton;

	public GameObject premium;

	public GameObject premium5percent;

	public GameObject premium10percent;

	public GameObject interfaceHolder;

	public UILabel connectionProblemLabel;

	public UILabel crackersWarningLabel;

	public UILabel notEnoughCoinsLabel;

	public UILabel notEnoughGemsLabel;

	public UISprite purchaseSuccessfulLabel;

	public UILabel[] eventX3RemainTime;

	public GameObject btnTabContainer;

	public UIButton btnTabGold;

	public UIButton btnTabGems;

	public UIScrollView goldScrollView;

	public UIGrid goldItemGrid;

	public BankViewItem goldItemPrefab;

	public UIScrollView gemsScrollView;

	public UIGrid gemsItemGrid;

	public BankViewItem gemsItemPrefab;

	public UIButton freeAwardButton;

	public UIWidget eventX3AmazonBonusWidget;

	public UILabel amazonEventCaptionLabel;

	public UILabel amazonEventTitleLabel;

	private UILabel _freeAwardButtonLagelCont;

	private StoreKitEventListener _storeKitEventListener;

	private bool _needResetScrollView;

	private bool _isPurchasesAreadyEnabled;

	public TweenColor colorBlinkForX3;

	private float _lastUpdateTime;

	private string _localizeSaleLabel;

	private readonly List<Action> _disposeActions = new List<Action>();

	private bool _disposed;

	public static int[] discountsCoins = new int[7] { 0, 0, 7, 10, 12, 15, 33 };

	private static bool _isInitGoldPurchasesInfo_AB_Static;

	private static IList<PurchaseEventArgs> _goldPurchasesInfo_AB_Static;

	public static int[] discountsGems = new int[7] { 0, 0, 7, 10, 12, 15, 33 };

	private static bool _isInitGemsPurchasesInfo_AB_Static;

	private static IList<PurchaseEventArgs> _gemsPurchasesInfo_AB_Static;

	private UILabel _freeAwardButtonLabel
	{
		get
		{
			if (_freeAwardButtonLagelCont != null)
			{
				return _freeAwardButtonLagelCont;
			}
			if (freeAwardButton == null)
			{
				return _freeAwardButtonLagelCont;
			}
			return _freeAwardButtonLagelCont = freeAwardButton.GetComponentInChildren<UILabel>();
		}
	}

	public bool InterfaceEnabled
	{
		get
		{
			return interfaceHolder != null && interfaceHolder.activeInHierarchy;
		}
		set
		{
			if (interfaceHolder != null)
			{
				interfaceHolder.SetActive(value);
			}
		}
	}

	public bool ConnectionProblemLabelEnabled
	{
		get
		{
			return connectionProblemLabel != null && connectionProblemLabel.gameObject.GetActive();
		}
		set
		{
			if (connectionProblemLabel != null)
			{
				connectionProblemLabel.gameObject.SetActive(value);
			}
		}
	}

	public bool CrackersWarningLabelEnabled
	{
		get
		{
			return crackersWarningLabel != null && crackersWarningLabel.gameObject.GetActive();
		}
		set
		{
			if (crackersWarningLabel != null)
			{
				crackersWarningLabel.gameObject.SetActive(value);
			}
		}
	}

	public bool NotEnoughCoinsLabelEnabled
	{
		get
		{
			return notEnoughCoinsLabel != null && notEnoughCoinsLabel.gameObject.GetActive();
		}
		set
		{
			if (notEnoughCoinsLabel != null)
			{
				notEnoughCoinsLabel.gameObject.SetActive(value);
			}
		}
	}

	public bool NotEnoughGemsLabelEnabled
	{
		get
		{
			return notEnoughGemsLabel != null && notEnoughGemsLabel.gameObject.GetActive();
		}
		set
		{
			if (notEnoughGemsLabel != null)
			{
				notEnoughGemsLabel.gameObject.SetActive(value);
			}
		}
	}

	private bool IsInAB_Static_Bank
	{
		get
		{
			return coinItemsABStatic != null && coinItemsABStatic.Any() && gemItemsABStatic != null && gemItemsABStatic.Any();
		}
	}

	public bool PurchaseButtonsEnabled
	{
		set
		{
			btnTabContainer.SetActive(value);
			if (value)
			{
				if (!_isPurchasesAreadyEnabled)
				{
					_isPurchasesAreadyEnabled = true;
					bool isEnabled = btnTabGold.isEnabled;
					if (IsInAB_Static_Bank)
					{
						goldContainerABStatic.SetActive(!isEnabled);
						gemsContainerABStatic.SetActive(isEnabled);
					}
					else
					{
						goldScrollView.gameObject.SetActive(!isEnabled);
						gemsScrollView.gameObject.SetActive(isEnabled);
						ResetScrollView(isEnabled, false);
					}
				}
			}
			else
			{
				if (IsInAB_Static_Bank)
				{
					goldContainerABStatic.SetActive(value);
					gemsContainerABStatic.SetActive(value);
				}
				else
				{
					goldScrollView.gameObject.SetActive(value);
					gemsScrollView.gameObject.SetActive(value);
				}
				_isPurchasesAreadyEnabled = false;
			}
		}
	}

	public bool PurchaseSuccessfulLabelEnabled
	{
		get
		{
			return purchaseSuccessfulLabel != null && purchaseSuccessfulLabel.gameObject.GetActive();
		}
		set
		{
			if (purchaseSuccessfulLabel != null)
			{
				purchaseSuccessfulLabel.gameObject.SetActive(value);
			}
		}
	}

	public static IList<PurchaseEventArgs> goldPurchasesInfo
	{
		get
		{
			List<PurchaseEventArgs> list = new List<PurchaseEventArgs>();
			list.Add(new PurchaseEventArgs(0, 0, 0m, "Coins", discountsCoins[0]));
			list.Add(new PurchaseEventArgs(1, 0, 0m, "Coins", discountsCoins[1]));
			list.Add(new PurchaseEventArgs(2, 0, 0m, "Coins", discountsCoins[2]));
			list.Add(new PurchaseEventArgs(3, 0, 0m, "Coins", discountsCoins[3]));
			list.Add(new PurchaseEventArgs(4, 0, 0m, "Coins", discountsCoins[4]));
			list.Add(new PurchaseEventArgs(5, 0, 0m, "Coins", discountsCoins[5]));
			list.Add(new PurchaseEventArgs(6, 0, 0m, "Coins", discountsCoins[6]));
			return list;
		}
	}

	public static IList<PurchaseEventArgs> goldPurchasesInfo_AB_StaticDefault
	{
		get
		{
			return goldPurchasesInfo.Where((PurchaseEventArgs pi) => pi.Index != 0).ToList();
		}
	}

	public static IList<PurchaseEventArgs> goldPurchasesInfo_AB_Static
	{
		get
		{
			if (!_isInitGoldPurchasesInfo_AB_Static)
			{
				_goldPurchasesInfo_AB_Static = goldPurchasesInfo_AB_StaticDefault;
				FriendsController.ParseABTestBankViewConfig();
			}
			return _goldPurchasesInfo_AB_Static;
		}
		set
		{
			_goldPurchasesInfo_AB_Static = value;
			_isInitGoldPurchasesInfo_AB_Static = true;
		}
	}

	public static IList<PurchaseEventArgs> gemsPurchasesInfo
	{
		get
		{
			List<PurchaseEventArgs> list = new List<PurchaseEventArgs>();
			list.Add(new PurchaseEventArgs(0, 0, 0m, "GemsCurrency", discountsCoins[0]));
			list.Add(new PurchaseEventArgs(1, 0, 0m, "GemsCurrency", discountsCoins[1]));
			list.Add(new PurchaseEventArgs(2, 0, 0m, "GemsCurrency", discountsCoins[2]));
			list.Add(new PurchaseEventArgs(3, 0, 0m, "GemsCurrency", discountsCoins[3]));
			list.Add(new PurchaseEventArgs(4, 0, 0m, "GemsCurrency", discountsCoins[4]));
			list.Add(new PurchaseEventArgs(5, 0, 0m, "GemsCurrency", discountsCoins[5]));
			list.Add(new PurchaseEventArgs(6, 0, 0m, "GemsCurrency", discountsCoins[6]));
			return list;
		}
	}

	public static IList<PurchaseEventArgs> gemsPurchasesInfo_AB_StaticDefault
	{
		get
		{
			return gemsPurchasesInfo.Where((PurchaseEventArgs pi) => pi.Index != 0).ToList();
		}
	}

	public static IList<PurchaseEventArgs> gemsPurchasesInfo_AB_Static
	{
		get
		{
			if (!_isInitGemsPurchasesInfo_AB_Static)
			{
				_gemsPurchasesInfo_AB_Static = gemsPurchasesInfo_AB_StaticDefault;
				FriendsController.ParseABTestBankViewConfig();
			}
			return _gemsPurchasesInfo_AB_Static;
		}
		set
		{
			_gemsPurchasesInfo_AB_Static = value;
			_isInitGemsPurchasesInfo_AB_Static = true;
		}
	}

	public event EventHandler<PurchaseEventArgs> PurchaseButtonPressed;

	public event EventHandler BackButtonPressed
	{
		add
		{
			if (backButton != null)
			{
				backButton.Clicked += value;
			}
		}
		remove
		{
			if (backButton != null)
			{
				backButton.Clicked -= value;
			}
		}
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}
		Debug.Log("Disposing " + GetType().Name);
		foreach (Action item in _disposeActions.Where((Action a) => a != null))
		{
			item();
		}
		_disposed = true;
	}

	private void OnDestroy()
	{
		if (IsInAB_Static_Bank)
		{
			FriendsController.StaticBankConfigUpdated -= UpdateViewConfigChanged;
		}
		Dispose();
	}

	private void Awake()
	{
		InitializeButtonsCoroutine();
		if (IsInAB_Static_Bank)
		{
			FriendsController.StaticBankConfigUpdated += UpdateViewConfigChanged;
		}
	}

	private void UpdateViewConfigChanged()
	{
		PopulateItemGrid(false);
		PopulateItemGrid(true);
	}

	private void Start()
	{
		if (!IsInAB_Static_Bank)
		{
			goldScrollView.panel.UpdateAnchors();
			gemsScrollView.panel.UpdateAnchors();
			ResetScrollView(false, false);
			ResetScrollView(true, false);
		}
	}

	private void Update()
	{
		if (_needResetScrollView)
		{
			_needResetScrollView = false;
			StartCoroutine(ResetScrollViewsDelayed());
		}
		if (Time.realtimeSinceStartup - _lastUpdateTime >= 0.5f)
		{
			long eventX3RemainedTime = PromoActionsManager.sharedManager.EventX3RemainedTime;
			TimeSpan timeSpan = TimeSpan.FromSeconds(eventX3RemainedTime);
			string empty = string.Empty;
			empty = ((timeSpan.Days <= 0) ? string.Format("{0}: {1:00}:{2:00}:{3:00}", _localizeSaleLabel, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) : string.Format("{0}: {1} {2} {3:00}:{4:00}:{5:00}", _localizeSaleLabel, timeSpan.Days, (timeSpan.Days != 1) ? "Days" : "Day", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
			if (eventX3RemainTime != null)
			{
				for (int i = 0; i < eventX3RemainTime.Length; i++)
				{
					eventX3RemainTime[i].text = empty;
				}
			}
			if (colorBlinkForX3 != null && timeSpan.TotalHours < (double)Defs.HoursToEndX3ForIndicate && !colorBlinkForX3.enabled)
			{
				colorBlinkForX3.enabled = true;
			}
			_lastUpdateTime = Time.realtimeSinceStartup;
		}
		PremiumAccountController.AccountType accountType = PremiumAccountController.AccountType.None;
		if (PremiumAccountController.Instance != null)
		{
			accountType = PremiumAccountController.Instance.GetCurrentAccount();
		}
		premium.SetActive(accountType == PremiumAccountController.AccountType.SevenDays || accountType == PremiumAccountController.AccountType.Month);
		premium5percent.SetActive(accountType == PremiumAccountController.AccountType.SevenDays);
		premium10percent.SetActive(accountType == PremiumAccountController.AccountType.Month);
		if (_freeAwardButtonLabel != null && freeAwardButton.isActiveAndEnabled)
		{
			_freeAwardButtonLabel.text = ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? string.Format("[FFA300FF]{0}[-]", ScriptLocalization.Get("Key_1155")) : string.Format("[50CEFFFF]{0}[-]", ScriptLocalization.Get("Key_2046")));
		}
	}

	private EventHandler CreateButtonHandler(PurchaseEventArgs purchaseInfo)
	{
		return delegate
		{
			EventHandler<PurchaseEventArgs> purchaseButtonPressed = this.PurchaseButtonPressed;
			if (purchaseButtonPressed != null)
			{
				purchaseButtonPressed(this, purchaseInfo);
			}
		};
	}

	private void InitializeButtonsCoroutine()
	{
		_storeKitEventListener = UnityEngine.Object.FindObjectOfType<StoreKitEventListener>();
		if (_storeKitEventListener == null)
		{
			Debug.LogWarning("storeKitEventListener == null");
			if (goldItemPrefab != null)
			{
				goldItemPrefab.gameObject.SetActive(false);
			}
			if (gemsItemPrefab != null)
			{
				gemsItemPrefab.gameObject.SetActive(false);
			}
		}
		else
		{
			if (!IsInAB_Static_Bank)
			{
				UpdateViewConfigChanged();
			}
			OnEnable();
		}
	}

	private void PopulateItemGrid(bool isGems)
	{
		IList<PurchaseEventArgs> list2;
		if (isGems)
		{
			IList<PurchaseEventArgs> list = gemsPurchasesInfo;
			list2 = list;
		}
		else
		{
			list2 = goldPurchasesInfo;
		}
		IList<PurchaseEventArgs> list3 = list2;
		if (IsInAB_Static_Bank)
		{
			for (int i = 0; i < list3.Count; i++)
			{
				BankViewItem item = ((!isGems) ? coinItemsABStatic[i] : gemItemsABStatic[i]);
				UpdateItem(item, i, isGems);
			}
			return;
		}
		BankViewItem bankViewItem = ((!isGems) ? goldItemPrefab : gemsItemPrefab);
		UIScrollView uIScrollView = ((!isGems) ? goldScrollView : gemsScrollView);
		UIGrid uIGrid = ((!isGems) ? goldItemGrid : gemsItemGrid);
		for (int j = 0; j < list3.Count; j++)
		{
			BankViewItem bankViewItem2 = UnityEngine.Object.Instantiate(bankViewItem);
			bankViewItem2.name = string.Format("{0:00}", j);
			bankViewItem2.transform.parent = uIGrid.transform;
			bankViewItem2.transform.localScale = Vector3.one;
			bankViewItem2.transform.localPosition = Vector3.zero;
			bankViewItem2.transform.localRotation = Quaternion.identity;
			UpdateItem(bankViewItem2, j, isGems);
		}
		bankViewItem.gameObject.SetActive(false);
		ResetScrollView(isGems, false);
	}

	private void LoadIconForItem(BankViewItem item, int i, bool isGems, bool load)
	{
		if (load)
		{
			string text = ((!isGems) ? ("Textures/Bank" + ((!IsInAB_Static_Bank) ? string.Empty : "/Static_Bank_Textures") + "/Coins_Shop_" + (i + 1)) : ("Textures/Bank/Coins_Shop_Gem_" + (i + 1)));
			if (Device.IsLoweMemoryDevice)
			{
				PreloadTexture component = item.icon.GetComponent<PreloadTexture>();
				if (component != null)
				{
					component.pathTexture = text;
				}
			}
			else
			{
				item.icon.mainTexture = Resources.Load<Texture>(text);
			}
		}
		else
		{
			item.icon.mainTexture = null;
		}
	}

	public void LoadCurrencyIcons(bool load)
	{
		List<BankViewItem> list = ((!IsInAB_Static_Bank) ? (goldItemGrid.GetComponentsInChildren<BankViewItem>(true) ?? new BankViewItem[0]).ToList() : coinItemsABStatic);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].purchaseInfo == null)
			{
				return;
			}
			LoadIconForItem(list[i], list[i].purchaseInfo.Index, false, load);
		}
		List<BankViewItem> list2 = ((!IsInAB_Static_Bank) ? (gemsItemGrid.GetComponentsInChildren<BankViewItem>(true) ?? new BankViewItem[0]).ToList() : gemItemsABStatic);
		for (int j = 0; j < list2.Count; j++)
		{
			LoadIconForItem(list2[j], list2[j].purchaseInfo.Index, true, load);
		}
	}

	private void UpdateItem(BankViewItem item, int i, bool isGems)
	{
		PurchaseEventArgs purchaseEventArgs = ((!isGems) ? goldPurchasesInfo[i] : gemsPurchasesInfo[i]);
		string[] array = ((!isGems) ? StoreKitEventListener.coinIds : StoreKitEventListener.gemsIds);
		if (purchaseEventArgs.Index < array.Length)
		{
			purchaseEventArgs.Count = ((!isGems) ? VirtualCurrencyHelper.coinInappsQuantity[purchaseEventArgs.Index] : VirtualCurrencyHelper.gemsInappsQuantity[purchaseEventArgs.Index]);
			decimal num = ((!isGems) ? VirtualCurrencyHelper.coinPriceIds[purchaseEventArgs.Index] : VirtualCurrencyHelper.gemsPriceIds[purchaseEventArgs.Index]);
			purchaseEventArgs.CurrencyAmount = num - 0.01m;
		}
		string price = string.Format("${0}", purchaseEventArgs.CurrencyAmount);
		if (purchaseEventArgs.Index < array.Length)
		{
			string id = array[purchaseEventArgs.Index];
			IMarketProduct marketProduct = _storeKitEventListener.Products.FirstOrDefault((IMarketProduct p) => p.Id == id);
			if (marketProduct != null)
			{
				price = marketProduct.Price;
			}
			else
			{
				Debug.LogWarning("marketProduct == null,    id: " + id);
			}
		}
		else
		{
			Debug.LogWarning("purchaseInfo.Index >= StoreKitEventListener.coinIds.Length");
		}
		item.Price = price;
		try
		{
			if (item.inappNameLabels != null)
			{
				foreach (UILabel inappNameLabel in item.inappNameLabels)
				{
					inappNameLabel.text = LocalizationStore.Get((!isGems) ? VirtualCurrencyHelper.coinInappsLocalizationKeys[purchaseEventArgs.Index] : VirtualCurrencyHelper.gemsInappsLocalizationKeys[purchaseEventArgs.Index]);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception setting inapp localizations: " + ex);
		}
		string text = ((!isGems) ? ("Textures/Bank" + ((!IsInAB_Static_Bank) ? string.Empty : "/Static_Bank_Textures") + "/Coins_Shop_" + (purchaseEventArgs.Index + 1)) : ("Textures/Bank/Coins_Shop_Gem_" + (purchaseEventArgs.Index + 1)));
		if (Device.IsLoweMemoryDevice)
		{
			PreloadTexture component = item.icon.GetComponent<PreloadTexture>();
			if (component != null)
			{
				component.pathTexture = text;
			}
		}
		else
		{
			item.icon.mainTexture = Resources.Load<Texture>(text);
		}
		ButtonHandler purchaseButton = item.btnBuy.GetComponent<ButtonHandler>();
		if (purchaseButton == null)
		{
			return;
		}
		item.Count = purchaseEventArgs.Count;
		item.CountX3 = 3 * purchaseEventArgs.Count;
		if (item.discountSprite != null)
		{
			item.discountSprite.gameObject.SetActive(purchaseEventArgs.Discount > 0);
		}
		if (item.discountPercentsLabel != null && purchaseEventArgs.Discount > 0)
		{
			item.discountPercentsLabel.text = string.Format("{0}%", purchaseEventArgs.Discount);
		}
		item.purchaseInfo = purchaseEventArgs;
		item.UpdateViewBestBuy();
		if (item.bonusButtonView != null)
		{
			item.bonusButtonView.UpdateState(purchaseEventArgs);
		}
		if (!purchaseButton.HasClickedHandlers)
		{
			EventHandler rawButtonHandler = CreateButtonHandler(purchaseEventArgs);
			purchaseButton.Clicked += rawButtonHandler;
			_disposeActions.Add(delegate
			{
				purchaseButton.Clicked -= rawButtonHandler;
			});
		}
	}

	public void OnBtnTabClick(UIButton btnTab)
	{
		if (btnTab == btnTabGold)
		{
			Debug.Log("Activated Tab Gold");
			btnTabGold.isEnabled = false;
			btnTabGems.isEnabled = true;
			if (IsInAB_Static_Bank)
			{
				goldContainerABStatic.SetActive(true);
				gemsContainerABStatic.SetActive(false);
			}
			else
			{
				goldScrollView.gameObject.SetActive(true);
				gemsScrollView.gameObject.SetActive(false);
				ResetScrollView(false, false);
			}
		}
		else if (btnTab == btnTabGems)
		{
			Debug.Log("Activated Tab Gems");
			btnTabGold.isEnabled = true;
			btnTabGems.isEnabled = false;
			if (IsInAB_Static_Bank)
			{
				goldContainerABStatic.SetActive(false);
				gemsContainerABStatic.SetActive(true);
			}
			else
			{
				goldScrollView.gameObject.SetActive(false);
				gemsScrollView.gameObject.SetActive(true);
				ResetScrollView(true, false);
			}
		}
		else
		{
			Debug.Log("Unknown btnTab");
		}
	}

	private void OnEnable()
	{
		if (IsInAB_Static_Bank)
		{
			UpdateViewConfigChanged();
		}
		else
		{
			SortItemGrid(false);
			SortItemGrid(true);
		}
		UIButton btnTab = btnTabGems;
		if (coinsShop.thisScript != null && coinsShop.thisScript.notEnoughCurrency != null && coinsShop.thisScript.notEnoughCurrency.Equals("Coins"))
		{
			btnTab = btnTabGold;
		}
		OnBtnTabClick(btnTab);
		_localizeSaleLabel = LocalizationStore.Get("Key_0419");
		if (connectionProblemLabel != null)
		{
			connectionProblemLabel.text = LocalizationStore.Get("Key_0278");
		}
	}

	private void SortItemGrid(bool isGems)
	{
		if (!IsInAB_Static_Bank)
		{
			UIGrid uIGrid = ((!isGems) ? goldItemGrid : gemsItemGrid);
			Transform transform = uIGrid.transform;
			List<BankViewItem> list = new List<BankViewItem>();
			for (int i = 0; i < transform.childCount; i++)
			{
				BankViewItem component = transform.GetChild(i).GetComponent<BankViewItem>();
				list.Add(component);
			}
			list.Sort();
			for (int j = 0; j < list.Count; j++)
			{
				list[j].gameObject.name = string.Format("{0:00}", j);
			}
			ResetScrollView(isGems, false);
		}
	}

	private IEnumerator ResetScrollViewsDelayed()
	{
		if (!IsInAB_Static_Bank)
		{
			yield return null;
			ResetScrollView(false, false);
			ResetScrollView(true, false);
		}
	}

	private void ResetScrollView(bool isGems, bool needDelayedUpdate = true)
	{
		if (!IsInAB_Static_Bank)
		{
			UIScrollView uIScrollView = ((!isGems) ? goldScrollView : gemsScrollView);
			UIGrid uIGrid = ((!isGems) ? goldItemGrid : gemsItemGrid);
			if (needDelayedUpdate)
			{
				_needResetScrollView = needDelayedUpdate;
				return;
			}
			uIGrid.Reposition();
			uIScrollView.ResetPosition();
		}
	}
}
