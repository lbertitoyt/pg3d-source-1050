using System;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

[Serializable]
public class CupHUD : MonoBehaviour
{
	public float curProgress;

	public UITexture txCup;

	public UITexture txFillLine;

	public GameObject objSelectCup;

	private float sizePerFillLine = 0.02f;

	private void Awake()
	{
		HOTween.Init();
	}

	public void UpdateByOrder(int order)
	{
		curProgress = ProfileController.GetPerFillProgress(order, ExperienceController.sharedController.currentLevel);
		SetHUDProgress(curProgress);
		if ((bool)objSelectCup)
		{
			if (ProfileController.CurOrderCup == order)
			{
				objSelectCup.SetActive(true);
			}
			else
			{
				objSelectCup.SetActive(false);
			}
		}
	}

	public void AnimateCup(int animToLev, float delay = 0f, float timeFull = 1.5f)
	{
		base.gameObject.SetActive(true);
		int curOrderCup = ProfileController.CurOrderCup;
		float perFillProgress = ProfileController.GetPerFillProgress(curOrderCup, animToLev - 1);
		float perFillProgress2 = ProfileController.GetPerFillProgress(curOrderCup, animToLev);
		curProgress = perFillProgress;
		SetHUDProgress(perFillProgress);
		HOTween.To(this, timeFull, new TweenParms().Prop("curProgress", perFillProgress2).Delay(delay).Ease(EaseType.Linear)
			.OnUpdate((TweenDelegate.TweenCallback)delegate
			{
				UpdateCurProgress();
			}));
	}

	private void SetHUDProgress(float val)
	{
		txCup.fillAmount = val;
		if (val < sizePerFillLine)
		{
			txFillLine.fillAmount = 0f;
		}
		else
		{
			txFillLine.fillAmount = val + sizePerFillLine;
		}
	}

	public void UpdateCurProgress()
	{
		SetHUDProgress(curProgress);
	}

	[ContextMenu("Find Select obj")]
	private void FindSelectObj()
	{
		objSelectCup = base.transform.Find("Select").gameObject;
	}

	[ContextMenu("Find need Tx")]
	private void FindNeedTX()
	{
		txCup = base.transform.Find("Cup_usual_Filled").GetComponent<UITexture>();
		txFillLine = base.transform.Find("Cup_line").GetComponent<UITexture>();
	}

	[ContextMenu("TestAnimate")]
	private void TestAnimate()
	{
		AnimateCup(ExperienceController.sharedController.currentLevel, 0f);
	}
}
