using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class ProgressSynchronizer
	{
		public const string Filename = "Progress";

		private static ProgressSynchronizer _instance;

		public static ProgressSynchronizer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ProgressSynchronizer();
				}
				return _instance;
			}
		}

		public void SynchronizeAmazonProgress()
		{
			if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android || Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
			{
				Debug.LogWarning("SynchronizeAmazonProgress() is not implemented for current target.");
				return;
			}
			AGSWhispersyncClient.Synchronize();
			using (AGSGameDataMap aGSGameDataMap = AGSWhispersyncClient.GetGameData())
			{
				if (aGSGameDataMap == null)
				{
					Debug.LogWarning("dataMap == null");
					return;
				}
				using (AGSGameDataMap aGSGameDataMap2 = aGSGameDataMap.GetMap("progressMap"))
				{
					if (aGSGameDataMap2 == null)
					{
						Debug.LogWarning("syncableProgressMap == null");
						return;
					}
					string[] array = (from k in aGSGameDataMap2.GetMapKeys()
						where !string.IsNullOrEmpty(k)
						select k).ToArray();
					string message = string.Format("Trying to sync progress.    Local: {0}    Cloud keys: {1}", CampaignProgress.GetCampaignProgressString(), Json.Serialize(array));
					Debug.Log(message);
					string[] array2 = array;
					foreach (string text in array2)
					{
						Dictionary<string, int> value;
						if (!CampaignProgress.boxesLevelsAndStars.TryGetValue(text, out value))
						{
							Debug.LogWarning("boxesLevelsAndStars doesn't contain “" + text + "”");
							value = new Dictionary<string, int>();
							CampaignProgress.boxesLevelsAndStars.Add(text, value);
						}
						else if (value == null)
						{
							Debug.LogWarning("localBox == null");
							value = new Dictionary<string, int>();
							CampaignProgress.boxesLevelsAndStars[text] = value;
						}
						using (AGSGameDataMap aGSGameDataMap3 = aGSGameDataMap2.GetMap(text))
						{
							if (aGSGameDataMap3 == null)
							{
								Debug.LogWarning("boxMap == null");
								continue;
							}
							string[] array3 = aGSGameDataMap3.GetHighestNumberKeys().ToArray();
							string message2 = string.Format("“{0}” levels: {1}", text, Json.Serialize(array3));
							Debug.Log(message2);
							string[] array4 = array3;
							foreach (string text2 in array4)
							{
								using (AGSSyncableNumber aGSSyncableNumber = aGSGameDataMap3.GetHighestNumber(text2))
								{
									if (aGSSyncableNumber == null)
									{
										Debug.LogWarning("syncableCloudValue == null");
										continue;
									}
									if (Debug.isDebugBuild)
									{
										Debug.Log("Synchronizing from cloud “" + text2 + "”...");
									}
									int num = aGSSyncableNumber.AsInt();
									int value2 = 0;
									if (value.TryGetValue(text2, out value2))
									{
										value[text2] = Math.Max(value2, num);
									}
									else
									{
										value.Add(text2, num);
									}
									if (Debug.isDebugBuild)
									{
										Debug.Log("Synchronized from cloud “" + text2 + "”...");
									}
								}
							}
						}
					}
					CampaignProgress.SaveCampaignProgress();
					Debug.Log("Trying to sync progress.    Merged: " + CampaignProgress.GetCampaignProgressString());
					foreach (KeyValuePair<string, Dictionary<string, int>> boxesLevelsAndStar in CampaignProgress.boxesLevelsAndStars)
					{
						if (Debug.isDebugBuild)
						{
							string message3 = string.Format("Synchronizing to cloud: “{0}”", boxesLevelsAndStar);
							Debug.Log(message3);
						}
						using (AGSGameDataMap aGSGameDataMap4 = aGSGameDataMap2.GetMap(boxesLevelsAndStar.Key))
						{
							if (aGSGameDataMap4 == null)
							{
								Debug.LogWarning("boxMap == null");
								continue;
							}
							Dictionary<string, int> dictionary = boxesLevelsAndStar.Value ?? new Dictionary<string, int>();
							foreach (KeyValuePair<string, int> item in dictionary)
							{
								using (AGSSyncableNumber aGSSyncableNumber2 = aGSGameDataMap4.GetHighestNumber(item.Key))
								{
									if (aGSSyncableNumber2 == null)
									{
										Debug.LogWarning("syncableCloudValue == null");
									}
									else
									{
										aGSSyncableNumber2.Set(item.Value);
									}
								}
							}
						}
					}
					AGSWhispersyncClient.Synchronize();
				}
			}
		}

		public void AuthenticateAndSynchronize(Action callback, bool silent)
		{
			if (GpgFacade.Instance.IsAuthenticated())
			{
				Debug.LogFormat("Already authenticated: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
				Instance.SynchronizeIfAuthenticated(callback);
				return;
			}
			Action<bool> callback2 = delegate(bool succeeded)
			{
				bool value = !silent && !succeeded;
				PlayerPrefs.SetInt("GoogleSignInDenied", Convert.ToInt32(value));
				if (succeeded)
				{
					string message = string.Format("Authentication succeeded: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
					Debug.Log(message);
					Instance.SynchronizeIfAuthenticated(callback);
				}
				else if (!Application.isEditor)
				{
					Debug.LogWarning("Authentication failed.");
				}
			};
			GpgFacade.Instance.Authenticate(callback2, silent);
		}

		private void SynchronizeIfAuthenticatedWithSavedGamesService(Action callback)
		{
			// um here was sync code lol
		}

		private static void MergeUpdateLocalProgress(IDictionary<string, Dictionary<string, int>> incomingProgress)
		{
			foreach (KeyValuePair<string, Dictionary<string, int>> item in incomingProgress)
			{
				Dictionary<string, int> value;
				if (CampaignProgress.boxesLevelsAndStars.TryGetValue(item.Key, out value))
				{
					foreach (KeyValuePair<string, int> item2 in item.Value)
					{
						int value2;
						if (value.TryGetValue(item2.Key, out value2))
						{
							value[item2.Key] = Math.Max(value2, item2.Value);
						}
						else
						{
							value.Add(item2.Key, item2.Value);
						}
					}
				}
				else
				{
					CampaignProgress.boxesLevelsAndStars.Add(item.Key, item.Value);
				}
			}
			CampaignProgress.SaveCampaignProgress();
		}

		public void SynchronizeIfAuthenticated(Action callback)
		{
			if (!GpgFacade.Instance.IsAuthenticated())
			{
				return;
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			using (new StopwatchLogger("SynchronizeIfAuthenticated(...)"))
			{
				SynchronizeIfAuthenticatedWithSavedGamesService(callback);
			}
		}
	}
}
