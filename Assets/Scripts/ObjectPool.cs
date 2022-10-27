﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace SergeiStarovoitov
{
	public class ObjectPool<T>
	{
		private List<ObjectPoolContainer<T>> list;
		private Dictionary<T, ObjectPoolContainer<T>> lookup;
		private Func<T> factoryFunc;
		private int lastIndex = 0;

		public ObjectPool(Func<T> factoryFunc, int initialSize)
		{
			this.factoryFunc = factoryFunc;

			list = new List<ObjectPoolContainer<T>>(initialSize);
			lookup = new Dictionary<T, ObjectPoolContainer<T>>(initialSize);

			Warm(initialSize);
		}

		private void Warm(int capacity)
		{
			for (int i = 0; i < capacity; i++)
			{
				CreateContainer();
			}
		}

		private ObjectPoolContainer<T> CreateContainer()
		{
			var container = new ObjectPoolContainer<T>();
			container.Item = factoryFunc();
			list.Add(container);
			return container;
		}

		public T GetItem()
		{
			ObjectPoolContainer<T> container = null;
			for (int i = 0; i < list.Count; i++)
			{
				lastIndex++;
				if (lastIndex > list.Count - 1) lastIndex = 0;

				if (list[lastIndex].InUse)
				{
					continue;
				}
				else
				{
					container = list[lastIndex];
					break;
				}
			}

			if (container == null)
			{
				container = CreateContainer();
			}

			container.InUse = true;
			lookup.Add(container.Item, container);
			return container.Item;
		}

		public void ReleaseItem(object item)
		{
			ReleaseItem((T)item);
		}

		public void ReleaseItem(T item)
		{
			if (lookup.ContainsKey(item))
			{
				var container = lookup[item];
				container.InUse = false;
				lookup.Remove(item);
			}
			else
			{
				Debug.LogWarning("This object pool does not contain the item provided: " + item);
			}
		}

        public int Count => list.Count;

        public int CountUsedItems => lookup.Count;
    }

	public class ObjectPoolContainer<T>
	{
		public T Item { get; set; }
		public bool InUse { get; set; }
	}
}