using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Rilisoft;
using UnityEngine;

public class ShopNGUIController : MonoBehaviour
{
	public enum TrainingState
	{
		NotInSniperCategory,
		OnSniperRifle,
		InSniperCategoryNotOnSniperRifle,
		NotInArmorCategory,
		OnArmor,
		InArmorCategoryNotOnArmor,
		BackBlinking
	}

	public enum CategoryNames
	{
		PrimaryCategory,
		BackupCategory,
		MeleeCategory,
		SpecilCategory,
		SniperCategory,
		PremiumCategory,
		HatsCategory,
		ArmorCategory,
		SkinsCategory,
		CapesCategory,
		BootsCategory,
		GearCategory,
		MaskCategory
	}

	public delegate void Action7<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

	public const string BoughtCurrencsySettingBase = "BoughtCurrency";

	public const string TrainingShopStageStepKey = "ShopNGUIController.TrainingShopStageStepKey";

	public const string CustomSkinID = "CustomSkinID";

	private const string ShowArmorKey = "ShowArmorKeySetting";

	private const string ShowHatKey = "ShowHatKeySetting";

	[Header("Training After Removing Novice Armor")]
	public GameObject trainingRemoveNoviceArmorCollider;

	public List<GameObject> trainingTipsRemovedNoviceArmor = new List<GameObject>();

	[Header("Training")]
	public GameObject trainingColliders;

	public List<GameObject> trainingTips = new List<GameObject>();

	private bool _trainStateInitialized;

	private UISprite toBlink;

	private Dictionary<TrainingState, Action> _setTrainingStateMethods;

	private TrainingState _trainingState;

	private TrainingState _trainingStateRemovedNoviceArmor;

	public static ShopNGUIController sharedShop;

	public GameObject tryGunPanel;

	public UILabel tryGunMatchesCount;

	public GameObject tryGunDiscountPanel;

	public UILabel tryGunDiscountTime;

	public UILabel nonArmorWearDEscription;

	public UILabel armorWearDescription;

	public UILabel armorCountLabel;

	public UIButton facebookLoginLockedSkinButton;

	public List<UISprite> effectsSprites;

	public List<UILabel> effectsLabels;

	public GameObject hatLock;

	public GameObject armorLock;

	public BoxCollider shopCarouselCollider;

	public ToggleButton showArmorButtonTempArmor;

	public ToggleButton showHatButtonTempHat;

	public GameObject prolongateRentText;

	public GameObject purchaseSuccessfulRent;

	public UILabel salePercRent;

	public GameObject saleRent;

	public GameObject limitedLabel;

	public GameObject upgradesLabel;

	public GameObject rentProperties;

	public GameObject notRented;

	public UILabel daysLeftLabel;

	public UILabel timeLeftLabel;

	public UILabel daysLeftValueLabel;

	public UILabel timeLeftValueLabel;

	public GameObject redBackForTime;

	public UIButton rent;

	public Transform rentScreenPoint;

	public ToggleButton showArmorButton;

	public ToggleButton showHatButton;

	public PropertyInfoScreenController infoScreen;

	public GameObject stub;

	public UIButton upgradeGear;

	public UISprite currencyImagePrice;

	public UISprite currencyImagePriceGear;

	public UISprite currencyImagePriceUpgradeGear;

	public UILabel price2Gear;

	public UILabel priceUpgradeGear;

	public GameObject wholePrice2Gear;

	public GameObject wholePriceUpgradeGear;

	public UIButton buyGear;

	public UITexture wholePriceBG;

	public UITexture wholePriceBG_Discount;

	public UITexture wholePriceBG2Gear;

	public UITexture wholePriceBG2Gear_Discount;

	public UITexture wholePriceUpgradeBG2Gear;

	public UITexture wholePriceUpgradeBG2Gear_Discount;

	public UILabel fireRate;

	public UILabel fireRateMElee;

	public UILabel mobility;

	public UILabel mobilityMelee;

	public UILabel capacity;

	public UILabel damage;

	public UILabel damageMelee;

	public GameObject needTier;

	public UILabel needTierLabel;

	public GameObject purchaseSuccessful;

	public List<Light> mylights;

	[Range(-200f, 200f)]
	public float firstOFfset = -50f;

	[Range(-200f, 200f)]
	public float secondOffset = 50f;

	public GameObject nonArmorWearProperties;

	public GameObject armorWearProperties;

	public GameObject skinProperties;

	public GameObject gearProperties;

	public GameObject border;

	public Action OnBecomeActive;

	private string offerID;

	public CategoryNames offerCategory;

	public float scaleCoef = 0.5f;

	public Transform maskPoint;

	public Transform hatPoint;

	public Transform capePoint;

	public Transform bootsPoint;

	public Transform armorPoint;

	public GameObject mainPanel;

	public UIButton backButton;

	public UIButton coinShopButton;

	public UIPanel scrollViewPanel;

	public UIGrid wrapContent;

	public MyCenterOnChild carouselCenter;

	public GameObject body;

	public GameObject weaponPointShop;

	public Transform MainMenu_Pers;

	public string viewedId;

	public string chosenId;

	public Action resumeAction;

	public Action wearResumeAction;

	public Action<CategoryNames, string> wearUnequipAction;

	public Action<CategoryNames, string, string> wearEquipAction;

	public Action<string> buyAction;

	public Action<string> equipAction;

	public Action<string> activatePotionAction;

	public Action<string> onEquipSkinAction;

	private GameObject weapon;

	private AnimationClip profile;

	public bool EnableConfigurePos;

	public MultipleToggleButton category;

	public UIButton[] equips;

	public UISprite[] equippeds;

	public UIButton create;

	public UIButton buy;

	public UIButton upgrade;

	public UIButton unequip;

	public UIButton edit;

	public UIButton enable;

	public UIButton delete;

	public GameObject weaponProperties;

	public GameObject meleeProperties;

	public GameObject upgradesAnchor;

	public GameObject upgrade_1;

	public GameObject upgrade_2;

	public GameObject upgrade_3;

	public GameObject SpecialParams;

	public GameObject[] upgradeSprites3;

	public GameObject[] upgradeSprites2;

	public GameObject[] upgradeSprites1;

	public GameObject wholePrice;

	public GameObject sale;

	public UILabel price;

	public UILabel caption;

	public UILabel salePerc;

	public CategoryNames currentCategory;

	private bool capeEditingEnabled;

	public bool inGame = true;

	public List<Camera> ourCameras;

	public UILabel onlyInThis;

	public AnimationCoroutineRunner animationCoroutineRunner;

	public GameObject ActiveObject;

	private List<GameObject> hats = new List<GameObject>();

	private List<GameObject> capes = new List<GameObject>();

	private List<GameObject> boots = new List<GameObject>();

	private List<GameObject> gear = new List<GameObject>();

	private List<GameObject> masks = new List<GameObject>();

	private List<GameObject> _armorList;

	private Action<List<GameObject>, CategoryNames> sort = delegate(List<GameObject> prefabs, CategoryNames c)
	{
		Comparison<GameObject> comparison = delegate(GameObject go1, GameObject go2)
		{
			List<string> list = null;
			List<string> list2 = null;
			foreach (List<string> item in Wear.wear[c])
			{
				if (item.Contains(go1.name))
				{
					list = item;
				}
				if (item.Contains(go2.name))
				{
					list2 = item;
				}
			}
			if (list == null || list2 == null)
			{
				return 0;
			}
			return (list == list2) ? (list.IndexOf(go1.name) - list.IndexOf(go2.name)) : (Wear.wear[c].IndexOf(list) - Wear.wear[c].IndexOf(list2));
		};
		prefabs.Sort(comparison);
	};

	private GameObject pixlMan;

	public static readonly Dictionary<string, string> weaponCategoryLocKeys = new Dictionary<string, string>
	{
		{
			CategoryNames.PrimaryCategory.ToString(),
			"Key_0352"
		},
		{
			CategoryNames.BackupCategory.ToString(),
			"Key_0442"
		},
		{
			CategoryNames.MeleeCategory.ToString(),
			"Key_0441"
		},
		{
			CategoryNames.SpecilCategory.ToString(),
			"Key_0440"
		},
		{
			CategoryNames.SniperCategory.ToString(),
			"Key_1669"
		},
		{
			CategoryNames.PremiumCategory.ToString(),
			"Key_0093"
		}
	};

	private Transform highlightedCarouselObject;

	public int itemIndex = -1;

	private GameObject _lastSelectedItem;

	private GameObject[] _onPersArmorRefs;

	private static bool _ShowArmor = true;

	private static bool _ShowHat = true;

	private float timeToUpdateTempGunTime;

	private bool _shouldShowRewardWindowSkin;

	private bool _shouldShowRewardWindowCape;

	private GameObject _refOnLowPolyArmor;

	private Material[] _refsOnLowPolyArmorMaterials;

	private GameObject _label;

	public static string[] gearOrder = PotionsController.potions;

	private string _currentCape;

	private string _currentHat;

	private string _currentBoots;

	private string _currentArmor;

	private string _currentMask;

	public bool _goingToEditCape;

	private float lastTime;

	public static float IdleTimeoutPers = 5f;

	private float idleTimerLastTime;

	private float _timePurchaseSuccessfulShown;

	private float _timePurchaseRentSuccessfulShown;

	private IDisposable _backSubscription;

	private bool _escapeRequested;

	private float timeOfEnteringShopForProtectFromPressingCoinsButton;

	private Color? _storedAmbientLight;

	private bool? _storedFogEnabled;

	private bool _isFromPromoActions;

	private string _promoActionsIdClicked;

	private string _assignedWeaponTag = string.Empty;

	private bool InTrainingAfterNoviceArmorRemoved;

	public static bool NoviceArmorAvailable
	{
		get
		{
			return Storager.getInt("Training.NoviceArmorUsedKey", false) == 1 && (!TrainingController.TrainingCompleted || Storager.getInt("Training.ShouldRemoveNoviceArmorInShopKey", false) == 1);
		}
	}

	private TrainingState trainingStateRemoveNoviceArmor
	{
		get
		{
			return _trainingStateRemovedNoviceArmor;
		}
		set
		{
			try
			{
				if (_trainingStateRemovedNoviceArmor != value)
				{
					_trainingStateRemovedNoviceArmor = value;
				}
				for (int i = 0; i < trainingTipsRemovedNoviceArmor.Count; i++)
				{
					GameObject obj = trainingTipsRemovedNoviceArmor[i];
					string text = trainingTipsRemovedNoviceArmor[i].name;
					int trainingStateRemovedNoviceArmor = (int)_trainingStateRemovedNoviceArmor;
					obj.SetActive(text == trainingStateRemovedNoviceArmor.ToString());
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in trainingStateRemoveNoviceArmor setter: " + ex);
			}
		}
	}

	private TrainingState trainingState
	{
		get
		{
			if (!_trainStateInitialized)
			{
				_trainingState = (TrainingState)Storager.getInt("ShopNGUIController.TrainingShopStageStepKey", false);
				_trainStateInitialized = true;
			}
			return _trainingState;
		}
		set
		{
			try
			{
				if (_trainingState != value)
				{
					_trainingState = value;
					if (_trainingState == TrainingState.NotInArmorCategory || _trainingState == TrainingState.BackBlinking)
					{
						Storager.setInt("ShopNGUIController.TrainingShopStageStepKey", (int)_trainingState, false);
					}
				}
				_trainStateInitialized = true;
				for (int i = 0; i < trainingTips.Count; i++)
				{
					trainingTips[i].SetActive(i == (int)_trainingState);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in trainingState setter: " + ex);
			}
		}
	}

	private List<GameObject> armor
	{
		get
		{
			if (_armorList == null)
			{
				_armorList = new List<GameObject>(Resources.LoadAll<GameObject>("Armor"));
				sort(_armorList, CategoryNames.ArmorCategory);
			}
			return _armorList;
		}
		set
		{
			_armorList = value;
		}
	}

	public bool WeaponCategory
	{
		get
		{
			return IsWeaponCategory(currentCategory);
		}
	}

	public static bool ShowArmor
	{
		get
		{
			return _ShowArmor;
		}
		private set
		{
			if (_ShowArmor != value)
			{
				_ShowArmor = value;
				Action showArmorChanged = ShopNGUIController.ShowArmorChanged;
				if (showArmorChanged != null)
				{
					showArmorChanged();
				}
			}
		}
	}

	public static bool ShowHat
	{
		get
		{
			return _ShowHat;
		}
		private set
		{
			if (_ShowHat != value)
			{
				_ShowHat = value;
				Action showArmorChanged = ShopNGUIController.ShowArmorChanged;
				if (showArmorChanged != null)
				{
					showArmorChanged();
				}
			}
		}
	}

	public bool WearCategory
	{
		get
		{
			return IsWearCategory(currentCategory);
		}
	}

	private string NextUpgradeIDForCurrentGear
	{
		get
		{
			if (viewedId == null)
			{
				return null;
			}
			return GearManager.UpgradeIDForGear(GearManager.HolderQuantityForID(viewedId), GearManager.CurrentNumberOfUphradesForGear(viewedId) + 1);
		}
	}

	private string IDForCurrentGear
	{
		get
		{
			if (viewedId == null)
			{
				return null;
			}
			return GearManager.HolderQuantityForID(viewedId);
		}
	}

	private string _CurrentEquippedSN
	{
		get
		{
			return SnForWearCategory(currentCategory);
		}
	}

	private string _CurrentNoneEquipped
	{
		get
		{
			return NoneEquippedForWearCategory(currentCategory);
		}
	}

	public string _CurrentEquippedWear
	{
		get
		{
			return WearForCat(currentCategory);
		}
		set
		{
			SetWearForCategory(currentCategory, value);
		}
	}

	public bool IsFromPromoActions
	{
		get
		{
			return _isFromPromoActions;
		}
	}

	public static bool GuiActive
	{
		get
		{
			return sharedShop != null && sharedShop.ActiveObject.activeInHierarchy;
		}
		set
		{
			if (value)
			{
				if (sharedShop._backSubscription != null)
				{
					sharedShop._backSubscription.Dispose();
				}
				sharedShop._backSubscription = BackSystem.Instance.Register(sharedShop.HandleEscape, "Shop");
			}
			else if (sharedShop._backSubscription != null)
			{
				sharedShop._backSubscription.Dispose();
				sharedShop._backSubscription = null;
				Storager.RefreshWeaponDigestIfDirty();
			}
			if (value)
			{
				Transform transform = sharedShop.category.buttons[11].transform;
				transform.localPosition = new Vector3(-10000f, transform.localPosition.y, transform.localPosition.z);
			}
			if (ZombieCreator.sharedCreator != null)
			{
				ZombieCreator.sharedCreator.SuppressDrawingWaveMessage();
			}
			if (Tools.RuntimePlatform == RuntimePlatform.MetroPlayerX64)
			{
				QualitySettings.antiAliasing = 0;
			}
			else
			{
				QualitySettings.antiAliasing = ((value && !Device.isWeakDevice) ? 4 : 0);
			}
			FreeAwardShowHandler.CheckShowChest(value);
			if (!(sharedShop != null))
			{
				return;
			}
			sharedShop.SetOtherCamerasEnabled(!value);
			if (value)
			{
				sharedShop.LoadGear();
				sharedShop.armorLock.SetActive(Defs.isHunger);
				sharedShop.stub.SetActive(true);
				try
				{
					sharedShop._storedAmbientLight = RenderSettings.ambientLight;
					sharedShop._storedFogEnabled = RenderSettings.fog;
					RenderSettings.ambientLight = Defs.AmbientLightColorForShop();
					RenderSettings.fog = false;
					sharedShop.timeOfEnteringShopForProtectFromPressingCoinsButton = Time.realtimeSinceStartup;
					sharedShop.LoadCurrentWearToVars();
					sharedShop.UpdateIcons();
					CategoryNames cn = CategoryNames.PrimaryCategory;
					string text;
					if (sharedShop.offerID != null)
					{
						text = sharedShop.offerID;
						cn = sharedShop.offerCategory;
						sharedShop.offerID = null;
					}
					else
					{
						CategoryNames categoryNames = CategoryNames.PrimaryCategory;
						if (WeaponManager.sharedManager != null && WeaponManager.sharedManager._currentFilterMap == 1)
						{
							cn = CategoryNames.MeleeCategory;
							categoryNames = CategoryNames.MeleeCategory;
						}
						else if (WeaponManager.sharedManager != null && WeaponManager.sharedManager._currentFilterMap == 2)
						{
							cn = CategoryNames.SniperCategory;
							categoryNames = CategoryNames.SniperCategory;
						}
						text = _CurrentWeaponSetIDs()[(int)categoryNames] ?? TempGunOrHighestDPSGun(categoryNames, out cn);
					}
					if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
					{
						AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Open_Shop);
						sharedShop.trainingColliders.SetActive(true);
						if (sharedShop.trainingState >= TrainingState.OnArmor)
						{
							cn = CategoryNames.ArmorCategory;
							text = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
						}
						else if (sharedShop.trainingState >= TrainingState.OnSniperRifle)
						{
							cn = CategoryNames.SniperCategory;
							text = WeaponTags.HunterRifleTag;
						}
					}
					else
					{
						sharedShop.HideAllTrainingInterface();
						if (!sharedShop.InTrainingAfterNoviceArmorRemoved)
						{
							sharedShop.HideAllTrainingInterfaceRemovedNoviceArmor();
						}
					}
					string text2 = WeaponManager.LastBoughtTag(text);
					if (text2 == null)
					{
						if (IsWearCategory(cn))
						{
							foreach (List<List<string>> value2 in Wear.wear.Values)
							{
								foreach (List<string> item in value2)
								{
									if (item.Contains(text))
									{
										text = item[0];
										break;
									}
								}
							}
						}
					}
					else
					{
						text = text2;
					}
					sharedShop.CategoryChosen(cn, text, true);
					SetIconChosen(cn);
					sharedShop.MakeACtiveAfterDelay(text, cn);
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in ShopNGUIController.GuiActive: " + ex);
				}
				sharedShop.StartCoroutine(sharedShop.DisableStub());
			}
			else
			{
				Color? storedAmbientLight = sharedShop._storedAmbientLight;
				RenderSettings.ambientLight = ((!storedAmbientLight.HasValue) ? RenderSettings.ambientLight : storedAmbientLight.Value);
				bool? storedFogEnabled = sharedShop._storedFogEnabled;
				RenderSettings.fog = ((!storedFogEnabled.HasValue) ? RenderSettings.fog : storedFogEnabled.Value);
				if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted)
				{
					sharedShop.HideAllTrainingInterface();
					CategoryNames cn2;
					string tg = TempGunOrHighestDPSGun(CategoryNames.PrimaryCategory, out cn2);
					tg = WeaponManager.FirstUnboughtTag(tg);
					sharedShop.CategoryChosen(cn2, tg, true);
				}
				else if (sharedShop.InTrainingAfterNoviceArmorRemoved)
				{
					sharedShop.HideAllTrainingInterfaceRemovedNoviceArmor();
				}
				MyCenterOnChild myCenterOnChild = sharedShop.carouselCenter;
				myCenterOnChild.onFinished = (SpringPanel.OnFinished)Delegate.Remove(myCenterOnChild.onFinished, new SpringPanel.OnFinished(sharedShop.HandleCarouselCentering));
				PromoActionsManager.ActionsUUpdated -= sharedShop.HandleActionsUUpdated;
				sharedShop.SetWeapon(null);
				sharedShop.ActiveObject.SetActive(false);
				sharedShop.carouselCenter.enabled = false;
				WeaponManager.sharedManager.ClearCachedInnerPrefabs();
				if (Device.isPixelGunLow)
				{
					sharedShop.armor = null;
				}
			}
		}
	}

	public static event Action GunOrArmorBought;

	public static event Action<string> TryGunBought;

	public static event Action ShowArmorChanged;

	public static event Action GunBought;

	public static void GoToShop(CategoryNames cat, string id)
	{
		sharedShop.SetOfferID(id);
		sharedShop.offerCategory = cat;
		if (GuiActive)
		{
			sharedShop.CategoryChosen(cat, id);
			SetIconChosen(cat);
		}
		else if (SceneLoader.ActiveSceneName.Equals(Defs.MainMenuScene))
		{
			if (MainMenuController.sharedController != null)
			{
				MainMenuController.sharedController.HandleShopClicked(null, EventArgs.Empty);
			}
		}
		else
		{
			sharedShop.resumeAction = null;
			GuiActive = true;
		}
	}

	public static void AddBoughtCurrency(string currency, int count)
	{
		if (currency == null)
		{
			Debug.LogWarning("AddBoughtCurrency: currency == null");
			return;
		}
		if (Debug.isDebugBuild)
		{
			Debug.Log(string.Format("<color=#ff00ffff>AddBoughtCurrency {0} {1}</color>", currency, count));
		}
		Storager.setInt("BoughtCurrency" + currency, Storager.getInt("BoughtCurrency" + currency, false) + count, false);
	}

	public static void SpendBoughtCurrency(string currency, int count)
	{
		if (currency == null)
		{
			Debug.LogWarning("SpendBoughtCurrency: currency == null");
		}
		else if (Debug.isDebugBuild)
		{
			Debug.Log(string.Format("<color=#ff00ffff>SpendBoughtCurrency {0} {1}</color>", currency, count));
		}
	}

