using System;

namespace Rilisoft
{
	internal struct GpgFacade
	{
		private static readonly GpgFacade s_instance = default(GpgFacade);

		public static GpgFacade Instance
		{
			get
			{
				return s_instance;
			}
		}

		public void Authenticate(Action<bool> callback, bool silent)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
		}

		public void IncrementAchievement(string achievementId, int steps, Action<bool> callback)
		{
			if (achievementId == null)
			{
				throw new ArgumentNullException("achievementId");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
		}

		public bool IsAuthenticated()
		{
			return true;
		}

		public void SignOut()
		{
		}
	}
}
