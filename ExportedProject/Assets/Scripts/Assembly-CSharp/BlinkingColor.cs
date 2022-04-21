using Holoville.HOTween;
using UnityEngine;

public class BlinkingColor : MonoBehaviour
{
	private Material mainMaterial;

	public bool IsActive = true;

	public string nameColor = "_MainColor";

	public float speed = 1f;

	public Color normal;

	public Color blink;

	[HideInInspector]
	public Color curColor;

	private Color cashColor;

	private bool startBlink;

	private void Start()
	{
		Renderer component = GetComponent<Renderer>();
		if ((bool)component)
		{
			mainMaterial = component.sharedMaterial;
			if ((bool)mainMaterial)
			{
				cashColor = mainMaterial.GetColor(nameColor);
			}
		}
	}

	private void OnDestroy()
	{
		ResetColor();
	}

	private void Update()
	{
		if (IsActive)
		{
			if ((bool)mainMaterial)
			{
				mainMaterial.SetColor(nameColor, curColor);
			}
			if (!startBlink)
			{
				SetColorTwo();
			}
		}
		else if (startBlink)
		{
			ResetColor();
		}
	}

	private void ResetColor()
	{
		if ((bool)mainMaterial)
		{
			mainMaterial.SetColor(nameColor, cashColor);
		}
		startBlink = false;
		HOTween.Kill(this);
	}

	private void SetColorOne()
	{
		startBlink = true;
		HOTween.To(this, speed, new TweenParms().Prop("curColor", normal).Ease(EaseType.Linear).OnComplete(SetColorTwo));
	}

	private void SetColorTwo()
	{
		startBlink = true;
		HOTween.To(this, speed, new TweenParms().Prop("curColor", blink).Ease(EaseType.Linear).OnComplete(SetColorOne));
	}
}
