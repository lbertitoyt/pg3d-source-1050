using System.Collections;
using Rilisoft;
using UnityEngine;

public class CoinsAddIndic : MonoBehaviour
{
	private const float blinkR = 255f;

	private const float blinkG = 255f;

	private const float blinkB = 0f;

	private const float blinkA = 115f;

	private const float blinkGemsR = 0f;

	private const float blinkGemsG = 0f;

	private const float blinkGemsB = 255f;

	private const float blinkGemsA = 115f;

	public bool stopBlinkingOnEnable;

	public bool isGems;

	public bool isX3;

	private UISprite ind;

	private bool blinking;

	public AudioClip coinsAdded;

	public AudioClip coinAdded;

	public bool remembeState;

	private int backgroundAdd;

	private bool isSurvival;

	private Color BlinkColor
	{
		get
		{
			return (!isGems) ? new Color(1f, 1f, 0f, 23f / 51f) : new Color(0f, 0f, 1f, 23f / 51f);
		}
	}

	private void Start()
	{
		ind = GetComponent<UISprite>();
		isSurvival = Defs.IsSurvival;
		if (remembeState)
		{
			CoinsMessage.CoinsLabelDisappeared -= BackgroundEventAdd;
			CoinsMessage.CoinsLabelDisappeared += BackgroundEventAdd;
		}
	}

	private void OnEnable()
	{
		CoinsMessage.CoinsLabelDisappeared += IndicateCoinsAdd;
		if (ind != null)
		{
			ind.color = NormalColor();
		}
		if (backgroundAdd > 0)
		{
			blinking = false;
			IndicateCoinsAdd(backgroundAdd == 1, 2);
			backgroundAdd = 0;
		}
		if (blinking && !stopBlinkingOnEnable)
		{
			StartCoroutine(blink());
		}
		else if (stopBlinkingOnEnable)
		{
			blinking = false;
		}
	}

	private void OnDisable()
	{
		CoinsMessage.CoinsLabelDisappeared -= IndicateCoinsAdd;
	}

	private void OnDestroy()
	{
		if (remembeState)
		{
			CoinsMessage.CoinsLabelDisappeared -= BackgroundEventAdd;
		}
	}

	private void IndicateCoinsAdd(bool gems, int count)
	{
		if (gems == isGems)
		{
			if (!blinking)
			{
				StartCoroutine(blink());
			}
			if (!Defs.isMulti && !Defs.IsSurvival && TrainingController.TrainingCompleted)
			{
				PlaySoundNow(count == 1);
			}
			else
			{
				StartCoroutine(PlaySound(count == 1));
			}
		}
	}

	private Color NormalColor()
	{
		return (!isX3) ? new Color(0f, 0f, 0f, 23f / 51f) : new Color(1f, 0f, 0f, 0.5882353f);
	}

	private IEnumerator blink()
	{
		if (ind == null)
		{
			Debug.LogWarning("Indicator sprite is null.");
			yield return null;
		}
		blinking = true;
		try
		{
			for (int i = 0; i < 15; i++)
			{
				ind.color = BlinkColor;
				yield return null;
				yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(0.1f));
				ind.color = NormalColor();
				yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(0.1f));
			}
			ind.color = NormalColor();
		}
		finally
		{
			blinking = false;
		}
	}

	private IEnumerator PlaySound(bool oneCoin)
	{
		yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(0.11f));
		PlaySoundNow(oneCoin);
	}

	private void PlaySoundNow(bool oneCoin)
	{
		if (!SceneLoader.ActiveSceneName.Equals("LevelComplete") && Defs.isSoundFX)
		{
			NGUITools.PlaySound((!oneCoin || !(coinAdded != null)) ? coinsAdded : coinAdded);
		}
	}

	private void BackgroundEventAdd(bool gems, int count)
	{
		if ((BankController.Instance == null || !BankController.Instance.InterfaceEnabled) && GiftBannerWindow.instance != null && GiftBannerWindow.instance.IsShow)
		{
			if (gems && isGems)
			{
				backgroundAdd = 1;
			}
			if (!gems && !isGems)
			{
				backgroundAdd = 2;
			}
		}
	}
}
