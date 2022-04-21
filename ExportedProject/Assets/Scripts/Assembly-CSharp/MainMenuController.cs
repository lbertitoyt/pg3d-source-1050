using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Facebook.Unity;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class MainMenuController : ControlsSettingsBase
{
	internal const string TrafficForwardedKey = "TrafficForwarded";

	private static readonly TaskCompletionSource<bool> _syncPromise = new TaskCompletionSource<bool>();

	public GameObject questButton;

	public GameObject facebookLoginContainer;

	public GameObject twitterLoginContainer;

	public GameObject facebookConnectedSettings;

	public GameObject facebookDisconnectedSettings;

	public GameObject facebookConnectSettings;

	public GameObject twitterConnectedSettings;

	public GameObject twitterDisconnectedSettings;

	public GameObject twitterConnectSettings;

	[Header("MainMenuController properties")]
	public GameObject socialGunBannerForFreePanel;

	public SocialGunBannerView socialBanner;

	public Transform topLeftAnchor;

	public GameObject buySmileButton;

	public GameObject postNewsLabel;

	public UIButton premiumButton;

	public GameObject premium;

	public GameObject daysOfValor;

	public GameObject adventureButton;

	public GameObject achievementsButton;

	public GameObject clansButton;

	public GameObject leadersButton;

	public UILabel battleNowLabel;

	public UILabel trainingNowLabel;

	public GameObject friendsGUI;

	public UILabel premiumTime;

	public GameObject premiumUpPlashka;

	public GameObject premiumbottomPlashka;

	public List<GameObject> premiumLevels = new List<GameObject>();

	public GameObject starParticleStarterPackGaemObject;

	public GameObject starParticleSocialGunButton;

	public Transform RentExpiredPoint;

	public Transform pers;

	public GameObject completeTraining;

	public GameObject stubLoading;

	public UITexture stubTexture;

	public MainMenuHeroCamera rotateCamera;

	public static MainMenuController sharedController;

	public GameObject campaignButton;

	public GameObject survivalButton;

	public GameObject multiplayerButton;

	public GameObject skinsMakerButton;

	public GameObject friendsButton;

	public GameObject profileButton;

	public GameObject freeButton;

	public GameObject youtubeButton;

	public GameObject everyplayButton;

	public GameObject gameCenterButton;

	public GameObject shopButton;

	public GameObject settingsButton;

	public GameObject supportButton;

	public GameObject supportPanel;

	public GameObject enderManButton;

	public GameObject coinsShopButton;

	public GameObject diclineButton;

	public GameObject agreeButton;

	public GameObject UserAgreementPanel;

	public GameObject newsButton;

	public GameObject newsPanel;

	public GameObject newsBackButton;

	public UIButton signOutButton;

	public GameObject postFacebookButton;

	public GameObject postTwitterButton;

	public GameObject rateUsButton;

	public GameObject backFromFreeButton;

	public GameObject twitterSubcribeButton;

	public GameObject facebookSubcribeButton;

	public GameObject freePanel;

	public GameObject mainPanel;

	public GameObject newsIndicator;

	[Header("Leaderboards")]
	public UIPanel leaderboardsPanel;

	public UIPanel oldLeaderboardsPanel;

	[Header("Misc")]
	public GameObject PromoActionsPanel;

	public UIToggle notShowAgain;

	public UILabel coinsLabel;

	public GameObject award800to810;

	public UIButton awardOk;

	public GameObject bannerContainer;

	public GameObject nicknameLabel;

	public UIButton developerConsole;

	public UICamera uiCamera;

	public NickLabelController persNickLabel;

	public GameObject eventX3Window;

	public UILabel[] eventX3RemainTime;

	public UIButton trafficForwardingButton;

	public static bool trafficForwardActive;

	private float _eventX3RemainTimeLastUpdateTime;

	private readonly System.Lazy<UISprite> _newClanIncomingInvitesSprite;

	private AdvertisementController _advertisementController;

	private ShopNGUIController _shopInstance;

	private bool isMultyPress;

	private bool isFriendsPress;

	private List<GameObject> saveOpenPanel = new List<GameObject>();

	public static bool canRotationLobbyPlayer = true;

	private readonly List<EventHandler> _backSubscribers = new List<EventHandler>();

	private bool loadReplaceAdmobPerelivRunning;

	private bool loadAdmobRunning;

	private float _lastTimeInterstitialShown;

	private static bool _drawLoadingProgress = true;

	[NonSerialized]
	public GameObject freeAwardChestObj;

	public static bool SingleModeOnStart;

	public static bool friendsOnStart;

	private static bool _socialNetworkingInitilized;

	private Rect campaignRect;

	private Rect survivalRect;

	private Rect shopRect;

	public TweenColor colorBlinkForX3;

	private string _localizeSaleLabel;

	private float _timePremiumTimeUpdated;

	private readonly System.Lazy<bool> _timeTamperingDetected = new System.Lazy<bool>(delegate
	{
		bool flag = FreeAwardController.Instance.TimeTamperingDetected();
		if (flag)
		{
		}
		return flag;
	});

	private IDisposable _backSubscription;

	private float lastTime;

	private float idleTimerLastTime;

	private float _bankEnteredTime;

	private MenuLeaderboardsController _menuLeaderboardsController;

	public UIPanel starterPackPanel;

	public UIPanel socialGunPanel;

	public UILabel starterPackTimer;

	public UILabel socialGunEventTimer;

	public UITexture buttonBackground;

	private bool _starterPackEnabled;

	private string _trafficForwardingUrl = "http://pixelgun3d.com/";

	private readonly System.Lazy<UIButton[]> _leaderboardsButton;

	private readonly System.Lazy<LeaderboardScript> _leaderboardScript;

	public UIWidget dayOfValorContainer;

	public UILabel dayOfValorTimer;

	private bool _dayOfValorEnabled;

	public GameObject singleModePanel;

	public UILabel singleModeBestScores;

	public UILabel singleModeStarsProgress;

	private Transform _parentBankPanel;

	private bool inAdventureScreen;

	[Header("Social panel settings")]
	public UIButton socialButton;

	internal System.Threading.Tasks.Task SyncFuture
	{
		get
		{
			return _syncPromise.Task;
		}
	}

	public static bool trainingCompleted { get; set; }

	public UIPanel LeaderboardsPanel
	{
		get
		{
			return (!LeaderboardScript.NewLeaderboards) ? oldLeaderboardsPanel : leaderboardsPanel;
		}
	}

	public static string RateUsURL
	{
		get
		{
			string result = Defs2.ApplicationUrl;
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				result = "https://play.google.com/store/apps/details?id=com.pixel.gun3d&hl=en";
			}
			return result;
		}
	}

	public bool InAdventureScreen
	{
		get
		{
			return inAdventureScreen;
		}
		private set
		{
			inAdventureScreen = value;
		}
	}

	public static event Action onLoadMenu;

	public static event Action onEnableMenuForAskname;

	public static event Action<bool> onActiveMainMenu;

	public event EventHandler BackPressed
	{
		add
		{
			_backSubscribers.Add(value);
		}
		remove
		{
			_backSubscribers.Remove(value);
		}
	}

	public MainMenuController()
	{
		_newClanIncomingInvitesSprite = new System.Lazy<UISprite>(() => clansButton.Map((GameObject c) => c.GetComponentsInChildren<UISprite>(true).FirstOrDefault((UISprite s) => "NewMessages".Equals(s.name))));
		_leaderboardsButton = new System.Lazy<UIButton[]>(() => leadersButton.GetComponentsInChildren<UIButton>(true));
		_leaderboardScript = new System.Lazy<LeaderboardScript>(FindObjectOfType<LeaderboardScript>);
	}

	internal static bool LevelAlreadySaved(int level)
	{
		string key = "currentLevel" + level;
		return Storager.hasKey(key) && Storager.getInt(key, false) > 0;
	}

	internal static int FindMaxLevel(IEnumerable<string> itemsToBeSaved)
	{
		int num = 0;
		foreach (string item in itemsToBeSaved)
		{
			if (!item.StartsWith("currentLevel"))
			{
				continue;
			}
			string[] array = item.Split(new string[1] { "currentLevel" }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length <= 0)
			{
				continue;
			}
			string value = array[array.Length - 1];
			if (!string.IsNullOrEmpty(value))
			{
				int num2 = Convert.ToInt32(value);
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	private IEnumerator SynchronizeEditorCoroutine()
	{
		if (Application.isEditor)
		{
			_syncPromise.TrySetResult(false);
			yield break;
		}
		Debug.Log("Trying to simulate syncing to cloud...");
		if (PlayerPrefs.GetInt("PendingGooglePlayGamesSync", 0) == 0)
		{
			Debug.Log("No pending GooglePlay Games sync.");
			_syncPromise.TrySetResult(false);
			yield break;
		}
		IEnumerator coroutine = PurchasesSynchronizer.Instance.SimulateSynchronization(delegate(bool succeeded)
		{
			Debug.LogFormat("[Rilisoft] MainMenuController.PurchasesSynchronizer.Callback({0}) >: {1:F3}", succeeded, Time.realtimeSinceStartup);
			try
			{
				if (!succeeded)
				{
					_syncPromise.TrySetResult(false);
				}
				else
				{
					Action action = delegate
					{
						PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
						if (WeaponManager.sharedManager != null)
						{
							CoroutineRunner.Instance.StartCoroutine(WeaponManager.sharedManager.ResetCoroutine());
						}
						_syncPromise.TrySetResult(true);
					};
					if (PurchasesSynchronizer.Instance.HasItemsToBeSaved)
					{
						int num = FindMaxLevel(PurchasesSynchronizer.Instance.ItemsToBeSaved);
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("[Rilisoft] Incoming level: {0}", num);
						}
						if (num > 0)
						{
							CoroutineRunner.Instance.StartCoroutine(WaitReturnToMainMenuAndShowRestorePanel(action));
						}
						else
						{
							_syncPromise.TrySetResult(false);
						}
					}
					else
					{
						action();
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				Debug.LogFormat("[Rilisoft] MainMenuController.PurchasesSynchronizer.Callback({0}) <: {1:F3}", succeeded, Time.realtimeSinceStartup);
			}
		});
		CoroutineRunner.Instance.StartCoroutine(coroutine);
	}

	private IEnumerator SynchronizeGoogleCoroutine(Action tryUpdateNickname, GameServicesController gameServicesController)
	{
		_socialNetworkingInitilized = true;
		Debug.Log("Trying to authenticate with Google Play Games...");
		try
		{
			if (PlayerPrefs.GetInt("PendingGooglePlayGamesSync", 0) == 0)
			{
				_syncPromise.TrySetResult(false);
			}
			if (PlayerPrefs.GetInt("GoogleSignInDenied", 0) > 0)
			{
				Debug.LogWarning("Skipping sync because authentication has already been denied.");
				yield break;
			}
			Action tryUpdateNickname2 = default(Action);
			bool flag = default(bool);
			Action<bool> authenticateCallback = delegate(bool succeeded)
			{
				Debug.LogFormat("[Rilisoft] MainMenuController.Authenticate.Callback({0}) >: {1:F3}", succeeded, Time.realtimeSinceStartup);
				string message = ((!succeeded) ? "Play Games Services authentication failed." : string.Format("User authenticated after call in SynchronizeGoogleCoroutine(): {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state));
				Debug.Log(message);
				PlayerPrefs.SetInt("GoogleSignInDenied", Convert.ToInt32(!succeeded));
				if (!succeeded)
				{
					_syncPromise.TrySetResult(false);
					Debug.LogFormat("IsCompleted: {0} IsCanceled: {1} IsFaulted: {2}", SyncFuture.IsCompleted, SyncFuture.IsCanceled, SyncFuture.IsFaulted);
				}
				else
				{
					tryUpdateNickname2();
					if (!flag)
					{
						Debug.Log("No pending GooglePlay Games sync.");
						_syncPromise.TrySetResult(false);
					}
					else
					{
						Debug.Log("Pending synchronization, retrying...");
						PurchasesSynchronizer.Instance.SynchronizeIfAuthenticated(delegate(bool succeded)
						{
							Debug.LogFormat("[Rilisoft] MainMenuController.PurchasesSynchronizer.Callback({0}) >: {1:F3}", succeded, Time.realtimeSinceStartup);
							try
							{
								if (!succeded)
								{
									_syncPromise.TrySetResult(false);
								}
								else
								{
									Action action = delegate
									{
										Debug.LogFormat("[Rilisoft] MainMenuController.PurchasesSynchronizer.InnerCallback >: {0:F3}", Time.realtimeSinceStartup);
										PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
										if (WeaponManager.sharedManager != null)
										{
											CoroutineRunner.Instance.StartCoroutine(WeaponManager.sharedManager.ResetCoroutine());
										}
										_syncPromise.TrySetResult(true);
										Debug.LogFormat("[Rilisoft] MainMenuController.PurchasesSynchronizer.InnerCallback <: {0:F3}", Time.realtimeSinceStartup);
									};
									Debug.LogFormat("[Rilisoft] PurchasesSynchronizer.HasItemsToBeSaved: {0}", PurchasesSynchronizer.Instance.HasItemsToBeSaved);
									if (PurchasesSynchronizer.Instance.HasItemsToBeSaved)
									{
										int num = FindMaxLevel(PurchasesSynchronizer.Instance.ItemsToBeSaved);
										if (Defs.IsDeveloperBuild)
										{
											Debug.LogFormat("[Rilisoft] Incoming level: {0}", num);
										}
										if (num > 0)
										{
											CoroutineRunner.Instance.StartCoroutine(WaitReturnToMainMenuAndShowRestorePanel(action));
										}
										else
										{
											_syncPromise.TrySetResult(false);
										}
									}
									else
									{
										Debug.LogFormat("[Rilisoft] > MainMenuController.PurchasesSynchronizer.InnerCallback: {0:F3}", Time.realtimeSinceStartup);
										action();
										Debug.LogFormat("[Rilisoft] < MainMenuController.PurchasesSynchronizer.InnerCallback: {0:F3}", Time.realtimeSinceStartup);
									}
								}
							}
							catch (Exception exception)
							{
								Debug.LogException(exception);
							}
							finally
							{
								Debug.LogFormat("[Rilisoft] MainMenuController.PurchasesSynchronizer.Callback({0}) <: {1:F3}", succeeded, Time.realtimeSinceStartup);
							}
						});
						Debug.LogFormat("[Rilisoft] MainMenuController.Authenticate.Callback({0}) <: {1:F3}", succeeded, Time.realtimeSinceStartup);
					}
				}
			};
			GpgFacade.Instance.Authenticate(authenticateCallback, false);
		}
		catch (InvalidOperationException ex2)
		{
			InvalidOperationException ex = ex2;
			Debug.LogWarning("SettingsMainMenuController: Exception occured while authenticating with Google Play Games. See next exception message for details.");
			Debug.LogException(ex);
		}
		yield return null;
		gameServicesController.WaitAuthenticationAndIncrementBeginnerAchievement();
	}

	private IEnumerator SynchronizeAmazonCoroutine(Action tryUpdateNickname, GameServicesController gameServicesController)
	{
		Social.Active = GameCircleSocial.Instance;
		Debug.Log("Social user authenticated: " + Social.localUser.authenticated);
		PurchasesSynchronizer.Instance.SynchronizeAmazonPurchases();
		if (PurchasesSynchronizer.Instance.HasItemsToBeSaved)
		{
			int maxLevel = FindMaxLevel(PurchasesSynchronizer.Instance.ItemsToBeSaved);
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("[Rilisoft] Incoming level: {0}", maxLevel);
			}
			if (maxLevel > 0 && !LevelAlreadySaved(maxLevel))
			{
				CoroutineRunner.Instance.StartCoroutine(WaitReturnToMainMenuAndShowRestorePanel(RefreshGui));
			}
			else
			{
				_syncPromise.TrySetResult(false);
			}
		}
		else
		{
			Debug.LogFormat("[Rilisoft] > MainMenuController.PurchasesSynchronizer.InnerCallback: {0:F3}", Time.realtimeSinceStartup);
			RefreshGui();
			Debug.LogFormat("[Rilisoft] < MainMenuController.PurchasesSynchronizer.InnerCallback: {0:F3}", Time.realtimeSinceStartup);
		}
		yield return new WaitForSeconds(1f);
		Debug.LogFormat("[Rilisoft] > MainMenuController.PurchasesSynchronizer.SynchronizeAmazonProgress: {0:F3}", Time.realtimeSinceStartup);
		ProgressSynchronizer.Instance.SynchronizeAmazonProgress();
		Debug.LogFormat("[Rilisoft] < MainMenuController.PurchasesSynchronizer.SynchronizeAmazonProgress: {0:F3}", Time.realtimeSinceStartup);
		yield return null;
		if (GameCircleSocial.Instance.localUser.authenticated)
		{
			tryUpdateNickname();
		}
		gameServicesController.WaitAuthenticationAndIncrementBeginnerAchievement();
		_syncPromise.TrySetResult(false);
	}

	private IEnumerator WaitReturnToMainMenuAndShowRestorePanel(Action refreshCallback)
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.LogFormat("WaitReturnToMainMenu >: {0:F3}", Time.realtimeSinceStartup);
		}
		while (ShopNGUIController.GuiActive || !StringComparer.Ordinal.Equals(SceneManager.GetActiveScene().name, Defs.MainMenuScene) || AskNameManager.isShow)
		{
			yield return null;
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.LogFormat("> WaitReturnToMainMenu.Callback: {0:F3}", Time.realtimeSinceStartup);
		}
		TrainingController.OnGetProgress();
		if (QuestSystem.Instance != null)
		{
			QuestSystem.Instance.QuestProgress.FilterFulfilledTutorialQuests();
		}
		if (HintController.instance != null)
		{
			HintController.instance.ShowNext();
		}
		RefreshSettingsButton();
		Debug.Log("Trying to fill weapon slots...");
		try
		{
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.playerWeapons != null)
			{
				IEnumerable<Weapon> playerWeapons = WeaponManager.sharedManager.playerWeapons.OfType<Weapon>();
				IEnumerable<Weapon> availableWeapons = WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>();
				if (!playerWeapons.Any((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 3))
				{
					string prefabName3 = ItemDb.GetByTag(WeaponTags.BASIC_FLAMETHROWER_Tag).PrefabName;
					WeaponManager.sharedManager.EquipWeapon(availableWeapons.First((Weapon w) => w.weaponPrefab.name.Replace("(Clone)", string.Empty) == prefabName3));
				}
				if (!playerWeapons.Any((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 5))
				{
					string prefabName2 = ItemDb.GetByTag(WeaponTags.SignalPistol_Tag).PrefabName;
					WeaponManager.sharedManager.EquipWeapon(availableWeapons.First((Weapon w) => w.weaponPrefab.name.Replace("(Clone)", string.Empty) == prefabName2));
				}
				if (!playerWeapons.Any((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 4))
				{
					string prefabName = ItemDb.GetByTag(WeaponTags.HunterRifleTag).PrefabName;
					WeaponManager.sharedManager.EquipWeapon(availableWeapons.First((Weapon w) => w.weaponPrefab.name.Replace("(Clone)", string.Empty) == prefabName));
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		Action refreshCallback2 = default(Action);
		InfoWindowController.ShowRestorePanel(delegate
		{
			CoroutineRunner.Instance.StartCoroutine(SaveItemsToStorager(refreshCallback2));
		});
		if (Defs.IsDeveloperBuild)
		{
			Debug.LogFormat("< WaitReturnToMainMenu.Callback: {0:F3}", Time.realtimeSinceStartup);
			Debug.LogFormat("WaitReturnToMainMenu <: {0:F3}", Time.realtimeSinceStartup);
		}
	}

	private void RefreshGui()
	{
		PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
		if (WeaponManager.sharedManager != null)
		{
			CoroutineRunner.Instance.StartCoroutine(WeaponManager.sharedManager.ResetCoroutine());
		}
		_syncPromise.TrySetResult(true);
	}

	public void SaveShowPanelAndClose()
	{
		if (!(mainPanel != null))
		{
			return;
		}
		saveOpenPanel.Clear();
		for (int i = 0; i < mainPanel.transform.childCount; i++)
		{
			GameObject gameObject = mainPanel.transform.GetChild(i).gameObject;
			if (!(gameObject.GetComponent<UICamera>() != null) && gameObject.activeSelf)
			{
				saveOpenPanel.Add(gameObject);
				gameObject.SetActive(false);
			}
		}
	}

	public void ShowSavePanel(bool needClear = true)
	{
		for (int i = 0; i < saveOpenPanel.Count; i++)
		{
			GameObject gameObject = saveOpenPanel[i];
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
		if (needClear)
		{
			saveOpenPanel.Clear();
		}
	}

	private void InvokeLastBackHandler()
	{
		if (_backSubscribers.Count != 0)
		{
			EventHandler o = _backSubscribers[_backSubscribers.Count - 1];
			o.Do(delegate(EventHandler lastHandler)
			{
				lastHandler(this, EventArgs.Empty);
			});
		}
	}

	private string _SocialMessage()
	{
		return "Come and play with me in epic multiplayer shooter - Pixel Gun 3D! http://goo.gl/dQMf4n";
	}

	public static bool ShowBannerOrLevelup()
	{
		return (ExperienceController.sharedController != null && ExperienceController.sharedController.isShowNextPlashka) || MainMenu.BlockInterface || Defs.isShowUserAgrement || (BannerWindowController.SharedController != null && BannerWindowController.SharedController.IsAnyBannerShown) || FriendsWindowGUI.Instance.InterfaceEnabled;
	}

	public static void DoMemoryConsumingTaskInEmptyScene(Action action, Action onSeparateSceneCaseAction = null)
	{
		if (Device.IsLoweMemoryDevice)
		{
			CleanUpAndDoAction.action = onSeparateSceneCaseAction ?? action;
			SceneManager.LoadScene("LoadAnotherApp");
		}
		else if (action != null)
		{
			action();
		}
	}

	public void HandleFacebookLoginButton()
	{
		// lmao
		Application.OpenURL("");
	}

	public void HandleTwitterLoginButton()
	{
		ButtonClickSound.TryPlayClick();
		if (TwitterController.IsLoggedIn && TwitterController.Instance != null)
		{
			TwitterController.Instance.Logout();
			return;
		}
		DoMemoryConsumingTaskInEmptyScene(delegate
		{
			if (TwitterController.Instance != null)
			{
				TwitterController.Instance.Login(null, null, "Options");
			}
		});
	}

	public void HandleSignOutButton()
	{
		ButtonClickSound.TryPlayClick();
		PlayerPrefs.SetInt("GoogleSignInDenied", 1);
		if (Application.isEditor)
		{
			Debug.Log("[Sign Out] pressed.");
		}
		else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			GpgFacade.Instance.SignOut();
		}
		string text = LocalizationStore.Get("Key_2103") ?? "Signed out.";
		InfoWindowController.ShowInfoBox(text);
		if (signOutButton != null)
		{
			signOutButton.gameObject.SetActive(false);
		}
	}

	private IEnumerator OnApplicationPause(bool pausing)
	{
		if (pausing)
		{
			yield break;
		}
		yield return new WaitForSeconds(1f);
		if (MobileAdManager.Instance.SuppressShowOnReturnFromPause)
		{
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = false;
		}
		else
		{
			bool shouldShowReplaceAdmob = ReplaceAdmobPerelivController.ReplaceAdmobWithPerelivApplicable() && ReplaceAdmobPerelivController.sharedController != null && FreeAwardController.FreeAwardChestIsInIdleState;
			if (shouldShowReplaceAdmob)
			{
				ReplaceAdmobPerelivController.IncreaseTimesCounter();
			}
			if (shouldShowReplaceAdmob && ReplaceAdmobPerelivController.ShouldShowAtThisTime && !loadAdmobRunning)
			{
				StartCoroutine(LoadAndShowReplaceAdmobPereliv("On return from pause to Lobby"));
			}
		}
	}

	private IEnumerator LoadAndShowReplaceAdmobPereliv(string context)
	{
		if (loadReplaceAdmobPerelivRunning)
		{
			yield break;
		}
		try
		{
			loadReplaceAdmobPerelivRunning = true;
			if (!ReplaceAdmobPerelivController.sharedController.DataLoading && !ReplaceAdmobPerelivController.sharedController.DataLoaded)
			{
				ReplaceAdmobPerelivController.sharedController.LoadPerelivData();
			}
			while (!ReplaceAdmobPerelivController.sharedController.DataLoaded)
			{
				if (!ReplaceAdmobPerelivController.sharedController.DataLoading)
				{
					loadReplaceAdmobPerelivRunning = false;
					yield break;
				}
				yield return null;
			}
			yield return new WaitForSeconds(0.5f);
			if (FreeAwardController.FreeAwardChestIsInIdleState && (!(BannerWindowController.SharedController != null) || !BannerWindowController.SharedController.IsAnyBannerShown))
			{
				ReplaceAdmobPerelivController.TryShowPereliv(context);
				ReplaceAdmobPerelivController.sharedController.DestroyImage();
			}
		}
		finally
		{
			loadReplaceAdmobPerelivRunning = false;
		}
	}

	public void OnSocialGunEventButtonClick()
	{
		if (!(SkinEditorController.sharedController != null))
		{
			BannerWindowController bannerWindowController = BannerWindowController.SharedController;
			if (!(bannerWindowController == null))
			{
				socialBanner.Show();
			}
		}
	}

	private void OnDestroy()
	{
		if (NickLabelStack.sharedStack != null && NickLabelStack.sharedStack.gameObject != null)
		{
			NickLabelStack.sharedStack.gameObject.SetActive(true);
		}
		if (FriendsController.sharedController != null)
		{
			FriendsController.sharedController.GetComponent<TrafficForwardingScript>().Do(delegate(TrafficForwardingScript tf)
			{
				tf.Updated = (EventHandler<TrafficForwardingInfo>)Delegate.Remove(tf.Updated, new EventHandler<TrafficForwardingInfo>(RefreshTrafficForwardingButton));
			});
		}
		SocialGunBannerView.SocialGunBannerViewLoginCompletedWithResult -= HandleSocialGunViewLoginCompleted;
		PromoActionsManager.EventX3Updated -= OnEventX3Updated;
		StarterPackController.OnStarterPackEnable -= OnStarterPackContainerShow;
		PromoActionsManager.OnDayOfValorEnable -= OnDayOfValorContainerShow;
		LocalizationStore.DelEventCallAfterLocalize(ChangeLocalizeLabel);
		PromoActionClick.Click -= HandlePromoActionClicked;
		SettingsController.ControlsClicked -= base.HandleControlsClicked;
		sharedController = null;
		if (FreeAwardController.Instance != null)
		{
			FreeAwardController instance = FreeAwardController.Instance;
			instance.transform.root.Map((Transform t) => t.gameObject).Do(UnityEngine.Object.Destroy);
		}
		if (!TrainingController.TrainingCompleted)
		{
			AskNameManager.onComplete -= HintController.instance.ShowCurrentHintObjectLabel;
		}
	}

	private void OnGUI()
	{
		if (!Launcher.UsingNewLauncher && _drawLoadingProgress)
		{
			ActivityIndicator.LoadingProgress = 1f;
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

	private void CheckIfPendingAward()
	{
		if (Storager.hasKey("PendingFreeAward"))
		{
			int @int = Storager.getInt("PendingFreeAward", false);
			if (@int > 0)
			{
				int num = FreeAwardController.Instance.GiveAwardAndIncrementCount();
				Storager.setInt("PendingInterstitial", 0, false);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Context", "FreeAwardVideo");
				Dictionary<string, string> dictionary2 = dictionary;
				dictionary2.Add("Device", SystemInfo.deviceModel);
				dictionary2.Add("Provider", @int.ToString());
				if (ExperienceController.sharedController != null)
				{
					dictionary2.Add("Level", ExperienceController.sharedController.currentLevel.ToString());
				}
				if (ExpController.Instance != null)
				{
					dictionary2.Add("Tier", ExpController.Instance.OurTier.ToString());
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Crash on advertising", dictionary2);
			}
		}
		if (!Storager.hasKey("PendingInterstitial"))
		{
			return;
		}
		int int2 = Storager.getInt("PendingInterstitial", false);
		if (int2 > 0)
		{
			Storager.setInt("PendingInterstitial", 0, false);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Context", "Interstitial");
			Dictionary<string, string> dictionary3 = dictionary;
			dictionary3.Add("Device", SystemInfo.deviceModel);
			dictionary3.Add("Provider", int2.ToString());
			if (ExperienceController.sharedController != null)
			{
				dictionary3.Add("Level", ExperienceController.sharedController.currentLevel.ToString());
			}
			if (ExpController.Instance != null)
			{
				dictionary3.Add("Tier", ExpController.Instance.OurTier.ToString());
			}
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Crash on advertising", dictionary3);
		}
	}

	private void RefreshSettingsButton()
	{
		if (!(settingsButton == null))
		{
			ButtonHandler component = settingsButton.GetComponent<ButtonHandler>();
			if (component != null)
			{
				component.Clicked += HandleSettingsClicked;
			}
			UIButton component2 = settingsButton.GetComponent<UIButton>();
			if (component2 != null)
			{
				component2.isEnabled = TrainingController.TrainingCompleted;
			}
		}
	}

	private new IEnumerator Start()
	{
		ConnectSceneNGUIController.isReturnFromGame = false;
		if (Storager.hasKey("Analytics:af_tutorial_completion"))
		{
			AnalyticsStuff.TrySendOnceToAppsFlyer("tutorial_lobby");
		}
		string myNick = ProfileController.GetPlayerNameOrDefault();
		string filteredNick = FilterBadWorld.FilterString(myNick);
		if (string.IsNullOrEmpty(filteredNick) || filteredNick.Trim() == string.Empty)
		{
			filteredNick = ProfileController.defaultPlayerName;
		}
		if (filteredNick != myNick)
		{
			if (Application.isEditor)
			{
				Debug.Log("Saving new name:    " + filteredNick);
			}
			PlayerPrefs.SetString("NamePlayer", filteredNick);
		}
		Storager.setInt(Defs.ShownLobbyLevelSN, 3, false);
		base.transform.GetChild(0).GetComponent<UICamera>().allowMultiTouch = false;
		Defs.isDaterRegim = false;
		SocialGunBannerView.SocialGunBannerViewLoginCompletedWithResult += HandleSocialGunViewLoginCompleted;
		TwitterController.CheckAndGiveTwitterReward("Start");
		if (FriendsController.sharedController != null && !string.IsNullOrEmpty(FriendsController.sharedController.id))
		{
			ClanIncomingInvitesController.FetchClanIncomingInvites(FriendsController.sharedController.id);
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Resetting request for interstitial advertisement.");
		}
		ConnectSceneNGUIController.InterstitialRequest = false;
		CheckIfPendingAward();
		if (socialButton != null)
		{
			socialButton.gameObject.SetActive(true);
			ButtonHandler handler = socialButton.GetComponent<ButtonHandler>();
			handler.Clicked += HandleSocialButton;
		}
		WeaponManager.RefreshExpControllers();
		PlayerPrefs.SetInt("CountRunMenu", PlayerPrefs.GetInt("CountRunMenu", 0) + 1);
		freeAwardChestObj = GameObject.FindGameObjectWithTag("FreeAwardChest");
		freeAwardChestObj.SetActive(false);
		premiumTime.gameObject.SetActive(true);
		InitializeBannerWindow();
		bool developerConsoleEnabled = Debug.isDebugBuild;
		if (developerConsole != null)
		{
			developerConsole.gameObject.SetActive(developerConsoleEnabled);
		}
		if (Device.isWeakDevice || Tools.RuntimePlatform == RuntimePlatform.MetroPlayerX64)
		{
			ParticleSystem[] particleSystems = UnityEngine.Object.FindObjectsOfType<ParticleSystem>();
			ParticleSystem[] array = particleSystems;
			foreach (ParticleSystem p in array)
			{
				p.gameObject.SetActive(false);
			}
		}
		starterPackPanel.gameObject.SetActive(false);
		TrafficForwardingScript trafficForwardingScript = FriendsController.sharedController.Map((FriendsController fc) => fc.GetComponent<TrafficForwardingScript>());
		if (trafficForwardingScript != null)
		{
			trafficForwardingScript.Updated = (EventHandler<TrafficForwardingInfo>)Delegate.Combine(trafficForwardingScript.Updated, new EventHandler<TrafficForwardingInfo>(RefreshTrafficForwardingButton));
			System.Threading.Tasks.Task<TrafficForwardingInfo> trafficForwardingResult = trafficForwardingScript.GetTrafficForwardingInfo().Filter((System.Threading.Tasks.Task<TrafficForwardingInfo> t) => ((System.Threading.Tasks.Task)t).IsCompleted && !((System.Threading.Tasks.Task)t).IsCanceled && !((System.Threading.Tasks.Task)t).IsFaulted);
			if (trafficForwardingResult != null)
			{
				_trafficForwardingUrl = trafficForwardingResult.Result.Url;
			}
			if (trafficForwardingButton != null)
			{
				RefreshTrafficForwardingButton(this, (trafficForwardingResult == null) ? TrafficForwardingInfo.DisabledInstance : trafficForwardingResult.Result);
			}
		}
		socialGunPanel.gameObject.SetActive(false);
		dayOfValorContainer.gameObject.SetActive(false);
		stubLoading.SetActive(true);
		string bgTextureName = ConnectSceneNGUIController.MainLoadingTexture();
		stubTexture.mainTexture = Resources.Load<Texture>(bgTextureName);
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager();
		base.Start();
		idleTimerLastTime = Time.realtimeSinceStartup;
		SettingsController.ControlsClicked += base.HandleControlsClicked;
		Defs.isShowUserAgrement = false;
		completeTraining.SetActive(!shopButton.GetComponent<UIButton>().isEnabled);
		mainPanel.SetActive(true);
		settingsPanel.SetActive(false);
		newsPanel.SetActive(false);
		freePanel.SetActive(false);
		SettingsJoysticksPanel.SetActive(false);
		ConnectSceneNGUIController.NeedShowReviewInConnectScene = false;
		sharedController = this;
		if (campaignButton != null)
		{
			ButtonHandler bh = campaignButton.GetComponent<ButtonHandler>();
			if (bh != null)
			{
				bh.Clicked += HandleCampaingClicked;
			}
		}
		if (multiplayerButton != null)
		{
			ButtonHandler bh2 = multiplayerButton.GetComponent<ButtonHandler>();
			if (bh2 != null)
			{
				bh2.Clicked += HandleMultiPlayerClicked;
			}
		}
		if (skinsMakerButton != null)
		{
			if (MainMenu.SkinsMakerSupproted())
			{
				ButtonHandler bh23 = skinsMakerButton.GetComponent<ButtonHandler>();
				if (bh23 != null)
				{
					bh23.Clicked += HandleSkinsMakerClicked;
				}
			}
			else
			{
				skinsMakerButton.SetActive(false);
			}
		}
		if (profileButton != null)
		{
			ButtonHandler bh22 = profileButton.GetComponent<ButtonHandler>();
			if (bh22 != null)
			{
				bh22.Clicked += HandleProfileClicked;
			}
		}
		if (freeButton != null)
		{
			ButtonHandler bh21 = freeButton.GetComponent<ButtonHandler>();
			if (bh21 != null)
			{
				bh21.Clicked += HandleFreeClicked;
			}
		}
		if (gameCenterButton != null)
		{
			bool gameCenterButtonEnabled = BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer || (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite);
			gameCenterButton.SetActive(gameCenterButtonEnabled);
			UIButton gameServicesButton = gameCenterButton.GetComponent<UIButton>();
			if (gameServicesButton != null)
			{
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					List<UITexture> uiTextures = (from c in gameCenterButton.GetComponentsInChildren<UITexture>(true)
						where gameCenterButton == c.transform.parent.gameObject
						select c).ToList();
					if (uiTextures.Count == 1)
					{
						UITexture uiTexture = uiTextures[0];
						Texture texture = settingsPanel.Map((GameObject s) => s.GetComponent<SettingsController>()).Map((SettingsController s) => s.googlePlayServicesTexture);
						if (texture != null)
						{
							uiTexture.mainTexture = texture;
						}
						else
						{
							Debug.LogError("Could not find Google Play Game Services texture.");
						}
					}
					else
					{
						Debug.LogError("Expected only one UITexture for [Game Center] button, but actual count is: " + uiTextures.Count);
					}
				}
				ButtonHandler bh20 = gameCenterButton.GetComponent<ButtonHandler>();
				if (bh20 != null)
				{
					bh20.Clicked += HandleGameServicesClicked;
				}
			}
		}
		if (signOutButton != null)
		{
			bool signOutButtonActive = Application.isEditor || (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite);
			signOutButton.gameObject.SetActive(signOutButtonActive);
		}
		if (shopButton != null)
		{
			ButtonHandler bh19 = shopButton.GetComponent<ButtonHandler>();
			if (bh19 != null)
			{
				bh19.Clicked += HandleShopClicked;
			}
		}
		if (PlayerPrefs.GetInt(Defs.ShouldEnableShopSN, 0) == 1)
		{
			HandleShopClicked(null, null);
			PlayerPrefs.SetInt(Defs.ShouldEnableShopSN, 0);
			PlayerPrefs.Save();
			if (PromoActionsPanel != null && PromoActionsPanel.GetComponent<PromoActionsGUIController>() != null)
			{
				PromoActionsPanel.GetComponent<PromoActionsGUIController>().MarkUpdateOnEnable();
			}
		}
		RefreshSettingsButton();
		postNewsLabel.SetActive(false);
		if (postFacebookButton != null)
		{
			ButtonHandler bh18 = postFacebookButton.GetComponent<ButtonHandler>();
			if (bh18 != null)
			{
				bh18.Clicked += HandlePostFacebookClicked;
			}
		}
		if (postTwitterButton != null)
		{
			ButtonHandler bh17 = postTwitterButton.GetComponent<ButtonHandler>();
			if (bh17 != null)
			{
				bh17.Clicked += HandlePostTwittwerClicked;
			}
		}
		if (rateUsButton != null)
		{
			ButtonHandler bh16 = rateUsButton.GetComponent<ButtonHandler>();
			if (bh16 != null)
			{
				bh16.Clicked += HandleRateAsClicked;
			}
		}
		if (backFromFreeButton != null)
		{
			ButtonHandler bh15 = backFromFreeButton.GetComponent<ButtonHandler>();
			if (bh15 != null)
			{
				bh15.Clicked += HandleBackFromSocialClicked;
			}
		}
		if (facebookSubcribeButton != null)
		{
			ButtonHandler bh14 = facebookSubcribeButton.GetComponent<ButtonHandler>();
			if (bh14 != null)
			{
				bh14.Clicked += HandleFacebookSubscribeClicked;
			}
		}
		if (twitterSubcribeButton != null)
		{
			ButtonHandler bh13 = twitterSubcribeButton.GetComponent<ButtonHandler>();
			if (bh13 != null)
			{
				bh13.Clicked += HandleTwitterSubscribeClicked;
			}
		}
		if (supportButton != null)
		{
			ButtonHandler bh12 = supportButton.GetComponent<ButtonHandler>();
			if (bh12 != null)
			{
				bh12.Clicked += HandleSupportButtonClicked;
			}
		}
		if (friendsButton != null)
		{
			ButtonHandler bh11 = friendsButton.GetComponent<ButtonHandler>();
			if (bh11 != null)
			{
				bh11.Clicked += HandleFriendsClicked;
			}
		}
		if (newsButton != null)
		{
			ButtonHandler bh10 = newsButton.GetComponent<ButtonHandler>();
			if (bh10 != null)
			{
				bh10.Clicked += HandleNewsButtonClicked;
			}
		}
		if (newsBackButton != null)
		{
			ButtonHandler bh9 = newsBackButton.GetComponent<ButtonHandler>();
			if (bh9 != null)
			{
				bh9.Clicked += HandleNewsBackButtonClicked;
			}
		}
		if (enderManButton != null)
		{
			ButtonHandler bh8 = enderManButton.GetComponent<ButtonHandler>();
			if (bh8 != null)
			{
				bh8.Clicked += HandleEnderClicked;
			}
		}
		if (agreeButton != null)
		{
			ButtonHandler bh7 = agreeButton.GetComponent<ButtonHandler>();
			if (bh7 != null)
			{
				bh7.Clicked += HandleAgreeClicked;
			}
		}
		if (diclineButton != null)
		{
			ButtonHandler bh6 = diclineButton.GetComponent<ButtonHandler>();
			if (bh6 != null)
			{
				bh6.Clicked += HandleDiclineClicked;
			}
		}
		if (coinsShopButton != null)
		{
			ButtonHandler bh5 = coinsShopButton.GetComponent<ButtonHandler>();
			if (bh5 != null)
			{
				bh5.Clicked += HandleBankClicked;
			}
		}
		if (youtubeButton != null)
		{
			ButtonHandler bh4 = youtubeButton.GetComponent<ButtonHandler>();
			if (bh4 != null)
			{
				bh4.Clicked += HandleYoutubeClicked;
			}
		}
		if (everyplayButton != null)
		{
			ButtonHandler bh3 = everyplayButton.GetComponent<ButtonHandler>();
			if (bh3 != null)
			{
				bh3.Clicked += HandleEveryPlayClicked;
			}
		}
		if (BankController.Instance != null)
		{
			UnityEngine.Object.DontDestroyOnLoad(BankController.Instance.transform.root.gameObject);
		}
		else
		{
			Debug.LogWarning("bankController == null");
		}
		if (SingleModeOnStart)
		{
			OnClickSingleModeButton();
		}
		yield return new WaitForSeconds(0.5f);
		PromoActionClick.Click += HandlePromoActionClicked;
		yield return new WaitForSeconds(0.5f);
		if (friendsOnStart)
		{
			HandleFriendsClicked(null, null);
			yield return null;
		}
		_drawLoadingProgress = false;
		stubLoading.SetActive(false);
		ActivityIndicator.IsActiveIndicator = false;
		Debug.Log("Start initializing ProfileGui.");
		ProfileController profileController = UnityEngine.Object.FindObjectOfType<ProfileController>();
		if (profileController == null)
		{
			GameObject profileGuiRequest = Resources.Load<GameObject>("ProfileGui");
			yield return profileGuiRequest;
			GameObject go = UnityEngine.Object.Instantiate(profileGuiRequest);
			UnityEngine.Object.DontDestroyOnLoad(go);
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.LogFormat("Training completed: {0}. Authenticating...", TrainingController.TrainingCompleted);
		}
		if (!_socialNetworkingInitilized)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Social networking is not initialized.");
			}
			GameServicesController gameServicesController = UnityEngine.Object.FindObjectOfType<GameServicesController>();
			if (gameServicesController == null)
			{
				GameObject gameServicesControllerGo = new GameObject("Rilisoft.GameServicesController");
				gameServicesController = gameServicesControllerGo.AddComponent<GameServicesController>();
			}
			TrophiesSynchronizer.Instance.Pull();
			Action tryUpdateNickname = delegate
			{
				NickLabelController nickLabelController = sharedController.persNickLabel;
				if (nickLabelController != null)
				{
					persNickLabel.UpdateNickInLobby();
					nickLabelController.UpdateInfo();
				}
				else
				{
					Debug.LogWarning("nickLabelController == null");
				}
			};
			if (false)
			{
				Debug.Log("Play Game Services explicitly disabled.");
				_syncPromise.TrySetResult(false);
			}
			else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Application.isEditor)
				{
					yield return CoroutineRunner.Instance.StartCoroutine(SynchronizeEditorCoroutine());
				}
				else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					yield return CoroutineRunner.Instance.StartCoroutine(SynchronizeGoogleCoroutine(tryUpdateNickname, gameServicesController));
				}
				else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					yield return CoroutineRunner.Instance.StartCoroutine(SynchronizeAmazonCoroutine(tryUpdateNickname, gameServicesController));
				}
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				GameCenterSingleton initializedInstance = GameCenterSingleton.Instance;
				_socialNetworkingInitilized = true;
				yield return null;
				gameServicesController.WaitAuthenticationAndIncrementBeginnerAchievement();
				_syncPromise.TrySetResult(false);
			}
			else
			{
				_syncPromise.TrySetResult(false);
			}
		}
		if (bannerContainer != null)
		{
			InGameGUI.SetLayerRecursively(bannerContainer, LayerMask.NameToLayer("Banners"));
		}
		PromoActionsManager.EventX3Updated += OnEventX3Updated;
		OnEventX3Updated();
		StarterPackController.OnStarterPackEnable += OnStarterPackContainerShow;
		OnStarterPackContainerShow(StarterPackController.Get.isEventActive);
		PromoActionsManager.OnDayOfValorEnable += OnDayOfValorContainerShow;
		OnDayOfValorContainerShow(PromoActionsManager.sharedManager.IsDayOfValorEventActive);
		if (ReplaceAdmobPerelivController.sharedController != null && ReplaceAdmobPerelivController.sharedController.ShouldShowInLobby && ReplaceAdmobPerelivController.sharedController.DataLoaded)
		{
			ReplaceAdmobPerelivController.sharedController.ShouldShowInLobby = false;
			ReplaceAdmobPerelivController.TryShowPereliv("Lobby after launch");
			ReplaceAdmobPerelivController.sharedController.DestroyImage();
		}
		string key = GetAbuseKey_f1a4329e(4054069918u);
		if (Storager.hasKey(key))
		{
			string ticksHalvedString = Storager.getString(key, false);
			if (!string.IsNullOrEmpty(ticksHalvedString) && ticksHalvedString != "0")
			{
				long nowTicksHalved = DateTime.UtcNow.Ticks >> 1;
				long abuseTicksHalved = nowTicksHalved;
				if (long.TryParse(ticksHalvedString, out abuseTicksHalved))
				{
					abuseTicksHalved = Math.Min(nowTicksHalved, abuseTicksHalved);
					Storager.setString(key, abuseTicksHalved.ToString(), false);
				}
				else
				{
					Storager.setString(key, nowTicksHalved.ToString(), false);
				}
				TimeSpan timespan = TimeSpan.FromTicks(nowTicksHalved - abuseTicksHalved);
				if (((!Defs.IsDeveloperBuild) ? (timespan.TotalDays >= 1.0) : (timespan.TotalMinutes >= 3.0)) && Application.platform != RuntimePlatform.IPhonePlayer)
				{
					PhotonNetwork.PhotonServerSettings.AppID = "68c9fbdb-682a-411f-a229-1a9786b5835c";
					PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
				}
			}
		}
		StartCoroutine(TryToShowExpiredBanner());
		LocalizationStore.AddEventCallAfterLocalize(ChangeLocalizeLabel);
		ChangeLocalizeLabel();
		if (friendsOnStart)
		{
			if (mainPanel != null)
			{
				mainPanel.transform.root.gameObject.SetActive(false);
			}
			friendsOnStart = false;
		}
		newsIndicator.SetActive(PlayerPrefs.GetInt("LobbyIsAnyNewsKey", 0) == 1);
		if (!TrainingController.TrainingCompleted)
		{
			AskNameManager.onComplete += HintController.instance.ShowCurrentHintObjectLabel;
		}
		if (MainMenuController.onLoadMenu != null)
		{
			MainMenuController.onLoadMenu();
		}
		if (MainMenuController.onEnableMenuForAskname != null)
		{
			MainMenuController.onEnableMenuForAskname();
		}
		QuestSystem.Instance.QuestCompleted += OnCompletedQuest;
	}

	internal static IEnumerator SaveItemsToStorager(Action callback)
	{
		Debug.LogFormat("> MainMenuController.SaveItemsToStorager {0:F3}", Time.realtimeSinceStartup);
		try
		{
			if (InfoWindowController.Instance.background != null)
			{
				while (InfoWindowController.IsActive)
				{
					yield return null;
				}
			}
			yield return null;
			IDisposable escapeSubscription = BackSystem.Instance.Register(delegate
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.Log("Ignoring [Escape] while syncing weapons.");
				}
			}, "MainMenuWaitingSaving");
			string caption = LocalizationStore.Get("Key_1974");
			ActivityIndicator.SetActiveWithCaption(caption);
			InfoWindowController.BlockAllClick();
			yield return CoroutineRunner.Instance.StartCoroutine(PurchasesSynchronizer.Instance.SavePendingItemsToStorager());
			InfoWindowController.HideCurrentWindow();
			ActivityIndicator.IsActiveIndicator = false;
			escapeSubscription.Dispose();
			if (callback != null)
			{
				callback();
			}
		}
		finally
		{
			Debug.LogFormat("< MainMenuController.SaveItemsToStorager {0:F3}", Time.realtimeSinceStartup);
		}
	}

	private static void OnCompletedQuest(object sender, QuestCompletedEventArgs e)
	{
		AccumulativeQuestBase accumulativeQuestBase = e.Quest as AccumulativeQuestBase;
		if (accumulativeQuestBase != null)
		{
			InfoWindowController.ShowAchievementBox(string.Empty, QuestConstants.GetAccumulativeQuestDescriptionByType(accumulativeQuestBase));
		}
	}

	private void HandleSocialGunViewLoginCompleted(bool success)
	{
		if (!(mainPanel == null))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/" + ((!success) ? "PanelAuthFailed" : "PanelAuthSucces")));
			gameObject.transform.parent = ((!(freePanel != null) || !freePanel.activeInHierarchy) ? mainPanel.transform : freePanel.transform);
			Player_move_c.SetLayerRecursively(gameObject, mainPanel.layer);
			gameObject.transform.localPosition = new Vector3(0f, 0f, -130f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	public void HandleClansClicked()
	{
		if (_shopInstance != null || ShowBannerOrLevelup())
		{
			return;
		}
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Clans", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		Action action = delegate
		{
			if (!ProtocolListGetter.currentVersionIsSupported)
			{
				BannerWindowController bannerWindowController = BannerWindowController.SharedController;
				if (bannerWindowController != null)
				{
					bannerWindowController.ForceShowBanner(BannerWindowType.NewVersion);
				}
			}
			else
			{
				GoClans();
			}
		};
		action();
	}

	private void ChangeLocalizeLabel()
	{
		_localizeSaleLabel = LocalizationStore.Get("Key_0419");
	}

	private void GoClans()
	{
		MenuBackgroundMusic.keepPlaying = true;
		LoadConnectScene.textureToShow = null;
		LoadConnectScene.sceneToLoad = "Clans";
		LoadConnectScene.noteToShow = null;
		Singleton<SceneLoader>.Instance.LoadScene(Defs.PromSceneName);
	}

	private static string GetAbuseKey_f1a4329e(uint pad)
	{
		return (0x354E43A7u ^ pad).ToString("x");
	}

	public static bool IsShowRentExpiredPoint()
	{
		if (sharedController == null)
		{
			return false;
		}
		Transform rentExpiredPoint = sharedController.RentExpiredPoint;
		if (rentExpiredPoint == null)
		{
			return false;
		}
		return rentExpiredPoint.childCount > 0;
	}

	public static bool SavedShwonLobbyLevelIsLessThanActual()
	{
		return Storager.getInt(Defs.ShownLobbyLevelSN, false) < ExpController.LobbyLevel;
	}

	private IEnumerator TryToShowExpiredBanner()
	{
		while (FriendsController.sharedController == null || TempItemsController.sharedController == null)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(1f));
			try
			{
				if (ShopNGUIController.GuiActive || (FreeAwardController.Instance != null && !FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>()) || (BannerWindowController.SharedController != null && BannerWindowController.SharedController.IsAnyBannerShown) || (BankController.Instance != null && BankController.Instance.InterfaceEnabled) || settingsPanel.activeInHierarchy || freePanel.activeInHierarchy || supportPanel.activeInHierarchy || (ProfileController.Instance != null && ProfileController.Instance.InterfaceEnabled) || (FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled) || stubLoading.activeInHierarchy || singleModePanel.activeSelf || UserAgreementPanel.activeInHierarchy || SettingsJoysticksPanel.activeInHierarchy || LeaderboardsPanel.gameObject.activeInHierarchy || (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown) || RentExpiredPoint.childCount != 0)
				{
					continue;
				}
				if (SavedShwonLobbyLevelIsLessThanActual())
				{
					GameObject window = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("LobbyLevels/LobbyLevelTips_" + (Storager.getInt(Defs.ShownLobbyLevelSN, false) + 1)));
					window.transform.parent = RentExpiredPoint;
					Player_move_c.SetLayerRecursively(window, LayerMask.NameToLayer("NGUI"));
					window.transform.localPosition = new Vector3(0f, 0f, -130f);
					window.transform.localRotation = Quaternion.identity;
					window.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				else
				{
					if (Storager.getInt(Defs.PremiumEnabledFromServer, false) == 1 && ShopNGUIController.ShowPremimAccountExpiredIfPossible(RentExpiredPoint, "NGUI", string.Empty))
					{
						continue;
					}
					ShopNGUIController.ShowTempItemExpiredIfPossible(RentExpiredPoint, "NGUI", null, null, null, delegate(string item)
					{
						if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.weaponsInGame != null)
						{
							int num = PromoActionsGUIController.CatForTg(item);
							if (num != -1)
							{
								ShopNGUIController.SetAsEquippedAndSendToServer(item, (ShopNGUIController.CategoryNames)num);
							}
						}
						if (PersConfigurator.currentConfigurator != null)
						{
							PersConfigurator.currentConfigurator._AddCapeAndHat();
						}
					});
					continue;
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("exception in Lobby  TryToShowExpiredBanner: " + e);
			}
		}
	}

	public void HandleDeveloperConsoleClicked()
	{
	}

	public void HandlePromoActionClicked(string tg)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Promoactions", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		AnalyticsStuff.LogSpecialOffersPanel("Efficiency", "View");
		if (tg != null && tg == "StickersPromoActionsPanelKey")
		{
			ButtonClickSound.Instance.PlayClick();
			BuySmileBannerController.openedFromPromoActions = true;
			OnBuySmilesClick();
			return;
		}
		int num = -1;
		ItemRecord byTag = ItemDb.GetByTag(tg);
		if (byTag != null)
		{
			string prefabName = byTag.PrefabName;
			if (prefabName != null)
			{
				UnityEngine.Object[] weaponsInGame = WeaponManager.sharedManager.weaponsInGame;
				for (int i = 0; i < weaponsInGame.Length; i++)
				{
					GameObject gameObject = (GameObject)weaponsInGame[i];
					if (gameObject.name == prefabName)
					{
						num = gameObject.GetComponent<WeaponSounds>().categoryNabor - 1;
						break;
					}
				}
			}
		}
		if (num == -1)
		{
			bool flag = false;
			foreach (KeyValuePair<ShopNGUIController.CategoryNames, List<List<string>>> item in Wear.wear)
			{
				foreach (List<string> item2 in item.Value)
				{
					if (item2.Contains(tg))
					{
						flag = true;
						num = (int)item.Key;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		if (num == -1 && (SkinsController.skinsNamesForPers.ContainsKey(tg) || tg.Equals("CustomSkinID")))
		{
			num = 8;
		}
		if (ShopNGUIController.sharedShop != null)
		{
			ShopNGUIController.sharedShop.SetOfferID(tg);
			ShopNGUIController.sharedShop.IsInShopFromPromoPanel(true, tg);
			ShopNGUIController.sharedShop.offerCategory = (ShopNGUIController.CategoryNames)num;
		}
		HandleShopClicked(null, EventArgs.Empty);
	}

	private void CalcBtnRects()
	{
		Transform transform = NGUITools.GetRoot(base.gameObject).transform;
		Camera component = transform.GetChild(0).GetComponent<Camera>();
		Transform relativeTo = component.transform;
		float num = 768f;
		float num2 = num * ((float)Screen.width / (float)Screen.height);
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, shopButton.GetComponent<UIButton>().tweenTarget.transform, true);
		bounds.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		shopRect = new Rect((bounds.center.x - 105.5f) * Defs.Coef, (bounds.center.y - 57f) * Defs.Coef, 211f * Defs.Coef, 114f * Defs.Coef);
		Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, survivalButton.GetComponent<UIButton>().tweenTarget.transform, true);
		bounds2.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		survivalRect = new Rect((bounds2.center.x - 107f) * Defs.Coef, (bounds2.center.y - 35f) * Defs.Coef, 214f * Defs.Coef, 70f * Defs.Coef);
		Bounds bounds3 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, campaignButton.GetComponent<UIButton>().tweenTarget.transform, true);
		bounds3.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		campaignRect = new Rect((bounds3.center.x - 107f) * Defs.Coef, (bounds3.center.y - 35f) * Defs.Coef, 214f * Defs.Coef, 70f * Defs.Coef);
	}

	private void UpdateEventX3RemainedTime()
	{
		long eventX3RemainedTime = PromoActionsManager.sharedManager.EventX3RemainedTime;
		TimeSpan timeSpan = TimeSpan.FromSeconds(eventX3RemainedTime);
		string empty = string.Empty;
		empty = ((timeSpan.Days <= 0) ? string.Format("{0}: {1:00}:{2:00}:{3:00}", _localizeSaleLabel, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) : string.Format("{0}: {1} {2} {3:00}:{4:00}:{5:00}", _localizeSaleLabel, timeSpan.Days, (timeSpan.Days != 1) ? "Days" : "Day", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
		if (eventX3RemainTime != null)
		{
			for (int i = 0; i < eventX3RemainTime.Length; i++)
			{
				eventX3RemainTime[i].text = empty;
			}
		}
		if (colorBlinkForX3 != null && timeSpan.TotalHours < (double)Defs.HoursToEndX3ForIndicate && !colorBlinkForX3.enabled)
		{
			colorBlinkForX3.enabled = true;
		}
	}

	public bool PromoOffersPanelShouldBeShown()
	{
		return _shopInstance == null && !ShowBannerOrLevelup();
	}

	private void Update()
	{
		if (InAdventureScreen && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = true;
		}
		if (settingsPanel.activeInHierarchy)
		{
			if (twitterConnectedSettings.activeSelf != (TwitterController.TwitterSupported && TwitterController.IsLoggedIn))
			{
				twitterConnectedSettings.SetActive(TwitterController.TwitterSupported && TwitterController.IsLoggedIn);
			}
			if (twitterDisconnectedSettings.activeSelf != (TwitterController.TwitterSupported && !TwitterController.IsLoggedIn && Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 1))
			{
				twitterDisconnectedSettings.SetActive(TwitterController.TwitterSupported && !TwitterController.IsLoggedIn && Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 1);
			}
			if (twitterConnectSettings.activeSelf != (TwitterController.TwitterSupported && !TwitterController.IsLoggedIn && Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 0))
			{
				twitterConnectSettings.SetActive(TwitterController.TwitterSupported && !TwitterController.IsLoggedIn && Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 0);
			}
			if (twitterLoginContainer != null)
			{
				twitterLoginContainer.SetActive(TwitterController.TwitterSupported);
			}
		}
		if (freePanel.activeInHierarchy)
		{
			if (postTwitterButton.activeSelf != (TwitterController.TwitterSupported && TwitterController.TwitterSupported_OldPosts && TwitterController.IsLoggedIn))
			{
				postTwitterButton.SetActive(TwitterController.TwitterSupported && TwitterController.TwitterSupported_OldPosts && TwitterController.IsLoggedIn);
			}
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer && gameCenterButton.activeSelf != Social.localUser.authenticated)
		{
			gameCenterButton.SetActive(Social.localUser.authenticated);
		}
		bool active = (Storager.getInt(Defs.PremiumEnabledFromServer, false) == 1 || PremiumAccountController.Instance.isAccountActive) && ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel >= 3;
		premium.SetActive(active);
		premiumButton.isEnabled = Storager.getInt(Defs.PremiumEnabledFromServer, false) == 1;
		if (premiumUpPlashka.activeSelf != (!(PremiumAccountController.Instance != null) || !PremiumAccountController.Instance.isAccountActive))
		{
			premiumUpPlashka.SetActive(!(PremiumAccountController.Instance != null) || !PremiumAccountController.Instance.isAccountActive);
		}
		if (premiumbottomPlashka.activeSelf != (PremiumAccountController.Instance != null && PremiumAccountController.Instance.isAccountActive))
		{
			premiumbottomPlashka.SetActive(PremiumAccountController.Instance != null && PremiumAccountController.Instance.isAccountActive);
		}
		if (PremiumAccountController.Instance != null)
		{
			long num = PremiumAccountController.Instance.GetDaysToEndAllAccounts();
			for (int i = 0; i < premiumLevels.Count; i++)
			{
				bool flag = false;
				if (num > 0 && num < 3 && i == 0)
				{
					flag = true;
				}
				if (num >= 3 && num < 7 && i == 1)
				{
					flag = true;
				}
				if (num >= 7 && num < 30 && i == 2)
				{
					flag = true;
				}
				if (num >= 30 && i == 3)
				{
					flag = true;
				}
				if (premiumLevels[i].activeSelf != flag)
				{
					premiumLevels[i].SetActive(flag);
				}
			}
			if (Time.realtimeSinceStartup - _timePremiumTimeUpdated >= 1f)
			{
				premiumTime.text = PremiumAccountController.Instance.GetTimeToEndAllAccounts();
				_timePremiumTimeUpdated = Time.realtimeSinceStartup;
			}
		}
		bool flag2 = (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && !ShopNGUIController.GuiActive;
		if (starParticleStarterPackGaemObject != null && starParticleStarterPackGaemObject.activeInHierarchy != flag2)
		{
			starParticleStarterPackGaemObject.SetActive(flag2);
		}
		if (starParticleSocialGunButton != null && starParticleSocialGunButton.activeInHierarchy != flag2)
		{
			starParticleSocialGunButton.SetActive(flag2);
		}
		if (Time.realtimeSinceStartup - _eventX3RemainTimeLastUpdateTime >= 0.5f)
		{
			_eventX3RemainTimeLastUpdateTime = Time.realtimeSinceStartup;
			UpdateEventX3RemainedTime();
			if (_dayOfValorEnabled)
			{
				dayOfValorTimer.text = PromoActionsManager.sharedManager.GetTimeToEndDaysOfValor();
			}
		}
		if (_isCancellationRequested)
		{
			MainMenuController mainMenuController = sharedController;
			if (SettingsJoysticksPanel.activeSelf)
			{
				SettingsJoysticksPanel.SetActive(false);
				settingsPanel.SetActive(true);
			}
			else if (freePanel.activeSelf)
			{
				if (_shopInstance == null && !ShowBannerOrLevelup())
				{
					mainPanel.SetActive(true);
					freePanel.SetActive(false);
					rotateCamera.OnMainMenuCloseOptions();
				}
			}
			else if (newsPanel.activeSelf)
			{
				mainPanel.SetActive(true);
				newsPanel.SetActive(false);
			}
			else if (BannerWindowController.SharedController != null && BannerWindowController.SharedController.IsAnyBannerShown)
			{
				BannerWindowController.SharedController.HideBannerWindow();
			}
			else if ((!(settingsPanel != null) || !settingsPanel.activeInHierarchy) && (!(freePanel != null) || !freePanel.activeInHierarchy) && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && !ShopNGUIController.GuiActive && (!(ProfileController.Instance != null) || !ProfileController.Instance.InterfaceEnabled))
			{
				if (PremiumAccountScreenController.Instance != null)
				{
					PremiumAccountScreenController.Instance.Hide();
				}
				else if (mainMenuController != null && mainMenuController.singleModePanel.activeSelf)
				{
					mainMenuController.OnClickBackSingleModeButton();
				}
				else
				{
					PlayerPrefs.Save();
					Application.Quit();
				}
			}
			_isCancellationRequested = false;
		}
		if (GiftBannerWindow.instance != null && GiftBannerWindow.instance.IsShow)
		{
			PromoActionsPanel.SetActive(false);
		}
		else
		{
			PromoActionsPanel.SetActive(PromoOffersPanelShouldBeShown() && Storager.getInt(Defs.ShownLobbyLevelSN, false) > 2);
		}
		if (rotateCamera != null && !rotateCamera.IsAnimPlaying)
		{
			float num2 = -120f;
			num2 *= ((BuildSettings.BuildTargetPlatform != RuntimePlatform.Android) ? 0.5f : 2f);
			Rect rect;
			if (settingsPanel.activeInHierarchy)
			{
				rect = new Rect(0f, 0.1f * (float)Screen.height, 0.5f * (float)Screen.width, 0.8f * (float)Screen.height);
			}
			else
			{
				if (campaignRect.width.Equals(0f))
				{
					CalcBtnRects();
				}
				rect = ((!(MenuLeaderboardsController.sharedController != null) || !MenuLeaderboardsController.sharedController.IsOpened) ? new Rect(0.2f * (float)Screen.width, 0.25f * (float)Screen.height, 1.4f * (float)Screen.width, 0.65f * (float)Screen.height) : new Rect(0.38f * (float)Screen.width, 0.25f * (float)Screen.height, 1.4f * (float)Screen.width, 0.65f * (float)Screen.height));
			}
			if (canRotationLobbyPlayer)
			{
				if (Input.touchCount > 0 && !ShopNGUIController.GuiActive)
				{
					Touch touch = Input.GetTouch(0);
					if (touch.phase == TouchPhase.Moved && rect.Contains(touch.position))
					{
						idleTimerLastTime = Time.realtimeSinceStartup;
						pers.Rotate(Vector3.up, touch.deltaPosition.x * num2 * 0.5f * (Time.realtimeSinceStartup - lastTime));
					}
				}
				if (Application.isEditor && !ShopNGUIController.GuiActive)
				{
					float num3 = Input.GetAxis("Mouse ScrollWheel") * 3f * num2 * (Time.realtimeSinceStartup - lastTime);
					pers.Rotate(Vector3.up, num3);
					if (num3 != 0f)
					{
						idleTimerLastTime = Time.realtimeSinceStartup;
					}
				}
			}
			lastTime = Time.realtimeSinceStartup;
		}
		if (Time.realtimeSinceStartup - idleTimerLastTime > ShopNGUIController.IdleTimeoutPers)
		{
			ReturnPersTonNormState();
		}
		if (_starterPackEnabled)
		{
			starterPackTimer.text = StarterPackController.Get.GetTimeToEndEvent();
		}
		if (!sharedController.stubLoading.activeInHierarchy && (!(ShopNGUIController.sharedShop != null) || !ShopNGUIController.GuiActive) && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && (!(BannerWindowController.SharedController != null) || !BannerWindowController.SharedController.IsAnyBannerShown) && !singleModePanel.gameObject.activeSelf)
		{
			if (!TrainingController.TrainingCompleted)
			{
				return;
			}
			if (true && MobileAdManager.AdIsApplicable(MobileAdManager.Type.Video) && !_timeTamperingDetected.Value && FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>())
			{
				if (FreeAwardController.Instance.AdvertCountLessThanLimit())
				{
					freeAwardChestObj.SetActive(true);
				}
				else if (freeAwardChestObj.GetActive())
				{
					FreeAwardShowHandler.Instance.HideChestWithAnimation();
				}
			}
		}
		if (_newClanIncomingInvitesSprite.Value != null)
		{
			if (ClanIncomingInvitesController.CurrentRequest == null || !((System.Threading.Tasks.Task)ClanIncomingInvitesController.CurrentRequest).IsCompleted)
			{
				_newClanIncomingInvitesSprite.Value.gameObject.SetActive(false);
			}
			else if (((System.Threading.Tasks.Task)ClanIncomingInvitesController.CurrentRequest).IsCanceled || ((System.Threading.Tasks.Task)ClanIncomingInvitesController.CurrentRequest).IsFaulted)
			{
				_newClanIncomingInvitesSprite.Value.gameObject.SetActive(false);
			}
			else
			{
				_newClanIncomingInvitesSprite.Value.gameObject.SetActive(ClanIncomingInvitesController.CurrentRequest.Result.Count > 0);
			}
		}
	}

	private void HandleEscape()
	{
		if (_backSubscribers.Count > 0)
		{
			InvokeLastBackHandler();
		}
		else
		{
			_isCancellationRequested = true;
		}
	}

	private void ReturnPersTonNormState()
	{
		HOTween.Kill(pers);
		Vector3 p_endVal = new Vector3(-0.33f, 28f, -0.28f);
		idleTimerLastTime = Time.realtimeSinceStartup;
		HOTween.To(pers, 0.5f, new TweenParms().Prop("localRotation", new PlugQuaternion(p_endVal)).Ease(EaseType.Linear).OnComplete((TweenDelegate.TweenCallback)delegate
		{
			idleTimerLastTime = Time.realtimeSinceStartup;
		}));
	}

	protected override void HandleSavePosJoystikClicked(object sender, EventArgs e)
	{
		base.HandleSavePosJoystikClicked(sender, e);
		ExperienceController.sharedController.isShowRanks = true;
		ExpController.Instance.InterfaceEnabled = true;
	}

	private new void OnEnable()
	{
		base.OnEnable();
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Main Menu Controller");
		RewardedLikeButton[] componentsInChildren = GetComponentsInChildren<RewardedLikeButton>(true);
		RewardedLikeButton[] array = componentsInChildren;
		foreach (RewardedLikeButton rewardedLikeButton in array)
		{
			rewardedLikeButton.Refresh();
		}
		if (ExperienceController.sharedController != null && !ShopNGUIController.GuiActive)
		{
			ExperienceController.sharedController.isShowRanks = true;
		}
		if (ExpController.Instance != null)
		{
			ExpController.Instance.InterfaceEnabled = true;
		}
		if (MainMenuController.onActiveMainMenu != null)
		{
			MainMenuController.onActiveMainMenu(true);
		}
		if (MainMenuController.onEnableMenuForAskname != null)
		{
			MainMenuController.onEnableMenuForAskname();
		}
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
		if (MainMenuController.onActiveMainMenu != null)
		{
			MainMenuController.onActiveMainMenu(false);
		}
	}

	private void HandleAgreeClicked(object sender, EventArgs e)
	{
		Defs.isShowUserAgrement = false;
		UserAgreementPanel.SetActive(false);
		if (notShowAgain.value)
		{
			PlayerPrefs.SetInt("UserAgreement", 1);
		}
		if (isMultyPress)
		{
			GoMulty();
		}
		if (isFriendsPress)
		{
			GoFriens();
		}
	}

	private void HandleDiclineClicked(object sender, EventArgs e)
	{
		Defs.isShowUserAgrement = false;
		UserAgreementPanel.SetActive(false);
	}

	public void ShowBankWindow()
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Bank", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		if (_shopInstance != null)
		{
			Debug.LogWarning("_shopInstance != null");
			return;
		}
		if (BankController.Instance == null)
		{
			Debug.LogWarning("bankController == null");
			return;
		}
		if (BankController.Instance.InterfaceEnabledCoroutineLocked)
		{
			Debug.LogWarning("InterfaceEnabledCoroutineLocked");
			return;
		}
		BankController.Instance.BackRequested += HandleBackFromBankClicked;
		if ((!(GiftBannerWindow.instance == null) && GiftBannerWindow.instance.IsShow) || !ShowBannerOrLevelup())
		{
			_bankEnteredTime = Time.realtimeSinceStartup;
			ButtonClickSound.Instance.PlayClick();
			if (mainPanel != null)
			{
				mainPanel.transform.root.gameObject.SetActive(false);
			}
			if (nicknameLabel != null)
			{
				nicknameLabel.transform.root.gameObject.SetActive(false);
			}
			BankController.Instance.InterfaceEnabled = true;
		}
	}

	private void HandleBankClicked(object sender, EventArgs e)
	{
		ShowBankWindow();
	}

	private void HandleBackFromBankClicked(object sender, EventArgs e)
	{
		if (_shopInstance != null)
		{
			Debug.LogWarning("_shopInstance != null");
			return;
		}
		if (BankController.Instance == null)
		{
			Debug.LogWarning("bankController == null");
			return;
		}
		if (BankController.Instance.InterfaceEnabledCoroutineLocked)
		{
			Debug.LogWarning("InterfaceEnabledCoroutineLocked");
			return;
		}
		BankController.Instance.BackRequested -= HandleBackFromBankClicked;
		BankController.Instance.InterfaceEnabled = false;
		if (nicknameLabel != null)
		{
			nicknameLabel.transform.root.gameObject.SetActive(true);
		}
		if (mainPanel != null)
		{
			mainPanel.transform.root.gameObject.SetActive(true);
		}
		if (singleModePanel != null && singleModePanel.activeSelf)
		{
			ExperienceController.SetEnable(true);
		}
	}

	private void HandleEnderClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Ender", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		if (Application.isEditor)
		{
			Debug.Log(MainMenu.GetEndermanUrl());
		}
		else
		{
			Application.OpenURL(MainMenu.GetEndermanUrl());
		}
	}

	private void HandleSupportButtonClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Support", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		settingsPanel.SetActive(false);
		supportPanel.SetActive(true);
	}

	public void StartCampaingButton()
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Campaign", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		Action action = delegate
		{
			Defs.isFlag = false;
			Defs.isCOOP = false;
			Defs.isMulti = false;
			Defs.isHunger = false;
			Defs.isCompany = false;
			Defs.IsSurvival = false;
			Defs.isCapturePoints = false;
			GlobalGameController.Score = 0;
			WeaponManager.sharedManager.Reset();
			FlurryPluginWrapper.LogCampaignModePress();
			StoreKitEventListener.State.Mode = "Campaign";
			StoreKitEventListener.State.PurchaseKey = "In game";
			StoreKitEventListener.State.Parameters.Clear();
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{
					Defs.RankParameterKey,
					ExperienceController.sharedController.currentLevel.ToString()
				},
				{
					Defs.MultiplayerModesKey,
					StoreKitEventListener.State.Mode
				}
			};
			FlurryPluginWrapper.LogEvent(Defs.GameModesEventKey, parameters);
			MenuBackgroundMusic.keepPlaying = true;
			LoadConnectScene.textureToShow = null;
			LoadConnectScene.sceneToLoad = "CampaignChooseBox";
			LoadConnectScene.noteToShow = null;
			Application.LoadLevel(Defs.PromSceneName);
		};
		action();
	}

	private void HandleCampaingClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			StartCampaingButton();
		}
	}

	public void StartSurvivalButton()
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Survival", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		Action action = delegate
		{
			Defs.isFlag = false;
			Defs.isCOOP = false;
			Defs.isMulti = false;
			Defs.isHunger = false;
			Defs.isCompany = false;
			Defs.isCapturePoints = false;
			Defs.IsSurvival = true;
			CurrentCampaignGame.levelSceneName = string.Empty;
			GlobalGameController.Score = 0;
			WeaponManager.sharedManager.Reset();
			FlurryPluginWrapper.LogTrueSurvivalModePress();
			FlurryPluginWrapper.LogEvent("Launch_Survival");
			StoreKitEventListener.State.Mode = "Survival";
			StoreKitEventListener.State.PurchaseKey = "In game";
			StoreKitEventListener.State.Parameters.Clear();
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{
					Defs.RankParameterKey,
					ExperienceController.sharedController.currentLevel.ToString()
				},
				{
					Defs.MultiplayerModesKey,
					StoreKitEventListener.State.Mode
				}
			};
			FlurryPluginWrapper.LogEvent(Defs.GameModesEventKey, parameters);
			Defs.CurrentSurvMapIndex = UnityEngine.Random.Range(0, Defs.SurvivalMaps.Length);
			Singleton<SceneLoader>.Instance.LoadScene("CampaignLoading");
		};
		action();
	}

	public void HandleSurvivalClicked()
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			StartSurvivalButton();
		}
	}

	public void HandleSandboxClicked()
	{
		if (_shopInstance != null || ShowBannerOrLevelup())
		{
			return;
		}
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Sandbox", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		if (!ProtocolListGetter.currentVersionIsSupported)
		{
			BannerWindowController bannerWindowController = BannerWindowController.SharedController;
			if (bannerWindowController != null)
			{
				bannerWindowController.ForceShowBanner(BannerWindowType.NewVersion);
			}
		}
		else
		{
			GoSandBox();
		}
	}

	public void GoSandBox()
	{
		ButtonClickSound.Instance.PlayClick();
		Defs.isFlag = false;
		Defs.isCOOP = false;
		Defs.isMulti = true;
		Defs.isHunger = false;
		Defs.isCompany = false;
		Defs.IsSurvival = false;
		Defs.isFlag = false;
		Defs.isCapturePoints = false;
		FlurryPluginWrapper.LogDeathmatchModePress();
		MenuBackgroundMusic.keepPlaying = true;
		string path = ConnectSceneNGUIController.MainLoadingTexture();
		LoadConnectScene.textureToShow = Resources.Load<Texture>(path);
		LoadConnectScene.sceneToLoad = "ConnectSceneSandbox";
		FlurryPluginWrapper.LogEvent("Launch_Sandbox");
		LoadConnectScene.noteToShow = null;
		Application.LoadLevel(Defs.PromSceneName);
	}

	private void GoMulty()
	{
		Defs.isFlag = false;
		Defs.isCOOP = false;
		Defs.isMulti = true;
		Defs.isHunger = false;
		Defs.isCompany = false;
		Defs.IsSurvival = false;
		Defs.isFlag = false;
		Defs.isCapturePoints = false;
		FlurryPluginWrapper.LogDeathmatchModePress();
		MenuBackgroundMusic.keepPlaying = true;
		string path = ConnectSceneNGUIController.MainLoadingTexture();
		LoadConnectScene.textureToShow = Resources.Load<Texture>(path);
		LoadConnectScene.sceneToLoad = "ConnectScene";
		FlurryPluginWrapper.LogEvent("Launch_Multiplayer");
		LoadConnectScene.noteToShow = null;
		Application.LoadLevel(Defs.PromSceneName);
	}

	public void OnClickMultiplyerButton()
	{
		ButtonClickSound.Instance.PlayClick();
		Action action = delegate
		{
			if (!ProtocolListGetter.currentVersionIsSupported)
			{
				BannerWindowController bannerWindowController = BannerWindowController.SharedController;
				if (bannerWindowController != null)
				{
					bannerWindowController.ForceShowBanner(BannerWindowType.NewVersion);
				}
			}
			else
			{
				GoMulty();
			}
		};
		action();
	}

	public void HandleMultiPlayerClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			OnClickMultiplyerButton();
		}
	}

	private void HandleSkinsMakerClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Skins Maker", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			PlayerPrefs.SetInt(Defs.SkinEditorMode, 0);
			FlurryPluginWrapper.LogSkinsMakerModePress();
			FlurryPluginWrapper.LogSkinsMakerEnteredEvent();
			GlobalGameController.EditingCape = 0;
			GlobalGameController.EditingLogo = 0;
			FlurryPluginWrapper.LogEvent("Launch_Skins Maker");
			Singleton<SceneLoader>.Instance.LoadScene("SkinEditor");
		}
	}

	private IEnumerator HideMenuInterfaceCoroutine(GameObject nickLabelObj)
	{
		yield return null;
		if (nickLabelObj != null)
		{
			nickLabelObj.SetActive(false);
		}
		rotateCamera.gameObject.SetActive(false);
		if (mainPanel != null)
		{
			mainPanel.transform.root.gameObject.SetActive(false);
		}
	}

	private void GoFriens()
	{
		MenuBackgroundMusic.keepPlaying = true;
		if (FriendsWindowGUI.Instance == null)
		{
			Debug.LogWarning("FriendsWindowController.Instance == null");
		}
		else
		{
			if (ShowBannerOrLevelup())
			{
				return;
			}
			FriendsController.sharedController.GetFriendsData(true);
			ButtonClickSound.Instance.PlayClick();
			GameObject nickLabelObj = null;
			if (NickLabelStack.sharedStack != null)
			{
				NickLabelStack.sharedStack.gameObject.SetActive(false);
			}
			if (!friendsOnStart)
			{
				StartCoroutine(HideMenuInterfaceCoroutine(nickLabelObj));
			}
			FriendsWindowGUI.Instance.ShowInterface(delegate
			{
				if (persNickLabel != null)
				{
					NickLabelStack.sharedStack.gameObject.SetActive(true);
					if (persNickLabel != null)
					{
						persNickLabel.UpdateNickInLobby();
						persNickLabel.UpdateInfo();
					}
					else if (Application.isEditor)
					{
						Debug.LogWarning("nickLabelController == null");
					}
				}
				rotateCamera.gameObject.SetActive(true);
				if (mainPanel != null)
				{
					mainPanel.transform.root.gameObject.SetActive(true);
				}
			});
			FriendsController.sharedController.DownloadDataAboutPossibleFriends();
		}
	}

	private void HandleFriendsClicked(object sender, EventArgs e)
	{
		if (_shopInstance != null || ShowBannerOrLevelup())
		{
			return;
		}
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Friends", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		Action action = delegate
		{
			if (!ProtocolListGetter.currentVersionIsSupported)
			{
				BannerWindowController bannerWindowController = BannerWindowController.SharedController;
				if (bannerWindowController != null)
				{
					bannerWindowController.ForceShowBanner(BannerWindowType.NewVersion);
				}
			}
			else
			{
				GoFriens();
			}
		};
		action();
	}

	private void HandleNewsButtonClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup() && !FriendsWindowGUI.Instance.InterfaceEnabled)
		{
			newsPanel.SetActive(true);
			mainPanel.SetActive(false);
		}
	}

	private void HandleProfileClicked(object sender, EventArgs e)
	{
		GoToProfile();
	}

	public void GoToProfile()
	{
		if (_shopInstance != null || ShowBannerOrLevelup() || FriendsWindowGUI.Instance.InterfaceEnabled)
		{
			return;
		}
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Profile", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		FlurryPluginWrapper.LogEvent("Profile");
		PlayerPrefs.SetInt(Defs.ProfileEnteredFromMenu, 0);
		if (ProfileController.Instance == null)
		{
			Debug.LogWarning("ProfileController.Instance == null");
		}
		else
		{
			if (ShowBannerOrLevelup())
			{
				return;
			}
			ButtonClickSound.Instance.PlayClick();
			if (NickLabelStack.sharedStack.gameObject != null)
			{
				NickLabelStack.sharedStack.gameObject.SetActive(false);
			}
			if (mainPanel != null)
			{
				mainPanel.transform.root.gameObject.SetActive(false);
			}
			ProfileController.Instance.ShowInterface(delegate
			{
				if (NickLabelStack.sharedStack.gameObject != null)
				{
					NickLabelStack.sharedStack.gameObject.SetActive(true);
					if (persNickLabel != null)
					{
						persNickLabel.UpdateNickInLobby();
						persNickLabel.UpdateInfo();
					}
					else if (Application.isEditor)
					{
						Debug.LogWarning("nickLabelController == null");
					}
				}
				if (mainPanel != null)
				{
					mainPanel.transform.root.gameObject.SetActive(true);
				}
			});
		}
	}

	private void HandleFreeClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Free Coins", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			settingsPanel.SetActive(false);
			freePanel.SetActive(true);
		}
	}

	private void HandleGameServicesClicked(object sender, EventArgs e)
	{
		if (_shopInstance != null || ShowBannerOrLevelup())
		{
			return;
		}
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Game Services", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		SettingsController settingsController = settingsPanel.GetComponent<SettingsController>();
		if (Application.isEditor)
		{
			Debug.Log("[Sign in] pressed.");
			if (settingsController != null)
			{
				settingsController.RefreshSignOutButton();
			}
			return;
		}
		switch (BuildSettings.BuildTargetPlatform)
		{
		case RuntimePlatform.IPhonePlayer:
			FlurryPluginWrapper.LogGamecenter();
			if (Application.isEditor)
			{
			}
			break;
		case RuntimePlatform.Android:
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				if (GpgFacade.Instance.IsAuthenticated())
				{
					Social.ShowAchievementsUI();
					break;
				}
				GpgFacade.Instance.Authenticate(delegate(bool succeeded)
				{
					PlayerPrefs.SetInt("GoogleSignInDenied", Convert.ToInt32(!succeeded));
					if (succeeded)
					{
						Social.ShowAchievementsUI();
					}
					else
					{
						Debug.LogWarning("Authentication failed.");
					}
					if (settingsController != null)
					{
						settingsController.RefreshSignOutButton();
					}
				}, false);
			}
			else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				AGSAchievementsClient.ShowAchievementsOverlay();
			}
			break;
		case RuntimePlatform.PS3:
		case RuntimePlatform.XBOX360:
			break;
		}
	}

	private void HandleResumeFromShop()
	{
		if (!(_shopInstance != null))
		{
			return;
		}
		ShopNGUIController.GuiActive = false;
		_shopInstance.resumeAction = delegate
		{
		};
		_shopInstance = null;
		if (NickLabelStack.sharedStack != null)
		{
			NickLabelStack.sharedStack.gameObject.SetActive(true);
			if (persNickLabel != null)
			{
				persNickLabel.UpdateNickInLobby();
				persNickLabel.UpdateInfo();
			}
			else if (Application.isEditor)
			{
				Debug.LogWarning("nickLabelController == null");
			}
		}
		if (StarterPackController.Get != null && StarterPackController.Get.isEventActive)
		{
			StarterPackController.Get.CheckShowStarterPack();
		}
		StartCoroutine(ShowRanks());
	}

	private IEnumerator ShowRanks()
	{
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = true;
		}
	}

	private static void UnequipSniperRifleAndArmryArmoIfNeeded()
	{
		try
		{
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
			{
				int trainingStep = AnalyticsStuff.TrainingStep;
				if (Storager.getInt("Training.NoviceArmorUsedKey", false) != 1 && trainingStep < 11 && FriendsController.sharedController != null && FriendsController.sharedController.armorName != null && FriendsController.sharedController.armorName != Defs.ArmorNewNoneEqupped)
				{
					ShopNGUIController.UnequipCurrentWearInCategory(ShopNGUIController.CategoryNames.ArmorCategory, false);
				}
				if (trainingStep < 9 && WeaponManager.sharedManager != null && WeaponManager.sharedManager.playerWeapons != null && (from w in WeaponManager.sharedManager.playerWeapons.OfType<Weapon>()
					select w.weaponPrefab.GetComponent<WeaponSounds>()).FirstOrDefault((WeaponSounds ws) => ws.categoryNabor - 1 == 4) != null)
				{
					WeaponManager.sharedManager.SaveWeaponSet(Defs.MultiplayerWSSN, string.Empty, 4);
					WeaponManager.sharedManager.Reset();
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in UnequipSniperRifleAndArmryArmoIfNeeded: " + ex);
		}
	}

	public void HandleShopClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Shop", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			if (PlayerPrefs.GetInt(Defs.ShouldEnableShopSN, 0) != 1)
			{
				ButtonClickSound.Instance.PlayClick();
			}
			FlurryPluginWrapper.LogEvent("Shop");
			_shopInstance = ShopNGUIController.sharedShop;
			if (_shopInstance != null)
			{
				UnequipSniperRifleAndArmryArmoIfNeeded();
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

	private void HandleSettingsClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Settings", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			rotateCamera.OnMainMenuOpenOptions();
			ButtonClickSound.Instance.PlayClick();
			StartCoroutine(OpenSettingPanelWithDelay());
		}
	}

	private IEnumerator OpenSettingPanelWithDelay()
	{
		yield return null;
		settingsPanel.SetActive(true);
		mainPanel.SetActive(false);
	}

	private void HandleYoutubeClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null))
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("YouTube", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL("http://www.youtube.com/channel/UCsClw1gnMrmF6ssIB_166_Q");
		}
	}

	private void HandleEveryPlayClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null))
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Everyplay", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL("https://everyplay.com/pixel-gun-3d--");
		}
	}

	private void HandlePostFacebookClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Post Facebook", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			FlurryPluginWrapper.LogFacebook();
			FlurryPluginWrapper.LogFreeCoinsFacebook();
		}
	}

	private void HandlePostTwittwerClicked(object sender, EventArgs e)
	{
		if (_shopInstance != null || ShowBannerOrLevelup())
		{
			return;
		}
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			FlurryEvents.LogAfterTraining("Post Twitter", TrainingController.TrainingCompletedFlagForLogging.Value);
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		FlurryPluginWrapper.LogTwitter();
		FlurryPluginWrapper.LogFreeCoinsTwitter();
		if (!Application.isEditor)
		{
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			if (TwitterController.Instance != null)
			{
				TwitterController.Instance.PostStatusUpdate(_SocialMessage());
			}
		}
	}

	private void HandleRateAsClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null) && !ShowBannerOrLevelup())
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Rate", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			FlurryPluginWrapper.LogFreeCoinsRateUs();
			string rateUsURL = RateUsURL;
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL(rateUsURL);
		}
	}

	private void HandleBackFromSocialClicked(object sender, EventArgs e)
	{
		_isCancellationRequested = true;
	}

	private void HandleNewsBackButtonClicked(object sender, EventArgs e)
	{
		_isCancellationRequested = true;
	}

	private void HandleTwitterSubscribeClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null))
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Subscribe Twitter", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL("https://twitter.com/PixelGun3D");
		}
	}

	private void HandleFacebookSubscribeClicked(object sender, EventArgs e)
	{
		if (!(_shopInstance != null))
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				FlurryEvents.LogAfterTraining("Subscribe Facebook", TrainingController.TrainingCompletedFlagForLogging.Value);
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL("http://pixelgun3d.com/facebook.html");
		}
	}

	public static void SetInputEnabled(bool enabled)
	{
		if (sharedController != null)
		{
			sharedController.uiCamera.enabled = enabled;
		}
	}

	private void OnEventX3Updated()
	{
		eventX3RemainTime[0].gameObject.SetActive(PromoActionsManager.sharedManager.IsEventX3Active);
	}

	private void OnStarterPackContainerShow(bool enable)
	{
		System.Threading.Tasks.Task<TrafficForwardingInfo> task = FriendsController.sharedController.Map((FriendsController f) => f.GetComponent<TrafficForwardingScript>()).Map((TrafficForwardingScript t) => t.GetTrafficForwardingInfo()).Filter((System.Threading.Tasks.Task<TrafficForwardingInfo> t) => ((System.Threading.Tasks.Task)t).IsCompleted && !((System.Threading.Tasks.Task)t).IsCanceled && !((System.Threading.Tasks.Task)t).IsFaulted);
		bool flag = (task == null || !TrafficForwardingEnabled(task.Result)) && enable;
		starterPackPanel.gameObject.SetActive(flag);
		if (flag)
		{
			buttonBackground.mainTexture = StarterPackController.Get.GetCurrentPackImage();
		}
		_starterPackEnabled = flag;
		starterPackTimer.text = StarterPackController.Get.GetTimeToEndEvent();
	}

	public void OnStarterPackButtonClick()
	{
		if (!(SkinEditorController.sharedController != null))
		{
			BannerWindowController bannerWindowController = BannerWindowController.SharedController;
			if (!(bannerWindowController == null))
			{
				bannerWindowController.ForceShowBanner(BannerWindowType.StarterPack);
			}
		}
	}

	public void HandleTrafficForwardingClicked()
	{
		if (string.IsNullOrEmpty(_trafficForwardingUrl))
		{
			Debug.LogError("HandleTrafficForwardingClicked() called while trafficForwardingUrl is empty.");
			return;
		}
		try
		{
			int @int = PlayerPrefs.GetInt("TrafficForwarded", 0);
			PlayerPrefs.SetInt("TrafficForwarded", @int + 1);
			AnalyticsStuff.LogTrafficForwarding(AnalyticsStuff.LogTrafficForwardingMode.Press);
			FriendsController.LogPromoTrafficForwarding(FriendsController.TypeTrafficForwardingLog.click);
		}
		finally
		{
			TrafficForwardingScript trafficForwardingScript = FriendsController.sharedController.Map((FriendsController fc) => fc.GetComponent<TrafficForwardingScript>());
			if (trafficForwardingScript != null)
			{
				System.Threading.Tasks.Task<TrafficForwardingInfo> trafficForwardingInfo = trafficForwardingScript.GetTrafficForwardingInfo();
				TrafficForwardingInfo e = ((!((System.Threading.Tasks.Task)trafficForwardingInfo).IsCompleted || ((System.Threading.Tasks.Task)trafficForwardingInfo).IsCanceled || ((System.Threading.Tasks.Task)trafficForwardingInfo).IsFaulted) ? TrafficForwardingInfo.DisabledInstance : trafficForwardingInfo.Result);
				RefreshTrafficForwardingButton(this, e);
			}
			else
			{
				RefreshTrafficForwardingButton(this, TrafficForwardingInfo.DisabledInstance);
			}
		}
		Application.OpenURL(_trafficForwardingUrl);
	}

	private bool TrafficForwardingEnabled(TrafficForwardingInfo e)
	{
		return PlayerPrefs.GetInt("TrafficForwarded", 0) < 1 && !SavedShwonLobbyLevelIsLessThanActual() && TrainingController.TrainingCompleted && e.Enabled && ExperienceController.sharedController.currentLevel >= e.MinLevel && ExperienceController.sharedController.currentLevel <= e.MaxLevel;
	}

	private void RefreshTrafficForwardingButton(object sender, TrafficForwardingInfo e)
	{
		if (e == null)
		{
			Debug.LogError("Null TrafficForwardingInfo passed.");
			e = TrafficForwardingInfo.DisabledInstance;
		}
		_trafficForwardingUrl = e.Url;
		bool enabled = false;
		try
		{
			if (!(this == null))
			{
				enabled = TrafficForwardingEnabled(e);
				if (enabled && PlayerPrefs.GetInt(Defs.TrafficForwardingShowAnalyticsSent, 0) == 0)
				{
					AnalyticsStuff.LogTrafficForwarding(AnalyticsStuff.LogTrafficForwardingMode.Show);
					PlayerPrefs.SetInt(Defs.TrafficForwardingShowAnalyticsSent, 1);
					FriendsController.LogPromoTrafficForwarding(FriendsController.TypeTrafficForwardingLog.newView);
				}
				else if (enabled)
				{
					FriendsController.LogPromoTrafficForwarding(FriendsController.TypeTrafficForwardingLog.view);
				}
				trafficForwardActive = enabled;
				ButtonBannerHUD.OnUpdateBanners();
				trafficForwardingButton.Do(delegate(UIButton tf)
				{
					tf.gameObject.SetActive(enabled);
				});
			}
		}
		finally
		{
			OnStarterPackContainerShow(!enabled && StarterPackController.Get.isEventActive);
		}
	}

	public void OnBuySmilesClick()
	{
		if (!(SkinEditorController.sharedController != null))
		{
			BannerWindowController bannerWindowController = BannerWindowController.SharedController;
			if (!(bannerWindowController == null))
			{
				bannerWindowController.ForceShowBanner(BannerWindowType.buySmiles);
			}
		}
	}

	public void OnShowBannerGift()
	{
		BannerWindowController bannerWindowController = BannerWindowController.SharedController;
		if (!(bannerWindowController == null))
		{
			bannerWindowController.ForceShowBanner(BannerWindowType.GiftBonuse);
		}
	}

	public void HandleLeaderboardsClicked()
	{
		StartCoroutine(HandleLeaderboardsClickedCoroutine());
	}

	private IEnumerator ContinueWithCoroutine(System.Threading.Tasks.Task task, Action<System.Threading.Tasks.Task> continuation)
	{
		if (task == null)
		{
			throw new ArgumentNullException("task");
		}
		if (continuation != null)
		{
			while (!task.IsCompleted)
			{
				yield return null;
			}
			continuation(task);
		}
	}

	private IEnumerator HandleLeaderboardsClickedCoroutine()
	{
		if (mainPanel == null || LeaderboardsPanel == null || !mainPanel.activeInHierarchy || LeaderboardsPanel.gameObject.activeInHierarchy)
		{
			yield break;
		}
		LeaderboardScript leaderboardScript = _leaderboardScript.Value;
		if (leaderboardScript == null)
		{
			yield break;
		}
		StartCoroutine(ContinueWithCoroutine(continuation: delegate
		{
			LeaderboardsPanel.gameObject.SetActive(false);
			mainPanel.SetActive(true);
		}, task: leaderboardScript.GetReturnFuture()));
		leaderboardScript.RefreshMyLeaderboardEntries();
		UIButton[] value = _leaderboardsButton.Value;
		foreach (UIButton b in value)
		{
			b.isEnabled = false;
		}
		LeaderboardsPanel.gameObject.SetActive(true);
		LeaderboardsPanel.alpha = float.Epsilon;
		LeaderboardsView view = LeaderboardsPanel.Map((UIPanel p) => p.GetComponent<LeaderboardsView>());
		if (view != null)
		{
			while (!view.Prepared)
			{
				yield return null;
			}
			int stateInt = PlayerPrefs.GetInt("Leaderboards.TabCache", 3);
			LeaderboardsView.State state = ((!Enum.IsDefined(typeof(LeaderboardsView.State), stateInt)) ? LeaderboardsView.State.BestPlayers : ((LeaderboardsView.State)stateInt));
			view.CurrentState = ((state == LeaderboardsView.State.None) ? LeaderboardsView.State.BestPlayers : state);
		}
		mainPanel.SetActive(false);
		LeaderboardsPanel.alpha = 1f;
		UIButton[] value2 = _leaderboardsButton.Value;
		foreach (UIButton b2 in value2)
		{
			b2.isEnabled = true;
		}
	}

	private void OnDayOfValorContainerShow(bool enable)
	{
		dayOfValorContainer.gameObject.SetActive(enable);
		_dayOfValorEnabled = enable;
		dayOfValorTimer.text = PromoActionsManager.sharedManager.GetTimeToEndDaysOfValor();
	}

	public void OnDayOfValorButtonClick()
	{
		BannerWindowController bannerWindowController = BannerWindowController.SharedController;
		if (!(bannerWindowController == null))
		{
			bannerWindowController.ForceShowBanner(BannerWindowType.DaysOfValor);
		}
	}

	public void HandlePremiumClicked()
	{
		ShopNGUIController.ShowPremimAccountExpiredIfPossible(RentExpiredPoint, "NGUI", string.Empty, false);
	}

	private IEnumerator SetActiveSinglePanel(bool isActive)
	{
		InAdventureScreen = isActive;
		mainPanel.SetActive(!isActive);
		singleModePanel.SetActive(isActive);
		FreeAwardShowHandler.CheckShowChest(isActive);
		ExperienceController.SetEnable(isActive && !stubLoading.activeSelf);
		if (isActive)
		{
			survivalButton.GetComponent<UIButton>().isEnabled = false;
			yield return null;
			survivalButton.GetComponent<UIButton>().isEnabled = true;
		}
	}

	public void OnClickSingleModeButton()
	{
		if (!ProtocolListGetter.currentVersionIsSupported)
		{
			BannerWindowController bannerWindowController = BannerWindowController.SharedController;
			if (bannerWindowController != null)
			{
				bannerWindowController.ForceShowBanner(BannerWindowType.NewVersion);
			}
			return;
		}
		Defs.isDaterRegim = false;
		StartCoroutine(SetActiveSinglePanel(true));
		rotateCamera.OnOpenSingleModePanel();
		_parentBankPanel = coinsShopButton.transform.parent;
		coinsShopButton.transform.parent = singleModePanel.transform;
		int num = 0;
		foreach (KeyValuePair<string, Dictionary<string, int>> boxesLevelsAndStar in CampaignProgress.boxesLevelsAndStars)
		{
			foreach (KeyValuePair<string, int> item in boxesLevelsAndStar.Value)
			{
				num += item.Value;
			}
		}
		singleModeStarsProgress.text = string.Format("{0}: {1}", LocalizationStore.Get("Key_1262"), num + "/60");
		singleModeBestScores.text = string.Format("{0} {1}", LocalizationStore.Get("Key_0234"), PlayerPrefs.GetInt(Defs.SurvivalScoreSett, 0).ToString());
	}

	public void OnClickBackSingleModeButton()
	{
		StartCoroutine(ShowRanks());
		StartCoroutine(SetActiveSinglePanel(false));
		rotateCamera.OnCloseSingleModePanel();
		coinsShopButton.transform.parent = _parentBankPanel;
	}

	private void HandleSocialButton(object sender, EventArgs e)
	{
		if (!FriendsWindowGUI.Instance.InterfaceEnabled)
		{
			rotateCamera.OnMainMenuOpenOptions();
			ButtonClickSound.Instance.PlayClick();
			freePanel.SetActive(true);
			mainPanel.SetActive(false);
		}
	}
}
