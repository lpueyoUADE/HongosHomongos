using System;
using System.Collections.Generic;

public class PriorityQueue<TData>
{
    public bool IsEmpty { get { return data.Count < 1; } }

    private List<Tuple<TData, float>> data;
    private Dictionary<TData, int> indexes;
    private Func<float, float, bool> critery;

    public PriorityQueue()
    {
        data = new List<Tuple<TData, float>>();
        indexes = new Dictionary<TData, int>();
        critery = (x, y) => x.CompareTo(y) < 0;
    }

    public PriorityQueue(Func<float, float, bool> critery)
    {
        data = new List<Tuple<TData, float>>();
        indexes = new Dictionary<TData, int>();
        this.critery = critery;
    }

    public void Enqueue(TData data, float priority)
    {
        Enqueue(new Tuple<TData, float>(data, priority));
    }

    public void Enqueue(Tuple<TData, float> dp)
    {
        int currentIndex;
        int parentIndex;

        if (indexes.ContainsKey(dp.Item1))
        {
            currentIndex = indexes[dp.Item1];
            parentIndex = (currentIndex - 1) / 2;

            if (critery(data[currentIndex].Item2, dp.Item2)) return;

            data[currentIndex] = dp;
        }
        else
        {
            data.Add(dp);

            currentIndex = data.Count - 1;
            parentIndex = (currentIndex - 1) / 2;

            indexes.Add(dp.Item1, currentIndex);
        }

        while (currentIndex > 0 && critery(data[currentIndex].Item2, data[parentIndex].Item2))
        {
            Swap(currentIndex, parentIndex);

            currentIndex = parentIndex;
            parentIndex = (currentIndex - 1) / 2;
        }
    }

    private void EnqueueData(Tuple<TData, float> dp)
    {
        data.Add(dp);

        int currentIndex = data.Count - 1;
        int parentIndex = (currentIndex - 1) / 2;

        indexes.Add(dp.Item1, currentIndex);

        while (currentIndex > 0 && critery(data[currentIndex].Item2, data[parentIndex].Item2))
        {
            Swap(currentIndex, parentIndex);

            currentIndex = parentIndex;
            parentIndex = (currentIndex - 1) / 2;
        }
    }

    public TData Peek()
    {
        return PeekTuple().Item1;
    }

    public Tuple<TData, float> PeekTuple()
    {
        return data[0];
    }

    public TData Dequeue()
    {
        return DequeueTuple().Item1;
    }

    public Tuple<TData, float> DequeueTuple()
    {
        var date = data[0];

        data[0] = data[data.Count - 1];
        indexes[data[0].Item1] = 0;

        data.RemoveAt(data.Count - 1);
        indexes.Remove(date.Item1);

        int currentIndex = 0;
        int leftIndex = 1;
        int rightIndex = 2;
        int explorIndex = GetExplorerIndex(leftIndex, rightIndex);


        if (explorIndex == -1) return date;

        while (critery(data[explorIndex].Item2, data[currentIndex].Item2))
        {
            Swap(explorIndex, currentIndex);

            currentIndex = explorIndex;
            leftIndex = (currentIndex * 2) + 1;
            rightIndex = (currentIndex * 2) + 2;
            explorIndex = GetExplorerIndex(leftIndex, rightIndex);

            if (explorIndex == -1) break;
        }
        return date;
    }

    private int GetExplorerIndex(int leftIndex, int rightIndex)
    {
        if (data.Count > rightIndex)
            return critery(data[leftIndex].Item2, data[rightIndex].Item2) ? leftIndex : rightIndex;
        else if (data.Count > leftIndex)
            return leftIndex;

        return -1;
    }

    private void Swap(int from, int to)
    {
        indexes[data[from].Item1] = to;
        indexes[data[to].Item1] = from;
        var aux = data[from];
        data[from] = data[to];
        data[to] = aux;
    }
}
