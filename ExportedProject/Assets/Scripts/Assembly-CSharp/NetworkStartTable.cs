using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using FyberPlugin;
using Prime31;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class NetworkStartTable : MonoBehaviour
{
	public struct infoClient
	{
		public string ipAddress;

		public string name;

		public string coments;
	}

	public string pixelBookID = "-1";

	private SaltedInt _scoreCommandFlag1 = new SaltedInt(818919);

	private SaltedInt _scoreCommandFlag2 = new SaltedInt(823016);

	public double timerFlag;

	private float maxTimerFlag = 150f;

	private float timerUpdateTimerFlag;

	private float maxTimerUpdateTimerFlag = 1f;

	public bool isShowAvard;

	private bool isShowFinished;

	private bool isEndInHunger;

	private int addExperience;

	public GameObject guiObj;

	public NetworkStartTableNGUIController networkStartTableNGUIController;

	public bool isRegimVidos;

	private int numberPlayerCun;

	private int numberPlayerCunId;

	public Player_move_c currentPlayerMoveCVidos;

	private bool oldIsZomming;

	private InGameGUI inGameGUI;

	public string playerVidosNick;

	public string playerVidosClanName;

	public Texture playerVidosClanTexture;

	public GameObject currentCamPlayer;

	public GameObject currentFPSPlayer;

	private GameObject currentBodyMech;

	public GameObject currentGameObjectPlayer;

	public bool isGoRandomRoom;

	public Texture mySkin;

	public List<GameObject> zombiePrefabs = new List<GameObject>();

	private GameObject _playerPrefab;

	public GameObject tempCam;

	public GameObject zombieManagerPrefab;

	public Texture2D serverLeftTheGame;

	public ExperienceController experienceController;

	private int addCoins;

	public bool isDeadInHungerGame;

	private bool showMessagFacebook;

	private bool clickButtonFacebook;

	public bool isIwin;

	public int myCommand;

	public int myCommandOld;

	private bool isLocal;

	private bool isMine;

	private bool isCOOP;

	private bool isServer;

	private bool isCompany;

	private bool isMulti;

	private bool isInet;

	private float timeNotRunZombiManager;

	private bool isSendZaprosZombiManager;

	private bool isGetZaprosZombiManager;

	private ExperienceController expController;

	public Texture myClanTexture;

	public string myClanID = string.Empty;

	public string myClanName = string.Empty;

	public string myClanLeaderID = string.Empty;

	private LANBroadcastService lanScan;

	private bool isSetNewMapButton;

	public bool isDrawInHanger;

	public List<infoClient> players = new List<infoClient>();

	public GUIStyle labelStyle;

	public GUIStyle messagesStyle;

	public GUIStyle ozidanieStyle;

	private Vector2 scrollPosition = Vector2.zero;

	private float koofScreen = (float)Screen.height / 768f;

	public WeaponManager _weaponManager;

	public bool _showTable;

	public string nickPobeditelya;

	public bool _isShowNickTable;

	public bool runGame = true;

	public GameObject[] zoneCreatePlayer;

	private GameObject _cam;

	public bool isDrawInDeathMatch;

	public bool showDisconnectFromServer;

	public bool showDisconnectFromMasterServer;

	private float timerShow = -1f;

	public string NamePlayer = "Player";

	public int CountKills;

	public int oldCountKills;

	public string[] oldSpisokName;

	public string[] oldCountLilsSpisok;

	public string[] oldScoreSpisok;

	public int[] oldSpisokRanks;

	public string[] oldSpisokNameBlue;

	public string[] oldCountLilsSpisokBlue;

	public int[] oldSpisokRanksBlue;

	public string[] oldSpisokNameRed;

	public string[] oldCountLilsSpisokRed;

	public string[] oldScoreSpisokRed;

	public string[] oldScoreSpisokBlue;

	public int[] oldSpisokRanksRed;

	public bool[] oldIsDeadInHungerGame;

	public string[] oldSpisokPixelBookID;

	public string[] oldSpisokPixelBookIDBlue;

	public string[] oldSpisokPixelBookIDRed;

	public Texture[] oldSpisokMyClanLogo;

	public Texture[] oldSpisokMyClanLogoBlue;

	public Texture[] oldSpisokMyClanLogoRed;

	public int oldIndexMy;

	private GameObject tc;

	public int scoreOld;

	private PhotonView photonView;

	private float timeTomig = 0.5f;

	private float timerSynchScore = -1f;

	private int countMigZagolovok;

	private int commandWinner;

	private bool isMigZag;

	private HungerGameController hungerGameController;

	private bool _canUserUseFacebookComposer;

	private bool _hasPublishPermission;

	private bool _hasPublishActions;

	private SaltedInt _score = default(SaltedInt);

	private static System.Random _prng = new System.Random(19937);

	public int myRanks = 1;

	public Player_move_c myPlayerMoveC;

	private bool isHunger;

	private int _gameRating = -1;

	private ShopNGUIController _shopInstance;

	private int playerCountInHunger;

	private bool isStartPlayerCoroutine;

	private string waitingPlayerLocalize;

	private string matchLocalize;

	private string preparingLocalize;

	private Pauser _pauser;

	private PauseNGUIController _pauseController;

	private Stopwatch _matchStopwatch = new Stopwatch();

	private int killCountMatch;

	private int deathCountMatch;

	public int scoreCommandFlag1
	{
		get
		{
			return _scoreCommandFlag1.Value;
		}
		set
		{
			_scoreCommandFlag1 = value;
		}
	}

	public int scoreCommandFlag2
	{
		get
		{
			return _scoreCommandFlag2.Value;
		}
		set
		{
			_scoreCommandFlag2 = value;
		}
	}

	public bool showTable
	{
		get
		{
			return _showTable;
		}
		set
		{
			_showTable = value;
			if (isMine)
			{
				Defs.showTableInNetworkStartTable = value;
			}
		}
	}

	public bool isShowNickTable
	{
		get
		{
			return _isShowNickTable;
		}
		set
		{
			_isShowNickTable = value;
			if (isMine)
			{
				Defs.showNickTableInNetworkStartTable = value;
			}
		}
	}

	public int score
	{
		get
		{
			return _score.Value;
		}
		set
		{
			_score = new SaltedInt(_prng.Next(), value);
		}
	}

	public int gameRating
	{
		get
		{
			return (!Defs.isMulti || !isMine) ? _gameRating : RatingSystem.instance.currentRating;
		}
		set
		{
			_gameRating = value;
		}
	}

	public static Vector2 ExperiencePosRanks
	{
		get
		{
			return new Vector2(21f * Defs.Coef, 21f * Defs.Coef);
		}
	}

	private string _SocialMessage()
	{
		int @int = Storager.getInt(Defs.COOPScore, false);
		bool flag = Defs.isCOOP;
		int int2 = Storager.getInt("Rating", false);
		string arg = "http://goo.gl/dQMf4n";
		if (isIwin)
		{
			return (!flag) ? string.Format("I won the match in @PixelGun3D ! Try it right now!\n#pixelgun3d #pg3d #pixelgun", int2, arg) : string.Format(" Now I have {0} score in @PixelGun3D ! Try it right now!\n#pixelgun3d #pg3d #pixelgun", @int, arg);
		}
		return (!flag) ? string.Format("I played a match in @PixelGun3D ! Try it right now!\n#pixelgun3d #pg3d #pixelgun", int2, arg) : string.Format("I received {0} points in @PixelGun3D ! Try it right now!\n#pixelgun3d #pg3d #pixelgun", @int, arg);
	}

	private string _SocialSentSuccess(string SocialName)
	{
		return "Message was sent to " + SocialName;
	}

	private void completionHandler(string error, object result)
	{
		if (error != null)
		{
			Debug.LogError(error);
			return;
		}
		Prime31.Utils.logObject(result);
		showMessagFacebook = true;
		Invoke("hideMessag", 3f);
	}

	private void Awake()
	{
		isLocal = !Defs.isInet;
		isInet = Defs.isInet;
		isCOOP = ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle;
		if (isInet)
		{
			isServer = PhotonNetwork.isMasterClient;
		}
		else
		{
			isServer = PlayerPrefs.GetString("TypeGame").Equals("server");
		}
		isMulti = Defs.isMulti;
		isCompany = ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight;
		isHunger = ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.DeadlyGames;
		experienceController = GameObject.FindGameObjectWithTag("ExperienceController").GetComponent<ExperienceController>();
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle)
		{
			string[] array = null;
			array = new string[10] { "1", "15", "14", "2", "3", "9", "11", "12", "10", "16" };
			for (int i = 0; i < array.Length; i++)
			{
				GameObject item = Resources.Load("Enemies/Enemy" + array[i] + "_go") as GameObject;
				zombiePrefabs.Add(item);
			}
		}
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
		{
			maxTimerFlag = (float)int.Parse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty].ToString()) * 60f;
		}
		photonView = PhotonView.Get(this);
		Initializer.networkTables.Add(this);
		if ((bool)photonView && photonView.isMine)
		{
			PhotonObjectCacher.AddObject(base.gameObject);
		}
	}

	public void ImDeadInHungerGames()
	{
		if (Defs.isInet && NetworkStartTableNGUIController.sharedController != null)
		{
			NetworkStartTableNGUIController.sharedController.UpdateGoMapButtons();
			isSetNewMapButton = true;
		}
		_matchStopwatch.Stop();
		int @int = PlayerPrefs.GetInt("CountMatch", 0);
		PlayerPrefs.SetInt("CountMatch", @int + 1);
		if (ExperienceController.sharedController != null)
		{
			string key = "Statistics.MatchCount.Level" + ExperienceController.sharedController.currentLevel;
			int int2 = PlayerPrefs.GetInt(key, 0);
			PlayerPrefs.SetInt(key, int2 + 1);
			FlurryPluginWrapper.LogMatchCompleted(ConnectSceneNGUIController.regim.ToString());
		}
		IncreaseTimeInMode(3, _matchStopwatch.get_Elapsed().TotalMinutes);
		StoreKitEventListener.State.PurchaseKey = "End match";
		if (_cam != null)
		{
			_cam.SetActive(true);
		}
		NickLabelController.currentCamera = Initializer.Instance.tc.GetComponent<Camera>();
		showTable = true;
		RatingSystem.RatingChange ratingChange = CalculateMatchRating(false);
		photonView.RPC("ImDeadInHungerGamesRPC", PhotonTargets.Others);
		isDeadInHungerGame = true;
		if (NetworkStartTableNGUIController.sharedController != null)
		{
			NetworkStartTableNGUIController.sharedController.ShowEndInterfaceDeadInHunger(LocalizationStore.Get("Key_1116"), ratingChange);
		}
		inGameGUI.ResetScope();
	}

	[RPC]
	[PunRPC]
	public void ImDeadInHungerGamesRPC()
	{
		isDeadInHungerGame = true;
	}

	public void setScoreFromGlobalGameController()
	{
		score = GlobalGameController.Score;
		SynhScore();
	}

	[PunRPC]
	[RPC]
	private void RunGame()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("NetworkTable");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.GetComponent<NetworkStartTable>().runGame = true;
		}
	}

	public void RemoveShop(bool disable = true)
	{
		ShopTapReceiver.ShopClicked -= HandleShopButton;
		if (_shopInstance != null)
		{
			if (disable)
			{
				ShopNGUIController.GuiActive = false;
			}
			_shopInstance.resumeAction = delegate
			{
			};
			_shopInstance = null;
		}
	}

	public void HandleShopButton()
	{
		if (_shopInstance == null && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled))
		{
			FlurryPluginWrapper.LogEvent("Shop");
			_shopInstance = ShopNGUIController.sharedShop;
			if (_shopInstance != null)
			{
				_shopInstance.SetGearCatEnabled(false);
				ShopNGUIController.GuiActive = true;
				_shopInstance.resumeAction = HandleResumeFromShop;
			}
			else
			{
				Debug.LogWarning("sharedShop == null");
			}
		}
	}

	public void HandleResumeFromShop()
	{
		if (_shopInstance != null)
		{
			expController.isShowRanks = true;
			ShopNGUIController.GuiActive = false;
			_shopInstance.resumeAction = delegate
			{
			};
			_shopInstance = null;
		}
	}

	public void BackButtonPress()
	{
		if (ExpController.Instance.IsLevelUpShown)
		{
			return;
		}
		NetworkStartTableNGUIController sharedController = NetworkStartTableNGUIController.sharedController;
		if (sharedController != null && sharedController.CheckHideInternalPanel())
		{
			return;
		}
		networkStartTableNGUIController.shopAnchor.SetActive(false);
		RemoveShop();
		if (!isInet)
		{
			if (isServer)
			{
				Network.Disconnect(200);
				GameObject.FindGameObjectWithTag("NetworkTable").GetComponent<LANBroadcastService>().StopBroadCasting();
			}
			else if (Network.connections.Length == 1)
			{
				Debug.Log("Disconnecting: " + Network.connections[0].ipAddress + ":" + Network.connections[0].port);
				Network.CloseConnection(Network.connections[0], true);
			}
			ActivityIndicator.IsActiveIndicator = false;
			ConnectSceneNGUIController.Local();
		}
		else
		{
			ActivityIndicator.IsActiveIndicator = false;
			Defs.typeDisconnectGame = Defs.DisconectGameType.Exit;
			PhotonNetwork.Disconnect();
		}
		if (EveryplayWrapper.Instance.CurrentState == EveryplayWrapper.State.Paused || EveryplayWrapper.Instance.CurrentState == EveryplayWrapper.State.Recording)
		{
			EveryplayWrapper.Instance.Stop();
		}
	}

	public void StartPlayerButtonClick(int _command)
	{
		if (NetworkStartTableNGUIController.sharedController != null)
		{
			NetworkStartTableNGUIController.sharedController.HideEndInterface();
		}
		isShowNickTable = false;
		CountKills = 0;
		score = 0;
		GlobalGameController.Score = 0;
		GlobalGameController.CountKills = 0;
		myCommand = _command;
		SynhCommand();
		SynhCountKills();
		SynhScore();
		startPlayer();
		countMigZagolovok = 0;
		timeTomig = 0.7f;
		isMigZag = false;
	}

	public void RandomRoomClickBtnInHunger()
	{
		isGoRandomRoom = true;
		if (isRegimVidos)
		{
			isRegimVidos = false;
			if (inGameGUI != null)
			{
				inGameGUI.ResetScope();
			}
		}
		Defs.typeDisconnectGame = Defs.DisconectGameType.RandomGameInHunger;
		PhotonNetwork.LeaveRoom();
	}

	public void SetRegimVidos(bool _isRegimVidos)
	{
		bool flag = isRegimVidos;
		isRegimVidos = _isRegimVidos;
		if (isRegimVidos != flag && !isRegimVidos && inGameGUI != null)
		{
			inGameGUI.ResetScope();
		}
	}

	private void playersTable()
	{
		if (!isShowAvard)
		{
			ShopTapReceiver.AddClickHndIfNotExist(HandleShopButton);
			networkStartTableNGUIController.shopAnchor.SetActive(!isShowFinished && !isHunger && _shopInstance == null && (expController == null || !expController.isShowNextPlashka));
			if (_shopInstance != null)
			{
				_shopInstance.SetGearCatEnabled(false);
			}
		}
	}

	public void PostFacebookBtnClick()
	{
		Debug.Log("show facebook dialog");
		FlurryPluginWrapper.LogFacebook();
		FacebookController.ShowPostDialog();
	}

	public void PostTwitterBtnClick()
	{
		FlurryPluginWrapper.LogTwitter();
		if (TwitterController.Instance != null)
		{
			TwitterController.Instance.PostStatusUpdate(_SocialMessage());
		}
	}

	private IEnumerator StartPlayerCoroutine()
	{
		Defs.inRespawnWindow = false;
		photonView.RPC("SynchGameRating", PhotonTargets.Others, gameRating);
		if (isStartPlayerCoroutine)
		{
			yield break;
		}
		isStartPlayerCoroutine = true;
		while (Defs.isMulti && Defs.isInet && PhotonNetwork.time > -0.01 && PhotonNetwork.time < 0.01)
		{
			yield return null;
		}
		isStartPlayerCoroutine = false;
		if (Defs.isMulti && !Defs.isHunger)
		{
			TimeGameController.sharedController.StartMatch();
		}
		if (Defs.isDaterRegim)
		{
			int _timeGame = 5;
			_timeGame = ((!Defs.isInet) ? (PlayerPrefs.GetString("MaxKill", "9").Equals(string.Empty) ? 5 : int.Parse(PlayerPrefs.GetString("MaxKill", "5"))) : ((int)PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty]));
			AnalyticsStuff.LogSandboxTimeGamePopularity(_timeGame, true);
		}
		isDrawInDeathMatch = false;
		_matchStopwatch.Start();
		StoreKitEventListener.State.PurchaseKey = "In game";
		StoreKitEventListener.State.Parameters.Clear();
		networkStartTableNGUIController.shopAnchor.SetActive(false);
		RemoveShop();
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
		{
			timerFlag = maxTimerFlag;
		}
		if (myRanks != expController.currentLevel)
		{
			SetRanks();
		}
		_cam = GameObject.FindGameObjectWithTag("CamTemp");
		_cam.SetActive(false);
		_weaponManager.useCam = null;
		zoneCreatePlayer = GameObject.FindGameObjectsWithTag((ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle) ? "MultyPlayerCreateZoneCOOP" : ((ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight) ? ("MultyPlayerCreateZoneCommand" + myCommand) : ((ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture) ? ("MultyPlayerCreateZoneFlagCommand" + myCommand) : ((ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.CapturePoints) ? "MultyPlayerCreateZone" : ("MultyPlayerCreateZonePointZone" + myCommand)))));
		GameObject chestSpawnZone = null;
		int numberSpawnZone = 0;
		int numberZoneChest = 0;
		if (isHunger)
		{
			if (PlayerPrefs.GetInt("StartAfterDisconnect") != 1)
			{
				_weaponManager.Reset();
			}
			int _myId = photonView.owner.ID;
			GameObject[] tabMas = GameObject.FindGameObjectsWithTag("NetworkTable");
			for (int i = 0; i < tabMas.Length; i++)
			{
				PhotonPlayer owner = tabMas[i].transform.GetComponent<PhotonView>().owner;
				if (owner != null && owner.ID < _myId)
				{
					numberSpawnZone++;
				}
			}
			numberZoneChest = numberSpawnZone;
			for (int j = 0; j < zoneCreatePlayer.Length; j++)
			{
				if (zoneCreatePlayer[j].GetComponent<NumberZone>().numberZone == numberSpawnZone)
				{
					numberSpawnZone = j;
					break;
				}
			}
			if (PlayerPrefs.GetInt("StartAfterDisconnect") != 1)
			{
				GameObject[] chestCreateZones = GameObject.FindGameObjectsWithTag("ChestCreateZone");
				for (int k = 0; k < chestCreateZones.Length; k++)
				{
					if (chestCreateZones[k].GetComponent<NumberZone>().numberZone == numberZoneChest)
					{
						chestSpawnZone = chestCreateZones[k];
						photonView.RPC("CreateChestRPC", PhotonTargets.MasterClient, chestSpawnZone.transform.position, chestSpawnZone.transform.rotation);
						break;
					}
				}
			}
			playerCountInHunger = Initializer.networkTables.Count;
		}
		GameObject spawnZone = zoneCreatePlayer[(!isHunger) ? UnityEngine.Random.Range(0, zoneCreatePlayer.Length - 1) : numberSpawnZone];
		BoxCollider spawnZoneCollider = spawnZone.GetComponent<BoxCollider>();
		Vector2 sz = new Vector2(spawnZoneCollider.size.x * spawnZone.transform.localScale.x, spawnZoneCollider.size.z * spawnZone.transform.localScale.z);
		Rect zoneRect = new Rect(spawnZone.transform.position.x - sz.x / 2f, spawnZone.transform.position.z - sz.y / 2f, sz.x, sz.y);
		Vector3 pos = ((!isHunger) ? new Vector3(zoneRect.x + UnityEngine.Random.Range(0f, zoneRect.width), spawnZone.transform.position.y, zoneRect.y + UnityEngine.Random.Range(0f, zoneRect.height)) : spawnZone.transform.position);
		Quaternion rot = spawnZone.transform.rotation;
		if (PlayerPrefs.GetInt("StartAfterDisconnect") == 1 && GlobalGameController.healthMyPlayer > 0f)
		{
			pos = GlobalGameController.posMyPlayer;
		}
		GameObject pl;
		if (isInet)
		{
			pl = PhotonNetwork.Instantiate("Player", pos, rot, 0);
		}
		else
		{
			if (_playerPrefab == null)
			{
				_playerPrefab = Resources.Load("Player") as GameObject;
			}
			pl = (GameObject)Network.Instantiate(_playerPrefab, pos, rot, 0);
			pl.GetComponent<SkinName>().playerMoveC.SetIDMyTable(GetComponent<NetworkView>().viewID.ToString());
		}
		NickLabelController.currentCamera = pl.GetComponent<SkinName>().camPlayer.GetComponent<Camera>();
		_weaponManager.myPlayer = pl;
		_weaponManager.myPlayerMoveC = pl.GetComponent<SkinName>().playerMoveC;
		if (!isInet && isServer)
		{
			Debug.Log("networkView.RPC(RunGame, RPCMode.OthersBuffered);");
			GetComponent<NetworkView>().RPC("RunGame", RPCMode.OthersBuffered);
		}
		Initializer.Instance.SetupObjectThatNeedsPlayer();
		if (NetworkStartTableNGUIController.sharedController != null)
		{
			NetworkStartTableNGUIController.sharedController.HideStartInterface();
		}
		showTable = false;
		if (FriendsController.useBuffSystem)
		{
			BuffSystem.instance.BuffsActive(!Defs.isDaterRegim && !Defs.isHunger && !Defs.isCOOP && Defs.isMulti && Defs.isInet && !LocalOrPasswordRoom());
		}
		else
		{
			KillRateCheck.instance.SetActive(!Defs.isDaterRegim && !Defs.isHunger && !Defs.isCOOP && Defs.isMulti && Defs.isInet && !LocalOrPasswordRoom() && WeaponManager.sharedManager._currentFilterMap == 0, TimeGameController.sharedController != null && TimeGameController.sharedController.timerToEndMatch > 30.0);
		}
	}

	[Obfuscation(Exclude = true)]
	public void startPlayer()
	{
		StartCoroutine(StartPlayerCoroutine());
	}

	[RPC]
	[PunRPC]
	public void CreateChestRPC(Vector3 pos, Quaternion rot)
	{
		PhotonNetwork.InstantiateSceneObject("HungerGames/Chest", pos, rot, 0, null);
	}

	[PunRPC]
	[RPC]
	private void SetPixelBookID(string _pixelBookID)
	{
		pixelBookID = _pixelBookID;
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if ((bool)photonView && photonView.isMine)
		{
			if (Defs.isFlag && !isShowFinished)
			{
				photonView.RPC("SynchScoreCommandRPC", player, 1, scoreCommandFlag1);
				photonView.RPC("SynchScoreCommandRPC", player, 2, scoreCommandFlag2);
			}
			SynhCommand(player);
			SynhCountKills(player);
			SendSynhScore(player);
			if (Defs.isMulti && Defs.isInet && isMine)
			{
				photonView.RPC("SynchGameRating", player, gameRating);
			}
		}
	}

	public void SetNewNick()
	{
		NamePlayer = ProfileController.GetPlayerNameOrDefault();
		if (Defs.isInet)
		{
			PhotonNetwork.playerName = NamePlayer;
			photonView.RPC("SynhNickNameRPC", PhotonTargets.OthersBuffered, NamePlayer);
		}
		else
		{
			GetComponent<NetworkView>().RPC("SynhNickNameRPC", RPCMode.OthersBuffered, NamePlayer);
		}
	}

	[PunRPC]
	[RPC]
	private void SynhNickNameRPC(string _nick)
	{
		NamePlayer = _nick;
	}

	public void SetRanks()
	{
		myRanks = expController.currentLevel;
		if (Defs.isInet)
		{
			photonView.RPC("SynhRanksRPC", PhotonTargets.OthersBuffered, myRanks);
		}
		else
		{
			GetComponent<NetworkView>().RPC("SynhRanksRPC", RPCMode.OthersBuffered, myRanks);
		}
	}

	[RPC]
	[PunRPC]
	private void SynhRanksRPC(int _ranks)
	{
		myRanks = _ranks;
	}

	public void SynhCommand(PhotonPlayer player = null)
	{
		if (Defs.isInet)
		{
			if (player == null)
			{
				photonView.RPC("SynhCommandRPC", PhotonTargets.Others, myCommand, myCommandOld);
			}
			else
			{
				photonView.RPC("SynhCommandRPC", player, myCommand, myCommandOld);
			}
		}
		else
		{
			GetComponent<NetworkView>().RPC("SynhCommandRPC", RPCMode.Others, myCommand, myCommandOld);
		}
	}

	[RPC]
	[PunRPC]
	private void SynhCommandRPC(int _command, int _oldCommand)
	{
		myCommand = _command;
		myCommandOld = _oldCommand;
		if (myPlayerMoveC != null)
		{
			myPlayerMoveC.myCommand = myCommand;
			if (Initializer.redPlayers.Contains(myPlayerMoveC) && myCommand == 1)
			{
				Initializer.redPlayers.Remove(myPlayerMoveC);
			}
			if (Initializer.bluePlayers.Contains(myPlayerMoveC) && myCommand == 2)
			{
				Initializer.bluePlayers.Remove(myPlayerMoveC);
			}
			if (myCommand == 1 && !Initializer.bluePlayers.Contains(myPlayerMoveC))
			{
				Initializer.bluePlayers.Add(myPlayerMoveC);
			}
			if (myCommand == 2 && !Initializer.redPlayers.Contains(myPlayerMoveC))
			{
				Initializer.redPlayers.Add(myPlayerMoveC);
			}
			if (myPlayerMoveC.myNickLabelController != null)
			{
				myPlayerMoveC.myNickLabelController.SetCommandColor(myCommand);
			}
		}
	}

	public void SynhCountKills(PhotonPlayer player = null)
	{
		if (Defs.isInet)
		{
			if (player == null)
			{
				photonView.RPC("SynhCountKillsRPC", PhotonTargets.Others, CountKills, oldCountKills);
			}
			else
			{
				photonView.RPC("SynhCountKillsRPC", player, CountKills, oldCountKills);
			}
		}
		else
		{
			GetComponent<NetworkView>().RPC("SynhCountKillsRPC", RPCMode.Others, CountKills, oldCountKills);
		}
	}

	[RPC]
	[PunRPC]
	private void SynhCountKillsRPC(int _countKills, int _oldCountKills)
	{
		CountKills = _countKills;
		oldCountKills = _oldCountKills;
	}

	public void SynhScore()
	{
		if (timerSynchScore < 0f)
		{
			timerSynchScore = 1f;
		}
	}

	public void ResetOldScore()
	{
		scoreOld = 0;
		SynhScore();
		oldCountKills = 0;
		SynhCountKills();
		GetMyTeam();
	}

	public void SendSynhScore(PhotonPlayer player = null)
	{
		if (Defs.isInet)
		{
			if (player == null)
			{
				photonView.RPC("SynhScoreRPC", PhotonTargets.Others, score, scoreOld);
			}
			else
			{
				photonView.RPC("SynhScoreRPC", player, score, scoreOld);
			}
		}
		else
		{
			GetComponent<NetworkView>().RPC("SynhScoreRPC", RPCMode.Others, score, scoreOld);
		}
	}

	[PunRPC]
	[RPC]
	private void SynhScoreRPC(int _score, int _oldScore)
	{
		score = _score;
		scoreOld = _oldScore;
	}

	[Obfuscation(Exclude = true)]
	private void hideMessag()
	{
		showMessagFacebook = false;
	}

	private void Start()
	{
		waitingPlayerLocalize = LocalizationStore.Key_0565;
		matchLocalize = LocalizationStore.Key_0566;
		preparingLocalize = LocalizationStore.Key_0567;
		lanScan = GetComponent<LANBroadcastService>();
		try
		{
			StartUnsafe();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		if (isMine && !TrainingController.TrainingCompleted)
		{
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Table_Deathmatch);
		}
	}

	private void StartUnsafe()
	{
		Stopwatch val = Stopwatch.StartNew();
		if (isMulti)
		{
			if (isLocal)
			{
				isMine = GetComponent<NetworkView>().isMine;
			}
			else
			{
				isMine = photonView.isMine;
			}
		}
		if (isMine)
		{
			networkStartTableNGUIController = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("NetworkStartTableNGUI"))).GetComponent<NetworkStartTableNGUIController>();
			_cam = GameObject.FindGameObjectWithTag("CamTemp");
			StoreKitEventListener.State.PurchaseKey = "Start table";
			if (FriendsController.sharedController.clanLogo != null && !string.IsNullOrEmpty(FriendsController.sharedController.clanLogo))
			{
				byte[] data = Convert.FromBase64String(FriendsController.sharedController.clanLogo);
				Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoWidth);
				texture2D.LoadImage(data);
				texture2D.filterMode = FilterMode.Point;
				texture2D.Apply();
				myClanTexture = texture2D;
				if (isInet)
				{
					photonView.RPC("SetMyClanTexture", PhotonTargets.AllBuffered, FriendsController.sharedController.clanLogo, FriendsController.sharedController.ClanID, FriendsController.sharedController.clanName, FriendsController.sharedController.clanLeaderID);
				}
				else
				{
					GetComponent<NetworkView>().RPC("SetMyClanTexture", RPCMode.AllBuffered, FriendsController.sharedController.clanLogo, FriendsController.sharedController.ClanID, FriendsController.sharedController.clanName, FriendsController.sharedController.clanLeaderID);
				}
			}
			Invoke("GetMyTeam", 1.5f);
			photonView.RPC("SynchGameRating", PhotonTargets.Others, gameRating);
		}
		if (isHunger)
		{
			hungerGameController = HungerGameController.Instance;
		}
		expController = ExperienceController.sharedController;
		expController.posRanks = ExperiencePosRanks;
		_weaponManager = WeaponManager.sharedManager;
		if (isMulti && isMine)
		{
			if (PlayerPrefs.GetInt("StartAfterDisconnect") == 0)
			{
				if (NetworkStartTableNGUIController.sharedController != null)
				{
					NetworkStartTableNGUIController.sharedController.ShowStartInterface();
				}
				showTable = true;
			}
			else
			{
				showTable = GlobalGameController.showTableMyPlayer;
				isDeadInHungerGame = GlobalGameController.imDeadInHungerGame;
				if (showTable || isEndInHunger)
				{
					if (!isDeadInHungerGame && !isEndInHunger)
					{
						if (NetworkStartTableNGUIController.sharedController != null)
						{
							NetworkStartTableNGUIController.sharedController.ShowStartInterface();
						}
					}
					else if (NetworkStartTableNGUIController.sharedController != null)
					{
						NetworkStartTableNGUIController.sharedController.ShowEndInterface(string.Empty, 0);
					}
				}
				else
				{
					if (NetworkStartTableNGUIController.sharedController != null)
					{
						NetworkStartTableNGUIController.sharedController.HideStartInterface();
					}
					Invoke("startPlayer", 0.1f);
				}
			}
			NickLabelController.currentCamera = Initializer.Instance.tc.GetComponent<Camera>();
			tempCam.SetActive(true);
			string text = (NamePlayer = FilterBadWorld.FilterString(ProfileController.GetPlayerNameOrDefault()));
			pixelBookID = FriendsController.sharedController.id;
			if (!isInet)
			{
				GetComponent<NetworkView>().RPC("SetPixelBookID", RPCMode.OthersBuffered, pixelBookID);
			}
			else
			{
				photonView.RPC("SetPixelBookID", PhotonTargets.OthersBuffered, pixelBookID);
			}
			if (isServer && !isInet)
			{
				lanScan.serverMessage.name = PlayerPrefs.GetString("ServerName");
				lanScan.serverMessage.map = PlayerPrefs.GetString("MapName");
				lanScan.serverMessage.connectedPlayers = 0;
				lanScan.serverMessage.playerLimit = int.Parse(PlayerPrefs.GetString("PlayersLimits"));
				lanScan.serverMessage.comment = PlayerPrefs.GetString("MaxKill");
				lanScan.serverMessage.regim = (int)ConnectSceneNGUIController.regim;
				lanScan.StartAnnounceBroadCasting();
				Debug.Log("lanScan.serverMessage.regim=" + lanScan.serverMessage.regim);
			}
			else
			{
				lanScan.enabled = false;
			}
			if (PlayerPrefs.GetInt("StartAfterDisconnect") == 1)
			{
				CountKills = GlobalGameController.CountKills;
				score = GlobalGameController.Score;
				Invoke("synchState", 1f);
			}
			else
			{
				CountKills = -1;
				score = -1;
				Invoke("synchState", 1f);
			}
			expController = ExperienceController.sharedController;
			SetNewNick();
			SetRanks();
			SynhCountKills();
			SynhScore();
			sendMySkin();
			ShopNGUIController.sharedShop.onEquipSkinAction = delegate
			{
				sendMySkin();
			};
		}
		else
		{
			showTable = false;
		}
		val.Stop();
	}

	private void GetMyTeam()
	{
		if (isMine && !LocalOrPasswordRoom() && (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture))
		{
			myCommand = GetMyCommandOnStart();
			SynhCommand();
		}
	}

	[RPC]
	[PunRPC]
	private void SetMyClanTexture(string str, string _clanID, string _clanName, string _clanLeaderId)
	{
		try
		{
			byte[] data = Convert.FromBase64String(str);
			Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoWidth);
			texture2D.LoadImage(data);
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			myClanTexture = texture2D;
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
		myClanID = _clanID;
		myClanName = _clanName;
		myClanLeaderID = _clanLeaderId;
	}

	[RPC]
	[PunRPC]
	private void setMySkin(byte[] _skinByte)
	{
		if (photonView == null || !Defs.isMulti)
		{
			return;
		}
		Texture2D texture2D = new Texture2D(64, 32);
		texture2D.LoadImage(_skinByte);
		texture2D.filterMode = FilterMode.Point;
		texture2D.Apply();
		mySkin = texture2D;
		foreach (Player_move_c player in Initializer.players)
		{
			if (player.mySkinName.photonView.owner != null && player.mySkinName.photonView.owner.Equals(photonView.owner))
			{
				if (player.myNetworkStartTable == null)
				{
					player.setMyTamble(base.gameObject);
					continue;
				}
				player._skin = mySkin;
				player.SetTextureForBodyPlayer(player._skin);
			}
		}
	}

	[RPC]
	[PunRPC]
	private void setMySkinLocal(string str1, string str2)
	{
		byte[] data = Convert.FromBase64String(str1 + str2);
		Texture2D texture2D = new Texture2D(64, 32);
		texture2D.LoadImage(data);
		texture2D.filterMode = FilterMode.Point;
		texture2D.Apply();
		mySkin = texture2D;
		if (GetComponent<NetworkView>().isMine && WeaponManager.sharedManager.myPlayer != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.SetIDMyTable(GetComponent<NetworkView>().viewID.ToString());
		}
	}

	public void sendMySkin()
	{
		mySkin = SkinsController.currentSkinForPers;
		Texture2D texture2D = mySkin as Texture2D;
		byte[] array = texture2D.EncodeToPNG();
		if (isInet)
		{
			photonView.RPC("setMySkin", PhotonTargets.AllBuffered, array);
			return;
		}
		string text = Convert.ToBase64String(array);
		GetComponent<NetworkView>().RPC("setMySkinLocal", RPCMode.AllBuffered, text.Substring(0, text.Length / 2), text.Substring(text.Length / 2, text.Length / 2));
	}

	public void ResetCamPlayer(int _nextPrev = 0)
	{
		if (_nextPrev != 0 && Initializer.players.Count == 1)
		{
			return;
		}
		if (Initializer.players.Count > 0)
		{
			if (_nextPrev == 0)
			{
				numberPlayerCun = UnityEngine.Random.Range(0, Initializer.players.Count);
				numberPlayerCunId = Initializer.players[numberPlayerCun].mySkinName.photonView.ownerId;
			}
			if (_nextPrev == 1)
			{
				int num = 10000000;
				int num2 = Initializer.players[0].mySkinName.photonView.ownerId;
				foreach (Player_move_c player in Initializer.players)
				{
					int ownerId = player.mySkinName.photonView.ownerId;
					if (ownerId < num2)
					{
						num2 = ownerId;
					}
					if (ownerId > numberPlayerCunId && ownerId < num)
					{
						num = ownerId;
					}
				}
				if (num == 10000000)
				{
					numberPlayerCunId = num2;
				}
				else
				{
					numberPlayerCunId = num;
				}
				for (int i = 0; i < Initializer.players.Count; i++)
				{
					if (Initializer.players[i].mySkinName.photonView.ownerId == numberPlayerCunId)
					{
						numberPlayerCun = i;
						break;
					}
				}
			}
			if (_nextPrev == -1)
			{
				int num3 = -1;
				int num4 = Initializer.players[0].mySkinName.photonView.ownerId;
				foreach (Player_move_c player2 in Initializer.players)
				{
					int ownerId2 = player2.mySkinName.photonView.ownerId;
					if (ownerId2 > num4)
					{
						num4 = ownerId2;
					}
					if (ownerId2 < numberPlayerCunId)
					{
						num3 = ownerId2;
					}
				}
				if (num3 == -1)
				{
					numberPlayerCunId = num4;
				}
				else
				{
					numberPlayerCunId = num3;
				}
				for (int j = 0; j < Initializer.players.Count; j++)
				{
					if (Initializer.players[j].mySkinName.photonView.ownerId == numberPlayerCunId)
					{
						numberPlayerCun = j;
						break;
					}
				}
			}
			if (currentCamPlayer != null)
			{
				currentCamPlayer.SetActive(false);
				if (!currentPlayerMoveCVidos.isMechActive)
				{
					currentFPSPlayer.SetActive(true);
				}
				currentBodyMech.SetActive(true);
				Player_move_c.SetLayerRecursively(currentGameObjectPlayer.transform.GetChild(0).gameObject, 0);
				currentGameObjectPlayer.GetComponent<InterolationGameObject>().sglajEnabled = false;
				currentCamPlayer.transform.parent.GetComponent<ThirdPersonNetwork1>().sglajEnabledVidos = false;
				currentCamPlayer = null;
				currentFPSPlayer = null;
				currentBodyMech = null;
				currentGameObjectPlayer = null;
				currentPlayerMoveCVidos = null;
			}
			SkinName mySkinName = Initializer.players[numberPlayerCun].mySkinName;
			mySkinName.camPlayer.SetActive(true);
			playerVidosNick = mySkinName.NickName;
			playerVidosClanName = mySkinName.playerMoveC.myTable.GetComponent<NetworkStartTable>().myClanName;
			playerVidosClanTexture = mySkinName.playerMoveC.myTable.GetComponent<NetworkStartTable>().myClanTexture;
			currentPlayerMoveCVidos = mySkinName.playerMoveC;
			currentCamPlayer = mySkinName.camPlayer;
			currentFPSPlayer = mySkinName.FPSplayerObject;
			currentBodyMech = mySkinName.playerMoveC.mechBody;
			Initializer.players[numberPlayerCun].myPersonNetwork.sglajEnabledVidos = true;
			currentGameObjectPlayer = mySkinName.playerGameObject;
			currentGameObjectPlayer.GetComponent<InterolationGameObject>().sglajEnabled = true;
			currentFPSPlayer.SetActive(false);
			currentBodyMech.SetActive(false);
			NickLabelController.currentCamera = mySkinName.camPlayer.GetComponent<Camera>();
			Player_move_c.SetLayerRecursively(currentGameObjectPlayer.transform.GetChild(0).gameObject, 9);
		}
		else
		{
			_cam.SetActive(true);
			showTable = true;
			isRegimVidos = false;
			NickLabelController.currentCamera = _cam.GetComponent<Camera>();
			if (inGameGUI != null)
			{
				inGameGUI.ResetScope();
			}
		}
	}

	private int GetMyCommandOnStart()
	{
		if (myCommand > 0)
		{
			return myCommand;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < Initializer.networkTables.Count; i++)
		{
			if (Initializer.networkTables[i].myCommand == 1)
			{
				num++;
			}
			if (Initializer.networkTables[i].myCommand == 2)
			{
				num2++;
			}
		}
		if (num2 < num)
		{
			return 2;
		}
		if (num2 > num)
		{
			return 1;
		}
		float num3 = ((!FriendsController.useBuffSystem) ? KillRateCheck.instance.GetKillRate() : BuffSystem.instance.GetKillrateByInteractions());
		int winningTeam = GetWinningTeam();
		if (winningTeam == 0)
		{
			return UnityEngine.Random.Range(1, 3);
		}
		if (num3 < 1f)
		{
			return winningTeam;
		}
		return (winningTeam != 1) ? 1 : 2;
	}

	private void ReplaceCommand()
	{
		myCommand = ((myCommand != 1) ? 1 : 2);
		SynhCommand();
		score = 0;
		CountKills = 0;
		GlobalGameController.Score = 0;
		GlobalGameController.CountKills = 0;
		if (WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.countKills = 0;
			WeaponManager.sharedManager.myPlayerMoveC.myCommand = myCommand;
			WeaponManager.sharedManager.myPlayerMoveC.myBaza = null;
			WeaponManager.sharedManager.myPlayerMoveC.myFlag = null;
			WeaponManager.sharedManager.myPlayerMoveC.enemyFlag = null;
			if (Initializer.redPlayers.Contains(WeaponManager.sharedManager.myPlayerMoveC) && myCommand == 1)
			{
				Initializer.redPlayers.Remove(WeaponManager.sharedManager.myPlayerMoveC);
			}
			if (Initializer.bluePlayers.Contains(WeaponManager.sharedManager.myPlayerMoveC) && myCommand == 2)
			{
				Initializer.bluePlayers.Remove(WeaponManager.sharedManager.myPlayerMoveC);
			}
			if (myCommand == 1 && !Initializer.bluePlayers.Contains(WeaponManager.sharedManager.myPlayerMoveC))
			{
				Initializer.bluePlayers.Add(WeaponManager.sharedManager.myPlayerMoveC);
			}
			if (myCommand == 2 && !Initializer.redPlayers.Contains(WeaponManager.sharedManager.myPlayerMoveC))
			{
				Initializer.redPlayers.Add(WeaponManager.sharedManager.myPlayerMoveC);
			}
		}
	}

	private void Update()
	{
		if (isMine)
		{
			if (inGameGUI == null)
			{
				inGameGUI = InGameGUI.sharedInGameGUI;
			}
			if (timerSynchScore > 0f)
			{
				timerSynchScore -= Time.deltaTime;
				if (timerSynchScore < 0f)
				{
					SendSynhScore();
				}
			}
			bool flag = isShowNickTable || showDisconnectFromServer || showDisconnectFromMasterServer || showTable || showMessagFacebook;
			if (guiObj.activeSelf != flag)
			{
				guiObj.SetActive(flag);
			}
			if (inGameGUI == null)
			{
				inGameGUI = InGameGUI.sharedInGameGUI;
			}
			if (_pauser == null)
			{
				_pauser = Pauser.sharedPauser;
			}
			if (ShopNGUIController.GuiActive || (ProfileController.Instance != null && ProfileController.Instance.InterfaceEnabled))
			{
				expController.isShowRanks = SkinEditorController.sharedController == null && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled);
			}
			else if (BankController.Instance != null && BankController.Instance.InterfaceEnabled)
			{
				expController.isShowRanks = false;
			}
			else if (_pauser != null && _pauser.paused)
			{
				if (_pauseController == null)
				{
					_pauseController = UnityEngine.Object.FindObjectOfType<PauseNGUIController>();
				}
				if (_pauseController != null)
				{
					expController.isShowRanks = !_pauseController.SettingsJoysticksPanel.activeInHierarchy;
				}
			}
			else if ((showTable || isShowNickTable) && !isRegimVidos && _shopInstance == null && !LoadingInAfterGame.isShowLoading && !isGoRandomRoom)
			{
				expController.isShowRanks = !isShowFinished && (!(networkStartTableNGUIController != null) || networkStartTableNGUIController.rentScreenPoint.childCount == 0);
			}
			else
			{
				expController.isShowRanks = false;
			}
			if (isRegimVidos && isDeadInHungerGame && _cam.activeInHierarchy && Initializer.players.Count > 0)
			{
				_cam.SetActive(false);
				ResetCamPlayer();
			}
			if (isRegimVidos && isDeadInHungerGame && currentCamPlayer == null)
			{
				ResetCamPlayer();
			}
			if (!isRegimVidos && isDeadInHungerGame && currentCamPlayer != null)
			{
				currentCamPlayer.SetActive(false);
				if (!currentPlayerMoveCVidos.isMechActive)
				{
					currentFPSPlayer.SetActive(true);
				}
				currentBodyMech.SetActive(true);
				currentCamPlayer = null;
				currentFPSPlayer = null;
				currentBodyMech = null;
				_cam.SetActive(true);
			}
			if (isRegimVidos && inGameGUI != null && currentPlayerMoveCVidos.isZooming != oldIsZomming)
			{
				oldIsZomming = currentPlayerMoveCVidos.isZooming;
				if (oldIsZomming)
				{
					string empty = string.Empty;
					float fieldOfView = 60f;
					if (currentGameObjectPlayer.transform.childCount > 0)
					{
						try
						{
							empty = ItemDb.GetByPrefabName(currentGameObjectPlayer.transform.GetChild(0).name.Replace("(Clone)", string.Empty)).Tag;
						}
						catch (Exception ex)
						{
							if (Application.isEditor)
							{
								Debug.LogWarning("Exception  tagWeapon = ItemDb.GetByPrefabName(currentGameObjectPlayer.transform.GetChild(0).name.Replace(\"(Clone)\",\"\")).Tag:  " + ex);
							}
						}
						fieldOfView = currentGameObjectPlayer.transform.GetChild(0).GetComponent<WeaponSounds>().fieldOfViewZomm;
					}
					if (!empty.Equals(string.Empty))
					{
						inGameGUI.SetScopeForWeapon(string.Empty + currentGameObjectPlayer.transform.GetChild(0).GetComponent<WeaponSounds>().scopeNum);
					}
					currentPlayerMoveCVidos.myCamera.fieldOfView = fieldOfView;
					currentPlayerMoveCVidos.gunCamera.fieldOfView = 1f;
				}
				else
				{
					currentPlayerMoveCVidos.myCamera.fieldOfView = 44f;
					currentPlayerMoveCVidos.gunCamera.fieldOfView = 75f;
					inGameGUI.ResetScope();
				}
			}
			if (Defs.isFlag || Defs.isCompany || Defs.isCapturePoints)
			{
				if (Defs.isInet && myCommand > 0)
				{
					int num = 0;
					for (int i = 0; i < Initializer.networkTables.Count; i++)
					{
						if (Initializer.networkTables[i] != null && Initializer.networkTables[i].myCommand == myCommand)
						{
							num++;
						}
					}
					if (num > 5)
					{
						int num2 = -1;
						for (int j = 0; j < Initializer.networkTables.Count; j++)
						{
							if (Initializer.networkTables[j] != null && Initializer.networkTables[j].myCommand == myCommand && Initializer.networkTables[j].photonView.ownerId > num2)
							{
								num2 = Initializer.networkTables[j].photonView.ownerId;
							}
						}
						if (num2 == photonView.ownerId)
						{
							ReplaceCommand();
						}
					}
				}
				if (Defs.isFlag)
				{
					timerFlag = TimeGameController.sharedController.timerToEndMatch;
					if (timerFlag < 0.0)
					{
						timerFlag = 0.0;
					}
					if (timerFlag < 0.10000000149011612)
					{
						if (WeaponManager.sharedManager.myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC.enabled)
						{
							WeaponManager.sharedManager.myPlayerMoveC.enabled = false;
							InGameGUI.sharedInGameGUI.gameObject.SetActive(false);
							Invoke("ClearScoreCommandInFlagGame", 0.5f);
							ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
							hashtable["TimeMatchEnd"] = -9000000.0;
							PhotonNetwork.room.SetCustomProperties(hashtable);
							if (scoreCommandFlag1 > scoreCommandFlag2)
							{
								win(string.Empty, 1, scoreCommandFlag1, scoreCommandFlag2);
							}
							else if (scoreCommandFlag1 < scoreCommandFlag2)
							{
								win(string.Empty, 2, scoreCommandFlag1, scoreCommandFlag2);
							}
							else
							{
								win(string.Empty, 0, scoreCommandFlag1, scoreCommandFlag2);
							}
						}
					}
					else if (inGameGUI != null && inGameGUI.message_draw.activeSelf)
					{
						inGameGUI.message_draw.SetActive(false);
					}
				}
			}
			if (isHunger && hungerGameController != null && hungerGameController.isStartGame && !hungerGameController.isRunPlayer && !isEndInHunger)
			{
				Debug.Log("Start hunger player");
				hungerGameController.isRunPlayer = true;
				isShowNickTable = false;
				CountKills = 0;
				score = 0;
				GlobalGameController.Score = 0;
				isDrawInHanger = false;
				startPlayer();
				countMigZagolovok = 0;
				timeTomig = 0.7f;
				isMigZag = false;
				SynhCountKills();
				SynhScore();
				return;
			}
			if (isHunger && hungerGameController != null && !hungerGameController.isStartGame)
			{
				string text = string.Empty;
				if (!hungerGameController.isStartTimer)
				{
					text = waitingPlayerLocalize;
				}
				else
				{
					if (hungerGameController.startTimer > 0f && !hungerGameController.isStartGame)
					{
						float startTimer = hungerGameController.startTimer;
						text = matchLocalize + " " + Mathf.FloorToInt(startTimer / 60f) + ":" + ((Mathf.FloorToInt(startTimer - (float)(Mathf.FloorToInt(startTimer / 60f) * 60)) >= 10) ? string.Empty : "0") + Mathf.FloorToInt(startTimer - (float)(Mathf.FloorToInt(startTimer / 60f) * 60));
					}
					if (hungerGameController.startTimer < 0f && !hungerGameController.isStartGame)
					{
						text = preparingLocalize;
					}
				}
				if (NetworkStartTableNGUIController.sharedController != null)
				{
					NetworkStartTableNGUIController.sharedController.HungerStartLabel.text = text;
				}
			}
			if (Defs.isFlag && isInet && PhotonNetwork.isMasterClient && Initializer.flag1 == null)
			{
				AddFlag();
			}
		}
		if (!isLocal && isMine)
		{
			GlobalGameController.showTableMyPlayer = showTable;
			GlobalGameController.imDeadInHungerGame = isDeadInHungerGame;
		}
		if (isLocal && isServer && lanScan != null)
		{
			lanScan.serverMessage.connectedPlayers = GameObject.FindGameObjectsWithTag("NetworkTable").Length;
		}
		if (timerShow >= 0f)
		{
			timerShow -= Time.deltaTime;
			if (timerShow < 0f)
			{
				ActivityIndicator.IsActiveIndicator = false;
				ConnectSceneNGUIController.Local();
			}
		}
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		if (GetComponent<NetworkView>().isMine)
		{
			SynhCommand();
			SynhCountKills();
			SynhScore();
		}
	}

	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Debug.Log("OnDisconnectedFromServer");
		showDisconnectFromServer = true;
		timerShow = 3f;
	}

	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		foreach (Player_move_c player2 in Initializer.players)
		{
			if (!player.ipAddress.Equals(player2.myIp) || !(NickLabelStack.sharedStack != null))
			{
				continue;
			}
			NickLabelController[] lables = NickLabelStack.sharedStack.lables;
			NickLabelController[] array = lables;
			foreach (NickLabelController nickLabelController in array)
			{
				if (nickLabelController.target == player2.transform)
				{
					nickLabelController.target = null;
					break;
				}
			}
			UnityEngine.Object.Destroy(player2.mySkinName.gameObject);
		}
	}

	private void OnFailedToConnectToMasterServer(NetworkConnectionError info)
	{
		Debug.Log("Could not connect to master server: " + info);
		showDisconnectFromMasterServer = true;
		timerShow = 3f;
	}

	public void WinInHunger()
	{
		isIwin = true;
		photonView.RPC("winInHungerRPC", PhotonTargets.AllBuffered, NamePlayer);
	}

	public IEnumerator DrawInHanger()
	{
		while (ShopNGUIController.GuiActive || BankController.Instance.uiRoot.gameObject.activeInHierarchy || (_pauser != null && _pauser.paused))
		{
			yield return null;
		}
		isEndInHunger = true;
		isDrawInHanger = true;
		showTable = true;
		if (_cam != null)
		{
			_cam.SetActive(true);
		}
		if (!isSetNewMapButton)
		{
			isSetNewMapButton = true;
			NetworkStartTableNGUIController.sharedController.UpdateGoMapButtons();
		}
		NickLabelController.currentCamera = Initializer.Instance.tc.GetComponent<Camera>();
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			UnityEngine.Object.Destroy(Initializer.players[i].mySkinName.gameObject);
		}
		if (NetworkStartTableNGUIController.sharedController != null)
		{
			NetworkStartTableNGUIController.sharedController.ShowEndInterface("Time's out!", 0);
		}
	}

	[RPC]
	[PunRPC]
	public void DrawInHangerRPC()
	{
	}

	[RPC]
	[PunRPC]
	public void winInHungerRPC(string winner)
	{
		isEndInHunger = true;
		if (_weaponManager != null && _weaponManager.myTable != null)
		{
			_weaponManager.myTable.GetComponent<NetworkStartTable>().win(winner);
		}
	}

	public static void IncreaseTimeInMode(int mode, double minutes)
	{
		if (!(ExperienceController.sharedController != null))
		{
			return;
		}
		string key = mode.ToString();
		string key2 = "Statistics.TimeInMode.Level" + ExperienceController.sharedController.currentLevel;
		if (PlayerPrefs.HasKey(key2))
		{
			string @string = PlayerPrefs.GetString(key2, "{}");
			Debug.Log("Time in mode string:    " + @string);
			try
			{
				Dictionary<string, object> dictionary = (Rilisoft.MiniJson.Json.Deserialize(@string) as Dictionary<string, object>) ?? new Dictionary<string, object>();
				object value;
				if (dictionary.TryGetValue(key, out value))
				{
					double num = Convert.ToDouble(value) + minutes;
					dictionary[key] = num;
				}
				else
				{
					dictionary.Add(key, minutes);
				}
				string value2 = Rilisoft.MiniJson.Json.Serialize(dictionary);
				PlayerPrefs.SetString(key2, value2);
			}
			catch (OverflowException exception)
			{
				Debug.LogError("Cannot deserialize time-in-mode:    " + @string);
				Debug.LogException(exception);
			}
			catch (Exception exception2)
			{
				Debug.LogError("Unknown exception:    " + @string);
				Debug.LogException(exception2);
			}
		}
		string key3 = "Statistics.RoundsInMode.Level" + ExperienceController.sharedController.currentLevel;
		if (PlayerPrefs.HasKey(key3))
		{
			string string2 = PlayerPrefs.GetString(key3);
			Dictionary<string, object> dictionary2 = (Rilisoft.MiniJson.Json.Deserialize(string2) as Dictionary<string, object>) ?? new Dictionary<string, object>();
			object value3;
			if (dictionary2.TryGetValue(key, out value3))
			{
				int num2 = Convert.ToInt32(value3) + 1;
				dictionary2[key] = num2;
			}
			else
			{
				dictionary2.Add(key, 1);
			}
			string value4 = Rilisoft.MiniJson.Json.Serialize(dictionary2);
			PlayerPrefs.SetString(key3, value4);
		}
		PlayerPrefs.Save();
	}

	private IEnumerator WaitInterstitialRequestAndShowCoroutine(System.Threading.Tasks.Task<Ad> request)
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Waiting until interstitial request is completed...");
		}
		while (!((System.Threading.Tasks.Task)request).get_IsCompleted())
		{
			yield return null;
		}
		if (((System.Threading.Tasks.Task)request).get_IsFaulted())
		{
			Debug.LogWarning("Interstitial request after match failed: " + ((Exception)(object)((System.Threading.Tasks.Task)request).get_Exception()).InnerException.Message);
			yield break;
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Interstitial request after match succeeded. Trying to show interstitial...");
		}
		yield return null;
		if (WeaponManager.sharedManager.myPlayer != null)
		{
			Debug.LogWarning("Stop waiting: WeaponManager.sharedManager.myPlayer != null");
			yield break;
		}
		yield return null;
		if (NetworkStartTableNGUIController.sharedController.rewardWindow != null)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Waiting until Reward panel is closed...");
			}
			while (NetworkStartTableNGUIController.sharedController.isRewardShow)
			{
				yield return null;
			}
			yield return null;
			while (ExpController.Instance.WaitingForLevelUpView)
			{
				yield return null;
			}
			yield return null;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log(string.Format("Waiting until Level up panel is closed if displayed ({0})...", ExpController.Instance.IsLevelUpShown));
			}
			while (ExpController.Instance.IsLevelUpShown)
			{
				yield return null;
			}
		}
		while (ShopNGUIController.GuiActive)
		{
			yield return null;
		}
		Dictionary<string, string> attributes = new Dictionary<string, string>
		{
			{ "af_content_type", "Interstitial" },
			{ "af_content_id", "Interstitial (NetworkTable)" }
		};
		FlurryPluginWrapper.LogEventToAppsFlyer("af_content_view", attributes);
		System.Threading.Tasks.Task<AdResult> future = FyberFacade.Instance.ShowInterstitial(new Dictionary<string, string> { { "Context", "Multiplayer Table" } }, "NetworkStartTable.WaitInterstitialRequestAndShow()");
		while (!((System.Threading.Tasks.Task)future).get_IsCompleted())
		{
			yield return null;
		}
		if (((System.Threading.Tasks.Task)future).get_IsFaulted())
		{
			Debug.LogWarningFormat("Interstitial show after match failed: {0}", ((Exception)(object)((System.Threading.Tasks.Task)future).get_Exception()).InnerException.Message);
		}
		else
		{
			Debug.LogFormat("Interstitial show finished with status {0}: {1}", future.get_Result().Status, future.get_Result().Message);
		}
	}

	private IEnumerator LockInterfaceCoroutine()
	{
		try
		{
			float startTime = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup - startTime < PromoActionsManager.MobileAdvert.TimeoutWaitingInterstitialAfterMatchSeconds)
			{
				yield return null;
			}
		}
		finally
		{
		}
	}

	public static bool LocalOrPasswordRoom()
	{
		return !Defs.isInet || (PhotonNetwork.room != null && !PhotonNetwork.room.customProperties[ConnectSceneNGUIController.passwordProperty].Equals(string.Empty));
	}

	private bool CheckForWin(int myPlace, int winnerTeam, int killCount, int myscore, bool scoreMatterForTeam = true)
	{
		switch (ConnectSceneNGUIController.regim)
		{
		case ConnectSceneNGUIController.RegimGame.Deathmatch:
		case ConnectSceneNGUIController.RegimGame.TimeBattle:
			return myPlace == 0 && myscore > 0;
		case ConnectSceneNGUIController.RegimGame.TeamFight:
		case ConnectSceneNGUIController.RegimGame.FlagCapture:
		case ConnectSceneNGUIController.RegimGame.CapturePoints:
			return myCommand == winnerTeam && (myscore > 0 || !scoreMatterForTeam);
		case ConnectSceneNGUIController.RegimGame.DeadlyGames:
			return killCount > 0 && isIwin;
		default:
			return false;
		}
	}

	public void win(string winner, int _commandWin = 0, int blueCount = 0, int redCount = 0)
	{
		if (NetworkStartTableNGUIController.sharedController.isRewardShow || isShowFinished)
		{
			return;
		}
		QuestSystem.Instance.SaveQuestProgressIfDirty();
		if (Defs.isInet)
		{
			PhotonNetwork.FetchServerTimestamp();
		}
		_matchStopwatch.Stop();
		double totalMinutes = _matchStopwatch.get_Elapsed().TotalMinutes;
		if (Defs.isHunger)
		{
			isEndInHunger = true;
		}
		if (Defs.isDaterRegim)
		{
			Storager.setInt("DaterDayLived", Storager.getInt("DaterDayLived", false) + 1, false);
		}
		if (Defs.isDaterRegim)
		{
			int num = 5;
			num = ((!Defs.isInet) ? (PlayerPrefs.GetString("MaxKill", "9").Equals(string.Empty) ? 5 : int.Parse(PlayerPrefs.GetString("MaxKill", "5"))) : ((int)PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty]));
			AnalyticsStuff.LogSandboxTimeGamePopularity(num, false);
		}
		StoreKitEventListener.State.PurchaseKey = "End match";
		if (!Defs.isHunger)
		{
			int @int = PlayerPrefs.GetInt("CountMatch", 0);
			PlayerPrefs.SetInt("CountMatch", @int + 1);
			if (ExperienceController.sharedController != null)
			{
				string key = "Statistics.MatchCount.Level" + ExperienceController.sharedController.currentLevel;
				int int2 = PlayerPrefs.GetInt(key, 0);
				PlayerPrefs.SetInt(key, int2 + 1);
				FlurryPluginWrapper.LogMatchCompleted(ConnectSceneNGUIController.regim.ToString());
			}
			IncreaseTimeInMode((int)ConnectSceneNGUIController.regim, _matchStopwatch.get_Elapsed().TotalMinutes);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("action", "time_in_match");
			wWWForm.AddField("mode", (int)ConnectSceneNGUIController.regim);
			wWWForm.AddField("time", _matchStopwatch.get_Elapsed().TotalSeconds.ToString());
			wWWForm.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
			wWWForm.AddField("uniq_id", FriendsController.sharedController.id);
			wWWForm.AddField("auth", FriendsController.Hash("time_in_match"));
			if (ExperienceController.sharedController != null)
			{
				wWWForm.AddField("level", ExperienceController.sharedController.currentLevel);
			}
			wWWForm.AddField("paying", Convert.ToInt32(FlurryPluginWrapper.IsPayingUser()).ToString());
			wWWForm.AddField("developer", Convert.ToInt32(Defs.IsDeveloperBuild).ToString());
			Debug.Log("Time in Match Event: " + Encoding.UTF8.GetString(wWWForm.data, 0, wWWForm.data.Length));
			WWW wWW = Tools.CreateWww(URLs.Friends, wWWForm, string.Empty);
			_matchStopwatch.Reset();
		}
		isShowAvard = false;
		commandWinner = _commandWin;
		GlobalGameController.countKillsBlue = 0;
		GlobalGameController.countKillsRed = 0;
		nickPobeditelya = winner;
		GameObject[] array = GameObject.FindGameObjectsWithTag("NetworkTable");
		List<GameObject> list = new List<GameObject>();
		List<GameObject> list2 = new List<GameObject>();
		List<GameObject> list3 = new List<GameObject>();
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.Deathmatch)
		{
			isDrawInDeathMatch = true;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetComponent<NetworkStartTable>().score >= AdminSettingsController.minScoreDeathMath)
				{
					isDrawInDeathMatch = false;
				}
			}
		}
		for (int j = 1; j < array.Length; j++)
		{
			NetworkStartTable component = array[j].GetComponent<NetworkStartTable>();
			for (int k = 0; k < j; k++)
			{
				NetworkStartTable component2 = array[k].GetComponent<NetworkStartTable>();
				if ((!Defs.isFlag && !Defs.isCapturePoints && (component.score > component2.score || (component.score == component2.score && component.CountKills > component2.CountKills))) || ((Defs.isFlag || Defs.isCapturePoints) && (component.CountKills > component2.CountKills || (component.CountKills == component2.CountKills && component.score > component2.score))))
				{
					GameObject gameObject = array[j];
					for (int num2 = j - 1; num2 >= k; num2--)
					{
						array[num2 + 1] = array[num2];
					}
					array[k] = gameObject;
					break;
				}
			}
		}
		int num3 = 0;
		for (int l = 0; l < array.Length; l++)
		{
			int num4 = array[l].GetComponent<NetworkStartTable>().myCommand;
			if (num4 == -1)
			{
				num4 = array[l].GetComponent<NetworkStartTable>().myCommandOld;
			}
			if (num4 == 0)
			{
				if (array[l].Equals(base.gameObject))
				{
					num3 = list3.Count;
				}
				list3.Add(array[l]);
			}
			if (num4 == 1)
			{
				if (array[l].Equals(base.gameObject))
				{
					num3 = list.Count;
				}
				list.Add(array[l]);
			}
			if (num4 == 2)
			{
				if (array[l].Equals(base.gameObject))
				{
					num3 = list2.Count;
				}
				list2.Add(array[l]);
			}
		}
		oldSpisokName = new string[list3.Count];
		oldScoreSpisok = new string[list3.Count];
		oldCountLilsSpisok = new string[list3.Count];
		oldSpisokRanks = new int[list3.Count];
		oldIsDeadInHungerGame = new bool[list3.Count];
		oldSpisokPixelBookID = new string[list3.Count];
		oldSpisokMyClanLogo = new Texture[list3.Count];
		oldSpisokNameBlue = new string[list.Count];
		oldCountLilsSpisokBlue = new string[list.Count];
		oldSpisokRanksBlue = new int[list.Count];
		oldSpisokPixelBookIDBlue = new string[list.Count];
		oldSpisokMyClanLogoBlue = new Texture[list.Count];
		oldScoreSpisokBlue = new string[list.Count];
		oldSpisokNameRed = new string[list2.Count];
		oldCountLilsSpisokRed = new string[list2.Count];
		oldSpisokRanksRed = new int[list2.Count];
		oldSpisokPixelBookIDRed = new string[list2.Count];
		oldSpisokMyClanLogoRed = new Texture[list2.Count];
		oldScoreSpisokRed = new string[list2.Count];
		addCoins = 0;
		addExperience = 0;
		bool flag = CheckForWin(num3, _commandWin, CountKills, score);
		bool iAmWinnerInTeam = CheckForWin(num3, _commandWin, CountKills, score, false);
		KillRateCheck.instance.LogFirstBattlesResult(flag);
		RatingSystem.RatingChange ratingChange = CalculateMatchRating(false);
		if (isInet)
		{
			if (Defs.abTestBalansCohort == Defs.ABTestCohortsType.B || myCommand == _commandWin || (!Defs.isCompany && !Defs.isFlag && !Defs.isCapturePoints) || ExperienceController.sharedController.currentLevel < 2)
			{
				if (Defs.abTestBalansCohort == Defs.ABTestCohortsType.B && (Defs.isCompany || Defs.isFlag || Defs.isCapturePoints))
				{
					isIwin = myCommand == _commandWin;
				}
				int timeGame = int.Parse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty].ToString());
				AdminSettingsController.Avard avardAfterMatch = AdminSettingsController.GetAvardAfterMatch(ConnectSceneNGUIController.regim, timeGame, num3, score, CountKills, isIwin);
				addCoins = avardAfterMatch.coin;
				addExperience = avardAfterMatch.expierense;
			}
			if (!TrainingController.TrainingCompleted)
			{
				AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Deathmatch_Completed, myCommand == _commandWin);
			}
			bool flag2 = totalMinutes > PromoActionsManager.MobileAdvert.MinMatchDurationMinutes;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log(string.Format("[Rilisoft] Match duration: {0:F2}, threshold: {1:F2}", totalMinutes, PromoActionsManager.MobileAdvert.MinMatchDurationMinutes));
			}
			if (isMine && !ShopNGUIController.GuiActive && flag2 && MobileAdManager.AdIsApplicable(MobileAdManager.Type.Image, true))
			{
				if (flag)
				{
					if (PromoActionsManager.MobileAdvert.ShowInterstitialAfterMatchWinner)
					{
						System.Threading.Tasks.Task<Ad> task = FyberFacade.Instance.RequestImageInterstitial("NetworkStartTable.win(Winner)");
						FyberFacade.Instance.Requests.AddLast(task);
						StartCoroutine(WaitInterstitialRequestAndShowCoroutine(task));
						StartCoroutine(LockInterfaceCoroutine());
					}
					else if (Defs.IsDeveloperBuild)
					{
						Debug.Log("showInterstitialAfterMatchWinner: false");
					}
				}
				else if (PromoActionsManager.MobileAdvert.ShowInterstitialAfterMatchLoser)
				{
					System.Threading.Tasks.Task<Ad> task2 = FyberFacade.Instance.RequestImageInterstitial("NetworkStartTable.win(Loser)");
					FyberFacade.Instance.Requests.AddLast(task2);
					StartCoroutine(WaitInterstitialRequestAndShowCoroutine(task2));
					StartCoroutine(LockInterfaceCoroutine());
				}
				else if (Defs.IsDeveloperBuild)
				{
					Debug.Log("showInterstitialAfterMatchLoser: false");
				}
			}
			if (WeaponManager.sharedManager != null)
			{
				WeaponManager.sharedManager.DecreaseTryGunsMatchesCount();
			}
			if (flag)
			{
				if (!LocalOrPasswordRoom())
				{
					QuestMediator.NotifyWin(ConnectSceneNGUIController.regim, Application.loadedLevelName);
				}
				if (Defs.isFlag)
				{
					int val = Storager.getInt(Defs.RatingFlag, false) + 1;
					Storager.setInt(Defs.RatingFlag, val, false);
				}
				if (Defs.isCompany)
				{
					int val2 = Storager.getInt(Defs.RatingTeamBattle, false) + 1;
					Storager.setInt(Defs.RatingTeamBattle, val2, false);
				}
				if (Defs.isCapturePoints)
				{
					int val3 = Storager.getInt(Defs.RatingCapturePoint, false) + 1;
					Storager.setInt(Defs.RatingCapturePoint, val3, false);
				}
				if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.Deathmatch)
				{
					int val4 = Storager.getInt(Defs.RatingDeathmatch, false) + 1;
					Storager.setInt(Defs.RatingDeathmatch, val4, false);
				}
				if (ExperienceController.sharedController != null)
				{
					string key2 = "Statistics.WinCount.Level" + ExperienceController.sharedController.currentLevel;
					int int3 = PlayerPrefs.GetInt(key2, 0);
					PlayerPrefs.SetInt(key2, int3 + 1);
					FlurryPluginWrapper.LogWinInMatch(ConnectSceneNGUIController.regim.ToString());
				}
				if (!Defs.isCOOP)
				{
					FriendsController.sharedController.SendRoundWon();
					if (PlayerPrefs.GetInt("LogCountMatch", 0) == 1)
					{
						PlayerPrefs.SetInt("LogCountMatch", 0);
						int int4 = PlayerPrefs.GetInt("CountMatch", 0);
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("Count matchs", int4.ToString());
						Dictionary<string, string> parameters = dictionary;
						FlurryPluginWrapper.LogEventAndDublicateToConsole("First WIN in Multiplayer", parameters);
						if (Social.localUser.authenticated)
						{
							string achievementID = "CgkIr8rGkPIJEAIQAg";
							Social.ReportProgress(achievementID, 100.0, delegate(bool success)
							{
								Debug.Log("Achievement First Win completed: " + success);
							});
						}
					}
				}
			}
			if (addCoins > 0 || (ExperienceController.sharedController.currentLevel < 31 && addExperience > 0))
			{
				isShowAvard = true;
				if (PhotonNetwork.room != null && !PhotonNetwork.room.customProperties[ConnectSceneNGUIController.passwordProperty].Equals(string.Empty))
				{
					addCoins = 0;
					addExperience = 0;
					isShowAvard = false;
				}
			}
		}
		bool flag3 = false;
		int num5 = 0;
		NetworkStartTable networkStartTable = null;
		for (int m = 0; m < list3.Count; m++)
		{
			if ((bool)_weaponManager && list3[m].Equals(_weaponManager.myTable))
			{
				oldIndexMy = m;
			}
			NetworkStartTable component3 = list3[m].GetComponent<NetworkStartTable>();
			oldSpisokName[m] = component3.NamePlayer;
			oldSpisokRanks[m] = component3.myRanks;
			oldSpisokPixelBookID[m] = component3.pixelBookID;
			oldSpisokMyClanLogo[m] = component3.myClanTexture;
			oldScoreSpisok[m] = ((component3.score == -1) ? component3.scoreOld.ToString() : component3.score.ToString());
			int num6 = ((component3.CountKills == -1) ? component3.oldCountKills : component3.CountKills);
			oldCountLilsSpisok[m] = num6.ToString();
			oldIsDeadInHungerGame[m] = component3.isDeadInHungerGame;
			if (Defs.isDaterRegim)
			{
				if (num6 > num5)
				{
					networkStartTable = component3;
					flag3 = false;
					num5 = num6;
				}
				else if (num6 > 0 && num6 == num5)
				{
					flag3 = true;
				}
			}
		}
		for (int n = 0; n < list.Count; n++)
		{
			if ((bool)_weaponManager && list[n].Equals(_weaponManager.myTable))
			{
				oldIndexMy = n;
			}
			oldSpisokNameBlue[n] = list[n].GetComponent<NetworkStartTable>().NamePlayer;
			oldSpisokRanksBlue[n] = list[n].GetComponent<NetworkStartTable>().myRanks;
			oldSpisokPixelBookIDBlue[n] = list[n].GetComponent<NetworkStartTable>().pixelBookID;
			oldSpisokMyClanLogoBlue[n] = list[n].GetComponent<NetworkStartTable>().myClanTexture;
			oldScoreSpisokBlue[n] = ((list[n].GetComponent<NetworkStartTable>().score == -1) ? (string.Empty + list[n].GetComponent<NetworkStartTable>().scoreOld) : (string.Empty + list[n].GetComponent<NetworkStartTable>().score));
			oldCountLilsSpisokBlue[n] = ((list[n].GetComponent<NetworkStartTable>().CountKills == -1) ? (string.Empty + list[n].GetComponent<NetworkStartTable>().oldCountKills) : (string.Empty + list[n].GetComponent<NetworkStartTable>().CountKills));
		}
		for (int num7 = 0; num7 < list2.Count; num7++)
		{
			if ((bool)_weaponManager && list2[num7].Equals(_weaponManager.myTable))
			{
				oldIndexMy = num7;
			}
			oldSpisokNameRed[num7] = list2[num7].GetComponent<NetworkStartTable>().NamePlayer;
			oldSpisokRanksRed[num7] = list2[num7].GetComponent<NetworkStartTable>().myRanks;
			oldSpisokPixelBookIDRed[num7] = list2[num7].GetComponent<NetworkStartTable>().pixelBookID;
			oldSpisokMyClanLogoRed[num7] = list2[num7].GetComponent<NetworkStartTable>().myClanTexture;
			oldScoreSpisokRed[num7] = ((list2[num7].GetComponent<NetworkStartTable>().score == -1) ? (string.Empty + list2[num7].GetComponent<NetworkStartTable>().scoreOld) : (string.Empty + list2[num7].GetComponent<NetworkStartTable>().score));
			oldCountLilsSpisokRed[num7] = ((list2[num7].GetComponent<NetworkStartTable>().CountKills == -1) ? (string.Empty + list2[num7].GetComponent<NetworkStartTable>().oldCountKills) : (string.Empty + list2[num7].GetComponent<NetworkStartTable>().CountKills));
		}
		myCommandOld = myCommand;
		oldCountKills = CountKills;
		scoreOld = score;
		score = -1;
		GlobalGameController.Score = -1;
		scoreCommandFlag1 = 0;
		scoreCommandFlag2 = 0;
		CountKills = -1;
		if (isCompany || Defs.isFlag || Defs.isCapturePoints)
		{
			myCommand = -1;
		}
		SynhCommand();
		SynhCountKills();
		SynhScore();
		if (WeaponManager.sharedManager.myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC.showRanks)
		{
			NetworkStartTableNGUIController.sharedController.BackPressFromRanksTable();
		}
		GameObject gameObject2 = GameObject.FindGameObjectWithTag("DamageFrame");
		if (gameObject2 != null)
		{
			UnityEngine.Object.Destroy(gameObject2);
		}
		int winnerCommand = 0;
		string winner2;
		if (Defs.isDaterRegim)
		{
			winner2 = ((!(networkStartTable != null)) ? LocalizationStore.Get("Key_1427") : (flag3 ? LocalizationStore.Get("Key_1764") : ((!networkStartTable.Equals(this)) ? string.Format(LocalizationStore.Get("Key_1763"), networkStartTable.NamePlayer) : LocalizationStore.Get("Key_1762"))));
		}
		else if (isCompany || Defs.isFlag || Defs.isCapturePoints)
		{
			string key_ = LocalizationStore.Key_0571;
			winner2 = ((commandWinner == 0) ? key_ : ((commandWinner != myCommandOld) ? LocalizationStore.Get("Key_1794") : LocalizationStore.Get("Key_1793")));
			winnerCommand = ((commandWinner != 0) ? ((commandWinner == myCommandOld) ? 1 : 2) : 0);
		}
		else
		{
			winner2 = (((isHunger && isDrawInHanger) || isDrawInDeathMatch) ? LocalizationStore.Key_0568 : ((!flag) ? LocalizationStore.Get("Key_1116") : LocalizationStore.Get("Key_1115")));
		}
		isShowFinished = true;
		if (NetworkStartTableNGUIController.sharedController != null)
		{
			if (!Defs.isDaterRegim && Defs.isInet && !isSetNewMapButton)
			{
				NetworkStartTableNGUIController.sharedController.UpdateGoMapButtons();
			}
			if (!Defs.isHunger || !isDeadInHungerGame)
			{
				StartCoroutine(NetworkStartTableNGUIController.sharedController.MatchFinishedInterface(winner2, ratingChange, isShowAvard, addCoins, addExperience, LocalOrPasswordRoom(), (!Defs.isHunger) ? (num3 == 0) : isIwin, iAmWinnerInTeam, winnerCommand, blueCount, redCount));
			}
			else
			{
				NetworkStartTableNGUIController.sharedController.MathFinishedDeadInHunger();
			}
		}
		isShowAvard = false;
		showTable = false;
		isShowNickTable = true;
	}

	public int GetPlaceInTable()
	{
		int result = 0;
		NetworkStartTable[] array = Initializer.networkTables.ToArray();
		for (int i = 1; i < array.Length; i++)
		{
			NetworkStartTable networkStartTable = array[i];
			for (int j = 0; j < i; j++)
			{
				NetworkStartTable networkStartTable2 = array[j];
				if ((!Defs.isFlag && !Defs.isCapturePoints && (networkStartTable.score > networkStartTable2.score || (networkStartTable.score == networkStartTable2.score && networkStartTable.CountKills > networkStartTable2.CountKills))) || ((Defs.isFlag || Defs.isCapturePoints) && (networkStartTable.CountKills > networkStartTable2.CountKills || (networkStartTable.CountKills == networkStartTable2.CountKills && networkStartTable.score > networkStartTable2.score))))
				{
					NetworkStartTable networkStartTable3 = array[i];
					for (int num = i - 1; num >= j; num--)
					{
						array[num + 1] = array[num];
					}
					array[j] = networkStartTable3;
					break;
				}
			}
		}
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int k = 0; k < array.Length; k++)
		{
			if (array[k].myCommand == 0)
			{
				if (array[k] == WeaponManager.sharedManager.myNetworkStartTable)
				{
					result = num2;
				}
				num2++;
			}
			if (array[k].myCommand == 1)
			{
				if (array[k] == WeaponManager.sharedManager.myNetworkStartTable)
				{
					result = num3;
				}
				num3++;
			}
			if (array[k].myCommand == 2)
			{
				if (array[k] == WeaponManager.sharedManager.myNetworkStartTable)
				{
					result = num4;
				}
				num4++;
			}
		}
		return result;
	}

	public int GetWinningTeam()
	{
		int result = 0;
		if (Defs.isFlag)
		{
			if (WeaponManager.sharedManager.myNetworkStartTable != null)
			{
				if (WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag1 > WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag2)
				{
					result = 1;
				}
				else if (WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag2 > WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag1)
				{
					result = 2;
				}
			}
		}
		else if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints)
		{
			if (CapturePointController.sharedController.scoreBlue > CapturePointController.sharedController.scoreRed)
			{
				result = 1;
			}
			else if (CapturePointController.sharedController.scoreRed > CapturePointController.sharedController.scoreBlue)
			{
				result = 2;
			}
		}
		else if (myPlayerMoveC != null)
		{
			if (WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandBlue > WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandRed)
			{
				result = 1;
			}
			else if (WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandRed > WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandBlue)
			{
				result = 2;
			}
		}
		else if (GlobalGameController.countKillsBlue > GlobalGameController.countKillsRed)
		{
			result = 1;
		}
		else if (GlobalGameController.countKillsRed > GlobalGameController.countKillsBlue)
		{
			result = 2;
		}
		return result;
	}

	public void CalculateMatchRatingOnDisconnect()
	{
		if (myPlayerMoveC != null && ((!Defs.isCOOP && !Defs.isCompany && !Defs.isFlag && !Defs.isCapturePoints) || myPlayerMoveC.liveTime > 90f))
		{
			CalculateMatchRating(true);
		}
	}

	public void IncrementKills()
	{
		killCountMatch++;
	}

	public void IncrementDeath()
	{
		deathCountMatch++;
	}

	public float GetMatchKillrate()
	{
		if (Defs.isCOOP)
		{
			return 1f;
		}
		if (deathCountMatch != 0)
		{
			return (float)killCountMatch / (float)deathCountMatch;
		}
		return killCountMatch;
	}

	public void ClearKillrate()
	{
		killCountMatch = 0;
		deathCountMatch = 0;
	}

	public RatingSystem.RatingChange CalculateMatchRating(bool disconnecting)
	{
		RatingSystem.RatingChange result = RatingSystem.instance.currentRatingChange;
		if (LocalOrPasswordRoom() || !RatingSystem.instance.ratingMatch || !FriendsController.isUseRatingSystem)
		{
			return result;
		}
		if (Defs.isHunger && isDeadInHungerGame)
		{
			return result;
		}
		int num = GetPlaceInTable();
		int num2 = Initializer.networkTables.Count;
		int winningTeam = GetWinningTeam();
		bool flag = CheckForWin(num, winningTeam, CountKills, score, false);
		if (myPlayerMoveC != null && (Defs.isHunger || score > 0 || myPlayerMoveC.killedInMatch))
		{
			List<int> list = new List<int>();
			if (isCompany || Defs.isFlag || Defs.isCapturePoints)
			{
				int num3 = 0;
				int num4 = 0;
				for (int i = 0; i < Initializer.networkTables.Count; i++)
				{
					if (Initializer.networkTables[i].myCommand == 1)
					{
						num4++;
					}
					else
					{
						num3++;
					}
				}
				int num5 = ((num3 <= num4) ? num4 : num3);
				num2 = num5 * 2;
				num += ((!flag) ? num5 : 0);
			}
			else if (!Defs.isHunger)
			{
				flag = num < Mathf.CeilToInt(num2 / 2);
			}
			else
			{
				num = ((!flag) ? (Initializer.players.Count - 1) : 0);
				flag = num < Mathf.CeilToInt(playerCountInHunger / 2);
			}
			Debug.Log(string.Format("<color=orange>My place: {0}, team winner: {1}, rating winner - {2}</color>", num.ToString(), winningTeam.ToString(), flag.ToString()));
			if (!flag && !Defs.isHunger && myPlayerMoveC.liveTime < 60f)
			{
				return result;
			}
			if (disconnecting && flag)
			{
				return result;
			}
			if (Defs.isHunger && CountKills <= 0 && flag)
			{
				return result;
			}
			result = (Defs.isHunger ? RatingSystem.instance.CalculateRating(playerCountInHunger, Mathf.Clamp(num, 0, playerCountInHunger - 1), GetMatchKillrate()) : ((!isCompany && !Defs.isFlag && !Defs.isCapturePoints) ? RatingSystem.instance.CalculateRating(num2, num, GetMatchKillrate()) : RatingSystem.instance.CalculateRating(num2, num, GetMatchKillrate(), winningTeam == 0)));
		}
		return result;
	}

	public RatingSystem.RatingChange CalculateMatchRatingOld(bool disconnecting)
	{
		RatingSystem.RatingChange currentRatingChange = RatingSystem.instance.currentRatingChange;
		if (LocalOrPasswordRoom() || !RatingSystem.instance.ratingMatch || !FriendsController.isUseRatingSystem)
		{
			return currentRatingChange;
		}
		if (Defs.isHunger && isDeadInHungerGame)
		{
			return currentRatingChange;
		}
		int num = GetPlaceInTable();
		int winningTeam = GetWinningTeam();
		bool flag = CheckForWin(num, winningTeam, CountKills, score, false);
		if (myPlayerMoveC != null && (Defs.isHunger || score > 0 || myPlayerMoveC.killedInMatch))
		{
			List<int> list = new List<int>();
			if (isCompany || Defs.isFlag || Defs.isCapturePoints)
			{
				for (int i = 0; i < Initializer.networkTables.Count; i++)
				{
					if (Initializer.networkTables[i] != this)
					{
						list.Add((Initializer.networkTables[i].gameRating == -1) ? gameRating : Initializer.networkTables[i].gameRating);
					}
				}
				if (list.Count == 0)
				{
					list.Add(gameRating);
				}
			}
			else if (!Defs.isHunger)
			{
				for (int j = 0; j < Initializer.networkTables.Count; j++)
				{
					if (Initializer.networkTables[j] != this)
					{
						list.Add((Initializer.networkTables[j].gameRating == -1) ? gameRating : Initializer.networkTables[j].gameRating);
					}
				}
				if (list.Count == 0)
				{
					return currentRatingChange;
				}
				float num2 = (float)Initializer.networkTables.Count / 2f;
				flag = (float)(num + 1) <= num2;
				if (!flag)
				{
					num -= Mathf.FloorToInt(num2);
				}
			}
			Debug.Log(string.Format("<color=orange>My place: {0}, team winner: {1}, rating winner - {2}</color>", num.ToString(), winningTeam.ToString(), flag.ToString()));
			if (!flag && !Defs.isHunger && myPlayerMoveC.liveTime < 60f)
			{
				return currentRatingChange;
			}
			if (disconnecting && flag)
			{
				return currentRatingChange;
			}
		}
		return currentRatingChange;
	}

	public void DestroyPlayer()
	{
		isShowFinished = false;
		NickLabelController.currentCamera = Initializer.Instance.tc.GetComponent<Camera>();
		if (_cam != null)
		{
			_cam.SetActive(true);
			_cam.GetComponent<RPG_Camera>().enabled = false;
		}
		if (!isInet)
		{
			DestroyMyPlayer();
		}
		else if ((bool)_weaponManager && (bool)_weaponManager.myPlayer)
		{
			PhotonNetwork.Destroy(_weaponManager.myPlayer);
		}
	}

	[Obfuscation(Exclude = true)]
	private void DestroyMyPlayer()
	{
		if (WeaponManager.sharedManager.myPlayer != null)
		{
			Network.RemoveRPCs(_weaponManager.myPlayer.GetComponent<NetworkView>().viewID);
			Network.Destroy(_weaponManager.myPlayer);
		}
	}

	private void finishTable()
	{
		playersTable();
	}

	public void MyOnGUI()
	{
		if (experienceController.isShowAdd)
		{
			GUI.enabled = false;
		}
		if (showDisconnectFromServer)
		{
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)serverLeftTheGame.width * 0.5f * koofScreen, (float)(Screen.height / 2) - (float)serverLeftTheGame.height * 0.5f * koofScreen, (float)serverLeftTheGame.width * koofScreen, (float)serverLeftTheGame.height * koofScreen), serverLeftTheGame);
			GUI.enabled = false;
		}
		if (showDisconnectFromMasterServer)
		{
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)serverLeftTheGame.width * 0.5f * koofScreen, (float)(Screen.height / 2) - (float)serverLeftTheGame.height * 0.5f * koofScreen, (float)serverLeftTheGame.width * koofScreen, (float)serverLeftTheGame.height * koofScreen), serverLeftTheGame);
		}
		if (showTable)
		{
			playersTable();
		}
		if (isShowNickTable)
		{
			finishTable();
		}
		if (showMessagFacebook)
		{
			labelStyle.fontSize = Player_move_c.FontSizeForMessages;
			GUI.Label(Tools.SuccessMessageRect(), _SocialSentSuccess("Facebook"), labelStyle);
		}
		GUI.enabled = true;
	}

	[Obfuscation(Exclude = true)]
	public void ClearScoreCommandInFlagGame()
	{
		photonView.RPC("ClearScoreCommandInFlagGameRPC", PhotonTargets.Others);
	}

	[PunRPC]
	[RPC]
	public void ClearScoreCommandInFlagGameRPC()
	{
		if (WeaponManager.sharedManager.myTable != null)
		{
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().scoreCommandFlag1 = 0;
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().scoreCommandFlag2 = 0;
		}
	}

	private void AddFlag()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("BazaZoneCommand1");
		GameObject gameObject2 = GameObject.FindGameObjectWithTag("BazaZoneCommand2");
		PhotonNetwork.InstantiateSceneObject("Flags/Flag1", gameObject.transform.position, gameObject.transform.rotation, 0, null);
		PhotonNetwork.InstantiateSceneObject("Flags/Flag2", gameObject2.transform.position, gameObject2.transform.rotation, 0, null);
	}

	[PunRPC]
	[RPC]
	private void AddPaticleBazeRPC(int _command)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("BazaZoneCommand" + _command);
		UnityEngine.Object.Instantiate(Resources.Load((_command != WeaponManager.sharedManager.myNetworkStartTable.myCommand) ? "Ring_Particle_Red" : "Ring_Particle_Blue"), new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.22f, gameObject.transform.position.z), gameObject.transform.rotation);
	}

	public void AddScore()
	{
		CountKills++;
		GlobalGameController.CountKills = CountKills;
		photonView.RPC("AddPaticleBazeRPC", PhotonTargets.All, myCommand);
		if (myCommand == 1)
		{
			photonView.RPC("SynchScoreCommandRPC", PhotonTargets.All, 1, scoreCommandFlag1 + 1);
		}
		else
		{
			photonView.RPC("SynchScoreCommandRPC", PhotonTargets.All, 2, scoreCommandFlag2 + 1);
		}
		SynhCountKills();
	}

	[PunRPC]
	[RPC]
	private void SynchScoreCommandRPC(int _command, int _score)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("NetworkTable");
		for (int i = 0; i < array.Length; i++)
		{
			if (_command == 1)
			{
				array[i].GetComponent<NetworkStartTable>().scoreCommandFlag1 = _score;
			}
			else
			{
				array[i].GetComponent<NetworkStartTable>().scoreCommandFlag2 = _score;
			}
		}
	}

	private void OnDestroy()
	{
		if (isMine)
		{
			ShopNGUIController.sharedShop.onEquipSkinAction = null;
			RemoveShop(false);
			if (networkStartTableNGUIController != null && !networkStartTableNGUIController.isRewardShow)
			{
				UnityEngine.Object.Destroy(networkStartTableNGUIController.gameObject);
			}
			if (ShopNGUIController.sharedShop != null)
			{
				ShopNGUIController.sharedShop.resumeAction = null;
			}
		}
		if (Initializer.networkTables.Contains(this))
		{
			Initializer.networkTables.Remove(this);
		}
		PhotonObjectCacher.RemoveObject(base.gameObject);
	}

	[RPC]
	[PunRPC]
	private void SynchGameRating(int _rating)
	{
		gameRating = _rating;
	}
}
