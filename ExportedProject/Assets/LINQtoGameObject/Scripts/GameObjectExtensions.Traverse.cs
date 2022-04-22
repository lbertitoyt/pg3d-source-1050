using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
	// API Frontend

	public static partial class GameObjectExtensions
	{
		// Traverse Game Objects, based on Axis(Parent, Child, Children, Ancestors/Descendants, BeforeSelf/BeforeAfter)

		/// <summary>Gets the parent GameObject of this GameObject. If this GameObject has no parent, returns null.</summary>
		public static GameObject Parent(this GameObject origin)
		{
			if (origin == null)
			{
				return null;
			}

			Transform parentTransform = origin.transform.parent;
			return parentTransform?.gameObject;
		}

		/// <summary>Gets the first child GameObject with the specified name. If there is no GameObject with the speficided name, returns null.</summary>
		public static GameObject Child(this GameObject origin, string name)
		{
			if (origin == null)
			{
				return null;
			}

			Transform child = origin.transform.Find(name); // transform.find can get inactive object
			return child?.gameObject;
		}

		/// <summary>Returns a collection of the child GameObjects.</summary>
		public static ChildrenEnumerable Children(this GameObject origin) => new ChildrenEnumerable(origin, false);

		/// <summary>Returns a collection of GameObjects that contain this GameObject, and the child GameObjects.</summary>
		public static ChildrenEnumerable ChildrenAndSelf(this GameObject origin) => new ChildrenEnumerable(origin, true);

		/// <summary>Returns a collection of the ancestor GameObjects of this GameObject.</summary>
		public static AncestorsEnumerable Ancestors(this GameObject origin) => new AncestorsEnumerable(origin, false);

		/// <summary>Returns a collection of GameObjects that contain this element, and the ancestors of this GameObject.</summary>
		public static AncestorsEnumerable AncestorsAndSelf(this GameObject origin) => new AncestorsEnumerable(origin, true);

		/// <summary>Returns a collection of the descendant GameObjects.</summary>
		public static DescendantsEnumerable Descendants(this GameObject origin, Func<Transform, bool> descendIntoChildren = null) => new DescendantsEnumerable(origin, false, descendIntoChildren);

		/// <summary>Returns a collection of GameObjects that contain this GameObject, and all descendant GameObjects of this GameObject.</summary>
		public static DescendantsEnumerable DescendantsAndSelf(this GameObject origin, Func<Transform, bool> descendIntoChildren = null) => new DescendantsEnumerable(origin, true, descendIntoChildren);

		/// <summary>Returns a collection of the sibling GameObjects before this GameObject.</summary>
		public static BeforeSelfEnumerable BeforeSelf(this GameObject origin) => new BeforeSelfEnumerable(origin, false);

		/// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject.</summary>
		public static BeforeSelfEnumerable BeforeSelfAndSelf(this GameObject origin) => new BeforeSelfEnumerable(origin, true);

		/// <summary>Returns a collection of the sibling GameObjects after this GameObject.</summary>
		public static AfterSelfEnumerable AfterSelf(this GameObject origin) => new AfterSelfEnumerable(origin, false);

		/// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject.</summary>
		public static AfterSelfEnumerable AfterSelfAndSelf(this GameObject origin) => new AfterSelfEnumerable(origin, true);

		// Implements hand struct enumerator.

		public struct ChildrenEnumerable : IEnumerable<GameObject>
		{
			private readonly GameObject origin;
			private readonly bool withSelf;

			public ChildrenEnumerable(GameObject origin, bool withSelf)
			{
				this.origin = origin;
				this.withSelf = withSelf;
			}

			/// <summary>Returns a collection of specified component in the source collection.</summary>
			public OfComponentEnumerable<T> OfComponent<T>()
				where T : Component => new OfComponentEnumerable<T>(ref this);

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			/// <param name="detachParent">set to parent = null.</param>
			public void Destroy(bool useDestroyImmediate = false, bool detachParent = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					e.Current.Destroy(useDestroyImmediate, false);
				}
				if (detachParent)
				{
					origin.transform.DetachChildren();
					if (withSelf)
					{
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
						origin.transform.SetParent(null);
#else
                        origin.transform.parent = null;
#endif
					}
				}
			}

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (predicate(item))
					{
						item.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator() =>
				// check GameObject is destroyed only on GetEnumerator timing
				(origin == null)
					? new Enumerator(null, withSelf, false)
					: new Enumerator(origin.transform, withSelf, true);

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator() => GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			#region LINQ

			private int GetChildrenSize() => origin.transform.childCount + (withSelf ? 1 : 0);

			public void ForEach(Action<GameObject> action)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					action(e.Current);
				}
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int index = 0;

				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (array.Length == index)
					{
						int newSize = (index == 0) ? GetChildrenSize() : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = item;
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (!filter(item))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? GetChildrenSize() : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = item;
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (array.Length == index)
					{
						int newSize = (index == 0) ? GetChildrenSize() : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(item);
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (!filter(item))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? GetChildrenSize() : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(item);
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					TState state = let(item);

					if (!filter(state))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? GetChildrenSize() : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(state);
				}

				return index;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[GetChildrenSize()];
				int len = ToArrayNonAlloc(ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[GetChildrenSize()];
				int len = ToArrayNonAlloc(filter, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[GetChildrenSize()];
				int len = ToArrayNonAlloc<T>(selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[GetChildrenSize()];
				int len = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[GetChildrenSize()];
				int len = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator e = GetEnumerator();
				if (e.MoveNext())
				{
					return e.Current;
				}
				else
				{
					throw new InvalidOperationException("sequence is empty.");
				}
			}

			public GameObject FirstOrDefault()
			{
				Enumerator e = GetEnumerator();
				return (e.MoveNext())
					? e.Current
					: null;
			}

			#endregion

			public struct Enumerator : IEnumerator<GameObject>
			{
				private readonly int childCount; // childCount is fixed when GetEnumerator is called.

				private readonly Transform originTransform;
				private readonly bool canRun;
				private bool withSelf;
				private int currentIndex;
				private GameObject current;

				internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
				{
					this.originTransform = originTransform;
					this.withSelf = withSelf;
					childCount = canRun ? originTransform.childCount : 0;
					currentIndex = -1;
					this.canRun = canRun;
					current = null;
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}

					if (withSelf)
					{
						current = originTransform.gameObject;
						withSelf = false;
						return true;
					}

					currentIndex++;
					if (currentIndex < childCount)
					{
						Transform child = originTransform.GetChild(currentIndex);
						current = child.gameObject;
						return true;
					}

					return false;
				}

				public GameObject Current => current;
				object IEnumerator.Current => current;
				public void Dispose() { }
				public void Reset() { throw new NotSupportedException(); }
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>
				where T : Component
			{
				private ChildrenEnumerable parent;

				public OfComponentEnumerable(ref ChildrenEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator() => new OfComponentEnumerator<T>(ref parent);

				IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

				#region LINQ

				public void ForEach(Action<T> action)
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					while (e.MoveNext())
					{
						action(e.Current);
					}
				}

				public T First()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					if (e.MoveNext())
					{
						return e.Current;
					}
					else
					{
						throw new InvalidOperationException("sequence is empty.");
					}
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					return (e.MoveNext())
						? e.Current
						: null;
				}

				public T[] ToArray()
				{
					T[] array = new T[parent.GetChildrenSize()];
					int len = ToArrayNonAlloc(ref array);
					if (array.Length != len)
					{
						Array.Resize(ref array, len);
					}
					return array;
				}

				/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
				public int ToArrayNonAlloc(ref T[] array)
				{
					int index = 0;
					OfComponentEnumerator<T> e = GetEnumerator();
					while (e.MoveNext())
					{
						if (array.Length == index)
						{
							int newSize = (index == 0) ? parent.GetChildrenSize() : index * 2;
							Array.Resize(ref array, newSize);
						}
						array[index++] = e.Current;
					}

					return index;
				}

				#endregion
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>
				where T : Component
			{
				private Enumerator enumerator; // enumerator is mutable
				private T current;

#if UNITY_EDITOR
				private static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

				public OfComponentEnumerator(ref ChildrenEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = default(T);
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
#if UNITY_EDITOR
						enumerator.Current.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							current = componentCache[0];
							componentCache.Clear();
							return true;
						}
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
					}

					return false;
				}

				public T Current => current;
				object IEnumerator.Current => current;
				public void Dispose() { }
				public void Reset() { throw new NotSupportedException(); }
			}
		}

		public struct AncestorsEnumerable : IEnumerable<GameObject>
		{
			private readonly GameObject origin;
			private readonly bool withSelf;

			public AncestorsEnumerable(GameObject origin, bool withSelf)
			{
				this.origin = origin;
				this.withSelf = withSelf;
			}

			/// <summary>Returns a collection of specified component in the source collection.</summary>
			public OfComponentEnumerable<T> OfComponent<T>()
				where T : Component => new OfComponentEnumerable<T>(ref this);

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					e.Current.Destroy(useDestroyImmediate, false);
				}
			}

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (predicate(item))
					{
						item.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator() =>
				// check GameObject is destroyed only on GetEnumerator timing
				(origin == null)
					? new Enumerator(null, null, withSelf, false)
					: new Enumerator(origin, origin.transform, withSelf, true);

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator() => GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			#region LINQ

			public void ForEach(Action<GameObject> action)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					action(e.Current);
				}
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int index = 0;

				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = item;
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (!filter(item))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = item;
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(item);
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (!filter(item))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(item);
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					TState state = let(item);

					if (!filter(state))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(state);
				}

				return index;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[4];
				int len = ToArrayNonAlloc(ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[4];
				int len = ToArrayNonAlloc(filter, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc<T>(selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator e = GetEnumerator();
				if (e.MoveNext())
				{
					return e.Current;
				}
				else
				{
					throw new InvalidOperationException("sequence is empty.");
				}
			}

			public GameObject FirstOrDefault()
			{
				Enumerator e = GetEnumerator();
				return (e.MoveNext())
					? e.Current
					: null;
			}

			#endregion

			public struct Enumerator : IEnumerator<GameObject>
			{
				private readonly bool canRun;
				private GameObject current;
				private Transform currentTransform;
				private bool withSelf;

				internal Enumerator(GameObject origin, Transform originTransform, bool withSelf, bool canRun)
				{
					current = origin;
					currentTransform = originTransform;
					this.withSelf = withSelf;
					this.canRun = canRun;
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}

					if (withSelf)
					{
						// withSelf, use origin and originTransform
						withSelf = false;
						return true;
					}

					Transform parentTransform = currentTransform.parent;
					if (parentTransform != null)
					{
						current = parentTransform.gameObject;
						currentTransform = parentTransform;
						return true;
					}

					return false;
				}

				public GameObject Current => current;
				object IEnumerator.Current => current;
				public void Dispose() { }
				public void Reset() { throw new NotSupportedException(); }
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>
				where T : Component
			{
				private AncestorsEnumerable parent;

				public OfComponentEnumerable(ref AncestorsEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator() => new OfComponentEnumerator<T>(ref parent);

				IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

				#region LINQ

				public void ForEach(Action<T> action)
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					while (e.MoveNext())
					{
						action(e.Current);
					}
				}

				public T First()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					if (e.MoveNext())
					{
						return e.Current;
					}
					else
					{
						throw new InvalidOperationException("sequence is empty.");
					}
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					return (e.MoveNext())
						? e.Current
						: null;
				}

				public T[] ToArray()
				{
					T[] array = new T[4];
					int len = ToArrayNonAlloc(ref array);
					if (array.Length != len)
					{
						Array.Resize(ref array, len);
					}
					return array;
				}

				/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
				public int ToArrayNonAlloc(ref T[] array)
				{
					int index = 0;
					OfComponentEnumerator<T> e = GetEnumerator();
					while (e.MoveNext())
					{
						if (array.Length == index)
						{
							int newSize = (index == 0) ? 4 : index * 2;
							Array.Resize(ref array, newSize);
						}
						array[index++] = e.Current;
					}

					return index;
				}

				#endregion
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>
				where T : Component
			{
				private Enumerator enumerator; // enumerator is mutable
				private T current;

#if UNITY_EDITOR
				private static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

				public OfComponentEnumerator(ref AncestorsEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = default(T);
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
#if UNITY_EDITOR
						enumerator.Current.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							current = componentCache[0];
							componentCache.Clear();
							return true;
						}
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
					}

					return false;
				}

				public T Current => current;
				object IEnumerator.Current => current;
				public void Dispose() { }
				public void Reset() { throw new NotSupportedException(); }
			}
		}

		public struct DescendantsEnumerable : IEnumerable<GameObject>
		{
			private static readonly Func<Transform, bool> alwaysTrue = _ => true;
			private readonly GameObject origin;
			private readonly bool withSelf;
			private readonly Func<Transform, bool> descendIntoChildren;

			public DescendantsEnumerable(GameObject origin, bool withSelf, Func<Transform, bool> descendIntoChildren)
			{
				this.origin = origin;
				this.withSelf = withSelf;
				this.descendIntoChildren = descendIntoChildren ?? alwaysTrue;
			}

			/// <summary>Returns a collection of specified component in the source collection.</summary>
			public OfComponentEnumerable<T> OfComponent<T>()
				where T : Component => new OfComponentEnumerable<T>(ref this);

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					e.Current.Destroy(useDestroyImmediate, false);
				}
			}

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (predicate(item))
					{
						item.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator()
			{
				// check GameObject is destroyed only on GetEnumerator timing
				if (origin == null)
				{
					return new Enumerator(null, withSelf, false, null, descendIntoChildren);
				}

				InternalUnsafeRefStack refStack;
				if (InternalUnsafeRefStack.RefStackPool.Count != 0)
				{
					refStack = InternalUnsafeRefStack.RefStackPool.Dequeue();
					refStack.Reset();
				}
				else
				{
					refStack = new InternalUnsafeRefStack(6);
				}

				return new Enumerator(origin.transform, withSelf, true, refStack, descendIntoChildren);
			}

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator() => GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			#region LINQ

			private void ResizeArray<T>(ref int index, ref T[] array)
			{
				if (array.Length == index)
				{
					int newSize = (index == 0) ? 4 : index * 2;
					Array.Resize(ref array, newSize);
				}
			}

			private void DescendantsCore(ref Transform transform, ref Action<GameObject> action)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}

				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					action(child.gameObject);
					DescendantsCore(ref child, ref action);
				}
			}

			private void DescendantsCore(ref Transform transform, ref int index, ref GameObject[] array)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}

				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					ResizeArray(ref index, ref array);
					array[index++] = child.gameObject;
					DescendantsCore(ref child, ref index, ref array);
				}
			}

			private void DescendantsCore(ref Func<GameObject, bool> filter, ref Transform transform, ref int index, ref GameObject[] array)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}

				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					GameObject childGameObject = child.gameObject;
					if (filter(childGameObject))
					{
						ResizeArray(ref index, ref array);
						array[index++] = childGameObject;
					}
					DescendantsCore(ref filter, ref child, ref index, ref array);
				}
			}

			private void DescendantsCore<T>(ref Func<GameObject, T> selector, ref Transform transform, ref int index, ref T[] array)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}

				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					ResizeArray(ref index, ref array);
					array[index++] = selector(child.gameObject);
					DescendantsCore(ref selector, ref child, ref index, ref array);
				}
			}

			private void DescendantsCore<T>(ref Func<GameObject, bool> filter, ref Func<GameObject, T> selector, ref Transform transform, ref int index, ref T[] array)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}

				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					GameObject childGameObject = child.gameObject;
					if (filter(childGameObject))
					{
						ResizeArray(ref index, ref array);
						array[index++] = selector(childGameObject);
					}
					DescendantsCore(ref filter, ref selector, ref child, ref index, ref array);
				}
			}

			private void DescendantsCore<TState, T>(ref Func<GameObject, TState> let, ref Func<TState, bool> filter, ref Func<TState, T> selector, ref Transform transform, ref int index, ref T[] array)
			{
				if (!descendIntoChildren(transform))
				{
					return;
				}

				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					TState state = let(child.gameObject);
					if (filter(state))
					{
						ResizeArray(ref index, ref array);
						array[index++] = selector(state);
					}
					DescendantsCore(ref let, ref filter, ref selector, ref child, ref index, ref array);
				}
			}

			/// <summary>Use internal iterator for performance optimization.</summary>
			/// <param name="action"></param>
			public void ForEach(Action<GameObject> action)
			{
				if (withSelf)
				{
					action(origin);
				}
				Transform originTransform = origin.transform;
				DescendantsCore(ref originTransform, ref action);
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int index = 0;
				if (withSelf)
				{
					ResizeArray(ref index, ref array);
					array[index++] = origin;
				}

				Transform originTransform = origin.transform;
				DescendantsCore(ref originTransform, ref index, ref array);

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int index = 0;
				if (withSelf && filter(origin))
				{
					ResizeArray(ref index, ref array);
					array[index++] = origin;
				}
				Transform originTransform = origin.transform;
				DescendantsCore(ref filter, ref originTransform, ref index, ref array);

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				if (withSelf)
				{
					ResizeArray(ref index, ref array);
					array[index++] = selector(origin);
				}
				Transform originTransform = origin.transform;
				DescendantsCore(ref selector, ref originTransform, ref index, ref array);

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				if (withSelf && filter(origin))
				{
					ResizeArray(ref index, ref array);
					array[index++] = selector(origin);
				}
				Transform originTransform = origin.transform;
				DescendantsCore(ref filter, ref selector, ref originTransform, ref index, ref array);

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int index = 0;
				if (withSelf)
				{
					TState state = let(origin);
					if (filter(state))
					{
						ResizeArray(ref index, ref array);
						array[index++] = selector(state);
					}
				}

				Transform originTransform = origin.transform;
				DescendantsCore(ref let, ref filter, ref selector, ref originTransform, ref index, ref array);

				return index;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[4];
				int len = ToArrayNonAlloc(ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[4];
				int len = ToArrayNonAlloc(filter, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc<T>(selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator e = GetEnumerator();
				try
				{
					if (e.MoveNext())
					{
						return e.Current;
					}
					else
					{
						throw new InvalidOperationException("sequence is empty.");
					}
				}
				finally
				{
					e.Dispose();
				}
			}

			public GameObject FirstOrDefault()
			{
				Enumerator e = GetEnumerator();
				try
				{
					return (e.MoveNext())
						? e.Current
						: null;
				}
				finally
				{
					e.Dispose();
				}
			}

			#endregion

			internal class InternalUnsafeRefStack
			{
				public static Queue<InternalUnsafeRefStack> RefStackPool = new Queue<InternalUnsafeRefStack>();

				public int size = 0;
				public Enumerator[] array; // Pop = this.array[--size];

				public InternalUnsafeRefStack(int initialStackDepth)
				{
					array = new GameObjectExtensions.DescendantsEnumerable.Enumerator[initialStackDepth];
				}

				public void Push(ref Enumerator e)
				{
					if (size == array.Length)
					{
						Array.Resize(ref array, array.Length * 2);
					}
					array[size++] = e;
				}

				public void Reset() => size = 0;
			}

			public struct Enumerator : IEnumerator<GameObject>
			{
				private readonly int childCount; // childCount is fixed when GetEnumerator is called.

				private readonly Transform originTransform;
				private bool canRun;
				private bool withSelf;
				private int currentIndex;
				private GameObject current;
				private InternalUnsafeRefStack sharedStack;
				private Func<Transform, bool> descendIntoChildren;

				internal Enumerator(Transform originTransform, bool withSelf, bool canRun, InternalUnsafeRefStack sharedStack, Func<Transform, bool> descendIntoChildren)
				{
					this.originTransform = originTransform;
					this.withSelf = withSelf;
					childCount = canRun ? originTransform.childCount : 0;
					currentIndex = -1;
					this.canRun = canRun;
					current = null;
					this.sharedStack = sharedStack;
					this.descendIntoChildren = descendIntoChildren;
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}

					while (sharedStack.size != 0)
					{
						if (sharedStack.array[sharedStack.size - 1].MoveNextCore(true, out current))
						{
							return true;
						}
					}

					if (!withSelf && !descendIntoChildren(originTransform))
					{
						// reuse
						canRun = false;
						InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
						return false;
					}

					if (MoveNextCore(false, out current))
					{
						return true;
					}
					else
					{
						// reuse
						canRun = false;
						InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
						return false;
					}
				}

				private bool MoveNextCore(bool peek, out GameObject current)
				{
					if (withSelf)
					{
						current = originTransform.gameObject;
						withSelf = false;
						return true;
					}

					++currentIndex;
					if (currentIndex < childCount)
					{
						Transform item = originTransform.GetChild(currentIndex);
						if (descendIntoChildren(item))
						{
							Enumerator childEnumerator = new Enumerator(item, true, true, sharedStack, descendIntoChildren);
							sharedStack.Push(ref childEnumerator);
							return sharedStack.array[sharedStack.size - 1].MoveNextCore(true, out current);
						}
						else
						{
							current = item.gameObject;
							return true;
						}
					}

					if (peek)
					{
						sharedStack.size--; // Pop
					}

					current = null;
					return false;
				}

				public GameObject Current => current;
				object IEnumerator.Current => current;

				public void Dispose()
				{
					if (canRun)
					{
						canRun = false;
						InternalUnsafeRefStack.RefStackPool.Enqueue(sharedStack);
					}
				}

				public void Reset() { throw new NotSupportedException(); }
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>
				where T : Component
			{
				private DescendantsEnumerable parent;

				public OfComponentEnumerable(ref DescendantsEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator() => new OfComponentEnumerator<T>(ref parent);

				IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

				#region LINQ

				public T First()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					try
					{
						if (e.MoveNext())
						{
							return e.Current;
						}
						else
						{
							throw new InvalidOperationException("sequence is empty.");
						}
					}
					finally
					{
						e.Dispose();
					}
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					try
					{
						return (e.MoveNext())
							? e.Current
							: null;
					}
					finally
					{
						e.Dispose();
					}
				}

				/// <summary>Use internal iterator for performance optimization.</summary>
				public void ForEach(Action<T> action)
				{
					if (parent.withSelf)
					{
						T component = default(T);
#if UNITY_EDITOR
						parent.origin.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							component = componentCache[0];
							componentCache.Clear();
						}
#else
                        component = parent.origin.GetComponent<T>();
#endif

						if (component != null)
						{
							action(component);
						}
					}

					Transform originTransform = parent.origin.transform;
					OfComponentDescendantsCore(ref originTransform, ref action);
				}


				public T[] ToArray()
				{
					T[] array = new T[4];
					int len = ToArrayNonAlloc(ref array);
					if (array.Length != len)
					{
						Array.Resize(ref array, len);
					}
					return array;
				}

#if UNITY_EDITOR
				private static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

				private void OfComponentDescendantsCore(ref Transform transform, ref Action<T> action)
				{
					if (!parent.descendIntoChildren(transform))
					{
						return;
					}

					int childCount = transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform child = transform.GetChild(i);

						T component = default(T);
#if UNITY_EDITOR
						child.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							component = componentCache[0];
							componentCache.Clear();
						}
#else
                        component = child.GetComponent<T>();
#endif

						if (component != null)
						{
							action(component);
						}
						OfComponentDescendantsCore(ref child, ref action);
					}
				}

				private void OfComponentDescendantsCore(ref Transform transform, ref int index, ref T[] array)
				{
					if (!parent.descendIntoChildren(transform))
					{
						return;
					}

					int childCount = transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform child = transform.GetChild(i);
						T component = default(T);
#if UNITY_EDITOR
						child.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							component = componentCache[0];
							componentCache.Clear();
						}
#else
                        component = child.GetComponent<T>();
#endif

						if (component != null)
						{
							if (array.Length == index)
							{
								int newSize = (index == 0) ? 4 : index * 2;
								Array.Resize(ref array, newSize);
							}

							array[index++] = component;
						}
						OfComponentDescendantsCore(ref child, ref index, ref array);
					}
				}

				/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
				public int ToArrayNonAlloc(ref T[] array)
				{
					int index = 0;
					if (parent.withSelf)
					{
						T component = default(T);
#if UNITY_EDITOR
						parent.origin.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							component = componentCache[0];
							componentCache.Clear();
						}
#else
                        component = parent.origin.GetComponent<T>();
#endif

						if (component != null)
						{
							if (array.Length == index)
							{
								int newSize = (index == 0) ? 4 : index * 2;
								Array.Resize(ref array, newSize);
							}

							array[index++] = component;
						}
					}

					Transform originTransform = parent.origin.transform;
					OfComponentDescendantsCore(ref originTransform, ref index, ref array);

					return index;
				}

				#endregion
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>
				where T : Component
			{
				private Enumerator enumerator; // enumerator is mutable
				private T current;

#if UNITY_EDITOR
				private static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

				public OfComponentEnumerator(ref DescendantsEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = default(T);
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
#if UNITY_EDITOR
						enumerator.Current.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							current = componentCache[0];
							componentCache.Clear();
							return true;
						}
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
					}

					return false;
				}

				public T Current => current;
				object IEnumerator.Current => current;

				public void Dispose() => enumerator.Dispose();

				public void Reset() { throw new NotSupportedException(); }
			}
		}

		public struct BeforeSelfEnumerable : IEnumerable<GameObject>
		{
			private readonly GameObject origin;
			private readonly bool withSelf;

			public BeforeSelfEnumerable(GameObject origin, bool withSelf)
			{
				this.origin = origin;
				this.withSelf = withSelf;
			}

			/// <summary>Returns a collection of specified component in the source collection.</summary>
			public OfComponentEnumerable<T> OfComponent<T>()
				where T : Component => new OfComponentEnumerable<T>(ref this);

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					e.Current.Destroy(useDestroyImmediate, false);
				}
			}

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (predicate(item))
					{
						item.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator() =>
				// check GameObject is destroyed only on GetEnumerator timing
				(origin == null)
					? new Enumerator(null, withSelf, false)
					: new Enumerator(origin.transform, withSelf, true);

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator() => GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			#region LINQ

			public void ForEach(Action<GameObject> action)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					action(e.Current);
				}
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int index = 0;

				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = item;
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (!filter(item))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = item;
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(item);
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (!filter(item))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(item);
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					TState state = let(item);

					if (!filter(state))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(state);
				}

				return index;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[4];
				int len = ToArrayNonAlloc(ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[4];
				int len = ToArrayNonAlloc(filter, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc<T>(selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator e = GetEnumerator();
				if (e.MoveNext())
				{
					return e.Current;
				}
				else
				{
					throw new InvalidOperationException("sequence is empty.");
				}
			}

			public GameObject FirstOrDefault()
			{
				Enumerator e = GetEnumerator();
				return (e.MoveNext())
					? e.Current
					: null;
			}

			#endregion

			public struct Enumerator : IEnumerator<GameObject>
			{
				private readonly int childCount; // childCount is fixed when GetEnumerator is called.
				private readonly Transform originTransform;
				private bool canRun;
				private bool withSelf;
				private int currentIndex;
				private GameObject current;
				private Transform parent;

				internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
				{
					this.originTransform = originTransform;
					this.withSelf = withSelf;
					currentIndex = -1;
					this.canRun = canRun;
					current = null;
					parent = originTransform.parent;
					childCount = (parent != null) ? parent.childCount : 0;
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}

					if (parent == null)
					{
						goto RETURN_SELF;
					}

					currentIndex++;
					if (currentIndex < childCount)
					{
						Transform item = parent.GetChild(currentIndex);

						if (item == originTransform)
						{
							goto RETURN_SELF;
						}

						current = item.gameObject;
						return true;
					}

					RETURN_SELF:
					if (withSelf)
					{
						current = originTransform.gameObject;
						withSelf = false;
						canRun = false; // reached self, run complete.
						return true;
					}

					return false;
				}

				public GameObject Current => current;
				object IEnumerator.Current => current;
				public void Dispose() { }
				public void Reset() { throw new NotSupportedException(); }
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>
				where T : Component
			{
				private BeforeSelfEnumerable parent;

				public OfComponentEnumerable(ref BeforeSelfEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator() => new OfComponentEnumerator<T>(ref parent);

				IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

				#region LINQ

				public void ForEach(Action<T> action)
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					while (e.MoveNext())
					{
						action(e.Current);
					}
				}

				public T First()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					if (e.MoveNext())
					{
						return e.Current;
					}
					else
					{
						throw new InvalidOperationException("sequence is empty.");
					}
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					return (e.MoveNext())
						? e.Current
						: null;
				}

				public T[] ToArray()
				{
					T[] array = new T[4];
					int len = ToArrayNonAlloc(ref array);
					if (array.Length != len)
					{
						Array.Resize(ref array, len);
					}
					return array;
				}

				/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
				public int ToArrayNonAlloc(ref T[] array)
				{
					int index = 0;
					OfComponentEnumerator<T> e = GetEnumerator();
					while (e.MoveNext())
					{
						if (array.Length == index)
						{
							int newSize = (index == 0) ? 4 : index * 2;
							Array.Resize(ref array, newSize);
						}
						array[index++] = e.Current;
					}

					return index;
				}

				#endregion
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>
				where T : Component
			{
				private Enumerator enumerator; // enumerator is mutable
				private T current;

#if UNITY_EDITOR
				private static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

				public OfComponentEnumerator(ref BeforeSelfEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = default(T);
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
#if UNITY_EDITOR
						enumerator.Current.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							current = componentCache[0];
							componentCache.Clear();
							return true;
						}
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
					}

					return false;
				}

				public T Current => current;
				object IEnumerator.Current => current;
				public void Dispose() { }
				public void Reset() { throw new NotSupportedException(); }
			}
		}

		public struct AfterSelfEnumerable : IEnumerable<GameObject>
		{
			private readonly GameObject origin;
			private readonly bool withSelf;

			public AfterSelfEnumerable(GameObject origin, bool withSelf)
			{
				this.origin = origin;
				this.withSelf = withSelf;
			}

			/// <summary>Returns a collection of specified component in the source collection.</summary>
			public OfComponentEnumerable<T> OfComponent<T>()
				where T : Component => new OfComponentEnumerable<T>(ref this);

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					e.Current.Destroy(useDestroyImmediate, false);
				}
			}

			/// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
			/// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
			public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (predicate(item))
					{
						item.Destroy(useDestroyImmediate, false);
					}
				}
			}

			public Enumerator GetEnumerator() =>
				// check GameObject is destroyed only on GetEnumerator timing
				(origin == null)
					? new Enumerator(null, withSelf, false)
					: new Enumerator(origin.transform, withSelf, true);

			IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator() => GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			#region LINQ

			public void ForEach(Action<GameObject> action)
			{
				Enumerator e = GetEnumerator();
				while (e.MoveNext())
				{
					action(e.Current);
				}
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(ref GameObject[] array)
			{
				int index = 0;

				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = item;
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (!filter(item))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = item;
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(item);
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					if (!filter(item))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(item);
				}

				return index;
			}

			/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
			public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
			{
				int index = 0;
				Enumerator e = GetEnumerator(); // does not need to call Dispose.
				while (e.MoveNext())
				{
					GameObject item = e.Current;
					TState state = let(item);

					if (!filter(state))
					{
						continue;
					}

					if (array.Length == index)
					{
						int newSize = (index == 0) ? 4 : index * 2;
						Array.Resize(ref array, newSize);
					}
					array[index++] = selector(state);
				}

				return index;
			}

			public GameObject[] ToArray()
			{
				GameObject[] array = new GameObject[4];
				int len = ToArrayNonAlloc(ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject[] ToArray(Func<GameObject, bool> filter)
			{
				GameObject[] array = new GameObject[4];
				int len = ToArrayNonAlloc(filter, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc<T>(selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc(filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
			{
				T[] array = new T[4];
				int len = ToArrayNonAlloc(let, filter, selector, ref array);
				if (array.Length != len)
				{
					Array.Resize(ref array, len);
				}
				return array;
			}

			public GameObject First()
			{
				Enumerator e = GetEnumerator();
				if (e.MoveNext())
				{
					return e.Current;
				}
				else
				{
					throw new InvalidOperationException("sequence is empty.");
				}
			}

			public GameObject FirstOrDefault()
			{
				Enumerator e = GetEnumerator();
				return (e.MoveNext())
					? e.Current
					: null;
			}

			#endregion

			public struct Enumerator : IEnumerator<GameObject>
			{
				private readonly int childCount; // childCount is fixed when GetEnumerator is called.
				private readonly Transform originTransform;
				private readonly bool canRun;
				private bool withSelf;
				private int currentIndex;
				private GameObject current;
				private Transform parent;

				internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
				{
					this.originTransform = originTransform;
					this.withSelf = withSelf;
					currentIndex = (originTransform != null) ? originTransform.GetSiblingIndex() + 1 : 0;
					this.canRun = canRun;
					current = null;
					parent = originTransform.parent;
					childCount = (parent != null) ? parent.childCount : 0;
				}

				public bool MoveNext()
				{
					if (!canRun)
					{
						return false;
					}

					if (withSelf)
					{
						current = originTransform.gameObject;
						withSelf = false;
						return true;
					}

					if (currentIndex < childCount)
					{
						current = parent.GetChild(currentIndex).gameObject;
						currentIndex++;
						return true;
					}

					return false;
				}

				public GameObject Current => current;
				object IEnumerator.Current => current;
				public void Dispose() { }
				public void Reset() { throw new NotSupportedException(); }
			}

			public struct OfComponentEnumerable<T> : IEnumerable<T>
				where T : Component
			{
				private AfterSelfEnumerable parent;

				public OfComponentEnumerable(ref AfterSelfEnumerable parent)
				{
					this.parent = parent;
				}

				public OfComponentEnumerator<T> GetEnumerator() => new OfComponentEnumerator<T>(ref parent);

				IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

				#region LINQ

				public void ForEach(Action<T> action)
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					while (e.MoveNext())
					{
						action(e.Current);
					}
				}

				public T First()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					if (e.MoveNext())
					{
						return e.Current;
					}
					else
					{
						throw new InvalidOperationException("sequence is empty.");
					}
				}

				public T FirstOrDefault()
				{
					OfComponentEnumerator<T> e = GetEnumerator();
					return (e.MoveNext())
						? e.Current
						: null;
				}

				public T[] ToArray()
				{
					T[] array = new T[4];
					int len = ToArrayNonAlloc(ref array);
					if (array.Length != len)
					{
						Array.Resize(ref array, len);
					}
					return array;
				}

				/// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
				public int ToArrayNonAlloc(ref T[] array)
				{
					int index = 0;
					OfComponentEnumerator<T> e = GetEnumerator();
					while (e.MoveNext())
					{
						if (array.Length == index)
						{
							int newSize = (index == 0) ? 4 : index * 2;
							Array.Resize(ref array, newSize);
						}
						array[index++] = e.Current;
					}

					return index;
				}

				#endregion
			}

			public struct OfComponentEnumerator<T> : IEnumerator<T>
				where T : Component
			{
				private Enumerator enumerator; // enumerator is mutable
				private T current;

#if UNITY_EDITOR
				private static List<T> componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

				public OfComponentEnumerator(ref AfterSelfEnumerable parent)
				{
					enumerator = parent.GetEnumerator();
					current = default(T);
				}

				public bool MoveNext()
				{
					while (enumerator.MoveNext())
					{
#if UNITY_EDITOR
						enumerator.Current.GetComponents<T>(componentCache);
						if (componentCache.Count != 0)
						{
							current = componentCache[0];
							componentCache.Clear();
							return true;
						}
#else
                        
                        var component = enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            current = component;
                            return true;
                        }
#endif
					}

					return false;
				}

				public T Current => current;
				object IEnumerator.Current => current;
				public void Dispose() { }
				public void Reset() { throw new NotSupportedException(); }
			}
		}
	}
}
