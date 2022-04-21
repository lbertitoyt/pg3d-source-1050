using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Prime31;
using Rilisoft;
using UnityEngine;

public sealed class RemotePushNotificationController : MonoBehaviour
{
	private const string UrlPushNotificationServer = "https://secure.pixelgunserver.com/push_service";

	public static RemotePushNotificationController Instance;

	private bool _isResponceRuning;

	private bool _isStartUpdateRecive;

	private IEnumerator Start()
	{
		if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite)
		{
			yield break;
		}
		if (Application.isEditor)
		{
			Debug.Log("Google Cloud Messaging initialization skipped in editor.");
		}
		else if (!IsDeviceRegistred())
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("[GCM] Trying to register for push notifications.");
			}
			GoogleCloudMessagingManager.registrationSucceededEvent += HandleRegistered;
			GoogleCloudMessagingManager.registrationFailedEvent += HandleError;
			yield return new WaitForSeconds(1f);
			GoogleCloudMessaging.register("339873998127");
		}
	}

	private void HandleError(string error)
	{
		Debug.LogError(error);
	}

	private void HandleRegistered(string registrationId)
	{
		if (string.IsNullOrEmpty(registrationId))
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogError("Registration id is empty.");
			}
			return;
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("[GCM] Registration id: " + registrationId);
		}
		StartCoroutine(ReciveUpdateDataToServer(registrationId));
	}

	private bool IsDeviceRegistred()
	{
		string @string = PlayerPrefs.GetString("RemotePushNotificationToken", string.Empty);
		return !string.IsNullOrEmpty(@string);
	}

	private IEnumerator ReciveUpdateDataToServer(string deviceToken)
	{
		if (_isResponceRuning)
		{
			yield break;
		}
		_isResponceRuning = true;
		bool friendsControllerIsNotInitialized = FriendsController.sharedController == null;
		if (Defs.IsDeveloperBuild && FriendsController.sharedController == null)
		{
			Debug.Log("Waiting FriendsController being initialized...");
		}
		while (FriendsController.sharedController == null)
		{
			yield return null;
		}
		if (friendsControllerIsNotInitialized)
		{
			yield return null;
		}
		if (Defs.IsDeveloperBuild && FriendsController.sharedController.id == null)
		{
			Debug.Log("Waiting FriendsController.id being initialized...");
		}
		while (string.IsNullOrEmpty(FriendsController.sharedController.id))
		{
			yield return null;
		}
		_isStartUpdateRecive = true;
		WWWForm form = new WWWForm();
		string appVersion = string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion);
		string playerId = FriendsController.sharedController.id;
		string languageCode = LocalizationStore.GetCurrentLanguageCode();
		string isPayingPlayer = Storager.getInt("PayingUser", true).ToString();
		string dateLastPaying = PlayerPrefs.GetString("Last Payment Time", string.Empty);
		if (string.IsNullOrEmpty(dateLastPaying))
		{
			dateLastPaying = "None";
		}
		string timeUtcOffsetString = DateTimeOffset.Now.Offset.Hours.ToString();
		string countMoney = Storager.getInt("Coins", false).ToString();
		string countGems = Storager.getInt("GemsCurrency", false).ToString();
		string playerLevel = ExperienceController.GetCurrentLevel().ToString();
		form.AddField("app_version", appVersion);
		form.AddField("device_token", deviceToken);
		form.AddField("uniq_id", playerId);
		form.AddField("is_paying", isPayingPlayer);
		form.AddField("last_payment_date", dateLastPaying);
		form.AddField("utc_shift", timeUtcOffsetString);
		form.AddField("coins", countMoney);
		form.AddField("gems", countGems);
		form.AddField("level", playerLevel);
		form.AddField("language", languageCode);
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("RemotePushNotificationController(ReciveDeviceTokenToServer): form data");
			StringBuilder dataLog = new StringBuilder();
			dataLog.AppendLine("app_version: " + appVersion);
			dataLog.AppendLine("device_token: " + deviceToken);
			dataLog.AppendLine("uniq_id: " + playerId);
			dataLog.AppendLine("is_paying: " + isPayingPlayer);
			dataLog.AppendLine("last_payment_date: " + dateLastPaying);
			dataLog.AppendLine("utc_shift: " + timeUtcOffsetString);
			dataLog.AppendLine("coins: " + countMoney);
			dataLog.AppendLine("gems: " + countGems);
			dataLog.AppendLine("level: " + playerLevel);
			dataLog.AppendLine("language: " + languageCode);
			Debug.Log(dataLog.ToString());
		}
		Dictionary<string, string> headers = new Dictionary<string, string> { 
		{
			"Authorization",
			FriendsController.HashForPush(form.data)
		} };
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Trying to send device token to server: " + deviceToken);
		}
		WWW request = Tools.CreateWwwIfNotConnected("https://secure.pixelgunserver.com/push_service", form, "RemotePushNotificationController.ReciveUpdateDataToServer()", headers);
		if (request == null)
		{
			yield break;
		}
		yield return request;
		try
		{
			if (!string.IsNullOrEmpty(request.error))
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.Log("RemotePushNotificationController(ReciveDeviceTokenToServer): error = " + request.error);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(request.text))
				{
					yield break;
				}
				if (Defs.IsDeveloperBuild)
				{
					Debug.Log("RemotePushNotificationController(ReciveDeviceTokenToServer): request.text = " + request.text);
				}
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("Saving push notification token: " + deviceToken);
					}
					PlayerPrefs.SetString("RemotePushNotificationToken", deviceToken);
				}
			}
		}
		finally
		{
			//((<ReciveUpdateDataToServer>c__IteratorF8)(object)this).<>__Finally0();
		}
	}
}
