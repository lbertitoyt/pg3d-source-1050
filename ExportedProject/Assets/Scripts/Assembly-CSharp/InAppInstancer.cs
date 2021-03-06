using System;
using System.Collections;
using System.Reflection;
using com.amazon.mas.cpt.ads;
using Rilisoft;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

[Obfuscation(Exclude = true)]
internal sealed class InAppInstancer : MonoBehaviour
{
	public GameObject inAppGameObjectPrefab;

	private bool _amazonGamecircleManagerInitialized;

	private bool _amazonIapManagerInitialized;

	private string _leaderboardId = string.Empty;

	private IEnumerator Start()
	{
		if (Launcher.UsingNewLauncher)
		{
			yield break;
		}
		if (!GameObject.FindGameObjectWithTag("InAppGameObject"))
		{
			UnityEngine.Object.Instantiate(inAppGameObjectPrefab, Vector3.zero, Quaternion.identity);
			yield return null;
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			if (!_amazonGamecircleManagerInitialized)
			{
				StartCoroutine(InitializeAmazonGamecircleManager());
				_amazonGamecircleManagerInitialized = true;
			}
		}
		else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
		}
	}

	private IEnumerator InitializeAmazonGamecircleManager()
	{
		GameObject amazonGameCircleManager = new GameObject("Rilisoft.AmazonGameCircleManager", typeof(GameCircleManager));
		UnityEngine.Object.DontDestroyOnLoad(amazonGameCircleManager);
		yield return null;
		_leaderboardId = ((Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite) ? "best_survival_scores" : "CgkIr8rGkPIJEAIQCg");
		try
		{
			Debug.Log("Initializing Amazon Ads.");
			IAmazonMobileAds mobileAds = AmazonMobileAdsImpl.Instance;
			ShouldEnable loggingEnabled = new ShouldEnable
			{
				BooleanValue = Defs.IsDeveloperBuild
			};
			mobileAds.EnableLogging(loggingEnabled);
			ApplicationKey applicationKey = new ApplicationKey
			{
				StringValue = "1bb979bc6c9e4059a318370a68dcaeea"
			};
			mobileAds.SetApplicationKey(applicationKey);
			mobileAds.RegisterApplication();
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		if (!AGSClient.IsServiceReady())
		{
			Debug.Log("Trying to initialize Amazon GameCircle service...");
			AGSClient.ServiceReadyEvent += HandleAmazonGamecircleServiceReady;
			AGSClient.ServiceNotReadyEvent += HandleAmazonGamecircleServiceNotReady;
			AGSClient.Init(true, true, true);
			AGSWhispersyncClient.OnNewCloudDataEvent += HandleAmazonPotentialProgressConflicts;
			AGSWhispersyncClient.OnDataUploadedToCloudEvent += HandleAmazonPotentialProgressConflicts;
			AGSWhispersyncClient.OnSyncFailedEvent += HandleAmazonSyncFailed;
			AGSWhispersyncClient.OnThrottledEvent += HandleAmazonThrottled;
		}
		else
		{
			Debug.Log("Amazon GameCircle was already initialized.");
			AGSLeaderboardsClient.SubmitScoreSucceededEvent += HandleSubmitScoreSucceeded;
			AGSLeaderboardsClient.SubmitScoreFailedEvent += HandleSubmitScoreFailed;
			AGSLeaderboardsClient.SubmitScore(_leaderboardId, PlayerPrefs.GetInt(Defs.SurvivalScoreSett, 0));
		}
		while (!AGSClient.IsServiceReady())
		{
			yield return null;
		}
		if (!GameCircleSocial.Instance.localUser.authenticated)
		{
			Debug.LogFormat("[Rilisoft] Sign in to GameCircle ({0})", GetType().Name);
			AGSClient.ShowSignInPage();
		}
	}

	private void HandleAmazonGamecircleServiceReady()
	{
		AGSClient.ServiceReadyEvent -= HandleAmazonGamecircleServiceReady;
		AGSClient.ServiceNotReadyEvent -= HandleAmazonGamecircleServiceNotReady;
		Debug.Log("Amazon GameCircle service is initialized.");
		AGSAchievementsClient.UpdateAchievementCompleted += HandleUpdateAchievementCompleted;
		AGSLeaderboardsClient.SubmitScoreSucceededEvent += HandleSubmitScoreSucceeded;
		AGSLeaderboardsClient.SubmitScoreFailedEvent += HandleSubmitScoreFailed;
		AGSLeaderboardsClient.SubmitScore(_leaderboardId, PlayerPrefs.GetInt(Defs.SurvivalScoreSett, 0));
	}

	private void HandleAmazonPotentialProgressConflicts()
	{
		Debug.Log("HandleAmazonPotentialProgressConflicts()");
	}

	private void HandleAmazonSyncFailed()
	{
		Debug.LogWarning("HandleAmazonSyncFailed(): " + AGSWhispersyncClient.failReason);
	}

	private void HandleAmazonThrottled()
	{
		Debug.LogWarning("HandleAmazonThrottled().");
	}

	private void HandleAmazonGamecircleServiceNotReady(string message)
	{
		Debug.LogError("Amazon GameCircle service is not ready:\n" + message);
	}

	private void HandleUpdateAchievementCompleted(AGSUpdateAchievementResponse response)
	{
		string message = ((!string.IsNullOrEmpty(response.error)) ? string.Format("Achievement {0} failed. {1}", response.achievementId, response.error) : string.Format("Achievement {0} succeeded.", response.achievementId));
		Debug.Log(message);
	}

	private void HandleSubmitScoreSucceeded(string leaderbordId)
	{
		AGSLeaderboardsClient.SubmitScoreSucceededEvent -= HandleSubmitScoreSucceeded;
		AGSLeaderboardsClient.SubmitScoreFailedEvent -= HandleSubmitScoreFailed;
		if (Debug.isDebugBuild)
		{
			Debug.Log("Submit score succeeded for leaderboard " + leaderbordId);
		}
	}

	private void HandleSubmitScoreFailed(string leaderbordId, string error)
	{
		AGSLeaderboardsClient.SubmitScoreSucceededEvent -= HandleSubmitScoreSucceeded;
		AGSLeaderboardsClient.SubmitScoreFailedEvent -= HandleSubmitScoreFailed;
		string message = string.Format("Submit score failed for leaderboard {0}:\n{1}", leaderbordId, error);
		Debug.LogError(message);
	}
}
