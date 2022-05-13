using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using com.amazon.device.iap.cpt;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Rilisoft;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using RilisoftBot;
using UnityEngine;

public sealed class Player_move_c : MonoBehaviour
{
	public enum TypeBonuses
	{
		Ammo,
		Health,
		Armor,
		Grenade
	}

	public enum TypeKills
	{
		none,
		himself,
		headshot,
		explosion,
		zoomingshot,
		flag,
		grenade,
		grenade_hell,
		turret,
		killTurret,
		mech,
		like
	}

	public struct MessageChat
	{
		public string text;

		public float time;

		public int ID;

		public int command;

		public bool isClanMessage;

		public Texture clanLogo;

		public string clanID;

		public string clanName;

		public NetworkViewID IDLocal;

		public string iconName;
	}

	public struct RayHitsInfo
	{
		public RaycastHit[] hits;

		public bool obstacleFound;

		public float lenRay;

		public Ray rayReflect;
	}

	public delegate void OnMessagesUpdate();

	private const string keyKilledPlayerCharactersCount = "KilledPlayerCharactersCount";

	private const string startShootAnimName = "Shoot_start";

	private const string endShootAnimName = "Shoot_end";

	private const float slowdownCoefConst = 0.75f;

	private int tierForKilledRate;

	private readonly Dictionary<string, int> weKillForKillRate = new Dictionary<string, int>();

	private readonly Dictionary<string, int> weWereKilledForKillRate = new Dictionary<string, int>();

	public TextMesh nickLabel;

	public AudioClip mechActivSound;

	public AudioClip mechBearActivSound;

	public AudioClip invisibleActivSound;

	public AudioClip jetpackActivSound;

	public AudioClip portalSound;

	public PlayerScoreController myScoreController;

	public bool isRocketJump;

	public float armorSynch;

	public bool isReloading;

	public bool _isPlacemarker;

	public GameObject placemarkerMark;

	public GameObject redMark;

	public GameObject blueMark;

	public Player_move_c placemarkerMoveC;

	public ParticleBonuse[] bonusesParticles;

	public GameObject particleBonusesPoint;

	public Transform myTransform;

	public Transform myPlayerTransform;

	public int myPlayerID;

	public NetworkViewID myPlayerIDLocal;

	public SkinName mySkinName;

	public GameObject mechPoint;

	public GameObject mechBody;

	public GameObject mechBearPoint;

	public GameObject mechBearBody;

	public GameObject mechBearHands;

	public Animation mechGunAnimation;

	public Animation mechBodyAnimation;

	public Animation mechBearGunAnimation;

	public Animation mechBearBodyAnimation;

	public WeaponSounds mechWeaponSounds;

	public WeaponSounds mechBearWeaponSounds;

	public ParticleSystem[] flashMech;

	public GameObject fpsPlayerBody;

	public GameObject myCurrentWeapon;

	public WeaponSounds myCurrentWeaponSounds;

	public GameObject mechExplossion;

	public GameObject bearExplosion;

	public AudioSource mechExplossionSound;

	public AudioSource mechBearExplosionSound;

	public AudioClip shootMechClip;

	public AudioClip shootMechBearClip;

	public SkinnedMeshRenderer playerBodyRenderer;

	public SkinnedMeshRenderer mechBodyRenderer;

	public SkinnedMeshRenderer mechHandRenderer;

	public SkinnedMeshRenderer mechGunRenderer;

	public SkinnedMeshRenderer mechBearBodyRenderer;

	public SkinnedMeshRenderer mechBearHandRenderer;

	public SynhRotationWithGameObject mechBearSyncRot;

	public Transform PlayerHeadTransform;

	public Transform MechHeadTransform;

	public CapsuleCollider bodyCollayder;

	public CapsuleCollider headCollayder;

	public Material[] mechGunMaterials;

	public Material[] mechBodyMaterials;

	private int numShootInDoubleShot = 1;

	public bool isMechActive;

	public bool isBearActive;

	public AudioClip flagGetClip;

	public AudioClip flagLostClip;

	public AudioClip flagScoreEnemyClip;

	public AudioClip flagScoreMyCommandClip;

	public float deltaAngle;

	public GameObject playerDeadPrefab;

	public ThirdPersonNetwork1 myPersonNetwork;

	public GameObject grenadePrefab;

	public GameObject likePrefab;

	public GameObject turretPrefab;

	public GameObject turretPoint;

	public GameObject currentTurret;

	public float liveMech;

	public float[] liveMechByTier;

	private GameObject currentGrenade;

	public int currentWeaponBeforeTurret = -1;

	private int currentWeaponBeforeGrenade;

	private float stdFov = 60f;

	private int countMultyFlag;

	private string[] iconShotName = new string[12]
	{
		string.Empty,
		"Chat_Death",
		"Chat_HeadShot",
		"Chat_Explode",
		"Chat_Sniper",
		"Chat_Flag",
		"Chat_grenade",
		"Chat_grenade_hell",
		"Chat_Turret",
		"Chat_Turret_Explode",
		string.Empty,
		"Smile_1_15"
	};

	public bool isImVisible;

	public bool isWeaponSet;

	public NetworkStartTableNGUIController networkStartTableNGUIController;

	public GameObject invisibleParticle;

	public bool isInvisible;

	public bool isBigHead;

	public float maxTimeSetTimerShow = 0.5f;

	private float _koofDamageWeaponFromPotoins;

	public bool isRegenerationLiveZel;

	private float maxTimerRegenerationLiveZel = 5f;

	public bool isRegenerationLiveCape;

	private float maxTimerRegenerationLiveCape = 15f;

	private float timerRegenerationLiveZel;

	private float timerRegenerationLiveCape;

	private float timerRegenerationArmor;

	private Shader[] oldShadersInInvisible;

	private Color[] oldColorInInvisible;

	public bool isCaptureFlag;

	public GameObject myBaza;

	public Camera myCamera;

	public Camera gunCamera;

	public GameObject hatsPoint;

	public GameObject capesPoint;

	public GameObject flagPoint;

	public GameObject bootsPoint;

	public GameObject armorPoint;

	public GameObject arrowToPortalPoint;

	public bool isZooming;

	public AudioClip headShotSound;

	public AudioClip flagCaptureClip;

	public AudioClip flagPointClip;

	public GameObject headShotParticle;

	public GameObject healthParticle;

	public GameObject chatViewer;

	public GUISkin MySkin;

	public GameObject myTable;

	public NetworkStartTable myNetworkStartTable;

	private float[] _byCatDamageModifs = new float[6];

	public int AimTextureWidth = 50;

	public int AimTextureHeight = 50;

	public Transform GunFlash;

	private bool isZachetWin;

	public bool showGUIUnlockFullVersion;

	public float timeHingerGame;

	public int BulletForce = 5000;

	public bool killed;

	public ZombiManager zombiManager;

	public NickLabelController myNickLabelController;

	public visibleObjPhoton visibleObj;

	public string textChat;

	public bool showGUI = true;

	public bool showRanks;

