using System;
using System.Collections.Generic;

[Serializable]
public class FastList<T>
{
	public T[] buffer;

	public int size;

	public T this[int i]
	{
		get
		{
			return buffer[i];
		}
		set
		{
			buffer[i] = value;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (buffer != null)
		{
			for (int i = 0; i < size; i++)
			{
				yield return buffer[i];
			}
		}
	}

	private void AllocateMore()
	{
		int num = ((buffer != null) ? (buffer.Length << 1) : 0);
		if (num < 32)
		{
			num = 32;
		}
		T[] array = new T[num];
		if (buffer != null && size > 0)
		{
			buffer.CopyTo(array, 0);
		}
		buffer = array;
	}

	private void Trim()
	{
		if (size > 0)
		{
			if (size < buffer.Length)
			{
				T[] array = new T[size];
				for (int i = 0; i < size; i++)
				{
					array[i] = buffer[i];
				}
				buffer = array;
			}
		}
		else
		{
			buffer = new T[0];
		}
	}

	public void Clear()
	{
		size = 0;
	}

	public void Release()
	{
		size = 0;
		buffer = null;
	}

	public void Add(T item)
	{
		if (buffer == null || size == buffer.Length)
		{
			AllocateMore();
		}
		buffer[size++] = item;
	}

	public void Insert(int index, T item)
	{
		if (buffer == null || size == buffer.Length)
		{
			AllocateMore();
		}
		if (index < size)
		{
			for (int num = size; num > index; num--)
			{
				buffer[num] = buffer[num - 1];
			}
			buffer[index] = item;
			size++;
		}
		else
		{
			Add(item);
		}
	}

	public bool Contains(T item)
	{
		if (buffer == null)
		{
			return false;
		}
		for (int i = 0; i < size; i++)
		{
			if (buffer[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	public bool Remove(T item)
	{
		if (buffer != null)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < size; i++)
			{
				if (@default.Equals(buffer[i], item))
				{
					size--;
					buffer[i] = default(T);
					for (int j = i; j < size; j++)
					{
						buffer[j] = buffer[j + 1];
					}
					return true;
				}
			}
		}
		return false;
	}

	public void RemoveAt(int index)
	{
		if (buffer != null && index < size)
		{
			size--;
			buffer[index] = default(T);
			for (int i = index; i < size; i++)
			{
				buffer[i] = buffer[i + 1];
			}
		}
	}

	public T Pop()
	{
		if (buffer != null && size != 0)
		{
			T result = buffer[--size];
			buffer[size] = default(T);
			return result;
		}
		return default(T);
	}

	public T[] ToArray()
	{
		Trim();
		return buffer;
	}

	public void Sort(Comparison<T> comparer)
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 1; i < size; i++)
			{
				if (comparer(buffer[i - 1], buffer[i]) > 0)
				{
					T val = buffer[i];
					buffer[i] = buffer[i - 1];
					buffer[i - 1] = val;
					flag = true;
				}
			}
		}
	}
}
