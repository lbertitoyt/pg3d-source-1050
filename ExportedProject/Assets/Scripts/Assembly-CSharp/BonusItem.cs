using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class BonusItem : MonoBehaviour
{
	private Player_move_c playerMoveC;

	private GameObject player;

	public bool isActive = true;

	public AudioClip[] itemSounds = new AudioClip[Enum.GetValues(typeof(BonusController.TypeBonus)).Length];

	public GameObject[] itemMdls = new GameObject[Enum.GetValues(typeof(BonusController.TypeBonus)).Length];

	private bool isMulti;

	private bool isInet;

	private bool isCOOP;

	private WeaponManager _weaponManager;

	public BonusController.TypeBonus type;

	public double expireTime = -1.0;

	public bool isTimeBonus;

	public int mySpawnNumber = -1;

	public int myIndex;

	private void Awake()
	{
		isMulti = Defs.isMulti;
		isInet = Defs.isInet;
		isCOOP = Defs.isCOOP;
	}

	private void Start()
	{
		_weaponManager = WeaponManager.sharedManager;
		if (!Defs.isMulti)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				playerMoveC = player.GetComponent<SkinName>().playerMoveC;
			}
			else
			{
				Debug.LogWarning("BonusItem.Start(): player == null");
			}
		}
		else
		{
			player = _weaponManager.myPlayer;
			playerMoveC = _weaponManager.myPlayerMoveC;
		}
	}

	public void ActivateBonus(BonusController.TypeBonus type, Vector3 position, double expireTime, int zoneNumber, int index)
	{
		if (!isActive)
		{
			this.type = type;
			base.transform.position = position;
			SetModel();
			mySpawnNumber = zoneNumber;
			myIndex = index;
			if (expireTime != -1.0)
			{
				isTimeBonus = true;
				this.expireTime = expireTime;
			}
			else
			{
				isTimeBonus = false;
				this.expireTime = -1.0;
			}
			isActive = true;
		}
	}

	private void SetModel(bool show = true)
	{
		itemMdls[(int)type].SetActive(show);
	}

	public void DeactivateBonus()
	{
		isActive = false;
		base.transform.position = Vector3.down * 100f;
		SetModel(false);
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		if (isMulti)
		{
			if (player == null)
			{
				player = _weaponManager.myPlayer;
				playerMoveC = _weaponManager.myPlayerMoveC;
			}
		}
		else if (player == null || playerMoveC == null || playerMoveC.isKilled || (playerMoveC.inGameGUI != null && (playerMoveC.inGameGUI.pausePanel.activeSelf || playerMoveC.inGameGUI.blockedCollider.activeSelf)) || ShopNGUIController.GuiActive || BankController.Instance.uiRoot.gameObject.activeInHierarchy)
		{
			return;
		}
		if (isTimeBonus && ((PhotonNetwork.isMasterClient && Defs.isInet && PhotonNetwork.time > expireTime) || (!Defs.isInet && Network.time > expireTime)))
		{
			BonusController.sharedController.RemoveBonus(myIndex);
		}
		else
		{
			if (player == null || playerMoveC == null || playerMoveC.isKilled)
			{
				return;
			}
			float num = Vector3.SqrMagnitude(base.transform.position - player.transform.position);
			if (!(num < 4f))
			{
				return;
			}
			bool flag = false;
			switch (type)
			{
			case BonusController.TypeBonus.Ammo:
				if (playerMoveC.NeedAmmo())
				{
					if (!_weaponManager.AddAmmo())
					{
						GlobalGameController.Score += Defs.ScoreForSurplusAmmo;
					}
					flag = true;
					if (Defs.isMulti)
					{
						playerMoveC.ShowBonuseParticle(Player_move_c.TypeBonuses.Ammo);
					}
				}
				break;
			case BonusController.TypeBonus.Health:
				if (playerMoveC.CurHealth == playerMoveC.MaxHealth)
				{
					if (!isMulti || isCOOP)
					{
						GlobalGameController.Score += 100;
					}
					flag = true;
					break;
				}
				playerMoveC.CurHealth += playerMoveC.MaxHealth / 2f;
				if (playerMoveC.CurHealth > playerMoveC.MaxHealth)
				{
					playerMoveC.CurHealth = playerMoveC.MaxHealth;
				}
				flag = true;
				if (Defs.isMulti)
				{
					playerMoveC.ShowBonuseParticle(Player_move_c.TypeBonuses.Health);
				}
				break;
			case BonusController.TypeBonus.Armor:
				if (playerMoveC.curArmor + 1f > playerMoveC.MaxArmor)
				{
					if (!isMulti || isCOOP)
					{
						GlobalGameController.Score += 100;
					}
				}
				else
				{
					playerMoveC.curArmor += 1f;
				}
				flag = true;
				if (Defs.isMulti)
				{
					playerMoveC.ShowBonuseParticle(Player_move_c.TypeBonuses.Armor);
				}
				break;
			case BonusController.TypeBonus.Grenade:
				if (WeaponManager.sharedManager.myPlayerMoveC.GrenadeCount < 10)
				{
					WeaponManager.sharedManager.myPlayerMoveC.GrenadeCount++;
				}
				if (Defs.isMulti)
				{
					playerMoveC.ShowBonuseParticle(Player_move_c.TypeBonuses.Grenade);
				}
				flag = true;
				break;
			case BonusController.TypeBonus.Mech:
				if (playerMoveC.myCurrentWeaponSounds != null && !playerMoveC.myCurrentWeaponSounds.isGrenadeWeapon && !playerMoveC.isGrenadePress && !ShopNGUIController.GuiActive && (Pauser.sharedPauser == null || !Pauser.sharedPauser.paused) && !playerMoveC.isImmortality)
				{
					PotionsController.sharedController.ActivatePotion("Mech", playerMoveC, new Dictionary<string, object>(), true);
					flag = true;
				}
				break;
			case BonusController.TypeBonus.JetPack:
				PotionsController.sharedController.ActivatePotion("Jetpack", playerMoveC, new Dictionary<string, object>(), true);
				flag = true;
				break;
			case BonusController.TypeBonus.Invisible:
				PotionsController.sharedController.ActivatePotion("InvisibilityPotion", playerMoveC, new Dictionary<string, object>(), true);
				flag = true;
				break;
			case BonusController.TypeBonus.Turret:
				if (playerMoveC.myCurrentWeaponSounds != null && !playerMoveC.myCurrentWeaponSounds.isGrenadeWeapon && InGameGUI.sharedInGameGUI != null && !InGameGUI.sharedInGameGUI.turretPanel.activeSelf && !playerMoveC.isGrenadePress && !ShopNGUIController.GuiActive && (Pauser.sharedPauser == null || !Pauser.sharedPauser.paused) && !playerMoveC.isImmortality)
				{
					if (PotionsController.sharedController.PotionIsActive("Turret"))
					{
						PotionsController.sharedController.ActivatePotion("Turret", playerMoveC, new Dictionary<string, object>(), true);
					}
					else
					{
						InGameGUI.sharedInGameGUI.ShowTurretInterface();
					}
					flag = true;
				}
				break;
			}
			if (flag)
			{
				if (Defs.isSoundFX)
				{
					playerMoveC.gameObject.GetComponent<AudioSource>().PlayOneShot(itemSounds[(int)type]);
				}
				BonusController.sharedController.RemoveBonus(myIndex);
			}
		}
	}
}
