using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rilisoft.MiniJson;
using UnityEngine;

public class TouchPadController : MonoBehaviour
{
	public const string GRENADE_BUY_NORMAL_SPRITE_NAME = "grenade_btn";

	private const string GRENADE_BUY_PRESSED_SPRITE_NAME = "grenade_btn_n";

	public const string LIKE_BUY_NORMAL_SPRITE_NAME = "grenade_like_btn";

	private const string LIKE_BUY_PRESSED_SPRITE_NAME = "grenade_like_btn_n";

	public bool grenadePressed;

	public GrenadeButton grenadeButton;

	public bool jumpPressed;

	public Transform fireSprite;

	public Transform jumpSprite;

	public Transform reloadSpirte;

	public Transform zoomSprite;

	public bool hasAmmo = true;

	public bool _isFirstFrame = true;

	public GameObject jetPackIcon;

	public GameObject jumpIcon;

	private Rect grenadeRect = default(Rect);

	private bool isInvokeGrenadePress;

	private UISprite reloadUISprite;

	public bool isShooting;

	public bool isShootingPressure;

	private bool isHunger;

	private Player_move_c move;

	private HungerGameController hungerGameController;

	private Rect fireRect = default(Rect);

	private Rect jumpRect = default(Rect);

	private Rect reloadRect;

	private Rect zoomRect;

	private Rect moveRect;

	private bool _joyActive = true;

	private bool _isBuyGrenadePressed;

	private CameraTouchControlScheme _touchControlScheme;

	private bool _shouldRecalcRects;

	private int framesCount;

	private int MoveTouchID = -1;

	private Vector2 pastPos = Vector2.zero;

	private Vector2 pastDelta = Vector2.zero;

	private Vector2 compDeltas = Vector2.zero;

	public CameraTouchControlScheme touchControlScheme
	{
		get
		{
			return _touchControlScheme;
		}
		set
		{
			_touchControlScheme = value;
			_touchControlScheme.Reset();
		}
	}

	public void MakeInactive()
	{
		jumpPressed = false;
		isShooting = false;
		isShootingPressure = false;
		Reset();
		HasAmmo();
		_joyActive = false;
	}

	public void MakeActive()
	{
		_joyActive = true;
	}

	private void Awake()
	{
		reloadUISprite = reloadSpirte.GetComponent<UISprite>();
		isHunger = Defs.isHunger;
		if (!Device.isPixelGunLow)
		{
			_touchControlScheme = new CameraTouchControlScheme_CleanNGUI();
		}
		else if (Defs.isTouchControlSmoothDump)
		{
			_touchControlScheme = new CameraTouchControlScheme_SmoothDump();
		}
		else
		{
			_touchControlScheme = new CameraTouchControlScheme_CleanNGUI();
		}
	}

	private void OnEnable()
	{
		isShooting = false;
		isShootingPressure = false;
		if (_shouldRecalcRects)
		{
			Invoke("ReCalcRects", 0.1f);
		}
		_shouldRecalcRects = false;
		StartCoroutine(_SetIsFirstFrame());
	}

