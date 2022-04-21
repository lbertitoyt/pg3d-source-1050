using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public static class RiliExtensions
	{
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static T? ToEnum<T>(this string str, T? defaultVal = null) where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			if (str.IsNullOrEmpty())
			{
				Debug.LogError("String is null or empty");
				return defaultVal;
			}
			str = str.ToLower();
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				if (value.ToString().ToLower() == str)
				{
					return value;
				}
			}
			Debug.LogErrorFormat("'{0}' does not contain '{1}'", typeof(T).Name, str);
			return defaultVal;
		}

		public static string[] EnumValues<T>() where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			return Enum.GetValues(typeof(T)).Cast<string>().ToArray();
		}

		public static int[] EnumNumbers<T>() where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			return Enum.GetValues(typeof(T)).Cast<int>().ToArray();
		}

		public static void ForEachEnum<T>(Action<T> action)
		{
			if (action == null)
			{
				return;
			}
			Array values = Enum.GetValues(typeof(T));
			foreach (object item in values)
			{
				action((T)item);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			foreach (T item in enumeration)
			{
				action(item);
			}
		}

		public static GameObject GetChildGameObject(this GameObject go, string name, bool includeInactive = false)
		{
			Transform transform = go.transform.GetComponentsInChildren<Transform>(includeInactive).FirstOrDefault((Transform t) => t.gameObject.name == name);
			return (!(transform != null)) ? null : transform.gameObject;
		}

		public static T GetComponentInChildren<T>(this GameObject go, string name, bool includeInactive = false)
		{
			Transform[] componentsInChildren = go.transform.GetComponentsInChildren<Transform>(includeInactive);
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				if (transform.gameObject.name == name)
				{
					return transform.gameObject.GetComponent<T>();
				}
			}
			return default(T);
		}

		public static GameObject GetGameObjectInParent(this GameObject go, string name, bool includeInactive = false)
		{
			Transform[] componentsInParent = go.transform.GetComponentsInParent<Transform>(includeInactive);
			Transform[] array = componentsInParent;
			foreach (Transform transform in array)
			{
				if (transform.gameObject.name == name)
				{
					return transform.gameObject;
				}
			}
			return null;
		}

		public static T GetComponentInParents<T>(this GameObject go)
		{
			T component = go.GetComponent<T>();
			if (component != null && !component.Equals(default(T)))
			{
				return component;
			}
			Transform parent = go.transform.parent;
			return parent.gameObject.GetComponentInParents<T>();
		}

		public static T GetOrAddComponent<T>(this Component child) where T : Component
		{
			T val = child.GetComponent<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = child.gameObject.AddComponent<T>();
			}
			return val;
		}

		public static T GetOrAddComponent<T>(this GameObject child) where T : Component
		{
			T val = child.GetComponent<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = child.gameObject.AddComponent<T>();
			}
			return val;
		}
	}
}
