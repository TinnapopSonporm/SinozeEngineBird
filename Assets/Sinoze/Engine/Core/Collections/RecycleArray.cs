using System;
using System.Collections.Generic;

namespace Sinoze.Engine.Collections
{
	public class RecycleArray<T>
	{
		// for now, provide public access
		// TODO: in the future we have to improve this by providing enumeration api
		public T[] items;
		public List<int> occupiedSlotIndices = new List<int>();
		private Queue<int> availableSlotIndices = new Queue<int>();

		/// <summary>
		/// Gets the maximum amount of item slots.
		/// </summary>
		/// <value>The capacity.</value>
		public int Capacity
		{
			get { return items == null ? 0 : items.Length; }
		}

		/// <summary>
		/// Gets the amount of occupied items.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get { return occupiedSlotIndices.Count; }
		}

		/// <summary>
		/// Alloc the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public int Alloc(ref T item)
		{
			if(availableSlotIndices.Count == 0)
				Expand(100);

			var slotIndex = availableSlotIndices.Dequeue();
			occupiedSlotIndices.Add(slotIndex);
			items[slotIndex] = item;
			return slotIndex;
		}

		/// <summary>
		/// Alloc new item
		/// </summary>
		public int Alloc()
		{
			var val = default(T);
			return Alloc(ref val);
		}

		/// <summary>
		/// Free the specified slotIndex.
		/// </summary>
		/// <param name="slotIndex">Slot index.</param>
		public void Free(int slotIndex)
		{
			items[slotIndex] = default(T);
			availableSlotIndices.Enqueue(slotIndex);
			occupiedSlotIndices.Remove(slotIndex);
		}

		/// <summary>
		/// Expand the specified amount.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void Expand(int amount)
		{
			int oldLength = items != null ? items.Length : 0;
			int newLength = oldLength + amount;
			
			if(items == null)
				items = new T[newLength];
			else
				Array.Resize(ref items, newLength);
			
			for(int i=oldLength; i<newLength; i++)
			{
				availableSlotIndices.Enqueue(i);
			}
		}
	}
}