	public string[][] killedSpisok = new string[3][]
	{
		new string[4]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		},
		new string[4]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		},
		new string[4]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		}
	};

	public GUIStyle combatRifleStyle;

	public GUIStyle goldenEagleInappStyle;

	public GUIStyle magicBowInappStyle;

	public GUIStyle spasStyle;

	public GUIStyle axeStyle;

	public GUIStyle famasStyle;

	public GUIStyle glockStyle;

	public GUIStyle chainsawStyle;

	public GUIStyle scytheStyle;

	public GUIStyle shovelStyle;

	private Vector3 camPosition;

	private Quaternion camRotaion;

	public bool showChat;

	public bool showChatOld;

	public bool showRanksOld;

	private bool isDeadFrame;

	private int _myCommand;

	public bool respawnedForGUI = true;

	private int _nickColorInd;

	public float liveTime;

	public float timerShowUp;

	public float timerShowLeft;

	public float timerShowDown;

	public float timerShowRight;

	public string myIp = string.Empty;

	public TrainingController trainigController;

	public bool isKilled;

	public bool theEnd;

	public string nickPobeditel;

	public Texture hitTexture;

	public Texture _skin;

	public float showNoInetTimer = 5f;

	private SaltedInt _killCount = new SaltedInt(428452539);

	private float _timeWhenPurchShown;

	private bool inAppOpenedFromPause;

	public bool isMulti;

	public bool isInet;

	public bool isMine;

	public bool isCompany;

	public bool isCOOP;

	private ExperienceController expController;

	private float inGameTime;

	public int multiKill;

	private HungerGameController hungerGameController;

	private bool isHunger;

	public static float maxTimerShowMultyKill = 3f;

	public FlagController flag1;

	public FlagController flag2;

	public FlagController myFlag;

	public FlagController enemyFlag;

	private GameObject rocketToLaunch;

	public bool isStartAngel;

	private float maxTimerImmortality = 3f;

	public bool isImmortality = true;

	private float timerImmortality = 3f;

	private float timerImmortalityForAlpha = 3f;

	private readonly KillerInfo _killerInfo = new KillerInfo();

	private List<int> myKillAssists = new List<int>();

	private readonly List<NetworkViewID> myKillAssistsLocal = new List<NetworkViewID>();

	private bool showGrenadeHint = true;

	private bool showZoomHint = true;

	private bool showChangeWeaponHint = true;

	private int respawnCountForTraining;

	[NonSerialized]
	public string currentWeapon;

	[NonSerialized]
	public int mechUpgrade;

	[NonSerialized]
	public int turretUpgrade;

	private bool _weaponPopularityCacheIsDirty;

	public int[] counterSerials = new int[6];

	private float _curBaseArmor;

	public int AmmoBoxWidth = 100;

	public int AmmoBoxHeight = 100;

	public int AmmoBoxOffset = 10;

	public int ScoreBoxWidth = 100;

	public int ScoreBoxHeight = 100;

	public int ScoreBoxOffset = 10;

	public float[] timerShow = new float[3] { -1f, -1f, -1f };

	public AudioClip deadPlayerSound;

	public AudioClip damagePlayerSound;

	public AudioClip damageArmorPlayerSound;

	private float GunFlashLifetime;

	public GameObject[] zoneCreatePlayer;

	public GUIStyle ScoreBox;

	public GUIStyle AmmoBox;

	private float mySens;

	public GUIStyle sliderSensStyle;

	public GUIStyle thumbSensStyle;

	private GameObject damage;

	private bool damageShown;

	private Pauser _pauser;

	private bool _backWasPressed;

	public GameObject _player;

	public GameObject bulletPrefab;

	public GameObject bulletPrefabRed;

	public GameObject bulletPrefabFor252;

	public GameObject _bulletSpawnPoint;

	private GameObject _inAppGameObject;

	public StoreKitEventListener _listener;

	public GUIStyle puliInApp;

	public InGameGUI inGameGUI;

	private Dictionary<string, Action<string>> _actionsForPurchasedItems = new Dictionary<string, Action<string>>();

	public GUIStyle crystalSwordInapp;

	public GUIStyle elixirInapp;

	public GUIStyle pulemetInApp;

	public bool _isInappWinOpen;

	private WeaponManager ___weaponManager;

	public GUIStyle armorStyle;

	private SaltedInt _countKillsCommandBlue = new SaltedInt(180068360);

	private SaltedInt _countKillsCommandRed = new SaltedInt(180068361);

	private bool canReceiveSwipes = true;

	public float slideMagnitudeX;

	public float slideMagnitudeY;

	public AudioClip ChangeWeaponClip;

	public AudioClip ChangeGrenadeClip;

	public AudioClip WeaponBonusClip;

	public PhotonView photonView;

	public AudioClip clickShop;

	public List<MessageChat> messages = new List<MessageChat>();

	public bool isSurvival;

	public string myTableId;

	private int oldKilledPlayerCharactersCount;

	public Material _bodyMaterial;

	public Material _mechMaterial;

	public Material _bearMaterial;

	public Material curMainSelect;

	public GameObject jetPackPoint;

	public GameObject jetPackPointMech;

	public GameObject wingsPoint;

	public GameObject wingsPointBear;

	public Animation wingsAnimation;

	public Animation wingsBearAnimation;

	private bool isPlayerFlying;

	public ParticleSystem[] jetPackParticle;

	public GameObject jetPackSound;

	public AudioSource wingsSound;

	private int indexWeapon;

	private bool shouldSetMaxAmmoWeapon;

	private bool _changingWeapon;

	private IDisposable _backSubscription;

	private bool BonusEffectForArmorWorksInThisMatch;

	private bool ArmorBonusGiven;

	private bool isDaterRegim;

	private bool pausedRating;

	public float _currentReloadAnimationSpeed = 1f;

	private int countHouseKeeperEvent;

	private bool isJumpPresedOld;

	private int countFixedUpdateForResetLabel;

	private float _chanceToIgnoreHeadshot;

	private bool roomTierInitialized;

	private int roomTier;

	private bool _escapePressed;

	private float oldAlphaImmortality = -1f;

	private float _timeOfSlowdown;

	private bool isActiveTurretPanelInPause;

	private SaltedInt numberOfGrenadesOnStart = new SaltedInt(45853678);

	private SaltedInt numberOfGrenades = new SaltedInt(29076718);

	public float timeBuyHealth = -10000f;

	private float _curHealth;

	public float synhHealth = -10000000f;

	public double synchTimeHealth;

	public bool isSuicided;

	public bool killedInMatch;

	private float damageBuff = 1f;

	private float protectionBuff = 1f;

	private bool getLocalHurt;

	private bool isRaiderMyPoint;

	private int countMySpotEvent;

	public Vector3 pointAutoAim;

	private float timerUpdatePointAutoAi = -1f;

	private Ray rayAutoAim;

	public bool isShootingLoop;

	private CancellationTokenSource ctsShootLoop = new CancellationTokenSource();

	private int _countShootInBurst;

	private float _timerDelayInShootingBurst = -1f;

	private float timeGrenadePress;

	public bool isGrenadePress;

	public bool isPlacemarker
	{
		get
		{
			return _isPlacemarker;
		}
		set
		{
			_isPlacemarker = value;
			placemarkerMark.SetActive(value);
		}
	}

	public float koofDamageWeaponFromPotoins
	{
		get
		{
			return _koofDamageWeaponFromPotoins;
		}
		set
		{
			_koofDamageWeaponFromPotoins = value;
		}
	}

	private float maxTimerRegenerationArmor
	{
		get
		{
			return EffectsController.RegeneratingArmorTime;
		}
	}

	public float[] byCatDamageModifs
	{
		get
		{
			return _byCatDamageModifs;
		}
	}

	public int myCommand
	{
		get
		{
			return _myCommand;
		}
		set
		{
			_myCommand = value;
			UpdateNickLabelColor();
		}
	}

	public int countKills
	{
		get
		{
			return _killCount.Value;
		}
		set
		{
			_killCount.Value = value;
		}
	}

	public KillerInfo killerInfo
	{
		get
		{
			return _killerInfo;
		}
	}

	internal static bool NeedApply { get; set; }

	internal static bool AnotherNeedApply { get; set; }

	private float maxBaseArmor
	{
		get
		{
			return 9f + EffectsController.ArmorBonus;
		}
	}

	private float CurrentBaseArmor
	{
		get
		{
			return _curBaseArmor;
		}
		set
		{
			_curBaseArmor = value;
		}
	}

	public bool isInappWinOpen
	{
		get
		{
			return _isInappWinOpen;
		}
		set
		{
			_isInappWinOpen = value;
			ShopNGUIController.GuiActive = value;
			if (!(myCurrentWeaponSounds != null))
			{
				return;
			}
			myCurrentWeaponSounds.animationObject.SetActive(!value);
			if (myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>() != null)
			{
				if (value)
				{
					myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().Stop();
				}
				else
				{
					myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().Play();
				}
			}
		}
	}

	public static int FontSizeForMessages
	{
		get
		{
			return Mathf.RoundToInt((float)Screen.height * 0.03f);
		}
	}

	public WeaponManager _weaponManager
	{
		get
		{
			return ___weaponManager;
		}
		set
		{
			___weaponManager = value;
		}
	}

	public int countKillsCommandBlue
	{
		get
		{
			return _countKillsCommandBlue.Value;
		}
		set
		{
			_countKillsCommandBlue.Value = value;
		}
	}

	public int countKillsCommandRed
	{
		get
		{
			return _countKillsCommandRed.Value;
		}
		set
		{
			_countKillsCommandRed.Value = value;
		}
	}

	public NetworkView _networkView { get; set; }

	private Material mainDamageMaterial
	{
		get
		{
			if (isMechActive)
			{
				curMainSelect = _mechMaterial;
				return _mechMaterial;
			}
			if (isBearActive)
			{
				curMainSelect = _bearMaterial;
				return _bearMaterial;
			}
			curMainSelect = _bodyMaterial;
			return _bodyMaterial;
		}
	}

	public bool isNeedTakePremiumAccountRewards { get; private set; }

	public int GrenadeCount
	{
		get
		{
			return numberOfGrenades.Value;
		}
		set
		{
			numberOfGrenades.Value = value;
		}
	}

	private float WearedCurrentArmor
	{
		get
		{
			return CurrentBodyArmor + CurrentHatArmor;
		}
	}

	private float CurrentBodyArmor
	{
		get
		{
			float value = 0f;
			Wear.curArmor.TryGetValue(FriendsController.sharedController.armorName ?? string.Empty, out value);
			return value;
		}
		set
		{
			if (Wear.curArmor.ContainsKey(FriendsController.sharedController.armorName ?? string.Empty))
			{
				Wear.curArmor[FriendsController.sharedController.armorName ?? string.Empty] = value;
			}
		}
	}

	private float CurrentHatArmor
	{
		get
		{
			float value = 0f;
			Wear.curArmor.TryGetValue(FriendsController.sharedController.hatName ?? string.Empty, out value);
			return value;
		}
		set
		{
			if (Wear.curArmor.ContainsKey(FriendsController.sharedController.hatName ?? string.Empty))
			{
				Wear.curArmor[FriendsController.sharedController.hatName ?? string.Empty] = value;
			}
		}
	}

	public static int _ShootRaycastLayerMask
	{
		get
		{
			return -2053 & ~(1 << LayerMask.NameToLayer("DamageCollider")) & ~(1 << LayerMask.NameToLayer("TransparentFX"));
		}
	}

	public static int MaxPlayerGUIHealth
	{
		get
		{
			return 9;
		}
	}

	public float CurHealth
	{
		get
		{
			return _curHealth;
		}
		set
		{
			float num = _curHealth - value;
			_curHealth -= num;
		}
	}

	public float MaxHealth
	{
		get
		{
			return (!Defs.isMulti || !Defs.isHunger) ? ExperienceController.HealthByLevel[(!Defs.isMulti || !(myNetworkStartTable != null)) ? ExperienceController.sharedController.currentLevel : myNetworkStartTable.myRanks] : ExperienceController.HealthByLevel[1];
		}
	}

	public float curArmor
	{
		get
		{
			return CurrentBaseArmor + CurrentBodyArmor + CurrentHatArmor;
		}
		set
		{
			float num = curArmor - value;
			if (num >= 0f)
			{
				if (CurrentHatArmor >= num)
				{
					CurrentHatArmor -= num;
					return;
				}
				num -= CurrentHatArmor;
				CurrentHatArmor = 0f;
				if (CurrentBodyArmor >= num)
				{
					CurrentBodyArmor -= num;
					return;
				}
				num -= CurrentBodyArmor;
				CurrentBodyArmor = 0f;
				CurrentBaseArmor -= num;
			}
			else if (num < 0f)
			{
				num *= -1f;
				num = ((!(WearedMaxArmor > 0f)) ? 1f : ((!(WearedMaxArmor > 5f)) ? (WearedMaxArmor - WearedCurrentArmor) : Mathf.Min(WearedMaxArmor - WearedCurrentArmor, WearedMaxArmor * 0.5f)));
				AddArmor(num);
			}
		}
	}

	public float MaxArmor
	{
		get
		{
			return maxBaseArmor + WearedMaxArmor;
		}
	}

	public float WearedMaxArmor
	{
		get
		{
			float num = Wear.MaxArmorForItem(FriendsController.sharedController.armorName, TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier));
			float num2 = Wear.MaxArmorForItem(FriendsController.sharedController.hatName, TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier));
			return num + num2;
		}
	}

	private bool isNeedShowRespawnWindow
	{
		get
		{
			return !isHunger && !Defs.isRegimVidosDebug && !_killerInfo.isSuicide && Defs.isMulti && !Defs.isCOOP;
		}
	}

	public static event Action StopBlinkShop;

	public event OnMessagesUpdate messageDelegate;

	public event EventHandler<EventArgs> WeaponChanged;

	public event Action<float> FreezerFired;

	private void SaveKillRate()
	{
		try
		{
			if (!isMulti || Defs.isHunger || Defs.isCOOP || (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.None) || Defs.IsSurvival)
			{
				return;
			}
			Action<Dictionary<string, int>, Dictionary<string, Dictionary<int, int>>> action = delegate(Dictionary<string, int> battleDict, Dictionary<string, Dictionary<int, int>> dictToDisk)
			{
				foreach (KeyValuePair<string, int> item in battleDict)
				{
					if (dictToDisk.ContainsKey(item.Key))
					{
						Dictionary<int, int> dictionary2 = dictToDisk[item.Key];
						if (dictionary2.ContainsKey(tierForKilledRate))
						{
							Dictionary<int, int> dictionary3;
							Dictionary<int, int> dictionary4 = (dictionary3 = dictionary2);
							int key;
							int key2 = (key = tierForKilledRate);
							key = dictionary3[key];
							dictionary4[key2] = key + item.Value;
						}
						else
						{
							dictionary2.Add(tierForKilledRate, item.Value);
						}
					}
					else
					{
						dictToDisk.Add(item.Key, new Dictionary<int, int> { { tierForKilledRate, item.Value } });
					}
				}
			};
			action(weKillForKillRate, KillRateStatisticsManager.WeKillOld);
			action(weWereKilledForKillRate, KillRateStatisticsManager.WeWereKilledOld);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("version", GlobalGameController.AppVersion);
			dictionary.Add("wekill", KillRateStatisticsManager.WeKillOld);
			dictionary.Add("wewerekilled", KillRateStatisticsManager.WeWereKilledOld);
			Dictionary<string, object> obj = dictionary;
			Storager.setString("KillRateKeyStatistics", Json.Serialize(obj), false);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in save kill rate statistics: " + ex);
		}
	}

	private void AddWeKillStatisctics(string weaponName)
	{
		if (string.IsNullOrEmpty(weaponName))
		{
			Debug.LogError("AddWeKillStatisctics string.IsNullOrEmpty (weaponName)");
		}
		else if (weKillForKillRate.ContainsKey(weaponName))
		{
			Dictionary<string, int> dictionary;
			Dictionary<string, int> dictionary2 = (dictionary = weKillForKillRate);
			string key;
			string key2 = (key = weaponName);
			int num = dictionary[key];
			dictionary2[key2] = num + 1;
		}
		else
		{
			weKillForKillRate.Add(weaponName, 1);
		}
	}

	private void AddWeWereKilledStatisctics(string weaponName)
	{
		if (string.IsNullOrEmpty(weaponName))
		{
			Debug.LogError("AddWeWereKilledStatisctics string.IsNullOrEmpty (weaponName)");
		}
		else if (weWereKilledForKillRate.ContainsKey(weaponName))
		{
			Dictionary<string, int> dictionary;
			Dictionary<string, int> dictionary2 = (dictionary = weWereKilledForKillRate);
			string key;
			string key2 = (key = weaponName);
			int num = dictionary[key];
			dictionary2[key2] = num + 1;
		}
		else
		{
			weWereKilledForKillRate.Add(weaponName, 1);
		}
	}

	private void UpdateNickLabelColor()
	{
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
		{
			if (WeaponManager.sharedManager.myNetworkStartTable == null || WeaponManager.sharedManager.myNetworkStartTable.myCommand == 0)
			{
				if (_nickColorInd != 0)
				{
					nickLabel.color = Color.white;
					_nickColorInd = 0;
				}
			}
			else if (WeaponManager.sharedManager.myNetworkStartTable.myCommand == myCommand)
			{
				if (_nickColorInd != 1)
				{
					nickLabel.color = Color.blue;
					_nickColorInd = 1;
				}
			}
			else if (_nickColorInd != 2)
			{
				nickLabel.color = Color.red;
				_nickColorInd = 2;
			}
		}
		else if (Defs.isDaterRegim)
		{
			if (_nickColorInd != 0)
			{
				nickLabel.color = Color.white;
				_nickColorInd = 0;
			}
		}
		else if (Defs.isCOOP)
		{
			if (_nickColorInd != 1)
			{
				nickLabel.color = Color.blue;
				_nickColorInd = 1;
			}
		}
		else if (_nickColorInd != 2)
		{
			nickLabel.color = Color.red;
			_nickColorInd = 2;
		}
	}

	public void IndicateDamage()
	{
		isDeadFrame = true;
		Invoke("setisDeadFrameFalse", 1f);
	}

	private void AddArmor(float dt)
	{
		if (WearedMaxArmor > 0f)
		{
			float num = Wear.MaxArmorForItem(FriendsController.sharedController.armorName, TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier));
			float num2 = num - CurrentBodyArmor;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			if (dt <= num2)
			{
				CurrentBodyArmor += dt;
				return;
			}
			CurrentBodyArmor += num2;
			dt -= num2;
			float num3 = Wear.MaxArmorForItem(FriendsController.sharedController.hatName, TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier));
			float num4 = num3 - CurrentHatArmor;
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			CurrentHatArmor += Mathf.Min(num4, dt);
		}
		else
		{
			float num5 = maxBaseArmor - CurrentBaseArmor;
			if (num5 < 0f)
			{
				num5 = 0f;
			}
			if (dt <= num5)
			{
				CurrentBaseArmor += dt;
			}
			else
			{
				CurrentBaseArmor += num5;
			}
		}
	}

	private void Awake()
	{
		isSurvival = Defs.IsSurvival;
		isMulti = Defs.isMulti;
		isInet = Defs.isInet;
		isCompany = Defs.isCompany;
		isCOOP = Defs.isCOOP;
		isHunger = Defs.isHunger;
		if (isHunger)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("HungerGameController");
			if (gameObject == null)
			{
				Debug.LogError("hungerGameControllerObject == null");
			}
			else
			{
				hungerGameController = gameObject.GetComponent<HungerGameController>();
			}
		}
		myCamera.fieldOfView = stdFov;
	}

	public void SetJetpackEnabled(bool _isEnabled)
	{
		Defs.isJetpackEnabled = _isEnabled;
		if (Defs.isSoundFX && _isEnabled)
		{
			AudioSource component = GetComponent<AudioSource>();
			if (component != null)
			{
				component.PlayOneShot(jetpackActivSound);
			}
		}
		if (!Defs.isMulti)
		{
			return;
		}
		if (Defs.isInet)
		{
			if (photonView != null)
			{
				photonView.RPC("SetJetpackEnabledRPC", PhotonTargets.Others, _isEnabled);
			}
		}
		else if (_networkView != null)
		{
			_networkView.RPC("SetJetpackEnabledRPC", RPCMode.Others, _isEnabled);
		}
	}

	[RPC]
	[PunRPC]
	public void SetJetpackEnabledRPC(bool _isEnabled)
	{
		if (Defs.isSoundFX && _isEnabled)
		{
			GetComponent<AudioSource>().PlayOneShot(jetpackActivSound);
		}
		if (Defs.isDaterRegim)
		{
			wingsPoint.SetActive(_isEnabled);
			wingsPointBear.SetActive(_isEnabled);
		}
		else
		{
			jetPackPoint.SetActive(_isEnabled);
			jetPackPointMech.SetActive(_isEnabled);
		}
		if (!_isEnabled)
		{
			for (int i = 0; i < jetPackParticle.Length; i++)
			{
				jetPackParticle[i].enableEmission = _isEnabled;
			}
		}
	}

	public void SetJetpackParticleEnabled(bool _isEnabled)
	{
		if (_isEnabled)
		{
			if (Defs.isDaterRegim)
			{
				isPlayerFlying = true;
			}
			if (ButtonClickSound.Instance != null && Defs.isSoundFX && !Defs.isDaterRegim)
			{
				jetPackSound.SetActive(true);
			}
		}
		else if (Defs.isDaterRegim)
		{
			isPlayerFlying = false;
		}
		else
		{
			jetPackSound.SetActive(false);
		}
		if (Defs.isMulti)
		{
			if (Defs.isInet)
			{
				photonView.RPC("SetJetpackParticleEnabledRPC", PhotonTargets.Others, _isEnabled);
			}
			else
			{
				_networkView.RPC("SetJetpackParticleEnabledRPC", RPCMode.Others, _isEnabled);
			}
		}
	}

	[RPC]
	[PunRPC]
	public void SetJetpackParticleEnabledRPC(bool _isEnabled)
	{
		if (_isEnabled)
		{
			if (Defs.isDaterRegim)
			{
				isPlayerFlying = true;
			}
			if (ButtonClickSound.Instance != null && Defs.isSoundFX && !Defs.isDaterRegim)
			{
				jetPackSound.SetActive(true);
			}
		}
		else if (Defs.isDaterRegim)
		{
			isPlayerFlying = false;
		}
		else
		{
			jetPackSound.SetActive(false);
		}
		for (int i = 0; i < jetPackParticle.Length; i++)
		{
			jetPackParticle[i].enableEmission = _isEnabled;
		}
	}

	[PunRPC]
	[RPC]
	private void SendChatMessageWithIcon(string text, bool _clanMode, string _clanLogo, string _ClanID, string _clanName, string _iconName)
	{
		if ((!_clanMode || _ClanID.Equals(FriendsController.sharedController.ClanID)) && !(_weaponManager == null) && !(_weaponManager.myPlayerMoveC == null))
		{
			if (!isInet)
			{
				_weaponManager.myPlayerMoveC.AddMessage(text, Time.time, -1, myPlayerTransform.GetComponent<NetworkView>().viewID, 0, _clanLogo, _iconName);
			}
			else
			{
				_weaponManager.myPlayerMoveC.AddMessage(text, Time.time, mySkinName.photonView.viewID, myPlayerTransform.GetComponent<NetworkView>().viewID, myCommand, _clanLogo, _iconName);
			}
		}
	}

	[RPC]
	[PunRPC]
	private void SendChatMessage(string text, bool _clanMode, string _clanLogo, string _ClanID, string _clanName)
	{
		SendChatMessageWithIcon(text, _clanMode, _clanLogo, _ClanID, _clanName, string.Empty);
	}

	public void SendChat(string text, bool clanMode, string iconName)
	{
		text = (text.Equals("-=ATTACK!=-") ? LocalizationStore.Get("Key_1086") : (text.Equals("-=HELP!=-") ? LocalizationStore.Get("Key_1087") : (text.Equals("-=OK!=-") ? LocalizationStore.Get("Key_1088") : ((!text.Equals("-=NO!=-")) ? FilterBadWorld.FilterString(text) : LocalizationStore.Get("Key_1089")))));
		if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(iconName))
		{
			if (!isInet)
			{
				_networkView.RPC("SendChatMessageWithIcon", RPCMode.All, "< " + _weaponManager.myNetworkStartTable.NamePlayer + " > " + text, clanMode, FriendsController.sharedController.clanLogo, FriendsController.sharedController.ClanID, FriendsController.sharedController.clanName, iconName);
			}
			else
			{
				photonView.RPC("SendChatMessageWithIcon", PhotonTargets.All, "< " + _weaponManager.myNetworkStartTable.NamePlayer + " > " + text, clanMode, FriendsController.sharedController.clanLogo, FriendsController.sharedController.ClanID, FriendsController.sharedController.clanName, iconName);
			}
		}
	}

	public void SendDaterChat(string nick1, string text, string nick2)
	{
		if (text != string.Empty)
		{
			if (!isInet)
			{
				_networkView.RPC("SendDaterChatRPC", RPCMode.All, nick1, text, nick2, false, FriendsController.sharedController.clanLogo, FriendsController.sharedController.ClanID, FriendsController.sharedController.clanName);
			}
			else
			{
				photonView.RPC("SendDaterChatRPC", PhotonTargets.All, nick1, text, nick2, false, FriendsController.sharedController.clanLogo, FriendsController.sharedController.ClanID, FriendsController.sharedController.clanName);
			}
		}
	}

	[RPC]
	[PunRPC]
	public void SendDaterChatRPC(string nick1, string text, string nick2, bool _clanMode, string _clanLogo, string _ClanID, string _clanName)
	{
		text = "< " + nick1 + "[-] > " + LocalizationStore.Get(text) + " < " + nick2 + "[-] >";
		SendChatMessage(text, _clanMode, _clanLogo, _ClanID, _clanName);
	}

	public void AddMessage(string text, float time, int ID, NetworkViewID IDLocal, int _command, string clanLogo, string iconName)
	{
		MessageChat item = default(MessageChat);
		item.text = text;
		item.iconName = iconName;
		item.time = time;
		item.ID = ID;
		item.IDLocal = IDLocal;
		item.command = _command;
		if (!string.IsNullOrEmpty(clanLogo))
		{
			byte[] data = Convert.FromBase64String(clanLogo);
			Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoWidth);
			texture2D.LoadImage(data);
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			item.clanLogo = texture2D;
		}
		else
		{
			item.clanLogo = null;
		}
		messages.Add(item);
		if (messages.Count > 30)
		{
			messages.RemoveAt(0);
		}
		OnMessagesUpdate onMessagesUpdate = this.messageDelegate;
		if (onMessagesUpdate != null)
		{
			onMessagesUpdate();
		}
	}

	public void WalkAnimation()
	{
		if (_singleOrMultiMine() || (Defs.isDaterRegim && isBearActive))
		{
			if ((isBearActive || isMechActive) && !mechGunAnimation.IsPlaying("Shoot"))
			{
				mechGunAnimation.CrossFade("Walk");
			}
			if ((bool)_weaponManager && (bool)_weaponManager.currentWeaponSounds && _weaponManager.currentWeaponSounds.animationObject != null)
			{
				_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().CrossFade("Walk");
			}
		}
	}

	public void IdleAnimation()
	{
		if (_singleOrMultiMine() || (Defs.isDaterRegim && isBearActive))
		{
			if ((isBearActive || isMechActive) && !mechGunAnimation.IsPlaying("Shoot"))
			{
				mechGunAnimation.CrossFade("Idle");
			}
			if ((bool)___weaponManager && (bool)___weaponManager.currentWeaponSounds && ___weaponManager.currentWeaponSounds.animationObject != null)
			{
				___weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().CrossFade("Idle");
			}
		}
	}

	public void ZoomPress()
	{
		if (WeaponManager.sharedManager.currentWeaponSounds.isGrenadeWeapon)
		{
			return;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted)
		{
			showZoomHint = false;
			HintController.instance.HideHintByName("use_zoom");
		}
		isZooming = !isZooming;
		if (isZooming)
		{
			if (Defs.isSoundFX && _weaponManager.currentWeaponSounds.zoomIn != null)
			{
				GetComponent<AudioSource>().PlayOneShot(_weaponManager.currentWeaponSounds.zoomIn);
			}
			myCamera.fieldOfView = _weaponManager.currentWeaponSounds.fieldOfViewZomm;
			gunCamera.gameObject.SetActive(false);
			inGameGUI.SetScopeForWeapon(_weaponManager.currentWeaponSounds.scopeNum.ToString());
			myTransform.localPosition = new Vector3(myTransform.localPosition.x, myTransform.localPosition.y, myTransform.localPosition.z);
		}
		else
		{
			if (Defs.isSoundFX && _weaponManager.currentWeaponSounds.zoomOut != null)
			{
				GetComponent<AudioSource>().PlayOneShot(_weaponManager.currentWeaponSounds.zoomOut);
			}
			myCamera.fieldOfView = stdFov;
			gunCamera.fieldOfView = 75f;
			gunCamera.gameObject.SetActive(true);
			if (inGameGUI != null)
			{
				inGameGUI.ResetScope();
			}
		}
		if (isMulti && isInet)
		{
			photonView.RPC("SynhIsZoming", PhotonTargets.All, isZooming);
		}
	}

	[RPC]
	[PunRPC]
	private void SynhIsZoming(bool _isZoomming)
	{
		isZooming = _isZoomming;
	}

	public void hideGUI()
	{
		showGUI = false;
	}

	public void setMyTamble(GameObject _myTable)
	{
		if (myTable == null || _myTable == null)
		{
			return;
		}
		NetworkStartTable component = myTable.GetComponent<NetworkStartTable>();
		if (component == null)
		{
			return;
		}
		component.myPlayerMoveC = this;
		myTable = _myTable;
		myNetworkStartTable = myTable.GetComponent<NetworkStartTable>();
		if (myNetworkStartTable == null)
		{
			return;
		}
		CurHealth = MaxHealth;
		myCommand = myNetworkStartTable.myCommand;
		if (Initializer.redPlayers.Contains(this) && myCommand == 1)
		{
			Initializer.redPlayers.Remove(this);
		}
		if (Initializer.bluePlayers.Contains(this) && myCommand == 2)
		{
			Initializer.bluePlayers.Remove(this);
		}
		if (myCommand == 1 && !Initializer.bluePlayers.Contains(this))
		{
			Initializer.bluePlayers.Add(this);
		}
		if (myCommand == 2 && !Initializer.redPlayers.Contains(this))
		{
			Initializer.redPlayers.Add(this);
		}
		_skin = myNetworkStartTable.mySkin;
		SetTextureForBodyPlayer(_skin);
		if (isMine)
		{
			if (FriendsController.useBuffSystem)
			{
				BuffSystem.instance.CheckForPlayerBuff();
			}
			else if (KillRateCheck.instance.buffEnabled)
			{
				SetupBuffParameters(KillRateCheck.instance.damageBuff, KillRateCheck.instance.healthBuff);
			}
			else
			{
				SetupBuffParameters(1f, 1f);
			}
		}
		if (Defs.isMulti && Defs.isInet && myNetworkStartTable.myRanks < 4)
		{
			BonusController.sharedController.lowLevelPlayers.Add(photonView.ownerId);
		}
	}

	public void SetupBuffParameters(float damage, float protection)
	{
		bool flag = damageBuff != damage || protectionBuff != protection;
		SetBuffParameters(damage, protection);
		if (flag && Defs.isMulti && Defs.isInet)
		{
			photonView.RPC("SendBuffParameters", PhotonTargets.Others, damageBuff, protectionBuff);
		}
	}

	private void SetBuffParameters(float damage, float protection)
	{
		damageBuff = Mathf.Clamp(damage, 0.01f, 10f);
		protectionBuff = Mathf.Clamp(protection, 0.01f, 10f);
		Debug.Log(string.Format("<color=green>{0}Damage: {1}, Protection: {2}</color>", (!isMine) ? ("(" + mySkinName.NickName + ") ") : "(you) ", damageBuff, protectionBuff));
	}

	[PunRPC]
	private void SendBuffParameters(float damage, float protection)
	{
		if (!isMine)
		{
			SetBuffParameters(damage, protection);
		}
	}

	public void AddWeapon(GameObject weaponPrefab)
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			int num = WeaponManager.sharedManager.playerWeapons.OfType<Weapon>().ToList().FindIndex((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor == weaponPrefab.GetComponent<WeaponSounds>().categoryNabor);
			if (num >= 0)
			{
				ChangeWeapon(num, false);
			}
			return;
		}
		int score;
		if (_weaponManager.AddWeapon(weaponPrefab, out score))
		{
			ChangeWeapon(_weaponManager.CurrentWeaponIndex, false);
			return;
		}
		if (ItemDb.IsWeaponCanDrop(ItemDb.GetByPrefabName(weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag))
		{
			GlobalGameController.Score += score;
			if (Defs.isSoundFX)
			{
				if (WeaponBonusClip != null)
				{
					base.gameObject.GetComponent<AudioSource>().PlayOneShot(WeaponBonusClip);
				}
				else
				{
					base.gameObject.GetComponent<AudioSource>().PlayOneShot(ChangeWeaponClip);
				}
			}
			return;
		}
		foreach (Weapon playerWeapon in _weaponManager.playerWeapons)
		{
			if (playerWeapon.weaponPrefab == weaponPrefab)
			{
				ChangeWeapon(_weaponManager.playerWeapons.IndexOf(playerWeapon), false);
				break;
			}
		}
	}

	public void minusLiveFromZombi(float _minusLive, Vector3 posZombi)
	{
		photonView.RPC("minusLiveFromZombiRPC", PhotonTargets.All, _minusLive, posZombi);
	}

	[RPC]
	[PunRPC]
	public void StartFlashRPC()
	{
		StartCoroutine(Flash(myPlayerTransform.gameObject));
	}

	public void SendStartFlashMine()
	{
		if (!isInet)
		{
			_networkView.RPC("StartFlashRPC", RPCMode.All);
		}
		else
		{
			photonView.RPC("StartFlashRPC", PhotonTargets.All);
		}
	}

	public void StartFlash(GameObject _obj)
	{
		StartCoroutine(Flash(_obj));
	}

	public static void SetLayerRecursively(GameObject obj, int newLayer)
	{
		if (null == obj)
		{
			return;
		}
		obj.layer = newLayer;
		int childCount = obj.transform.childCount;
		Transform transform = obj.transform;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!(null == child))
			{
				SetLayerRecursively(child.gameObject, newLayer);
			}
		}
	}

	public static void PerformActionRecurs(GameObject obj, Action<Transform> act)
	{
		if (act == null || null == obj)
		{
			return;
		}
		act(obj.transform);
		int childCount = obj.transform.childCount;
		Transform transform = obj.transform;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!(null == child))
			{
				PerformActionRecurs(child.gameObject, act);
			}
		}
	}

	public void ChangeWeapon(int index, bool shouldSetMaxAmmo = true)
	{
		if (index == 1001)
		{
			currentWeaponBeforeTurret = WeaponManager.sharedManager.CurrentWeaponIndex;
		}
		indexWeapon = index;
		shouldSetMaxAmmoWeapon = shouldSetMaxAmmo;
		StopCoroutine("ChangeWeaponCorutine");
		StopCoroutine(BazookaShoot());
		StartCoroutine("ChangeWeaponCorutine");
		if (GetComponent<AudioSource>() != null && !isMechActive)
		{
			GetComponent<AudioSource>().Stop();
		}
	}

	private IEnumerator ChangeWeaponCorutine()
	{
		_changingWeapon = true;
		if (inGameGUI != null)
		{
			inGameGUI.StopAllCircularIndicators();
		}
		photonView.synchronization = ViewSynchronization.Off;
		_networkView.stateSynchronization = NetworkStateSynchronization.Off;
		if (!Defs.isTurretWeapon)
		{
			while (deltaAngle < 40f && !Defs.isTurretWeapon && !isMechActive)
			{
				deltaAngle += 300f * Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			if (!isMechActive)
			{
				deltaAngle = 40f;
			}
			Defs.isTurretWeapon = false;
		}
		ChangeWeaponReal(indexWeapon, shouldSetMaxAmmoWeapon);
		if (indexWeapon != 1001 && !isMechActive)
		{
			while (deltaAngle > 0f)
			{
				deltaAngle -= 300f * Time.deltaTime;
				if (deltaAngle < 0f)
				{
					deltaAngle = -0.01f;
				}
				yield return null;
			}
		}
		if (indexWeapon == 1001)
		{
			deltaAngle = 0f;
		}
		photonView.synchronization = ViewSynchronization.Unreliable;
		_networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		_changingWeapon = false;
	}

	public void ChangeWeaponReal(int index, bool shouldSetMaxAmmo = true)
	{
		if (inGameGUI != null)
		{
			inGameGUI.StopAllCircularIndicators();
		}
		EventHandler<EventArgs> weaponChanged = this.WeaponChanged;
		if (weaponChanged != null)
		{
			weaponChanged(this, EventArgs.Empty);
		}
		if (isZooming)
		{
			ZoomPress();
		}
		photonView = PhotonView.Get(this);
		_networkView = GetComponent<NetworkView>();
		Quaternion rotation = Quaternion.identity;
		if ((bool)_player)
		{
			rotation = _player.transform.rotation;
		}
		if ((bool)_weaponManager.currentWeaponSounds)
		{
			rotation = _weaponManager.currentWeaponSounds.gameObject.transform.rotation;
			_SetGunFlashActive(false);
			_weaponManager.currentWeaponSounds.gameObject.transform.parent = null;
			UnityEngine.Object.Destroy(_weaponManager.currentWeaponSounds.gameObject);
			_weaponManager.currentWeaponSounds = null;
		}
		if (isShootingLoop)
		{
			ShotUnPressed();
		}
		ResetShootingBurst();
		GameObject gameObject = null;
		object obj;
		switch (index)
		{
		case 1000:
			obj = ((!Defs.isDaterRegim) ? grenadePrefab : likePrefab);
			break;
		case 1001:
			obj = turretPrefab;
			break;
		default:
			obj = ((Weapon)_weaponManager.playerWeapons[index]).weaponPrefab;
			break;
		}
		GameObject gameObject2 = (GameObject)obj;
		gameObject = (myCurrentWeapon = (GameObject)UnityEngine.Object.Instantiate(gameObject2, Vector3.zero, Quaternion.identity));
		Debug.Log("setting my weapon sonds");
		myCurrentWeaponSounds = myCurrentWeapon.GetComponent<WeaponSounds>();
		Debug.Log("go on?");
		if (!ShopNGUIController.GuiActive && myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>() != null)
		{
			myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().Play();
		}
		if (myCurrentWeaponSounds.isDoubleShot && !isMechActive)
		{
			gunCamera.transform.localPosition = Vector3.zero;
		}
		else
		{
			gunCamera.transform.localPosition = new Vector3(-0.1f, 0f, 0f);
		}
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.rotation = rotation;
		myCurrentWeaponSounds.animationObject.GetComponent<Animation>().cullingType = AnimationCullingType.AlwaysAnimate;
		if (isMechActive)
		{
			myCurrentWeapon.SetActive(false);
		}
		if (Defs.isDaterRegim)
		{
			SetWeaponVisible(!isBearActive);
		}
		if (myCurrentWeaponSounds != null && PhotonNetwork.room != null)
		{
			Statistics.Instance.IncrementWeaponPopularity(LocalizationStore.GetByDefault(myCurrentWeaponSounds.localizeWeaponKey), false);
			_weaponPopularityCacheIsDirty = true;
		}
		if (isMulti)
		{
			if (isInet)
			{
				photonView.RPC("SetWeaponRPC", PhotonTargets.Others, gameObject2.name, gameObject2.GetComponent<WeaponSounds>().alternativeName);
			}
			else
			{
				GetComponent<NetworkView>().RPC("SetWeaponRPC", RPCMode.OthersBuffered, gameObject2.name, gameObject2.GetComponent<WeaponSounds>().alternativeName);
			}
		}
		if (index == 1000)
		{
			WeaponSounds component = gameObject2.GetComponent<WeaponSounds>();
			GameObject rocket = RocketStack.sharedController.GetRocket();
			if (rocket != null)
			{
				Rocket component2 = rocket.GetComponent<Rocket>();
				component2.rocketNum = ((!Defs.isDaterRegim) ? 10 : 40);
				component2.weaponName = ((!Defs.isDaterRegim) ? "WeaponGrenade" : "WeaponLike");
				component2.weaponPrefabName = component2.weaponName;
				component2.damage = (float)component.damage * (1f + koofDamageWeaponFromPotoins + EffectsController.GrenadeExplosionDamageIncreaseCoef);
				component2.radiusDamage = component.bazookaExplosionRadius * EffectsController.GrenadeExplosionRadiusIncreaseCoef;
				component2.radiusDamageSelf = component.bazookaExplosionRadiusSelf;
				component2.radiusImpulse = component.bazookaImpulseRadius * (1f + EffectsController.ExplosionImpulseRadiusIncreaseCoef);
				component2.impulseForce = component.impulseForce;
				component2.impulseForceSelf = component.impulseForceSelf;
				component2.damageRange = component.damageRange * (1f + koofDamageWeaponFromPotoins);
				float num = ((ExpController.Instance != null && ExpController.Instance.OurTier < component.DamageByTier.Length) ? component.DamageByTier[TierOrRoomTier(ExpController.Instance.OurTier)] : ((component.DamageByTier.Length <= 0) ? 0f : component.DamageByTier[0]));
				component2.multiplayerDamage = num * (1f + koofDamageWeaponFromPotoins + EffectsController.GrenadeExplosionDamageIncreaseCoef);
				rocket.GetComponent<Rigidbody>().useGravity = false;
				rocket.GetComponent<Rigidbody>().isKinematic = true;
				component2.SendSetRocketActiveRPC();
				if (Defs.isMulti && !Defs.isInet)
				{
					component2.SendNetworkViewMyPlayer(myPlayerTransform.GetComponent<NetworkView>().viewID);
				}
			}
			currentGrenade = rocket;
		}
		if (index == 1001)
		{
			Defs.isTurretWeapon = true;
			turretUpgrade = GearManager.CurrentNumberOfUphradesForGear(GearManager.Turret);
			if (isMulti)
			{
				if (isInet)
				{
					photonView.RPC("SyncTurretUpgrade", PhotonTargets.Others, turretUpgrade);
				}
				else
				{
					GetComponent<NetworkView>().RPC("SyncTurretUpgrade", RPCMode.Others, turretUpgrade);
				}
			}
			GameObject gameObject3;
			if (isMulti)
			{
				if (!isInet)
				{
					UnityEngine.Object prefab = Resources.Load((!Defs.isDaterRegim) ? "Turret" : "MusicBox");
					gameObject3 = (GameObject)Network.Instantiate(prefab, new Vector3(-10000f, -10000f, -10000f), base.transform.rotation, 0);
				}
				else
				{
					gameObject3 = PhotonNetwork.Instantiate((!Defs.isDaterRegim) ? "Turret" : "MusicBox", new Vector3(-10000f, -10000f, -10000f), base.transform.rotation, 0);
				}
			}
			else
			{
				GameObject original = Resources.Load((!Defs.isDaterRegim) ? "Turret" : "MusicBox") as GameObject;
				gameObject3 = UnityEngine.Object.Instantiate(original, new Vector3(-10000f, -10000f, -10000f), base.transform.rotation) as GameObject;
			}
			if (gameObject3 != null)
			{
				TurretController component3 = gameObject3.GetComponent<TurretController>();
				gameObject3.GetComponent<Rigidbody>().useGravity = false;
				gameObject3.GetComponent<Rigidbody>().isKinematic = true;
				if (Defs.isMulti && !Defs.isInet)
				{
					component3.SendNetworkViewMyPlayer(myPlayerTransform.GetComponent<NetworkView>().viewID);
				}
			}
			currentTurret = gameObject3;
		}
		GameObject gameObject4 = null;
		if (!myCurrentWeaponSounds.isMelee)
		{
			foreach (Transform item in gameObject.transform)
			{
				if (item.gameObject.name.Equals("BulletSpawnPoint") && item.childCount > 0)
				{
					gameObject4 = item.GetChild(0).gameObject;
					WeaponManager.SetGunFlashActive(gameObject4, false);
					break;
				}
			}
		}
		SetTextureForBodyPlayer(_skin);
		SetLayerRecursively(gameObject, 9);
		_weaponManager.currentWeaponSounds = myCurrentWeaponSounds;
		if (index < 1000)
		{
			_weaponManager.CurrentWeaponIndex = index;
			_weaponManager.SaveWeaponAsLastUsed(_weaponManager.CurrentWeaponIndex);
			if (inGameGUI != null)
			{
				if (_weaponManager.currentWeaponSounds.isMelee && !_weaponManager.currentWeaponSounds.isShotMelee && !isMechActive)
				{
					inGameGUI.fireButtonSprite.spriteName = "controls_strike";
					inGameGUI.fireButtonSprite2.spriteName = "controls_strike";
				}
				else
				{
					inGameGUI.fireButtonSprite.spriteName = "controls_fire";
					inGameGUI.fireButtonSprite2.spriteName = "controls_fire";
				}
			}
		}
		if (gameObject.transform.parent == null)
		{
			Debug.LogWarning("nw.transform.parent == null");
		}
		else if (_weaponManager.currentWeaponSounds == null)
		{
			Debug.LogWarning("_weaponManager.currentWeaponSounds == null");
		}
		else
		{
			gameObject.transform.position = gameObject.transform.parent.TransformPoint(_weaponManager.currentWeaponSounds.gunPosition);
		}
		TouchPadController rightJoystick = JoystickController.rightJoystick;
		if (index < 1000 && rightJoystick != null)
		{
			if (((Weapon)_weaponManager.playerWeapons[index]).currentAmmoInClip > 0 || (_weaponManager.currentWeaponSounds.isMelee && !_weaponManager.currentWeaponSounds.isShotMelee))
			{
				rightJoystick.HasAmmo();
				if (inGameGUI != null)
				{
					inGameGUI.BlinkNoAmmo(0);
				}
			}
			else
			{
				rightJoystick.NoAmmo();
				if (inGameGUI != null)
				{
					inGameGUI.BlinkNoAmmo(1);
				}
			}
		}
		if (_weaponManager.currentWeaponSounds.animationObject != null)
		{
			if (_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().GetClip("Reload") != null)
			{
				_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].layer = 1;
			}
			if (!_weaponManager.currentWeaponSounds.isDoubleShot)
			{
				if (_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().GetClip("Shoot") != null)
				{
					_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot"].layer = 1;
				}
			}
			else
			{
				_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot1"].layer = 1;
				_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot2"].layer = 1;
			}
		}
		if (!_weaponManager.currentWeaponSounds.isMelee)
		{
			foreach (Transform item2 in _weaponManager.currentWeaponSounds.gameObject.transform)
			{
				if (item2.name.Equals("BulletSpawnPoint"))
				{
					_bulletSpawnPoint = item2.gameObject;
					break;
				}
			}
			GunFlash = _bulletSpawnPoint.transform.GetChild(0);
		}
		if (Defs.isSoundFX && !Defs.isDaterRegim && !isMechActive)
		{
			base.gameObject.GetComponent<AudioSource>().PlayOneShot((index != 1000) ? ChangeWeaponClip : ChangeGrenadeClip);
		}
		if (!Defs.isDaterRegim && isInvisible)
		{
			SetInVisibleShaders(isInvisible);
		}
		if (inGameGUI != null)
		{
			if (isMechActive)
			{
				inGameGUI.SetCrosshair(mechWeaponSounds);
			}
			else
			{
				inGameGUI.SetCrosshair(_weaponManager.currentWeaponSounds);
			}
		}
		UpdateEffectsForCurrentWeapon(mySkinName.currentCape, mySkinName.currentMask, mySkinName.currentHat);
		if (myCurrentWeaponSounds.isZooming && !TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && showZoomHint)
		{
			Invoke("TrainingShowZoomHint", 3f);
		}
	}

	private void TrainingShowZoomHint()
	{
		if (myCurrentWeaponSounds.isZooming && !TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && showZoomHint)
		{
			HintController.instance.ShowHintByName("use_zoom", 0f);
		}
	}

	[RPC]
	[PunRPC]
	private void SetWeaponRPC(string _nameWeapon, string _alternativeNameWeapon)
	{
		isWeaponSet = true;
		GameObject gameObject = null;
		if (_nameWeapon.Equals("WeaponGrenade"))
		{
			gameObject = grenadePrefab;
			currentWeapon = null;
		}
		else if (_nameWeapon.Equals("WeaponLike"))
		{
			gameObject = likePrefab;
			currentWeapon = null;
		}
		else if (_nameWeapon.Equals("WeaponTurret"))
		{
			gameObject = turretPrefab;
			currentWeapon = null;
		}
		else
		{
			if (Device.isPixelGunLow && !Defs.isHunger && !Defs.isDaterRegim && (_nameWeapon == null || !(_nameWeapon == WeaponManager.PistolWN) || !(WeaponManager.sharedManager != null) || WeaponManager.sharedManager._currentFilterMap != 2))
			{
				try
				{
					_nameWeapon = WeaponManager.sharedManager.GunsForPixelGunLow[_nameWeapon];
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in _nameWeapon = WeaponManager.sharedManager.GunsForPixelGunLow: " + ex);
				}
			}
			else if (_nameWeapon != null && _alternativeNameWeapon != null && WeaponManager.Removed150615_PrefabNames.Contains(_nameWeapon))
			{
				_nameWeapon = _alternativeNameWeapon;
			}
			gameObject = Resources.Load("Weapons/" + _nameWeapon) as GameObject;
			if (gameObject != null)
			{
				WeaponSounds component = gameObject.GetComponent<WeaponSounds>();
				if (component != null && component.tier > 100)
				{
					gameObject = null;
				}
			}
			if (gameObject != null)
			{
				currentWeapon = ItemDb.GetByPrefabName(gameObject.name.Replace("(Clone)", string.Empty)).Tag;
			}
		}
		if (gameObject == null)
		{
			gameObject = Resources.Load("Weapons/" + _alternativeNameWeapon) as GameObject;
			if (gameObject != null)
			{
				currentWeapon = ItemDb.GetByPrefabName(gameObject.name.Replace("(Clone)", string.Empty)).Tag;
			}
		}
		if (_nameWeapon.Equals("WeaponGrenade") && Defs.isSoundFX && !Defs.isDaterRegim)
		{
			base.gameObject.GetComponent<AudioSource>().PlayOneShot(ChangeGrenadeClip);
		}
		if (gameObject != null)
		{
			// why is this here?
			// GameObject gameObject2 = null;
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
			if (isMechActive)
			{
				gameObject2.SetActive(false);
			}
			Debug.Log("sttting my weapo");
			myCurrentWeapon = gameObject2;
			Debug.Log("go on set wepon?");
			myCurrentWeaponSounds = myCurrentWeapon.GetComponent<WeaponSounds>();
			if (myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>() != null)
			{
				myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().Play();
			}
			if (Defs.isDaterRegim)
			{
				SetWeaponVisible(!isBearActive);
			}
			GunFlash = myCurrentWeaponSounds.gunFlash;
			Transform transform = mySkinName.armorPoint.transform;
			if (transform.childCount > 0)
			{
				ArmorRefs component2 = transform.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
				component2.leftBone.GetComponent<SetPosInArmor>().target = myCurrentWeaponSounds.LeftArmorHand;
				component2.rightBone.GetComponent<SetPosInArmor>().target = myCurrentWeaponSounds.RightArmorHand;
			}
			foreach (Transform item in base.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			gameObject2.transform.parent = base.gameObject.transform;
			GameObject gameObject3 = null;
			gameObject2.transform.position = Vector3.zero;
			if (!myCurrentWeaponSounds.isMelee)
			{
				foreach (Transform item2 in gameObject2.transform)
				{
					if (item2.gameObject.name.Equals("BulletSpawnPoint") && item2.childCount > 0)
					{
						gameObject3 = item2.GetChild(0).gameObject;
						WeaponManager.SetGunFlashActive(gameObject3, false);
						break;
					}
				}
			}
			if (base.transform.Find("BulletSpawnPoint") != null)
			{
				_bulletSpawnPoint = base.transform.Find("BulletSpawnPoint").gameObject;
			}
			base.transform.localPosition = new Vector3(0f, 0.4f, 0f);
			gameObject2.transform.localPosition = new Vector3(0f, -1.4f, 0f);
			gameObject2.transform.rotation = base.transform.rotation;
			SetTextureForBodyPlayer(_skin);
		}
		UpdateEffectsForCurrentWeapon(mySkinName.currentCape, mySkinName.currentMask, mySkinName.currentHat);
	}

	[Obfuscation(Exclude = true)]
	public void SetStealthModifier()
	{
		if (!(_player != null))
		{
		}
	}

	public bool NeedAmmo()
	{
		if (_weaponManager == null)
		{
			return false;
		}
		int currentWeaponIndex = _weaponManager.CurrentWeaponIndex;
		Weapon weapon = (Weapon)_weaponManager.playerWeapons[currentWeaponIndex];
		return weapon.currentAmmoInBackpack < _weaponManager.currentWeaponSounds.MaxAmmoWithEffectApplied;
	}

	private void SwitchPause()
	{
		if (CurHealth > 0f)
		{
			SetPause();
		}
	}

	private void ShopPressed()
	{
		JoystickController.rightJoystick.jumpPressed = false;
		JoystickController.leftTouchPad.isJumpPressed = false;
		JoystickController.rightJoystick.Reset();
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			if (TrainingController.stepTrainingList.ContainsKey("InterTheShop"))
			{
				TrainingController.isNextStep = TrainingState.EnterTheShop;
				if (Player_move_c.StopBlinkShop != null)
				{
					Player_move_c.StopBlinkShop();
				}
			}
			else
			{
				TrainingController.isNextStep = TrainingState.TapToShoot;
			}
		}
		if (CurHealth > 0f)
		{
			SetInApp();
			SetPause(false);
			if (Defs.isSoundFX)
			{
				NGUITools.PlaySound(clickShop);
			}
		}
	}

	public void PlayPortalSound()
	{
		if (Defs.isMulti)
		{
			if (Defs.isInet)
			{
				photonView.RPC("PlayPortalSoundRPC", PhotonTargets.All);
			}
			else
			{
				GetComponent<NetworkView>().RPC("PlayPortalSoundRPC", RPCMode.All);
			}
		}
		else
		{
			PlayPortalSoundRPC();
		}
	}

	[PunRPC]
	[RPC]
	public void PlayPortalSoundRPC()
	{
		if (Defs.isSoundFX && portalSound != null)
		{
			GetComponent<AudioSource>().PlayOneShot(portalSound);
		}
	}

	public void AddButtonHandlers()
	{
		PauseTapReceiver.PauseClicked += SwitchPause;
		ShopTapReceiver.ShopClicked += ShopPressed;
		RanksTapReceiver.RanksClicked += RanksPressed;
		TopPanelsTapReceiver.OnClicked += RanksPressed;
		ChatTapReceiver.ChatClicked += ShowChat;
		if (JoystickController.leftJoystick != null)
		{
			JoystickController.leftJoystick.SetJoystickActive(true);
		}
		if (JoystickController.leftTouchPad != null)
		{
			JoystickController.leftTouchPad.SetJoystickActive(true);
		}
	}

	public void RemoveButtonHandelrs()
	{
		PauseTapReceiver.PauseClicked -= SwitchPause;
		ShopTapReceiver.ShopClicked -= ShopPressed;
		RanksTapReceiver.RanksClicked -= RanksPressed;
		TopPanelsTapReceiver.OnClicked -= RanksPressed;
		ChatTapReceiver.ChatClicked -= ShowChat;
		if (JoystickController.leftJoystick != null)
		{
			JoystickController.leftJoystick.SetJoystickActive(false);
		}
		if (JoystickController.leftTouchPad != null)
		{
			JoystickController.leftTouchPad.SetJoystickActive(false);
		}
	}

	public void RanksPressed()
	{
		JoystickController.rightJoystick.jumpPressed = false;
		JoystickController.leftTouchPad.isJumpPressed = false;
		JoystickController.rightJoystick.Reset();
		RemoveButtonHandelrs();
		showRanks = true;
		networkStartTableNGUIController.winnerPanelCom1.SetActive(false);
		networkStartTableNGUIController.winnerPanelCom2.SetActive(false);
		networkStartTableNGUIController.ShowRanksTable();
		inGameGUI.gameObject.SetActive(false);
	}

	public void BackRanksPressed()
	{
		AddButtonHandlers();
		showRanks = false;
		if (inGameGUI != null && inGameGUI.interfacePanel != null)
		{
			inGameGUI.gameObject.SetActive(true);
		}
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	[PunRPC]
	[RPC]
	private void setIp(string _ip)
	{
		myIp = _ip;
	}

	private void CheckTimeCondition()
	{
		CampaignLevel campaignLevel = null;
		foreach (LevelBox campaignBox in LevelBox.campaignBoxes)
		{
			if (!campaignBox.name.Equals(CurrentCampaignGame.boXName))
			{
				continue;
			}
			foreach (CampaignLevel level in campaignBox.levels)
			{
				if (level.sceneName.Equals(CurrentCampaignGame.levelSceneName))
				{
					campaignLevel = level;
					break;
				}
			}
			break;
		}
		float timeToComplete = campaignLevel.timeToComplete;
		if (inGameTime >= timeToComplete)
		{
			CurrentCampaignGame.completeInTime = false;
		}
	}

	private IEnumerator GetHardwareKeysInput()
	{
		while (true)
		{
			bool androidBackPressed2 = false;
			if (true)
			{
				if (_escapePressed)
				{
					if (Application.isEditor)
					{
						Debug.Log("[Escape] presed in PlayerMoveC");
					}
					_escapePressed = false;
					_backWasPressed = true;
				}
				else
				{
					if (_backWasPressed)
					{
						androidBackPressed2 = true;
					}
					_backWasPressed = false;
				}
			}
			if (androidBackPressed2 && !isInappWinOpen)
			{
				androidBackPressed2 = false;
				if (inGameGUI == null || inGameGUI.pausePanel == null)
				{
					yield return null;
					continue;
				}
				if (inGameGUI.blockedCollider.activeSelf)
				{
					yield return null;
					continue;
				}
				PauseNGUIController pauseController = inGameGUI.pausePanel.GetComponent<PauseNGUIController>();
				if (pauseController == null)
				{
					yield return null;
					continue;
				}
				if (pauseController.SettingsJoysticksPanel != null && !pauseController.SettingsJoysticksPanel.activeInHierarchy)
				{
					SwitchPause();
				}
				else if (pauseController.settingsPanel != null)
				{
					pauseController.SettingsJoysticksPanel.SetActive(false);
					pauseController.settingsPanel.SetActive(true);
				}
			}
			yield return null;
		}
	}

	private void InitiailizeIcnreaseArmorEffectFlags()
	{
		BonusEffectForArmorWorksInThisMatch = EffectsController.IcnreaseEquippedArmorPercentage > 1f;
		ArmorBonusGiven = EffectsController.ArmorBonus > 0f;
	}

	private IEnumerator Start()
	{
		string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.Start()", GetType().Name);
		IEnumerator startSteps = StartSteps();
		int i = 0;
		while (true)
		{
			string callee = string.Format(CultureInfo.InvariantCulture, "Step {0}", i);
			ScopeLogger scopeLogger = new ScopeLogger(thisMethod, callee, Defs.IsDeveloperBuild);
			try
			{
				if (!startSteps.MoveNext())
				{
					break;
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
			yield return startSteps.Current;
			i++;
		}
	}

	private IEnumerator StartSteps()
	{
		string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.StartSteps()", GetType().Name);
		_bodyMaterial = playerBodyRenderer.material;
		playerBodyRenderer.sharedMaterial = _bodyMaterial;
		_mechMaterial = new Material(mechBodyRenderer.material);
		mechBodyRenderer.sharedMaterial = _mechMaterial;
		mechHandRenderer.sharedMaterial = _mechMaterial;
		_bearMaterial = new Material(mechBearBodyRenderer.material);
		mechBearBodyRenderer.sharedMaterial = _bearMaterial;
		mechBearHandRenderer.sharedMaterial = _bearMaterial;
		SetMaterialForArms();
		try
		{
			tierForKilledRate = ExpController.OurTierForAnyPlace() + 1;
			weKillForKillRate.Clear();
			weWereKilledForKillRate.Clear();
		}
		catch (Exception ex)
		{
			Exception e = ex;
			Debug.LogError("Exception in cleaning kill rate stats Player_move_c.Start(): " + e);
			if (weKillForKillRate != null)
			{
				weKillForKillRate.Clear();
			}
			if (weWereKilledForKillRate != null)
			{
				weWereKilledForKillRate.Clear();
			}
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			isImmortality = false;
			timerImmortality = 0f;
		}
		isDaterRegim = Defs.isDaterRegim;
		_killerInfo.Reset();
		isNeedTakePremiumAccountRewards = PremiumAccountController.Instance.isAccountActive;
		InitiailizeIcnreaseArmorEffectFlags();
		Initializer.players.Add(this);
		Initializer.playersObj.Add(myPlayerTransform.gameObject);
		if (!Defs.isMulti)
		{
			WeaponManager.sharedManager.myPlayerMoveC = this;
			WeaponManager.sharedManager.myPlayer = myPlayerTransform.gameObject;
		}
		AmmoBox.fontSize = Mathf.RoundToInt(18f * (float)Screen.width / 1024f);
		ScoreBox.fontSize = Mathf.RoundToInt((float)Screen.height * 0.035f);
		if (Defs.isFlag)
		{
			flag1 = Initializer.flag1;
			flag2 = Initializer.flag2;
		}
		timerRegenerationLiveZel = maxTimerRegenerationLiveZel;
		timerRegenerationLiveCape = maxTimerRegenerationLiveCape;
		timerRegenerationArmor = maxTimerRegenerationArmor;
		photonView = PhotonView.Get(this);
		if (isMulti)
		{
			if (!isInet)
			{
				isMine = GetComponent<NetworkView>().isMine;
			}
			else if (photonView == null)
			{
				Debug.Log("Player_move_c.Start():    photonView == null");
			}
			else
			{
				isMine = photonView.isMine;
			}
		}
		if (!isMulti || isMine)
		{
			if (_backSubscription != null)
			{
				_backSubscription.Dispose();
			}
			_backSubscription = BackSystem.Instance.Register(HandleEscape, "Player Move C");
		}
		if ((bool)photonView && photonView.isMine)
		{
			PhotonObjectCacher.AddObject(base.gameObject);
		}
		if (!isMulti || isMine)
		{
			if (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None)
			{
				if (!Defs.isDaterRegim && Storager.getInt("GrenadeID", false) <= 0)
				{
					Storager.setInt("GrenadeID", 1, false);
				}
				if (Defs.isDaterRegim && Storager.getInt("LikeID", false) <= 0)
				{
					Storager.setInt("LikeID", 1, false);
				}
			}
			EffectsController.SlowdownCoeff = 1f;
			ScopeLogger workingLogger = new ScopeLogger(thisMethod, "Resources.Load('InGameGui')", Defs.IsDeveloperBuild);
			UnityEngine.Object inGameGuiPrefab = Resources.Load("InGameGUI");
			workingLogger.Dispose();
			workingLogger = new ScopeLogger(thisMethod, "Instantiate(inGameGuiPrefab)", Defs.IsDeveloperBuild);
			GameObject inGameGuiInstance = (GameObject)UnityEngine.Object.Instantiate(inGameGuiPrefab, Vector3.up * 10000f, Quaternion.identity);
			workingLogger.Dispose();
			inGameGUI = inGameGuiInstance.GetComponent<InGameGUI>();
			SetGrenateFireEnabled();
			Defs.isJetpackEnabled = false;
			Defs.isTurretWeapon = false;
			oldKilledPlayerCharactersCount = (Storager.hasKey("KilledPlayerCharactersCount") ? Storager.getInt("KilledPlayerCharactersCount", false) : 0);
		}
		if (!isMulti)
		{
			_skin = SkinsController.currentSkinForPers;
			_skin.filterMode = FilterMode.Point;
			ShopNGUIController.sharedShop.onEquipSkinAction = delegate
			{
				UpdateSkin();
			};
		}
		if (!Defs.isMulti)
		{
			GameObject trainigControllerGo = GameObject.FindGameObjectWithTag("TrainingController");
			if (trainigControllerGo != null)
			{
				trainigController = trainigControllerGo.GetComponent<TrainingController>();
			}
		}
		expController = ExperienceController.sharedController;
		if (isMulti && isInet)
		{
			GameObject[] tables = GameObject.FindGameObjectsWithTag("NetworkTable");
			for (int j = 0; j < tables.Length; j++)
			{
				if (tables[j].GetComponent<PhotonView>().owner == base.transform.GetComponent<PhotonView>().owner)
				{
					myTable = tables[j];
					setMyTamble(myTable);
					break;
				}
			}
		}
		if (isMulti)
		{
			if (isInet)
			{
				myPlayerID = myPlayerTransform.GetComponent<PhotonView>().viewID;
			}
			else
			{
				myPlayerIDLocal = myPlayerTransform.GetComponent<NetworkView>().viewID;
			}
		}
		if (isMulti && !isMine)
		{
			base.transform.localPosition = new Vector3(0f, 0.4f, 0f);
		}
		if (!isMulti)
		{
			CurrentCampaignGame.ResetConditionParameters();
			CurrentCampaignGame._levelStartedAtTime = Time.time;
			ZombieCreator.BossKilled += CheckTimeCondition;
		}
		if (isMulti && isCompany && isMine)
		{
			countKillsCommandBlue = GlobalGameController.countKillsBlue;
			countKillsCommandRed = GlobalGameController.countKillsRed;
		}
		if (isMulti && isCOOP)
		{
			zombiManager = ZombiManager.sharedManager;
		}
		if (isMulti && isMine)
		{
			networkStartTableNGUIController = NetworkStartTableNGUIController.sharedController;
		}
		if (!isMulti || isMine)
		{
			InitPurchaseActions();
			ActivityIndicator.IsActiveIndicator = false;
		}
		if (!Defs.isMulti || isMine)
		{
			_inAppGameObject = GameObject.FindGameObjectWithTag("InAppGameObject");
			_listener = _inAppGameObject.GetComponent<StoreKitEventListener>();
		}
		if (!isMulti)
		{
			fpsPlayerBody.SetActive(false);
		}
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager();
		if (isMulti)
		{
			showGUI = isMine;
		}
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.AddPurchaseResponseListener(HandlePurchaseSuccessful);
		}
		else
		{
			GoogleIABManager.purchaseSucceededEvent += purchaseSuccessful;
		}
		if (!isMulti || isMine)
		{
			_player = myPlayerTransform.gameObject;
		}
		else
		{
			_player = null;
		}
		_weaponManager = WeaponManager.sharedManager;
		if (Defs.isMulti && ((!Defs.isInet && GetComponent<NetworkView>().isMine) || (Defs.isInet && photonView.isMine && PlayerPrefs.GetInt("StartAfterDisconnect") == 0)))
		{
			foreach (Weapon _w in _weaponManager.allAvailablePlayerWeapons)
			{
				_w.currentAmmoInClip = _w.weaponPrefab.GetComponent<WeaponSounds>().ammoInClip;
				_w.currentAmmoInBackpack = _w.weaponPrefab.GetComponent<WeaponSounds>().InitialAmmoWithEffectsApplied;
			}
		}
		if (!isMulti || isMine)
		{
			GameObject tmpDamage = Resources.Load("Damage") as GameObject;
			damage = UnityEngine.Object.Instantiate(tmpDamage);
			Color rgba = damage.GetComponent<GUITexture>().color;
			rgba.a = 0f;
			damage.GetComponent<GUITexture>().color = rgba;
		}
		if (!isMulti || isMine)
		{
			_pauser = GameObject.FindGameObjectWithTag("GameController").GetComponent<Pauser>();
			if (_pauser == null)
			{
				Debug.LogWarning("Start(): _pauser is null.");
			}
		}
		if (_singleOrMultiMine())
		{
			numberOfGrenadesOnStart.Value = ((!Defs.isHunger) ? ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0) ? Storager.getInt((!Defs.isDaterRegim) ? "GrenadeID" : "LikeID", false) : 0) : 0);
			numberOfGrenades.Value = numberOfGrenadesOnStart.Value;
			if (!isMulti)
			{
				indexWeapon = _weaponManager.CurrentWeaponIndex;
				ChangeWeaponReal(_weaponManager.CurrentWeaponIndex, false);
			}
			else
			{
				ChangeWeaponReal(_weaponManager.CurrentIndexOfLastUsedWeaponInPlayerWeapons(), false);
			}
			_weaponManager.myGun = base.gameObject;
			if (_weaponManager.currentWeaponSounds != null)
			{
				_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].layer = 1;
				_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().Stop();
			}
		}
		if (isMulti && isMine)
		{
			string _nameFilter = FilterBadWorld.FilterString(ProfileController.GetPlayerNameOrDefault());
			if (isInet)
			{
				photonView.RPC("SetNickName", PhotonTargets.AllBuffered, _nameFilter);
			}
			else
			{
				GetComponent<NetworkView>().RPC("SetNickName", RPCMode.AllBuffered, _nameFilter);
			}
		}
		CurrentBaseArmor = EffectsController.ArmorBonus;
		CurHealth = MaxHealth;
		if (!isMulti || isMine)
		{
			Wear.RenewCurArmor(TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier));
			string armorEquipped = Storager.getString(Defs.ArmorEquppedSN, false);
			if (_actionsForPurchasedItems.ContainsKey(armorEquipped))
			{
				_actionsForPurchasedItems[armorEquipped](armorEquipped);
				Storager.setString(Defs.ArmorEquppedSN, Defs.ArmorNoneEqupped, false);
			}
			if (Storager.getInt(Defs.AmmoBoughtSN, false) == 1)
			{
				if (_actionsForPurchasedItems.ContainsKey("bigammopack"))
				{
					_actionsForPurchasedItems["bigammopack"]("bigammopack");
				}
				Storager.setInt(Defs.AmmoBoughtSN, 0, false);
			}
		}
		if (_singleOrMultiMine())
		{
			StartCoroutine(GetHardwareKeysInput());
			SetLayerRecursively(mechGunAnimation.gameObject, 9);
			if (false)
			{
				UnityEngine.Object videoRecordingPrefab = Resources.Load("VideoRecordingPanel");
				GameObject videoRecordingPanel = UnityEngine.Object.Instantiate(videoRecordingPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				if (videoRecordingPrefab == null)
				{
					Debug.LogError("videoRecordingPrefab == null");
				}
				if (videoRecordingPanel != null)
				{
					videoRecordingPanel.transform.parent = inGameGUI.interfacePanel.transform;
					videoRecordingPanel.AddComponent<VideoRecordingController>();
				}
				else
				{
					Debug.LogError("videoRecordingPanel != null");
				}
			}
			inGameGUI.health = () => (!isMechActive) ? CurHealth : liveMech;
			inGameGUI.armor = () => curArmor;
			inGameGUI.killsToMaxKills = () => myScoreController.currentScore.ToString();
			inGameGUI.timeLeft = delegate
			{
				float num3;
				if (isHunger)
				{
					if (hungerGameController == null)
					{
						hungerGameController = HungerGameController.Instance;
					}
					num3 = ((!(hungerGameController != null)) ? 0f : hungerGameController.gameTimer);
				}
				else
				{
					num3 = (float)TimeGameController.sharedController.timerToEndMatch;
				}
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				return Mathf.FloorToInt(num3 / 60f) + ":" + ((Mathf.FloorToInt(num3 - (float)(Mathf.FloorToInt(num3 / 60f) * 60)) >= 10) ? string.Empty : "0") + Mathf.FloorToInt(num3 - (float)(Mathf.FloorToInt(num3 / 60f) * 60));
			};
			AddButtonHandlers();
			ShopNGUIController.sharedShop.SetGearCatEnabled(TrainingController.TrainingCompleted);
			ShopNGUIController.sharedShop.buyAction = PurchaseSuccessful;
			ShopNGUIController.sharedShop.equipAction = delegate
			{
				ChangeWeaponReal(_weaponManager.CurrentWeaponIndex, false);
				if (WeaponManager.sharedManager != null)
				{
					WeaponManager.sharedManager.ReloadWeaponFromSet(WeaponManager.sharedManager.CurrentWeaponIndex);
				}
			};
			ShopNGUIController.sharedShop.activatePotionAction = delegate(string potion)
			{
				Storager.setInt(potion, Storager.getInt(potion, false) - 1, false);
				PotionsController.sharedController.ActivatePotion(potion, this, new Dictionary<string, object>());
			};
			ShopNGUIController.sharedShop.resumeAction = delegate
			{
				if (base.gameObject != null)
				{
					SetInApp();
					if (inAppOpenedFromPause)
					{
						inAppOpenedFromPause = false;
						if (inGameGUI != null && inGameGUI.pausePanel != null)
						{
							inGameGUI.pausePanel.SetActive(true);
							PauseNGUIController component = inGameGUI.pausePanel.GetComponent<PauseNGUIController>();
							if (component != null && component.settingsPanel != null)
							{
								component.settingsPanel.SetActive(true);
							}
						}
						ExperienceController.sharedController.isShowRanks = true;
					}
					else
					{
						SetPause();
					}
				}
				else
				{
					ShopNGUIController.GuiActive = false;
				}
			};
			ShopNGUIController.sharedShop.wearEquipAction = delegate(ShopNGUIController.CategoryNames category, string unequippedItem, string equippedItem)
			{
				if (!BonusEffectForArmorWorksInThisMatch)
				{
					float num = Wear.MaxArmorForItem(FriendsController.sharedController.armorName ?? string.Empty, TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier)) * (EffectsController.IcnreaseEquippedArmorPercentage - 1f);
					float num2 = Wear.MaxArmorForItem(FriendsController.sharedController.hatName ?? string.Empty, TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier)) * (EffectsController.IcnreaseEquippedArmorPercentage - 1f);
					BonusEffectForArmorWorksInThisMatch = (double)(num + num2) > 0.001;
					AddArmor(num + num2);
				}
				if (!ArmorBonusGiven)
				{
					ArmorBonusGiven = (double)EffectsController.ArmorBonus > 0.001;
					CurrentBaseArmor += EffectsController.ArmorBonus;
				}
				if (category == ShopNGUIController.CategoryNames.CapesCategory)
				{
					mySkinName.SetCape();
				}
				if (category == ShopNGUIController.CategoryNames.MaskCategory)
				{
					mySkinName.SetMask();
				}
				if (category == ShopNGUIController.CategoryNames.HatsCategory)
				{
					mySkinName.SetHat();
					if (equippedItem != null && unequippedItem != null && (!Wear.NonArmorHat(equippedItem) || !Wear.NonArmorHat(unequippedItem)))
					{
						CurrentBaseArmor = 0f;
					}
				}
				if (category == ShopNGUIController.CategoryNames.BootsCategory)
				{
					mySkinName.SetBoots();
				}
				if (category == ShopNGUIController.CategoryNames.ArmorCategory)
				{
					mySkinName.SetArmor();
					respawnedForGUI = true;
					CurrentBaseArmor = 0f;
				}
			};
			ShopNGUIController.sharedShop.wearUnequipAction = delegate(ShopNGUIController.CategoryNames category, string unequippedItem)
			{
				if (category == ShopNGUIController.CategoryNames.CapesCategory)
				{
					mySkinName.SetCape();
				}
				if (category == ShopNGUIController.CategoryNames.MaskCategory)
				{
					mySkinName.SetMask();
				}
				if (category == ShopNGUIController.CategoryNames.HatsCategory)
				{
					mySkinName.SetHat();
					if (!Wear.NonArmorHat(unequippedItem))
					{
						CurrentBaseArmor = 0f;
					}
				}
				if (category == ShopNGUIController.CategoryNames.BootsCategory)
				{
					mySkinName.SetBoots();
				}
				if (category == ShopNGUIController.CategoryNames.ArmorCategory)
				{
					mySkinName.SetArmor();
					CurrentBaseArmor = 0f;
				}
			};
			ShopNGUIController.ShowArmorChanged += HandleShowArmorChanged;
		}
		if (PlayerPrefs.GetInt("StartAfterDisconnect") == 1 && Defs.isMulti && Defs.isInet && photonView.isMine)
		{
			countKills = GlobalGameController.CountKills;
			myScoreController.currentScore = Mathf.Max(0, GlobalGameController.Score);
			if (countKills < 0)
			{
				countKills = 0;
			}
			if (GlobalGameController.healthMyPlayer > 0f || Defs.isHunger)
			{
				CurHealth = GlobalGameController.healthMyPlayer;
				myPlayerTransform.position = GlobalGameController.posMyPlayer;
				myPlayerTransform.rotation = GlobalGameController.rotMyPlayer;
				curArmor = GlobalGameController.armorMyPlayer;
			}
			RatingSystem.instance.BackupLastRatingTake();
			PlayerPrefs.SetInt("StartAfterDisconnect", 0);
		}
		yield return null;
		if (_singleOrMultiMine())
		{
			PotionsController.sharedController.ReactivatePotions(this, new Dictionary<string, object>());
			string curHat = Storager.getString(Defs.HatEquppedSN, false);
			if (!curHat.Equals(Defs.HatNoneEqupped) && Wear.hatsMethods.ContainsKey(curHat))
			{
				Wear.hatsMethods[curHat].Key(this, new Dictionary<string, object>());
			}
			string curCape = Storager.getString(Defs.CapeEquppedSN, false);
			if (!curCape.Equals(Defs.CapeNoneEqupped) && Wear.capesMethods.ContainsKey(curCape))
			{
				Wear.capesMethods[curCape].Key(this, new Dictionary<string, object>());
			}
			string curBoots = Storager.getString(Defs.BootsEquppedSN, false);
			if (!curBoots.Equals(Defs.BootsNoneEqupped) && Wear.bootsMethods.ContainsKey(curBoots))
			{
				Wear.bootsMethods[curBoots].Key(this, new Dictionary<string, object>());
			}
			string curArmor_ = Storager.getString(Defs.ArmorNewEquppedSN, false);
			if (!curArmor_.Equals(Defs.ArmorNewNoneEqupped) && Wear.armorMethods.ContainsKey(curArmor_))
			{
				Wear.armorMethods[curArmor_].Key(this, new Dictionary<string, object>());
			}
			if (JoystickController.leftJoystick != null)
			{
				JoystickController.leftJoystick.SetJoystickActive(true);
			}
			if (JoystickController.rightJoystick != null)
			{
				JoystickController.rightJoystick.MakeActive();
			}
			if (JoystickController.leftTouchPad != null)
			{
				JoystickController.leftTouchPad.SetJoystickActive(true);
			}
		}
		if (isMulti && myTable != null)
		{
			_skin = myNetworkStartTable.mySkin;
			if (_skin != null)
			{
				SetTextureForBodyPlayer(_skin);
			}
		}
		if (isMine && !TrainingController.TrainingCompleted)
		{
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Play_Deathmatch);
		}
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			Initializer.players[i].SetNicklabelVisible();
		}
	}

	private void ActualizeNumberOfGrenades()
	{
		if (!Defs.isHunger && !SceneLoader.ActiveSceneName.Equals(Defs.TrainingSceneName) && numberOfGrenades.Value != numberOfGrenadesOnStart.Value)
		{
			Storager.setInt((!Defs.isDaterRegim) ? "GrenadeID" : "LikeID", numberOfGrenades.Value, false);
			numberOfGrenadesOnStart.Value = numberOfGrenades.Value;
		}
	}

	public void OnApplicationPause(bool pause)
	{
		if (!_singleOrMultiMine())
		{
			return;
		}
		if (pause)
		{
			ActualizeNumberOfGrenades();
			if (Application.platform == RuntimePlatform.IPhonePlayer && liveTime > 90f)
			{
				pausedRating = true;
				myNetworkStartTable.CalculateMatchRating(true);
			}
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer && pausedRating)
		{
			pausedRating = false;
			RatingSystem.instance.BackupLastRatingTake();
		}
	}

	private void HandleShowArmorChanged()
	{
		mySkinName.SetArmor();
		mySkinName.SetHat();
	}

	public void UpdateSkin()
	{
		if (!isMulti)
		{
			_skin = SkinsController.currentSkinForPers;
			_skin.filterMode = FilterMode.Point;
			SetTextureForBodyPlayer(_skin);
		}
	}

	public void SetIDMyTable(string _id)
	{
		myTableId = _id;
		Invoke("SetIDMyTableInvoke", 0.1f);
	}

	[Obfuscation(Exclude = true)]
	private void SetIDMyTableInvoke()
	{
		GetComponent<NetworkView>().RPC("SetIDMyTableRPC", RPCMode.AllBuffered, myTableId);
	}

	[RPC]
	[PunRPC]
	private void SetIDMyTableRPC(string _id)
	{
		myTableId = _id;
		GameObject[] array = GameObject.FindGameObjectsWithTag("NetworkTable");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.GetComponent<NetworkView>().viewID.ToString().Equals(_id))
			{
				myTable = gameObject;
				setMyTamble(myTable);
			}
		}
	}

	[PunRPC]
	[RPC]
	public void SetNickName(string _nickName)
	{
		photonView = PhotonView.Get(this);
		mySkinName.NickName = _nickName;
		if (!isMine)
		{
			nickLabel.gameObject.SetActive(true);
			nickLabel.text = _nickName;
		}
	}

	public bool _singleOrMultiMine()
	{
		return !isMulti || isMine;
	}

	private void OnDestroy()
	{
		if (isMine && Defs.isMulti && Defs.isInet && FriendsController.useBuffSystem)
		{
			BuffSystem.instance.PlayerLeaved();
		}
		_bodyMaterial = null;
		_mechMaterial = null;
		_bearMaterial = null;
		Initializer.players.Remove(this);
		Initializer.playersObj.Remove(myPlayerTransform.gameObject);
		if (Defs.isMulti && Defs.isInet)
		{
			BonusController.sharedController.lowLevelPlayers.Remove(photonView.ownerId);
		}
		if (Initializer.bluePlayers.Contains(this))
		{
			Initializer.bluePlayers.Remove(this);
		}
		if (Initializer.redPlayers.Contains(this))
		{
			Initializer.redPlayers.Remove(this);
		}
		if (Defs.isCapturePoints && CapturePointController.sharedController != null)
		{
			for (int i = 0; i < CapturePointController.sharedController.basePointControllers.Length; i++)
			{
				if (CapturePointController.sharedController.basePointControllers[i].capturePlayers.Contains(this))
				{
					CapturePointController.sharedController.basePointControllers[i].capturePlayers.Remove(this);
				}
			}
		}
		if (_weaponPopularityCacheIsDirty)
		{
			Statistics.Instance.SaveWeaponPopularity();
			_weaponPopularityCacheIsDirty = false;
		}
		if (!isMulti)
		{
			ShopNGUIController.sharedShop.onEquipSkinAction = null;
		}
		if (_singleOrMultiMine())
		{
			ActualizeNumberOfGrenades();
			SaveKillRate();
			if (networkStartTableNGUIController != null)
			{
				networkStartTableNGUIController.ranksInterface.SetActive(false);
			}
			if (ShopNGUIController.sharedShop != null)
			{
				ShopNGUIController.sharedShop.resumeAction = null;
			}
			if ((bool)inGameGUI && (bool)inGameGUI.gameObject)
			{
				if (!isHunger && !Defs.isRegimVidosDebug)
				{
					UnityEngine.Object.Destroy(inGameGUI.gameObject);
				}
				else
				{
					inGameGUI.topAnchor.SetActive(false);
					inGameGUI.leftAnchor.SetActive(false);
					inGameGUI.rightAnchor.SetActive(false);
					inGameGUI.joystickContainer.SetActive(false);
					inGameGUI.bottomAnchor.SetActive(false);
					inGameGUI.fastShopPanel.SetActive(false);
					inGameGUI.swipeWeaponPanel.gameObject.SetActive(false);
					inGameGUI.turretPanel.SetActive(false);
					for (int j = 0; j < 3; j++)
					{
						if (inGameGUI.messageAddScore[j].gameObject.activeSelf)
						{
							inGameGUI.messageAddScore[j].gameObject.SetActive(false);
						}
					}
				}
			}
			if (ChatViewrController.sharedController != null)
			{
				ChatViewrController.sharedController.CloseChat(true);
			}
			if (coinsShop.thisScript != null && coinsShop.thisScript.enabled)
			{
				coinsShop.ExitFromShop(false);
			}
			coinsPlashka.hidePlashka();
		}
		if (isMulti && isMine && CameraSceneController.sharedController != null)
		{
			CameraSceneController.sharedController.SetTargetKillCam();
		}
		if (!isMulti || isMine)
		{
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				AmazonIapV2Impl.Instance.RemovePurchaseResponseListener(HandlePurchaseSuccessful);
			}
			else
			{
				GoogleIABManager.purchaseSucceededEvent -= purchaseSuccessful;
			}
			if (Defs.isTurretWeapon && currentTurret != null)
			{
				if (Defs.isMulti)
				{
					if (Defs.isInet)
					{
						PhotonNetwork.Destroy(currentTurret);
					}
					else
					{
						Network.RemoveRPCs(currentTurret.GetComponent<NetworkView>().viewID);
						Network.Destroy(currentTurret);
					}
				}
				else
				{
					UnityEngine.Object.Destroy(currentTurret);
				}
			}
		}
		if (_singleOrMultiMine() || (_weaponManager != null && _weaponManager.myPlayer == myPlayerTransform.gameObject))
		{
			if (_pauser != null && (bool)_pauser && _pauser.paused)
			{
				_pauser.paused = !_pauser.paused;
				Time.timeScale = 1f;
				AddButtonHandlers();
			}
			GameObject gameObject = GameObject.FindGameObjectWithTag("DamageFrame");
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			RemoveButtonHandelrs();
			ShopNGUIController.sharedShop.buyAction = null;
			ShopNGUIController.sharedShop.equipAction = null;
			ShopNGUIController.sharedShop.activatePotionAction = null;
			ShopNGUIController.sharedShop.resumeAction = null;
			ShopNGUIController.sharedShop.wearEquipAction = null;
			ShopNGUIController.sharedShop.wearUnequipAction = null;
			ZombieCreator.BossKilled -= CheckTimeCondition;
			ShopNGUIController.ShowArmorChanged -= HandleShowArmorChanged;
		}
		if (isMulti && isMine)
		{
			ProfileController.ResaveStatisticToKeychain();
		}
		PhotonObjectCacher.RemoveObject(base.gameObject);
		if (Defs.isMulti && Defs.isCOOP)
		{
			int @int = Storager.getInt(Defs.COOPScore, false);
			int num = ((myNetworkStartTable.score != -1) ? myNetworkStartTable.score : myNetworkStartTable.scoreOld);
			if (num > @int)
			{
				Storager.setInt(Defs.COOPScore, num, false);
				FriendsController.sharedController.coopScore = num;
				FriendsController.sharedController.SendOurData();
			}
		}
	}

	public bool HasFreezerFireSubscr()
	{
		return this.FreezerFired != null;
	}

	private void _SetGunFlashActive(bool state)
	{
		WeaponSounds weaponSounds = ((!isMechActive) ? _weaponManager.currentWeaponSounds : mechWeaponSounds);
		if (weaponSounds.isDoubleShot && !_weaponManager.currentWeaponSounds.isMelee)
		{
			weaponSounds.gunFlashDouble[numShootInDoubleShot - 1].GetChild(0).gameObject.SetActive(state);
			if (state)
			{
				return;
			}
		}
		if (GunFlash != null && !_weaponManager.currentWeaponSounds.isMelee && (!isZooming || (isZooming && !state)))
		{
			WeaponManager.SetGunFlashActive(GunFlash.gameObject, state);
		}
	}

	public void setInString(string nick)
	{
		if (!(_weaponManager == null) && !(_weaponManager.myPlayer == null))
		{
			_weaponManager.myPlayerMoveC.AddSystemMessage(string.Format("{0} {1}", nick, LocalizationStore.Get("Key_0995")));
		}
	}

	public void setOutString(string nick)
	{
		if (!(_weaponManager == null) && !(_weaponManager.myPlayer == null))
		{
			_weaponManager.myPlayerMoveC.AddSystemMessage(string.Format("{0} {1}", nick, LocalizationStore.Get("Key_0996")));
		}
	}

	public void AddSystemMessage(string _nick1, string _message2, string _nick2, string _message = null)
	{
		killedSpisok[2][0] = killedSpisok[1][0];
		killedSpisok[2][1] = killedSpisok[1][1];
		killedSpisok[2][2] = killedSpisok[1][2];
		killedSpisok[2][3] = killedSpisok[1][3];
		killedSpisok[1][0] = killedSpisok[0][0];
		killedSpisok[1][1] = killedSpisok[0][1];
		killedSpisok[1][2] = killedSpisok[0][2];
		killedSpisok[1][3] = killedSpisok[0][3];
		killedSpisok[0][0] = _nick1;
		killedSpisok[0][1] = _message2;
		killedSpisok[0][2] = _nick2;
		killedSpisok[0][3] = _message;
		timerShow[2] = timerShow[1];
		timerShow[1] = timerShow[0];
		timerShow[0] = 3f;
	}

	public void AddSystemMessage(string nick1, int _typeKills)
	{
		AddSystemMessage(nick1, iconShotName[_typeKills], string.Empty);
	}

	public void AddSystemMessage(string nick1, int _typeKills, string nick2, string iconWeapon = null)
	{
		AddSystemMessage(nick1, iconShotName[_typeKills], nick2, iconWeapon);
	}

	public void AddSystemMessage(string _message)
	{
		AddSystemMessage(_message, string.Empty, string.Empty);
	}

	[PunRPC]
	[RPC]
	public void SendSystemMessegeFromFlagDroppedRPC(bool isBlueFlag, string nick)
	{
		if (WeaponManager.sharedManager.myPlayer != null)
		{
			if ((isBlueFlag && WeaponManager.sharedManager.myPlayerMoveC.myCommand == 1) || (!isBlueFlag && WeaponManager.sharedManager.myPlayerMoveC.myCommand == 2))
			{
				WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(string.Format("{0} {1}", nick, LocalizationStore.Get("Key_1798")));
			}
			else
			{
				WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(string.Format("{0} {1}", nick, LocalizationStore.Get("Key_1799")));
			}
		}
	}

	public void SendSystemMessegeFromFlagReturned(bool isBlueFlag)
	{
		photonView.RPC("SendSystemMessegeFromFlagReturnedRPC", PhotonTargets.All, isBlueFlag);
	}

	[PunRPC]
	[RPC]
	public void SendSystemMessegeFromFlagReturnedRPC(bool isBlueFlag)
	{
		if (WeaponManager.sharedManager.myPlayer != null)
		{
			if ((isBlueFlag && WeaponManager.sharedManager.myPlayerMoveC.myCommand == 1) || (!isBlueFlag && WeaponManager.sharedManager.myPlayerMoveC.myCommand == 2))
			{
				WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1800"));
			}
			else
			{
				WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1801"));
			}
		}
	}

	[PunRPC]
	[RPC]
	public void SendSystemMessegeFromFlagCaptureRPC(bool isBlueFlag, string nick)
	{
		if (!(WeaponManager.sharedManager.myPlayer != null))
		{
			return;
		}
		bool flag = WeaponManager.sharedManager.myPlayerMoveC.myCommand == 1;
		if (flag == isBlueFlag)
		{
			WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(string.Format("{0} {1}", nick, LocalizationStore.Get("Key_1001")));
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot(flagLostClip);
			}
		}
		else
		{
			WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1002"));
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot(flagGetClip);
			}
		}
	}

	[PunRPC]
	[RPC]
	public void SendSystemMessegeFromFlagAddScoreRPC(bool isCommandBlue, string nick)
	{
		if (WeaponManager.sharedManager.myPlayer != null)
		{
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot((isCommandBlue != (_weaponManager.myPlayerMoveC.myCommand == 1)) ? flagScoreEnemyClip : flagScoreMyCommandClip);
			}
			isCaptureFlag = false;
			WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(nick, 5);
		}
	}

	public void SendHouseKeeperEvent()
	{
		countHouseKeeperEvent++;
		if (countHouseKeeperEvent == 1)
		{
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.houseKeeperPoint);
		}
		if (countHouseKeeperEvent == 3)
		{
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.defenderPoint);
		}
		if (countHouseKeeperEvent == 5)
		{
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.guardianPoint);
		}
		if (countHouseKeeperEvent == 10)
		{
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.oneManArmyPoint);
		}
	}

	private void ResetHouseKeeperEvent()
	{
		countHouseKeeperEvent = 0;
	}

	public void ShowBonuseParticle(TypeBonuses _type)
	{
		if (Defs.isMulti)
		{
			if (Defs.isInet)
			{
				photonView.RPC("ShowBonuseParticleRPC", PhotonTargets.Others, (int)_type);
			}
			else
			{
				GetComponent<NetworkView>().RPC("ShowBonuseParticleRPC", RPCMode.Others, (int)_type);
			}
		}
	}

	[PunRPC]
	[RPC]
	public void ShowBonuseParticleRPC(int _type)
	{
		if (bonusesParticles.Length >= _type)
		{
			bonusesParticles[_type].ShowParticle();
		}
	}

	public void SetTextureForBodyPlayer(Texture needTx)
	{
		SetMaterialForArms();
		if (_bodyMaterial != null)
		{
			_bodyMaterial.mainTexture = needTx;
		}
	}

	public void SetTextureForActiveMesh(Texture needTx)
	{
		SetMaterialForArms();
		if (mainDamageMaterial != null)
		{
			mainDamageMaterial.mainTexture = needTx;
		}
	}

	private void SetMaterialForArms()
	{
		if (myCurrentWeaponSounds != null && !isBearActive)
		{
			myCurrentWeaponSounds._innerPars.SetMaterialForArms(_bodyMaterial);
		}
	}

	public static void SetTextureRecursivelyFrom(GameObject obj, Texture txt, GameObject[] stopObjs)
	{
		Transform transform = obj.transform;
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			bool flag = false;
			foreach (GameObject o in stopObjs)
			{
				if (child.gameObject.Equals(o))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			if ((bool)child.gameObject.GetComponent<Renderer>() && (bool)child.gameObject.GetComponent<Renderer>().material)
			{
				child.gameObject.GetComponent<Renderer>().material.mainTexture = txt;
			}
			flag = false;
			foreach (GameObject o2 in stopObjs)
			{
				if (child.gameObject.Equals(o2))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				SetTextureRecursivelyFrom(child.gameObject, txt, stopObjs);
			}
		}
	}

	private IEnumerator Flash(GameObject _obj)
	{
		if (!isDaterRegim)
		{
			SetTextureForBodyPlayer(hitTexture);
			if (mainDamageMaterial != null)
			{
				mainDamageMaterial.SetColor("_ColorRili", new Color(1f, 0f, 0f, 1f));
			}
			yield return new WaitForSeconds(0.125f);
			SetTextureForBodyPlayer(_skin);
			if (mainDamageMaterial != null)
			{
				mainDamageMaterial.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
			}
		}
	}

	public static GameObject[] GetStopObjFromPlayer(GameObject _obj)
	{
		List<GameObject> list = new List<GameObject>();
		Transform transform = _obj.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!child.gameObject.name.Equals("GameObject") || child.transform.childCount <= 0)
			{
				continue;
			}
			for (int j = 0; j < child.transform.childCount; j++)
			{
				GameObject gameObject = null;
				GameObject gameObject2 = null;
				WeaponSounds component = child.transform.GetChild(j).gameObject.GetComponent<WeaponSounds>();
				gameObject = component.bonusPrefab;
				if (!component.isMelee)
				{
					gameObject2 = child.transform.GetChild(j).Find("BulletSpawnPoint").gameObject;
				}
				if (component.noFillObjects != null && component.noFillObjects.Length > 0)
				{
					for (int k = 0; k < component.noFillObjects.Length; k++)
					{
						list.Add(component.noFillObjects[k]);
					}
				}
				if (gameObject != null)
				{
					list.Add(gameObject);
				}
				if (gameObject2 != null)
				{
					list.Add(gameObject2);
				}
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
				if (component.animationObject != null && component.animationObject.GetComponent<InnerWeaponPars>() != null && component.animationObject.GetComponent<InnerWeaponPars>().particlePoint != null)
				{
					list.Add(component.animationObject.GetComponent<InnerWeaponPars>().particlePoint);
				}
				List<GameObject> listWeaponAnimEffects = component.GetListWeaponAnimEffects();
				if (listWeaponAnimEffects != null)
				{
					list.AddRange(listWeaponAnimEffects);
				}
			}
			break;
		}
		if (_obj != null && _obj.GetComponent<SkinName>() != null)
		{
			SkinName component2 = _obj.GetComponent<SkinName>();
			list.Add(component2.capesPoint);
			list.Add(component2.hatsPoint);
			list.Add(component2.maskPoint);
			list.Add(component2.bootsPoint);
			list.Add(component2.armorPoint);
			list.Add(component2.onGroundEffectsPoint.gameObject);
			if (component2.playerMoveC != null)
			{
				list.Add(component2.playerMoveC.flagPoint);
				list.Add(component2.playerMoveC.invisibleParticle);
				list.Add(component2.playerMoveC.jetPackPoint);
				list.Add(component2.playerMoveC.jetPackPointMech);
				list.Add(component2.playerMoveC.wingsPoint);
				list.Add(component2.playerMoveC.wingsPointBear);
				list.Add(component2.playerMoveC.turretPoint);
				list.Add(component2.playerMoveC.mechPoint);
				list.Add(component2.playerMoveC.mechBearPoint);
				list.Add(component2.playerMoveC.mechExplossion);
				list.Add(component2.playerMoveC.bearExplosion);
				if (Defs.isDaterRegim && component2.playerMoveC.myCurrentWeaponSounds != null)
				{
					list.Add(component2.playerMoveC.myCurrentWeaponSounds.BearWeaponObject);
				}
				list.Add(component2.playerMoveC.particleBonusesPoint);
				component2.playerMoveC.arrowToPortalPoint.Do(list.Add);
			}
		}
		else
		{
			Debug.Log("Condition failed: “_obj != null && _obj.GetComponent<SkinName>() != null”");
		}
		return list.ToArray();
	}

	private IEnumerator RunOnGroundEffectCoroutine(string name, float tm)
	{
		yield return new WaitForSeconds(tm);
		RunOnGroundEffect(name);
	}

	private void FixedUpdate()
	{
		if (rocketToLaunch != null)
		{
			rocketToLaunch.GetComponent<Rigidbody>().AddForce(190f * rocketToLaunch.transform.forward);
			rocketToLaunch = null;
		}
		if (!isMulti || isMine)
		{
			ShopNGUIController.sharedShop.SetGearCatEnabled(true);
			Debug.Log((JoystickController.rightJoystick == null).ToString());
			if (((JoystickController.rightJoystick.jumpPressed || JoystickController.leftTouchPad.isJumpPressed) && Defs.isJetpackEnabled) != isJumpPresedOld && (Defs.isJetpackEnabled || isJumpPresedOld))
			{
				SetJetpackParticleEnabled((JoystickController.rightJoystick.jumpPressed || JoystickController.leftTouchPad.isJumpPressed) && Defs.isJetpackEnabled);
				isJumpPresedOld = (JoystickController.rightJoystick.jumpPressed || JoystickController.leftTouchPad.isJumpPressed) && Defs.isJetpackEnabled;
			}
		}
		if (isMulti && isMine && !(Camera.main == null))
		{
		}
	}

	public static int TierOfCurrentRoom()
	{
		if (PhotonNetwork.room != null && PhotonNetwork.room.customProperties.ContainsKey("tier"))
		{
			return (int)PhotonNetwork.room.customProperties["tier"];
		}
		return ExpController.Instance.OurTier;
	}

	private int TierOrRoomTier(int tier)
	{
		if (!roomTierInitialized)
		{
			roomTierInitialized = true;
			roomTier = TierOfCurrentRoom();
		}
		return Math.Min(tier, roomTier);
	}

	private IEnumerator Fade(float start, float end, float length, GameObject currentObject)
	{
		if (currentObject == null)
		{
			Debug.LogWarningFormat("{0}: currentObject == null", GetType().Name);
			yield break;
		}
		GUITexture texture = currentObject.GetComponent<GUITexture>();
		for (float i = 0f; i < 1f; i += Time.deltaTime / length)
		{
			if (texture == null)
			{
				Debug.LogWarningFormat("{0}: texture == null", GetType().Name);
				break;
			}
			Color rgba = texture.color;
			rgba.a = Mathf.Lerp(start, end, i);
			texture.color = rgba;
			yield return 0;
			if (texture == null)
			{
				Debug.LogWarningFormat("{0}: texture == null", GetType().Name);
				break;
			}
			Color rgba_ = texture.color;
			rgba_.a = end;
			texture.color = rgba_;
		}
	}

	private IEnumerator SetCanReceiveSwipes()
	{
		yield return new WaitForSeconds(0.1f);
		canReceiveSwipes = true;
	}

	[Obfuscation(Exclude = true)]
	private void setisDeadFrameFalse()
	{
		isDeadFrame = false;
	}

	private void UpdateImmortalityAlpColor(float _alpha)
	{
		if (Mathf.Abs(_alpha - oldAlphaImmortality) < 0.001f)
		{
			return;
		}
		oldAlphaImmortality = _alpha;
		if (myCurrentWeaponSounds != null)
		{
			playerBodyRenderer.material.SetColor("_ColorRili", new Color(1f, 1f, 1f, _alpha));
			Shader shader = Shader.Find("Mobile/Diffuse-Color");
			if (shader != null && myCurrentWeaponSounds.bonusPrefab != null && myCurrentWeaponSounds.bonusPrefab.transform.parent != null)
			{
				myCurrentWeaponSounds.bonusPrefab.transform.parent.GetComponent<Renderer>().material.shader = shader;
				myCurrentWeaponSounds.bonusPrefab.transform.parent.GetComponent<Renderer>().material.SetColor("_ColorRili", new Color(1f, 1f, 1f, _alpha));
			}
		}
	}

	private void Update()
	{
		liveTime += Time.deltaTime;
		if (_timerDelayInShootingBurst > 0f)
		{
			_timerDelayInShootingBurst -= Time.deltaTime;
		}
		UpdateHealth();
		UpdateNickLabelColor();
		if (timerUpdatePointAutoAi > 0f)
		{
			timerUpdatePointAutoAi -= Time.deltaTime;
		}
		if ((!isMulti || isMine) && _timeOfSlowdown > 0f)
		{
			_timeOfSlowdown -= Time.deltaTime;
			if (_timeOfSlowdown <= 0f)
			{
				EffectsController.SlowdownCoeff = 1f;
			}
		}
		if (!isMulti || isMine)
		{
			Defs.isZooming = isZooming;
		}
		if (!isKilled && timerImmortality > 0f)
		{
			timerImmortality -= Time.deltaTime;
			if (timerImmortality <= 0f)
			{
				isImmortality = false;
			}
		}
		if (!isInvisible)
		{
			if (isImmortality)
			{
				float num = 1f;
				timerImmortalityForAlpha += Time.deltaTime;
				float num2 = 2f * (timerImmortalityForAlpha - Mathf.Floor(timerImmortalityForAlpha / num) * num) / num;
				if (num2 > 1f)
				{
					num2 = 2f - num2;
				}
				UpdateImmortalityAlpColor(0.5f + num2 * 0.4f);
			}
			else
			{
				UpdateImmortalityAlpColor(1f);
			}
		}
		if (isMulti && isMine)
		{
			if ((isCompany || Defs.isFlag) && myCommand == 0 && myTable != null)
			{
				myCommand = myNetworkStartTable.myCommand;
			}
			if (Defs.isFlag && myBaza == null && myCommand != 0)
			{
				if (myCommand == 1)
				{
					myBaza = GameObject.FindGameObjectWithTag("BazaZoneCommand1");
				}
				else
				{
					myBaza = GameObject.FindGameObjectWithTag("BazaZoneCommand2");
				}
			}
			if (Defs.isFlag && (myFlag == null || enemyFlag == null) && myCommand != 0)
			{
				myFlag = ((myCommand != 1) ? flag2 : flag1);
				enemyFlag = ((myCommand != 1) ? flag1 : flag2);
			}
			if (Defs.isFlag && myFlag != null && enemyFlag != null)
			{
				if (!myFlag.isCapture && !myFlag.isBaza && Vector3.SqrMagnitude(myPlayerTransform.position - myFlag.transform.position) < 2.25f)
				{
					photonView.RPC("SendSystemMessegeFromFlagReturnedRPC", PhotonTargets.All, myFlag.isBlue);
					myFlag.GoBaza();
				}
				if (!enemyFlag.isCapture && !isKilled && enemyFlag.GetComponent<FlagController>().flagModel.activeSelf && Vector3.SqrMagnitude(myPlayerTransform.position - enemyFlag.transform.position) < 2.25f)
				{
					enemyFlag.SetCapture(photonView.ownerId);
					isCaptureFlag = true;
					photonView.RPC("SendSystemMessegeFromFlagCaptureRPC", PhotonTargets.All, enemyFlag.isBlue, mySkinName.NickName);
				}
			}
			if (isCaptureFlag && Vector3.SqrMagnitude(myPlayerTransform.position - myBaza.transform.position) < 2.25f)
			{
				if (myFlag.isBaza)
				{
					if (Defs.isSoundFX)
					{
						GetComponent<AudioSource>().PlayOneShot(flagScoreMyCommandClip);
					}
					if (myTable != null)
					{
						myNetworkStartTable.AddScore();
					}
					countMultyFlag++;
					if (!NetworkStartTable.LocalOrPasswordRoom())
					{
						QuestMediator.NotifyCapture(ConnectSceneNGUIController.RegimGame.FlagCapture);
					}
					myScoreController.AddScoreOnEvent((countMultyFlag == 3) ? PlayerEventScoreController.ScoreEvent.flagTouchDownTriple : ((countMultyFlag != 2) ? PlayerEventScoreController.ScoreEvent.flagTouchDown : PlayerEventScoreController.ScoreEvent.flagTouchDouble));
					isCaptureFlag = false;
					photonView.RPC("SendSystemMessegeFromFlagAddScoreRPC", PhotonTargets.Others, !enemyFlag.isBlue, mySkinName.NickName);
					AddSystemMessage(LocalizationStore.Get("Key_1003"));
					enemyFlag.GoBaza();
				}
				else if (!inGameGUI.message_returnFlag.activeSelf)
				{
					inGameGUI.message_returnFlag.SetActive(true);
				}
			}
			else if (inGameGUI.message_returnFlag.activeSelf)
			{
				inGameGUI.message_returnFlag.SetActive(false);
			}
			if (Defs.isFlag && inGameGUI != null)
			{
				if (isCaptureFlag)
				{
					if (!inGameGUI.flagRedCaptureTexture.activeSelf)
					{
						inGameGUI.flagRedCaptureTexture.SetActive(true);
					}
				}
				else if (inGameGUI.flagRedCaptureTexture.activeSelf)
				{
					inGameGUI.flagRedCaptureTexture.SetActive(false);
				}
			}
		}
		if (!isMulti || isMine)
		{
			if (((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInClip == 0 && !_changingWeapon && ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInBackpack > 0 && !_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().IsPlaying("Shoot") && !isReloading)
			{
				ReloadPressed();
			}
			if (!isHunger || hungerGameController.isGo)
			{
				PotionsController.sharedController.Step(Time.deltaTime, this);
			}
		}
		if (isHunger && isMine)
		{
			timeHingerGame += Time.deltaTime;
			bool flag = InGameGUI.sharedInGameGUI != null && InGameGUI.sharedInGameGUI.pausePanel.activeSelf;
			if (Initializer.players.Count == 1 && hungerGameController.isGo && timeHingerGame > 10f && !isZachetWin && !flag)
			{
				isZachetWin = true;
				int val = Storager.getInt(Defs.RatingHunger, false) + 1;
				Storager.setInt(Defs.RatingHunger, val, false);
				val = Storager.getInt("Rating", false) + 1;
				Storager.setInt("Rating", val, false);
				if (FriendsController.sharedController != null)
				{
					FriendsController.sharedController.TryIncrementWinCountTimestamp();
				}
				FriendsController.sharedController.wins.Value = val;
				FriendsController.sharedController.SendOurData();
				myNetworkStartTable.WinInHunger();
			}
		}
		if (!isMulti)
		{
			inGameTime += Time.deltaTime;
		}
		if ((isCompany || Defs.isFlag) && myCommand == 0 && myTable != null)
		{
			myCommand = myNetworkStartTable.myCommand;
		}
		if (isMulti && isMine && _weaponManager.myPlayer != null)
		{
			GlobalGameController.posMyPlayer = _weaponManager.myPlayer.transform.position;
			GlobalGameController.rotMyPlayer = _weaponManager.myPlayer.transform.rotation;
			GlobalGameController.healthMyPlayer = CurHealth;
			GlobalGameController.armorMyPlayer = curArmor;
		}
		if (!isMulti || isMine)
		{
			if (timerShow[0] > 0f)
			{
				timerShow[0] -= Time.deltaTime;
			}
			if (timerShow[1] > 0f)
			{
				timerShow[1] -= Time.deltaTime;
			}
			if (timerShow[2] > 0f)
			{
				timerShow[2] -= Time.deltaTime;
			}
		}
		if (!isMulti || isMine)
		{
			Func<bool> func = () => _pauser != null && _pauser.paused;
			if (!func() && canReceiveSwipes && isInappWinOpen)
			{
			}
		}
		if (GunFlashLifetime > 0f)
		{
			GunFlashLifetime -= Time.deltaTime;
			if (GunFlashLifetime <= 0f)
			{
				GunFlashLifetime = 0f;
				_SetGunFlashActive(false);
			}
		}
		else if (GunFlashLifetime == -1f && JoystickController.IsButtonFireUp())
		{
			GunFlashLifetime = 0f;
			_SetGunFlashActive(false);
		}
		if (!Defs.isDaterRegim || !isPlayerFlying)
		{
			return;
		}
		if (!isMine)
		{
			if (!wingsAnimation.isPlaying)
			{
				wingsAnimation.Play();
			}
			if (!wingsBearAnimation.isPlaying)
			{
				wingsBearAnimation.Play();
			}
		}
		if (Defs.isSoundFX && !wingsSound.isPlaying)
		{
			wingsSound.Play();
		}
	}

	private void HandleEscape()
	{
		if (trainigController != null)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Ignoring [Escape] in training scene.");
			}
			return;
		}
		if (isMulti && !isMine)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("Ignoring [Escape]; isMulti: {0}, isMine: {1}", isMulti, isMine);
			}
			return;
		}
		if (!Cursor.visible)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Handling [Escape]. Cursor locked.");
			}
			_escapePressed = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			return;
		}
		if (showRanks)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("Ignoring [Escape]; showRanks: {0}", showRanks);
			}
			return;
		}
		if (RespawnWindow.Instance != null && RespawnWindow.Instance.isShown)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Handling [Escape] in Respawn Window.");
			}
			RespawnWindow.Instance.OnBtnGoBattleClick();
			return;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("ChatViewer");
		if (gameObject == null)
		{
			if (!isInappWinOpen && Cursor.lockState != CursorLockMode.Locked)
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Handling [Escape]; isInappWinOpen: {0}, lockState: '{1}'", isInappWinOpen, Cursor.lockState);
				}
				_escapePressed = true;
			}
		}
		else if (!gameObject.GetComponent<ChatViewrController>().buySmileBannerPrefab.activeSelf)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Handling [Escape]. Closing chat");
			}
			gameObject.GetComponent<ChatViewrController>().CloseChat();
		}
	}

	public void GoToShopFromPause()
	{
		SetInApp();
		inAppOpenedFromPause = true;
	}

	public void QuitGame()
	{
		Time.timeScale = 1f;
		Time.timeScale = 1f;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			LevelCompleteLoader.action = null;
			LevelCompleteLoader.sceneName = Defs.MainMenuScene;
			Application.LoadLevel("LevelToCompleteProm");
		}
		else if (isMulti)
		{
			if (EveryplayWrapper.Instance.CurrentState == EveryplayWrapper.State.Paused || EveryplayWrapper.Instance.CurrentState == EveryplayWrapper.State.Recording)
			{
				EveryplayWrapper.Instance.Stop();
			}
			if (!isInet)
			{
				if (PlayerPrefs.GetString("TypeGame").Equals("server"))
				{
					Network.Disconnect(200);
					GameObject.FindGameObjectWithTag("NetworkTable").GetComponent<LANBroadcastService>().StopBroadCasting();
				}
				else if (Network.connections.Length == 1)
				{
					Network.CloseConnection(Network.connections[0], true);
				}
				ActivityIndicator.IsActiveIndicator = false;
				coinsShop.hideCoinsShop();
				coinsPlashka.hidePlashka();
				ConnectSceneNGUIController.Local();
			}
			else
			{
				coinsShop.hideCoinsShop();
				coinsPlashka.hidePlashka();
				Defs.typeDisconnectGame = Defs.DisconectGameType.Exit;
				PhotonNetwork.LeaveRoom();
			}
		}
		else if (Defs.IsSurvival)
		{
			if (GlobalGameController.Score > PlayerPrefs.GetInt(Defs.SurvivalScoreSett, 0))
			{
				GlobalGameController.HasSurvivalRecord = true;
				PlayerPrefs.SetInt(Defs.SurvivalScoreSett, GlobalGameController.Score);
				PlayerPrefs.Save();
				FriendsController.sharedController.survivalScore = GlobalGameController.Score;
				FriendsController.sharedController.SendOurData();
			}
			if (Storager.getInt("SendFirstResaltArena", false) != 1)
			{
				Storager.setInt("SendFirstResaltArena", 1, false);
				AnalyticsStuff.LogArenaFirst(true, false);
			}
			Debug.Log("Player_move_c.QuitGame(): Trying to report survival score: " + GlobalGameController.Score);
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				AGSLeaderboardsClient.SubmitScore("best_survival_scores", GlobalGameController.Score);
			}
			else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite && Social.localUser.authenticated)
			{
				Social.ReportScore(GlobalGameController.Score, "CgkIr8rGkPIJEAIQCg", delegate(bool success)
				{
					Debug.Log("Player_move_c.QuitGame(): " + ((!success) ? "Failed to report score." : "Reported score successfully."));
				});
			}
			PlayerPrefs.SetInt("IsGameOver", 1);
			LevelCompleteLoader.action = null;
			LevelCompleteLoader.sceneName = "LevelComplete";
			Application.LoadLevel("LevelToCompleteProm");
		}
		else
		{
			LevelCompleteLoader.action = null;
			LevelCompleteLoader.sceneName = "ChooseLevel";
			bool flag = !isMulti;
			if (!flag)
			{
				FlurryPluginWrapper.LogEvent("Back to Main Menu");
			}
			Application.LoadLevel((!flag) ? Defs.MainMenuScene : "LevelToCompleteProm");
		}
	}

	public void SetPause(bool showGUI = true)
	{
		JoystickController.rightJoystick.jumpPressed = false;
		JoystickController.leftTouchPad.isJumpPressed = false;
		JoystickController.rightJoystick.Reset();
		if (_pauser == null)
		{
			Debug.LogWarning("SetPause(): _pauser is null.");
			return;
		}
		_pauser.paused = !_pauser.paused;
		if (myCurrentWeaponSounds != null)
		{
			myCurrentWeaponSounds.animationObject.SetActive(!_pauser.paused);
		}
		if (_pauser.paused)
		{
			isActiveTurretPanelInPause = InGameGUI.sharedInGameGUI.turretPanel.activeSelf;
			InGameGUI.sharedInGameGUI.turretPanel.SetActive(false);
		}
		else
		{
			InGameGUI.sharedInGameGUI.turretPanel.SetActive(isActiveTurretPanelInPause);
		}
		if (showGUI && inGameGUI != null && inGameGUI.pausePanel != null)
		{
			inGameGUI.pausePanel.SetActive(_pauser.paused);
			inGameGUI.fastShopPanel.SetActive(!_pauser.paused);
			if (ExperienceController.sharedController != null && ExpController.Instance != null)
			{
				ExperienceController.sharedController.isShowRanks = _pauser.paused;
				ExpController.Instance.InterfaceEnabled = _pauser.paused;
			}
		}
		if (_pauser.paused)
		{
			if (!isMulti)
			{
				Time.timeScale = 0f;
				if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
				{
					TrainingController.isPause = true;
				}
			}
		}
		else
		{
			Time.timeScale = 1f;
			TrainingController.isPause = false;
		}
		if (_pauser.paused)
		{
			RemoveButtonHandelrs();
		}
		else
		{
			AddButtonHandlers();
		}
	}

	public void WinFromTimer()
	{
		if (!base.enabled)
		{
			return;
		}
		base.enabled = false;
		InGameGUI.sharedInGameGUI.gameObject.SetActive(false);
		if (Defs.isCompany)
		{
			int commandWin = 0;
			if (countKillsCommandBlue > countKillsCommandRed)
			{
				commandWin = 1;
			}
			if (countKillsCommandRed > countKillsCommandBlue)
			{
				commandWin = 2;
			}
			if (WeaponManager.sharedManager.myTable != null)
			{
				WeaponManager.sharedManager.myNetworkStartTable.win(string.Empty, commandWin, countKillsCommandBlue, countKillsCommandRed);
			}
		}
		else if (Defs.isCOOP)
		{
			ZombiManager.sharedManager.EndMatch();
		}
		else if (WeaponManager.sharedManager.myTable != null)
		{
			WeaponManager.sharedManager.myNetworkStartTable.win(string.Empty);
		}
	}

	private void SetInApp()
	{
		isInappWinOpen = !isInappWinOpen;
		if (isInappWinOpen)
		{
			if (StoreKitEventListener.restoreInProcess)
			{
				ActivityIndicator.IsActiveIndicator = true;
			}
			if (!isMulti)
			{
				Time.timeScale = 0f;
			}
			return;
		}
		if (InGameGUI.sharedInGameGUI.shopPanelForSwipe.gameObject.activeSelf)
		{
			InGameGUI.sharedInGameGUI.shopPanelForSwipe.gameObject.SetActive(false);
			InGameGUI.sharedInGameGUI.shopPanelForSwipe.gameObject.SetActive(TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None);
		}
		if (InGameGUI.sharedInGameGUI.shopPanelForTap.gameObject.activeSelf)
		{
			InGameGUI.sharedInGameGUI.shopPanelForTap.gameObject.SetActive(false);
			InGameGUI.sharedInGameGUI.shopPanelForTap.gameObject.SetActive(true);
		}
		ActivityIndicator.IsActiveIndicator = false;
		if (_pauser == null)
		{
			Debug.LogWarning("SetInApp(): _pauser is null.");
		}
		else if (!_pauser.paused)
		{
			Time.timeScale = 1f;
		}
	}

	private void providePotion(string inShopId)
	{
	}

	private void ProvideAmmo(string inShopId)
	{
		_listener.ProvideContent();
		_weaponManager.SetMaxAmmoFrAllWeapons();
		if (JoystickController.rightJoystick != null)
		{
			if (inGameGUI != null)
			{
				inGameGUI.BlinkNoAmmo(0);
			}
			JoystickController.rightJoystick.HasAmmo();
		}
		else
		{
			Debug.Log("JoystickController.rightJoystick = null");
		}
	}

	public void PurchaseSuccessful(string id)
	{
		if (_actionsForPurchasedItems.ContainsKey(id))
		{
			_actionsForPurchasedItems[id](id);
		}
		_timeWhenPurchShown = Time.realtimeSinceStartup;
	}

	private void purchaseSuccessful(GooglePurchase purchase)
	{
		try
		{
			if (purchase == null)
			{
				throw new ArgumentNullException("purchase");
			}
			PurchaseSuccessful(purchase.productId);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	private void HandlePurchaseSuccessful(PurchaseResponse response)
	{
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning("Amazon PurchaseResponse (Player_move_c): " + response.Status);
			return;
		}
		Debug.Log("Amazon PurchaseResponse (Player_move_c): " + response.PurchaseReceipt.ToJson());
		PurchaseSuccessful(response.PurchaseReceipt.Sku);
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		if (isMine)
		{
			_networkView.RPC("SetInvisibleRPC", player, (!Defs.isDaterRegim) ? isInvisible : isBigHead);
			_networkView.RPC("CountKillsCommandSynch", player, countKillsCommandBlue, countKillsCommandRed);
			_networkView.RPC("SetWeaponRPC", player, ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).weaponPrefab.name, ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).weaponPrefab.GetComponent<WeaponSounds>().alternativeName);
			SendSynhHealth(true);
			if (Defs.isJetpackEnabled)
			{
				_networkView.RPC("SetJetpackEnabledRPC", player, Defs.isJetpackEnabled);
			}
			if (isMechActive || isBearActive)
			{
				_networkView.RPC("ActivateMechRPC", player, mechUpgrade);
			}
			_networkView.RPC("SynhIsZoming", player, isZooming);
		}
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if ((bool)photonView && photonView.isMine)
		{
			photonView.RPC("CountKillsCommandSynch", player, countKillsCommandBlue, countKillsCommandRed);
			photonView.RPC("SetInvisibleRPC", player, (!Defs.isDaterRegim) ? isInvisible : isBigHead);
			photonView.RPC("SetWeaponRPC", player, ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).weaponPrefab.name, ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).weaponPrefab.GetComponent<WeaponSounds>().alternativeName);
			SendSynhHealth(true, player);
			if (Defs.isJetpackEnabled)
			{
				photonView.RPC("SetJetpackEnabledRPC", player, Defs.isJetpackEnabled);
			}
			if (isMechActive || isBearActive)
			{
				photonView.RPC("ActivateMechRPC", player, mechUpgrade);
			}
			photonView.RPC("SynhIsZoming", player, isZooming);
			if (FriendsController.useBuffSystem || KillRateCheck.instance.buffEnabled)
			{
				photonView.RPC("SendBuffParameters", player, damageBuff, protectionBuff);
			}
		}
	}

	public void ShowChat()
	{
		if (!isKilled)
		{
			if (JoystickController.rightJoystick != null)
			{
				JoystickController.rightJoystick.jumpPressed = false;
				JoystickController.leftTouchPad.isJumpPressed = false;
				JoystickController.rightJoystick.Reset();
			}
			RemoveButtonHandelrs();
			showChat = true;
			if (inGameGUI.gameObject != null)
			{
				inGameGUI.gameObject.SetActive(false);
			}
			_weaponManager.currentWeaponSounds.gameObject.SetActive(false);
			mechPoint.SetActive(false);
			GameObject gameObject = UnityEngine.Object.Instantiate(chatViewer);
		}
	}

	public void SetInvisible(bool _isInvisible)
	{
		if (isMulti)
		{
			if (!isInet)
			{
				GetComponent<NetworkView>().RPC("SetInvisibleRPC", RPCMode.All, _isInvisible);
			}
			else if (photonView != null)
			{
				photonView.RPC("SetInvisibleRPC", PhotonTargets.All, _isInvisible);
			}
		}
		else
		{
			SetInvisibleRPC(_isInvisible);
		}
	}

	public void SetNicklabelVisible()
	{
		if (!isMine)
		{
			nickLabel.gameObject.SetActive(!isInvisible || ((ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture) && WeaponManager.sharedManager.myPlayerMoveC != null && myCommand == WeaponManager.sharedManager.myPlayerMoveC.myCommand));
		}
	}

	[PunRPC]
	[RPC]
	private void SetInvisibleRPC(bool _isInvisible)
	{
		if (!Defs.isDaterRegim)
		{
			if (isInvisible == _isInvisible)
			{
				return;
			}
			isInvisible = _isInvisible;
			if (Defs.isSoundFX && _isInvisible)
			{
				GetComponent<AudioSource>().PlayOneShot(invisibleActivSound);
			}
			if (!isMulti || isMine)
			{
				SetInVisibleShaders(isInvisible);
				return;
			}
			SetNicklabelVisible();
			if (!isInvisible)
			{
				invisibleParticle.SetActive(false);
				if (isMechActive)
				{
					mechPoint.SetActive(true);
				}
				else
				{
					mySkinName.FPSplayerObject.SetActive(true);
				}
			}
			else
			{
				invisibleParticle.SetActive(true);
				mySkinName.FPSplayerObject.SetActive(false);
				mechPoint.SetActive(false);
			}
		}
		else
		{
			if (isBigHead == _isInvisible)
			{
				return;
			}
			isBigHead = _isInvisible;
			if (Defs.isSoundFX && _isInvisible)
			{
				GetComponent<AudioSource>().PlayOneShot(invisibleActivSound);
			}
			if (!isMulti || isMine)
			{
				return;
			}
			if (_isInvisible)
			{
				MechHeadTransform.localScale = Vector3.one * 2f;
				PlayerHeadTransform.localScale = Vector3.one * 2f;
				if (isBearActive)
				{
					nickLabel.transform.localPosition = 2.549f * Vector3.up;
				}
				else
				{
					nickLabel.transform.localPosition = 1.678f * Vector3.up;
				}
			}
			else
			{
				MechHeadTransform.localScale = Vector3.one;
				PlayerHeadTransform.localScale = Vector3.one;
				if (isBearActive)
				{
					nickLabel.transform.localPosition = Vector3.up * 1.54f;
				}
				else
				{
					nickLabel.transform.localPosition = Vector3.up * 1.08f;
				}
			}
		}
	}

	private void SetInVisibleShaders(bool _isInvisible)
	{
		if (isGrenadePress)
		{
			return;
		}
		if (_isInvisible)
		{
			if (WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab != null)
			{
				oldShadersInInvisible = new Shader[WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.transform.parent.GetComponent<Renderer>().materials.Length + ((WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>() != null) ? WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials.Length : 0)];
				oldColorInInvisible = new Color[oldShadersInInvisible.Length];
				oldShadersInInvisible[0] = WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.transform.parent.GetComponent<Renderer>().material.shader;
				WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.transform.parent.GetComponent<Renderer>().material.shader = Shader.Find("Mobile/Diffuse-Color");
				WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.transform.parent.GetComponent<Renderer>().material.SetColor("_ColorRili", new Color(1f, 1f, 1f, 0.5f));
				oldColorInInvisible[0] = WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.transform.parent.GetComponent<Renderer>().material.color;
				if (WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>() != null)
				{
					for (int i = 0; i < WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials.Length; i++)
					{
						oldShadersInInvisible[i + 1] = WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials[i].shader;
						oldColorInInvisible[i + 1] = WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials[i].color;
						WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials[i].shader = Shader.Find("Mobile/Diffuse-Color");
						WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials[i].SetColor("_ColorRili", new Color(1f, 1f, 1f, 0.5f));
					}
				}
			}
			_mechMaterial.SetColor("_ColorRili", new Color(1f, 1f, 1f, 0.5f));
			mechGunRenderer.material.SetColor("_ColorRili", new Color(1f, 1f, 1f, 0.5f));
			return;
		}
		if (WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab != null)
		{
			WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.transform.parent.GetComponent<Renderer>().material.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
			if (WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>() != null)
			{
				for (int j = 0; j < WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials.Length; j++)
				{
					WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials[j].shader = oldShadersInInvisible[j + 1];
					WeaponManager.sharedManager.currentWeaponSounds.bonusPrefab.GetComponent<Renderer>().materials[j].color = oldColorInInvisible[j + 1];
				}
			}
		}
		_mechMaterial.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
		mechGunRenderer.material.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
	}

	[PunRPC]
	[RPC]
	public void ActivateMechRPC(int num)
	{
		ActivateMech(num);
	}

	[PunRPC]
	[RPC]
	public void ActivateMechRPC()
	{
		ActivateMech();
	}

	[RPC]
	[PunRPC]
	public void DeactivateMechRPC()
	{
		DeactivateMech();
	}

	private void SetWeaponVisible(bool visible)
	{
		myCurrentWeaponSounds.SetDaterBearHandsAnim(!visible);
		if (currentGrenade != null)
		{
			currentGrenade.transform.parent = myCurrentWeaponSounds.grenatePoint;
		}
	}

	public void ActivateBear()
	{
		if (isBearActive)
		{
			return;
		}
		float num = -1f;
		if (myCurrentWeaponSounds != null && myCurrentWeaponSounds.animationObject.GetComponent<Animation>().IsPlaying("Reload"))
		{
			num = myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].time;
		}
		mechPoint = mechBearPoint;
		mechBody = mechBearBody;
		mechBodyAnimation = mechBearBodyAnimation;
		mechGunAnimation = mechBearGunAnimation;
		mechBodyRenderer = mechBearBodyRenderer;
		mechHandRenderer = mechBearHandRenderer;
		shootMechClip = shootMechBearClip;
		mechExplossionSound = mechBearExplosionSound;
		mySkinName.walkMech = mySkinName.walkMechBear;
		mechExplossion = bearExplosion;
		if ((!Defs.isMulti || isMine) && isZooming)
		{
			ZoomPress();
		}
		deltaAngle = 0f;
		mechUpgrade = 0;
		if (Defs.isSoundFX)
		{
			GetComponent<AudioSource>().PlayOneShot(mechBearActivSound);
		}
		isBearActive = true;
		fpsPlayerBody.SetActive(false);
		if (myCurrentWeapon != null)
		{
			SetWeaponVisible(false);
		}
		if (isMine || (!isMine && !isInvisible) || !isMulti)
		{
			mechPoint.SetActive(true);
		}
		mechPoint.GetComponent<DisableObjectFromTimer>().timer = -1f;
		if (!isMulti || isMine)
		{
			base.transform.localPosition = myCamera.transform.localPosition;
			mechBody.SetActive(false);
			mechBearSyncRot.enabled = true;
			mechPoint.transform.localPosition = Vector3.zero;
			myCurrentWeaponSounds.animationObject.GetComponent<Animation>().cullingType = AnimationCullingType.AlwaysAnimate;
			if (myCurrentWeaponSounds.animationObject != null)
			{
				if (myCurrentWeaponSounds.animationObject.GetComponent<Animation>().GetClip("Reload") != null)
				{
					myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].layer = 1;
				}
				if (!myCurrentWeaponSounds.isDoubleShot)
				{
					if (myCurrentWeaponSounds.animationObject.GetComponent<Animation>().GetClip("Shoot") != null)
					{
						myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot"].layer = 1;
					}
				}
				else
				{
					myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot1"].layer = 1;
					myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot2"].layer = 1;
				}
			}
		}
		else
		{
			bodyCollayder.height = 2.07f;
			bodyCollayder.center = new Vector3(0f, 0.19f, 0f);
			headCollayder.center = new Vector3(0f, 0.54f, 0f);
			if (isBigHead)
			{
				nickLabel.transform.localPosition = 2.549f * Vector3.up;
			}
			else
			{
				nickLabel.transform.localPosition = Vector3.up * 1.54f;
			}
		}
		liveMech = liveMechByTier[0];
		_mechMaterial.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
		if (isMulti && isMine)
		{
			if (Defs.isInet)
			{
				photonView.RPC("ActivateMechRPC", PhotonTargets.Others, 0);
			}
			else
			{
				GetComponent<NetworkView>().RPC("ActivateMechRPC", RPCMode.Others, 0);
			}
		}
		if (num != -1f)
		{
			myCurrentWeaponSounds.animationObject.GetComponent<Animation>().Play("Reload");
			myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].time = num;
		}
		mySkinName.SetAnim(mySkinName.currentAnim, EffectsController.WeAreStealth);
	}

	public void DeactivateBear()
	{
		if (!isBearActive)
		{
			return;
		}
		isBearActive = false;
		float num = -1f;
		if (myCurrentWeaponSounds != null && myCurrentWeaponSounds.animationObject.GetComponent<Animation>().IsPlaying("Reload"))
		{
			num = myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].time;
		}
		if (myCurrentWeapon != null)
		{
			SetWeaponVisible(true);
		}
		myCamera.transform.localPosition = new Vector3(0f, 0.7f, 0f);
		if (Defs.isSoundFX)
		{
			mechExplossionSound.Play();
		}
		if (isMulti && !isMine)
		{
			if (!isInvisible)
			{
				fpsPlayerBody.SetActive(true);
			}
			bodyCollayder.height = 1.51f;
			bodyCollayder.center = Vector3.zero;
			headCollayder.center = Vector3.zero;
			mechExplossion.SetActive(true);
			mechExplossion.GetComponent<DisableObjectFromTimer>().timer = 1f;
			mechBodyAnimation.Play("Dead");
			mechGunAnimation.Play("Dead");
			mechPoint.GetComponent<DisableObjectFromTimer>().timer = 0.46f;
			myCurrentWeaponSounds.animationObject.GetComponent<Animation>().cullingType = AnimationCullingType.AlwaysAnimate;
			if (myCurrentWeaponSounds.animationObject != null)
			{
				if (myCurrentWeaponSounds.animationObject.GetComponent<Animation>().GetClip("Reload") != null)
				{
					myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].layer = 1;
				}
				if (!myCurrentWeaponSounds.isDoubleShot)
				{
					if (myCurrentWeaponSounds.animationObject.GetComponent<Animation>().GetClip("Shoot") != null)
					{
						myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot"].layer = 1;
					}
				}
				else
				{
					myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot1"].layer = 1;
					myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot2"].layer = 1;
				}
			}
			if (isBigHead)
			{
				nickLabel.transform.localPosition = Vector3.up * 1.54f;
			}
			else
			{
				nickLabel.transform.localPosition = Vector3.up * 1.08f;
			}
		}
		else
		{
			mechPoint.SetActive(false);
			gunCamera.fieldOfView = 75f;
			base.transform.localPosition = myCamera.transform.localPosition;
			gunCamera.transform.localPosition = new Vector3(-0.1f, 0f, 0f);
		}
		if (!isMulti || isMine)
		{
			PotionsController.sharedController.DeActivePotion(GearManager.Mech, this);
		}
		if (isMulti && isMine)
		{
			if (Defs.isInet)
			{
				photonView.RPC("DeactivateMechRPC", PhotonTargets.Others);
			}
			else
			{
				GetComponent<NetworkView>().RPC("DeactivateMechRPC", RPCMode.Others);
			}
		}
		if (num != -1f)
		{
			myCurrentWeaponSounds.animationObject.GetComponent<Animation>().Play("Reload");
			myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].time = Mathf.Min(num, myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].length);
		}
		mySkinName.SetAnim(mySkinName.currentAnim, EffectsController.WeAreStealth);
	}

	public void ActivateMech(int num = 0)
	{
		if (isMechActive)
		{
			return;
		}
		if (Defs.isDaterRegim)
		{
			ActivateBear();
			return;
		}
		if ((!Defs.isMulti || isMine) && isZooming)
		{
			ZoomPress();
		}
		deltaAngle = 0f;
		mechUpgrade = num;
		if (Defs.isSoundFX)
		{
			GetComponent<AudioSource>().PlayOneShot(mechActivSound);
		}
		isMechActive = true;
		fpsPlayerBody.SetActive(false);
		if (myCurrentWeapon != null)
		{
			myCurrentWeapon.SetActive(false);
		}
		if (isMine || (!isMine && !isInvisible) || !isMulti)
		{
			mechPoint.SetActive(true);
		}
		mechPoint.GetComponent<DisableObjectFromTimer>().timer = -1f;
		myCamera.transform.localPosition = new Vector3(0.12f, 0.7f, -0.3f);
		if (!isMulti || isMine)
		{
			num = GearManager.CurrentNumberOfUphradesForGear(GearManager.Mech);
			mechBody.SetActive(false);
			mechBearSyncRot.enabled = true;
			mechPoint.transform.localPosition = new Vector3(0f, -0.3f, 0f);
			gunCamera.fieldOfView = 45f;
			gunCamera.transform.localPosition = new Vector3(-0.1f, 0f, 0f);
			if (inGameGUI != null)
			{
				inGameGUI.fireButtonSprite.spriteName = "controls_fire";
				inGameGUI.fireButtonSprite2.spriteName = "controls_fire";
			}
		}
		else
		{
			bodyCollayder.height = 2.07f;
			bodyCollayder.center = new Vector3(0f, 0.19f, 0f);
			headCollayder.center = new Vector3(0f, 0.54f, 0f);
			nickLabel.transform.localPosition = Vector3.up * 1.72f;
		}
		liveMech = liveMechByTier[num];
		if (!Defs.isDaterRegim)
		{
			_mechMaterial = new Material(mechBodyMaterials[num]);
			mechBodyRenderer.sharedMaterial = _mechMaterial;
			mechHandRenderer.sharedMaterial = _mechMaterial;
			mechGunRenderer.material = mechGunMaterials[num];
		}
		if (!Defs.isDaterRegim && isInvisible && (!isMulti || isMine))
		{
			_mechMaterial.SetColor("_ColorRili", new Color(1f, 1f, 1f, 0.5f));
			mechGunRenderer.material.SetColor("_ColorRili", new Color(1f, 1f, 1f, 0.5f));
		}
		else
		{
			_mechMaterial.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
		}
		if (isMulti && isMine)
		{
			if (Defs.isInet)
			{
				photonView.RPC("ActivateMechRPC", PhotonTargets.Others, num);
			}
			else
			{
				GetComponent<NetworkView>().RPC("ActivateMechRPC", RPCMode.Others, num);
			}
		}
		if (!Defs.isDaterRegim)
		{
			for (int i = 0; i < mechWeaponSounds.gunFlashDouble.Length; i++)
			{
				mechWeaponSounds.gunFlashDouble[i].GetChild(0).gameObject.SetActive(false);
			}
		}
		if ((!isMulti || isMine) && inGameGUI != null)
		{
			inGameGUI.SetCrosshair(mechWeaponSounds);
		}
		mySkinName.SetAnim(mySkinName.currentAnim, EffectsController.WeAreStealth);
	}

	public void DeactivateMech()
	{
		if (Defs.isDaterRegim)
		{
			DeactivateBear();
		}
		else
		{
			if (!isMechActive)
			{
				return;
			}
			isMechActive = false;
			if (myCurrentWeapon != null)
			{
				myCurrentWeapon.SetActive(true);
			}
			myCamera.transform.localPosition = new Vector3(0f, 0.7f, 0f);
			if (Defs.isSoundFX)
			{
				mechExplossionSound.Play();
			}
			if (isMulti && !isMine)
			{
				if (!isInvisible)
				{
					fpsPlayerBody.SetActive(true);
				}
				bodyCollayder.height = 1.51f;
				bodyCollayder.center = Vector3.zero;
				headCollayder.center = Vector3.zero;
				mechExplossion.SetActive(true);
				mechExplossion.GetComponent<DisableObjectFromTimer>().timer = 1f;
				mechBodyAnimation.Play("Dead");
				mechGunAnimation.Play("Dead");
				mechPoint.GetComponent<DisableObjectFromTimer>().timer = 0.46f;
				nickLabel.transform.localPosition = Vector3.up * 1.08f;
			}
			else
			{
				mechPoint.SetActive(false);
				gunCamera.fieldOfView = 75f;
				if (myCurrentWeaponSounds.isDoubleShot)
				{
					gunCamera.transform.localPosition = Vector3.zero;
				}
				else
				{
					gunCamera.transform.localPosition = new Vector3(-0.1f, 0f, 0f);
				}
				if (inGameGUI != null)
				{
					if (_weaponManager.currentWeaponSounds.isMelee && !_weaponManager.currentWeaponSounds.isShotMelee)
					{
						inGameGUI.fireButtonSprite.spriteName = "controls_strike";
						inGameGUI.fireButtonSprite2.spriteName = "controls_strike";
					}
					else
					{
						inGameGUI.fireButtonSprite.spriteName = "controls_fire";
						inGameGUI.fireButtonSprite2.spriteName = "controls_fire";
					}
				}
			}
			if (!isMulti || isMine)
			{
				PotionsController.sharedController.DeActivePotion(GearManager.Mech, this);
			}
			if (isMulti && isMine)
			{
				if (Defs.isInet)
				{
					photonView.RPC("DeactivateMechRPC", PhotonTargets.Others);
				}
				else
				{
					GetComponent<NetworkView>().RPC("DeactivateMechRPC", RPCMode.Others);
				}
			}
			if ((!isMulti || isMine) && inGameGUI != null)
			{
				inGameGUI.SetCrosshair(_weaponManager.currentWeaponSounds);
			}
			mySkinName.SetAnim(mySkinName.currentAnim, EffectsController.WeAreStealth);
		}
	}

	public void UpdateEffectsForCurrentWeapon(string currentCape, string currentMask, string currentHat)
	{
		if (!(myCurrentWeaponSounds == null))
		{
			if (!isMine)
			{
				_chanceToIgnoreHeadshot = EffectsController.GetChanceToIgnoreHeadshot(myCurrentWeaponSounds.categoryNabor, currentCape, currentMask, currentHat);
			}
			_currentReloadAnimationSpeed = EffectsController.GetReloadAnimationSpeed(myCurrentWeaponSounds.categoryNabor, currentCape, currentMask, currentHat);
		}
	}

	public void BlockPlayerInEnd()
	{
		mySkinName.BlockFirstPersonController();
		myCurrentWeaponSounds.animationObject.GetComponent<Animation>().enabled = false;
		if (GunFlash != null)
		{
			GunFlash.gameObject.SetActive(false);
		}
		mySkinName.character.enabled = false;
		base.enabled = false;
	}

	[ContextMenu("Active mech")]
	public void TestActiveMech()
	{
		ActivateMech();
	}

	public bool MinusMechHealth(float _minus)
	{
		liveMech -= _minus;
		if (liveMech <= 0f)
		{
			DeactivateMech();
			return true;
		}
		return false;
	}

	public void ImSuicide()
	{
		isSuicided = true;
		respawnedForGUI = true;
		if (Defs.isFlag && isCaptureFlag)
		{
			enemyFlag.GoBaza();
			isCaptureFlag = false;
			SendSystemMessegeFromFlagReturned(enemyFlag.isBlue);
		}
		if (countKills > 0)
		{
			GlobalGameController.CountKills = countKills;
		}
		_weaponManager.myNetworkStartTable.CountKills = countKills;
		_weaponManager.myNetworkStartTable.SynhCountKills();
		sendImDeath(mySkinName.NickName);
	}

	private void UpdateHealth()
	{
		if (isMulti && isMine && CurHealth + curArmor - synhHealth > 0.1f)
		{
			SendSynhHealth(true);
		}
		if (!isMulti || isMine)
		{
			if (!isRegenerationLiveCape)
			{
				timerRegenerationLiveCape = maxTimerRegenerationLiveCape;
			}
			if (isRegenerationLiveCape)
			{
				if (timerRegenerationLiveCape > 0f)
				{
					timerRegenerationLiveCape -= Time.deltaTime;
				}
				else
				{
					timerRegenerationLiveCape = maxTimerRegenerationLiveCape;
					if (CurHealth < MaxHealth)
					{
						CurHealth += 1f;
					}
				}
			}
			if (!EffectsController.IsRegeneratingArmor)
			{
				timerRegenerationArmor = maxTimerRegenerationArmor;
			}
			if (EffectsController.IsRegeneratingArmor)
			{
				if (timerRegenerationArmor > 0f)
				{
					timerRegenerationArmor -= Time.deltaTime;
				}
				else
				{
					timerRegenerationArmor = maxTimerRegenerationArmor;
					if (curArmor < MaxArmor && Storager.getString(Defs.ArmorNewEquppedSN, false) != Defs.ArmorNewNoneEqupped)
					{
						AddArmor(1f);
					}
				}
			}
			if (!isRegenerationLiveZel)
			{
				timerRegenerationLiveZel = maxTimerRegenerationLiveZel;
			}
			if (isRegenerationLiveZel)
			{
				if (timerRegenerationLiveZel > 0f)
				{
					timerRegenerationLiveZel -= Time.deltaTime;
				}
				else
				{
					timerRegenerationLiveZel = maxTimerRegenerationLiveZel;
					if (CurHealth < MaxHealth)
					{
						CurHealth += 1f;
					}
				}
			}
			if (timerShowUp > 0f)
			{
				timerShowUp -= Time.deltaTime;
			}
			if (timerShowDown > 0f)
			{
				timerShowDown -= Time.deltaTime;
			}
			if (timerShowLeft > 0f)
			{
				timerShowLeft -= Time.deltaTime;
			}
			if (timerShowRight > 0f)
			{
				timerShowRight -= Time.deltaTime;
			}
		}
		if ((isMulti && !isMine) || !(CurHealth <= 0f) || isKilled || showRanks || showChat || ShopNGUIController.GuiActive || BankController.Instance.uiRoot.gameObject.activeInHierarchy || (!(_pauser == null) && (!(_pauser != null) || _pauser.paused)))
		{
			return;
		}
		countMultyFlag = 0;
		ResetMySpotEvent();
		ResetHouseKeeperEvent();
		if (myCurrentWeaponSounds == null)
		{
			Debug.Log("immense failure 1");
		}
		if (myCurrentWeaponSounds.animationObject != null)
		{
			Debug.Log("immense failure 2");
		}
		if (myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>() != null)
		{
			myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().Stop();
		}
		if (Mathf.Abs(Time.time - timeBuyHealth) < 1.5f)
		{
			int @int = Storager.getInt("Coins", false);
			Storager.setInt("Coins", @int + Defs.healthInGamePanelPrice, false);
			CoinsMessage.FireCoinsAddedEvent();
			timeBuyHealth = -10000f;
		}
		if (Defs.isCOOP)
		{
			SendImKilled();
			SendSynhHealth(false);
		}
		inGameGUI.ResetDamageTaken();
		if (Defs.isTurretWeapon)
		{
			CancelTurret();
			InGameGUI.sharedInGameGUI.HideTurretInterface();
			Defs.isTurretWeapon = false;
		}
		if (isGrenadePress)
		{
			ReturnWeaponAfterGrenade();
			isGrenadePress = false;
		}
		if (isZooming)
		{
			ZoomPress();
		}
		if (isMulti)
		{
			if ((!isMulti || isMine) && _player != null && (bool)_player)
			{
				ImpactReceiverTrampoline component = _player.GetComponent<ImpactReceiverTrampoline>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
			}
			if (Defs.isFlag && isCaptureFlag)
			{
				isCaptureFlag = false;
				photonView.RPC("SendSystemMessegeFromFlagDroppedRPC", PhotonTargets.All, enemyFlag.isBlue, mySkinName.NickName);
				enemyFlag.SetNOCapture(flagPoint.transform.position, flagPoint.transform.rotation);
			}
			resetMultyKill();
			isKilled = true;
			if (Defs.isMulti && isMine && !Defs.isHunger && !isSuicided && UnityEngine.Random.Range(0, 100) < 50)
			{
				BonusController.sharedController.AddBonusAfterKillPlayer(new Vector3(myPlayerTransform.position.x, myPlayerTransform.position.y - 1f, myPlayerTransform.position.z));
			}
			isSuicided = false;
			if (isHunger && ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).weaponPrefab.name.Replace("(Clone)", string.Empty) != WeaponManager.KnifeWN)
			{
				BonusController.sharedController.AddWeaponAfterKillPlayer(((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).weaponPrefab.name, myPlayerTransform.position);
			}
			if (Defs.isSoundFX)
			{
				base.gameObject.GetComponent<AudioSource>().PlayOneShot(deadPlayerSound);
			}
			if (isCOOP)
			{
				_weaponManager.myNetworkStartTable.score -= 1000;
				if (_weaponManager.myNetworkStartTable.score < 0)
				{
					_weaponManager.myNetworkStartTable.score = 0;
				}
				GlobalGameController.Score = _weaponManager.myNetworkStartTable.score;
				_weaponManager.myNetworkStartTable.SynhScore();
			}
			isDeadFrame = true;
			AutoFade.fadeKilled(0.5f, (!isNeedShowRespawnWindow || Defs.inRespawnWindow) ? 1.5f : 0.5f, 0.5f, Color.white);
			Invoke("setisDeadFrameFalse", 1f);
			StartCoroutine(FlashWhenDead());
			if (JoystickController.leftJoystick != null)
			{
				JoystickController.leftJoystick.transform.parent.gameObject.SetActive(false);
				JoystickController.leftJoystick.SetJoystickActive(false);
			}
			if (JoystickController.leftTouchPad != null)
			{
				JoystickController.leftTouchPad.SetJoystickActive(false);
			}
			if (JoystickController.rightJoystick != null)
			{
				JoystickController.rightJoystick.gameObject.SetActive(false);
				JoystickController.rightJoystick.MakeInactive();
			}
			if (Defs.inRespawnWindow)
			{
				Defs.inRespawnWindow = false;
				RespawnPlayer();
				return;
			}
			Vector3 localPosition = myPlayerTransform.localPosition;
			TweenParms p_parms = new TweenParms().Prop("localPosition", new Vector3(localPosition.x, 100f, localPosition.z)).Ease(EaseType.EaseInCubic).OnComplete((TweenDelegate.TweenCallback)delegate
			{
				myPlayerTransform.localPosition = new Vector3(0f, -1000f, 0f);
				if (isNeedShowRespawnWindow && !Defs.inRespawnWindow)
				{
					SetMapCameraActive(true);
					StartCoroutine(KillCam());
				}
				else
				{
					Defs.inRespawnWindow = false;
					RespawnPlayer();
				}
			});
			HOTween.To(myPlayerTransform, (!isNeedShowRespawnWindow) ? 2f : 0.75f, p_parms);
			return;
		}
		if (Defs.IsSurvival)
		{
			if (GlobalGameController.Score > PlayerPrefs.GetInt(Defs.SurvivalScoreSett, 0))
			{
				GlobalGameController.HasSurvivalRecord = true;
				PlayerPrefs.SetInt(Defs.SurvivalScoreSett, GlobalGameController.Score);
				PlayerPrefs.Save();
				FriendsController.sharedController.survivalScore = GlobalGameController.Score;
				FriendsController.sharedController.SendOurData();
			}
			if (ZombieCreator.sharedCreator != null)
			{
				if (Storager.getInt("SendFirstResaltArena", false) != 1)
				{
					Storager.setInt("SendFirstResaltArena", 1, false);
					AnalyticsStuff.LogArenaFirst(false, ZombieCreator.sharedCreator.currentWave > 0);
				}
				AnalyticsStuff.LogArenaWavesPassed(ZombieCreator.sharedCreator.currentWave);
			}
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				AGSLeaderboardsClient.SubmitScore("best_survival_scores", GlobalGameController.Score);
			}
			else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite && Social.localUser.authenticated)
			{
				Social.ReportScore(GlobalGameController.Score, "CgkIr8rGkPIJEAIQCg", delegate(bool success)
				{
					Debug.Log("Player_move_c.Update(): " + ((!success) ? "Failed to report score." : "Reported score successfully."));
				});
			}
		}
		else if (GlobalGameController.Score > PlayerPrefs.GetInt(Defs.BestScoreSett, 0))
		{
			PlayerPrefs.SetInt(Defs.BestScoreSett, GlobalGameController.Score);
			PlayerPrefs.Save();
		}
		PlayerPrefs.SetInt("IsGameOver", 1);
		LevelCompleteLoader.action = null;
		LevelCompleteLoader.sceneName = "LevelComplete";
		Singleton<SceneLoader>.Instance.LoadScene("LevelToCompleteProm");
	}

	private bool DamagePlayerAndCheckDeath(float damage)
	{
		if (isMechActive)
		{
			MinusMechHealth(damage);
		}
		else
		{
			if (curArmor >= damage)
			{
				curArmor -= damage;
			}
			else
			{
				CurHealth -= damage - curArmor;
				curArmor = 0f;
				CurrentCampaignGame.withoutHits = false;
			}
			if (CurHealth <= 0f)
			{
				return true;
			}
		}
		return false;
	}

	public void SendDamageFromEnv(float damage, Vector3 pos)
	{
		if (!isInet)
		{
			GetComponent<NetworkView>().RPC("GetDamageFromEnvRPC", RPCMode.All, damage, pos);
		}
		else
		{
			photonView.RPC("GetDamageFromEnvRPC", PhotonTargets.All, damage, pos);
		}
	}

	[RPC]
	[PunRPC]
	public void GetDamageFromEnvRPC(float damage, Vector3 pos)
	{
		StartCoroutine(Flash(myPlayerTransform.gameObject));
		if (!isMine || isKilled || isImmortality)
		{
			return;
		}
		if (pos != Vector3.zero)
		{
			ShowDamageDirection(pos);
		}
		if (Defs.isSoundFX)
		{
			NGUITools.PlaySound((!(curArmor > 0f) && !isMechActive) ? damagePlayerSound : damageArmorPlayerSound);
		}
		if (DamagePlayerAndCheckDeath(damage) && Defs.isMulti)
		{
			ImSuicide();
			if (!Defs.isCOOP)
			{
				SendImKilled();
			}
		}
	}

	public void GetDamageFromEnv(float damage, Vector3 pos)
	{
		if (isKilled || isImmortality)
		{
			return;
		}
		if (pos != Vector3.zero)
		{
			ShowDamageDirection(pos);
		}
		if (Defs.isSoundFX)
		{
			NGUITools.PlaySound((!(curArmor > 0f) && !isMechActive) ? damagePlayerSound : damageArmorPlayerSound);
		}
		if (DamagePlayerAndCheckDeath(damage) && Defs.isMulti)
		{
			ImSuicide();
			if (!Defs.isCOOP)
			{
				SendImKilled();
			}
		}
		if (Defs.isMulti)
		{
			SendStartFlashMine();
		}
		else
		{
			StartFlashRPC();
		}
	}

	public void KillSelf()
	{
		if ((isMulti && !isMine) || isKilled || CurHealth <= 0f)
		{
			return;
		}
		curArmor = 0f;
		CurHealth = 0f;
		if (Defs.isMulti)
		{
			ImSuicide();
			if (!Defs.isCOOP)
			{
				SendImKilled();
			}
		}
		else
		{
			StartFlash(mySkinName.gameObject);
		}
	}

	[RPC]
	[PunRPC]
	public void minusLiveFromZombiRPC(float live, Vector3 posZombi)
	{
		if (photonView.isMine && !isKilled && !isImmortality)
		{
			if (isMechActive)
			{
				MinusMechHealth(live);
			}
			else
			{
				float num = live - curArmor;
				if (num < 0f)
				{
					curArmor -= live;
					num = 0f;
				}
				else
				{
					curArmor = 0f;
				}
				CurHealth -= num;
			}
			ShowDamageDirection(posZombi);
		}
		StartCoroutine(Flash(myPlayerTransform.gameObject));
	}

	[RPC]
	[PunRPC]
	public void MinusLiveRPC(NetworkViewID idKiller, float minus, int _typeKills, int _typeWeapon, NetworkViewID idTurret, string weaponName)
	{
		MinusLiveRPCEffects(_typeKills);
		if (!isMine || isKilled || isImmortality)
		{
			return;
		}
		float num = 0f;
		if (isMechActive)
		{
			MinusMechHealth(minus);
		}
		else
		{
			num = minus - curArmor;
			if (num < 0f)
			{
				curArmor -= minus;
				num = 0f;
			}
			else
			{
				curArmor = 0f;
			}
		}
		if (!(CurHealth > 0f))
		{
			return;
		}
		CurHealth -= num;
		if (CurHealth <= 0f)
		{
			if (myKillAssistsLocal.Contains(idKiller))
			{
				myKillAssistsLocal.Remove(idKiller);
			}
			if (placemarkerMoveC != null)
			{
				placemarkerMoveC.isPlacemarker = false;
			}
			GetComponent<NetworkView>().RPC("Killed", RPCMode.All, idKiller, _typeKills, _typeWeapon, weaponName);
		}
		else if (!myKillAssistsLocal.Contains(idKiller))
		{
			myKillAssistsLocal.Add(idKiller);
		}
		SendSynhHealth(false);
		Vector3 zero = Vector3.zero;
		if (_typeKills != 8)
		{
			foreach (Player_move_c player in Initializer.players)
			{
				if (player.GetComponent<NetworkView>() != null && player.GetComponent<NetworkView>().viewID.Equals(idKiller))
				{
					zero = player.transform.position;
					ShowDamageDirection(zero);
					break;
				}
			}
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Turret");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.GetComponent<NetworkView>() != null && gameObject.GetComponent<NetworkView>().viewID.Equals(idTurret))
			{
				zero = gameObject.transform.position;
				ShowDamageDirection(zero);
				break;
			}
		}
	}

	public void MinusLive(int idKiller, float minus, TypeKills _typeKills, int _typeWeapon = 0, string weaponName = "", int idTurret = 0)
	{
		if (Defs.isDaterRegim || isImmortality)
		{
			return;
		}
		ProfileController.OnGameHit();
		if (_typeKills != TypeKills.turret && InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.ShowImpact();
		}
		if (!FriendsController.useBuffSystem || !BuffSystem.instance.haveBuffForWeapon(weaponName))
		{
			minus *= WeaponManager.sharedManager.myPlayerMoveC.damageBuff;
		}
		else
		{
			minus *= BuffSystem.instance.weaponBuffValue;
			Debug.Log("Buffed shot!");
		}
		minus /= protectionBuff;
		if (isMechActive)
		{
			if (MinusMechHealth(minus))
			{
				WeaponManager.sharedManager.myPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.deadMech);
				minus = 1000f;
				if (_typeKills != TypeKills.grenade && _typeKills != TypeKills.mech && _typeKills != TypeKills.turret)
				{
					try
					{
						WeaponManager.sharedManager.myPlayerMoveC.AddWeKillStatisctics((weaponName ?? string.Empty).Replace("(Clone)", string.Empty));
					}
					catch (Exception ex)
					{
						Debug.LogError("Exception we were killed AddWeKillStatisctics: " + ex);
					}
				}
			}
		}
		else if (synhHealth > 0f)
		{
			getLocalHurt = true;
			synhHealth -= minus;
			if (synhHealth < 0f)
			{
				synhHealth = 0f;
			}
			if (armorSynch > minus)
			{
				armorSynch -= minus;
			}
			else
			{
				armorSynch = 0f;
			}
			if (synhHealth <= 0f)
			{
				if (_typeKills != TypeKills.grenade && _typeKills != TypeKills.mech && _typeKills != TypeKills.turret)
				{
					try
					{
						WeaponManager.sharedManager.myPlayerMoveC.AddWeKillStatisctics((weaponName ?? string.Empty).Replace("(Clone)", string.Empty));
					}
					catch (Exception ex2)
					{
						Debug.LogError("Exception we were killed AddWeKillStatisctics: " + ex2);
					}
				}
				minus = 10000f;
				if (isCaptureFlag)
				{
					WeaponManager.sharedManager.myPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.deadWithFlag);
					if (!NetworkStartTable.LocalOrPasswordRoom())
					{
						QuestMediator.NotifyKillOtherPlayerWithFlag();
					}
				}
				if (Defs.isCapturePoints && WeaponManager.sharedManager.myPlayerMoveC != null)
				{
					for (int i = 0; i < CapturePointController.sharedController.basePointControllers.Length; i++)
					{
						if (CapturePointController.sharedController.basePointControllers[i].captureConmmand == (BasePointController.TypeCapture)WeaponManager.sharedManager.myPlayerMoveC.myCommand && CapturePointController.sharedController.basePointControllers[i].capturePlayers.Contains(this))
						{
							isRaiderMyPoint = true;
							break;
						}
					}
				}
				if (getLocalHurt)
				{
					getLocalHurt = false;
				}
				ImKilled(myPlayerTransform.position, myPlayerTransform.rotation, _typeWeapon);
				myPersonNetwork.StartAngel();
				if (Defs.isFlag && isCaptureFlag)
				{
					FlagController flagController = null;
					if (flag1.targetTrasform == flagPoint.transform)
					{
						flagController = flag1;
					}
					if (flag2.targetTrasform == flagPoint.transform)
					{
						flagController = flag2;
					}
					if (flagController != null)
					{
						flagController.SetNOCaptureRPC(myPlayerTransform.position, myPlayerTransform.rotation);
					}
				}
			}
		}
		photonView.RPC("MinusLiveRPCPhoton", PhotonTargets.Others, idKiller, minus, (int)_typeKills, _typeWeapon, idTurret, weaponName);
		MinusLiveRPCEffects((int)_typeKills);
	}

	public void MinusLive(NetworkViewID idKiller, float minus, TypeKills _typeKills, int _typeWeapon, string nameWeapon, [Optional] NetworkViewID idTurret)
		// you could remove [Optional] and just null it
	{
		nameWeapon = String.IsNullOrWhiteSpace(nameWeapon) ? "" : nameWeapon;
		if (!Defs.isDaterRegim)
		{
			if (InGameGUI.sharedInGameGUI != null)
			{
				InGameGUI.sharedInGameGUI.ShowImpact();
			}
			ProfileController.OnGameHit();
			getLocalHurt = true;
			GetComponent<NetworkView>().RPC("MinusLiveRPC", RPCMode.All, idKiller, minus, (int)_typeKills, _typeWeapon, idTurret, nameWeapon);
		}
	}

	[PunRPC]
	[RPC]
	public void MinusLiveRPCWithTurretPhoton(int idKiller, float minus, int _typeKills, int idTurret)
	{
		MinusLiveRPCPhoton(idKiller, minus, _typeKills, 0, idTurret, null);
	}

	[PunRPC]
	[RPC]
	public void MinusLiveRPCWithTurretPhoton(int idKiller, float minus, int _typeKills, int idTurret, string weaponName)
	{
		MinusLiveRPCPhoton(idKiller, minus, _typeKills, 0, idTurret, null);
	}

	private void MinusLiveRPCEffects(int _typeKills)
	{
		if (!Device.isPixelGunLow && !isDaterRegim && !isMine)
		{
			if (_typeKills == 2)
			{
				HitParticle currentParticle = HeadShotStackController.sharedController.GetCurrentParticle(false);
				if (currentParticle != null)
				{
					currentParticle.StartShowParticle(myPlayerTransform.position, myPlayerTransform.rotation, false);
				}
			}
			else
			{
				HitParticle currentParticle2 = HitStackController.sharedController.GetCurrentParticle(false);
				if (currentParticle2 != null)
				{
					currentParticle2.StartShowParticle(myPlayerTransform.position, myPlayerTransform.rotation, false);
				}
			}
		}
		if (Defs.isSoundFX)
		{
			base.gameObject.GetComponent<AudioSource>().PlayOneShot((curArmor > 0f || isMechActive) ? damageArmorPlayerSound : ((_typeKills != 2) ? damagePlayerSound : headShotSound));
		}
		StartCoroutine(Flash(myPlayerTransform.gameObject));
	}

	[RPC]
	[PunRPC]
	public void MinusLiveRPCPhoton(int idKiller, float minus, int _typeKills, int _typeWeapon, int idTurret, string weaponName)
	{
		MinusLiveRPCEffects(_typeKills);
		if (!isMine)
		{
			synhHealth -= minus;
			if (synhHealth < 0f)
			{
				synhHealth = 0f;
			}
			if (armorSynch > minus)
			{
				armorSynch -= minus;
			}
			else
			{
				armorSynch = 0f;
			}
		}
		if (!isMine || isKilled || isImmortality)
		{
			return;
		}
		float num = 0f;
		if (isMechActive)
		{
			MinusMechHealth(minus);
		}
		else
		{
			num = minus - curArmor;
			if (num < 0f)
			{
				curArmor -= minus;
				num = 0f;
			}
			else
			{
				curArmor = 0f;
			}
		}
		if (CurHealth > 0f)
		{
			CurHealth -= num;
			if (CurHealth <= 0f)
			{
				try
				{
					if (!WeaponManager.sharedManager.currentWeaponSounds.isGrenadeWeapon)
					{
						WeaponManager.sharedManager.myPlayerMoveC.AddWeWereKilledStatisctics((WeaponManager.sharedManager.currentWeaponSounds.name ?? string.Empty).Replace("(Clone)", string.Empty));
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception we were killed AddWeWereKilledStatisctics: " + ex);
				}
				if (myKillAssists.Contains(idKiller))
				{
					myKillAssists.Remove(idKiller);
				}
				if (placemarkerMoveC != null)
				{
					placemarkerMoveC.isPlacemarker = false;
				}
				this.photonView.RPC("KilledPhoton", PhotonTargets.All, idKiller, _typeKills, weaponName, _typeWeapon);
			}
			else if (!myKillAssists.Contains(idKiller))
			{
				myKillAssists.Add(idKiller);
			}
			SynhHealthRPC(CurHealth + curArmor, curArmor, false);
		}
		if (_typeKills != 8)
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < Initializer.players.Count; i++)
			{
				PhotonView photonView = Initializer.players[i].mySkinName.photonView;
				if (photonView != null && photonView.viewID == idKiller)
				{
					zero = Initializer.players[i].myPlayerTransform.position;
					ShowDamageDirection(zero);
					break;
				}
			}
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Turret");
		Vector3 zero2 = Vector3.zero;
		for (int j = 0; j < array.Length; j++)
		{
			PhotonView component = array[j].GetComponent<PhotonView>();
			if (component != null && component.viewID == idTurret)
			{
				zero2 = array[j].transform.position;
				ShowDamageDirection(zero2);
				break;
			}
		}
	}

	public void SendSynhHealth(bool isUp, PhotonPlayer player = null)
	{
		if (Defs.isInet)
		{
			if (player == null)
			{
				photonView.RPC("SynhHealthRPC", PhotonTargets.All, CurHealth + curArmor, curArmor, isUp);
			}
			else
			{
				photonView.RPC("SynhHealthRPC", player, CurHealth + curArmor, curArmor, isUp);
			}
		}
		else
		{
			GetComponent<NetworkView>().RPC("SynhHealthRPC", RPCMode.All, CurHealth + curArmor, curArmor, isUp);
		}
	}

	[PunRPC]
	[RPC]
	private void SynhHealth(float _synhHealth, bool isUp)
	{
		SynhHealthRPC(_synhHealth, (!(_synhHealth > 9f)) ? 0f : (_synhHealth - 9f), isUp);
	}

	[RPC]
	[PunRPC]
	private void SynhHealthRPC(float _synhHealth, float _synchArmor, bool isUp)
	{
		if (isMine)
		{
			synhHealth = _synhHealth;
		}
		else if (!isUp)
		{
			if (_synhHealth < synhHealth)
			{
				synhHealth = _synhHealth;
			}
			if (_synchArmor < armorSynch)
			{
				armorSynch = _synchArmor;
			}
		}
		else
		{
			synhHealth = _synhHealth;
			armorSynch = _synchArmor;
			isRaiderMyPoint = false;
		}
		if (synhHealth > 0f)
		{
			isStartAngel = false;
			myPersonNetwork.isStartAngel = false;
		}
	}

	private void ShowDamageDirection(Vector3 posDamage)
	{
		if (!isDaterRegim)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			Vector3 vector = posDamage - myPlayerTransform.position;
			float num = Mathf.Atan(vector.z / vector.x);
			num = num * 180f / (float)Math.PI;
			if (vector.x > 0f)
			{
				num = 90f - num;
			}
			if (vector.x < 0f)
			{
				num = 270f - num;
			}
			float y = myPlayerTransform.rotation.eulerAngles.y;
			float num2 = num - y;
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			if (num2 < -180f)
			{
				num2 += 360f;
			}
			if (inGameGUI != null)
			{
				inGameGUI.AddDamageTaken(num);
			}
			if (num2 > -45f && num2 <= 45f)
			{
				flag3 = true;
			}
			if (num2 < -45f && num2 >= -135f)
			{
				flag = true;
			}
			if (num2 > 45f && num2 <= 135f)
			{
				flag2 = true;
			}
			if (num2 < -135f || num2 >= 135f)
			{
				flag4 = true;
			}
			if (flag3)
			{
				timerShowUp = maxTimeSetTimerShow;
			}
			if (flag4)
			{
				timerShowDown = maxTimeSetTimerShow;
			}
			if (flag)
			{
				timerShowLeft = maxTimeSetTimerShow;
			}
			if (flag2)
			{
				timerShowRight = maxTimeSetTimerShow;
			}
		}
	}

	private void UpdateKillerInfo(Player_move_c killerPlayerMoveC, int killType)
	{
		_killerInfo.isGrenade = killType == 6;
		_killerInfo.isMech = killType == 10;
		_killerInfo.isTurret = killType == 8;
		SkinName skinName = killerPlayerMoveC.mySkinName;
		_killerInfo.nickname = skinName.NickName;
		if (killerPlayerMoveC.myTable != null)
		{
			NetworkStartTable component = killerPlayerMoveC.myTable.GetComponent<NetworkStartTable>();
			int myRanks = component.myRanks;
			if (myRanks > 0 && myRanks < expController.marks.Length)
			{
				_killerInfo.rankTex = ExperienceController.sharedController.marks[myRanks];
				_killerInfo.rank = myRanks;
			}
			if (component.myClanTexture != null)
			{
				_killerInfo.clanLogoTex = component.myClanTexture;
			}
			_killerInfo.clanName = component.myClanName;
		}
		_killerInfo.weapon = killerPlayerMoveC.currentWeapon;
		_killerInfo.skinTex = killerPlayerMoveC._skin;
		_killerInfo.hat = skinName.currentHat;
		_killerInfo.mask = skinName.currentMask;
		_killerInfo.armor = skinName.currentArmor;
		_killerInfo.cape = skinName.currentCape;
		_killerInfo.capeTex = skinName.currentCapeTex;
		_killerInfo.boots = skinName.currentBoots;
		_killerInfo.mechUpgrade = killerPlayerMoveC.mechUpgrade;
		_killerInfo.turretUpgrade = killerPlayerMoveC.turretUpgrade;
		_killerInfo.killerTransform = killerPlayerMoveC.myPlayerTransform;
		_killerInfo.healthValue = Mathf.CeilToInt(_killerInfo.isMech ? killerPlayerMoveC.liveMech : ((!(killerPlayerMoveC.synhHealth - killerPlayerMoveC.armorSynch > 0f)) ? 0f : (killerPlayerMoveC.synhHealth - killerPlayerMoveC.armorSynch)));
		_killerInfo.armorValue = Mathf.CeilToInt(killerPlayerMoveC.armorSynch);
	}

	[RPC]
	[PunRPC]
	public void Killed(NetworkViewID idKiller, int _typeKill, int _typeWeapon, string weaponName)
	{
		if (_weaponManager == null || _weaponManager.myPlayer == null)
		{
			return;
		}
		string nick = string.Empty;
		string empty = string.Empty;
		empty = mySkinName.NickName;
		foreach (Player_move_c player in Initializer.players)
		{
			if (!player.mySkinName.GetComponent<NetworkView>().viewID.Equals(idKiller))
			{
				continue;
			}
			SkinName skinName = player.mySkinName;
			nick = skinName.NickName;
			if (isMine && Defs.isJetpackEnabled && !mySkinName.character.isGrounded)
			{
				player.AddScoreDuckHunt();
			}
			if ((bool)_weaponManager && player == _weaponManager.myPlayerMoveC)
			{
				ProfileController.OnGameTotalKills();
				PlayerScoreController playerScoreController = player.myScoreController;
				int @event;
				switch (_typeKill)
				{
				case 6:
					@event = 60;
					break;
				case 9:
					@event = 8;
					break;
				case 2:
					@event = 10;
					break;
				case 3:
					@event = 61;
					break;
				default:
					@event = 9;
					break;
				}
				playerScoreController.AddScoreOnEvent((PlayerEventScoreController.ScoreEvent)@event);
				if (Defs.isJetpackEnabled && !_weaponManager.myPlayerMoveC.mySkinName.character.isGrounded && _typeKill != 6 && _typeKill != 8)
				{
					player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.deathFromAbove);
				}
				if (player.isRocketJump && _typeKill != 6 && _typeKill != 8)
				{
					player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.rocketJumpKill);
				}
				if (multiKill > 1)
				{
					if (!NetworkStartTable.LocalOrPasswordRoom())
					{
						QuestMediator.NotifyBreakSeries();
					}
					if (multiKill == 2)
					{
						player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill2);
					}
					else if (multiKill == 3)
					{
						player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill3);
					}
					else if (multiKill == 4)
					{
						player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill4);
					}
					else if (multiKill == 5)
					{
						player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill5);
					}
					else if (multiKill < 10)
					{
						player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill6);
					}
					else if (multiKill < 20)
					{
						player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill10);
					}
					else if (multiKill < 50)
					{
						player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill20);
					}
					else
					{
						player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill50);
					}
				}
				if (isInvisible)
				{
					player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.invisibleKill);
				}
				if (isPlacemarker)
				{
					player.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.revenge);
				}
				if (_typeKill != 6 && _typeKill != 8 && _typeKill != 10)
				{
					GameObject gameObject = Resources.Load("Weapons/" + weaponName) as GameObject;
					if (gameObject != null && gameObject.GetComponent<WeaponSounds>() != null)
					{
						AddCountSerials(gameObject.GetComponent<WeaponSounds>().categoryNabor - 1, player);
					}
				}
				player.ImKill(idKiller, _typeKill);
				if (Equals(_weaponManager.myPlayerMoveC.placemarkerMoveC))
				{
					_weaponManager.myPlayerMoveC.placemarkerMoveC = null;
					isPlacemarker = false;
				}
				if (getLocalHurt)
				{
					getLocalHurt = false;
				}
			}
			if (isMine)
			{
				player.isPlacemarker = true;
				placemarkerMoveC = player;
			}
			UpdateKillerInfo(player, _typeKill);
			break;
		}
		ImKilled(myPlayerTransform.position, myPlayerTransform.rotation, _typeWeapon);
		if ((bool)_weaponManager && _weaponManager.myPlayer != null)
		{
			if (Device.isPixelGunLow && !Defs.isHunger && !Defs.isDaterRegim && _weaponManager.myPlayerMoveC.mySkinName.GetComponent<NetworkView>().viewID != idKiller && _weaponManager.GunsForPixelGunLow.ContainsKey(weaponName) && (weaponName == null || !(weaponName == WeaponManager.PistolWN) || !(WeaponManager.sharedManager != null) || WeaponManager.sharedManager._currentFilterMap != 2))
			{
				_weaponManager.myPlayerMoveC.AddSystemMessage(nick, _typeKill, empty, _weaponManager.GunsForPixelGunLow[weaponName]);
			}
			else
			{
				_weaponManager.myPlayerMoveC.AddSystemMessage(nick, _typeKill, empty, weaponName);
			}
		}
	}

	[PunRPC]
	[RPC]
	public void KilledPhoton(int idKiller, int _typekill)
	{
		KilledPhoton(idKiller, _typekill, string.Empty);
	}

	[PunRPC]
	[RPC]
	public void KilledPhoton(int idKiller, int _typekill, string weaponName)
	{
		KilledPhoton(idKiller, _typekill, weaponName, 0);
	}

	[RPC]
	[PunRPC]
	public void imDeath(string _name)
	{
		if (!(_weaponManager == null) && !(_weaponManager.myPlayer == null))
		{
			_weaponManager.myPlayerMoveC.AddSystemMessage(_name, 1);
		}
	}

	public void sendImDeath(string _name)
	{
		if (!isInet)
		{
			GetComponent<NetworkView>().RPC("imDeath", RPCMode.All, _name);
		}
		else
		{
			photonView.RPC("imDeath", PhotonTargets.All, _name);
		}
		_killerInfo.isSuicide = true;
	}

	[PunRPC]
	[RPC]
	public void KilledPhoton(int idKiller, int _typekill, string weaponName, int _typeWeapon)
	{
		if (_weaponManager == null || _weaponManager.myPlayer == null)
		{
			return;
		}
		string nick = string.Empty;
		string nickName = mySkinName.NickName;
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			if (!(Initializer.players[i].mySkinName.photonView != null) || Initializer.players[i].mySkinName.photonView.viewID != idKiller)
			{
				continue;
			}
			SkinName skinName = Initializer.players[i].mySkinName;
			Player_move_c player_move_c = Initializer.players[i];
			nick = skinName.NickName;
			if (isMine && Defs.isJetpackEnabled && !mySkinName.character.isGrounded)
			{
				player_move_c.AddScoreDuckHunt();
			}
			if (_weaponManager != null && Initializer.players[i] == _weaponManager.myPlayerMoveC)
			{
				ProfileController.OnGameTotalKills();
				if (FriendsController.useBuffSystem)
				{
					BuffSystem.instance.KillInteraction();
				}
				else
				{
					KillRateCheck.instance.IncrementKills();
				}
				WeaponManager.sharedManager.myNetworkStartTable.IncrementKills();
				if (isRaiderMyPoint)
				{
					WeaponManager.sharedManager.myPlayerMoveC.SendHouseKeeperEvent();
					isRaiderMyPoint = false;
				}
				if (Defs.isJetpackEnabled && !_weaponManager.myPlayerMoveC.mySkinName.character.isGrounded && _typekill != 6 && _typekill != 8)
				{
					player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.deathFromAbove);
				}
				if (player_move_c.isRocketJump && _typekill != 6 && _typekill != 8)
				{
					player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.rocketJumpKill);
				}
				if (_typekill != 6 && _typekill != 8 && _typekill != 10)
				{
					GameObject gameObject = Resources.Load("Weapons/" + weaponName) as GameObject;
					if (gameObject != null && gameObject.GetComponent<WeaponSounds>() != null)
					{
						AddCountSerials(gameObject.GetComponent<WeaponSounds>().categoryNabor - 1, player_move_c);
					}
				}
				if (multiKill > 1)
				{
					if (!NetworkStartTable.LocalOrPasswordRoom())
					{
						QuestMediator.NotifyBreakSeries();
					}
					if (multiKill == 2)
					{
						player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill2);
					}
					else if (multiKill == 3)
					{
						player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill3);
					}
					else if (multiKill == 4)
					{
						player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill4);
					}
					else if (multiKill == 5)
					{
						player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill5);
					}
					else if (multiKill < 10)
					{
						player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill6);
					}
					else if (multiKill < 20)
					{
						player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill10);
					}
					else if (multiKill < 50)
					{
						player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill20);
					}
					else
					{
						player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killMultyKill50);
					}
				}
				if (!Defs.isFlag)
				{
					player_move_c.ImKill(idKiller, _typekill);
				}
				ShopNGUIController.CategoryNames weaponSlot = (ShopNGUIController.CategoryNames)(-1);
				ItemRecord byPrefabName = ItemDb.GetByPrefabName(weaponName);
				if (byPrefabName != null)
				{
					int num = PromoActionsGUIController.CatForTg(byPrefabName.Tag);
					weaponSlot = (ShopNGUIController.CategoryNames)num;
				}
				if (!NetworkStartTable.LocalOrPasswordRoom())
				{
					QuestMediator.NotifyKillOtherPlayer(ConnectSceneNGUIController.regim, weaponSlot, _typekill == 2, _typekill == 6, isPlacemarker);
				}
				PlayerScoreController playerScoreController = player_move_c.myScoreController;
				int @event;
				switch (_typekill)
				{
				case 6:
					@event = 60;
					break;
				case 9:
					@event = 8;
					break;
				case 2:
					@event = 10;
					break;
				case 3:
					@event = 61;
					break;
				default:
					@event = 9;
					break;
				}
				playerScoreController.AddScoreOnEvent((PlayerEventScoreController.ScoreEvent)@event);
				if (isInvisible)
				{
					player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.invisibleKill);
				}
				if (isPlacemarker)
				{
					player_move_c.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.revenge);
				}
				if (Equals(_weaponManager.myPlayerMoveC.placemarkerMoveC))
				{
					_weaponManager.myPlayerMoveC.placemarkerMoveC = null;
					isPlacemarker = false;
				}
				if (getLocalHurt)
				{
					getLocalHurt = false;
				}
			}
			if (isMine)
			{
				player_move_c.isPlacemarker = true;
				placemarkerMoveC = player_move_c;
			}
			UpdateKillerInfo(Initializer.players[i], _typekill);
			break;
		}
		ImKilled(myPlayerTransform.position, myPlayerTransform.rotation, _typeWeapon);
		if ((bool)_weaponManager && _weaponManager.myPlayerMoveC != null)
		{
			if (Device.isPixelGunLow && !Defs.isHunger && !Defs.isDaterRegim && _weaponManager.myPlayerMoveC.mySkinName.photonView.viewID != idKiller && _weaponManager.GunsForPixelGunLow.ContainsKey(weaponName) && (weaponName == null || !(weaponName == WeaponManager.PistolWN) || !(WeaponManager.sharedManager != null) || WeaponManager.sharedManager._currentFilterMap != 2))
			{
				_weaponManager.myPlayerMoveC.AddSystemMessage(nick, _typekill, nickName, _weaponManager.GunsForPixelGunLow[weaponName]);
			}
			else
			{
				_weaponManager.myPlayerMoveC.AddSystemMessage(nick, _typekill, nickName, weaponName);
			}
		}
	}

	private void AddCountSerials(int categoryNabor, Player_move_c killerPlayerMoveC)
	{
		killerPlayerMoveC.counterSerials[categoryNabor]++;
		switch (killerPlayerMoveC.counterSerials[categoryNabor])
		{
		case 1:
			if (categoryNabor == 2)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.melee);
			}
			break;
		case 2:
			if (categoryNabor == 2)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.melee2);
			}
			break;
		case 3:
			if (categoryNabor == 0)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.primary1);
			}
			if (categoryNabor == 1)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.backup1);
			}
			if (categoryNabor == 2)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.melee3);
			}
			if (categoryNabor == 3)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.special1);
			}
			if (categoryNabor == 4)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.sniper1);
			}
			if (categoryNabor == 5)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.premium1);
			}
			break;
		case 5:
			if (categoryNabor == 0)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.primary2);
			}
			if (categoryNabor == 1)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.backup2);
			}
			if (categoryNabor == 2)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.melee5);
			}
			if (categoryNabor == 3)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.special2);
			}
			if (categoryNabor == 4)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.sniper2);
			}
			if (categoryNabor == 5)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.premium2);
			}
			break;
		case 7:
			if (categoryNabor == 0)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.primary3);
			}
			if (categoryNabor == 1)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.backup3);
			}
			if (categoryNabor == 2)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.melee7);
			}
			if (categoryNabor == 3)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.special3);
			}
			if (categoryNabor == 4)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.sniper3);
			}
			if (categoryNabor == 5)
			{
				killerPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.premium3);
			}
			break;
		case 4:
		case 6:
			break;
		}
	}

	public void hit(float dam, Vector3 posEnemy, bool damageColliderHit = false)
	{
		if (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None)
		{
			if (isMechActive)
			{
				MinusMechHealth(dam);
			}
			else if (curArmor >= dam)
			{
				curArmor -= dam;
			}
			else
			{
				CurHealth -= dam - curArmor;
				curArmor = 0f;
				CurrentCampaignGame.withoutHits = false;
			}
		}
		if (!damageColliderHit)
		{
			ShowDamageDirection(posEnemy);
		}
		if (!damageShown)
		{
			StartCoroutine(FlashWhenHit());
		}
	}

	public void SendImKilled()
	{
		if (Defs.isInet)
		{
			photonView.RPC("ImKilled", PhotonTargets.All, myPlayerTransform.position, myPlayerTransform.rotation, 0);
			SendSynhHealth(false);
		}
	}

	[RPC]
	[PunRPC]
	private void ImKilled(Vector3 pos, Quaternion rot)
	{
		ImKilled(pos, rot, 0);
	}

	[PunRPC]
	[RPC]
	private void ImKilled(Vector3 pos, Quaternion rot, int _typeDead = 0)
	{
		if (Device.isPixelGunLow)
		{
			_typeDead = 0;
		}
		if (!isStartAngel || Defs.isCOOP)
		{
			isStartAngel = true;
			if (Defs.inComingMessagesCounter < 15)
			{
				PlayerDeadController currentParticle = PlayerDeadStackController.sharedController.GetCurrentParticle(false);
				if (currentParticle != null)
				{
					currentParticle.StartShow(pos, rot, _typeDead, false, _skin);
				}
				if (Defs.isSoundFX)
				{
					base.gameObject.GetComponent<AudioSource>().PlayOneShot(deadPlayerSound);
				}
			}
		}
		if (!isMine && getLocalHurt)
		{
			WeaponManager.sharedManager.myPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.killAssist);
			getLocalHurt = false;
		}
	}

	private IEnumerator FlashWhenHit()
	{
		damageShown = true;
		Color rgba = damage.GetComponent<GUITexture>().color;
		rgba.a = 0f;
		damage.GetComponent<GUITexture>().color = rgba;
		float danageTime = 0.15f;
		yield return StartCoroutine(Fade(0f, 1f, danageTime, damage));
		yield return new WaitForSeconds(0.01f);
		yield return StartCoroutine(Fade(1f, 0f, danageTime, damage));
		damageShown = false;
	}

	private IEnumerator FlashWhenDead()
	{
		damageShown = true;
		Color rgba = damage.GetComponent<GUITexture>().color;
		rgba.a = 0f;
		damage.GetComponent<GUITexture>().color = rgba;
		float danageTime = 0.15f;
		yield return StartCoroutine(Fade(0f, 1f, danageTime, damage));
		while (isDeadFrame)
		{
			yield return null;
		}
		yield return StartCoroutine(Fade(1f, 0f, danageTime / 3f, damage));
		damageShown = false;
	}

	private IEnumerator KillCam()
	{
		ProfileController.OnGameDeath();
		if (FriendsController.useBuffSystem)
		{
			BuffSystem.instance.DeathInteraction();
		}
		else
		{
			KillRateCheck.instance.IncrementDeath();
		}
		myNetworkStartTable.IncrementDeath();
		killedInMatch = true;
		CameraSceneController.sharedController.killCamController.lastDistance = 1f;
		CameraSceneController.sharedController.SetTargetKillCam(_killerInfo.killerTransform);
		InGameGUI.sharedInGameGUI.respawnWindow.Show(_killerInfo);
		InGameGUI.sharedInGameGUI.characterDrag.SetActive(false);
		InGameGUI.sharedInGameGUI.cameraDrag.SetActive(true);
		float _timerKillCam = 0f;
		Defs.inRespawnWindow = true;
		SkinName skinNameComponent = _killerInfo.killerTransform.GetComponent<SkinName>();
		while (Defs.inRespawnWindow && _killerInfo.killerTransform != null && _killerInfo.killerTransform.position.y > -5000f && !skinNameComponent.playerMoveC.isKilled)
		{
			yield return null;
			_timerKillCam += Time.deltaTime;
		}
		InGameGUI.sharedInGameGUI.characterDrag.SetActive(true);
		InGameGUI.sharedInGameGUI.cameraDrag.SetActive(false);
		if (Defs.inRespawnWindow)
		{
			RespawnWindow.Instance.ShowCharacter(killerInfo);
		}
		CameraSceneController.sharedController.SetTargetKillCam();
	}

	private void SetMapCameraActive(bool active)
	{
		InGameGUI.sharedInGameGUI.SetInterfaceVisible(!active);
		Camera component = Initializer.Instance.tc.GetComponent<Camera>();
		Camera camera = myCamera;
		component.gameObject.SetActive(active);
		camera.gameObject.SetActive(!active);
		Camera camera2 = (NickLabelController.currentCamera = ((!active) ? camera : component));
	}

	[Obfuscation(Exclude = true)]
	private void SetNoKilled()
	{
		isKilled = false;
		resetMultyKill();
	}

	[Obfuscation(Exclude = true)]
	private void ChangePositionAfterRespawn()
	{
		myPlayerTransform.position += Vector3.forward * 0.01f;
	}

	public void RespawnPlayer()
	{
		Defs.inRespawnWindow = false;
		respawnedForGUI = true;
		SetMapCameraActive(false);
		_killerInfo.Reset();
		Func<bool> func = () => _pauser != null && _pauser.paused;
		if (base.transform.parent == null)
		{
			Debug.Log("transform.parent == null");
			return;
		}
		myPlayerTransform.localScale = new Vector3(1f, 1f, 1f);
		myTransform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
		if (isHunger || Defs.isRegimVidosDebug)
		{
			myTable.GetComponent<NetworkStartTable>().ImDeadInHungerGames();
			PhotonNetwork.Destroy(myPlayerTransform.gameObject);
			return;
		}
		InitiailizeIcnreaseArmorEffectFlags();
		isDeadFrame = false;
		isImmortality = true;
		timerImmortality = maxTimerImmortality;
		SetNoKilled();
		if (_weaponManager.myPlayer == null)
		{
			Debug.Log("_weaponManager.myPlayer == null");
			return;
		}
		_weaponManager.myPlayerMoveC.mySkinName.camPlayer.transform.parent = _weaponManager.myPlayer.transform;
		if (!func())
		{
			if (JoystickController.leftJoystick != null)
			{
				JoystickController.leftJoystick.transform.parent.gameObject.SetActive(true);
			}
			if (JoystickController.rightJoystick != null)
			{
				JoystickController.rightJoystick.gameObject.SetActive(true);
				JoystickController.rightJoystick._isFirstFrame = false;
			}
		}
		if (JoystickController.leftJoystick != null)
		{
			JoystickController.leftJoystick.SetJoystickActive(true);
		}
		if (JoystickController.rightJoystick != null)
		{
			JoystickController.rightJoystick.MakeActive();
		}
		if (JoystickController.leftTouchPad != null)
		{
			JoystickController.leftTouchPad.SetJoystickActive(true);
		}
		if (JoystickController.rightJoystick != null)
		{
			if (inGameGUI != null)
			{
				inGameGUI.BlinkNoAmmo(0);
			}
			JoystickController.rightJoystick.HasAmmo();
		}
		else
		{
			Debug.Log("JoystickController.rightJoystick = null");
		}
		CurHealth = MaxHealth;
		Wear.RenewCurArmor(TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier));
		CurrentBaseArmor = EffectsController.ArmorBonus;
		zoneCreatePlayer = GameObject.FindGameObjectsWithTag(isCOOP ? "MultyPlayerCreateZoneCOOP" : (isCompany ? ("MultyPlayerCreateZoneCommand" + myCommand) : (Defs.isFlag ? ("MultyPlayerCreateZoneFlagCommand" + myCommand) : ((!Defs.isCapturePoints) ? "MultyPlayerCreateZone" : ("MultyPlayerCreateZonePointZone" + myCommand)))));
		GameObject gameObject = zoneCreatePlayer[UnityEngine.Random.Range(0, zoneCreatePlayer.Length - 1)];
		BoxCollider component = gameObject.GetComponent<BoxCollider>();
		Vector2 vector = new Vector2(component.size.x * gameObject.transform.localScale.x, component.size.z * gameObject.transform.localScale.z);
		Rect rect = new Rect(gameObject.transform.position.x - vector.x / 2f, gameObject.transform.position.z - vector.y / 2f, vector.x, vector.y);
		Vector3 position = new Vector3(rect.x + UnityEngine.Random.Range(0f, rect.width), gameObject.transform.position.y, rect.y + UnityEngine.Random.Range(0f, rect.height));
		Quaternion rotation = gameObject.transform.rotation;
		myPlayerTransform.position = position;
		myPlayerTransform.rotation = rotation;
		if (Storager.getInt("GrenadeID", false) <= 0)
		{
			Storager.setInt("GrenadeID", 1, false);
		}
		if (myCurrentWeaponSounds != null && myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>() != null)
		{
			myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().Play();
		}
		Vector3 eulerAngles = myCamera.transform.rotation.eulerAngles;
		myCamera.transform.rotation = Quaternion.Euler(0f, eulerAngles.y, eulerAngles.z);
		Invoke("ChangePositionAfterRespawn", 0.01f);
		foreach (Weapon allAvailablePlayerWeapon in _weaponManager.allAvailablePlayerWeapons)
		{
			allAvailablePlayerWeapon.currentAmmoInClip = allAvailablePlayerWeapon.weaponPrefab.GetComponent<WeaponSounds>().ammoInClip;
			allAvailablePlayerWeapon.currentAmmoInBackpack = allAvailablePlayerWeapon.weaponPrefab.GetComponent<WeaponSounds>().InitialAmmoWithEffectsApplied;
		}
		if (WeaponManager.sharedManager != null)
		{
			for (int i = 0; i < WeaponManager.sharedManager.playerWeapons.Count; i++)
			{
				WeaponSounds component2 = (WeaponManager.sharedManager.playerWeapons[i] as Weapon).weaponPrefab.GetComponent<WeaponSounds>();
				if (component2 != null && (!component2.isMelee || component2.isShotMelee))
				{
					WeaponManager.sharedManager.ReloadWeaponFromSet(i);
				}
			}
		}
		EffectsController.SlowdownCoeff = 1f;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && showGrenadeHint && ++respawnCountForTraining == 2)
		{
			HintController.instance.ShowHintByName("use_grenade", 5f);
			respawnCountForTraining = 0;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && showChangeWeaponHint)
		{
			HintController.instance.ShowHintByName("change_weapon", 5f);
		}
	}

	public void HideChangeWeaponTrainingHint()
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && showChangeWeaponHint)
		{
			showChangeWeaponHint = false;
			HintController.instance.HideHintByName("change_weapon");
		}
	}

	[PunRPC]
	[RPC]
	private void CountKillsCommandSynch(int _blue, int _red)
	{
		GlobalGameController.countKillsBlue = _blue;
		GlobalGameController.countKillsRed = _red;
	}

	[RPC]
	[PunRPC]
	private void plusCountKillsCommand(int _command)
	{
		Debug.Log("plusCountKillsCommand: " + _command);
		if (_command == 1)
		{
			if ((bool)_weaponManager && (bool)_weaponManager.myPlayer)
			{
				_weaponManager.myPlayerMoveC.countKillsCommandBlue++;
			}
			else
			{
				GlobalGameController.countKillsBlue++;
			}
		}
		if (_command == 2)
		{
			if ((bool)_weaponManager && (bool)_weaponManager.myPlayer)
			{
				_weaponManager.myPlayerMoveC.countKillsCommandRed++;
			}
			else
			{
				GlobalGameController.countKillsRed++;
			}
		}
	}

	public void addMultyKill()
	{
		multiKill++;
		if (multiKill <= 1)
		{
			return;
		}
		PlayerEventScoreController.ScoreEvent scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill6;
		if (multiKill > 1 && !NetworkStartTable.LocalOrPasswordRoom())
		{
			QuestMediator.NotifyMakeSeries();
		}
		switch (multiKill)
		{
		case 2:
			scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill2;
			myScoreController.AddScoreOnEvent(scoreEvent);
			break;
		case 3:
			scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill3;
			myScoreController.AddScoreOnEvent(scoreEvent);
			break;
		case 4:
			scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill4;
			myScoreController.AddScoreOnEvent(scoreEvent);
			break;
		case 5:
			scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill5;
			myScoreController.AddScoreOnEvent(scoreEvent);
			break;
		case 6:
			scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill6;
			myScoreController.AddScoreOnEvent(scoreEvent);
			break;
		case 10:
			scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill10;
			myScoreController.AddScoreOnEvent(scoreEvent);
			break;
		case 20:
			scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill20;
			myScoreController.AddScoreOnEvent(scoreEvent);
			break;
		case 50:
			scoreEvent = PlayerEventScoreController.ScoreEvent.multyKill50;
			myScoreController.AddScoreOnEvent(scoreEvent);
			break;
		}
		if (Defs.isMulti)
		{
			if (Defs.isInet)
			{
				photonView.RPC("ShowMultyKillRPC", PhotonTargets.Others, multiKill);
			}
			else
			{
				GetComponent<NetworkView>().RPC("ShowMultyKillRPC", RPCMode.Others, multiKill);
			}
		}
	}

	[RPC]
	[PunRPC]
	public void ShowMultyKillRPC(int countMulty)
	{
		multiKill = countMulty;
	}

	public void resetMultyKill()
	{
		multiKill = 0;
		for (int i = 0; i < counterSerials.Length; i++)
		{
			counterSerials[i] = 0;
		}
	}

	public void ImKill(NetworkViewID idKiller, int _typeKill)
	{
		countKills++;
		GlobalGameController.CountKills = countKills;
		CheckRookieKillerAchievement();
		addMultyKill();
		if (isCompany)
		{
			if (myCommand == 1)
			{
				countKillsCommandBlue++;
				if (isInet)
				{
					photonView.RPC("plusCountKillsCommand", PhotonTargets.Others, 1);
				}
				else
				{
					GetComponent<NetworkView>().RPC("plusCountKillsCommand", RPCMode.Others, 1);
				}
			}
			if (myCommand == 2)
			{
				countKillsCommandRed++;
				if (isInet)
				{
					photonView.RPC("plusCountKillsCommand", PhotonTargets.Others, 2);
				}
				else
				{
					GetComponent<NetworkView>().RPC("plusCountKillsCommand", RPCMode.Others, 2);
				}
			}
		}
		_weaponManager.myNetworkStartTable.CountKills = countKills;
		_weaponManager.myNetworkStartTable.SynhCountKills();
	}

	public void ImKill(int idKiller, int _typeKill)
	{
		if (WeaponManager.sharedManager == null)
		{
			Debug.LogWarning("WeaponManager.sharedManager == null");
		}
		else
		{
			WeaponSounds currentWeaponSounds = WeaponManager.sharedManager.currentWeaponSounds;
			if (currentWeaponSounds == null)
			{
				Debug.LogWarning("ws == null");
			}
			else
			{
				Initializer initializer = UnityEngine.Object.FindObjectOfType<Initializer>();
				if (initializer == null)
				{
					Debug.LogWarning("initializer == null");
				}
				else
				{
					string weapon = ((_typeKill != 6) ? currentWeaponSounds.shopName : "GRENADE");
					initializer.IncrementKillCountForWeapon(weapon);
				}
			}
		}
		countKills++;
		GlobalGameController.CountKills = countKills;
		CheckRookieKillerAchievement();
		addMultyKill();
		if (isCompany)
		{
			if (myCommand == 1)
			{
				countKillsCommandBlue++;
				if (isInet)
				{
					photonView.RPC("plusCountKillsCommand", PhotonTargets.Others, 1);
				}
				else
				{
					GetComponent<NetworkView>().RPC("plusCountKillsCommand", RPCMode.Others, 1);
				}
			}
			if (myCommand == 2)
			{
				countKillsCommandRed++;
				if (isInet)
				{
					photonView.RPC("plusCountKillsCommand", PhotonTargets.Others, 2);
				}
				else
				{
					GetComponent<NetworkView>().RPC("plusCountKillsCommand", RPCMode.Others, 2);
				}
			}
		}
		_weaponManager.myNetworkStartTable.CountKills = countKills;
		_weaponManager.myNetworkStartTable.SynhCountKills();
		if (isHunger && Initializer.players.Count == 1)
		{
			if (Defs.isHunger)
			{
				int val = Storager.getInt(Defs.RatingHunger, false) + 1;
				Storager.setInt(Defs.RatingHunger, val, false);
			}
			photonView.RPC("pobedaPhoton", PhotonTargets.All, idKiller, myCommand);
			int num = Storager.getInt("Rating", false) + 1;
			Storager.setInt("Rating", num, false);
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.TryIncrementWinCountTimestamp();
			}
			FriendsController.sharedController.wins.Value = num;
			FriendsController.sharedController.SendOurData();
			_weaponManager.myNetworkStartTable.isIwin = true;
		}
	}

	private void CheckRookieKillerAchievement()
	{
		int num = oldKilledPlayerCharactersCount + 1;
		if (num <= 15)
		{
			Storager.setInt("KilledPlayerCharactersCount", num, false);
		}
		oldKilledPlayerCharactersCount = num;
		if (!Social.localUser.authenticated || Storager.hasKey("RookieKillerAchievmentCompleted") || num < 15)
		{
			return;
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			GpgFacade.Instance.IncrementAchievement("CgkIr8rGkPIJEAIQBw", 1, delegate(bool success)
			{
				Debug.Log("Achievement Rookie Killer incremented: " + success);
			});
		}
		Storager.setInt("RookieKillerAchievmentCompleted", 1, false);
	}

	public void AddScoreDuckHunt()
	{
		if (Defs.isInet)
		{
			photonView.RPC("AddScoreDuckHuntRPC", PhotonTargets.All);
		}
		else
		{
			GetComponent<NetworkView>().RPC("AddScoreDuckHuntRPC", RPCMode.All);
		}
	}

	[RPC]
	[PunRPC]
	public void AddScoreDuckHuntRPC()
	{
		if (isMine)
		{
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.duckHunt);
		}
	}

	[RPC]
	[PunRPC]
	public void pobeda(NetworkViewID idKiller)
	{
		foreach (Player_move_c player in Initializer.players)
		{
			if (idKiller.Equals(player.mySkinName.GetComponent<NetworkView>().viewID))
			{
				nickPobeditel = player.mySkinName.NickName;
			}
		}
		if ((bool)_weaponManager && (bool)_weaponManager.myTable)
		{
			_weaponManager.myNetworkStartTable.win(nickPobeditel);
		}
	}

	[RPC]
	[PunRPC]
	public void pobedaPhoton(int idKiller, int _command)
	{
		foreach (Player_move_c player in Initializer.players)
		{
			if (idKiller == player.mySkinName.photonView.viewID)
			{
				nickPobeditel = player.mySkinName.NickName;
			}
		}
		if (_weaponManager != null && _weaponManager.myTable != null)
		{
			_weaponManager.myNetworkStartTable.win(nickPobeditel, _command);
		}
		else
		{
			Debug.Log("_weaponManager.myTable==null");
		}
	}

	public void SendMySpotEvent()
	{
		countMySpotEvent++;
		if (countMySpotEvent == 1)
		{
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.mySpotPoint);
		}
		if (countMySpotEvent == 2)
		{
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.unstoppablePoint);
		}
		if (countMySpotEvent >= 3)
		{
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.monopolyPoint);
		}
	}

	private void ResetMySpotEvent()
	{
		countMySpotEvent = 0;
	}

	private void ProvideHealth(string inShopId)
	{
		CurHealth = MaxHealth;
		CurrentCampaignGame.withoutHits = true;
	}

	public Vector3 GetPointAutoAim(Vector3 _posTo)
	{
		if (timerUpdatePointAutoAi < 0f)
		{
			rayAutoAim = myCamera.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
			RaycastHit hitInfo;
			if (Physics.Raycast(rayAutoAim, out hitInfo, 300f, Tools.AllWithoutDamageCollidersMaskAndWithoutRocket))
			{
				if (hitInfo.collider.gameObject.name.Equals("Rocket(Clone)"))
				{
					Debug.Log("Rocket(Clone)");
				}
				pointAutoAim = hitInfo.point;
			}
			else
			{
				pointAutoAim = Vector3.down * 10000f;
			}
			timerUpdatePointAutoAi = 0.2f;
		}
		if (pointAutoAim.y < -1000f)
		{
			return rayAutoAim.GetPoint(Vector3.Magnitude(myCamera.transform.position - _posTo));
		}
		return pointAutoAim;
	}

	public void ShotUnPressed()
	{
		if (isMulti && isMine)
		{
			if (isInet)
			{
				photonView.RPC("StartShootLoopRPC", PhotonTargets.Others, false);
			}
			else
			{
				_networkView.RPC("StartShootLoopRPC", RPCMode.Others, false);
			}
		}
		isShootingLoop = false;
		ctsShootLoop.Cancel();
		Animation component = myCurrentWeaponSounds.animationObject.GetComponent<Animation>();
		if (component.IsPlaying("Shoot"))
		{
			component.Stop();
		}
		else if (component.IsPlaying("Shoot_start"))
		{
			float num = component["Shoot_start"].length - component["Shoot_start"].time;
			component.Stop();
			component["Shoot_end"].time = component["Shoot_start"].length - component["Shoot_start"].time;
		}
		if (component["Shoot_end"] != null)
		{
			component.Play("Shoot_end");
		}
		if (Defs.isSoundFX)
		{
			myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().clip = myCurrentWeaponSounds.idle;
			myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().Play();
		}
	}

	public void StartLoopShot()
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (isMulti && isMine)
		{
			if (isInet)
			{
				photonView.RPC("StartShootLoopRPC", PhotonTargets.Others, true);
			}
			else
			{
				_networkView.RPC("StartShootLoopRPC", RPCMode.Others, true);
			}
		}
		isShootingLoop = true;
		Animation component = myCurrentWeaponSounds.animationObject.GetComponent<Animation>();
		float lengthAnimShootDown = 0f;
		if (component.GetClip("Shoot_start") != null)
		{
			if (component.IsPlaying("Shoot_end"))
			{
				lengthAnimShootDown = component["Shoot_end"].length - component["Shoot_end"].time;
			}
			else
			{
				component.Stop();
				lengthAnimShootDown = component["Shoot_start"].length;
				component.Play("Shoot_start");
			}
		}
		else
		{
			component.Stop();
		}
		ctsShootLoop.Cancel();
		ctsShootLoop = new CancellationTokenSource();
		StartCoroutine(ShootLoop(ctsShootLoop.Token, lengthAnimShootDown));
	}

	private IEnumerator ShootLoop(CancellationToken token, float _lengthAnimShootDown)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		yield return new WaitForSeconds(_lengthAnimShootDown);
		Animation currentWeaponAnimation = myCurrentWeaponSounds.animationObject.GetComponent<Animation>();
		float lengthAnimShoot = currentWeaponAnimation["Shoot"].length;
		if (!((CancellationToken)(token)).IsCancellationRequested)
		{
			currentWeaponAnimation.Play("Shoot");
			if (Defs.isSoundFX)
			{
				myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().clip = myCurrentWeaponSounds.shoot;
				myCurrentWeaponSounds.animationObject.GetComponent<AudioSource>().Play();
			}
		}
		if (!Defs.isMulti || isMine)
		{
			while (!((CancellationToken)(token)).IsCancellationRequested)
			{
				shootS();
				yield return new WaitForSeconds(lengthAnimShoot);
			}
		}
	}

	[PunRPC]
	[RPC]
	private void StartShootLoopRPC(bool isStart)
	{
		Animation component = myCurrentWeaponSounds.animationObject.GetComponent<Animation>();
		if (isStart && !isShootingLoop)
		{
			StartLoopShot();
		}
		if (!isStart && isShootingLoop)
		{
			ShotUnPressed();
		}
	}

	public void ResetShootingBurst()
	{
		_countShootInBurst = 0;
		_timerDelayInShootingBurst = -1f;
	}

	public void ShotPressed()
	{
		if (deltaAngle > 10f)
		{
			return;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining == TrainingState.TapToShoot)
		{
			TrainingController.isNextStep = TrainingState.TapToShoot;
		}
		if ((isMulti && isInet && (bool)photonView && !photonView.isMine) || _weaponManager == null || _weaponManager.currentWeaponSounds == null || _weaponManager.currentWeaponSounds.animationObject == null || _weaponManager.currentWeaponSounds.name.Contains("WeaponGrenade") || Defs.isTurretWeapon)
		{
			return;
		}
		if (!isMechActive && _weaponManager.currentWeaponSounds.isLoopShoot)
		{
			if (!isShootingLoop)
			{
				StartLoopShot();
			}
			return;
		}
		Animation animation = ((!isMechActive) ? _weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>() : mechGunAnimation);
		if (animation.IsPlaying("Shoot1") || animation.IsPlaying("Shoot2") || animation.IsPlaying("Shoot") || animation.IsPlaying("Shoot1") || animation.IsPlaying("Shoot2") || animation.IsPlaying("Reload") || animation.IsPlaying("Empty") || _timerDelayInShootingBurst > 0f)
		{
			return;
		}
		animation.Stop();
		if (_weaponManager.currentWeaponSounds.isBurstShooting)
		{
			_countShootInBurst++;
			if (_countShootInBurst >= _weaponManager.currentWeaponSounds.countShootInBurst)
			{
				_timerDelayInShootingBurst = _weaponManager.currentWeaponSounds.delayInBurstShooting;
				_countShootInBurst = 0;
			}
		}
		if (_weaponManager.currentWeaponSounds.isMelee && !_weaponManager.currentWeaponSounds.isShotMelee && !isMechActive)
		{
			_Shot();
			return;
		}
		Weapon weapon = (Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex];
		if (weapon.currentAmmoInClip > 0 || isMechActive)
		{
			if (!isMechActive)
			{
				weapon.currentAmmoInClip--;
				if (weapon.currentAmmoInClip == 0)
				{
					if (weapon.currentAmmoInBackpack > 0)
					{
						if (_weaponManager.currentWeaponSounds.isShotMelee)
						{
							Reload();
						}
					}
					else
					{
						TouchPadController rightJoystick = JoystickController.rightJoystick;
						if ((bool)rightJoystick)
						{
							rightJoystick.NoAmmo();
						}
						if (inGameGUI != null)
						{
							inGameGUI.BlinkNoAmmo(3);
							inGameGUI.PlayLowResourceBeep(3);
						}
					}
				}
			}
			_Shot();
			if (!_weaponManager.currentWeaponSounds.isShotMelee || isMechActive)
			{
				_SetGunFlashActive(true);
				if (isMechActive)
				{
					GunFlashLifetime = 0.15f;
				}
				else
				{
					GunFlashLifetime = _weaponManager.currentWeaponSounds.gameObject.GetComponent<FlashFire>().timeFireAction;
				}
			}
			return;
		}
		if (inGameGUI != null)
		{
			inGameGUI.BlinkNoAmmo(1);
			if (weapon.currentAmmoInBackpack == 0)
			{
				inGameGUI.PlayLowResourceBeepIfNotPlaying(1);
			}
		}
		if (!_weaponManager.currentWeaponSounds.isMelee)
		{
			if (!isMechActive && weapon.currentAmmoInBackpack <= 0 && !TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && showChangeWeaponHint)
			{
				HintController.instance.ShowHintByName("change_weapon", 2f);
			}
			_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().Play("Empty");
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot(_weaponManager.currentWeaponSounds.empty);
			}
		}
	}

	private void _Shot()
	{
		if (!TrainingController.TrainingCompleted)
		{
			TrainingController.timeShowFire = 1000f;
			HintController.instance.HideHintByName("press_fire");
		}
		if (isGrenadePress || showChat)
		{
			return;
		}
		if (Defs.isMulti)
		{
			ProfileController.OnGameShoot();
		}
		float num = 0f;
		if (isMechActive)
		{
			int numShootInDouble = GetNumShootInDouble();
			mechGunAnimation.Play("Shoot" + numShootInDouble);
			num = mechGunAnimation["Shoot" + numShootInDouble].length;
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot(shootMechClip);
			}
		}
		else
		{
			if (!_weaponManager.currentWeaponSounds.isDoubleShot)
			{
				_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().Play("Shoot");
				num = _weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot"].length;
			}
			else
			{
				int numShootInDouble2 = GetNumShootInDouble();
				_weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>().Play("Shoot" + numShootInDouble2);
				num = _weaponManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Shoot" + numShootInDouble2].length;
			}
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot(_weaponManager.currentWeaponSounds.shoot);
			}
		}
		if (inGameGUI != null)
		{
			inGameGUI.StartFireCircularIndicators(num);
		}
		shootS();
	}

	public RayHitsInfo GetHitsFromRay(Ray ray, bool getAll = true)
	{
		RayHitsInfo result = default(RayHitsInfo);
		result.obstacleFound = false;
		result.lenRay = 150f;
		RaycastHit[] array = Physics.RaycastAll(ray, 150f, _ShootRaycastLayerMask);
		if (array == null || array.Length == 0)
		{
			array = new RaycastHit[0];
		}
		if (!getAll)
		{
			Array.Sort(array, delegate(RaycastHit hit1, RaycastHit hit2)
			{
				float num = (hit1.point - GunFlash.position).sqrMagnitude - (hit2.point - GunFlash.position).sqrMagnitude;
				return (num > 0f) ? 1 : ((num != 0f) ? (-1) : 0);
			});
			bool flag = false;
			RaycastHit raycastHit = default(RaycastHit);
			List<RaycastHit> list = new List<RaycastHit>();
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit2 = array2[i];
				if (isHunger && raycastHit2.collider.gameObject != null && raycastHit2.collider.gameObject.CompareTag("Chest"))
				{
					list.Add(raycastHit2);
					continue;
				}
				if (raycastHit2.collider.gameObject.transform.parent != null && raycastHit2.collider.gameObject.transform.parent.CompareTag("Enemy"))
				{
					list.Add(raycastHit2);
					continue;
				}
				if (raycastHit2.collider.gameObject.transform.parent != null && raycastHit2.collider.gameObject.transform.parent.CompareTag("Player"))
				{
					list.Add(raycastHit2);
					continue;
				}
				if (raycastHit2.collider.gameObject != null && raycastHit2.collider.gameObject.CompareTag("Turret"))
				{
					list.Add(raycastHit2);
					continue;
				}
				if (raycastHit2.collider.gameObject != null && raycastHit2.collider.gameObject.CompareTag("DamagedExplosion"))
				{
					list.Add(raycastHit2);
					continue;
				}
				flag = true;
				raycastHit = raycastHit2;
				result.obstacleFound = true;
				Vector3 point = raycastHit2.point;
				Vector3 a = point - ray.origin;
				result.lenRay = Vector3.Magnitude(a);
				result.rayReflect = new Ray(point, Vector3.Reflect(ray.direction, raycastHit2.normal));
				break;
			}
			result.hits = list.ToArray();
		}
		else
		{
			result.hits = array;
		}
		return result;
	}

	private IEnumerator ShowRayWithDelay(Vector3 _origin, Vector3 _direction, string _railName, float _len, float _delay)
	{
		yield return new WaitForSeconds(_delay);
		WeaponManager.AddRay(_origin, _direction, _railName, _len);
	}

	private List<GameObject> GetAllTargets()
	{
		List<GameObject> list = new List<GameObject>();
		if (isMulti && !isCOOP)
		{
			list.AddRange(Initializer.playersObj);
			list.AddRange(Initializer.turretsObj);
		}
		else
		{
			list.AddRange(Initializer.enemiesObj);
		}
		if (Defs.isHunger)
		{
			list.AddRange(Initializer.chestsObj);
		}
		list.AddRange(Initializer.damagedObj);
		return list;
	}

	private void FlamethrowerShot(WeaponSounds weapon)
	{
		_FireFlash();
		GameObject gameObject = null;
		RaycastHit hitInfo;
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, weapon.range, _ShootRaycastLayerMask) && hitInfo.collider.gameObject != null)
		{
			gameObject = ((!hitInfo.transform.parent || (!hitInfo.transform.parent.CompareTag("Enemy") && !hitInfo.transform.parent.CompareTag("Player"))) ? hitInfo.transform.gameObject : hitInfo.transform.parent.gameObject);
			_HitEnemy(gameObject, false, 0f);
		}
		List<GameObject> allTargets = GetAllTargets();
		float num = weapon.range * weapon.range;
		for (int i = 0; i < allTargets.Count; i++)
		{
			if (!(allTargets[i] == _player) && !(gameObject == allTargets[i]))
			{
				Vector3 to = allTargets[i].transform.position - _player.transform.position;
				if (to.sqrMagnitude < num && Vector3.Angle(base.gameObject.transform.forward, to) < weapon.meleeAngle)
				{
					_HitEnemy(allTargets[i], false, 0f);
				}
			}
		}
	}

	private IEnumerator MeleeShot(WeaponSounds weapon)
	{
		_FireFlash(false, weapon.isDoubleShot ? numShootInDoubleShot : 0);
		yield return new WaitForSeconds(TimeOfMeleeAttack(weapon));
		if (weapon == null)
		{
			yield break;
		}
		GameObject raycastedObj = null;
		GameObject closestTargetObj = null;
		float closestTarget = float.MaxValue;
		bool isHeadshot = false;
		RaycastHit _hit;
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, weapon.range, _ShootRaycastLayerMask) && _hit.collider.gameObject != null)
		{
			if ((bool)_hit.transform.parent)
			{
				raycastedObj = _hit.transform.gameObject;
				if (_hit.transform.parent.CompareTag("Enemy"))
				{
					raycastedObj = _hit.transform.parent.gameObject;
					isHeadshot = _hit.collider.GetType() == typeof(SphereCollider);
				}
				if (_hit.transform.parent.CompareTag("Player"))
				{
					raycastedObj = _hit.transform.parent.gameObject;
					isHeadshot = _hit.transform.name == "HeadCollider";
				}
			}
			else
			{
				raycastedObj = _hit.transform.gameObject;
				isHeadshot = _hit.transform.name == "HeadCollider";
			}
			closestTargetObj = raycastedObj;
			closestTarget = 0f;
		}
		List<GameObject> targets = GetAllTargets();
		float weaponRangeSqr = weapon.range * weapon.range;
		for (int i = 0; i < targets.Count; i++)
		{
			if (!(targets[i] == _player) && !(raycastedObj == targets[i]))
			{
				Vector3 enemyDelta = targets[i].transform.position - _player.transform.position;
				float targetDistance = enemyDelta.sqrMagnitude;
				if (targetDistance < closestTarget && targetDistance < weaponRangeSqr && Vector3.Angle(base.gameObject.transform.forward, enemyDelta) < weapon.meleeAngle)
				{
					closestTarget = targetDistance;
					closestTargetObj = targets[i];
				}
			}
		}
		if (closestTargetObj != null)
		{
			_HitEnemy(closestTargetObj, isHeadshot, 0f);
		}
	}

	private void RailgunShot(WeaponSounds weapon)
	{
		weapon.fire();
		_FireFlash();
		float num = weapon.tekKoof * Defs.Coef;
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(((float)Screen.width - weapon.startZone.x * num) * 0.5f + (float)UnityEngine.Random.Range(0, Mathf.RoundToInt(weapon.startZone.x * num)), ((float)Screen.height - weapon.startZone.y * num) * 0.5f + (float)UnityEngine.Random.Range(0, Mathf.RoundToInt(weapon.startZone.y * num)), 0f));
		if (weapon.freezer)
		{
			RayHitsInfo hitsFromRay = GetHitsFromRay(ray, false);
			RaycastHit[] hits = hitsFromRay.hits;
			foreach (RaycastHit raycastHit in hits)
			{
				_DoHit(raycastHit, true);
			}
			AddFreezerRayWithLength(hitsFromRay.lenRay);
			if (isMulti)
			{
				if (isInet)
				{
					photonView.RPC("AddFreezerRayWithLength", PhotonTargets.Others, hitsFromRay.lenRay);
				}
				else
				{
					GetComponent<NetworkView>().RPC("AddFreezerRayWithLength", RPCMode.Others, hitsFromRay.lenRay);
				}
			}
			return;
		}
		bool flag = false;
		int num2 = 0;
		do
		{
			RayHitsInfo hitsFromRay2 = GetHitsFromRay(ray, weapon.countReflectionRay == 1);
			RaycastHit[] hits2 = hitsFromRay2.hits;
			foreach (RaycastHit raycastHit2 in hits2)
			{
				_DoHit(raycastHit2);
			}
			bool flag2 = num2 == 0 && (weapon.countReflectionRay == 1 || !hitsFromRay2.obstacleFound);
			Vector3 origin = ((num2 != 0) ? ray.origin : GunFlash.gameObject.transform.parent.position);
			Vector3 direction = (flag2 ? GunFlash.gameObject.transform.parent.parent.forward : ((num2 != 0) ? ray.direction : (hitsFromRay2.rayReflect.origin - GunFlash.gameObject.transform.parent.position)));
			float len = (flag2 ? 150f : ((num2 != 0) ? hitsFromRay2.lenRay : (hitsFromRay2.rayReflect.origin - GunFlash.gameObject.transform.parent.position).magnitude));
			StartCoroutine(ShowRayWithDelay(origin, direction, weapon.railName, len, (float)num2 * 0.05f));
			if (hitsFromRay2.obstacleFound)
			{
				ray = hitsFromRay2.rayReflect;
				flag = true;
			}
			num2++;
		}
		while (flag && num2 < weapon.countReflectionRay);
	}

	private void BulletShot(WeaponSounds weapon)
	{
		int num = ((!weapon.isShotGun) ? 1 : weapon.countShots);
		float maxDistance = ((!weapon.isShotGun) ? 100f : 30f);
		Vector3[] array = null;
		Quaternion[] array2 = null;
		bool[] array3 = null;
		int num2 = Mathf.Min(7, num);
		bool flag = false;
		bool flag2 = false;
		Vector3 vector = Vector3.zero;
		Quaternion quaternion = Quaternion.identity;
		if (weapon.bulletExplode)
		{
			maxDistance = 250f;
		}
		for (int i = 0; i < num; i++)
		{
			float num3 = weapon.tekKoof * Defs.Coef;
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(((float)Screen.width - weapon.startZone.x * num3) * 0.5f + (float)UnityEngine.Random.Range(0, Mathf.RoundToInt(weapon.startZone.x * num3)), ((float)Screen.height - weapon.startZone.y * num3) * 0.5f + (float)UnityEngine.Random.Range(0, Mathf.RoundToInt(weapon.startZone.y * num3)), 0f));
			Transform transform = ((!weapon.isDoubleShot) ? GunFlash : weapon.gunFlashDouble[numShootInDoubleShot - 1]);
			if (transform != null && !Defs.isDaterRegim)
			{
				GameObject currentBullet = BulletStackController.sharedController.GetCurrentBullet((int)weapon.typeTracer);
				if (currentBullet != null)
				{
					currentBullet.transform.rotation = myTransform.rotation;
					Bullet component = currentBullet.GetComponent<Bullet>();
					component.endPos = ray.GetPoint(200f);
					component.startPos = ((!weapon.isDoubleShot) ? GunFlash.position : weapon.gunFlashDouble[numShootInDoubleShot - 1].position);
					component.StartBullet();
				}
				weapon.fire();
			}
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo, maxDistance, _ShootRaycastLayerMask))
			{
				continue;
			}
			if (!weapon.bulletExplode)
			{
				if (!hitInfo.collider.gameObject.transform.CompareTag("DamagedExplosion"))
				{
					vector = hitInfo.point + hitInfo.normal * 0.001f;
					quaternion = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
					flag2 = true;
					flag = ((!(hitInfo.collider.gameObject.transform.parent != null) || hitInfo.collider.gameObject.transform.parent.CompareTag("Enemy") || hitInfo.collider.gameObject.transform.parent.CompareTag("Player")) ? true : false);
					HoleRPC(flag, vector, quaternion);
					if (isMulti)
					{
						if (!isInet)
						{
							_networkView.RPC("HoleRPC", RPCMode.Others, flag, vector, quaternion);
						}
						else if (num2 > 1 && i < num2)
						{
							if (array == null)
							{
								array = new Vector3[num2];
								array2 = new Quaternion[num2];
								array3 = new bool[num2];
							}
							array[i] = vector;
							array2[i] = quaternion;
							array3[i] = flag;
						}
					}
				}
				_DoHit(hitInfo);
			}
			else
			{
				Rocket rocket = CreateRocket(hitInfo.point, Quaternion.identity, koofDamageWeaponFromPotoins, isMulti, isInet, TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier));
				rocket.dontExecStart = true;
				rocket.SendSetRocketActiveRPC();
				rocket.KillRocket(hitInfo.collider);
			}
		}
		if (!flag2 || !isInet)
		{
			_FireFlash(true, weapon.isDoubleShot ? numShootInDoubleShot : 0);
		}
		else if (num2 > 1)
		{
			_FireFlashWithManyHoles(array3, array, array2, true, weapon.isDoubleShot ? numShootInDoubleShot : 0);
		}
		else
		{
			_FireFlashWithHole(flag, vector, quaternion, true, weapon.isDoubleShot ? numShootInDoubleShot : 0);
		}
	}

	public void shootS()
	{
		if (isGrenadePress)
		{
			return;
		}
		if (isMechActive)
		{
			BulletShot(mechWeaponSounds);
			return;
		}
		WeaponSounds currentWeaponSounds = _weaponManager.currentWeaponSounds;
		if (currentWeaponSounds.bazooka)
		{
			StartCoroutine(BazookaShoot());
		}
		else if (currentWeaponSounds.railgun || currentWeaponSounds.freezer)
		{
			RailgunShot(currentWeaponSounds);
		}
		else if (currentWeaponSounds.flamethrower)
		{
			FlamethrowerShot(currentWeaponSounds);
		}
		else if (currentWeaponSounds.isRoundMelee)
		{
			StartCoroutine(HitRoundMelee(currentWeaponSounds));
		}
		else if (currentWeaponSounds.isMelee)
		{
			StartCoroutine(MeleeShot(currentWeaponSounds));
		}
		else
		{
			BulletShot(currentWeaponSounds);
		}
	}

	public void GrenadePress()
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted)
		{
			showGrenadeHint = false;
			HintController.instance.HideHintByName("use_grenade");
		}
		if (indexWeapon != 1001)
		{
			GrenadePressInvoke();
		}
	}

	[Obfuscation(Exclude = true)]
	public void GrenadePressInvoke()
	{
		isGrenadePress = true;
		currentWeaponBeforeGrenade = WeaponManager.sharedManager.CurrentWeaponIndex;
		ChangeWeapon(1000, false);
		timeGrenadePress = Time.realtimeSinceStartup;
		if (inGameGUI != null && inGameGUI.blockedCollider != null)
		{
			inGameGUI.blockedCollider.SetActive(true);
		}
		if (inGameGUI != null && inGameGUI.blockedCollider2 != null)
		{
			inGameGUI.blockedCollider2.SetActive(true);
		}
		if (inGameGUI != null && inGameGUI.blockedColliderDater != null)
		{
			inGameGUI.blockedColliderDater.SetActive(true);
		}
		if (inGameGUI != null)
		{
			for (int i = 0; i < inGameGUI.upButtonsInShopPanel.Length; i++)
			{
				inGameGUI.upButtonsInShopPanel[i].GetComponent<ButtonHandler>().isEnable = false;
			}
			for (int j = 0; j < inGameGUI.upButtonsInShopPanelSwipeRegim.Length; j++)
			{
				inGameGUI.upButtonsInShopPanelSwipeRegim[j].GetComponent<ButtonHandler>().isEnable = false;
			}
		}
	}

	public void GrenadeFire()
	{
		if (isGrenadePress)
		{
			float num = Time.realtimeSinceStartup - timeGrenadePress;
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining == TrainingState.TapToThrowGrenade)
			{
				TrainingController.isNextStep = TrainingState.TapToThrowGrenade;
			}
			Defs.isGrenateFireEnable = false;
			if (num - 0.4f > 0f)
			{
				GrenadeStartFire();
			}
			else
			{
				Invoke("GrenadeStartFire", 0.4f - num);
			}
		}
	}

	[Obfuscation(Exclude = true)]
	public void GrenadeStartFire()
	{
		if (isMulti)
		{
			if (!isInet)
			{
				GetComponent<NetworkView>().RPC("fireFlash", RPCMode.All, false, 0);
			}
			else
			{
				photonView.RPC("fireFlash", PhotonTargets.All, false, 0);
			}
		}
		else
		{
			fireFlash(false, 0);
		}
		GrenadeCount--;
		Invoke("RunGrenade", 0.2667f);
		Invoke("SetGrenateFireEnabled", 1f);
	}

	[Obfuscation(Exclude = true)]
	private void SetGrenateFireEnabled()
	{
		Defs.isGrenateFireEnable = true;
	}

	[Obfuscation(Exclude = true)]
	private void RunGrenade()
	{
		if ((bool)currentGrenade)
		{
			currentGrenade.GetComponent<Rigidbody>().isKinematic = false;
			currentGrenade.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(0f, -5f, 0f) * (150f * myTransform.forward));
			currentGrenade.GetComponent<Rigidbody>().useGravity = true;
			currentGrenade.GetComponent<Rocket>().StartRocket();
		}
		Invoke("ReturnWeaponAfterGrenade", 0.5f);
		isGrenadePress = false;
	}

	[Obfuscation(Exclude = true)]
	private void ReturnWeaponAfterGrenade()
	{
		ChangeWeapon(currentWeaponBeforeGrenade, false);
		if (inGameGUI != null && inGameGUI.blockedCollider != null)
		{
			inGameGUI.blockedCollider.SetActive(false);
		}
		if (inGameGUI != null && inGameGUI.blockedCollider2 != null)
		{
			inGameGUI.blockedCollider2.SetActive(false);
		}
		if (inGameGUI != null && inGameGUI.blockedColliderDater != null)
		{
			inGameGUI.blockedColliderDater.SetActive(false);
		}
		if (inGameGUI != null)
		{
			for (int i = 0; i < inGameGUI.upButtonsInShopPanel.Length; i++)
			{
				inGameGUI.upButtonsInShopPanel[i].GetComponent<ButtonHandler>().isEnable = true;
			}
			for (int j = 0; j < inGameGUI.upButtonsInShopPanelSwipeRegim.Length; j++)
			{
				inGameGUI.upButtonsInShopPanelSwipeRegim[j].GetComponent<ButtonHandler>().isEnable = true;
			}
		}
	}

	public static Rocket CreateRocket(Vector3 pos, Quaternion rot, float customDamageAdd, bool isMulti, bool isInet, int tierOrRoomTier)
	{
		GameObject gameObject = null;
		gameObject = RocketStack.sharedController.GetRocket();
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rot;
		Rocket component = gameObject.GetComponent<Rocket>();
		component.rocketNum = WeaponManager.sharedManager.currentWeaponSounds.rocketNum;
		component.weaponPrefabName = WeaponManager.sharedManager.currentWeaponSounds.gameObject.name.Replace("(Clone)", string.Empty);
		component.weaponName = WeaponManager.sharedManager.currentWeaponSounds.bazookaExplosionName;
		component.damage = (float)WeaponManager.sharedManager.currentWeaponSounds.damage * (1f + customDamageAdd + EffectsController.DamageModifsByCats(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1));
		component.radiusDamage = WeaponManager.sharedManager.currentWeaponSounds.bazookaExplosionRadius;
		component.radiusDamageSelf = WeaponManager.sharedManager.currentWeaponSounds.bazookaExplosionRadiusSelf;
		component.radiusImpulse = WeaponManager.sharedManager.currentWeaponSounds.bazookaImpulseRadius * (1f + EffectsController.ExplosionImpulseRadiusIncreaseCoef);
		component.damageRange = WeaponManager.sharedManager.currentWeaponSounds.damageRange * (1f + customDamageAdd + EffectsController.DamageModifsByCats(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1));
		component.isSlowdown = WeaponManager.sharedManager.currentWeaponSounds.isSlowdown;
		component.slowdownCoeff = WeaponManager.sharedManager.currentWeaponSounds.slowdownCoeff;
		component.slowdownTime = WeaponManager.sharedManager.currentWeaponSounds.slowdownTime;
		component.impulseForce = WeaponManager.sharedManager.currentWeaponSounds.impulseForce;
		component.impulseForceSelf = WeaponManager.sharedManager.currentWeaponSounds.impulseForceSelf;
		float num = (component.multiplayerDamage = ((ExpController.Instance != null && ExpController.Instance.OurTier < WeaponManager.sharedManager.currentWeaponSounds.DamageByTier.Length) ? WeaponManager.sharedManager.currentWeaponSounds.DamageByTier[tierOrRoomTier] : ((WeaponManager.sharedManager.currentWeaponSounds.DamageByTier.Length <= 0) ? 0f : WeaponManager.sharedManager.currentWeaponSounds.DamageByTier[0])));
		gameObject.GetComponent<Rigidbody>().useGravity = WeaponManager.sharedManager.currentWeaponSounds.grenadeLauncher;
		return component;
	}

	private IEnumerator BazookaShoot()
	{
		for (int i = 0; i < _weaponManager.currentWeaponSounds.countInSeriaBazooka; i++)
		{
			_weaponManager.currentWeaponSounds.fire();
			_FireFlash();
			float rangeFromUs = 0.2f;
			Rocket rocketScript = CreateRocket((!(_weaponManager.currentWeaponSounds.gunFlash != null)) ? (myTransform.position + myTransform.forward * rangeFromUs) : _weaponManager.currentWeaponSounds.gunFlash.position, myTransform.rotation, koofDamageWeaponFromPotoins, isMulti, isInet, TierOrRoomTier((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier));
			rocketScript.SendSetRocketActiveRPC();
			rocketToLaunch = rocketScript.gameObject;
			if (i != _weaponManager.currentWeaponSounds.countInSeriaBazooka - 1)
			{
				yield return new WaitForSeconds(_weaponManager.currentWeaponSounds.stepTimeInSeriaBazooka);
			}
		}
	}

	private void RunOnGroundEffect(string name)
	{
		if (name == null || mySkinName == null)
		{
			return;
		}
		GameObject objectFromName = RayAndExplosionsStackController.sharedController.GetObjectFromName("OnGroundWeaponEffects/" + name + "_OnGroundEffect");
		if (!(objectFromName == null))
		{
			PerformActionRecurs(objectFromName, delegate(Transform t)
			{
				t.gameObject.SetActive(false);
			});
			objectFromName.transform.parent = mySkinName.onGroundEffectsPoint;
			objectFromName.transform.localPosition = Vector3.zero;
			objectFromName.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			PerformActionRecurs(objectFromName, delegate(Transform t)
			{
				t.gameObject.SetActive(true);
			});
			ParticleSystem component = objectFromName.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Play();
			}
		}
	}

	private IEnumerator HitRoundMelee(WeaponSounds weapon)
	{
		_FireFlash(false, weapon.isDoubleShot ? numShootInDoubleShot : 0);
		yield return new WaitForSeconds(TimeOfMeleeAttack(weapon));
		if (weapon == null)
		{
			yield break;
		}
		RunOnGroundEffect(weapon.gameObject.name.Replace("(Clone)", string.Empty));
		List<GameObject> targets = GetAllTargets();
		float weaponRangeSqr = weapon.radiusRoundMelee * weapon.radiusRoundMelee;
		for (int i = 0; i < targets.Count; i++)
		{
			if (!(targets[i] == _player))
			{
				float targetDistance = (targets[i].transform.position - _player.transform.position).sqrMagnitude;
				if (targetDistance < weaponRangeSqr)
				{
					_HitEnemy(targets[i], false, targetDistance);
				}
			}
		}
	}

	private float GetDamageValueForTargetsInRadius(float distanceToTagetSqr, float radiusDamageSqr)
	{
		float num = (float)_weaponManager.currentWeaponSounds.damage + WeaponManager.sharedManager.currentWeaponSounds.damageRange.x;
		float num2 = (float)_weaponManager.currentWeaponSounds.damage + WeaponManager.sharedManager.currentWeaponSounds.damageRange.y;
		return (num + (num2 = num) * (1f - distanceToTagetSqr / radiusDamageSqr)) * (1f + koofDamageWeaponFromPotoins + EffectsController.DamageModifsByCats(_weaponManager.currentWeaponSounds.categoryNabor - 1));
	}

	private float GetDamageForBotsAndExplosionObjects(bool isTakeDamageMech = false)
	{
		WeaponSounds weaponSounds = ((!isTakeDamageMech) ? _weaponManager.currentWeaponSounds : mechWeaponSounds);
		return ((float)(-weaponSounds.damage) + UnityEngine.Random.Range(weaponSounds.damageRange.x, weaponSounds.damageRange.y)) * (1f + koofDamageWeaponFromPotoins + EffectsController.DamageModifsByCats(weaponSounds.categoryNabor - 1));
	}

	private void _DoHit(RaycastHit _hit, bool slowdown = false)
	{
		bool flag = false;
		if ((bool)_hit.transform.parent)
		{
			if (_hit.transform.parent.CompareTag("Enemy"))
			{
				flag = _hit.collider.name == "HeadCollider" || _hit.collider is SphereCollider;
				_HitEnemy(_hit.transform.parent.gameObject, flag, 0f);
				return;
			}
			if (_hit.transform.parent.CompareTag("Player"))
			{
				flag = _hit.collider.name == "HeadCollider";
				_HitEnemy(_hit.transform.parent.gameObject, flag, 0f);
				return;
			}
		}
		flag = _hit.collider.name == "HeadCollider";
		_HitEnemy(_hit.transform.gameObject, flag, 0f);
	}

	private static float TimeOfMeleeAttack(WeaponSounds ws)
	{
		return ws.animationObject.GetComponent<Animation>()[(!ws.isDoubleShot) ? "Shoot" : "Shoot1"].length * ws.meleeAttackTimeModifier;
	}

	private void _FireFlash(bool isFlash = true, int numFlash = 0)
	{
		if (!myCurrentWeaponSounds.isLoopShoot && isMulti)
		{
			if (isInet)
			{
				photonView.RPC("fireFlash", PhotonTargets.Others, isFlash, numFlash);
			}
			else
			{
				_networkView.RPC("fireFlash", RPCMode.Others, isFlash, numFlash);
			}
		}
	}

	private void _FireFlashWithHole(bool _isBloodParticle, Vector3 _pos, Quaternion _rot, bool isFlash = true, int numFlash = 0)
	{
		if (isMulti && isInet)
		{
			photonView.RPC("fireFlashWithHole", PhotonTargets.Others, _isBloodParticle, _pos, _rot, isFlash, numFlash);
		}
	}

	private void _FireFlashWithManyHoles(bool[] _isBloodParticle, Vector3[] _pos, Quaternion[] _rot, bool isFlash = true, int numFlash = 0)
	{
		if (isMulti && isInet)
		{
			photonView.RPC("fireFlashWithManyHoles", PhotonTargets.Others, _isBloodParticle, _pos, _rot, isFlash, numFlash);
		}
	}

	[RPC]
	[PunRPC]
	private void fireFlashWithManyHoles(bool[] _isBloodParticle, Vector3[] _pos, Quaternion[] _rot, bool isFlash, int numFlash)
	{
		fireFlash(isFlash, numFlash);
		if (_isBloodParticle != null)
		{
			for (int i = 0; i < _isBloodParticle.Length; i++)
			{
				HoleRPC(_isBloodParticle[i], _pos[i], _rot[i]);
			}
		}
	}

	[PunRPC]
	[RPC]
	private void fireFlashWithHole(bool _isBloodParticle, Vector3 _pos, Quaternion _rot, bool isFlash, int numFlash)
	{
		fireFlash(isFlash, numFlash);
		HoleRPC(_isBloodParticle, _pos, _rot);
	}

	[PunRPC]
	[RPC]
	private void fireFlash(bool isFlash, int numFlash)
	{
		WeaponSounds weaponSounds = ((!isMechActive) ? myCurrentWeaponSounds : mechWeaponSounds);
		if (weaponSounds == null)
		{
			return;
		}
		if (isFlash)
		{
			if (numFlash == 0)
			{
				FlashFire component = weaponSounds.GetComponent<FlashFire>();
				if (component != null)
				{
					component.fire(this);
				}
			}
			else if (weaponSounds.gunFlashDouble.Length > numFlash - 1)
			{
				weaponSounds.gunFlashDouble[numFlash - 1].GetComponent<FlashFire>().fire(this);
			}
		}
		if (weaponSounds.isRoundMelee)
		{
			float tm = TimeOfMeleeAttack(weaponSounds);
			StartCoroutine(RunOnGroundEffectCoroutine(weaponSounds.gameObject.name.Replace("(Clone)", string.Empty), tm));
		}
		string animation = (weaponSounds.isDoubleShot ? ("Shoot" + numFlash) : "Shoot");
		if (isMechActive)
		{
			mechGunAnimation.Play(animation);
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot(shootMechClip);
			}
		}
		else
		{
			weaponSounds.animationObject.GetComponent<Animation>().Play(animation);
		}
		if (Defs.isSoundFX && !isMechActive)
		{
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().PlayOneShot(weaponSounds.shoot);
		}
	}

	[PunRPC]
	[RPC]
	public void HoleRPC(bool _isBloodParticle, Vector3 _pos, Quaternion _rot)
	{
		if (Device.isPixelGunLow)
		{
			return;
		}
		if (_isBloodParticle)
		{
			WallBloodParticle currentParticle = BloodParticleStackController.sharedController.GetCurrentParticle(false);
			if (currentParticle != null)
			{
				currentParticle.StartShowParticle(_pos, _rot, false);
			}
			return;
		}
		HoleScript currentHole = HoleBulletStackController.sharedController.GetCurrentHole(false);
		if (currentHole != null)
		{
			currentHole.StartShowHole(_pos, _rot, false);
		}
		WallBloodParticle currentParticle2 = WallParticleStackController.sharedController.GetCurrentParticle(false);
		if (currentParticle2 != null)
		{
			currentParticle2.StartShowParticle(_pos, _rot, false);
		}
	}

	[RPC]
	[PunRPC]
	public void SlowdownRPC(float coef, float time)
	{
		if (isMine || !isMulti)
		{
			EffectsController.SlowdownCoeff = coef;
			_timeOfSlowdown = time;
		}
	}

	private void _HitChest(GameObject go)
	{
		WeaponSounds weaponSounds = ((!isMechActive) ? _weaponManager.currentWeaponSounds : mechWeaponSounds);
		go.GetComponent<ChestController>().MinusLive(((float)weaponSounds.damage + UnityEngine.Random.Range(weaponSounds.damageRange.x, weaponSounds.damageRange.y)) * (1f + koofDamageWeaponFromPotoins + EffectsController.DamageModifsByCats(weaponSounds.categoryNabor - 1)));
	}

	private void _HitZombie(GameObject zmb, bool isHeadShot, float sqrDistance)
	{
		WeaponSounds weaponSounds = ((!isMechActive) ? _weaponManager.currentWeaponSounds : mechWeaponSounds);
		float num = 0f;
		if (weaponSounds.isRoundMelee)
		{
			num = GetDamageValueForTargetsInRadius(sqrDistance, weaponSounds.radiusRoundMelee * weaponSounds.radiusRoundMelee);
			Debug.Log(num);
		}
		else
		{
			num = ((float)weaponSounds.damage + UnityEngine.Random.Range(weaponSounds.damageRange.x, weaponSounds.damageRange.y)) * (1f + koofDamageWeaponFromPotoins + ((!isMechActive) ? EffectsController.DamageModifsByCats(weaponSounds.categoryNabor - 1) : 0f));
		}
		BaseBot botScriptForObject = BaseBot.GetBotScriptForObject(zmb.transform.parent);
		if (!isMulti)
		{
			if (botScriptForObject != null)
			{
				botScriptForObject.GetDamage(0f - num, myPlayerTransform, myCurrentWeaponSounds.name, true, isHeadShot);
				return;
			}
			TrainingEnemy componentInParent = zmb.GetComponentInParent<TrainingEnemy>();
			if (componentInParent != null)
			{
				componentInParent.ApplyDamage(num, isHeadShot);
			}
		}
		else if (isCOOP && !botScriptForObject.IsDeath)
		{
			botScriptForObject.GetDamageForMultiplayer(0f - num, null, myCurrentWeaponSounds.name, isHeadShot);
			_weaponManager.myNetworkStartTable.score = GlobalGameController.Score;
			_weaponManager.myNetworkStartTable.SynhScore();
		}
	}

	private IEnumerator _HitEnemyWithDelay(GameObject hitEnemy, float time, bool headshot = false)
	{
		yield return new WaitForSeconds(time);
		_HitEnemy(hitEnemy, headshot, 0f);
	}

	private void _HitEnemy(GameObject hitEnemy, bool headshot = false, float sqrDistance = 0f)
	{
		switch (hitEnemy.tag)
		{
		case "Enemy":
			_HitZombie(hitEnemy.transform.GetChild(0).gameObject, headshot, sqrDistance);
			break;
		case "Player":
			_HitPlayer(hitEnemy, headshot, sqrDistance);
			break;
		case "Chest":
			_HitChest(hitEnemy);
			break;
		case "Turret":
			_HitTurret(hitEnemy, sqrDistance);
			break;
		case "DamagedExplosion":
		{
			float damageForBotsAndExplosionObjects = GetDamageForBotsAndExplosionObjects();
			DamagedExplosionObject.TryApplyDamageToObject(hitEnemy, damageForBotsAndExplosionObjects);
			break;
		}
		}
	}

	private float GetMultyDamage()
	{
		WeaponSounds weaponSounds = ((!isMechActive) ? _weaponManager.currentWeaponSounds : mechWeaponSounds);
		if (isMechActive)
		{
			return weaponSounds.DamageByTier[TierOrRoomTier(GearManager.CurrentNumberOfUphradesForGear(GearManager.Mech))];
		}
		return (ExpController.Instance != null && ExpController.Instance.OurTier < _weaponManager.currentWeaponSounds.DamageByTier.Length) ? _weaponManager.currentWeaponSounds.DamageByTier[TierOrRoomTier(ExpController.Instance.OurTier)] : ((_weaponManager.currentWeaponSounds.DamageByTier.Length <= 0) ? 0f : _weaponManager.currentWeaponSounds.DamageByTier[0]);
	}

	private void _HitTurret(GameObject _turret, float sqrDistance)
	{
		if (Defs.isCOOP)
		{
			return;
		}
		WeaponSounds weaponSounds = ((!isMechActive) ? _weaponManager.currentWeaponSounds : mechWeaponSounds);
		TurretController component = _turret.GetComponent<TurretController>();
		if (component.isEnemyTurret)
		{
			float num = 0f;
			if (weaponSounds.isRoundMelee)
			{
				float num2 = ((ExpController.Instance != null && ExpController.Instance.OurTier < weaponSounds.DamageByTier.Length) ? weaponSounds.DamageByTier[TierOrRoomTier(ExpController.Instance.OurTier)] : ((weaponSounds.DamageByTier.Length <= 0) ? 0f : weaponSounds.DamageByTier[0]));
				float num3 = num2 * 0.7f;
				float num4 = num2;
				num = (num3 + (num4 - num3) * (1f - sqrDistance / (weaponSounds.radiusRoundMelee * weaponSounds.radiusRoundMelee))) * (1f + koofDamageWeaponFromPotoins + ((!isMechActive) ? EffectsController.DamageModifsByCats(weaponSounds.categoryNabor - 1) : 0f));
			}
			else
			{
				num = GetMultyDamage() * (1f + koofDamageWeaponFromPotoins);
			}
			myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.damageTurret, num);
			if (Defs.isInet)
			{
				component.MinusLive(num, myPlayerTransform.GetComponent<PhotonView>().viewID);
			}
			else
			{
				component.MinusLive(num, 0, myPlayerTransform.GetComponent<NetworkView>().viewID);
			}
		}
	}

	[PunRPC]
	[RPC]
	private void ReloadGun()
	{
		if (!(myCurrentWeaponSounds == null))
		{
			myCurrentWeaponSounds.animationObject.GetComponent<Animation>().Play("Reload");
			myCurrentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].speed = _currentReloadAnimationSpeed;
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot(myCurrentWeaponSounds.reload);
			}
		}
	}

	private void Reload()
	{
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null && inGameGUI != null)
		{
			if (WeaponManager.sharedManager.currentWeaponSounds.ammoInClip > 1 || !WeaponManager.sharedManager.currentWeaponSounds.isShotMelee)
			{
				inGameGUI.ShowCircularIndicatorOnReload(WeaponManager.sharedManager.currentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].length / _currentReloadAnimationSpeed);
			}
			else
			{
				WeaponManager.sharedManager.ReloadAmmo();
			}
		}
		WeaponManager.sharedManager.Reload();
	}

	[Obfuscation(Exclude = true)]
	public void ReloadPressed()
	{
		if (isGrenadePress || isReloading || (_weaponManager.currentWeaponSounds.isMelee && !_weaponManager.currentWeaponSounds.isShotMelee))
		{
			return;
		}
		if (isZooming)
		{
			ZoomPress();
		}
		if (_weaponManager.CurrentWeaponIndex < 0 || _weaponManager.CurrentWeaponIndex >= _weaponManager.playerWeapons.Count || ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInBackpack <= 0 || ((Weapon)_weaponManager.playerWeapons[_weaponManager.CurrentWeaponIndex]).currentAmmoInClip == _weaponManager.currentWeaponSounds.ammoInClip)
		{
			return;
		}
		Reload();
		if (_weaponManager.currentWeaponSounds.isShotMelee)
		{
			return;
		}
		if (isMulti)
		{
			if (!isInet)
			{
				GetComponent<NetworkView>().RPC("ReloadGun", RPCMode.Others);
			}
			else
			{
				photonView.RPC("ReloadGun", PhotonTargets.Others);
			}
		}
		if (Defs.isSoundFX)
		{
			GetComponent<AudioSource>().PlayOneShot(_weaponManager.currentWeaponSounds.reload);
		}
		if (JoystickController.rightJoystick != null)
		{
			JoystickController.rightJoystick.HasAmmo();
			if (inGameGUI != null)
			{
				inGameGUI.BlinkNoAmmo(0);
			}
		}
		else
		{
			Debug.Log("JoystickController.rightJoystick = null");
		}
	}

	[RPC]
	[PunRPC]
	public void AddFreezerRayWithLength(float len)
	{
		Transform gunFlash = GunFlash;
		if (gunFlash == null && myTransform.childCount > 0)
		{
			Transform child = myTransform.GetChild(0);
			FlashFire component = child.GetComponent<FlashFire>();
			if (component != null && component.gunFlashObj != null)
			{
				gunFlash = component.gunFlashObj.transform;
			}
		}
		if (!(gunFlash != null))
		{
			return;
		}
		if (this.FreezerFired != null)
		{
			this.FreezerFired(len);
			return;
		}
		GameObject gameObject = WeaponManager.AddRay(gunFlash.gameObject.transform.parent.position, gunFlash.gameObject.transform.parent.parent.forward, gunFlash.gameObject.transform.parent.parent.GetComponent<WeaponSounds>().railName, len);
		if (gameObject != null)
		{
			FreezerRay component2 = gameObject.GetComponent<FreezerRay>();
			if (component2 != null)
			{
				component2.SetParentMoveC(this);
			}
		}
	}

	public void RunTurret()
	{
		if (Defs.isTurretWeapon)
		{
			string key = ((!Defs.isDaterRegim) ? GearManager.Turret : GearManager.MusicBox);
			Storager.setInt(key, Storager.getInt(key, false) - 1, false);
			PotionsController.sharedController.ActivatePotion(GearManager.Turret, this, new Dictionary<string, object>());
			currentTurret.transform.parent = null;
			currentTurret.GetComponent<TurretController>().StartTurret();
			ChangeWeapon(currentWeaponBeforeTurret, false);
			currentWeaponBeforeTurret = -1;
		}
	}

	public void CancelTurret()
	{
		ChangeWeapon(currentWeaponBeforeTurret, false);
		currentWeaponBeforeTurret = -1;
		if (Defs.isMulti)
		{
			if (Defs.isInet)
			{
				PhotonNetwork.Destroy(currentTurret);
				return;
			}
			Network.RemoveRPCs(currentTurret.GetComponent<NetworkView>().viewID);
			Network.Destroy(currentTurret);
		}
		else
		{
			UnityEngine.Object.Destroy(currentTurret);
		}
	}

	public void SendLike(Player_move_c whomMoveC)
	{
		if (whomMoveC != null)
		{
			whomMoveC.SendDaterChat(mySkinName.NickName, "Key_1803", whomMoveC.mySkinName.NickName);
		}
		if (Defs.isInet)
		{
			photonView.RPC("LikeRPC", PhotonTargets.All, photonView.ownerId, whomMoveC.photonView.ownerId);
		}
		else
		{
			GetComponent<NetworkView>().RPC("LikeRPCLocal", RPCMode.All, GetComponent<NetworkView>().viewID, whomMoveC.GetComponent<NetworkView>().viewID);
		}
	}

	[PunRPC]
	[RPC]
	private void LikeRPC(int idWho, int idWhom)
	{
		Player_move_c player_move_c = null;
		Player_move_c player_move_c2 = null;
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			Player_move_c player_move_c3 = Initializer.players[i];
			if (idWho == player_move_c3.photonView.ownerId)
			{
				player_move_c = player_move_c3;
			}
			if (idWhom == player_move_c3.photonView.ownerId)
			{
				player_move_c2 = player_move_c3;
			}
		}
		if (player_move_c != null && player_move_c2 != null)
		{
			Like(player_move_c, player_move_c2);
		}
	}

	[RPC]
	[PunRPC]
	private void LikeRPCLocal(NetworkViewID idWho, NetworkViewID idWhom)
	{
		Player_move_c player_move_c = null;
		Player_move_c player_move_c2 = null;
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			Player_move_c player_move_c3 = Initializer.players[i];
			if (idWho.Equals(player_move_c3.GetComponent<NetworkView>().viewID))
			{
				player_move_c = player_move_c3;
			}
			if (idWhom.Equals(player_move_c3.GetComponent<NetworkView>().viewID))
			{
				player_move_c2 = player_move_c3;
			}
		}
		if (player_move_c != null && player_move_c2 != null)
		{
			Like(player_move_c, player_move_c2);
		}
	}

	private void Like(Player_move_c whoMoveC, Player_move_c whomMoveC)
	{
		if (whomMoveC.Equals(WeaponManager.sharedManager.myPlayerMoveC))
		{
			countKills++;
			GlobalGameController.CountKills = countKills;
			WeaponManager.sharedManager.myNetworkStartTable.CountKills = countKills;
			WeaponManager.sharedManager.myNetworkStartTable.SynhCountKills();
			ProfileController.OnGetLike();
		}
	}

	private int GetNumShootInDouble()
	{
		numShootInDoubleShot++;
		if (numShootInDoubleShot == 3)
		{
			numShootInDoubleShot = 1;
		}
		return numShootInDoubleShot;
	}

	[PunRPC]
	[RPC]
	private void SyncTurretUpgrade(int turretUpgrade)
	{
		this.turretUpgrade = turretUpgrade;
	}

	private void _HitPlayer(GameObject plr, bool isHeadShot, float sqrDistance)
	{
		WeaponSounds weaponSounds = ((!isMechActive) ? _weaponManager.currentWeaponSounds : mechWeaponSounds);
		Player_move_c playerMoveC = plr.GetComponent<SkinName>().playerMoveC;
		float num = 1f;
		if (isHeadShot)
		{
			float num2 = UnityEngine.Random.Range(0f, 1f);
			isHeadShot = num2 >= playerMoveC._chanceToIgnoreHeadshot;
		}
		if (isHeadShot)
		{
			num = 2f;
			if (!isMechActive)
			{
				num += EffectsController.AddingForHeadshot(weaponSounds.categoryNabor - 1);
			}
		}
		if ((!isMulti || isCOOP || isCompany || Defs.isFlag || Defs.isCapturePoints) && ((!isCompany && !Defs.isFlag && !Defs.isCapturePoints) || myCommand == playerMoveC.myCommand))
		{
			return;
		}
		if (Defs.isDaterRegim && weaponSounds.isDaterWeapon)
		{
			playerMoveC.SendDaterChat(mySkinName.NickName, weaponSounds.daterMessage, playerMoveC.mySkinName.NickName);
			return;
		}
		TypeKills typeKills = (isMechActive ? TypeKills.mech : (isHeadShot ? TypeKills.headshot : (isZooming ? TypeKills.zoomingshot : TypeKills.none)));
		float num3 = 0f;
		if (weaponSounds.isRoundMelee)
		{
			float num4 = ((ExpController.Instance != null && ExpController.Instance.OurTier < weaponSounds.DamageByTier.Length) ? weaponSounds.DamageByTier[TierOrRoomTier(ExpController.Instance.OurTier)] : ((weaponSounds.DamageByTier.Length <= 0) ? 0f : weaponSounds.DamageByTier[0]));
			float num5 = num4 * 0.7f;
			float num6 = num4;
			num3 = (num5 + (num6 - num5) * (1f - sqrDistance / (weaponSounds.radiusRoundMelee * weaponSounds.radiusRoundMelee))) * (1f + koofDamageWeaponFromPotoins + ((!isMechActive) ? EffectsController.DamageModifsByCats(weaponSounds.categoryNabor - 1) : 0f));
		}
		else
		{
			num3 = GetMultyDamage() * num * (1f + koofDamageWeaponFromPotoins + ((!isMechActive) ? EffectsController.DamageModifsByCats(weaponSounds.categoryNabor - 1) : 0f));
		}
		myScoreController.AddScoreOnEvent(playerMoveC.isMechActive ? ((!isHeadShot) ? PlayerEventScoreController.ScoreEvent.damageMechBody : PlayerEventScoreController.ScoreEvent.damageMechHead) : (isHeadShot ? PlayerEventScoreController.ScoreEvent.damageHead : PlayerEventScoreController.ScoreEvent.damageBody), num3);
		if (!isInet)
		{
			playerMoveC.MinusLive(myPlayerIDLocal, num3, typeKills, (int)weaponSounds.typeDead, (!isMechActive) ? weaponSounds.gameObject.name.Replace("(Clone)", string.Empty) : "Chat_Mech");
		}
		else
		{
			playerMoveC.MinusLive(myPlayerID, num3, typeKills, (int)weaponSounds.typeDead, (!isMechActive) ? weaponSounds.gameObject.name.Replace("(Clone)", string.Empty) : "Chat_Mech");
		}
	}

	private void InitPurchaseActions()
	{
		_actionsForPurchasedItems.Add("bigammopack", ProvideAmmo);
		_actionsForPurchasedItems.Add("Fullhealth", ProvideHealth);
		_actionsForPurchasedItems.Add(StoreKitEventListener.elixirID, delegate
		{
			Defs.NumberOfElixirs++;
		});
		_actionsForPurchasedItems.Add(StoreKitEventListener.armor, delegate
		{
		});
		_actionsForPurchasedItems.Add(StoreKitEventListener.armor2, delegate
		{
		});
		_actionsForPurchasedItems.Add(StoreKitEventListener.armor3, delegate
		{
		});
		string[] potions = PotionsController.potions;
		foreach (string key in potions)
		{
			_actionsForPurchasedItems.Add(key, providePotion);
		}
		string[] canBuyWeaponTags = ItemDb.GetCanBuyWeaponTags(true);
		for (int j = 0; j < canBuyWeaponTags.Length; j++)
		{
			string shopIdByTag = ItemDb.GetShopIdByTag(canBuyWeaponTags[j]);
			_actionsForPurchasedItems.Add(shopIdByTag, AddWeaponToInv);
		}
	}

	private void AddWeaponToInv(string shopId)
	{
		string tagByShopId = ItemDb.GetTagByShopId(shopId);
		ItemRecord byTag = ItemDb.GetByTag(tagByShopId);
		if ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None) && byTag != null && !byTag.TemporaryGun)
		{
			SaveWeaponInPrefs(tagByShopId);
		}
		GameObject prefabByTag = _weaponManager.GetPrefabByTag(tagByShopId);
		AddWeapon(prefabByTag);
	}

	public static void SaveWeaponInPrefs(string weaponTag, int timeForRentIndex = 0)
	{
		string storageIdByTag = ItemDb.GetStorageIdByTag(weaponTag);
		if (storageIdByTag == null)
		{
			int tm = TempItemsController.RentTimeForIndex(timeForRentIndex);
			TempItemsController.sharedController.AddTemporaryItem(weaponTag, tm);
			return;
		}
		Storager.setInt(storageIdByTag, 1, true);
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
			PlayerPrefs.Save();
		}
	}
}
