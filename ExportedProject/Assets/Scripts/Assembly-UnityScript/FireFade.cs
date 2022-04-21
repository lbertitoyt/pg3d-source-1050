using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class FireFade : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class Start19 : GenericGenerator<WaitForSeconds>
	{
		internal FireFade self_21;

		public Start19(FireFade self_)
		{
			self_21 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return (IEnumerator<WaitForSeconds>)(object)self_21;
		}
	}

	public float smokeDestroyTime;

	public float destroySpeed;

	private bool destroyEnabled;

	public FireFade()
	{
		smokeDestroyTime = 6f;
		destroySpeed = 0.05f;
	}

	public IEnumerator Start()
	{
		return new Start19(this).GetEnumerator();
	}

	public void Update()
	{
		if (destroyEnabled)
		{
			ParticleRenderer particleRenderer = (ParticleRenderer)GetComponent(typeof(ParticleRenderer));
			Color color = particleRenderer.materials[1].GetColor("_TintColor");
			color.a -= destroySpeed * Time.deltaTime;
			particleRenderer.materials[1].SetColor("_TintColor", color);
		}
	}

	public void Main()
	{
	}
}
