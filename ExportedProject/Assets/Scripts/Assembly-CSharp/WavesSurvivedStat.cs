using UnityEngine;

public sealed class WavesSurvivedStat : MonoBehaviour
{
	private void Start()
	{
		GetComponent<UILabel>().text = PlayerPrefs.GetInt(Defs.WavesSurvivedS, 0).ToString();
	}
}
