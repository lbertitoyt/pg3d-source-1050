using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class FriendProfileView : MonoBehaviour
{
	private const string DefaultStatisticString = "-";

	public Transform pers;

	public GameObject[] bootsPoint;

	public GameObject capePoint;

	public GameObject armorPoint;

	public GameObject hatPoint;

	public GameObject maskPoint;

	public GameObject characterModel;

	public GameObject armorLeftPl;

	public GameObject armorRightPl;

	public UISprite rankSprite;

	public UILabel friendCountLabel;

	public UILabel friendLocationLabel;

	public UILabel friendGameModeLabel;

	public UILabel friendNameLabel;

	public UILabel survivalScoreLabel;

	public UILabel winCountLabel;

	public UILabel totalWinCountLabel;

	public UILabel clanName;

	public UILabel friendIdLabel;

	public UILabel[] titlesLabel;

	public UITexture clanLogo;

	[Header("Online state settings")]
	public UILabel inFriendStateLabel;

	[Header("Online state settings")]
	public UILabel offlineStateLabel;

	[Header("Online state settings")]
	public UILabel playingStateLabel;

	public UISprite inFriendState;

	public UISprite offlineState;

	public UISprite playingState;

	public GameObject playingStateInfoContainer;

	[Header("Buttons settings")]
	public UIButton backButton;

	public UIButton joinButton;

	public UIButton sendMyIdButton;

	public UIButton chatButton;

	public UIButton inviteToClanButton;

	public UIButton addFriendButton;

	public UIButton removeFriendButton;

	public UITable buttonAlignContainer;

	public UILabel addOrRemoveButtonLabel;

	public UISprite notConnectJoinButtonSprite;

	public UISprite addFrienButtonSentState;

	public UISprite addClanButtonSentState;

	private IDisposable _backSubscription;

	private bool _escapePressed;

	private float lastTime;

	private float idleTimerLastTime;

	private readonly System.Lazy<Rect> _touchZone = new System.Lazy<Rect>(() => new Rect(0f, 0.1f * (float)Screen.height, 0.5f * (float)Screen.width, 0.8f * (float)Screen.height));

	public bool IsCanConnectToFriend { get; set; }

	public string FriendLocation { get; set; }

	public int FriendCount { get; set; }

	public string FriendName { get; set; }

	public OnlineState Online { get; set; }

	public int Rank { get; set; }

	public int SurvivalScore { get; set; }

	public string Username { get; set; }

	public int WinCount { get; set; }

	public int TotalWinCount { get; set; }

	public string FriendGameMode { get; set; }

	public string FriendId { get; set; }

	public string NotConnectCondition { get; set; }

	public event Action BackButtonClickEvent;

	public event Action JoinButtonClickEvent;

	public event Action CopyMyIdButtonClickEvent;

	public event Action ChatButtonClickEvent;

	public event Action AddButtonClickEvent;

	public event Action RemoveButtonClickEvent;

	public event Action InviteToClanButtonClickEvent;

	public event Action UpdateRequested;

	public void Reset()
	{
		IsCanConnectToFriend = false;
		FriendLocation = string.Empty;
		FriendCount = 0;
		FriendName = string.Empty;
		Online = ((!FriendsController.IsPlayerOurFriend(FriendId)) ? OnlineState.none : OnlineState.offline);
		Rank = 0;
		SurvivalScore = 0;
		Username = string.Empty;
		WinCount = 0;
		if (characterModel != null)
		{
			Texture texture = Resources.Load<Texture>(ResPath.Combine(Defs.MultSkinsDirectoryName, "multi_skin_1"));
			if (texture != null)
			{
				GameObject[] stopObjs = new GameObject[7]
				{
					(bootsPoint == null || bootsPoint.Length <= 0) ? null : bootsPoint[0].transform.parent.gameObject,
					hatPoint,
					capePoint,
					armorPoint,
					armorLeftPl,
					armorRightPl,
					maskPoint
				};
				Player_move_c.SetTextureRecursivelyFrom(characterModel, texture, stopObjs);
			}
		}
		SetOnlineState(Online);
		if (bootsPoint != null && bootsPoint.Length > 0)
		{
			GameObject[] array = bootsPoint;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(false);
			}
		}
		if (hatPoint != null)
		{
			Transform transform = hatPoint.transform;
			for (int j = 0; j != transform.childCount; j++)
			{
				Transform child = transform.GetChild(j);
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
		if (maskPoint != null)
		{
			Transform transform2 = maskPoint.transform;
			for (int k = 0; k != transform2.childCount; k++)
			{
				UnityEngine.Object.Destroy(transform2.GetChild(k).gameObject);
			}
		}
		if (capePoint != null)
		{
			Transform transform3 = capePoint.transform;
			for (int l = 0; l != transform3.childCount; l++)
			{
				Transform child2 = transform3.GetChild(l);
				UnityEngine.Object.Destroy(child2.gameObject);
			}
		}
		if (armorPoint != null)
		{
			Transform transform4 = armorPoint.transform;
			for (int m = 0; m != transform4.childCount; m++)
			{
				Transform child3 = transform4.GetChild(m);
				UnityEngine.Object.Destroy(child3.gameObject);
			}
		}
		SetEnableAddButton(true);
		SetEnableInviteClanButton(true);
	}

	public void SetBoots(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			Debug.LogWarning("Name of boots should not be empty.");
		}
		else if (bootsPoint != null && bootsPoint.Length > 0)
		{
			for (int i = 0; i != bootsPoint.Length; i++)
			{
				bootsPoint[i].SetActive(bootsPoint[i].name.Equals(name));
			}
		}
	}

	private void SetOnlineState(OnlineState onlineState)
	{
		bool isStateOffline = onlineState == OnlineState.offline;
		bool isStateInFriends = onlineState == OnlineState.inFriends;
		bool isStatePlaying = onlineState == OnlineState.playing;
		offlineStateLabel.Do(delegate(UILabel l)
		{
			l.gameObject.SetActive(isStateOffline);
		});
		inFriendStateLabel.Do(delegate(UILabel l)
		{
			l.gameObject.SetActive(isStateInFriends);
		});
		playingStateLabel.Do(delegate(UILabel l)
		{
			l.gameObject.SetActive(isStatePlaying);
		});
		offlineState.Do(delegate(UISprite l)
		{
			l.gameObject.SetActive(isStateOffline);
		});
		inFriendState.Do(delegate(UISprite l)
		{
			l.gameObject.SetActive(isStateInFriends);
		});
		playingState.Do(delegate(UISprite l)
		{
			l.gameObject.SetActive(isStatePlaying);
		});
		if (playingStateInfoContainer != null)
		{
			playingStateInfoContainer.SetActive(isStatePlaying);
		}
	}

	public void SetStockCape(string capeName)
	{
		if (string.IsNullOrEmpty(capeName))
		{
			Debug.LogWarning("Name of cape should not be empty.");
		}
		else if (capePoint != null)
		{
			Transform transform = capePoint.transform;
			for (int i = 0; i != transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			UnityEngine.Object @object = Resources.Load("Capes/" + capeName);
			if (@object != null)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(@object);
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				Player_move_c.SetLayerRecursively(gameObject, capePoint.layer);
			}
		}
	}

	public void SetCustomCape(byte[] capeBytes)
	{
		if (capePoint != null)
		{
			Transform transform = capePoint.transform;
			for (int i = 0; i != transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			UnityEngine.Object @object = Resources.Load("Capes/cape_Custom");
			if (@object != null)
			{
				capeBytes = capeBytes ?? new byte[0];
				Texture2D texture2D = new Texture2D(12, 16, TextureFormat.ARGB32, false);
				texture2D.LoadImage(capeBytes);
				texture2D.filterMode = FilterMode.Point;
				texture2D.Apply();
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(@object);
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				Player_move_c.SetLayerRecursively(gameObject, capePoint.layer);
				gameObject.GetComponent<CustomCapePicker>().shouldLoadTexture = false;
				Player_move_c.SetTextureRecursivelyFrom(gameObject, texture2D, new GameObject[0]);
			}
		}
	}

	public void SetArmor(string armorName)
	{
		if (string.IsNullOrEmpty(armorName))
		{
			Debug.LogWarning("Name of armor should not be empty.");
			return;
		}
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < armorPoint.transform.childCount; i++)
		{
			list.Add(armorPoint.transform.GetChild(i));
		}
		foreach (Transform item in list)
		{
			ArmorRefs component = item.GetChild(0).GetComponent<ArmorRefs>();
			if (component != null)
			{
				if (component.leftBone != null)
				{
					component.leftBone.parent = item.GetChild(0);
				}
				if (component.rightBone != null)
				{
					component.rightBone.parent = item.GetChild(0);
				}
				item.parent = null;
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		if (armorName.Equals(Defs.ArmorNewNoneEqupped))
		{
			return;
		}
		UnityEngine.Object @object = Resources.Load("Armor/" + armorName);
		if (!(@object == null))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
			ArmorRefs component2 = gameObject.transform.GetChild(0).GetComponent<ArmorRefs>();
			if (component2 != null)
			{
				component2.leftBone.parent = armorLeftPl.transform;
				component2.leftBone.localPosition = Vector3.zero;
				component2.leftBone.localRotation = Quaternion.identity;
				component2.leftBone.localScale = Vector3.one;
				component2.rightBone.parent = armorRightPl.transform;
				component2.rightBone.localPosition = Vector3.zero;
				component2.rightBone.localRotation = Quaternion.identity;
				component2.rightBone.localScale = Vector3.one;
				gameObject.transform.parent = armorPoint.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				Player_move_c.SetLayerRecursively(gameObject, armorPoint.layer);
			}
		}
	}

	public void SetHat(string hatName)
	{
		if (string.IsNullOrEmpty(hatName))
		{
			Debug.LogWarning("Name of hat should not be empty.");
		}
		else if (hatPoint != null)
		{
			Transform transform = hatPoint.transform;
			for (int i = 0; i != transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			UnityEngine.Object @object = Resources.Load("Hats/" + hatName);
			if (@object != null)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(@object);
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				Player_move_c.SetLayerRecursively(gameObject, hatPoint.layer);
			}
		}
	}

	public void SetMask(string maskName)
	{
		if (string.IsNullOrEmpty(maskName))
		{
			Debug.LogWarning("Name of mask should not be empty.");
		}
		else if (maskPoint != null)
		{
			Transform transform = maskPoint.transform;
			for (int i = 0; i != transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			UnityEngine.Object @object = Resources.Load("Masks/" + maskName);
			if (@object != null)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(@object);
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				Player_move_c.SetLayerRecursively(gameObject, maskPoint.layer);
			}
		}
	}

	public void SetSkin(byte[] skinBytes)
	{
		skinBytes = skinBytes ?? new byte[0];
		if (characterModel != null)
		{
			Func<byte[], Texture2D> func = delegate(byte[] bytes)
			{
				Texture2D texture2D = new Texture2D(64, 32)
				{
					filterMode = FilterMode.Point
				};
				texture2D.LoadImage(bytes);
				texture2D.Apply();
				return texture2D;
			};
			Texture2D txt = ((skinBytes.Length <= 0) ? Resources.Load<Texture2D>(ResPath.Combine(Defs.MultSkinsDirectoryName, "multi_skin_1")) : func(skinBytes));
			GameObject[] stopObjs = new GameObject[7]
			{
				(bootsPoint == null || bootsPoint.Length <= 0) ? null : bootsPoint[0].transform.parent.gameObject,
				hatPoint,
				capePoint,
				armorPoint,
				armorLeftPl,
				armorRightPl,
				maskPoint
			};
			Player_move_c.SetTextureRecursivelyFrom(characterModel, txt, stopObjs);
		}
	}

	private void Awake()
	{
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager();
		Reset();
		if (backButton != null)
		{
			EventDelegate.Add(backButton.onClick, OnBackButtonClick);
		}
		if (joinButton != null)
		{
			EventDelegate.Add(joinButton.onClick, OnJoinButtonClick);
		}
		if (sendMyIdButton != null)
		{
			EventDelegate.Add(sendMyIdButton.onClick, OnSendMyIdButtonClick);
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				sendMyIdButton.gameObject.SetActive(false);
			}
		}
		if (chatButton != null)
		{
			EventDelegate.Add(chatButton.onClick, OnChatButtonClick);
		}
		if (addFriendButton != null)
		{
			EventDelegate.Add(addFriendButton.onClick, OnAddButtonClick);
		}
		if (removeFriendButton != null)
		{
			EventDelegate.Add(removeFriendButton.onClick, OnRemoveButtonClick);
		}
		if (inviteToClanButton != null)
		{
			EventDelegate.Add(inviteToClanButton.onClick, OnInviteToClanButtonClick);
		}
	}

	private void OnDisable()
	{
		StopCoroutine("RequestUpdate");
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void OnEnable()
	{
		StartCoroutine("RequestUpdate");
		idleTimerLastTime = Time.realtimeSinceStartup;
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Friend Profile");
	}

	private void HandleEscape()
	{
		if (!InfoWindowController.IsActive)
		{
			_escapePressed = true;
		}
	}

	private void OnBackButtonClick()
	{
		if (this.BackButtonClickEvent != null)
		{
			this.BackButtonClickEvent();
		}
	}

	private void OnJoinButtonClick()
	{
		if (this.JoinButtonClickEvent != null)
		{
			this.JoinButtonClickEvent();
		}
	}

	private void OnSendMyIdButtonClick()
	{
		if (this.CopyMyIdButtonClickEvent != null)
		{
			this.CopyMyIdButtonClickEvent();
		}
	}

	private void OnChatButtonClick()
	{
		if (this.ChatButtonClickEvent != null)
		{
			this.ChatButtonClickEvent();
		}
	}

	private void OnAddButtonClick()
	{
		if (this.AddButtonClickEvent != null)
		{
			this.AddButtonClickEvent();
		}
	}

	private void OnRemoveButtonClick()
	{
		if (this.RemoveButtonClickEvent != null)
		{
			this.RemoveButtonClickEvent();
		}
	}

	private void OnInviteToClanButtonClick()
	{
		if (this.InviteToClanButtonClickEvent != null)
		{
			this.InviteToClanButtonClickEvent();
		}
	}

	[Obfuscation(Exclude = true)]
	private IEnumerator RequestUpdate()
	{
		while (true)
		{
			if (this.UpdateRequested != null)
			{
				this.UpdateRequested();
			}
			yield return new WaitForSeconds(5f);
		}
	}

	private void Update()
	{
		if (_escapePressed)
		{
			_escapePressed = false;
			OnBackButtonClick();
			return;
		}
		UpdateLightweight();
		float num = -120f;
		num *= ((BuildSettings.BuildTargetPlatform != RuntimePlatform.Android) ? 0.5f : 2f);
		Rect value = _touchZone.Value;
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Moved && value.Contains(touch.position))
			{
				idleTimerLastTime = Time.realtimeSinceStartup;
				pers.Rotate(Vector3.up, touch.deltaPosition.x * num * 0.5f * (Time.realtimeSinceStartup - lastTime));
			}
		}
		if (Application.isEditor)
		{
			float num2 = Input.GetAxis("Mouse ScrollWheel") * 3f * num * (Time.realtimeSinceStartup - lastTime);
			pers.Rotate(Vector3.up, num2);
			if (num2 != 0f)
			{
				idleTimerLastTime = Time.realtimeSinceStartup;
			}
		}
		if (Time.realtimeSinceStartup - idleTimerLastTime > ShopNGUIController.IdleTimeoutPers)
		{
			ReturnPersTonNormState();
		}
		lastTime = Time.realtimeSinceStartup;
	}

	private void ReturnPersTonNormState()
	{
		HOTween.Kill(pers);
		Vector3 p_endVal = new Vector3(0f, -180f, 0f);
		idleTimerLastTime = Time.realtimeSinceStartup;
		HOTween.To(pers, 0.5f, new TweenParms().Prop("localRotation", new PlugQuaternion(p_endVal)).Ease(EaseType.Linear).OnComplete((TweenDelegate.TweenCallback)delegate
		{
			idleTimerLastTime = Time.realtimeSinceStartup;
		}));
	}

	private void UpdateLightweight()
	{
		if (friendLocationLabel != null)
		{
			friendLocationLabel.text = FriendLocation ?? string.Empty;
		}
		if (friendCountLabel != null)
		{
			friendCountLabel.text = ((FriendCount >= 0) ? FriendCount.ToString() : "-");
		}
		if (friendNameLabel != null)
		{
			friendNameLabel.text = FriendName ?? string.Empty;
		}
		SetOnlineState(Online);
		notConnectJoinButtonSprite.alpha = ((!IsCanConnectToFriend) ? 1f : 0f);
		if (rankSprite != null)
		{
			string text = "Rank_" + Rank;
			if (!rankSprite.spriteName.Equals(text))
			{
				rankSprite.spriteName = text;
			}
		}
		if (survivalScoreLabel != null)
		{
			survivalScoreLabel.text = ((SurvivalScore >= 0) ? SurvivalScore.ToString() : "-");
		}
		if (winCountLabel != null)
		{
			winCountLabel.text = ((WinCount >= 0) ? WinCount.ToString() : "-");
		}
		if (totalWinCountLabel != null)
		{
			totalWinCountLabel.text = ((TotalWinCount >= 0) ? TotalWinCount.ToString() : "-");
		}
		if (friendGameModeLabel != null)
		{
			friendGameModeLabel.text = FriendGameMode;
		}
		if (friendIdLabel != null)
		{
			friendIdLabel.text = FriendId;
		}
	}

	public void SetTitle(string titleText)
	{
		for (int i = 0; i < titlesLabel.Length; i++)
		{
			titlesLabel[i].text = titleText;
		}
	}

	private void SetActiveAndRepositionButtons(GameObject button, bool isActive)
	{
		bool activeSelf = button.activeSelf;
		button.SetActive(isActive);
		if (activeSelf != isActive)
		{
			buttonAlignContainer.Reposition();
			buttonAlignContainer.repositionNow = true;
		}
	}

	public void SetActiveChatButton(bool isActive)
	{
		SetActiveAndRepositionButtons(chatButton.gameObject, isActive);
	}

	public void SetActiveInviteButton(bool isActive)
	{
		SetActiveAndRepositionButtons(inviteToClanButton.gameObject, isActive);
	}

	public void SetActiveAddButton(bool isActive)
	{
		SetActiveAndRepositionButtons(addFriendButton.gameObject, isActive);
	}

	public void SetActiveAddButtonSent(bool isActive)
	{
		SetActiveAndRepositionButtons(addFrienButtonSentState.gameObject, isActive);
	}

	public void SetActiveAddClanButtonSent(bool isActive)
	{
		SetActiveAndRepositionButtons(addClanButtonSentState.gameObject, isActive);
	}

	public void SetActiveRemoveButton(bool isActive)
	{
		SetActiveAndRepositionButtons(removeFriendButton.gameObject, isActive);
	}

	public void SetEnableAddButton(bool enable)
	{
		if (addFriendButton != null)
		{
			addFriendButton.isEnabled = enable;
		}
	}

	public void SetEnableRemoveButton(bool enable)
	{
		if (removeFriendButton != null)
		{
			removeFriendButton.isEnabled = enable;
		}
	}

	public void SetEnableInviteClanButton(bool enable)
	{
		if (inviteToClanButton != null)
		{
			inviteToClanButton.isEnabled = enable;
		}
	}
}