	public static int GetGrenadeCount()
	{
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			return WeaponManager.sharedManager.myPlayerMoveC.GrenadeCount;
		}
		return 0;
	}

	private static bool IsButtonGrenadeVisible()
	{
		return ((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive && !Defs.isTurretWeapon) || InGameGUI.sharedInGameGUI.playerMoveC == null) && !Defs.isZooming;
	}

	private bool IsUseGrenadeActive()
	{
		return IsButtonGrenadeVisible() && Defs.isGrenateFireEnable && (!isHunger || hungerGameController.isGo) && GetGrenadeCount() > 0;
	}

	public static bool IsBuyGrenadeActive()
	{
		if (IsButtonGrenadeVisible() && Defs.isDaterRegim && WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC.GrenadeCount <= 0)
		{
			return true;
		}
		return false;
	}

	private void SetSpritesState()
	{
		SetGrenadeUISpriteState();
		if (!(WeaponManager.sharedManager != null) || (!WeaponManager.sharedManager.currentWeaponSounds.gameObject.name.Equals("WeaponGrenade(Clone)") && !WeaponManager.sharedManager.currentWeaponSounds.gameObject.name.Equals("WeaponLike(Clone)")))
		{
			jumpSprite.gameObject.SetActive((Defs.isJumpAndShootButtonOn || !Defs.isUse3DTouch) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0 || TrainingController.stepTraining >= TrainingState.GetTheGun));
			bool flag = TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None || TrainingController.FireButtonEnabled;
			fireSprite.gameObject.SetActive((Defs.isJumpAndShootButtonOn || !Defs.isUse3DTouch) && !Defs.isTurretWeapon && flag && (!isHunger || hungerGameController.isGo) && !WeaponManager.sharedManager.currentWeaponSounds.isGrenadeWeapon);
			reloadSpirte.gameObject.SetActive(((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive) || InGameGUI.sharedInGameGUI.playerMoveC == null) && !Defs.isTurretWeapon && flag && WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null && !WeaponManager.sharedManager.currentWeaponSounds.isMelee && !WeaponManager.sharedManager.currentWeaponSounds.isShotMelee);
			zoomSprite.gameObject.SetActive(((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive) || InGameGUI.sharedInGameGUI.playerMoveC == null) && !Defs.isTurretWeapon && flag && WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null && WeaponManager.sharedManager.currentWeaponSounds.isZooming);
			if (jumpIcon.activeSelf == Defs.isJetpackEnabled)
			{
				jumpIcon.SetActive(!Defs.isJetpackEnabled);
			}
			if (jetPackIcon.activeSelf != Defs.isJetpackEnabled)
			{
				jetPackIcon.SetActive(Defs.isJetpackEnabled);
			}
		}
	}

	private void SetGrenadeUISpriteState()
	{
		bool flag = IsBuyGrenadeActive();
		bool flag2 = IsUseGrenadeActive();
		SetActiveChecked(grenadeButton.gameObject, flag || flag2);
		if (!grenadeButton.gameObject.activeSelf)
		{
			return;
		}
		grenadeButton.grenadeSprite.spriteName = (((grenadePressed || _isBuyGrenadePressed) && grenadeRect.Contains(UICamera.lastTouchPosition)) ? ((!Defs.isDaterRegim) ? "grenade_btn_n" : "grenade_like_btn_n") : ((!Defs.isDaterRegim) ? "grenade_btn" : "grenade_like_btn"));
		if (flag)
		{
			if (Defs.isDaterRegim)
			{
				SetActiveChecked(grenadeButton.priceLabel.gameObject, true);
				SetActiveChecked(grenadeButton.countLabel.gameObject, false);
				SetActiveChecked(grenadeButton.fullLabel.gameObject, false);
			}
			else
			{
				grenadeButton.gameObject.SetActive(false);
			}
		}
		else
		{
			grenadeButton.gameObject.SetActive(true);
			SetActiveChecked(grenadeButton.priceLabel.gameObject, false);
			int grenadeCount = GetGrenadeCount();
			SetActiveChecked(grenadeButton.countLabel.gameObject, true);
			grenadeButton.countLabel.text = grenadeCount.ToString();
			SetActiveChecked(grenadeButton.fullLabel.gameObject, false);
		}
	}

	private void SetActiveChecked(GameObject obj, bool active)
	{
		if (active && !obj.activeSelf)
		{
			obj.SetActive(true);
		}
		else if (!active && obj.activeSelf)
		{
			obj.SetActive(false);
		}
	}

	private void SetSide()
	{
		bool flag = (GetComponent<UIAnchor>().side == UIAnchor.Side.BottomRight && GlobalGameController.LeftHanded) || (GetComponent<UIAnchor>().side == UIAnchor.Side.BottomLeft && !GlobalGameController.LeftHanded);
		GetComponent<UIAnchor>().side = (GlobalGameController.LeftHanded ? UIAnchor.Side.BottomRight : UIAnchor.Side.BottomLeft);
		Vector3 center = GetComponent<BoxCollider>().center;
		center.x *= ((!flag) ? (-1f) : 1f);
		GetComponent<BoxCollider>().center = center;
	}

	private void SetSideAndCalcRects()
	{
		SetSide();
		SetShouldRecalcRects();
	}

	private void SetShouldRecalcRects()
	{
		_shouldRecalcRects = true;
	}

	[Obfuscation(Exclude = true)]
	private void ReCalcRects()
	{
		CalcRects();
	}

	private IEnumerator Start()
	{
		SetSide();
		PauseNGUIController.PlayerHandUpdated += SetSideAndCalcRects;
		ControlsSettingsBase.ControlsChanged += SetShouldRecalcRects;
		if (isHunger)
		{
			hungerGameController = GameObject.FindGameObjectWithTag("HungerGameController").GetComponent<HungerGameController>();
		}
		SetSpritesState();
		yield return null;
		CalcRects();
		Reset();
	}

	public void Reset()
	{
		_touchControlScheme.Reset();
	}

	private void CalcRects()
	{
		Transform transform = NGUITools.GetRoot(base.gameObject).transform;
		Camera component = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
		Transform relativeTo = component.transform;
		float num = 768f;
		float num2 = num * ((float)Screen.width / (float)Screen.height);
		List<object> list = Json.Deserialize(PlayerPrefs.GetString("Controls.Size", "[]")) as List<object>;
		if (list == null)
		{
			list = new List<object>();
			Debug.LogWarning(list.GetType().FullName);
		}
		int[] array = list.Select(Convert.ToInt32).ToArray();
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, fireSprite, true);
		float num3 = 62f;
		if (array.Length > 3)
		{
			num3 = (float)array[3] * 0.5f;
		}
		bounds.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		fireRect = new Rect((bounds.center.x - num3) * Defs.Coef, (bounds.center.y - num3) * Defs.Coef, 2f * num3 * Defs.Coef, 2f * num3 * Defs.Coef);
		Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, jumpSprite, true);
		bounds2.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		float num4 = 62f;
		if (array.Length > 2)
		{
			num4 = (float)array[2] * 0.5f;
		}
		jumpRect = new Rect((bounds2.center.x - num4 * 0.7f) * Defs.Coef, (bounds2.center.y - num4) * Defs.Coef, 2f * num4 * Defs.Coef, 2f * num4 * Defs.Coef);
		Bounds bounds3 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, reloadSpirte, true);
		float num5 = 55f;
		if (array.Length > 1)
		{
			num5 = (float)array[1] * 0.5f;
		}
		bounds3.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		reloadRect = new Rect((bounds3.center.x - num5) * Defs.Coef, (bounds3.center.y - num5) * Defs.Coef, 2f * num5 * Defs.Coef, 2f * num5 * Defs.Coef);
		float num6 = 55f;
		if (array.Length > 0)
		{
			num6 = (float)array[0] * 0.5f;
		}
		Bounds bounds4 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, zoomSprite, true);
		bounds4.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		zoomRect = new Rect((bounds4.center.x - num6) * Defs.Coef, (bounds4.center.y - num6) * Defs.Coef, 2f * num6 * Defs.Coef, 2f * num6 * Defs.Coef);
		float num7 = 55f;
		if (array.Length > 5)
		{
			num7 = (float)array[5] * 0.5f;
		}
		Bounds bounds5 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, grenadeButton.grenadeSprite.transform, true);
		bounds5.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		grenadeRect = new Rect((bounds5.center.x - num7) * Defs.Coef, (bounds5.center.y - num7) * Defs.Coef, 2f * num7 * Defs.Coef, 2f * num7 * Defs.Coef);
		float num8 = (float)Screen.height * 0.81f;
		if (!GlobalGameController.LeftHanded)
		{
			moveRect = new Rect(0f, 0f, num8, (float)Screen.height * 0.65f);
		}
		else
		{
			moveRect = new Rect((float)Screen.width - num8, 0f, num8, (float)Screen.height * 0.65f);
		}
	}

	private IEnumerator _SetIsFirstFrame()
	{
		float tm = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - tm < 0.1f);
		_isFirstFrame = false;
	}

	private void Update()
	{
		framesCount++;
		if (fireRect.width.Equals(0f))
		{
			CalcRects();
		}
		SetSpritesState();
		_isFirstFrame = false;
		if (!_joyActive)
		{
			jumpPressed = false;
			isShooting = false;
			isShootingPressure = false;
			_touchControlScheme.Reset();
			return;
		}
		if (isInvokeGrenadePress && !grenadeRect.Contains(UICamera.lastTouchPosition))
		{
			isInvokeGrenadePress = false;
			CancelInvoke("GrenadePressInvoke");
		}
		_touchControlScheme.OnUpdate();
		if (WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			if (isShooting || isShootingPressure)
			{
				if ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None || TrainingController.FireButtonEnabled) && (!isHunger || hungerGameController.isGo) && hasAmmo && !WeaponManager.sharedManager.currentWeaponSounds.isGrenadeWeapon)
				{
					WeaponManager.sharedManager.myPlayerMoveC.ShotPressed();
				}
			}
			else
			{
				if (WeaponManager.sharedManager.myPlayerMoveC.isShootingLoop && !JoystickController.leftTouchPad.isActiveFireButton)
				{
					WeaponManager.sharedManager.myPlayerMoveC.ShotUnPressed();
				}
				if (!JoystickController.leftTouchPad.isActiveFireButton)
				{
					WeaponManager.sharedManager.myPlayerMoveC.ResetShootingBurst();
				}
			}
		}
		if (Input.touchCount > 0)
		{
			if (MoveTouchID == -1)
			{
				for (int i = 0; i < Input.touches.Length; i++)
				{
					if (Input.touches[i].phase == TouchPhase.Began && moveRect.Contains(Input.touches[i].position))
					{
						MoveTouchID = Input.touches[i].fingerId;
					}
				}
			}
			UpdateMoveTouch();
		}
		else
		{
			MoveTouchID = -1;
			pastPos = Vector2.zero;
			pastDelta = Vector2.zero;
		}
	}

	private void UpdateMoveTouch()
	{
		for (int i = 0; i < Input.touches.Length; i++)
		{
			if (Input.touches[i].fingerId == MoveTouchID)
			{
				Touch touch = Input.touches[i];
				if (pastPos == Vector2.zero)
				{
					pastPos = touch.position;
				}
				Vector2 vector = touch.position - pastPos;
				if (vector == Vector2.zero && pastDelta != Vector2.zero && IsDifferendDirections(pastDelta, JoystickController.leftJoystick.value) && pastDelta.sqrMagnitude > 15f)
				{
					vector = pastDelta / 2f;
					compDeltas += vector;
				}
				else
				{
					vector -= compDeltas;
					compDeltas = Vector2.zero;
				}
				OnDragTouch(vector);
				pastPos = touch.position;
				pastDelta = vector;
				if (touch.phase == TouchPhase.Ended)
				{
					MoveTouchID = -1;
					pastPos = Vector2.zero;
					pastDelta = Vector2.zero;
				}
			}
		}
	}

	private bool IsDifferendDirections(Vector2 delta1, Vector2 delta2)
	{
		return JoystickController.leftJoystick.value != Vector2.zero && Mathf.Sign(delta1.x) != Mathf.Sign(delta2.x);
	}

	public void HasAmmo()
	{
		BlinkReloadButton.isBlink = false;
	}

	public void NoAmmo()
	{
		BlinkReloadButton.isBlink = true;
	}

	[Obfuscation(Exclude = true)]
	private IEnumerator BlinkReload()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			reloadUISprite.spriteName = "Reload_0";
			yield return new WaitForSeconds(0.5f);
			reloadUISprite.spriteName = "Reload_1";
		}
	}

	private void OnPress(bool isDown)
	{
		_touchControlScheme.OnPress(isDown);
		if (!move)
		{
			if (!Defs.isMulti)
			{
				move = GameObject.FindGameObjectWithTag("Player").GetComponent<SkinName>().playerMoveC;
			}
			else
			{
				move = WeaponManager.sharedManager.myPlayerMoveC;
			}
		}
		if (fireRect.width.Equals(0f))
		{
			CalcRects();
		}
		if (!_joyActive || _isFirstFrame)
		{
			return;
		}
		if (isDown && fireRect.Contains(UICamera.lastTouchPosition) && (Defs.isJumpAndShootButtonOn || !Defs.isUse3DTouch))
		{
			isShooting = true;
		}
		if (isDown && grenadeRect.Contains(UICamera.lastTouchPosition))
		{
			if (IsBuyGrenadeActive())
			{
				BuyGrenadePressInvoke();
			}
			else if (IsUseGrenadeActive() && (!(InGameGUI.sharedInGameGUI != null) || !(InGameGUI.sharedInGameGUI.changeWeaponScroll != null) || !InGameGUI.sharedInGameGUI.changeWeaponScroll.isDragging))
			{
				isInvokeGrenadePress = true;
				GrenadePressInvoke();
			}
		}
		if (isDown && jumpRect.Contains(UICamera.lastTouchPosition) && (Defs.isJumpAndShootButtonOn || !Defs.isUse3DTouch) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0 || TrainingController.stepTraining >= TrainingState.GetTheGun))
		{
			jumpPressed = true;
		}
		if (isDown && ((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive) || InGameGUI.sharedInGameGUI.playerMoveC == null) && reloadRect.Contains(UICamera.lastTouchPosition) && (bool)move && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None || TrainingController.FireButtonEnabled))
		{
			move.ReloadPressed();
		}
		bool flag = zoomSprite != null && zoomSprite.gameObject.activeInHierarchy;
		if (isDown && ((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive) || InGameGUI.sharedInGameGUI.playerMoveC == null) && flag && zoomRect.Contains(UICamera.lastTouchPosition) && (bool)move && WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null && WeaponManager.sharedManager.currentWeaponSounds.isZooming)
		{
			move.ZoomPress();
		}
		if (isDown)
		{
			return;
		}
		if (isInvokeGrenadePress)
		{
			isInvokeGrenadePress = false;
			CancelInvoke("GrenadePressInvoke");
		}
		if (_isBuyGrenadePressed)
		{
			_isBuyGrenadePressed = false;
			grenadeButton.grenadeSprite.spriteName = ((!Defs.isDaterRegim) ? "grenade_btn" : "grenade_like_btn");
			if (grenadeRect.Contains(UICamera.lastTouchPosition))
			{
				InGameGUI.sharedInGameGUI.HandleBuyGrenadeClicked(null, EventArgs.Empty);
			}
		}
		isShooting = false;
		jumpPressed = false;
		if (grenadePressed)
		{
			grenadePressed = false;
			grenadeButton.grenadeSprite.spriteName = ((!Defs.isDaterRegim) ? "grenade_btn" : "grenade_like_btn");
			move.GrenadeFire();
		}
	}

	[Obfuscation(Exclude = true)]
	private void GrenadePressInvoke()
	{
		grenadePressed = true;
		grenadeButton.grenadeSprite.spriteName = "grenade_btn_n";
		move.GrenadePress();
	}

	[Obfuscation(Exclude = true)]
	private void BuyGrenadePressInvoke()
	{
		_isBuyGrenadePressed = true;
		grenadeButton.grenadeSprite.spriteName = "grenade_btn_n";
	}

	private void OnPressure(float pressure)
	{
		if (Input.touchPressureSupported && Defs.isUse3DTouch && WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			if ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None || TrainingController.FireButtonEnabled) && (!isHunger || hungerGameController.isGo) && hasAmmo && !WeaponManager.sharedManager.currentWeaponSounds.isGrenadeWeapon && pressure > 0.8f)
			{
				isShootingPressure = true;
			}
			else
			{
				isShootingPressure = false;
			}
		}
	}

	private void OnDragTouch(Vector2 delta)
	{
		if (!_joyActive)
		{
			jumpPressed = false;
			isShooting = false;
			_touchControlScheme.ResetDelta();
		}
		else
		{
			framesCount = 0;
			_touchControlScheme.OnDrag(delta);
		}
	}

	private void OnDestroy()
	{
		PauseNGUIController.PlayerHandUpdated -= SetSideAndCalcRects;
		ControlsSettingsBase.ControlsChanged -= SetShouldRecalcRects;
	}

	public Vector2 GrabDeltaPosition()
	{
		Vector2 result = Vector2.zero;
		if (_touchControlScheme != null)
		{
			result = _touchControlScheme.DeltaPosition;
			_touchControlScheme.ResetDelta();
		}
		return result;
	}

	public void ApplyDeltaTo(Vector2 deltaPosition, Transform yawTransform, Transform pitchTransform, float sensitivity, bool invert)
	{
		if (_touchControlScheme != null)
		{
			_touchControlScheme.ApplyDeltaTo(deltaPosition, yawTransform, pitchTransform, sensitivity, invert);
		}
	}
}
