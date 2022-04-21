using UnityEngine;

public class ABTestInfoInConsol : MonoBehaviour
{
	private void Update()
	{
		UILabel component = GetComponent<UILabel>();
		string text = string.Empty;
		if (Defs.isActivABTestStaticBank)
		{
			text = text + "А/Б тест статического банка:" + ((!Defs.isActivABTestStaticBank) ? "Не активен." : "Активен.") + ((!Defs.isActivABTestStaticBank) ? string.Empty : ("Когорта:" + ((!FriendsController.isShowStaticBank) ? "Scroll" : "Static"))) + "\n";
		}
		component.text = text;
	}
}
