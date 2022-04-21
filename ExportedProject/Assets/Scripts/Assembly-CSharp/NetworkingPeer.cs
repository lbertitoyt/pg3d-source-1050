using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using UnityEngine;

internal class NetworkingPeer : LoadbalancingPeer, IPhotonPeerListener
{
	public const string NameServerHost = "ns.exitgames.com";

	public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

	protected internal const string CurrentSceneProperty = "curScn";

	public const int SyncViewId = 0;

	public const int SyncCompressed = 1;

	public const int SyncNullValues = 2;

	public const int SyncFirstValue = 3;

	protected internal string mAppId;

	private string tokenCache;

	private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
	{
		{
			ConnectionProtocol.Udp,
			5058
		},
		{
			ConnectionProtocol.Tcp,
			4533
		},
		{
			ConnectionProtocol.WebSocket,
			9093
		},
		{
			ConnectionProtocol.WebSocketSecure,
			19093
		}
	};

	public bool IsInitialConnect;

	private bool didAuthenticate;

	protected internal List<TypedLobbyInfo> LobbyStatistics = new List<TypedLobbyInfo>();

	public bool insideLobby;

	public Dictionary<string, RoomInfo> mGameList = new Dictionary<string, RoomInfo>();

	public RoomInfo[] mGameListCopy = new RoomInfo[0];

	private bool isFetchingFriends;

	private string[] friendListRequested;

	private int friendListTimestamp;

	private string playername = string.Empty;

	private bool mPlayernameHasToBeUpdated;

	protected internal EnterRoomParams enterRoomParamsCache;

	private JoinType mLastJoinType;

	private Room mCurrentGame;

	public Dictionary<int, PhotonPlayer> mActors = new Dictionary<int, PhotonPlayer>();

	public PhotonPlayer[] mOtherPlayerListCopy = new PhotonPlayer[0];

	public PhotonPlayer[] mPlayerListCopy = new PhotonPlayer[0];

	public bool hasSwitchedMC;

	private HashSet<int> allowedReceivingGroups = new HashSet<int>();

	private HashSet<int> blockSendingGroups = new HashSet<int>();

	protected internal Dictionary<int, PhotonView> photonViewList = new Dictionary<int, PhotonView>();

	private readonly PhotonStream pStream = new PhotonStream(true, null);

	private readonly Dictionary<int, ExitGames.Client.Photon.Hashtable> dataPerGroupReliable = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();

	private readonly Dictionary<int, ExitGames.Client.Photon.Hashtable> dataPerGroupUnreliable = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();

	protected internal short currentLevelPrefix;

	protected internal bool loadingLevelAndPausedNetwork;

	public static bool UsePrefabCache = true;

	internal IPunPrefabPool ObjectPool;

	public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

	private Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

	private readonly Dictionary<string, int> rpcShortcuts;

	private Dictionary<int, object[]> tempInstantiationData = new Dictionary<int, object[]>();

	protected internal string mAppVersionPun
	{
		get
		{
			return string.Format("{0}_{1}", PhotonNetwork.gameVersion, "1.67");
		}
	}

	public AuthenticationValues CustomAuthenticationValues { get; set; }

	public string NameServerAddress
	{
		get
		{
			return GetNameServerAddress();
		}
	}

	public string MasterServerAddress { get; protected internal set; }

	public string mGameserver { get; internal set; }

	protected internal ServerConnection server { get; private set; }

	public PeerState State { get; internal set; }

	public bool IsUsingNameServer { get; protected internal set; }

	public bool IsAuthorizeSecretAvailable
	{
		get
		{
			return CustomAuthenticationValues != null && !string.IsNullOrEmpty(CustomAuthenticationValues.Token);
		}
	}

	public List<Region> AvailableRegions { get; protected internal set; }

	public CloudRegionCode CloudRegion { get; protected internal set; }

	private bool requestLobbyStatistics
	{
		get
		{
			return PhotonNetwork.EnableLobbyStatistics && server == ServerConnection.MasterServer;
		}
	}

	public TypedLobby lobby { get; set; }

	public int mPlayersOnMasterCount { get; internal set; }

	public int mGameCount { get; internal set; }

	public int mPlayersInRoomsCount { get; internal set; }

	protected internal int FriendsListAge
	{
		get
		{
			return (!isFetchingFriends && friendListTimestamp != 0) ? (Environment.TickCount - friendListTimestamp) : 0;
		}
	}

	public string PlayerName
	{
		get
		{
			return playername;
		}
		set
		{
			if (!string.IsNullOrEmpty(value) && !value.Equals(playername))
			{
				if (mLocalActor != null)
				{
					mLocalActor.name = value;
				}
				playername = value;
				if (CurrentGame != null)
				{
					SendPlayerName();
				}
			}
		}
	}

	public Room CurrentGame
	{
		get
		{
			if (mCurrentGame != null && mCurrentGame.isLocalClientInside)
			{
				return mCurrentGame;
			}
			return null;
		}
		private set
		{
			mCurrentGame = value;
		}
	}

	public PhotonPlayer mLocalActor { get; internal set; }

