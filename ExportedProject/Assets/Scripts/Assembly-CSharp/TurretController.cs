using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Rilisoft;
using RilisoftBot;
using UnityEngine;
using ZeichenKraftwerk;

public sealed class TurretController : MonoBehaviour
{
	public AudioClip turretActivSound;

	public AudioClip turretDeadSound;

	public AudioClip musicDater;

	public SkinnedMeshRenderer turretRenderer;

	public SkinnedMeshRenderer turretExplosionRenderer;

	[NonSerialized]
	public int numUpdate;

	public Rotator rotator;

	private float maxSpeedRotator = -1000f;

	private float downSpeedRotator = 500f;

	public Material[] turretRunMaterials;

	public Material musicBoxMaterial;

	public GameObject hitObj;

	public GameObject killedParticle;

	public GameObject explosionAnimObj;

	public GameObject turretObj;

	public float damage = 1f;

	public float[] damageMultyByTier = new float[5] { 0.1f, 0.3f, 0.5f, 0.7f, 1f };

	public float[] healthMultyByTier = new float[5] { 20f, 40f, 60f, 80f, 100f };

	public Transform tower;

	public Transform gun;

	public AudioClip shotClip;

	public ParticleSystem gunFlash1;

	public ParticleSystem gunFlash2;

	public Transform healthBar;

	public float health = 10000000f;

	public float maxHealth = 10000000f;

	public bool isRunAvailable;

	public bool isRun;

	public GameObject myPlayer;

	public Player_move_c myPlayerMoveC;

	private bool isStartSynh;

	public Transform myTransform;

	public PhotonView photonView;

	private bool isMine;

	public Transform spherePoint;

	public Transform rayGroundPoint;

	public BoxCollider myCollider;

	public Transform target;

	public GameObject isEnemySprite;

	public Transform shotPoint;

	public Transform shotPoint2;

	public bool isKilled;

	public bool isEnemyTurret;

	public int myCommand;

	public NickLabelController myLabel;

	public TextMesh nickLabel;

	private float speedRotateY = 220f;

	private float speedRotateX = 30f;

	private float speedFire = 3f;

	private float radiusZoneMeele = 10f;

	private float radiusZoneMeeleSyrvival = 4f;

	public float maxRadiusScanTarget = 30f;

	private float maxRadiusScanTargetSQR;

	private float idleAlphaY;

	private float idleAlphaX;

	private float idleRotateSpeedX = 20f;

	private float idleRotateSpeedY = 20f;

	private float maxDeltaRotateY = 60f;

	private float maxRotateX = 75f;

	private float minRotateX = -60f;

	private float timerScanTarget = -1f;

	private float maxTimerScanTarget = 1f;

	private float timerScanTargetIdle = -1f;

	private float maxTimerScanTargetIdle = 0.5f;

	private float timerShot;

	private float maxTimerShot = 0.1f;

	private float dissipation = 0.015f;

	public bool dontExecStart;

	private bool inScaning;

	private Rigidbody rigidBody;

	private Vector3 turretMinPos = new Vector3(0f, float.MaxValue, 0f);

	public GameObject redMark;

	public GameObject blueMark;

	private bool isPlayMusicDater;

	private int _nickColorInd;

	private bool _isSetNickLabelText;

	public NetworkView networkView { get; set; }

