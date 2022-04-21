using System;
using UnityEngine;

namespace Rilisoft
{
	public struct SaltedInt : IEquatable<SaltedInt>
	{
		private readonly int _salt;

		private int _saltedValue;

		public int Value
		{
			get
			{
				return _salt ^ _saltedValue;
			}
			set
			{
				_saltedValue = _salt ^ value;
			}
		}

		public SaltedInt(int salt, int value)
		{
			_salt = salt;
			_saltedValue = salt ^ value;
		}

		public SaltedInt(int salt)
			: this(salt, 0)
		{
		}

		public bool Equals(SaltedInt other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SaltedInt))
			{
				return false;
			}
			SaltedInt other = (SaltedInt)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static implicit operator SaltedInt(int i)
		{
			return new SaltedInt(UnityEngine.Random.Range(int.MinValue, int.MaxValue), i);
		}
	}
}
