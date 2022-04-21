using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class ReplaceAdmobPerelivController : MonoBehaviour
{
	public static ReplaceAdmobPerelivController sharedController;

	private Texture2D _image;

	private string _adUrl;

	private static int _timesWantToShow = -1;

	private static int _timesShown;

	private long _timeSuspended;

	public static bool ShouldShowAtThisTime
	{
		get
		{
			return PromoActionsManager.ReplaceAdmobPereliv != null && _timesWantToShow % ((PromoActionsManager.ReplaceAdmobPereliv.ShowEveryTimes == 0) ? 1 : PromoActionsManager.ReplaceAdmobPereliv.ShowEveryTimes) == 0;
		}
	}

	public Texture2D Image
	{
		get
		{
			return _image;
		}
	}

	public string AdUrl
	{
		get
		{
			return _adUrl;
		}
	}

	public bool DataLoaded
	{
		get
		{
			return _image != null && _adUrl != null;
		}
	}

	public bool DataLoading { get; private set; }

	public bool ShouldShowInLobby { get; set; }

	private static bool LimitReached
	{
		get
		{
			if (PromoActionsManager.ReplaceAdmobPereliv == null)
			{
				return true;
			}
			if (PromoActionsManager.ReplaceAdmobPereliv.ShowTimesTotal == 0)
			{
				return false;
			}
			return _timesShown >= PromoActionsManager.ReplaceAdmobPereliv.ShowTimesTotal;
		}
	}

	public static void IncreaseTimesCounter()
	{
		_timesWantToShow++;
	}

	public static void TryShowPereliv(string context)
	{
		if (sharedController != null && sharedController.Image != null && sharedController.AdUrl != null)
		{
			AdmobPerelivWindow.admobTexture = sharedController.Image;
			AdmobPerelivWindow.admobUrl = sharedController.AdUrl;
			AdmobPerelivWindow.Context = context;
			PlayerPrefs.SetString(Defs.LastTimeShowBanerKey, DateTime.UtcNow.ToString("s"));
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Replace Admob With Pereliv Show", FlurryPluginWrapper.LevelAndTierParameters);
			_timesShown++;
		}
	}

	public void DestroyImage()
	{
		if (Image != null)
		{
			_image = null;
		}
	}

	public static bool ReplaceAdmobWithPerelivApplicable()
	{
		if (PromoActionsManager.ReplaceAdmobPereliv == null)
		{
			return false;
		}
		bool flag = ExperienceController.sharedController != null && (PromoActionsManager.ReplaceAdmobPereliv.MinLevel == -1 || PromoActionsManager.ReplaceAdmobPereliv.MinLevel <= ExperienceController.sharedController.currentLevel) && (PromoActionsManager.ReplaceAdmobPereliv.MaxLevel == -1 || PromoActionsManager.ReplaceAdmobPereliv.MaxLevel >= ExperienceController.sharedController.currentLevel);
		bool showToPaying = PromoActionsManager.ReplaceAdmobPereliv.ShowToPaying;
		bool showToNew = PromoActionsManager.ReplaceAdmobPereliv.ShowToNew;
		bool flag2 = MobileAdManager.UserPredicate(MobileAdManager.Type.Image, Defs.IsDeveloperBuild, showToPaying, showToNew);
		if (Debug.isDebugBuild)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(7);
			dictionary.Add("MinLevel", PromoActionsManager.ReplaceAdmobPereliv.MinLevel);
			dictionary.Add("MaxLevel", PromoActionsManager.ReplaceAdmobPereliv.MaxLevel);
			dictionary.Add("levelConstraintIsOk", flag);
			dictionary.Add("LimitReached", LimitReached);
			dictionary.Add("adIsApplicable", flag2);
			dictionary.Add("PromoActionsManager.ReplaceAdmobPereliv.enabled", PromoActionsManager.ReplaceAdmobPereliv.enabled);
			Dictionary<string, object> obj = dictionary;
			string message = string.Format("ReplaceAdmobWithPerelivApplicable Details: {0}", Json.Serialize(obj));
			Debug.Log(message);
		}
		return flag2 && PromoActionsManager.ReplaceAdmobPereliv.enabled && !LimitReached && flag;
	}

	public void LoadPerelivData()
	{
		if (DataLoading)
		{
			Debug.LogWarning("ReplaceAdmobPerelivController: data is already loading. returning...");
			return;
		}
		if (_image != null)
		{
			UnityEngine.Object.Destroy(_image);
		}
		_image = null;
		_adUrl = null;
		int num = 0;
		if (PromoActionsManager.ReplaceAdmobPereliv.imageUrls.Count > 0)
		{
			num = UnityEngine.Random.Range(0, PromoActionsManager.ReplaceAdmobPereliv.imageUrls.Count);
			StartCoroutine(LoadDataCoroutine(num));
		}
		else
		{
			Debug.LogWarning("ReplaceAdmobPerelivController:PromoActionsManager.ReplaceAdmobPereliv.imageUrls.Count = 0. returning...");
		}
	}

	private string GetImageURLForOurQuality(string urlString)
	{
		string value = string.Empty;
		if (Screen.height >= 500)
		{
			value = "-Medium";
		}
		if (Screen.height >= 900)
		{
			value = "-Hi";
		}
		urlString = urlString.Insert(urlString.LastIndexOf("."), value);
		return urlString;
	}

	private IEnumerator LoadDataCoroutine(int index)
	{
		DataLoading = true;
		string replaceAdmobUrl = GetImageURLForOurQuality(PromoActionsManager.ReplaceAdmobPereliv.imageUrls[index]);
		WWW imageRequest = Tools.CreateWwwIfNotConnected(replaceAdmobUrl);
		if (imageRequest == null)
		{
			DataLoading = false;
			yield break;
		}
		yield return imageRequest;
		if (!string.IsNullOrEmpty(imageRequest.error))
		{
			Debug.LogWarningFormat("ReplaceAdmobPerelivController: {0}", imageRequest.error);
			DataLoading = false;
		}
		else if (!imageRequest.texture)
		{
			DataLoading = false;
			Debug.LogWarning("ReplaceAdmobPerelivController: imageRequest.texture = null. returning...");
		}
		else if (imageRequest.texture.width < 20)
		{
			DataLoading = false;
			Debug.LogWarning("ReplaceAdmobPerelivController: imageRequest.texture is dummy. returning...");
		}
		else
		{
			_image = imageRequest.texture;
			_adUrl = PromoActionsManager.ReplaceAdmobPereliv.adUrls[index];
			DataLoading = false;
		}
	}

	private void Awake()
	{
		sharedController = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationPause(bool pausing)
	{
		if (!pausing)
		{
			if (PromoActionsManager.CurrentUnixTime - _timeSuspended > 3600)
			{
				_timesShown = 0;
			}
		}
		else
		{
			_timeSuspended = PromoActionsManager.CurrentUnixTime;
		}
	}
}
