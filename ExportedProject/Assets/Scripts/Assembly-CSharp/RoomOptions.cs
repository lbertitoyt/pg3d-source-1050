using ExitGames.Client.Photon;

public class RoomOptions
{
	private bool isVisibleField = true;

	private bool isOpenField = true;

	public byte maxPlayers;

	public int PlayerTtl;

	private bool cleanupCacheOnLeaveField = PhotonNetwork.autoCleanUpPlayerObjects;

	public Hashtable customRoomProperties;

	public string[] customRoomPropertiesForLobby = new string[0];

	public string[] plugins;

	private bool suppressRoomEventsField;

	private bool publishUserIdField;

	public bool isVisible
	{
		get
		{
			return isVisibleField;
		}
		set
		{
			isVisibleField = value;
		}
	}

	public bool isOpen
	{
		get
		{
			return isOpenField;
		}
		set
		{
			isOpenField = value;
		}
	}

	public bool cleanupCacheOnLeave
	{
		get
		{
			return cleanupCacheOnLeaveField;
		}
		set
		{
			cleanupCacheOnLeaveField = value;
		}
	}

	public bool suppressRoomEvents
	{
		get
		{
			return suppressRoomEventsField;
		}
	}

	public bool publishUserId
	{
		get
		{
			return publishUserIdField;
		}
		set
		{
			publishUserIdField = value;
		}
	}
}
