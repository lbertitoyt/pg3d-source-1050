using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

internal sealed class NotificationController : MonoBehaviour
{
	private const string ScheduledNotificationsKey = "Scheduled Notifications";

	public static bool isGetEveryDayMoney;

	public static float timeStartApp;

	public bool pauserTemp;

	private float playTime;

	private float playTimeInMatch;

	public float savedPlayTime;

	public float savedPlayTimeInMatch;

	public static NotificationController instance;

	private static bool _paused;

	private readonly List<int> _notificationIds = new List<int>();

	public float currentPlayTimeMatch
	{
		get
		{
			return savedPlayTimeInMatch + playTimeInMatch;
		}
	}

	public float currentPlayTime
	{
		get
		{
			return savedPlayTime + playTime;
		}
	}

	internal static bool Paused
	{
		get
		{
			return _paused;
		}
	}

	private void Start()
	{
		ScopeLogger scopeLogger = new ScopeLogger("NotificationController.Start()", Defs.IsDeveloperBuild);
		try
		{
			if (!Load.LoadBool("bilZapuskKey"))
			{
				Save.SaveBool("bilZapuskKey", true);
			}
			else
			{
				StartCoroutine(appStart());
			}
			instance = this;
			float result;
			if (Storager.hasKey("PlayTime") && float.TryParse(Storager.getString("PlayTime", false), out result))
			{
				savedPlayTime = result;
			}
			float result2;
			if (Storager.hasKey("PlayTimeInMatch") && float.TryParse(Storager.getString("PlayTimeInMatch", false), out result2))
			{
				savedPlayTimeInMatch = result2;
			}
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	private void Update()
	{
		if (pauserTemp)
		{
			pauserTemp = false;
			_paused = true;
			PhotonNetwork.Disconnect();
		}
		if (!FriendsController.sharedController.idle)
		{
			playTime += Time.deltaTime;
			if (Initializer.Instance != null && (PhotonNetwork.room == null || PhotonNetwork.room.customProperties[ConnectSceneNGUIController.passwordProperty].Equals(string.Empty)) && !Defs.isDaterRegim && !Defs.isHunger && !Defs.isCOOP && !NetworkStartTable.LocalOrPasswordRoom())
			{
				playTimeInMatch += Time.deltaTime;
			}
		}
	}

	public void SaveTimeValues()
	{
		if (playTime > 0f)
		{
			savedPlayTime += playTime;
			Debug.Log(string.Format("PlayTime saved: {0} (+{1})", savedPlayTime, playTime));
			playTime = 0f;
			Storager.setString("PlayTime", savedPlayTime.ToString(), false);
		}
		if (playTimeInMatch > 0f)
		{
			savedPlayTimeInMatch += playTimeInMatch;
			Debug.Log(string.Format("PlayTimeInMatch saved: {0} (+{1})", savedPlayTimeInMatch, playTimeInMatch));
			playTimeInMatch = 0f;
			Storager.setString("PlayTimeInMatch", savedPlayTimeInMatch.ToString(), false);
		}
	}

	internal static void ResetPaused()
	{
		_paused = false;
	}

	private void appStop()
	{
		bool flag = BankController.Instance != null && BankController.Instance.InterfaceEnabled;
		if (PhotonNetwork.connected)
		{
			_paused = true;
		}
		int hour = DateTime.Now.Hour;
		int num = 82800;
		hour += 23;
		if (hour > 24)
		{
			hour -= 24;
		}
		int num2 = ((hour <= 16) ? (16 - hour) : (24 - hour + 16));
		num += num2 * 3600;
		DateTime now = DateTime.Now;
		DateTime dateTime = now + TimeSpan.FromHours(23.0);
		DateTime dateTime2 = ((dateTime.Hour >= 16) ? dateTime.Date.AddHours(40.0) : dateTime.Date.AddHours(16.0));
		TimeSpan timeSpan = TimeSpan.FromDays(1.0);
		int num3 = 0;
		for (int i = 0; i < num3; i++)
		{
			int num4 = num + i * 86400;
			num4 = num4 - 1800 + UnityEngine.Random.Range(0, 3600);
			DateTime dateTime3 = dateTime2 + TimeSpan.FromTicks(timeSpan.Ticks * i);
			int num5 = (int)(dateTime3 - now).TotalSeconds + UnityEngine.Random.Range(-1800, 1800);
			string empty = string.Empty;
			int item = EtceteraAndroid.scheduleNotification(num5, "Challenge", LocalizationStore.Get("Key_1657"), LocalizationStore.Get("Key_0012"), empty);
			_notificationIds.Add(item);
		}
		string text = Json.Serialize(_notificationIds);
		Debug.Log("Notifications to save: " + text);
		PlayerPrefs.SetString("Scheduled Notifications", text);
		PlayerPrefs.Save();
	}

	private IEnumerator appStart()
	{
		timeStartApp = Time.time;
		string serializedNotifocationIds = PlayerPrefs.GetString("Scheduled Notifications", "[]");
		Debug.Log("Notifications to discard: " + serializedNotifocationIds);
		object deserializedNotifications = Json.Deserialize(serializedNotifocationIds);
		List<object> scheduledNotifications = deserializedNotifications as List<object>;
		if (scheduledNotifications != null)
		{
			foreach (object i in scheduledNotifications)
			{
				int notificationId = Convert.ToInt32(i);
				EtceteraAndroid.cancelNotification(notificationId);
			}
			PlayerPrefs.DeleteKey("Scheduled Notifications");
		}
		else if (!Application.isEditor)
		{
			Debug.LogWarning("scheduledNotifications == null    " + deserializedNotifications.GetType());
		}
		PlayerPrefs.Save();
		yield break;
	}

	private IEnumerator OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			if (Initializer.Instance != null)
			{
				SaveTimeValues();
			}
			appStop();
			if (PhotonNetwork.connected && TimeGameController.sharedController == null && (Application.platform == RuntimePlatform.Android || !PhotonNetwork.isMessageQueueRunning || ConnectSceneNGUIController.sharedController != null))
			{
				PhotonNetwork.Disconnect();
			}
		}
		else
		{
			StartCoroutine(appStart());
			yield return null;
			yield return null;
			Tools.AddSessionNumber();
		}
	}
}
