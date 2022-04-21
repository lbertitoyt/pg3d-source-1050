using System.Runtime.InteropServices;
using UnityEngine;

public class EveryplayHudCamera : MonoBehaviour
{
	protected const int EPSR = 1162892114;

	private void Awake()
	{
		EveryplayUnityPluginInterfaceInitialize();
	}

	private void OnPreRender()
	{
		GL.IssuePluginEvent(1162892114);
	}

	[DllImport("everyplay")]
	private static extern void EveryplayUnityPluginInterfaceInitialize();
}
