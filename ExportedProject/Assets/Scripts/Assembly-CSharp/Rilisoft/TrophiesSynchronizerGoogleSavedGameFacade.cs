/*using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	public struct TrophiesSynchronizerGoogleSavedGameFacade
	{
		private abstract class Callback
		{
			protected TrophiesMemento? _resolvedTrophies;

			internal abstract void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata);

			protected abstract void HandleAuthenticationCompleted(bool succeeded);

			protected abstract void TrySetException(Exception ex);

			internal void HandleOpenConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
			{
				//Discarded unreachable code: IL_01a5
				string callee = string.Format(CultureInfo.InstalledUICulture, "{0}.HandleOpenConflict('{1}', '{2}')", GetType().Name, original.Description, unmerged.Description);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
						return;
					}
					TrophiesMemento trophiesMemento = ParseTrophies(originalData);
					TrophiesMemento trophiesMemento2 = ParseTrophies(unmergedData);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("[Trophies] Original: {0}, unmerged: {1}", trophiesMemento, trophiesMemento2);
					}
					if (trophiesMemento.TrophiesNegative >= trophiesMemento2.TrophiesNegative && trophiesMemento.TrophiesPositive >= trophiesMemento2.TrophiesPositive)
					{
						resolver.ChooseMetadata(original);
						_resolvedTrophies = CombineWithResolved(trophiesMemento, false);
					}
					else if (trophiesMemento.TrophiesNegative <= trophiesMemento2.TrophiesNegative && trophiesMemento.TrophiesPositive <= trophiesMemento2.TrophiesPositive)
					{
						resolver.ChooseMetadata(unmerged);
						_resolvedTrophies = CombineWithResolved(trophiesMemento2, false);
					}
					else if (trophiesMemento.Trophies >= trophiesMemento2.Trophies)
					{
						resolver.ChooseMetadata(original);
						_resolvedTrophies = CombineWithResolved(trophiesMemento, true);
					}
					else
					{
						resolver.ChooseMetadata(unmerged);
						_resolvedTrophies = CombineWithResolved(trophiesMemento2, true);
					}
					SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, HandleOpenConflict, HandleOpenCompleted);
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected static TrophiesMemento Combine(TrophiesMemento left, TrophiesMemento right)
			{
				int trophiesNegative = Math.Max(left.TrophiesNegative, right.TrophiesNegative);
				int trophiesPositive = Math.Max(left.TrophiesPositive, right.TrophiesPositive);
				bool conflicted = left.Conflicted || right.Conflicted;
				return new TrophiesMemento(trophiesNegative, trophiesPositive, conflicted);
			}

			protected TrophiesMemento CombineWithResolved(TrophiesMemento trophies, bool forceConflicted)
			{
				TrophiesMemento result = ((!_resolvedTrophies.HasValue) ? trophies : Combine(_resolvedTrophies.Value, trophies));
				if (forceConflicted)
				{
					return new TrophiesMemento(result.TrophiesNegative, result.TrophiesPositive, true);
				}
				return result;
			}

			protected static TrophiesMemento ParseTrophies(byte[] data)
			{
				if (data != null && data.Length > 0)
				{
					string @string = Encoding.UTF8.GetString(data);
					return JsonUtility.FromJson<TrophiesMemento>(@string);
				}
				return default(TrophiesMemento);
			}
		}

		private sealed class PushCallback : Callback
		{
			private readonly TrophiesMemento _trophies;

			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> _promise;

			public PushCallback(TrophiesMemento trophies, TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> promise)
			{
				_trophies = trophies;
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
			}

			internal override void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleOpenCompleted('{1}', '{2}')", GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
						return;
					}
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						TrophiesMemento trophiesMemento = CombineWithResolved(_trophies, false);
						string text2 = (trophiesMemento.Conflicted ? "resolved" : ((!_resolvedTrophies.HasValue) ? "none" : "trivial"));
						string description = string.Format(CultureInfo.InvariantCulture, "device:'{0}', conflict:'{1}'", SystemInfo.deviceModel, text2);
						SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(description).Build();
						string s = JsonUtility.ToJson(trophiesMemento);
						byte[] bytes = Encoding.UTF8.GetBytes(s);
						SavedGame.CommitUpdate(metadata, updateForMetadata, bytes, HandleCommitCompleted);
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<ISavedGameMetadata>(requestStatus, metadata));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleAuthenticationCompleted({1})", GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<ISavedGameMetadata>(SavedGameRequestStatus.AuthenticationError, null));
					}
					else if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected override void TrySetException(Exception ex)
			{
				_promise.TrySetException(ex);
			}

			private void HandleCommitCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleCommitCompleted('{1}', '{2}')", GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.TimeoutError:
						if (SavedGame == null)
						{
							TrySetException(new InvalidOperationException("SavedGameClient is null."));
						}
						else
						{
							SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						}
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
					{
						GoogleSavedGameRequestResult<ISavedGameMetadata> googleSavedGameRequestResult = new GoogleSavedGameRequestResult<ISavedGameMetadata>(requestStatus, metadata);
						_promise.TrySetResult(googleSavedGameRequestResult);
						break;
					}
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}
		}

		private sealed class PullCallback : Callback
		{
			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>> _promise;

			public PullCallback(TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>> promise)
			{
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>>();
			}

			internal override void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleOpenCompleted('{1}', '{2}')", GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
						return;
					}
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
						SavedGame.ReadBinaryData(metadata, HandleReadCompleted);
						break;
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<TrophiesMemento>(requestStatus, default(TrophiesMemento)));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleAuthenticationCompleted({1})", GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<TrophiesMemento>(SavedGameRequestStatus.AuthenticationError, default(TrophiesMemento)));
					}
					else if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected override void TrySetException(Exception ex)
			{
				_promise.TrySetException(ex);
			}

			private void HandleReadCompleted(SavedGameRequestStatus requestStatus, byte[] data)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleReadCompleted('{1}', {2})", GetType().Name, requestStatus, (data != null) ? data.Length : 0);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						TrophiesMemento trophiesMemento = Callback.ParseTrophies(data);
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("[Trophies] Incoming: {0}", trophiesMemento);
						}
						TrophiesMemento value = CombineWithResolved(trophiesMemento, false);
						_promise.TrySetResult(new GoogleSavedGameRequestResult<TrophiesMemento>(requestStatus, value));
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						if (SavedGame == null)
						{
							TrySetException(new InvalidOperationException("SavedGameClient is null."));
						}
						else
						{
							SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						}
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<TrophiesMemento>(requestStatus, default(TrophiesMemento)));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}
		}

		public const string Filename = "Trophies";

		private const string SavedGameClientIsNullMessage = "SavedGameClient is null.";

		private static ISavedGameClient SavedGame
		{
			get
			{
				//Discarded unreachable code: IL_0021, IL_002e
				try
				{
					if (PlayGamesPlatform.Instance == null)
					{
						return null;
					}
					return PlayGamesPlatform.Instance.SavedGame;
				}
				catch (NullReferenceException)
				{
					return null;
				}
			}
		}

		public System.Threading.Tasks.Task<GoogleSavedGameRequestResult<TrophiesMemento>> Pull()
		{
			//Discarded unreachable code: IL_00b8
			string text = GetType().Name + ".Pull()";
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>> val = new TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>>();
				PullCallback pullCallback = new PullCallback(val);
				if (SavedGame == null)
				{
					val.TrySetException((Exception)new InvalidOperationException("SavedGameClient is null."));
					return val.get_Task();
				}
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild);
				try
				{
					SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, pullCallback.HandleOpenConflict, pullCallback.HandleOpenCompleted);
				}
				finally
				{
					scopeLogger2.Dispose();
				}
				return val.get_Task();
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		public System.Threading.Tasks.Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> Push(TrophiesMemento trophies)
		{
			//Discarded unreachable code: IL_00d0
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.Push({1})", GetType().Name, trophies);
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> val = new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
				PushCallback pushCallback = new PushCallback(trophies, val);
				if (SavedGame == null)
				{
					val.TrySetException((Exception)new InvalidOperationException("SavedGameClient is null."));
					return val.get_Task();
				}
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild);
				try
				{
					SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, pushCallback.HandleOpenConflict, pushCallback.HandleOpenCompleted);
				}
				finally
				{
					scopeLogger2.Dispose();
				}
				return val.get_Task();
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}
	}
}
*/