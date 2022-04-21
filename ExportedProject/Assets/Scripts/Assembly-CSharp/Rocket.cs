using System;
using System.Collections;
using System.Reflection;
using RilisoftBot;
using UnityEngine;

public sealed class Rocket : MonoBehaviour
{
	private int _rocketNum;

	public string weaponName = string.Empty;

	public GameObject[] rockets;

	public float damage;

	public float radiusDamage;

	public float radiusDamageSelf;

	public float radiusImpulse;

	public float multiplayerDamage;

	public Vector2 damageRange;

	public PhotonView photonView;

	private bool isMulti;

	private bool isInet;

	private bool isCompany;

	private bool isCOOP;

	public bool isMine;

	private WeaponManager _weaponManager;

	private bool isKilled;

	public Transform target;

	public bool isRun;

	private Player_move_c myPlayerMoveC;

	private bool isStartSynh;

	private Vector3 correctPos = Vector3.zero;

	public Transform myTransform;

	private bool isFirstPos = true;

	public bool dontExecStart;

	public RocketSettings currentRocketSettings;

	public float impulseForce;

	public float impulseForceSelf;

	public string weaponPrefabName { get; set; }

	public float slowdownTime { get; set; }

	public float slowdownCoeff { get; set; }

	public bool isSlowdown { get; set; }

	public int rocketNum
	{
		get
		{
			return _rocketNum;
		}
		set
		{
			_rocketNum = value;
			currentRocketSettings = rockets[value].GetComponent<RocketSettings>();
		}
	}

	private void Awake()
	{
		isMulti = Defs.isMulti;
		isInet = Defs.isInet;
		isCompany = Defs.isCompany;
		isCOOP = Defs.isCOOP;
		photonView = GetComponent<PhotonView>();
		myTransform = base.transform;
		if (isMulti)
		{
			if (!isInet)
			{
				isMine = GetComponent<NetworkView>().isMine;
			}
			else
			{
				isMine = photonView.isMine;
			}
		}
		else
		{
			isMine = true;
		}
		_weaponManager = WeaponManager.sharedManager;
	}

	public void SendSetRocketActiveRPC()
	{
		if (isMulti && isMine)
		{
			if (!isInet)
			{
				GetComponent<NetworkView>().RPC("SetRocketActive", RPCMode.All, weaponPrefabName, radiusImpulse, base.transform.position);
			}
			else
			{
				photonView.RPC("SetRocketActive", PhotonTargets.All, weaponPrefabName, radiusImpulse, base.transform.position);
			}
		}
		else if (!isMulti)
		{
			SetRocketActive(weaponPrefabName, radiusImpulse, base.transform.position);
		}
	}

	private IEnumerator StartRocketCoroutine()
	{
		isKilled = false;
		if (isMine && (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Bomb || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.GravityRocket))
		{
			GetComponent<Rigidbody>().useGravity = true;
		}
		if (!IsGrenadeRocketNum(rocketNum))
		{
			StartRocketRPC();
		}
		if (!isRun)
		{
			if (!isMulti || isMine)
			{
				Player_move_c.SetLayerRecursively(base.gameObject, 9);
			}
			if (Defs.isMulti)
			{
				if (Defs.isInet)
				{
					for (int i = 0; i < Initializer.players.Count; i++)
					{
						if (Initializer.players[i].mySkinName.photonView != null && photonView.ownerId == Initializer.players[i].mySkinName.photonView.ownerId)
						{
							myPlayerMoveC = Initializer.players[i];
							break;
						}
					}
				}
			}
			else
			{
				myPlayerMoveC = WeaponManager.sharedManager.myPlayerMoveC;
			}
		}
		if (IsGrenadeRocketNum(rocketNum))
		{
			while (myPlayerMoveC == null || myPlayerMoveC.myPlayerTransform.childCount == 0 || myPlayerMoveC.myCurrentWeaponSounds.grenatePoint == null)
			{
				yield return null;
			}
			base.transform.parent = myPlayerMoveC.myCurrentWeaponSounds.grenatePoint;
			yield return null;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			yield return null;
			rockets[rocketNum].transform.localPosition = Vector3.zero;
		}
	}

