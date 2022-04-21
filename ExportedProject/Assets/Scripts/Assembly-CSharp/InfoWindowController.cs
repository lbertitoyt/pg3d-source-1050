using System;
using Rilisoft;
using UnityEngine;

public class InfoWindowController : MonoBehaviour
{
	private enum WindowType
	{
		infoBox,
		processDataBox,
		blockClick,
		DialogBox,
		AchievementMessage,
		RestoreInventory,
		DeveloperConsoleMini,
		None
	}

	public UIWidget background;

	[Header("Processing data box")]
	public UIWidget processindDataBoxContainer;

	public UILabel processingDataBoxLabel;

	[Header("Info box")]
	public UIWidget infoBoxContainer;

	public UILabel infoBoxLabel;

	[Header("Dialog box Warning")]
	public UIWidget dialogBoxContainer;

	public UILabel dialogBoxText;

	[Header("Restore Window")]
	public GameObject restoreWindowPanel;

	[Header("Achievement box")]
	public AchieveBox achievementBox;

	public UILabel achievementHeader;

	public UILabel achievementText;

	public AudioClip questCompleteSound;

	public Transform InfoWindowsRoot;

	private GameObject developerConsole;

	private Action DialogBoxOkClick;

	private Action DialogBoxCancelClick;

	private WindowType _typeCurrentWindow = WindowType.None;

	private static InfoWindowController _instance;

	private IDisposable _backSubscription;

	private Action _unsubscribe;

