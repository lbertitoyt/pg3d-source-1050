using System.Collections;
using System.Threading;
using UnityEngine;

namespace Rilisoft
{
	public class AreaFog : AreaBase
	{
		[SerializeField]
		private float animationTime = 1f;

		[SerializeField]
		private FogSettings _settings;

		[ReadOnly]
		[SerializeField]
		private FogSettings _prevSettings;

		private CancellationTokenSource _tokenSource = new CancellationTokenSource();

		private new void Awake()
		{
			_prevSettings = new FogSettings().FromCurrent();
		}

		public override void CheckIn(GameObject to)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			base.CheckIn(to);
			_tokenSource.Cancel();
			_tokenSource = new CancellationTokenSource();
			StartCoroutine(Change(_settings, animationTime, _tokenSource.Token));
		}

		public override void CheckOut(GameObject from)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			base.CheckOut(from);
			_tokenSource.Cancel();
			_tokenSource = new CancellationTokenSource();
			StartCoroutine(Change(_prevSettings, animationTime, _tokenSource.Token));
		}

		private IEnumerator Change(FogSettings to, float time, CancellationToken token)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			RenderSettings.fog = to.Active;
			if (RenderSettings.fog)
			{
				FogSettings fr = new FogSettings().FromCurrent();
				RenderSettings.fogMode = to.Mode;
				float elapsed = 0f;
				while (elapsed < time && !((CancellationToken)(token)).IsCancellationRequested)
				{
					elapsed += Time.deltaTime;
					float rate = elapsed / time;
					RenderSettings.fogStartDistance = Mathf.Lerp(fr.Start, to.Start, rate);
					RenderSettings.fogEndDistance = Mathf.Lerp(fr.End, to.End, rate);
					RenderSettings.fogColor = Color.Lerp(fr.Color, to.Color, rate);
					yield return null;
				}
			}
		}
	}
}
