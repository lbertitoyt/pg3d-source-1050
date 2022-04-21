using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponSounds : MonoBehaviour
{
	public enum Effects
	{
		Automatic,
		SingleShot,
		Rockets,
		Mortar,
		Laser,
		Shotgun,
		Chainsaw,
		Flamethrower,
		ElectroThrower,
		WallBreak,
		AreaDamage,
		Zoom,
		ThroughEnemies,
		Detonation,
		GuidedAmmunition,
		Ricochet,
		SeveralMissiles,
		Silent,
		ForSandbox,
		SlowTheTarget
	}

	public enum TypeTracer
	{
		none = -1,
		standart,
		red,
		for252,
		turquoise,
		green,
		violet
	}

	public enum TypeDead
	{
		angel,
		explosion,
		energyBlue,
		energyRed,
		energyPink,
		energyCyan,
		energyLight,
		energyGreen,
		energyOrange,
		energyWhite,
		like
	}

	public const string RememberedTierWhereGetGunKey = "RememberedTierWhenObtainGun_";

	public WeaponManager.WeaponTypeForLow typeForLow;

	public static Dictionary<Effects, KeyValuePair<string, string>> keysAndSpritesForEffects = new Dictionary<Effects, KeyValuePair<string, string>>
	{
		{
			Effects.Automatic,
			new KeyValuePair<string, string>("shop_stats_auto", "Key_1391")
		},
		{
			Effects.SingleShot,
			new KeyValuePair<string, string>("shop_stats_sngl", "Key_1392")
		},
		{
			Effects.Rockets,
			new KeyValuePair<string, string>("shop_stats_rkt", "Key_1394")
		},
		{
			Effects.Mortar,
			new KeyValuePair<string, string>("shop_stats_grav", "Key_1396")
		},
		{
			Effects.Laser,
			new KeyValuePair<string, string>("shop_stats_lsr", "Key_1393")
		},
		{
			Effects.Shotgun,
			new KeyValuePair<string, string>("shop_stats_shtgn", "Key_1390")
		},
		{
			Effects.Chainsaw,
			new KeyValuePair<string, string>("shop_stats_chain", "Key_1383")
		},
		{
			Effects.Flamethrower,
			new KeyValuePair<string, string>("shop_stats_fire", "Key_1387")
		},
		{
			Effects.ElectroThrower,
			new KeyValuePair<string, string>("shop_stats_elctrc", "Key_1395")
		},
		{
			Effects.WallBreak,
			new KeyValuePair<string, string>("shop_stats_no_wall", "Key_0402")
		},
		{
			Effects.AreaDamage,
			new KeyValuePair<string, string>("shop_stats_area_dmg", "Key_0403")
		},
		{
			Effects.Zoom,
			new KeyValuePair<string, string>("shop_stats_zoom", "Key_0404")
		},
		{
			Effects.ThroughEnemies,
			new KeyValuePair<string, string>("shop_stats_mtpl_enms", "Key_1388")
		},
		{
			Effects.Detonation,
			new KeyValuePair<string, string>("shop_stats_det", "Key_1385")
		},
		{
			Effects.GuidedAmmunition,
			new KeyValuePair<string, string>("shop_stats_cntrl", "Key_1384")
		},
		{
			Effects.Ricochet,
			new KeyValuePair<string, string>("shop_stats_refl", "Key_1389")
		},
		{
			Effects.SeveralMissiles,
			new KeyValuePair<string, string>("shop_stats_few", "Key_1386")
		},
		{
			Effects.Silent,
			new KeyValuePair<string, string>("shop_stats_g_slnt", "Key_1397")
		},
		{
			Effects.ForSandbox,
			new KeyValuePair<string, string>("shop_stats_sandbox", "Key_1603")
		},
		{
			Effects.SlowTheTarget,
			new KeyValuePair<string, string>("shop_stats_slow_target", "Key_1759")
		}
	};

	public List<Effects> InShopEffects = new List<Effects>();

	public int zoomShop;

	public bool isSlowdown;

	[Range(0.01f, 10f)]
	public float slowdownCoeff;

	public float slowdownTime;

	public GameObject[] noFillObjects;

	private GameObject BearWeapon;

	private bool bearActive;

	public TypeTracer typeTracer;

	[HideInInspector]
	public InnerWeaponPars _innerPars;

	private BearInnerWeaponPars _bearPars;

	public TypeDead typeDead;

	public Transform gunFlash;

	public Transform[] gunFlashDouble;

	public float lengthForShot;

	private float[] damageByTierRememberedTierWhereGet;

	public float[] damageByTier = new float[ExpController.LevelsForTiers.Length];

	public float[] dpses = new float[6];

	private float[] _dpsesCorrectedByRememberedGun;

	public int tier;

	public int categoryNabor = 1;

	public bool isGrenadeWeapon;

	public int fireRateShop;

	public int CapacityShop;

	public int mobilityShop;

	public int[] filterMap;

	public string alternativeName = WeaponManager.PistolWN;

	public bool isBurstShooting;

	public int countShootInBurst = 3;

	public float delayInBurstShooting = 1f;

	public bool isDaterWeapon;

	public string daterMessage = string.Empty;

	public int ammoInClip = 12;

	public int InitialAmmo = 24;

	public int maxAmmo = 84;

	public int ammoForBonusShotMelee = 10;

	public bool isMelee;

	public bool isRoundMelee;

	public float radiusRoundMelee = 5f;

	public bool isLoopShoot;

	public bool isShotGun;

	public bool isDoubleShot;

	public int countShots = 15;

	public bool isShotMelee;

	public bool isZooming;

	public bool isMagic;

	public bool flamethrower;

	public bool bulletExplode;

	public bool bazooka;

	public int countInSeriaBazooka = 1;

	public float stepTimeInSeriaBazooka = 0.2f;

	public bool railgun;

	public string railName = "Weapon77";

	public bool freezer;

	public int countReflectionRay = 1;

	public bool grenadeLauncher;

	public string bazookaExplosionName = "Weapon75";

	public float bazookaExplosionRadius = 5f;

	public float bazookaExplosionRadiusSelf = 2.5f;

	public float bazookaImpulseRadius = 6f;

	public float impulseForce = 90f;

	public float impulseForceSelf = 133.4f;

	public float fieldOfViewZomm = 75f;

	public float range = 3f;

	public int damage = 50;

	public float speedModifier = 1f;

	public int Probability = 1;

	public Vector2 damageRange = new Vector2(-15f, 15f);

	public Vector3 gunPosition = new Vector3(0.35f, -0.25f, 0.6f);

	public int inAppExtensionModifier = 10;

	public float meleeAngle = 50f;

	public float multiplayerDamage = 1f;

	public float meleeAttackTimeModifier = 0.57f;

	public Vector2 startZone;

	public float tekKoof = 1f;

	public float upKoofFire = 0.5f;

	public float maxKoof = 4f;

	public float downKoofFirst = 0.2f;

	public float downKoof = 0.2f;

	public bool campaignOnly;

	public int rocketNum;

	public int scopeNum;

	public float scaleShop = 150f;

	public Vector3 positionShop;

	public Vector3 rotationShop;

	public string localizeWeaponKey;

	private float animLength;

	private float timeFromFire = 1000f;

	private Player_move_c myPlayerC;

	public bool DPSRememberWhenGet;

	public GameObject BearWeaponObject
	{
		get
		{
			return BearWeapon;
		}
	}

	public float DPS
	{
		get
		{
			if (ExpController.Instance == null)
			{
				return 0f;
			}
			int ourTier = ExpController.Instance.OurTier;
			int num = Math.Max(ourTier, tier);
			if (dpsesCorrectedByRememberedGun.Length <= num)
			{
				return 0f;
			}
			return dpsesCorrectedByRememberedGun[num] * ((!isShotGun) ? ((float)countInSeriaBazooka) : ((float)countShots * WeaponManager.ShotgunShotsCountModif()));
		}
	}

	public GameObject animationObject
	{
		get
		{
			return bearActive ? BearWeapon : ((!(_innerPars != null)) ? null : _innerPars.animationObject);
		}
	}

	public Texture preview
	{
		get
		{
			return (!(_innerPars != null)) ? null : _innerPars.preview;
		}
	}

	public AudioClip shoot
	{
		get
		{
			return (bearActive && _bearPars != null && _bearPars.shoot != null) ? _bearPars.shoot : ((!(_innerPars != null)) ? null : _innerPars.shoot);
		}
	}

	public AudioClip reload
	{
		get
		{
			return (bearActive && _bearPars != null && _bearPars.reload != null) ? _bearPars.reload : ((!(_innerPars != null)) ? null : _innerPars.reload);
		}
	}

	public AudioClip empty
	{
		get
		{
			return (bearActive && _bearPars != null && _bearPars.empty != null) ? _bearPars.empty : ((!(_innerPars != null)) ? null : _innerPars.empty);
		}
	}

	public AudioClip idle
	{
		get
		{
			return (!(_innerPars != null)) ? null : _innerPars.idle;
		}
	}

	public AudioClip zoomIn
	{
		get
		{
			return (!(_innerPars != null)) ? null : _innerPars.zoomIn;
		}
	}

	public AudioClip zoomOut
	{
		get
		{
			return (!(_innerPars != null)) ? null : ((!(_innerPars.zoomOut != null)) ? _innerPars.zoomIn : _innerPars.zoomOut);
		}
	}

	public GameObject bonusPrefab
	{
		get
		{
			return (!(_innerPars != null)) ? null : _innerPars.bonusPrefab;
		}
	}

	public Texture2D aimTextureV
	{
		get
		{
			return (!(_innerPars != null)) ? null : _innerPars.aimTextureV;
		}
	}

	public Texture2D aimTextureH
	{
		get
		{
			return (!(_innerPars != null)) ? null : _innerPars.aimTextureH;
		}
	}

	public Transform LeftArmorHand
	{
		get
		{
			return (!(_innerPars != null)) ? null : _innerPars.LeftArmorHand;
		}
	}

	public Transform RightArmorHand
	{
		get
		{
			return (!(_innerPars != null)) ? null : _innerPars.RightArmorHand;
		}
	}

	public Transform grenatePoint
	{
		get
		{
			return (bearActive && _bearPars != null) ? _bearPars.grenatePoint : ((!(_innerPars != null)) ? null : _innerPars.grenatePoint);
		}
	}

	public float[] DamageByTier
	{
		get
		{
			if (DPSRememberWhenGet)
			{
				if (damageByTierRememberedTierWhereGet == null)
				{
					int @int = Storager.getInt("RememberedTierWhenObtainGun_" + base.gameObject.name.Replace("(Clone)", string.Empty), false);
					damageByTierRememberedTierWhereGet = new float[damageByTier.Length];
					for (int i = 0; i <= @int; i++)
					{
						damageByTierRememberedTierWhereGet[i] = damageByTier[i];
					}
					for (int j = @int + 1; j < damageByTierRememberedTierWhereGet.Length; j++)
					{
						damageByTierRememberedTierWhereGet[j] = damageByTier[@int];
					}
				}
				return damageByTierRememberedTierWhereGet;
			}
			return damageByTier;
		}
	}

	public float[] dpsesCorrectedByRememberedGun
	{
		get
		{
			if (DPSRememberWhenGet)
			{
				if (_dpsesCorrectedByRememberedGun == null)
				{
					int @int = Storager.getInt("RememberedTierWhenObtainGun_" + base.gameObject.name.Replace("(Clone)", string.Empty), false);
					_dpsesCorrectedByRememberedGun = new float[dpses.Length];
					for (int i = 0; i <= @int; i++)
					{
						_dpsesCorrectedByRememberedGun[i] = dpses[i];
					}
					for (int j = @int + 1; j < _dpsesCorrectedByRememberedGun.Length; j++)
					{
						_dpsesCorrectedByRememberedGun[j] = dpses[@int];
					}
				}
				return _dpsesCorrectedByRememberedGun;
			}
			return dpses;
		}
	}

	public int damageShop
	{
		get
		{
			if (DPSRememberWhenGet)
			{
				int @int = Storager.getInt("RememberedTierWhenObtainGun_" + base.gameObject.name.Replace("(Clone)", string.Empty), false);
				return Mathf.RoundToInt(dpses[@int] * ((!isShotGun) ? ((float)((!bazooka) ? 1 : countInSeriaBazooka)) : ((float)countShots * WeaponManager.ShotgunShotsCountModif())));
			}
			return Mathf.RoundToInt(dpses[dpses.Length - 1] * ((!isShotGun) ? ((float)((!bazooka) ? 1 : countInSeriaBazooka)) : ((float)countShots * WeaponManager.ShotgunShotsCountModif())));
		}
	}

	public int MaxAmmoWithEffectApplied
	{
		get
		{
			return (int)((float)maxAmmo * EffectsController.AmmoModForCategory(categoryNabor - 1));
		}
	}

	public int InitialAmmoWithEffectsApplied
	{
		get
		{
			return (int)((float)InitialAmmo * EffectsController.AmmoModForCategory(categoryNabor - 1));
		}
	}

	public string shopName
	{
		get
		{
			return LocalizationStore.Get(localizeWeaponKey);
		}
	}

	public string shopNameNonLocalized
	{
		get
		{
			return LocalizationStore.GetByDefault(localizeWeaponKey);
		}
	}

	public void SetDaterBearHandsAnim(bool set)
	{
		bearActive = set && BearWeapon != null;
		_innerPars.animationObject.SetActive(!bearActive);
		if (BearWeapon != null)
		{
			BearWeapon.SetActive(bearActive);
		}
	}

	private void Awake()
	{
		if (!base.gameObject.name.Contains("Weapon"))
		{
			return;
		}
		GameObject gameObject = Resources.Load<GameObject>(ResPath.Combine(Defs.InnerWeaponsFolder, base.gameObject.name.Replace("(Clone)", string.Empty) + Defs.InnerWeapons_Suffix));
		if (gameObject != null)
		{
			_innerPars = (UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, base.gameObject.transform.position.y, 0f), Quaternion.identity) as GameObject).GetComponent<InnerWeaponPars>();
			if (Defs.isDaterRegim)
			{
				string path = "MechBearWeapons/" + base.gameObject.name.Replace("(Clone)", string.Empty) + "_MechBear";
				UnityEngine.Object @object = Resources.Load(path);
				if (@object != null)
				{
					BearWeapon = (GameObject)UnityEngine.Object.Instantiate(@object, new Vector3(0f, base.gameObject.transform.position.y, 0f), Quaternion.identity);
					_bearPars = BearWeapon.GetComponent<BearInnerWeaponPars>();
					BearWeapon.transform.parent = base.gameObject.transform;
					BearWeapon.SetActive(false);
				}
			}
			_innerPars.gameObject.transform.parent = base.gameObject.transform;
		}
		if (!isMelee)
		{
			gunFlash = ((base.transform.childCount <= 0 || base.transform.GetChild(0).childCount <= 0) ? null : base.transform.GetChild(0).GetChild(0));
		}
	}

	private void OnDestroy()
	{
		if (_innerPars != null)
		{
			UnityEngine.Object.Destroy(_innerPars.gameObject);
		}
	}

	private void Start()
	{
		if (string.IsNullOrEmpty(bazookaExplosionName))
		{
			bazookaExplosionName = base.gameObject.name.Replace("(Clone)", string.Empty);
		}
		if (isDoubleShot)
		{
			if (animationObject != null && animationObject.GetComponent<Animation>()["Shoot1"] != null)
			{
				animLength = animationObject.GetComponent<Animation>()["Shoot1"].length;
			}
		}
		else if (animationObject != null && animationObject.GetComponent<Animation>()["Shoot"] != null)
		{
			animLength = animationObject.GetComponent<Animation>()["Shoot"].length;
		}
	}

	private void Update()
	{
		if (base.transform.parent != null)
		{
			if (myPlayerC == null)
			{
				myPlayerC = base.transform.parent.GetComponent<Player_move_c>();
			}
			if (base.transform.parent != null && myPlayerC != null && !myPlayerC.isMine && myPlayerC.isMulti && animationObject.activeSelf == myPlayerC.isInvisible)
			{
				animationObject.SetActive(!myPlayerC.isInvisible);
			}
		}
		if (timeFromFire < animLength)
		{
			timeFromFire += Time.deltaTime;
			if (tekKoof > 1f)
			{
				tekKoof -= downKoofFirst * Time.deltaTime / animLength;
			}
			if (tekKoof < 1f)
			{
				tekKoof = 1f;
			}
		}
		else
		{
			if (tekKoof > 1f)
			{
				tekKoof -= downKoof * Time.deltaTime / animLength;
			}
			if (tekKoof < 1f)
			{
				tekKoof = 1f;
			}
		}
		CheckPlayDefaultAnimInMulti();
	}

	private void CheckPlayDefaultAnimInMulti()
	{
		if (Defs.isInet && Defs.isMulti)
		{
			Player_move_c component = base.transform.parent.GetComponent<Player_move_c>();
			if (component != null && !component.isMine && !_innerPars.GetComponent<Animation>().isPlaying)
			{
				_innerPars.GetComponent<Animation>().Play("Idle");
			}
		}
	}

	public bool IsAvalibleFromFilter(int filter)
	{
		if (filter == 0)
		{
			return true;
		}
		if (filterMap != null && filterMap.Contains(filter))
		{
			return true;
		}
		return false;
	}

	public void fire()
	{
		timeFromFire = 0f;
		tekKoof += upKoofFire + downKoofFirst;
		if (tekKoof > maxKoof + downKoofFirst)
		{
			tekKoof = maxKoof + downKoofFirst;
		}
	}

	public List<GameObject> GetListWeaponAnimEffects()
	{
		if (_innerPars == null)
		{
			return null;
		}
		WeaponAnimParticleEffects component = _innerPars.GetComponent<WeaponAnimParticleEffects>();
		if (component == null)
		{
			return null;
		}
		return component.GetListAnimEffects();
	}
}
