using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rilisoft
{
	public class SceneLoader : Singleton<SceneLoader>
	{
		public const string SCENE_INFOS_ASSET_PATH = "Assets/Resources/ScenesList.asset";

		[SerializeField]
		private ScenesList _scenesList;

		[SerializeField]
		private List<SceneLoadInfo> _loadingHistory = new List<SceneLoadInfo>();

		public Action<SceneLoadInfo> OnSceneLoading = delegate
		{
		};

		public Action<SceneLoadInfo> OnSceneLoaded = delegate
		{
		};

		public static string ActiveSceneName
		{
			get
			{
				return SceneManager.GetActiveScene().name ?? string.Empty;
			}
		}

		private void OnInstanceCreated()
		{
			if (_scenesList == null)
			{
				throw new Exception("scenes list is null");
			}
			IGrouping<string, ExistsSceneInfo>[] source = (from i in _scenesList.Infos
				group i by i.Name into g
				where g.Count() > 1
				select g).ToArray();
			if (source.Any())
			{
				string text = source.Select((IGrouping<string, ExistsSceneInfo> g) => g.Key).Aggregate((string cur, string next) => string.Format("{0},{1}{2}", cur, next, Environment.NewLine));
				Debug.LogError("[SCENELOADER] duplicate scenes: " + text);
			}
			else
			{
				OnSceneLoaded = (Action<SceneLoadInfo>)Delegate.Combine(OnSceneLoaded, new Action<SceneLoadInfo>(_loadingHistory.Add));
			}
		}

		public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
		{
			ExistsSceneInfo sceneInfo = GetSceneInfo(sceneName);
			SceneLoadInfo sceneLoadInfo = default(SceneLoadInfo);
			sceneLoadInfo.SceneName = sceneInfo.Name;
			sceneLoadInfo.LoadMode = mode;
			SceneLoadInfo obj = sceneLoadInfo;
			OnSceneLoading(obj);
			SceneManager.LoadScene(sceneName, mode);
			OnSceneLoaded(obj);
		}

		public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
		{
			ExistsSceneInfo sceneInfo = GetSceneInfo(sceneName);
			SceneLoadInfo sceneLoadInfo = default(SceneLoadInfo);
			sceneLoadInfo.SceneName = sceneInfo.Name;
			sceneLoadInfo.LoadMode = mode;
			SceneLoadInfo sceneLoadInfo2 = sceneLoadInfo;
			OnSceneLoading(sceneLoadInfo2);
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneInfo.Name, mode);
			Singleton<SceneLoader>.Instance.StartCoroutine(WaitSceneIsLoaded(sceneLoadInfo2, asyncOperation));
			return asyncOperation;
		}

		public ExistsSceneInfo GetSceneInfo(string sceneName)
		{
			ExistsSceneInfo existsSceneInfo = ((!string.IsNullOrEmpty(Path.GetDirectoryName(sceneName))) ? _scenesList.Infos.FirstOrDefault((ExistsSceneInfo i) => i.Path == sceneName) : _scenesList.Infos.FirstOrDefault((ExistsSceneInfo i) => i.Name == sceneName));
			if (existsSceneInfo == null)
			{
				throw new ArgumentException(string.Format("Unknown scene : '{0}'", sceneName));
			}
			return existsSceneInfo;
		}

		private IEnumerator WaitSceneIsLoaded(SceneLoadInfo loadInfo, AsyncOperation op)
		{
			while (!op.isDone)
			{
				yield return null;
			}
			OnSceneLoaded(loadInfo);
		}
	}
}
