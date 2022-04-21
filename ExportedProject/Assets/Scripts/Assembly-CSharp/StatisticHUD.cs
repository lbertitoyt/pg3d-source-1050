using System.Collections.Generic;
using UnityEngine;

public class StatisticHUD : MonoBehaviour
{
	public enum TypeOpenTab
	{
		multiplayer,
		singleplayer,
		leagues
	}

	[Header("Вкладки")]
	public TypeOpenTab curOpenTab;

	public GameObject tabMultiplayer;

	public GameObject tabSingleplayer;

	public GameObject tabLeaguesOld;

	public GameObject tabLeagues;

	[Header("Кнопки по вкладкам")]
	public UIButton btnMultiplayer;

	public UIButton btnSingleplayer;

	public UIButton btnLeagues;

	[Header("КУБКИ")]
	[Header("добавить в порядке заполнения")]
	public List<CupHUD> listAllCup = new List<CupHUD>();

	private void OnEnable()
	{
		OpenActiveTab();
	}

	public void OpenTab(TypeOpenTab tab)
	{
		curOpenTab = tab;
		OpenActiveTab(true);
	}

	public void OpenMultiplayer()
	{
		curOpenTab = TypeOpenTab.multiplayer;
		OpenActiveTab(true);
	}

	public void OpenSingleplayer()
	{
		curOpenTab = TypeOpenTab.singleplayer;
		OpenActiveTab(true);
	}

	public void OpenLeagues()
	{
		curOpenTab = TypeOpenTab.leagues;
		OpenActiveTab(true);
	}

	private void OpenActiveTab(bool playSound = false)
	{
		if (playSound)
		{
			ButtonClickSound.TryPlayClick();
		}
		switch (curOpenTab)
		{
		case TypeOpenTab.multiplayer:
			OnOpenMultiplayer();
			break;
		case TypeOpenTab.singleplayer:
			OnOpenSingleplayer();
			break;
		case TypeOpenTab.leagues:
			OnOpenLeagues();
			break;
		}
	}

	private void HideAllTab()
	{
		tabMultiplayer.SetActive(false);
		tabSingleplayer.SetActive(false);
		tabLeaguesOld.SetActive(false);
		tabLeagues.SetActive(false);
		btnMultiplayer.enabled = true;
		btnSingleplayer.enabled = true;
		btnLeagues.enabled = true;
		btnMultiplayer.SetState(UIButtonColor.State.Normal, true);
		btnSingleplayer.SetState(UIButtonColor.State.Normal, true);
		btnLeagues.SetState(UIButtonColor.State.Normal, true);
	}

	private void OnOpenMultiplayer()
	{
		HideAllTab();
		btnMultiplayer.enabled = false;
		btnMultiplayer.SetState(UIButtonColor.State.Pressed, true);
		tabMultiplayer.SetActive(true);
	}

	private void OnOpenSingleplayer()
	{
		HideAllTab();
		btnSingleplayer.enabled = false;
		btnSingleplayer.SetState(UIButtonColor.State.Pressed, true);
		tabSingleplayer.SetActive(true);
	}

	private void OnOpenLeagues()
	{
		for (int i = 0; i < listAllCup.Count; i++)
		{
			listAllCup[i].UpdateByOrder(i);
		}
		HideAllTab();
		btnLeagues.enabled = false;
		btnLeagues.SetState(UIButtonColor.State.Pressed, true);
		if (FriendsController.isUseRatingSystem)
		{
			tabLeagues.SetActive(true);
			tabLeaguesOld.SetActive(false);
		}
		else
		{
			tabLeagues.SetActive(false);
			tabLeaguesOld.SetActive(true);
		}
	}

	[ContextMenu("Add all cup")]
	private void AddAllCup()
	{
		listAllCup.Clear();
		listAllCup.AddRange(GetComponentsInChildren<CupHUD>(true));
	}
}
