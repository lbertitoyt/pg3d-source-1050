using System.Collections;
using I2.Loc;
using UnityEngine;

public class GiftHUDItem : MonoBehaviour
{
	public bool isInfo;

	public UISprite sprIcon;

	public UITexture textureIcon;

	public UILabel nameGift;

	public GameObject parentForSkin;

	public BoxCollider colliderForDrag;

	public UILabel lbInfoGift;

	private Transform skinModelTransform;

	[SerializeField]
	[ReadOnly]
	private string nameAndCountGift = string.Empty;

	private Vector3 offsetSkin = new Vector3(0f, -44.12f, 0f);

	private Vector3 scaleSkin = new Vector3(45f, 45f, 45f);

	private bool endAnim;

	[SerializeField]
	private SlotInfo _data;

	private void OnEnable()
	{
		if (colliderForDrag == null)
		{
			colliderForDrag = GetComponent<BoxCollider>();
		}
		if (!isInfo)
		{
			StartCoroutine(ActiveSkinAfterWait());
		}
	}

	public void SetInfoButton(SlotInfo curInfo)
	{
		_data = curInfo;
		if (_data == null)
		{
			Debug.LogError("SetInfoButton");
			return;
		}
		if ((bool)sprIcon)
		{
			sprIcon.gameObject.SetActive(false);
		}
		if ((bool)textureIcon)
		{
			textureIcon.gameObject.SetActive(false);
		}
		if (skinModelTransform != null)
		{
			Object.Destroy(skinModelTransform.gameObject);
			skinModelTransform = null;
		}
		string text = string.Empty;
		if (_data.CountGift > 1)
		{
			text = _data.CountGift + " ";
		}
		switch (_data.category.Type)
		{
		case TypeGiftCategory.Skins:
			nameAndCountGift = SkinsController.skinsNamesForPers[_data.gift.Id];
			break;
		case TypeGiftCategory.Coins:
			nameAndCountGift = LocalizationStore.Get("Key_0275");
			break;
		case TypeGiftCategory.Gems:
			nameAndCountGift = LocalizationStore.Get("Key_0951");
			break;
		case TypeGiftCategory.Grenades:
		case TypeGiftCategory.Gear:
		case TypeGiftCategory.ArmorAndHat:
		case TypeGiftCategory.Wear:
		case TypeGiftCategory.Masks:
		case TypeGiftCategory.Capes:
		case TypeGiftCategory.Boots:
		case TypeGiftCategory.Hats:
		case TypeGiftCategory.Gun1:
		case TypeGiftCategory.Gun2:
		case TypeGiftCategory.Gun3:
		case TypeGiftCategory.Guns_gray:
		{
			string id = _data.gift.Id;
			ShopNGUIController.CategoryNames? typeShopCat = _data.gift.TypeShopCat;
			nameAndCountGift = RespawnWindowItemToBuy.GetItemName(id, (!typeShopCat.HasValue) ? ShopNGUIController.CategoryNames.ArmorCategory : typeShopCat.Value);
			break;
		}
		case TypeGiftCategory.Editor:
			if (_data.gift.Id == "editor_Cape")
			{
				nameAndCountGift = LocalizationStore.Get("Key_0746");
			}
			else if (_data.gift.Id == "editor_Skin")
			{
				nameAndCountGift = LocalizationStore.Get("Key_0086");
			}
			else
			{
				Debug.LogError(string.Format("[GIFT] unknown gift id: '{0}'", _data.gift.Id));
			}
			break;
		}
		nameAndCountGift = text + nameAndCountGift;
		if ((bool)nameGift)
		{
			nameGift.text = nameAndCountGift;
		}
		if (lbInfoGift != null)
		{
			if (!string.IsNullOrEmpty(_data.gift.KeyTranslateInfo))
			{
				lbInfoGift.text = ScriptLocalization.Get(_data.gift.KeyTranslateInfo);
				lbInfoGift.gameObject.SetActive(true);
			}
			else if (!string.IsNullOrEmpty(_data.category.KeyTranslateInfoCommon))
			{
				lbInfoGift.text = ScriptLocalization.Get(_data.category.KeyTranslateInfoCommon);
				lbInfoGift.gameObject.SetActive(true);
			}
			else
			{
				lbInfoGift.gameObject.SetActive(false);
			}
		}
		switch (_data.category.Type)
		{
		case TypeGiftCategory.Skins:
			if ((bool)parentForSkin)
			{
				if (!isInfo)
				{
					parentForSkin.layer = LayerMask.NameToLayer("FriendsWindowGUI");
				}
				skinModelTransform = SkinsController.SkinModel(_data.gift.Id, 1, parentForSkin.transform, offsetSkin, scaleSkin);
			}
			break;
		case TypeGiftCategory.Event_content:
			break;
		default:
			if (textureIcon != null)
			{
				textureIcon.mainTexture = GiftBannerWindow.instance.GetTextureForSlot(_data);
				textureIcon.gameObject.SetActive(true);
			}
			break;
		}
	}

