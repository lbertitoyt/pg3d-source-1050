using System;
using System.IO;
using System.Net;
using UnityEngine;

internal sealed class InternetChecker : MonoBehaviour
{
	public static bool InternetAvailable;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void CheckForInternetConn()
	{
		string htmlFromUri = GetHtmlFromUri("http://google.com");
		if (htmlFromUri == string.Empty)
		{
			InternetAvailable = false;
		}
		else if (!htmlFromUri.Contains("schema.org/WebPage"))
		{
			InternetAvailable = false;
		}
		else
		{
			InternetAvailable = true;
		}
	}

	public static string GetHtmlFromUri(string resource)
	{
		//Discarded unreachable code: IL_00e1
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		string text = string.Empty;
		HttpWebRequest val = (HttpWebRequest)WebRequest.Create(resource);
		try
		{
			HttpWebResponse val2 = (HttpWebResponse)val.GetResponse();
			try
			{
				if ((int)val2.get_StatusCode() < 299 && (int)val2.get_StatusCode() >= 200)
				{
					Debug.Log("Trying to check internet");
					using (StreamReader streamReader = new StreamReader(val2.GetResponseStream()))
					{
						char[] array = new char[80];
						streamReader.Read(array, 0, array.Length);
						char[] array2 = array;
						foreach (char c in array2)
						{
							text += c;
						}
						return text;
					}
				}
				return text;
			}
			finally
			{
				if (val2 != null)
				{
					((IDisposable)val2).Dispose();
				}
			}
		}
		catch
		{
			return string.Empty;
		}
	}
}