	private void Awake()
	{
		photonView = PhotonView.Get(this);
		networkView = GetComponent<NetworkView>();
		if ((bool)photonView && photonView.isMine)
		{
			PhotonObjectCacher.AddObject(base.gameObject);
		}
		maxRadiusScanTargetSQR = maxRadiusScanTarget * maxRadiusScanTarget;
		rigidBody = base.transform.GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision col)
	{
		if (isMine && col.gameObject.name == "DeadCollider")
		{
			MinusLive(1000f);
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	private IEnumerator Start()
	{
		if (dontExecStart)
		{
			yield break;
		}
		turretRenderer.material.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
		if (Defs.isMulti)
		{
			if (!Defs.isInet)
			{
				isMine = GetComponent<NetworkView>().isMine;
			}
			else
			{
				isMine = photonView.isMine;
			}
		}
		if (!Defs.isMulti || Defs.isCOOP)
		{
			maxHealth = 18f;
			health = maxHealth;
			radiusZoneMeele = radiusZoneMeeleSyrvival;
		}
		if (!Defs.isMulti || isMine)
		{
			numUpdate = GearManager.CurrentNumberOfUphradesForGear(GearManager.Turret);
			Player_move_c.SetLayerRecursively(base.gameObject, 9);
			if (Defs.isMulti)
			{
				if (!Defs.isInet)
				{
					GetComponent<NetworkView>().RPC("SynchNumUpdateRPC", RPCMode.AllBuffered, numUpdate);
				}
				else
				{
					photonView.RPC("SynchNumUpdateRPC", PhotonTargets.AllBuffered, numUpdate);
				}
			}
		}
		if (Defs.isMulti)
		{
			if (Defs.isInet)
			{
				for (int i = 0; i < Initializer.players.Count; i++)
				{
					if (photonView.ownerId == Initializer.players[i].mySkinName.photonView.ownerId)
					{
						myPlayer = Initializer.players[i].mySkinName.gameObject;
						myPlayerMoveC = myPlayer.GetComponent<SkinName>().playerMoveC;
						break;
					}
				}
			}
		}
		else
		{
			myPlayer = GameObject.FindGameObjectWithTag("Player");
			myPlayerMoveC = myPlayer.GetComponent<SkinName>().playerMoveC;
		}
		if (!isRun)
		{
			while (myPlayer == null || myPlayer.GetComponent<SkinName>().playerMoveC.turretPoint == null)
			{
				yield return null;
			}
			base.transform.parent = myPlayer.GetComponent<SkinName>().playerMoveC.turretPoint.transform;
			yield return null;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
		Initializer.turretsObj.Add(base.gameObject);
	}

	private List<GameObject> GetAllTargets()
	{
		bool flag = ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints;
		int num = 0;
		if (Defs.isMulti && !Defs.isCOOP)
		{
			num += Initializer.players.Count;
			num += Initializer.turretsObj.Count;
		}
		else
		{
			num += Initializer.enemiesObj.Count;
		}
		List<GameObject> list = new List<GameObject>(num);
		if (Defs.isMulti && !Defs.isCOOP)
		{
			if (flag)
			{
				if (WeaponManager.sharedManager.myPlayerMoveC.myCommand == 1)
				{
					for (int i = 0; i < Initializer.redPlayers.Count; i++)
					{
						if (!Initializer.redPlayers[i].isInvisible && !Initializer.redPlayers[i].isKilled)
						{
							list.Add(Initializer.redPlayers[i].mySkinName.gameObject);
						}
					}
				}
				else
				{
					for (int j = 0; j < Initializer.bluePlayers.Count; j++)
					{
						if (!Initializer.bluePlayers[j].isInvisible && !Initializer.bluePlayers[j].isKilled)
						{
							list.Add(Initializer.bluePlayers[j].mySkinName.gameObject);
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < Initializer.players.Count; k++)
				{
					if (!Initializer.players[k].isInvisible && !Initializer.players[k].isKilled)
					{
						list.Add(Initializer.players[k].mySkinName.gameObject);
					}
				}
			}
			for (int l = 0; l < Initializer.turretsObj.Count; l++)
			{
				if (Initializer.turretsObj[l].GetComponent<TurretController>().isEnemyTurret)
				{
					list.Add(Initializer.turretsObj[l]);
				}
			}
		}
		else
		{
			for (int m = 0; m < Initializer.enemiesObj.Count; m++)
			{
				list.Add(Initializer.enemiesObj[m].transform.GetChild(0).gameObject);
			}
		}
		return list;
	}

	private IEnumerator ScanTarget()
	{
		inScaning = true;
		GameObject closestTargetObj = null;
		float closestTarget = float.MaxValue;
		List<GameObject> targets = GetAllTargets();
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] == null || targets[i] == base.gameObject || targets[i] == WeaponManager.sharedManager.myPlayer)
			{
				continue;
			}
			Vector3 enemyDelta = targets[i].transform.position - base.transform.position;
			Vector3 enemyForward = new Vector3(enemyDelta.x, 0f, enemyDelta.z);
			float targetDistance = enemyDelta.sqrMagnitude;
			if (targetDistance < closestTarget && targetDistance < maxRadiusScanTargetSQR && (Defs.isDaterRegim || Vector3.Angle(enemyForward, enemyDelta) < maxRotateX))
			{
				Vector3 popravochka = Vector3.zero;
				BoxCollider _collider = targets[i].GetComponent<BoxCollider>();
				if (_collider != null)
				{
					popravochka = _collider.center;
				}
				Ray ray = new Ray(tower.position, targets[i].transform.position + popravochka - tower.position);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, maxRadiusScanTarget, Tools.AllWithoutDamageCollidersMask) && (hit.collider.gameObject == targets[i] || (hit.collider.gameObject.transform.parent != null && (hit.collider.gameObject.transform.parent.Equals(targets[i].transform) || hit.collider.gameObject.transform.parent.Equals(targets[i].transform.parent)))))
				{
					closestTarget = targetDistance;
					closestTargetObj = targets[i];
				}
			}
			yield return null;
		}
		if (closestTargetObj != null)
		{
			target = closestTargetObj.transform;
		}
		else
		{
			target = null;
		}
		inScaning = false;
	}

