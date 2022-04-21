using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public class GiftController : MonoBehaviour
{
	public const string KEY_COUNT_GIFT_FOR_NEW_PLAYER = "keyCountGiftNewPlayer";

	public const string KEY_EDITOR_SKIN = "editor_Skin";

	public const string KEY_EDITOR_CAPE = "editor_Cape";

	public const string KEY_COLLECTION_GUNS_GRAY = "guns_gray";

	public const string KEY_COLLECTION_MASK = "equip_Mask";

	public const string KEY_COLLECTION_CAPE = "equip_Cape";

	public const string KEY_COLLECTION_BOOTS = "equip_Boots";

	public const string KEY_COLLECTION_HAT = "equip_Hat";

	private const string KEY_FOR_SAVE_SERVER_TIME = "SaveServerTime";

	private const string KEY_NEWPLAYER_ARMOR_GETTED = "keyIsGetArmorNewPlayer";

	private const string KEY_NEWPLAYER_SKIN_GETTED = "keyIsGetSkinNewPlayer";

	private const float UPDATE_DATA_FROM_SERVER_INTERVAL = 870f;

	private const int TIME_TO_NEXT_GIFT = 14400;

	public static GiftController Instance;

	public SaltedInt CostBuyCanGetGift = new SaltedInt(15461355, 0);

	private bool _canGetGift;

	public float LocalTimer = -1f;

	private int _oldTime = -1;

	[ReadOnly]
	[SerializeField]
	private readonly List<GiftCategory> _categories = new List<GiftCategory>();

	[ReadOnly]
	[SerializeField]
	private readonly List<SlotInfo> _slots = new List<SlotInfo>();

	[ReadOnly]
	[SerializeField]
	private readonly List<GiftNewPlayerInfo> _forNewPlayer = new List<GiftNewPlayerInfo>();

	private bool _isLoadingDataActive;

	private int _countGiftForNewPlayer;

	private bool _activeGift;

	private bool _alreadyGenerateSlot;

	private static Dictionary<int, List<ItemRecord>> _grayCategoryWeapons;

	public List<SlotInfo> Slots
	{
		get
		{
			return _slots;
		}
	}

	public bool CanGetGift
	{
		get
		{
			return _canGetGift && ActiveGift;
		}
		set
		{
			_canGetGift = value;
		}
	}

	public bool ActiveGift
	{
		get
		{
			return _activeGift && DataIsLoaded && FriendsController.ServerTime >= 0;
		}
	}

	public bool DataIsLoaded
	{
		get
		{
			if (_slots == null)
			{
				return false;
			}
			if (_slots.Count == 0)
			{
				return false;
			}
			return true;
		}
	}

	public static Dictionary<int, List<ItemRecord>> GrayCategoryWeapons
	{
		get
		{
			if (_grayCategoryWeapons == null)
			{
				_grayCategoryWeapons = new Dictionary<int, List<ItemRecord>>();
				_grayCategoryWeapons.Add(0, new List<ItemRecord>
				{
					ItemDb.GetByPrefabName("Weapon10"),
					ItemDb.GetByPrefabName("Weapon44"),
					ItemDb.GetByPrefabName("Weapon79")
				});
				_grayCategoryWeapons.Add(1, new List<ItemRecord>
				{
					ItemDb.GetByPrefabName("Weapon278"),
					ItemDb.GetByPrefabName("Weapon336"),
					ItemDb.GetByPrefabName("Weapon65"),
					ItemDb.GetByPrefabName("Weapon286")
				});
				_grayCategoryWeapons.Add(2, new List<ItemRecord>
				{
					ItemDb.GetByPrefabName("Weapon252"),
					ItemDb.GetByPrefabName("Weapon258"),
					ItemDb.GetByPrefabName("Weapon48"),
					ItemDb.GetByPrefabName("Weapon253")
				});
				_grayCategoryWeapons.Add(3, new List<ItemRecord>
				{
					ItemDb.GetByPrefabName("Weapon257"),
					ItemDb.GetByPrefabName("Weapon262"),
					ItemDb.GetByPrefabName("Weapon251")
				});
				_grayCategoryWeapons.Add(4, new List<ItemRecord>
				{
					ItemDb.GetByPrefabName("Weapon330"),
					ItemDb.GetByPrefabName("Weapon308")
				});
				_grayCategoryWeapons.Add(5, new List<ItemRecord> { ItemDb.GetByPrefabName("Weapon222") });
			}
			return _grayCategoryWeapons;
		}
	}

	public static int CountGetGiftForNewPlayer
	{
		get
		{
			return Storager.getInt("keyCountGiftNewPlayer", false);
		}
		set
		{
			if (value >= 0 && value < CountGetGiftForNewPlayer)
			{
				Storager.setInt("keyCountGiftNewPlayer", value, false);
			}
		}
	}

	public static string UrlForLoadData
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_wp8.json";
			}
			return string.Empty;
		}
	}

	private long LastTimeGetGift
	{
		get
		{
			return Storager.getInt("SaveServerTime", false);
		}
		set
		{
			int val = (int)value;
			Storager.setInt("SaveServerTime", val, false);
		}
	}

	public static event Action OnChangeSlots;

	public static event Action OnTimerEnded;

	public static event Action<string> OnUpdateTimer;

	static GiftController()
	{
		GiftController.OnChangeSlots = delegate
		{
		};
		GiftController.OnTimerEnded = delegate
		{
		};
		GiftController.OnUpdateTimer = delegate
		{
		};
	}

	private void Awake()
	{
		Instance = this;
		LocalTimer = -1f;
		_categories.Clear();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Storager.hasKey("SaveServerTime"))
		{
			Storager.setInt("keyCountGiftNewPlayer", 2, false);
		}
		if (!Storager.hasKey("keyCountGiftNewPlayer"))
		{
			Storager.setInt("keyCountGiftNewPlayer", 0, false);
		}
		if (!Storager.hasKey("keyIsGetArmorNewPlayer"))
		{
			Storager.setInt("keyIsGetArmorNewPlayer", 0, false);
		}
		if (!Storager.hasKey("keyIsGetSkinNewPlayer"))
		{
			Storager.setInt("keyIsGetSkinNewPlayer", 0, false);
		}
		Storager.getInt("keyCountGiftNewPlayer", false);
		StartCoroutine(GetDataFromServerLoop());
		FriendsController.ServerTimeUpdated += OnUpdateTimeFromServer;
	}

	private void OnDestroy()
	{
		FriendsController.ServerTimeUpdated -= OnUpdateTimeFromServer;
		Instance = null;
	}

	private void Update()
	{
		if (LocalTimer > 0f)
		{
			LocalTimer -= Time.deltaTime;
			if (LocalTimer < 0f)
			{
				LocalTimer = 0f;
			}
			CanGetGift = false;
			if (_oldTime != (int)LocalTimer)
			{
				_oldTime = (int)LocalTimer;
				if (GiftController.OnUpdateTimer != null)
				{
					GiftController.OnUpdateTimer(GetStringTimer());
				}
			}
		}
		else if (!CanGetGift && (int)LocalTimer == 0)
		{
			LocalTimer = -1f;
			CanGetGift = true;
			if (GiftController.OnUpdateTimer != null)
			{
				GiftController.OnUpdateTimer(GetStringTimer());
			}
			if (GiftController.OnTimerEnded != null)
			{
				GiftController.OnTimerEnded();
			}
		}
	}

	public void SetTimer(int val)
	{
		if (val > 14400)
		{
			val = 14400;
		}
		if (val != 0)
		{
			long num2 = (LastTimeGetGift = FriendsController.ServerTime - (14400 - val));
		}
		else
		{
			LastTimeGetGift = FriendsController.ServerTime - 14400 + 1;
		}
		OnUpdateTimeFromServer();
	}

	private TypeGiftCategory ParseToEnum(string typeCat)
	{
		TypeGiftCategory? typeGiftCategory = typeCat.ToEnum<TypeGiftCategory>();
		return typeGiftCategory.HasValue ? typeGiftCategory.Value : TypeGiftCategory.none;
	}

	public void CheckGifts()
	{
		if (_activeGift)
		{
			if (_categories != null && _categories.Count > 0)
			{
				StartCoroutine(CheckAvailableGifts());
			}
			return;
		}
		_categories.Clear();
		_slots.Clear();
		if (GiftController.OnChangeSlots != null)
		{
			GiftController.OnChangeSlots();
		}
	}

	public void CreateSlots()
	{
		if (_alreadyGenerateSlot || !_activeGift)
		{
			return;
		}
		_alreadyGenerateSlot = true;
		_slots.Clear();
		foreach (GiftCategory category in _categories)
		{
			category.CheckGifts();
			if (category.AvaliableGiftsCount < 1)
			{
				continue;
			}
			SlotInfo slotInfo = new SlotInfo();
			slotInfo.category = category;
			slotInfo.gift = category.GetRandomGift();
			if (slotInfo.gift != null && !string.IsNullOrEmpty(slotInfo.gift.Id))
			{
				slotInfo.percentGetSlot = category.PercentChance;
				slotInfo.positionInScroll = category.ScrollPosition;
				slotInfo.isActiveEvent = false;
				if (CountGetGiftForNewPlayer > 0)
				{
					SetPerGetGiftForNewPlayer(slotInfo);
				}
				_slots.Add(slotInfo);
			}
		}
		_slots.Sort(delegate(SlotInfo left, SlotInfo right)
		{
			if (left == null && right == null)
			{
				return 0;
			}
			if (left == null)
			{
				return -1;
			}
			return (right == null) ? 1 : left.positionInScroll.CompareTo(right.positionInScroll);
		});
		if (GiftController.OnChangeSlots != null)
		{
			GiftController.OnChangeSlots();
		}
		OnUpdateTimeFromServer();
	}

	private IEnumerator WaitDrop(GiftCategory cat, string id, bool isContains = false)
	{
		bool lk = true;
		int iter = 0;
		GiftInfo gift2 = null;
		while (lk)
		{
			iter++;
			gift2 = cat.GetRandomGift();
			if ((!isContains) ? (gift2.Id == id) : gift2.Id.Contains(id))
			{
				lk = false;
				Debug.Log(string.Format("[TTT] found '{0}' iterations count: {1}", gift2.Id, iter));
			}
			if (iter > 100)
			{
				Debug.Log("[TTT] stop waiting");
				lk = false;
			}
			yield return null;
		}
	}

	public GiftNewPlayerInfo GetInfoNewPlayer(TypeGiftCategory needCat)
	{
		return _forNewPlayer.Find((GiftNewPlayerInfo val) => val.TypeCategory == needCat);
	}

	private void SetPerGetGiftForNewPlayer(SlotInfo curSlot)
	{
		float percentGetSlot = 0f;
		int value = curSlot.gift.Count.Value;
		curSlot.isActiveEvent = true;
		GiftNewPlayerInfo infoNewPlayer = GetInfoNewPlayer(curSlot.category.Type);
		if (infoNewPlayer != null)
		{
			value = infoNewPlayer.Count.Value;
			if (curSlot.category.Type == TypeGiftCategory.ArmorAndHat && Storager.getInt("keyIsGetArmorNewPlayer", false) == 0)
			{
				percentGetSlot = infoNewPlayer.Percent;
			}
			if (curSlot.category.Type == TypeGiftCategory.Skins && Storager.getInt("keyIsGetSkinNewPlayer", false) == 0)
			{
				percentGetSlot = infoNewPlayer.Percent;
			}
			if (curSlot.category.Type == TypeGiftCategory.Coins)
			{
				percentGetSlot = infoNewPlayer.Percent;
			}
			if (curSlot.category.Type == TypeGiftCategory.Gems)
			{
				percentGetSlot = infoNewPlayer.Percent;
			}
		}
		curSlot.percentGetSlot = percentGetSlot;
		curSlot.CountGift = value;
	}

	public void UpdateSlot(SlotInfo curSlot)
	{
		curSlot.category.CheckGifts();
		curSlot.gift = curSlot.category.GetRandomGift();
		if (curSlot.gift == null)
		{
			_slots.Remove(curSlot);
		}
		else
		{
			curSlot.percentGetSlot = curSlot.category.PercentChance;
			curSlot.positionInScroll = curSlot.category.ScrollPosition;
		}
		SlotInfo slot;
		foreach (SlotInfo slot2 in _slots)
		{
			slot = slot2;
			GiftCategory giftCategory = _categories.FirstOrDefault((GiftCategory c) => c == slot.category);
			if (CountGetGiftForNewPlayer > 0)
			{
				SetPerGetGiftForNewPlayer(slot);
			}
			else
			{
				slot.percentGetSlot = slot.category.PercentChance;
			}
		}
	}

	public void ReCreateSlots()
	{
		_alreadyGenerateSlot = false;
		CheckGifts();
	}

	public SlotInfo GetRandomSlot()
	{
		return null;
	}

	private IEnumerator GetDataFromServerLoop()
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.None)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(DownloadDataFormServer());
			yield return new WaitForSeconds(870f);
		}
	}

	private IEnumerator DownloadDataFormServer()
	{
		if (_isLoadingDataActive)
		{
			yield break;
		}
		_isLoadingDataActive = true;
		string urlDataAddress = UrlForLoadData;
		WWW downloadData = null;
		int iter = 3;
		while (iter > 0)
		{
			downloadData = Tools.CreateWwwIfNotConnected(urlDataAddress);
			if (downloadData == null)
			{
				yield break;
			}
			while (!downloadData.isDone)
			{
				yield return null;
			}
			if (!string.IsNullOrEmpty(downloadData.error))
			{
				yield return new WaitForSeconds(5f);
				iter--;
				continue;
			}
			break;
		}
		if (downloadData == null || !string.IsNullOrEmpty(downloadData.error))
		{
			if (Defs.IsDeveloperBuild && downloadData != null)
			{
				Debug.LogWarningFormat("Request to {0} failed: {1}", urlDataAddress, downloadData.error);
			}
			_isLoadingDataActive = false;
			yield break;
		}
		string responseText = URLs.Sanitize(downloadData);
		Dictionary<string, object> allData = Json.Deserialize(responseText) as Dictionary<string, object>;
		if (allData == null)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogError("Bad response: " + responseText);
			}
			_isLoadingDataActive = false;
			yield break;
		}
		if (allData.ContainsKey("isActive"))
		{
			_activeGift = Convert.ToBoolean(allData["isActive"], CultureInfo.InvariantCulture);
			if (!_activeGift)
			{
				_isLoadingDataActive = false;
				OnDataLoaded();
				yield break;
			}
		}
		if (allData.ContainsKey("price"))
		{
			CostBuyCanGetGift.Value = Convert.ToInt32(allData["price"], CultureInfo.InvariantCulture);
		}
		_forNewPlayer.Clear();
		if (allData.ContainsKey("newPlayerEvent"))
		{
			List<object> listAllParametrNewPlayer = allData["newPlayerEvent"] as List<object>;
			if (listAllParametrNewPlayer != null)
			{
				for (int iTG = 0; iTG < listAllParametrNewPlayer.Count; iTG++)
				{
					Dictionary<string, object> curParametr = listAllParametrNewPlayer[iTG] as Dictionary<string, object>;
					GiftNewPlayerInfo curAddInfo = new GiftNewPlayerInfo();
					if (curParametr.ContainsKey("typeCategory"))
					{
						curAddInfo.TypeCategory = ParseToEnum(curParametr["typeCategory"].ToString());
						if (curParametr.ContainsKey("count"))
						{
							curAddInfo.Count.Value = int.Parse(curParametr["count"].ToString());
						}
						if (curParametr.ContainsKey("percent"))
						{
							object curPercentObject = curParametr["percent"];
							curAddInfo.Percent = (float)Convert.ToDouble(curPercentObject, CultureInfo.InvariantCulture);
						}
						_forNewPlayer.Add(curAddInfo);
					}
				}
			}
		}
		_categories.Clear();
		if (allData.ContainsKey("categories"))
		{
			List<object> listCategories = allData["categories"] as List<object>;
			if (listCategories != null)
			{
				for (int iC = 0; iC < listCategories.Count; iC++)
				{
					Dictionary<string, object> infoCategory = listCategories[iC] as Dictionary<string, object>;
					if (infoCategory == null)
					{
						continue;
					}
					GiftCategory newCategory = new GiftCategory();
					if (!infoCategory.ContainsKey("typeCategory"))
					{
						continue;
					}
					newCategory.Type = ParseToEnum(infoCategory["typeCategory"].ToString());
					if (infoCategory.ContainsKey("posInScroll"))
					{
						newCategory.ScrollPosition = int.Parse(infoCategory["posInScroll"].ToString());
					}
					if (!infoCategory.ContainsKey("gifts"))
					{
						continue;
					}
					if (infoCategory.ContainsKey("keyTransInfo"))
					{
						newCategory.KeyTranslateInfoCommon = infoCategory["keyTransInfo"].ToString();
					}
					List<object> gifts = infoCategory["gifts"] as List<object>;
					if (gifts != null)
					{
						for (int iG = 0; iG < gifts.Count; iG++)
						{
							Dictionary<string, object> infoGift = gifts[iG] as Dictionary<string, object>;
							if (infoGift == null)
							{
								continue;
							}
							GiftInfo newGiftInfo = new GiftInfo();
							switch (newCategory.Type)
							{
							case TypeGiftCategory.Coins:
								newGiftInfo.Id = "Coins";
								break;
							case TypeGiftCategory.Gems:
								newGiftInfo.Id = "Gems";
								break;
							default:
								if (infoGift.ContainsKey("idGift"))
								{
									newGiftInfo.Id = infoGift["idGift"].ToString();
								}
								break;
							}
							if (infoGift.ContainsKey("count"))
							{
								newGiftInfo.Count.Value = int.Parse(infoGift["count"].ToString());
							}
							if (infoGift.ContainsKey("percent"))
							{
								object percentObject = infoGift["percent"];
								newGiftInfo.PercentAddInSlot = (float)Convert.ToDouble(percentObject, CultureInfo.InvariantCulture);
							}
							if (infoGift.ContainsKey("keyTransInfo"))
							{
								newGiftInfo.KeyTranslateInfo = infoGift["keyTransInfo"].ToString();
							}
							if (newGiftInfo.Count.Value == 0)
							{
								newGiftInfo.Count.Value = 1;
							}
							newCategory.AddGift(newGiftInfo);
						}
					}
					if (newCategory.AnyGifts)
					{
						_categories.Add(newCategory);
					}
				}
			}
		}
		OnDataLoaded();
		_isLoadingDataActive = false;
	}

	private void OnDataLoaded()
	{
		CheckGifts();
	}

	public SlotInfo GetGift()
	{
		float num = 0f;
		for (int i = 0; i < _slots.Count; i++)
		{
			num += _slots[i].percentGetSlot;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		SlotInfo slotInfo = null;
		for (int j = 0; j < _slots.Count; j++)
		{
			SlotInfo slotInfo2 = _slots[j];
			num3 += slotInfo2.percentGetSlot;
			if (num2 <= num3)
			{
				slotInfo = slotInfo2;
				slotInfo.numInScroll = j;
				break;
			}
		}
		if (slotInfo != null)
		{
			CountGetGiftForNewPlayer--;
			GiveProductForSlot(slotInfo);
		}
		return slotInfo;
	}

	public void CheckAvaliableSlots()
	{
		bool flag = false;
		for (int i = 0; i < _slots.Count; i++)
		{
			SlotInfo slotInfo = _slots[i];
			if (slotInfo.CheckAvaliableGift())
			{
				flag = true;
			}
			if (slotInfo.gift == null)
			{
				_slots.RemoveAt(i);
				i--;
			}
		}
		if (flag && GiftController.OnChangeSlots != null)
		{
			GiftController.OnChangeSlots();
		}
	}

	public void GiveProductForSlot(SlotInfo curSlot)
	{
		if (curSlot == null)
		{
			return;
		}
		switch (curSlot.category.Type)
		{
		case TypeGiftCategory.Coins:
			BankController.AddCoins(curSlot.CountGift, false);
			StartCoroutine(BankController.WaitForIndicationGems(false));
			break;
		case TypeGiftCategory.Gems:
			BankController.AddGems(curSlot.CountGift, false);
			StartCoroutine(BankController.WaitForIndicationGems(true));
			break;
		case TypeGiftCategory.Skins:
			Storager.setInt("keyIsGetSkinNewPlayer", 1, false);
			ShopNGUIController.ProvideShopItemOnStarterPackBoguht(ShopNGUIController.CategoryNames.SkinsCategory, curSlot.gift.Id, 1, false, 0, null, null, false, true, false);
			break;
		case TypeGiftCategory.Gear:
		{
			int int2 = Storager.getInt(curSlot.gift.Id, false);
			Storager.setInt(curSlot.gift.Id, int2 + curSlot.gift.Count.Value, false);
			break;
		}
		case TypeGiftCategory.Grenades:
		{
			int @int = Storager.getInt(curSlot.gift.Id, false);
			Storager.setInt(curSlot.gift.Id, @int + curSlot.gift.Count.Value, false);
			break;
		}
		case TypeGiftCategory.Wear:
			ShopNGUIController.ProvideShopItemOnStarterPackBoguht(curSlot.gift.TypeShopCat.Value, curSlot.gift.Id, 1, false, 0, null, null, true, true, false);
			if (ShopNGUIController.sharedShop != null && ShopNGUIController.sharedShop.wearEquipAction != null)
			{
				ShopNGUIController.sharedShop.wearEquipAction(curSlot.gift.TypeShopCat.Value, string.Empty, string.Empty);
			}
			break;
		case TypeGiftCategory.ArmorAndHat:
			Storager.setInt("keyIsGetArmorNewPlayer", 1, false);
			if (curSlot.gift.TypeShopCat == ShopNGUIController.CategoryNames.ArmorCategory)
			{
				ShopNGUIController.ProvideShopItemOnStarterPackBoguht(ShopNGUIController.CategoryNames.ArmorCategory, curSlot.gift.Id, 1, false, 0, null, null, true, true, false);
				if (ShopNGUIController.sharedShop != null && ShopNGUIController.sharedShop.wearEquipAction != null)
				{
					ShopNGUIController.sharedShop.wearEquipAction(ShopNGUIController.CategoryNames.ArmorCategory, string.Empty, string.Empty);
				}
			}
			break;
		case TypeGiftCategory.Gun1:
		case TypeGiftCategory.Gun2:
		case TypeGiftCategory.Gun3:
		case TypeGiftCategory.Guns_gray:
			if (WeaponManager.IsExclusiveWeapon(curSlot.gift.Id))
			{
				WeaponManager.ProvideExclusiveWeaponByTag(curSlot.gift.Id);
			}
			else
			{
				GiveProduct(curSlot.gift.TypeShopCat.Value, curSlot.gift.Id);
			}
			break;
		case TypeGiftCategory.Editor:
			if (curSlot.gift.Id == "editor_Cape")
			{
				GiveProduct(ShopNGUIController.CategoryNames.CapesCategory, "cape_Custom");
			}
			else if (curSlot.gift.Id == "editor_Skin")
			{
				Storager.setInt(Defs.SkinsMakerInProfileBought, 1, true);
			}
			else
			{
				Debug.LogError(string.Format("[GIFT] unknown editor id: '{0}'", curSlot.gift.Id));
			}
			break;
		case TypeGiftCategory.Masks:
			GiveProduct(ShopNGUIController.CategoryNames.MaskCategory, curSlot.gift.Id);
			break;
		case TypeGiftCategory.Capes:
			GiveProduct(ShopNGUIController.CategoryNames.CapesCategory, curSlot.gift.Id);
			break;
		case TypeGiftCategory.Boots:
			GiveProduct(ShopNGUIController.CategoryNames.BootsCategory, curSlot.gift.Id);
			break;
		case TypeGiftCategory.Hats:
			GiveProduct(ShopNGUIController.CategoryNames.HatsCategory, curSlot.gift.Id);
			break;
		case TypeGiftCategory.Event_content:
			break;
		}
	}

	private void GiveProduct(ShopNGUIController.CategoryNames category, string tag)
	{
		ShopNGUIController.ProvideShopItemOnStarterPackBoguht(category, tag, 1, false, 0, null, null, true, true, false);
		if (ShopNGUIController.sharedShop != null && ShopNGUIController.sharedShop.wearEquipAction != null)
		{
			ShopNGUIController.sharedShop.wearEquipAction(category, string.Empty, string.Empty);
		}
	}

	public static List<string> GetAvailableGrayWeaponsTags()
	{
		int key = ExpController.OurTierForAnyPlace();
		List<ItemRecord> source = GrayCategoryWeapons[key];
		return (from w in source
			where Storager.getInt(w.StorageId, true) == 0
			select w.Tag).ToList();
	}

	private string GetRandomGrayWeapon()
	{
		List<string> availableGrayWeaponsTags = GetAvailableGrayWeaponsTags();
		if (!availableGrayWeaponsTags.Any())
		{
			return string.Empty;
		}
		int index = UnityEngine.Random.Range(0, availableGrayWeaponsTags.Count);
		return availableGrayWeaponsTags[index];
	}

	private IEnumerator CheckAvailableGifts()
	{
		while (!(WeaponManager.sharedManager != null))
		{
			yield return null;
		}
		CreateSlots();
	}

	public void ReSaveLastTimeSever()
	{
		LastTimeGetGift = FriendsController.ServerTime;
		OnUpdateTimeFromServer();
	}

	public string GetStringTimer()
	{
		int num = (int)(LocalTimer / 3600f);
		int num2 = (int)(LocalTimer / 60f) - num * 60;
		int num3 = (int)LocalTimer - num * 3600 - num2 * 60;
		string text = ((num >= 10) ? num.ToString() : ("0" + num));
		string text2 = ((num2 >= 10) ? num2.ToString() : ("0" + num2));
		string text3 = ((num3 >= 10) ? num3.ToString() : ("0" + num3));
		return text + ":" + text2 + ":" + text3;
	}

	private void OnUpdateTimeFromServer()
	{
		if (_slots.Count == 0)
		{
			StartCoroutine(DownloadDataFormServer());
		}
		else
		{
			if (FriendsController.ServerTime < 0)
			{
				return;
			}
			LocalTimer = -1f;
			CanGetGift = false;
			if (!Storager.hasKey("SaveServerTime"))
			{
				LastTimeGetGift = FriendsController.ServerTime - 14400 + 1;
			}
			int num = (int)(FriendsController.ServerTime - LastTimeGetGift);
			if (num >= 14400)
			{
				CanGetGift = true;
				if (GiftController.OnTimerEnded != null)
				{
					GiftController.OnTimerEnded();
				}
			}
			else
			{
				CanGetGift = false;
				LocalTimer = 14400 - num;
			}
		}
	}

	public void TryGetData()
	{
		if (!DataIsLoaded)
		{
			StartCoroutine(DownloadDataFormServer());
		}
	}
}
