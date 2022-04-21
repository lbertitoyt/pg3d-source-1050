using System;
using System.Collections;
using UnityEngine;

public class DeviceOrientationMonitor : MonoBehaviour
{
	public static float CheckDelay = 0.5f;

	public static DeviceOrientation CurrentOrientation { get; private set; }

	public static event Action<DeviceOrientation> OnOrientationChange;

	static DeviceOrientationMonitor()
	{
		DeviceOrientationMonitor.OnOrientationChange = delegate
		{
		};
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		StartCoroutine(CheckForChange());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator CheckForChange()
	{
		CurrentOrientation = Input.deviceOrientation;
		while (true)
		{
			DeviceOrientation deviceOrientation = Input.deviceOrientation;
			if ((deviceOrientation == DeviceOrientation.LandscapeLeft || deviceOrientation == DeviceOrientation.LandscapeRight) && CurrentOrientation != Input.deviceOrientation)
			{
				CurrentOrientation = Input.deviceOrientation;
				DeviceOrientationMonitor.OnOrientationChange(CurrentOrientation);
			}
			yield return new WaitForSeconds(CheckDelay);
		}
	}
}
