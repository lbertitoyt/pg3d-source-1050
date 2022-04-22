using System;
using UnityEngine;

[Serializable]
public class Scale : MonoBehaviour
{
	public ParticleEmitter[] particleEmitters;

	public float scale;

	[SerializeField]
	[HideInInspector]
	private float[] minsize;

	[HideInInspector]
	[SerializeField]
	private float[] maxsize;

	[HideInInspector]
	[SerializeField]
	private Vector3[] worldvelocity;

	[SerializeField]
	[HideInInspector]
	private Vector3[] localvelocity;

	[SerializeField]
	[HideInInspector]
	private Vector3[] rndvelocity;

	[HideInInspector]
	[SerializeField]
	private Vector3[] scaleBackUp;

	[HideInInspector]
	[SerializeField]
	private bool firstUpdate;

	public Scale()
	{
		scale = 1f;
		firstUpdate = true;
	}

	public void UpdateScale() // should be override
	{
		int num = Extensions.Get_Length((Array)particleEmitters);
		if (firstUpdate)
		{
			minsize = new float[num];
			maxsize = new float[num];
			worldvelocity = new Vector3[num];
			localvelocity = new Vector3[num];
			rndvelocity = new Vector3[num];
			scaleBackUp = new Vector3[num];
		}
		for (int i = 0; i < Extensions.Get_Length((Array)particleEmitters); i++)
		{
			if (firstUpdate)
			{
				minsize[i] = particleEmitters[i].minSize;
				maxsize[i] = particleEmitters[i].maxSize;
				worldvelocity[i] = particleEmitters[i].worldVelocity;
				localvelocity[i] = particleEmitters[i].localVelocity;
				rndvelocity[i] = particleEmitters[i].rndVelocity;
				scaleBackUp[i] = particleEmitters[i].transform.localScale;
			}
			particleEmitters[i].minSize = minsize[i] * scale;
			particleEmitters[i].maxSize = maxsize[i] * scale;
			particleEmitters[i].worldVelocity = worldvelocity[i] * scale;
			particleEmitters[i].localVelocity = localvelocity[i] * scale;
			particleEmitters[i].rndVelocity = rndvelocity[i] * scale;
			particleEmitters[i].transform.localScale = scaleBackUp[i] * scale;
		}
		firstUpdate = false;
	}

	public void Main() // should be override
	{
	}
}
