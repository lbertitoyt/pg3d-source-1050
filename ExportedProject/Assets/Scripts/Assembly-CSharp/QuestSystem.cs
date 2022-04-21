using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.DictionaryExtensions;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class QuestSystem : MonoBehaviour
{
	internal const string QuestProgressKey = "QuestProgress";

	private const int _questConfigClientVersion = 28;

	private static readonly System.Lazy<QuestSystem> _instance = new System.Lazy<QuestSystem>(InitializeInstance);

	private Coroutine _getConfigLoopCoroutine;

	private Coroutine _getTutorialQuestsConfigLoopCoroutine;

	private QuestProgress _questProgress;

	private DateTime? _startupTimeAccordingToServer;

	public static QuestSystem Instance
	{
		get
		{
			return _instance.Value;
		}
	}

	public QuestProgress QuestProgress
	{
		get
		{
			return _questProgress;
		}
	}

	internal int QuestConfigClientVersion
	{
		get
		{
			return 28;
		}
	}

	public event EventHandler Updated;

	public event EventHandler<QuestCompletedEventArgs> QuestCompleted;

	public void Initialize()
	{
	}

	private void Start()
	{
		_questProgress = LoadQuestProgress();
		if (_questProgress != null)
		{
			_questProgress.QuestCompleted += HandleQuestCompleted;
			if (!TutorialQuestManager.Instance.Received)
			{
				_getTutorialQuestsConfigLoopCoroutine.Do(base.StopCoroutine);
				_getTutorialQuestsConfigLoopCoroutine = StartCoroutine(GetTutorialQuestConfigLoopCoroutine());
			}
		}
		this.Updated.Do(delegate(EventHandler handler)
		{
			handler(this, EventArgs.Empty);
		});
		_getConfigLoopCoroutine = StartCoroutine(GetConfigLoopCoroutine(false));
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			SaveQuestProgress(_questProgress);
			TutorialQuestManager.Instance.SaveIfDirty();
		}
		else
		{
			_getConfigLoopCoroutine.Do(base.StopCoroutine);
			_getConfigLoopCoroutine = StartCoroutine(GetConfigLoopCoroutine(true));
		}
	}

	internal void DebugDecrementDay()
	{
		if (_questProgress != null)
		{
			_questProgress.DebugDecrementDay();
		}
		this.Updated.Do(delegate(EventHandler handler)
		{
			handler(this, EventArgs.Empty);
		});
	}

	internal void ForceGetConfig()
	{
		_getConfigLoopCoroutine.Do(base.StopCoroutine);
		_getConfigLoopCoroutine = StartCoroutine(GetConfigLoopCoroutine(false));
	}

	private void HandleQuestCompleted(object sender, QuestCompletedEventArgs e)
	{
		SaveQuestProgressIfDirty();
		TutorialQuestManager.Instance.SaveIfDirty();
		this.QuestCompleted.Do(delegate(EventHandler<QuestCompletedEventArgs> handler)
		{
			handler(sender, e);
		});
	}

	private System.Threading.Tasks.Task<string> GetQuestConfig()
	{
		TaskCompletionSource<string> val = new TaskCompletionSource<string>();
		StartCoroutine(GetQuestConfigCoroutine(val));
		return val.get_Task();
	}

	private IEnumerator GetQuestConfigCoroutine(TaskCompletionSource<string> tcs)
	{
		WWW response = Tools.CreateWwwIfNotConnected(URLs.QuestConfig);
		if (response == null)
		{
			tcs.TrySetException((Exception)new InvalidOperationException("Skipped quest config request because the player is connected."));
			yield break;
		}
		yield return response;
		if (string.IsNullOrEmpty(response.error))
		{
			string responseText = ((response.text == null) ? string.Empty : URLs.Sanitize(response));
			tcs.TrySetResult(responseText);
		}
		else
		{
			tcs.TrySetException((Exception)new InvalidOperationException(response.error));
		}
	}

	private System.Threading.Tasks.Task<string> GetConfigUpdate()
	{
		TaskCompletionSource<string> val = new TaskCompletionSource<string>();
		StartCoroutine(GetConfigUpdateCoroutine(val));
		return val.get_Task();
	}

	private IEnumerator GetConfigUpdateCoroutine(TaskCompletionSource<string> tcs)
	{
		while (string.IsNullOrEmpty(FriendsController.sharedController.Map((FriendsController fc) => fc.id)))
		{
			yield return null;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_quest_version_info");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("uniq_id", FriendsController.sharedController.id);
		form.AddField("auth", FriendsController.Hash("get_quest_version_info"));
		WWW response = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
		if (response == null)
		{
			tcs.TrySetException((Exception)new InvalidOperationException("Cannot send request while connected."));
			yield break;
		}
		yield return response;
		if (string.IsNullOrEmpty(response.error))
		{
			string responseText = ((response.text == null) ? string.Empty : URLs.Sanitize(response));
			tcs.TrySetResult(responseText);
		}
		else
		{
			tcs.TrySetException((Exception)new InvalidOperationException(response.error));
		}
	}

	private IEnumerator GetTutorialQuestsConfigOnceCoroutine()
	{
		WWW response = Tools.CreateWwwIfNotConnected(URLs.TutorialQuestConfig);
		if (response == null)
		{
			yield break;
		}
		yield return response;
		if (!string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarningFormat("Failed to load tutorial quests: {0}", response.error);
			yield break;
		}
		string responseText = ((response.text == null) ? string.Empty : URLs.Sanitize(response));
		Dictionary<string, object> config = Json.Deserialize(responseText) as Dictionary<string, object>;
		if (config == null)
		{
			Debug.LogWarningFormat("Failed to parse config: '{0}'", responseText);
			yield break;
		}
		List<object> tutorialQuestJsons = config.TryGet("quests") as List<object>;
		if (_questProgress != null && !TutorialQuestManager.Instance.Received)
		{
			if (tutorialQuestJsons != null)
			{
				TutorialQuestManager.Instance.SetReceived();
			}
			_questProgress.FillTutorialQuests(tutorialQuestJsons);
			this.Updated.Do(delegate(EventHandler handler)
			{
				handler(this, EventArgs.Empty);
			});
			SaveQuestProgressIfDirty();
			TutorialQuestManager.Instance.SaveIfDirty();
		}
	}

	private IEnumerator GetConfigOnceCoroutine(bool resumed)
	{
		System.Threading.Tasks.Task<string> configUpdateRequest = GetConfigUpdate();
		while (!((System.Threading.Tasks.Task)configUpdateRequest).get_IsCompleted())
		{
			yield return null;
		}
		float responceReceivedTime = Time.realtimeSinceStartup;
		if (((System.Threading.Tasks.Task)configUpdateRequest).get_IsFaulted())
		{
			Debug.LogWarning(((System.Threading.Tasks.Task)configUpdateRequest).get_Exception());
			yield break;
		}
		Dictionary<string, object> response = Json.Deserialize(configUpdateRequest.get_Result()) as Dictionary<string, object>;
		if (response == null)
		{
			Debug.LogWarning("GetConfigOnceCoroutine(): Bad update response: " + configUpdateRequest.get_Result());
			yield break;
		}
		string version2 = string.Empty;
		long day2 = 0L;
		float timeLeftSeconds2 = 0f;
		DateTime timestamp2 = default(DateTime);
		try
		{
			int serverVersion = Convert.ToInt32(response["version"]);
			version2 = string.Format(arg1: QuestConfigClientVersion, format: "{0}.{1}", arg0: serverVersion);
			day2 = Convert.ToInt64(response["day"]);
			timeLeftSeconds2 = (float)Convert.ToDouble(response["timeLeftSeconds"], CultureInfo.InvariantCulture);
			long timestampUnix = Convert.ToInt64(response["timestamp"], CultureInfo.InvariantCulture);
			timestamp2 = Tools.GetCurrentTimeByUnixTime(timestampUnix);
			_startupTimeAccordingToServer = timestamp2 - TimeSpan.FromSeconds(responceReceivedTime);
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			Debug.LogException(ex);
			yield break;
		}
		if (_questProgress != null && _questProgress.ConfigVersion.Equals(version2, StringComparison.Ordinal) && _questProgress.Day == day2)
		{
			yield break;
		}
		System.Threading.Tasks.Task<string> questConfigRequest = GetQuestConfig();
		while (!((System.Threading.Tasks.Task)questConfigRequest).get_IsCompleted())
		{
			yield return null;
		}
		if (((System.Threading.Tasks.Task)questConfigRequest).get_IsFaulted())
		{
			Debug.LogWarning(((System.Threading.Tasks.Task)questConfigRequest).get_Exception());
			yield break;
		}
		Dictionary<string, object> rawQuests = Json.Deserialize(questConfigRequest.get_Result()) as Dictionary<string, object>;
		if (rawQuests == null)
		{
			Debug.LogWarning("GetConfigOnceCoroutine(): Bad config response: " + questConfigRequest.get_Result());
			yield break;
		}
		List<Difficulty> allowedDifficulties = new List<Difficulty>
		{
			Difficulty.Easy,
			Difficulty.Normal,
			Difficulty.Hard
		};
		if (ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel < 3 && allowedDifficulties.Remove(Difficulty.Hard))
		{
			allowedDifficulties.Add(Difficulty.Normal);
		}
		System.Lazy<IDictionary<int, List<QuestBase>>> newQuests = new System.Lazy<IDictionary<int, List<QuestBase>>>(() => QuestProgress.CreateQuests(rawQuests, day2, allowedDifficulties.ToArray()));
		if (_questProgress == null)
		{
			_questProgress = new QuestProgress(version2, day2, timestamp2, timeLeftSeconds2);
			_getTutorialQuestsConfigLoopCoroutine.Do(base.StopCoroutine);
			_getTutorialQuestsConfigLoopCoroutine = StartCoroutine(GetTutorialQuestConfigLoopCoroutine());
			_questProgress.QuestCompleted += HandleQuestCompleted;
			_questProgress.PopulateQuests(newQuests.Value, null);
			this.Updated.Do(delegate(EventHandler handler)
			{
				handler(this, EventArgs.Empty);
			});
		}
		else if (!_questProgress.ConfigVersion.Equals(version2, StringComparison.Ordinal))
		{
			_questProgress.Dispose();
			_questProgress.QuestCompleted -= HandleQuestCompleted;
			_questProgress = new QuestProgress(version2, day2, timestamp2, timeLeftSeconds2, _questProgress);
			_getTutorialQuestsConfigLoopCoroutine.Do(base.StopCoroutine);
			_getTutorialQuestsConfigLoopCoroutine = StartCoroutine(GetTutorialQuestConfigLoopCoroutine());
			_questProgress.QuestCompleted += HandleQuestCompleted;
			_questProgress.PopulateQuests(newQuests.Value, null);
			this.Updated.Do(delegate(EventHandler handler)
			{
				handler(this, EventArgs.Empty);
			});
		}
		else if (_questProgress.Day < day2)
		{
			_questProgress.UpdateQuests(day2, rawQuests, newQuests.Value);
			this.Updated.Do(delegate(EventHandler handler)
			{
				handler(this, EventArgs.Empty);
			});
		}
		SaveQuestProgressIfDirty();
		TutorialQuestManager.Instance.SaveIfDirty();
	}

	public void SaveQuestProgressIfDirty()
	{
		if (_questProgress == null || !_questProgress.IsDirty())
		{
			return;
		}
		try
		{
			SaveQuestProgress(_questProgress);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private IEnumerator GetConfigLoopCoroutine(bool resumed)
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			yield return null;
		}
		float delaySeconds = ((!Application.isEditor) ? 600f : 30f);
		Coroutine configCoroutine = null;
		while (true)
		{
			if (configCoroutine != null)
			{
				StopCoroutine(configCoroutine);
			}
			configCoroutine = StartCoroutine(GetConfigOnceCoroutine(resumed));
			yield return new WaitForSeconds(delaySeconds);
		}
	}

	private IEnumerator GetTutorialQuestConfigLoopCoroutine()
	{
		float delaySeconds = ((!Application.isEditor) ? 600f : 30f);
		Coroutine configCoroutine = null;
		while (_questProgress == null || !TutorialQuestManager.Instance.Received)
		{
			if (configCoroutine != null)
			{
				StopCoroutine(configCoroutine);
			}
			configCoroutine = StartCoroutine(GetTutorialQuestsConfigOnceCoroutine());
			yield return new WaitForSeconds(delaySeconds);
		}
	}

	private static QuestProgress LoadQuestProgress()
	{
		if (!Storager.hasKey("QuestProgress"))
		{
			return null;
		}
		string @string = Storager.getString("QuestProgress", false);
		if (string.IsNullOrEmpty(@string))
		{
			return null;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null)
		{
			return null;
		}
		if (dictionary.Count == 0)
		{
			return null;
		}
		Func<string, Version> createVersion = delegate(string v)
		{
			//Discarded unreachable code: IL_000c, IL_0021
			try
			{
				return new Version(v);
			}
			catch
			{
				return new Version(0, 0, 0, 0);
			}
		};
		string text = ((dictionary.Count != 1) ? dictionary.Keys.Select((string k) => new KeyValuePair<string, Version>(k, createVersion(k))).Aggregate((KeyValuePair<string, Version> l, KeyValuePair<string, Version> r) => (!(l.Value > r.Value)) ? r : l).Key : dictionary.Keys.First());
		Dictionary<string, object> dictionary2 = dictionary[text] as Dictionary<string, object>;
		if (dictionary2 == null)
		{
			return null;
		}
		object value;
		if (!dictionary2.TryGetValue("day", out value))
		{
			return null;
		}
		object value2;
		if (!dictionary2.TryGetValue("timeLeftSeconds", out value2))
		{
			return null;
		}
		object value3;
		if (!dictionary2.TryGetValue("timestamp", out value3))
		{
			return null;
		}
		QuestProgress questProgress = null;
		try
		{
			long day = Convert.ToInt64(value, CultureInfo.InvariantCulture);
			DateTime timestamp = Convert.ToDateTime(value3, CultureInfo.InvariantCulture);
			float timeLeftSeconds = (float)Convert.ToDouble(value2, CultureInfo.InvariantCulture);
			questProgress = new QuestProgress(text, day, timestamp, timeLeftSeconds);
			Dictionary<string, object> dictionary3 = dictionary2["currentQuests"] as Dictionary<string, object>;
			if (dictionary3 == null)
			{
				return questProgress;
			}
			Dictionary<string, object> dictionary4 = dictionary2["previousQuests"] as Dictionary<string, object>;
			if (dictionary4 == null)
			{
				return questProgress;
			}
			IDictionary<int, List<QuestBase>> currentQuests = QuestProgress.RestoreQuests(dictionary3);
			IDictionary<int, List<QuestBase>> previousQuests = QuestProgress.RestoreQuests(dictionary4);
			questProgress.PopulateQuests(currentQuests, previousQuests);
			List<object> questJsons = dictionary2.TryGet("tutorialQuests") as List<object>;
			questProgress.FillTutorialQuests(questJsons);
			return questProgress;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return questProgress;
		}
	}

	private static void SaveQuestProgress(QuestProgress questProgress)
	{
		if (questProgress != null)
		{
			Dictionary<string, object> value = questProgress.ToJson();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(questProgress.ConfigVersion, value);
			Dictionary<string, object> obj = dictionary;
			string text = Json.Serialize(obj);
			if (questProgress.Count == 0)
			{
				Debug.LogWarning("SaveQuestProgress(): Bad progress: " + text);
				Storager.setString("QuestProgress", "{}", false);
			}
			else
			{
				Storager.setString("QuestProgress", text, false);
				questProgress.SetClean();
			}
		}
	}

	private static QuestSystem InitializeInstance()
	{
		QuestSystem questSystem = UnityEngine.Object.FindObjectOfType<QuestSystem>();
		if (questSystem != null)
		{
			UnityEngine.Object.DontDestroyOnLoad(questSystem.gameObject);
			return questSystem;
		}
		GameObject gameObject = new GameObject("Rilisoft.QuestSystem");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		return gameObject.AddComponent<QuestSystem>();
	}
}
