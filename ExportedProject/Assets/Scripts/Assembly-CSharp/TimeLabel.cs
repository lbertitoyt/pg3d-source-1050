using UnityEngine;

public class TimeLabel : MonoBehaviour
{
	private UILabel _label;

	public UISprite timerBackground;

	public AudioSource timerSound;

	public ParticleSystem timerParticles;

	public GameObject pausePanel;

	private Vector3 targetScale = Vector3.one;

	private bool blink;

	private float startTime = 11f;

	private void Start()
	{
		base.gameObject.SetActive(Defs.isMulti);
		_label = GetComponent<UILabel>();
	}

	private void Update()
	{
		if (!InGameGUI.sharedInGameGUI || !_label)
		{
			return;
		}
		_label.text = InGameGUI.sharedInGameGUI.timeLeft();
		if (Defs.isHunger)
		{
			return;
		}
		float num = (float)TimeGameController.sharedController.timerToEndMatch;
		if (num <= startTime)
		{
			float num2 = Mathf.Round(num) - num;
			blink = num2 > 0f;
			_label.transform.localScale = Vector3.MoveTowards(_label.transform.localScale, (!blink) ? Vector3.one : (Vector3.one * Mathf.Min(1.4f + (startTime - num) / 20f, 2f)), (!blink) ? (2.4f * Time.deltaTime) : (12f * Time.deltaTime));
			_label.color = ((!blink) ? Color.white : Color.red);
			_label.GetComponentInChildren<TweenRotation>().enabled = true;
			_label.GetComponentInChildren<TweenRotation>().PlayForward();
			if (Defs.isSoundFX)
			{
				timerSound.enabled = true;
			}
			timerSound.loop = true;
			if (!pausePanel.activeSelf)
			{
				timerParticles.gameObject.SetActive(true);
			}
			else
			{
				timerParticles.gameObject.SetActive(false);
			}
			ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = timerParticles.textureSheetAnimation;
			ParticleSystemCurveMode mode = textureSheetAnimation.frameOverTime.mode;
			textureSheetAnimation.frameOverTime = new ParticleSystem.MinMaxCurve((num - 1f) / 9f);
			if (num < 1f)
			{
				timerParticles.gameObject.SetActive(false);
			}
		}
		else
		{
			timerParticles.gameObject.SetActive(false);
			timerSound.enabled = false;
			_label.color = Color.white;
			_label.transform.localScale = Vector3.one;
			_label.GetComponentInChildren<TweenRotation>().ResetToBeginning();
			_label.GetComponentInChildren<TweenRotation>().enabled = false;
		}
	}
}
