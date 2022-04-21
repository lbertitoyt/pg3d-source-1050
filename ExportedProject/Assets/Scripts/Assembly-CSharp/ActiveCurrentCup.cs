using System.Collections.Generic;
using UnityEngine;

public class ActiveCurrentCup : MonoBehaviour
{
	public bool isTir;

	[Header("Добавить в порядке активации")]
	public List<CupHUD> arrCup = new List<CupHUD>();

	public float timeForFull = 1.5f;

	public float timeDelayLevelSec = 2f;

	public float timeDelayTirSec = 2f;

	public float timeDelayLev2Sec = 4.5f;

	private int curOrder;

	private void OnEnable()
	{
		float timeDelay = ((ExperienceController.sharedController.currentLevel != 2) ? ((!isTir) ? timeDelayLevelSec : timeDelayTirSec) : timeDelayLev2Sec);
		foreach (CupHUD item in arrCup)
		{
			item.gameObject.SetActive(false);
		}
		ActivateCurCupAnimation(timeDelay);
	}

	private void ActivateCurCupAnimation(float timeDelay = 0f)
	{
		int curOrderCup = ProfileController.CurOrderCup;
		if (arrCup.Count > curOrderCup)
		{
			arrCup[curOrderCup].AnimateCup(ExperienceController.sharedController.currentLevel, timeDelay, timeForFull);
		}
	}

	[ContextMenu("add all cup")]
	private void AddAllCup()
	{
		arrCup.Clear();
		arrCup.AddRange(GetComponentsInChildren<CupHUD>(true));
		arrCup.Sort(delegate(CupHUD x, CupHUD y)
		{
			if (x.gameObject.name == null && y.gameObject.name == null)
			{
				return 0;
			}
			if (x.gameObject.name == null)
			{
				return -1;
			}
			return (y.gameObject.name == null) ? 1 : x.gameObject.name.CompareTo(y.gameObject.name);
		});
	}

	[ContextMenu("Reanimate")]
	private void Hrenanimate()
	{
		ActivateCurCupAnimation(0f);
	}
}
