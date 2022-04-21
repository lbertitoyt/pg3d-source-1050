using Rilisoft;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelReset : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData data)
	{
		Singleton<SceneLoader>.Instance.LoadSceneAsync(Application.loadedLevelName);
	}

	private void Update()
	{
	}
}
