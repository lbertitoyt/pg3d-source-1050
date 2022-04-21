using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FyberPlugin;
using Rilisoft;
using UnityEngine;

internal sealed class FyberFacade
{
	private readonly LinkedList<System.Threading.Tasks.Task<Ad>> _requests = new LinkedList<System.Threading.Tasks.Task<Ad>>();

	private static readonly Rilisoft.Lazy<FyberFacade> _instance = new Rilisoft.Lazy<FyberFacade>(() => new FyberFacade());

	public static FyberFacade Instance
	{
		get
		{
			return _instance.Value;
		}
	}

	public LinkedList<System.Threading.Tasks.Task<Ad>> Requests
	{
		get
		{
			return _requests;
		}
	}

	private FyberFacade()
	{
	}

	public System.Threading.Tasks.Task<Ad> RequestImageInterstitial(string callerName = null)
	{
		if (callerName == null)
		{
			callerName = string.Empty;
		}
		TaskCompletionSource<Ad> promise = new TaskCompletionSource<Ad>();
		return RequestImageInterstitialCore(promise, callerName);
	}

	private System.Threading.Tasks.Task<Ad> RequestImageInterstitialCore(TaskCompletionSource<Ad> promise, string callerName)
	{
		Action<Ad> onAdAvailable = null;
		Action<AdFormat> onAdNotAvailable = null;
		Action<RequestError> onRequestFail = null;
		onAdAvailable = delegate(Ad ad)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("RequestImageInterstitialCore > AdAvailable: {{ format: {0}, placementId: '{1}' }}", ad.AdFormat, ad.PlacementId);
			}
			promise.SetResult(ad);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		onAdNotAvailable = delegate(AdFormat adFormat)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("RequestImageInterstitialCore > AdNotAvailable: {{ format: {0} }}", adFormat);
			}
			AdNotAwailableException exception2 = new AdNotAwailableException("Ad not available: " + adFormat);
			promise.SetException((Exception)exception2);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		onRequestFail = delegate(RequestError requestError)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("RequestImageInterstitialCore > RequestFail: {{ requestError: {0} }}", requestError.Description);
			}
			AdRequestException exception = new AdRequestException(requestError.Description);
			promise.SetException((Exception)exception);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		FyberCallback.AdAvailable += onAdAvailable;
		FyberCallback.AdNotAvailable += onAdNotAvailable;
		FyberCallback.RequestFail += onRequestFail;
		RequestInterstitialAds(callerName);
		if (Application.isEditor)
		{
			promise.SetException((Exception)new NotSupportedException("Ads are not supported in Editor."));
		}
		return promise.Task;
	}

	public System.Threading.Tasks.Task<AdResult> ShowInterstitial(Dictionary<string, string> parameters, string callerName = null)
	{
		if (parameters == null)
		{
			parameters = new Dictionary<string, string>();
		}
		if (callerName == null)
		{
			callerName = string.Empty;
		}
		Debug.LogFormat("[Rilisoft] ShowInterstitial('{0}')", callerName);
		if (Requests.Count == 0)
		{
			Debug.LogWarning("[Rilisoft]No active requests.");
			TaskCompletionSource<AdResult> val = new TaskCompletionSource<AdResult>();
			val.SetException((Exception)new InvalidOperationException("No active requests."));
			return val.Task;
		}
		Debug.LogWarning("[Rilisoft] Active requests count: " + Requests.Count);
		LinkedListNode<System.Threading.Tasks.Task<Ad>> requestNode = null;
		for (LinkedListNode<System.Threading.Tasks.Task<Ad>> val2 = Requests.Last; val2 != null; val2 = val2.Previous)
		{
			if (!((System.Threading.Tasks.Task)val2.Value).IsFaulted)
			{
				if (((System.Threading.Tasks.Task)val2.Value).IsCompleted)
				{
					requestNode = val2;
					break;
				}
				if (requestNode == null)
				{
					requestNode = val2;
				}
			}
		}
		if (requestNode == null)
		{
			string text = "All requests are faulted: " + Requests.Count;
			Debug.LogWarning("[Rilisoft]" + text);
			TaskCompletionSource<AdResult> val3 = new TaskCompletionSource<AdResult>();
			val3.SetException((Exception)new InvalidOperationException(text));
			return val3.Task;
		}
		TaskCompletionSource<AdResult> showPromise = new TaskCompletionSource<AdResult>();
		Action<System.Threading.Tasks.Task<Ad>> action = delegate(System.Threading.Tasks.Task<Ad> requestFuture)
		{
			if (((System.Threading.Tasks.Task)requestFuture).IsFaulted)
			{
				string text2 = "Ad request failed: " + ((Exception)(object)((System.Threading.Tasks.Task)requestFuture).Exception).InnerException.Message;
				Debug.LogWarningFormat("[Rilisoft] {0}", text2);
				showPromise.SetException((Exception)new AdRequestException(text2, ((Exception)(object)((System.Threading.Tasks.Task)requestFuture).Exception).InnerException));
			}
			else
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("[Rilisoft] Ad request succeeded: {{ adFormat: {0}, placementId: '{1}' }}", requestFuture.Result.AdFormat, requestFuture.Result.PlacementId);
				}
				Action<AdResult> adFinished = null;
				adFinished = delegate(AdResult adResult)
				{
					Rilisoft.Lazy<string> lazy = new Rilisoft.Lazy<string>(() => string.Format("[Rilisoft] Ad show finished: {{ format: {0}, status: {1}, message: '{2}' }}", adResult.AdFormat, adResult.Status, adResult.Message));
					if (adResult.Status == AdStatus.Error)
					{
						Debug.LogWarning(lazy.Value);
					}
					else if (Defs.IsDeveloperBuild)
					{
						Debug.Log(lazy.Value);
					}
					FyberCallback.AdFinished -= adFinished;
					showPromise.SetResult(adResult);
					if (adResult.Status == AdStatus.OK)
					{
						parameters["Fyber - Interstitial"] = "Impression: " + adResult.Message;
						FlurryPluginWrapper.LogEventAndDublicateToConsole("Ads Show Stats - Total", parameters);
					}
				};
				FyberCallback.AdFinished += adFinished;
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Start showing ad: {{ format: {0}, placementId: '{1}' }}", requestFuture.Result.AdFormat, requestFuture.Result.PlacementId);
				}
				requestFuture.Result.Start();
				Requests.Remove(requestNode);
			}
		};
		if (((System.Threading.Tasks.Task)requestNode.Value).IsCompleted)
		{
			action(requestNode.Value);
		}
		else
		{
			requestNode.Value.ContinueWith(action);
		}
		return showPromise.Task;
	}

	public void SetUserPaying(string payingBin)
	{
		if (string.IsNullOrEmpty(payingBin))
		{
			payingBin = "0";
		}
		SetUserPayingCore(payingBin);
	}

	public void UpdateUserPaying()
	{
		string userPayingCore = Storager.getInt("PayingUser", true).ToString(CultureInfo.InvariantCulture);
		SetUserPayingCore(userPayingCore);
	}

	private void SetUserPayingCore(string payingBin)
	{
		User.PutCustomValue("pg3d_paying", payingBin);
	}

	private static void RequestInterstitialAds(string callerName)
	{
		InterstitialRequester.Create().Request();
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("[Rilisoft] RequestInterstitialAds('{0}')", callerName);
			Debug.Log(message);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Fyber - Interstitial", "Request");
		Dictionary<string, string> parameters = dictionary;
		FlurryPluginWrapper.LogEventAndDublicateToConsole("Ads Show Stats - Total", parameters);
	}
}