	public static void TryToBuy(GameObject mainPanel, ItemPrice price, Action onSuccess, Action onFailure = null, Func<bool> successAdditionalCond = null, Action onReturnFromBank = null, Action onEnterCoinsShopAction = null, Action onExitCoinsShopAction = null)
	{
		Debug.Log("Trying to buy from ShopNGUIController...");
		if (BankController.Instance == null)
		{
			Debug.LogWarning("BankController.Instance == null");
			return;
		}
		if (price == null)
		{
			Debug.LogWarning("price == null");
			return;
		}
		EventHandler handleBackFromBank = null;
		handleBackFromBank = delegate
		{
			BankController.Instance.BackRequested -= handleBackFromBank;
			BankController.Instance.InterfaceEnabled = false;
			coinsShop.thisScript.notEnoughCurrency = null;
			if (mainPanel != null)
			{
				mainPanel.SetActive(true);
			}
			if (onReturnFromBank != null)
			{
				onReturnFromBank();
			}
			if (onExitCoinsShopAction != null)
			{
				onExitCoinsShopAction();
			}
		};
		EventHandler act = null;
		act = delegate
		{
			BankController.Instance.BackRequested -= act;
			mainPanel.SetActive(true);
			coinsShop.thisScript.notEnoughCurrency = null;
			coinsShop.thisScript.onReturnAction = null;
			int @int = Storager.getInt(price.Currency, false);
			int num = @int;
			num -= price.Price;
			bool flag = num >= 0;
			bool num2;
			if (successAdditionalCond != null)
			{
				if (successAdditionalCond())
				{
					goto IL_0088;
				}
				num2 = flag;
			}
			else
			{
				num2 = flag;
			}
			if (!num2)
			{
				if (onFailure != null)
				{
					onFailure();
				}
				coinsShop.thisScript.notEnoughCurrency = price.Currency;
				Debug.Log("Trying to display bank interface...");
				BankController.Instance.BackRequested += handleBackFromBank;
				BankController.Instance.InterfaceEnabled = true;
				mainPanel.SetActive(false);
				if (onEnterCoinsShopAction != null)
				{
					onEnterCoinsShopAction();
				}
				return;
			}
			goto IL_0088;
			IL_0088:
			Storager.setInt(price.Currency, num, false);
			SpendBoughtCurrency(price.Currency, price.Price);
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				PlayerPrefs.Save();
			}
			if (FriendsController.useBuffSystem)
			{
				BuffSystem.instance.OnSomethingPurchased();
			}
			if (onSuccess != null)
			{
				onSuccess();
			}
		};
		act(BankController.Instance, EventArgs.Empty);
	}

	private void FilterUpgrades(List<GameObject> modelsList, GameObject a, CategoryNames category, string visualDefName)
	{
		if (a.name.Replace("(Clone)", string.Empty) == "Armor_Novice")
		{
			return;
		}
		if (a != null && TempItemsController.PriceCoefs.ContainsKey(a.name) && TempItemsController.sharedController != null)
		{
			if (TempItemsController.sharedController.ContainsItem(a.name))
			{
				modelsList.Add(a);
			}
			return;
		}
		List<List<string>> list = null;
		try
		{
			list = Wear.wear[category];
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat("Exception in FilterUpgrades ll = Wear.wear [category]: ", ex, " category = ", category.ToString()));
		}
		if (list == null)
		{
			Debug.LogError("FilterUpgrades: ll == null   category = " + category);
			return;
		}
		List<string> list2 = list.FirstOrDefault((List<string> l) => l.Contains(a.name)).ToList();
		if (list2 == null)
		{
			return;
		}
		string text = ((!string.IsNullOrEmpty(visualDefName)) ? Storager.getString(visualDefName, false) : string.Empty);
		int num = list2.IndexOf(a.name);
		if (Storager.getInt(a.name, true) > 0)
		{
			if (num == list2.Count - 1)
			{
				modelsList.Add(a);
			}
			else if (num >= 0 && num < list2.Count - 1 && Storager.getInt(list2[num + 1], true) == 0)
			{
				modelsList.Add(a);
			}
			return;
		}
		if (num == 0 && Wear.LeagueForWear(a.name, category) <= (int)RatingSystem.instance.currentLeague)
		{
			modelsList.Add(a);
		}
		if (num <= 0)
		{
			return;
		}
		if (Storager.getInt(list2[num - 1], true) > 0)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (list2.IndexOf(text) <= num)
				{
					modelsList.Add(a);
				}
			}
			else
			{
				modelsList.Add(a);
			}
		}
		else if (!string.IsNullOrEmpty(text) && a.name.Equals(text))
		{
			modelsList.Add(a);
		}
	}

	public static bool ShowLockedFacebookSkin()
	{
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null && SkinsController.shopKeyFromNameSkin.ContainsKey("61"))
		{
			string value = SkinsController.shopKeyFromNameSkin["61"];
			if (Array.IndexOf(StoreKitEventListener.skinIDs, value) >= 0)
			{
				foreach (KeyValuePair<string, string> value2 in InAppData.inAppData.Values)
				{
					if (value2.Key != null && value2.Key.Equals(value) && Storager.getInt(value2.Value, true) == 0)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public List<GameObject> FillModelsList(CategoryNames c)
	{
		Func<CategoryNames, Comparison<GameObject>> func = (CategoryNames cn) => delegate(GameObject lh, GameObject rh)
		{
			List<string> list2 = null;
			List<string> list3 = null;
			foreach (List<string> item in Wear.wear[cn])
			{
				if (item.Contains(lh.name))
				{
					list2 = item;
				}
				if (item.Contains(rh.name))
				{
					list3 = item;
				}
			}
			if (list2 == null || list3 == null)
			{
				return 0;
			}
			return (list2 == list3) ? (list2.IndexOf(lh.name) - list2.IndexOf(rh.name)) : (Wear.wear[cn].IndexOf(list2) - Wear.wear[cn].IndexOf(list3));
		};
		List<GameObject> list = new List<GameObject>();
		if (IsWeaponCategory(c))
		{
			list = WeaponManager.sharedManager.FilteredShopLists[(int)c];
		}
		else
		{
			switch (c)
			{
			case CategoryNames.HatsCategory:
				foreach (GameObject hat in hats)
				{
					FilterUpgrades(list, hat, CategoryNames.HatsCategory, Defs.VisualHatArmor);
				}
				list.Sort(func(CategoryNames.HatsCategory));
				break;
			case CategoryNames.ArmorCategory:
				if (Storager.getInt("Training.NoviceArmorUsedKey", false) == 1 && !TrainingController.TrainingCompleted)
				{
					GameObject gameObject2 = armor.FirstOrDefault((GameObject a) => a.name.Replace("(Clone)", string.Empty) == "Armor_Novice");
					if (gameObject2 != null)
					{
						list.Add(gameObject2);
					}
					else
					{
						Debug.LogError("No novice armor when Storager.getInt(Defs.NoviceArmorUsedKey,false) == 1 && !TrainingController.TrainingCompleted");
					}
				}
				else
				{
					foreach (GameObject item2 in armor)
					{
						FilterUpgrades(list, item2, CategoryNames.ArmorCategory, Defs.VisualArmor);
					}
				}
				list.Sort(func(CategoryNames.ArmorCategory));
				break;
			case CategoryNames.SkinsCategory:
				foreach (string key in SkinsController.skinsForPers.Keys)
				{
					list.Add(pixlMan);
				}
				if (ShowLockedFacebookSkin())
				{
					list.Add(pixlMan);
				}
				break;
			case CategoryNames.CapesCategory:
				foreach (GameObject cape in capes)
				{
					FilterUpgrades(list, cape, CategoryNames.CapesCategory, string.Empty);
				}
				list.Sort(func(CategoryNames.CapesCategory));
				break;
			case CategoryNames.BootsCategory:
				foreach (GameObject boot in boots)
				{
					FilterUpgrades(list, boot, CategoryNames.BootsCategory, string.Empty);
				}
				list.Sort(func(CategoryNames.BootsCategory));
				break;
			case CategoryNames.MaskCategory:
				foreach (GameObject mask in masks)
				{
					FilterUpgrades(list, mask, CategoryNames.MaskCategory, string.Empty);
				}
				list.Sort(func(CategoryNames.MaskCategory));
				break;
			case CategoryNames.GearCategory:
			{
				string[] array = ((!Defs.isDaterRegim) ? GearManager.Gear : GearManager.DaterGear);
				foreach (string g in array)
				{
					GameObject gameObject = null;
					for (int i = GearManager.NumOfGearUpgrades; i >= 0; i--)
					{
						if (i == 0)
						{
							gameObject = gear.Find((GameObject go) => go.name.Equals(GearManager.UpgradeIDForGear(g, 0)));
							break;
						}
						if (GearManager.CurrentNumberOfUphradesForGear(g) >= i)
						{
							gameObject = gear.Find((GameObject go) => go.name.Equals(GearManager.UpgradeIDForGear(g, i)));
							break;
						}
					}
					if (gameObject != null)
					{
						list.Add(gameObject);
					}
					else
					{
						Debug.LogWarning("toAdd: object not found");
					}
				}
				break;
			}
			}
		}
		return list;
	}

	private static string ItemIDForPrefab(string name, CategoryNames c)
	{
		switch (c)
		{
		case CategoryNames.ArmorCategory:
		{
			string string2 = Storager.getString(Defs.VisualArmor, false);
			if (!string.IsNullOrEmpty(string2) && Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(name) >= 0 && Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(name) < Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(string2))
			{
				return string2;
			}
			break;
		}
		case CategoryNames.HatsCategory:
		{
			string @string = Storager.getString(Defs.VisualHatArmor, false);
			if (!string.IsNullOrEmpty(@string) && Wear.wear[CategoryNames.HatsCategory][0].IndexOf(name) >= 0 && Wear.wear[CategoryNames.HatsCategory][0].IndexOf(name) < Wear.wear[CategoryNames.HatsCategory][0].IndexOf(@string))
			{
				return @string;
			}
			break;
		}
		}
		return name;
	}

	private static string ItemIDForPrefabReverse(string name, CategoryNames c)
	{
		switch (c)
		{
		case CategoryNames.ArmorCategory:
		{
			string string2 = Storager.getString(Defs.VisualArmor, false);
			if (string.IsNullOrEmpty(string2) || !string2.Equals(name) || Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(name) < 0)
			{
				break;
			}
			for (int j = 1; j < Wear.wear[CategoryNames.ArmorCategory][0].Count; j++)
			{
				if (Storager.getInt(Wear.wear[CategoryNames.ArmorCategory][0][j], true) == 0)
				{
					return Wear.wear[CategoryNames.ArmorCategory][0][j];
				}
			}
			break;
		}
		case CategoryNames.HatsCategory:
		{
			string @string = Storager.getString(Defs.VisualHatArmor, false);
			if (string.IsNullOrEmpty(@string) || !@string.Equals(name) || Wear.wear[CategoryNames.HatsCategory][0].IndexOf(name) < 0)
			{
				break;
			}
			for (int i = 1; i < Wear.wear[CategoryNames.HatsCategory][0].Count; i++)
			{
				if (Storager.getInt(Wear.wear[CategoryNames.HatsCategory][0][i], true) == 0)
				{
					return Wear.wear[CategoryNames.HatsCategory][0][i];
				}
			}
			break;
		}
		}
		return name;
	}

	private static string GetWeaponStatText(int currentValue, int nextValue)
	{
		if (nextValue - currentValue == 0)
		{
			return currentValue.ToString();
		}
		if (nextValue - currentValue > 0)
		{
			return string.Format("{0}[00ff00]+{1}[-]", currentValue, nextValue - currentValue);
		}
		return string.Format("{0}[FACC2E]{1}[-]", currentValue, nextValue - currentValue);
	}

	public void SetCamera()
	{
		Transform mainMenu_Pers = MainMenu_Pers;
		HOTween.Kill(mainMenu_Pers);
		Vector3 vector = new Vector3(0f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, 0f, 0f);
		Vector3 vector3 = new Vector3(1f, 1f, 1f);
		switch (currentCategory)
		{
		case CategoryNames.CapesCategory:
			vector = new Vector3(0f, 0f, 0f);
			vector2 = new Vector3(0f, -130.76f, 0f);
			break;
		case CategoryNames.HatsCategory:
			vector = new Vector3(1.06f, -0.54f, 2.19f);
			vector2 = new Vector3(0f, -9.5f, 0f);
			break;
		case CategoryNames.MaskCategory:
			vector = new Vector3(1.06f, -0.54f, 2.19f);
			vector2 = new Vector3(0f, -9.5f, 0f);
			break;
		default:
			vector = new Vector3(0f, 0f, 0f);
			vector2 = new Vector3(0f, 0f, 0f);
			break;
		}
		float p_duration = 0.5f;
		idleTimerLastTime = Time.realtimeSinceStartup;
		HOTween.To(mainMenu_Pers, p_duration, new TweenParms().Prop("localPosition", vector).Prop("localRotation", new PlugQuaternion(vector2)).UpdateType(UpdateType.TimeScaleIndependentUpdate)
			.Ease(EaseType.Linear)
			.OnComplete((TweenDelegate.TweenCallback)delegate
			{
				idleTimerLastTime = Time.realtimeSinceStartup;
			}));
	}

	private void LogPurchaseAfterPaymentAnalyticsEvent(string itemName)
	{
		if (!FlurryEvents.PaymentTime.HasValue)
		{
			return;
		}
		float? paymentTime = FlurryEvents.PaymentTime;
		float? num = ((!paymentTime.HasValue) ? null : new float?(Time.realtimeSinceStartup - paymentTime.Value));
		Dictionary<string, string> dictionary;
		if (num.HasValue && num.Value < 30f)
		{
			dictionary = new Dictionary<string, string>();
			dictionary.Add("0-30", itemName);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchase After Payment", parameters);
		}
		else
		{
			float? paymentTime2 = FlurryEvents.PaymentTime;
			float? num2 = ((!paymentTime2.HasValue) ? null : new float?(Time.realtimeSinceStartup - paymentTime2.Value));
			if (num2.HasValue && num2.Value < 60f)
			{
				dictionary = new Dictionary<string, string>();
				dictionary.Add("30-60", itemName);
				Dictionary<string, string> parameters2 = dictionary;
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchase After Payment", parameters2);
			}
			else
			{
				float? paymentTime3 = FlurryEvents.PaymentTime;
				float? num3 = ((!paymentTime3.HasValue) ? null : new float?(Time.realtimeSinceStartup - paymentTime3.Value));
				if (num3.HasValue && num3.Value < 90f)
				{
					dictionary = new Dictionary<string, string>();
					dictionary.Add("60-90", itemName);
					Dictionary<string, string> parameters3 = dictionary;
					FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchase After Payment", parameters3);
				}
				else
				{
					dictionary = new Dictionary<string, string>();
					dictionary.Add("90+", itemName);
					Dictionary<string, string> parameters4 = dictionary;
					FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchase After Payment", parameters4);
				}
			}
		}
		float? paymentTime4 = FlurryEvents.PaymentTime;
		float? num4 = ((!paymentTime4.HasValue) ? null : new float?(Time.realtimeSinceStartup - paymentTime4.Value));
		if (num4.HasValue && num4.Value < 30f)
		{
			dictionary = new Dictionary<string, string>();
			dictionary.Add("0-30", itemName);
			Dictionary<string, string> parameters5 = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchase After Payment Cumulative", parameters5);
		}
		float? paymentTime5 = FlurryEvents.PaymentTime;
		float? num5 = ((!paymentTime5.HasValue) ? null : new float?(Time.realtimeSinceStartup - paymentTime5.Value));
		if (num5.HasValue && num5.Value < 60f)
		{
			dictionary = new Dictionary<string, string>();
			dictionary.Add("0-60", itemName);
			Dictionary<string, string> parameters6 = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchase After Payment Cumulative", parameters6);
		}
		float? paymentTime6 = FlurryEvents.PaymentTime;
		float? num6 = ((!paymentTime6.HasValue) ? null : new float?(Time.realtimeSinceStartup - paymentTime6.Value));
		if (num6.HasValue && num6.Value < 90f)
		{
			dictionary = new Dictionary<string, string>();
			dictionary.Add("0-90", itemName);
			Dictionary<string, string> parameters7 = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchase After Payment Cumulative", parameters7);
		}
		dictionary = new Dictionary<string, string>();
		dictionary.Add("All", itemName);
		Dictionary<string, string> parameters8 = dictionary;
		FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchase After Payment Cumulative", parameters8);
	}

	public void PlayWeaponAnimation()
	{
		if (profile != null && weapon != null)
		{
			Animation component = weapon.GetComponent<WeaponSounds>().animationObject.GetComponent<Animation>();
			if (Time.timeScale != 0f)
			{
				if (component.GetClip("Profile") == null)
				{
					component.AddClip(profile, "Profile");
				}
				component.Play("Profile");
			}
			else
			{
				animationCoroutineRunner.StopAllCoroutines();
				if (component.GetClip("Profile") == null)
				{
					component.AddClip(profile, "Profile");
				}
				if (animationCoroutineRunner.gameObject.activeInHierarchy)
				{
					animationCoroutineRunner.StartPlay(component, "Profile", false, null);
				}
				else
				{
					Debug.LogWarning("Coroutine couldn't be started because the the game object 'AnimationCoroutineRunner' is inactive!");
				}
			}
		}
		MainMenu_Pers.GetComponent<Animation>().Stop();
		MainMenu_Pers.GetComponent<Animation>().Play("Idle");
	}

	public static Texture TextureForCat(int cc)
	{
		string text = (IsWeaponCategory((CategoryNames)cc) ? _CurrentWeaponSetIDs()[cc] : ((!IsWearCategory((CategoryNames)cc)) ? "potion" : sharedShop.WearForCat((CategoryNames)cc)));
		if (text == null)
		{
			return null;
		}
		text = ItemIDForPrefab(text, (CategoryNames)cc);
		int num = 1;
		if (IsWeaponCategory((CategoryNames)cc))
		{
			ItemRecord byTag = ItemDb.GetByTag(text);
			if ((byTag == null || !byTag.UseImagesFromFirstUpgrade) && (byTag == null || !byTag.TemporaryGun))
			{
				bool maxUpgrade;
				num = _CurrentNumberOfUpgrades(text, out maxUpgrade, (CategoryNames)cc);
			}
		}
		string text2 = ((!IsWeaponCategory((CategoryNames)cc)) ? text : (_TagForId(text) ?? string.Empty)) + "_icon" + num;
		string text3 = text2 + "_big";
		Texture texture = Resources.Load<Texture>("OfferIcons/" + text3);
		if (texture == null)
		{
			texture = Resources.Load<Texture>("ShopIcons/" + text2);
		}
		return texture;
	}

	public void SetIconModelsPositions(Transform t, CategoryNames c)
	{
		switch (c)
		{
		case CategoryNames.ArmorCategory:
		{
			t.transform.localPosition = new Vector3(0f, 0f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			float num5 = 1f;
			t.transform.localScale = new Vector3(num5, num5, num5);
			break;
		}
		case CategoryNames.BootsCategory:
		{
			t.transform.localPosition = new Vector3(-0.4f, -0.1f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(13f, 149f, 0f));
			float num4 = 75f;
			t.transform.localScale = new Vector3(num4, num4, num4);
			break;
		}
		case CategoryNames.CapesCategory:
		{
			t.transform.localPosition = new Vector3(-0.720093f, -0.00859833f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(0f, 30f, -15f));
			float num3 = 50f;
			t.transform.localScale = new Vector3(num3, num3, num3);
			break;
		}
		case CategoryNames.SkinsCategory:
			SkinsController.SetTransformParamtersForSkinModel(t);
			break;
		case CategoryNames.GearCategory:
		{
			t.transform.localPosition = new Vector3(4.648193f, 2.444565f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(0f, 30f, -30f));
			float num2 = 319.3023f;
			t.transform.localScale = new Vector3(num2, num2, num2);
			break;
		}
		case CategoryNames.HatsCategory:
		{
			t.transform.localPosition = new Vector3(-0.62f, -0.04f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(-75f, -165f, -90f));
			float num = 82.5f;
			t.transform.localScale = new Vector3(num, num, num);
			break;
		}
		}
	}

	public GameObject GetModelForCurrentEquippedt(CategoryNames c, List<GameObject> modelsList)
	{
		if (IsWeaponCategory(c))
		{
			UnityEngine.Object[] weaponsInGame = WeaponManager.sharedManager.weaponsInGame;
			for (int i = 0; i < weaponsInGame.Length; i++)
			{
				GameObject gameObject = (GameObject)weaponsInGame[i];
				if (_CurrentWeaponSetIDs()[(int)c] != null && ItemDb.GetByPrefabName(gameObject.name).Tag.Equals(_CurrentWeaponSetIDs()[(int)c]))
				{
					return gameObject;
				}
			}
			return null;
		}
		switch (c)
		{
		case CategoryNames.SkinsCategory:
			return modelsList[0];
		case CategoryNames.GearCategory:
			return modelsList[0];
		default:
		{
			string key = Defs.HatEquppedSN;
			if (c == CategoryNames.ArmorCategory)
			{
				key = Defs.ArmorNewEquppedSN;
			}
			if (c == CategoryNames.CapesCategory)
			{
				key = Defs.CapeEquppedSN;
			}
			if (c == CategoryNames.MaskCategory)
			{
				key = "MaskEquippedSN";
			}
			if (c == CategoryNames.BootsCategory)
			{
				key = Defs.BootsEquppedSN;
			}
			string @string = Storager.getString(key, false);
			foreach (GameObject models in modelsList)
			{
				if (models.name.Equals(@string))
				{
					return models;
				}
			}
			return null;
		}
		}
	}

	private void DisableGunflashes(GameObject root)
	{
		if (root == null)
		{
			return;
		}
		if (root.name.Equals("GunFlash"))
		{
			root.SetActive(false);
		}
		foreach (Transform item in root.transform)
		{
			if (!(null == item))
			{
				DisableGunflashes(item.gameObject);
			}
		}
	}

	public void UpdateIcons()
	{
		for (int i = 0; i < category.buttons.Length; i++)
		{
			UpdateIcon((CategoryNames)i);
		}
	}

	public void UpdateIcon(CategoryNames c, bool animateModel = false)
	{
		ToggleButton tb = category.buttons[(int)c];
		List<GameObject> toDestroy = new List<GameObject>();
		Player_move_c.PerformActionRecurs(tb.gameObject, delegate(Transform ch)
		{
			if (!(ch.gameObject == tb.gameObject) && !(ch.gameObject == tb.onButton.gameObject) && !(ch.gameObject == tb.offButton.gameObject) && !ch.gameObject.name.Equals("Label") && !ch.gameObject.name.Equals("ShopIcon"))
			{
				if (ch.gameObject.name.Equals("Sprite"))
				{
					ch.gameObject.SetActive(false);
				}
				else
				{
					toDestroy.Add(ch.gameObject);
				}
			}
		});
		foreach (GameObject item in toDestroy)
		{
			UnityEngine.Object.Destroy(item);
		}
		if (c == CategoryNames.SkinsCategory || (c == CategoryNames.CapesCategory && _currentCape.Equals("cape_Custom")))
		{
			List<GameObject> modelsList = FillModelsList(c);
			GameObject modelForCurrentEquippedt = GetModelForCurrentEquippedt(c, modelsList);
			if (modelForCurrentEquippedt != null)
			{
				AddModel(modelForCurrentEquippedt, delegate(GameObject manipulateObject, Vector3 positionShop, Vector3 rotationShop, string readableName, float sc, int _unusedTier, int _unusedLeague)
				{
					manipulateObject.transform.parent = tb.transform;
					if (c == CategoryNames.SkinsCategory)
					{
						Player_move_c.SetTextureRecursivelyFrom(manipulateObject, SkinsController.currentSkinForPers, new GameObject[0]);
					}
					float num = 0.5f;
					Transform transform = manipulateObject.transform;
					transform.localPosition = tb.onButton.transform.localPosition + positionShop * num;
					transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
					transform.Rotate(rotationShop, Space.World);
					transform.localScale = new Vector3(sc * num, sc * num, sc * num);
					if (c == CategoryNames.CapesCategory && _currentCape.Equals("cape_Custom") && SkinsController.capeUserTexture != null)
					{
						Player_move_c.SetTextureRecursivelyFrom(manipulateObject, SkinsController.capeUserTexture, new GameObject[0]);
					}
					SetIconModelsPositions(transform, c);
					if (animateModel)
					{
						Vector3 localScale = transform.localScale;
						transform.localScale *= 1.25f;
						HOTween.To(transform, 0.25f, new TweenParms().Prop("localScale", localScale).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.Linear)
							.OnComplete((TweenDelegate.TweenCallback)delegate
							{
							}));
					}
				}, c);
			}
			else
			{
				Player_move_c.PerformActionRecurs(tb.gameObject, delegate(Transform ch)
				{
					if (ch.gameObject.name.Equals("Sprite"))
					{
						ch.gameObject.SetActive(true);
					}
				});
			}
			Player_move_c.PerformActionRecurs(tb.gameObject, delegate(Transform ch)
			{
				if (ch.gameObject.name.Equals("ShopIcon"))
				{
					ch.GetComponent<UITexture>().mainTexture = null;
				}
			});
			return;
		}
		Texture texture = TextureForCat((int)c);
		if (texture != null)
		{
			Player_move_c.PerformActionRecurs(tb.gameObject, delegate(Transform ch)
			{
				if (ch.gameObject.name.Equals("Sprite"))
				{
					ch.gameObject.SetActive(false);
				}
				else if (ch.gameObject.name.Equals("ShopIcon"))
				{
					UITexture component = ch.GetComponent<UITexture>();
					if (component.mainTexture == null || !component.mainTexture.name.Equals(texture.name) || HOTween.IsTweening(ch))
					{
						HOTween.Kill(ch);
						ch.localScale = new Vector3(1.25f, 1.25f, 1.25f);
						HOTween.To(ch, 0.25f, new TweenParms().Prop("localScale", new Vector3(1f, 1f, 1f)).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.Linear)
							.OnComplete((TweenDelegate.TweenCallback)delegate
							{
							}));
					}
					component.mainTexture = texture;
				}
			});
			return;
		}
		Player_move_c.PerformActionRecurs(tb.gameObject, delegate(Transform ch)
		{
			if (ch.gameObject.name.Equals("Sprite"))
			{
				ch.gameObject.SetActive(true);
			}
			else if (ch.gameObject.name.Equals("ShopIcon"))
			{
				ch.GetComponent<UITexture>().mainTexture = null;
			}
		});
	}

	private static string _TagForId(string id)
	{
		foreach (List<string> upgrade in WeaponUpgrades.upgrades)
		{
			if (upgrade.Contains(id))
			{
				return upgrade[0];
			}
		}
		return id;
	}

	public void EquipWear(string tg)
	{
		EquipWearInCategory(tg, currentCategory, inGame);
	}

	public static void EquipWearInCategoryIfNotEquiped(string tg, CategoryNames cat, bool inGameLocal)
	{
		if (!Storager.hasKey(SnForWearCategory(cat)))
		{
			Storager.setString(SnForWearCategory(cat), NoneEquippedForWearCategory(cat), false);
		}
		if (!Storager.getString(SnForWearCategory(cat), false).Equals(tg))
		{
			EquipWearInCategory(tg, cat, inGameLocal);
		}
	}

	private static void EquipWearInCategory(string tg, CategoryNames cat, bool inGameLocal)
	{
		bool flag = !Defs.isMulti;
		string @string = Storager.getString(SnForWearCategory(cat), false);
		Player_move_c player_move_c = null;
		if (inGameLocal)
		{
			if (flag)
			{
				if (!SceneLoader.ActiveSceneName.Equals("LevelComplete") && !SceneLoader.ActiveSceneName.Equals("ChooseLevel"))
				{
					player_move_c = GameObject.FindGameObjectWithTag("Player").GetComponent<SkinName>().playerMoveC;
				}
			}
			else if (WeaponManager.sharedManager.myPlayer != null)
			{
				player_move_c = WeaponManager.sharedManager.myPlayerMoveC;
			}
		}
		SetAsEquippedAndSendToServer(tg, cat);
		sharedShop.SetWearForCategory(cat, tg);
		if (sharedShop.wearEquipAction != null)
		{
			sharedShop.wearEquipAction(cat, @string ?? NoneEquippedForWearCategory(cat), sharedShop.WearForCat(cat));
		}
		if (cat == CategoryNames.BootsCategory && inGameLocal && player_move_c != null)
		{
			if (!@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.bootsMethods.ContainsKey(@string))
			{
				Wear.bootsMethods[@string].Value(player_move_c, new Dictionary<string, object>());
			}
			if (Wear.bootsMethods.ContainsKey(tg))
			{
				Wear.bootsMethods[tg].Key(player_move_c, new Dictionary<string, object>());
			}
		}
		if (cat == CategoryNames.CapesCategory && inGameLocal && player_move_c != null)
		{
			if (!@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.capesMethods.ContainsKey(@string))
			{
				Wear.capesMethods[@string].Value(player_move_c, new Dictionary<string, object>());
			}
			if (Wear.capesMethods.ContainsKey(tg))
			{
				Wear.capesMethods[tg].Key(player_move_c, new Dictionary<string, object>());
			}
		}
		if (cat == CategoryNames.HatsCategory && inGameLocal && player_move_c != null)
		{
			if (!@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.hatsMethods.ContainsKey(@string))
			{
				Wear.hatsMethods[@string].Value(player_move_c, new Dictionary<string, object>());
			}
			if (Wear.hatsMethods.ContainsKey(tg))
			{
				Wear.hatsMethods[tg].Key(player_move_c, new Dictionary<string, object>());
			}
		}
		if (cat == CategoryNames.ArmorCategory && inGameLocal && player_move_c != null)
		{
			if (!@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.armorMethods.ContainsKey(@string))
			{
				Wear.armorMethods[@string].Value(player_move_c, new Dictionary<string, object>());
			}
			if (Wear.armorMethods.ContainsKey(tg))
			{
				Wear.armorMethods[tg].Key(player_move_c, new Dictionary<string, object>());
			}
		}
		if (GuiActive)
		{
			sharedShop.UpdateButtons();
			sharedShop.UpdateIcon(cat, true);
		}
	}

	public static void UnequipCurrentWearInCategory(CategoryNames cat, bool inGameLocal)
	{
		bool flag = !Defs.isMulti;
		string @string = Storager.getString(SnForWearCategory(cat), false);
		Player_move_c player_move_c = null;
		if (inGameLocal)
		{
			if (flag)
			{
				if (!SceneLoader.ActiveSceneName.Equals("LevelComplete") && !SceneLoader.ActiveSceneName.Equals("ChooseLevel"))
				{
					player_move_c = GameObject.FindGameObjectWithTag("Player").GetComponent<SkinName>().playerMoveC;
				}
			}
			else if (WeaponManager.sharedManager.myPlayer != null)
			{
				player_move_c = WeaponManager.sharedManager.myPlayerMoveC;
			}
		}
		Storager.setString(SnForWearCategory(cat), NoneEquippedForWearCategory(cat), false);
		switch (cat)
		{
		case CategoryNames.CapesCategory:
			FriendsController.sharedController.capeName = NoneEquippedForWearCategory(cat);
			break;
		case CategoryNames.MaskCategory:
			FriendsController.sharedController.maskName = NoneEquippedForWearCategory(cat);
			break;
		case CategoryNames.HatsCategory:
			FriendsController.sharedController.hatName = NoneEquippedForWearCategory(cat);
			break;
		case CategoryNames.BootsCategory:
			FriendsController.sharedController.bootsName = NoneEquippedForWearCategory(cat);
			break;
		case CategoryNames.ArmorCategory:
			FriendsController.sharedController.armorName = NoneEquippedForWearCategory(cat);
			break;
		}
		FriendsController.sharedController.SendOurData();
		sharedShop.SetWearForCategory(cat, NoneEquippedForWearCategory(cat));
		if (sharedShop.wearEquipAction != null)
		{
			sharedShop.wearEquipAction(cat, @string ?? NoneEquippedForWearCategory(cat), NoneEquippedForWearCategory(cat));
		}
		if (cat == CategoryNames.BootsCategory && inGameLocal && player_move_c != null && !@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.bootsMethods.ContainsKey(@string))
		{
			Wear.bootsMethods[@string].Value(player_move_c, new Dictionary<string, object>());
		}
		if (cat == CategoryNames.CapesCategory && inGameLocal && player_move_c != null && !@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.capesMethods.ContainsKey(@string))
		{
			Wear.capesMethods[@string].Value(player_move_c, new Dictionary<string, object>());
		}
		if (cat == CategoryNames.HatsCategory && inGameLocal && player_move_c != null && !@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.hatsMethods.ContainsKey(@string))
		{
			Wear.hatsMethods[@string].Value(player_move_c, new Dictionary<string, object>());
		}
		if (cat == CategoryNames.ArmorCategory && inGameLocal && player_move_c != null && !@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.armorMethods.ContainsKey(@string))
		{
			Wear.armorMethods[@string].Value(player_move_c, new Dictionary<string, object>());
		}
		if (sharedShop.wearUnequipAction != null)
		{
			sharedShop.wearUnequipAction(cat, @string ?? NoneEquippedForWearCategory(cat));
		}
		if (GuiActive)
		{
			sharedShop.UpdateIcon(cat);
		}
	}

	public static void ShowTryGunIfPossible(bool placeForGiveNewTryGun, Transform point, string layer, Action<string> onPurchase = null, Action onEnterCoinsShopAdditional = null, Action onExitoinsShopAdditional = null, Action<string> customEquipWearAction = null)
	{
		if (!Defs.isHunger && !placeForGiveNewTryGun && WeaponManager.sharedManager != null && WeaponManager.sharedManager.ExpiredTryGuns.Count > 0 && TrainingController.TrainingCompleted)
		{
			{
				string tg;
				foreach (string expiredTryGun in WeaponManager.sharedManager.ExpiredTryGuns)
				{
					tg = expiredTryGun;
					try
					{
						if (WeaponManager.sharedManager.weaponsInGame.FirstOrDefault((UnityEngine.Object w) => ItemDb.GetByPrefabName(w.name).Tag == tg) != null)
						{
							WeaponManager.sharedManager.ExpiredTryGuns.RemoveAll((string t) => t == tg);
							if (WeaponManager.LastBoughtTag(tg) == null)
							{
								ShowAddTryGun(tg, point, layer, onPurchase, onEnterCoinsShopAdditional, onExitoinsShopAdditional, customEquipWearAction, true);
								break;
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Exception in foreach (var tg in WeaponManager.sharedManager.ExpiredTryGuns): " + ex);
					}
				}
				return;
			}
		}
		if (Defs.isHunger || Defs.isDaterRegim || WeaponManager.sharedManager._currentFilterMap != 0 || !((!FriendsController.useBuffSystem) ? KillRateCheck.instance.giveWeapon : BuffSystem.instance.giveTryGun) || !TrainingController.TrainingCompleted)
		{
			return;
		}
		try
		{
			int maximumCoinBank = UpperCoinsBankBound();
			List<ItemRecord> source = (from prefabName in WeaponManager.tryGunsTable.SelectMany((KeyValuePair<CategoryNames, List<List<string>>> kvp) => kvp.Value[ExpController.OurTierForAnyPlace()])
				select ItemDb.GetByPrefabName(prefabName) into rec
				where rec.StorageId != null && Storager.getInt(rec.StorageId, true) == 0
				where !WeaponManager.sharedManager.IsAvailableTryGun(rec.Tag) && !WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(rec.Tag)
				select rec).ToList();
			List<ItemRecord> source2 = (from rec in source
				where rec.Price.Currency == "Coins"
				where PriceIfGunWillBeTryGun(rec.Tag) > maximumCoinBank
				select rec).Randomize().ToList();
			string text = null;
			if (source2.Any())
			{
				text = source2.First().Tag;
			}
			else
			{
				int maximumGemBank = UpperGemsBankBound();
				List<ItemRecord> source3 = (from rec in source
					where rec.Price.Currency == "GemsCurrency"
					where PriceIfGunWillBeTryGun(rec.Tag) > maximumGemBank
					select rec).Randomize().ToList();
				text = ((!source3.Any()) ? TryGunForCategoryWithMaxUnbought() : source3.First().Tag);
			}
			if (text != null)
			{
				ShowAddTryGun(text, point, layer, onPurchase, onEnterCoinsShopAdditional, onExitoinsShopAdditional, customEquipWearAction);
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("Exception in giving: " + ex2);
		}
	}

	private static int UpperCoinsBankBound()
	{
		int value = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
		value = Mathf.Clamp(value, 0, ExperienceController.addCoinsFromLevels.Length - 1);
		return Storager.getInt("Coins", false) + 30 + ExperienceController.addCoinsFromLevels[value];
	}

	private static int UpperGemsBankBound()
	{
		int value = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
		value = Mathf.Clamp(value, 0, ExperienceController.addGemsFromLevels.Length - 1);
		return Storager.getInt("GemsCurrency", false) + ExperienceController.addGemsFromLevels[value];
	}

	private static string TryGunForCategoryWithMaxUnbought()
	{
		List<CategoryNames> list = new List<CategoryNames>();
		list.Add(CategoryNames.PrimaryCategory);
		list.Add(CategoryNames.BackupCategory);
		list.Add(CategoryNames.MeleeCategory);
		list.Add(CategoryNames.SpecilCategory);
		list.Add(CategoryNames.SniperCategory);
		list.Add(CategoryNames.PremiumCategory);
		List<CategoryNames> list2 = list.Randomize().OrderBy(delegate(CategoryNames cat)
		{
			List<WeaponSounds> list4 = (from w in WeaponManager.sharedManager.weaponsInGame
				select ((GameObject)w).GetComponent<WeaponSounds>() into ws
				where ws.categoryNabor - 1 == (int)cat && ws.tier == ExpController.OurTierForAnyPlace()
				where WeaponManager.tagToStoreIDMapping.ContainsKey(ItemDb.GetByPrefabName(ws.name).Tag) && WeaponManager.storeIDtoDefsSNMapping.ContainsKey(WeaponManager.tagToStoreIDMapping[ItemDb.GetByPrefabName(ws.name).Tag]) && Storager.getInt(WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[ItemDb.GetByPrefabName(ws.name).Tag]], true) == 1
				select ws).ToList();
			return list4.Count;
		}).ToList();
		string result = null;
		for (int i = 0; i < list2.Count; i++)
		{
			CategoryNames cat2 = list2[i];
			List<WeaponSounds> source = (from ws in (from w in WeaponManager.sharedManager.weaponsInGame
					select ((GameObject)w).GetComponent<WeaponSounds>() into ws
					where ws.categoryNabor - 1 == (int)cat2 && ws.tier == ExpController.OurTierForAnyPlace()
					select ws).Where(delegate(WeaponSounds ws)
				{
					List<string> list3 = WeaponUpgrades.ChainForTag(ItemDb.GetByPrefabName(ws.name).Tag);
					return list3 == null || (list3.Count > 0 && list3[0] == ItemDb.GetByPrefabName(ws.name).Tag);
				})
				where WeaponManager.tagToStoreIDMapping.ContainsKey(ItemDb.GetByPrefabName(ws.name).Tag) && WeaponManager.storeIDtoDefsSNMapping.ContainsKey(WeaponManager.tagToStoreIDMapping[ItemDb.GetByPrefabName(ws.name).Tag]) && Storager.getInt(WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[ItemDb.GetByPrefabName(ws.name).Tag]], true) == 0
				where WeaponManager.tryGunsTable[cat2][ExpController.OurTierForAnyPlace()].Contains(ItemDb.GetByTag(ItemDb.GetByPrefabName(ws.name).Tag).PrefabName)
				where !WeaponManager.sharedManager.IsAvailableTryGun(ItemDb.GetByPrefabName(ws.name).Tag) && !WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(ItemDb.GetByPrefabName(ws.name).Tag)
				select ws).Randomize().ToList();
			if (source.Count() != 0)
			{
				result = ItemDb.GetByPrefabName(source.First().name).Tag;
				break;
			}
		}
		return result;
	}

	public static void ShowTempItemExpiredIfPossible(Transform point, string layer, Action<string> onPurchase = null, Action onEnterCoinsShopAdditional = null, Action onExitoinsShopAdditional = null, Action<string> customEquipWearAction = null)
	{
		List<string> list = new List<string>();
		foreach (string expiredItem in TempItemsController.sharedController.ExpiredItems)
		{
			if (TempItemsController.sharedController.CanShowExpiredBannerForTag(expiredItem))
			{
				ShowRentScreen(expiredItem, point, layer, LocalizationStore.Get("Key_1156"), LocalizationStore.Get("Key_1157"), onPurchase, onEnterCoinsShopAdditional, onExitoinsShopAdditional, customEquipWearAction);
				list.Add(expiredItem);
				break;
			}
		}
		foreach (string item in list)
		{
			TempItemsController.sharedController.ExpiredItems.Remove(item);
		}
	}

	public static bool ShowPremimAccountExpiredIfPossible(Transform point, string layer, string header = "", bool showOnlyIfExpired = true)
	{
		if (showOnlyIfExpired && (!PremiumAccountController.AccountHasExpired || !Defs2.CanShowPremiumAccountExpiredWindow))
		{
			return false;
		}
		if (point != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("PremiumAccount"));
			gameObject.transform.parent = point;
			Player_move_c.SetLayerRecursively(gameObject, LayerMask.NameToLayer(layer ?? "Default"));
			gameObject.transform.localPosition = new Vector3(0f, 0f, -130f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			PremiumAccountScreenController component = gameObject.GetComponent<PremiumAccountScreenController>();
			component.Header = header;
			PremiumAccountController.AccountHasExpired = false;
			return true;
		}
		return false;
	}

	public static void GiveArmorArmy1OrNoviceArmor()
	{
		ProvideShopItemOnStarterPackBoguht(CategoryNames.ArmorCategory, (Storager.getInt("Training.NoviceArmorUsedKey", false) != 1) ? "Armor_Army_1" : "Armor_Novice", 1, false, 0, null, null, true, Storager.getInt("Training.NoviceArmorUsedKey", false) == 1, false);
	}

	private void setInArmorCategoryNotOnArmorRemovedNoviceArmor()
	{
		try
		{
			StopCoroutine("Blink");
			TrainingState trainingState = trainingStateRemoveNoviceArmor;
			if (trainingState == TrainingState.OnArmor)
			{
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
			}
			trainingStateRemoveNoviceArmor = TrainingState.InArmorCategoryNotOnArmor;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setInArmorCategoryNotOnArmorRemovedNoviceArmor: " + ex);
		}
	}

	private void setNotInArmorCategoryRemovedNoviceArmor()
	{
		try
		{
			StopCoroutine("Blink");
			TrainingState trainingState = trainingStateRemoveNoviceArmor;
			if (trainingState == TrainingState.OnArmor)
			{
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
			}
			trainingStateRemoveNoviceArmor = TrainingState.NotInArmorCategory;
			toBlink = category.buttons[7].offButton.tweenTarget.GetComponent<UISprite>();
			StartCoroutine("Blink", new string[2] { "green_btn", "trans_btn" });
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setNotInArmorCategoryRemovedNoviceArmor: " + ex);
		}
	}

	private void setOnArmorRemovedNoviceArmor()
	{
		try
		{
			StopCoroutine("Blink");
			TrainingState trainingState = trainingStateRemoveNoviceArmor;
			if (trainingState == TrainingState.NotInArmorCategory)
			{
				category.buttons[7].offButton.tweenTarget.GetComponent<UISprite>().spriteName = "trans_btn";
				category.buttons[7].offButton.normalSprite = "trans_btn";
			}
			trainingStateRemoveNoviceArmor = TrainingState.OnArmor;
			toBlink = equips[1].tweenTarget.GetComponent<UISprite>();
			StartCoroutine("Blink", new string[2] { "or_btn", "green_btn" });
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setOnArmorRemovedNoviceArmor: " + ex);
		}
	}

	private void setNotInSniperCategory()
	{
		try
		{
			StopCoroutine("Blink");
			TrainingState trainingState = this.trainingState;
			if (trainingState == TrainingState.OnSniperRifle)
			{
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
			}
			this.trainingState = TrainingState.NotInSniperCategory;
			toBlink = category.buttons[4].offButton.tweenTarget.GetComponent<UISprite>();
			StartCoroutine("Blink", new string[2] { "green_btn", "trans_btn" });
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setNotInSniperCategory: " + ex);
		}
	}

	private void setOnSniperRifle()
	{
		try
		{
			StopCoroutine("Blink");
			if (trainingState == TrainingState.NotInSniperCategory)
			{
				category.buttons[4].offButton.tweenTarget.GetComponent<UISprite>().spriteName = "trans_btn";
				category.buttons[4].offButton.normalSprite = "trans_btn";
			}
			trainingState = TrainingState.OnSniperRifle;
			toBlink = equips[1].tweenTarget.GetComponent<UISprite>();
			StartCoroutine("Blink", new string[2] { "or_btn", "green_btn" });
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setOnSniperRifle: " + ex);
		}
	}

	private void setInSniperCategoryNotOnSniperRifle()
	{
		try
		{
			StopCoroutine("Blink");
			TrainingState trainingState = this.trainingState;
			if (trainingState == TrainingState.OnSniperRifle)
			{
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
			}
			this.trainingState = TrainingState.InSniperCategoryNotOnSniperRifle;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setInSniperCategoryNotOnSniperRifle: " + ex);
		}
	}

	private void setNotInArmorCategory()
	{
		try
		{
			StopCoroutine("Blink");
			switch (trainingState)
			{
			case TrainingState.OnSniperRifle:
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
				break;
			case TrainingState.OnArmor:
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
				break;
			}
			trainingState = TrainingState.NotInArmorCategory;
			toBlink = category.buttons[7].offButton.tweenTarget.GetComponent<UISprite>();
			StartCoroutine("Blink", new string[2] { "green_btn", "trans_btn" });
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setNotInArmorCategory: " + ex);
		}
	}

	private void setOnArmor()
	{
		try
		{
			StopCoroutine("Blink");
			TrainingState trainingState = this.trainingState;
			if (trainingState == TrainingState.NotInArmorCategory)
			{
				category.buttons[7].offButton.tweenTarget.GetComponent<UISprite>().spriteName = "trans_btn";
				category.buttons[7].offButton.normalSprite = "trans_btn";
			}
			this.trainingState = TrainingState.OnArmor;
			toBlink = equips[1].tweenTarget.GetComponent<UISprite>();
			StartCoroutine("Blink", new string[2] { "or_btn", "green_btn" });
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setOnArmor: " + ex);
		}
	}

	private void setInArmorCategoryNotOnArmor()
	{
		try
		{
			StopCoroutine("Blink");
			TrainingState trainingState = this.trainingState;
			if (trainingState == TrainingState.OnArmor)
			{
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
			}
			this.trainingState = TrainingState.InArmorCategoryNotOnArmor;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setInArmorCategoryNotOnArmor: " + ex);
		}
	}

	private void setBackBlinking()
	{
		try
		{
			StopCoroutine("Blink");
			switch (trainingState)
			{
			case TrainingState.OnArmor:
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
				break;
			case TrainingState.OnSniperRifle:
				equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
				equips[1].normalSprite = "or_btn";
				break;
			}
			trainingState = TrainingState.BackBlinking;
			toBlink = backButton.tweenTarget.GetComponent<UISprite>();
			StartCoroutine("Blink", new string[2] { "yell_btn", "green_btn" });
			trainingColliders.SetActive(false);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShopNGUIController.setBackBlinking: " + ex);
		}
	}

	private void ForceResetTrainingState()
	{
		try
		{
			if (_setTrainingStateMethods == null)
			{
				_setTrainingStateMethods = new Dictionary<TrainingState, Action>
				{
					{
						TrainingState.NotInSniperCategory,
						setNotInSniperCategory
					},
					{
						TrainingState.OnSniperRifle,
						setOnSniperRifle
					},
					{
						TrainingState.InSniperCategoryNotOnSniperRifle,
						setInSniperCategoryNotOnSniperRifle
					},
					{
						TrainingState.NotInArmorCategory,
						setNotInArmorCategory
					},
					{
						TrainingState.OnArmor,
						setOnArmor
					},
					{
						TrainingState.InArmorCategoryNotOnArmor,
						setInArmorCategoryNotOnArmor
					},
					{
						TrainingState.BackBlinking,
						setBackBlinking
					}
				};
			}
			_setTrainingStateMethods[trainingState]();
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ForceResetTrainingState: " + ex);
		}
	}

	private void HideAllTrainingInterface()
	{
		try
		{
			foreach (GameObject trainingTip in trainingTips)
			{
				trainingTip.SetActive(false);
			}
			trainingColliders.SetActive(false);
			StopCoroutine("Blink");
			equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
			equips[1].normalSprite = "or_btn";
			category.buttons[4].offButton.tweenTarget.GetComponent<UISprite>().spriteName = "trans_btn";
			category.buttons[4].offButton.normalSprite = "trans_btn";
			category.buttons[7].offButton.tweenTarget.GetComponent<UISprite>().spriteName = "trans_btn";
			category.buttons[7].offButton.normalSprite = "trans_btn";
			backButton.tweenTarget.GetComponent<UISprite>().spriteName = "yell_btn";
			backButton.normalSprite = "yell_btn";
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in HideAllTrainingInterface: " + ex);
		}
	}

	private void HideAllTrainingInterfaceRemovedNoviceArmor()
	{
		try
		{
			foreach (GameObject item in trainingTipsRemovedNoviceArmor)
			{
				item.SetActive(false);
			}
			trainingColliders.SetActive(false);
			trainingRemoveNoviceArmorCollider.SetActive(false);
			StopCoroutine("Blink");
			equips[1].tweenTarget.GetComponent<UISprite>().spriteName = "or_btn";
			equips[1].normalSprite = "or_btn";
			category.buttons[7].offButton.tweenTarget.GetComponent<UISprite>().spriteName = "trans_btn";
			category.buttons[7].offButton.normalSprite = "trans_btn";
			backButton.tweenTarget.GetComponent<UISprite>().spriteName = "yell_btn";
			backButton.normalSprite = "yell_btn";
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in HideAllTrainingInterfaceRemovedNoviceArmor: " + ex);
		}
	}

	private IEnumerator Blink(string[] images)
	{
		while (true)
		{
			try
			{
				toBlink.spriteName = ((!toBlink.spriteName.Equals(images[0])) ? images[0] : images[1]);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				Debug.LogError("Exception in ShopNGUIController.Blink: " + e);
			}
			yield return StartCoroutine(MyWaitForSeconds(0.5f));
		}
	}

	public static void FireWeaponOrArmorBought()
	{
		Action gunOrArmorBought = ShopNGUIController.GunOrArmorBought;
		if (gunOrArmorBought != null)
		{
			gunOrArmorBought();
		}
	}

	public void SetOfferID(string oid)
	{
		offerID = oid;
	}

	public void HandleFacebookButton()
	{
		_isFromPromoActions = false;
	}

	public void HandleProfileButton()
	{
		GameObject mainMenu = GameObject.Find("MainMenuNGUI");
		if ((bool)mainMenu)
		{
			mainMenu.SetActive(false);
		}
		GameObject inGameGui = GameObject.FindWithTag("InGameGUI");
		if ((bool)inGameGui)
		{
			inGameGui.SetActive(false);
		}
		GameObject networkTable = GameObject.FindWithTag("NetworkStartTableNGUI");
		if ((bool)networkTable)
		{
			networkTable.SetActive(false);
		}
		GuiActive = false;
		Action action = delegate
		{
		};
		ProfileController.Instance.DesiredWeaponTag = _assignedWeaponTag;
		ProfileController.Instance.ShowInterface(action, delegate
		{
			GuiActive = true;
			if ((bool)mainMenu)
			{
				mainMenu.SetActive(true);
			}
			if ((bool)inGameGui)
			{
				inGameGui.SetActive(true);
			}
			if ((bool)networkTable)
			{
				networkTable.SetActive(true);
			}
		});
	}

	public static bool IsWeaponCategory(CategoryNames c)
	{
		return c < CategoryNames.HatsCategory;
	}

	public static bool IsWearCategory(CategoryNames c)
	{
		return c == CategoryNames.CapesCategory || c == CategoryNames.BootsCategory || c == CategoryNames.HatsCategory || c == CategoryNames.ArmorCategory || c == CategoryNames.MaskCategory;
	}

	private static string[] _CurrentWeaponSetIDs()
	{
		string[] array = new string[6];
		WeaponManager sharedManager = WeaponManager.sharedManager;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (num >= sharedManager.playerWeapons.Count)
			{
				array[i] = null;
				continue;
			}
			Weapon weapon = sharedManager.playerWeapons[num] as Weapon;
			if (weapon.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == i)
			{
				num++;
				array[i] = ItemDb.GetByPrefabName(weapon.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
			}
			else
			{
				array[i] = null;
			}
		}
		return array;
	}

	public static void ShowAddTryGun(string gunTag, Transform point, string lr, Action<string> onPurchase = null, Action onEnterCoinsShopAdditional = null, Action onExitCoinsShopAdditional = null, Action<string> customEquipWearAction = null, bool expiredTryGun = false)
	{
		try
		{
			GameObject original = Resources.Load<GameObject>("TryGunScreen");
			GameObject gameObject = UnityEngine.Object.Instantiate(original);
			TryGunScreenController component = gameObject.GetComponent<TryGunScreenController>();
			Player_move_c.SetLayerRecursively(component.gameObject, LayerMask.NameToLayer(lr));
			component.transform.parent = point;
			component.transform.localPosition = new Vector3(0f, 0f, -130f);
			component.transform.localScale = new Vector3(1f, 1f, 1f);
			if (expiredTryGun)
			{
				WeaponManager.sharedManager.AddTryGunPromo(gunTag);
			}
			component.ItemTag = gunTag;
			component.onPurchaseCustomAction = onPurchase;
			component.onEnterCoinsShopAdditionalAction = onEnterCoinsShopAdditional;
			component.onExitCoinsShopAdditionalAction = onExitCoinsShopAdditional;
			component.customEquipWearAction = customEquipWearAction;
			component.ExpiredTryGun = expiredTryGun;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShowAddTryGun: " + ex);
		}
	}

	public static void ShowRentScreen(string itemTag, Transform point, string lr, string hdr, string rentText, Action<string> onPurchase = null, Action onEnterCoinsShopAdditional = null, Action onExitCoinsShopAdditional = null, Action<string> customEquipWearAction = null)
	{
		GameObject original = Resources.Load<GameObject>("RentScreen");
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		RentScreenController component = gameObject.GetComponent<RentScreenController>();
		Player_move_c.SetLayerRecursively(component.gameObject, LayerMask.NameToLayer(lr));
		component.transform.parent = point;
		component.transform.localPosition = new Vector3(0f, 0f, -130f);
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		component.ItemTag = itemTag;
		component.Header = hdr;
		component.RentFor = rentText;
		component.onPurchaseCustomAction = onPurchase;
		component.onEnterCoinsShopAdditionalAction = onEnterCoinsShopAdditional;
		component.onExitCoinsShopAdditionalAction = onExitCoinsShopAdditional;
		component.customEquipWearAction = customEquipWearAction;
	}

	public void HandleRentButton()
	{
		if (Defs.isSoundFX)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		bool hasBefore = TempItemsController.sharedController != null && TempItemsController.sharedController.ContainsItem(viewedId);
		CategoryNames c = currentCategory;
		ShowRentScreen(viewedId, rentScreenPoint, "NGUIShop", LocalizationStore.Get("Key_1149"), LocalizationStore.Get("Key_1150"), delegate
		{
			chosenId = WeaponManager.LastBoughtTag(viewedId);
			viewedId = ((currentCategory != CategoryNames.GearCategory) ? chosenId : GearManager.NameForUpgrade(GearManager.HolderQuantityForID(viewedId), GearManager.CurrentNumberOfUphradesForGear(GearManager.HolderQuantityForID(viewedId))));
			UpdateButtons();
			UpdateIcon(c);
			if (hasBefore)
			{
				purchaseSuccessfulRent.SetActive(true);
				_timePurchaseRentSuccessfulShown = Time.realtimeSinceStartup;
			}
			else
			{
				purchaseSuccessful.SetActive(true);
				_timePurchaseSuccessfulShown = Time.realtimeSinceStartup;
			}
		}, delegate
		{
			SetBankCamerasEnabled();
		}, delegate
		{
			SetOtherCamerasEnabled(false);
		}, delegate(string tg)
		{
			if (tg != null)
			{
				EquipWearInCategoryIfNotEquiped(tg, c, inGame);
			}
		});
	}

	public void HandlePropertiesInfoButton()
	{
		if (WeaponCategory)
		{
			if (Defs.isSoundFX)
			{
				ButtonClickSound.Instance.PlayClick();
			}
			infoScreen.Show(currentCategory == CategoryNames.MeleeCategory);
		}
	}

	private void ReloadCarousel(string idToChoose = null)
	{
		ShopCarouselElement[] componentsInChildren = wrapContent.GetComponentsInChildren<ShopCarouselElement>(true);
		ShopCarouselElement[] array = componentsInChildren;
		foreach (ShopCarouselElement shopCarouselElement in array)
		{
			UnityEngine.Object.Destroy(shopCarouselElement.gameObject);
			shopCarouselElement.transform.parent = null;
		}
		wrapContent.Reposition();
		List<GameObject> list = FillModelsList(currentCategory);
		string[] array2 = null;
		if (currentCategory == CategoryNames.SkinsCategory)
		{
			array2 = new string[list.Count];
			List<string> list2 = SkinsController.skinsForPers.Keys.ToList();
			if (!ShowLockedFacebookSkin())
			{
				list2.Remove("61");
			}
			list2.Sort(delegate(string kvp1, string kvp2)
			{
				//Discarded unreachable code: IL_00be, IL_00cc
				try
				{
					if (kvp1 == "61" || kvp2 == "61")
					{
						string text = 4.ToString();
						string text2 = ((!(kvp1 == "61")) ? kvp1 : kvp2);
						if (text2 == text)
						{
							text2 = 6.ToString();
						}
						if (kvp1 == "61")
						{
							return int.Parse(text).CompareTo(int.Parse(text2));
						}
						return int.Parse(text2).CompareTo(int.Parse(text));
					}
					return int.Parse(kvp1).CompareTo(int.Parse(kvp2));
				}
				catch (Exception)
				{
					return 0;
				}
			});
			int num = list2.FindIndex(delegate(string kvp)
			{
				//Discarded unreachable code: IL_0016, IL_0023
				try
				{
					return int.Parse(kvp) >= 1000;
				}
				catch (Exception)
				{
					return false;
				}
			});
			int num2 = 0;
			if (num >= 0 && num < list2.Count)
			{
				List<string> list3 = new List<string>();
				list3.AddRange(list2.GetRange(num, list2.Count - num));
				list3.Reverse();
				list3.CopyTo(array2);
				num2 = list3.Count;
				array2[num2] = "CustomSkinID";
				num2++;
				list2.CopyTo(0, array2, num2, num);
			}
			else
			{
				array2[0] = "CustomSkinID";
				num2++;
				list2.CopyTo(array2, 1);
			}
		}
		if (EnableConfigurePos)
		{
			List<string> list4 = new List<string>();
			List<GameObject> list5 = new List<GameObject>();
			for (int j = 0; j < list.Count; j++)
			{
				List<string> list6 = null;
				foreach (List<string> upgrade in WeaponUpgrades.upgrades)
				{
					if (upgrade.Contains((!WeaponCategory) ? list[j].tag : ItemDb.GetByPrefabName(list[j].name.Replace("(Clone)", string.Empty)).Tag))
					{
						list6 = upgrade;
						break;
					}
				}
				if (list6 == null)
				{
					list5.Add(list[j]);
					continue;
				}
				for (int k = 0; k < list6.Count; k++)
				{
					UnityEngine.Object[] weaponsInGame = WeaponManager.sharedManager.weaponsInGame;
					for (int l = 0; l < weaponsInGame.Length; l++)
					{
						GameObject gameObject = (GameObject)weaponsInGame[l];
						if (ItemDb.GetByPrefabName(gameObject.name).Tag.Equals(list6[k]))
						{
							list5.Add(gameObject);
							break;
						}
					}
				}
			}
			list = list5;
		}
		for (int m = 0; m < list.Count; m++)
		{
			GameObject original = Resources.Load<GameObject>("ShopCarouselElement");
			GameObject gameObject2 = UnityEngine.Object.Instantiate(original);
			gameObject2.transform.parent = wrapContent.transform;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			GameObject gameObject3 = list[m];
			gameObject2.name = m.ToString("D7");
			if (WeaponCategory)
			{
				gameObject2.name = gameObject2.name + "_" + int.Parse(gameObject3.name.Substring("Weapon".Length)).ToString("D5");
			}
			ShopCarouselElement sce = gameObject2.GetComponent<ShopCarouselElement>();
			string itenID = ((!WeaponCategory) ? ((currentCategory != CategoryNames.SkinsCategory) ? ItemIDForPrefabReverse(gameObject3.name, currentCategory) : array2[m]) : ItemDb.GetByPrefabName(gameObject3.name.Replace("(Clone)", string.Empty)).Tag);
			AddModel((!WeaponCategory) ? gameObject3 : WeaponManager.InnerPrefabForWeapon(gameObject3), delegate(GameObject manipulateObject, Vector3 positionShop, Vector3 rotationShop, string readableName, float scaleCoefShop, int tier, int league)
			{
				manipulateObject.transform.parent = sce.transform;
				sce.itemID = itenID;
				sce.baseScale = new Vector3(scaleCoefShop, scaleCoefShop, scaleCoefShop);
				sce.model = manipulateObject.transform;
				sce.ourPosition = positionShop;
				sce.SetPos((!EnableConfigurePos) ? 0f : 1f, 0f);
				sce.model.Rotate(rotationShop, Space.World);
				sce.readableName = readableName ?? string.Empty;
				if (currentCategory == CategoryNames.GearCategory)
				{
					sce.showQuantity = true;
					sce.SetQuantity();
				}
				bool flag = false;
				if (WeaponCategory && WeaponManager.tagToStoreIDMapping.ContainsKey(sce.itemID) && WeaponManager.storeIDtoDefsSNMapping.ContainsKey(WeaponManager.tagToStoreIDMapping[sce.itemID]))
				{
					flag = Storager.getInt(WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[sce.itemID]], true) > 0;
				}
				if (ExpController.Instance != null && ExpController.Instance.OurTier < tier && tier < 100 && ((WeaponCategory && sce.itemID.Equals(WeaponManager.FirstUnboughtOrForOurTier(sce.itemID)) && !flag) || (WearCategory && sce.itemID.Equals(WeaponManager.FirstUnboughtTag(sce.itemID)) && sce.itemID != "cape_Custom" && sce.itemID != "boots_tabi")) && sce.locked != null)
				{
					sce.locked.SetActive(true);
				}
				if ((WeaponCategory && !sce.itemID.Equals(WeaponManager.FirstUnboughtOrForOurTier(sce.itemID)) && tier < 100) || (WearCategory && !sce.itemID.Equals(WeaponManager.FirstUnboughtTag(sce.itemID))))
				{
					if (sce.arrow != null)
					{
						sce.arrow.gameObject.SetActive(true);
					}
					if (currentCategory == CategoryNames.HatsCategory)
					{
						sce.arrnoInitialPos = new Vector3(85f, sce.arrnoInitialPos.y, sce.arrnoInitialPos.z);
					}
					if (currentCategory == CategoryNames.ArmorCategory)
					{
						sce.arrnoInitialPos = new Vector3(105f, sce.arrnoInitialPos.y, sce.arrnoInitialPos.z);
					}
					if (currentCategory == CategoryNames.CapesCategory)
					{
						sce.arrnoInitialPos = new Vector3(75f, sce.arrnoInitialPos.y, sce.arrnoInitialPos.z);
					}
					if (currentCategory == CategoryNames.BootsCategory)
					{
						sce.arrnoInitialPos = new Vector3(81f, sce.arrnoInitialPos.y, sce.arrnoInitialPos.z);
					}
					if (currentCategory == CategoryNames.MaskCategory)
					{
						sce.arrnoInitialPos = new Vector3(75f, sce.arrnoInitialPos.y, sce.arrnoInitialPos.z);
					}
				}
				if (currentCategory == CategoryNames.SkinsCategory)
				{
					sce.readableName = (itenID.Equals("CustomSkinID") ? LocalizationStore.Get("Key_1090") : ((!SkinsController.skinsNamesForPers.ContainsKey(itenID)) ? string.Empty : SkinsController.skinsNamesForPers[itenID]));
					Player_move_c.SetTextureRecursivelyFrom(manipulateObject, (!itenID.Equals("CustomSkinID")) ? SkinsController.skinsForPers[itenID] : (Resources.Load("Skin_Start") as Texture), new GameObject[0]);
				}
				if (PromoActionsManager.sharedManager.topSellers.Contains(itenID))
				{
					sce.showTS = true;
				}
				if (PromoActionsManager.sharedManager.news.Contains(itenID))
				{
					sce.showNew = true;
				}
				if (itenID.Equals("cape_Custom") && SkinsController.capeUserTexture != null)
				{
					Player_move_c.SetTextureRecursivelyFrom(manipulateObject, SkinsController.capeUserTexture, new GameObject[0]);
				}
			}, currentCategory, false, (!WeaponCategory) ? null : gameObject3.GetComponent<WeaponSounds>());
		}
		wrapContent.Reposition();
		ChooseCarouselItem(idToChoose ?? viewedId, true);
	}

	public static void AddModel(GameObject pref, Action7<GameObject, Vector3, Vector3, string, float, int, int> act, CategoryNames c, bool isButtonInGameGui = false, WeaponSounds wsForPos = null)
	{
		float arg = 150f;
		Vector3 arg2 = Vector3.zero;
		Vector3 arg3 = Vector3.zero;
		GameObject gameObject = null;
		int arg4 = 0;
		int arg5 = 0;
		string arg6 = null;
		if (IsWeaponCategory(c))
		{
			arg = wsForPos.scaleShop;
			arg2 = wsForPos.positionShop;
			arg3 = wsForPos.rotationShop;
			gameObject = pref.GetComponent<InnerWeaponPars>().bonusPrefab;
			try
			{
				ItemRecord byPrefabName = ItemDb.GetByPrefabName(wsForPos.name.Replace("(Clone)", string.Empty));
				string text = WeaponUpgrades.TagOfFirstUpgrade(byPrefabName.Tag);
				ItemRecord firstRec = ItemDb.GetByTag(text);
				arg6 = WeaponManager.AllWrapperPrefabs().First((WeaponSounds weapon) => weapon.name == firstRec.PrefabName).shopName;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error in getting shop name of first upgrade: " + ex);
				arg6 = wsForPos.shopName;
			}
			arg4 = wsForPos.tier;
		}
		else
		{
			switch (c)
			{
			case CategoryNames.SkinsCategory:
			{
				gameObject = UnityEngine.Object.Instantiate(pref);
				ShopPositionParams component2 = pref.GetComponent<ShopPositionParams>();
				arg3 = component2.rotationShop;
				arg = component2.scaleShop;
				arg2 = component2.positionShop;
				arg4 = component2.tier;
				break;
			}
			case CategoryNames.HatsCategory:
			case CategoryNames.ArmorCategory:
			case CategoryNames.CapesCategory:
			case CategoryNames.BootsCategory:
			case CategoryNames.GearCategory:
			case CategoryNames.MaskCategory:
			{
				gameObject = pref.transform.GetChild(0).gameObject;
				ShopPositionParams component = pref.GetComponent<ShopPositionParams>();
				arg3 = component.rotationShop;
				arg = component.scaleShop;
				arg2 = component.positionShop;
				arg6 = component.shopName;
				arg4 = component.tier;
				arg5 = component.League;
				break;
			}
			}
		}
		Vector3 localPosition = Vector3.zero;
		GameObject gameObject2 = null;
		if (c == CategoryNames.SkinsCategory)
		{
			gameObject2 = gameObject;
			localPosition = new Vector3(0f, -1f, 0f);
		}
		else if (gameObject != null)
		{
			Material[] array = null;
			Mesh mesh = null;
			SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
			if (skinnedMeshRenderer == null)
			{
				SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					skinnedMeshRenderer = componentsInChildren[0];
				}
			}
			if (skinnedMeshRenderer != null)
			{
				array = skinnedMeshRenderer.sharedMaterials;
				mesh = skinnedMeshRenderer.sharedMesh;
			}
			else
			{
				MeshFilter component3 = gameObject.GetComponent<MeshFilter>();
				MeshRenderer component4 = gameObject.GetComponent<MeshRenderer>();
				if (component3 != null)
				{
					mesh = component3.sharedMesh;
				}
				if (component4 != null)
				{
					array = component4.sharedMaterials;
				}
			}
			if (array != null && mesh != null)
			{
				gameObject2 = new GameObject();
				gameObject2.AddComponent<MeshFilter>().sharedMesh = mesh;
				MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
				meshRenderer.materials = array;
				localPosition = -meshRenderer.bounds.center;
			}
		}
		try
		{
			DisableLightProbesRecursively(gameObject2);
		}
		catch (Exception ex2)
		{
			Debug.LogError("Exception DisableLightProbesRecursively: " + ex2);
		}
		GameObject gameObject3 = new GameObject();
		gameObject3.name = gameObject2.name;
		gameObject2.transform.localPosition = localPosition;
		gameObject2.transform.parent = gameObject3.transform;
		Player_move_c.SetLayerRecursively(gameObject3, LayerMask.NameToLayer("NGUIShop"));
		if (act != null)
		{
			act(gameObject3, arg2, arg3, arg6, arg, arg4, arg5);
		}
	}

	public void ChooseCarouselItem(string itemID, bool moveCarousel = false, bool setManuallyToChosen = false)
	{
		if (itemID == null)
		{
			if (WeaponCategory)
			{
				UpdatePersWithNewItem();
			}
			return;
		}
		ShopCarouselElement[] array = wrapContent.GetComponentsInChildren<ShopCarouselElement>(true);
		if (array == null)
		{
			array = new ShopCarouselElement[0];
		}
		ShopCarouselElement[] array2 = array;
		foreach (ShopCarouselElement shopCarouselElement in array2)
		{
			if (!shopCarouselElement.itemID.Equals(itemID))
			{
				continue;
			}
			if (moveCarousel || setManuallyToChosen)
			{
				SpringPanel component = scrollViewPanel.GetComponent<SpringPanel>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				if (scrollViewPanel.gameObject.activeInHierarchy)
				{
					scrollViewPanel.GetComponent<UIScrollView>().MoveRelative(new Vector3(0f - shopCarouselElement.transform.localPosition.x - scrollViewPanel.transform.localPosition.x, scrollViewPanel.transform.localPosition.y, scrollViewPanel.transform.localPosition.z));
				}
				wrapContent.Reposition();
			}
			viewedId = itemID;
			UpdatePersWithNewItem();
			UpdateButtons();
			UpdateItemParameters();
			caption.text = shopCarouselElement.readableName ?? string.Empty;
			caption.applyGradient = TempItemsController.PriceCoefs.ContainsKey(itemID);
			SetLabelsForElem(shopCarouselElement);
			try
			{
				if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
				{
					if (trainingState == TrainingState.OnSniperRifle && viewedId != null && viewedId != WeaponTags.HunterRifleTag)
					{
						setInSniperCategoryNotOnSniperRifle();
					}
					else if (trainingState == TrainingState.InSniperCategoryNotOnSniperRifle && viewedId != null && viewedId == WeaponTags.HunterRifleTag)
					{
						setOnSniperRifle();
					}
					else if (trainingState == TrainingState.OnArmor && viewedId != null && viewedId != (WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1"))
					{
						setInArmorCategoryNotOnArmor();
					}
					else if (trainingState == TrainingState.InArmorCategoryNotOnArmor && viewedId != null && viewedId == (WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1"))
					{
						setOnArmor();
					}
				}
				if (InTrainingAfterNoviceArmorRemoved)
				{
					if (trainingStateRemoveNoviceArmor == TrainingState.OnArmor && viewedId != null && viewedId != (WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1"))
					{
						setInArmorCategoryNotOnArmorRemovedNoviceArmor();
					}
					else if (trainingStateRemoveNoviceArmor == TrainingState.InArmorCategoryNotOnArmor && viewedId != null && viewedId == (WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1"))
					{
						setOnArmorRemovedNoviceArmor();
					}
				}
				break;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in training in ChooseCarouselItem: " + ex);
				break;
			}
		}
	}

	private int TierForWear(string v, CategoryNames c)
	{
		if (v == null || v == "Armor_Novice")
		{
			return 0;
		}
		List<string> list = null;
		foreach (List<string> item in Wear.wear[c])
		{
			if (item.Contains(v))
			{
				list = item;
				break;
			}
		}
		if (list == null)
		{
			return 0;
		}
		List<GameObject> list2 = null;
		switch (c)
		{
		case CategoryNames.HatsCategory:
			list2 = hats;
			break;
		case CategoryNames.ArmorCategory:
			list2 = armor;
			break;
		case CategoryNames.CapesCategory:
			list2 = capes;
			break;
		case CategoryNames.BootsCategory:
			list2 = boots;
			break;
		case CategoryNames.MaskCategory:
			list2 = masks;
			break;
		}
		foreach (GameObject item2 in list2)
		{
			if (item2.name.Equals(v))
			{
				return item2.GetComponent<ShopPositionParams>().tier;
			}
		}
		return 0;
	}

	private void SetUpUpgradesAndTiers(bool bought, ref bool buyActive, ref bool upgradeActive, ref bool saleActive, ref bool needTierActive, ref bool rentActive, ref bool saleRentActive)
	{
		bool flag = TempItemsController.PriceCoefs.ContainsKey(viewedId);
		bool maxUpgrade = false;
		int num = ((viewedId == null) ? (-1) : _CurrentNumberOfWearUpgrades(viewedId, out maxUpgrade, currentCategory));
		bool flag2 = maxUpgrade;
		if ((!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted) || InTrainingAfterNoviceArmorRemoved)
		{
			buyActive = false;
			upgradeActive = false;
		}
		else
		{
			buyActive = viewedId != null && !flag2 && num == 0 && !viewedId.Equals("cape_Custom") && !flag;
			upgradeActive = viewedId != null && !flag2 && num != 0 && !flag;
			rentActive = false;
		}
		if (!flag2)
		{
			int num2 = TierForWear(WeaponManager.FirstUnboughtTag(viewedId), currentCategory);
			needTierActive = upgradeActive && ExpController.Instance != null && ExpController.Instance.OurTier < num2 && !flag && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
			if (needTierActive)
			{
				int num3 = ((num2 < 0 || num2 >= ExpController.LevelsForTiers.Length) ? ExpController.LevelsForTiers[ExpController.LevelsForTiers.Length - 1] : ExpController.LevelsForTiers[num2]);
				string text = string.Format("{0} {1} {2}", LocalizationStore.Key_0226, num3, LocalizationStore.Get("Key_1022"));
				needTierLabel.text = text;
			}
			upgrade.isEnabled = (!upgradeActive || !(ExpController.Instance != null) || ExpController.Instance.OurTier >= num2) && !flag;
		}
		string text2 = WeaponManager.FirstUnboughtTag(viewedId);
		bool onlyServerDiscount;
		int num4 = DiscountFor(text2, out onlyServerDiscount);
		if (!flag2 && !flag && !needTierActive && num4 > 0 && (text2 == null || !text2.Equals("cape_Custom") || !inGame) && (!(text2 == "cape_Custom") || !Defs.isDaterRegim))
		{
			saleActive = (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted) && !InTrainingAfterNoviceArmorRemoved;
			salePerc.text = num4 + "%";
		}
		else
		{
			saleActive = false;
		}
		saleRentActive = false;
	}

	private void UpdateTryGunDiscountTime()
	{
		try
		{
			tryGunDiscountTime.text = TempItemsController.TempItemTimeRemainsStringRepresentation(WeaponManager.sharedManager.StartTimeForTryGunDiscount(viewedId) + (long)WeaponManager.TryGunPromoDuration() - PromoActionsManager.CurrentUnixTime);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in tryGunDiscountPanelActive.text: " + ex);
		}
	}

	public void UpdateButtons()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		bool buyActive = false;
		bool rentActive = false;
		bool flag9 = false;
		bool flag10 = false;
		wholePrice2Gear.SetActive(false);
		bool upgradeActive = false;
		bool[] array = new bool[2];
		bool saleActive = false;
		bool flag11 = false;
		bool needTierActive = false;
		bool flag12 = viewedId != null && TempItemsController.PriceCoefs.ContainsKey(viewedId);
		rentProperties.SetActive(viewedId != null && TempItemsController.IsCategoryContainsTempItems(currentCategory) && TempItemsController.PriceCoefs.ContainsKey(viewedId));
		prolongateRentText.SetActive(viewedId != null && TempItemsController.PriceCoefs.ContainsKey(viewedId) && TempItemsController.sharedController != null && TempItemsController.sharedController.ContainsItem(viewedId));
		bool saleRentActive = false;
		upgrade.isEnabled = true;
		upgradeGear.isEnabled = true;
		if (WeaponCategory)
		{
			WeaponSounds weaponSounds = null;
			WeaponSounds weaponSounds2 = null;
			string text = null;
			string text2 = null;
			if (viewedId != null)
			{
				text = WeaponManager.FirstUnboughtTag(viewedId) ?? viewedId;
				string text3 = WeaponManager.FirstTagForOurTier(viewedId);
				List<string> list = WeaponUpgrades.ChainForTag(viewedId);
				if (text3 != null && list != null && list.IndexOf(text3) > list.IndexOf(text))
				{
					text = text3;
				}
				text2 = WeaponManager.LastBoughtTag(viewedId);
				UnityEngine.Object[] weaponsInGame = WeaponManager.sharedManager.weaponsInGame;
				for (int i = 0; i < weaponsInGame.Length; i++)
				{
					GameObject gameObject = (GameObject)weaponsInGame[i];
					if (!ItemDb.GetByPrefabName(gameObject.name).Tag.Equals(text2 ?? viewedId))
					{
						continue;
					}
					weaponSounds = gameObject.GetComponent<WeaponSounds>();
					UnityEngine.Object[] weaponsInGame2 = WeaponManager.sharedManager.weaponsInGame;
					for (int j = 0; j < weaponsInGame2.Length; j++)
					{
						GameObject gameObject2 = (GameObject)weaponsInGame2[j];
						if (ItemDb.GetByPrefabName(gameObject2.name).Tag.Equals(text))
						{
							weaponSounds2 = gameObject2.GetComponent<WeaponSounds>();
							break;
						}
					}
					break;
				}
			}
			bool flag13 = false;
			bool maxUpgrade = false;
			int num = ((viewedId == null) ? (-1) : _CurrentNumberOfUpgrades(viewedId, out maxUpgrade, currentCategory));
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsAvailableTryGun(viewedId))
			{
				num = 0;
			}
			flag13 = maxUpgrade && (!(WeaponManager.sharedManager != null) || !WeaponManager.sharedManager.IsAvailableTryGun(viewedId));
			buyActive = viewedId != null && !flag13 && num == 0 && !flag12 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
			upgradeActive = viewedId != null && !flag13 && num != 0 && weaponSounds2.tier < 100 && !flag12 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
			rentActive = false;
			if (WeaponManager.sharedManager != null && viewedId != null)
			{
				flag = WeaponManager.sharedManager.IsAvailableTryGun(viewedId);
				if (flag)
				{
					try
					{
						tryGunMatchesCount.text = ((SaltedInt)WeaponManager.sharedManager.TryGuns[viewedId]["NumberOfMatchesKey"]).Value.ToString();
					}
					catch (Exception ex)
					{
						Debug.LogError("Exception in tryGunMatchesCount.text: " + ex);
					}
				}
				flag2 = WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(viewedId);
				if (flag2)
				{
					UpdateTryGunDiscountTime();
				}
			}
			needTierActive = upgradeActive && weaponSounds2 != null && ExpController.Instance != null && ExpController.Instance.OurTier < weaponSounds2.tier && !flag12 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
			if (needTierActive)
			{
				int num2 = ((weaponSounds2.tier < 0 || weaponSounds2.tier >= ExpController.LevelsForTiers.Length) ? ExpController.LevelsForTiers[ExpController.LevelsForTiers.Length - 1] : ExpController.LevelsForTiers[weaponSounds2.tier]);
				string text4 = string.Format("{0} {1} {2}", LocalizationStore.Key_0226, num2, LocalizationStore.Get("Key_1022"));
				needTierLabel.text = text4;
			}
			upgrade.isEnabled = !upgradeActive || !(weaponSounds2 != null) || !(ExpController.Instance != null) || ExpController.Instance.OurTier >= weaponSounds2.tier;
			string text5 = null;
			if (viewedId != null)
			{
				text5 = WeaponManager.LastBoughtTag(viewedId);
			}
			if (text5 == null && viewedId != null && WeaponManager.sharedManager.IsAvailableTryGun(viewedId))
			{
				text5 = viewedId;
			}
			bool flag14 = text5 != null && viewedId != null && (chosenId == null || !chosenId.Equals(text5)) && viewedId != null && (num > 0 || WeaponManager.sharedManager.IsAvailableTryGun(viewedId)) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted || viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
			bool flag15 = !string.IsNullOrEmpty(viewedId) && ((_CurrentWeaponSetIDs()[(int)currentCategory] != null && _CurrentWeaponSetIDs()[(int)currentCategory].Equals(WeaponManager.LastBoughtTag(viewedId) ?? string.Empty)) || (WeaponManager.sharedManager.IsAvailableTryGun(viewedId) && _CurrentWeaponSetIDs()[(int)currentCategory].Equals(viewedId))) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted || viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
			if ((upgradeActive || flag12 || WeaponManager.sharedManager.IsAvailableTryGun(viewedId)) && viewedId != null && text != null && (flag12 || !text.Equals(viewedId) || WeaponManager.sharedManager.IsAvailableTryGun(viewedId)) && flag14)
			{
				flag6 = (!(WeaponManager.sharedManager != null) || !(WeaponManager.sharedManager.myPlayerMoveC != null) || !Device.isPixelGunLow) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted || viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
				flag7 = false;
			}
			if ((upgradeActive || flag12 || WeaponManager.sharedManager.IsAvailableTryGun(viewedId)) && viewedId != null && text != null && (flag12 || !text.Equals(viewedId) || WeaponManager.sharedManager.IsAvailableTryGun(viewedId)) && flag15)
			{
				array[0] = (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted || viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
				array[1] = false;
			}
			if (!upgradeActive && !flag12 && !buyActive && flag14)
			{
				flag6 = false;
				flag7 = (!(WeaponManager.sharedManager != null) || !(WeaponManager.sharedManager.myPlayerMoveC != null) || !Device.isPixelGunLow) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted || viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
			}
			if (!upgradeActive && !flag12 && !buyActive && flag15)
			{
				array[0] = false;
				array[1] = (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted || viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
			}
			bool onlyServerDiscount;
			int num3 = DiscountFor(text, out onlyServerDiscount);
			if (!flag13 && !flag12 && weaponSounds != null && weaponSounds.tier < 100 && !needTierActive && num3 > 0)
			{
				saleActive = (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
				salePerc.text = num3 + "%";
			}
			else
			{
				saleActive = false;
			}
			saleRentActive = false;
			if (text != null)
			{
				int num4 = 1;
				int num5 = 0;
				if (num4 == 1 && text2 != null && text.Equals(text2))
				{
					num5 = 1;
				}
				foreach (List<string> upgrade in WeaponUpgrades.upgrades)
				{
					if (!upgrade.Contains(text))
					{
						continue;
					}
					num4 = upgrade.Count;
					if (text2 != null)
					{
						num5 = upgrade.IndexOf(text2) + 1;
						break;
					}
					string text6 = WeaponManager.FirstTagForOurTier(text);
					if (text6 != null && upgrade.IndexOf(text6) > 0)
					{
						num5 = upgrade.IndexOf(text6);
					}
					break;
				}
				bool flag16 = (!weaponSounds.isMelee || weaponSounds.isShotMelee) && viewedId != null;
				if (weaponProperties.activeSelf != flag16)
				{
					weaponProperties.SetActive(flag16);
				}
				bool flag17 = weaponSounds.isMelee && !weaponSounds.isShotMelee && viewedId != null;
				if (meleeProperties.activeSelf != flag17)
				{
					meleeProperties.SetActive(flag17);
				}
				if (upgradesAnchor.activeSelf != (viewedId != null && !flag12))
				{
					upgradesAnchor.SetActive(viewedId != null && !flag12);
				}
				bool flag18 = num4 == 1 && !flag12;
				if (upgrade_1.activeSelf != flag18)
				{
					upgrade_1.SetActive(flag18);
				}
				bool flag19 = num4 == 2 && !flag12;
				if (upgrade_2.activeSelf != flag19)
				{
					upgrade_2.SetActive(flag19);
				}
				bool flag20 = num4 == 3 && !flag12;
				if (upgrade_3.activeSelf != flag20)
				{
					upgrade_3.SetActive(flag20);
				}
				if (num4 > 0)
				{
					GameObject obj;
					switch (num4)
					{
					case 3:
						obj = upgrade_3;
						break;
					case 2:
						obj = upgrade_2;
						break;
					default:
						obj = upgrade_1;
						break;
					}
					GameObject gameObject3 = obj;
					GameObject[] array2 = upgradeSprites1;
					if (num4 == 3)
					{
						array2 = upgradeSprites3;
					}
					if (num4 == 2)
					{
						array2 = upgradeSprites2;
					}
					if (array2 == null)
					{
						array2 = new GameObject[0];
					}
					Array.Sort(array2, (GameObject sp1, GameObject sp2) => Defs.CompareAlphaNumerically(sp1.gameObject.name, sp2.gameObject.name));
					for (int k = 0; k < array2.Length; k++)
					{
						array2[k].SetActive((num5 >= k) ? true : false);
						if (num5 == k)
						{
							array2[k].GetComponent<TweenColor>().enabled = true;
							continue;
						}
						array2[k].GetComponent<TweenColor>().enabled = false;
						array2[k].GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 1f);
					}
				}
				UpdateTempItemTime();
				if (weaponSounds2 == null)
				{
					weaponSounds2 = weaponSounds;
				}
				int[] array3 = null;
				array3 = ((!weaponSounds.isMelee || weaponSounds.isShotMelee) ? new int[4]
				{
					(!flag12) ? weaponSounds.damageShop : ((int)weaponSounds.DPS),
					weaponSounds.fireRateShop,
					weaponSounds.CapacityShop,
					weaponSounds.mobilityShop
				} : new int[3]
				{
					(!flag12) ? weaponSounds.damageShop : ((int)weaponSounds.DPS),
					weaponSounds.fireRateShop,
					weaponSounds.mobilityShop
				});
				int[] array4 = null;
				array4 = ((!weaponSounds.isMelee || weaponSounds.isShotMelee) ? new int[4]
				{
					(!flag12) ? weaponSounds2.damageShop : ((int)weaponSounds2.DPS),
					weaponSounds2.fireRateShop,
					weaponSounds2.CapacityShop,
					weaponSounds2.mobilityShop
				} : new int[3]
				{
					(!flag12) ? weaponSounds2.damageShop : ((int)weaponSounds2.DPS),
					weaponSounds2.fireRateShop,
					weaponSounds2.mobilityShop
				});
				bool flag21 = text2 != null && text != null && text2 != text && viewedId != null && text2 == viewedId;
				int[] array5 = ((!flag21) ? array4 : array3);
				if (weaponSounds.isMelee && !weaponSounds.isShotMelee)
				{
					damageMelee.text = GetWeaponStatText(array3[0], array5[0]);
					fireRateMElee.text = GetWeaponStatText(array3[1], array5[1]);
					mobilityMelee.text = GetWeaponStatText(array3[2], array5[2]);
				}
				else
				{
					damage.text = GetWeaponStatText(array3[0], array5[0]);
					fireRate.text = GetWeaponStatText(array3[1], array5[1]);
					capacity.text = GetWeaponStatText(array3[2], array5[2]);
					mobility.text = GetWeaponStatText(array3[3], array5[3]);
				}
				if (!SpecialParams.activeSelf)
				{
					SpecialParams.SetActive(true);
				}
				float num6 = 0.81500006f;
				if (weaponSounds2 == null)
				{
					weaponSounds2 = weaponSounds;
				}
				WeaponSounds weaponSounds3 = ((!flag21) ? weaponSounds2 : weaponSounds);
				if (weaponSounds3 != null)
				{
					float num7 = 90f / (float)weaponSounds3.InShopEffects.Count;
					for (int l = 0; l < effectsLabels.Count; l++)
					{
						if (effectsLabels[l].gameObject.activeSelf != l < weaponSounds3.InShopEffects.Count)
						{
							effectsLabels[l].gameObject.SetActive(l < weaponSounds3.InShopEffects.Count);
						}
						if (l >= weaponSounds3.InShopEffects.Count)
						{
							continue;
						}
						Transform transform = effectsLabels[l].transform;
						float num8 = 0f;
						if (weaponSounds3.InShopEffects.Count == 3)
						{
							if (l == 0)
							{
								num8 = 1f;
							}
							if (l == 2)
							{
								num8 = -3f;
							}
						}
						else if (weaponSounds3.InShopEffects.Count == 2)
						{
							if (l == 0)
							{
								num8 = -6f;
							}
							if (l == 1)
							{
								num8 = 6f;
							}
						}
						transform.localPosition = new Vector3(transform.localPosition.x, 39f - num7 * ((float)l + 0.5f) + num8, transform.localPosition.z);
						effectsLabels[l].text = ((weaponSounds3.InShopEffects[l] != WeaponSounds.Effects.Zoom) ? string.Empty : (weaponSounds3.zoomShop + "X ")) + LocalizationStore.Get(WeaponSounds.keysAndSpritesForEffects[weaponSounds3.InShopEffects[l]].Value);
						effectsSprites[l].spriteName = WeaponSounds.keysAndSpritesForEffects[weaponSounds3.InShopEffects[l]].Key;
					}
				}
			}
		}
		else
		{
			if (weaponProperties.activeSelf)
			{
				weaponProperties.SetActive(false);
			}
			if (meleeProperties.activeSelf)
			{
				meleeProperties.SetActive(false);
			}
			if (upgradesAnchor.activeSelf)
			{
				upgradesAnchor.SetActive(false);
			}
			if (SpecialParams.activeSelf)
			{
				SpecialParams.SetActive(false);
			}
			switch (currentCategory)
			{
			case CategoryNames.HatsCategory:
			case CategoryNames.ArmorCategory:
			case CategoryNames.CapesCategory:
			case CategoryNames.BootsCategory:
			case CategoryNames.MaskCategory:
			{
				string text8 = WeaponManager.LastBoughtTag(viewedId);
				bool flag28 = text8 != null;
				bool flag29 = flag28 && _CurrentEquippedWear.Equals(text8);
				string text9 = WeaponManager.FirstUnboughtTag(viewedId);
				SetUpUpgradesAndTiers(flag28, ref buyActive, ref upgradeActive, ref saleActive, ref needTierActive, ref rentActive, ref saleRentActive);
				bool flag30 = viewedId != "Armor_Novice" && flag28 && !flag29 && text8 != null && text8.Equals(viewedId) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted || trainingState >= TrainingState.OnArmor) && (!InTrainingAfterNoviceArmorRemoved || viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
				flag8 = viewedId != "Armor_Novice" && flag28 && flag29 && text8 != null && text8.Equals(viewedId) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
				bool flag31 = viewedId == "Armor_Novice" || (text8 != null && _CurrentEquippedWear.Equals(text8) && !upgradeActive && !flag12);
				if (!flag28 && viewedId != null && viewedId.Equals("cape_Custom"))
				{
					flag3 = !inGame && !Defs.isDaterRegim && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
				}
				if (!inGame && flag28 && viewedId != null && viewedId.Equals("cape_Custom"))
				{
					flag4 = !Defs.isDaterRegim && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
				}
				if (upgradeActive || flag12)
				{
					flag6 = flag30 && (flag12 || (viewedId != null && text9 != null && (text8 == null || text8.Equals(text9) || !text9.Equals(viewedId))));
					flag7 = false;
				}
				else
				{
					flag6 = false;
					flag7 = flag30 && viewedId != null && text9 != null && (text8 == null || text8.Equals(text9) || !text9.Equals(viewedId));
				}
				if (viewedId == "cape_Custom" && flag28)
				{
					array[1] = flag31;
					array[0] = false;
				}
				else
				{
					array[(!upgradeActive) ? 1 : 0] = flag31 && (flag12 || (viewedId != null && text9 != null && (text8 == null || text8.Equals(text9) || !text9.Equals(viewedId))));
					array[upgradeActive ? 1 : 0] = false;
				}
				try
				{
					if (currentCategory == CategoryNames.ArmorCategory)
					{
						armorCountLabel.text = Wear.armorNum[viewedId].ToString();
						armorCountLabel.gameObject.SetActive(viewedId != "Armor_Novice");
						armorWearDescription.text = LocalizationStore.Get("Key_0354");
					}
					else
					{
						nonArmorWearDEscription.text = LocalizationStore.Get(Wear.descriptionLocalizationKeys[viewedId]);
					}
				}
				catch (Exception ex2)
				{
					Debug.LogError("Exception in setting desciption for wear: " + ex2);
				}
				UpdateTempItemTime();
				break;
			}
			case CategoryNames.SkinsCategory:
			{
				flag3 = false;
				bool flag22 = false;
				bool flag23 = false;
				bool flag24 = false;
				bool flag25 = viewedId == "61";
				bool flag26 = false;
				if (!viewedId.Equals("CustomSkinID"))
				{
					bool isForMoneySkin = false;
					flag26 = SkinsController.IsSkinBought(viewedId, out isForMoneySkin);
					buyActive = isForMoneySkin && !flag26 && !flag25 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
					upgradeActive = false;
					flag23 = (!isForMoneySkin || flag26) && !viewedId.Equals(SkinsController.currentSkinNameForPers) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
					flag8 = false;
					flag22 = (!isForMoneySkin || flag26) && viewedId.Equals(SkinsController.currentSkinNameForPers);
					flag10 = flag25 && !flag26 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
					bool flag27 = false;
					int result;
					flag27 = int.TryParse(viewedId, out result) && result >= 1000;
					flag4 = !inGame && flag27 && !Defs.isDaterRegim && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
					flag5 = flag27 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
					bool onlyServerDiscount2;
					int num10 = DiscountFor(viewedId, out onlyServerDiscount2);
					if (!flag26 && !flag25 && !needTierActive && num10 > 0)
					{
						saleActive = (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
						salePerc.text = num10 + "%";
					}
					else
					{
						saleActive = false;
					}
				}
				else
				{
					flag24 = Storager.getInt(Defs.SkinsMakerInProfileBought, true) > 0;
					flag3 = !inGame && !flag24 && !Defs.isDaterRegim && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
					flag4 = false;
					flag11 = !inGame && flag24 && !Defs.isDaterRegim && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
					bool onlyServerDiscount3;
					int num11 = DiscountFor(viewedId, out onlyServerDiscount3);
					if (!flag24 && !needTierActive && num11 > 0 && (viewedId == null || !viewedId.Equals("CustomSkinID") || !inGame) && !inGame && !Defs.isDaterRegim)
					{
						saleActive = (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
						salePerc.text = num11 + "%";
					}
					else
					{
						saleActive = false;
					}
				}
				flag6 = false;
				flag7 = flag23 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
				array[0] = false;
				array[1] = flag22 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && !InTrainingAfterNoviceArmorRemoved;
				foreach (Transform item in skinProperties.transform)
				{
					if (viewedId != null && viewedId.Equals("CustomSkinID"))
					{
						if (!flag24)
						{
							item.gameObject.SetActive(item.gameObject.name.Equals("Custom_Skin"));
						}
						else
						{
							item.gameObject.SetActive(item.gameObject.name.Equals("Custom1_Skin"));
						}
					}
					else if (flag25 && !flag26)
					{
						item.gameObject.SetActive(item.gameObject.name.Equals("Facebook_Skin"));
					}
					else
					{
						item.gameObject.SetActive(item.gameObject.name.Equals("Usual_Skin"));
					}
				}
				break;
			}
			case CategoryNames.GearCategory:
			{
				flag4 = false;
				upgradeActive = false;
				array[0] = false;
				array[1] = false;
				flag8 = false;
				flag6 = false;
				flag7 = false;
				flag3 = false;
				flag9 = Storager.getInt(GearManager.HolderQuantityForID(viewedId), false) < GearManager.MaxCountForGear(GearManager.HolderQuantityForID(viewedId));
				ShopPositionParams shopPositionParams = null;
				GameObject gameObject4 = gear.Find((GameObject g) => g.name.Equals(GearManager.NameForUpgrade(GearManager.HolderQuantityForID(viewedId), GearManager.CurrentNumberOfUphradesForGear(GearManager.HolderQuantityForID(viewedId)) + 1)));
				if (gameObject4 != null)
				{
					shopPositionParams = gameObject4.GetComponent<ShopPositionParams>();
				}
				if (shopPositionParams != null && upgradeGear.gameObject.activeInHierarchy)
				{
					needTierActive = upgradeGear.gameObject.activeSelf && ExpController.Instance != null && ExpController.Instance.OurTier < shopPositionParams.tier;
					if (needTierActive)
					{
						int num9 = ((shopPositionParams.tier < 0 || shopPositionParams.tier >= ExpController.LevelsForTiers.Length) ? ExpController.LevelsForTiers[ExpController.LevelsForTiers.Length - 1] : ExpController.LevelsForTiers[shopPositionParams.tier]);
						string text7 = string.Format("{0} {1} {2}", LocalizationStore.Key_0226, num9, LocalizationStore.Get("Key_1022"));
						needTierLabel.text = text7;
					}
					upgradeGear.isEnabled = viewedId != null && ExpController.Instance.OurTier >= shopPositionParams.tier;
				}
				foreach (Transform item2 in gearProperties.transform)
				{
					item2.gameObject.SetActive(viewedId != null && item2.gameObject.name.Equals(viewedId));
				}
				break;
			}
			}
		}
		if (tryGunPanel != null && tryGunPanel.activeSelf != flag)
		{
			tryGunPanel.SetActive(flag);
		}
		if (tryGunDiscountPanel != null && tryGunDiscountPanel.activeSelf != flag2)
		{
			tryGunDiscountPanel.SetActive(flag2);
		}
		if (edit != null && edit.gameObject.activeSelf != flag4)
		{
			edit.gameObject.SetActive(flag4);
		}
		if (enable != null && enable.gameObject.activeSelf != flag3)
		{
			enable.gameObject.SetActive(flag3);
		}
		if (delete.gameObject.activeSelf != flag5)
		{
			delete.gameObject.SetActive(flag5);
		}
		if (buy.gameObject.activeSelf != buyActive)
		{
			buy.gameObject.SetActive(buyActive);
		}
		if (rent.gameObject.activeSelf != rentActive)
		{
			rent.gameObject.SetActive(rentActive);
		}
		if (equips[0].gameObject.activeSelf != flag6)
		{
			equips[0].gameObject.SetActive(flag6);
		}
		if (equips[1].gameObject.activeSelf != flag7)
		{
			equips[1].gameObject.SetActive(flag7);
		}
		if (unequip.gameObject.activeSelf != flag8)
		{
			unequip.gameObject.SetActive(flag8);
		}
		if (buyGear.gameObject.activeSelf != flag9)
		{
			buyGear.gameObject.SetActive(flag9);
		}
		if (this.upgrade.gameObject.activeSelf != upgradeActive)
		{
			this.upgrade.gameObject.SetActive(upgradeActive);
		}
		if (facebookLoginLockedSkinButton.gameObject.activeSelf != flag10)
		{
			facebookLoginLockedSkinButton.gameObject.SetActive(flag10);
		}
		if (equippeds[0].gameObject.activeSelf != array[0])
		{
			equippeds[0].gameObject.SetActive(array[0]);
		}
		if (equippeds[1].gameObject.activeSelf != array[1])
		{
			equippeds[1].gameObject.SetActive(array[1]);
		}
		if (sale.gameObject.activeSelf != saleActive)
		{
			sale.gameObject.SetActive(saleActive);
		}
		if (saleRent.gameObject.activeSelf != saleRentActive)
		{
			saleRent.gameObject.SetActive(saleRentActive);
		}
		if (create.gameObject.activeSelf != flag11)
		{
			create.gameObject.SetActive(flag11);
		}
		if (needTier.gameObject.activeSelf != needTierActive)
		{
			needTier.gameObject.SetActive(needTierActive);
		}
	}

	private void UpdateTempItemTime()
	{
		if (TempItemsController.sharedController != null)
		{
			bool flag = TempItemsController.PriceCoefs.ContainsKey(viewedId) && !TempItemsController.sharedController.ContainsItem(viewedId);
			if (notRented.activeInHierarchy != flag)
			{
				notRented.SetActive(flag);
			}
			string text = TempItemsController.sharedController.TimeRemainingForItemString(viewedId);
			bool flag2 = TempItemsController.sharedController.ContainsItem(viewedId) && text.Length < 5;
			if (daysLeftLabel.gameObject.activeInHierarchy != flag2)
			{
				daysLeftLabel.gameObject.SetActive(flag2);
			}
			if (daysLeftValueLabel.gameObject.activeInHierarchy != flag2)
			{
				daysLeftValueLabel.gameObject.SetActive(flag2);
			}
			if (flag2)
			{
				daysLeftValueLabel.text = text;
			}
			bool flag3 = TempItemsController.sharedController.ContainsItem(viewedId) && text.Length >= 5;
			if (timeLeftLabel.gameObject.activeInHierarchy != flag3)
			{
				timeLeftLabel.gameObject.SetActive(flag3);
			}
			if (timeLeftValueLabel.gameObject.activeInHierarchy != flag3)
			{
				timeLeftValueLabel.gameObject.SetActive(flag3);
			}
			if (flag3)
			{
				timeLeftValueLabel.text = text;
			}
			bool flag4 = flag3 && TempItemsController.sharedController.TimeRemainingForItems(viewedId) <= 3600;
			if (redBackForTime.activeInHierarchy != flag4)
			{
				redBackForTime.SetActive(flag4);
			}
		}
	}

	public static int DiscountFor(string idForPromo, out bool onlyServerDiscount)
	{
		//Discarded unreachable code: IL_0179, IL_019c
		try
		{
			if (idForPromo == null)
			{
				Debug.LogError("DiscountFor: idForPromo == null");
				onlyServerDiscount = false;
				return 0;
			}
			bool flag = false;
			bool flag2 = false;
			float num = 100f;
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(idForPromo))
			{
				long num2 = WeaponManager.sharedManager.DiscountForTryGun(idForPromo);
				num -= (float)num2;
				num = Math.Max(1f, num);
				num = Math.Min(100f, num);
				flag2 = true;
			}
			num /= 100f;
			float num3 = 100f;
			if (!flag2 && PromoActionsManager.sharedManager.discounts.ContainsKey(idForPromo) && PromoActionsManager.sharedManager.discounts[idForPromo].Count > 0)
			{
				num3 -= (float)PromoActionsManager.sharedManager.discounts[idForPromo][0];
				num3 = Math.Max(10f, num3);
				num3 = Math.Min(100f, num3);
				flag = true;
			}
			num3 /= 100f;
			onlyServerDiscount = !flag2 && flag;
			if (!flag2 && !flag)
			{
				return 0;
			}
			float value = num * num3;
			value = Mathf.Clamp(value, 0.01f, 1f);
			float f = (1f - value) * 100f;
			int num4 = Mathf.RoundToInt(f);
			if (onlyServerDiscount && num4 % 5 != 0)
			{
				num4 = 5 * (num4 / 5 + 1);
			}
			return Math.Min(num4, 99);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in DiscountFor: " + ex);
			onlyServerDiscount = false;
			return 0;
		}
	}

	public static ItemPrice currentPrice(string viewedId, CategoryNames currentCategory, bool upgradeNotBuy = false, bool useDiscounts = true)
	{
		//Discarded unreachable code: IL_0196, IL_01c0
		try
		{
			string text = viewedId;
			string text2 = text;
			if (text2 == null)
			{
				return new ItemPrice(0, "Coins");
			}
			if (text2 != null && WeaponManager.tagToStoreIDMapping.ContainsKey(text2))
			{
				text = WeaponManager.tagToStoreIDMapping[WeaponManager.FirstUnboughtOrForOurTier(text2)];
			}
			if (currentCategory == CategoryNames.SkinsCategory && SkinsController.shopKeyFromNameSkin.ContainsKey(text))
			{
				text = SkinsController.shopKeyFromNameSkin[text];
			}
			if (currentCategory == CategoryNames.GearCategory)
			{
				text = ((!upgradeNotBuy) ? GearManager.OneItemIDForGear(GearManager.HolderQuantityForID(text), GearManager.CurrentNumberOfUphradesForGear(text)) : GearManager.UpgradeIDForGear(GearManager.HolderQuantityForID(text), GearManager.CurrentNumberOfUphradesForGear(text) + 1));
			}
			if (IsWearCategory(currentCategory))
			{
				text = WeaponManager.FirstUnboughtTag(text);
			}
			string idForPromo = ((!IsWeaponCategory(currentCategory) && !IsWearCategory(currentCategory)) ? viewedId : WeaponManager.FirstUnboughtOrForOurTier(viewedId));
			ItemPrice itemPrice = ItemDb.GetPriceByShopId(text) ?? new ItemPrice(10, "Coins");
			int num = itemPrice.Price;
			if (useDiscounts)
			{
				bool onlyServerDiscount;
				int num2 = DiscountFor(idForPromo, out onlyServerDiscount);
				if (num2 > 0)
				{
					float num3 = num2;
					num = Math.Max((int)((float)num * 0.01f), Mathf.RoundToInt((float)num * (1f - num3 / 100f)));
					if (onlyServerDiscount)
					{
						num = ((num % 5 >= 3) ? (num + (5 - num % 5)) : (num - num % 5));
					}
				}
			}
			if (currentCategory == CategoryNames.GearCategory && !upgradeNotBuy)
			{
				num *= GearManager.ItemsInPackForGear(GearManager.HolderQuantityForID(text));
			}
			return new ItemPrice(num, itemPrice.Currency);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in currentPrice: " + ex);
			return new ItemPrice(0, "Coins");
		}
	}

	public static int PriceIfGunWillBeTryGun(string tg)
	{
		return Mathf.RoundToInt((float)currentPrice(tg, (CategoryNames)ItemDb.GetItemCategory(tg), false, false).Price * ((float)WeaponManager.BaseTryGunDiscount() / 100f));
	}

	public void UpdateItemParameters()
	{
		wholePrice.gameObject.SetActive(buy.gameObject.activeInHierarchy || upgrade.gameObject.activeInHierarchy || enable.gameObject.activeInHierarchy);
		wholePriceUpgradeGear.gameObject.SetActive(false);
		wholePrice2Gear.gameObject.SetActive(buyGear.gameObject.activeInHierarchy);
		bool onlyServerDiscount;
		bool flag = viewedId != null && DiscountFor(WeaponManager.FirstUnboughtOrForOurTier(viewedId), out onlyServerDiscount) > 0;
		wholePriceBG.gameObject.SetActive(!flag);
		wholePriceBG_Discount.gameObject.SetActive(flag);
		bool flag2 = viewedId != null && DiscountFor(viewedId, out onlyServerDiscount) > 0;
		wholePriceBG2Gear.gameObject.SetActive(!flag2);
		wholePriceBG2Gear_Discount.gameObject.SetActive(flag2);
		wholePriceUpgradeBG2Gear.gameObject.SetActive(!flag2);
		wholePriceUpgradeBG2Gear_Discount.gameObject.SetActive(flag2);
		if (viewedId != null)
		{
			ItemPrice itemPrice = currentPrice(viewedId, currentCategory, currentCategory == CategoryNames.GearCategory);
			if (currentCategory == CategoryNames.GearCategory)
			{
				priceUpgradeGear.text = itemPrice.Price.ToString();
				currencyImagePriceUpgradeGear.spriteName = ((!itemPrice.Currency.Equals("Coins")) ? "gem_znachek" : "ingame_coin");
				currencyImagePriceUpgradeGear.width = ((!itemPrice.Currency.Equals("Coins")) ? 34 : 30);
				currencyImagePriceUpgradeGear.height = ((!itemPrice.Currency.Equals("Coins")) ? 24 : 30);
			}
			else
			{
				price.text = itemPrice.Price.ToString();
				currencyImagePrice.spriteName = ((!itemPrice.Currency.Equals("Coins")) ? "gem_znachek" : "ingame_coin");
				currencyImagePrice.width = ((!itemPrice.Currency.Equals("Coins")) ? 34 : 30);
				currencyImagePrice.height = ((!itemPrice.Currency.Equals("Coins")) ? 24 : 30);
			}
			if (currentCategory == CategoryNames.GearCategory)
			{
				ItemPrice itemPrice2 = currentPrice(viewedId, currentCategory);
				price2Gear.text = itemPrice2.Price.ToString();
				currencyImagePriceGear.spriteName = ((!itemPrice2.Currency.Equals("Coins")) ? "gem_znachek" : "ingame_coin");
				currencyImagePriceGear.width = ((!itemPrice2.Currency.Equals("Coins")) ? 34 : 30);
				currencyImagePriceGear.height = ((!itemPrice2.Currency.Equals("Coins")) ? 24 : 30);
			}
		}
	}

	private static int _CurrentNumberOfWearUpgrades(string id, out bool maxUpgrade, CategoryNames c)
	{
		if (id == "Armor_Novice")
		{
			maxUpgrade = NoviceArmorAvailable;
			return NoviceArmorAvailable ? 1 : 0;
		}
		List<string> list = null;
		foreach (List<string> item in Wear.wear[c])
		{
			if (item.Contains(id))
			{
				list = item;
				break;
			}
		}
		if (list == null)
		{
			maxUpgrade = false;
			return 0;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (Storager.getInt(list[i], true) == 0)
			{
				maxUpgrade = false;
				return i;
			}
		}
		maxUpgrade = true;
		return list.Count;
	}

	public static int _CurrentNumberOfUpgrades(string id, out bool maxUpgrade, CategoryNames c, bool countTryGunsAsUpgrade = true)
	{
		List<string> list = new List<string>();
		int num = 0;
		if (IsWeaponCategory(c))
		{
			list.Add(id);
			foreach (List<string> upgrade in WeaponUpgrades.upgrades)
			{
				if (upgrade.Contains(id))
				{
					list = upgrade;
					break;
				}
			}
			num = list.Count;
			if (WeaponManager.tagToStoreIDMapping.ContainsKey(id))
			{
				int num2 = list.Count - 1;
				while (num2 >= 0)
				{
					string text = list[num2];
					string defName = id;
					bool flag = ItemDb.IsTemporaryGun(id);
					if (!flag)
					{
						defName = WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[text]];
					}
					bool flag2 = !_HasntBoughtGood(defName, flag);
					if (!flag2 && countTryGunsAsUpgrade && WeaponManager.sharedManager != null)
					{
						flag2 = WeaponManager.sharedManager.IsAvailableTryGun(text);
					}
					if (!flag2)
					{
						num--;
						num2--;
						continue;
					}
					break;
				}
			}
		}
		else if (IsWearCategory(c))
		{
			num = ((!_HasntBoughtGood(id, TempItemsController.PriceCoefs.ContainsKey(id))) ? 1 : 0);
		}
		if (id.Equals(StoreKitEventListener.elixirID) && Defs.NumberOfElixirs > 0)
		{
			num++;
		}
		maxUpgrade = num == ((list.Count <= 0) ? 1 : list.Count);
		return num;
	}

	private static bool _HasntBoughtGood(string defName, bool tempGun = false)
	{
		return (!tempGun) ? (Storager.getInt(defName, true) == 0) : (!TempItemsController.sharedController.ContainsItem(defName));
	}

	public void UpdatePersWithNewItem()
	{
		if (WeaponCategory)
		{
			string text = viewedId;
			if (text == null && WeaponManager.sharedManager.playerWeapons.Count > 0)
			{
				text = ItemDb.GetByPrefabName((WeaponManager.sharedManager.playerWeapons[0] as Weapon).weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
			}
			SetWeapon(text);
			return;
		}
		switch (currentCategory)
		{
		case CategoryNames.HatsCategory:
			UpdatePersHat(viewedId);
			break;
		case CategoryNames.SkinsCategory:
			if (!viewedId.Equals("CustomSkinID"))
			{
				UpdatePersSkin(viewedId);
			}
			break;
		case CategoryNames.CapesCategory:
			UpdatePersCape(viewedId);
			break;
		case CategoryNames.MaskCategory:
			UpdatePersMask(viewedId);
			break;
		case CategoryNames.BootsCategory:
			UpdatePersBoots(viewedId);
			break;
		case CategoryNames.ArmorCategory:
			UpdatePersArmor(viewedId);
			break;
		case CategoryNames.GearCategory:
			break;
		}
	}

	public void UpdatePersHat(string hat)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < hatPoint.transform.childCount; i++)
		{
			list.Add(hatPoint.transform.GetChild(i));
		}
		foreach (Transform item in list)
		{
			item.parent = null;
			item.position = new Vector3(0f, -10000f, 0f);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		if (!hat.Equals(Defs.HatNoneEqupped))
		{
			string @string = Storager.getString(Defs.VisualHatArmor, false);
			if (!string.IsNullOrEmpty(@string) && Wear.wear[CategoryNames.HatsCategory][0].IndexOf(hat) >= 0 && Wear.wear[CategoryNames.HatsCategory][0].IndexOf(hat) < Wear.wear[CategoryNames.HatsCategory][0].IndexOf(@string))
			{
				hat = @string;
			}
			GameObject gameObject = Resources.Load("Hats/" + hat) as GameObject;
			if (!(gameObject == null))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				DisableLightProbesRecursively(gameObject2);
				gameObject2.transform.parent = hatPoint.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				Player_move_c.SetLayerRecursively(gameObject2, LayerMask.NameToLayer("NGUIShop"));
				SetPersHatVisible(hatPoint);
			}
		}
	}

	public void UpdatePersArmor(string armor)
	{
		if (armorPoint.childCount > 0)
		{
			Transform child = armorPoint.GetChild(0);
			ArmorRefs component = child.GetChild(0).GetComponent<ArmorRefs>();
			if (component != null)
			{
				if (component.leftBone != null)
				{
					component.leftBone.parent = child.GetChild(0);
				}
				if (component.rightBone != null)
				{
					component.rightBone.parent = child.GetChild(0);
				}
				child.parent = null;
				child.position = new Vector3(0f, -10000f, 0f);
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
		if (armor.Equals(Defs.ArmorNewNoneEqupped))
		{
			return;
		}
		string @string = Storager.getString(Defs.VisualArmor, false);
		if (!string.IsNullOrEmpty(@string) && Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(armor) >= 0 && Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(armor) < Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(@string))
		{
			armor = @string;
		}
		if (!(weapon != null))
		{
			return;
		}
		GameObject gameObject = Resources.Load("Armor_Shop/" + armor) as GameObject;
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		DisableLightProbesRecursively(gameObject2);
		ArmorRefs component2 = gameObject2.transform.GetChild(0).GetComponent<ArmorRefs>();
		if (component2 != null)
		{
			WeaponSounds component3 = weapon.GetComponent<WeaponSounds>();
			gameObject2.transform.parent = armorPoint.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			Player_move_c.SetLayerRecursively(gameObject2, LayerMask.NameToLayer("NGUIShop"));
			if (component2.leftBone != null && component3.LeftArmorHand != null)
			{
				component2.leftBone.parent = component3.LeftArmorHand;
				component2.leftBone.localPosition = Vector3.zero;
				component2.leftBone.localRotation = Quaternion.identity;
				component2.leftBone.localScale = new Vector3(1f, 1f, 1f);
			}
			if (component2.rightBone != null && component3.RightArmorHand != null)
			{
				component2.rightBone.parent = component3.RightArmorHand;
				component2.rightBone.localPosition = Vector3.zero;
				component2.rightBone.localRotation = Quaternion.identity;
				component2.rightBone.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		SetPersArmorVisible(armorPoint);
	}

	public void UpdatePersMask(string mask)
	{
		for (int i = 0; i < maskPoint.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(maskPoint.transform.GetChild(i).gameObject);
		}
		if (!mask.Equals("MaskNoneEquipped"))
		{
			GameObject gameObject = Resources.Load("Masks/" + mask) as GameObject;
			if (gameObject == null)
			{
				Debug.LogWarning("ShopNGUIController UpdatePersMask: maskPrefab == null  mask = " + (mask ?? "(null)"));
				return;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			DisableLightProbesRecursively(gameObject2);
			gameObject2.transform.parent = maskPoint.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			Player_move_c.SetLayerRecursively(gameObject2, LayerMask.NameToLayer("NGUIShop"));
		}
	}

	public void UpdatePersCape(string cape)
	{
		for (int i = 0; i < capePoint.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(capePoint.transform.GetChild(i).gameObject);
		}
		if (!cape.Equals(Defs.CapeNoneEqupped))
		{
			GameObject gameObject = Resources.Load("Capes/" + cape) as GameObject;
			if (!(gameObject == null))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				DisableLightProbesRecursively(gameObject2);
				gameObject2.transform.parent = capePoint.transform;
				gameObject2.transform.localPosition = new Vector3(0f, -0.8f, 0f);
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				Player_move_c.SetLayerRecursively(gameObject2, LayerMask.NameToLayer("NGUIShop"));
			}
		}
	}

	public void UpdatePersSkin(string skinID)
	{
		SetSkinOnPers(SkinsController.skinsForPers[skinID]);
	}

	public void SetSkinOnPers(Texture skin)
	{
		WeaponSounds weaponSounds = ((body.transform.childCount <= 0) ? null : body.transform.GetChild(0).GetComponent<WeaponSounds>());
		GameObject gameObject = ((!(weaponSounds != null)) ? null : weaponSounds.bonusPrefab);
		GameObject gameObject2 = null;
		GameObject gameObject3 = null;
		if (gameObject != null)
		{
			Transform leftArmorHand = weaponSounds.LeftArmorHand;
			Transform rightArmorHand = weaponSounds.RightArmorHand;
			if (leftArmorHand != null)
			{
				gameObject2 = leftArmorHand.gameObject;
			}
			if (rightArmorHand != null)
			{
				gameObject3 = rightArmorHand.gameObject;
			}
		}
		List<GameObject> list = new List<GameObject>();
		list.Add(capePoint.gameObject);
		list.Add(hatPoint.gameObject);
		list.Add(bootsPoint.gameObject);
		list.Add(armorPoint.gameObject);
		list.Add(maskPoint.gameObject);
		List<GameObject> list2 = list;
		if (weaponSounds != null && weaponSounds.grenatePoint != null)
		{
			list2.Add(weaponSounds.grenatePoint.gameObject);
		}
		if (gameObject != null)
		{
			list2.Add(gameObject);
		}
		if (gameObject2 != null)
		{
			list2.Add(gameObject2);
		}
		if (gameObject3 != null)
		{
			list2.Add(gameObject3);
		}
		if (weaponSounds != null)
		{
			List<GameObject> listWeaponAnimEffects = weaponSounds.GetListWeaponAnimEffects();
			if (listWeaponAnimEffects != null)
			{
				list2.AddRange(listWeaponAnimEffects);
			}
		}
		Player_move_c.SetTextureRecursivelyFrom(MainMenu_Pers.gameObject, skin, list2.ToArray());
	}

	public void UpdatePersBoots(string bs)
	{
		foreach (Transform item in bootsPoint.transform)
		{
			if (item.gameObject.name.Equals(bs))
			{
				item.gameObject.SetActive(true);
			}
			else
			{
				item.gameObject.SetActive(false);
			}
		}
	}

	public void ReloadCategoryTempItemsRemoved(List<string> expired)
	{
		if (currentCategory != CategoryNames.HatsCategory && expired.Contains("hat_Adamant_3"))
		{
			UpdatePersHat(Defs.HatNoneEqupped);
		}
		if (currentCategory != CategoryNames.ArmorCategory && expired.Contains("Armor_Adamant_3"))
		{
			UpdatePersArmor(Defs.ArmorNewNoneEqupped);
		}
		if (GuiActive && TempItemsController.IsCategoryContainsTempItems(currentCategory))
		{
			CategoryChosen(currentCategory, (expired.Count <= 0 || !TempItemsController.GunsMappingFromTempToConst.ContainsKey(expired[0])) ? viewedId : TempItemsController.GunsMappingFromTempToConst[expired[0]]);
			UpdateIcons();
		}
	}

	public void SimulateCategoryChoose(int num)
	{
		if (num >= 0 && num < category.buttons.Length && num != 0)
		{
			category.buttons[0].IsChecked = false;
			category.buttons[num].IsChecked = true;
		}
	}

	public void CategoryChosen(CategoryNames i, string idToSet = null, bool initial = false)
	{
		WeaponManager.sharedManager.PrepareForShopWeaponCategory((int)i);
		if (!initial)
		{
			switch (currentCategory)
			{
			case CategoryNames.HatsCategory:
				viewedId = _CurrentEquippedWear;
				break;
			case CategoryNames.ArmorCategory:
				viewedId = _CurrentEquippedWear;
				break;
			case CategoryNames.SkinsCategory:
				if (SkinsController.currentSkinNameForPers != null)
				{
					viewedId = SkinsController.currentSkinNameForPers;
				}
				else
				{
					if (SkinsController.skinsForPers == null || SkinsController.skinsForPers.Keys.Count <= 0)
					{
						break;
					}
					using (Dictionary<string, Texture2D>.KeyCollection.Enumerator enumerator = SkinsController.skinsForPers.Keys.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							string text = (viewedId = enumerator.Current);
						}
					}
				}
				break;
			case CategoryNames.CapesCategory:
			case CategoryNames.BootsCategory:
			case CategoryNames.MaskCategory:
				viewedId = _CurrentEquippedWear;
				break;
			}
			if (WearCategory || currentCategory == CategoryNames.SkinsCategory)
			{
				UpdatePersWithNewItem();
			}
		}
		currentCategory = i;
		if (highlightedCarouselObject != null)
		{
		}
		highlightedCarouselObject = null;
		if (WeaponCategory)
		{
			chosenId = _CurrentWeaponSetIDs()[(int)i];
			viewedId = idToSet;
			if (viewedId != null && chosenId != null && viewedId.Equals(chosenId) && WeaponManager.sharedManager != null && WeaponManager.sharedManager.FilteredShopLists.Count > (int)i && !WeaponManager.sharedManager.FilteredShopLists[(int)i].Find((GameObject go) => go.name.Equals(viewedId)))
			{
				viewedId = null;
			}
			viewedId = chosenId;
			if (viewedId == null)
			{
				CategoryNames cn;
				string text2 = TempGunOrHighestDPSGun(i, out cn);
				if (i == cn)
				{
					viewedId = text2;
				}
				else
				{
					viewedId = TemppOrHighestDPSGunInCategory((int)i);
				}
			}
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
			{
				if (trainingState > TrainingState.OnArmor && currentCategory == CategoryNames.ArmorCategory)
				{
					viewedId = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
					idToSet = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
				}
				else if (currentCategory == CategoryNames.SniperCategory)
				{
					viewedId = WeaponTags.HunterRifleTag;
					idToSet = WeaponTags.HunterRifleTag;
				}
			}
			if (InTrainingAfterNoviceArmorRemoved && currentCategory == CategoryNames.ArmorCategory)
			{
				viewedId = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
				idToSet = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
			}
		}
		else
		{
			switch (currentCategory)
			{
			case CategoryNames.HatsCategory:
			{
				string text3 = null;
				try
				{
					Dictionary<Wear.LeagueItemState, List<string>> dictionary = Wear.LeagueItems();
					text3 = (from item in dictionary[Wear.LeagueItemState.Open].Union(dictionary[Wear.LeagueItemState.Purchased])
						orderby Wear.LeagueForWear(item, currentCategory)
						select item).FirstOrDefault();
				}
				catch (Exception ex)
				{
					Debug.LogError("CategoryChoosen: exception in getting firstLeagueItem: " + ex);
				}
				string fu = text3 ?? WeaponManager.FirstUnboughtTag("hat_Headphones");
				int num3 = hats.FindIndex((GameObject go) => go.name.Equals(fu));
				viewedId = ((_CurrentEquippedWear == null || _CurrentNoneEquipped == null || _CurrentEquippedWear.Equals(_CurrentNoneEquipped) || WeaponManager.LastBoughtTag(_CurrentEquippedWear) == null || !WeaponManager.LastBoughtTag(_CurrentEquippedWear).Equals(_CurrentEquippedWear)) ? hats[(num3 <= -1) ? (hats.Count - 1) : num3].name : _CurrentEquippedWear);
				break;
			}
			case CategoryNames.ArmorCategory:
				viewedId = ((_CurrentEquippedWear == null || _CurrentNoneEquipped == null || _CurrentEquippedWear.Equals(_CurrentNoneEquipped) || WeaponManager.LastBoughtTag(_CurrentEquippedWear) == null || !WeaponManager.LastBoughtTag(_CurrentEquippedWear).Equals(_CurrentEquippedWear)) ? WeaponManager.FirstUnboughtTag(Wear.wear[CategoryNames.ArmorCategory][0][0]) : _CurrentEquippedWear);
				scrollViewPanel.transform.localPosition = Vector3.zero;
				scrollViewPanel.clipOffset = new Vector2(0f, 0f);
				if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
				{
					viewedId = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
					idToSet = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
				}
				if (InTrainingAfterNoviceArmorRemoved)
				{
					viewedId = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
					idToSet = WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1";
				}
				break;
			case CategoryNames.SkinsCategory:
				if (SkinsController.currentSkinNameForPers != null)
				{
					viewedId = SkinsController.currentSkinNameForPers;
				}
				else
				{
					if (SkinsController.skinsForPers == null || SkinsController.skinsForPers.Keys.Count <= 0)
					{
						break;
					}
					using (Dictionary<string, Texture2D>.KeyCollection.Enumerator enumerator2 = SkinsController.skinsForPers.Keys.GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							string text4 = (viewedId = enumerator2.Current);
						}
					}
				}
				break;
			case CategoryNames.CapesCategory:
				viewedId = ((_CurrentEquippedWear == null || _CurrentNoneEquipped == null || _CurrentEquippedWear.Equals(_CurrentNoneEquipped) || WeaponManager.LastBoughtTag(_CurrentEquippedWear) == null || !WeaponManager.LastBoughtTag(_CurrentEquippedWear).Equals(_CurrentEquippedWear)) ? (WeaponManager.LastBoughtTag(capes[1].name) ?? capes[1].name) : _CurrentEquippedWear);
				break;
			case CategoryNames.BootsCategory:
				viewedId = ((_CurrentEquippedWear == null || _CurrentNoneEquipped == null || _CurrentEquippedWear.Equals(_CurrentNoneEquipped) || WeaponManager.LastBoughtTag(_CurrentEquippedWear) == null || !WeaponManager.LastBoughtTag(_CurrentEquippedWear).Equals(_CurrentEquippedWear)) ? (WeaponManager.LastBoughtTag(boots[0].name) ?? boots[0].name) : _CurrentEquippedWear);
				break;
			case CategoryNames.MaskCategory:
				viewedId = ((_CurrentEquippedWear == null || _CurrentNoneEquipped == null || _CurrentEquippedWear.Equals(_CurrentNoneEquipped) || WeaponManager.LastBoughtTag(_CurrentEquippedWear) == null || !WeaponManager.LastBoughtTag(_CurrentEquippedWear).Equals(_CurrentEquippedWear)) ? (WeaponManager.LastBoughtTag(masks[0].name) ?? masks[0].name) : _CurrentEquippedWear);
				break;
			case CategoryNames.GearCategory:
			{
				int num = GearManager.CurrentNumberOfUphradesForGear(GearManager.InvisibilityPotion);
				int num2 = ((num >= GearManager.NumOfGearUpgrades) ? num : num);
				viewedId = GearManager.NameForUpgrade(GearManager.InvisibilityPotion, num2);
				break;
			}
			}
		}
		armorWearProperties.SetActive(currentCategory == CategoryNames.ArmorCategory);
		nonArmorWearProperties.SetActive(IsWearCategory(currentCategory) && currentCategory != CategoryNames.ArmorCategory);
		gearProperties.SetActive(currentCategory == CategoryNames.GearCategory);
		skinProperties.SetActive(currentCategory == CategoryNames.SkinsCategory);
		border.SetActive(currentCategory != CategoryNames.SkinsCategory);
		if (currentCategory == CategoryNames.ArmorCategory)
		{
			List<GameObject> list = sharedShop.armor;
		}
		ReloadCarousel(idToSet);
		SetCamera();
		if (!IsWeaponCategory(i) && weapon == null)
		{
			SetWeapon(_CurrentWeaponSetIDs()[0] ?? WeaponManager._initialWeaponName);
		}
		if (currentCategory == CategoryNames.SkinsCategory)
		{
			shopCarouselCollider.center = new Vector3(0f, -40f, 0f);
			shopCarouselCollider.size = new Vector3(shopCarouselCollider.size.x, 363f, shopCarouselCollider.size.z);
		}
		else
		{
			shopCarouselCollider.center = new Vector3(0f, 0f, 0f);
			shopCarouselCollider.size = new Vector3(shopCarouselCollider.size.x, 252f, shopCarouselCollider.size.z);
		}
	}

	private void HandleCarouselCentering()
	{
		HandleCarouselCentering(carouselCenter.centeredObject);
	}

	private void HandleCarouselCentering(GameObject centeredObj)
	{
		if (centeredObj != null && centeredObj != _lastSelectedItem)
		{
			_lastSelectedItem = centeredObj;
			if (highlightedCarouselObject != null)
			{
			}
			highlightedCarouselObject = centeredObj.transform;
			if (highlightedCarouselObject != null)
			{
			}
			ShopCarouselElement component = centeredObj.GetComponent<ShopCarouselElement>();
			SetLabelsForElem(component);
			ChooseCarouselItem(component.itemID);
		}
		if (EnableConfigurePos && centeredObj != null)
		{
			centeredObj.GetComponent<ShopCarouselElement>().SetPos(1f, 0f);
		}
	}

	private void CheckCenterItemChanging()
	{
		if (!scrollViewPanel.cachedGameObject.activeInHierarchy)
		{
			return;
		}
		Transform cachedTransform = scrollViewPanel.cachedTransform;
		itemIndex = -1;
		int num = (int)wrapContent.cellWidth;
		int childCount = wrapContent.transform.childCount;
		if (cachedTransform.localPosition.x > 0f)
		{
			itemIndex = 0;
		}
		else if (cachedTransform.localPosition.x < (float)(-1 * num * childCount))
		{
			itemIndex = childCount - 1;
		}
		else
		{
			itemIndex = -1 * Mathf.RoundToInt((cachedTransform.localPosition.x - (float)(Mathf.CeilToInt(cachedTransform.localPosition.x / (float)num / (float)childCount) * num * childCount)) / (float)num);
		}
		itemIndex = Mathf.Clamp(itemIndex, 0, childCount - 1);
		if (itemIndex >= 0 && itemIndex < wrapContent.transform.childCount)
		{
			GameObject centeredObj = wrapContent.transform.GetChild(itemIndex).gameObject;
			if (!EnableConfigurePos)
			{
				HandleCarouselCentering(centeredObj);
			}
		}
	}

	private void SetLabelsForElem(ShopCarouselElement sce)
	{
	}

	public static void SetPersArmorVisible(Transform armorPoint)
	{
		SetRenderersVisibleFromPoint(armorPoint, ShowArmor);
		if (armorPoint.childCount <= 0)
		{
			return;
		}
		Transform child = armorPoint.GetChild(0);
		ArmorRefs component = child.GetChild(0).GetComponent<ArmorRefs>();
		if (component != null)
		{
			if (component.leftBone != null)
			{
				SetRenderersVisibleFromPoint(component.leftBone, ShowArmor);
			}
			if (component.rightBone != null)
			{
				SetRenderersVisibleFromPoint(component.rightBone, ShowArmor);
			}
		}
	}

	public static void SetPersHatVisible(Transform hatPoint)
	{
	}

	public static void SetRenderersVisibleFromPoint(Transform pt, bool showArmor)
	{
		Player_move_c.PerformActionRecurs(pt.gameObject, delegate(Transform t)
		{
			Renderer component = t.GetComponent<Renderer>();
			if (component != null)
			{
				component.material.shader = Shader.Find((!showArmor) ? "Mobile/Transparent-Shop" : "Mobile/Diffuse");
			}
		});
	}

	private void Awake()
	{
		showHatButton.gameObject.SetActive(false);
		_ShowArmor = PlayerPrefs.GetInt("ShowArmorKeySetting", 1) == 1;
		_ShowHat = PlayerPrefs.GetInt("ShowHatKeySetting", 1) == 1;
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager();
		timeToUpdateTempGunTime = Time.realtimeSinceStartup;
		if (category != null)
		{
			category.Clicked += delegate(object sender, MultipleToggleEventArgs e)
			{
				if (e != null)
				{
					if (Defs.isSoundFX && category.buttons != null && category.buttons.Length > e.Num)
					{
						category.buttons[e.Num].offButton.GetComponent<UIPlaySound>().Play();
					}
					try
					{
						if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
						{
							if (e.Num == 4 && trainingState == TrainingState.NotInSniperCategory)
							{
								AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Category_Sniper);
								setOnSniperRifle();
							}
							else if (e.Num != 4 && (trainingState == TrainingState.OnSniperRifle || trainingState == TrainingState.InSniperCategoryNotOnSniperRifle))
							{
								setNotInSniperCategory();
							}
							else if (e.Num == 7 && trainingState == TrainingState.NotInArmorCategory)
							{
								setOnArmor();
								AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Category_Armor);
							}
							else if (e.Num != 7 && (trainingState == TrainingState.OnArmor || trainingState == TrainingState.InArmorCategoryNotOnArmor))
							{
								setNotInArmorCategory();
							}
						}
						if (InTrainingAfterNoviceArmorRemoved)
						{
							if (e.Num == 7 && trainingStateRemoveNoviceArmor == TrainingState.NotInArmorCategory)
							{
								setOnArmorRemovedNoviceArmor();
								AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Category_Armor);
							}
							else if (e.Num != 7 && (trainingStateRemoveNoviceArmor == TrainingState.OnArmor || trainingStateRemoveNoviceArmor == TrainingState.InArmorCategoryNotOnArmor))
							{
								setNotInArmorCategoryRemovedNoviceArmor();
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Exceptio in training in category.Clicked: " + ex);
					}
					CategoryChosen((CategoryNames)e.Num);
				}
			};
		}
		if (buy != null)
		{
			buy.GetComponent<ButtonHandler>().Clicked += delegate
			{
				BuyOrUpgradeWeapon();
			};
		}
		if (buyGear != null)
		{
			buyGear.GetComponent<ButtonHandler>().Clicked += delegate
			{
				BuyOrUpgradeWeapon();
			};
		}
		if (upgrade != null)
		{
			upgrade.GetComponent<ButtonHandler>().Clicked += delegate
			{
				BuyOrUpgradeWeapon(true);
			};
		}
		if (upgradeGear != null)
		{
			upgradeGear.GetComponent<ButtonHandler>().Clicked += delegate
			{
				BuyOrUpgradeWeapon(true);
			};
		}
		showArmorButton.IsChecked = !ShowArmor;
		showHatButton.IsChecked = !ShowHat;
		showArmorButtonTempArmor.IsChecked = !ShowArmor;
		showHatButtonTempHat.IsChecked = !ShowHat;
		Action toggleShowArmor = delegate
		{
			ShowArmor = !ShowArmor;
			SetPersArmorVisible(armorPoint);
			PlayerPrefs.SetInt("ShowArmorKeySetting", ShowArmor ? 1 : 0);
		};
		Action toggleShowHat = delegate
		{
			ShowHat = !ShowHat;
			SetPersHatVisible(hatPoint);
			PlayerPrefs.SetInt("ShowHatKeySetting", ShowHat ? 1 : 0);
		};
		showArmorButton.Clicked += delegate
		{
			toggleShowArmor();
			showArmorButtonTempArmor.SetCheckedWithoutEvent(!ShowArmor);
		};
		showHatButton.Clicked += delegate
		{
			toggleShowHat();
			showHatButtonTempHat.SetCheckedWithoutEvent(!ShowHat);
		};
		showArmorButtonTempArmor.Clicked += delegate
		{
			toggleShowArmor();
			showArmorButton.SetCheckedWithoutEvent(!ShowArmor);
		};
		showHatButtonTempHat.Clicked += delegate
		{
			toggleShowHat();
			showHatButton.SetCheckedWithoutEvent(!ShowHat);
		};
		sharedShop = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		ActiveObject.SetActive(false);
		if (coinShopButton != null)
		{
			coinShopButton.GetComponent<ButtonHandler>().Clicked += delegate
			{
				if (!(Time.realtimeSinceStartup - timeOfEnteringShopForProtectFromPressingCoinsButton < 0.5f) && BankController.Instance != null)
				{
					if (BankController.Instance.InterfaceEnabledCoroutineLocked)
					{
						Debug.LogWarning("InterfaceEnabledCoroutineLocked");
					}
					else
					{
						EventHandler handleBackFromBank = null;
						handleBackFromBank = delegate
						{
							if (BankController.Instance.InterfaceEnabledCoroutineLocked)
							{
								Debug.LogWarning("InterfaceEnabledCoroutineLocked");
							}
							else
							{
								BankController.Instance.BackRequested -= handleBackFromBank;
								BankController.Instance.InterfaceEnabled = false;
								GuiActive = true;
							}
						};
						BankController.Instance.BackRequested += handleBackFromBank;
						BankController.Instance.InterfaceEnabled = true;
						GuiActive = false;
					}
				}
			};
		}
		if (backButton != null)
		{
			backButton.GetComponent<ButtonHandler>().Clicked += delegate
			{
				StartCoroutine(BackAfterDelay());
			};
		}
		UIButton[] array = equips;
		UIButton ee;
		for (int i = 0; i < array.Length; i++)
		{
			ee = array[i];
			ee.GetComponent<ButtonHandler>().Clicked += delegate
			{
				if (Defs.isSoundFX)
				{
					ee.GetComponent<UIPlaySound>().Play();
				}
				if (WeaponCategory)
				{
					string text2 = WeaponManager.LastBoughtTag(viewedId);
					if (text2 == null && WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsAvailableTryGun(viewedId))
					{
						text2 = viewedId;
					}
					if (text2 == null)
					{
						return;
					}
					Weapon w = null;
					string prefabName = ItemDb.GetByTag(text2).PrefabName;
					foreach (Weapon allAvailablePlayerWeapon in WeaponManager.sharedManager.allAvailablePlayerWeapons)
					{
						if (allAvailablePlayerWeapon.weaponPrefab.name.Replace("(Clone)", string.Empty) == prefabName)
						{
							w = allAvailablePlayerWeapon;
							break;
						}
					}
					WeaponManager.sharedManager.EquipWeapon(w, true, true);
					WeaponManager.sharedManager.SaveWeaponAsLastUsed(WeaponManager.sharedManager.CurrentWeaponIndex);
					if (equipAction != null)
					{
						equipAction(text2);
					}
					chosenId = text2;
					UpdateIcon(currentCategory);
					if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted && trainingState == TrainingState.OnSniperRifle && text2 != null && text2 == WeaponTags.HunterRifleTag)
					{
						if (Storager.getInt("Training.NoviceArmorUsedKey", false) == 1)
						{
							setBackBlinking();
						}
						else
						{
							setNotInArmorCategory();
							AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Equip_Sniper);
						}
					}
					if (InTrainingAfterNoviceArmorRemoved)
					{
						HideAllTrainingInterfaceRemovedNoviceArmor();
						InTrainingAfterNoviceArmorRemoved = false;
						HandleActionsUUpdated();
					}
				}
				else if (WearCategory)
				{
					string text3 = WeaponManager.LastBoughtTag(viewedId);
					if (text3 != null)
					{
						EquipWear(text3);
					}
					if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
					{
						if (trainingState == TrainingState.OnArmor)
						{
							setBackBlinking();
							AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Equip_Armor);
						}
					}
					else if (InTrainingAfterNoviceArmorRemoved)
					{
						HideAllTrainingInterfaceRemovedNoviceArmor();
						InTrainingAfterNoviceArmorRemoved = false;
						HandleActionsUUpdated();
					}
				}
				else if (currentCategory == CategoryNames.SkinsCategory)
				{
					SetSkinAsCurrent(viewedId);
					UpdateIcon(currentCategory, true);
				}
				UpdateButtons();
			};
		}
		unequip.GetComponent<ButtonHandler>().Clicked += delegate
		{
			if (Defs.isSoundFX)
			{
				unequip.GetComponent<UIPlaySound>().Play();
			}
			if (WearCategory)
			{
				CategoryNames cat = currentCategory;
				UnequipCurrentWearInCategory(cat, inGame);
			}
			UpdateButtons();
		};
		enable.GetComponent<ButtonHandler>().Clicked += delegate
		{
			if (viewedId != null && viewedId.Equals("CustomSkinID"))
			{
				ItemPrice itemPrice = currentPrice(viewedId, currentCategory);
				int priceAmount = itemPrice.Price;
				string priceCurrency = itemPrice.Currency;
				TryToBuy(mainPanel, itemPrice, delegate
				{
					if (Defs.isSoundFX)
					{
						enable.GetComponent<UIPlaySound>().Play();
					}
					if (ShopNGUIController.GunBought != null)
					{
						ShopNGUIController.GunBought();
					}
					FlurryPluginWrapper.LogPurchaseByModes(CategoryNames.SkinsCategory, Defs.SkinsMakerInProfileBought, 1, false);
					FlurryPluginWrapper.LogPurchasesPoints(false);
					FlurryPluginWrapper.LogPurchaseByPoints(CategoryNames.SkinsCategory, Defs.SkinsMakerInProfileBought, 1);
					LogShopPurchasesTotalAndPayingNonPaying(Defs.SkinsMakerInProfileBought);
					string text = FlurryEvents.shopCategoryToLogSalesNamesMapping[CategoryNames.SkinsCategory];
					AnalyticsStuff.LogSales(Defs.SkinsMakerInProfileBought, text);
					AnalyticsFacade.InAppPurchase(Defs.SkinsMakerInProfileBought, text, 1, priceAmount, priceCurrency);
					Storager.setInt(Defs.SkinsMakerInProfileBought, 1, true);
					SynchronizeAndroidPurchases("Custom skin");
					FlurryPluginWrapper.LogEvent("Enable_Custom Skin");
					wholePrice.gameObject.SetActive(false);
					if (!inGame)
					{
						goToSM();
					}
				}, delegate
				{
					FlurryPluginWrapper.LogEvent("Try_Enable_Custom Skin");
				}, null, delegate
				{
					PlayWeaponAnimation();
				}, delegate
				{
					SetBankCamerasEnabled();
				}, delegate
				{
					SetOtherCamerasEnabled(false);
				});
				UpdateButtons();
			}
			else if (viewedId != null && viewedId.Equals("cape_Custom"))
			{
				BuyOrUpgradeWeapon();
			}
		};
		create.GetComponent<ButtonHandler>().Clicked += delegate
		{
			ButtonClickSound.Instance.PlayClick();
			if (!inGame)
			{
				goToSM();
			}
		};
		edit.GetComponent<ButtonHandler>().Clicked += delegate
		{
			ButtonClickSound.Instance.PlayClick();
			if (!inGame)
			{
				goToSM();
			}
		};
		delete.GetComponent<ButtonHandler>().Clicked += delegate
		{
			string skinWhereDelteWasPressed = viewedId;
			InfoWindowController.ShowDialogBox(LocalizationStore.Get("Key_1693"), delegate
			{
				ButtonClickSound.Instance.PlayClick();
				string currentSkinNameForPers = SkinsController.currentSkinNameForPers;
				if (skinWhereDelteWasPressed != null)
				{
					SkinsController.DeleteUserSkin(skinWhereDelteWasPressed);
					if (skinWhereDelteWasPressed.Equals(currentSkinNameForPers))
					{
						SetSkinAsCurrent("0");
						UpdateIcon(CategoryNames.SkinsCategory);
					}
				}
				ReloadCarousel(SkinsController.currentSkinNameForPers ?? "0");
			});
		};
		hats.AddRange(Resources.LoadAll<GameObject>("Hats"));
		sort(hats, CategoryNames.HatsCategory);
		capes.AddRange(Resources.LoadAll<GameObject>("Capes"));
		sort(capes, CategoryNames.CapesCategory);
		masks.AddRange(Resources.LoadAll<GameObject>("Masks"));
		sort(masks, CategoryNames.MaskCategory);
		boots.AddRange(Resources.LoadAll<GameObject>("Shop_Boots"));
		sort(boots, CategoryNames.BootsCategory);
		pixlMan = Resources.Load<GameObject>("PixlManForSkins");
		if (!Device.IsLoweMemoryDevice)
		{
			_onPersArmorRefs = Resources.LoadAll<GameObject>("Armor_Shop");
		}
		if (!Device.isPixelGunLow)
		{
			List<GameObject> list = sharedShop.armor;
		}
		if (Device.isPixelGunLow)
		{
			_refOnLowPolyArmor = Resources.Load<GameObject>("Armor_Low");
			_refsOnLowPolyArmorMaterials = Resources.LoadAll<Material>("LowPolyArmorMaterials");
		}
	}

	private void LoadGear()
	{
		gear.Clear();
		string[] array = ((!Defs.isDaterRegim) ? GearManager.Gear : GearManager.DaterGear);
		foreach (string id in array)
		{
			string text = GearManager.UpgradeIDForGear(GearManager.HolderQuantityForID(id), GearManager.CurrentNumberOfUphradesForGear(id));
			GameObject gameObject = Resources.Load<GameObject>("Shop_Gear/" + text);
			if (gameObject != null)
			{
				gear.Add(gameObject);
			}
		}
		gear.Sort((GameObject go1, GameObject go2) => Array.IndexOf(gearOrder, go1.name) - Array.IndexOf(gearOrder, go2.name));
	}

	public void goToSM()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("SkinEditorController"));
		SkinEditorController component = gameObject.GetComponent<SkinEditorController>();
		if (!(component != null))
		{
			return;
		}
		Action<string> backHandler = null;
		backHandler = delegate(string n)
		{
			SkinEditorController.ExitFromSkinEditor -= backHandler;
			MenuBackgroundMusic.sharedMusic.StopCustomMusicFrom(SkinEditorController.sharedController.gameObject);
			mainPanel.SetActive(true);
			if (currentCategory == CategoryNames.CapesCategory || n != null)
			{
				if (viewedId != null && viewedId.Equals("CustomSkinID"))
				{
					SetSkinAsCurrent(n);
				}
				if (currentCategory == CategoryNames.SkinsCategory && viewedId != null && viewedId.Equals(SkinsController.currentSkinNameForPers))
				{
					FireOnEquipSkin(n);
				}
				if (viewedId != null && viewedId.Equals("cape_Custom"))
				{
					EquipWear("cape_Custom");
				}
				StartCoroutine(ReloadAfterEditing(n));
			}
			else
			{
				StartCoroutine(ReloadAfterEditing(n, n == null));
			}
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC == null)
			{
				if (currentCategory != CategoryNames.CapesCategory && PlayerPrefs.GetInt(Defs.ShownRewardWindowForSkin, 0) == 0)
				{
					_shouldShowRewardWindowSkin = true;
				}
				if (currentCategory == CategoryNames.CapesCategory && PlayerPrefs.GetInt(Defs.ShownRewardWindowForCape, 0) == 0)
				{
					_shouldShowRewardWindowCape = true;
				}
			}
		};
		SkinEditorController.ExitFromSkinEditor += backHandler;
		SkinEditorController.currentSkinName = ((!viewedId.Equals("CustomSkinID")) ? viewedId : null);
		SkinEditorController.modeEditor = ((currentCategory != CategoryNames.SkinsCategory) ? SkinEditorController.ModeEditor.Cape : SkinEditorController.ModeEditor.SkinPers);
		mainPanel.SetActive(false);
	}

	public IEnumerator ReloadAfterEditing(string n, bool shouldReload = true)
	{
		yield return null;
		if (shouldReload)
		{
			ReloadCarousel(n ?? "cape_Custom");
		}
		PlayWeaponAnimation();
		UpdateIcon(currentCategory);
	}

	private static void SetBankCamerasEnabled()
	{
		List<Camera> list = BankController.Instance.GetComponentsInChildren<Camera>(true).ToList();
		List<Camera> list2 = list;
		foreach (Camera item in list2)
		{
			if ((!(ExpController.Instance != null) || !ExpController.Instance.IsRenderedWithCamera(item)) && !item.gameObject.tag.Equals("CamTemp") && !sharedShop.ourCameras.Contains(item))
			{
				item.rect = new Rect(0f, 0f, 1f, 1f);
			}
		}
	}

	private void BuyOrUpgradeWeapon(bool upgradeNotBuy = false)
	{
		string id = ((currentCategory != CategoryNames.GearCategory) ? viewedId : ((!upgradeNotBuy) ? IDForCurrentGear : NextUpgradeIDForCurrentGear));
		string tg = id;
		if (WeaponManager.tagToStoreIDMapping.ContainsKey(tg))
		{
			id = WeaponManager.tagToStoreIDMapping[WeaponManager.FirstUnboughtOrForOurTier(tg)];
		}
		if (WearCategory)
		{
			id = WeaponManager.FirstUnboughtTag(id);
			tg = id;
		}
		if (id == null)
		{
			return;
		}
		ItemPrice price = currentPrice(viewedId, currentCategory, upgradeNotBuy);
		TryToBuy(mainPanel, price, delegate
		{
			if (Defs.isSoundFX)
			{
				((!upgradeNotBuy) ? buy : upgrade).GetComponent<UIPlaySound>().Play();
			}
			ActualBuy(id, tg, price);
		}, delegate
		{
			if (currentCategory == CategoryNames.CapesCategory && viewedId.Equals("cape_Custom"))
			{
				FlurryPluginWrapper.LogEvent("Try_Enable_Custom Cape");
			}
		}, null, delegate
		{
			PlayWeaponAnimation();
		}, delegate
		{
			SetBankCamerasEnabled();
		}, delegate
		{
			SetOtherCamerasEnabled(false);
			StartCoroutine(ReloadAfterEditing(viewedId));
		});
	}

	public static void ProvideShopItemOnStarterPackBoguht(CategoryNames c, string sourceTg, int gearCount = 1, bool buyArmorUpToSourceTg = false, int timeForRentIndex = 0, Action<string> contextSpecificAction = null, Action<string> customEquipWearAction = null, bool equipSkin = true, bool equipWear = true, bool doAndroidCloudSync = true)
	{
		string text = ((c != CategoryNames.GearCategory) ? sourceTg : GearManager.HolderQuantityForID(sourceTg));
		string text2 = text;
		if (WeaponManager.tagToStoreIDMapping.ContainsKey(text2))
		{
			text = WeaponManager.tagToStoreIDMapping[text2];
		}
		if (text == null)
		{
			return;
		}
		ProvdeShopItemWithRightId(c, text, text2, null, delegate(string item)
		{
			if (customEquipWearAction != null)
			{
				customEquipWearAction(item);
			}
			else if (equipWear)
			{
				SetAsEquippedAndSendToServer(item, c);
			}
		}, contextSpecificAction, delegate(string item)
		{
			if (equipSkin)
			{
				SaveSkinAndSendToServer(item);
			}
		}, true, gearCount, buyArmorUpToSourceTg, timeForRentIndex, doAndroidCloudSync);
	}

	private void EquipTemporaryItem(string itemTag)
	{
	}

	public static void ProvideAllTypeShopItem(CategoryNames category, string sourceTag, int gearCount, int timeForRent)
	{
		int timeForRentIndex = 0;
		if (timeForRent != -1)
		{
			int days = timeForRent / 24;
			timeForRentIndex = TempItemsController.RentIndexFromDays(days);
		}
		ProvideShopItemOnStarterPackBoguht(category, sourceTag, gearCount, false, timeForRentIndex, null, delegate(string tg)
		{
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.weaponsInGame != null)
			{
				if (GuiActive && sharedShop != null)
				{
					int num = PromoActionsGUIController.CatForTg(tg);
					if (num != -1)
					{
						EquipWearInCategoryIfNotEquiped(tg, (CategoryNames)num, WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null);
					}
				}
				else
				{
					int num2 = PromoActionsGUIController.CatForTg(tg);
					if (num2 != -1)
					{
						SetAsEquippedAndSendToServer(tg, (CategoryNames)num2);
					}
				}
			}
		});
		TempItemsController.sharedController.ExpiredItems.Remove(sourceTag);
	}

	private static int AddedNumberOfGearWhenBuyingPack(string id)
	{
		int num = GearManager.ItemsInPackForGear(GearManager.HolderQuantityForID(id));
		if (Storager.getInt(id, false) + num > GearManager.MaxCountForGear(id))
		{
			num = GearManager.MaxCountForGear(id) - Storager.getInt(id, false);
		}
		return num;
	}

	private static void ProvdeShopItemWithRightId(CategoryNames c, string id, string tg, Action UNUSED_DO_NOT_SET_onTrainingAction, Action<string> onEquipWearAction, Action<string> contextSpecificAction, Action<string> onSkinBoughtAction, bool giveOneItemOfGear = false, int gearCount = 1, bool buyArmorAndHatsUpToTg = false, int timeForRentIndex = 0, bool doAndroidCloudSync = true)
	{
		if (ShopNGUIController.GunBought != null)
		{
			ShopNGUIController.GunBought();
		}
		if (IsWearCategory(c))
		{
			if (buyArmorAndHatsUpToTg && Wear.wear.ContainsKey(c))
			{
				List<List<string>> list = Wear.wear[c];
				List<string> list2 = null;
				foreach (List<string> item in list)
				{
					if (item.Contains(tg))
					{
						list2 = item;
						break;
					}
				}
				if (list2 != null)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						Storager.setInt(list2[i], 1, true);
						if (list2[i].Equals(tg))
						{
							break;
						}
					}
				}
			}
			else if (TempItemsController.PriceCoefs.ContainsKey(tg))
			{
				int tm = TempItemsController.RentTimeForIndex(timeForRentIndex);
				TempItemsController.sharedController.AddTemporaryItem(tg, tm);
			}
			else
			{
				Storager.setInt(tg, 1, true);
			}
			if ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted) && doAndroidCloudSync)
			{
				SynchronizeAndroidPurchases("Wear: " + tg);
			}
			if (onEquipWearAction != null)
			{
				onEquipWearAction(tg);
			}
		}
		if (IsWeaponCategory(c) && !WeaponManager.FirstUnboughtTag(tg).Equals(tg))
		{
			List<string> list3 = WeaponUpgrades.ChainForTag(tg);
			if (list3 != null)
			{
				int num = list3.IndexOf(tg) - 1;
				if (num >= 0)
				{
					for (int j = 0; j <= num; j++)
					{
						try
						{
							Storager.setInt(WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[list3[j]]], 1, true);
						}
						catch
						{
							Debug.LogError("Error filling chain in indexOfWeaponBeforeCurrentTg");
						}
					}
				}
			}
		}
		WeaponManager.sharedManager.AddMinerWeapon(id, timeForRentIndex);
		if (WeaponManager.sharedManager != null)
		{
			try
			{
				bool flag = WeaponManager.sharedManager.IsAvailableTryGun(WeaponManager.LastBoughtTag(tg));
				WeaponManager.sharedManager.TryGuns.Remove(WeaponManager.LastBoughtTag(tg));
				WeaponManager.sharedManager.ExpiredTryGuns.RemoveAll((string expiredGunTag) => expiredGunTag == WeaponManager.LastBoughtTag(tg));
				bool flag2 = WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(WeaponManager.LastBoughtTag(tg));
				WeaponManager.sharedManager.RemoveDiscountForTryGun(WeaponManager.LastBoughtTag(tg));
				if (flag2)
				{
					string empty = string.Empty;
					string itemNameNonLocalized = ItemDb.GetItemNameNonLocalized(WeaponManager.LastBoughtTag(tg), empty, c);
					AnalyticsStuff.LogWEaponsSpecialOffers_Conversion(false, itemNameNonLocalized);
				}
				if (flag2 || flag)
				{
					Action<string> tryGunBought = ShopNGUIController.TryGunBought;
					if (tryGunBought != null)
					{
						tryGunBought(WeaponManager.LastBoughtTag(tg));
					}
					if (FriendsController.useBuffSystem)
					{
						BuffSystem.instance.OnTryGunBuyed(ItemDb.GetByTag(tg).PrefabName);
					}
					else
					{
						KillRateCheck.OnTryGunBuyed();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in removeing TryGun structures: " + ex);
			}
		}
		if (c == CategoryNames.GearCategory)
		{
			if (id.Contains(GearManager.UpgradeSuffix))
			{
				string key = GearManager.NameForUpgrade(GearManager.HolderQuantityForID(id), GearManager.CurrentNumberOfUphradesForGear(GearManager.HolderQuantityForID(id)) + 1);
				Storager.setInt(key, 1, false);
			}
			else
			{
				int num2 = AddedNumberOfGearWhenBuyingPack(id);
				Storager.setInt(id, Storager.getInt(id, false) + ((!giveOneItemOfGear) ? num2 : gearCount), false);
			}
		}
		if (contextSpecificAction != null)
		{
			contextSpecificAction(id);
		}
		if (c != CategoryNames.SkinsCategory)
		{
			return;
		}
		if (id != null && SkinsController.shopKeyFromNameSkin.ContainsKey(id))
		{
			string text = SkinsController.shopKeyFromNameSkin[id];
			if (Array.IndexOf(StoreKitEventListener.skinIDs, text) >= 0)
			{
				foreach (KeyValuePair<string, string> value in InAppData.inAppData.Values)
				{
					if (value.Key != null && value.Key.Equals(text))
					{
						Storager.setInt(value.Value, 1, true);
						if (doAndroidCloudSync)
						{
							SynchronizeAndroidPurchases("Skin: " + text);
						}
						break;
					}
				}
			}
		}
		if (onSkinBoughtAction != null)
		{
			onSkinBoughtAction(id);
		}
	}

	public void FireBuyAction(string item)
	{
		if (buyAction != null)
		{
			buyAction(item);
		}
	}

	private void LogShopPurchasesTotalAndPayingNonPaying(string itemName)
	{
		try
		{
			string text = currentCategory.ToString();
			string eventName = string.Format("Shop Purchases {0}", "Total");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("All Categories", text);
			dictionary.Add(text, itemName);
			dictionary.Add("Item", itemName);
			Dictionary<string, string> dictionary2 = dictionary;
			if (currentCategory != CategoryNames.GearCategory)
			{
				dictionary2.Add("Without Quick Shop", itemName);
			}
			FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, dictionary2);
			string payingSuffix = FlurryPluginWrapper.GetPayingSuffix();
			string eventName2 = string.Format("Shop Purchases {0}", "Total" + payingSuffix);
			FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName2, dictionary2);
		}
		catch (Exception ex)
		{
			Debug.LogError("LogShopPurchasesTotalAndPayingNonPaying exception: " + ex);
		}
	}

	private void ActualBuy(string id, string tg, ItemPrice itemPrice)
	{
		if (currentCategory == CategoryNames.ArmorCategory || IsWeaponCategory(currentCategory))
		{
			FireWeaponOrArmorBought();
		}
		CategoryNames c = currentCategory;
		int num = AddedNumberOfGearWhenBuyingPack(id);
		ProvdeShopItemWithRightId(c, id, tg, null, delegate(string item)
		{
			EquipWear(item);
		}, delegate(string item)
		{
			if (IsWeaponCategory(c) || IsWearCategory(c))
			{
				FireBuyAction(item);
			}
			purchaseSuccessful.SetActive(true);
			_timePurchaseSuccessfulShown = Time.realtimeSinceStartup;
		}, delegate(string item)
		{
			SetSkinAsCurrent(item);
		});
		if (WeaponManager.tagToStoreIDMapping.ContainsValue(id))
		{
			IEnumerable<string> source = from item in WeaponManager.tagToStoreIDMapping
				where item.Value == id
				select item into kv
				select kv.Key;
			SynchronizeAndroidPurchases("Weapon: " + source.FirstOrDefault());
			ItemPrice priceByShopId = ItemDb.GetPriceByShopId(id);
			int? num2 = null;
			if (priceByShopId != null)
			{
				num2 = priceByShopId.Price;
			}
			if (num2.HasValue && num2.Value >= PlayerPrefs.GetInt(Defs.MostExpensiveWeapon, 0))
			{
				PlayerPrefs.SetInt(Defs.MostExpensiveWeapon, num2.Value);
				PlayerPrefs.SetString(Defs.MenuPersWeaponTag, (source.Count() <= 0) ? string.Empty : source.ElementAt(0));
				PlayerPrefs.Save();
			}
		}
		string text = id;
		try
		{
			if (WeaponManager.tagToStoreIDMapping.ContainsKey(id))
			{
				text = WeaponManager.tagToStoreIDMapping[id];
			}
			if (currentCategory == CategoryNames.SkinsCategory && text != null && SkinsController.shopKeyFromNameSkin.ContainsKey(text))
			{
				text = SkinsController.shopKeyFromNameSkin[text];
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in setting shopId: " + ex);
		}
		try
		{
			string text2 = WeaponManager.LastBoughtTag(viewedId) ?? WeaponManager.FirstUnboughtTag(viewedId);
			string text3 = ItemDb.GetItemNameNonLocalized(text2, text, currentCategory);
			try
			{
				if (currentCategory == CategoryNames.SkinsCategory)
				{
					text3 = LocalizationStore.GetByDefault(SkinsController.skinsLocalizeKey[int.Parse(id)]);
				}
			}
			catch (Exception ex2)
			{
				Debug.LogError("Shop: ActualBuy: get readable skin name: " + ex2);
			}
			FlurryPluginWrapper.LogPurchaseByModes(currentCategory, (currentCategory != CategoryNames.GearCategory) ? text3 : GearManager.HolderQuantityForID(text), (currentCategory != CategoryNames.GearCategory) ? 1 : num, false);
			if (currentCategory != CategoryNames.GearCategory)
			{
				FlurryPluginWrapper.LogPurchasesPoints(IsWeaponCategory(currentCategory));
				FlurryPluginWrapper.LogPurchaseByPoints(currentCategory, text3, 1);
			}
			else
			{
				FlurryPluginWrapper.LogGearPurchases(GearManager.HolderQuantityForID(text), num, false);
				if (Application.loadedLevelName == Defs.MainMenuScene)
				{
					FlurryPluginWrapper.LogPurchaseByPoints(currentCategory, GearManager.HolderQuantityForID(text), num);
				}
			}
			try
			{
				bool isDaterWeapon = false;
				if (IsWeaponCategory(currentCategory))
				{
					WeaponSounds weaponInfo = ItemDb.GetWeaponInfo(text2);
					isDaterWeapon = weaponInfo != null && weaponInfo.IsAvalibleFromFilter(3);
				}
				string text4 = ((!FlurryEvents.shopCategoryToLogSalesNamesMapping.ContainsKey(currentCategory)) ? currentCategory.ToString() : FlurryEvents.shopCategoryToLogSalesNamesMapping[currentCategory]);
				AnalyticsStuff.LogSales(text3, text4, isDaterWeapon);
				AnalyticsFacade.InAppPurchase(text3, text4, 1, itemPrice.Price, itemPrice.Currency);
				if (_isFromPromoActions && _promoActionsIdClicked != null && text2 != null && _promoActionsIdClicked == text2)
				{
					AnalyticsStuff.LogSpecialOffersPanel("Efficiency", "Buy", text4 ?? "Unknown", text3);
				}
				_isFromPromoActions = false;
			}
			catch (Exception ex3)
			{
				Debug.LogError("Exception in LogSales block in Shop: " + ex3);
			}
			bool onlyServerDiscount;
			int num3 = DiscountFor(WeaponManager.LastBoughtTag(viewedId) ?? WeaponManager.FirstUnboughtTag(viewedId), out onlyServerDiscount);
			if (num3 > 0)
			{
				string itemNameNonLocalized = ItemDb.GetItemNameNonLocalized(WeaponManager.LastBoughtTag(viewedId) ?? WeaponManager.FirstUnboughtTag(viewedId), text, currentCategory, "Unknown");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(num3.ToString(), itemNameNonLocalized);
				Dictionary<string, string> parameters = dictionary;
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Offers Sale", parameters);
			}
			if (currentCategory == CategoryNames.GearCategory && text3 != null && !text3.Contains(GearManager.UpgradeSuffix) && GearManager.AllGear.Contains(text3))
			{
				text3 = GearManager.AnalyticsIDForOneItemOfGear(text3);
			}
			LogShopPurchasesTotalAndPayingNonPaying(text3);
			if (ExperienceController.sharedController != null)
			{
				int currentLevel = ExperienceController.sharedController.currentLevel;
				int num4 = (currentLevel - 1) / 9;
				string arg = string.Format("[{0}, {1})", num4 * 9 + 1, (num4 + 1) * 9 + 1);
				string eventName = string.Format("Shop Purchases On Level {0} ({1}){2}", arg, FlurryPluginWrapper.GetPayingSuffix().Trim(), string.Empty);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Level " + currentLevel, text3);
				Dictionary<string, string> parameters2 = dictionary;
				FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters2);
			}
			LogPurchaseAfterPaymentAnalyticsEvent(text3);
		}
		catch (Exception ex4)
		{
			Debug.LogError("Exception in Shop Logging: " + ex4);
		}
		chosenId = WeaponManager.LastBoughtTag(viewedId);
		viewedId = ((currentCategory != CategoryNames.GearCategory) ? chosenId : GearManager.NameForUpgrade(GearManager.HolderQuantityForID(viewedId), GearManager.CurrentNumberOfUphradesForGear(GearManager.HolderQuantityForID(viewedId))));
		UpdateIcon(currentCategory, true);
		ReloadCarousel();
		ChooseCarouselItem(viewedId, false, true);
		Resources.UnloadUnusedAssets();
		if (!inGame && currentCategory == CategoryNames.CapesCategory && viewedId.Equals("cape_Custom"))
		{
			FlurryPluginWrapper.LogEvent("Enable_Custom Cape");
			wholePrice.gameObject.SetActive(false);
			goToSM();
		}
	}

	private static void SaveSkinAndSendToServer(string id)
	{
		SkinsController.SetCurrentSkin(id);
		byte[] array = SkinsController.currentSkinForPers.EncodeToPNG();
		if (array != null)
		{
			string text = Convert.ToBase64String(array);
			if (text != null)
			{
				FriendsController.sharedController.skin = text;
				FriendsController.sharedController.SendOurData(true);
			}
		}
	}

	private void FireOnEquipSkin(string id)
	{
		if (onEquipSkinAction != null)
		{
			onEquipSkinAction(id);
		}
	}

	public void SetSkinAsCurrent(string id)
	{
		SaveSkinAndSendToServer(id);
		FireOnEquipSkin(id);
	}

	public static void SetAsEquippedAndSendToServer(string tg, CategoryNames c)
	{
		Storager.setString(SnForWearCategory(c), tg, false);
		if (FriendsController.sharedController == null)
		{
			Debug.LogError("FriendsController.sharedController == null");
			return;
		}
		switch (c)
		{
		case CategoryNames.CapesCategory:
			FriendsController.sharedController.capeName = tg;
			break;
		case CategoryNames.HatsCategory:
			FriendsController.sharedController.hatName = tg;
			break;
		case CategoryNames.BootsCategory:
			FriendsController.sharedController.bootsName = tg;
			break;
		case CategoryNames.ArmorCategory:
			FriendsController.sharedController.armorName = tg;
			break;
		case CategoryNames.MaskCategory:
			FriendsController.sharedController.maskName = tg;
			break;
		}
		if (c == CategoryNames.CapesCategory && tg.Equals("cape_Custom"))
		{
			byte[] array = SkinsController.capeUserTexture.EncodeToPNG();
			if (array != null)
			{
				string text = Convert.ToBase64String(array);
				if (text != null)
				{
					FriendsController.sharedController.capeSkin = text;
				}
			}
		}
		FriendsController.sharedController.SendOurData();
	}

	public IEnumerator BackAfterDelay()
	{
		_isFromPromoActions = false;
		yield return null;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			TrainingController.CompletedTrainingStage = TrainingController.NewTrainingCompletedStage.ShopCompleted;
			HintController.instance.StartShow();
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Back_Shop);
		}
		if (resumeAction != null)
		{
			resumeAction();
		}
		else
		{
			GuiActive = false;
		}
		if (wearResumeAction != null)
		{
			wearResumeAction();
		}
		if (InTrainingAfterNoviceArmorRemoved)
		{
			trainingColliders.SetActive(false);
			trainingRemoveNoviceArmorCollider.SetActive(false);
		}
		InTrainingAfterNoviceArmorRemoved = false;
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = false;
		}
	}

	public static string SnForWearCategory(CategoryNames c)
	{
		object result;
		switch (c)
		{
		case CategoryNames.CapesCategory:
			result = Defs.CapeEquppedSN;
			break;
		case CategoryNames.BootsCategory:
			result = Defs.BootsEquppedSN;
			break;
		case CategoryNames.ArmorCategory:
			result = Defs.ArmorNewEquppedSN;
			break;
		case CategoryNames.MaskCategory:
			result = "MaskEquippedSN";
			break;
		default:
			result = Defs.HatEquppedSN;
			break;
		}
		return (string)result;
	}

	public static string NoneEquippedForWearCategory(CategoryNames c)
	{
		object result;
		switch (c)
		{
		case CategoryNames.CapesCategory:
			result = Defs.CapeNoneEqupped;
			break;
		case CategoryNames.BootsCategory:
			result = Defs.BootsNoneEqupped;
			break;
		case CategoryNames.ArmorCategory:
			result = Defs.ArmorNewNoneEqupped;
			break;
		case CategoryNames.MaskCategory:
			result = "MaskNoneEquipped";
			break;
		default:
			result = Defs.HatNoneEqupped;
			break;
		}
		return (string)result;
	}

	public string WearForCat(CategoryNames c)
	{
		string result;
		switch (c)
		{
		case CategoryNames.CapesCategory:
			result = _currentCape;
			break;
		case CategoryNames.BootsCategory:
			result = _currentBoots;
			break;
		case CategoryNames.ArmorCategory:
			result = _currentArmor;
			break;
		case CategoryNames.HatsCategory:
			result = _currentHat;
			break;
		case CategoryNames.MaskCategory:
			result = _currentMask;
			break;
		default:
			result = string.Empty;
			break;
		}
		return result;
	}

	private void SetWearForCategory(CategoryNames cat, string wear)
	{
		switch (cat)
		{
		case CategoryNames.CapesCategory:
			_currentCape = wear;
			break;
		case CategoryNames.HatsCategory:
			_currentHat = wear;
			break;
		case CategoryNames.BootsCategory:
			_currentBoots = wear;
			break;
		case CategoryNames.ArmorCategory:
			_currentArmor = wear;
			break;
		case CategoryNames.MaskCategory:
			_currentMask = wear;
			break;
		case CategoryNames.SkinsCategory:
		case CategoryNames.GearCategory:
			break;
		}
	}

	public void LoadCurrentWearToVars()
	{
		_currentCape = Storager.getString(Defs.CapeEquppedSN, false);
		_currentHat = Storager.getString(Defs.HatEquppedSN, false);
		_currentBoots = Storager.getString(Defs.BootsEquppedSN, false);
		_currentArmor = Storager.getString(Defs.ArmorNewEquppedSN, false);
		_currentMask = Storager.getString("MaskEquippedSN", false);
	}

	private void HandleActionsUUpdated()
	{
		UpdateButtons();
		UpdateItemParameters();
	}

	private void OnDestroy()
	{
		if (profile != null)
		{
			Resources.UnloadAsset(profile);
			profile = null;
		}
	}

	private void Start()
	{
		StartCoroutine(TryToShowExpiredBanner());
	}

	private IEnumerator TryToShowExpiredBanner()
	{
		while (FriendsController.sharedController == null || TempItemsController.sharedController == null)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(1f));
			try
			{
				if (!GuiActive || rentScreenPoint.childCount != 0)
				{
					continue;
				}
				if (Storager.getInt("Training.ShouldRemoveNoviceArmorInShopKey", false) == 1)
				{
					GameObject window = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/WeRemoveNoviceArmorBanner"));
					window.transform.parent = rentScreenPoint;
					Player_move_c.SetLayerRecursively(window, LayerMask.NameToLayer("NGUIShop"));
					window.transform.localPosition = new Vector3(0f, 0f, -130f);
					window.transform.localRotation = Quaternion.identity;
					window.transform.localScale = new Vector3(1f, 1f, 1f);
					UpdatePersArmor(Defs.ArmorNewNoneEqupped);
					sharedShop.trainingColliders.SetActive(true);
					sharedShop.trainingRemoveNoviceArmorCollider.SetActive(true);
					trainingStateRemoveNoviceArmor = TrainingState.NotInArmorCategory;
					InTrainingAfterNoviceArmorRemoved = true;
					if (HintController.instance != null)
					{
						HintController.instance.HideHintByName("shop_remove_novice_armor");
					}
				}
				else if ((_shouldShowRewardWindowSkin || _shouldShowRewardWindowCape) && !Device.isPixelGunLow)
				{
					PlayerPrefs.SetInt((!_shouldShowRewardWindowSkin) ? Defs.ShownRewardWindowForCape : Defs.ShownRewardWindowForSkin, 1);
					if (TwitterController.TwitterSupported)
					{
						_isFromPromoActions = false;
						GameObject window2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/CreateNewItemNGUI"));
						RewardWindowBase rwb = window2.GetComponent<RewardWindowBase>();
						bool skin = _shouldShowRewardWindowSkin;
						rwb.HasReward = false;
						if (skin)
						{
							rwb.twitterStatus = () => "I've painted a new skin in @PixelGun3D! Make your own one now! #pixelgun3d #pixelgun #pg3d #mobile #fps #shooter http://goo.gl/8fzL9u";
							rwb.EventTitle = "Painted Skin";
						}
						else
						{
							rwb.twitterStatus = () => "I've painted a new cape in @PixelGun3D! Make your own one now! #pixelgun3d #pixelgun #pg3d #mobile #fps #shooter http://goo.gl/8fzL9u";
							rwb.EventTitle = "Painted Cape";
						}
						window2.transform.parent = rentScreenPoint;
						Player_move_c.SetLayerRecursively(window2, LayerMask.NameToLayer("NGUIShop"));
						window2.transform.localPosition = new Vector3(0f, 0f, -130f);
						window2.transform.localRotation = Quaternion.identity;
						window2.transform.localScale = new Vector3(1f, 1f, 1f);
						DrawItemRewardWindow dirw = window2.GetComponent<DrawItemRewardWindow>();
						dirw.skin.SetActive(_shouldShowRewardWindowSkin);
						dirw.cape.SetActive(!_shouldShowRewardWindowSkin);
					}
					if (_shouldShowRewardWindowSkin)
					{
						_shouldShowRewardWindowSkin = false;
					}
					else if (_shouldShowRewardWindowCape)
					{
						_shouldShowRewardWindowCape = false;
					}
				}
				else
				{
					if (Storager.getInt(Defs.PremiumEnabledFromServer, false) == 1 && WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC == null && ShowPremimAccountExpiredIfPossible(rentScreenPoint, "NGUIShop", string.Empty))
					{
						continue;
					}
					ShowTempItemExpiredIfPossible(rentScreenPoint, "NGUIShop", delegate(string tg)
					{
						if (PromoActionsGUIController.CatForTg(tg) == (int)currentCategory)
						{
							chosenId = WeaponManager.LastBoughtTag(tg);
						}
						if (tg.Equals(viewedId))
						{
							viewedId = ((currentCategory != CategoryNames.GearCategory) ? chosenId : GearManager.NameForUpgrade(GearManager.HolderQuantityForID(viewedId), GearManager.CurrentNumberOfUphradesForGear(GearManager.HolderQuantityForID(viewedId))));
							UpdateButtons();
						}
						UpdateIcons();
						purchaseSuccessful.SetActive(true);
						_timePurchaseSuccessfulShown = Time.realtimeSinceStartup;
					}, delegate
					{
						SetBankCamerasEnabled();
					}, delegate
					{
						SetOtherCamerasEnabled(false);
					}, delegate(string item)
					{
						if (item != null)
						{
							int num = PromoActionsGUIController.CatForTg(item);
							if (num != -1)
							{
								EquipWearInCategoryIfNotEquiped(item, (CategoryNames)num, inGame);
							}
						}
					});
					continue;
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("exception in Shop  TryToShowExpiredBanner: " + e);
			}
		}
	}

	private void Update()
	{
		if (!ActiveObject.activeInHierarchy)
		{
			return;
		}
		ExperienceController.sharedController.isShowRanks = rentScreenPoint.childCount == 0 && SkinEditorController.sharedController == null && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled);
		if (Time.realtimeSinceStartup - timeToUpdateTempGunTime >= 1f)
		{
			timeToUpdateTempGunTime = Time.realtimeSinceStartup;
			if (GuiActive && viewedId != null && WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(viewedId))
			{
				UpdateTryGunDiscountTime();
			}
		}
		string showHatTag = ((hatPoint.transform.childCount > 1) ? hatPoint.transform.GetChild(1).gameObject.tag : ((hatPoint.transform.childCount <= 0) ? "none" : hatPoint.transform.GetChild(0).gameObject.tag));
		bool flag = currentCategory == CategoryNames.HatsCategory && !Wear.NonArmorHat(showHatTag);
		bool flag2 = currentCategory == CategoryNames.ArmorCategory && viewedId != null && !TempItemsController.PriceCoefs.ContainsKey(viewedId);
		if (showArmorButton.gameObject.activeSelf != flag2)
		{
			showArmorButton.gameObject.SetActive(flag2);
		}
		bool flag3 = false;
		if (showHatButton.gameObject.activeSelf != flag3)
		{
			showHatButton.gameObject.SetActive(flag3);
		}
		bool flag4 = currentCategory == CategoryNames.ArmorCategory && viewedId != null && TempItemsController.PriceCoefs.ContainsKey(viewedId);
		if (showArmorButtonTempArmor.gameObject.activeSelf != flag4)
		{
			showArmorButtonTempArmor.gameObject.SetActive(flag4);
		}
		bool flag5 = flag && viewedId != null && TempItemsController.PriceCoefs.ContainsKey(viewedId);
		if (showHatButtonTempHat.gameObject.activeSelf != flag5)
		{
			showHatButtonTempHat.gameObject.SetActive(flag5);
		}
		if (Time.realtimeSinceStartup - _timePurchaseSuccessfulShown >= 2f)
		{
			purchaseSuccessful.SetActive(false);
		}
		if (Time.realtimeSinceStartup - _timePurchaseRentSuccessfulShown >= 2f)
		{
			purchaseSuccessfulRent.SetActive(false);
		}
		if (mainPanel.activeInHierarchy && !HOTween.IsTweening(MainMenu_Pers))
		{
			float num = -120f;
			num *= ((BuildSettings.BuildTargetPlatform == RuntimePlatform.Android) ? 2f : ((!Application.isEditor) ? 0.5f : 100f));
			Rect rect = new Rect(0f, 0.1f * (float)Screen.height, 0.5f * (float)Screen.width, 0.8f * (float)Screen.height);
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Moved && rect.Contains(touch.position))
				{
					idleTimerLastTime = Time.realtimeSinceStartup;
					MainMenu_Pers.Rotate(Vector3.up, touch.deltaPosition.x * num * 0.5f * (Time.realtimeSinceStartup - lastTime));
				}
			}
			if (Application.isEditor)
			{
				float num2 = Input.GetAxis("Mouse ScrollWheel") * 3f * num * (Time.realtimeSinceStartup - lastTime);
				MainMenu_Pers.Rotate(Vector3.up, num2);
				if (num2 != 0f)
				{
					idleTimerLastTime = Time.realtimeSinceStartup;
				}
			}
			lastTime = Time.realtimeSinceStartup;
		}
		if (currentCategory != CategoryNames.CapesCategory && Time.realtimeSinceStartup - idleTimerLastTime > IdleTimeoutPers)
		{
			SetCamera();
		}
		ActivityIndicator.IsActiveIndicator = StoreKitEventListener.restoreInProcess;
		CheckCenterItemChanging();
	}

	private void LateUpdate()
	{
		float num = scrollViewPanel.GetViewSize().x / 2f;
		ShopCarouselElement[] componentsInChildren = wrapContent.GetComponentsInChildren<ShopCarouselElement>(false);
		ShopCarouselElement[] array = componentsInChildren;
		foreach (ShopCarouselElement shopCarouselElement in array)
		{
			Transform transform = shopCarouselElement.transform;
			float x = scrollViewPanel.clipOffset.x;
			float num2 = Mathf.Abs(transform.localPosition.x - x);
			float num3 = scaleCoef + (1f - scaleCoef) * (1f - num2 / num);
			float num4 = 0.65f;
			num3 = ((!(num2 <= num / 3f)) ? (scaleCoef + (num4 - scaleCoef) * (1f - (num2 - num / 3f) / (num * 2f / 3f))) : (num4 + (1f - num4) * (1f - num2 / (num / 3f))));
			if (num2 >= num * 0.9f)
			{
				num3 = 0f;
			}
			float num5 = transform.localPosition.x - x;
			float num6 = 0f;
			float num7 = ((num5 <= 0f) ? 1 : (-1));
			if (num5 != 0f)
			{
				num6 = ((Mathf.Abs(num5) <= wrapContent.cellWidth) ? (firstOFfset * (Mathf.Abs(num5) / wrapContent.cellWidth)) : ((!(Mathf.Abs(num5) <= 2f * wrapContent.cellWidth)) ? (secondOffset * (1f - (Mathf.Abs(num5) - 2f * wrapContent.cellWidth) / wrapContent.cellWidth)) : (firstOFfset + (secondOffset - firstOFfset) * ((Mathf.Abs(num5) - wrapContent.cellWidth) / wrapContent.cellWidth))));
			}
			num6 *= num7;
			if (!EnableConfigurePos || scrollViewPanel.GetComponent<UIScrollView>().isDragging || scrollViewPanel.GetComponent<UIScrollView>().currentMomentum.x > 0f)
			{
				shopCarouselElement.SetPos(num3, num6);
			}
			shopCarouselElement.topSeller.gameObject.SetActive(shopCarouselElement.showTS && Mathf.Abs(num2) <= wrapContent.cellWidth / 10f);
			shopCarouselElement.newnew.gameObject.SetActive(shopCarouselElement.showNew && Mathf.Abs(num2) <= wrapContent.cellWidth / 10f);
			shopCarouselElement.quantity.gameObject.SetActive(shopCarouselElement.showQuantity && Mathf.Abs(num2) <= wrapContent.cellWidth / 10f);
		}
		if (_escapeRequested)
		{
			StartCoroutine(BackAfterDelay());
			_escapeRequested = false;
		}
	}

	private void HandleEscape()
	{
		if ((BankController.Instance != null && BankController.Instance.InterfaceEnabled) || (ProfileController.Instance != null && ProfileController.Instance.InterfaceEnabled) || (FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled))
		{
			return;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShopCompleted)
		{
			if (Application.isEditor)
			{
				Debug.Log("Ignoring [Escape] since Tutorial is not completed.");
			}
		}
		else if (InTrainingAfterNoviceArmorRemoved)
		{
			if (Application.isEditor)
			{
				Debug.Log("Ignoring [Escape] since Tutorial after removing Novice Armor is not completed.");
			}
		}
		else if (!GuiActive)
		{
			if (Application.isEditor)
			{
				Debug.Log(GetType().Name + ".LateUpdate():    Ignoring Escape because Shop GUI is not active.");
			}
		}
		else
		{
			_escapeRequested = true;
		}
	}

	public void SetGearCatEnabled(bool e)
	{
		inGame = e;
	}

	private IEnumerator DisableStub()
	{
		for (int i = 0; i < 3; i++)
		{
			yield return null;
		}
		stub.SetActive(false);
	}

	public void MakeACtiveAfterDelay(string idToSet, CategoryNames cn)
	{
		Light[] array = UnityEngine.Object.FindObjectsOfType<Light>();
		if (array == null)
		{
			array = new Light[0];
		}
		Light[] array2 = array;
		foreach (Light light in array2)
		{
			if (!mylights.Contains(light))
			{
				light.cullingMask &= ~(1 << LayerMask.NameToLayer("NGUIShop"));
			}
		}
		sharedShop.ActiveObject.SetActive(true);
		wrapContent.Reposition();
		if (ExperienceController.sharedController != null && ExpController.Instance != null)
		{
			ExperienceController.sharedController.isShowRanks = true;
			ExpController.Instance.InterfaceEnabled = true;
		}
		UpdatePersHat(_currentHat);
		UpdatePersCape(_currentCape);
		UpdatePersArmor(_currentArmor);
		UpdatePersBoots(_currentBoots);
		UpdatePersMask(_currentMask);
		UpdatePersSkin(SkinsController.currentSkinNameForPers);
		MyCenterOnChild myCenterOnChild = carouselCenter;
		myCenterOnChild.onFinished = (SpringPanel.OnFinished)Delegate.Combine(myCenterOnChild.onFinished, new SpringPanel.OnFinished(HandleCarouselCentering));
		PromoActionsManager.ActionsUUpdated += HandleActionsUUpdated;
		PlayWeaponAnimation();
		idleTimerLastTime = Time.realtimeSinceStartup;
		if (idToSet != null)
		{
			sharedShop.ChooseCarouselItem(idToSet, false, true);
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			ForceResetTrainingState();
		}
		sharedShop.carouselCenter.enabled = true;
		AdjustCategoryButtonsForFilterMap();
	}

	private void AdjustCategoryButtonsForFilterMap()
	{
		List<int> list = new List<int>();
		if (SceneLoader.ActiveSceneName.Equals("Sniper"))
		{
			List<int> list2 = new List<int>();
			list2.Add(0);
			list2.Add(3);
			list2.Add(5);
			list = list2;
		}
		else if (SceneLoader.ActiveSceneName.Equals("Knife"))
		{
			List<int> list2 = new List<int>();
			list2.Add(0);
			list2.Add(1);
			list2.Add(3);
			list2.Add(5);
			list2.Add(4);
			list = list2;
		}
		else if ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted) && Defs.isHunger)
		{
			List<int> list2 = new List<int>();
			list2.Add(7);
			list = list2;
		}
		for (int i = 0; i < category.buttons.Length; i++)
		{
			category.buttons[i].onButton.GetComponent<BoxCollider>().enabled = !list.Contains(i) && category.buttons[i].onButton.GetComponent<BoxCollider>().enabled;
			category.buttons[i].offButton.GetComponent<BoxCollider>().enabled = !list.Contains(i);
		}
	}

	public IEnumerator MyWaitForSeconds(float tm)
	{
		float startTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}

	private static string TemppOrHighestDPSGunInCategory(int cInt)
	{
		string text = null;
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.FilteredShopLists != null && WeaponManager.sharedManager.FilteredShopLists.Count > cInt)
		{
			List<GameObject> list = WeaponManager.sharedManager.FilteredShopLists[cInt];
			GameObject gameObject = list.Find((GameObject w) => ItemDb.IsTemporaryGun(ItemDb.GetByPrefabName(w.name.Replace("(Clone)", string.Empty)).Tag));
			if (gameObject != null)
			{
				text = ItemDb.GetByPrefabName(gameObject.name.Replace("(Clone)", string.Empty)).Tag;
			}
			if (text == null && list.Count > 0)
			{
				for (int num = list.Count - 1; num >= 0; num--)
				{
					string text2 = ItemDb.GetByPrefabName(list[num].name.Replace("(Clone)", string.Empty)).Tag;
					if (!ItemDb.IsTemporaryGun(text2) && ExpController.Instance != null && list[num].GetComponent<WeaponSounds>().tier <= ExpController.Instance.OurTier)
					{
						text = text2;
						break;
					}
				}
			}
		}
		return text;
	}

	public static string TempGunOrHighestDPSGun(CategoryNames c, out CategoryNames cn)
	{
		cn = c;
		string text = null;
		text = TemppOrHighestDPSGunInCategory((int)c);
		if (text == null && WeaponManager.sharedManager.playerWeapons.Count > 0)
		{
			int num = (WeaponManager.sharedManager.playerWeapons[0] as Weapon).weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1;
			text = TemppOrHighestDPSGunInCategory(num);
			cn = (CategoryNames)num;
		}
		return text;
	}

	private void OnLevelWasLoaded(int level)
	{
		if (GuiActive)
		{
			_storedAmbientLight = RenderSettings.ambientLight;
			_storedFogEnabled = RenderSettings.fog;
			RenderSettings.ambientLight = Defs.AmbientLightColorForShop();
			RenderSettings.fog = false;
		}
	}

	public void SetOtherCamerasEnabled(bool e)
	{
		List<Camera> list = (Camera.allCameras ?? new Camera[0]).ToList();
		List<Camera> collection = ProfileController.Instance.GetComponentsInChildren<Camera>(true).ToList();
		List<Camera> collection2 = BankController.Instance.GetComponentsInChildren<Camera>(true).ToList();
		list.AddRange(collection);
		list.AddRange(collection2);
		foreach (Camera item in list)
		{
			if ((!(ExpController.Instance != null) || !ExpController.Instance.IsRenderedWithCamera(item)) && !item.gameObject.tag.Equals("CamTemp") && !sharedShop.ourCameras.Contains(item))
			{
				item.rect = new Rect(0f, 0f, e ? 1 : 0, e ? 1 : 0);
			}
		}
	}

	private static void SetIconChosen(CategoryNames cn)
	{
		for (int i = 0; i < sharedShop.category.buttons.Length; i++)
		{
			sharedShop.category.buttons[i].SetCheckedImage(i == (int)cn);
			if (i == (int)cn)
			{
				sharedShop.category.buttons[i].onButton.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}

	public void IsInShopFromPromoPanel(bool isFromPromoACtions, string tg)
	{
		_isFromPromoActions = isFromPromoACtions;
		_promoActionsIdClicked = tg;
	}

	public IEnumerator HideAfterDelay()
	{
		yield return null;
	}

	public static void DisableLightProbesRecursively(GameObject w)
	{
		Player_move_c.PerformActionRecurs(w, delegate(Transform t)
		{
			MeshRenderer component = t.GetComponent<MeshRenderer>();
			SkinnedMeshRenderer component2 = t.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				component.useLightProbes = false;
			}
			if (component2 != null)
			{
				component2.useLightProbes = false;
			}
		});
	}

	public void SetWeapon(string tg)
	{
		animationCoroutineRunner.StopAllCoroutines();
		if (WeaponManager.sharedManager == null)
		{
			return;
		}
		if (armorPoint.childCount > 0)
		{
			ArmorRefs component = armorPoint.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
			if (component != null)
			{
				if (component.leftBone != null)
				{
					Vector3 position = component.leftBone.position;
					Quaternion rotation = component.leftBone.rotation;
					component.leftBone.parent = armorPoint.GetChild(0).GetChild(0);
					component.leftBone.position = position;
					component.leftBone.rotation = rotation;
				}
				if (component.rightBone != null)
				{
					Vector3 position2 = component.rightBone.position;
					Quaternion rotation2 = component.rightBone.rotation;
					component.rightBone.parent = armorPoint.GetChild(0).GetChild(0);
					component.rightBone.position = position2;
					component.rightBone.rotation = rotation2;
				}
			}
		}
		List<Transform> list = new List<Transform>();
		foreach (Transform item in body.transform)
		{
			list.Add(item);
		}
		foreach (Transform item2 in list)
		{
			item2.parent = null;
			item2.position = new Vector3(0f, -10000f, 0f);
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		if (tg == null)
		{
			return;
		}
		if (profile != null)
		{
			Resources.UnloadAsset(profile);
			profile = null;
		}
		GameObject gameObject = null;
		UnityEngine.Object[] weaponsInGame = WeaponManager.sharedManager.weaponsInGame;
		for (int i = 0; i < weaponsInGame.Length; i++)
		{
			GameObject gameObject2 = (GameObject)weaponsInGame[i];
			if (ItemDb.GetByPrefabName(gameObject2.name).Tag.Equals(tg))
			{
				gameObject = gameObject2;
				break;
			}
		}
		if (gameObject == null)
		{
			Debug.Log("pref==null");
			return;
		}
		profile = Resources.Load<AnimationClip>("ProfileAnimClips/" + gameObject.name + "_Profile");
		GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject);
		DisableLightProbesRecursively(gameObject3);
		Player_move_c.SetLayerRecursively(gameObject3, LayerMask.NameToLayer("NGUIShop"));
		gameObject3.transform.parent = body.transform;
		weapon = gameObject3;
		weapon.transform.localScale = new Vector3(1f, 1f, 1f);
		weapon.transform.position = body.transform.position;
		weapon.transform.localPosition = Vector3.zero;
		weapon.transform.localRotation = Quaternion.identity;
		WeaponSounds component2 = weapon.GetComponent<WeaponSounds>();
		if (armorPoint.childCount > 0 && component2 != null)
		{
			ArmorRefs component3 = armorPoint.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
			if (component3 != null)
			{
				if (component3.leftBone != null && component2.LeftArmorHand != null)
				{
					component3.leftBone.parent = component2.LeftArmorHand;
					component3.leftBone.localPosition = Vector3.zero;
					component3.leftBone.localRotation = Quaternion.identity;
					component3.leftBone.localScale = new Vector3(1f, 1f, 1f);
				}
				if (component3.rightBone != null && component2.RightArmorHand != null)
				{
					component3.rightBone.parent = component2.RightArmorHand;
					component3.rightBone.localPosition = Vector3.zero;
					component3.rightBone.localRotation = Quaternion.identity;
					component3.rightBone.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}
		PlayWeaponAnimation();
		DisableGunflashes(weapon);
		if (SkinsController.currentSkinForPers != null)
		{
			SetSkinOnPers(SkinsController.currentSkinForPers);
		}
		_assignedWeaponTag = tg;
	}

	internal static void SynchronizeAndroidPurchases(string comment)
	{
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			return;
		}
		Debug.LogFormat("Trying to synchronize purchases to cloud ({0})", comment);
		Action ResetWeaponManager = delegate
		{
			PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
			if (WeaponManager.sharedManager != null)
			{
				int currentWeaponIndex = WeaponManager.sharedManager.CurrentWeaponIndex;
				WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(Application.loadedLevelName) ? Defs.filterMaps[Application.loadedLevelName] : 0);
				WeaponManager.sharedManager.CurrentWeaponIndex = currentWeaponIndex;
			}
			if (GuiActive)
			{
				sharedShop.UpdateIcons();
			}
		};
		switch (Defs.AndroidEdition)
		{
		case Defs.RuntimeAndroidEdition.Amazon:
			PurchasesSynchronizer.Instance.SynchronizeAmazonPurchases();
			ResetWeaponManager();
			break;
		case Defs.RuntimeAndroidEdition.GoogleLite:
			PlayerPrefs.SetInt("PendingGooglePlayGamesSync", 1);
			PurchasesSynchronizer.Instance.AuthenticateAndSynchronize(delegate(bool success)
			{
				Debug.LogFormat("[Rilisoft] ShopNguiController.PurchasesSynchronizer.Callback({0}) >: {1:F3}", success, Time.realtimeSinceStartup);
				try
				{
					Debug.LogFormat("Google purchases syncronized ({0}): {1}", comment, success);
					if (success)
					{
						ResetWeaponManager();
					}
				}
				finally
				{
					Debug.LogFormat("[Rilisoft] ShopNguiController.PurchasesSynchronizer.Callback({0}) <: {1:F3}", success, Time.realtimeSinceStartup);
				}
			}, true);
			break;
		}
	}
}
