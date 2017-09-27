using System.Collections.Generic;
using UnityEngine;


namespace Neuroevolution
{
	public static class Utils
	{
		public static float rangeAroundZero(float range)
		{
			return range * (2 * Random.value - 1);
		}


		public static bool chance(float chance)
		{
			return (Random.value <= chance);
		}


		public static float octavePerlinNoise(float x, int octaves, float persistence)
		{
			float total = 0;
			float frequency = 1;
			float amplitude = 1;
			float maxValue = 1;

			for(int i = 0; i < octaves; i++)
			{
				total += Mathf.PerlinNoise(x * frequency, 0f) * amplitude;

				maxValue += amplitude;

				amplitude *= persistence;
				frequency *= 2;
			}

			return total / maxValue;
		}
	}

	public class Pool<T>
	{
		public delegate T PoolCreateItem();


		public List<T> allItems = new List<T>();
		public List<T> freeItems = new List<T>();

		PoolCreateItem createItem;


		public int Size {
			get
			{
				return allItems.Count;
			}
		}


		public int FreeSize {
			get
			{
				return freeItems.Count;
			}
		}


		public Pool(PoolCreateItem itemCreator)
		{
			createItem = itemCreator;
		}


		public T this[int index] {
			get
			{
				return allItems[index];
			}
			set
			{
				allItems[index] = value;
			}
		}


		public void AddItem(T item, bool free = true)
		{
			allItems.Add(item);

			if(free)
			{
				freeItems.Add(item);
			}
		}


		public T GrabItem()
		{
			if(freeItems.Count == 0)
			{
				//TODO if there's no createItem function, return false
				T newItem = createItem();
				AddItem(newItem, false);
				
				return newItem;
			}

			T item = freeItems[0];
			freeItems.RemoveAt(0);

			return item;
		}


		public void ReleaseItem(T item)
		{
			if(allItems.Contains(item))
			{
				freeItems.Add(item);
			}
		}


		public void ReleaseAll()
		{
			freeItems = new List<T>(allItems);
		}
	}
}