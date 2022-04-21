using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class PersistentCacheManager : MonoBehaviour
	{
		[Serializable]
		private struct CacheEntry
		{
			public string signature;

			public string payload;
		}

		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		private readonly TaskCompletionSource<Dictionary<string, string>> _firstResponsePromise = new TaskCompletionSource<Dictionary<string, string>>();

		private readonly Dictionary<string, string> _signatures = new Dictionary<string, string>();

		private static readonly Lazy<PersistentCacheManager> _instance = new Lazy<PersistentCacheManager>(Create);

		internal bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		internal System.Threading.Tasks.Task FirstResponse
		{
			get
			{
				return _firstResponsePromise.Task;
			}
		}

		public static PersistentCacheManager Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		public System.Threading.Tasks.Task StartDownloadSignaturesLoop()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			TaskCompletionSource<Dictionary<string, string>> firstResponsePromise = _firstResponsePromise;
			if (!IsEnabled)
			{
				firstResponsePromise.TrySetException((Exception)new NotSupportedException());
				return firstResponsePromise.Task;
			}
			_cancellationTokenSource.Cancel();
			_cancellationTokenSource = new CancellationTokenSource();
			float delaySeconds = (Application.isEditor ? 30f : ((!Defs.IsDeveloperBuild) ? 600f : 60f));
			StartCoroutine(DownloadSignaturesLoopCoroutine(delaySeconds, _cancellationTokenSource.Token, firstResponsePromise));
			return firstResponsePromise.Task;
		}

		public string GetValue(string keyUrl)
		{
			//Discarded unreachable code: IL_00d6, IL_00ec
			if (keyUrl == null)
			{
				throw new ArgumentNullException("keyUrl");
			}
			if (!IsEnabled)
			{
				return null;
			}
			if (string.IsNullOrEmpty(keyUrl))
			{
				return string.Empty;
			}
			string segmentsAsString = GetSegmentsAsString(keyUrl);
			if (string.IsNullOrEmpty(segmentsAsString))
			{
				return string.Empty;
			}
			string storageKey = GetStorageKey(segmentsAsString);
			string @string = Storager.getString(storageKey, false);
			if (string.IsNullOrEmpty(@string))
			{
				return string.Empty;
			}
			try
			{
				CacheEntry cacheEntry = JsonUtility.FromJson<CacheEntry>(@string);
				string value;
				if (_signatures.TryGetValue(segmentsAsString, out value) && StringComparer.Ordinal.Equals(value, cacheEntry.signature))
				{
					return cacheEntry.payload ?? string.Empty;
				}
				if (Storager.hasKey(storageKey))
				{
					Storager.setString(storageKey, string.Empty, false);
				}
				return string.Empty;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return null;
			}
		}

		public bool SetValue(string keyUrl, string value)
		{
			if (keyUrl == null)
			{
				throw new ArgumentNullException("keyUrl");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!IsEnabled)
			{
				return false;
			}
			string segmentsAsString = GetSegmentsAsString(keyUrl);
			if (string.IsNullOrEmpty(segmentsAsString))
			{
				return false;
			}
			string value2;
			if (!_signatures.TryGetValue(segmentsAsString, out value2))
			{
				return false;
			}
			string storageKey = GetStorageKey(segmentsAsString);
			CacheEntry cacheEntry = default(CacheEntry);
			cacheEntry.signature = value2;
			cacheEntry.payload = value;
			CacheEntry cacheEntry2 = cacheEntry;
			string val = JsonUtility.ToJson(cacheEntry2);
			Storager.setString(storageKey, val, false);
			return true;
		}

		internal static void DebugReportCacheHit(string url)
		{
			if (Defs.IsDeveloperBuild)
			{
				string text = string.Format("Cache hit: {0}", url ?? string.Empty);
				if (Application.isEditor)
				{
					Debug.LogFormat("<color=green><b>{0}</b></color>", text);
				}
				else
				{
					Debug.Log(text);
				}
			}
		}

		internal static void DebugReportCacheMiss(string url)
		{
			if (Defs.IsDeveloperBuild)
			{
				string text = string.Format("Cache miss: {0}", url ?? string.Empty);
				if (Application.isEditor)
				{
					Debug.LogFormat("<color=orange><b>{0}</b></color>", text);
				}
				else
				{
					Debug.Log(text);
				}
			}
		}

		private IEnumerator DownloadSignaturesLoopCoroutine(float delaySeconds, CancellationToken cancellationToken, TaskCompletionSource<Dictionary<string, string>> promise)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			while (!((CancellationToken)(cancellationToken)).IsCancellationRequested)
			{
				StartCoroutine(DownloadSignaturesOnceCoroutine(cancellationToken, promise, i));
				yield return new WaitForSeconds(delaySeconds);
				i++;
			}
		}

		private IEnumerator DownloadSignaturesOnceCoroutine(CancellationToken cancellationToken, TaskCompletionSource<Dictionary<string, string>> promise, int context = 0)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			string[] urls = new string[8]
			{
				URLs.PromoActions,
				URLs.PromoActionsTest,
				URLs.LobbyNews,
				URLs.Advert,
				ChestBonusModel.GetUrlForDownloadBonusesData(),
				(!FriendsController.useBuffSystem) ? URLs.BuffSettings1031 : URLs.BuffSettings1050,
				URLs.BestBuy,
				URLs.PixelbookSettings
			};
			TaskCompletionSource<string> signaturesStringPromise = new TaskCompletionSource<string>();
			yield return StartCoroutine(DownloadSignaturesCoroutine(urls, cancellationToken, signaturesStringPromise));
			yield return StartCoroutine(WaitCompletionAndParseSignaturesCoroutine(signaturesStringPromise.Task, promise));
			if (((System.Threading.Tasks.Task)promise.Task).IsCanceled)
			{
				Debug.LogWarningFormat("DownloadSignaturesOnceCoroutine({0}) cancelled.", context);
				yield break;
			}
			if (((System.Threading.Tasks.Task)promise.Task).IsFaulted)
			{
				Debug.LogWarningFormat("DownloadSignaturesOnceCoroutine({0}) failed: {1}", context, ((Exception)(object)((System.Threading.Tasks.Task)promise.Task).Exception).InnerException);
				string[] array = urls;
				foreach (string url in array)
				{
					string segments = GetSegmentsAsString(url);
					string storageKey = GetStorageKey(segments);
					if (Storager.hasKey(storageKey))
					{
						Storager.setString(storageKey, string.Empty, false);
					}
				}
				_signatures.Clear();
				yield break;
			}
			Dictionary<string, string> d = promise.Task.Result;
			if (Defs.IsDeveloperBuild)
			{
				string format = ((!Application.isEditor) ? "DownloadSignaturesOnceCoroutine({0}) succeeded: {1}" : "DownloadSignaturesOnceCoroutine({0}) succeeded: <color=magenta>{1}</color>");
				Debug.LogFormat(format, context, Json.Serialize(d));
			}
			foreach (KeyValuePair<string, string> kv in d)
			{
				_signatures[kv.Key] = kv.Value;
			}
		}

		private static IEnumerator WaitCompletionAndParseSignaturesCoroutine(System.Threading.Tasks.Task<string> future, TaskCompletionSource<Dictionary<string, string>> promise)
		{
			try
			{
				System.Threading.Tasks.Task<string> future2 = default(System.Threading.Tasks.Task<string>);
				yield return new WaitUntil(() => ((System.Threading.Tasks.Task)future2).IsCompleted);
				if (((System.Threading.Tasks.Task)future).IsCanceled)
				{
					promise.TrySetCanceled();
					yield break;
				}
				if (((System.Threading.Tasks.Task)future).IsFaulted)
				{
					promise.TrySetException(((System.Threading.Tasks.Task)future).Exception);
					yield break;
				}
				Dictionary<string, string> result = new Dictionary<string, string>();
				Dictionary<string, object> signaturesDictionaryObjects = Json.Deserialize(future.Result) as Dictionary<string, object>;
				if (signaturesDictionaryObjects == null)
				{
					promise.TrySetResult(result);
					yield break;
				}
				foreach (KeyValuePair<string, object> kv in signaturesDictionaryObjects)
				{
					string s = kv.Value as string;
					if (!string.IsNullOrEmpty(s))
					{
						result[kv.Key] = s;
					}
				}
				promise.TrySetResult(result);
			}
			finally
			{
			}
		}

		private static IEnumerator DownloadSignaturesCoroutine(string[] urls, CancellationToken cancellationToken, TaskCompletionSource<string> promise)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (urls.Length == 0)
				{
					promise.TrySetResult("{}");
					yield break;
				}
				List<string> names = urls.Select(GetSegmentsAsString).ToList();
				string namesSerialized = Json.Serialize(names);
				WWWForm form = new WWWForm();
				form.AddField("app_version", string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion));
				form.AddField("names", namesSerialized);
				WWW request = Tools.CreateWwwIfNotConnected("https://secure.pixelgunserver.com/get_files_info.php", form, "DownloadSignaturesCoroutine()");
				if (request == null)
				{
					promise.TrySetCanceled();
					yield break;
				}
				while (!request.isDone)
				{
					yield return null;
					if (((CancellationToken)(cancellationToken)).IsCancellationRequested)
					{
						promise.TrySetCanceled();
						yield break;
					}
				}
				if (!string.IsNullOrEmpty(request.error))
				{
					promise.TrySetException((Exception)new InvalidOperationException(request.error));
					yield break;
				}
				string response = URLs.Sanitize(request);
				promise.TrySetResult(response);
			}
			finally
			{
			}
		}

		private static string GetStorageKey(string segments)
		{
			return string.Format("Cache:{0}", segments);
		}

		private static string GetSegmentsAsString(string url)
		{
			//Discarded unreachable code: IL_0090, IL_00a2
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			if (string.IsNullOrEmpty(url))
			{
				return string.Empty;
			}
			try
			{
				Uri val = new Uri(url);
				string[] segments = val.Segments;
				if (segments.Length == 0)
				{
					return string.Empty;
				}
				StringComparer c = StringComparer.OrdinalIgnoreCase;
				string[] array = segments.SkipWhile((string s, int i) => i == 0 && c.Equals(s, "/")).SkipWhile((string s, int i) => i == 0 && c.Equals(s, "pixelgun3d-config/")).ToArray();
				return string.Concat(array).TrimStart('/');
			}
			catch
			{
				return string.Empty;
			}
		}

		private static PersistentCacheManager Create()
		{
			PersistentCacheManager persistentCacheManager = UnityEngine.Object.FindObjectOfType<PersistentCacheManager>();
			if (persistentCacheManager != null)
			{
				UnityEngine.Object.DontDestroyOnLoad(persistentCacheManager.gameObject);
				return persistentCacheManager;
			}
			GameObject gameObject = new GameObject("Rilisoft.PersistentCacheManager");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<PersistentCacheManager>();
		}
	}
}
