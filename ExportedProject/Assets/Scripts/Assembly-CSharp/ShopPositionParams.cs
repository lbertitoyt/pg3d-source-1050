using UnityEngine;

public class ShopPositionParams : MonoBehaviour
{
	public int tier = 10;

	public int league = 1;

	public float scaleShop = 150f;

	public Vector3 positionShop;

	public Vector3 rotationShop;

	public string localizeKey;

	public int League
	{
		get
		{
			return FriendsController.isUseRatingSystem ? (league - 1) : (base.name.Contains("league") ? 100000 : 0);
		}
	}

	public string shopName
	{
		get
		{
			return LocalizationStore.Get(localizeKey);
		}
	}

	public string shopNameNonLocalized
	{
		get
		{
			return LocalizationStore.GetByDefault(localizeKey);
		}
	}
}
