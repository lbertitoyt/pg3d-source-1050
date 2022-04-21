using System;
using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
	private static CoroutineRunner _instance;

	public static CoroutineRunner Instance
	{
		get
		{
			if (_instance == null)
			{
				try
				{
					GameObject gameObject = new GameObject("CoroutineRunner");
					_instance = gameObject.AddComponent<CoroutineRunner>();
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
				catch (Exception ex)
				{
					Debug.LogError("[Rilisoft] CoroutineRunner: Instance exception: " + ex);
				}
			}
			return _instance;
		}
	}

	public static IEnumerator WaitForSeconds(float tm)
	{
		float startTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}
}
