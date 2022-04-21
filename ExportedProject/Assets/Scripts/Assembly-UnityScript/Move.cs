using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class Move : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class Start22 : GenericGenerator<WaitForSeconds>
	{
		internal Move self_24;

		public Start22(Move self_)
		{
			self_24 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return (IEnumerator<WaitForSeconds>)(object)self_24;
		}
	}

	public Transform target;

	public float speed;

	public float smokeDestroyTime;

	public ParticleRenderer smokeStem;

	public float destroySpeed;

	public float destroySpeedStem;

	private bool destroyEnabled;

	public IEnumerator Start()
	{
		return new Start22(this).GetEnumerator();
	}

	public void Update()
	{
		transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
		Color color = default(Color);
		if (destroyEnabled)
		{
			ParticleRenderer particleRenderer = (ParticleRenderer)GetComponent(typeof(ParticleRenderer));
			color = particleRenderer.material.GetColor("_TintColor");
			Color color2 = smokeStem.material.GetColor("_TintColor");
			if (!(color.a <= 0f))
			{
				color.a -= destroySpeed * Time.deltaTime;
			}
			if (!(color2.a <= 0f))
			{
				color2.a -= destroySpeedStem * Time.deltaTime;
			}
			smokeStem.material.SetColor("_TintColor", color2);
			particleRenderer.material.SetColor("_TintColor", color);
		}
		if (!(color.a >= 0f))
		{
			UnityEngine.Object.Destroy(transform.root.gameObject);
		}
	}

	public void Main()
	{
	}
}
