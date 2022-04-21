using I2.Loc;
using UnityEngine;

[ExecuteInEditMode]
public class LocalizeRili : MonoBehaviour
{
	public GameObject[] labels;

	public string term;

	public bool execute;

	private void Start()
	{
	}

	private void Update()
	{
		if (!execute || labels == null)
		{
			return;
		}
		Debug.Log("Localized");
		GameObject[] array = labels;
		foreach (GameObject gameObject in array)
		{
			gameObject.gameObject.AddComponent<Localize>().SetTerm("Key_04B_03", "Key_04B_03");
			if (term != string.Empty)
			{
				gameObject.gameObject.AddComponent<Localize>().SetTerm(term, term);
			}
		}
		execute = false;
	}
}
