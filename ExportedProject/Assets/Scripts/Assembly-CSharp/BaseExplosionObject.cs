using System.Collections.Generic;
using Rilisoft;
using RilisoftBot;
using UnityEngine;

public class BaseExplosionObject : MonoBehaviour
{
	private const string ExplosionAnimationName = "Broken";

	[Header("Common Settings")]
	public GameObject explosionObject;

	[Header("Common Damage settings")]
	public float radiusExplosion;

	[Header("Common Damage settings")]
	public float radiusMaxExplosion;

	public float damageZombie = 2f;

	public float[] damageByTier = new float[ExpController.LevelsForTiers.Length];

	[Header("Common Effect settings")]
	public GameObject explosionEffect;

	protected bool isMultiplayerMode;

	protected PhotonView photonView;

	private ExplosionObjectRespawnController _respawnController;

	private void Start()
	{
		InitializeData();
		Initializer.damagedObj.Add(base.gameObject);
	}

	private void OnDestroy()
	{
		Initializer.damagedObj.Remove(base.gameObject);
	}

	protected virtual void InitializeData()
	{
		isMultiplayerMode = Defs.isMulti;
		photonView = PhotonView.Get(this);
		InitializeRespawnPoint();
	}

	private void InitializeRespawnPoint()
	{
		if (isMultiplayerMode && !PhotonNetwork.isMasterClient)
		{
			GameObject gameObject = null;
			float num = float.MaxValue;
			for (int i = 0; i < ExplosionObjectRespawnController.respawnList.Count; i++)
			{
				if (ExplosionObjectRespawnController.respawnList[i] != null)
				{
					float sqrMagnitude = (ExplosionObjectRespawnController.respawnList[i].transform.position - base.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						gameObject = ExplosionObjectRespawnController.respawnList[i];
					}
				}
			}
			if (gameObject != null)
			{
				base.transform.parent = gameObject.transform;
				_respawnController = gameObject.GetComponent<ExplosionObjectRespawnController>();
			}
			else
			{
				_respawnController = null;
			}
		}
		else
		{
			_respawnController = base.transform.parent.GetComponent<ExplosionObjectRespawnController>();
		}
	}

	private void PlayDestroyEffect()
	{
		Object.Instantiate(explosionEffect, base.transform.position, Quaternion.identity);
		GetComponent<Animation>().Play("Broken");
	}

	protected bool IsTargetAvailable(Transform targetTransform)
	{
		if (targetTransform.Equals(base.transform))
		{
			return false;
		}
		return targetTransform.CompareTag("Player") || targetTransform.CompareTag("Enemy") || targetTransform.CompareTag("Turret") || (targetTransform.childCount > 0 && targetTransform.GetChild(0).CompareTag("DamagedExplosion"));
	}

	private void CheckTakeDamage()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, radiusExplosion, Tools.AllWithoutDamageCollidersMask);
		if (array.Length == 0)
		{
			return;
		}
		List<Transform> list = new List<Transform>();
		float num = radiusExplosion * radiusExplosion;
		float diameterMaxExplosion = radiusMaxExplosion * radiusMaxExplosion;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject == null)
			{
				continue;
			}
			Transform root = array[i].transform.root;
			if (!(root.gameObject == null) && !(base.transform.gameObject == null) && !list.Contains(root) && IsTargetAvailable(root))
			{
				float sqrMagnitude = (root.position - base.transform.position).sqrMagnitude;
				if (!(sqrMagnitude > num))
				{
					ApplyDamage(root, sqrMagnitude, num, diameterMaxExplosion);
					list.Add(root);
				}
			}
		}
	}

	private void ApplyDamage(Transform target, float distanceToTarget, float diameterExplosion, float diameterMaxExplosion)
	{
		float num = 0f;
		if (target.CompareTag("Player"))
		{
			Player_move_c playerMoveC = target.GetComponent<SkinName>().playerMoveC;
			int num2 = ((!isMultiplayerMode) ? ExpController.OurTierForAnyPlace() : ((playerMoveC.myTable != null) ? ExpController.TierForLevel(playerMoveC.myTable.GetComponent<NetworkStartTable>().myRanks) : 0));
			num = ((!(distanceToTarget > diameterMaxExplosion)) ? damageByTier[num2] : (damageByTier[num2] * ((diameterExplosion - (distanceToTarget - diameterMaxExplosion)) / diameterExplosion)));
			if (isMultiplayerMode)
			{
				playerMoveC.SendDamageFromEnv(num, base.transform.position);
			}
			else
			{
				playerMoveC.GetDamageFromEnv(num, base.transform.position);
			}
		}
		else if (target.CompareTag("Turret"))
		{
			TurretController component = target.GetComponent<TurretController>();
			num = ((!(distanceToTarget > diameterMaxExplosion)) ? damageByTier[component.numUpdate] : (damageByTier[component.numUpdate] * ((diameterExplosion - (distanceToTarget - diameterMaxExplosion)) / diameterExplosion)));
			component.MinusLive(num);
		}
		else if (target.CompareTag("Enemy"))
		{
			BaseBot botScriptForObject = BaseBot.GetBotScriptForObject(target);
			num = ((!(distanceToTarget > diameterMaxExplosion)) ? damageZombie : (damageZombie * ((diameterExplosion - (distanceToTarget - diameterMaxExplosion)) / diameterExplosion)));
			if (isMultiplayerMode)
			{
				botScriptForObject.GetDamageForMultiplayer(0f - num, null);
			}
			else
			{
				botScriptForObject.GetDamage(0f - num, null, false);
			}
		}
		else if (target.childCount > 0 && target.GetChild(0).CompareTag("DamagedExplosion"))
		{
			DamagedExplosionObject component2 = target.GetChild(0).GetComponent<DamagedExplosionObject>();
			if (component2 != null && component2.healthPoints > 0f)
			{
				component2.healthPoints = 0f;
				component2.Invoke("RunExplosion", 0.1f);
			}
		}
	}

	private void RecreateObject()
	{
		if (isMultiplayerMode)
		{
			DestroyObjectByNetworkRpc();
			photonView.RPC("DestroyObjectByNetworkRpc", PhotonTargets.Others);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		if (isMultiplayerMode)
		{
			StartNewRespanObjectRpc();
			photonView.RPC("StartNewRespanObjectRpc", PhotonTargets.Others);
		}
		else if (_respawnController != null)
		{
			_respawnController.StartProcessNewRespawn();
		}
	}

	public void RunExplosion()
	{
		if (isMultiplayerMode)
		{
			PlayDestroyEffect();
			photonView.RPC("PlayDestroyEffectRpc", PhotonTargets.Others);
		}
		else
		{
			PlayDestroyEffect();
		}
		CheckTakeDamage();
		RecreateObject();
	}

	[PunRPC]
	[RPC]
	public void DestroyObjectByNetworkRpc()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			explosionObject.SetActive(false);
		}
	}

	[PunRPC]
	[RPC]
	public void StartNewRespanObjectRpc()
	{
		if (_respawnController != null)
		{
			_respawnController.StartProcessNewRespawn();
		}
	}

	[PunRPC]
	[RPC]
	public void PlayDestroyEffectRpc()
	{
		PlayDestroyEffect();
	}
}
