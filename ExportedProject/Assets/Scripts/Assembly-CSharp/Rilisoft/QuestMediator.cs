using System;

namespace Rilisoft
{
	internal sealed class QuestMediator
	{
		private sealed class QuestEventSource : QuestEvents
		{
			internal new void RaiseWin(WinEventArgs e)
			{
				base.RaiseWin(e);
			}

			internal new void RaiseKillOtherPlayer(KillOtherPlayerEventArgs e)
			{
				base.RaiseKillOtherPlayer(e);
			}

			internal new void RaiseKillOtherPlayerWithFlag(EventArgs e)
			{
				base.RaiseKillOtherPlayerWithFlag(e);
			}

			internal new void RaiseCapture(CaptureEventArgs e)
			{
				base.RaiseCapture(e);
			}

			internal new void RaiseKillMonster(KillMonsterEventArgs e)
			{
				base.RaiseKillMonster(e);
			}

			internal new void RaiseBreakSeries(EventArgs e)
			{
				base.RaiseBreakSeries(e);
			}

			internal new void RaiseMakeSeries(EventArgs e)
			{
				base.RaiseMakeSeries(e);
			}

			internal new void RaiseSurviveWaveInArena(EventArgs e)
			{
				base.RaiseSurviveWaveInArena(e);
			}

			internal new void RaiseGetGotcha(EventArgs e)
			{
				base.RaiseGetGotcha(e);
			}

			internal new void RaiseSocialInteraction(SocialInteractionEventArgs e)
			{
				base.RaiseSocialInteraction(e);
			}
		}

		private static readonly QuestEventSource _eventSource = new QuestEventSource();

		public static QuestEvents Events
		{
			get
			{
				return _eventSource;
			}
		}

		public static void NotifyWin(ConnectSceneNGUIController.RegimGame mode, string map)
		{
			WinEventArgs winEventArgs = new WinEventArgs();
			winEventArgs.Mode = mode;
			winEventArgs.Map = map ?? string.Empty;
			WinEventArgs e = winEventArgs;
			_eventSource.RaiseWin(e);
		}

		public static void NotifyKillOtherPlayer(ConnectSceneNGUIController.RegimGame mode, ShopNGUIController.CategoryNames weaponSlot, bool headshot = false, bool grenade = false, bool revenge = false)
		{
			KillOtherPlayerEventArgs killOtherPlayerEventArgs = new KillOtherPlayerEventArgs();
			killOtherPlayerEventArgs.Mode = mode;
			killOtherPlayerEventArgs.WeaponSlot = weaponSlot;
			killOtherPlayerEventArgs.Headshot = headshot;
			killOtherPlayerEventArgs.Grenade = grenade;
			killOtherPlayerEventArgs.Revenge = revenge;
			KillOtherPlayerEventArgs e = killOtherPlayerEventArgs;
			_eventSource.RaiseKillOtherPlayer(e);
		}

		public static void NotifyKillOtherPlayerWithFlag()
		{
			_eventSource.RaiseKillOtherPlayerWithFlag(EventArgs.Empty);
		}

		public static void NotifyCapture(ConnectSceneNGUIController.RegimGame mode)
		{
			CaptureEventArgs captureEventArgs = new CaptureEventArgs();
			captureEventArgs.Mode = mode;
			CaptureEventArgs e = captureEventArgs;
			_eventSource.RaiseCapture(e);
		}

		public static void NotifyKillMonster(ShopNGUIController.CategoryNames weaponSlot, bool campaign = false)
		{
			KillMonsterEventArgs killMonsterEventArgs = new KillMonsterEventArgs();
			killMonsterEventArgs.WeaponSlot = weaponSlot;
			killMonsterEventArgs.Campaign = campaign;
			KillMonsterEventArgs e = killMonsterEventArgs;
			_eventSource.RaiseKillMonster(e);
		}

		public static void NotifyBreakSeries()
		{
			_eventSource.RaiseBreakSeries(EventArgs.Empty);
		}

		public static void NotifyMakeSeries()
		{
			_eventSource.RaiseMakeSeries(EventArgs.Empty);
		}

		public static void NotifySurviveWaveInArena()
		{
			_eventSource.RaiseSurviveWaveInArena(EventArgs.Empty);
		}

		public static void NotifyGetGotcha()
		{
			_eventSource.RaiseGetGotcha(EventArgs.Empty);
		}

		public static void NotifySocialInteraction(string kind)
		{
			SocialInteractionEventArgs socialInteractionEventArgs = new SocialInteractionEventArgs();
			socialInteractionEventArgs.Kind = kind ?? string.Empty;
			SocialInteractionEventArgs e = socialInteractionEventArgs;
			_eventSource.RaiseSocialInteraction(e);
		}
	}
}
