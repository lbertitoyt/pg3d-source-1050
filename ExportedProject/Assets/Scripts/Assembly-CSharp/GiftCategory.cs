using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

[Serializable]
public class GiftCategory
{
	public TypeGiftCategory Type;

	public int ScrollPosition;

	public string KeyTranslateInfoCommon = string.Empty;

	private readonly List<GiftInfo> _rootGifts = new List<GiftInfo>();

	private List<GiftInfo> _ag;

	private List<string> _availableRandomProducts;

	private List<GiftInfo> _allGifts
	{
		get
		{
			return _ag ?? (_ag = GetAvailableGifts());
		}
		set
		{
			_ag = value;
		}
	}

	public bool AnyGifts
	{
		get
		{
			return _allGifts.Any();
		}
	}

	public float PercentChance
	{
		get
		{
			if (Type == TypeGiftCategory.Guns_gray || Type == TypeGiftCategory.Masks || Type == TypeGiftCategory.Boots || Type == TypeGiftCategory.Capes || Type == TypeGiftCategory.Hats || Type == TypeGiftCategory.ArmorAndHat)
			{
				return _allGifts[0].PercentAddInSlot;
			}
			return _allGifts.Sum((GiftInfo g) => g.PercentAddInSlot);
		}
	}

	private List<GiftInfo> _availableGifts
	{
		get
		{
			return _allGifts.Where((GiftInfo g) => AvailableGift(g.Id, Type)).ToList();
		}
	}

	public int AvaliableGiftsCount
	{
		get
		{
			return _availableGifts.Count;
		}
	}

	private float _availableGiftsPercentSum
	{
		get
		{
			return _availableGifts.Sum((GiftInfo g) => g.PercentAddInSlot);
		}
	}

	public void AddGift(GiftInfo info)
	{
		_rootGifts.Add(info);
	}

	public void CheckGifts()
	{
		_allGifts = GetAvailableGifts();
		foreach (GiftInfo allGift in _allGifts)
		{
			if (Type == TypeGiftCategory.ArmorAndHat)
			{
				allGift.Id = Wear.ArmorOrArmorHatAvailableForBuy(ShopNGUIController.CategoryNames.ArmorCategory);
			}
			if (Type == TypeGiftCategory.Skins && allGift.Id.ToLower().Equals("all"))
			{
				allGift.IsRandomSkin = true;
				allGift.Id = SkinsController.RandomUnboughtSkinId();
			}
		}
	}

	public bool AvailableGift(string idGift, TypeGiftCategory curType)
	{
		if (string.IsNullOrEmpty(idGift))
		{
			return false;
		}
		switch (curType)
		{
		case TypeGiftCategory.Coins:
		case TypeGiftCategory.Gems:
			return true;
		case TypeGiftCategory.Grenades:
			return true;
		case TypeGiftCategory.Gear:
			return true;
		case TypeGiftCategory.Guns_gray:
		{
			if (idGift.IsNullOrEmpty())
			{
				return false;
			}
			ItemRecord itemRecord = GiftController.GrayCategoryWeapons[ExpController.OurTierForAnyPlace()].FirstOrDefault((ItemRecord rec) => rec.Tag == idGift);
			return itemRecord != null && Storager.getInt(itemRecord.StorageId, true) == 0;
		}
		case TypeGiftCategory.Gun1:
		case TypeGiftCategory.Gun2:
		case TypeGiftCategory.Gun3:
			return Storager.getInt(idGift, true) == 0;
		case TypeGiftCategory.Wear:
			return !ItemDb.IsItemInInventory(idGift);
		case TypeGiftCategory.ArmorAndHat:
		{
			string text = Wear.ArmorOrArmorHatAvailableForBuy(ShopNGUIController.CategoryNames.ArmorCategory);
			return idGift == text;
		}
		case TypeGiftCategory.Event_content:
			return true;
		case TypeGiftCategory.Skins:
		{
			bool isForMoneySkin = false;
			return !SkinsController.IsSkinBought(idGift, out isForMoneySkin);
		}
		case TypeGiftCategory.Editor:
			if (idGift.IsNullOrEmpty() || (idGift != "editor_Cape" && idGift != "editor_Skin"))
			{
				return false;
			}
			if (idGift == "editor_Skin" && Storager.getInt(Defs.SkinsMakerInProfileBought, false) > 0)
			{
				return false;
			}
			if (idGift == "editor_Cape" && Storager.getInt("cape_Custom", false) > 0)
			{
				return false;
			}
			return true;
		case TypeGiftCategory.Masks:
		case TypeGiftCategory.Boots:
		case TypeGiftCategory.Hats:
			return !idGift.IsNullOrEmpty() && Storager.getInt(idGift, true) == 0;
		case TypeGiftCategory.Capes:
			if (idGift != "cape_Custom")
			{
				return false;
			}
			return !idGift.IsNullOrEmpty() && Storager.getInt(idGift, true) == 0;
		default:
			return false;
		}
	}