	private IEnumerator ActiveSkinAfterWait()
	{
		while (skinModelTransform == null)
		{
			yield return null;
		}
		skinModelTransform.gameObject.SetActive(false);
		while (!(GiftBannerWindow.instance != null) || !GiftBannerWindow.instance.bannerObj.activeSelf)
		{
			yield return null;
		}
		yield return null;
		skinModelTransform.gameObject.SetActive(true);
	}

	public void InCenter(bool anim = false, int countBut = 1)
	{
		UIScrollView componentInParent = GetComponentInParent<UIScrollView>();
		if (componentInParent == null)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3[] worldCorners = componentInParent.panel.worldCorners;
		Vector3 position = (worldCorners[2] + worldCorners[0]) * 0.5f;
		if (!(transform != null) || !(componentInParent != null) || !(componentInParent.panel != null))
		{
			return;
		}
		Transform cachedTransform = componentInParent.panel.cachedTransform;
		GameObject gameObject = transform.gameObject;
		Vector3 vector = cachedTransform.InverseTransformPoint(transform.position);
		Vector3 vector2 = cachedTransform.InverseTransformPoint(position);
		Vector3 vector3 = vector - vector2;
		if (!componentInParent.canMoveHorizontally)
		{
			vector3.x = 0f;
		}
		if (!componentInParent.canMoveVertically)
		{
			vector3.y = 0f;
		}
		vector3.z = 0f;
		if (anim)
		{
			Vector3 offset = cachedTransform.localPosition - vector3;
			StartCoroutine(Crt_Anim_InCenter(componentInParent.panel.cachedGameObject, offset, countBut * 130));
			return;
		}
		Vector3 vector4 = Vector3.zero;
		if (componentInParent.transform.localPosition.Equals(cachedTransform.localPosition - vector3))
		{
			vector4 = new Vector3(1f, 0f, 0f);
		}
		SpringPanel.Begin(componentInParent.gameObject, cachedTransform.localPosition - vector3 + vector4, 10f);
	}

	private void FastCenter(UIScrollView scroll, Vector3 needPos)
	{
		float deltaTime = RealTime.deltaTime;
		Vector3 localPosition = scroll.transform.localPosition;
		scroll.transform.localPosition = needPos;
		Vector3 vector = needPos - localPosition;
		Vector2 clipOffset = scroll.panel.clipOffset;
		clipOffset.x -= vector.x;
		clipOffset.y -= vector.y;
		scroll.panel.clipOffset = clipOffset;
	}

	private IEnumerator Crt_Anim_InCenter(GameObject obj, Vector3 offset, float width)
	{
		StartCoroutine(Crt_TimerAnim());
		float speedAnim = 0f;
		Vector3 animOffset = new Vector3(width * 5f, 0f, 0f) + offset;
		while (!endAnim)
		{
			if (speedAnim < 1f)
			{
				speedAnim += 0.05f;
			}
			SpringPanel.Begin(obj, animOffset, speedAnim);
			yield return new WaitForEndOfFrame();
		}
		SpringPanel.Begin(obj, animOffset, 1f);
	}

	private IEnumerator Crt_TimerAnim()
	{
		endAnim = false;
		yield return new WaitForSeconds(1.5f);
		endAnim = true;
	}
}