	public int mMasterClientId
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return mLocalActor.ID;
			}
			return (CurrentGame != null) ? CurrentGame.masterClientId : 0;
		}
		private set
		{
			if (CurrentGame != null)
			{
				CurrentGame.masterClientId = value;
			}
		}
	}

	public NetworkingPeer(string playername, ConnectionProtocol connectionProtocol)
		: base(connectionProtocol)
	{
		base.Listener = this;
		if (PhotonHandler.PingImplementation == null)
		{
			PhotonHandler.PingImplementation = typeof(PingMono);
		}
		base.LimitOfUnreliableCommands = 40;
		lobby = TypedLobby.Default;
		PlayerName = playername;
		mLocalActor = new PhotonPlayer(true, -1, this.playername);
		AddNewPlayer(mLocalActor.ID, mLocalActor);
		rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
		for (int i = 0; i < PhotonNetwork.PhotonServerSettings.RpcList.Count; i++)
		{
			string key = PhotonNetwork.PhotonServerSettings.RpcList[i];
			rpcShortcuts[key] = i;
		}
		State = global::PeerState.PeerCreated;
	}

	private string GetNameServerAddress()
	{
		ConnectionProtocol usedProtocol = base.UsedProtocol;
		int value = 0;
		ProtocolToNameServerPort.TryGetValue(usedProtocol, out value);
		string arg = string.Empty;
		switch (usedProtocol)
		{
		case ConnectionProtocol.WebSocket:
			arg = "ws://";
			break;
		case ConnectionProtocol.WebSocketSecure:
			arg = "wss://";
			break;
		}
		return string.Format("{0}{1}:{2}", arg, "ns.exitgames.com", value);
	}

	public override bool Connect(string serverAddress, string applicationName)
	{
		Debug.LogError("Avoid using this directly. Thanks.");
		return false;
	}

	public bool ReconnectToMaster()
	{
		if (CustomAuthenticationValues == null)
		{
			Debug.LogWarning("ReconnectToMaster() with CustomAuthenticationValues == null is not correct!");
			CustomAuthenticationValues = new AuthenticationValues();
		}
		CustomAuthenticationValues.Token = tokenCache;
		return Connect(MasterServerAddress, ServerConnection.MasterServer);
	}

	public bool ReconnectAndRejoin()
	{
		if (CustomAuthenticationValues == null)
		{
			Debug.LogWarning("ReconnectAndRejoin() with CustomAuthenticationValues == null is not correct!");
			CustomAuthenticationValues = new AuthenticationValues();
		}
		CustomAuthenticationValues.Token = tokenCache;
		if (!string.IsNullOrEmpty(mGameserver) && enterRoomParamsCache != null)
		{
			mLastJoinType = JoinType.JoinGame;
			enterRoomParamsCache.RejoinOnly = true;
			return Connect(mGameserver, ServerConnection.GameServer);
		}
		return false;
	}

	public bool Connect(string serverAddress, ServerConnection type)
	{
		if (PhotonHandler.AppQuits)
		{
			Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		if (PhotonNetwork.connectionStateDetailed == global::PeerState.Disconnecting)
		{
			Debug.LogError("Connect() failed. Can't connect while disconnecting (still). Current state: " + PhotonNetwork.connectionStateDetailed);
			return false;
		}
		bool flag = base.Connect(serverAddress, string.Empty);
		if (flag)
		{
			switch (type)
			{
			case ServerConnection.NameServer:
				State = global::PeerState.ConnectingToNameServer;
				break;
			case ServerConnection.MasterServer:
				State = global::PeerState.ConnectingToMasterserver;
				break;
			case ServerConnection.GameServer:
				State = global::PeerState.ConnectingToGameserver;
				break;
			}
		}
		return flag;
	}

	public bool ConnectToNameServer()
	{
		if (PhotonHandler.AppQuits)
		{
			Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		IsUsingNameServer = true;
		CloudRegion = CloudRegionCode.none;
		if (State == global::PeerState.ConnectedToNameServer)
		{
			return true;
		}
		string nameServerAddress = NameServerAddress;
		if (!base.Connect(nameServerAddress, "ns"))
		{
			return false;
		}
		State = global::PeerState.ConnectingToNameServer;
		return true;
	}

	public bool ConnectToRegionMaster(CloudRegionCode region)
	{
		if (PhotonHandler.AppQuits)
		{
			Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		IsUsingNameServer = true;
		CloudRegion = region;
		if (State == global::PeerState.ConnectedToNameServer)
		{
			AuthenticationValues authenticationValues = CustomAuthenticationValues;
			if (authenticationValues == null)
			{
				AuthenticationValues authenticationValues2 = new AuthenticationValues();
				authenticationValues2.UserId = PlayerName;
				authenticationValues = authenticationValues2;
			}
			AuthenticationValues authValues = authenticationValues;
			return OpAuthenticate(mAppId, mAppVersionPun, authValues, region.ToString(), requestLobbyStatistics);
		}
		string nameServerAddress = NameServerAddress;
		if (!base.Connect(nameServerAddress, "ns"))
		{
			return false;
		}
		State = global::PeerState.ConnectingToNameServer;
		return true;
	}

	public bool GetRegions()
	{
		if (server != ServerConnection.NameServer)
		{
			return false;
		}
		bool flag = OpGetRegions(mAppId);
		if (flag)
		{
			AvailableRegions = null;
		}
		return flag;
	}

	public override void Disconnect()
	{
		if (base.PeerState == PeerStateValue.Disconnected)
		{
			if (!PhotonHandler.AppQuits)
			{
				Debug.LogWarning(string.Format("Can't execute Disconnect() while not connected. Nothing changed. State: {0}", State));
			}
		}
		else
		{
			State = global::PeerState.Disconnecting;
			base.Disconnect();
		}
	}

	private void DisconnectToReconnect()
	{
		switch (server)
		{
		case ServerConnection.NameServer:
			State = global::PeerState.DisconnectingFromNameServer;
			base.Disconnect();
			break;
		case ServerConnection.MasterServer:
			State = global::PeerState.DisconnectingFromMasterserver;
			base.Disconnect();
			break;
		case ServerConnection.GameServer:
			State = global::PeerState.DisconnectingFromGameserver;
			base.Disconnect();
			break;
		}
	}

	private void LeftLobbyCleanup()
	{
		mGameList = new Dictionary<string, RoomInfo>();
		mGameListCopy = new RoomInfo[0];
		if (insideLobby)
		{
			insideLobby = false;
			SendMonoMessage(PhotonNetworkingMessage.OnLeftLobby);
		}
	}

	private void LeftRoomCleanup()
	{
		bool flag = CurrentGame != null;
		bool flag2 = ((CurrentGame == null) ? PhotonNetwork.autoCleanUpPlayerObjects : CurrentGame.autoCleanUp);
		hasSwitchedMC = false;
		CurrentGame = null;
		mActors = new Dictionary<int, PhotonPlayer>();
		mPlayerListCopy = new PhotonPlayer[0];
		mOtherPlayerListCopy = new PhotonPlayer[0];
		allowedReceivingGroups = new HashSet<int>();
		blockSendingGroups = new HashSet<int>();
		mGameList = new Dictionary<string, RoomInfo>();
		mGameListCopy = new RoomInfo[0];
		isFetchingFriends = false;
		ChangeLocalID(-1);
		if (flag2)
		{
			LocalCleanupAnythingInstantiated(true);
			PhotonNetwork.manuallyAllocatedViewIds = new List<int>();
		}
		if (flag)
		{
			SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
		}
	}

	protected internal void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
	{
		if (tempInstantiationData.Count > 0)
		{
			Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite this.");
		}
		if (destroyInstantiatedGameObjects)
		{
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			foreach (PhotonView value in photonViewList.Values)
			{
				if (value.isRuntimeInstantiated)
				{
					hashSet.Add(value.gameObject);
				}
			}
			foreach (GameObject item in hashSet)
			{
				RemoveInstantiatedGO(item, true);
			}
		}
		tempInstantiationData.Clear();
		PhotonNetwork.lastUsedViewSubId = 0;
		PhotonNetwork.lastUsedViewSubIdStatic = 0;
	}

	private void ReadoutProperties(ExitGames.Client.Photon.Hashtable gameProperties, ExitGames.Client.Photon.Hashtable pActorProperties, int targetActorNr)
	{
		if (pActorProperties != null && pActorProperties.Count > 0)
		{
			if (targetActorNr > 0)
			{
				PhotonPlayer playerWithId = GetPlayerWithId(targetActorNr);
				if (playerWithId != null)
				{
					ExitGames.Client.Photon.Hashtable actorPropertiesForActorNr = GetActorPropertiesForActorNr(pActorProperties, targetActorNr);
					playerWithId.InternalCacheProperties(actorPropertiesForActorNr);
					SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, playerWithId, actorPropertiesForActorNr);
				}
			}
			else
			{
				foreach (object key in pActorProperties.Keys)
				{
					int num = (int)key;
					ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)pActorProperties[key];
					string name = (string)hashtable[byte.MaxValue];
					PhotonPlayer photonPlayer = GetPlayerWithId(num);
					if (photonPlayer == null)
					{
						photonPlayer = new PhotonPlayer(false, num, name);
						AddNewPlayer(num, photonPlayer);
					}
					photonPlayer.InternalCacheProperties(hashtable);
					SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, photonPlayer, hashtable);
				}
			}
		}
		if (CurrentGame != null && gameProperties != null)
		{
			CurrentGame.InternalCacheProperties(gameProperties);
			SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, gameProperties);
			if (PhotonNetwork.automaticallySyncScene)
			{
				LoadLevelIfSynced();
			}
		}
	}

	private void AddNewPlayer(int ID, PhotonPlayer player)
	{
		if (!mActors.ContainsKey(ID))
		{
			mActors[ID] = player;
			RebuildPlayerListCopies();
		}
		else
		{
			Debug.LogError("Adding player twice: " + ID);
		}
	}

	private void RemovePlayer(int ID, PhotonPlayer player)
	{
		mActors.Remove(ID);
		if (!player.isLocal)
		{
			RebuildPlayerListCopies();
		}
	}

	private void RebuildPlayerListCopies()
	{
		mPlayerListCopy = new PhotonPlayer[mActors.Count];
		mActors.Values.CopyTo(mPlayerListCopy, 0);
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		PhotonPlayer[] array = mPlayerListCopy;
		foreach (PhotonPlayer photonPlayer in array)
		{
			if (!photonPlayer.isLocal)
			{
				list.Add(photonPlayer);
			}
		}
		mOtherPlayerListCopy = list.ToArray();
	}

	private void ResetPhotonViewsOnSerialize()
	{
		foreach (PhotonView value in photonViewList.Values)
		{
			value.lastOnSerializeDataSent = null;
		}
	}

	private void HandleEventLeave(int actorID, EventData evLeave)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log("HandleEventLeave for player ID: " + actorID + " evLeave: " + evLeave.ToStringFull());
		}
		PhotonPlayer playerWithId = GetPlayerWithId(actorID);
		if (playerWithId == null)
		{
			Debug.LogError(string.Format("Received event Leave for unknown player ID: {0}", actorID));
			return;
		}
		if (evLeave.Parameters.ContainsKey(233))
		{
			playerWithId.isInactive = (bool)evLeave.Parameters[233];
			if (playerWithId.isInactive)
			{
				Debug.LogWarning("HandleEventLeave for player ID: " + actorID + " isInactive: " + playerWithId.isInactive + ". Stopping handling if inactive.");
				return;
			}
		}
		if (evLeave.Parameters.ContainsKey(203))
		{
			if ((int)evLeave[203] != 0)
			{
				mMasterClientId = (int)evLeave[203];
				UpdateMasterClient();
			}
		}
		else if (!CurrentGame.serverSideMasterClient)
		{
			CheckMasterClient(actorID);
		}
		if (CurrentGame != null && CurrentGame.autoCleanUp)
		{
			DestroyPlayerObjects(actorID, true);
		}
		RemovePlayer(actorID, playerWithId);
		SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, playerWithId);
	}

	private void CheckMasterClient(int leavingPlayerId)
	{
		bool flag = mMasterClientId == leavingPlayerId;
		bool flag2 = leavingPlayerId > 0;
		if (flag2 && !flag)
		{
			return;
		}
		int num;
		if (mActors.Count <= 1)
		{
			num = mLocalActor.ID;
		}
		else
		{
			num = int.MaxValue;
			foreach (int key in mActors.Keys)
			{
				if (key < num && key != leavingPlayerId)
				{
					num = key;
				}
			}
		}
		mMasterClientId = num;
		if (flag2)
		{
			SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, GetPlayerWithId(num));
		}
	}

	protected internal void UpdateMasterClient()
	{
		SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, PhotonNetwork.masterClient);
	}

	private static int ReturnLowestPlayerId(PhotonPlayer[] players, int playerIdToIgnore)
	{
		if (players == null || players.Length == 0)
		{
			return -1;
		}
		int num = int.MaxValue;
		foreach (PhotonPlayer photonPlayer in players)
		{
			if (photonPlayer.ID != playerIdToIgnore && photonPlayer.ID < num)
			{
				num = photonPlayer.ID;
			}
		}
		return num;
	}

	protected internal bool SetMasterClient(int playerId, bool sync)
	{
		if (mMasterClientId == playerId || !mActors.ContainsKey(playerId))
		{
			return false;
		}
		if (sync && !OpRaiseEvent(208, new ExitGames.Client.Photon.Hashtable { 
		{
			(byte)1,
			playerId
		} }, true, null))
		{
			return false;
		}
		hasSwitchedMC = true;
		CurrentGame.masterClientId = playerId;
		SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, GetPlayerWithId(playerId));
		return true;
	}

	public bool SetMasterClient(int nextMasterId)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add((byte)248, nextMasterId);
		ExitGames.Client.Photon.Hashtable gameProperties = hashtable;
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add((byte)248, mMasterClientId);
		ExitGames.Client.Photon.Hashtable expectedProperties = hashtable;
		return OpSetPropertiesOfRoom(gameProperties, expectedProperties);
	}

	private ExitGames.Client.Photon.Hashtable GetActorPropertiesForActorNr(ExitGames.Client.Photon.Hashtable actorProperties, int actorNr)
	{
		if (actorProperties.ContainsKey(actorNr))
		{
			return (ExitGames.Client.Photon.Hashtable)actorProperties[actorNr];
		}
		return actorProperties;
	}

	protected internal PhotonPlayer GetPlayerWithId(int number)
	{
		if (mActors == null)
		{
			return null;
		}
		PhotonPlayer value = null;
		mActors.TryGetValue(number, out value);
		return value;
	}

	private void SendPlayerName()
	{
		if (State == global::PeerState.Joining)
		{
			mPlayernameHasToBeUpdated = true;
		}
		else if (mLocalActor != null)
		{
			mLocalActor.name = PlayerName;
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[byte.MaxValue] = PlayerName;
			if (mLocalActor.ID > 0)
			{
				OpSetPropertiesOfActor(mLocalActor.ID, hashtable);
				mPlayernameHasToBeUpdated = false;
			}
		}
	}

	private void GameEnteredOnGameServer(OperationResponse operationResponse)
	{
		if (operationResponse.ReturnCode != 0)
		{
			switch (operationResponse.OperationCode)
			{
			case 227:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.Log("Create failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
				break;
			case 226:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
					if (operationResponse.ReturnCode == 32758)
					{
						Debug.Log("Most likely the game became empty during the switch to GameServer.");
					}
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
				break;
			case 225:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
					if (operationResponse.ReturnCode == 32758)
					{
						Debug.Log("Most likely the game became empty during the switch to GameServer.");
					}
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
				break;
			}
			DisconnectToReconnect();
		}
		else
		{
			Room room = new Room(enterRoomParamsCache.RoomName, null);
			room.isLocalClientInside = true;
			CurrentGame = room;
			State = global::PeerState.Joined;
			if (operationResponse.Parameters.ContainsKey(252))
			{
				int[] actorsInRoom = (int[])operationResponse.Parameters[252];
				UpdatedActorList(actorsInRoom);
			}
			int newID = (int)operationResponse[254];
			ChangeLocalID(newID);
			ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[249];
			ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[248];
			ReadoutProperties(gameProperties, pActorProperties, 0);
			if (!CurrentGame.serverSideMasterClient)
			{
				CheckMasterClient(-1);
			}
			if (mPlayernameHasToBeUpdated)
			{
				SendPlayerName();
			}
			switch (operationResponse.OperationCode)
			{
			case 227:
				SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
				break;
			case 225:
			case 226:
				break;
			}
		}
	}

	private ExitGames.Client.Photon.Hashtable GetLocalActorProperties()
	{
		if (PhotonNetwork.player != null)
		{
			return PhotonNetwork.player.allProperties;
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[byte.MaxValue] = PlayerName;
		return hashtable;
	}

	public void ChangeLocalID(int newID)
	{
		if (mLocalActor == null)
		{
			Debug.LogWarning(string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", mLocalActor, mActors == null, newID));
		}
		if (mActors.ContainsKey(mLocalActor.ID))
		{
			mActors.Remove(mLocalActor.ID);
		}
		mLocalActor.InternalChangeLocalID(newID);
		mActors[mLocalActor.ID] = mLocalActor;
		RebuildPlayerListCopies();
	}

	public override bool OpFindFriends(string[] friendsToFind)
	{
		if (isFetchingFriends)
		{
			return false;
		}
		friendListRequested = friendsToFind;
		isFetchingFriends = true;
		return base.OpFindFriends(friendsToFind);
	}

	public bool OpCreateGame(EnterRoomParams enterRoomParams)
	{
		bool flag = (enterRoomParams.OnGameServer = server == ServerConnection.GameServer);
		enterRoomParams.PlayerProperties = GetLocalActorProperties();
		if (!flag)
		{
			enterRoomParamsCache = enterRoomParams;
		}
		mLastJoinType = JoinType.CreateGame;
		return base.OpCreateRoom(enterRoomParams);
	}

	public override bool OpJoinRoom(EnterRoomParams opParams)
	{
		if (!(opParams.OnGameServer = server == ServerConnection.GameServer))
		{
			enterRoomParamsCache = opParams;
		}
		mLastJoinType = ((!opParams.CreateIfNotExists) ? JoinType.JoinGame : JoinType.JoinOrCreateOnDemand);
		return base.OpJoinRoom(opParams);
	}

	public override bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
	{
		enterRoomParamsCache = new EnterRoomParams();
		enterRoomParamsCache.Lobby = opJoinRandomRoomParams.TypedLobby;
		mLastJoinType = JoinType.JoinRandomGame;
		return base.OpJoinRandomRoom(opJoinRandomRoomParams);
	}

	public virtual bool OpLeave()
	{
		if (State != global::PeerState.Joined)
		{
			Debug.LogWarning("Not sending leave operation. State is not 'Joined': " + State);
			return false;
		}
		return OpCustom(254, null, true, 0);
	}

	public override bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
	{
		if (PhotonNetwork.offlineMode)
		{
			return false;
		}
		return base.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log(string.Format("OnStatusChanged: {0}", statusCode.ToString()));
		}
		switch (statusCode)
		{
		case StatusCode.Connect:
			if (State == global::PeerState.ConnectingToNameServer)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
				{
					Debug.Log("Connected to NameServer.");
				}
				server = ServerConnection.NameServer;
				if (CustomAuthenticationValues != null)
				{
					CustomAuthenticationValues.Token = null;
				}
			}
			if (State == global::PeerState.ConnectingToGameserver)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
				{
					Debug.Log("Connected to gameserver.");
				}
				server = ServerConnection.GameServer;
				State = global::PeerState.ConnectedToGameserver;
			}
			if (State == global::PeerState.ConnectingToMasterserver)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
				{
					Debug.Log("Connected to masterserver.");
				}
				server = ServerConnection.MasterServer;
				State = global::PeerState.Authenticating;
				if (IsInitialConnect)
				{
					IsInitialConnect = false;
					SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton);
				}
			}
			if (!base.IsProtocolSecure)
			{
				EstablishEncryption();
			}
			else
			{
				Debug.Log("Skipping EstablishEncryption. Protocol is secure.");
			}
			if (IsAuthorizeSecretAvailable || base.IsProtocolSecure)
			{
				AuthenticationValues authenticationValues4 = CustomAuthenticationValues;
				if (authenticationValues4 == null)
				{
					AuthenticationValues authenticationValues2 = new AuthenticationValues();
					authenticationValues2.UserId = PlayerName;
					authenticationValues4 = authenticationValues2;
				}
				AuthenticationValues authValues3 = authenticationValues4;
				didAuthenticate = OpAuthenticate(mAppId, mAppVersionPun, authValues3, CloudRegion.ToString(), requestLobbyStatistics);
				if (didAuthenticate)
				{
					State = global::PeerState.Authenticating;
				}
			}
			break;
		case StatusCode.EncryptionEstablished:
			if (server == ServerConnection.NameServer)
			{
				State = global::PeerState.ConnectedToNameServer;
				if (!didAuthenticate && CloudRegion == CloudRegionCode.none)
				{
					OpGetRegions(mAppId);
				}
			}
			if (!didAuthenticate && (!IsUsingNameServer || CloudRegion != CloudRegionCode.none))
			{
				AuthenticationValues authenticationValues = CustomAuthenticationValues;
				if (authenticationValues == null)
				{
					AuthenticationValues authenticationValues2 = new AuthenticationValues();
					authenticationValues2.UserId = PlayerName;
					authenticationValues = authenticationValues2;
				}
				AuthenticationValues authValues = authenticationValues;
				didAuthenticate = OpAuthenticate(mAppId, mAppVersionPun, authValues, CloudRegion.ToString(), requestLobbyStatistics);
				if (didAuthenticate)
				{
					State = global::PeerState.Authenticating;
				}
			}
			break;
		case StatusCode.EncryptionFailedToEstablish:
		{
			Debug.LogError(string.Concat("Encryption wasn't established: ", statusCode, ". Going to authenticate anyways."));
			AuthenticationValues authenticationValues3 = CustomAuthenticationValues;
			if (authenticationValues3 == null)
			{
				AuthenticationValues authenticationValues2 = new AuthenticationValues();
				authenticationValues2.UserId = PlayerName;
				authenticationValues3 = authenticationValues2;
			}
			AuthenticationValues authValues2 = authenticationValues3;
			OpAuthenticate(mAppId, mAppVersionPun, authValues2, CloudRegion.ToString(), requestLobbyStatistics);
			break;
		}
		case StatusCode.Disconnect:
			didAuthenticate = false;
			isFetchingFriends = false;
			if (server == ServerConnection.GameServer)
			{
				LeftRoomCleanup();
			}
			if (server == ServerConnection.MasterServer)
			{
				LeftLobbyCleanup();
			}
			if (State == global::PeerState.DisconnectingFromMasterserver)
			{
				if (Connect(mGameserver, ServerConnection.GameServer))
				{
					State = global::PeerState.ConnectingToGameserver;
				}
				break;
			}
			if (State == global::PeerState.DisconnectingFromGameserver || State == global::PeerState.DisconnectingFromNameServer)
			{
				if (Connect(MasterServerAddress, ServerConnection.MasterServer))
				{
					State = global::PeerState.ConnectingToMasterserver;
				}
				break;
			}
			if (CustomAuthenticationValues != null)
			{
				CustomAuthenticationValues.Token = null;
			}
			State = global::PeerState.PeerCreated;
			SendMonoMessage(PhotonNetworkingMessage.OnDisconnectedFromPhoton);
			break;
		case StatusCode.SecurityExceptionOnConnect:
		case StatusCode.ExceptionOnConnect:
		{
			State = global::PeerState.PeerCreated;
			if (CustomAuthenticationValues != null)
			{
				CustomAuthenticationValues.Token = null;
			}
			DisconnectCause disconnectCause = (DisconnectCause)statusCode;
			SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, disconnectCause);
			break;
		}
		case StatusCode.Exception:
			if (IsInitialConnect)
			{
				Debug.LogError("Exception while connecting to: " + base.ServerAddress + ". Check if the server is available.");
				if (base.ServerAddress == null || base.ServerAddress.StartsWith("127.0.0.1"))
				{
					Debug.LogWarning("The server address is 127.0.0.1 (localhost): Make sure the server is running on this machine. Android and iOS emulators have their own localhost.");
					if (base.ServerAddress == mGameserver)
					{
						Debug.LogWarning("This might be a misconfiguration in the game server config. You need to edit it to a (public) address.");
					}
				}
				State = global::PeerState.PeerCreated;
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, disconnectCause);
			}
			else
			{
				State = global::PeerState.PeerCreated;
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, disconnectCause);
			}
			Disconnect();
			break;
		case StatusCode.TimeoutDisconnect:
			if (IsInitialConnect)
			{
				Debug.LogWarning(string.Concat(statusCode, " while connecting to: ", base.ServerAddress, ". Check if the server is available."));
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, disconnectCause);
			}
			else
			{
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, disconnectCause);
			}
			if (CustomAuthenticationValues != null)
			{
				CustomAuthenticationValues.Token = null;
			}
			if (base.ServerAddress.Equals(mGameserver))
			{
				mGameserver = null;
			}
			if (base.ServerAddress.Equals(MasterServerAddress))
			{
				base.ServerAddress = null;
			}
			Disconnect();
			break;
		case StatusCode.ExceptionOnReceive:
		case StatusCode.DisconnectByServer:
		case StatusCode.DisconnectByServerUserLimit:
		case StatusCode.DisconnectByServerLogic:
			if (IsInitialConnect)
			{
				Debug.LogWarning(string.Concat(statusCode, " while connecting to: ", base.ServerAddress, ". Check if the server is available."));
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, disconnectCause);
			}
			else
			{
				DisconnectCause disconnectCause = (DisconnectCause)statusCode;
				SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, disconnectCause);
			}
			if (CustomAuthenticationValues != null)
			{
				CustomAuthenticationValues.Token = null;
			}
			Disconnect();
			break;
		case StatusCode.SendError:
			break;
		case StatusCode.QueueOutgoingReliableWarning:
		case StatusCode.QueueOutgoingUnreliableWarning:
		case StatusCode.QueueOutgoingAcksWarning:
		case StatusCode.QueueSentWarning:
			break;
		case StatusCode.QueueIncomingReliableWarning:
		case StatusCode.QueueIncomingUnreliableWarning:
			Debug.Log(string.Concat(statusCode, ". This client buffers many incoming messages. This is OK temporarily. With lots of these warnings, check if you send too much or execute messages too slow. ", (!PhotonNetwork.isMessageQueueRunning) ? "Your isMessageQueueRunning is false. This can cause the issue temporarily." : string.Empty));
			break;
		default:
			Debug.LogError("Received unknown status code: " + statusCode);
			break;
		}
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		switch (level)
		{
		case DebugLevel.ERROR:
			Debug.LogError(message);
			return;
		case DebugLevel.WARNING:
			Debug.LogWarning(message);
			return;
		case DebugLevel.INFO:
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log(message);
				return;
			}
			break;
		}
		if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
		{
			Debug.Log(message);
		}
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
		if (PhotonNetwork.networkingPeer.State == global::PeerState.Disconnecting)
		{
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log("OperationResponse ignored while disconnecting. Code: " + operationResponse.OperationCode);
			}
			return;
		}
		if (operationResponse.ReturnCode == 0)
		{
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log(operationResponse.ToString());
			}
		}
		else if (operationResponse.ReturnCode == -3)
		{
			Debug.LogError("Operation " + operationResponse.OperationCode + " could not be executed (yet). Wait for state JoinedLobby or ConnectedToMaster and their callbacks before calling operations. WebRPCs need a server-side configuration. Enum OperationCode helps identify the operation.");
		}
		else if (operationResponse.ReturnCode == 32752)
		{
			Debug.LogError("Operation " + operationResponse.OperationCode + " failed in a server-side plugin. Check the configuration in the Dashboard. Message from server-plugin: " + operationResponse.DebugMessage);
		}
		else if (operationResponse.ReturnCode == 32760)
		{
			Debug.LogWarning("Operation failed: " + operationResponse.ToStringFull());
		}
		else
		{
			Debug.LogError("Operation failed: " + operationResponse.ToStringFull() + " Server: " + server);
		}
		if (operationResponse.Parameters.ContainsKey(221))
		{
			if (CustomAuthenticationValues == null)
			{
				CustomAuthenticationValues = new AuthenticationValues();
			}
			CustomAuthenticationValues.Token = operationResponse[221] as string;
			tokenCache = CustomAuthenticationValues.Token;
		}
		switch (operationResponse.OperationCode)
		{
		case 230:
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == -2)
				{
					Debug.LogError(string.Format("If you host Photon yourself, make sure to start the 'Instance LoadBalancing' " + base.ServerAddress));
				}
				else if (operationResponse.ReturnCode == short.MaxValue)
				{
					Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
					SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);
				}
				else if (operationResponse.ReturnCode == 32755)
				{
					Debug.LogError(string.Format("Custom Authentication failed (either due to user-input or configuration or AuthParameter string format). Calling: OnCustomAuthenticationFailed()"));
					SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationFailed, operationResponse.DebugMessage);
				}
				else
				{
					Debug.LogError(string.Format("Authentication failed: '{0}' Code: {1}", operationResponse.DebugMessage, operationResponse.ReturnCode));
				}
				State = global::PeerState.Disconnecting;
				Disconnect();
				if (operationResponse.ReturnCode == 32757)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.LogWarning(string.Format("Currently, the limit of users is reached for this title. Try again later. Disconnecting"));
					}
					SendMonoMessage(PhotonNetworkingMessage.OnPhotonMaxCccuReached);
					SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.MaxCcuReached);
				}
				else if (operationResponse.ReturnCode == 32756)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.LogError(string.Format("The used master server address is not available with the subscription currently used. Got to Photon Cloud Dashboard or change URL. Disconnecting."));
					}
					SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.InvalidRegion);
				}
				else if (operationResponse.ReturnCode == 32753)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.LogError(string.Format("The authentication ticket expired. You need to connect (and authenticate) again. Disconnecting."));
					}
					SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.AuthenticationTicketExpired);
				}
				break;
			}
			if (server == ServerConnection.NameServer || server == ServerConnection.MasterServer)
			{
				if (operationResponse.Parameters.ContainsKey(225))
				{
					string text = (string)operationResponse.Parameters[225];
					if (!string.IsNullOrEmpty(text))
					{
						if (CustomAuthenticationValues == null)
						{
							CustomAuthenticationValues = new AuthenticationValues();
						}
						CustomAuthenticationValues.UserId = text;
						PhotonNetwork.player.userId = text;
						if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
						{
							DebugReturn(DebugLevel.INFO, string.Format("Received your UserID from server. Updating local value to: {0}", text));
						}
					}
				}
				if (operationResponse.Parameters.ContainsKey(202))
				{
					playername = (string)operationResponse.Parameters[202];
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						DebugReturn(DebugLevel.INFO, string.Format("Received your NickName from server. Updating local value to: {0}", playername));
					}
				}
			}
			if (server == ServerConnection.NameServer)
			{
				MasterServerAddress = operationResponse[230] as string;
				DisconnectToReconnect();
			}
			else if (server == ServerConnection.MasterServer)
			{
				if (PhotonNetwork.autoJoinLobby)
				{
					State = global::PeerState.Authenticated;
					OpJoinLobby(lobby);
				}
				else
				{
					State = global::PeerState.ConnectedToMaster;
					SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
				}
			}
			else if (server == ServerConnection.GameServer)
			{
				State = global::PeerState.Joining;
				enterRoomParamsCache.PlayerProperties = GetLocalActorProperties();
				enterRoomParamsCache.OnGameServer = true;
				if (mLastJoinType == JoinType.JoinGame || mLastJoinType == JoinType.JoinRandomGame || mLastJoinType == JoinType.JoinOrCreateOnDemand)
				{
					OpJoinRoom(enterRoomParamsCache);
				}
				else if (mLastJoinType == JoinType.CreateGame)
				{
					OpCreateGame(enterRoomParamsCache);
				}
			}
			if (operationResponse.Parameters.ContainsKey(245))
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)operationResponse.Parameters[245];
				if (dictionary != null)
				{
					SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationResponse, dictionary);
				}
			}
			break;
		case 220:
		{
			if (operationResponse.ReturnCode == short.MaxValue)
			{
				Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
				SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);
				State = global::PeerState.Disconnecting;
				Disconnect();
				break;
			}
			if (operationResponse.ReturnCode != 0)
			{
				Debug.LogError("GetRegions failed. Can't provide regions list. Error: " + operationResponse.ReturnCode + ": " + operationResponse.DebugMessage);
				break;
			}
			string[] array3 = operationResponse[210] as string[];
			string[] array4 = operationResponse[230] as string[];
			if (array3 == null || array4 == null || array3.Length != array4.Length)
			{
				Debug.LogError("The region arrays from Name Server are not ok. Must be non-null and same length. " + (array3 == null) + " " + (array4 == null) + "\n" + operationResponse.ToStringFull());
				break;
			}
			AvailableRegions = new List<Region>(array3.Length);
			for (int j = 0; j < array3.Length; j++)
			{
				string text3 = array3[j];
				if (string.IsNullOrEmpty(text3))
				{
					continue;
				}
				text3 = text3.ToLower();
				CloudRegionCode cloudRegionCode = Region.Parse(text3);
				bool flag = true;
				if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion && PhotonNetwork.PhotonServerSettings.EnabledRegions != 0)
				{
					CloudRegionFlag cloudRegionFlag = Region.ParseFlag(text3);
					flag = (PhotonNetwork.PhotonServerSettings.EnabledRegions & cloudRegionFlag) != 0;
					if (!flag && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.Log("Skipping region because it's not in PhotonServerSettings.EnabledRegions: " + cloudRegionCode);
					}
				}
				if (flag)
				{
					AvailableRegions.Add(new Region
					{
						Code = cloudRegionCode,
						HostAndPort = array4[j]
					});
				}
			}
			if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
			{
				PhotonHandler.PingAvailableRegionsAndConnectToBest();
			}
			break;
		}
		case 227:
			if (server == ServerConnection.GameServer)
			{
				GameEnteredOnGameServer(operationResponse);
			}
			else if (operationResponse.ReturnCode != 0)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.LogWarning(string.Format("CreateRoom failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));
				}
				State = ((!insideLobby) ? global::PeerState.ConnectedToMaster : global::PeerState.JoinedLobby);
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
			}
			else
			{
				string text2 = (string)operationResponse[byte.MaxValue];
				if (!string.IsNullOrEmpty(text2))
				{
					enterRoomParamsCache.RoomName = text2;
				}
				mGameserver = (string)operationResponse[230];
				DisconnectToReconnect();
			}
			break;
		case 226:
			if (server != ServerConnection.GameServer)
			{
				if (operationResponse.ReturnCode != 0)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.Log(string.Format("JoinRoom failed (room maybe closed by now). Client stays on masterserver: {0}. State: {1}", operationResponse.ToStringFull(), State));
					}
					SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
				}
				else
				{
					mGameserver = (string)operationResponse[230];
					DisconnectToReconnect();
				}
			}
			else
			{
				GameEnteredOnGameServer(operationResponse);
			}
			break;
		case 225:
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == 32760)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
					{
						Debug.Log("JoinRandom failed: No open game. Calling: OnPhotonRandomJoinFailed() and staying on master server.");
					}
				}
				else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					Debug.LogWarning(string.Format("JoinRandom failed: {0}.", operationResponse.ToStringFull()));
				}
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
			}
			else
			{
				string roomName = (string)operationResponse[byte.MaxValue];
				enterRoomParamsCache.RoomName = roomName;
				mGameserver = (string)operationResponse[230];
				DisconnectToReconnect();
			}
			break;
		case 229:
			State = global::PeerState.JoinedLobby;
			insideLobby = true;
			SendMonoMessage(PhotonNetworkingMessage.OnJoinedLobby);
			break;
		case 228:
			State = global::PeerState.Authenticated;
			LeftLobbyCleanup();
			break;
		case 254:
			DisconnectToReconnect();
			break;
		case 251:
		{
			ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[249];
			ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[248];
			ReadoutProperties(gameProperties, pActorProperties, 0);
			break;
		}
		case 252:
		case 253:
			break;
		case 222:
		{
			bool[] array = operationResponse[1] as bool[];
			string[] array2 = operationResponse[2] as string[];
			if (array != null && array2 != null && friendListRequested != null && array.Length == friendListRequested.Length)
			{
				List<FriendInfo> list = new List<FriendInfo>(friendListRequested.Length);
				for (int i = 0; i < friendListRequested.Length; i++)
				{
					FriendInfo friendInfo = new FriendInfo();
					friendInfo.Name = friendListRequested[i];
					friendInfo.Room = array2[i];
					friendInfo.IsOnline = array[i];
					list.Insert(i, friendInfo);
				}
				PhotonNetwork.Friends = list;
			}
			else
			{
				Debug.LogError("FindFriends failed to apply the result, as a required value wasn't provided or the friend list length differed from result.");
			}
			friendListRequested = null;
			isFetchingFriends = false;
			friendListTimestamp = Environment.TickCount;
			if (friendListTimestamp == 0)
			{
				friendListTimestamp = 1;
			}
			SendMonoMessage(PhotonNetworkingMessage.OnUpdatedFriendList);
			break;
		}
		case 219:
			SendMonoMessage(PhotonNetworkingMessage.OnWebRpcResponse, operationResponse);
			break;
		default:
			Debug.LogWarning(string.Format("OperationResponse unhandled: {0}", operationResponse.ToString()));
			break;
		}
	}

	public void OnEvent(EventData photonEvent)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log(string.Format("OnEvent: {0}", photonEvent.ToString()));
		}
		int num = -1;
		PhotonPlayer photonPlayer = null;
		if (photonEvent.Parameters.ContainsKey(254))
		{
			num = (int)photonEvent[254];
			photonPlayer = GetPlayerWithId(num);
		}
		switch (photonEvent.Code)
		{
		case 209:
		{
			int[] array = (int[])photonEvent.Parameters[245];
			int num3 = array[0];
			int num4 = array[1];
			Debug.Log("Ev OwnershipRequest: " + photonEvent.Parameters.ToStringFull() + " ViewID: " + num3 + " from: " + num4 + " Time: " + Environment.TickCount % 1000);
			PhotonView photonView = PhotonView.Find(num3);
			if (photonView == null)
			{
				Debug.LogWarning("Can't find PhotonView of incoming OwnershipRequest. ViewId not found: " + num3);
				return;
			}
			Debug.Log(string.Concat("Ev OwnershipRequest PhotonView.ownershipTransfer: ", photonView.ownershipTransfer, " .ownerId: ", photonView.ownerId, " isOwnerActive: ", photonView.isOwnerActive, ". This client's player: ", PhotonNetwork.player.ToStringFull()));
			switch (photonView.ownershipTransfer)
			{
			case OwnershipOption.Fixed:
				Debug.LogWarning("Ownership mode == fixed. Ignoring request.");
				break;
			case OwnershipOption.Takeover:
				if (num4 == photonView.ownerId)
				{
					photonView.ownerId = num;
				}
				break;
			case OwnershipOption.Request:
				if ((num4 == PhotonNetwork.player.ID || PhotonNetwork.player.isMasterClient) && (photonView.ownerId == PhotonNetwork.player.ID || (PhotonNetwork.player.isMasterClient && !photonView.isOwnerActive)))
				{
					SendMonoMessage(PhotonNetworkingMessage.OnOwnershipRequest, photonView, photonPlayer);
				}
				break;
			}
			return;
		}
		case 210:
		{
			int[] array6 = (int[])photonEvent.Parameters[245];
			Debug.Log("Ev OwnershipTransfer. ViewID " + array6[0] + " to: " + array6[1] + " Time: " + Environment.TickCount % 1000);
			int viewID = array6[0];
			int ownerId = array6[1];
			PhotonView photonView2 = PhotonView.Find(viewID);
			if (photonView2 != null)
			{
				photonView2.ownerId = ownerId;
			}
			return;
		}
		case 230:
		{
			mGameList = new Dictionary<string, RoomInfo>();
			ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent[222];
			foreach (DictionaryEntry item in hashtable)
			{
				string text = (string)item.Key;
				mGameList[text] = new RoomInfo(text, (ExitGames.Client.Photon.Hashtable)item.Value);
			}
			mGameListCopy = new RoomInfo[mGameList.Count];
			mGameList.Values.CopyTo(mGameListCopy, 0);
			SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
			return;
		}
		case 229:
		{
			ExitGames.Client.Photon.Hashtable hashtable4 = (ExitGames.Client.Photon.Hashtable)photonEvent[222];
			foreach (DictionaryEntry item2 in hashtable4)
			{
				string text2 = (string)item2.Key;
				RoomInfo roomInfo = new RoomInfo(text2, (ExitGames.Client.Photon.Hashtable)item2.Value);
				if (roomInfo.removedFromList)
				{
					mGameList.Remove(text2);
				}
				else
				{
					mGameList[text2] = roomInfo;
				}
			}
			mGameListCopy = new RoomInfo[mGameList.Count];
			mGameList.Values.CopyTo(mGameListCopy, 0);
			SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
			return;
		}
		case 228:
			return;
		case 226:
			mPlayersInRoomsCount = (int)photonEvent[229];
			mPlayersOnMasterCount = (int)photonEvent[227];
			mGameCount = (int)photonEvent[228];
			return;
		case byte.MaxValue:
		{
			ExitGames.Client.Photon.Hashtable properties = (ExitGames.Client.Photon.Hashtable)photonEvent[249];
			if (photonPlayer == null)
			{
				bool isLocal = mLocalActor.ID == num;
				AddNewPlayer(num, new PhotonPlayer(isLocal, num, properties));
				ResetPhotonViewsOnSerialize();
			}
			else
			{
				photonPlayer.InternalCacheProperties(properties);
				photonPlayer.isInactive = false;
			}
			if (num == mLocalActor.ID)
			{
				int[] actorsInRoom = (int[])photonEvent[252];
				UpdatedActorList(actorsInRoom);
				if (mLastJoinType == JoinType.JoinOrCreateOnDemand && mLocalActor.ID == 1)
				{
					SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
				}
				SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
			}
			else
			{
				SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerConnected, mActors[num]);
			}
			return;
		}
		case 254:
			HandleEventLeave(num, photonEvent);
			return;
		case 253:
		{
			int num2 = (int)photonEvent[253];
			ExitGames.Client.Photon.Hashtable gameProperties = null;
			ExitGames.Client.Photon.Hashtable pActorProperties = null;
			if (num2 == 0)
			{
				gameProperties = (ExitGames.Client.Photon.Hashtable)photonEvent[251];
			}
			else
			{
				pActorProperties = (ExitGames.Client.Photon.Hashtable)photonEvent[251];
			}
			ReadoutProperties(gameProperties, pActorProperties, num2);
			return;
		}
		case 200:
			ExecuteRpc(photonEvent[245] as object[], photonPlayer);
			return;
		case 201:
		case 206:
		{
			ExitGames.Client.Photon.Hashtable hashtable3 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int networkTime = (int)hashtable3[(byte)0];
			short correctPrefix = -1;
			short num6 = 1;
			if (hashtable3.ContainsKey((byte)1))
			{
				correctPrefix = (short)hashtable3[(byte)1];
				num6 = 2;
			}
			for (short num7 = num6; num7 < hashtable3.Count; num7 = (short)(num7 + 1))
			{
				OnSerializeRead(hashtable3[num7] as object[], photonPlayer, networkTime, correctPrefix);
			}
			return;
		}
		case 202:
			DoInstantiate((ExitGames.Client.Photon.Hashtable)photonEvent[245], photonPlayer, null);
			return;
		case 203:
			if (photonPlayer == null || !photonPlayer.isMasterClient)
			{
				Debug.LogError(string.Concat("Error: Someone else(", photonPlayer, ") then the masterserver requests a disconnect!"));
			}
			else
			{
				PhotonNetwork.LeaveRoom();
			}
			return;
		case 207:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int num8 = (int)hashtable2[(byte)0];
			if (num8 >= 0)
			{
				DestroyPlayerObjects(num8, true);
				return;
			}
			if ((int)base.DebugOut >= 3)
			{
				Debug.Log("Ev DestroyAll! By PlayerId: " + num);
			}
			DestroyAll(true);
			return;
		}
		case 204:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int num5 = (int)hashtable2[(byte)0];
			PhotonView value = null;
			if (photonViewList.TryGetValue(num5, out value))
			{
				RemoveInstantiatedGO(value.gameObject, true);
			}
			else if ((int)base.DebugOut >= 1)
			{
				Debug.LogError("Ev Destroy Failed. Could not find PhotonView with instantiationId " + num5 + ". Sent by actorNr: " + num);
			}
			return;
		}
		case 208:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int playerId = (int)hashtable2[(byte)1];
			SetMasterClient(playerId, false);
			return;
		}
		case 224:
		{
			string[] array2 = photonEvent[213] as string[];
			byte[] array3 = photonEvent[212] as byte[];
			int[] array4 = photonEvent[229] as int[];
			int[] array5 = photonEvent[228] as int[];
			LobbyStatistics.Clear();
			for (int i = 0; i < array2.Length; i++)
			{
				TypedLobbyInfo typedLobbyInfo = new TypedLobbyInfo();
				typedLobbyInfo.Name = array2[i];
				typedLobbyInfo.Type = (LobbyType)array3[i];
				typedLobbyInfo.PlayerCount = array4[i];
				typedLobbyInfo.RoomCount = array5[i];
				LobbyStatistics.Add(typedLobbyInfo);
			}
			SendMonoMessage(PhotonNetworkingMessage.OnLobbyStatisticsUpdate);
			return;
		}
		case 251:
			if (PhotonNetwork.OnEventCall != null)
			{
				object content = photonEvent[245];
				PhotonNetwork.OnEventCall(photonEvent.Code, content, num);
			}
			else
			{
				Debug.LogWarning("Warning: Unhandled Event ErrorInfo (251). Set PhotonNetwork.OnEventCall to the method PUN should call for this event.");
			}
			return;
		}
		if (photonEvent.Code < 200)
		{
			if (PhotonNetwork.OnEventCall != null)
			{
				object content2 = photonEvent[245];
				PhotonNetwork.OnEventCall(photonEvent.Code, content2, num);
			}
			else
			{
				Debug.LogWarning(string.Concat("Warning: Unhandled event ", photonEvent, ". Set PhotonNetwork.OnEventCall."));
			}
		}
	}

	protected internal void UpdatedActorList(int[] actorsInRoom)
	{
		foreach (int num in actorsInRoom)
		{
			if (mLocalActor.ID != num && !mActors.ContainsKey(num))
			{
				AddNewPlayer(num, new PhotonPlayer(false, num, string.Empty));
			}
		}
	}

	private void SendVacantViewIds()
	{
		Debug.Log("SendVacantViewIds()");
		List<int> list = new List<int>();
		foreach (PhotonView value in photonViewList.Values)
		{
			if (!value.isOwnerActive)
			{
				list.Add(value.viewID);
			}
		}
		Debug.Log("Sending vacant view IDs. Length: " + list.Count);
		OpRaiseEvent(211, list.ToArray(), true, null);
	}

	public static void SendMonoMessage(PhotonNetworkingMessage methodString, params object[] parameters)
	{
		HashSet<GameObject> hashSet = ((PhotonNetwork.SendMonoMessageTargets == null) ? PhotonNetwork.FindGameObjectsWithComponent(PhotonNetwork.SendMonoMessageTargetType) : PhotonNetwork.SendMonoMessageTargets);
		string methodName = methodString.ToString();
		object value = ((parameters == null || parameters.Length != 1) ? parameters : parameters[0]);
		foreach (GameObject item in hashSet)
		{
			if (item != null)
			{
				item.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	protected internal void ExecuteRpc(object[] rpcData, PhotonPlayer sender)
	{
		if (rpcData == null)
		{
			Debug.LogError("Malformed RPC; this should never occur. Content: " + LogObjectArray(rpcData));
			return;
		}
		int num = (int)rpcData[0];
		int num2 = 0;
		if (rpcData[1] != null)
		{
			num2 = (short)rpcData[1];
		}
		string text;
		if (rpcData[5] != null)
		{
			int num3 = (byte)rpcData[5];
			if (num3 > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
			{
				Debug.LogError("Could not find RPC with index: " + num3 + ". Going to ignore! Check PhotonServerSettings.RpcList");
				return;
			}
			text = PhotonNetwork.PhotonServerSettings.RpcList[num3];
		}
		else
		{
			text = (string)rpcData[3];
		}
		if (Defs.inComingMessagesCounter > 5 && Defs.unimportantRPCList.Contains(text))
		{
			return;
		}
		object[] array = (object[])rpcData[4];
		if (array == null)
		{
			array = new object[0];
		}
		PhotonView photonView = GetPhotonView(num);
		if (photonView == null)
		{
			int num4 = num / PhotonNetwork.MAX_VIEW_IDS;
			bool flag = num4 == mLocalActor.ID;
			bool flag2 = num4 == sender.ID;
			if (flag)
			{
				Debug.LogWarning("Received RPC \"" + text + "\" for viewID " + num + " but this PhotonView does not exist! View was/is ours." + ((!flag2) ? " Remote called." : " Owner called.") + " By: " + sender.ID);
			}
			else
			{
				Debug.LogWarning("Received RPC \"" + text + "\" for viewID " + num + " but this PhotonView does not exist! Was remote PV." + ((!flag2) ? " Remote called." : " Owner called.") + " By: " + sender.ID + " Maybe GO was destroyed but RPC not cleaned up.");
			}
			return;
		}
		if (photonView.prefix != num2)
		{
			Debug.LogError("Received RPC \"" + text + "\" on viewID " + num + " with a prefix of " + num2 + ", our prefix is " + photonView.prefix + ". The RPC has been ignored.");
			return;
		}
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError("Malformed RPC; this should never occur. Content: " + LogObjectArray(rpcData));
			return;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log("Received RPC: " + text);
		}
		if (photonView.group != 0 && !allowedReceivingGroups.Contains(photonView.group))
		{
			return;
		}
		Type[] array2 = new Type[0];
		if (array.Length > 0)
		{
			array2 = new Type[array.Length];
			int num5 = 0;
			foreach (object obj in array)
			{
				if (obj == null)
				{
					array2[num5] = null;
				}
				else
				{
					array2[num5] = obj.GetType();
				}
				num5++;
			}
		}
		int num6 = 0;
		int num7 = 0;
		if (!PhotonNetwork.UseRpcMonoBehaviourCache || photonView.RpcMonoBehaviours == null || photonView.RpcMonoBehaviours.Length == 0)
		{
			photonView.RefreshRpcMonoBehaviourCache();
		}
		for (int j = 0; j < photonView.RpcMonoBehaviours.Length; j++)
		{
			MonoBehaviour monoBehaviour = photonView.RpcMonoBehaviours[j];
			if (monoBehaviour == null)
			{
				Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
				continue;
			}
			Type type = monoBehaviour.GetType();
			List<MethodInfo> value = null;
			if (!monoRPCMethodsCache.TryGetValue(type, out value))
			{
				List<MethodInfo> methods = SupportClass.GetMethods(type, typeof(PunRPC));
				monoRPCMethodsCache[type] = methods;
				value = methods;
			}
			if (value == null)
			{
				continue;
			}
			for (int k = 0; k < value.Count; k++)
			{
				MethodInfo methodInfo = value[k];
				if (!methodInfo.Name.Equals(text))
				{
					continue;
				}
				num7++;
				ParameterInfo[] cachedParemeters = methodInfo.GetCachedParemeters();
				if (cachedParemeters.Length == array2.Length)
				{
					if (CheckTypeMatch(cachedParemeters, array2))
					{
						num6++;
						object obj2 = methodInfo.Invoke(monoBehaviour, array);
						if (PhotonNetwork.StartRpcsAsCoroutine && methodInfo.ReturnType == typeof(IEnumerator))
						{
							monoBehaviour.StartCoroutine((IEnumerator)obj2);
						}
					}
				}
				else if (cachedParemeters.Length - 1 == array2.Length)
				{
					if (CheckTypeMatch(cachedParemeters, array2) && cachedParemeters[cachedParemeters.Length - 1].ParameterType == typeof(PhotonMessageInfo))
					{
						num6++;
						int timestamp = (int)rpcData[2];
						object[] array3 = new object[array.Length + 1];
						array.CopyTo(array3, 0);
						array3[array3.Length - 1] = new PhotonMessageInfo(sender, timestamp, photonView);
						object obj3 = methodInfo.Invoke(monoBehaviour, array3);
						if (PhotonNetwork.StartRpcsAsCoroutine && methodInfo.ReturnType == typeof(IEnumerator))
						{
							monoBehaviour.StartCoroutine((IEnumerator)obj3);
						}
					}
				}
				else if (cachedParemeters.Length == 1 && cachedParemeters[0].ParameterType.IsArray)
				{
					num6++;
					object obj4 = methodInfo.Invoke(monoBehaviour, new object[1] { array });
					if (PhotonNetwork.StartRpcsAsCoroutine && methodInfo.ReturnType == typeof(IEnumerator))
					{
						monoBehaviour.StartCoroutine((IEnumerator)obj4);
					}
				}
			}
		}
		if (num6 == 1)
		{
			return;
		}
		string text2 = string.Empty;
		foreach (Type type2 in array2)
		{
			if (text2 != string.Empty)
			{
				text2 += ", ";
			}
			text2 = ((type2 != null) ? (text2 + type2.Name) : (text2 + "null"));
		}
		if (num6 == 0)
		{
			if (num7 == 0)
			{
				Debug.LogError("PhotonView with ID " + num + " has no method \"" + text + "\" marked with the [PunRPC](C#) or @PunRPC(JS) property! Args: " + text2);
			}
			else
			{
				Debug.LogError("PhotonView with ID " + num + " has no method \"" + text + "\" that takes " + array2.Length + " argument(s): " + text2);
			}
		}
		else
		{
			Debug.LogError("PhotonView with ID " + num + " has " + num6 + " methods \"" + text + "\" that takes " + array2.Length + " argument(s): " + text2 + ". Should be just one?");
		}
	}

	private bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
	{
		if (methodParameters.Length < callParameterTypes.Length)
		{
			return false;
		}
		for (int i = 0; i < callParameterTypes.Length; i++)
		{
			Type parameterType = methodParameters[i].ParameterType;
			if (callParameterTypes[i] != null && !parameterType.IsAssignableFrom(callParameterTypes[i]) && (!parameterType.IsEnum || !Enum.GetUnderlyingType(parameterType).IsAssignableFrom(callParameterTypes[i])))
			{
				return false;
			}
		}
		return true;
	}

	internal ExitGames.Client.Photon.Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, int group, int[] viewIDs, object[] data, bool isGlobalObject)
	{
		int num = viewIDs[0];
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = prefabName;
		if (position != Vector3.zero)
		{
			hashtable[(byte)1] = position;
		}
		if (rotation != Quaternion.identity)
		{
			hashtable[(byte)2] = rotation;
		}
		if (group != 0)
		{
			hashtable[(byte)3] = group;
		}
		if (viewIDs.Length > 1)
		{
			hashtable[(byte)4] = viewIDs;
		}
		if (data != null)
		{
			hashtable[(byte)5] = data;
		}
		if (currentLevelPrefix > 0)
		{
			hashtable[(byte)8] = currentLevelPrefix;
		}
		hashtable[(byte)6] = PhotonNetwork.ServerTimestamp;
		hashtable[(byte)7] = num;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = ((!isGlobalObject) ? EventCaching.AddToRoomCache : EventCaching.AddToRoomCacheGlobal);
		OpRaiseEvent(202, hashtable, true, raiseEventOptions);
		return hashtable;
	}

	internal GameObject DoInstantiate(ExitGames.Client.Photon.Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject)
	{
		string text = (string)evData[(byte)0];
		int timestamp = (int)evData[(byte)6];
		int num = (int)evData[(byte)7];
		Vector3 position = ((!evData.ContainsKey((byte)1)) ? Vector3.zero : ((Vector3)evData[(byte)1]));
		Quaternion rotation = Quaternion.identity;
		if (evData.ContainsKey((byte)2))
		{
			rotation = (Quaternion)evData[(byte)2];
		}
		int num2 = 0;
		if (evData.ContainsKey((byte)3))
		{
			num2 = (int)evData[(byte)3];
		}
		short prefix = 0;
		if (evData.ContainsKey((byte)8))
		{
			prefix = (short)evData[(byte)8];
		}
		int[] array = ((!evData.ContainsKey((byte)4)) ? new int[1] { num } : ((int[])evData[(byte)4]));
		object[] array2 = ((!evData.ContainsKey((byte)5)) ? null : ((object[])evData[(byte)5]));
		if (num2 != 0 && !allowedReceivingGroups.Contains(num2))
		{
			return null;
		}
		if (ObjectPool != null)
		{
			GameObject gameObject = ObjectPool.Instantiate(text, position, rotation);
			PhotonView[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
			if (photonViewsInChildren.Length != array.Length)
			{
				throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
			}
			for (int i = 0; i < photonViewsInChildren.Length; i++)
			{
				photonViewsInChildren[i].didAwake = false;
				photonViewsInChildren[i].viewID = 0;
				photonViewsInChildren[i].prefix = prefix;
				photonViewsInChildren[i].instantiationId = num;
				photonViewsInChildren[i].isRuntimeInstantiated = true;
				photonViewsInChildren[i].instantiationDataField = array2;
				photonViewsInChildren[i].didAwake = true;
				photonViewsInChildren[i].viewID = array[i];
			}
			gameObject.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, timestamp, null), SendMessageOptions.DontRequireReceiver);
			return gameObject;
		}
		if (resourceGameObject == null)
		{
			if (!UsePrefabCache || !PrefabCache.TryGetValue(text, out resourceGameObject))
			{
				resourceGameObject = (GameObject)Resources.Load(text, typeof(GameObject));
				if (UsePrefabCache)
				{
					PrefabCache.Add(text, resourceGameObject);
				}
			}
			if (resourceGameObject == null)
			{
				Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + text + "]. Please verify you have this gameobject in a Resources folder.");
				return null;
			}
		}
		PhotonView[] photonViewsInChildren2 = resourceGameObject.GetPhotonViewsInChildren();
		if (photonViewsInChildren2.Length != array.Length)
		{
			throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
		}
		for (int j = 0; j < array.Length; j++)
		{
			photonViewsInChildren2[j].viewID = array[j];
			photonViewsInChildren2[j].prefix = prefix;
			photonViewsInChildren2[j].instantiationId = num;
			photonViewsInChildren2[j].isRuntimeInstantiated = true;
		}
		StoreInstantiationData(num, array2);
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(resourceGameObject, position, rotation);
		for (int k = 0; k < array.Length; k++)
		{
			photonViewsInChildren2[k].viewID = 0;
			photonViewsInChildren2[k].prefix = -1;
			photonViewsInChildren2[k].prefixBackup = -1;
			photonViewsInChildren2[k].instantiationId = -1;
			photonViewsInChildren2[k].isRuntimeInstantiated = false;
		}
		RemoveInstantiationData(num);
		gameObject2.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, timestamp, null), SendMessageOptions.DontRequireReceiver);
		return gameObject2;
	}

	private void StoreInstantiationData(int instantiationId, object[] instantiationData)
	{
		tempInstantiationData[instantiationId] = instantiationData;
	}

	public object[] FetchInstantiationData(int instantiationId)
	{
		object[] value = null;
		if (instantiationId == 0)
		{
			return null;
		}
		tempInstantiationData.TryGetValue(instantiationId, out value);
		return value;
	}

	private void RemoveInstantiationData(int instantiationId)
	{
		tempInstantiationData.Remove(instantiationId);
	}

	public void DestroyPlayerObjects(int playerId, bool localOnly)
	{
		if (playerId <= 0)
		{
			Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
			return;
		}
		if (!localOnly)
		{
			OpRemoveFromServerInstantiationsOfPlayer(playerId);
			OpCleanRpcBuffer(playerId);
			SendDestroyOfPlayer(playerId);
		}
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		foreach (PhotonView value in photonViewList.Values)
		{
			if (value.CreatorActorNr == playerId)
			{
				hashSet.Add(value.gameObject);
			}
		}
		foreach (GameObject item in hashSet)
		{
			RemoveInstantiatedGO(item, true);
		}
		foreach (PhotonView value2 in photonViewList.Values)
		{
			if (value2.ownerId == playerId)
			{
				value2.ownerId = value2.CreatorActorNr;
			}
		}
	}

	public void DestroyAll(bool localOnly)
	{
		if (!localOnly)
		{
			OpRemoveCompleteCache();
			SendDestroyOfAll();
		}
		LocalCleanupAnythingInstantiated(true);
	}

	protected internal void RemoveInstantiatedGO(GameObject go, bool localOnly)
	{
		if (go == null)
		{
			Debug.LogError("Failed to 'network-remove' GameObject because it's null.");
			return;
		}
		PhotonView[] componentsInChildren = go.GetComponentsInChildren<PhotonView>(true);
		if (componentsInChildren == null || componentsInChildren.Length <= 0)
		{
			Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
			return;
		}
		PhotonView photonView = componentsInChildren[0];
		int creatorActorNr = photonView.CreatorActorNr;
		int instantiationId = photonView.instantiationId;
		if (!localOnly)
		{
			if (!photonView.isMine)
			{
				Debug.LogError("Failed to 'network-remove' GameObject. Client is neither owner nor masterClient taking over for owner who left: " + photonView);
				return;
			}
			if (instantiationId < 1)
			{
				Debug.LogError(string.Concat("Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: ", photonView, ". Not Destroying GameObject or PhotonViews!"));
				return;
			}
		}
		if (!localOnly)
		{
			ServerCleanInstantiateAndDestroy(instantiationId, creatorActorNr, photonView.isRuntimeInstantiated);
		}
		for (int num = componentsInChildren.Length - 1; num >= 0; num--)
		{
			PhotonView photonView2 = componentsInChildren[num];
			if (!(photonView2 == null))
			{
				if (photonView2.instantiationId >= 1)
				{
					LocalCleanPhotonView(photonView2);
				}
				if (!localOnly)
				{
					OpCleanRpcBuffer(photonView2);
				}
			}
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log("Network destroy Instantiated GO: " + go.name);
		}
		if (ObjectPool != null)
		{
			PhotonView[] photonViewsInChildren = go.GetPhotonViewsInChildren();
			for (int i = 0; i < photonViewsInChildren.Length; i++)
			{
				photonViewsInChildren[i].viewID = 0;
			}
			ObjectPool.Destroy(go);
		}
		else
		{
			UnityEngine.Object.Destroy(go);
		}
	}

	public int GetInstantiatedObjectsId(GameObject go)
	{
		int result = -1;
		if (go == null)
		{
			Debug.LogError("GetInstantiatedObjectsId() for GO == null.");
			return result;
		}
		PhotonView[] photonViewsInChildren = go.GetPhotonViewsInChildren();
		if (photonViewsInChildren != null && photonViewsInChildren.Length > 0 && photonViewsInChildren[0] != null)
		{
			return photonViewsInChildren[0].instantiationId;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log("GetInstantiatedObjectsId failed for GO: " + go);
		}
		return result;
	}

	private void ServerCleanInstantiateAndDestroy(int instantiateId, int creatorId, bool isRuntimeInstantiated)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)7] = instantiateId;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.TargetActors = new int[1] { creatorId };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(202, hashtable, true, raiseEventOptions2);
		ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
		hashtable2[(byte)0] = instantiateId;
		raiseEventOptions2 = null;
		if (!isRuntimeInstantiated)
		{
			raiseEventOptions2 = new RaiseEventOptions();
			raiseEventOptions2.CachingOption = EventCaching.AddToRoomCacheGlobal;
			Debug.Log("Destroying GO as global. ID: " + instantiateId);
		}
		OpRaiseEvent(204, hashtable2, true, raiseEventOptions2);
	}

	private void SendDestroyOfPlayer(int actorNr)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = actorNr;
		OpRaiseEvent(207, hashtable, true, null);
	}

	private void SendDestroyOfAll()
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = -1;
		OpRaiseEvent(207, hashtable, true, null);
	}

	private void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.TargetActors = new int[1] { actorNr };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(202, null, true, raiseEventOptions2);
	}

	protected internal void RequestOwnership(int viewID, int fromOwner)
	{
		Debug.Log("RequestOwnership(): " + viewID + " from: " + fromOwner + " Time: " + Environment.TickCount % 1000);
		OpRaiseEvent(209, new int[2] { viewID, fromOwner }, true, new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		});
	}

	protected internal void TransferOwnership(int viewID, int playerID)
	{
		Debug.Log("TransferOwnership() view " + viewID + " to: " + playerID + " Time: " + Environment.TickCount % 1000);
		OpRaiseEvent(210, new int[2] { viewID, playerID }, true, new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		});
	}

	public bool LocalCleanPhotonView(PhotonView view)
	{
		view.removedFromLocalViewList = true;
		return photonViewList.Remove(view.viewID);
	}

	public PhotonView GetPhotonView(int viewID)
	{
		PhotonView value = null;
		photonViewList.TryGetValue(viewID, out value);
		if (value == null)
		{
			PhotonView[] array = UnityEngine.Object.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];
			PhotonView[] array2 = array;
			foreach (PhotonView photonView in array2)
			{
				if (photonView.viewID == viewID)
				{
					if (photonView.didAwake)
					{
						Debug.LogWarning("Had to lookup view that wasn't in photonViewList: " + photonView);
					}
					return photonView;
				}
			}
		}
		return value;
	}

	public void RegisterPhotonView(PhotonView netView)
	{
		if (!Application.isPlaying)
		{
			photonViewList = new Dictionary<int, PhotonView>();
			return;
		}
		if (netView.viewID == 0)
		{
			Debug.Log("PhotonView register is ignored, because viewID is 0. No id assigned yet to: " + netView);
			return;
		}
		PhotonView value = null;
		if (photonViewList.TryGetValue(netView.viewID, out value))
		{
			if (!(netView != value))
			{
				return;
			}
			Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.viewID, netView, value));
			RemoveInstantiatedGO(value.gameObject, true);
		}
		photonViewList.Add(netView.viewID, netView);
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log("Registered PhotonView: " + netView.viewID);
		}
	}

	public void OpCleanRpcBuffer(int actorNumber)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.TargetActors = new int[1] { actorNumber };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(200, null, true, raiseEventOptions2);
	}

	public void OpRemoveCompleteCacheOfPlayer(int actorNumber)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.TargetActors = new int[1] { actorNumber };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(0, null, true, raiseEventOptions2);
	}

	public void OpRemoveCompleteCache()
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		raiseEventOptions.Receivers = ReceiverGroup.MasterClient;
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(0, null, true, raiseEventOptions2);
	}

	private void RemoveCacheOfLeftPlayers()
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[244] = (byte)0;
		dictionary[247] = (byte)7;
		OpCustom(253, dictionary, true, 0);
	}

	public void CleanRpcBufferIfMine(PhotonView view)
	{
		if (view.ownerId != mLocalActor.ID && !mLocalActor.isMasterClient)
		{
			Debug.LogError(string.Concat("Cannot remove cached RPCs on a PhotonView thats not ours! ", view.owner, " scene: ", view.isSceneView));
		}
		else
		{
			OpCleanRpcBuffer(view);
		}
	}

	public void OpCleanRpcBuffer(PhotonView view)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = view.viewID;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		OpRaiseEvent(200, hashtable, true, raiseEventOptions2);
	}

	public void RemoveRPCsInGroup(int group)
	{
		foreach (KeyValuePair<int, PhotonView> photonView in photonViewList)
		{
			PhotonView value = photonView.Value;
			if (value.group == group)
			{
				CleanRpcBufferIfMine(value);
			}
		}
	}

	public void SetLevelPrefix(short prefix)
	{
		currentLevelPrefix = prefix;
	}

	internal void RPC(PhotonView view, string methodName, PhotonPlayer player, bool encrypt, params object[] parameters)
	{
		if (!blockSendingGroups.Contains(view.group))
		{
			if (view.viewID < 1)
			{
				Debug.LogError("Illegal view ID:" + view.viewID + " method: " + methodName + " GO:" + view.gameObject.name);
			}
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
			{
				Debug.Log(string.Concat("Sending RPC \"", methodName, "\" to player[", player, "]"));
			}
			object[] array = new object[6] { view.viewID, null, null, null, null, null };
			if (view.prefix > 0)
			{
				array[1] = (short)view.prefix;
			}
			array[2] = PhotonNetwork.ServerTimestamp;
			int value = 0;
			if (rpcShortcuts.TryGetValue(methodName, out value))
			{
				array[5] = (byte)value;
			}
			else
			{
				array[3] = methodName;
			}
			if (parameters != null && parameters.Length > 0)
			{
				array[4] = parameters;
			}
			if (mLocalActor.ID == player.ID)
			{
				ExecuteRpc(array, player);
				return;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.TargetActors = new int[1] { player.ID };
			raiseEventOptions.Encrypt = encrypt;
			RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
			OpRaiseEvent(200, array, true, raiseEventOptions2);
		}
	}

	internal void RPC(PhotonView view, string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
	{
		if (blockSendingGroups.Contains(view.group))
		{
			return;
		}
		if (view.viewID < 1)
		{
			Debug.LogError("Illegal view ID:" + view.viewID + " method: " + methodName + " GO:" + view.gameObject.name);
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log("Sending RPC \"" + methodName + "\" to " + target);
		}
		object[] array = new object[6] { view.viewID, null, null, null, null, null };
		if (view.prefix > 0)
		{
			array[1] = (short)view.prefix;
		}
		array[2] = PhotonNetwork.ServerTimestamp;
		int value = 0;
		if (rpcShortcuts.TryGetValue(methodName, out value))
		{
			array[5] = (byte)value;
		}
		else
		{
			array[3] = methodName;
		}
		if (parameters != null && parameters.Length > 0)
		{
			array[4] = parameters;
		}
		switch (target)
		{
		case PhotonTargets.All:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.InterestGroup = (byte)view.group;
			raiseEventOptions.Encrypt = encrypt;
			RaiseEventOptions raiseEventOptions8 = raiseEventOptions;
			OpRaiseEvent(200, array, true, raiseEventOptions8);
			ExecuteRpc(array, mLocalActor);
			break;
		}
		case PhotonTargets.Others:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.InterestGroup = (byte)view.group;
			raiseEventOptions.Encrypt = encrypt;
			RaiseEventOptions raiseEventOptions7 = raiseEventOptions;
			OpRaiseEvent(200, array, true, raiseEventOptions7);
			break;
		}
		case PhotonTargets.AllBuffered:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
			raiseEventOptions.Encrypt = encrypt;
			RaiseEventOptions raiseEventOptions5 = raiseEventOptions;
			OpRaiseEvent(200, array, true, raiseEventOptions5);
			ExecuteRpc(array, mLocalActor);
			break;
		}
		case PhotonTargets.OthersBuffered:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
			raiseEventOptions.Encrypt = encrypt;
			RaiseEventOptions raiseEventOptions3 = raiseEventOptions;
			OpRaiseEvent(200, array, true, raiseEventOptions3);
			break;
		}
		case PhotonTargets.MasterClient:
		{
			if (mMasterClientId == mLocalActor.ID)
			{
				ExecuteRpc(array, mLocalActor);
				break;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.Receivers = ReceiverGroup.MasterClient;
			raiseEventOptions.Encrypt = encrypt;
			RaiseEventOptions raiseEventOptions6 = raiseEventOptions;
			OpRaiseEvent(200, array, true, raiseEventOptions6);
			break;
		}
		case PhotonTargets.AllViaServer:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.InterestGroup = (byte)view.group;
			raiseEventOptions.Receivers = ReceiverGroup.All;
			raiseEventOptions.Encrypt = encrypt;
			RaiseEventOptions raiseEventOptions4 = raiseEventOptions;
			OpRaiseEvent(200, array, true, raiseEventOptions4);
			if (PhotonNetwork.offlineMode)
			{
				ExecuteRpc(array, mLocalActor);
			}
			break;
		}
		case PhotonTargets.AllBufferedViaServer:
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.InterestGroup = (byte)view.group;
			raiseEventOptions.Receivers = ReceiverGroup.All;
			raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
			raiseEventOptions.Encrypt = encrypt;
			RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
			OpRaiseEvent(200, array, true, raiseEventOptions2);
			if (PhotonNetwork.offlineMode)
			{
				ExecuteRpc(array, mLocalActor);
			}
			break;
		}
		default:
			Debug.LogError("Unsupported target enum: " + target);
			break;
		}
	}

	public void SetReceivingEnabled(int group, bool enabled)
	{
		if (group <= 0)
		{
			Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + group + ". The group number should be at least 1.");
		}
		else if (enabled)
		{
			if (!allowedReceivingGroups.Contains(group))
			{
				allowedReceivingGroups.Add(group);
				byte[] groupsToAdd = new byte[1] { (byte)group };
				OpChangeGroups(null, groupsToAdd);
			}
		}
		else if (allowedReceivingGroups.Contains(group))
		{
			allowedReceivingGroups.Remove(group);
			byte[] groupsToRemove = new byte[1] { (byte)group };
			OpChangeGroups(groupsToRemove, null);
		}
	}

	public void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
	{
		List<byte> list = new List<byte>();
		List<byte> list2 = new List<byte>();
		if (enableGroups != null)
		{
			foreach (int num in enableGroups)
			{
				if (num <= 0)
				{
					Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + num + ". The group number should be at least 1.");
				}
				else if (!allowedReceivingGroups.Contains(num))
				{
					allowedReceivingGroups.Add(num);
					list.Add((byte)num);
				}
			}
		}
		if (disableGroups != null)
		{
			foreach (int num2 in disableGroups)
			{
				if (num2 <= 0)
				{
					Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + num2 + ". The group number should be at least 1.");
				}
				else if (list.Contains((byte)num2))
				{
					Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled disableGroups contains a group that is also in the enableGroups: " + num2 + ".");
				}
				else if (allowedReceivingGroups.Contains(num2))
				{
					allowedReceivingGroups.Remove(num2);
					list2.Add((byte)num2);
				}
			}
		}
		OpChangeGroups((list2.Count <= 0) ? null : list2.ToArray(), (list.Count <= 0) ? null : list.ToArray());
	}

	public void SetSendingEnabled(int group, bool enabled)
	{
		if (!enabled)
		{
			blockSendingGroups.Add(group);
		}
		else
		{
			blockSendingGroups.Remove(group);
		}
	}

	public void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
	{
		if (enableGroups != null)
		{
			foreach (int item in enableGroups)
			{
				if (blockSendingGroups.Contains(item))
				{
					blockSendingGroups.Remove(item);
				}
			}
		}
		if (disableGroups == null)
		{
			return;
		}
		foreach (int item2 in disableGroups)
		{
			if (!blockSendingGroups.Contains(item2))
			{
				blockSendingGroups.Add(item2);
			}
		}
	}

	public void NewSceneLoaded()
	{
		if (loadingLevelAndPausedNetwork)
		{
			loadingLevelAndPausedNetwork = false;
			PhotonNetwork.isMessageQueueRunning = true;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, PhotonView> photonView in photonViewList)
		{
			PhotonView value = photonView.Value;
			if (value == null)
			{
				list.Add(photonView.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			int key = list[i];
			photonViewList.Remove(key);
		}
		if (list.Count > 0 && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log("New level loaded. Removed " + list.Count + " scene view IDs from last level.");
		}
	}

	public void RunViewUpdate()
	{
		if (!PhotonNetwork.connected || PhotonNetwork.offlineMode || mActors == null || mActors.Count <= 1)
		{
			return;
		}
		dataPerGroupReliable.Clear();
		dataPerGroupUnreliable.Clear();
		foreach (KeyValuePair<int, PhotonView> photonView in photonViewList)
		{
			PhotonView value = photonView.Value;
			if (value.synchronization == ViewSynchronization.Off || !value.isMine || !value.gameObject.activeInHierarchy || blockSendingGroups.Contains(value.group))
			{
				continue;
			}
			object[] array = OnSerializeWrite(value);
			if (array == null)
			{
				continue;
			}
			if (value.synchronization == ViewSynchronization.ReliableDeltaCompressed || value.mixedModeIsReliable)
			{
				ExitGames.Client.Photon.Hashtable value2 = null;
				if (!dataPerGroupReliable.TryGetValue(value.group, out value2))
				{
					value2 = new ExitGames.Client.Photon.Hashtable(4);
					value2[(byte)0] = PhotonNetwork.ServerTimestamp;
					if (currentLevelPrefix >= 0)
					{
						value2[(byte)1] = currentLevelPrefix;
					}
					dataPerGroupReliable[value.group] = value2;
				}
				value2.Add((short)value2.Count, array);
				continue;
			}
			ExitGames.Client.Photon.Hashtable value3 = null;
			if (!dataPerGroupUnreliable.TryGetValue(value.group, out value3))
			{
				value3 = new ExitGames.Client.Photon.Hashtable(4);
				value3[(byte)0] = PhotonNetwork.ServerTimestamp;
				if (currentLevelPrefix >= 0)
				{
					value3[(byte)1] = currentLevelPrefix;
				}
				dataPerGroupUnreliable[value.group] = value3;
			}
			value3.Add((short)value3.Count, array);
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> item in dataPerGroupReliable)
		{
			raiseEventOptions.InterestGroup = (byte)item.Key;
			OpRaiseEvent(206, item.Value, true, raiseEventOptions);
		}
		foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> item2 in dataPerGroupUnreliable)
		{
			raiseEventOptions.InterestGroup = (byte)item2.Key;
			OpRaiseEvent(201, item2.Value, false, raiseEventOptions);
		}
	}

	private object[] OnSerializeWrite(PhotonView view)
	{
		if (view.synchronization == ViewSynchronization.Off)
		{
			return null;
		}
		PhotonMessageInfo info = new PhotonMessageInfo(mLocalActor, PhotonNetwork.ServerTimestamp, view);
		pStream.ResetWriteStream();
		pStream.SendNext(view.viewID);
		pStream.SendNext(false);
		pStream.SendNext(null);
		view.SerializeView(pStream, info);
		if (pStream.Count <= 3)
		{
			return null;
		}
		if (view.synchronization == ViewSynchronization.Unreliable)
		{
			return pStream.ToArray();
		}
		object[] array = pStream.ToArray();
		if (view.synchronization == ViewSynchronization.UnreliableOnChange)
		{
			if (AlmostEquals(array, view.lastOnSerializeDataSent))
			{
				if (view.mixedModeIsReliable)
				{
					return null;
				}
				view.mixedModeIsReliable = true;
				view.lastOnSerializeDataSent = array;
			}
			else
			{
				view.mixedModeIsReliable = false;
				view.lastOnSerializeDataSent = array;
			}
			return array;
		}
		if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
		{
			object[] result = DeltaCompressionWrite(view.lastOnSerializeDataSent, array);
			view.lastOnSerializeDataSent = array;
			return result;
		}
		return null;
	}

	private string LogObjectArray(object[] data)
	{
		string[] array = new string[data.Length];
		for (int i = 0; i < data.Length; i++)
		{
			object obj = data[i];
			array[i] = ((obj == null) ? "null" : obj.ToString());
		}
		return string.Join(", ", array);
	}

	private void OnSerializeRead(object[] data, PhotonPlayer sender, int networkTime, short correctPrefix)
	{
		int num = (int)data[0];
		PhotonView photonView = GetPhotonView(num);
		if (photonView == null)
		{
			Debug.LogWarning("Received OnSerialization for view ID " + num + ". We have no such PhotonView! Ignored this if you're leaving a room. State: " + State);
		}
		else if (photonView.prefix > 0 && correctPrefix != photonView.prefix)
		{
			Debug.LogError("Received OnSerialization for view ID " + num + " with prefix " + correctPrefix + ". Our prefix is " + photonView.prefix);
		}
		else
		{
			if (photonView.group != 0 && !allowedReceivingGroups.Contains(photonView.group))
			{
				return;
			}
			if (photonView.synchronization == ViewSynchronization.ReliableDeltaCompressed)
			{
				object[] array = DeltaCompressionRead(photonView.lastOnSerializeDataReceived, data);
				if (array == null)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						Debug.Log("Skipping packet for " + photonView.name + " [" + photonView.viewID + "] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game.");
					}
					return;
				}
				photonView.lastOnSerializeDataReceived = array;
				data = array;
			}
			if (sender.ID != photonView.ownerId && (!photonView.isSceneView || !sender.isMasterClient))
			{
				Debug.Log("Adjusting owner to sender of updates. From: " + photonView.ownerId + " to: " + sender.ID);
				photonView.ownerId = sender.ID;
			}
			PhotonStream photonStream = new PhotonStream(false, data);
			photonStream.currentItem = 3;
			PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, photonView);
			photonView.DeserializeView(photonStream, info);
		}
	}

	private bool AlmostEquals(object[] lastData, object[] currentContent)
	{
		if (lastData == null && currentContent == null)
		{
			return true;
		}
		if (lastData == null || currentContent == null || lastData.Length != currentContent.Length)
		{
			return false;
		}
		for (int i = 0; i < currentContent.Length; i++)
		{
			object one = currentContent[i];
			object two = lastData[i];
			if (!ObjectIsSameWithInprecision(one, two))
			{
				return false;
			}
		}
		return true;
	}

	private object[] DeltaCompressionWrite(object[] previousContent, object[] currentContent)
	{
		if (currentContent == null || previousContent == null || previousContent.Length != currentContent.Length)
		{
			return currentContent;
		}
		if (currentContent.Length <= 3)
		{
			return null;
		}
		object[] array = new object[currentContent.Length];
		array[1] = false;
		int num = 0;
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 3; i < currentContent.Length; i++)
		{
			object obj = currentContent[i];
			object two = previousContent[i];
			if (ObjectIsSameWithInprecision(obj, two))
			{
				num++;
				continue;
			}
			array[i] = obj;
			if (obj == null)
			{
				hashSet.Add(i);
			}
		}
		if (num > 0)
		{
			if (num == currentContent.Length - 3)
			{
				return null;
			}
			array[1] = true;
			if (hashSet.Count > 0)
			{
				array[2] = hashSet.ToArray();
			}
		}
		array[0] = currentContent[0];
		return array;
	}

	private object[] DeltaCompressionRead(object[] lastOnSerializeDataReceived, object[] incomingData)
	{
		if (!(bool)incomingData[1])
		{
			return incomingData;
		}
		if (lastOnSerializeDataReceived == null)
		{
			return null;
		}
		int[] array = incomingData[2] as int[];
		for (int i = 3; i < incomingData.Length; i++)
		{
			if ((array == null || !array.Contains(i)) && incomingData[i] == null)
			{
				object obj = (incomingData[i] = lastOnSerializeDataReceived[i]);
			}
		}
		return incomingData;
	}

	private bool ObjectIsSameWithInprecision(object one, object two)
	{
		if (one == null || two == null)
		{
			return one == null && two == null;
		}
		if (!one.Equals(two))
		{
			if (one is Vector3)
			{
				Vector3 target = (Vector3)one;
				Vector3 second = (Vector3)two;
				if (target.AlmostEquals(second, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Vector2)
			{
				Vector2 target2 = (Vector2)one;
				Vector2 second2 = (Vector2)two;
				if (target2.AlmostEquals(second2, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Quaternion)
			{
				Quaternion target3 = (Quaternion)one;
				Quaternion second3 = (Quaternion)two;
				if (target3.AlmostEquals(second3, PhotonNetwork.precisionForQuaternionSynchronization))
				{
					return true;
				}
			}
			else if (one is float)
			{
				float target4 = (float)one;
				float second4 = (float)two;
				if (target4.AlmostEquals(second4, PhotonNetwork.precisionForFloatSynchronization))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	protected internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
	{
		mi = null;
		if (monob == null || string.IsNullOrEmpty(methodType))
		{
			return false;
		}
		List<MethodInfo> methods = SupportClass.GetMethods(monob.GetType(), null);
		for (int i = 0; i < methods.Count; i++)
		{
			MethodInfo methodInfo = methods[i];
			if (methodInfo.Name.Equals(methodType))
			{
				mi = methodInfo;
				return true;
			}
		}
		return false;
	}

	protected internal void LoadLevelIfSynced()
	{
		if (!PhotonNetwork.automaticallySyncScene || PhotonNetwork.isMasterClient || PhotonNetwork.room == null || !PhotonNetwork.room.customProperties.ContainsKey("curScn"))
		{
			return;
		}
		object obj = PhotonNetwork.room.customProperties["curScn"];
		if (obj is int)
		{
			if (SceneManagerHelper.ActiveSceneBuildIndex != (int)obj)
			{
				PhotonNetwork.LoadLevel((int)obj);
			}
		}
		else if (obj is string && SceneManagerHelper.ActiveSceneName != (string)obj)
		{
			PhotonNetwork.LoadLevel((string)obj);
		}
	}

	protected internal void SetLevelInPropsIfSynced(object levelId)
	{
		if (!PhotonNetwork.automaticallySyncScene || !PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
		{
			return;
		}
		if (levelId == null)
		{
			Debug.LogError("Parameter levelId can't be null!");
			return;
		}
		if (PhotonNetwork.room.customProperties.ContainsKey("curScn"))
		{
			object obj = PhotonNetwork.room.customProperties["curScn"];
			if ((obj is int && SceneManagerHelper.ActiveSceneBuildIndex == (int)obj) || (obj is string && SceneManagerHelper.ActiveSceneName != null && SceneManagerHelper.ActiveSceneName.Equals((string)obj)))
			{
				return;
			}
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if (levelId is int)
		{
			hashtable["curScn"] = (int)levelId;
		}
		else if (levelId is string)
		{
			hashtable["curScn"] = (string)levelId;
		}
		else
		{
			Debug.LogError("Parameter levelId must be int or string!");
		}
		PhotonNetwork.room.SetCustomProperties(hashtable);
		SendOutgoingCommands();
	}

	public void SetApp(string appId, string gameVersion)
	{
		mAppId = appId.Trim();
		if (!string.IsNullOrEmpty(gameVersion))
		{
			PhotonNetwork.gameVersion = gameVersion.Trim();
		}
	}

	public bool WebRpc(string uriPath, object parameters)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(209, uriPath);
		dictionary.Add(208, parameters);
		return OpCustom(219, dictionary, true);
	}
}