	private void Start()
	{
		if (!isMulti || isMine)
		{
			myPlayerMoveC = WeaponManager.sharedManager.myPlayerMoveC;
		}
		else
		{
			base.transform.GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	public void SendNetworkViewMyPlayer(NetworkViewID myId)
	{
		GetComponent<NetworkView>().RPC("SendNetworkViewMyPlayerRPC", RPCMode.AllBuffered, myId);
	}

	[PunRPC]
	[RPC]
	public void SendNetworkViewMyPlayerRPC(NetworkViewID myId)
	{
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			if (myId.Equals(Initializer.players[i].mySkinName.GetComponent<NetworkView>().viewID))
			{
				myPlayerMoveC = Initializer.players[i];
				break;
			}
		}
	}

	public void StartRocket()
	{
		if (isMulti && isMine)
		{
			if (!isInet)
			{
				GetComponent<NetworkView>().RPC("StartRocketRPC", RPCMode.All);
			}
			else
			{
				photonView.RPC("StartRocketRPC", PhotonTargets.All);
			}
		}
		else if (!isMulti)
		{
			StartRocketRPC();
		}
		base.transform.GetComponent<Rigidbody>().isKinematic = false;
	}

	[RPC]
	[PunRPC]
	public void StartRocketRPC()
	{
		if (isMulti && isInet && photonView != null)
		{
			photonView.synchronization = ViewSynchronization.UnreliableOnChange;
		}
		if (isMulti && !isInet)
		{
			GetComponent<NetworkView>().stateSynchronization = NetworkStateSynchronization.Unreliable;
		}
		if (rocketNum == 10 && currentRocketSettings != null && currentRocketSettings.trail != null)
		{
			currentRocketSettings.trail.enabled = true;
		}
		base.transform.parent = null;
		Player_move_c.SetLayerRecursively(base.gameObject, LayerMask.NameToLayer("Rocket"));
		isRun = true;
		if (!isMulti || isMine)
		{
			Invoke("KillRocket", (rocketNum == 10) ? 1.35f : ((rocketNum == 24) ? 2f : ((rocketNum == 31 || rocketNum == 46) ? 3f : ((rocketNum == 35) ? 1.5f : ((!Defs.isDaterRegim || (rocketNum != 36 && rocketNum != 18)) ? 7f : 1f)))));
		}
	}

	[RPC]
	[PunRPC]
	public void SetRocketDeactive()
	{
		isRun = false;
		isStartSynh = false;
		dontExecStart = false;
		int num = rocketNum;
		if (num >= rockets.Length)
		{
			num = 0;
		}
		rockets[num].SetActive(false);
		if (isMulti && isInet && photonView != null)
		{
			photonView.synchronization = ViewSynchronization.Off;
		}
		if (!isMulti || !isInet)
		{
		}
		base.transform.position = Vector3.down * 10000f;
		if (currentRocketSettings != null && currentRocketSettings.trail != null)
		{
			currentRocketSettings.trail.enabled = false;
		}
	}

	[PunRPC]
	[RPC]
	public void SetRocketActive(string weapon, float _radiusImpulse, Vector3 pos)
	{
		if (weapon == "WeaponGrenade")
		{
			weaponName = "WeaponGrenade";
			impulseForce = 50f;
			impulseForceSelf = 133.4f;
			SetRocketActive(10, _radiusImpulse, pos);
			return;
		}
		if (weapon == "WeaponLike")
		{
			impulseForce = 50f;
			impulseForceSelf = 133.4f;
			weaponName = "WeaponLike";
			SetRocketActive(40, _radiusImpulse, pos);
			return;
		}
		if (!isMine && Device.isPixelGunLow && !Defs.isHunger && !Defs.isDaterRegim)
		{
			try
			{
				weapon = WeaponManager.sharedManager.GunsForPixelGunLow[weapon];
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in _nameWeapon = WeaponManager.sharedManager.GunsForPixelGunLow: " + ex);
			}
		}
		WeaponSounds component = (Resources.Load("Weapons/" + weapon) as GameObject).GetComponent<WeaponSounds>();
		weaponName = component.bazookaExplosionName;
		impulseForce = component.impulseForce;
		impulseForceSelf = component.impulseForceSelf;
		SetRocketActive(component.rocketNum, _radiusImpulse, pos);
	}

	[RPC]
	[PunRPC]
	public void SetRocketActive(int rn, float _radiusImpulse, Vector3 pos)
	{
		if (!isMine)
		{
			base.transform.position = pos;
		}
		rocketNum = rn;
		radiusImpulse = _radiusImpulse;
		if (rockets.Length != 0)
		{
			if (rn >= rockets.Length)
			{
				rn = 0;
			}
			rockets[rn].SetActive(true);
			GetComponent<BoxCollider>().size = rockets[rn].GetComponent<RocketSettings>().sizeBoxCollider;
			GetComponent<BoxCollider>().center = rockets[rn].transform.localPosition + default(Vector3);
			StartCoroutine(StartRocketCoroutine());
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (isRun && rocketNum != 10 && (!isMulti || isMine) && !other.gameObject.CompareTag("CapturePoint") && (isMulti || (!other.gameObject.tag.Equals("Player") && (!(other.transform.parent != null) || !other.transform.parent.gameObject.tag.Equals("Player")))) && (!isMulti || ((!other.gameObject.tag.Equals("Player") || !(other.gameObject == _weaponManager.myPlayer)) && (!(other.transform.parent != null) || !other.transform.parent.gameObject.tag.Equals("Player") || !(other.transform.parent.gameObject == _weaponManager.myPlayer)))) && !other.gameObject.name.Equals("DamageCollider") && ((currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Bomb && currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Ball) || ((!(other.gameObject.transform.parent == null) || other.gameObject.transform.gameObject.CompareTag("Turret")) && (!(other.gameObject.transform.parent != null) || other.gameObject.transform.parent.gameObject.CompareTag("Player") || other.gameObject.transform.parent.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("DamagedExplosion")))))
		{
			if (currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.MegaBullet || (other.gameObject.transform.parent != null && other.gameObject.transform.parent.gameObject.CompareTag("Untagged")) || (other.gameObject.transform.parent == null && other.gameObject.CompareTag("Untagged")))
			{
				KillRocket(other.collider);
			}
			else
			{
				Hit(other.collider);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isRun && rocketNum != 10 && (!isMulti || isMine) && !other.gameObject.name.Equals("DamageCollider") && !other.gameObject.CompareTag("CapturePoint") && !other.gameObject.CompareTag("Area") && (isMulti || (!other.gameObject.tag.Equals("Player") && (!(other.transform.parent != null) || !other.transform.parent.gameObject.tag.Equals("Player")))) && (!isMulti || ((!other.gameObject.tag.Equals("Player") || !(other.gameObject == _weaponManager.myPlayer)) && (!(other.transform.parent != null) || !other.transform.parent.gameObject.tag.Equals("Player") || !(other.transform.parent.gameObject == _weaponManager.myPlayer)))) && ((currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Bomb && currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Ball) || ((!(other.gameObject.transform.parent == null) || other.gameObject.transform.gameObject.CompareTag("Turret")) && (!(other.gameObject.transform.parent != null) || other.gameObject.transform.parent.gameObject.CompareTag("Player") || other.gameObject.transform.parent.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("DamagedExplosion")))))
		{
			if (currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.MegaBullet || (other.gameObject.transform.parent != null && other.gameObject.transform.parent.gameObject.CompareTag("Untagged")) || (other.gameObject.transform.parent == null && other.gameObject.CompareTag("Untagged")))
			{
				KillRocket(other);
			}
			else if (currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.MegaBullet)
			{
				Hit(other);
			}
		}
	}

	[Obfuscation(Exclude = true)]
	private void KillRocket()
	{
		KillRocket(null);
	}

	public void KillRocket(Collider _hitCollision)
	{
		if (isKilled)
		{
			return;
		}
		Hit(_hitCollision);
		isKilled = true;
		if (isMulti)
		{
			if (!isInet)
			{
				GetComponent<NetworkView>().RPC("Collide", RPCMode.All, weaponName, myTransform.position);
			}
			else if (photonView != null)
			{
				photonView.RPC("Collide", PhotonTargets.All, weaponName, myTransform.position);
			}
			else
			{
				Debug.Log("Rocket.KillRocket():    photonView == null");
			}
		}
		else
		{
			Collide(weaponName, myTransform.position);
		}
	}

	[PunRPC]
	[RPC]
	private void Collide(string _weaponName, Vector3 _pos)
	{
		myTransform.position = _pos;
		if (Defs.inComingMessagesCounter <= 5)
		{
			if (!string.IsNullOrEmpty(weaponName))
			{
				_weaponName = weaponName;
			}
			BazookaExplosion(_weaponName);
		}
		DestroyRocket();
	}

	private bool IsDamageByRadius()
	{
		return currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Grenade || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Bomb || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Rocket || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.GravityRocket || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Autoaim || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Ball;
	}

	private bool IsHitInDamageRadius(Vector3 targetPos, Vector3 selfPos, float radius)
	{
		return (targetPos - selfPos).sqrMagnitude < radius * radius;
	}

	public void Hit(Collider hitCollider)
	{
		GameObject gameObject = null;
		if (hitCollider != null)
		{
			gameObject = hitCollider.gameObject;
		}
		Vector3 position = base.transform.position;
		if ((isMulti && !isMine) || (isMulti && _weaponManager.myPlayer == null))
		{
			return;
		}
		if (currentRocketSettings.typeDead == WeaponSounds.TypeDead.like)
		{
			Player_move_c player_move_c = null;
			if (hitCollider != null && hitCollider.gameObject.transform.parent != null && hitCollider.gameObject.transform.parent.gameObject.CompareTag("Player"))
			{
				player_move_c = hitCollider.gameObject.transform.parent.gameObject.GetComponent<SkinName>().playerMoveC;
			}
			else
			{
				float num = 1E+09f;
				for (int i = 0; i < Initializer.players.Count; i++)
				{
					Player_move_c player_move_c2 = Initializer.players[i];
					if (!player_move_c2.Equals(WeaponManager.sharedManager.myPlayerMoveC))
					{
						float num2 = Vector3.SqrMagnitude(Initializer.players[i].myPlayerTransform.position - position);
						if (num2 < radiusDamage * radiusDamage && num2 < num)
						{
							player_move_c = player_move_c2;
						}
					}
				}
			}
			if (player_move_c != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.SendLike(player_move_c);
			}
			return;
		}
		for (int j = 0; j < Initializer.enemiesObj.Count; j++)
		{
			bool flag = false;
			bool flag2 = false;
			if (IsDamageByRadius())
			{
				if (IsHitInDamageRadius(Initializer.enemiesObj[j].transform.position, position, radiusDamage))
				{
					flag = true;
				}
			}
			else if (gameObject != null && (bool)gameObject.transform.parent && gameObject.transform.parent.gameObject.Equals(Initializer.enemiesObj[j]))
			{
				flag = true;
				flag2 = ((hitCollider.GetType() == typeof(SphereCollider)) ? true : false);
			}
			if (!flag)
			{
				continue;
			}
			BaseBot botScriptForObject = BaseBot.GetBotScriptForObject(Initializer.enemiesObj[j].transform);
			float num3 = 1f;
			if (flag2)
			{
				num3 = 2f + EffectsController.AddingForHeadshot(_weaponManager.currentWeaponSounds.categoryNabor - 1);
			}
			float num4 = damage + UnityEngine.Random.Range(damageRange.x, damageRange.y);
			num4 *= num3;
			if (!isMulti)
			{
				if (isSlowdown)
				{
					botScriptForObject.ApplyDebuffByMode(BotDebuffType.DecreaserSpeed, slowdownTime, slowdownCoeff);
				}
				botScriptForObject.GetDamage(0f - num4, WeaponManager.sharedManager.myPlayer.transform, weaponPrefabName, true, flag2);
			}
			else if (!botScriptForObject.IsDeath)
			{
				if (isSlowdown)
				{
					botScriptForObject.ApplyDebuffByMode(BotDebuffType.DecreaserSpeed, slowdownTime, slowdownCoeff);
				}
				botScriptForObject.GetDamageForMultiplayer(0f - num4, null, weaponPrefabName, flag2);
				_weaponManager.myNetworkStartTable.score = GlobalGameController.Score;
				_weaponManager.myNetworkStartTable.SynhScore();
			}
		}
		if (!Defs.isCOOP)
		{
			for (int k = 0; k < Initializer.turretsObj.Count; k++)
			{
				TurretController component = Initializer.turretsObj[k].GetComponent<TurretController>();
				bool flag3 = false;
				if (IsDamageByRadius())
				{
					if (component.isEnemyTurret && IsHitInDamageRadius(Initializer.turretsObj[k].transform.position, position, radiusDamage))
					{
						flag3 = true;
					}
				}
				else if (gameObject != null && component.isEnemyTurret && gameObject.Equals(Initializer.turretsObj[k]))
				{
					flag3 = true;
				}
				if (flag3)
				{
					bool isExplosion = currentRocketSettings.typeDead == WeaponSounds.TypeDead.explosion;
					float num5 = multiplayerDamage;
					_weaponManager.myPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.damageTurret, num5);
					if (Defs.isInet)
					{
						component.MinusLive(num5, isExplosion, WeaponManager.sharedManager.myPlayer.GetComponent<PhotonView>().viewID);
					}
					else
					{
						component.MinusLive(num5, isExplosion, 0, WeaponManager.sharedManager.myPlayer.GetComponent<NetworkView>().viewID);
					}
				}
			}
		}
		for (int l = 0; l < Initializer.damagedObj.Count; l++)
		{
			bool flag4 = false;
			if (IsDamageByRadius())
			{
				if (IsHitInDamageRadius(Initializer.damagedObj[l].transform.position, position, radiusDamage))
				{
					flag4 = true;
				}
			}
			else if (gameObject != null && gameObject.transform.gameObject.Equals(Initializer.damagedObj[l]))
			{
				flag4 = true;
			}
			if (flag4)
			{
				float num6 = damage + UnityEngine.Random.Range(damageRange.x, damageRange.y);
				DamagedExplosionObject.TryApplyDamageToObject(Initializer.damagedObj[l], 0f - num6);
			}
		}
		if (!isMulti)
		{
			return;
		}
		foreach (Player_move_c player in Initializer.players)
		{
			bool flag5 = false;
			flag5 = (isInet ? player.mySkinName.photonView.isMine : player.mySkinName.GetComponent<NetworkView>().isMine);
			if ((!isCOOP || !flag5) && (isCOOP || (!flag5 && (isCompany || Defs.isFlag || Defs.isCapturePoints) && ((!isCompany && !Defs.isFlag && !Defs.isCapturePoints) || player.myCommand == _weaponManager.myTable.GetComponent<NetworkStartTable>().myCommand))))
			{
				continue;
			}
			bool flag6 = false;
			bool flag7 = false;
			if (IsDamageByRadius())
			{
				if (IsHitInDamageRadius(player.myPlayerTransform.position, position, (!flag5) ? radiusDamage : radiusDamageSelf))
				{
					flag6 = true;
				}
			}
			else if (gameObject != null && gameObject.transform.parent != null && gameObject.transform.parent.gameObject.Equals(player.mySkinName.gameObject))
			{
				flag6 = true;
				flag7 = gameObject.CompareTag("HeadCollider");
			}
			if (!flag6)
			{
				continue;
			}
			if (Defs.isDaterRegim)
			{
				if (!flag5 && _weaponManager.currentWeaponSounds.isDaterWeapon && !player.isMechActive)
				{
					player.SendDaterChat(WeaponManager.sharedManager.myPlayerMoveC.mySkinName.NickName, WeaponManager.sharedManager.currentWeaponSounds.daterMessage, player.mySkinName.NickName);
				}
				continue;
			}
			float num7 = 1f;
			if (flag7)
			{
				num7 = 2f + EffectsController.AddingForHeadshot(_weaponManager.currentWeaponSounds.categoryNabor - 1);
			}
			if (flag5)
			{
				float num8 = multiplayerDamage * EffectsController.SelfExplosionDamageDecreaseCoef * num7;
				float num9 = num8 - player.curArmor;
				player.SendStartFlashMine();
				if (num9 < 0f)
				{
					player.curArmor -= num8;
					num9 = 0f;
				}
				else
				{
					player.curArmor = 0f;
				}
				if (player.CurHealth > 0f)
				{
					player.CurHealth -= num9;
					if (player.CurHealth <= 0f)
					{
						player.isSuicided = true;
						player.sendImDeath(player.mySkinName.NickName);
						player.SendImKilled();
					}
					else
					{
						player.IndicateDamage();
					}
				}
				continue;
			}
			int typeDead = (int)currentRocketSettings.typeDead;
			Player_move_c.TypeKills typeKills = ((!flag7) ? currentRocketSettings.typeKilsIconChat : Player_move_c.TypeKills.headshot);
			float num10 = multiplayerDamage * num7;
			_weaponManager.myPlayerMoveC.myScoreController.AddScoreOnEvent(flag7 ? ((!player.isMechActive) ? PlayerEventScoreController.ScoreEvent.damageHead : PlayerEventScoreController.ScoreEvent.damageMechHead) : ((currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Grenade) ? PlayerEventScoreController.ScoreEvent.damageGrenade : ((currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Rocket || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.GravityRocket || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Autoaim) ? PlayerEventScoreController.ScoreEvent.damageExplosion : (player.isMechActive ? PlayerEventScoreController.ScoreEvent.damageMechBody : PlayerEventScoreController.ScoreEvent.damageBody))), num10);
			if (isSlowdown)
			{
				if (isInet)
				{
					player.photonView.RPC("SlowdownRPC", PhotonTargets.All, slowdownCoeff, slowdownTime);
				}
				else
				{
					player.GetComponent<NetworkView>().RPC("SlowdownRPC", RPCMode.All, slowdownCoeff, slowdownTime);
				}
			}
			if (!isInet)
			{
				player.MinusLive(_weaponManager.myPlayer.GetComponent<NetworkView>().viewID, num10, typeKills, typeDead, (rocketNum != 10) ? weaponPrefabName : string.Empty);
			}
			else
			{
				player.MinusLive(_weaponManager.myPlayer.GetComponent<PhotonView>().viewID, num10, typeKills, typeDead, (rocketNum != 10) ? weaponPrefabName : string.Empty);
			}
		}
	}

	[Obfuscation(Exclude = true)]
	private void DestroyRocket()
	{
		if (!isMulti || isMine)
		{
			CancelInvoke("KillRocket");
			RocketStack.sharedController.ReturnRocket(base.gameObject);
		}
		SetRocketDeactive();
	}

	public void BazookaExplosion(string explosionName)
	{
		Vector3 position = base.transform.position;
		string text = ResPath.Combine("Explosions", explosionName);
		GameObject objectFromName = RayAndExplosionsStackController.sharedController.GetObjectFromName(text);
		if (objectFromName != null)
		{
			objectFromName.transform.position = position;
		}
		GameObject myPlayer = WeaponManager.sharedManager.myPlayer;
		if (myPlayer == null)
		{
			return;
		}
		Vector3 dir = myPlayer.transform.position - position;
		float sqrMagnitude = dir.sqrMagnitude;
		float num = radiusImpulse * radiusImpulse;
		if (sqrMagnitude < num)
		{
			ImpactReceiver impactReceiver = myPlayer.GetComponent<ImpactReceiver>();
			if (impactReceiver == null)
			{
				impactReceiver = myPlayer.AddComponent<ImpactReceiver>();
			}
			float num2 = 100f;
			if (radiusImpulse != 0f)
			{
				num2 = Mathf.Sqrt(sqrMagnitude / num);
			}
			float num3 = Mathf.Max(0f, 1f - num2);
			float num4 = ((isMulti && !isMine) ? impulseForce : impulseForceSelf);
			num4 *= num3;
			impactReceiver.AddImpact(dir, num4);
			if ((!isMulti || isMine) && num3 > 0.01f)
			{
				WeaponManager.sharedManager.myPlayerMoveC.isRocketJump = true;
			}
		}
	}

	private void Update()
	{
		if ((!isMulti || isMine) && isRun && (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Autoaim || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.AutoaimBullet) && WeaponManager.sharedManager.myPlayerMoveC != null && !WeaponManager.sharedManager.myPlayerMoveC.isKilled)
		{
			Vector3 pointAutoAim = WeaponManager.sharedManager.myPlayerMoveC.GetPointAutoAim(myTransform.position);
			Vector3 normalized = (pointAutoAim - myTransform.position).normalized;
			GetComponent<Rigidbody>().AddForce(normalized * 27f);
			GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * 15f;
			myTransform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
		}
		if (Defs.isMulti && isStartSynh && ((Defs.isInet && photonView != null && !photonView.isMine) || (!Defs.isInet && !GetComponent<NetworkView>().isMine)))
		{
			if (!Defs.isInet && Vector3.SqrMagnitude(myTransform.position - correctPos) > 300f)
			{
				myTransform.position = correctPos;
			}
			else
			{
				myTransform.position = Vector3.Lerp(myTransform.position, correctPos, Time.deltaTime * 5f);
			}
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		isStartSynh = true;
		if (stream.isWriting)
		{
			stream.SendNext(myTransform.position);
			stream.SendNext(myTransform.rotation);
		}
		else
		{
			correctPos = (Vector3)stream.ReceiveNext();
			myTransform.rotation = (Quaternion)stream.ReceiveNext();
		}
	}

	private bool IsGrenadeRocketNum(int _rocketNum)
	{
		return _rocketNum == 10 || _rocketNum == 40;
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		isStartSynh = true;
		if (stream.isWriting)
		{
			Vector3 value = myTransform.position;
			Quaternion value2 = myTransform.rotation;
			stream.Serialize(ref value);
			stream.Serialize(ref value2);
			return;
		}
		Vector3 value3 = Vector3.zero;
		Quaternion value4 = Quaternion.identity;
		stream.Serialize(ref value3);
		stream.Serialize(ref value4);
		correctPos = value3;
		myTransform.rotation = value4;
		if (isFirstPos)
		{
			isFirstPos = false;
			myTransform.position = value3;
			myTransform.rotation = value4;
		}
	}
}