	private Transform ScanTargetObs()
	{
		GameObject[] array = null;
		GameObject[] array2;
		if ((Defs.isMulti && Defs.isCOOP) || !Defs.isMulti)
		{
			array2 = Initializer.enemiesObj.ToArray();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array2[i].transform.GetChild(0).gameObject;
			}
		}
		else
		{
			array2 = GameObject.FindGameObjectsWithTag("HeadCollider");
			array = Initializer.turretsObj.ToArray();
		}
		float num = 1000f;
		float num2 = 1000f;
		Transform transform = null;
		Transform transform2 = null;
		float num3 = 1E+09f;
		Transform result = null;
		int num4 = 0 + ((array2 != null) ? array2.Length : 0) + ((array != null) ? array.Length : 0);
		GameObject[] array3 = new GameObject[num4];
		if (array2 != null)
		{
			array2.CopyTo(array3, 0);
		}
		if (array != null)
		{
			array.CopyTo(array3, (array2 != null) ? array2.Length : 0);
		}
		for (int j = 0; j < array3.Length; j++)
		{
			if (Defs.isMulti && (!Defs.isMulti || !Defs.isCOOP) && (!(array3[j].transform.parent != null) || !array3[j].transform.parent.gameObject.CompareTag("Player") || (array3[j].transform.parent.gameObject.Equals(WeaponManager.sharedManager.myPlayer) && !Defs.isDaterRegim) || ((ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints) && array3[j].transform.parent.GetComponent<SkinName>().playerMoveC.myCommand == WeaponManager.sharedManager.myPlayerMoveC.myCommand) || array3[j].transform.parent.GetComponent<SkinName>().playerMoveC.isKilled || !(array3[j].transform.position.y > -500f) || array3[j].transform.parent.GetComponent<SkinName>().playerMoveC.isInvisible) && (!array3[j].CompareTag("Turret") || !array3[j].GetComponent<TurretController>().isEnemyTurret))
			{
				continue;
			}
			Transform transform3 = array3[j].transform;
			float num5 = Vector3.SqrMagnitude(transform3.position - myTransform.position);
			if (num5 < maxRadiusScanTargetSQR && (Defs.isDaterRegim || Mathf.Acos(Mathf.Sqrt((transform3.position.x - myTransform.position.x) * (transform3.position.x - myTransform.position.x) + (transform3.position.z - myTransform.position.z) * (transform3.position.z - myTransform.position.z)) / Vector3.Distance(transform3.position, myTransform.position)) * 180f / (float)Math.PI < maxRotateX))
			{
				if (Defs.isDaterRegim)
				{
					return transform3;
				}
				Vector3 vector = Vector3.zero;
				BoxCollider component = transform3.GetComponent<BoxCollider>();
				if (component != null)
				{
					vector = component.center;
				}
				Ray ray = new Ray(tower.position, transform3.position + vector - tower.position);
				RaycastHit hitInfo;
				bool flag = Physics.Raycast(ray, out hitInfo, maxRadiusScanTarget, Tools.AllWithoutDamageCollidersMask);
				Transform transform4 = hitInfo.transform;
				Transform transform5 = transform3;
				if (flag && (hitInfo.collider.gameObject.transform.Equals(transform3) || (hitInfo.collider.gameObject.transform.parent != null && (hitInfo.collider.gameObject.transform.parent.Equals(transform3) || hitInfo.collider.gameObject.transform.parent.Equals(transform3.parent)))))
				{
					num3 = num5;
					result = transform3;
				}
			}
		}
		return result;
	}

	private void UpdateNickLabelColor()
	{
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
		{
			if (WeaponManager.sharedManager.myPlayerMoveC == null || myPlayerMoveC == null)
			{
				if (_nickColorInd != 0)
				{
					nickLabel.color = Color.white;
					_nickColorInd = 0;
				}
			}
			else if (WeaponManager.sharedManager.myPlayerMoveC.myCommand == myPlayerMoveC.myCommand)
			{
				if (_nickColorInd != 1)
				{
					nickLabel.color = Color.blue;
					_nickColorInd = 1;
				}
			}
			else if (_nickColorInd != 2)
			{
				nickLabel.color = Color.red;
				_nickColorInd = 2;
			}
		}
		else if (Defs.isDaterRegim)
		{
			if (_nickColorInd != 0)
			{
				nickLabel.color = Color.white;
				_nickColorInd = 0;
			}
		}
		else if (Defs.isCOOP || (myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC == myPlayerMoveC))
		{
			if (_nickColorInd != 1)
			{
				nickLabel.color = Color.blue;
				_nickColorInd = 1;
			}
		}
		else if (_nickColorInd != 2)
		{
			nickLabel.color = Color.red;
			_nickColorInd = 2;
		}
	}

	private void Update()
	{
		if (dontExecStart)
		{
			return;
		}
		if (Defs.isMulti && myPlayerMoveC != null && !_isSetNickLabelText)
		{
			nickLabel.text = FilterBadWorld.FilterString(myPlayerMoveC.mySkinName.NickName);
		}
		UpdateNickLabelColor();
		if (isRun && healthBar != null)
		{
			healthBar.localScale = new Vector3((!(health > 0f)) ? 0f : (health / maxHealth), 1f, 1f);
		}
		SetStateIsEnemyTurret();
		if (isEnemySprite != null && isEnemySprite.activeSelf != isEnemyTurret)
		{
			isEnemySprite.SetActive(isEnemyTurret);
		}
		if (rotator != null && rotator.eulersPerSecond.z < -200f)
		{
			rotator.eulersPerSecond = new Vector3(0f, 0f, rotator.eulersPerSecond.z + downSpeedRotator * Time.deltaTime);
		}
		if (Defs.isDaterRegim && isPlayMusicDater)
		{
			tower.Rotate(new Vector3(0f, 0f, 180f * Time.deltaTime));
			gun.Rotate(new Vector3(180f * Time.deltaTime, 0f, 0f));
		}
		if (Defs.isMulti && !isMine)
		{
			return;
		}
		if (Defs.isMulti && isMine && WeaponManager.sharedManager.myPlayer == null)
		{
			if (!Defs.isInet)
			{
				Network.Destroy(base.gameObject);
			}
			else
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
			return;
		}
		if (!isRun)
		{
			bool flag = false;
			Ray ray = new Ray(rayGroundPoint.position, Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1f, Tools.AllWithoutDamageCollidersMask) && hitInfo.distance > 0.05f && hitInfo.distance < 0.7f && !Physics.CheckSphere(spherePoint.position, 1.1f, Tools.AllWithoutMyPlayerMask))
			{
				isRunAvailable = true;
				turretRenderer.materials[0].SetColor("_TintColor", new Color(1f, 1f, 1f, 0.08f));
				if (InGameGUI.sharedInGameGUI != null)
				{
					InGameGUI.sharedInGameGUI.runTurrelButton.GetComponent<UIButton>().isEnabled = true;
				}
			}
			else
			{
				isRunAvailable = false;
				turretRenderer.materials[0].SetColor("_TintColor", new Color(1f, 0f, 0f, 0.08f));
				if (InGameGUI.sharedInGameGUI != null)
				{
					InGameGUI.sharedInGameGUI.runTurrelButton.GetComponent<UIButton>().isEnabled = false;
				}
			}
			return;
		}
		if (isKilled)
		{
			if (gun.rotation.x > minRotateX)
			{
				gun.Rotate(speedRotateX * Time.deltaTime, 0f, 0f);
			}
			return;
		}
		if (target != null && (target.position.y < -500f || (!Defs.isDaterRegim && target.CompareTag("Player") && target.GetComponent<SkinName>().playerMoveC.isInvisible)))
		{
			target = null;
		}
		if (target == null)
		{
			if (Defs.isDaterRegim && isPlayMusicDater)
			{
				PlayMusic(false);
				if (!Defs.isInet)
				{
					GetComponent<NetworkView>().RPC("PlayMusic", RPCMode.Others, false);
				}
				else
				{
					photonView.RPC("PlayMusic", PhotonTargets.Others, false);
				}
			}
			timerScanTargetIdle -= Time.deltaTime;
			if (timerScanTargetIdle < 0f)
			{
				timerScanTargetIdle = maxTimerScanTargetIdle;
				if (!inScaning)
				{
					StartCoroutine(ScanTarget());
				}
			}
			if (!Defs.isDaterRegim)
			{
				if (Mathf.Abs(idleAlphaY) < 0.5f)
				{
					idleAlphaY = UnityEngine.Random.Range(-1f * maxDeltaRotateY / 2f, maxDeltaRotateY / 2f);
				}
				else
				{
					float num = Time.deltaTime * idleRotateSpeedY * Mathf.Abs(idleAlphaY) / idleAlphaY;
					idleAlphaY -= num;
					tower.localRotation = Quaternion.Euler(new Vector3(0f, tower.localRotation.eulerAngles.y + num, 0f));
				}
				if (Mathf.Abs(gun.localRotation.eulerAngles.x) > 1f)
				{
					gun.Rotate((float)((!(gun.localRotation.eulerAngles.x < 180f)) ? 1 : (-1)) * speedRotateX * Time.deltaTime, 0f, 0f);
				}
			}
			return;
		}
		if (Defs.isDaterRegim)
		{
			if (Defs.isDaterRegim && !isPlayMusicDater)
			{
				PlayMusic(true);
				if (!Defs.isInet)
				{
					GetComponent<NetworkView>().RPC("PlayMusic", RPCMode.Others, true);
				}
				else
				{
					photonView.RPC("PlayMusic", PhotonTargets.Others, true);
				}
			}
		}
		else
		{
			bool flag2 = false;
			Vector2 to = new Vector2(target.position.x, target.position.z) - new Vector2(tower.position.x, tower.position.z);
			float deltaAngles = GetDeltaAngles(tower.rotation.eulerAngles.y, Mathf.Abs(to.x) / to.x * Vector2.Angle(Vector2.up, to));
			float num2 = (0f - speedRotateY) * Time.deltaTime * Mathf.Abs(deltaAngles) / deltaAngles;
			if (Mathf.Abs(deltaAngles) < 10f)
			{
				flag2 = true;
			}
			if (Mathf.Abs(num2) > Mathf.Abs(deltaAngles))
			{
				num2 = 0f - deltaAngles;
			}
			if (Mathf.Abs(num2) > 0.001f)
			{
				tower.Rotate(0f, num2, 0f);
			}
			Vector3 vector = Vector3.zero;
			BoxCollider component = target.GetComponent<BoxCollider>();
			if (component != null)
			{
				vector = component.center;
			}
			float angle = -180f * Mathf.Atan((target.position.y + vector.y - tower.position.y) / Vector3.Distance(target.position + vector, myTransform.position)) / (float)Math.PI;
			float deltaAngles2 = GetDeltaAngles(gun.rotation.eulerAngles.x, angle);
			num2 = (0f - speedRotateX) * Time.deltaTime * Mathf.Abs(deltaAngles2) / deltaAngles2;
			if (Mathf.Abs(num2) > Mathf.Abs(deltaAngles2))
			{
				num2 = 0f - deltaAngles2;
			}
			if (num2 > 0f && gun.rotation.x > maxRotateX)
			{
				num2 = 0f;
			}
			if (num2 < 0f && gun.rotation.x < minRotateX)
			{
				num2 = 0f;
			}
			if (Mathf.Abs(num2) > 0.001f)
			{
				gun.Rotate(num2, 0f, 0f);
			}
			if (flag2)
			{
				timerShot -= Time.deltaTime;
				if (timerShot < 0f)
				{
					if (!Defs.isMulti)
					{
						ShotRPC();
					}
					else if (!Defs.isInet)
					{
						GetComponent<NetworkView>().RPC("ShotRPC", RPCMode.All);
					}
					else
					{
						photonView.RPC("ShotRPC", PhotonTargets.All);
					}
					timerShot = maxTimerShot;
				}
			}
		}
		timerScanTargetIdle -= Time.deltaTime;
		if (timerScanTargetIdle < 0f)
		{
			timerScanTargetIdle = maxTimerScanTargetIdle;
			if (!inScaning)
			{
				StartCoroutine(ScanTarget());
			}
		}
		if (!rigidBody.isKinematic)
		{
			if (base.transform.position.y < turretMinPos.y)
			{
				turretMinPos = base.transform.position;
			}
			else
			{
				base.transform.position = turretMinPos;
			}
		}
	}

	[RPC]
	[PunRPC]
	public void SynchNumUpdateRPC(int _numUpdate)
	{
		numUpdate = _numUpdate;
		if (Defs.isMulti && !Defs.isCOOP)
		{
			maxHealth = healthMultyByTier[_numUpdate];
			health = maxHealth;
		}
	}

	private void SetStateIsEnemyTurret()
	{
		bool flag = isEnemyTurret;
		isEnemyTurret = false;
		if (Defs.isMulti && (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints))
		{
			if (myPlayer != null && WeaponManager.sharedManager.myPlayerMoveC != null && myPlayer.GetComponent<SkinName>().playerMoveC.myCommand != WeaponManager.sharedManager.myPlayerMoveC.myCommand)
			{
				isEnemyTurret = true;
			}
		}
		else if (Defs.isMulti && !isMine)
		{
			isEnemyTurret = true;
		}
	}

	[PunRPC]
	[RPC]
	public void ShotRPC()
	{
		if (shotPoint2 == null || shotPoint == null)
		{
			return;
		}
		if (rotator != null)
		{
			rotator.eulersPerSecond = new Vector3(0f, 0f, maxSpeedRotator);
		}
		if (Defs.isSoundFX)
		{
			GetComponent<AudioSource>().PlayOneShot(shotClip);
		}
		if (gunFlash1 != null)
		{
			gunFlash1.enableEmission = true;
			gunFlash1.Play();
		}
		CancelInvoke("StopGunFlash");
		Invoke("StopGunFlash", maxTimerShot * 1.1f);
		if (Defs.isMulti && !isMine)
		{
			return;
		}
		Vector3 direction = new Vector3(shotPoint2.position.x - shotPoint.position.x + UnityEngine.Random.Range(0f - dissipation, dissipation), shotPoint2.position.y - shotPoint.position.y + UnityEngine.Random.Range(0f - dissipation, dissipation), shotPoint2.position.z - shotPoint.position.z + UnityEngine.Random.Range(0f - dissipation, dissipation));
		Ray ray = new Ray(shotPoint.position, direction);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 100f, Tools.AllWithoutDamageCollidersMask) || (Defs.isMulti && !(WeaponManager.sharedManager.myPlayer != null)))
		{
			return;
		}
		bool flag = false;
		if ((Defs.isMulti && Defs.isCOOP) || !Defs.isMulti)
		{
			hitObj = hitInfo.collider.gameObject;
			if (hitInfo.collider.gameObject.transform.parent != null && hitInfo.collider.gameObject.transform.parent.CompareTag("Enemy"))
			{
				HitZombie(hitInfo.collider.gameObject);
				flag = true;
			}
		}
		else
		{
			if (hitInfo.collider.gameObject.transform.parent != null && hitInfo.collider.gameObject.transform.parent.CompareTag("Player") && hitInfo.collider.gameObject.transform.parent.gameObject != WeaponManager.sharedManager.myPlayer)
			{
				Player_move_c playerMoveC = hitInfo.collider.gameObject.transform.parent.GetComponent<SkinName>().playerMoveC;
				float minus = damageMultyByTier[numUpdate];
				if (Defs.isInet)
				{
					playerMoveC.MinusLive(WeaponManager.sharedManager.myPlayer.GetComponent<PhotonView>().viewID, minus, Player_move_c.TypeKills.turret, 0, string.Empty, photonView.viewID);
				}
				else
				{
					playerMoveC.MinusLive(WeaponManager.sharedManager.myPlayer.GetComponent<NetworkView>().viewID, minus, Player_move_c.TypeKills.turret, 0, string.Empty, GetComponent<NetworkView>().viewID);
				}
				flag = true;
			}
			if (hitInfo.collider.gameObject != null && hitInfo.collider.gameObject.CompareTag("Turret"))
			{
				TurretController component = hitInfo.collider.gameObject.GetComponent<TurretController>();
				float dm = damageMultyByTier[numUpdate];
				if (Defs.isInet)
				{
					component.MinusLive(dm, WeaponManager.sharedManager.myPlayer.GetComponent<PhotonView>().viewID);
				}
				else
				{
					component.MinusLive(dm, 0, WeaponManager.sharedManager.myPlayer.GetComponent<NetworkView>().viewID);
				}
				flag = true;
			}
		}
		if (Defs.isMulti)
		{
			if (!Defs.isInet)
			{
				WeaponManager.sharedManager.myPlayerMoveC.GetComponent<NetworkView>().RPC("HoleRPC", RPCMode.All, flag, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
			}
			else
			{
				WeaponManager.sharedManager.myPlayerMoveC.photonView.RPC("HoleRPC", PhotonTargets.All, flag, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
			}
		}
	}

	private void HitZombie(GameObject zmb)
	{
		BaseBot botScriptForObject = BaseBot.GetBotScriptForObject(zmb.transform.parent);
		if (!Defs.isMulti)
		{
			botScriptForObject.GetDamage(0f - damage, myTransform);
		}
		else if (Defs.isCOOP && !botScriptForObject.IsDeath)
		{
			botScriptForObject.GetDamageForMultiplayer(0f - damage, null);
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().score = GlobalGameController.Score;
			WeaponManager.sharedManager.myNetworkStartTable.SynhScore();
		}
	}

	[PunRPC]
	[RPC]
	private void PlayMusic(bool isPlay)
	{
		if (isPlayMusicDater == isPlay)
		{
			return;
		}
		isPlayMusicDater = isPlay;
		if (isPlay)
		{
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().loop = true;
				GetComponent<AudioSource>().clip = musicDater;
				GetComponent<AudioSource>().Play();
			}
		}
		else
		{
			GetComponent<AudioSource>().Stop();
		}
	}

	[Obfuscation(Exclude = true)]
	private void StopGunFlash()
	{
		gunFlash1.enableEmission = false;
	}

	private float GetDeltaAngles(float angle1, float angle2)
	{
		if (angle1 < 0f)
		{
			angle1 += 360f;
		}
		if (angle2 < 0f)
		{
			angle2 += 360f;
		}
		float num = angle1 - angle2;
		if (Mathf.Abs(num) > 180f)
		{
			num = ((!(angle1 > angle2)) ? (num + 360f) : (num - 360f));
		}
		return num;
	}

	public void StartTurret()
	{
		if (Defs.isMulti && isMine)
		{
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("StartTurretRPC", RPCMode.AllBuffered);
			}
			else
			{
				photonView.RPC("StartTurretRPC", PhotonTargets.AllBuffered);
			}
		}
		else if (!Defs.isMulti)
		{
			StartTurretRPC();
		}
		myCollider.enabled = true;
		rigidBody.isKinematic = false;
		rigidBody.useGravity = true;
		Invoke("SetNoUseGravityInvoke", 5f);
	}

	[Obfuscation(Exclude = true)]
	private void SetNoUseGravityInvoke()
	{
		rigidBody.useGravity = false;
		rigidBody.isKinematic = true;
		base.transform.GetComponent<BoxCollider>().isTrigger = true;
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		if (isMine)
		{
			networkView.RPC("SynchHealth", player, health);
			if (Defs.isDaterRegim)
			{
				networkView.RPC("PlayMusic", player, isPlayMusicDater);
			}
		}
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (isMine)
		{
			photonView.RPC("SynchHealth", player, health);
			if (Defs.isDaterRegim)
			{
				photonView.RPC("PlayMusic", player, isPlayMusicDater);
			}
		}
	}

	[PunRPC]
	[RPC]
	public void SynchHealth(float _health)
	{
		if (health > _health)
		{
			health = _health;
		}
	}

	private IEnumerator FlashRed()
	{
		turretRenderer.material.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
		yield return null;
		turretRenderer.material.SetColor("_ColorRili", new Color(1f, 0f, 0f, 1f));
		yield return new WaitForSeconds(0.1f);
		turretRenderer.material.SetColor("_ColorRili", new Color(1f, 1f, 1f, 1f));
	}

	public void MinusLive(float dm, [Optional] int idKillerPhoton, [Optional] NetworkViewID idKillerLocal)
	// optional null
	{
		MinusLive(dm, false, idKillerPhoton, idKillerLocal);
	}

	public void MinusLive(float dm, bool isExplosion, int idKillerPhoton, [Optional] NetworkViewID idKillerLocal)
	// optional null
	{
		if (Defs.isDaterRegim || !isRun)
		{
			return;
		}
		if (Defs.isMulti)
		{
			health -= dm;
			if (health < 0f)
			{
				ImKilledRPCWithExplosion(isExplosion);
				dm = 10000f;
			}
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("MinusLiveRPCLocal", RPCMode.All, dm, isExplosion, idKillerLocal);
			}
			else
			{
				photonView.RPC("MinusLiveRPC", PhotonTargets.All, dm, isExplosion, idKillerPhoton);
			}
		}
		else
		{
			MinusLiveReal(dm, isExplosion);
		}
	}

	[RPC]
	[PunRPC]
	public void MinusLiveRPC(float dm, int idKillerPhoton)
	{
		MinusLiveReal(dm, false, idKillerPhoton);
	}

	[PunRPC]
	[RPC]
	public void MinusLiveRPC(float dm, bool isExplosion, int idKillerPhoton)
	{
		MinusLiveReal(dm, isExplosion, idKillerPhoton);
	}

	[PunRPC]
	[RPC]
	public void MinusLiveRPCLocal(float dm, bool isExplosion, NetworkViewID idKillerLocal)
	{
		MinusLiveReal(dm, isExplosion, 0, idKillerLocal);
	}

	public void MinusLiveReal([Optional] float dm, [Optional] bool isExplosion, [Optional] int idKillerPhoton, [Optional] NetworkViewID idKillerLocal)
		// optional null
	{
		StopCoroutine(FlashRed());
		StartCoroutine(FlashRed());
		if (isKilled || (Defs.isMulti && !isMine))
		{
			return;
		}
		health -= dm;
		if (Defs.isMulti)
		{
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("SynchHealth", RPCMode.Others, health);
			}
			else
			{
				photonView.RPC("SynchHealth", PhotonTargets.Others, health);
			}
		}
		if (!(health < 0f))
		{
			return;
		}
		health = 0f;
		if (Defs.isMulti)
		{
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("ImKilledRPCWithExplosion", RPCMode.AllBuffered, isExplosion);
				GetComponent<NetworkView>().RPC("MeKillRPCLocal", RPCMode.All, idKillerLocal);
			}
			else
			{
				photonView.RPC("ImKilledRPCWithExplosion", PhotonTargets.AllBuffered, isExplosion);
				photonView.RPC("MeKillRPC", PhotonTargets.All, idKillerPhoton);
			}
		}
		else
		{
			ImKilledRPCWithExplosion(isExplosion);
		}
	}

	[RPC]
	[PunRPC]
	public void MeKillRPC(int idKillerPhoton)
	{
		string nick = string.Empty;
		foreach (Player_move_c player in Initializer.players)
		{
			if (player.mySkinName.photonView != null && player.mySkinName.photonView.viewID == idKillerPhoton)
			{
				nick = player.mySkinName.NickName;
				if (player.Equals(WeaponManager.sharedManager.myPlayerMoveC))
				{
					WeaponManager.sharedManager.myPlayerMoveC.ImKill(idKillerPhoton, 9);
				}
				break;
			}
		}
		MeKill(nick);
	}

	[RPC]
	[PunRPC]
	public void MeKillRPCLocal(NetworkViewID idKillerLocal)
	{
		string nick = string.Empty;
		foreach (Player_move_c player in Initializer.players)
		{
			if (player.mySkinName.GetComponent<NetworkView>() != null && player.mySkinName.GetComponent<NetworkView>().viewID.Equals(idKillerLocal))
			{
				nick = player.mySkinName.NickName;
				if (player.Equals(WeaponManager.sharedManager.myPlayerMoveC))
				{
					WeaponManager.sharedManager.myPlayerMoveC.ImKill(idKillerLocal, 9);
				}
				break;
			}
		}
		MeKill(nick);
	}

	public void MeKill(string _nick)
	{
		if (WeaponManager.sharedManager.myPlayerMoveC != null && myPlayer != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(_nick, 9, myPlayer.GetComponent<SkinName>().NickName);
		}
	}

	[PunRPC]
	[RPC]
	public void ImKilledRPC()
	{
		ImKilledRPCWithExplosion(false);
	}

	[RPC]
	[PunRPC]
	public void ImKilledRPCWithExplosion(bool isExplosion)
	{
		isKilled = true;
		nickLabel.gameObject.SetActive(false);
		isExplosion = true;
		if (Defs.isSoundFX)
		{
			GetComponent<AudioSource>().PlayOneShot(turretDeadSound);
		}
		if (isExplosion)
		{
			explosionAnimObj.SetActive(true);
			turretObj.SetActive(false);
		}
		else
		{
			killedParticle.SetActive(true);
		}
		Invoke("DestroyTurrel", 2f);
	}

	[Obfuscation(Exclude = true)]
	private void DestroyTurrel()
	{
		if (Defs.isMulti)
		{
			if (isMine)
			{
				if (!Defs.isInet)
				{
					Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
					Network.Destroy(base.gameObject);
				}
				else
				{
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
			else
			{
				base.transform.position = new Vector3(-1000f, -1000f, -1000f);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	[PunRPC]
	[RPC]
	public void StartTurretRPC()
	{
		nickLabel.gameObject.SetActive(true);
		myCollider.enabled = true;
		base.transform.parent = null;
		if (Defs.isSoundFX)
		{
			GetComponent<AudioSource>().PlayOneShot(turretActivSound);
		}
		Player_move_c.SetLayerRecursively(base.gameObject, LayerMask.NameToLayer("Default"));
		if (Defs.isInet)
		{
			photonView.synchronization = ViewSynchronization.UnreliableOnChange;
		}
		else
		{
			GetComponent<NetworkView>().stateSynchronization = NetworkStateSynchronization.ReliableDeltaCompressed;
		}
		isRun = true;
		if (!Defs.isDaterRegim)
		{
			turretRenderer.material = turretRunMaterials[numUpdate];
		}
		else
		{
			turretRenderer.material = musicBoxMaterial;
		}
		if (turretExplosionRenderer != null)
		{
			turretExplosionRenderer.material = turretRunMaterials[numUpdate];
		}
	}

	private void OnDestroy()
	{
		if (!Defs.isMulti || isMine)
		{
			PotionsController.sharedController.DeActivePotion(GearManager.Turret, null, false);
		}
		PhotonObjectCacher.RemoveObject(base.gameObject);
		Initializer.turretsObj.Remove(base.gameObject);
	}

	public Vector3 GetHeadPoint()
	{
		Vector3 position = base.transform.position;
		position.y += myCollider.size.y - 0.5f;
		return position;
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
				myPlayer = Initializer.players[i].mySkinName.gameObject;
				myPlayerMoveC = myPlayer.GetComponent<SkinName>().playerMoveC;
				break;
			}
		}
	}
}
