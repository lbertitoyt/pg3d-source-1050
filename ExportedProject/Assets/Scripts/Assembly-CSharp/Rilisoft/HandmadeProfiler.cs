using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rilisoft
{
	public class HandmadeProfiler : MonoBehaviour
	{
		private struct SampleMemento : IEquatable<SampleMemento>
		{
			[SerializeField]
			private int frame;

			[SerializeField]
			private double dt;

			[SerializeField]
			private string scene;

			[SerializeField]
			private List<string> components;

			public int Frame
			{
				get
				{
					return frame;
				}
				set
				{
					frame = value;
				}
			}

			public double Dt
			{
				get
				{
					return dt;
				}
				set
				{
					dt = value;
				}
			}

			public string Scene
			{
				get
				{
					return scene ?? string.Empty;
				}
				set
				{
					scene = value ?? string.Empty;
				}
			}

			public List<string> Components
			{
				get
				{
					return components ?? new List<string>();
				}
				set
				{
					components = value ?? new List<string>();
				}
			}

			public bool Equals(SampleMemento other)
			{
				if (Frame != other.Frame)
				{
					return false;
				}
				if (Dt != other.Dt)
				{
					return false;
				}
				if (Scene != other.Scene)
				{
					return false;
				}
				if (!Components.SequenceEqual(Components))
				{
					return false;
				}
				return true;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is SampleMemento))
				{
					return false;
				}
				SampleMemento other = (SampleMemento)obj;
				return Equals(other);
			}

			public override int GetHashCode()
			{
				return Frame.GetHashCode() ^ Dt.GetHashCode() ^ Scene.GetHashCode() ^ Components.GetHashCode();
			}

			public override string ToString()
			{
				return JsonUtility.ToJson(this);
			}
		}

		private float _dtThreshold = 1f;

		private void Awake()
		{
			_dtThreshold = ((!Application.isEditor) ? 2f : 1f);
		}

		private void LateUpdate()
		{
			if (!Defs.IsDeveloperBuild)
			{
				return;
			}
			float num = Time.realtimeSinceStartup - Time.unscaledTime;
			if (!(num > _dtThreshold))
			{
				return;
			}
			List<string> list = (from s in (from c in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>()
					where c.gameObject.activeInHierarchy
					select c.GetType().Name).Distinct()
				orderby s
				select s).ToList();
			SampleMemento sampleMemento = default(SampleMemento);
			sampleMemento.Dt = Math.Round(num, 3);
			sampleMemento.Frame = Time.frameCount;
			sampleMemento.Scene = SceneManager.GetActiveScene().name;
			SampleMemento sample = sampleMemento;
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && list.Count > 32)
			{
				int num2 = 0;
				while (list.Count > 0)
				{
					List<string> list2 = new List<string>(34);
					if (num2 > 0)
					{
						list2.Add("...");
					}
					list2.AddRange(list.Take(32));
					list.RemoveRange(0, Math.Min(32, list.Count));
					if (list.Count > 0)
					{
						list2.Add("...");
					}
					sample.Components = list2;
					LogSample(sample);
					num2++;
				}
			}
			else
			{
				sample.Components = list;
				LogSample(sample);
			}
		}

		private void LogSample(SampleMemento sample)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Frame rate drop: {0}", sample);
			string message = ((!Application.isEditor) ? text : ("<color=orange><b>" + text + "</b></color>"));
			Debug.LogWarning(message);
		}
	}
}