	public static InfoWindowController Instance
	{
		get
		{
			if (_instance == null)
			{
				UnityEngine.Object original = Resources.Load("InfoWindows");
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, Vector3.down * 567f, Quaternion.identity);
				_instance = gameObject.GetComponent<InfoWindowController>();
				return _instance;
			}
			return _instance;
		}
	}

	public static bool IsActive
	{
		get
		{
			return _instance != null && _instance.infoBoxContainer != null && _instance.infoBoxContainer.gameObject != null && _instance.infoBoxContainer.gameObject.activeInHierarchy;
		}
	}

	private void Start()
	{
		processingDataBoxLabel.text = LocalizationStore.Key_0348;
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void OnDestroy()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
		if (_unsubscribe != null)
		{
			_unsubscribe();
		}
	}

	private void HandleLocalizationChanged()
	{
		processingDataBoxLabel.text = LocalizationStore.Key_0348;
	}

	private void ActivateDevConsole()
	{
	}

	private void ActivateInfoBox(string text)
	{
		if (Instance._backSubscription != null)
		{
			Instance._backSubscription.Dispose();
		}
		Instance._backSubscription = BackSystem.Instance.Register(Instance.HandleEscape, "Info Window");
		infoBoxLabel.text = text;
		infoBoxContainer.gameObject.SetActive(true);
		background.gameObject.SetActive(true);
	}

	public void OnClickOkDialog()
	{
		if (DialogBoxOkClick != null)
		{
			DialogBoxOkClick();
		}
		Hide();
	}

	public void OnClickCancelDialog()
	{
		if (DialogBoxCancelClick != null)
		{
			DialogBoxCancelClick();
		}
		Hide();
	}

	private void ActivateDialogBox(string text, Action onOkClick, Action onCancelClick)
	{
		dialogBoxText.text = text;
		dialogBoxContainer.gameObject.SetActive(true);
		SetActiveBackground(true);
		DialogBoxOkClick = onOkClick;
		DialogBoxCancelClick = onCancelClick;
		if (Instance._backSubscription != null)
		{
			Instance._backSubscription.Dispose();
		}
		Instance._backSubscription = BackSystem.Instance.Register(Instance.HandleEscape, "Dialog Box");
	}

	public void ActivateRestorePanel(Action okCallback)
	{
		if (!(restoreWindowPanel == null))
		{
			restoreWindowPanel.SetActive(true);
			SetActiveBackground(false);
			DialogBoxOkClick = okCallback;
			if (Instance._backSubscription != null)
			{
				Instance._backSubscription.Dispose();
			}
			Instance._backSubscription = BackSystem.Instance.Register(Instance.BackButtonFromRestoreClick, "Restore Panel");
		}
	}

	private void BackButtonFromRestoreClick()
	{
	}

	private void ActivateAchievementBox(string header, string text)
	{
		if (achievementBox.isOpened)
		{
			if (Application.isEditor)
			{
				Debug.LogWarningFormat("Skipping activating achievement box: {0}", text);
			}
			return;
		}
		achievementText.text = text;
		achievementBox.ShowBox();
		Invoke("DeactivateAchievementBox", 3f);
		if (Defs.isSoundFX)
		{
			NGUITools.PlaySound(questCompleteSound);
		}
	}

	private void DeactivateAchievementBox()
	{
		achievementBox.HideBox();
	}

	private void DeactivateRestorePanel()
	{
		DialogBoxOkClick = null;
		DialogBoxCancelClick = null;
		if (restoreWindowPanel != null)
		{
			restoreWindowPanel.SetActive(false);
		}
	}

	private void DeactivateDialogBox()
	{
		DialogBoxOkClick = null;
		DialogBoxCancelClick = null;
		dialogBoxContainer.gameObject.SetActive(false);
	}

	private void DeactivateInfoBox()
	{
		background.gameObject.SetActive(false);
		infoBoxContainer.gameObject.SetActive(false);
	}

	private void SetActiveProcessDataBox(bool enable)
	{
		processindDataBoxContainer.gameObject.SetActive(enable);
	}

	private void SetActiveBackground(bool enable)
	{
		background.gameObject.SetActive(enable);
	}

	private void Initialize(WindowType typeWindow)
	{
		_typeCurrentWindow = typeWindow;
		base.gameObject.SetActive(true);
	}

	private void HideInfoAndProcessingBox()
	{
		if (_unsubscribe != null)
		{
			_unsubscribe();
		}
		if (_typeCurrentWindow != WindowType.None && (_typeCurrentWindow == WindowType.infoBox || _typeCurrentWindow == WindowType.processDataBox))
		{
			if (_typeCurrentWindow == WindowType.infoBox)
			{
				DeactivateInfoBox();
			}
			else if (_typeCurrentWindow == WindowType.processDataBox)
			{
				SetActiveProcessDataBox(false);
			}
			SetActiveBackground(true);
			_typeCurrentWindow = WindowType.None;
			base.gameObject.SetActive(false);
		}
	}

	private void Hide()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
		if (_unsubscribe != null)
		{
			_unsubscribe();
		}
		if (_typeCurrentWindow != WindowType.None)
		{
			if (_typeCurrentWindow == WindowType.infoBox)
			{
				DeactivateInfoBox();
			}
			else if (_typeCurrentWindow == WindowType.processDataBox)
			{
				SetActiveProcessDataBox(false);
			}
			else if (_typeCurrentWindow == WindowType.DialogBox)
			{
				DeactivateDialogBox();
			}
			else if (_typeCurrentWindow == WindowType.RestoreInventory)
			{
				DeactivateRestorePanel();
			}
			SetActiveBackground(true);
			_typeCurrentWindow = WindowType.None;
			base.gameObject.SetActive(false);
		}
	}

	public static void ShowInfoBox(string text)
	{
		Instance.Initialize(WindowType.infoBox);
		Instance.ActivateInfoBox(text);
	}

	public static void ShowDevConsole()
	{
		Instance.Initialize(WindowType.DeveloperConsoleMini);
		Instance.ActivateDevConsole();
	}

	private void HandleEscape()
	{
		OnClickCancelDialog();
	}

	public static void ShowProcessingDataBox()
	{
		Instance.Initialize(WindowType.processDataBox);
		Instance.SetActiveProcessDataBox(true);
		Instance.SetActiveBackground(false);
	}

	public static void BlockAllClick()
	{
		Instance.Initialize(WindowType.blockClick);
		Instance.SetActiveBackground(true);
	}

	public static void ShowDialogBox(string text, Action callbackOkButton, Action callbackCancelButton = null)
	{
		Instance.Initialize(WindowType.DialogBox);
		Instance.ActivateDialogBox(text, callbackOkButton, callbackCancelButton);
	}

	public static void ShowRestorePanel(Action okCallback)
	{
		Instance.Initialize(WindowType.RestoreInventory);
		Instance.ActivateRestorePanel(okCallback);
	}

	public static void ShowAchievementBox(string header, string text)
	{
		Instance.Initialize(WindowType.AchievementMessage);
		Instance.ActivateAchievementBox(header, text);
	}

	public static void HideCurrentWindow()
	{
		Instance.Hide();
	}

	public static void HideProcessing(float time)
	{
		Instance.Invoke("HideInfoAndProcessingBox", time);
	}

	public static void HideProcessing()
	{
		Instance.HideInfoAndProcessingBox();
	}

	public void OnClickExitButton()
	{
		if (_typeCurrentWindow != WindowType.blockClick)
		{
			Hide();
		}
	}

	public static void CheckShowRequestServerInfoBox(bool isComplete, bool isRequestExist)
	{
		if (!isComplete)
		{
			ShowInfoBox(LocalizationStore.Get("Key_1528"));
		}
		else if (isRequestExist)
		{
			ShowInfoBox(LocalizationStore.Get("Key_1563"));
		}
	}
}
