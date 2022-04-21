using UnityEngine;

public class LeagueItemStot : MonoBehaviour
{
	[SerializeField]
	private UITexture _texture;

	[SerializeField]
	private GameObject _lockIndicator;

	private Color _baseTextureColor;

	private void Awake()
	{
		_baseTextureColor = _texture.color;
		base.gameObject.SetActive(false);
	}

	public void Set(string itemId, bool opened, bool purchased)
	{
		base.gameObject.SetActive(true);
		if (!opened && !purchased)
		{
			_texture.color = Color.white;
			_lockIndicator.gameObject.SetActive(true);
		}
		else
		{
			_texture.color = _baseTextureColor;
			_lockIndicator.gameObject.SetActive(false);
		}
		_texture.mainTexture = ItemDb.GetItemIcon(itemId, ShopNGUIController.CategoryNames.HatsCategory);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}
}