	private List<string> GetAvailableProducts(ShopNGUIController.CategoryNames category, string[] withoutIds = null)
	{
		if (_availableRandomProducts == null)
		{
			_availableRandomProducts = Wear.AllWears(category, 0, true, true);
			if (withoutIds != null)
			{
				_availableRandomProducts = _availableRandomProducts.Where((string w) => !withoutIds.Contains(w)).ToList();
			}
		}
		List<string> list = _availableRandomProducts.ToList();
		foreach (string item in list)
		{
			if (Storager.getInt(item, true) > 0)
			{
				_availableRandomProducts.Remove(item);
			}
		}
		return _availableRandomProducts;
	}

	public GiftInfo GetRandomGift()
	{
		if (_availableGifts == null || _availableGifts.Count == 0)
		{
			return null;
		}
		if (_availableGiftsPercentSum < 0f)
		{
			return null;
		}
		float num = UnityEngine.Random.Range(0f, _availableGiftsPercentSum);
		float num2 = 0f;
		GiftInfo result = null;
		for (int i = 0; i < _availableGifts.Count; i++)
		{
			GiftInfo giftInfo = _availableGifts[i];
			num2 += giftInfo.PercentAddInSlot;
			if (num2 > num)
			{
				result = giftInfo;
				break;
			}
		}
		return result;
	}

	private List<GiftInfo> GetAvailableGifts()
	{
		List<GiftInfo> res = new List<GiftInfo>();
		foreach (GiftInfo root in _rootGifts)
		{
			if (root.Id == "guns_gray")
			{
				List<string> availableGrayWeaponsTags = GiftController.GetAvailableGrayWeaponsTags();
				availableGrayWeaponsTags.ForEach(delegate(string w)
				{
					res.Add(GiftInfo.CreateInfo(root, w));
				});
			}
			else if (root.Id == "equip_Mask")
			{
				List<string> availableProducts = GetAvailableProducts(ShopNGUIController.CategoryNames.MaskCategory);
				availableProducts.ForEach(delegate(string p)
				{
					res.Add(GiftInfo.CreateInfo(root, p));
				});
			}
			else if (root.Id == "equip_Cape")
			{
				List<string> availableProducts2 = GetAvailableProducts(ShopNGUIController.CategoryNames.CapesCategory, new string[1] { "cape_Custom" });
				availableProducts2.ForEach(delegate(string p)
				{
					res.Add(GiftInfo.CreateInfo(root, p));
				});
			}
			else if (root.Id == "equip_Boots")
			{
				List<string> availableProducts3 = GetAvailableProducts(ShopNGUIController.CategoryNames.BootsCategory);
				availableProducts3.ForEach(delegate(string p)
				{
					res.Add(GiftInfo.CreateInfo(root, p));
				});
			}
			else if (root.Id == "equip_Hat")
			{
				List<string> availableProducts4 = GetAvailableProducts(ShopNGUIController.CategoryNames.HatsCategory);
				availableProducts4.ForEach(delegate(string p)
				{
					res.Add(GiftInfo.CreateInfo(root, p));
				});
			}
			else
			{
				res.Add(root);
			}
		}
		return res;
	}
}
