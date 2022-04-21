using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public static class Storager
{
	private const bool useCryptoPlayerPrefs = true;

	private const bool _useSignedPreferences = true;

	private static bool iCloudAvailable;

	private static IDictionary<string, SaltedInt> _keychainCache;

	private static IDictionary<string, string> _keychainStringCache;

	private static Dictionary<string, int> iosCloudSyncBuffer;

	private static bool _weaponDigestIsDirty;

	private static readonly IDictionary<string, SaltedInt> _protectedIntCache;

	private static readonly System.Random _prng;

	private static readonly string[] _expendableKeys;

	public static bool UseSignedPreferences
	{
		get
		{
			return true;
		}
	}

	static Storager()
	{
		iCloudAvailable = false;
		_keychainCache = new Dictionary<string, SaltedInt>();
		_keychainStringCache = new Dictionary<string, string>();
		iosCloudSyncBuffer = new Dictionary<string, int>();
		_protectedIntCache = new Dictionary<string, SaltedInt>();
		_prng = new System.Random();
		_expendableKeys = new string[4]
		{
			GearManager.InvisibilityPotion,
			GearManager.Jetpack,
			GearManager.Turret,
			GearManager.Mech
		};
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer)
		{
			return;
		}
		IEnumerable<string> enumerable = PurchasesSynchronizer.AllItemIds();
		foreach (string item in enumerable)
		{
			iosCloudSyncBuffer.Add(item, 0);
		}
	}

	public static void SynchronizeIosWithCloud(ref List<string> weaponsForWhichSetRememberedTier, out bool armorArmy1Comes)
	{
		armorArmy1Comes = false;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer && !iCloudAvailable)
		{
		}
	}

	public static void Initialize(bool cloudAvailable)
	{
	}

	public static bool hasKey(string key)
	{
		bool flag = CryptoPlayerPrefs.HasKey(key);
		string value;
		int result;
		if (!flag && (key.Equals("Coins") || key.Equals("GemsCurrency")) && Defs2.SignedPreferences.TryGetValue(key, out value) && Defs2.SignedPreferences.Verify(key) && int.TryParse(value, out result))
		{
			setInt(key, Math.Max(0, result), false);
			return true;
		}
		return flag;
	}

	public static void setInt(string key, int val, bool useICloud)
	{
		if (Application.isEditor)
		{
			PlayerPrefs.SetInt(key, val);
		}
		else
		{
			CryptoPlayerPrefs.SetInt(key, val);
			_protectedIntCache[key] = new SaltedInt(_prng.Next(), val);
			if (key.Equals("Coins") || key.Equals("GemsCurrency"))
			{
				Defs2.SignedPreferences.Add(key, val.ToString());
			}
		}
		if (key.Equals("Coins") || key.Equals("GemsCurrency"))
		{
			DigestStorager.Instance.Set(key, val);
		}
		if (_expendableKeys.Contains(key))
		{
			RefreshExpendablesDigest();
		}
		if (WeaponManager.PurchasableWeaponSetContains(key))
		{
			_weaponDigestIsDirty = true;
		}
	}

	public static int getInt(string key, bool useICloud)
	{
		if (Application.isEditor)
		{
			return PlayerPrefs.GetInt(key);
		}
		SaltedInt value;
		if (_protectedIntCache.TryGetValue(key, out value))
		{
			return value.Value;
		}
		if (CryptoPlayerPrefs.HasKey(key))
		{
			int @int = CryptoPlayerPrefs.GetInt(key);
			_protectedIntCache.Add(key, new SaltedInt(_prng.Next(), @int));
			return @int;
		}
		string value2;
		int result;
		if ((key.Equals("Coins") || key.Equals("GemsCurrency")) && Defs2.SignedPreferences.TryGetValue(key, out value2) && Defs2.SignedPreferences.Verify(key) && int.TryParse(value2, out result))
		{
			return result;
		}
		return 0;
	}

	public static void setString(string key, string val, bool useICloud)
	{
		if (Application.isEditor)
		{
			PlayerPrefs.SetString(key, val);
			return;
		}
		CryptoPlayerPrefs.SetString(key, val);
		_keychainStringCache[key] = val;
	}

	public static string getString(string key, bool useICloud)
	{
		if (Application.isEditor)
		{
			return PlayerPrefs.GetString(key);
		}
		string value;
		if (_keychainStringCache.TryGetValue(key, out value))
		{
			return value;
		}
		if (CryptoPlayerPrefs.HasKey(key))
		{
			string @string = CryptoPlayerPrefs.GetString(key, string.Empty);
			_keychainStringCache.Add(key, @string);
			return @string;
		}
		return string.Empty;
	}

	public static bool IsInitialized(string flagName)
	{
		if (Application.isEditor)
		{
			return PlayerPrefs.HasKey(flagName);
		}
		return hasKey(flagName);
	}

	public static void SetInitialized(string flagName)
	{
		setInt(flagName, 0, false);
	}

	public static void SyncWithCloud(string storageId)
	{
		int @int = getInt(storageId, true);
		if (@int > 0)
		{
			setInt(storageId, @int, true);
		}
	}

	private static void RefreshExpendablesDigest()
	{
		byte[] value = _expendableKeys.SelectMany((string key) => BitConverter.GetBytes(getInt(key, false))).ToArray();
		DigestStorager.Instance.Set("ExpendablesCount", value);
	}

	public static void RefreshWeaponDigestIfDirty()
	{
		if (_weaponDigestIsDirty)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("[Rilisoft] > RefreshWeaponsDigest: {0:F3}", Time.realtimeSinceStartup);
			}
			RefreshWeaponsDigest();
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("[Rilisoft] < RefreshWeaponsDigest: {0:F3}", Time.realtimeSinceStartup);
			}
		}
	}

	private static void RefreshWeaponsDigest()
	{
		IEnumerable<string> source = WeaponManager.storeIDtoDefsSNMapping.Values.Where((string w) => getInt(w, false) == 1);
		int value = source.Count();
		DigestStorager.Instance.Set("WeaponsCount", value);
		_weaponDigestIsDirty = false;
	}
}
