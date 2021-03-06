using System;
using System.Collections;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class SettingsController : MonoBehaviour
{
	public const int SensitivityLowerBound = 6;

	public const int SensitivityUpperBound = 19;

	public MainMenuHeroCamera rotateCamera;

	public UIButton backButton;

	public UIButton controlsButton;

	public UIButton syncButton;

	public UIButton signOutButton;

	public GameObject controlsSettings;

	public GameObject tapPanel;

	public GameObject swipePanel;

	public GameObject mainPanel;

	public UISlider sensitivitySlider;

	public UILabel versionLabel;

	public SettingsToggleButtons chatToggleButtons;

	public SettingsToggleButtons musicToggleButtons;

	public SettingsToggleButtons soundToggleButtons;

	public SettingsToggleButtons invertCameraToggleButtons;

	public SettingsToggleButtons recToggleButtons;

	public SettingsToggleButtons pressureToucheToggleButtons;

	public SettingsToggleButtons hideJumpAndShootButtons;

	public SettingsToggleButtons leftHandedToggleButtons;

	public SettingsToggleButtons switchingWeaponsToggleButtons;

	public Texture googlePlayServicesTexture;

	private IDisposable _backSubscription;

	private bool _backRequested;

	private float _cachedSensitivity;

	public static event Action ControlsClicked;

	private IEnumerator SynchronizeAmazonCoroutine(UIButton syncButton)
	{
		if (syncButton != null)
		{
			syncButton.isEnabled = false;
		}
		try
		{
			if (!GameCircleSocial.Instance.localUser.authenticated)
			{
				Debug.LogFormat("[Rilisoft] Sign in to GameCircle ({0})", GetType().Name);
				AGSClient.ShowSignInPage();
			}
			Scene activeScene = SceneManager.GetActiveScene();
			float endTime = Time.realtimeSinceStartup + 60f;
			while (!GameCircleSocial.Instance.localUser.authenticated && Time.realtimeSinceStartup < endTime)
			{
				yield return null;
			}
			if (!GameCircleSocial.Instance.localUser.authenticated || !activeScene.IsValid() || !activeScene.isLoaded)
			{
				Debug.LogWarningFormat("Stop syncing attempt. Scene {0} valid: {1}, loaded: {2}. User authenticated: {3}", activeScene.name, activeScene.IsValid(), activeScene.isLoaded, GameCircleSocial.Instance.localUser.authenticated);
				yield break;
			}
			PurchasesSynchronizer.Instance.SynchronizeAmazonPurchases();
			if (PurchasesSynchronizer.Instance.HasItemsToBeSaved)
			{
				int maxLevel = MainMenuController.FindMaxLevel(PurchasesSynchronizer.Instance.ItemsToBeSaved);
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("[Rilisoft] Incoming level: {0}", maxLevel);
				}
				if (maxLevel > 0)
				{
					if (ShopNGUIController.GuiActive)
					{
						Debug.LogWarning("Skipping saving to storager while in Shop.");
						yield break;
					}
					if (!StringComparer.Ordinal.Equals(SceneManager.GetActiveScene().name, Defs.MainMenuScene))
					{
						Debug.LogWarning("Skipping saving to storager while not Main Menu.");
						yield break;
					}
					TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();
					InfoWindowController.ShowRestorePanel(delegate
					{
						CoroutineRunner.Instance.StartCoroutine(MainMenuController.SaveItemsToStorager(delegate
						{
							Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.InnerCallback >: {0:F3}", Time.realtimeSinceStartup);
							PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
							if (WeaponManager.sharedManager != null)
							{
								StartCoroutine(WeaponManager.sharedManager.ResetCoroutine());
							}
							Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.InnerCallback <: {0:F3}", Time.realtimeSinceStartup);
							promise.TrySetResult(true);
						}));
					});
					System.Threading.Tasks.Task<bool> future = promise.Task;
					while (!((System.Threading.Tasks.Task)future).IsCompleted)
					{
						yield return null;
					}
					if (WeaponManager.sharedManager != null)
					{
						WeaponManager.sharedManager.Reset();
					}
					ProgressSynchronizer.Instance.SynchronizeAmazonProgress();
				}
			}
			StarterPackController.Get.RestoreStarterPackForAmazon();
			SetSyncLabelText();
		}
		finally
		{
			if (syncButton != null)
			{
				syncButton.isEnabled = true;
			}
		}
	}

	public static void SwitchChatSetting(bool on, Action additional = null)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Chat] button clicked: " + on);
		}
		bool isChatOn = Defs.IsChatOn;
		if (isChatOn != on)
		{
			Defs.IsChatOn = on;
			if (additional != null)
			{
				additional();
			}
		}
	}

	public static void ChangeLeftHandedRightHanded(bool isChecked, Action handler = null)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Left Handed] button clicked: " + isChecked);
		}
		if (GlobalGameController.LeftHanded == isChecked)
		{
			return;
		}
		GlobalGameController.LeftHanded = isChecked;
		PlayerPrefs.SetInt(Defs.LeftHandedSN, isChecked ? 1 : 0);
		PlayerPrefs.Save();
		if (handler != null)
		{
			handler();
		}
		if (SettingsController.ControlsClicked != null)
		{
			SettingsController.ControlsClicked();
		}
		if (!isChecked)
		{
			FlurryPluginWrapper.LogEvent("Left-handed Layout Enabled");
			if (Debug.isDebugBuild)
			{
				Debug.Log("Left-handed Layout Enabled");
			}
		}
	}

	public static void ChangeSwitchingWeaponHanded(bool isChecked, Action handler = null)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Switching Weapon button clicked: " + isChecked);
		}
		if (GlobalGameController.switchingWeaponSwipe == isChecked)
		{
			GlobalGameController.switchingWeaponSwipe = !isChecked;
			PlayerPrefs.SetInt(Defs.SwitchingWeaponsSwipeRegimSN, GlobalGameController.switchingWeaponSwipe ? 1 : 0);
			PlayerPrefs.Save();
			if (handler != null)
			{
				handler();
			}
		}
	}

	private void SetSyncLabelText()
	{
		UILabel uILabel = null;
		Transform transform = syncButton.transform.Find("Label");
		if (transform != null)
		{
			uILabel = transform.gameObject.GetComponent<UILabel>();
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (uILabel != null)
			{
				uILabel.text = LocalizationStore.Get("Key_0080");
			}
		}
		else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && uILabel != null)
		{
			uILabel.text = LocalizationStore.Get("Key_0935");
		}
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(delegate
		{
			HandleBackFromSettings(this, EventArgs.Empty);
		}, "Settings");
		RefreshSignOutButton();
	}

	internal void RefreshSignOutButton()
	{
		if (signOutButton != null)
		{
			if (Application.isEditor)
			{
				signOutButton.gameObject.SetActive(true);
			}
			else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				signOutButton.gameObject.SetActive(GpgFacade.Instance.IsAuthenticated());
			}
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

	private void Start()
	{
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
		string text = typeof(SettingsController).Assembly.GetName().Version.ToString();
		if (versionLabel != null)
		{
			versionLabel.text = text;
		}
		else
		{
			Debug.LogWarning("versionLabel == null");
		}
		if (backButton != null)
		{
			ButtonHandler component = backButton.GetComponent<ButtonHandler>();
			component.Clicked += HandleBackFromSettings;
		}
		if (controlsButton != null)
		{
			ButtonHandler component2 = controlsButton.GetComponent<ButtonHandler>();
			component2.Clicked += HandleControlsClicked;
		}
		if (syncButton != null)
		{
			ButtonHandler component3 = syncButton.GetComponent<ButtonHandler>();
			SetSyncLabelText();
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				syncButton.gameObject.SetActive(true);
				component3.Clicked += HandleRestoreClicked;
			}
			else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				syncButton.gameObject.SetActive(true);
				component3.Clicked += HandleSyncClicked;
			}
			else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				bool active = false;
				syncButton.gameObject.SetActive(active);
				component3.Clicked += HandleSyncClicked;
			}
		}
		if (sensitivitySlider != null)
		{
			float sensitivity = Defs.Sensitivity;
			float num = Mathf.Clamp(sensitivity, 6f, 19f);
			float num2 = num - 6f;
			sensitivitySlider.value = num2 / 13f;
			_cachedSensitivity = num;
		}
		else
		{
			Debug.LogWarning("sensitivitySlider == null");
		}
		musicToggleButtons.IsChecked = Defs.isSoundMusic;
		musicToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("[Music] button clicked: " + e.IsChecked);
			}
			GameObject gameObject = GameObject.FindGameObjectWithTag("MenuBackgroundMusic");
			MenuBackgroundMusic menuBackgroundMusic = ((!(gameObject != null)) ? null : gameObject.GetComponent<MenuBackgroundMusic>());
			if (Defs.isSoundMusic != e.IsChecked)
			{
				Defs.isSoundMusic = e.IsChecked;
				PlayerPrefsX.SetBool(PlayerPrefsX.SoundMusicSetting, Defs.isSoundMusic);
				PlayerPrefs.Save();
				if (menuBackgroundMusic != null)
				{
					if (e.IsChecked)
					{
						menuBackgroundMusic.Play();
					}
					else
					{
						menuBackgroundMusic.Stop();
					}
				}
				else
				{
					Debug.LogWarning("menuBackgroundMusic == null");
				}
			}
		};
		soundToggleButtons.IsChecked = Defs.isSoundFX;
		soundToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("[Sound] button clicked: " + e.IsChecked);
			}
			if (Defs.isSoundFX != e.IsChecked)
			{
				Defs.isSoundFX = e.IsChecked;
				PlayerPrefsX.SetBool(PlayerPrefsX.SoundFXSetting, Defs.isSoundFX);
				PlayerPrefs.Save();
			}
		};
		chatToggleButtons.IsChecked = Defs.IsChatOn;
		chatToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			SwitchChatSetting(e.IsChecked);
		};
		invertCameraToggleButtons.IsChecked = PlayerPrefs.GetInt(Defs.InvertCamSN, 0) == 1;
		invertCameraToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("[Invert Camera] button clicked: " + e.IsChecked);
			}
			bool flag = PlayerPrefs.GetInt(Defs.InvertCamSN, 0) == 1;
			if (flag != e.IsChecked)
			{
				PlayerPrefs.SetInt(Defs.InvertCamSN, Convert.ToInt32(e.IsChecked));
				PlayerPrefs.Save();
			}
		};
		if (leftHandedToggleButtons != null)
		{
			leftHandedToggleButtons.IsChecked = GlobalGameController.LeftHanded;
			leftHandedToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				ChangeLeftHandedRightHanded(e.IsChecked);
			};
		}
		if (switchingWeaponsToggleButtons != null)
		{
			switchingWeaponsToggleButtons.IsChecked = !GlobalGameController.switchingWeaponSwipe;
			switchingWeaponsToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				ChangeSwitchingWeaponHanded(e.IsChecked);
			};
		}
		if (Input.touchPressureSupported || Application.isEditor)
		{
			pressureToucheToggleButtons.gameObject.SetActive(true);
			recToggleButtons.gameObject.SetActive(false);
			pressureToucheToggleButtons.IsChecked = Defs.isUse3DTouch;
			pressureToucheToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				if (Application.isEditor)
				{
					Debug.Log("3D touche button clicked: " + e.IsChecked);
				}
				Defs.isUse3DTouch = e.IsChecked;
				hideJumpAndShootButtons.gameObject.SetActive(Defs.isUse3DTouch);
			};
		}
		else
		{
			pressureToucheToggleButtons.gameObject.SetActive(false);
			recToggleButtons.gameObject.SetActive(PauseNGUIController.RecButtonsAvailable());
			recToggleButtons.IsChecked = GlobalGameController.ShowRec;
			recToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				if (Application.isEditor)
				{
					Debug.Log("[Rec. Buttons] button clicked: " + e.IsChecked);
				}
				if (GlobalGameController.ShowRec != e.IsChecked)
				{
					GlobalGameController.ShowRec = e.IsChecked;
					PlayerPrefs.SetInt(Defs.ShowRecSN, e.IsChecked ? 1 : 0);
					PlayerPrefs.Save();
				}
			};
		}
		if (Input.touchPressureSupported || Application.isEditor)
		{
			hideJumpAndShootButtons.gameObject.SetActive(Defs.isUse3DTouch);
			hideJumpAndShootButtons.IsChecked = Defs.isJumpAndShootButtonOn;
			hideJumpAndShootButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				if (Application.isEditor)
				{
					Debug.Log("3D touche button clicked: " + e.IsChecked);
				}
				Defs.isJumpAndShootButtonOn = e.IsChecked;
			};
		}
		else
		{
			hideJumpAndShootButtons.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (_backRequested)
		{
			_backRequested = false;
			mainPanel.SetActive(true);
			base.gameObject.SetActive(false);
			rotateCamera.OnMainMenuCloseOptions();
			return;
		}
		float num = sensitivitySlider.value * 13f;
		float num2 = Mathf.Clamp(num + 6f, 6f, 19f);
		if (_cachedSensitivity != num2)
		{
			if (Application.isEditor)
			{
				Debug.Log("New sensitivity: " + num2);
			}
			Defs.Sensitivity = num2;
			_cachedSensitivity = num2;
		}
	}

	private void HandleBackFromSettings(object sender, EventArgs e)
	{
		_backRequested = true;
	}

	private void HandleControlsClicked(object sender, EventArgs e)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Controls] button clicked.");
		}
		controlsSettings.SetActive(true);
		tapPanel.SetActive(!GlobalGameController.switchingWeaponSwipe);
		swipePanel.SetActive(false);
		swipePanel.transform.parent.gameObject.SetActive(!GlobalGameController.switchingWeaponSwipe);
		base.gameObject.SetActive(false);
		if (SettingsController.ControlsClicked != null)
		{
			SettingsController.ControlsClicked();
		}
	}

	private void HandleRestoreClicked(object sender, EventArgs e)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Restore] button clicked.");
		}
		WeaponManager.RefreshExpControllers();
	}

	private void HandleSyncClicked(object sender, EventArgs e)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Sync] button clicked.");
			RefreshSignOutButton();
		}
		TrophiesSynchronizer.Instance.Sync();
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				UIButton uIButton = (sender as MonoBehaviour).Map((MonoBehaviour o) => o.GetComponent<UIButton>());
				CoroutineRunner.Instance.StartCoroutine(SynchronizeAmazonCoroutine(uIButton));
			}
			else
			{
				if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return;
				}
				UIButton syncButton = (sender as MonoBehaviour).Map((MonoBehaviour o) => o.GetComponent<UIButton>());
				if (syncButton != null)
				{
					syncButton.isEnabled = false;
				}
				Action afterAuth = delegate
				{
					Action<bool> callback = delegate(bool succeeded)
					{
						try
						{
							Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.Callback({0}) >: {1:F3}", succeeded, Time.realtimeSinceStartup);
							if (succeeded && WeaponManager.sharedManager != null)
							{
								WeaponManager.sharedManager.Reset();
							}
							StoreKitEventListener.purchaseInProcess = false;
							Debug.LogFormat("[Rilisoft] PurchasesSynchronizer.HasItemsToBeSaved: {0}", PurchasesSynchronizer.Instance.HasItemsToBeSaved);
							if (PurchasesSynchronizer.Instance.HasItemsToBeSaved)
							{
								int num = MainMenuController.FindMaxLevel(PurchasesSynchronizer.Instance.ItemsToBeSaved);
								if (Defs.IsDeveloperBuild)
								{
									Debug.LogFormat("[Rilisoft] Incoming level: {0}", num);
								}
								if (num > 0)
								{
									if (ShopNGUIController.GuiActive)
									{
										Debug.LogWarning("Skipping saving to storager while in Shop.");
										return;
									}
									if (!StringComparer.Ordinal.Equals(SceneManager.GetActiveScene().name, Defs.MainMenuScene))
									{
										Debug.LogWarning("Skipping saving to storager while not Main Menu.");
										return;
									}
									TrainingController.OnGetProgress();
									if (HintController.instance != null)
									{
										HintController.instance.ShowNext();
									}
									string text = LocalizationStore.Get("Key_1977");
									Debug.LogFormat("[Rilisoft] > StartCoroutine(SaveItemsToStorager): {1} {0:F3}", Time.realtimeSinceStartup, text);
									InfoWindowController.ShowRestorePanel(delegate
									{
										CoroutineRunner.Instance.StartCoroutine(MainMenuController.SaveItemsToStorager(delegate
										{
											Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.InnerCallback >: {0:F3}", Time.realtimeSinceStartup);
											PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
											if (WeaponManager.sharedManager != null)
											{
												StartCoroutine(WeaponManager.sharedManager.ResetCoroutine());
											}
											Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.InnerCallback <: {0:F3}", Time.realtimeSinceStartup);
										}));
									});
									Debug.LogFormat("[Rilisoft] < StartCoroutine(SaveItemsToStorager): {1} {0:F3}", Time.realtimeSinceStartup, text);
								}
							}
							PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
							Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.Callback({0}) <: {1:F3}", succeeded, Time.realtimeSinceStartup);
						}
						finally
						{
							if (syncButton != null)
							{
								syncButton.isEnabled = true;
							}
						}
					};
					if (Application.isEditor)
					{
						Debug.Log("Simulating sync...");
					}
					else
					{
						ProgressSynchronizer.Instance.SynchronizeIfAuthenticated(delegate
						{
						});
						GoogleIAB.queryInventory(StoreKitEventListener.starterPackIds);
					}
					RefreshSignOutButton();
					SetSyncLabelText();
				};
				StoreKitEventListener.purchaseInProcess = true;
				StartCoroutine(RestoreProgressIndicator(5f));
				if (GpgFacade.Instance.IsAuthenticated())
				{
					string message = string.Format("Already authenticated: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
					Debug.Log(message);
					afterAuth();
					return;
				}
				if (!Application.isEditor)
				{
					try
					{
						GpgFacade.Instance.Authenticate(delegate(bool succeeded)
						{
							PlayerPrefs.SetInt("GoogleSignInDenied", Convert.ToInt32(!succeeded));
							if (succeeded)
							{
								Debug.LogFormat("Authentication succeeded: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
								afterAuth();
							}
							else
							{
								Debug.LogWarning("Authentication failed.");
								StoreKitEventListener.purchaseInProcess = false;
								if (syncButton != null)
								{
									syncButton.isEnabled = true;
								}
							}
						}, false);
						return;
					}
					catch (InvalidOperationException exception)
					{
						Debug.LogWarning("SettingsController: Exception occured while authenticating with Google Play Games. See next exception message for details.");
						Debug.LogException(exception);
						if (syncButton != null)
						{
							syncButton.isEnabled = true;
						}
						return;
					}
				}
				afterAuth();
			}
		}
		else if (BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64)
		{
		}
	}

	private IEnumerator RestoreProgressIndicator(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		StoreKitEventListener.purchaseInProcess = false;
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		SetSyncLabelText();
	}
}
