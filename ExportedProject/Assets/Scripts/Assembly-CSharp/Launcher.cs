using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;

internal sealed class Launcher : MonoBehaviour
{
	private struct Bounds
	{
		private readonly float _lower;

		private readonly float _upper;

		public float Lower
		{
			get
			{
				return _lower;
			}
		}

		public float Upper
		{
			get
			{
				return _upper;
			}
		}

		public Bounds(float lower, float upper)
		{
			_lower = Mathf.Min(lower, upper);
			_upper = Mathf.Max(lower, upper);
		}

		private float Clamp(float value)
		{
			return Mathf.Clamp(value, _lower, _upper);
		}

		public float Lerp(float value, float t)
		{
			return Mathf.Lerp(Clamp(value), _upper, t);
		}

		public float Lerp(float t)
		{
			return Lerp(_lower, t);
		}
	}

	public string intendedSignatureHash;

	public GameObject inAppGameObjectPrefab;

	public Canvas Canvas;

	public Slider ProgressSlider;

	public Text ProgressLabel;

	public RawImage SplashScreen;

	public GameObject amazonIapManagerPrefab;

	private GameObject amazonGameCircleManager;

	private static float? _progress;

	private bool _amazonGamecircleManagerInitialized;

	private bool _amazonIapManagerInitialized;

	private bool _crossfadeFinished;

	private static bool? _usingNewLauncher;

	private string _leaderboardId = string.Empty;

	private int _targetFramerate = 30;

	internal static LicenseVerificationController.PackageInfo? PackageInfo { get; set; }

	internal static bool UsingNewLauncher
	{
		get
		{
			return _usingNewLauncher.HasValue && _usingNewLauncher.Value;
		}
	}

	private void Awake()
	{
		if (Application.platform == RuntimePlatform.Android || Tools.RuntimePlatform == RuntimePlatform.MetroPlayerX64)
		{
			Application.targetFrameRate = 30;
		}
		_targetFramerate = ((Application.targetFrameRate != -1) ? Mathf.Clamp(Application.targetFrameRate, 30, 60) : 300);
		if (!_usingNewLauncher.HasValue)
		{
			_usingNewLauncher = SceneLoader.ActiveSceneName.Equals("Launcher");
		}
		if (ProgressLabel != null)
		{
			ProgressLabel.text = 0f.ToString("P0");
		}
	}

	private IEnumerable<float> SplashScreenFadeOut()
	{
		if (SplashScreen != null)
		{
			int splashScreenFadeOutFrameCount = 1 * _targetFramerate;
			SplashScreen.gameObject.SetActive(true);
			for (int i = 0; i != splashScreenFadeOutFrameCount; i++)
			{
				Color newColor = Color.Lerp(Color.white, Color.black, (float)i / (float)splashScreenFadeOutFrameCount);
				SplashScreen.color = newColor;
				yield return 0f;
			}
			SplashScreen.color = Color.black;
			yield return 1f;
		}
	}

	private IEnumerable<float> LoadingProgressFadeIn()
	{
		if (SplashScreen != null)
		{
			int loadingFadeInFrameCount = 1 * _targetFramerate;
			Color transparentColor = new Color(0f, 0f, 0f, 0f);
			for (int i = 0; i != loadingFadeInFrameCount; i++)
			{
				Color newColor = Color.Lerp(t: Mathf.Pow((float)i / (float)loadingFadeInFrameCount, 2.2f), a: Color.black, b: transparentColor);
				SplashScreen.color = newColor;
				yield return 0.5f;
			}
			UnityEngine.Object.Destroy(SplashScreen.gameObject);
			yield return 1f;
		}
		_crossfadeFinished = true;
	}

	private IEnumerator LoadingProgressFadeInCoroutine()
	{
		foreach (float item in LoadingProgressFadeIn())
		{
			float step = item;
			yield return null;
		}
	}

