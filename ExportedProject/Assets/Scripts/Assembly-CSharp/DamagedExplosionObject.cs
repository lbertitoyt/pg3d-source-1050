using UnityEngine;

public class DamagedExplosionObject : BaseExplosionObject
{
	public const string NameTag = "DamagedExplosion";

	[Header("Damaged Health settings")]
	public float healthPoints = 100f;

	[Range(1f, 100f)]
	public float percentHealthForFireEffect = 95f;

	[Header("Damaged Effect settings")]
	public GameObject fireEffect;

	public float timeToDestroyByFire = 5f;

	private float _maxHealth;

	public void GetDamage(float damage)
	{
		if (!(healthPoints <= 0f))
		{
			float num = healthPoints / 100f * healthPoints;
			if (num <= percentHealthForFireEffect && !fireEffect.activeSelf)
			{
				SetVisibleFireEffect(true);
				Invoke("RunExplosion", timeToDestroyByFire);
			}
			healthPoints += damage;
			if (healthPoints <= 0f)
			{
				healthPoints = 0f;
				RunExplosion();
			}
		}
	}

	protected override void InitializeData()
	{
		base.InitializeData();
		_maxHealth = healthPoints;
		SetVisibleFireEffect(false);
	}

	private void SetVisibleFireEffect(bool visible)
	{
		if (isMultiplayerMode)
		{
			SetVisibleFireEffectRpc(visible);
			photonView.RPC("SetVisibleFireEffectRpc", PhotonTargets.Others, visible);
		}
		else
		{
			fireEffect.SetActive(visible);
		}
	}

	public static void TryApplyDamageToObject(GameObject explosionObject, float damage)
	{
		DamagedExplosionObject component = explosionObject.GetComponent<DamagedExplosionObject>();
		if (component != null)
		{
			component.GetDamage(damage);
		}
	}

	[RPC]
	[PunRPC]
	private void SetVisibleFireEffectRpc(bool visible)
	{
		fireEffect.SetActive(visible);
	}
}
