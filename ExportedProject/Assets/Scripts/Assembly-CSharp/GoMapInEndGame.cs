using UnityEngine;

public class GoMapInEndGame : MonoBehaviour
{
	public int mapIndex;

	public UITexture mapTexture;

	public UILabel mapLabel;

	private float enableTime;

	private void OnEnable()
	{
		enableTime = Time.time;
	}

	private void Start()
	{
		if (!Defs.isInet || Defs.isDaterRegim)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SetMap(SceneInfo scInfo)
	{
		mapIndex = scInfo.indexMap;
		mapTexture.mainTexture = Resources.Load<Texture>("LevelLoadingsSmall/Loading_" + scInfo.NameScene);
		if (scInfo != null)
		{
			mapLabel.text = scInfo.TranslateName;
		}
	}

	public void OnClick()
	{
		if (!(Time.time - enableTime < 2f) && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && (!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown))
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(mapIndex);
			Defs.typeDisconnectGame = Defs.DisconectGameType.SelectNewMap;
			Initializer.Instance.goMapName = infoScene.NameScene;
			GlobalGameController.countKillsRed = 0;
			GlobalGameController.countKillsBlue = 0;
			PhotonNetwork.LeaveRoom();
		}
	}
}
