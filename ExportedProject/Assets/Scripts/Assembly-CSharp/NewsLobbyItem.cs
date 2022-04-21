using System;
using System.Collections;
using System.IO;
using Rilisoft;
using UnityEngine;

public class NewsLobbyItem : MonoBehaviour
{
	public GameObject indicatorNew;

	public UILabel headerLabel;

	public UILabel shortDescLabel;

	public UILabel dateLabel;

	public UITexture previewPic;

	public string previewPicUrl;

	public void LoadPreview(string url)
	{
		StartCoroutine(LoadPreviewPicture(url));
	}

	private IEnumerator LoadPreviewPicture(string picLink)
	{
		if (previewPic.mainTexture != null && previewPicUrl == picLink)
		{
			yield break;
		}
		previewPic.width = 100;
		if (previewPic.mainTexture != null)
		{
			UnityEngine.Object.Destroy(previewPic.mainTexture);
		}
		string cachePath = PersistentCache.Instance.GetCachePathByUri(picLink);
		if (!string.IsNullOrEmpty(cachePath))
		{
			try
			{
				bool cacheExists = File.Exists(cachePath);
				if (Defs.IsDeveloperBuild)
				{
					string formattedPath = ((!Application.isEditor) ? cachePath : ("<color=magenta>" + cachePath + "</color>"));
					Debug.LogFormat("Trying to load preview from cache '{0}': {1}", formattedPath, cacheExists);
				}
				if (cacheExists)
				{
					byte[] cacheBytes2 = File.ReadAllBytes(cachePath);
					Texture2D cachedTexture = new Texture2D(2, 2);
					cachedTexture.LoadImage(cacheBytes2);
					cachedTexture.filterMode = FilterMode.Point;
					previewPicUrl = picLink;
					previewPic.mainTexture = cachedTexture;
					previewPic.mainTexture.filterMode = FilterMode.Point;
					previewPic.width = 100;
					yield break;
				}
			}
			catch (Exception ex3)
			{
				Exception ex2 = ex3;
				Debug.LogWarning("Caught exception while reading cached preview. See next message for details.");
				Debug.LogException(ex2);
			}
		}
		WWW loadPic = Tools.CreateWwwIfNotConnected(picLink);
		if (loadPic == null)
		{
			yield return new WaitForSeconds(60f);
			StartCoroutine(LoadPreviewPicture(picLink));
			yield break;
		}
		yield return loadPic;
		if (!string.IsNullOrEmpty(loadPic.error))
		{
			Debug.LogWarning("Download preview pic error: " + loadPic.error);
			if (loadPic.error.StartsWith("Resolving host timed out"))
			{
				yield return new WaitForSeconds(1f);
				if (Application.isEditor && FriendsController.isDebugLogWWW)
				{
					Debug.Log("Reloading timed out pic");
				}
				StartCoroutine(LoadPreviewPicture(picLink));
			}
			yield break;
		}
		previewPicUrl = picLink;
		previewPic.mainTexture = loadPic.texture;
		previewPic.mainTexture.filterMode = FilterMode.Point;
		previewPic.width = 100;
		if (string.IsNullOrEmpty(cachePath))
		{
			yield break;
		}
		try
		{
			if (Defs.IsDeveloperBuild)
			{
				string formattedPath2 = ((!Application.isEditor) ? cachePath : ("<color=magenta>" + cachePath + "</color>"));
				Debug.LogFormat("Trying to save preview to cache '{0}'", formattedPath2);
			}
			string directoryPath = Path.GetDirectoryName(cachePath);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
			byte[] cacheBytes = loadPic.texture.EncodeToPNG();
			File.WriteAllBytes(cachePath, cacheBytes);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Caught exception while saving preview to cache. See next message for details.");
			Debug.LogException(ex);
		}
	}
}
