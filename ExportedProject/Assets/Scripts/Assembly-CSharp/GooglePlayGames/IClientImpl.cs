using GooglePlayGames.Native.PInvoke;

namespace GooglePlayGames
{
	internal interface IClientImpl
	{
		PlatformConfiguration CreatePlatformConfiguration();

		TokenClient CreateTokenClient(string playerId, bool reset);
	}
}