	private IEnumerator Start()
	{
		if (!_progress.HasValue)
		{
			foreach (float item in SplashScreenFadeOut())
			{
				float step3 = item;
				yield return null;
			}
			foreach (float item2 in LoadingProgressFadeIn())
			{
				float step2 = item2;
				yield return null;
			}
			_progress = 0f;
			FrameStopwatchScript stopwatch = GetComponent<FrameStopwatchScript>();
			if (stopwatch == null)
			{
				stopwatch = base.gameObject.AddComponent<FrameStopwatchScript>();
			}
			foreach (float item3 in InitRootCoroutine())
			{
				float step = item3;
				if (step >= 0f)
				{
					_progress = step;
				}
				if (stopwatch != null)
				{
					float elapsedSeconds = stopwatch.GetSecondsSinceFrameStarted();
					if (step >= 0f && elapsedSeconds < 1.618f / (float)_targetFramerate)
					{
						continue;
					}
				}
				if (ProgressSlider != null)
				{
					ProgressSlider.value = _progress.Value;
				}
				if (ProgressLabel != null)
				{
					ProgressLabel.text = _progress.Value.ToString("P0");
				}
				if (!ActivityIndicator.IsActiveIndicator)
				{
					ActivityIndicator.IsActiveIndicator = _crossfadeFinished;
				}
				yield return null;
			}
			if (Canvas != null)
			{
				UnityEngine.Object.Destroy(Canvas.gameObject);
			}
			UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}
		while (true)
		{
			float? progress = _progress;
			if (progress.HasValue && progress.Value < 1f)
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	private static void LogMessageWithBounds(string prefix, Bounds bounds)
	{
		string message = string.Format("{0}: [{1:P0}, {2:P0}]\t\t{3}", prefix, bounds.Lower, bounds.Upper, Time.frameCount);
		Debug.Log(message);
	}

	private IEnumerable<float> InitRootCoroutine()
	{
		Bounds bounds2 = new Bounds(0f, 0.04f);
		LogMessageWithBounds("AppsMenuAwakeCoroutine()", bounds2);
		Bounds bounds7 = new Bounds(0.05f, 0.09f);
		LogMessageWithBounds("AppsMenuStartCoroutine()", bounds7);
		foreach (float item in AppsMenuStartCoroutine())
		{
			float step4 = item;
			yield return bounds7.Lerp(step4);
		}
		Bounds bounds6 = new Bounds(0.1f, 0.19f);
		LogMessageWithBounds("InAppInstancerStartCoroutine()", bounds6);
		foreach (float item2 in InAppInstancerStartCoroutine())
		{
			float step3 = item2;
			yield return bounds6.Lerp(step3);
		}
		Bounds bounds5 = new Bounds(0.2f, 0.24f);
		LogMessageWithBounds("Application.LoadLevelAdditiveAsync(\"AppCenter\")", bounds5);
		AsyncOperation loadingCoroutine2 = Application.LoadLevelAdditiveAsync("AppCenter");
		while (!loadingCoroutine2.isDone)
		{
			yield return bounds5.Lerp(loadingCoroutine2.progress);
		}
		yield return -1f;
		Bounds bounds4 = new Bounds(0.25f, 0.29f);
		LogMessageWithBounds("Application.LoadLevelAdditiveAsync(\"Loading\")", bounds4);
		AsyncOperation loadingCoroutine = Application.LoadLevelAdditiveAsync("Loading");
		while (!loadingCoroutine.isDone)
		{
			yield return bounds4.Lerp(loadingCoroutine.progress);
		}
		yield return -1f;
		Switcher switcher = UnityEngine.Object.FindObjectOfType<Switcher>();
		if (switcher != null)
		{
			Bounds bounds3 = new Bounds(0.3f, 0.89f);
			LogMessageWithBounds("Switcher.InitializeSwitcher()", bounds3);
			foreach (float item3 in switcher.InitializeSwitcher())
			{
				float step2 = item3;
				yield return (!(step2 < 0f)) ? bounds3.Lerp(step2) : step2;
			}
		}
		Bounds bounds = new Bounds(0.9f, 0.99f);
		LogMessageWithBounds("Switcher.LoadMainMenu()", bounds);
		foreach (float item4 in switcher.LoadMainMenu())
		{
			float step = item4;
			yield return bounds.Lerp(step);
		}
		yield return 1f;
	}

	private static string GetTerminalSceneName_3afcc97c(uint gamma)
	{
		return "ClosingScene";
	}

	private IEnumerable<float> AppsMenuStartCoroutine()
	{
		if (Application.platform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			LicenseVerificationController.PackageInfo actualPackageInfo = default(LicenseVerificationController.PackageInfo);
			try
			{
				actualPackageInfo = LicenseVerificationController.GetPackageInfo();
				PackageInfo = actualPackageInfo;
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				Debug.Log("LicenseVerificationController.GetPackageInfo() failed:    " + ex);
				Singleton<SceneLoader>.Instance.LoadScene(GetTerminalSceneName_3afcc97c(989645180u));
			}
			finally
			{
				//((<AppsMenuStartCoroutine>c__Iterator14E)(object)this).<>__Finally0();
			}
			string actualPackageName = actualPackageInfo.PackageName;
			if (string.Compare(actualPackageName, Defs.GetIntendedAndroidPackageName(), StringComparison.Ordinal) != 0)
			{
				Debug.LogWarning("Verification FakeBundleDetected:    " + actualPackageName);
				FlurryPluginWrapper.LogEventWithParameterAndValue("Verification FakeBundleDetected", "Actual Package Name", actualPackageName);
				Singleton<SceneLoader>.Instance.LoadScene(GetTerminalSceneName_3afcc97c(989645180u));
			}
			else
			{
				Debug.Log("Package check passed.");
			}
			if (string.IsNullOrEmpty(intendedSignatureHash))
			{
				Debug.LogWarning("String.IsNullOrEmpty(intendedSignatureHash)");
				Singleton<SceneLoader>.Instance.LoadScene(GetTerminalSceneName_3afcc97c(989645180u));
			}
			string actualSignatureHash = actualPackageInfo.SignatureHash;
			if (string.Compare(actualSignatureHash, intendedSignatureHash, StringComparison.Ordinal) != 0)
			{
				Debug.LogWarning("Verification FakeSignatureDetected:    " + actualSignatureHash);
				FlurryPluginWrapper.LogEventWithParameterAndValue("Verification FakeSignatureDetected", "Actual Signature Hash", actualSignatureHash);
				Singleton<SceneLoader>.Instance.LoadScene(GetTerminalSceneName_3afcc97c(989645180u));
			}
			else
			{
				Debug.Log("Signature check passed.");
			}
			yield return 0.2f;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			if (AppsMenu.ApplicationBinarySplitted && !Application.isEditor)
			{
				string expPath = GooglePlayDownloader.GetExpansionFilePath();
				if (string.IsNullOrEmpty(expPath))
				{
					Debug.LogError(string.Format("ExpPath: “{0}”", expPath));
				}
				else if (Defs.IsDeveloperBuild)
				{
					Debug.Log(string.Format("ExpPath: “{0}”", expPath));
				}
				string mainPath2 = GooglePlayDownloader.GetMainOBBPath(expPath);
				if (mainPath2 == null)
				{
					Debug.Log("Trying to fetch OBB...");
					GooglePlayDownloader.FetchOBB();
				}
				mainPath2 = GooglePlayDownloader.GetMainOBBPath(expPath);
				if (mainPath2 == null)
				{
					Debug.Log("Waiting OBB fetch...");
				}
				while (mainPath2 == null)
				{
					yield return 0.6f;
					if (Time.frameCount % 120 == 0)
					{
						mainPath2 = GooglePlayDownloader.GetMainOBBPath(expPath);
					}
				}
			}
			yield return 0.6f;
		}
		yield return 0.8f;
		AppsMenu.SetCurrentLanguage();
		yield return 1f;
	}

	private IEnumerable<float> InAppInstancerStartCoroutine()
	{
		if (!GameObject.FindGameObjectWithTag("InAppGameObject"))
		{
			UnityEngine.Object.Instantiate(inAppGameObjectPrefab, Vector3.zero, Quaternion.identity);
			yield return 0.1f;
		}
		if (amazonIapManagerPrefab == null)
		{
			Debug.LogWarning("amazonIapManager == null");
		}
		else if (!_amazonIapManagerInitialized)
		{
			UnityEngine.Object.Instantiate(amazonIapManagerPrefab, Vector3.zero, Quaternion.identity);
			_amazonIapManagerInitialized = true;
			yield return 0.2f;
		}
		if (Application.platform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			if (amazonGameCircleManager == null)
			{
				Debug.LogWarning("amazonGamecircleManager == null");
			}
			else if (!_amazonGamecircleManagerInitialized)
			{
				UnityEngine.Object.DontDestroyOnLoad(amazonGameCircleManager);
				_leaderboardId = ((Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite) ? "best_survival_scores" : "CgkIr8rGkPIJEAIQCg");
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
					AGSLeaderboardsClient.SubmitScoreSucceededEvent += HandleAmazonSubmitScoreSucceeded;
					AGSLeaderboardsClient.SubmitScoreFailedEvent += HandleAmazonSubmitScoreFailed;
					AGSLeaderboardsClient.SubmitScore(_leaderboardId, PlayerPrefs.GetInt(Defs.SurvivalScoreSett, 0));
				}
				_amazonGamecircleManagerInitialized = true;
			}
		}
		else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
		}
		yield return 1f;
	}

	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive)
	{
		Debug.Log(string.Format("GameThrive HandleNotification(“{0}”, ..., {1})", message, isActive));
	}

	private void HandleAmazonGamecircleServiceReady()
	{
		AGSClient.ServiceReadyEvent -= HandleAmazonGamecircleServiceReady;
		AGSClient.ServiceNotReadyEvent -= HandleAmazonGamecircleServiceNotReady;
		Debug.Log("Amazon GameCircle service is initialized.");
		AGSAchievementsClient.UpdateAchievementCompleted += HandleUpdateAchievementCompleted;
		AGSLeaderboardsClient.SubmitScoreSucceededEvent += HandleAmazonSubmitScoreSucceeded;
		AGSLeaderboardsClient.SubmitScoreFailedEvent += HandleAmazonSubmitScoreFailed;
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

	private void HandleAmazonSubmitScoreSucceeded(string leaderbordId)
	{
		AGSLeaderboardsClient.SubmitScoreSucceededEvent -= HandleAmazonSubmitScoreSucceeded;
		AGSLeaderboardsClient.SubmitScoreFailedEvent -= HandleAmazonSubmitScoreFailed;
		if (Debug.isDebugBuild)
		{
			Debug.Log("Submit score succeeded for leaderboard " + leaderbordId);
		}
	}

	private void HandleAmazonSubmitScoreFailed(string leaderbordId, string error)
	{
		AGSLeaderboardsClient.SubmitScoreSucceededEvent -= HandleAmazonSubmitScoreSucceeded;
		AGSLeaderboardsClient.SubmitScoreFailedEvent -= HandleAmazonSubmitScoreFailed;
		string message = string.Format("Submit score failed for leaderboard {0}:\n{1}", leaderbordId, error);
		Debug.LogError(message);
	}
}
