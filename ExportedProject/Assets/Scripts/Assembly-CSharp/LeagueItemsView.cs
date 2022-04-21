using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

[RequireComponent(typeof(UIGrid))]
public class LeagueItemsView : MonoBehaviour
{
	[SerializeField]
	private UILabel _headerText;

	private UIGrid _grid;

	private LeagueItemStot[] _slots;

	private void Awake()
	{
		_grid = GetComponent<UIGrid>();
		_slots = GetComponentsInChildren<LeagueItemStot>(true);
	}

	public void Repaint(RatingSystem.RatingLeague league)
	{
		List<string> list = Wear.LeagueItemsByLeagues()[league];
		_headerText.gameObject.SetActive(list.Any());
		int num = 0;
		LeagueItemStot[] slots = _slots;
		foreach (LeagueItemStot leagueItemStot in slots)
		{
			if (list.Count() > num)
			{
				string itemId = list[num];
				List<Wear.LeagueItemState> statesForItem = GetStatesForItem(itemId);
				leagueItemStot.Set(itemId, statesForItem.Contains(Wear.LeagueItemState.Open), statesForItem.Contains(Wear.LeagueItemState.Purchased));
			}
			else
			{
				leagueItemStot.Hide();
			}
			num++;
		}
		_grid.Reposition();
	}

	private List<Wear.LeagueItemState> GetStatesForItem(string itemId)
	{
		List<Wear.LeagueItemState> res = new List<Wear.LeagueItemState>();
		Dictionary<Wear.LeagueItemState, List<string>> items = Wear.LeagueItems();
		RiliExtensions.ForEachEnum(delegate(Wear.LeagueItemState val)
		{
			if (items[val].Contains(itemId))
			{
				res.Add(val);
			}
		});
		return res;
	}
}
