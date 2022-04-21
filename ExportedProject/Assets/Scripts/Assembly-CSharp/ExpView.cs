using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public sealed class ExpView : MonoBehaviour
{
	public GameObject rankIndicatorContainer;

	public UIRoot interfaceHolder;

	public Camera experienceCamera;

	public UISprite experienceFrame;

	public UISprite experienceFrameWithFooter;

	public UILabel levelLabel;

	public UILabel experienceLabel;

	public UISlider currentProgress;

	public UISlider oldProgress;

	public UISprite rankSprite;

	public LevelUpWithOffers levelUpPanel;

	public LevelUpWithOffers levelUpPanelTier;

	public GameObject[] arrows;

	public GameObject[] shineNodes;

	public GameObject objHUD;

	private int _currentBgArrowPrefabIndex = -1;

	private GameObject[] _bgArrowRows;

	private UIButton _profileButton;

	public bool FrameFooterEnabled
	{
		get
		{
			return experienceFrameWithFooter != null && experienceFrameWithFooter.enabled;
		}
		private set
		{
		}
	}

	public bool VisibleHUD
	{
		get
		{
			return objHUD.activeSelf;
		}
		set
		{
			objHUD.SetActive(value);
		}
	}

	public string LevelLabel
	{
		get
		{
			return (!(levelLabel != null)) ? string.Empty : levelLabel.text;
		}
		set
		{
			if (levelLabel != null)
			{
				levelLabel.text = value ?? string.Empty;
			}
		}
	}

	public string ExperienceLabel
	{
		get
		{
			return (!(experienceLabel != null)) ? string.Empty : experienceLabel.text;
		}
		set
		{
			if (experienceLabel != null)
			{
				experienceLabel.text = value ?? string.Empty;
			}
		}
	}

	public float CurrentProgress
	{
		get
		{
			return (!(currentProgress != null)) ? 0f : currentProgress.value;
		}
		set
		{
			if (currentProgress != null)
			{
				currentProgress.value = value;
			}
		}
	}

	public float OldProgress
	{
		get
		{
			return (!(oldProgress != null)) ? 0f : oldProgress.value;
		}
		set
		{
			if (oldProgress != null)
			{
				oldProgress.value = value;
			}
		}
	}

	public int RankSprite
	{
		get
		{
			if (rankSprite == null)
			{
				return 1;
			}
			string s = rankSprite.spriteName.Replace("Rank_", string.Empty);
			int result = 0;
			return (!int.TryParse(s, out result)) ? 1 : result;
		}
		set
		{
			if (rankSprite != null)
			{
				string spriteName = string.Format("Rank_{0}", value);
				rankSprite.spriteName = spriteName;
			}
		}
	}

	public void ShowLevelUpPanel(LevelUpWithOffers levelUpPanel, List<string> newItems, int currentRank, int coinsReward, int gemsReward)
	{
		levelUpPanel.SetCurrentRank(currentRank.ToString());
		levelUpPanel.SetRewardPrice("+" + coinsReward + "\n" + LocalizationStore.Get("Key_0275"));
		levelUpPanel.SetGemsRewardPrice("+" + gemsReward + "\n" + LocalizationStore.Get("Key_0951"));
		levelUpPanel.SetAddHealthCount(string.Format(LocalizationStore.Get("Key_1856"), ExperienceController.sharedController.AddHealthOnCurLevel.ToString()));
		levelUpPanel.SetItems(newItems);
		ExpController.ShowTierPanel(levelUpPanel.gameObject);
	}

	public void StopAnimation()
	{
		if (currentProgress != null && currentProgress.gameObject.activeInHierarchy)
		{
			currentProgress.StopAllCoroutines();
		}
		if (oldProgress != null && oldProgress.gameObject.activeInHierarchy)
		{
			oldProgress.StopAllCoroutines();
		}
	}

	public IDisposable StartBlinkingWithNewProgress()
	{
		if (currentProgress == null || !currentProgress.gameObject.activeInHierarchy)
		{
			Debug.LogWarning("(currentProgress == null || !currentProgress.gameObject.activeInHierarchy)");
			return new ActionDisposable(delegate
			{
			});
		}
		currentProgress.StopAllCoroutines();
		IEnumerator c = StartBlinkingCoroutine();
		currentProgress.StartCoroutine(c);
		return new ActionDisposable(delegate
		{
			currentProgress.StopCoroutine(c);
			if (currentProgress.foregroundWidget != null)
			{
				currentProgress.foregroundWidget.enabled = true;
			}
		});
	}

	public void WaitAndUpdateOldProgress(AudioClip sound)
	{
		if (!(oldProgress == null) && oldProgress.gameObject.activeInHierarchy)
		{
			oldProgress.StopAllCoroutines();
			oldProgress.StartCoroutine(WaitAndUpdateCoroutine(sound));
		}
	}

	private void OnEnable()
	{
		if (_profileButton == null)
		{
			IEnumerable<UIButton> source = from b in UnityEngine.Object.FindObjectsOfType<UIButton>()
				where b.gameObject.name.Equals("Profile")
				select b;
			_profileButton = source.FirstOrDefault();
		}
	}

	private void OnDisable()
	{
		if (currentProgress != null && currentProgress.gameObject.activeInHierarchy)
		{
			currentProgress.StopAllCoroutines();
		}
	}

	internal void Refresh()
	{
		RefreshCore();
	}

	private void Start()
	{
		StartCoroutine(LoopBackgroundAnimation());
	}

	private IEnumerator LoopBackgroundAnimation()
	{
		GameObject arrowRowPrefab = arrows[0];
		_bgArrowRows = new GameObject[8];
		for (int l = 0; l < _bgArrowRows.Length; l++)
		{
			GameObject newArrowRow = UnityEngine.Object.Instantiate(arrowRowPrefab);
			newArrowRow.transform.parent = arrowRowPrefab.transform.parent;
			_bgArrowRows[l] = newArrowRow;
			yield return null;
		}
		for (int k = 0; k < arrows.Length; k++)
		{
			arrows[k].SetActive(false);
		}
		_currentBgArrowPrefabIndex = -1;
		while (true)
		{
			if (interfaceHolder != null && interfaceHolder.gameObject.activeInHierarchy)
			{
				for (int j = 0; j < shineNodes.Length; j++)
				{
					GameObject shine = shineNodes[j];
					if (shine != null && shine.activeInHierarchy)
					{
						shine.transform.Rotate(Vector3.forward, Time.deltaTime * 10f, Space.Self);
						if (j != _currentBgArrowPrefabIndex)
						{
							_currentBgArrowPrefabIndex = j;
							ResetBackgroundArrows(arrows[j].transform);
						}
					}
				}
				for (int i = 0; i < _bgArrowRows.Length; i++)
				{
					if (!(_bgArrowRows[i] == null))
					{
						Transform t = _bgArrowRows[i].transform;
						float newLocalY = t.localPosition.y + Time.deltaTime * 60f;
						if (newLocalY > 474f)
						{
							newLocalY -= 880f;
						}
						t.localPosition = new Vector3(t.localPosition.x, newLocalY, t.localPosition.z);
					}
				}
			}
			yield return null;
		}
	}

	private void ResetBackgroundArrows(Transform target)
	{
		for (int i = 0; i < _bgArrowRows.Length; i++)
		{
			Transform transform = _bgArrowRows[i].transform;
			transform.parent = target.parent;
			transform.localScale = Vector3.one;
			transform.localPosition = new Vector3(target.localPosition.x + ((i % 2 != 1) ? 0f : 90f), target.localPosition.y - 110f * (float)i, target.localPosition.z);
			transform.localRotation = target.localRotation;
		}
	}

	private void Update()
	{
		RefreshCore();
	}

	private void RefreshCore()
	{
		if (_profileButton == null && Defs.MainMenuScene.Equals(Application.loadedLevelName))
		{
			_profileButton = UnityEngine.Object.FindObjectsOfType<UIButton>().FirstOrDefault((UIButton b) => b.gameObject.name.Equals("Profile"));
		}
		if (_profileButton == null)
		{
			FrameFooterEnabled = false;
		}
		else if (ShopNGUIController.GuiActive)
		{
			FrameFooterEnabled = false;
		}
		else if (ProfileController.Instance != null && ProfileController.Instance.InterfaceEnabled)
		{
			FrameFooterEnabled = false;
		}
		else if (FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled)
		{
			FrameFooterEnabled = false;
		}
		else
		{
			FrameFooterEnabled = _profileButton.gameObject.activeInHierarchy;
		}
	}

	private IEnumerator StartBlinkingCoroutine()
	{
		for (int i = 0; i != 4; i++)
		{
			currentProgress.foregroundWidget.enabled = false;
			yield return new WaitForSeconds(0.15f);
			currentProgress.foregroundWidget.enabled = true;
			yield return new WaitForSeconds(0.15f);
		}
	}

	private IEnumerator WaitAndUpdateCoroutine(AudioClip sound)
	{
		yield return new WaitForSeconds(1.2f);
		if (currentProgress != null)
		{
			oldProgress.value = currentProgress.value;
		}
		if (Defs.isSoundFX)
		{
			NGUITools.PlaySound(sound);
		}
	}
}
