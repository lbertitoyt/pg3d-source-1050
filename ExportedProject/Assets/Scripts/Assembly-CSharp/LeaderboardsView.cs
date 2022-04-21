using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

public sealed class LeaderboardsView : MonoBehaviour
{
	public enum State
	{
		None = 0,
		Clans = 1,
		Friends = 2,
		BestPlayers = 3,
		League = 4,
		Default = 3
	}

	internal const string LeaderboardsTabCache = "Leaderboards.TabCache";

	public UIWrapContent clansGrid;

	public UIWrapContent friendsGrid;

	public UIWrapContent bestPlayersGrid;

	public UIWrapContent leagueGrid;

	public ButtonHandler backButton;

	public UIButton clansButton;

	public UIButton friendsButton;

	public UIButton bestPlayersButton;

	[SerializeField]
	private UIButton leagueButton;

	public UIDragScrollView clansPanel;

	public UIDragScrollView friendsPanel;

	public UIDragScrollView bestPlayersPanel;

	[SerializeField]
	private UIDragScrollView leaguePanel;

	public UIScrollView clansScroll;

	public UIScrollView friendsScroll;

	public UIScrollView bestPlayersScroll;

	[SerializeField]
	private UIScrollView LeagueScroll;

	public GameObject defaultTableHeader;

	public GameObject clansTableHeader;

	public GameObject tableFooterClan;

	public GameObject tableFooterIndividual;

	public UILabel expirationLabel;

	private bool _overallTopFooterActive;

	private bool _leagueTopFooterActive;

	private readonly System.Lazy<UIPanel> _leaderboardsPanel;

	private bool _prepared;

	private State _currentState;

