using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class DestroyRubble : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class Start15 : GenericGenerator<WaitForSeconds>
	{
		internal DestroyRubble self_18;

		public Start15(DestroyRubble self_)
		{
			self_18 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return (IEnumerator<WaitForSeconds>)(object)self_18;
		}
	}

	public float maxTime;

	public ParticleEmitter[] particleEmitters;

	public float time;

	public DestroyRubble()
	{
		maxTime = 3f;
	}

	public IEnumerator Start()
	{
		return new Start15(this).GetEnumerator();
	}

	public void Main()
	{
	}
}
