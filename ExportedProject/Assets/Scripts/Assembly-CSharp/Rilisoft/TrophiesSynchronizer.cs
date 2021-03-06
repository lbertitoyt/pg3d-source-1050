using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class TrophiesSynchronizer : MonoBehaviour
	{
		private const string TrophiesNegativeKey = "RatingNegative";

		private const string TrophiesPositiveKey = "RatingPositive";

		private static readonly TrophiesSynchronizer _instance = new TrophiesSynchronizer();

		public static TrophiesSynchronizer Instance
		{
			get
			{
				return _instance;
			}
		}

		private bool Ready
		{
			get
			{
				return true;
			}
		}

		public event EventHandler Updated;

		private TrophiesSynchronizer()
		{
		}

		public Coroutine Pull()
		{
			if (!Ready)
			{
				return null;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				return CoroutineRunner.Instance.StartCoroutine(SyncGoogleCoroutine(true));
			}
			return null;
		}

		public Coroutine Push()
		{
			if (!Ready)
			{
				return null;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
			}
			return null;
		}

		public Coroutine Sync()
		{
			if (!Ready)
			{
				return null;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				return CoroutineRunner.Instance.StartCoroutine(SyncGoogleCoroutine(false));
			}
			return null;
		}

		internal IEnumerator SyncGoogleCoroutine(bool pullOnly)
		{
			if (!Ready)
			{
				yield break;
			}
			string thisName = string.Format(CultureInfo.InvariantCulture, "TrophiesSynchronizer.PullGoogleCoroutine('{0}')", (!pullOnly) ? "sync" : "pull");
			ScopeLogger scopeLogger = new ScopeLogger(thisName, Defs.IsDeveloperBuild && !Application.isEditor);
				WaitForSeconds delay = new WaitForSeconds(30f);
				int i = 0;
				while (true)
				{
					string callee = string.Format(CultureInfo.InvariantCulture, "Pull and wait ({0})", i);
					using (ScopeLogger logger = new ScopeLogger(thisName, callee, Defs.IsDeveloperBuild && !Application.isEditor))
					{
						ScopeLogger scopeLogger2 = new ScopeLogger("TrophiesSynchronizer.PullGoogleCoroutine()", "PushGoogleCoroutine(conflict)", Defs.IsDeveloperBuild && !Application.isEditor);
							scopeLogger2.Dispose();
					}
					Debug.LogWarning("Failed to push trophies with status: null coz i deleted the code lol");
					yield return delay;
				}
		}
	}

		/*internal IEnumerator PushGoogleCoroutine()
		{
			ScopeLogger scopeLogger = new ScopeLogger("TrophiesSynchronizer.PushGoogleCoroutine()", Defs.IsDeveloperBuild);
			try
			{
				WaitForSeconds delay = new WaitForSeconds(30f);
				int i = 0;
				while (true)
				{
					int trophiesNegative = Storager.getInt("RatingNegative", false);
					int trophiesPositive = Storager.getInt("RatingPositive", false);
					TrophiesMemento localTrophies = new TrophiesMemento(trophiesNegative, trophiesPositive);
					string callee = string.Format(CultureInfo.InvariantCulture, "Push and wait {0} ({1})", localTrophies, i);
					using (ScopeLogger logger = new ScopeLogger("TrophiesSynchronizer.PushGoogleCoroutine()", callee, Defs.IsDeveloperBuild))
					{
						System.Threading.Tasks.Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> future = googleSavedGamesFacade.Push(localTrophies);
						while (!((System.Threading.Tasks.Task)future).get_IsCompleted())
						{
							yield return null;
						}
						logger.Dispose();
						if (((System.Threading.Tasks.Task)future).get_IsFaulted())
						{
							Exception ex = (Exception)(((object)((System.Threading.Tasks.Task)future).get_Exception().get_InnerExceptions().FirstOrDefault()) ?? ((object)((System.Threading.Tasks.Task)future).get_Exception()));
							Debug.LogWarning("Failed to push trophies with exception: " + ex.Message);
							yield return delay;
						}
						else
						{
							SavedGameRequestStatus requestStatus = future.get_Result().RequestStatus;
							if (requestStatus == SavedGameRequestStatus.Success)
							{
								if (Defs.IsDeveloperBuild)
								{
									ISavedGameMetadata metadata = future.get_Result().Value;
									string description = ((metadata == null) ? string.Empty : metadata.Description);
									Debug.LogFormat("[Trophies] Succeeded to push trophies with status: '{0}'", description);
								}
								break;
							}
							Debug.LogWarning("Failed to push trophies with status: " + requestStatus);
							yield return delay;
						}
					}
					i++;
				} 
			}
			finally
			{
				scopeLogger.Dispose();
			}
		} 
	} */
}
