using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PersConfigurator : MonoBehaviour
{
	public static PersConfigurator currentConfigurator;

	public Transform armorPoint;

	public Transform boots;

	public Transform cape;

	public Transform hat;

	public Transform maskPoint;

	public GameObject body;

	public GameObject gun;

	private GameObject weapon;

	private NickLabelController _label;

	private GameObject shadow;

	private AnimationClip profile;

	private void Awake()
	{
		currentConfigurator = this;
	}

	private IEnumerator Start()
	{
		WeaponManager weaponManager = WeaponManager.sharedManager;
		int maxCost = 0;
		GameObject pref = null;
		List<Weapon> boughtWeapons = new List<Weapon>();
		foreach (Weapon pw2 in weaponManager.allAvailablePlayerWeapons)
		{
			if (WeaponManager.tagToStoreIDMapping.ContainsKey(ItemDb.GetByPrefabName(pw2.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag))
			{
				boughtWeapons.Add(pw2);
			}
		}
		if (boughtWeapons.Count == 0)
		{
			foreach (Weapon pw in weaponManager.allAvailablePlayerWeapons)
			{
				if (pw.weaponPrefab.name.Replace("(Clone)", string.Empty) == WeaponManager.PistolWN)
				{
					pref = pw.weaponPrefab;
					break;
				}
			}
		}
		else
		{
			pref = boughtWeapons[Random.Range(0, boughtWeapons.Count)].weaponPrefab;
		}
		if (pref == null)
		{
			Debug.LogWarning("pref == null");
		}
		else
		{
			Debug.Log("ProfileAnims/" + pref.name + "_Profile");
			profile = Resources.Load<AnimationClip>("ProfileAnimClips/" + pref.name + "_Profile");
			GameObject w = Object.Instantiate(pref);
			w.transform.parent = body.transform;
			weapon = w;
			weapon.transform.localPosition = Vector3.zero;
			weapon.transform.localRotation = Quaternion.identity;
			if (profile != null)
			{
				weapon.GetComponent<WeaponSounds>().animationObject.GetComponent<Animation>().AddClip(profile, "Profile");
				weapon.GetComponent<WeaponSounds>().animationObject.GetComponent<Animation>().Play("Profile");
			}
			gun = w.GetComponent<WeaponSounds>().bonusPrefab;
		}
		GameObject[] gunflashes = Object.FindObjectsOfType<GameObject>();
		GameObject[] array = gunflashes;
		foreach (GameObject go in array)
		{
			if (go.name.Equals("GunFlash"))
			{
				go.SetActive(false);
			}
		}
		SetCurrentSkin();
		ShopNGUIController.sharedShop.onEquipSkinAction = delegate
		{
			SetCurrentSkin();
		};
		yield return new WaitForEndOfFrame();
		_AddCapeAndHat();
		ShopNGUIController.sharedShop.wearEquipAction = delegate
		{
			_AddCapeAndHat();
		};
		ShopNGUIController.sharedShop.wearUnequipAction = delegate
		{
			_AddCapeAndHat();
		};
		ShopNGUIController.ShowArmorChanged += HandleShowArmorChanged;
		while (NickLabelStack.sharedStack == null)
		{
			yield return null;
		}
		NickLabelController.currentCamera = Camera.main;
		_label = NickLabelStack.sharedStack.GetNextCurrentLabel();
		_label.StartShow(NickLabelController.TypeNickLabel.PlayerLobby, base.transform);
		MainMenuController.sharedController.persNickLabel = _label;
	}

	private void HandleShowArmorChanged()
	{
		_AddCapeAndHat();
	}

	private void SetCurrentSkin()
	{
		Texture currentSkinForPers = SkinsController.currentSkinForPers;
		if (!(currentSkinForPers != null))
		{
			return;
		}
		currentSkinForPers.filterMode = FilterMode.Point;
		GameObject[] collection = new GameObject[6] { gun, cape.gameObject, hat.gameObject, boots.gameObject, armorPoint.gameObject, maskPoint.gameObject };
		List<GameObject> list = new List<GameObject>(collection);
		if (weapon != null)
		{
			WeaponSounds component = weapon.GetComponent<WeaponSounds>();
			if (component.LeftArmorHand != null)
			{
				list.Add(component.LeftArmorHand.gameObject);
			}
			if (component.RightArmorHand != null)
			{
				list.Add(component.RightArmorHand.gameObject);
			}
			if (component.grenatePoint != null)
			{
				list.Add(component.grenatePoint.gameObject);
			}
			List<GameObject> listWeaponAnimEffects = component.GetListWeaponAnimEffects();
			if (listWeaponAnimEffects != null)
			{
				list.AddRange(listWeaponAnimEffects);
			}
		}
		Player_move_c.SetTextureRecursivelyFrom(base.gameObject, currentSkinForPers, list.ToArray());
	}

	public void _AddCapeAndHat()
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < cape.childCount; i++)
		{
			list.Add(cape.GetChild(i));
		}
		foreach (Transform item in list)
		{
			Object.Destroy(item.gameObject);
		}
		string @string = Storager.getString(Defs.CapeEquppedSN, false);
		if (!@string.Equals(Defs.CapeNoneEqupped))
		{
			GameObject gameObject = Resources.Load(ResPath.Combine(Defs.CapesDir, @string)) as GameObject;
			if (gameObject != null)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject2.transform.parent = cape;
				gameObject2.transform.localPosition = new Vector3(0f, -0.8f, 0f);
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.GetComponent<Animation>().Play("Profile");
			}
			else
			{
				Debug.LogWarning("capePrefab == null");
			}
		}
		list = new List<Transform>();
		for (int j = 0; j < maskPoint.childCount; j++)
		{
			list.Add(maskPoint.GetChild(j));
		}
		foreach (Transform item2 in list)
		{
			Object.Destroy(item2.gameObject);
		}
		string string2 = Storager.getString("MaskEquippedSN", false);
		if (!string2.Equals("MaskNoneEquipped"))
		{
			GameObject gameObject3 = Resources.Load(ResPath.Combine("Masks", string2)) as GameObject;
			if (gameObject3 != null)
			{
				GameObject gameObject4 = Object.Instantiate(gameObject3, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject4.transform.parent = maskPoint;
				gameObject4.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject4.transform.localRotation = Quaternion.identity;
			}
			else
			{
				Debug.LogWarning("maskPrefab == null");
			}
		}
		list = new List<Transform>();
		for (int k = 0; k < hat.childCount; k++)
		{
			list.Add(hat.GetChild(k));
		}
		foreach (Transform item3 in list)
		{
			item3.parent = null;
			Object.Destroy(item3.gameObject);
		}
		string text = Storager.getString(Defs.HatEquppedSN, false);
		string string3 = Storager.getString(Defs.VisualHatArmor, false);
		if (!string.IsNullOrEmpty(string3) && Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(text) >= 0 && Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(text) < Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(string3))
		{
			text = string3;
		}
		if (!text.Equals(Defs.HatNoneEqupped))
		{
			GameObject gameObject5 = Resources.Load(ResPath.Combine(Defs.HatsDir, text)) as GameObject;
			if (gameObject5 != null)
			{
				GameObject gameObject6 = Object.Instantiate(gameObject5, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject6.transform.parent = hat;
				gameObject6.transform.localPosition = Vector3.zero;
				gameObject6.transform.localRotation = Quaternion.identity;
			}
			else
			{
				Debug.LogWarning("hatPrefab == null");
			}
		}
		list = new List<Transform>();
		for (int l = 0; l < boots.childCount; l++)
		{
			boots.GetChild(l).gameObject.SetActive(false);
		}
		string string4 = Storager.getString(Defs.BootsEquppedSN, false);
		if (!string4.Equals(Defs.BootsNoneEqupped))
		{
			foreach (Transform boot in boots)
			{
				if (boot.gameObject.name.Equals(string4))
				{
					boot.gameObject.SetActive(true);
				}
				else
				{
					boot.gameObject.SetActive(false);
				}
			}
		}
		list = new List<Transform>();
		for (int m = 0; m < armorPoint.childCount; m++)
		{
			list.Add(armorPoint.GetChild(m));
		}
		foreach (Transform item4 in list)
		{
			ArmorRefs component = item4.GetChild(0).GetComponent<ArmorRefs>();
			if (component != null)
			{
				if (component.leftBone != null)
				{
					component.leftBone.parent = item4.GetChild(0);
				}
				if (component.rightBone != null)
				{
					component.rightBone.parent = item4.GetChild(0);
				}
				item4.parent = null;
				Object.Destroy(item4.gameObject);
			}
		}
		string text2 = Storager.getString(Defs.ArmorNewEquppedSN, false);
		string string5 = Storager.getString(Defs.VisualArmor, false);
		if (!string.IsNullOrEmpty(string5) && Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(text2) >= 0 && Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(text2) < Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(string5))
		{
			text2 = string5;
		}
		if (!text2.Equals(Defs.ArmorNewNoneEqupped))
		{
			GameObject gameObject7 = Resources.Load("Armor/" + text2) as GameObject;
			if (gameObject7 == null)
			{
				return;
			}
			GameObject gameObject8 = Object.Instantiate(gameObject7);
			ArmorRefs component2 = gameObject8.transform.GetChild(0).GetComponent<ArmorRefs>();
			if (component2 != null)
			{
				if (weapon != null)
				{
					WeaponSounds component3 = weapon.GetComponent<WeaponSounds>();
					if (component3 != null && component2.leftBone != null && component3.LeftArmorHand != null)
					{
						component2.leftBone.parent = component3.LeftArmorHand;
						component2.leftBone.localPosition = Vector3.zero;
						component2.leftBone.localRotation = Quaternion.identity;
						component2.leftBone.localScale = new Vector3(1f, 1f, 1f);
					}
					if (component3 != null && component2.rightBone != null && component3.RightArmorHand != null)
					{
						component2.rightBone.parent = component3.RightArmorHand;
						component2.rightBone.localPosition = Vector3.zero;
						component2.rightBone.localRotation = Quaternion.identity;
						component2.rightBone.localScale = new Vector3(1f, 1f, 1f);
					}
				}
				gameObject8.transform.parent = armorPoint.transform;
				gameObject8.transform.localPosition = Vector3.zero;
				gameObject8.transform.localRotation = Quaternion.identity;
				gameObject8.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		ShopNGUIController.SetPersHatVisible(hat);
		ShopNGUIController.SetPersArmorVisible(armorPoint);
	}

	private void Update()
	{
		if (!(Camera.main != null))
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
		Touch[] touches = Input.touches;
		foreach (Touch touch in touches)
		{
			RaycastHit hitInfo;
			if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out hitInfo, 1000f, -5) && hitInfo.collider.gameObject.name.Equals("MainMenu_Pers"))
			{
				PlayerPrefs.SetInt(Defs.ProfileEnteredFromMenu, 1);
				ConnectSceneNGUIController.GoToProfile();
				break;
			}
		}
	}

	private void OnDestroy()
	{
		if (ShopNGUIController.sharedShop != null)
		{
			ShopNGUIController.sharedShop.onEquipSkinAction = null;
			ShopNGUIController.sharedShop.wearEquipAction = null;
			ShopNGUIController.sharedShop.wearUnequipAction = null;
		}
		if (profile != null)
		{
			Resources.UnloadAsset(profile);
		}
		ShopNGUIController.ShowArmorChanged -= HandleShowArmorChanged;
		currentConfigurator = null;
	}
}
