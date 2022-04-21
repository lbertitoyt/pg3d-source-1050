using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using FyberPlugin;
using Rilisoft;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectSceneNGUIController : MonoBehaviour
{
	public enum PlatformConnect
	{
		ios = 1,
		android,
		custom
	}

	public enum RegimGame
	{
		Deathmatch,
		TimeBattle,
		TeamFight,
		DeadlyGames,
		FlagCapture,
		CapturePoints,
		InFriendWindow,
		InClanWindow
	}

	public struct infoServer
	{
		public string ipAddress;

		public int port;

		public string name;

		public string map;

		public int playerLimit;

		public int connectedPlayers;

		public string coments;
	}

	public struct infoClient
	{
		public string ipAddress;

		public string name;

		public string coments;
	}

	public enum ABTestParams
	{
		Old = 1,
		Rating,
		Buff
	}

	public const string PendingInterstitialKey = "PendingInterstitial";

	public static PlatformConnect myPlatformConnect = PlatformConnect.android;

	private string rulesDeadmatch;

	private string rulesDater;

	private string rulesTeamFight;

	private string rulesTimeBattle;

	private string rulesDeadlyGames;

	private string rulesFlagCapture;

	private string rulesCapturePoint;

	public int myLevelGame;

	public UILabel rulesLabel;

	public static int gameTier = 1;

	public static readonly IDictionary<string, string> gameModesLocalizeKey = new Dictionary<string, string>
	{
		{
			0.ToString(),
			"Key_0104"
		},
		{
			1.ToString(),
			"Key_0135"
		},
		{
			2.ToString(),
			"Key_0130"
		},
		{
			3.ToString(),
			"Key_0121"
		},
		{
			4.ToString(),
			"Key_0113"
		},
		{
			5.ToString(),
			"Key_1263"
		},
		{
			6.ToString(),
			"Key_1465"
		},
		{
			7.ToString(),
			"Key_1466"
		}
	};

	public List<infoServer> servers = new List<infoServer>();

	private float posNumberOffPlayersX = -139f;

	private string goMapName;

	public static TypeModeGame curSelectMode;

	private Dictionary<string, Texture> mapPreview = new Dictionary<string, Texture>();

	public UILabel priceRegimLabel;

	public UILabel priceMapLabel;

	public UILabel priceMapLabelInCreate;

	public GameObject mapPreviewTexture;

	public GameObject grid;

	public MyCenterOnChild centerScript;

	public Transform ScrollTransform;

	public Transform selectMapPanelTransform;

	public MapPreviewController selectMap;

	public float widthCell;

	public int countMap;

	public UIButton createRoomUIBtn;

	public UISprite fonMapPreview;

	public UIPanel mapPreviewPanel;

	public GameObject mainPanel;

	public GameObject localBtn;

	public GameObject customBtn;

	public GameObject randomBtn;

	public GameObject goBtn;

	public GameObject backBtn;

	public GameObject unlockBtn;

	public GameObject unlockMapBtnInCreate;

	public GameObject unlockMapBtn;

	public GameObject coinsShopButton;

	public GameObject cancelFromConnectToPhotonBtn;

	public GameObject connectToPhotonPanel;

	public GameObject failInternetLabel;

	public GameObject customPanel;

	public GameObject gameInfoItemPrefab;

	public GameObject loadingMapPanel;

	public GameObject searchPanel;

	public GameObject clearBtn;

	public GameObject searchBtn;

	public GameObject showSearchPanelBtn;

	public GameObject selectMapPanel;

	public GameObject createPanel;

	public GameObject goToCreateRoomBtn;

	public GameObject createRoomBtn;

	public GameObject setPasswordBtn;

	public GameObject clearInSetPasswordBtn;

	public GameObject okInsetPasswordBtn;

	public GameObject setPasswordPanel;

	public GameObject passONSprite;

	public GameObject enterPasswordPanel;

	public GameObject joinRoomFromEnterPasswordBtn;

	public GameObject connectToWiFIInCreateLabel;

	public GameObject connectToWiFIInCustomLabel;

	public Transform scrollViewSelectMapTransform;

	public PlusMinusController numberOfPlayer;

	public PlusMinusController killToWin;

	public TeamNumberOfPlayer teamCountPlayer;

	public UIGrid gridGames;

	public UIInput searchInput;

	public UIInput nameServerInput;

	public UIInput setPasswordInput;

	public UIInput enterPasswordInput;

	public Transform gridGamesTransform;

	public UITexture loadingToDraw;

	public UILabel conditionLabel;

	private static RegimGame _regim = RegimGame.Deathmatch;

	public static bool isReturnFromGame;

	public int nRegim;

	private bool isSetUseMap;

	public string gameNameFilter;

	public List<GameObject> gamesInfo = new List<GameObject>();

	public DisableObjectFromTimer gameIsfullLabel;

	public DisableObjectFromTimer incorrectPasswordLabel;

	public DisableObjectFromTimer serverIsNotAvalible;

	public DisableObjectFromTimer accountBlockedLabel;

	public DisableObjectFromTimer nameAlreadyUsedLabel;

	private float timerShowBan = -1f;

	private bool isConnectingToPhoton;

	private bool isCancelConnectingToPhoton;

	private int pressButton;

	private List<RoomInfo> filteredRoomList = new List<RoomInfo>();

	private int countNoteCaptureDeadmatch = 5;

	private int countNoteCaptureCOOP = 5;

	private int countNoteCaptureHunger = 5;

	private int countNoteCaptureFlag = 5;

	private int countNoteCaptureCompany = 5;

	public static ConnectSceneNGUIController sharedController;

	private string password = string.Empty;

	public LANBroadcastService lanScan;

	private RoomInfo joinRoomInfoFromCustom;

	private bool firstConnectToPhoton;

	private bool isGoInPhotonGame;

	private bool isMainPanelActiv = true;

	public GameObject ChooseMapLabelSmall;

	private AdvertisementController _advertisementController;

	public UIToggle deathmatchToggle;

	public UIToggle teamFightToogle;

	public UIToggle timeBattleToogle;

	public UIToggle deadlyGamesToogle;

	public UIToggle flagCaptureToogle;

	public UIToggle capturePointsToogle;

	public bool isStartShowAdvert;

	private Action actAfterConnectToPhoton;

	private GameInfo[] roomFields;

	public UIWrapContent wrapGames;

	public UIScrollView scrollGames;

	public static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> mapStatistics = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>();

	public static string selectedMap = string.Empty;

	public static bool directedFromQuests = false;

	public GameObject modeAnimObj;

	public GameObject fingerAnimObj;

	public UIButton[] modeButtonByLevel;

	public UILabel[] modeUnlockLabelByLevel;

	private bool fingerStopped;

	private bool animationStarted;

	private bool loadReplaceAdmobPerelivRunning;

	private bool loadAdmobRunning;

	private int _countOfLoopsRequestAdThisTime;

	private float _lastTimeInterstitialShown;

	public static bool NeedShowReviewInConnectScene = false;

	public static readonly string mapProperty = "C0";

	public static readonly string passwordProperty = "C1";

	public static readonly string platformProperty = "C2";

	public static readonly string endingProperty = "C3";

	public static readonly string maxKillProperty = "C4";

	public static readonly string ABTestProperty = "C5";

	public static readonly string ABTestEnum = "C6";

	private bool abTestConnect = Defs.isActivABTestBuffSystem || Defs.isActivABTestRatingSystem;

	private int joinNewRoundTries;

	private int tryJoinRoundMap;

	private IDisposable _someWindowSubscription;

	private int _tempMinValue = 3;

	private int _tempMaxValue = 7;

	private int _tempStep = 2;

	private int daterStep = 5;

	private int daterMinValue = 5;

	private int daterMaxValue = 10;

	private IDisposable _backSubscription;

	private float startPosX;

	private LoadingNGUIController _loadingNGUIController;

	private LANBroadcastService.ReceivedMessage[] _copy;

	public static RegimGame regim
	{
		get
		{
			return _regim;
		}
		set
		{
			_regim = value;
			UpdateUseMasMaps();
		}
	}

	internal static bool InterstitialRequest { get; set; }

	internal static bool ReplaceAdmobWithPerelivRequest { get; set; }

	public static string MainLoadingTexture()
	{
		return (!Device.isRetinaAndStrong) ? "main_loading" : "main_loading_Hi";
	}

	public static void GoToClans()
	{
		LoadConnectScene.textureToShow = null;
		LoadConnectScene.sceneToLoad = "Clans";
		LoadConnectScene.noteToShow = null;
		SceneManager.LoadScene(Defs.PromSceneName);
	}

	public static void GoToFriends()
	{
		FriendsController friendsController = FriendsController.sharedController;
		if (friendsController != null)
		{
			friendsController.GetFriendsData();
		}
		MainMenuController.friendsOnStart = true;
		LoadConnectScene.textureToShow = null;
		LoadConnectScene.sceneToLoad = Defs.MainMenuScene;
		LoadConnectScene.noteToShow = null;
		SceneManager.LoadScene(Defs.PromSceneName);
		Defs.isDaterRegim = false;
	}

	public static void Local()
	{
		if (EveryplayWrapper.Instance.CurrentState == EveryplayWrapper.State.Paused || EveryplayWrapper.Instance.CurrentState == EveryplayWrapper.State.Recording)
		{
			EveryplayWrapper.Instance.Stop();
		}
		PhotonNetwork.Disconnect();
		if (Defs.isGameFromFriends)
		{
			GoToFriends();
			return;
		}
		if (Defs.isGameFromClans)
		{
			GoToClans();
			return;
		}
		LoadConnectScene.textureToShow = null;
		if (!Defs.isDaterRegim)
		{
			LoadConnectScene.sceneToLoad = "ConnectScene";
		}
		else
		{
			LoadConnectScene.sceneToLoad = "ConnectSceneSandbox";
		}
		LoadConnectScene.noteToShow = null;
		SceneManager.LoadScene(Defs.PromSceneName);
	}

	public static void GoToProfile()
	{
		PlayerPrefs.SetInt(Defs.SkinEditorMode, 1);
		GlobalGameController.EditingLogo = 0;
		GlobalGameController.EditingCape = 0;
		SceneManager.LoadScene("SkinEditor");
	}

	public void StopFingerAnim()
	{
		if (fingerAnimObj != null && fingerAnimObj.activeSelf)
		{
			fingerStopped = true;
			fingerAnimObj.SetActive(false);
			UIScrollView component = scrollViewSelectMapTransform.GetComponent<UIScrollView>();
			component.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Remove(component.onDragStarted, new UIScrollView.OnDragNotification(StopFingerAnim));
		}
	}

	private void OnEnableWhenAnimate()
	{
		if (animationStarted)
		{
			StopFingerAnim();
			modeAnimObj.SetActive(false);
			fingerStopped = false;
			StartCoroutine(AnimateModeOpen());
		}
	}

	private IEnumerator AnimateModeOpen()
	{
		modeAnimObj.GetComponent<AudioSource>().enabled = Defs.isSoundFX;
		animationStarted = true;
		if (!TrainingController.TrainingCompleted)
		{
			localBtn.GetComponent<UIButton>().isEnabled = false;
			randomBtn.GetComponent<UIButton>().isEnabled = false;
			customBtn.GetComponent<UIButton>().isEnabled = false;
			goBtn.GetComponent<UIButton>().isEnabled = false;
		}
		int storagedStage = Storager.getInt("ModeUnlockStage", false);
		if (storagedStage == 0 && Storager.getInt("TrainingCompleted_4_4_Sett", false) == 1)
		{
			storagedStage = modeButtonByLevel.Length;
		}
		int currentStage = Mathf.Clamp(storagedStage, 0, modeButtonByLevel.Length);
		for (int j = 0; j < modeButtonByLevel.Length; j++)
		{
			modeButtonByLevel[j].isEnabled = j < currentStage;
		}
		int currentLevel = ExperienceController.sharedController.currentLevel;
		if (currentLevel >= 4)
		{
			currentLevel = modeButtonByLevel.Length;
		}
		if (modeUnlockLabelByLevel != null)
		{
			for (int i = 0; i < modeUnlockLabelByLevel.Length; i++)
			{
				modeUnlockLabelByLevel[i].gameObject.SetActive(i > Mathf.Max(currentStage, currentLevel) - 2);
				modeUnlockLabelByLevel[i].text = string.Format(LocalizationStore.Get("Key_1923"), Mathf.Min(i + 2, 4));
			}
		}
		if (currentStage < Mathf.Min(currentLevel, modeButtonByLevel.Length))
		{
			BannerWindowController.SharedController.AddBannersTimeout(20.1f);
		}
		if (currentStage == 0 && !TrainingController.TrainingCompleted)
		{
			UIScrollView component = scrollViewSelectMapTransform.GetComponent<UIScrollView>();
			component.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Combine(component.onDragStarted, new UIScrollView.OnDragNotification(StopFingerAnim));
			modeButtonByLevel[0].GetComponent<UIToggle>().value = false;
			modeButtonByLevel[0].GetComponent<UIToggle>().Set(false);
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Connect_Scene);
		}
		yield return new WaitForSeconds(0.5f);
		for (; currentStage < Mathf.Min(currentLevel, modeButtonByLevel.Length); currentStage++)
		{
			BannerWindowController.SharedController.AddBannersTimeout(20.1f);
			if (currentStage == 1)
			{
				BannerWindowController.firstScreen = true;
				BannerWindowController.SharedController.ClearBannerStates();
			}
			UIButton currentMode = modeButtonByLevel[currentStage];
			modeAnimObj.transform.SetParent(currentMode.transform, false);
			modeAnimObj.SetActive(true);
			yield return new WaitForSeconds(0.1f);
			if (currentStage == 0)
			{
				modeButtonByLevel[currentStage].GetComponent<UIToggle>().value = true;
			}
			modeButtonByLevel[currentStage].isEnabled = true;
			yield return new WaitForSeconds(1.4f);
			modeAnimObj.SetActive(false);
			if (currentStage == 0 && !TrainingController.TrainingCompleted)
			{
				if (!fingerStopped)
				{
					fingerAnimObj.SetActive(true);
					yield return new WaitForSeconds(0.22f);
				}
				if (!fingerStopped)
				{
					SpringPanel.Begin(scrollViewSelectMapTransform.gameObject, scrollViewSelectMapTransform.localPosition - Vector3.left * 410f, 3f);
					yield return new WaitForSeconds(1.65f);
				}
				if (!fingerStopped)
				{
					SpringPanel.Begin(scrollViewSelectMapTransform.gameObject, scrollViewSelectMapTransform.localPosition - Vector3.left * 410f, 3f);
					yield return new WaitForSeconds(1.75f);
				}
				if (!fingerStopped)
				{
					SpringPanel.Begin(scrollViewSelectMapTransform.gameObject, scrollViewSelectMapTransform.localPosition - Vector3.left * 410f, 3f);
					yield return new WaitForSeconds(0.7f);
					fingerAnimObj.SetActive(false);
					UIScrollView component2 = scrollViewSelectMapTransform.GetComponent<UIScrollView>();
					component2.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Remove(component2.onDragStarted, new UIScrollView.OnDragNotification(StopFingerAnim));
					scrollViewSelectMapTransform.GetChild(0).GetComponent<MyCenterOnChild>().Recenter();
				}
			}
			if (currentStage == 1)
			{
				HintController.instance.ShowHintByName("deathmatch", 0f);
				HintController.instance.ShowHintByName("gobattletimeout", 0f);
			}
		}
		if (storagedStage != currentStage)
		{
			Storager.setInt("ModeUnlockStage", currentStage, false);
		}
		if (!TrainingController.TrainingCompleted)
		{
			goBtn.GetComponent<UIButton>().isEnabled = true;
			HintController.instance.ShowHintByName("gobattle", 0f);
		}
		animationStarted = false;
	}

	private void Start()
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted)
		{
			WeaponManager.sharedManager.SaveWeaponAsLastUsed(0);
		}
		if (FriendsController.sharedController != null)
		{
			FriendsController.sharedController.profileInfo.Clear();
		}
		Defs.isDaterRegim = SceneLoader.ActiveSceneName.Equals("ConnectSceneSandbox");
		WeaponManager.RefreshExpControllers();
		rulesDeadmatch = LocalizationStore.Key_0550;
		rulesTeamFight = LocalizationStore.Key_0551;
		rulesTimeBattle = LocalizationStore.Key_0552;
		rulesDeadlyGames = LocalizationStore.Key_0553;
		rulesFlagCapture = LocalizationStore.Key_0554;
		rulesCapturePoint = LocalizationStore.Get("Key_1368");
		rulesDater = LocalizationStore.Get("Key_1538");
		sharedController = this;
		myLevelGame = ((!(ExperienceController.sharedController != null) || ExperienceController.sharedController.currentLevel > 2) ? ((ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel <= 5) ? 1 : 2) : 0);
		mainPanel.SetActive(false);
		coinsShopButton.SetActive(false);
		selectMapPanel.SetActive(false);
		createPanel.SetActive(false);
		customPanel.SetActive(false);
		searchPanel.SetActive(false);
		setPasswordPanel.SetActive(false);
		enterPasswordPanel.SetActive(false);
		StartSearchLocalServers();
		PlayerPrefs.SetString("TypeGame", "client");
		gameIsfullLabel.gameObject.SetActive(false);
		accountBlockedLabel.gameObject.SetActive(false);
		serverIsNotAvalible.gameObject.SetActive(false);
		nameAlreadyUsedLabel.gameObject.SetActive(false);
		incorrectPasswordLabel.gameObject.SetActive(false);
		unlockMapBtn.SetActive(false);
		unlockMapBtnInCreate.SetActive(false);
		unlockBtn.SetActive(false);
		string path = MainLoadingTexture();
		loadingToDraw.mainTexture = Resources.Load<Texture>(path);
		loadingMapPanel.SetActive(true);
		connectToPhotonPanel.SetActive(false);
		if (PhotonNetwork.connectionState == ConnectionState.Connected)
		{
			firstConnectToPhoton = true;
		}
		if (!Defs.isDaterRegim)
		{
			ScrollTransform.GetComponent<UIPanel>().baseClipRegion = new Vector4(0f, 0f, 760 * Screen.width / Screen.height, 350f);
		}
		SetPosSelectMapPanelInMainMenu();
		regim = ((!TrainingController.TrainingCompleted) ? RegimGame.TeamFight : ((!Defs.isDaterRegim) ? ((RegimGame)PlayerPrefs.GetInt("RegimMulty", 2)) : RegimGame.Deathmatch));
		directedFromQuests = false;
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(selectedMap);
		if (infoScene != null)
		{
			if (infoScene.IsAvaliableForMode(TypeModeGame.TeamFight))
			{
				regim = RegimGame.TeamFight;
			}
			else if (infoScene.IsAvaliableForMode(TypeModeGame.Deathmatch))
			{
				regim = RegimGame.Deathmatch;
			}
			else if (infoScene.IsAvaliableForMode(TypeModeGame.FlagCapture))
			{
				regim = RegimGame.FlagCapture;
			}
			else if (infoScene.IsAvaliableForMode(TypeModeGame.CapturePoints))
			{
				regim = RegimGame.CapturePoints;
			}
			else if (infoScene.IsAvaliableForMode(TypeModeGame.DeadlyGames))
			{
				regim = RegimGame.DeadlyGames;
			}
			else if (infoScene.IsAvaliableForMode(TypeModeGame.TimeBattle))
			{
				regim = RegimGame.TimeBattle;
			}
		}
		if (!Defs.isDaterRegim)
		{
			deathmatchToggle.value = regim == RegimGame.Deathmatch;
			timeBattleToogle.value = regim == RegimGame.TimeBattle;
			teamFightToogle.value = regim == RegimGame.TeamFight;
			deadlyGamesToogle.value = regim == RegimGame.DeadlyGames;
			flagCaptureToogle.value = regim == RegimGame.FlagCapture;
			capturePointsToogle.value = regim == RegimGame.CapturePoints;
			deathmatchToggle.GetComponent<ButtonHandler>().Clicked += SetRegimDeathmatch;
			timeBattleToogle.GetComponent<ButtonHandler>().Clicked += SetRegimTimeBattle;
			teamFightToogle.GetComponent<ButtonHandler>().Clicked += SetRegimTeamFight;
			deadlyGamesToogle.GetComponent<ButtonHandler>().Clicked += SetRegimDeadleGames;
			flagCaptureToogle.GetComponent<ButtonHandler>().Clicked += SetRegimFlagCapture;
			capturePointsToogle.GetComponent<ButtonHandler>().Clicked += SetRegimCapturePoints;
		}
		StartCoroutine(LoadMapPreview());
		if (localBtn != null)
		{
			ButtonHandler component = localBtn.GetComponent<ButtonHandler>();
			if (component != null)
			{
				component.Clicked += HandleLocalBtnClicked;
			}
		}
		if (customBtn != null)
		{
			ButtonHandler component2 = customBtn.GetComponent<ButtonHandler>();
			if (component2 != null)
			{
				component2.Clicked += HandleCustomBtnClicked;
			}
		}
		if (randomBtn != null)
		{
			ButtonHandler component3 = randomBtn.GetComponent<ButtonHandler>();
			if (component3 != null)
			{
				component3.Clicked += HandleRandomBtnClicked;
			}
		}
		if (goBtn != null)
		{
			ButtonHandler component4 = goBtn.GetComponent<ButtonHandler>();
			if (component4 != null)
			{
				component4.Clicked += HandleGoBtnClicked;
			}
		}
		if (backBtn != null)
		{
			ButtonHandler component5 = backBtn.GetComponent<ButtonHandler>();
			if (component5 != null)
			{
				component5.Clicked += HandleBackBtnClicked;
			}
		}
		if (unlockBtn != null)
		{
			ButtonHandler component6 = unlockBtn.GetComponent<ButtonHandler>();
			if (component6 != null)
			{
				component6.Clicked += HandleUnlockBtnClicked;
			}
		}
		if (coinsShopButton != null)
		{
			ButtonHandler component7 = coinsShopButton.GetComponent<ButtonHandler>();
			if (component7 != null)
			{
				component7.Clicked += HandleCoinsShopClicked;
			}
		}
		if (unlockMapBtn != null)
		{
			ButtonHandler component8 = unlockMapBtn.GetComponent<ButtonHandler>();
			if (component8 != null)
			{
				component8.Clicked += HandleUnlockMapBtnClicked;
			}
		}
		if (unlockMapBtnInCreate != null)
		{
			ButtonHandler component9 = unlockMapBtnInCreate.GetComponent<ButtonHandler>();
			if (component9 != null)
			{
				component9.Clicked += HandleUnlockMapBtnClicked;
			}
		}
		if (cancelFromConnectToPhotonBtn != null)
		{
			ButtonHandler component10 = cancelFromConnectToPhotonBtn.GetComponent<ButtonHandler>();
			if (component10 != null)
			{
				component10.Clicked += HandleCancelFromConnectToPhotonBtnClicked;
			}
		}
		if (clearBtn != null)
		{
			ButtonHandler component11 = clearBtn.GetComponent<ButtonHandler>();
			if (component11 != null)
			{
				component11.Clicked += HandleClearBtnClicked;
			}
		}
		if (searchBtn != null)
		{
			ButtonHandler component12 = searchBtn.GetComponent<ButtonHandler>();
			if (component12 != null)
			{
				component12.Clicked += HandleSearchBtnClicked;
			}
		}
		if (showSearchPanelBtn != null)
		{
			ButtonHandler component13 = showSearchPanelBtn.GetComponent<ButtonHandler>();
			if (component13 != null)
			{
				component13.Clicked += HandleShowSearchPanelBtnClicked;
			}
		}
		if (goToCreateRoomBtn != null)
		{
			ButtonHandler component14 = goToCreateRoomBtn.GetComponent<ButtonHandler>();
			if (component14 != null)
			{
				component14.Clicked += HandleGoToCreateRoomBtnClicked;
			}
		}
		if (createRoomBtn != null)
		{
			createRoomUIBtn = createRoomBtn.GetComponent<UIButton>();
			ButtonHandler component15 = createRoomBtn.GetComponent<ButtonHandler>();
			if (component15 != null)
			{
				component15.Clicked += HandleCreateRoomBtnClicked;
			}
		}
		if (setPasswordBtn != null)
		{
			ButtonHandler component16 = setPasswordBtn.GetComponent<ButtonHandler>();
			if (component16 != null)
			{
				component16.Clicked += HandleSetPasswordBtnClicked;
			}
		}
		if (clearInSetPasswordBtn != null)
		{
			ButtonHandler component17 = clearInSetPasswordBtn.GetComponent<ButtonHandler>();
			if (component17 != null)
			{
				component17.Clicked += HandleClearInSetPasswordBtnClicked;
			}
		}
		if (okInsetPasswordBtn != null)
		{
			ButtonHandler component18 = okInsetPasswordBtn.GetComponent<ButtonHandler>();
			if (component18 != null)
			{
				component18.Clicked += delegate
				{
					OnPaswordSelected();
				};
			}
		}
		if (joinRoomFromEnterPasswordBtn != null)
		{
			ButtonHandler component19 = joinRoomFromEnterPasswordBtn.GetComponent<ButtonHandler>();
			if (component19 != null)
			{
				component19.Clicked += HandleJoinRoomFromEnterPasswordBtnClicked;
			}
		}
		if (!Defs.isDaterRegim)
		{
			if (true)
			{
				SetUnLockedButton(flagCaptureToogle);
			}
			if (true)
			{
				SetUnLockedButton(deadlyGamesToogle);
			}
		}
		InitializeBannerWindow();
		InterstitialManager.Instance.ResetAdProvider();
		if (!NeedShowReviewInConnectScene)
		{
			if (ReplaceAdmobPerelivController.ReplaceAdmobWithPerelivApplicable() && ReplaceAdmobWithPerelivRequest)
			{
				ReplaceAdmobWithPerelivRequest = false;
				StartCoroutine(WaitLoadingAndShowReplaceAdmobPereliv("Connect Scene", false));
			}
			else
			{
				Debug.LogFormat("{0}, Start(), InterstitialRequest: {1}", GetType().Name, InterstitialRequest);
				if (MobileAdManager.AdIsApplicable(MobileAdManager.Type.Image, true) && InterstitialRequest)
				{
					isStartShowAdvert = true;
					StartCoroutine(WaitLoadingAndShowInterstitialCoroutine("Connect Scene", false));
				}
			}
		}
		wrapGames.onInitializeItem = OnInitializeItem;
		enterPasswordInput.onSubmit.Add(new EventDelegate(EnterPassInputSubmit));
	}

	private IEnumerator OnApplicationPause(bool pausing)
	{
		if (pausing)
		{
			LogUserQuit();
			lanScan.StopBroadCasting();
			yield break;
		}
		yield return new WaitForSeconds(1f);
		StartSearchLocalServers();
		InterstitialManager.Instance.ResetAdProvider();
		if (MobileAdManager.Instance.SuppressShowOnReturnFromPause)
		{
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = false;
			yield break;
		}
		bool shouldShowReplaceAdmob = ReplaceAdmobPerelivController.ReplaceAdmobWithPerelivApplicable() && ReplaceAdmobPerelivController.sharedController != null;
		if (shouldShowReplaceAdmob)
		{
			ReplaceAdmobPerelivController.IncreaseTimesCounter();
		}
		if (shouldShowReplaceAdmob && ReplaceAdmobPerelivController.ShouldShowAtThisTime && !loadAdmobRunning)
		{
			StartCoroutine(WaitLoadingAndShowReplaceAdmobPereliv("On return from pause to Connect Scene"));
		}
	}

	private IEnumerator WaitLoadingAndShowReplaceAdmobPereliv(string context, bool loadData = true)
	{
		if (loadReplaceAdmobPerelivRunning)
		{
			yield break;
		}
		try
		{
			loadReplaceAdmobPerelivRunning = true;
			if (loadData && !ReplaceAdmobPerelivController.sharedController.DataLoading && !ReplaceAdmobPerelivController.sharedController.DataLoaded)
			{
				ReplaceAdmobPerelivController.sharedController.LoadPerelivData();
			}
			while (ReplaceAdmobPerelivController.sharedController == null || !ReplaceAdmobPerelivController.sharedController.DataLoaded)
			{
				if (!ReplaceAdmobPerelivController.sharedController.DataLoading)
				{
					loadReplaceAdmobPerelivRunning = false;
					yield break;
				}
				yield return null;
			}
			if (mainPanel != null)
			{
				while (!mainPanel.activeInHierarchy)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.5f);
			}
			ReplaceAdmobPerelivController.TryShowPereliv(context);
			ReplaceAdmobPerelivController.sharedController.DestroyImage();
		}
		finally
		{
			loadReplaceAdmobPerelivRunning = false;
		}
	}

	private IEnumerator WaitLoadingAndShowInterstitialCoroutine(string context, bool loadData = true)
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Starting WaitLoadingAndShowInterstitialCoroutine()    " + InterstitialManager.Instance.Provider);
		}
		if (loadAdmobRunning)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Quitting WaitLoadingAndShowInterstitialCoroutine() because loadAdmobRunning==true");
			}
			yield break;
		}
		loadAdmobRunning = true;
		try
		{
			if (InterstitialManager.Instance.Provider == AdProvider.Fyber)
			{
				float loadAttemptTime = Time.realtimeSinceStartup;
				if (Defs.IsDeveloperBuild)
				{
					Debug.Log("FyberFacade.Instance.Requests.Count: " + FyberFacade.Instance.Requests.Count);
				}
				if (FyberFacade.Instance.Requests.Count == 0)
				{
					LogUserInterstitialRequest();
					System.Threading.Tasks.Task<Ad> r2 = FyberFacade.Instance.RequestImageInterstitial("WaitLoadingAndShowInterstitialCoroutine(), requests count: 0");
					FyberFacade.Instance.Requests.AddLast(r2);
				}
				if (Defs.IsDeveloperBuild)
				{
					Debug.Log("Waiting either at least one loading request completed successfully, or all failed...");
				}
				while (true)
				{
					if (((IEnumerable<System.Threading.Tasks.Task<Ad>>)FyberFacade.Instance.Requests).Any((System.Threading.Tasks.Task<Ad> r) => ((System.Threading.Tasks.Task)r).get_IsCompleted() && !((System.Threading.Tasks.Task)r).get_IsFaulted()))
					{
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("Found successfully completed request among {0}", FyberFacade.Instance.Requests.Count);
						}
						break;
					}
					if (((IEnumerable<System.Threading.Tasks.Task<Ad>>)FyberFacade.Instance.Requests).All((System.Threading.Tasks.Task<Ad> r) => ((System.Threading.Tasks.Task)r).get_IsCompleted()))
					{
						if (Defs.IsDeveloperBuild)
						{
							Debug.Log("All requests are completed.");
						}
						break;
					}
					if (Time.realtimeSinceStartup - loadAttemptTime > 5.2f)
					{
						if (Defs.IsDeveloperBuild)
						{
							Debug.Log("Loading timed out.");
						}
						break;
					}
					yield return null;
				}
				List<System.Threading.Tasks.Task<Ad>> completedRequests = ((IEnumerable<System.Threading.Tasks.Task<Ad>>)FyberFacade.Instance.Requests).Where((System.Threading.Tasks.Task<Ad> r) => ((System.Threading.Tasks.Task)r).get_IsCompleted()).ToList();
				List<System.Threading.Tasks.Task<Ad>> noOffersRequests = completedRequests.Where((System.Threading.Tasks.Task<Ad> r) => ((System.Threading.Tasks.Task)r).get_IsFaulted() && ((Exception)(object)((System.Threading.Tasks.Task)r).get_Exception()).InnerException is AdNotAwailableException).ToList();
				if (noOffersRequests.Count > 0)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("Removing not filled requests: " + noOffersRequests.Count);
					}
					foreach (System.Threading.Tasks.Task<Ad> noOffersRequest in noOffersRequests)
					{
						FyberFacade.Instance.Requests.Remove(noOffersRequest);
						completedRequests = null;
					}
				}
				if (completedRequests == null)
				{
					completedRequests = ((IEnumerable<System.Threading.Tasks.Task<Ad>>)FyberFacade.Instance.Requests).Where((System.Threading.Tasks.Task<Ad> r) => ((System.Threading.Tasks.Task)r).get_IsCompleted()).ToList();
				}
				List<System.Threading.Tasks.Task<Ad>> errorRequests = completedRequests.Where((System.Threading.Tasks.Task<Ad> r) => ((System.Threading.Tasks.Task)r).get_IsFaulted() && ((Exception)(object)((System.Threading.Tasks.Task)r).get_Exception()).InnerException is AdRequestException).ToList();
				if (errorRequests.Count > 0)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("Removing failed requests: " + errorRequests.Count);
					}
					foreach (System.Threading.Tasks.Task<Ad> errorRequest in errorRequests)
					{
						FyberFacade.Instance.Requests.Remove(errorRequest);
						completedRequests = null;
					}
				}
				if (mainPanel != null)
				{
					while (!mainPanel.activeInHierarchy)
					{
						yield return null;
					}
					yield return new WaitForSeconds(0.5f);
				}
				if (!PhotonNetwork.inRoom)
				{
					Dictionary<string, string> attributes = new Dictionary<string, string>
					{
						{ "af_content_type", "Interstitial" },
						{ "af_content_id", "Interstitial (ConnectScene)" }
					};
					FlurryPluginWrapper.LogEventToAppsFlyer("af_content_view", attributes);
					System.Threading.Tasks.Task<AdResult> showTask = FyberFacade.Instance.ShowInterstitial(new Dictionary<string, string> { { "Context", "Connect Scene" } }, "WaitLoadingAndShowInterstitialCoroutine()");
					Storager.setInt("PendingInterstitial", 8, false);
					string context2 = default(string);
					showTask.ContinueWith((Action<System.Threading.Tasks.Task<AdResult>>)delegate(System.Threading.Tasks.Task<AdResult> t)
					{
						Storager.setInt("PendingInterstitial", 0, false);
						isStartShowAdvert = false;
						if (((System.Threading.Tasks.Task)t).get_IsFaulted())
						{
							Debug.LogWarningFormat("[Rilisoft] Showing interstitial failed: {0}", ((Exception)(object)((System.Threading.Tasks.Task)t).get_Exception()).InnerException.Message);
						}
						else
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string> { { "Context", context2 } };
							if (ExperienceController.sharedController != null)
							{
								dictionary.Add("Level", ExperienceController.sharedController.currentLevel.ToString());
							}
							if (ExpController.Instance != null)
							{
								dictionary.Add("Tier", ExpController.Instance.OurTier.ToString());
							}
							FlurryPluginWrapper.LogEventAndDublicateToConsole("Fyber ADV Interstitial", dictionary);
							LogIsShowAdvert("Connect Scene", true);
						}
					});
				}
				else
				{
					Dictionary<string, string> parameters2 = new Dictionary<string, string> { { "Fyber - Interstitial", "Impression: Canceled (in Photon room)" } };
					FlurryPluginWrapper.LogEventAndDublicateToConsole("Ads Show Stats - Total", parameters2);
				}
			}
			else if (InterstitialManager.Instance.Provider == AdProvider.GoogleMobileAds)
			{
				float loadAttemptTime2 = Time.realtimeSinceStartup;
				if (!loadData || MobileAdManager.Instance.ImageInterstitialState != MobileAdManager.State.Loaded)
				{
				}
				while (MobileAdManager.Instance.ImageInterstitialState != MobileAdManager.State.Loaded)
				{
					if (!string.IsNullOrEmpty(MobileAdManager.Instance.ImageAdFailedToLoadMessage) || Time.realtimeSinceStartup - loadAttemptTime2 > 5.2f)
					{
						bool unitIdsLooped = MobileAdManager.Instance.SwitchImageAdUnitId();
						if (unitIdsLooped)
						{
							InterstitialManager.Instance.SwitchAdProvider();
						}
						if (!unitIdsLooped || _countOfLoopsRequestAdThisTime < PromoActionsManager.MobileAdvert.CountRoundReplaceProviders - 1)
						{
							if (unitIdsLooped)
							{
								_countOfLoopsRequestAdThisTime++;
							}
							if (InterstitialManager.Instance.Provider == AdProvider.Fyber)
							{
								LogUserInterstitialRequest();
								System.Threading.Tasks.Task<Ad> request = FyberFacade.Instance.RequestImageInterstitial("WaitLoadingAndShowInterstitialCoroutine()");
								FyberFacade.Instance.Requests.AddLast(request);
							}
							else if (InterstitialManager.Instance.Provider == AdProvider.GoogleMobileAds)
							{
								LogUserInterstitialRequest();
							}
							loadAdmobRunning = false;
							yield return StartCoroutine(WaitLoadingAndShowInterstitialCoroutine(context, loadData));
						}
						yield break;
					}
					yield return null;
				}
				if (mainPanel != null)
				{
					while (!mainPanel.activeInHierarchy)
					{
						yield return null;
					}
					yield return new WaitForSeconds(0.5f);
				}
				while (ShopNGUIController.GuiActive)
				{
					yield return null;
				}
				if (!PhotonNetwork.inRoom)
				{
					MobileAdManager.Instance.ShowImageInterstitial(context);
					MobileAdManager.Instance.DestroyImageInterstitial();
				}
				else
				{
					Dictionary<string, string> parameters = new Dictionary<string, string> { { "AdMob - Image", "Impression: Canceled (in Photon room)" } };
					FlurryPluginWrapper.LogEventAndDublicateToConsole("Ads Show Stats - Total", parameters);
				}
			}
			_lastTimeInterstitialShown = Time.realtimeSinceStartup;
		}
		finally
		{
			loadAdmobRunning = false;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Finishing WaitLoadingAndShowInterstitialCoroutine()    " + InterstitialManager.Instance.Provider);
			}
		}
	}

	private void InitializeBannerWindow()
	{
		_advertisementController = base.gameObject.GetComponent<AdvertisementController>();
		if (_advertisementController == null)
		{
			_advertisementController = base.gameObject.AddComponent<AdvertisementController>();
		}
		BannerWindowController.SharedController.advertiseController = _advertisementController;
	}

	private void SetUnLockedButton(UIToggle butToogle)
	{
		UIButton component = butToogle.gameObject.GetComponent<UIButton>();
		component.normalSprite = "yell_btn";
		component.hoverSprite = "yell_btn";
		component.pressedSprite = "green_btn_n";
		butToogle.transform.Find("LockedSprite").gameObject.SetActive(false);
		butToogle.transform.Find("Checkmark").GetComponent<UISprite>().spriteName = "green_btn";
	}

	private void SetRegimDeathmatch(object sender, EventArgs e)
	{
		if (regim != 0)
		{
			SetRegim(RegimGame.Deathmatch);
		}
	}

	private void SetRegimTeamFight(object sender, EventArgs e)
	{
		if (regim != RegimGame.TeamFight)
		{
			SetRegim(RegimGame.TeamFight);
		}
	}

	private void SetRegimTimeBattle(object sender, EventArgs e)
	{
		if (regim != RegimGame.TimeBattle)
		{
			SetRegim(RegimGame.TimeBattle);
		}
	}

	private void SetRegimDeadleGames(object sender, EventArgs e)
	{
		if (regim != RegimGame.DeadlyGames)
		{
			SetRegim(RegimGame.DeadlyGames);
		}
	}

	private void SetRegimFlagCapture(object sender, EventArgs e)
	{
		if (regim != RegimGame.FlagCapture)
		{
			SetRegim(RegimGame.FlagCapture);
		}
	}

	private void SetRegimCapturePoints(object sender, EventArgs e)
	{
		if (regim != RegimGame.CapturePoints)
		{
			SetRegim(RegimGame.CapturePoints);
		}
	}

	private void HandleJoinRoomFromEnterPasswordBtnClicked(object sender, EventArgs e)
	{
		if (enterPasswordInput.value.Equals(joinRoomInfoFromCustom.customProperties[passwordProperty].ToString()))
		{
			JoinToRoomPhotonAfterCheck();
			return;
		}
		enterPasswordPanel.SetActive(false);
		ExperienceController.sharedController.isShowRanks = true;
		customPanel.SetActive(true);
		Invoke("UpdateFilteredRoomListInvoke", 0.03f);
	}

	private void HandleSetPasswordBtnClicked(object sender, EventArgs e)
	{
		createPanel.SetActive(false);
		coinsShopButton.SetActive(false);
		selectMapPanel.SetActive(false);
		setPasswordInput.value = password;
		setPasswordPanel.SetActive(true);
	}

	private void HandleClearInSetPasswordBtnClicked(object sender, EventArgs e)
	{
		setPasswordInput.value = string.Empty;
	}

	private void OnPaswordSelected()
	{
		password = setPasswordInput.value;
		BackFromSetPasswordPanel();
	}

	private void BackFromSetPasswordPanel()
	{
		createPanel.SetActive(true);
		coinsShopButton.SetActive(true);
		selectMapPanel.SetActive(true);
		passONSprite.SetActive(!string.IsNullOrEmpty(password));
		setPasswordPanel.SetActive(false);
	}

	public static void CreateGameRoom(string roomName, int playerLimit, int mapIndex, int MaxKill, string password, RegimGame gameMode)
	{
		int num = 9;
		string[] array = new string[num];
		array[0] = mapProperty;
		array[1] = passwordProperty;
		array[2] = platformProperty;
		array[3] = endingProperty;
		array[4] = maxKillProperty;
		array[5] = "TimeMatchEnd";
		array[6] = "tier";
		array[7] = ABTestProperty;
		array[8] = ABTestEnum;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[mapProperty] = mapIndex;
		hashtable[passwordProperty] = password;
		hashtable[platformProperty] = (int)((!string.IsNullOrEmpty(password)) ? PlatformConnect.custom : myPlatformConnect);
		hashtable[endingProperty] = 0;
		hashtable[maxKillProperty] = MaxKill;
		hashtable["TimeMatchEnd"] = PhotonNetwork.time;
		hashtable["tier"] = ExpController.Instance.OurTier;
		if (ExpController.Instance.OurTier == 0)
		{
			hashtable[ABTestProperty] = (Defs.isABTestBalansCohortActual ? 1 : 0);
		}
		if (Defs.isActivABTestRatingSystem)
		{
			hashtable[ABTestEnum] = ((!FriendsController.isUseRatingSystem) ? 1 : 2);
		}
		else if (Defs.isActivABTestBuffSystem)
		{
			hashtable[ABTestEnum] = ((!FriendsController.useBuffSystem) ? 1 : 3);
		}
		PhotonCreateRoom(roomName, true, true, (playerLimit <= 10) ? playerLimit : 10, hashtable, array);
	}

	public static void PhotonCreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers, ExitGames.Client.Photon.Hashtable roomProps, string[] roomPropsInLobby)
	{
		PlayerPrefs.SetString("TypeGame", "server");
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.customRoomProperties = roomProps;
		roomOptions.customRoomPropertiesForLobby = roomPropsInLobby;
		RoomOptions roomOptions2 = roomOptions;
		roomOptions2.maxPlayers = (byte)maxPlayers;
		roomOptions2.isOpen = isOpen;
		roomOptions2.isVisible = isVisible;
		if (!Defs.useSqlLobby)
		{
			PhotonNetwork.CreateRoom(roomName, roomOptions2, TypedLobby.Default);
			return;
		}
		TypedLobby typedLobby = new TypedLobby("PixelGun3D", LobbyType.SqlLobby);
		PhotonNetwork.CreateRoom(roomName, roomOptions2, typedLobby);
	}

	public static void JoinRandomGameRoom(int mapIndex, RegimGame gameMode, int joinToNewRound, bool abTestSeparate = false)
	{
		string text = string.Empty;
		if (Defs.useSqlLobby)
		{
			if (mapIndex == -1)
			{
				TypeModeGame needMode = (TypeModeGame)(int)Enum.Parse(typeof(TypeModeGame), gameMode.ToString());
				int[] array = SceneInfoController.instance.GetListScenesForMode(needMode).avaliableScenes.Select((SceneInfo m) => m.indexMap).ToArray();
				text += "( ";
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = text;
					text = text2 + mapProperty + " = " + array[i];
					if (i + 1 < array.Length)
					{
						text += " OR ";
					}
				}
				text += " )";
			}
			else
			{
				text = mapProperty + " = " + mapIndex;
			}
			text = text + " AND " + passwordProperty + " = \"\"";
			if (!Defs.isDaterRegim)
			{
				string text2 = text;
				string[] obj = new string[5] { text2, " AND ", platformProperty, " = ", null };
				int num = (int)myPlatformConnect;
				obj[4] = num.ToString();
				text = string.Concat(obj);
			}
			switch (joinToNewRound)
			{
			case 0:
				text = text + " AND " + endingProperty + " = 0";
				break;
			case 1:
				text = text + " AND " + endingProperty + " = 2";
				break;
			}
			if (ExpController.Instance.OurTier == 0)
			{
				text = ((!Defs.isABTestBalansCohortActual) ? (text + " AND " + ABTestProperty + " = 0") : (text + " AND " + ABTestProperty + " = 1"));
			}
			if (Defs.isActivABTestBuffSystem || Defs.isActivABTestRatingSystem)
			{
				text = text + " AND " + ABTestEnum + " = ";
				if (abTestSeparate)
				{
					if (Defs.isActivABTestRatingSystem)
					{
						text += ((!FriendsController.isUseRatingSystem) ? 1 : 2);
					}
					else if (Defs.isActivABTestBuffSystem)
					{
						text += ((!FriendsController.useBuffSystem) ? 1 : 3);
					}
				}
				else if (Defs.isActivABTestRatingSystem)
				{
					text += (FriendsController.isUseRatingSystem ? 1 : 2);
				}
				else if (Defs.isActivABTestBuffSystem)
				{
					text += (FriendsController.useBuffSystem ? 1 : 3);
				}
			}
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[passwordProperty] = string.Empty;
		if (!Defs.useSqlLobby)
		{
			hashtable[mapProperty] = mapIndex;
		}
		if (!Defs.isDaterRegim && regim != RegimGame.DeadlyGames && regim != RegimGame.TimeBattle)
		{
			hashtable[maxKillProperty] = 3;
		}
		if (joinToNewRound == 0)
		{
			hashtable[endingProperty] = 0;
		}
		if (!Defs.isDaterRegim)
		{
			hashtable[platformProperty] = (int)myPlatformConnect;
		}
		if (ExpController.Instance.OurTier == 0)
		{
			if (Defs.isABTestBalansCohortActual)
			{
				hashtable[ABTestProperty] = 1;
			}
			else
			{
				hashtable[ABTestProperty] = 0;
			}
		}
		PlayerPrefs.SetString("TypeGame", "client");
		if (Defs.useSqlLobby)
		{
			TypedLobby typedLobby = new TypedLobby("PixelGun3D", LobbyType.SqlLobby);
			Debug.Log(text);
			PhotonNetwork.JoinRandomRoom(hashtable, 0, MatchmakingMode.FillRoom, typedLobby, text);
		}
		else
		{
			PhotonNetwork.JoinRandomRoom(hashtable, 0);
		}
		FlurryPluginWrapper.LogMultiplayerWayStart();
	}

	private void JoinRandomRoom(int mapIndex, RegimGame gameMode)
	{
		joinNewRoundTries = 0;
		tryJoinRoundMap = mapIndex;
		if (mapIndex != -1)
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(mapIndex);
			if (infoScene == null)
			{
				Debug.LogError("scInfo == null");
				return;
			}
			FlurryPluginWrapper.LogEnteringMap(0, infoScene.NameScene);
			goMapName = infoScene.NameScene;
		}
		else if (!Defs.useSqlLobby)
		{
			mapIndex = GetRandomMapIndex();
			if (mapIndex == -1)
			{
				return;
			}
			SceneInfo infoScene2 = SceneInfoController.instance.GetInfoScene(mapIndex);
			if (infoScene2 == null)
			{
				Debug.LogError("scInfo == null");
				return;
			}
			FlurryPluginWrapper.LogEnteringMap(0, infoScene2.NameScene);
			goMapName = infoScene2.NameScene;
		}
		else
		{
			goMapName = string.Empty;
		}
		if (!string.IsNullOrEmpty(goMapName))
		{
			if (WeaponManager.sharedManager != null)
			{
				WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
			}
			StartCoroutine(SetFonLoadingWaitForReset(goMapName));
			loadingMapPanel.SetActive(true);
			ActivityIndicator.IsActiveIndicator = true;
		}
		JoinRandomGameRoom(mapIndex, gameMode, joinNewRoundTries, abTestConnect);
	}

	private void HandleCreateRoomBtnClicked(object sender, EventArgs e)
	{
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(selectMap.mapID);
		if (infoScene == null)
		{
			return;
		}
		string text = infoScene.gameObject.name;
		if (infoScene.isPremium && Storager.getInt(text + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(text))
		{
			PhotonNetwork.Disconnect();
			return;
		}
		string text2 = FilterBadWorld.FilterString(nameServerInput.value);
		bool flag = false;
		if (Defs.isInet)
		{
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			for (int i = 0; i < roomList.Length; i++)
			{
				if (roomList[i].name.Equals(text2))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			nameAlreadyUsedLabel.timer = 3f;
			nameAlreadyUsedLabel.gameObject.SetActive(true);
			return;
		}
		goMapName = text;
		PlayerPrefs.SetString("MapName", goMapName);
		if (killToWin.value.Value > killToWin.maxValue.Value)
		{
			killToWin.value = killToWin.maxValue;
		}
		if (killToWin.value.Value < killToWin.minValue.Value)
		{
			killToWin.value = killToWin.minValue;
		}
		PlayerPrefs.SetString("MaxKill", killToWin.value.Value.ToString());
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(text) ? Defs.filterMaps[text] : 0);
		}
		StartCoroutine(SetFonLoadingWaitForReset(goMapName));
		loadingMapPanel.SetActive(true);
		int num = ((regim != 0 && regim != RegimGame.TimeBattle && regim != RegimGame.DeadlyGames) ? teamCountPlayer.value : numberOfPlayer.value.Value);
		int maxKill = (Defs.isDaterRegim ? killToWin.value.Value : ((regim == RegimGame.DeadlyGames) ? 10 : ((!Defs.isDaterRegim) ? 4 : 5)));
		if (Defs.isInet)
		{
			loadingMapPanel.SetActive(true);
			ActivityIndicator.IsActiveIndicator = true;
			CreateGameRoom(text2, num, infoScene.indexMap, maxKill, password, regim);
		}
		else
		{
			bool useNat = Network.HavePublicAddress();
			Network.InitializeServer(num - 1, 25002, useNat);
			PlayerPrefs.SetString("ServerName", text2);
			PlayerPrefs.SetString("PlayersLimits", num.ToString());
			Singleton<SceneLoader>.Instance.LoadSceneAsync("PromScene");
		}
	}

	private void ShowKillToWinPanel(bool show)
	{
		if (show)
		{
			numberOfPlayer.transform.localPosition = new Vector3(posNumberOffPlayersX, numberOfPlayer.transform.localPosition.y, numberOfPlayer.transform.localPosition.z);
			teamCountPlayer.transform.localPosition = new Vector3(posNumberOffPlayersX, teamCountPlayer.transform.localPosition.y, teamCountPlayer.transform.localPosition.z);
			killToWin.headLabel.text = LocalizationStore.Get("Key_0953");
			killToWin.gameObject.SetActive(true);
		}
		else
		{
			numberOfPlayer.transform.localPosition = new Vector3(0f, numberOfPlayer.transform.localPosition.y, numberOfPlayer.transform.localPosition.z);
			teamCountPlayer.transform.localPosition = new Vector3(0f, teamCountPlayer.transform.localPosition.y, teamCountPlayer.transform.localPosition.z);
			killToWin.gameObject.SetActive(false);
		}
	}

	private void HandleGoToCreateRoomBtnClicked(object sender, EventArgs e)
	{
		PlayerPrefs.SetString("TypeGame", "server");
		password = string.Empty;
		passONSprite.SetActive(false);
		SetPosSelectMapPanelInCreatePanel();
		createPanel.SetActive(true);
		coinsShopButton.SetActive(true);
		setPasswordBtn.SetActive(Defs.isInet);
		selectMapPanel.SetActive(true);
		customPanel.SetActive(false);
		nameAlreadyUsedLabel.timer = -1f;
		nameAlreadyUsedLabel.gameObject.SetActive(false);
		if (regim == RegimGame.Deathmatch)
		{
			teamCountPlayer.gameObject.SetActive(false);
			numberOfPlayer.gameObject.SetActive(false);
			numberOfPlayer.minValue.Value = 2;
			numberOfPlayer.maxValue.Value = 10;
			numberOfPlayer.value.Value = 10;
			if (Defs.isDaterRegim)
			{
				ShowKillToWinPanel(true);
				killToWin.minValue.Value = daterMinValue;
				killToWin.maxValue.Value = daterMaxValue;
				killToWin.value.Value = daterMinValue;
				killToWin.stepValue = daterStep;
			}
			else
			{
				ShowKillToWinPanel(false);
				if (ExperienceController.sharedController != null)
				{
					if (ExperienceController.sharedController.currentLevel <= 2)
					{
						killToWin.minValue.Value = 3;
						killToWin.maxValue.Value = 7;
						killToWin.value.Value = 3;
						killToWin.stepValue = 2;
					}
					else
					{
						killToWin.minValue.Value = 3;
						killToWin.maxValue.Value = 7;
						killToWin.value.Value = 3;
						killToWin.stepValue = 2;
					}
				}
			}
		}
		if (regim == RegimGame.TimeBattle)
		{
			teamCountPlayer.gameObject.SetActive(false);
			numberOfPlayer.gameObject.SetActive(false);
			numberOfPlayer.minValue.Value = 2;
			numberOfPlayer.maxValue.Value = 4;
			numberOfPlayer.value.Value = 4;
			ShowKillToWinPanel(false);
		}
		if (regim == RegimGame.TeamFight)
		{
			teamCountPlayer.gameObject.SetActive(false);
			teamCountPlayer.SetValue(10);
			numberOfPlayer.gameObject.SetActive(false);
			ShowKillToWinPanel(false);
			killToWin.stepValue = 2;
			if (ExperienceController.sharedController != null)
			{
				if (ExperienceController.sharedController.currentLevel <= 2)
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
				else
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
			}
		}
		if (regim == RegimGame.FlagCapture)
		{
			teamCountPlayer.gameObject.SetActive(false);
			teamCountPlayer.SetValue(10);
			numberOfPlayer.gameObject.SetActive(false);
			ShowKillToWinPanel(false);
			killToWin.stepValue = 2;
			if (ExperienceController.sharedController != null)
			{
				if (ExperienceController.sharedController.currentLevel <= 2)
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
				else
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
			}
		}
		if (regim == RegimGame.CapturePoints)
		{
			teamCountPlayer.gameObject.SetActive(false);
			teamCountPlayer.SetValue(10);
			numberOfPlayer.gameObject.SetActive(false);
			ShowKillToWinPanel(false);
			killToWin.stepValue = 2;
			if (ExperienceController.sharedController != null)
			{
				if (ExperienceController.sharedController.currentLevel <= 2)
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
				else
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
			}
		}
		if (regim != RegimGame.DeadlyGames)
		{
			return;
		}
		teamCountPlayer.gameObject.SetActive(false);
		numberOfPlayer.gameObject.SetActive(false);
		numberOfPlayer.minValue.Value = 3;
		numberOfPlayer.maxValue.Value = 8;
		numberOfPlayer.value.Value = 6;
		ShowKillToWinPanel(false);
		killToWin.stepValue = 5;
		if (ExperienceController.sharedController != null)
		{
			if (ExperienceController.sharedController.currentLevel <= 2)
			{
				killToWin.minValue.Value = 5;
				killToWin.maxValue.Value = 10;
				killToWin.value.Value = 10;
			}
			else
			{
				killToWin.minValue.Value = 5;
				killToWin.maxValue.Value = 10;
				killToWin.value.Value = 10;
			}
		}
	}

	private void HandleShowSearchPanelBtnClicked(object sender, EventArgs e)
	{
		customPanel.SetActive(false);
		if (searchInput != null)
		{
			searchInput.value = gameNameFilter;
		}
		searchPanel.SetActive(true);
	}

	private void HandleClearBtnClicked(object sender, EventArgs e)
	{
		if (searchInput != null)
		{
			searchInput.value = string.Empty;
		}
	}

	private void HandleSearchBtnClicked(object sender, EventArgs e)
	{
		customPanel.SetActive(true);
		if (searchInput != null)
		{
			gameNameFilter = searchInput.value;
		}
		updateFilteredRoomList(gameNameFilter);
		searchPanel.SetActive(false);
		wrapGames.SortAlphabetically();
		scrollGames.ResetPosition();
	}

	private void HandleCancelFromConnectToPhotonBtnClicked()
	{
		if (_someWindowSubscription != null)
		{
			_someWindowSubscription.Dispose();
		}
		failInternetLabel.SetActive(false);
		connectToPhotonPanel.SetActive(false);
		if (actAfterConnectToPhoton != null)
		{
			actAfterConnectToPhoton = null;
		}
		else
		{
			PhotonNetwork.Disconnect();
		}
	}

	private void HandleCancelFromConnectToPhotonBtnClicked(object sender, EventArgs e)
	{
		HandleCancelFromConnectToPhotonBtnClicked();
	}

	private void LogBuyMap(string context)
	{
		try
		{
			AnalyticsStuff.LogSales(context, "Premium Maps");
		}
		catch (Exception ex)
		{
			Debug.LogError("LogBuyMap exception: " + ex);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Premium Maps", context);
		Dictionary<string, string> dictionary2 = dictionary;
		if (ExperienceController.sharedController != null)
		{
			dictionary2.Add("Level", ExperienceController.sharedController.currentLevel.ToString());
		}
		if (ExpController.Instance != null)
		{
			dictionary2.Add("Tier", ExpController.Instance.OurTier.ToString());
		}
		FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchases Premium Maps " + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
	}

	private void HandleUnlockMapBtnClicked(object sender, EventArgs e)
	{
		SceneInfo scInfo = SceneInfoController.instance.GetInfoScene(selectMap.mapID);
		if (scInfo == null)
		{
			return;
		}
		int mapPrice = Defs.PremiumMaps[scInfo.NameScene];
		Action action = null;
		action = delegate
		{
			coinsShop.thisScript.notEnoughCurrency = null;
			coinsShop.thisScript.onReturnAction = null;
			int @int = Storager.getInt("Coins", false);
			int num = @int - mapPrice;
			string nameScene = scInfo.NameScene;
			if (num >= 0)
			{
				LogBuyMap(nameScene);
				AnalyticsFacade.InAppPurchase(nameScene, "Premium Maps", 1, mapPrice, "Coins");
				Storager.setInt(nameScene + "Key", 1, true);
				selectMap.mapPreviewTexture.mainTexture = mapPreview[nameScene];
				Storager.setInt("Coins", num, false);
				ShopNGUIController.SpendBoughtCurrency("Coins", mapPrice);
				if (Application.platform != RuntimePlatform.IPhonePlayer)
				{
					PlayerPrefs.Save();
				}
				ShopNGUIController.SynchronizeAndroidPurchases("Map unlocked from connect scene: " + nameScene);
				if (coinsPlashka.thisScript != null)
				{
					coinsPlashka.thisScript.enabled = false;
				}
			}
			else
			{
				StoreKitEventListener.State.PurchaseKey = "In map selection";
				FlurryPluginWrapper.LogEvent("Try_Enable " + nameScene + " map");
				if (BankController.Instance != null)
				{
					EventHandler handleBackFromBank = null;
					handleBackFromBank = delegate
					{
						BankController.Instance.BackRequested -= handleBackFromBank;
						mainPanel.transform.root.gameObject.SetActive(true);
						coinsShop.thisScript.notEnoughCurrency = null;
						BankController.Instance.InterfaceEnabled = false;
					};
					BankController.Instance.BackRequested += handleBackFromBank;
					mainPanel.transform.root.gameObject.SetActive(false);
					coinsShop.thisScript.notEnoughCurrency = "Coins";
					BankController.Instance.InterfaceEnabled = true;
				}
				else
				{
					Debug.LogWarning("BankController.Instance == null");
				}
			}
		};
		action();
	}

	public void ShowBankWindow()
	{
		if (BankController.Instance != null)
		{
			EventHandler backFromBankHandler = null;
			backFromBankHandler = delegate
			{
				BankController.Instance.BackRequested -= backFromBankHandler;
				mainPanel.transform.root.gameObject.SetActive(true);
				BankController.Instance.InterfaceEnabled = false;
			};
			BankController.Instance.BackRequested += backFromBankHandler;
			mainPanel.transform.root.gameObject.SetActive(false);
			BankController.Instance.InterfaceEnabled = true;
		}
		else
		{
			Debug.LogWarning("BankController.Instance == null");
		}
	}

	private void HandleCoinsShopClicked(object sender, EventArgs e)
	{
		ShowBankWindow();
	}

	private void HandleLocalBtnClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining(string.Concat(regim, ".Local"), TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		Defs.isInet = false;
		UpdateLocalServersList();
		CustomBtnAct();
		wrapGames.SortAlphabetically();
		scrollGames.enabled = true;
		scrollGames.ResetPosition();
	}

	private void ShowConnectToPhotonPanel()
	{
		_someWindowSubscription = BackSystem.Instance.Register(HandleCancelFromConnectToPhotonBtnClicked, "Connect to Photon panel");
		if (FriendsController.sharedController != null && FriendsController.sharedController.Banned == 1)
		{
			accountBlockedLabel.timer = 3f;
			accountBlockedLabel.gameObject.SetActive(true);
		}
		else
		{
			ConnectToPhoton();
			connectToPhotonPanel.SetActive(true);
		}
	}

	private void HandleCustomBtnClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining(string.Concat(regim, ".Custom"), TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		actAfterConnectToPhoton = CustomBtnAct;
		PhotonNetwork.autoJoinLobby = true;
		ShowConnectToPhotonPanel();
	}

	private void CustomBtnAct()
	{
		gameNameFilter = string.Empty;
		if (Defs.isInet)
		{
			Invoke("UpdateFilteredRoomListInvoke", 0.03f);
		}
		showSearchPanelBtn.SetActive(Defs.isInet);
		mainPanel.SetActive(false);
		coinsShopButton.SetActive(false);
		selectMapPanel.SetActive(false);
		customPanel.SetActive(true);
		password = string.Empty;
		incorrectPasswordLabel.timer = -1f;
		incorrectPasswordLabel.gameObject.SetActive(false);
		gameIsfullLabel.timer = -1f;
		gameIsfullLabel.gameObject.SetActive(false);
		wrapGames.SortAlphabetically();
		scrollGames.ResetPosition();
	}

	[Obfuscation(Exclude = true)]
	private void UpdateFilteredRoomListInvoke()
	{
		updateFilteredRoomList(gameNameFilter);
	}

	private void HandleRandomBtnClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining(string.Concat(regim, ".Random"), TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		actAfterConnectToPhoton = RandomBtnAct;
		PhotonNetwork.autoJoinLobby = false;
		ShowConnectToPhotonPanel();
	}

	private void RandomBtnAct()
	{
		JoinRandomRoom(-1, regim);
	}

	private int GetRandomMapIndex()
	{
		bool flag = true;
		AllScenesForMode listScenesForMode = SceneInfoController.instance.GetListScenesForMode(curSelectMode);
		if (listScenesForMode == null)
		{
			return -1;
		}
		int count = listScenesForMode.avaliableScenes.Count;
		int num = UnityEngine.Random.Range(0, count);
		int num2 = 0;
		SceneInfo sceneInfo;
		do
		{
			if (num2 > count)
			{
				return -1;
			}
			sceneInfo = listScenesForMode.avaliableScenes[num];
			if (!(sceneInfo == null))
			{
				num++;
				num2++;
				if (num >= count)
				{
					num = 0;
				}
				flag = sceneInfo.isPremium && Storager.getInt(sceneInfo.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(sceneInfo.NameScene);
			}
		}
		while (flag);
		return sceneInfo.indexMap;
	}

	public void HandleGoBtnClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining(string.Concat(regim, ".Go"), TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		actAfterConnectToPhoton = GoBtnAct;
		PhotonNetwork.autoJoinLobby = false;
		ShowConnectToPhotonPanel();
	}

	private void GoBtnAct()
	{
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(selectMap.mapID);
		if (!(infoScene == null))
		{
			bool isPremium = infoScene.isPremium;
			if (!isPremium || (isPremium && (Storager.getInt(infoScene.NameScene + "Key", true) == 1 || PremiumAccountController.MapAvailableDueToPremiumAccount(infoScene.NameScene))))
			{
				JoinRandomRoom(infoScene.indexMap, regim);
			}
			else
			{
				PhotonNetwork.Disconnect();
			}
		}
	}

	private void HandleBackBtnClicked(object sender, EventArgs e)
	{
		if (mainPanel != null && mainPanel.activeSelf)
		{
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.GetFriendsData();
			}
			FlurryPluginWrapper.LogEvent("Back to Main Menu");
			MenuBackgroundMusic.keepPlaying = true;
			LoadConnectScene.textureToShow = null;
			LoadConnectScene.sceneToLoad = Defs.MainMenuScene;
			LoadConnectScene.noteToShow = null;
			Application.LoadLevel(Defs.PromSceneName);
			isGoInPhotonGame = false;
		}
		if (customPanel != null && customPanel.activeSelf)
		{
			connectToWiFIInCreateLabel.SetActive(false);
			connectToWiFIInCustomLabel.SetActive(false);
			createRoomUIBtn.isEnabled = true;
			Defs.isInet = true;
			customPanel.SetActive(false);
			mainPanel.SetActive(true);
			coinsShopButton.SetActive(true);
			selectMapPanel.SetActive(true);
			PhotonNetwork.Disconnect();
		}
		if (searchPanel != null && searchPanel.activeSelf)
		{
			searchInput.value = gameNameFilter;
			searchPanel.SetActive(false);
			customPanel.SetActive(true);
		}
		if (createPanel != null && createPanel.activeSelf)
		{
			PlayerPrefs.SetString("TypeGame", "client");
			SetPosSelectMapPanelInMainMenu();
			createPanel.SetActive(false);
			coinsShopButton.SetActive(false);
			selectMapPanel.SetActive(false);
			customPanel.SetActive(true);
		}
		if (setPasswordPanel != null && setPasswordPanel.activeSelf)
		{
			BackFromSetPasswordPanel();
		}
		if (enterPasswordPanel != null && enterPasswordPanel.activeSelf)
		{
			enterPasswordPanel.SetActive(false);
			customPanel.SetActive(true);
			ExperienceController.sharedController.isShowRanks = true;
		}
	}

	private void HandleUnlockBtnClicked(object sender, EventArgs e)
	{
		int _price = 0;
		string _storagerPurchasedKey = string.Empty;
		if (regim == RegimGame.FlagCapture)
		{
			_price = Defs.CaptureFlagPrice;
			_storagerPurchasedKey = Defs.CaptureFlagPurchasedKey;
		}
		if (regim == RegimGame.DeadlyGames)
		{
			_price = Defs.HungerGamesPrice;
			_storagerPurchasedKey = Defs.hungerGamesPurchasedKey;
		}
		Action action = null;
		action = delegate
		{
			coinsShop.thisScript.notEnoughCurrency = null;
			coinsShop.thisScript.onReturnAction = null;
			int @int = Storager.getInt("Coins", false);
			int num = @int - _price;
			if (num >= 0)
			{
				FlurryPluginWrapper.LogEvent("Enable_Flags");
				Storager.setInt(_storagerPurchasedKey, 1, true);
				Storager.setInt("Coins", num, false);
				ShopNGUIController.SpendBoughtCurrency("Coins", _price);
				if (Application.platform != RuntimePlatform.IPhonePlayer)
				{
					PlayerPrefs.Save();
				}
				ShopNGUIController.SynchronizeAndroidPurchases("Mode enabled");
				if (coinsPlashka.thisScript != null)
				{
					coinsPlashka.thisScript.enabled = false;
				}
				if (regim == RegimGame.FlagCapture)
				{
					SetUnLockedButton(flagCaptureToogle);
				}
				if (regim == RegimGame.DeadlyGames)
				{
					SetUnLockedButton(deadlyGamesToogle);
				}
				unlockBtn.SetActive(false);
				customBtn.SetActive(true);
				randomBtn.SetActive(true);
				conditionLabel.gameObject.SetActive(false);
				goBtn.SetActive(true);
			}
			else
			{
				FlurryPluginWrapper.LogEvent("Try_Enable_CaptureFlag");
				StoreKitEventListener.State.PurchaseKey = "Mode opened";
				if (BankController.Instance != null)
				{
					EventHandler handleBackFromBank = null;
					handleBackFromBank = delegate
					{
						BankController.Instance.BackRequested -= handleBackFromBank;
						mainPanel.transform.root.gameObject.SetActive(true);
						coinsShop.thisScript.notEnoughCurrency = null;
						BankController.Instance.InterfaceEnabled = false;
					};
					BankController.Instance.BackRequested += handleBackFromBank;
					mainPanel.transform.root.gameObject.SetActive(false);
					coinsShop.thisScript.notEnoughCurrency = "Coins";
					BankController.Instance.InterfaceEnabled = true;
				}
				else
				{
					Debug.LogWarning("BankController.Instance == null");
				}
			}
		};
		action();
	}

	private void SetRegim(RegimGame _regim)
	{
		bool flag = true;
		bool flag2 = true;
		PlayerPrefs.SetInt("RegimMulty", (int)_regim);
		regim = _regim;
		if (!Defs.isDaterRegim)
		{
			deathmatchToggle.GetComponent<UIButton>().pressedSprite = ((regim != 0) ? "yell_btn_n" : "green_btn_n");
			timeBattleToogle.GetComponent<UIButton>().pressedSprite = ((regim != RegimGame.TimeBattle) ? "yell_btn_n" : "green_btn_n");
			teamFightToogle.GetComponent<UIButton>().pressedSprite = ((regim != RegimGame.TeamFight) ? "yell_btn_n" : "green_btn_n");
			if (flag)
			{
				deadlyGamesToogle.GetComponent<UIButton>().pressedSprite = ((regim != RegimGame.DeadlyGames) ? "yell_btn_n" : "green_btn_n");
			}
			if (flag2)
			{
				flagCaptureToogle.GetComponent<UIButton>().pressedSprite = ((regim != RegimGame.FlagCapture) ? "yell_btn_n" : "green_btn_n");
			}
			unlockMapBtn.SetActive(false);
			unlockMapBtnInCreate.SetActive(false);
		}
		createRoomBtn.SetActive(true);
		if (regim == RegimGame.Deathmatch)
		{
			Defs.isMulti = true;
			Defs.isInet = true;
			Defs.isCOOP = false;
			Defs.isCompany = false;
			Defs.isHunger = false;
			Defs.isFlag = false;
			Defs.isCapturePoints = false;
			Defs.IsSurvival = false;
			StoreKitEventListener.State.Mode = "Deathmatch Wordwide";
			StoreKitEventListener.State.Parameters.Clear();
			if (unlockBtn != null)
			{
				unlockBtn.SetActive(false);
			}
			customBtn.SetActive(true);
			if (randomBtn != null)
			{
				randomBtn.SetActive(true);
			}
			if (conditionLabel != null)
			{
				conditionLabel.gameObject.SetActive(false);
			}
			goBtn.SetActive(true);
			localBtn.SetActive(BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64);
			if (Defs.isDaterRegim)
			{
				rulesLabel.text = rulesDater;
			}
			else
			{
				rulesLabel.text = rulesDeadmatch;
			}
		}
		if (regim == RegimGame.TimeBattle)
		{
			Defs.isMulti = true;
			Defs.isInet = true;
			Defs.isCOOP = true;
			Defs.isCompany = false;
			Defs.isHunger = false;
			Defs.isFlag = false;
			Defs.isCapturePoints = false;
			if (unlockBtn != null)
			{
				unlockBtn.SetActive(false);
			}
			customBtn.SetActive(true);
			randomBtn.SetActive(true);
			conditionLabel.gameObject.SetActive(false);
			goBtn.SetActive(true);
			StoreKitEventListener.State.Mode = "Time Survival";
			StoreKitEventListener.State.Parameters.Clear();
			localBtn.SetActive(false);
			rulesLabel.text = rulesTimeBattle;
		}
		if (regim == RegimGame.TeamFight)
		{
			Defs.isMulti = true;
			Defs.isInet = true;
			Defs.isCOOP = false;
			Defs.isCompany = true;
			Defs.isHunger = false;
			Defs.isFlag = false;
			Defs.isCapturePoints = false;
			if (unlockBtn != null)
			{
				unlockBtn.SetActive(false);
			}
			customBtn.SetActive(true);
			randomBtn.SetActive(true);
			conditionLabel.gameObject.SetActive(false);
			goBtn.SetActive(true);
			localBtn.SetActive(BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64);
			StoreKitEventListener.State.Mode = "Team Battle";
			StoreKitEventListener.State.Parameters.Clear();
			rulesLabel.text = rulesTeamFight;
		}
		if (regim == RegimGame.FlagCapture)
		{
			Defs.isMulti = true;
			Defs.isInet = true;
			Defs.isCOOP = false;
			Defs.isCompany = false;
			Defs.isHunger = false;
			Defs.isFlag = true;
			Defs.isCapturePoints = false;
			localBtn.SetActive(false);
			rulesLabel.text = rulesFlagCapture;
			if (!flag2)
			{
				priceRegimLabel.text = Defs.CaptureFlagPrice.ToString();
				if (unlockBtn != null)
				{
					unlockBtn.SetActive(true);
				}
				customBtn.SetActive(false);
				randomBtn.SetActive(false);
				conditionLabel.gameObject.SetActive(true);
				conditionLabel.text = "REACH LEVEL 4 TO OPEN";
				goBtn.SetActive(false);
			}
			else
			{
				if (unlockBtn != null)
				{
					unlockBtn.SetActive(false);
				}
				customBtn.SetActive(true);
				randomBtn.SetActive(true);
				conditionLabel.gameObject.SetActive(false);
				goBtn.SetActive(true);
			}
			StoreKitEventListener.State.Mode = "Flag Capture";
			StoreKitEventListener.State.Parameters.Clear();
		}
		if (regim == RegimGame.CapturePoints)
		{
			Defs.isMulti = true;
			Defs.isInet = true;
			Defs.isCOOP = false;
			Defs.isCompany = false;
			Defs.isHunger = false;
			Defs.isCapturePoints = true;
			Defs.isFlag = false;
			localBtn.SetActive(false);
			rulesLabel.text = rulesCapturePoint;
			if (unlockBtn != null)
			{
				unlockBtn.SetActive(false);
			}
			customBtn.SetActive(true);
			randomBtn.SetActive(true);
			conditionLabel.gameObject.SetActive(false);
			goBtn.SetActive(true);
			StoreKitEventListener.State.Mode = "Capture points";
			StoreKitEventListener.State.Parameters.Clear();
		}
		if (regim == RegimGame.DeadlyGames)
		{
			Defs.isMulti = true;
			Defs.isInet = true;
			Defs.isCOOP = false;
			Defs.isCompany = false;
			Defs.isHunger = true;
			Defs.isFlag = false;
			Defs.isCapturePoints = false;
			localBtn.SetActive(false);
			rulesLabel.text = rulesDeadlyGames;
			if (!flag)
			{
				priceRegimLabel.text = Defs.HungerGamesPrice.ToString();
				if (unlockBtn != null)
				{
					unlockBtn.SetActive(true);
				}
				customBtn.SetActive(false);
				randomBtn.SetActive(false);
				conditionLabel.gameObject.SetActive(true);
				conditionLabel.text = "REACH LEVEL 3 TO OPEN";
				goBtn.SetActive(false);
			}
			else
			{
				if (unlockBtn != null)
				{
					unlockBtn.SetActive(false);
				}
				customBtn.SetActive(true);
				randomBtn.SetActive(true);
				conditionLabel.gameObject.SetActive(false);
				goBtn.SetActive(true);
			}
			Defs.IsSurvival = false;
			StoreKitEventListener.State.Mode = "Deadly Games";
			StoreKitEventListener.State.Parameters.Clear();
			if (WeaponManager.sharedManager != null)
			{
				WeaponManager.sharedManager.GetWeaponPrefabs();
			}
		}
		StartCoroutine(SetUseMasMap());
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(delegate
		{
			if (BannerWindowController.SharedController != null && BannerWindowController.SharedController.IsAnyBannerShown)
			{
				BannerWindowController.SharedController.HideBannerWindow();
			}
			else
			{
				HandleBackBtnClicked(null, EventArgs.Empty);
			}
		}, "Connect Scene");
		OnEnableWhenAnimate();
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void Update()
	{
		if (customPanel.activeSelf && !Defs.isInet)
		{
			UpdateLocalServersList();
		}
		if (!Defs.isInet)
		{
			connectToWiFIInCreateLabel.SetActive(!CheckLocalAvailability());
			connectToWiFIInCustomLabel.SetActive(!CheckLocalAvailability());
			if (createRoomUIBtn.isEnabled != CheckLocalAvailability())
			{
				createRoomUIBtn.isEnabled = CheckLocalAvailability();
			}
		}
		else
		{
			if (connectToWiFIInCreateLabel.activeSelf)
			{
				connectToWiFIInCreateLabel.SetActive(false);
			}
			if (connectToWiFIInCreateLabel.activeSelf)
			{
				connectToWiFIInCustomLabel.SetActive(false);
			}
		}
		if (selectMapPanel.activeInHierarchy && centerScript != null && centerScript.centeredObject != null)
		{
			selectMap = centerScript.centeredObject.GetComponent<MapPreviewController>();
		}
		if (!unlockBtn.activeSelf && (mainPanel.activeSelf || createPanel.activeSelf) && selectMap != null)
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(selectMap.mapID);
			if (infoScene == null)
			{
				return;
			}
			if (!isSetUseMap && infoScene.isPremium && Storager.getInt(infoScene.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(infoScene.NameScene))
			{
				if (!unlockMapBtn.activeSelf)
				{
					priceMapLabel.text = Defs.PremiumMaps[infoScene.NameScene].ToString();
					unlockMapBtn.SetActive(true);
					goBtn.SetActive(false);
					priceMapLabelInCreate.text = Defs.PremiumMaps[infoScene.NameScene].ToString();
					unlockMapBtnInCreate.SetActive(true);
					createRoomBtn.SetActive(false);
				}
			}
			else if (unlockMapBtn.activeSelf)
			{
				unlockMapBtn.SetActive(false);
				goBtn.SetActive(true);
				unlockMapBtnInCreate.SetActive(false);
				createRoomBtn.SetActive(true);
			}
		}
		if ((!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && (!(BannerWindowController.SharedController != null) || !BannerWindowController.SharedController.IsAnyBannerShown) && (!(loadingToDraw != null) || !loadingToDraw.gameObject.activeInHierarchy) && (!(_loadingNGUIController != null) || !_loadingNGUIController.gameObject.activeInHierarchy) && ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = true;
		}
	}

	private bool IsUseMap(int indMap)
	{
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(curSelectMode, indMap);
		if (infoScene != null)
		{
			bool flag = infoScene.isPremium && Storager.getInt(infoScene.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(infoScene.NameScene);
			return !flag;
		}
		return false;
	}

	private IEnumerator LoadMapPreview()
	{
		List<SceneInfo> listAllNeedMap = new List<SceneInfo>();
		if (Defs.isDaterRegim)
		{
			AllScenesForMode scMode2 = SceneInfoController.instance.GetListScenesForMode(TypeModeGame.Dater);
			if (scMode2 != null)
			{
				listAllNeedMap.AddRange(scMode2.avaliableScenes);
			}
		}
		else
		{
			TypeModeGame[] arrNeedMode = new TypeModeGame[6]
			{
				TypeModeGame.Deathmatch,
				TypeModeGame.TeamFight,
				TypeModeGame.TimeBattle,
				TypeModeGame.FlagCapture,
				TypeModeGame.DeadlyGames,
				TypeModeGame.CapturePoints
			};
			TypeModeGame[] array = arrNeedMode;
			foreach (TypeModeGame curMode in array)
			{
				AllScenesForMode scMode = SceneInfoController.instance.GetListScenesForMode(curMode);
				if (scMode != null)
				{
					listAllNeedMap.AddRange(scMode.avaliableScenes);
				}
			}
		}
		string allScene = string.Empty;
		for (int scI = 0; scI < listAllNeedMap.Count; scI++)
		{
			if (!mapPreview.ContainsKey(listAllNeedMap[scI].NameScene))
			{
				allScene = allScene + listAllNeedMap[scI].NameScene + "\n";
				mapPreview.Add(listAllNeedMap[scI].NameScene, Resources.Load("LevelLoadingsPreview" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + listAllNeedMap[scI].NameScene) as Texture);
				if (listAllNeedMap[scI].isPremium && Storager.getInt(listAllNeedMap[scI].NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(listAllNeedMap[scI].NameScene))
				{
					mapPreview.Add(listAllNeedMap[scI].NameScene + "_off", Resources.Load<Texture>("LevelLoadingsPreview" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + listAllNeedMap[scI].NameScene + "_off"));
				}
				yield return null;
			}
		}
		if (Application.isEditor)
		{
			Debug.Log(allScene);
		}
		yield return null;
		mainPanel.SetActive(true);
		coinsShopButton.SetActive(true);
		selectMapPanel.SetActive(true);
		SetRegim(regim);
		yield return null;
		if (PromoActionsManager.MobileAdvert == null)
		{
			Debug.LogWarning("PromoActionsManager.MobileAdvert == null");
		}
		else if (MobileAdManager.AdIsApplicable(MobileAdManager.Type.Image, true))
		{
			Debug.LogFormat("{0}, LoadMapPreview(), InterstitialRequest: {1}", GetType().Name, InterstitialRequest);
			if (InterstitialRequest)
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Waiting for {0:F2}s...", PromoActionsManager.MobileAdvert.ConnectSceneDelaySeconds);
				}
				float startWaitingTime = Time.realtimeSinceStartup;
				while ((double)(Time.realtimeSinceStartup - startWaitingTime) < PromoActionsManager.MobileAdvert.ConnectSceneDelaySeconds)
				{
					yield return null;
				}
			}
		}
		InterstitialRequest = false;
		yield return null;
		loadingMapPanel.SetActive(false);
		ActivityIndicator.IsActiveIndicator = false;
		if (!Defs.isDaterRegim)
		{
			StartCoroutine(AnimateModeOpen());
		}
		if (NeedShowReviewInConnectScene)
		{
			BannerWindowController.firstScreen = true;
		}
		yield return new WaitForSeconds(1f);
		if (NeedShowReviewInConnectScene)
		{
			NeedShowReviewInConnectScene = false;
			ReviewHUDWindow.Instance.ShowWindowRating();
		}
	}

	public static void UpdateUseMasMaps()
	{
		if (Defs.isDaterRegim)
		{
			curSelectMode = TypeModeGame.Dater;
		}
		else if (regim == RegimGame.TimeBattle)
		{
			curSelectMode = TypeModeGame.TimeBattle;
		}
		else if (regim == RegimGame.TeamFight)
		{
			curSelectMode = TypeModeGame.TeamFight;
		}
		else if (regim == RegimGame.DeadlyGames)
		{
			curSelectMode = TypeModeGame.DeadlyGames;
		}
		else if (regim == RegimGame.FlagCapture)
		{
			curSelectMode = TypeModeGame.FlagCapture;
		}
		else if (regim == RegimGame.CapturePoints)
		{
			curSelectMode = TypeModeGame.CapturePoints;
		}
		else
		{
			curSelectMode = TypeModeGame.Deathmatch;
		}
	}

	private IEnumerator SetUseMasMap()
	{
		isSetUseMap = true;
		SpringPanel _spr = ScrollTransform.GetComponent<SpringPanel>();
		if (_spr != null)
		{
			UnityEngine.Object.Destroy(_spr);
		}
		ScrollTransform.GetComponent<UIPanel>().clipOffset = new Vector2(0f, 0f);
		SetPosSelectMapPanelInMainMenu();
		ScrollTransform.localPosition = new Vector3(0f, 0f, 0f);
		for (int j = 0; j < grid.transform.childCount; j++)
		{
			UnityEngine.Object.Destroy(grid.transform.GetChild(j).gameObject);
		}
		yield return null;
		AllScenesForMode modeInfo = SceneInfoController.instance.GetListScenesForMode(curSelectMode);
		if (modeInfo == null)
		{
			Debug.LogError("modeInfo == null");
			yield break;
		}
		for (int k = 0; k < modeInfo.avaliableScenes.Count; k++)
		{
			SceneInfo scInfo = modeInfo.avaliableScenes[k];
			GameObject newTexture = UnityEngine.Object.Instantiate(mapPreviewTexture);
			newTexture.transform.parent = grid.transform;
			bool _isClose = scInfo.isPremium && Storager.getInt(scInfo.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(scInfo.NameScene);
			newTexture.GetComponent<MapPreviewController>().mapPreviewTexture.mainTexture = mapPreview[scInfo.NameScene + ((!_isClose) ? string.Empty : "_off")];
			newTexture.name = "Map_" + k;
			newTexture.transform.localScale = new Vector3(1f, 1f, 1f);
			newTexture.transform.localPosition = new Vector3(widthCell * (float)k, 0f, 0f);
			if (!(scInfo == null))
			{
				newTexture.GetComponent<MapPreviewController>().NameMapLbl.GetComponent<SetHeadLabelText>().SetText(scInfo.TranslatePreviewName.ToUpper());
				newTexture.GetComponent<MapPreviewController>().SizeMapNameLbl.text = scInfo.TranslateSizeMap;
				newTexture.GetComponent<MapPreviewController>().mapID = scInfo.indexMap;
				if (scInfo.isPremium)
				{
					newTexture.GetComponent<MapPreviewController>().premium.SetActive(true);
				}
				if (scInfo.AvaliableWeapon == ModeWeapon.knifes)
				{
					newTexture.GetComponent<MapPreviewController>().milee.SetActive(true);
					newTexture.GetComponent<MapPreviewController>().milee.GetComponent<UILabel>().text = LocalizationStore.Get("Key_0096");
				}
				if (scInfo.AvaliableWeapon == ModeWeapon.sniper)
				{
					newTexture.GetComponent<MapPreviewController>().milee.SetActive(true);
					newTexture.GetComponent<MapPreviewController>().milee.GetComponent<UILabel>().text = LocalizationStore.Get("Key_0949");
				}
				if (Defs.isDaterRegim)
				{
					newTexture.GetComponent<MapPreviewController>().milee.SetActive(true);
					newTexture.GetComponent<MapPreviewController>().milee.GetComponent<UILabel>().text = LocalizationStore.Get("Key_1421");
				}
			}
		}
		grid.GetComponent<UIWrapContent>().SortBasedOnScrollMovement();
		Transform curr = grid.transform.GetChild(0);
		foreach (Transform c in grid.transform)
		{
			if (c.gameObject.name.Equals("Map_0"))
			{
				curr = c;
				break;
			}
		}
		grid.GetComponent<UIWrapContent>().WrapContent();
		grid.SetActive(false);
		grid.SetActive(true);
		centerScript = grid.GetComponent<MyCenterOnChild>();
		if (!TrainingController.TrainingCompleted)
		{
			selectedMap = modeInfo.avaliableScenes[UnityEngine.Random.Range(0, modeInfo.avaliableScenes.Count)].NameScene;
		}
		if (!string.IsNullOrEmpty(selectedMap))
		{
			for (int i = 0; i < modeInfo.avaliableScenes.Count; i++)
			{
				if (selectedMap == modeInfo.avaliableScenes[i].NameScene)
				{
					centerScript.springStrength = 1E+11f;
					centerScript.CenterOn(grid.transform.GetChild(i));
					centerScript.springStrength = 8f;
					break;
				}
			}
			selectedMap = string.Empty;
		}
		else
		{
			centerScript.springStrength = 1E+11f;
			centerScript.CenterOn(curr);
			centerScript.springStrength = 8f;
		}
		grid.transform.GetChild(1).transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		grid.transform.GetChild(grid.transform.childCount - 1).transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		startPosX = curr.localPosition.x;
		yield return null;
		isSetUseMap = false;
		Resources.UnloadUnusedAssets();
	}

	public void OnReceivedRoomListUpdate()
	{
		if (customPanel.activeSelf && Defs.isInet)
		{
			updateFilteredRoomList(gameNameFilter);
		}
	}

	private void SetRoomInfo(GameInfo _gameInfo, int index)
	{
		_gameInfo.index = index;
		if (filteredRoomList.Count > index)
		{
			_gameInfo.gameObject.SetActive(true);
			RoomInfo roomInfo = filteredRoomList[index];
			string text = roomInfo.name;
			if (text.Length == 36 && text.IndexOf("-") == 8 && text.LastIndexOf("-") == 23)
			{
				text = LocalizationStore.Get("Key_0088");
			}
			_gameInfo.serverNameLabel.text = text;
			_gameInfo.countPlayersLabel.text = roomInfo.playerCount + "/" + roomInfo.maxPlayers;
			bool flag = string.IsNullOrEmpty(roomInfo.customProperties[passwordProperty].ToString());
			_gameInfo.openSprite.SetActive(flag);
			_gameInfo.closeSprite.SetActive(!flag);
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(roomInfo.customProperties[mapProperty].ToString()));
			string text2 = string.Format("{0}: {1}", LocalizationStore.Get("Key_0947"), infoScene.TranslateName);
			_gameInfo.mapNameLabel.text = text2;
			_gameInfo.roomInfo = roomInfo;
		}
		else
		{
			_gameInfo.gameObject.SetActive(false);
		}
	}

	private void OnInitializeItem(GameObject go, int wrapInd, int realInd)
	{
		if (!Defs.isInet)
		{
			SetLocalRoomInfo(go.GetComponent<GameInfo>(), Mathf.Abs(realInd));
		}
		else
		{
			SetRoomInfo(go.GetComponent<GameInfo>(), Mathf.Abs(realInd));
		}
	}

	public void updateFilteredRoomList(string gFilter)
	{
		filteredRoomList.Clear();
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		bool flag = !string.IsNullOrEmpty(gFilter);
		for (int i = 0; i < roomList.Length; i++)
		{
			if (!Defs.isDaterRegim && roomList[i].customProperties[platformProperty] != null)
			{
				string text = roomList[i].customProperties[platformProperty].ToString();
				int num = (int)myPlatformConnect;
				if (!text.Equals(num.ToString()) && !roomList[i].customProperties[platformProperty].ToString().Equals(3.ToString()))
				{
					continue;
				}
			}
			if ((ExpController.Instance.OurTier != 0 || !Defs.isABTestBalansCohortActual || (roomList[i].customProperties[ABTestProperty] != null && (int)roomList[i].customProperties[ABTestProperty] == 1)) && (ExpController.Instance.OurTier != 0 || Defs.isABTestBalansCohortActual || roomList[i].customProperties[ABTestProperty] == null || (int)roomList[i].customProperties[ABTestProperty] != 1))
			{
				bool flag2 = true;
				if (flag)
				{
					flag2 = roomList[i].name.StartsWith(gFilter, true, null) && (roomList[i].name.Length != 36 || roomList[i].name.IndexOf("-") != 8 || roomList[i].name.LastIndexOf("-") != 23);
				}
				if (flag2 && IsUseMap((int)roomList[i].customProperties[mapProperty]))
				{
					filteredRoomList.Add(roomList[i]);
				}
			}
		}
		if (filteredRoomList.Count < 4)
		{
			if (scrollGames.enabled)
			{
				wrapGames.SortAlphabetically();
				scrollGames.ResetPosition();
				scrollGames.enabled = false;
			}
		}
		else
		{
			scrollGames.enabled = true;
		}
		wrapGames.minIndex = filteredRoomList.Count * -1;
		if (filteredRoomList.Count > 0 && roomFields == null)
		{
			roomFields = new GameInfo[5];
			for (int j = 0; j < roomFields.Length; j++)
			{
				GameObject gameObject = NGUITools.AddChild(wrapGames.gameObject, gameInfoItemPrefab);
				gameObject.name = "GameInfo_" + j;
				roomFields[j] = gameObject.GetComponent<GameInfo>();
			}
			wrapGames.SortAlphabetically();
			scrollGames.enabled = true;
			scrollGames.ResetPosition();
		}
		if (roomFields != null)
		{
			for (int k = 0; k < roomFields.Length; k++)
			{
				SetRoomInfo(roomFields[k], roomFields[k].index);
			}
		}
	}

	private void OnPhotonRandomJoinFailed()
	{
		Debug.Log("OnPhotonJoinRoomFailed");
		if (string.IsNullOrEmpty(goMapName))
		{
			int randomMapIndex = GetRandomMapIndex();
			if (randomMapIndex == -1)
			{
				return;
			}
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(randomMapIndex);
			if (infoScene == null)
			{
				return;
			}
			goMapName = infoScene.name;
		}
		SceneInfo infoScene2 = SceneInfoController.instance.GetInfoScene(goMapName);
		if (infoScene2 == null)
		{
			return;
		}
		if (joinNewRoundTries >= 2 && abTestConnect)
		{
			abTestConnect = false;
			joinNewRoundTries = 0;
		}
		if (joinNewRoundTries < 2)
		{
			Debug.Log("No rooms with new round: " + joinNewRoundTries + ((!abTestConnect) ? string.Empty : " <color=yellow>AbTestSeparate</color>"));
			joinNewRoundTries++;
			JoinRandomGameRoom(tryJoinRoundMap, regim, joinNewRoundTries, abTestConnect);
			return;
		}
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
		}
		StartCoroutine(SetFonLoadingWaitForReset(goMapName));
		int maxKill = ((regim == RegimGame.DeadlyGames) ? 10 : ((!Defs.isDaterRegim) ? 4 : 5));
		int playerLimit = (Defs.isCOOP ? 4 : (Defs.isCompany ? 10 : ((!Defs.isHunger) ? 10 : 6)));
		CreateGameRoom(null, playerLimit, infoScene2.indexMap, maxKill, string.Empty, regim);
	}

	private void OnPhotonJoinRoomFailed()
	{
		ActivityIndicator.IsActiveIndicator = false;
		loadingMapPanel.SetActive(false);
		gameIsfullLabel.timer = 3f;
		gameIsfullLabel.gameObject.SetActive(true);
		incorrectPasswordLabel.timer = -1f;
		incorrectPasswordLabel.gameObject.SetActive(false);
		Debug.Log("OnPhotonJoinRoomFailed");
	}

	private void OnJoinedRoom()
	{
		AnalyticsStuff.LogMultiplayer();
		Debug.Log("OnJoinedRoom " + PhotonNetwork.room.customProperties[mapProperty].ToString());
		PhotonNetwork.isMessageQueueRunning = false;
		NotificationController.ResetPaused();
		GlobalGameController.healthMyPlayer = 0f;
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(PhotonNetwork.room.customProperties[mapProperty].ToString()));
		goMapName = infoScene.NameScene;
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
		}
		StartCoroutine(MoveToGameScene(infoScene.NameScene));
	}

	private void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
	}

	private void OnPhotonCreateRoomFailed()
	{
		Debug.Log("OnPhotonCreateRoomFailed");
		nameAlreadyUsedLabel.timer = 3f;
		nameAlreadyUsedLabel.gameObject.SetActive(true);
		loadingMapPanel.SetActive(false);
		ActivityIndicator.IsActiveIndicator = false;
	}

	private void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");
		if ((!mainPanel.activeSelf || loadingMapPanel.activeSelf) && firstConnectToPhoton && Defs.isInet)
		{
			mainPanel.SetActive(true);
			coinsShopButton.SetActive(true);
			selectMapPanel.SetActive(true);
			createPanel.SetActive(false);
			customPanel.SetActive(false);
			searchPanel.SetActive(false);
			setPasswordPanel.SetActive(false);
			enterPasswordPanel.SetActive(false);
			ExperienceController.sharedController.isShowRanks = true;
			loadingMapPanel.SetActive(false);
			SetPosSelectMapPanelInMainMenu();
			serverIsNotAvalible.timer = 3f;
			serverIsNotAvalible.gameObject.SetActive(true);
			UICamera.selectedObject = null;
		}
		if (actAfterConnectToPhoton != null)
		{
			Invoke("ConnectToPhoton", 0.5f);
		}
	}

	private void OnFailedToConnectToPhoton(object parameters)
	{
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
		if (connectToPhotonPanel.activeSelf)
		{
			failInternetLabel.SetActive(true);
		}
		if (!isCancelConnectingToPhoton)
		{
			Invoke("ConnectToPhoton", 1f);
		}
	}

	public void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster");
		firstConnectToPhoton = true;
		PhotonNetwork.playerName = ProfileController.GetPlayerNameOrDefault();
		if (connectToPhotonPanel.activeSelf && actAfterConnectToPhoton != new Action(RandomBtnAct))
		{
			connectToPhotonPanel.SetActive(false);
		}
		if (actAfterConnectToPhoton != null)
		{
			actAfterConnectToPhoton();
			actAfterConnectToPhoton = null;
		}
		else
		{
			PhotonNetwork.Disconnect();
		}
	}

	public void OnConnectedToPhoton()
	{
		Debug.Log("OnConnectedToPhoton");
	}

	public void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby: " + PhotonNetwork.lobby.Name);
		OnConnectedToMaster();
	}

	private IEnumerator SetFonLoadingWaitForReset(string _mapName = "", bool isAddCountRun = false)
	{
		GetMapName(_mapName, isAddCountRun);
		if (_loadingNGUIController != null)
		{
			UnityEngine.Object.Destroy(_loadingNGUIController.gameObject);
			_loadingNGUIController = null;
		}
		while (WeaponManager.sharedManager == null)
		{
			yield return null;
		}
		while (WeaponManager.sharedManager.LockGetWeaponPrefabs > 0)
		{
			yield return null;
		}
		ShowLoadingGUI(_mapName);
	}

	private void SetFonLoading(string _mapName = "", bool isAddCountRun = false)
	{
		GetMapName(_mapName, isAddCountRun);
		if (_loadingNGUIController != null)
		{
			UnityEngine.Object.Destroy(_loadingNGUIController.gameObject);
			_loadingNGUIController = null;
		}
		ShowLoadingGUI(_mapName);
	}

	private void ShowLoadingGUI(string _mapName)
	{
		BannerWindowController.SharedController.HideBannerWindowNoShowNext();
		_loadingNGUIController = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("LoadingGUI")).GetComponent<LoadingNGUIController>();
		_loadingNGUIController.SceneToLoad = _mapName;
		_loadingNGUIController.loadingNGUITexture.mainTexture = LoadConnectScene.textureToShow;
		_loadingNGUIController.transform.parent = loadingMapPanel.transform;
		_loadingNGUIController.transform.localPosition = Vector3.zero;
		_loadingNGUIController.Init();
	}

	private void GetMapName(string _mapName, bool isAddCountRun)
	{
		Debug.Log("setFonLoading " + _mapName);
		Texture texture = null;
		if (Defs.isCOOP)
		{
			int @int = PlayerPrefs.GetInt("CountRunCoop", 0);
			bool flag = @int < 5;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunCoop", PlayerPrefs.GetInt("CountRunCoop", 0) + 1);
			}
			Texture texture2 = Resources.Load("NoteLoadings/note_Time_Survival_" + @int % countNoteCaptureCOOP) as Texture;
		}
		else if (Defs.isCompany)
		{
			int int2 = PlayerPrefs.GetInt("CountRunCompany", 0);
			bool flag = int2 < 5;
			Texture texture2 = Resources.Load("NoteLoadings/note_Team_Battle_" + int2 % countNoteCaptureCompany) as Texture;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunCompany", PlayerPrefs.GetInt("CountRunCompany", 0) + 1);
			}
		}
		else if (Defs.isHunger)
		{
			int int3 = PlayerPrefs.GetInt("CountRunHunger", 0);
			bool flag = int3 < 5;
			Texture texture2 = Resources.Load("NoteLoadings/note_Deadly_Games_" + int3 % countNoteCaptureHunger) as Texture;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunHunger", PlayerPrefs.GetInt("CountRunHunger", 0) + 1);
			}
		}
		else if (Defs.isFlag)
		{
			int int4 = PlayerPrefs.GetInt("CountRunFlag", 0);
			bool flag = int4 < 5;
			Texture texture2 = Resources.Load("NoteLoadings/note_Flag_Capture_" + int4 % countNoteCaptureFlag) as Texture;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunFlag", PlayerPrefs.GetInt("CountRunFlag", 0) + 1);
			}
		}
		else
		{
			int int5 = PlayerPrefs.GetInt("CountRunDeadmath", 0);
			bool flag = int5 < 5;
			Texture texture2 = Resources.Load("NoteLoadings/note_Deathmatch_" + int5 % countNoteCaptureDeadmatch) as Texture;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunDeadmath", PlayerPrefs.GetInt("CountRunDeadmath", 0) + 1);
			}
		}
		LoadConnectScene.textureToShow = Resources.Load("LevelLoadings" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + _mapName) as Texture2D;
		LoadingInAfterGame.loadingTexture = LoadConnectScene.textureToShow;
		LoadConnectScene.sceneToLoad = _mapName;
		LoadConnectScene.noteToShow = null;
		loadingToDraw.gameObject.SetActive(false);
	}

	private IEnumerator MoveToGameScene(string _goMapName)
	{
		Debug.Log("MoveToGameScene=" + _goMapName);
		Defs.isGameFromFriends = false;
		Defs.isGameFromClans = false;
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(_goMapName) ? Defs.filterMaps[_goMapName] : 0);
		}
		GlobalGameController.countKillsBlue = 0;
		GlobalGameController.countKillsRed = 0;
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		PlayerPrefs.SetString("RoomName", PhotonNetwork.room.name);
		PlayerPrefs.SetInt("CustomGame", 0);
		PhotonNetwork.isMessageQueueRunning = false;
		yield return null;
		yield return Resources.UnloadUnusedAssets();
		yield return StartCoroutine(SetFonLoadingWaitForReset(_goMapName, true));
		loadingMapPanel.SetActive(true);
		isGoInPhotonGame = true;
		AsyncOperation async = Singleton<SceneLoader>.Instance.LoadSceneAsync("PromScene");
		FlurryPluginWrapper.LogEvent("Play_" + _goMapName);
		if (FriendsController.sharedController != null)
		{
			FriendsController.sharedController.GetFriendsData();
		}
		yield return async;
		for (int i = 0; i < grid.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(grid.transform.GetChild(i).gameObject);
		}
		mapPreview.Clear();
	}

	[Obfuscation(Exclude = true)]
	private void ConnectToPhoton()
	{
		if (FriendsController.sharedController != null && FriendsController.sharedController.Banned == 1)
		{
			return;
		}
		if (PhotonNetwork.connectionState == ConnectionState.Connecting || PhotonNetwork.connectionState == ConnectionState.Connected)
		{
			Debug.Log("ConnectToPhoton return");
			return;
		}
		Debug.Log("ConnectToPhoton");
		if (FriendsController.sharedController != null && FriendsController.sharedController.Banned == 1)
		{
			timerShowBan = 3f;
			return;
		}
		isConnectingToPhoton = true;
		isCancelConnectingToPhoton = false;
		gameTier = ((!(ExpController.Instance != null)) ? 1 : ExpController.Instance.OurTier);
		if (Defs.useSqlLobby)
		{
			PhotonNetwork.lobby = new TypedLobby("PixelGun3D", LobbyType.SqlLobby);
		}
		PhotonNetwork.ConnectUsingSettings(Initializer.Separator + regim.ToString() + (Defs.isDaterRegim ? "Dater" : ((!Defs.isHunger) ? gameTier.ToString() : "0")) + "v" + GlobalGameController.MultiplayerProtocolVersion);
	}

	private void StartSearchLocalServers()
	{
		lanScan.StartSearchBroadCasting(SeachServer);
	}

	private void SeachServer(string ipServerSeaches)
	{
		bool flag = false;
		if (servers.Count > 0)
		{
			foreach (infoServer server in servers)
			{
				if (server.ipAddress.Equals(ipServerSeaches))
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			infoServer item = default(infoServer);
			item.ipAddress = ipServerSeaches;
			servers.Add(item);
		}
	}

	private int LocalServerComparison(LANBroadcastService.ReceivedMessage msg1, LANBroadcastService.ReceivedMessage msg2)
	{
		return msg1.ipAddress.CompareTo(msg2.ipAddress);
	}

	private void SetLocalRoomInfo(GameInfo _gameInfo, int index)
	{
		_gameInfo.index = index;
		if (_copy != null && _copy.Length > index)
		{
			_gameInfo.gameObject.SetActive(true);
			LANBroadcastService.ReceivedMessage roomInfoLocal = _copy[index];
			string text = roomInfoLocal.name;
			if (string.IsNullOrEmpty(text))
			{
				text = LocalizationStore.Get("Key_0948");
			}
			_gameInfo.serverNameLabel.text = text;
			_gameInfo.countPlayersLabel.text = roomInfoLocal.connectedPlayers + "/" + roomInfoLocal.playerLimit;
			_gameInfo.openSprite.SetActive(true);
			_gameInfo.closeSprite.SetActive(false);
			string arg = roomInfoLocal.map;
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(roomInfoLocal.map);
			if (infoScene != null)
			{
				arg = infoScene.TranslateName;
			}
			_gameInfo.mapNameLabel.text = string.Format("{0}: {1}", LocalizationStore.Get("Key_0947"), arg);
			_gameInfo.roomInfoLocal = roomInfoLocal;
		}
		else
		{
			_gameInfo.gameObject.SetActive(false);
		}
	}

	private void UpdateLocalServersList()
	{
		List<LANBroadcastService.ReceivedMessage> list = new List<LANBroadcastService.ReceivedMessage>();
		for (int i = 0; i < lanScan.lstReceivedMessages.Count; i++)
		{
			bool flag = Defs.filterMaps.ContainsKey(lanScan.lstReceivedMessages[i].map) && Defs.filterMaps[lanScan.lstReceivedMessages[i].map] == 3;
			if (((Defs.isDaterRegim && flag) || (!Defs.isDaterRegim && !flag)) && lanScan.lstReceivedMessages[i].regim == (int)regim)
			{
				list.Add(lanScan.lstReceivedMessages[i]);
			}
		}
		if (list.Count > 0)
		{
			_copy = list.ToArray();
			Array.Sort(_copy, LocalServerComparison);
		}
		else
		{
			_copy = null;
		}
		if (_copy != null)
		{
			if (_copy.Length < 4)
			{
				if (scrollGames.enabled)
				{
					wrapGames.SortAlphabetically();
					scrollGames.ResetPosition();
					scrollGames.enabled = false;
				}
			}
			else
			{
				scrollGames.enabled = true;
			}
			wrapGames.minIndex = _copy.Length * -1;
			if (_copy.Length > 0 && roomFields == null)
			{
				roomFields = new GameInfo[5];
				for (int j = 0; j < roomFields.Length; j++)
				{
					GameObject gameObject = NGUITools.AddChild(wrapGames.gameObject, gameInfoItemPrefab);
					gameObject.name = "GameInfo_" + j;
					roomFields[j] = gameObject.GetComponent<GameInfo>();
				}
				wrapGames.SortAlphabetically();
				scrollGames.enabled = true;
				scrollGames.ResetPosition();
			}
		}
		if (roomFields != null)
		{
			for (int k = 0; k < roomFields.Length; k++)
			{
				SetLocalRoomInfo(roomFields[k], roomFields[k].index);
			}
		}
	}

	public void JoinToLocalRoom(LANBroadcastService.ReceivedMessage _roomInfo)
	{
		if (_roomInfo.connectedPlayers == _roomInfo.playerLimit)
		{
			gameIsfullLabel.timer = 3f;
			gameIsfullLabel.gameObject.SetActive(true);
			incorrectPasswordLabel.timer = -1f;
			incorrectPasswordLabel.gameObject.SetActive(false);
			return;
		}
		GlobalGameController.countKillsBlue = 0;
		GlobalGameController.countKillsRed = 0;
		Defs.ServerIp = _roomInfo.ipAddress;
		PlayerPrefs.SetString("MaxKill", _roomInfo.comment);
		PlayerPrefs.SetString("MapName", _roomInfo.map);
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(_roomInfo.map) ? Defs.filterMaps[_roomInfo.map] : 0);
		}
		StartCoroutine(SetFonLoadingWaitForReset(_roomInfo.map));
		Invoke("goGame", 0.1f);
	}

	private bool CheckLocalAvailability()
	{
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			return true;
		}
		return false;
	}

	public void JoinToRoomPhoton(RoomInfo _roomInfo)
	{
		if (_roomInfo.playerCount == _roomInfo.maxPlayers)
		{
			gameIsfullLabel.timer = 3f;
			gameIsfullLabel.gameObject.SetActive(true);
			incorrectPasswordLabel.timer = -1f;
			incorrectPasswordLabel.gameObject.SetActive(false);
			return;
		}
		joinRoomInfoFromCustom = _roomInfo;
		if (string.IsNullOrEmpty(_roomInfo.customProperties[passwordProperty].ToString()))
		{
			JoinToRoomPhotonAfterCheck();
			return;
		}
		gameIsfullLabel.timer = -1f;
		gameIsfullLabel.gameObject.SetActive(false);
		incorrectPasswordLabel.timer = 3f;
		incorrectPasswordLabel.gameObject.SetActive(true);
		enterPasswordInput.value = string.Empty;
		enterPasswordPanel.SetActive(true);
		enterPasswordInput.isSelected = false;
		enterPasswordInput.isSelected = true;
		ExperienceController.sharedController.isShowRanks = false;
		customPanel.SetActive(false);
	}

	private void EnterPassInputSubmit()
	{
		enterPasswordInput.RemoveFocus();
		enterPasswordInput.isSelected = false;
		Invoke("EnterPassInput", 0.1f);
	}

	private void EnterPassInput()
	{
		HandleJoinRoomFromEnterPasswordBtnClicked(null, null);
	}

	public void JoinToRoomPhotonAfterCheck()
	{
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(joinRoomInfoFromCustom.customProperties[mapProperty].ToString()));
		StartCoroutine(SetFonLoadingWaitForReset(infoScene.NameScene));
		loadingMapPanel.SetActive(true);
		PhotonNetwork.JoinRoom(joinRoomInfoFromCustom.name);
		ActivityIndicator.IsActiveIndicator = true;
	}

	private void SetPosSelectMapPanelInMainMenu()
	{
		if (Defs.isDaterRegim)
		{
			ChooseMapLabelSmall.SetActive(false);
			return;
		}
		float num = (float)Screen.width * 768f / (float)Screen.height - 322f;
		if (!Defs.isDaterRegim)
		{
			selectMapPanelTransform.localPosition = new Vector3(149f, 73f, 0f);
		}
		fonMapPreview.width = Mathf.RoundToInt(num);
		fonMapPreview.height = 434;
		fonMapPreview.transform.localPosition = Vector3.zero;
		mapPreviewPanel.baseClipRegion = new Vector4(0f, 0f, num, mapPreviewPanel.baseClipRegion.w);
		ChooseMapLabelSmall.SetActive(true);
	}

	private void SetPosSelectMapPanelInCreatePanel()
	{
		if (!Defs.isDaterRegim)
		{
			selectMapPanelTransform.localPosition = new Vector3(0f, 35f, 0f);
			fonMapPreview.width = Mathf.RoundToInt((float)Screen.width * 768f / (float)Screen.height + 10f);
			fonMapPreview.height = 376;
			if (!Defs.isDaterRegim)
			{
				fonMapPreview.transform.localPosition = new Vector3(0f, -24f, 0f);
			}
			mapPreviewPanel.baseClipRegion = new Vector4(0f, 0f, (float)Screen.width * 768f / (float)Screen.height, mapPreviewPanel.baseClipRegion.w);
			ChooseMapLabelSmall.SetActive(false);
		}
	}

	[Obfuscation(Exclude = true)]
	private void goGame()
	{
		WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(PlayerPrefs.GetString("MapName")) ? Defs.filterMaps[PlayerPrefs.GetString("MapName")] : 0);
		Singleton<SceneLoader>.Instance.LoadScene(PlayerPrefs.GetString("MapName"));
	}

	private void LogUserQuit()
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Round {0}", _countOfLoopsRequestAdThisTime + 1);
			stringBuilder.AppendFormat(", Slot {0} ({1})", InterstitialManager.Instance.ProviderClampedIndex + 1, AnalyticsHelper.GetAdProviderName(InterstitialManager.Instance.Provider));
			if (InterstitialManager.Instance.Provider == AdProvider.GoogleMobileAds)
			{
				stringBuilder.AppendFormat(", Unit {0}", MobileAdManager.Instance.ImageAdUnitIndexClamped + 1);
			}
			stringBuilder.Append(" - User quit");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Quit - Interstitial", stringBuilder.ToString());
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole("ADS Statistics Total", parameters);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void LogUserInterstitialRequest()
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Round {0}", _countOfLoopsRequestAdThisTime + 1);
			stringBuilder.AppendFormat(", Slot {0} ({1})", InterstitialManager.Instance.ProviderClampedIndex + 1, AnalyticsHelper.GetAdProviderName(InterstitialManager.Instance.Provider));
			if (InterstitialManager.Instance.Provider == AdProvider.GoogleMobileAds)
			{
				stringBuilder.AppendFormat(", Unit {0}", MobileAdManager.Instance.ImageAdUnitIndexClamped + 1);
			}
			stringBuilder.Append(" - Request");
			string value = stringBuilder.ToString();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Quit - Interstitial", value);
			dictionary.Add("Statistics - Interstitial", value);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole("ADS Statistics Total", parameters);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void Awake()
	{
		if (isReturnFromGame)
		{
			Defs.countReturnInConnectScene++;
		}
		PhotonObjectCacher.AddObject(base.gameObject);
		setPasswordInput.onSubmit.Add(new EventDelegate(delegate
		{
			OnPaswordSelected();
		}));
	}

	private void OnDestroy()
	{
		Debug.Log("OnDestroy ConnectSceneController");
		if (isStartShowAdvert)
		{
			LogIsShowAdvert("Connect Scene", false);
		}
		LogUserQuit();
		if (!Defs.isInet || (!isGoInPhotonGame && PhotonNetwork.connectionState == ConnectionState.Connected) || PhotonNetwork.connectionState == ConnectionState.Connecting)
		{
			PhotonNetwork.Disconnect();
			Debug.Log("PhotonNetwork.Disconnect()");
		}
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = false;
			ExperienceController.sharedController.isMenu = false;
			ExperienceController.sharedController.isConnectScene = false;
		}
		lanScan.StopBroadCasting();
		sharedController = null;
		PhotonObjectCacher.RemoveObject(base.gameObject);
	}

	private void LogIsShowAdvert(string context, bool isShow)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Context", context);
		Dictionary<string, string> dictionary2 = dictionary;
		dictionary2.Add("Show", isShow.ToString());
		if (ExperienceController.sharedController != null)
		{
			dictionary2.Add("Level", ExperienceController.sharedController.currentLevel.ToString());
		}
		if (ExpController.Instance != null)
		{
			dictionary2.Add("Tier", ExpController.Instance.OurTier.ToString());
		}
		FlurryPluginWrapper.LogEventAndDublicateToConsole("Advert show", dictionary2);
	}
}
