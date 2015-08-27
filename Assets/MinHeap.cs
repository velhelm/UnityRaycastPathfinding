using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

class MinHeap<T> where T: IComparable
{
    IList<T> elements;
    public int Count { get { return elements.Count; } }

    public MinHeap()
    {
        elements = new List<T>();
    }

    public void Add(T item)
    {
        elements.Add(item);
        Heapify();
    }

    public void Delete(T item)
    {
        int i = elements.IndexOf(item);
        int last = elements.Count - 1;

        elements[i] = elements[last];
        elements.RemoveAt(last);
        Heapify();
    }

    public T PopMin()
    {
        if (elements.Count > 0)
        {
            T item = elements[0];
            Delete(item);
            return (item);
        }

        throw new InvalidOperationException("MinHeap contains 0 elements");
    }
    void Heapify()
    {
        for (int i= elements.Count - 1; i>0; i--)
        {
            int parentPosition = (i + 1) / 2 - 1;
            parentPosition = parentPosition >= 0 ? parentPosition : 0;

            if (elements[parentPosition].CompareTo(elements[i])>1)
            {
                SwapElements(parentPosition, i);
            }
        }
    }

    void SwapElements(int firstIndex, int secondIndex)
    {
        T tmp = elements[firstIndex];
        elements[firstIndex] = elements[secondIndex];
        elements[secondIndex] = tmp;
    }
}
