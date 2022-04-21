using UnityEngine;

public class AchieveBox : MonoBehaviour
{
	private Vector3 posToMove;

	private Vector3 hidePos;

	private UISprite mySprite;

	[HideInInspector]
	public bool isOpened;

	private bool toggled;

	public float speed = 300f;

	private void Awake()
	{
		mySprite = GetComponent<UISprite>();
		hidePos = base.transform.localPosition;
	}

	public void ShowBox()
	{
		base.gameObject.SetActive(true);
		toggled = true;
		posToMove = hidePos + Vector3.down * mySprite.height;
	}

	public void HideBox()
	{
		toggled = true;
		posToMove = hidePos;
	}

	private void Update()
	{
		if (!toggled)
		{
			return;
		}
		if (!(base.transform.localPosition == posToMove))
		{
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, posToMove, speed * RealTime.deltaTime);
			return;
		}
		toggled = false;
		isOpened = !isOpened;
		if (!isOpened)
		{
			base.gameObject.SetActive(false);
		}
	}
}
