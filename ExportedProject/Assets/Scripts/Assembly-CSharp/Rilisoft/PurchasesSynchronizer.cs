using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class PurchasesSynchronizer
	{
		public const string Filename = "Purchases";

		private readonly List<string> _itemsToBeSaved = new List<string>();

		private static PurchasesSynchronizer _instance;

		private static IEnumerable<string> _allItemIds;

		public bool HasItemsToBeSaved
		{
			get
			{
				return _itemsToBeSaved.Count > 0;
			}
		}

		public ICollection<string> ItemsToBeSaved
		{
			get
			{
				return _itemsToBeSaved;
			}
		}

		public static PurchasesSynchronizer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PurchasesSynchronizer();
				}
				return _instance;
			}
		}

		public event EventHandler<PurchasesSavingEventArgs> PurchasesSavingStarted;

		public static IEnumerable<string> AllItemIds()
		{
			if (_allItemIds == null)
			{
				Dictionary<string, string>.ValueCollection values = WeaponManager.storeIDtoDefsSNMapping.Values;
				List<string> list = new List<string>();
				foreach (KeyValuePair<ShopNGUIController.CategoryNames, List<List<string>>> item in Wear.wear)
				{
					foreach (List<string> item2 in item.Value)
					{
						list.AddRange(item2);
					}
				}
				IEnumerable<string> second = InAppData.inAppData.Values.Select((KeyValuePair<string, string> kv) => kv.Value);
				IEnumerable<string> second2 = from i in Enumerable.Range(1, ExperienceController.maxLevel)
					select "currentLevel" + i;
				string[] second3 = new string[6]
				{
					Defs.SkinsMakerInProfileBought,
					Defs.hungerGamesPurchasedKey,
					Defs.CaptureFlagPurchasedKey,
					Defs.smallAsAntKey,
					Defs.code010110_Key,
					Defs.UnderwaterKey
				};
				string[] second4 = new string[1] { "PayingUser" };
				string[] second5 = new string[1] { Defs.IsFacebookLoginRewardaGained };
				string[] second6 = new string[1] { Defs.IsTwitterLoginRewardaGained };
				_allItemIds = values.Concat(list).Concat(second).Concat(second2)
					.Concat(second3)
					.Concat(second4)
					.Concat(second5)
					.Concat(second6)
					.Concat(WeaponManager.GotchaGuns);
			}
			return _allItemIds;
		}

		public static IEnumerable<string> GetPurchasesIds()
		{
			IEnumerable<string> source = AllItemIds();
			return source.Where((string id) => Storager.getInt(id, false) != 0);
		}

		public void SynchronizeAmazonPurchases()
		{
			if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android || Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
			{
				Debug.LogWarning("SynchronizeAmazonPurchases() is not implemented for current target.");
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
				using (AGSSyncableStringSet aGSSyncableStringSet = aGSGameDataMap.GetStringSet("purchases"))
				{
					List<string> list = (from s in aGSSyncableStringSet.GetValues()
						select s.GetValue()).ToList();
					Debug.Log("Trying to sync purchases cloud -> local:    " + Json.Serialize(list));
					List<string> list2 = new List<string>();
					foreach (string item in list)
					{
						if (Storager.getInt(item, false) == 0 && (item == Defs.IsFacebookLoginRewardaGained || WeaponManager.GotchaGuns.Contains(item)))
						{
							list2.Add(item);
						}
						_itemsToBeSaved.Add(item);
					}
					string[] array = GetPurchasesIds().ToArray();
					Debug.Log("Trying to sync purchases local -> cloud:    " + Json.Serialize(array));
					string[] array2 = array;
					foreach (string val in array2)
					{
						aGSSyncableStringSet.Add(val);
					}
					AGSWhispersyncClient.Synchronize();
					WeaponManager.SetRememberedTiersForWeaponsComesFromCloud(list2);
				}
			}
		}

		public void AuthenticateAndSynchronize(Action<bool> callback, bool silent)
		{
			if (GpgFacade.Instance.IsAuthenticated())
			{
				Debug.LogFormat("Already authenticated: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
				return;
			}
			GpgFacade.Instance.Authenticate(delegate(bool succeeded)
			{
				bool value = !silent && !succeeded;
				PlayerPrefs.SetInt("GoogleSignInDenied", Convert.ToInt32(value));
				if (succeeded)
				{
					string message = string.Format("Authentication succeeded: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
					Debug.Log(message);
				}
				else
				{
					Debug.LogWarning("Authentication failed.");
				}
			}, silent);
		}

		private void HandleReadBinaryData()
		{
		} 

		private void SynchronizeIfAuthenticatedWithSavedGamesService(Action<bool> callback)
		{
						PlayerPrefs.Save();
						int levelBefore = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
						WeaponManager.RefreshExpControllers();
						ExperienceController.SendAnalyticsForLevelsFromCloud(levelBefore);
						HashSet<string> source = new HashSet<string>(GetPurchasesIds());
						string outputString = Json.Serialize(source.ToArray());
						byte[] bytes = Encoding.UTF8.GetBytes(outputString);
						string description = string.Format("Merged by '{0}': '{1}' and '{2}'", SystemInfo.deviceModel);
		} 
	}
}
