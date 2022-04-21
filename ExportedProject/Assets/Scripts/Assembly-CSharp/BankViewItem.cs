using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BankViewItem : MonoBehaviour, IComparable<BankViewItem>
{
	public List<UILabel> inappNameLabels;

	public List<UILabel> countLabelsList;

	public List<UILabel> countLabelsX3List;

	public UILabel countLabel;

	public UILabel countX3Label;

	public UITexture icon;

	public UILabel priceLabel;

	public UILabel priceLabelBestBuy;

	public UISprite discountSprite;

	public UILabel discountPercentsLabel;

	public UIButton btnBuy;

	[NonSerialized]
	public PurchaseEventArgs purchaseInfo;

	public UISprite bestBuy;

	public ChestBonusButtonView bonusButtonView;

	private Animator _bestBuyAnimator;

	private Animator _discountAnimator;

	public string Price
	{
		set
		{
			if (priceLabel != null)
			{
				priceLabel.text = value ?? string.Empty;
			}
			if (priceLabelBestBuy != null)
			{
				priceLabelBestBuy.text = value ?? string.Empty;
			}
		}
	}

	public int Count
	{
		set
		{
			if (countLabelsList != null && countLabelsList.Any())
			{
				foreach (UILabel countLabels in countLabelsList)
				{
					countLabels.text = value.ToString();
				}
				return;
			}
			if (countLabel != null)
			{
				countLabel.text = value.ToString();
			}
		}
	}

	public int CountX3
	{
		set
		{
			if (countLabelsX3List != null && countLabelsX3List.Any())
			{
				foreach (UILabel countLabelsX in countLabelsX3List)
				{
					countLabelsX.text = value.ToString();
				}
				return;
			}
			if (countX3Label != null)
			{
				countX3Label.text = value.ToString();
			}
		}
	}

	private static bool PaymentOccursInLastTwoWeeks()
	{
		string @string = PlayerPrefs.GetString("Last Payment Time", string.Empty);
		DateTime result;
		if (!string.IsNullOrEmpty(@string) && DateTime.TryParse(@string, out result))
		{
			TimeSpan timeSpan = DateTime.UtcNow - result;
			return timeSpan <= TimeSpan.FromDays(14.0);
		}
		return false;
	}

	public int CompareTo(BankViewItem other)
	{
		int value = ((other != null) ? other.purchaseInfo.Count : 0);
		return (!PaymentOccursInLastTwoWeeks()) ? purchaseInfo.Count.CompareTo(value) : value.CompareTo(purchaseInfo.Count);
	}

	private void Awake()
	{
		_bestBuyAnimator = ((!(bestBuy == null)) ? bestBuy.GetComponent<Animator>() : null);
		_discountAnimator = ((!(discountSprite == null)) ? discountSprite.GetComponent<Animator>() : null);
		if (bonusButtonView != null)
		{
			bonusButtonView.Initialize();
			if (purchaseInfo != null)
			{
				bonusButtonView.UpdateState(purchaseInfo);
			}
		}
		PromoActionsManager.BestBuyStateUpdate += UpdateViewBestBuy;
	}

	private void UpdateAnimationEventSprite(bool isEventActive)
	{
		PromoActionsManager sharedManager = PromoActionsManager.sharedManager;
		if (sharedManager != null && sharedManager.IsEventX3Active)
		{
			return;
		}
		bool flag = discountSprite != null && discountSprite.gameObject.activeSelf;
		if (flag && _discountAnimator != null)
		{
			if (isEventActive)
			{
				_discountAnimator.Play("DiscountAnimation");
			}
			else
			{
				_discountAnimator.Play("Idle");
			}
		}
		if (isEventActive && _bestBuyAnimator != null)
		{
			if (flag)
			{
				_bestBuyAnimator.Play("BestBuyAnimation");
			}
			else
			{
				_bestBuyAnimator.Play("Idle");
			}
		}
	}

	public void UpdateViewBestBuy()
	{
		PromoActionsManager sharedManager = PromoActionsManager.sharedManager;
		bool flag = !(sharedManager == null) && sharedManager.IsBankItemBestBuy(purchaseInfo);
		if (priceLabelBestBuy == null)
		{
			bestBuy.gameObject.SetActive(flag);
			UpdateAnimationEventSprite(flag);
			return;
		}
		if (priceLabel != null)
		{
			priceLabel.transform.gameObject.SetActive(!flag);
		}
		if (priceLabelBestBuy != null)
		{
			bestBuy.gameObject.SetActive(flag);
		}
	}

	private void OnEnable()
	{
		UpdateViewBestBuy();
	}

	private void OnDestroy()
	{
		if (bonusButtonView != null)
		{
			bonusButtonView.Deinitialize();
		}
		PromoActionsManager.BestBuyStateUpdate -= UpdateViewBestBuy;
	}
}