	public State CurrentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			if (_currentState == value)
			{
				return;
			}
			int value2 = (int)((LeaderboardScript.NewLeaderboards || value != State.League) ? value : State.Friends);
			PlayerPrefs.SetInt("Leaderboards.TabCache", value2);
			if (clansButton != null)
			{
				clansButton.isEnabled = value != State.Clans;
				Transform transform = clansButton.gameObject.transform.Find("SpriteLabel");
				if (transform != null)
				{
					transform.gameObject.SetActive(value != State.Clans);
				}
				Transform transform2 = clansButton.gameObject.transform.Find("ChekmarkLabel");
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(value == State.Clans);
				}
			}
			if (friendsButton != null)
			{
				friendsButton.isEnabled = value != State.Friends;
				Transform transform3 = friendsButton.gameObject.transform.Find("SpriteLabel");
				if (transform3 != null)
				{
					transform3.gameObject.SetActive(value != State.Friends);
				}
				Transform transform4 = friendsButton.gameObject.transform.Find("ChekmarkLabel");
				if (transform4 != null)
				{
					transform4.gameObject.SetActive(value == State.Friends);
				}
			}
			if (bestPlayersButton != null)
			{
				bestPlayersButton.isEnabled = value != State.BestPlayers;
				Transform transform5 = bestPlayersButton.gameObject.transform.Find("SpriteLabel");
				if (transform5 != null)
				{
					transform5.gameObject.SetActive(value != State.BestPlayers);
				}
				Transform transform6 = bestPlayersButton.gameObject.transform.Find("ChekmarkLabel");
				if (transform6 != null)
				{
					transform6.gameObject.SetActive(value == State.BestPlayers);
				}
			}
			if (leagueButton != null)
			{
				leagueButton.isEnabled = value != State.League;
				Transform transform7 = leagueButton.gameObject.transform.Find("SpriteLabel");
				if (transform7 != null)
				{
					transform7.gameObject.SetActive(value != State.League);
				}
				Transform transform8 = leagueButton.gameObject.transform.Find("ChekmarkLabel");
				if (transform8 != null)
				{
					transform8.gameObject.SetActive(value == State.League);
				}
			}
			if (defaultTableHeader != null)
			{
				defaultTableHeader.SetActive(value != State.Clans);
				if (tableFooterIndividual != null)
				{
					tableFooterIndividual.SetActive((value == State.BestPlayers && !_overallTopFooterActive) || (value == State.League && !_leagueTopFooterActive));
				}
			}
			if (clansTableHeader != null)
			{
				clansTableHeader.SetActive(value == State.Clans);
				string clanId = FriendsController.sharedController.Map((FriendsController c) => c.ClanID);
				tableFooterClan.Do(delegate(GameObject f)
				{
					f.SetActive(!string.IsNullOrEmpty(clanId));
				});
			}
			bestPlayersGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition4 = g.transform.localPosition;
				localPosition4.x = ((value != State.BestPlayers) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition4;
				if (!g.gameObject.activeInHierarchy)
				{
				}
			});
			friendsGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition3 = g.transform.localPosition;
				localPosition3.x = ((value != State.Friends) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition3;
				if (!g.gameObject.activeInHierarchy)
				{
				}
			});
			clansGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition2 = g.transform.localPosition;
				localPosition2.x = ((value != State.Clans) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition2;
				if (!g.gameObject.activeInHierarchy)
				{
				}
			});
			if (leagueGrid != null)
			{
				UIWrapContent uIWrapContent = leagueGrid;
				Vector3 localPosition = uIWrapContent.transform.localPosition;
				localPosition.x = ((value != State.League) ? 9000f : 0f);
				uIWrapContent.gameObject.transform.localPosition = localPosition;
			}
			_currentState = value;
		}
	}

	internal bool Prepared
	{
		get
		{
			return _prepared;
		}
	}

	public event EventHandler BackPressed;

	public LeaderboardsView()
	{
		_leaderboardsPanel = new System.Lazy<UIPanel>(GetComponent<UIPanel>);
	}

	public void SetOverallTopFooterActive()
	{
		_overallTopFooterActive = true;
	}

	public void SetLeagueTopFooterActive()
	{
		_leagueTopFooterActive = true;
	}

	private void RefreshGrid(UIGrid grid)
	{
		grid.Reposition();
	}

	private IEnumerator SkipFrameAndExecuteCoroutine(Action a)
	{
		if (a != null)
		{
			yield return new WaitForEndOfFrame();
			a();
		}
	}

	private void HandleTabPressed(object sender, EventArgs e)
	{
		GameObject gameObject = ((ButtonHandler)sender).gameObject;
		if (clansButton != null && gameObject == clansButton.gameObject)
		{
			CurrentState = State.Clans;
		}
		else if (friendsButton != null && gameObject == friendsButton.gameObject)
		{
			CurrentState = State.Friends;
		}
		else if (bestPlayersButton != null && gameObject == bestPlayersButton.gameObject)
		{
			CurrentState = State.BestPlayers;
		}
		else if (leagueButton != null && gameObject == leagueButton.gameObject)
		{
			CurrentState = State.League;
		}
	}

	private void RaiseBackPressed(object sender, EventArgs e)
	{
		EventHandler backPressed = this.BackPressed;
		if (backPressed != null)
		{
			backPressed(sender, e);
		}
	}

	private static IEnumerator SetGrid(UIGrid grid, IList<LeaderboardItemViewModel> value, string itemPrefabPath)
	{
		if (string.IsNullOrEmpty(itemPrefabPath))
		{
			throw new ArgumentException("itemPrefabPath");
		}
		if (!(grid != null))
		{
			yield break;
		}
		while (!grid.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		IEnumerable<LeaderboardItemViewModel> enumerable2;
		if (value == null)
		{
			IEnumerable<LeaderboardItemViewModel> enumerable = new List<LeaderboardItemViewModel>();
			enumerable2 = enumerable;
		}
		else
		{
			enumerable2 = value.Where((LeaderboardItemViewModel it) => it != null);
		}
		IEnumerable<LeaderboardItemViewModel> filteredList = enumerable2;
		List<Transform> list = grid.GetChildList();
		for (int i = 0; i != list.Count; i++)
		{
			UnityEngine.Object.Destroy(list[i].gameObject);
		}
		list.Clear();
		grid.Reposition();
		foreach (LeaderboardItemViewModel item in filteredList)
		{
			GameObject o = UnityEngine.Object.Instantiate(Resources.Load(itemPrefabPath)) as GameObject;
			if (o != null)
			{
				LeaderboardItemView liv = o.GetComponent<LeaderboardItemView>();
				if (liv != null)
				{
					liv.Reset(item);
					o.transform.parent = grid.transform;
					grid.AddChild(o.transform);
					o.transform.localScale = Vector3.one;
				}
			}
		}
		grid.Reposition();
		UIScrollView scrollView = grid.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			yield return null;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
			yield return null;
			scrollView.enabled = value.Count >= 10;
		}
	}

	private IEnumerator UpdateGridsAndScrollers()
	{
		_prepared = false;
		yield return new WaitForEndOfFrame();
		IEnumerable<UIWrapContent> wraps = new UIWrapContent[4] { friendsGrid, bestPlayersGrid, clansGrid, leagueGrid }.Where((UIWrapContent g) => g != null);
		foreach (UIWrapContent w in wraps)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		}
		yield return null;
		IEnumerable<UIScrollView> scrolls = new UIScrollView[4] { clansScroll, friendsScroll, bestPlayersScroll, LeagueScroll }.Where((UIScrollView s) => s != null);
		foreach (UIScrollView s2 in scrolls)
		{
			s2.ResetPosition();
			s2.UpdatePosition();
		}
		_prepared = true;
	}

	private void OnDestroy()
	{
		if (backButton != null)
		{
			backButton.Clicked -= RaiseBackPressed;
		}
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateGridsAndScrollers());
	}

	private void OnDisable()
	{
		_prepared = false;
	}

	private void Awake()
	{
		List<UIWrapContent> list = new UIWrapContent[1] { friendsGrid }.Where((UIWrapContent g) => g != null).ToList();
		foreach (UIWrapContent item in list)
		{
			item.gameObject.SetActive(true);
			Vector3 localPosition = item.transform.localPosition;
			localPosition.x = 9000f;
			item.gameObject.transform.localPosition = localPosition;
		}
		List<UIWrapContent> list2 = new UIWrapContent[3] { bestPlayersGrid, clansGrid, leagueGrid }.Where((UIWrapContent g) => g != null).ToList();
		foreach (UIWrapContent item2 in list2)
		{
			item2.gameObject.SetActive(true);
			Vector3 localPosition2 = item2.transform.localPosition;
			localPosition2.x = 9000f;
			item2.gameObject.transform.localPosition = localPosition2;
		}
	}

	private IEnumerator Start()
	{
		IEnumerable<UIButton> buttons = new UIButton[4] { clansButton, friendsButton, bestPlayersButton, leagueButton }.Where((UIButton b) => b != null);
		foreach (UIButton b2 in buttons)
		{
			ButtonHandler bh = b2.GetComponent<ButtonHandler>();
			if (bh != null)
			{
				bh.Clicked += HandleTabPressed;
			}
		}
		if (backButton != null)
		{
			backButton.Clicked += RaiseBackPressed;
		}
		IEnumerable<UIScrollView> scrollViews = new UIScrollView[4] { clansScroll, friendsScroll, bestPlayersScroll, LeagueScroll }.Where((UIScrollView s) => s != null);
		foreach (UIScrollView scrollView in scrollViews)
		{
			scrollView.ResetPosition();
		}
		yield return null;
		friendsGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		bestPlayersGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		clansGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		if (leagueGrid != null)
		{
			leagueGrid.SortBasedOnScrollMovement();
			leagueGrid.WrapContent();
		}
		yield return null;
		int stateInt = PlayerPrefs.GetInt("Leaderboards.TabCache", 3);
		State state = ((!Enum.IsDefined(typeof(State), stateInt)) ? State.BestPlayers : ((State)stateInt));
		CurrentState = ((state == State.None) ? State.BestPlayers : state);
	}
}
